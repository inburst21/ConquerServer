// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Monster.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Core.Mathematics;
using Comet.Game.Database.Models;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.World;
using Comet.Game.World.Maps;
using Comet.Network.Packets;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.States
{
    public class Monster : Role
    {
        private readonly Generator m_generator;
        private readonly DbMonstertype m_dbMonster;

        private TimeOutMS m_tStatusCheck = new TimeOutMS(500);
        private TimeOutMS m_tAttackMs = new TimeOutMS();
        private TimeOutMS m_tAction = new TimeOutMS();
        private TimeOutMS m_tMoveMs = new TimeOutMS();
        private TimeOut m_tHealPeriod = new TimeOut(2);
        private TimeOut m_disappear = new TimeOut(5);

        private AiStage m_stage;
        private FacingDirection m_nextDir = FacingDirection.Invalid;

        private Role m_actTarget;
        private bool m_bAheadPath;

        private uint m_idRole = 0;

        public Monster(DbMonstertype type, uint identity, Generator generator)
        {
            m_dbMonster = type;
            m_generator = generator;
            m_idRole = identity;

            m_idMap = generator.MapIdentity;

            m_tStatusCheck.Update();

            m_tMoveMs.Startup(m_dbMonster.MoveSpeed);
            m_tAttackMs.Startup(m_dbMonster.AttackSpeed);
            m_tAction.Startup(500);
        }
        
        #region Identity

        public override uint Identity
        {
            get => m_idRole;
            protected set { }
        }

        public override string Name
        {
            get => m_dbMonster.Name;
            set => m_dbMonster.Name = value;
        }

        #endregion

        #region Initialization

        public bool Initialize(uint idMap, ushort x, ushort y)
        {
            m_idMap = idMap;

            if ((Map = Kernel.MapManager.GetMap(idMap)) == null)
                return false;

            m_posX = x;
            m_posY = y;

            Life = MaxLife;

            return true;
        }

        #endregion

        #region Appearence

        public uint Type => m_dbMonster.Id;

        public override uint Mesh
        {
            get => m_dbMonster.Lookface;
            set => m_dbMonster.Lookface = (ushort) value;
        }

        #endregion

        public override byte Level
        {
            get => (byte) (m_dbMonster?.Level ?? 0);
            set => m_dbMonster.Level = value;
        }

        public override uint Life
        {
            get;
            set;
        }

        public override uint MaxLife => (uint) (m_dbMonster?.Life ?? 1);

        #region Battle Attributes

        public override int BattlePower => 1;

        public override int MinAttack => m_dbMonster?.AttackMin ?? 0;

        public override int MaxAttack => m_dbMonster?.AttackMax ?? 0;

        public override int MagicAttack => m_dbMonster?.AttackMax ?? 0;

        public override int Defense => m_dbMonster?.Defence ?? 0;

        public override int MagicDefense => m_dbMonster?.MagicDef ?? 0;

        public override int Dodge => (int) (m_dbMonster?.Dodge ?? 0);

        public override int AttackSpeed => m_dbMonster?.AttackSpeed ?? 1000;

        public override int Accuracy => (int) (m_dbMonster?.Dexterity ?? 0);

        public uint AttackUser => m_dbMonster?.AttackUser ?? 0;

        public int ViewRange => m_dbMonster?.ViewRange ?? 1;

        #endregion

        #region Battle

        public async Task Attack()
        {

        }

        public override bool IsAttackable(Role attacker)
        {
            if (!IsAlive)
                return false;

            return true;
        }

        public override async Task<bool> BeAttack(BattleSystem.MagicType magic, Role attacker, int nPower, bool bReflectEnable)
        {
            if (!IsAlive)
                return false;

            await AddAttributesAsync(ClientUpdateType.Hitpoints, nPower * -1);

            if (!IsAlive)
            {
                await BeKill(attacker);
                return true;
            }

            if (m_disappear.IsActive())
                return false;

            return true;
        }

        public override async Task BeKill(Role attacker)
        {
            if (m_disappear.IsActive())
                return;

            if (attacker?.BattleSystem.IsActive() == true)
                attacker.BattleSystem.ResetBattle();

            await DetachAllStatus();
            await AttachStatus(attacker, StatusSet.DEAD, 0, 20, 0, 0);
            await AttachStatus(attacker, StatusSet.GHOST, 0, 20, 0, 0);
            await AttachStatus(attacker, StatusSet.FADE, 0, 20, 0, 0);

            Character user = attacker as Character;
            int dieType = user?.KoCount * 65541 ?? 1;
            
            await BroadcastRoomMsgAsync(new MsgInteract
            {
                SenderIdentity = attacker?.Identity ?? 0,
                TargetIdentity = Identity,
                PosX = MapX,
                PosY = MapY,
                Action = MsgInteractType.Kill,
                Data = dieType
            }, false);

            m_disappear.Startup(5);

            if (m_dbMonster.Action > 0)
                await GameAction.ExecuteActionAsync(m_dbMonster.Action, user, this, null, "");

            uint idDropOwner = user?.Identity ?? 0;

            if (IsGuard())
                return;

            Item jar = user?.UserPackage.GetItemByType(Item.TYPE_JAR);
            if (jar != null)
            {
                if (jar.MaximumDurability == m_dbMonster.StcType)
                {
                    jar.Data += 1;
                    await jar.SaveAsync();

                    if (jar.Data % 50 == 0)
                    {
                        await jar.SendJarAsync();
                    }
                }
            }

            int chanceAdjust = 35;
            if (user != null && BattleSystem.GetNameType(user.Level, Level) == BattleSystem.NAME_GREEN)
                chanceAdjust = 9;

            if (await Kernel.ChanceCalcAsync(chanceAdjust))
            {
                int moneyMin = (int)(m_dbMonster.DropMoney * 0.85f);
                int moneyMax = (int)(m_dbMonster.DropMoney * 1.15f);
                uint money = (uint)(moneyMin + await Kernel.NextAsync(moneyMin, moneyMax) + 1);

                int heapNum = 1 + await Kernel.NextAsync(1, 3);
                uint moneyAve = (uint)(money / heapNum);

                for (int i = 0; i < heapNum; i++)
                {
                    uint moneyTmp = (uint)Calculations.MulDiv((int)moneyAve, 90 + await Kernel.NextAsync(3, 21), 100);
                    await DropMoney(moneyTmp, idDropOwner);
                }
            }

            if (await Kernel.ChanceCalcAsync(1, 5000))
            {
                await DropItem(Item.TYPE_DRAGONBALL, idDropOwner);
                await Kernel.RoleManager.BroadcastMsgAsync(string.Format(Language.StrDragonBallDropped, attacker.Name, attacker.Map.Name));
            }

            if (await Kernel.ChanceCalcAsync(1, 400))
            {
                await DropItem(Item.TYPE_METEOR, idDropOwner);
            }

            if (!IsPkKiller() && !IsGuard() && !IsEvilKiller() && !IsDynaNpc())
            {
                if (await Kernel.ChanceCalcAsync(.1d))
                {
                    uint[] normalGem = { 700001, 700011, 700021, 700031, 700041, 700051, 700061, 700071, 700101, 7000121 };
                    uint dGem = normalGem[await Kernel.NextAsync(0, normalGem.Length) % normalGem.Length];
                    await DropItem(dGem, idDropOwner); // normal gems
                }
            }

            if ((m_dbMonster.Id == 15 || m_dbMonster.Id == 74) && await Kernel.ChanceCalcAsync(2f))
            {
                await DropItem(1080001, idDropOwner); // emerald
            }

            int dropNum = 0;
            int rate = await Kernel.NextAsync(0, 10000);
            int chance = BattleSystem.AdjustDrop(4000, attacker.Level, Level);
            if (rate < Math.Min(10000, chance))
            {
                dropNum = 1 + await Kernel.NextAsync(5, 9); // drop 6-10 items
            }
            else
            {
                chance += BattleSystem.AdjustDrop(5000, attacker.Level, Level);
                if (rate < Math.Min(10000, chance))
                {
                    dropNum = 1 + await Kernel.NextAsync(3, 6); // drop 4-7 items
                }
                else
                {
                    chance += BattleSystem.AdjustDrop(7500, attacker.Level, Level);
                    if (rate < Math.Min(10000, chance))
                    {
                        dropNum = 1 + await Kernel.NextAsync(1, 4); // drop 4-7 items
                    }
                    else
                    {
                        chance += BattleSystem.AdjustDrop(10000, attacker.Level, Level);
                        if (rate < Math.Min(10000, chance))
                        {
                            dropNum = 1; // drop 1 item
                        }
                    }
                }
            }

            for (int i = 0; i < dropNum; i++)
            {
                uint idItemtype = await GetDropItem();

                DbItemtype itemtype = Kernel.ItemManager.GetItemtype(idItemtype);
                if (itemtype == null)
                    continue;

                await DropItem(itemtype, idDropOwner);
            }
        }

        #endregion

        #region Drop Function

        public async Task DropItem(uint type, uint owner)
        {
            DbItemtype itemType = Kernel.ItemManager.GetItemtype(type);
            if (itemType != null) await DropItem(itemType, owner);
        }

        private async Task DropItem(DbItemtype itemtype, uint idOwner)
        {
            Point targetPos = new Point(MapX, MapY);
            if (Map.FindDropItemCell(4, ref targetPos))
            {
                MapItem drop = new MapItem((uint)IdentityGenerator.MapItem.GetNextIdentity);
                if (drop.Create(Map, targetPos, itemtype, idOwner, 0, 0, 0))
                {
                    await drop.GenerateRandomInfoAsync();
                    await drop.EnterMap();
                }
                else
                {
                    IdentityGenerator.MapItem.ReturnIdentity(drop.Identity);
                }
            }
        }

        public async Task DropMoney(uint amount, uint idOwner)
        {
            Point targetPos = new Point(MapX, MapY);
            if (Map.FindDropItemCell(4, ref targetPos))
            {
                MapItem drop = new MapItem((uint) IdentityGenerator.MapItem.GetNextIdentity);
                if (drop.CreateMoney(Map, targetPos, amount, idOwner))
                    await drop.EnterMap();
                else
                {
                    IdentityGenerator.MapItem.ReturnIdentity(drop.Identity);
                }
            }
        }

        public async Task<uint> GetDropItem()
        {
            /*
             * 0 = armet
             * 1 = necklace
             * 2 = armor
             * 3 = ring
             * 4 = weapon
             * 5 = shield
             * 6 = shoes
             * 7 = hp
             * 8 = mp
             */
            var possibleDrops = Kernel.ItemManager.GetByRange(Level, 15, 15)
                .Where(x => Item.IsHelmet(x.Type) || Item.IsNeck(x.Type) || Item.IsRing(x.Type) || Item.IsBangle(x.Type) || Item.IsWeapon(x.Type) || Item.IsArmor(x.Type) || Item.IsShoes(x.Type))
                .ToList();
            if (m_dbMonster.DropArmet == 99)
                possibleDrops.RemoveAll(x => Item.IsHelmet(x.Type));
            if (m_dbMonster.DropNecklace == 99)
                possibleDrops.RemoveAll(x => Item.IsNeck(x.Type));
            if (m_dbMonster.DropArmor == 99)
                possibleDrops.RemoveAll(x => Item.IsArmor(x.Type));
            if (m_dbMonster.DropRing == 99)
                possibleDrops.RemoveAll(x => Item.IsRing(x.Type) || Item.IsBangle(x.Type));
            if (m_dbMonster.DropWeapon == 99)
                possibleDrops.RemoveAll(x => Item.IsWeapon(x.Type));
            if (m_dbMonster.DropShield == 99)
                possibleDrops.RemoveAll(x => Item.IsShield(x.Type));
            if (m_dbMonster.DropShoes == 99)
                possibleDrops.RemoveAll(x => Item.IsShoes(x.Type));

            DbItemtype pot = Kernel.ItemManager.GetItemtype(m_dbMonster.DropHp);
            if (pot != null)
                possibleDrops.Add(pot);
            pot = Kernel.ItemManager.GetItemtype(m_dbMonster.DropMp);
            if (pot != null)
                possibleDrops.Add(pot);

            if (possibleDrops.Count <= 0)
                return 0;

            if (await Kernel.ChanceCalcAsync(625, 10000000))
            {
                possibleDrops.RemoveAll(x => Item.GetQuality(x.Type) != 9);
            }
            else if (await Kernel.ChanceCalcAsync(625, 1000000))
            {
                possibleDrops.RemoveAll(x => Item.GetQuality(x.Type) != 8);
            }
            else if (await Kernel.ChanceCalcAsync(100, 20000))
            {
                possibleDrops.RemoveAll(x => Item.GetQuality(x.Type) != 7);
            }
            else if (await Kernel.ChanceCalcAsync(312, 15000))
            {
                possibleDrops.RemoveAll(x => Item.GetQuality(x.Type) != 6);
            }
            else if (await Kernel.ChanceCalcAsync(2, 5))
            {
                possibleDrops.RemoveAll(x => !Item.IsMedicine(x.Type));
            }
            else
            {
                possibleDrops.RemoveAll(x => Item.GetQuality(x.Type) > 5 || Item.GetQuality(x.Type) < 3);
            }

            if (possibleDrops.Count == 0)
                return 0;

            return possibleDrops[(await Kernel.NextAsync(possibleDrops.Count))%possibleDrops.Count]?.Type ?? 0;
        }

        #endregion

        #region Checks

        public bool CanDisappear()
        {
            return m_disappear.IsTimeOut();
        }

        public bool IsLockUser()
        {
            return (AttackUser & 256) != 0;
        }

        public bool IsRighteous()
        {
            return (AttackUser & 4) != 0;
        }

        public bool IsGuard()
        {
            return (AttackUser & 8) != 0;
        }

        public bool IsPkKiller()
        {
            return (AttackUser & 16) != 0;
        }

        public bool IsWalkEnable()
        {
            return (AttackUser & 64) == 0;
        }

        public bool IsJumpEnable()
        {
            return (AttackUser & 32) != 0;
        }

        public bool IsFastBack()
        {
            return (AttackUser & 128) != 0;
        }

        public bool IsLockOne()
        {
            return (AttackUser & 512) != 0;
        }

        public bool IsAddLife()
        {
            return (AttackUser & 1024) != 0;
        }

        public bool IsEvilKiller()
        {
            return (AttackUser & 2048) != 0;
        }

        public bool IsDormancyEnable()
        {
            return (AttackUser & 256) == 0;
        }

        #endregion

        #region Map and Movement

        /// <summary>
        ///     The current map identity for the role.
        /// </summary>
        public override uint MapIdentity
        {
            get => m_idMap;
            set => m_idMap = value;
        }

        /// <summary>
        ///     Current X position of the user in the map.
        /// </summary>
        public override ushort MapX
        {
            get => m_posX;
            set => m_posX = value;
        }

        /// <summary>
        ///     Current Y position of the user in the map.
        /// </summary>
        public override ushort MapY
        {
            get => m_posY;
            set => m_posY = value;
        }

        public override async Task EnterMap()
        {
            Map = Kernel.MapManager.GetMap(MapIdentity);
            if (Map != null)
                await Map.AddAsync(this);

            await BroadcastRoomMsgAsync(new MsgAction
            {
                Action = MsgAction.ActionType.MapEffect,
                Identity = Identity,
                CommandX = MapX,
                CommandY= MapY
            }, false);
        }

        public override async Task LeaveMap()
        {
            IdentityGenerator.Monster.ReturnIdentity(Identity);
            m_generator.Remove(Identity);
            if (Map != null)
            {
                await Map.RemoveAsync(Identity);
                Kernel.RoleManager.RemoveRole(Identity);
            }
            Map = null;
        }

        #endregion

        #region AI

        public bool IsCloseAttack()
        {
            return !IsBowman;
        }

        public bool IsMoveEnable()
        {
            return IsWalkEnable() || IsJumpEnable();
        }

        public override bool IsEvil() { return (AttackUser & (4 | 8192)) == 0; }

        public bool CheckTarget()
        {
            if (m_actTarget == null || !m_actTarget.IsAlive)
            {
                return false;
            }

            if (m_actTarget.IsWing && !IsWing && !IsCloseAttack())
                return false;

            int nDistance = ViewRange;
            int nAtkDistance = Calculations.CutOverflow(nDistance, GetAttackRange(m_actTarget.SizeAddition));
            int nDist = GetDistance(m_actTarget.MapX, m_actTarget.MapY);

            if (!(nDist <= nDistance) || (nDist <= nAtkDistance && GetAttackRange(m_actTarget.SizeAddition) > 1))
            {
                m_actTarget = null;
                return false;
            }

            return true;
        }

        public async Task<bool> FindNewTarget()
        {
            Role target = null;

            // lock target
            if (IsLockUser() || IsLockOne())
            {
                if (CheckTarget())
                {
                    if (IsLockOne())
                        return true;
                }
                else
                {
                    if (IsLockUser())
                        return false;
                }
            }

            uint idOldTarget = m_actTarget?.Identity ?? 0;
            int distance = ViewRange;
            var roles = Map.Query9BlocksByPos(MapX, MapY);
            foreach (var role in roles)
            {
                if (role.Identity == Identity)
                    continue;

                if (!role.IsAlive)
                    continue;

                if (role.QueryStatus(StatusSet.INVISIBLE) != null)
                    continue;

                if (role is Character targetUser)
                {
                    bool pkKill = IsPkKiller() && targetUser.IsPker();
                    bool evilKill = IsEvilKiller() && !targetUser.IsVirtuous();
                    if ((IsGuard() && targetUser.IsCrime())
                        || (pkKill)
                        || (evilKill)
                        || (IsEvil() && !(IsPkKiller() || IsEvilKiller())))
                    {
                        int nDist = GetDistance(targetUser.MapX, targetUser.MapY);
                        if (nDist <= distance)
                        {
                            distance = nDist;
                            m_actTarget = targetUser;

                            if (pkKill || evilKill)
                                break;
                        }
                    }
                }
                else if (role is Monster monster)
                {
                    if ((IsEvil() && monster.IsRighteous()
                                         || IsRighteous() && monster.IsEvil()))
                    {
                        if (monster.IsWing && !IsWing) continue;

                        int nDist = GetDistance(monster.MapX, monster.MapY);
                        if (nDist < distance)
                        {
                            distance = nDist;
                            m_actTarget = monster;
                        }
                    }
                }
            }

            if (m_actTarget != null)
            {
                if (m_actTarget is Character targetUser && targetUser.Identity != idOldTarget)
                {
                    if (IsGuard() && targetUser.IsCrime())
                    {
                        await  targetUser.BroadcastRoomMsgAsync(
                            new MsgTalk(Identity, MsgTalk.TalkChannel.Talk, Color.White, target.Name, Name,
                                Language.StrGuardYouPay), true);
                    }
                    else if (IsPkKiller() && targetUser.IsPker() && m_stage == AiStage.Idle)
                    {
                        await targetUser.BroadcastRoomMsgAsync(
                            new MsgTalk(Identity, MsgTalk.TalkChannel.Talk, Color.White, target.Name, Name,
                                Language.StrGuardYouPay), true);
                    }
                }
            }
            return true;
        }

        #endregion

        #region On Timer
        
        public override async Task OnTimerAsync()
        {
            if (!IsAlive)
                return;

            if (m_tStatusCheck.ToNextTime())
            {
                if (StatusSet.Status.Values.Count > 0)
                {
                    foreach (var stts in StatusSet.Status.Values)
                    {
                        await stts.OnTimer();
                        if (!stts.IsValid && stts.Identity != StatusSet.GHOST && stts.Identity != StatusSet.DEAD)
                        {
                            await StatusSet.DelObj(stts.Identity);
                        }
                    }
                }
            }
        }

        #endregion

        #region Socket

        public override async Task BroadcastRoomMsgAsync(IPacket msg, bool self)
        {
            if (Map != null)
                await Map.BroadcastRoomMsgAsync(MapX, MapY, msg);
        }

        public override async Task SendSpawnToAsync(Character player)
        {
            if (IsAlive)
                await player.SendAsync(new MsgPlayer(this));
        }

        #endregion

        public const int ATKUSER_LEAVEONLY = 0, // Ö»»áÌÓÅÜ
            ATKUSER_PASSIVE = 0x01, // ±»¶¯¹¥»÷
            ATKUSER_ACTIVE = 0x02, // Ö÷¶¯¹¥»÷
            ATKUSER_RIGHTEOUS = 0x04, // ÕýÒåµÄ(ÎÀ±ø»òÍæ¼ÒÕÙ»½ºÍ¿ØÖÆµÄ¹ÖÎï)
            ATKUSER_GUARD = 0x08, // ÎÀ±ø(ÎÞÊÂ»ØÔ­Î»ÖÃ)
            ATKUSER_PPKER = 0x10, // ×·É±ºÚÃû 
            ATKUSER_JUMP = 0x20, // »áÌø
            ATKUSER_FIXED = 0x40, // ²»»á¶¯µÄ
            ATKUSER_FASTBACK = 0x0080, // ËÙ¹é
            ATKUSER_LOCKUSER = 0x0100, // Ëø¶¨¹¥»÷Ö¸¶¨Íæ¼Ò£¬Íæ¼ÒÀë¿ª×Ô¶¯ÏûÊ§ 
            ATKUSER_LOCKONE = 0x0200, // Ëø¶¨¹¥»÷Ê×ÏÈ¹¥»÷×Ô¼ºµÄÍæ¼Ò
            ATKUSER_ADDLIFE = 0x0400, // ×Ô¶¯¼ÓÑª
            ATKUSER_EVIL_KILLER = 0x0800, // °×ÃûÉ±ÊÖ
            ATKUSER_WING = 0x1000, // ·ÉÐÐ×´Ì¬
            ATKUSER_NEUTRAL = 0x2000, // ÖÐÁ¢
            ATKUSER_ROAR = 0x4000, // ³öÉúÊ±È«µØÍ¼Å­ºð
            ATKUSER_NOESCAPE = 0x8000, // ²»»áÌÓÅÜ
            ATKUSER_EQUALITY = 0x10000; // ²»ÃêÊÓ

        enum AiStage
        {
            /// <summary>
            /// Monster is doing absolutely nothing.
            /// </summary>
            Idle,
            /// <summary>
            /// Monster wont do nothing, just heal. Activated if on active block but haven't triggered any other action.
            /// </summary>
            Dormancy,
            /// <summary>
            /// Monster will do some random move?
            /// </summary>
            Move,
            /// <summary>
            /// Monster is ready for attack.
            /// </summary>
            Attack,
            /// <summary>
            /// When monster is low life and want to run from the attacker.
            /// </summary>
            Escape
        }
    }
}