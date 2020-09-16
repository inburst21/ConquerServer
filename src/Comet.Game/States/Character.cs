// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Character.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Core.Mathematics;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.States.Magics;
using Comet.Game.States.NPCs;
using Comet.Game.States.Relationship;
using Comet.Game.States.Syndicates;
using Comet.Game.World;
using Comet.Game.World.Managers;
using Comet.Game.World.Maps;
using Comet.Network.Packets;
using Comet.Shared;

#endregion

namespace Comet.Game.States
{
    /// <summary>
    ///     Character class defines a database record for a player's character. This allows
    ///     for easy saving of character information, as well as means for wrapping character
    ///     data for spawn packet maintenance, interface update pushes, etc.
    /// </summary>
    public class Character : Role
    {
        private Client m_socket;
        private readonly DbCharacter m_dbObject;

        private TimeOut m_energyTm = new TimeOut(ADD_ENERGY_STAND_SECS);
        private TimeOut m_autoHeal = new TimeOut(AUTOHEALLIFE_TIME);
        private TimeOut m_pkDecrease = new TimeOut(PK_DEC_TIME);
        private TimeOut m_xpPoints = new TimeOut(3);
        private TimeOut m_ghost = new TimeOut(3);
        private TimeOut m_transformation = new TimeOut();
        private TimeOut m_tRevive = new TimeOut();
        private TimeOut m_respawn = new TimeOut();
        private TimeOut m_mine = new TimeOut(2);
        private TimeOut m_teamLeaderPos = new TimeOut(3);
        private TimeOut m_timeSync = new TimeOut(15);

        private ConcurrentDictionary<RequestType, uint> m_dicRequests = new ConcurrentDictionary<RequestType, uint>();

        /// <summary>
        ///     Instantiates a new instance of <see cref="Character" /> using a database fetched
        ///     <see cref="DbCharacter" />. Copies attributes over to the base class of this
        ///     class, which will then be used to save the character from the game world.
        /// </summary>
        /// <param name="character">Database character information</param>
        /// <param name="socket"></param>
        public Character(DbCharacter character, Client socket)
        {
            if (socket == null)
                return; // ?

            m_socket = socket;

            /*
             * Removed the base class because we'll be inheriting role stuff.
             */
            m_dbObject = character;

            m_mesh = m_dbObject.Mesh;

            m_posX = character.X;
            m_posY = character.Y;
            m_idMap = character.MapID;

            Screen = new Screen(this);
            WeaponSkill = new WeaponSkill(this);
            UserPackage = new UserPackage(this);
            Statistic = new UserStatistic(this);
            TaskDetail = new TaskDetail(this);

            m_energyTm.Update();
            m_autoHeal.Update();
            m_pkDecrease.Update();
            m_xpPoints.Update();
            m_ghost.Update();
        }

        public Client Client => m_socket;

        #region Identity

        public override uint Identity
        {
            get => m_dbObject.Identity;
            protected set
            {
                // cannot change the identity
            }
        }

        public override string Name
        {
            get => m_dbObject.Name;
            set => m_dbObject.Name = value;
        }

        public string MateName { get; set; }

        public uint MateIdentity
        {
            get => m_dbObject.Mate;
            set => m_dbObject.Mate = value;
        }

        #endregion
        
        #region Appearence

        private uint m_mesh = 0;
        private ushort m_transformMesh = 0;

        public int Gender => Body == BodyType.AgileMale || Body == BodyType.MuscularMale ? 1 : 2;

        public ushort TransformationMesh
        {
            get => m_transformMesh;
            set
            {
                m_transformMesh = value;
                Mesh = (uint)((uint)value * 10000000 + Avatar * 10000 + (uint) Body);
            }
        }

        public override uint Mesh
        {
            get => m_mesh;
            set
            {
                m_mesh = value;
                m_dbObject.Mesh = value % 10000000;
            }
        }

        public BodyType Body
        {
            get => (BodyType)(Mesh % 10000);
            set => Mesh = (uint)(value + (ushort)(Avatar * 10000));
        }

        public ushort Avatar
        {
            get => (ushort)(Mesh % 1000000 / 10000);
            set => Mesh = (uint)(value * 10000 + (int)Body);
        }

        public ushort Hairstyle
        {
            get => m_dbObject.Hairstyle;
            set => m_dbObject.Hairstyle = value;
        }

        #endregion

        #region Transformation

        public Transformation Transformation { get; protected set; }

        public async Task<bool> Transform(uint dwLook, int nKeepSecs, bool bSynchro)
        {
            bool bBack = false;

            if (Transformation != null)
            {
                await ClearTransformation();
                bBack = true;
            }

            DbMonstertype pType = Kernel.RoleManager.GetMonstertype(dwLook);
            if (pType == null)
            {
                return false;
            }

            Transformation pTransform = new Transformation(this);
            if (pTransform.Create(pType))
            {
                Transformation = pTransform;
                TransformationMesh = (ushort)pTransform.Lookface;
                await SetAttributesAsync(ClientUpdateType.Mesh, Mesh);
                Life = MaxLife;
                m_transformation = new TimeOut(nKeepSecs);
                m_transformation.Startup(nKeepSecs);
                if (bSynchro)
                    await SynchroTransform();
            }
            else
            {
                pTransform = null;
            }

            if (bBack)
                await SynchroTransform();

            return false;
        }

        public async Task ClearTransformation()
        {
            TransformationMesh = 0;
            Transformation = null;
            m_transformation.Clear();
            
            await SynchroTransform();
            await MagicData.AbortMagic(true);
            BattleSystem.ResetBattle();
        }

        public async Task<bool> SynchroTransform()
        {
            MsgUserAttrib msg = new MsgUserAttrib(Identity, ClientUpdateType.Mesh, Mesh);
            if (TransformationMesh != 98 && TransformationMesh != 99)
            {
                Life = MaxLife;
                msg.Append(ClientUpdateType.MaxHitpoints, MaxLife);
                msg.Append(ClientUpdateType.Hitpoints, Life);
            }
            await BroadcastRoomMsgAsync(msg, true);


            //await SynchroAttributesAsync(ClientUpdateType.Mesh, Mesh, true);
            //await SynchroAttributesAsync(ClientUpdateType.Hitpoints, Life, true);
            //await SynchroAttributesAsync(ClientUpdateType.MaxHitpoints, MaxLife, true);
            return true;
        }

        public async Task SetGhost()
        {
            if (IsAlive) return;

            ushort trans = 98;
            if (Gender == 2)
                trans = 99;
            //TransformationMesh = trans;
            //await Transform(trans, int.MaxValue, true);
            TransformationMesh = trans;
            await SynchroTransform();
        }

        #endregion

        #region Profession

        public byte ProfessionSort => (byte)(Profession / 10);

        public byte ProfessionLevel => (byte)(Profession % 10);

        public byte Profession
        {
            get => m_dbObject?.Profession ?? 0;
            set => m_dbObject.Profession = value;
        }

        public byte PreviousProfession
        {
            get => m_dbObject?.PreviousProfession ?? 0;
            set => m_dbObject.PreviousProfession = value;
        }

        public byte FirstProfession
        {
            get => m_dbObject?.FirstProfession ?? 0;
            set => m_dbObject.FirstProfession = value;
        }

        #endregion

        #region Attribute Points

        public ushort Strength
        {
            get => m_dbObject?.Strength ?? 0;
            set => m_dbObject.Strength = value;
        }

        public ushort Agility
        {
            get => m_dbObject?.Agility ?? 0;
            set => m_dbObject.Agility = value;
        }

        public ushort Vitality
        {
            get => m_dbObject?.Vitality ?? 0;
            set => m_dbObject.Vitality = value;
        }

        public ushort Spirit
        {
            get => m_dbObject?.Spirit ?? 0;
            set => m_dbObject.Spirit = value;
        }

        public ushort AttributePoints
        {
            get => m_dbObject?.AttributePoints ?? 0;
            set => m_dbObject.AttributePoints = value;
        }

        #endregion

        #region Life and Mana

        public override uint Life
        {
            get => m_dbObject.HealthPoints;
            set => m_dbObject.HealthPoints = (ushort)Math.Min(MaxLife, value);
        }

        public override uint MaxLife
        {
            get
            {
                if (Transformation != null)
                    return (uint) Transformation.MaxLife;

                uint result = (uint)(Vitality * 24);
                switch (Profession)
                {
                    case 11:
                        result = (uint)(result * 1.05d);
                        break;
                    case 12:
                        result = (uint)(result * 1.08d);
                        break;
                    case 13:
                        result = (uint)(result * 1.10d);
                        break;
                    case 14:
                        result = (uint)(result * 1.12d);
                        break;
                    case 15:
                        result = (uint)(result * 1.15d);
                        break;
                }

                result += (uint)((Strength + Agility + Spirit) * 3);

                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin;
                    pos < Item.ItemPosition.EquipmentEnd;
                    pos++)
                {
                    result += (uint)(UserPackage[pos]?.Life ?? 0);
                }

                return result;
            }
        }

        public override uint Mana
        {
            get => m_dbObject.ManaPoints;
            set => 
                m_dbObject.ManaPoints = (ushort)Math.Min(MaxMana, value);
        }

        public override uint MaxMana
        {
            get
            {
                uint result = (uint)(Spirit * 5);
                switch (Profession)
                {
                    case 132:
                    case 142:
                        result *= 3;
                        break;
                    case 133:
                    case 143:
                        result *= 4;
                        break;
                    case 134:
                    case 144:
                        result *= 5;
                        break;
                    case 135:
                    case 145:
                        result *= 6;
                        break;
                }

                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin;
                    pos < Item.ItemPosition.EquipmentEnd;
                    pos++)
                {
                    result += (uint)(UserPackage[pos]?.Mana ?? 0);
                }

                return result;
            }
        }

        #endregion

        #region Level and Experience

        public bool AutoAllot
        {
            get => m_dbObject.AutoAllot != 0;
            set => m_dbObject.AutoAllot = (byte) (value ? 1 : 0);
        }

        public override byte Level
        {
            get => m_dbObject?.Level ?? 0;
            set => m_dbObject.Level = Math.Min(MAX_UPLEV, Math.Max((byte)1, value));
        }

        public ulong Experience
        {
            get => m_dbObject?.Experience ?? 0;
            set
            {
                if (Level >= MAX_UPLEV)
                    return;

                m_dbObject.Experience = value;
            }
        }

        public byte Metempsychosis
        {
            get => m_dbObject?.Rebirths ?? 0;
            set => m_dbObject.Rebirths = value;
        }

        public bool IsNewbie()
        {
            return Level < 70;
        }

        public async Task<bool> AwardLevelAsync(ushort amount)
        {
            if (Level >= MAX_UPLEV)
                return false;

            if (Level + amount <= 0)
                return false;

            int addLev = amount;
            if (addLev + Level > MAX_UPLEV)
                addLev = MAX_UPLEV - Level;

            if (addLev <= 0)
                return false;

            await AddAttributesAsync(ClientUpdateType.Atributes, (ushort) (addLev * 3));
            await AddAttributesAsync(ClientUpdateType.Level, addLev);
            await BroadcastRoomMsgAsync(new MsgAction
            {
                Identity = Identity,
                Action = MsgAction.ActionType.CharacterLevelUp,
                ArgumentX = MapX,
                ArgumentY = MapY
            }, true);
            return true;
        }

        public async Task AwardBattleExp(long nExp, bool bGemEffect)
        {
            if (Metempsychosis == 2)
                nExp /= 3;

            if (nExp == 0)
                return;

            double multiplier = 1;
            if (HasMultipleExp)
                multiplier += ExperienceMultiplier - 1;

            if (Level >= 70 && ProfessionSort == 13 && ProfessionLevel >= 3)
                multiplier += 1;

            /*
             * TODO: Attention, this is only for ALPHA!!! REMOVE
             */
            multiplier += 10;

            nExp = (long)(nExp * Math.Max(1, multiplier));

            if (nExp < 0)
            {
                await AddAttributesAsync(ClientUpdateType.Experience, nExp);
                return;
            }

            nExp = (long) (nExp * 1 + BattlePower / 100d);

            if (bGemEffect)
            {
                nExp += (int)(nExp * (1 + (RainbowGemBonus / 100d)));

                if (RainbowGemBonus > 0 && IsPm())
                    await SendAsync($"got gem exp add percent: {RainbowGemBonus:0.00}%");
            }

            if (Level >= MAX_UPLEV)
                return;

            if (Level >= 120)
                nExp /= 2;

            if (Metempsychosis >= 2)
                nExp /= 3;

            if (IsPm())
                await SendAsync($"got battle exp: {nExp}");

            await AwardExperience(nExp);
        }

        public long AdjustExperience(Role pTarget, long nRawExp, bool bNewbieBonusMsg)
        {
            if (pTarget == null) return 0;
            long nExp = nRawExp;
            nExp = BattleSystem.AdjustExp(nExp, Level, pTarget.Level);
            return nExp;
        }

        public async Task<bool> AwardExperience(long amount)
        {
            if (Level > Kernel.RoleManager.GetLevelLimit())
                return true;
            
            amount += (long)Experience;
            bool leveled = false;
            uint pointAmount = 0;
            byte newLevel = Level;
            ushort virtue = 0;
            while (newLevel < MAX_UPLEV && amount >= (long)Kernel.RoleManager.GetLevelExperience(newLevel).Exp)
            {
                DbLevelExperience dbExp = Kernel.RoleManager.GetLevelExperience(newLevel);
                amount -= (long) dbExp.Exp;
                leveled = true;
                newLevel++;

                if (newLevel <= 70)
                {
                    virtue += (ushort)dbExp.UpLevTime;
                }

                if (!AutoAllot || Level > 120)
                {
                    pointAmount += 3;
                    continue;
                }

                if (newLevel < Kernel.RoleManager.GetLevelLimit()) continue;
                amount = 0;
                break;
            }

            uint metLev = 0;
            var leveXp = Kernel.RoleManager.GetLevelExperience(newLevel);
            if (leveXp != null)
            {
                float fExp = amount / (float)leveXp.Exp;
                metLev = (uint)(newLevel * 10000 + fExp * 1000);
            }

            byte checkLevel = (byte)(m_dbObject.Reincarnation > 0 ? 110 : 130);
            if (newLevel >= checkLevel && Metempsychosis > 0 && m_dbObject.MeteLevel > metLev)
            {
                byte extra = 0;
                if (newLevel >= checkLevel && m_dbObject.MeteLevel / 10000 > newLevel)
                {
                    var mete = m_dbObject.MeteLevel / 10000;
                    extra += (byte)(mete - newLevel);
                    pointAmount += (uint)(extra * 3);
                    leveled = true;
                    amount = 0;
                }

                newLevel += extra;

                if (newLevel >= Kernel.RoleManager.GetLevelLimit())
                {
                    newLevel = (byte)Kernel.RoleManager.GetLevelLimit();
                    amount = 0;
                }
                else if (m_dbObject.MeteLevel >= newLevel * 10000)
                {
                    amount = (long)(Kernel.RoleManager.GetLevelExperience(newLevel).Exp * ((m_dbObject.MeteLevel % 10000) / 1000d));
                }
            }

            if (leveled)
            {
                byte job;
                if ((int)Profession > 100)
                    job = 10;
                else
                    job = (byte)(((int)Profession - (int)Profession % 10) / 10);

                var allot = Kernel.RoleManager.GetPointAllot(job, newLevel);
                Level = newLevel;
                if (AutoAllot && allot != null)
                {
                    await SetAttributesAsync(ClientUpdateType.Strength, allot.Strength);
                    await SetAttributesAsync(ClientUpdateType.Agility, allot.Agility);
                    await SetAttributesAsync(ClientUpdateType.Vitality, allot.Vitality);
                    await SetAttributesAsync(ClientUpdateType.Spirit, allot.Spirit);
                }
                else if (pointAmount > 0)
                    await AddAttributesAsync(ClientUpdateType.Atributes, (int)pointAmount);

                await SetAttributesAsync(ClientUpdateType.Level, Level);
                await SetAttributesAsync(ClientUpdateType.Hitpoints, MaxLife);
                await SetAttributesAsync(ClientUpdateType.Mana, MaxMana);
                await Screen.BroadcastRoomMsgAsync(new MsgAction
                {
                    Action = MsgAction.ActionType.CharacterLevelUp,
                    Identity = Identity
                });
            }

            if (Team != null && !Team.IsLeader(Identity) && virtue > 0)
            {
                Team.Leader.VirtuePoints += virtue;
                await Team.SendAsync(new MsgTalk(Identity, MsgTalk.TalkChannel.Team, Color.White, 
                    string.Format(Language.StrAwardVirtue, Team.Leader.Name, virtue)));
            }

            Experience = (ulong)amount;
            await SetAttributesAsync(ClientUpdateType.Experience, (long) Experience);
            return true;
        }

        public long CalculateExpBall(int amount = EXPBALL_AMOUNT)
        {
            long exp = 0;

            if (Level >= Kernel.RoleManager.GetLevelLimit())
                return 0;

            byte level = Level;
            if (Experience > 0)
            {
                double pct = 1.00 - Experience / (double)Kernel.RoleManager.GetLevelExperience(Level).Exp;
                if (amount > pct * Kernel.RoleManager.GetLevelExperience(Level).UpLevTime)
                {
                    amount -= (int)(pct * Kernel.RoleManager.GetLevelExperience(Level).UpLevTime);
                    exp += (long)(Kernel.RoleManager.GetLevelExperience(Level).Exp - Experience);
                    level++;
                }
            }

            while (amount > Kernel.RoleManager.GetLevelExperience(level).UpLevTime)
            {
                amount -= Kernel.RoleManager.GetLevelExperience(level).UpLevTime;
                exp += (long)Kernel.RoleManager.GetLevelExperience(level).Exp;

                if (level >= Kernel.RoleManager.GetLevelLimit())
                    return exp;
                level++;
            }

            exp += (long)(amount / (double)Kernel.RoleManager.GetLevelExperience(Level).UpLevTime *
                          Kernel.RoleManager.GetLevelExperience(Level).Exp);
            return exp;
        }

        public void IncrementExpBall()
        {
            m_dbObject.ExpBallUsage = uint.Parse(DateTime.Now.ToString("yyyyMMdd"));
            m_dbObject.ExpBallNum += 1;
        }

        public bool CanUseExpBall()
        {
            if (Level >= Kernel.RoleManager.GetLevelLimit())
                return false;

            if (m_dbObject.ExpBallUsage < uint.Parse(DateTime.Now.ToString("yyyyMMdd")))
            {
                m_dbObject.ExpBallNum = 0;
                return true;
            }

            return m_dbObject.ExpBallNum < 10;
        }

        #endregion

        #region Weapon Skill

        public WeaponSkill WeaponSkill { get; }
        
        public async Task AddWeaponSkillExpAsync(ushort usType, int nExp)
        {
            DbWeaponSkill skill = WeaponSkill[usType];
            if (skill == null)
            {
                await WeaponSkill.CreateAsync(usType, 0);
                if ((skill = WeaponSkill[usType]) == null)
                    return;
            }

            if (skill.Level >= MAX_WEAPONSKILLLEVEL)
                return;

            if (skill.Unlearn != 0)
                skill.Unlearn = 0;

            nExp = (int)(nExp * (1 + VioletGemBonus / 100d));

            uint nIncreaseLev = 0;
            if (skill.Level > MASTER_WEAPONSKILLLEVEL)
            {
                int nRatio = (int)(100 - (skill.Level - MASTER_WEAPONSKILLLEVEL) * 20);
                if (nRatio < 10)
                    nRatio = 10;
                nExp = Calculations.MulDiv(nExp, nRatio, 100) / 2;
            }

            int nNewExp = (int)Math.Max(nExp + skill.Experience, skill.Experience);

#if DEBUG
            if (IsPm())
                await SendAsync($"Add Weapon Skill exp: {nExp}, CurExp: {nNewExp}");
#endif

            int nLevel = (int)skill.Level;
            if (nLevel < MAX_WEAPONSKILLLEVEL)
            {
                if (nNewExp > MsgWeaponSkill.RequiredExperience[nLevel] ||
                    nLevel >= skill.OldLevel / 2 && nLevel < skill.OldLevel)
                {
                    nNewExp = 0;
                    nIncreaseLev = 1;
                }
            }

            if (skill.Level < Level / 10 + 1
                || skill.Level >= MASTER_WEAPONSKILLLEVEL)
            {
                skill.Experience = (uint)nNewExp;

                if (nIncreaseLev > 0)
                {
                    skill.Level += (byte) nIncreaseLev;
                    await SendAsync(new MsgWeaponSkill(skill));
                    await SendAsync(Language.StrWeaponSkillUp);
                }
                else
                {
                    await SendAsync(new MsgFlushExp
                    {
                        Action = MsgFlushExp.FlushMode.WeaponSkill,
                        Identity = (ushort) skill.Type,
                        Experience = skill.Experience
                    });
                }

                await WeaponSkill.SaveAsync(skill);
            }
        }

        #endregion

        #region Currency

        public uint Silvers
        {
            get => m_dbObject?.Silver ?? 0;
            set => m_dbObject.Silver = value;
        }

        public uint ConquerPoints
        {
            get => m_dbObject?.ConquerPoints ?? 0;
            set => m_dbObject.ConquerPoints = value;
        }

        public uint StorageMoney
        {
            get => m_dbObject?.StorageMoney ?? 0;
            set => m_dbObject.StorageMoney = value;
        }

        public async Task<bool> ChangeMoney(int amount, bool notify = false)
        {
            if (amount > 0)
            {
                await AwardMoney(amount);
                return true;
            }
            if (amount < 0)
            {
                return await SpendMoney(amount * -1, notify);
            }
            return false;
        }

        public async Task AwardMoney(int amount)
        {
            Silvers = (uint) (Silvers + amount);
            await SaveAsync();
            await SynchroAttributesAsync(ClientUpdateType.Money, Silvers);
        }

        public async Task<bool> SpendMoney(int amount, bool notify = false)
        {
            if (amount > Silvers)
            {
                if (notify)
                    await SendAsync(Language.StrNotEnoughMoney, MsgTalk.TalkChannel.TopLeft, Color.Red);
                return false;
            }

            Silvers = (uint)(Silvers - amount);
            await SaveAsync();
            await SynchroAttributesAsync(ClientUpdateType.Money, Silvers);
            return true;
        }

        public async Task<bool> ChangeConquerPoints(int amount, bool notify = false)
        {
            if (amount > 0)
            {
                await AwardConquerPoints(amount);
                return true;
            }
            if (amount < 0)
            {
                return await SpendConquerPoints(amount * -1, notify);
            }
            return false;
        }

        public async Task AwardConquerPoints(int amount)
        {
            ConquerPoints = (uint)(ConquerPoints + amount);
            await SaveAsync();
            await SynchroAttributesAsync(ClientUpdateType.ConquerPoints, ConquerPoints);
        }

        public async Task<bool> SpendConquerPoints(int amount, bool notify = false)
        {
            if (amount > ConquerPoints)
            {
                if (notify)
                    await SendAsync(Language.StrNotEnoughEmoney, MsgTalk.TalkChannel.TopLeft, Color.Red);
                return false;
            }

            ConquerPoints = (uint)(ConquerPoints - amount);
            await SaveAsync();
            await SynchroAttributesAsync(ClientUpdateType.ConquerPoints, ConquerPoints);
            return true;
        }

        #endregion

        #region Pk

        public PkModeType PkMode { get; set; }

        public ushort PkPoints
        {
            get => m_dbObject?.KillPoints ?? 0;
            set => m_dbObject.KillPoints = value;
        }

        public async Task ProcessPk(Character target)
        {
            if (!Map.IsPkField() && !Map.IsPkGameMap() && !Map.IsSynMap() && !Map.IsPrisionMap())
            {
                if (!Map.IsDeadIsland() && !target.IsEvil())
                {
                    int nAddPk = 10;
                    if (target.Level < 130)
                    {
                        nAddPk = 20;
                    }
                    else
                    {
                        //if (Syndicate?.IsHostile((ushort)target.SyndicateIdentity) == true)
                        //    nAddPk = 3;
                        //else if (ContainsEnemy(target.Identity))
                        //    nAddPk = 5;
                        if (target.PkPoints > 29)
                            nAddPk /= 2;
                    }

                    await AddAttributesAsync(ClientUpdateType.PkPoints, nAddPk);

                    await SetCrimeStatus(60);

                    if (PkPoints > 29)
                        await SendAsync(Language.StrKillingTooMuch);
                }
            }
        }

        public override async Task<bool> CheckCrime(Role target)
        {
            if (target == null) return false;
            if (!target.IsEvil() && !target.IsMonster())
            {
                if (!Map.IsTrainingMap() && !Map.IsDeadIsland() && !Map.IsDeadIsland() && !Map.IsPrisionMap() &&
                    !Map.IsFamilyMap() && !Map.IsPkGameMap() && !Map.IsPkField())
                {
                    await SetCrimeStatus(25);
                }
                return true;
            }

            if (target.IsMonster() && ((Monster) target).IsGuard())
            {
                await SetCrimeStatus(25);
                return true;
            }

            return false;
        }

        #endregion

        #region Equipment

        public Item Headgear => UserPackage[Item.ItemPosition.Headwear];
        public Item Necklace => UserPackage[Item.ItemPosition.Necklace];
        public Item Ring => UserPackage[Item.ItemPosition.Ring];
        public Item RightHand => UserPackage[Item.ItemPosition.RightHand];
        public Item LeftHand => UserPackage[Item.ItemPosition.LeftHand];
        public Item Armor => UserPackage[Item.ItemPosition.Armor];
        public Item Boots => UserPackage[Item.ItemPosition.Boots];
        public Item Garment => UserPackage[Item.ItemPosition.Garment];

        #endregion

        #region User Package

        public uint LastAddItemIdentity { get; set; }

        public UserPackage UserPackage { get; }

        public async Task<bool> SpendEquipItem(uint dwItem, uint dwAmount, bool bSynchro)
        {
            if (dwItem <= 0)
                return false;

            Item item = null;
            if (UserPackage[Item.ItemPosition.RightHand]?.GetItemSubType() == dwItem &&
                UserPackage[Item.ItemPosition.RightHand]?.Durability >= dwAmount)
                item = UserPackage[Item.ItemPosition.RightHand];
            else if (UserPackage[Item.ItemPosition.LeftHand]?.GetItemSubType() == dwItem)
                item = UserPackage[Item.ItemPosition.LeftHand];

            if (item == null)
                return false;

            if (!item.IsExpend() && item.Durability < dwAmount && !item.IsArrowSort())
                return false;

            if (item.IsExpend())
            {
                item.Durability = (ushort)Math.Max(0, item.Durability - (int) dwAmount);
                if (bSynchro)
                    await SendAsync(new MsgItemInfo(item, MsgItemInfo.ItemMode.Update));
            }
            else
            {
                if (item.IsNonsuchItem())
                {
                    await Log.GmLog("SpendEquipItem",
                        $"{Name}({Identity}) Spend item:[id={item.Identity}, type={item.Type}], dur={item.Durability}, max_dur={item.MaximumDurability}");
                }
            }

            if (item.IsArrowSort() && item.Durability == 0)
            {
                Item.ItemPosition pos = item.Position;
                await UserPackage.UnequipAsync(item.Position, UserPackage.RemovalType.Delete);
                Item other = UserPackage.GetItemByType(item.Type);
                if (other != null)
                    await UserPackage.EquipItemAsync(other, pos);
            }

            if (item.Durability > 0)
                await item.SaveAsync();
            return true;
        }

        public bool CheckWeaponSubType(uint idItem, uint dwNum = 0)
        {
            uint[] items = new uint[idItem.ToString().Length / 3];
            for (int i = 0; i < items.Length; i++)
            {
                if (idItem > 999 && idItem != 40000 && idItem != 50000)
                {
                    int idx = i * 3; // + (i > 0 ? -1 : 0);
                    items[i] = uint.Parse(idItem.ToString().Substring(idx, 3));
                }
                else
                {
                    items[i] = uint.Parse(idItem.ToString());
                }
            }

            if (items.Length <= 0) return false;

            foreach (var dwItem in items)
            {
                if (dwItem <= 0) continue;

                if (UserPackage[Item.ItemPosition.RightHand] != null &&
                    UserPackage[Item.ItemPosition.RightHand].GetItemSubType() == dwItem &&
                    UserPackage[Item.ItemPosition.RightHand].Durability >= dwNum)
                    return true;
                if (UserPackage[Item.ItemPosition.LeftHand] != null &&
                    UserPackage[Item.ItemPosition.LeftHand].GetItemSubType() == dwItem &&
                    UserPackage[Item.ItemPosition.LeftHand].Durability >= dwNum)
                    return true;

                ushort[] set1Hand = { 410, 420, 421, 430, 440, 450, 460, 480, 481, 490 };
                ushort[] set2Hand = { 510, 530, 540, 560, 561, 580 };
                ushort[] setSword = { 420, 421 };
                ushort[] setSpecial = { 601, 610, 611, 612, 613 };

                if (dwItem == 40000 || dwItem == 400)
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null)
                    {
                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        for (int i = 0; i < set1Hand.Length; i++)
                        {
                            if (item.GetItemSubType() == set1Hand[i] && item.Durability >= dwNum)
                                return true;
                        }
                    }
                }

                if (dwItem == 50000)
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null)
                    {
                        if (dwItem == 50000) return true;

                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        for (int i = 0; i < set2Hand.Length; i++)
                        {
                            if (item.GetItemSubType() == set2Hand[i] && item.Durability >= dwNum)
                                return true;
                        }
                    }
                }

                if (dwItem == 50) // arrow
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null &&
                        UserPackage[Item.ItemPosition.LeftHand] != null)
                    {
                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        Item arrow = UserPackage[Item.ItemPosition.LeftHand];
                        if (arrow.GetItemSubType() == 1050 && arrow.Durability >= dwNum)
                            return true;
                    }
                }

                if (dwItem == 500)
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null &&
                        UserPackage[Item.ItemPosition.LeftHand] != null)
                    {
                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        if (item.GetItemSubType() == idItem && item.Durability >= dwNum)
                            return true;
                    }
                }

                if (dwItem == 420)
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null)
                    {
                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        for (int i = 0; i < setSword.Length; i++)
                        {
                            if (item.GetItemSubType() == setSword[i] && item.Durability >= dwNum)
                                return true;
                        }
                    }
                }

                if (dwItem == 601 || dwItem == 610 || dwItem == 611 || dwItem == 612 || dwItem == 613)
                {
                    if (UserPackage[Item.ItemPosition.RightHand] != null)
                    {
                        Item item = UserPackage[Item.ItemPosition.RightHand];
                        if (item.GetItemSubType() == dwItem && item.Durability >= dwNum)
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Booth

        public BoothNpc Booth { get; private set; }

        public async Task<bool> CreateBoothAsync()
        {
            if (Booth != null)
            {
                await Booth.LeaveMap();
                Booth = null;
                return false;
            }

            if (Map?.IsBoothEnable() != true)
            {
                await SendAsync(Language.StrBoothRegionCantSetup);
                return false;
            }

            Booth = new BoothNpc(this);
            if (!await Booth.InitializeAsync())
                return false;
            return true;
        }

        public async Task<bool> DestroyBoothAsync()
        {
            if (Booth == null)
                return false;

            await Booth.LeaveMap();
            Booth = null;
            return true;
        }

        public bool AddBoothItem(uint idItem, uint value, MsgItem.Moneytype type)
        {
            if (Booth == null)
                return false;

            if (!Booth.ValidateItem(idItem))
                return false;

            Item item = UserPackage[idItem];
            return Booth.AddItem(item, value, type);
        }

        public bool RemoveBoothItem(uint idItem)
        {
            if (Booth == null)
                return false;
            return Booth.RemoveItem(idItem);
        }

        public async Task<bool> SellBoothItemAsync(uint idItem, Character target)
        {
            if (Booth == null)
                return false;

            if (target.Identity == Identity)
                return false;

            if (!target.UserPackage.IsPackSpare(1))
                return false;

            if (GetDistance(target) > Screen.VIEW_SIZE)
                return false;

            if (!Booth.ValidateItem(idItem))
                return false;

            BoothItem item = Booth.QueryItem(idItem);
            int value = (int) item.Value;
            string moneyType = item.IsSilver ? Language.StrSilvers : Language.StrConquerPoints;
            if (item.IsSilver)
            {
                if (!await target.SpendMoney((int) item.Value, true))
                    return false;
                await AwardMoney(value);
            }
            else
            {
                if (!await target.SpendConquerPoints((int) item.Value, true))
                    return false;
                await AwardConquerPoints(value);
            }

            Booth.RemoveItem(idItem);

            await SendAsync(new MsgItem(item.Identity, MsgItem.ItemActionType.BoothRemove) {Command = Booth.Identity});
            await UserPackage.RemoveFromInventoryAsync(item.Item, UserPackage.RemovalType.RemoveAndDisappear);
            await item.Item.ChangeOwnerAsync(target.Identity, Item.ChangeOwnerType.BoothSale);
            await target.UserPackage.AddItemAsync(item.Item);

            await SendAsync(string.Format(Language.StrBoothSold, target.Name, item.Item.Name, value, moneyType), MsgTalk.TalkChannel.Talk, Color.White);
            await target.SendAsync(string.Format(Language.StrBoothBought, item.Item.Name, value, moneyType), MsgTalk.TalkChannel.Talk, Color.White);

            return true;
        }

        #endregion

        #region Map Item

        public async Task<bool> DropItem(uint idItem, int x, int y, bool force = false)
        {
            Point pos = new Point(x, y);
            if (!Map.FindDropItemCell(9, ref pos))
                return false;

            Item item = UserPackage[idItem];
            if (item == null)
                return false;

            await Log.GmLog("drop_item",
                $"{Name}({Identity}) drop item:[id={item.Identity}, type={item.Type}], dur={item.Durability}, max_dur={item.OriginalMaximumDurability}\r\n\t{item.ToJson()}");

            if ((item.CanBeDropped() || force) && item.IsDisappearWhenDropped())
                return await UserPackage.RemoveFromInventoryAsync(item, UserPackage.RemovalType.Delete);

            if (item.CanBeDropped() || force)
            {
                await UserPackage.RemoveFromInventoryAsync(item, UserPackage.RemovalType.RemoveAndDisappear);
            }
            else
            {
                await SendAsync(string.Format(Language.StrItemCannotDiscard, item.Name));
                return false;
            }

            item.Position = Item.ItemPosition.Floor;
            await item.SaveAsync();

            MapItem mapItem = new MapItem((uint) IdentityGenerator.MapItem.GetNextIdentity);
            if (await mapItem.Create(Map, pos, item, Identity))
            {
                await mapItem.EnterMap();
                await item.SaveAsync();
            }
            else
            {
                IdentityGenerator.MapItem.ReturnIdentity(mapItem.Identity);
                if (IsGm())
                {
                    await SendAsync($"The MapItem object could not be created. Check Output log");
                }
                return false;
            }

            return true;
        }

        public async Task<bool> DropSilver(uint amount)
        {
            if (amount > 10000000)
                return false;

            Point pos = new Point(MapX, MapY);
            if (!Map.FindDropItemCell(1, ref pos))
                return false;

            if (!await SpendMoney((int) amount, true))
                return false;

            await Log.GmLog("drop_money", $"drop money: {Identity} {Name} has dropped {amount} silvers");

            MapItem mapItem = new MapItem((uint)IdentityGenerator.MapItem.GetNextIdentity);
            if (mapItem.CreateMoney(Map, pos, amount, 0u))
                await mapItem.EnterMap();
            else
            {
                IdentityGenerator.MapItem.ReturnIdentity(mapItem.Identity);
                if (IsGm())
                {
                    await SendAsync($"The DropSilver MapItem object could not be created. Check Output log");
                }
                return false;
            }

            return true;
        }

        public async Task<bool> PickMapItem(uint idItem)
        {
            MapItem mapItem = Map.QueryAroundRole(this, idItem) as MapItem;
            if (mapItem == null)
                return false;

            if (GetDistance(mapItem) > 0)
            {
                await SendAsync(Language.StrTargetNotInRange);
                return false;
            }

            if (!mapItem.IsMoney() && !UserPackage.IsPackSpare(1))
            {
                await SendAsync(Language.StrYourBagIsFull);
                return false;
            }

            if (mapItem.OwnerIdentity != Identity && mapItem.IsPrivate())
            {
                Character owner = Kernel.RoleManager.GetUser(mapItem.OwnerIdentity);
                if (owner != null && !IsMate(owner))
                {
                    // todo check team
                    await SendAsync(Language.StrCannotPickupOtherItems);
                    return false;
                }
            }

            if (mapItem.IsMoney())
            {
                await AwardMoney((int) mapItem.Money);
                if (mapItem.Money > 1000)
                {
                    await SendAsync(new MsgAction
                    {
                        Identity = Identity,
                        Command = mapItem.Money,
                        ArgumentX = MapX,
                        ArgumentY = MapY,
                        Action = MsgAction.ActionType.MapGold
                    });
                }
                await SendAsync(string.Format(Language.StrPickupSilvers, mapItem.Money));

                await Log.GmLog("pickup_money", $"User[{Identity},{Name}] picked up {mapItem.Money} at {MapIdentity}({Map.Name}) {MapX}, {MapY}");
            }
            else
            {
                Item item = await mapItem.GetInfo(this);

                if (item != null)
                {
                    await UserPackage.AddItemAsync(item);
                    await SendAsync(string.Format(Language.StrPickupItem, item.Name));

                    await Log.GmLog("pickup_item", $"User[{Identity},{Name}] picked up (id:{mapItem.ItemIdentity}) {mapItem.Itemtype} at {MapIdentity}({Map.Name}) {MapX}, {MapY}");
                }
            }
            
            await mapItem.LeaveMap();
            return true;
        }

        #endregion

        #region Trade

        public Trade Trade { get; set; }

        #endregion

        #region Peerage

        public NobilityRank NobilityRank => Kernel.PeerageManager.GetRanking(Identity);

        public int NobilityPosition => Kernel.PeerageManager.GetPosition(Identity);

        public ulong NobilityDonation
        {
            get => m_dbObject.Donation;
            set => m_dbObject.Donation = value;
        }

        public async Task SendNobilityInfo(bool broadcast = false)
        {
            MsgPeerage msg = new MsgPeerage
            {
                Action = NobilityAction.Info,
                DataLow = Identity
            };
            msg.Strings.Add($"{Identity} {NobilityDonation} {(int) NobilityRank} {NobilityPosition}");
            await SendAsync(msg);

            if (broadcast)
                await BroadcastRoomMsgAsync(msg, false);
        }

        #endregion

        #region Team

        public uint VirtuePoints
        {
            get => m_dbObject.Virtue;
            set => m_dbObject.Virtue = value;
        }

        public Team Team { get; set; }

        #endregion

        #region Battle Attributes

        public override int BattlePower
        {
            get
            {
#if BATTLE_POWER
                int result = Level + Metempsychosis * 5 + (int) NobilityRank;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.BattlePower ?? 0;
                }
                return result;
#else
                return 1;
#endif
            }
        }

        public override int MinAttack
        {
            get
            {
                int result = Strength;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    if (pos == Item.ItemPosition.LeftHand)
                        result += (UserPackage[pos]?.MinAttack ?? 0) / 2;
                    else 
                        result += UserPackage[pos]?.MinAttack ?? 0;
                }

                result = (int) (result * (1 + (DragonGemBonus / 100d)));
                return result;
            }
        }

        public override int MaxAttack
        {
            get
            {
                int result = Strength;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    //if (pos == Item.ItemPosition.LeftHand && UserPackage[pos]?.IsShield() != true)
                    //    result += (UserPackage[pos]?.MaxAttack ?? 0) / 2;
                    //else
                    result += UserPackage[pos]?.MaxAttack ?? 0;
                }

                result = (int)(result * (1 + (DragonGemBonus / 100d)));
                return result;
            }
        }

        public override int MagicAttack
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.MagicAttack ?? 0;
                }

                result = (int)(result * (1 + (PhoenixGemBonus/ 100d)));
                return result;
            }
        }

        public override int Defense
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.Defense ?? 0;
                }
                return result;
            }
        }

        public override int MagicDefense
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.MagicDefense ?? 0;
                }

                if (Metempsychosis > 0 && Level >= 70)
                    result = (int)(result * 1.3d);

                return result;
            }
        }

        public override int MagicDefenseBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.MagicDefenseBonus ?? 0;
                }
                return result;
            }
        }

        public override int Dodge
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.Dodge ?? 0;
                }
                return result;
            }
        }

        public override int Blessing
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.Blessing ?? 0;
                }
                return result;
            }
        }

        public override int AttackSpeed { get; } = 1000;

        public override int Accuracy
        {
            get
            {
                int result = Agility;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.Accuracy ?? 0;
                }
                return result;
            }
        }

        public int DragonGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    Item item = UserPackage[pos];
                    if (item != null)
                    {
                        result += item.DragonGemEffect;
                        //if ((item.IsWeaponTwoHand() || item.IsBow()) && UserPackage[Item.ItemPosition.LeftHand]?.IsArrowSort() == true)
                            //result += item.DragonGemEffect;
                    }
                }
                return result;
            }
        }

        public int PhoenixGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.PhoenixGemEffect ?? 0;
                }
                return result;
            }
        }

        public int VioletGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.VioletGemEffect ?? 0;
                }
                return result;
            }
        }

        public int MoonGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.MoonGemEffect ?? 0;
                }
                return result;
            }
        }

        public int RainbowGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.RainbowGemEffect ?? 0;
                }
                return result;
            }
        }

        public int FuryGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.FuryGemEffect ?? 0;
                }
                return result;
            }
        }

        public int TortoiseGemBonus
        {
            get
            {
                int result = 0;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.TortoiseGemEffect ?? 0;
                }
                return result;
            }
        }

        public int KoCount { get; set; }

        #endregion

        #region Battle

        public override bool IsBowman => UserPackage[Item.ItemPosition.RightHand]?.IsBow() == true;

        public async Task<bool> AutoSkillAttack(Role target)
        {
            foreach (var magic in MagicData.Magics.Values)
            {
                float percent = magic.Percent;
                if (magic.AutoActive > 0
                    && Transformation == null
                    && (magic.WeaponSubtype == 0
                        || CheckWeaponSubType(magic.WeaponSubtype, magic.UseItemNum))
                    && await Kernel.ChanceCalcAsync(percent))
                {
                    return await ProcessMagicAttack(magic.Type, target.Identity, target.MapX, target.MapY, magic.AutoActive);
                }
            }

            return false;
        }

        public async Task SendWeaponMagic2(Role pTarget = null)
        {
            Item item = null;

            if (UserPackage[Item.ItemPosition.RightHand] != null &&
                UserPackage[Item.ItemPosition.RightHand].Effect != Item.ItemEffect.None)
                item = UserPackage[Item.ItemPosition.RightHand];
            if (UserPackage[Item.ItemPosition.LeftHand] != null &&
                UserPackage[Item.ItemPosition.LeftHand].Effect != Item.ItemEffect.None)
                if (item != null && await Kernel.ChanceCalcAsync(50f) || item == null)
                    item = UserPackage[Item.ItemPosition.LeftHand];

            if (item != null)
            {
                switch (item.Effect)
                {
                    case Item.ItemEffect.Life:
                        {
                            if (!await Kernel.ChanceCalcAsync(15f))
                                return;
                            await AddAttributesAsync(ClientUpdateType.Hitpoints, 310);
                            var msg = new MsgMagicEffect
                            {
                                AttackerIdentity = Identity,
                                MagicIdentity = 1005
                            };
                            msg.Append(Identity, 310, false);
                            await BroadcastRoomMsgAsync(msg, true);
                            break;
                        }

                    case Item.ItemEffect.Mana:
                        {
                            if (!await Kernel.ChanceCalcAsync(17.5f))
                                return;
                            await AddAttributesAsync(ClientUpdateType.Mana, 310);
                            var msg = new MsgMagicEffect
                            {
                                AttackerIdentity = Identity,
                                MagicIdentity = 1195
                            };
                            msg.Append(Identity, 310, false);
                            await BroadcastRoomMsgAsync(msg, true);
                            break;
                        }

                    case Item.ItemEffect.Poison:
                        {
                            if (pTarget == null)
                                return;

                            if (!await Kernel.ChanceCalcAsync(5f))
                                return;

                            var msg = new MsgMagicEffect
                            {
                                AttackerIdentity = Identity,
                                MagicIdentity = 1320
                            };
                            msg.Append(pTarget.Identity, 210, true);
                            await BroadcastRoomMsgAsync(msg, true);

                            await pTarget.AttachStatus(this, StatusSet.POISONED, 310, POISONDAMAGE_INTERVAL, 20, 0);

                            var result = await Attack(pTarget);
                            int nTargetLifeLost = result.Damage;

                            await SendDamageMsgAsync(pTarget.Identity, nTargetLifeLost);

                            if (!pTarget.IsAlive)
                            {
                                int dwDieWay = 1;
                                if (nTargetLifeLost > pTarget.MaxLife / 3)
                                    dwDieWay = 2;

                                await Kill(pTarget, IsBowman ? 5 : (uint)dwDieWay);
                            }
                            break;
                        }
                }
            }
        }

        public async Task<bool> DecEquipmentDurability(bool bAttack, int hitByMagic, ushort useItemNum)
        {
            int nInc = -1 * useItemNum;

            for (Item.ItemPosition i = Item.ItemPosition.Headwear; i <= Item.ItemPosition.Crop; i++)
            {
                if (i == Item.ItemPosition.Garment || i == Item.ItemPosition.Gourd || i == Item.ItemPosition.Steed
                    || i == Item.ItemPosition.SteedArmor || i == Item.ItemPosition.LeftHandAccessory ||
                    i == Item.ItemPosition.RightHandAccessory)
                    continue;
                if (hitByMagic == 1)
                {
                    if (i == Item.ItemPosition.Ring
                        || i == Item.ItemPosition.RightHand
                        || i == Item.ItemPosition.LeftHand
                        || i == Item.ItemPosition.Boots)
                    {
                        if (!bAttack)
                            await AddEquipmentDurability(i, nInc);
                    }
                    else
                    {
                        if (bAttack)
                            await AddEquipmentDurability(i, nInc);
                    }
                }
                else
                {
                    if (i == Item.ItemPosition.Ring
                        || i == Item.ItemPosition.RightHand
                        || i == Item.ItemPosition.LeftHand
                        || i == Item.ItemPosition.Boots)
                    {
                        if (!bAttack)
                            await AddEquipmentDurability(i, -1);
                    }
                    else
                    {
                        if (bAttack)
                            await AddEquipmentDurability(i, nInc);
                    }
                }
            }

            return true;
        }

        public async Task AddEquipmentDurability(Item.ItemPosition pos, int nInc)
        {
            if (nInc >= 0)
                return;

            Item item = UserPackage[pos];
            if (item == null
                || !item.IsEquipment()
                || item.GetItemSubType() == 2100)
                return;

            ushort nOldDur = item.Durability;
            ushort nDurability = (ushort)Math.Max(0, item.Durability + nInc);

            if (nDurability < 100)
            {
                if (nDurability % 10 == 0)
                    await SendAsync(string.Format(Language.StrDamagedRepair, item.Itemtype.Name));
            }
            else if (nDurability < 200)
            {
                if (nDurability % 10 == 0)
                    await SendAsync(string.Format(Language.StrDurabilityRepair, item.Itemtype.Name));
            }

            item.Durability = nDurability;
            await item.SaveAsync();

            int noldDur = (int) Math.Floor(nOldDur / 100f);
            int nnewDur = (int) Math.Floor(nDurability / 100f);

            if (nDurability <= 0)
            {
                await SendAsync(new MsgItemInfo(item, MsgItemInfo.ItemMode.Update));
            }
            else if (noldDur != nnewDur)
            {
                await SendAsync(new MsgItemInfo(item, MsgItemInfo.ItemMode.Update));
            }
        }

        public bool SetAttackTarget(Role target)
        {
            if (target == null)
            {
                BattleSystem.ResetBattle();
                return false;
            }

            if (!target.IsAttackable(this))
            {
                BattleSystem.ResetBattle();
                return false;
            }

            if (target.IsWing && !IsWing && !IsBowman)
            {
                BattleSystem.ResetBattle();
                return false;
            }

            if (GetDistance(target) > GetAttackRange(target.SizeAddition))
            {
                BattleSystem.ResetBattle();
                return false;
            }
            return true;
        }

        public async Task AddSynWarScore(DynamicNpc npc, int score)
        {
            if (npc == null || score == 0)
                return;

            if (Syndicate == null || npc.OwnerIdentity == SyndicateIdentity)
                return;

            Syndicate target = Kernel.SyndicateManager.GetSyndicate((int) npc.OwnerIdentity);
            if (target != null && target.Money > 0)
            {
                int addProffer = (int) Math.Min(target.Money, Calculations.MulDiv(score, 1, 100));
                target.Money = (uint) Math.Max(0, target.Money - addProffer);
                await target.SaveAsync();
                await AwardMoney(addProffer);
                Syndicate.Money += (uint) addProffer;
                await Syndicate.SaveAsync();
            }

            npc.AddSynWarScore(Syndicate, score);
        }

        public async Task<int> GetInterAtkRate()
        {
            int nRate = USER_ATTACK_SPEED;
            int nRateR = 0, nRateL = 0;

            if (UserPackage[Item.ItemPosition.RightHand] != null)
                nRateR = UserPackage[Item.ItemPosition.RightHand].Itemtype.AtkSpeed;
            if (UserPackage[Item.ItemPosition.LeftHand] != null && !UserPackage[Item.ItemPosition.LeftHand].IsArrowSort())
                nRateL = UserPackage[Item.ItemPosition.LeftHand].Itemtype.AtkSpeed;

            if (nRateR > 0 && nRateL > 0)
                nRate = (nRateR + nRateL) / 2;
            else if (nRateR > 0)
                nRate = nRateR;
            else if (nRateL > 0)
                nRate = nRateL;

#if DEBUG
            if (QueryStatus(StatusSet.CYCLONE) != null)
            {
                nRate = Calculations.CutTrail(0,
                    Calculations.AdjustData(nRate, QueryStatus(StatusSet.CYCLONE).Power));
                if (IsPm())
                    await SendAsync($"attack speed+: {nRate}");
            }
#endif

            return Math.Max(400, nRate);
        }

        public override int AdjustWeaponDamage(int damage)
        {
            int type1 = 0, type2 = 0;
            if (UserPackage[Item.ItemPosition.RightHand] != null)
                type1 = UserPackage[Item.ItemPosition.RightHand].GetItemSubType();
            if (UserPackage[Item.ItemPosition.LeftHand] != null)
                type2 = UserPackage[Item.ItemPosition.LeftHand].GetItemSubType();

            if (type1 > 0 && WeaponSkill[(ushort)type1] != null &&
                WeaponSkill[(ushort)type1].Level > 12)
            {
                damage = (int)(damage * (1 + (20 - WeaponSkill[(ushort)type1].Level) / 100f));
            }
            else if (type2 > 0 && WeaponSkill[(ushort)type2] != null &&
                     WeaponSkill[(ushort)type2].Level > 12)
            {
                damage = (int)(damage * (1 + (20 - WeaponSkill[(ushort)type2].Level) / 100f));
            }

            return damage;
        }

        public override int GetAttackRange(int sizeAdd)
        {
            int nRange = 1, nRangeL = 0, nRangeR = 0;

            if (UserPackage[Item.ItemPosition.RightHand] != null && UserPackage[Item.ItemPosition.RightHand].IsWeapon())
                nRangeR = UserPackage[Item.ItemPosition.RightHand].AttackRange;
            if (UserPackage[Item.ItemPosition.LeftHand] != null && UserPackage[Item.ItemPosition.LeftHand].IsWeapon())
                nRangeL = UserPackage[Item.ItemPosition.LeftHand].AttackRange;

            if (nRangeR > 0 && nRangeL > 0)
                nRange = (nRangeR + nRangeL) / 2;
            else if (nRangeR > 0)
                nRange = nRangeR;
            else if (nRangeL > 0)
                nRange = nRangeL;

            nRange += (SizeAddition + sizeAdd + 1) / 2;

            return nRange + 1;
        }

        public override bool IsImmunity(Role target)
        {
            if (base.IsImmunity(target))
                return true;

            if (target is Character user)
            {
                switch (PkMode)
                {
                    case PkModeType.Capture:
                        return !user.IsEvil();
                    case PkModeType.Peace:
                        return true;
                    case PkModeType.FreePk:
                        if (Level >= 70 && user.Level < 70)
                            return true;
                        return false;
                    case PkModeType.Team:
                        if (IsFriend(user.Identity))
                            return false;
                        if (IsMate(user.Identity))
                            return false;
                        if (Syndicate?.QueryMember(user.SyndicateIdentity) != null)
                            return false;
                        if (Syndicate?.IsAlly(user.SyndicateIdentity) == true)
                            return false;
                        if (Team?.IsMember(user.Identity) == true)
                            return false;
                        return true;
                }
            }
            else if (target is Monster monster)
            {
                switch (PkMode)
                {
                    case PkModeType.Peace:
                        return false;
                    case PkModeType.Team:
                    case PkModeType.Capture:
                        if (monster.IsGuard() || monster.IsPkKiller())
                            return true;
                        return false;
                    case PkModeType.FreePk:
                        return false;
                }
            }
            else if (target is DynamicNpc dynaNpc)
            {
                return false;
            }
            
            return true;
        }

        public override bool IsAttackable(Role attacker)
        {
            return (!m_respawn.IsActive() || m_respawn.IsTimeOut()) && IsAlive && !Map.QueryRegion(RegionTypes.PkProtected, MapX, MapY);
        }

        public override async Task<(int Damage, InteractionEffect Effect)> Attack(Role target)
        {
            if (target == null)
                return (0, InteractionEffect.None);

            if (!target.IsEvil() && Map.IsDeadIsland() || (target is Monster mob && mob.IsGuard()))
                await SetCrimeStatus(15);

            return await BattleSystem.CalcPower(BattleSystem.MagicType.None, this, target);
        }

        public override async Task Kill(Role target, uint dieWay)
        {
            if (target == null)
                return;

            Character targetUser = target as Character;
            if (targetUser != null)
            {
                await BroadcastRoomMsgAsync(new MsgInteract
                {
                    Action = MsgInteractType.Kill,
                    SenderIdentity = Identity,
                    TargetIdentity = target.Identity,
                    PosX = target.MapX,
                    PosY = target.MapY,
                    Data = (int) dieWay
                }, true);

                await ProcessPk(targetUser);
            }
            else if (target is Monster monster)
            {
                await AddXp(1);

                if (QueryStatus(StatusSet.CYCLONE) != null || QueryStatus(StatusSet.SUPERMAN) != null)
                {
                    KoCount += 1;
                    var status = QueryStatus(StatusSet.CYCLONE) ?? QueryStatus(StatusSet.SUPERMAN);
                    status?.IncTime(700, 30000);
                }
            }

            await target.BeKill(this);
        }

        public override async Task<bool> BeAttack(BattleSystem.MagicType magic, Role attacker, int power,
            bool bReflectEnable)
        {
            if (attacker == null)
                return false;

            if (PreviousProfession == 25 || FirstProfession == 25 && bReflectEnable && await Kernel.ChanceCalcAsync(15d))
            {
                power = Math.Max(1700, power);
                await attacker.BeAttack(magic, this, power, false);
                await BroadcastRoomMsgAsync(new MsgInteract
                {
                    Action = MsgInteractType.ReflectMagic,
                    Data = power,
                    PosX = MapX,
                    PosY = MapY,
                    SenderIdentity = Identity,
                    TargetIdentity = attacker.Identity
                }, true);
                return true;
            }

            if (power > 0)
            {
                await AddAttributesAsync(ClientUpdateType.Hitpoints, power * -1);
            }

            if (!IsAlive)
            {
                await BeKill(this);
            } 
            else if (Action == EntityAction.Sit)
                await SetAttributesAsync(ClientUpdateType.Stamina, Energy / 2);

            return true;
        }

        public override async Task BeKill(Role attacker)
        {
            if (QueryStatus(StatusSet.GHOST) != null)
                return;

            BattleSystem.ResetBattle();

            TransformationMesh = 0;
            Transformation = null;
            m_transformation.Clear();

            await SetAttributesAsync(ClientUpdateType.Mesh, Mesh);

            await DetachStatus(StatusSet.BLUE_NAME);
            await DetachAllStatus();
            await AttachStatus(this, StatusSet.DEAD, 0, int.MaxValue, 0, 0);
            await AttachStatus(this, StatusSet.GHOST, 0, int.MaxValue, 0, 0);

            m_ghost.Startup(4);

            uint idMap = 0;
            Point posTarget = new Point();
            if (Map.GetRebornMap(ref idMap, ref posTarget))
                await SavePositionAsync(idMap, (ushort) posTarget.X, (ushort) posTarget.Y);

            if (Map.IsPkField() || Map.IsSynMap())
            {
                if (Map.IsSynMap() && !Map.IsWarTime())
                    await SavePositionAsync(1002, 430, 378);
                return;
            }

            if (Map.IsPrisionMap())
            {
                if (!Map.IsDeadIsland())
                {
                    int nChance = Math.Min(90, 20 + PkPoints / 2);
                    await UserPackage.RandDropItemAsync(3, nChance);
                }
                return;
            }

            if (attacker == null)
                return;

            if (!Map.IsDeadIsland())
            {
                // todo enemy

                int nChance = 0;
                if (PkPoints < 30)
                    nChance = 10 + await Kernel.NextAsync(40);
                else if (PkPoints < 100)
                    nChance = 50 + await Kernel.NextAsync(50);
                else
                    nChance = 100;

                int nItems = UserPackage.InventoryCount;
                int nDropItem = nItems * nChance / 100;

                await UserPackage.RandDropItemAsync(nDropItem);

                if (attacker.Identity != Identity && attacker is Character targetUser)
                {
                    await CreateEnemy(targetUser);

                    float nLossPercent;
                    if (PkPoints < 30)
                        nLossPercent = 0.01f;
                    else if (PkPoints < 100)
                        nLossPercent = 0.02f;
                    else nLossPercent = 0.03f;
                    long nLevExp = (long)Experience;
                    long nLostExp = (long)(nLevExp * nLossPercent);

                    if (nLostExp > 0)
                    {
                        await AddAttributesAsync(ClientUpdateType.Experience, nLostExp * -1);
                        await attacker.AddAttributesAsync(ClientUpdateType.Experience, nLostExp / 3);
                    }

                    if (PkPoints >= 300)
                    {
                        await UserPackage.RandDropEquipmentAsync(targetUser);
                        await UserPackage.RandDropEquipmentAsync(targetUser);
                    }
                    else if (PkPoints >= 100)
                    {
                        await UserPackage.RandDropEquipmentAsync(targetUser);
                    }
                    else if (PkPoints >= 30 && await Kernel.ChanceCalcAsync(30))
                    {
                        await UserPackage.RandDropEquipmentAsync(targetUser);
                    }

                    if (PkPoints >= 100)
                    {
                        await SavePositionAsync(6000, 31, 72);
                        await FlyMap(6000, 31, 72);
                        await Kernel.RoleManager.BroadcastMsgAsync(
                            string.Format(Language.StrGoToJail, attacker.Name, Name), MsgTalk.TalkChannel.Talk,
                            Color.White);
                    }
                }
            }
            else if (attacker is Character atkUser && Map.IsDeadIsland())
            {
                await CreateEnemy(atkUser);
            }
            else if (attacker is Monster monster)
            {
                if (monster.IsGuard() && PkPoints > 99)
                {
                    await SavePositionAsync(6000, 31, 72);
                    await FlyMap(6000, 31, 72);
                    await Kernel.RoleManager.BroadcastMsgAsync(
                        string.Format(Language.StrGoToJail, attacker.Name, Name), MsgTalk.TalkChannel.Talk,
                        Color.White);
                }
            }
        }

        public async Task SendGemEffect()
        {
            var setGem = new List<Item.SocketGem>();

            for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos < Item.ItemPosition.EquipmentEnd; pos++)
            {
                Item item = UserPackage[pos];
                if (item == null)
                    continue;

                setGem.Add(item.SocketOne);
                if (item.SocketTwo != Item.SocketGem.NoSocket)
                    setGem.Add(item.SocketTwo);
            }

            int nGems = setGem.Count;
            if (nGems <= 0)
                return;

            string strEffect = "";
            switch (setGem[await Kernel.NextAsync(0, nGems)])
            {
                case Item.SocketGem.SuperPhoenixGem:
                    strEffect = "phoenix";
                    break;
                case Item.SocketGem.SuperDragonGem:
                    strEffect = "goldendragon";
                    break;
                case Item.SocketGem.SuperFuryGem:
                    strEffect = "fastflash";
                    break;
                case Item.SocketGem.SuperRainbowGem:
                    strEffect = "rainbow";
                    break;
                case Item.SocketGem.SuperKylinGem:
                    strEffect = "goldenkylin";
                    break;
                case Item.SocketGem.SuperVioletGem:
                    strEffect = "purpleray";
                    break;
                case Item.SocketGem.SuperMoonGem:
                    strEffect = "moon";
                    break;
            }

            await SendEffectAsync(strEffect, true);
        }

        #endregion

        #region Revive

        public bool CanRevive()
        {
            return !IsAlive && m_tRevive.IsTimeOut();
        }

        public async Task Reborn(bool chgMap, bool isSpell = false)
        {
            if (IsAlive || !CanRevive() && !isSpell)
            {
                if (QueryStatus(StatusSet.GHOST) != null)
                {
                    await DetachStatus(StatusSet.GHOST);
                }

                if (QueryStatus(StatusSet.DEAD) != null)
                {
                    await DetachStatus(StatusSet.DEAD);
                }

                if (TransformationMesh == 98 || TransformationMesh == 99)
                    await ClearTransformation();
                return;
            }

            BattleSystem.ResetBattle();
            
            await DetachStatus(StatusSet.GHOST);
            await DetachStatus(StatusSet.DEAD);
            
            await ClearTransformation();

            await SetAttributesAsync(ClientUpdateType.Stamina, DEFAULT_USER_ENERGY);
            await SetAttributesAsync(ClientUpdateType.Hitpoints, MaxLife);
            await SetAttributesAsync(ClientUpdateType.Mana, MaxMana);
            await SetXp(0);

            if (chgMap || !IsBlessed && !isSpell)
            {
                await FlyMap(m_dbObject.MapID, m_dbObject.X, m_dbObject.Y);
            }
            else
            {
                if (!isSpell && (Map.IsPrisionMap()
                                 || Map.IsPkField()
                                 || Map.IsPkGameMap()
                                 || Map.IsSynMap()))
                {
                    await FlyMap(m_dbObject.MapID, m_dbObject.X, m_dbObject.Y);
                }
                else
                {
                    await FlyMap(m_idMap, m_posX, m_posY);
                }
            }

            m_respawn.Startup(CHGMAP_LOCK_SECS);
        }

        #endregion

        #region Rebirth

        public async Task<bool> RebirthAsync(ushort prof, ushort look)
        {
            DbRebirth data = Kernel.RoleManager.GetRebirth(Profession, prof, Metempsychosis + 1);

            if (data == null)
            {
                if (IsPm())
                    await SendAsync($"No rebirth set for {Profession} -> {prof}");
                return false;
            }

            if (Level < data.NeedLevel)
            {
                await SendAsync(Language.StrNotEnoughLevel);
                return false;
            }

            if (Level >= 130)
            {
                DbLevelExperience levExp = Kernel.RoleManager.GetLevelExperience(Level);
                if (levExp != null)
                {
                    float fExp = Experience / (float)levExp.Exp;
                    uint metLev = (uint)(Level * 10000 + fExp * 1000);
                    if (metLev > m_dbObject.MeteLevel)
                        m_dbObject.MeteLevel = metLev;
                }
                else if (Level >= MAX_UPLEV)
                    m_dbObject.MeteLevel = MAX_UPLEV * 10000;
            }

            int oldProf = Profession;
            await ResetUserAttributesAsync(Metempsychosis, prof, look, data.NewLevel);

            for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
            {
                UserPackage[pos]?.DegradeItem(false);
            }

            var removeSkills = Kernel.RoleManager.GetMagictypeOp(MagicTypeOp.MagictypeOperation.RemoveOnRebirth, oldProf/10, prof/10)?.Magics;
            var resetSkills = Kernel.RoleManager.GetMagictypeOp(MagicTypeOp.MagictypeOperation.ResetOnRebirth, oldProf / 10, prof/10)?.Magics;
            var learnSkills = Kernel.RoleManager.GetMagictypeOp(MagicTypeOp.MagictypeOperation.LearnAfterRebirth, oldProf / 10, prof/10)?.Magics;

            if (removeSkills != null)
            {
                foreach (var skill in removeSkills)
                {
                    await MagicData.UnlearnMagic(skill, true);
                }
            }

            if (resetSkills != null)
            {
                foreach (var skill in resetSkills)
                {
                    await MagicData.UnlearnMagic(skill, false);
                }
            }

            if (learnSkills != null)
            {
                foreach (var skill in learnSkills)
                {
                    await MagicData.Create(skill, 0);
                }
            }

            if (UserPackage[Item.ItemPosition.LeftHand]?.IsArrowSort() == false)
                await UserPackage.UnequipAsync(Item.ItemPosition.LeftHand);

            if (UserPackage[Item.ItemPosition.RightHand]?.IsBow() == true && ProfessionSort != 4)
                await UserPackage.UnequipAsync(Item.ItemPosition.RightHand);

            return true;
        }

        public async Task ResetUserAttributesAsync(byte mete, ushort newProf, ushort newLook, int newLev)
        {
            if (newProf == 0) newProf = (ushort) (Profession / 10 * 10 + 1);
            byte prof = (byte) (newProf > 100 ? 10 : newProf / 10);

            int force = 0, speed = 0, health = 0, soul = 0;
            DbPointAllot pointAllot = Kernel.RoleManager.GetPointAllot(prof, 1);
            if (pointAllot != null)
            {
                force = pointAllot.Strength;
                speed = pointAllot.Agility;
                health = pointAllot.Vitality;
                soul = pointAllot.Spirit;
            }
            else if (prof == 1)
            {
                force = 5;
                speed = 2;
                health = 3;
                soul = 0;
            }
            else if (prof == 2)
            {
                force = 5;
                speed = 2;
                health = 3;
                soul = 0;
            }
            else if (prof == 4)
            {
                force = 2;
                speed = 7;
                health = 1;
                soul = 0;
            }
            else if (prof == 10)
            {
                force = 0;
                speed = 2;
                health = 3;
                soul = 5;
            }
            else
            {
                force = 5;
                speed = 2;
                health = 3;
                soul = 0;
            }

            AutoAllot = false;

            int newAttrib = (GetRebirthAddPoint(Profession, Level, mete) + (newLev * 3));
            await SetAttributesAsync(ClientUpdateType.Atributes, newAttrib);
            await SetAttributesAsync(ClientUpdateType.Strength, force);
            await SetAttributesAsync(ClientUpdateType.Agility, speed);
            await SetAttributesAsync(ClientUpdateType.Vitality, health);
            await SetAttributesAsync(ClientUpdateType.Spirit, soul);
            await SetAttributesAsync(ClientUpdateType.Hitpoints, MaxLife);
            await SetAttributesAsync(ClientUpdateType.Mana, MaxMana);
            await SetAttributesAsync(ClientUpdateType.Stamina, MaxEnergy);
            await SetAttributesAsync(ClientUpdateType.XpCircle, 0);

            if (newLook > 0 && newLook != Mesh % 10)
                await SetAttributesAsync(ClientUpdateType.Mesh, Mesh);

            await SetAttributesAsync(ClientUpdateType.Level, newLev);
            await SetAttributesAsync(ClientUpdateType.Experience, 0);

            if (mete == 0)
            {
                FirstProfession = Profession;
                mete++;
            }
            else if (mete == 1)
            {
                PreviousProfession = Profession;
                mete++;
            }
            else
            {
                FirstProfession = PreviousProfession;
                PreviousProfession = Profession;
            }


            await SetAttributesAsync(ClientUpdateType.Class, newProf);
            await SetAttributesAsync(ClientUpdateType.Reborn, mete);
            await SaveAsync();
        }

        public int GetRebirthAddPoint(int oldProf, int oldLev, int metempsychosis)
        {
            int points = 0;

            if (metempsychosis == 0)
            {
                if (oldProf == HIGHEST_WATER_WIZARD_PROF)
                {
                    points += Math.Min((1 + (oldLev - 110) / 2) * ((oldLev - 110) / 2) / 2, 55);
                }
                else
                {
                    points += Math.Min((1 + (oldLev - 120)) * (oldLev - 120) / 2, 55);
                }
            }
            else
            {
                if (oldProf == HIGHEST_WATER_WIZARD_PROF)
                    points += 52 + Math.Min((1 + (oldLev - 110) / 2) * ((oldLev - 110) / 2) / 2, 55);
                else
                    points += 52 + Math.Min((1 + (oldLev - 120)) * (oldLev - 120) / 2, 55);
            }

            return points;
        }

        public async Task<bool> UnlearnAllSkill()
        {
            return await WeaponSkill.UnearnAll();
        }

        #endregion

        #region Bonus

        public async Task<bool> DoBonusAsync()
        {
            if (!UserPackage.IsPackSpare(10))
            {
                await SendAsync(string.Format(Language.StrNotEnoughSpaceN, 10));
                return false;
            }

            DbBonus bonus = await BonusRepository.GetAsync(m_dbObject.AccountIdentity);
            if (bonus == null || bonus.Flag != 0 || bonus.Time != null)
            {
                await SendAsync(Language.StrNoBonus);
                return false;
            }

            bonus.Flag = 1;
            bonus.Time = DateTime.Now;
            await BaseRepository.SaveAsync(bonus);
            if (!await GameAction.ExecuteActionAsync(bonus.Action, this, null, null, ""))
            {
                await Log.GmLog("bonus_error", $"{bonus.Identity},{bonus.AccountIdentity},{Identity},{bonus.Action}");
                return false;
            }

            await Log.GmLog("bonus", $"{bonus.Identity},{bonus.AccountIdentity},{Identity},{bonus.Action}");
            return true;
        }

        public async Task<int> BonusCount()
        {
            return await BonusRepository.CountAsync(m_dbObject.AccountIdentity);
        }

        #endregion

        #region Statistic

        public UserStatistic Statistic { get; }

        public long Iterator = -1;
        public long[] VarData = new long[MAX_VAR_AMOUNT];
        public string[] VarString = new string[MAX_VAR_AMOUNT];

        #endregion

        #region Task Detail

        public TaskDetail TaskDetail { get; }

        #endregion

        #region Game Action

        private List<uint> m_setTaskId = new List<uint>();

        public uint InteractingItem { get; set; }
        public uint InteractingNpc { get; set; }

        private List<QueuedAction> m_queuedActions = new List<QueuedAction>();

        public void AddActionToQueue(QueuedAction action)
        {
            m_queuedActions.Add(action);
        }

        public bool CheckItem(DbTask task)
        {
            if (task.Itemname1.Length > 0)
            {
                if (UserPackage[task.Itemname1] == null)
                    return false;

                if (task.Itemname2.Length > 0)
                {
                    if (UserPackage[task.Itemname2] == null)
                        return false;
                }
            }

            return true;
        }

        public void CancelInteraction()
        {
            m_setTaskId.Clear();
            InteractingItem = 0;
            InteractingNpc = 0;
        }

        public byte PushTaskId(uint idTask)
        {
            if (idTask != 0 && m_setTaskId.Count < MAX_MENUTASKSIZE)
            {
                m_setTaskId.Add(idTask);
                return (byte)m_setTaskId.Count;
            }

            return 0;
        }

        public void ClearTaskId()
        {
            m_setTaskId.Clear();
        }
        public uint GetTaskId(int idx)
        {
            return idx > 0 && idx <= m_setTaskId.Count ? m_setTaskId[idx - 1] : 0u;
        }

        public async Task<bool> TestTask(DbTask task)
        {
            if (task == null) return false;

            try
            {
                if (!CheckItem(task))
                    return false;

                if (Silvers < task.Money)
                    return false;

                if (task.Profession != 0 && Profession != task.Profession)
                    return false;

                if (task.Sex != 0 && task.Sex != 999 && task.Sex != Gender)
                    return false;

                if (PkPoints < task.MinPk || PkPoints > task.MaxPk)
                    return false;

                if (task.Marriage >= 0)
                {
                    if (task.Marriage == 0 && MateIdentity != 0)
                        return false;
                    if (task.Marriage == 1 && MateIdentity == 0)
                        return false;
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Test task error");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                return false;
            }
            return true;
        }

        public async Task AddTaskMask(int idx)
        {
            if (idx < 0 || idx >= 32)
                return;

            m_dbObject.TaskMask |= (1u << idx);
            await SaveAsync();
        }

        public async Task ClearTaskMask(int idx)
        {
            if (idx < 0 || idx >= 32)
                return;

            m_dbObject.TaskMask &= ~(1u << idx);
            await SaveAsync();
        }

        public bool CheckTaskMask(int idx)
        {
            if (idx < 0 || idx >= 32)
                return false;
            return (m_dbObject.TaskMask & (1u << idx)) != 0;
        }

        #endregion

        #region Home

        public uint HomeIdentity
        {
            get => m_dbObject?.HomeIdentity ?? 0u;
            set => m_dbObject.HomeIdentity = value;
        }

        #endregion

        #region Marriage

        public bool IsMate(Character user)
        {
            return user.Identity == MateIdentity;
        }

        public bool IsMate(uint idMate)
        {
            return idMate == MateIdentity;
        }

        #endregion

        #region Requests

        public void SetRequest(RequestType type, uint target)
        {
            m_dicRequests.TryRemove(type, out _);
            if (target == 0)
                return;

            m_dicRequests.TryAdd(type, target);
        }

        public uint QueryRequest(RequestType type)
        {
            return m_dicRequests.TryGetValue(type, out var value) ? value : 0;
        }

        public uint PopRequest(RequestType type)
        {
            return m_dicRequests.TryRemove(type, out var value) ? value : 0;
        }

        #endregion

        #region Friend

        private ConcurrentDictionary<uint, Friend> m_dicFriends = new ConcurrentDictionary<uint, Friend>();

        public int FriendAmount => m_dicFriends.Count;
        
        public int MaxFriendAmount => 50;

        public bool AddFriend(Friend friend)
        {
            return m_dicFriends.TryAdd(friend.Identity, friend);
        }

        public async Task<bool> CreateFriend(Character target)
        {
            if (IsFriend(target.Identity))
                return false;

            Friend friend = new Friend(this);
            if (!await friend.CreateAsync(target))
                return false;

            Friend targetFriend = new Friend(target);
            if (!await targetFriend.CreateAsync(this))
                return false;

            await friend.SaveAsync();
            await targetFriend.SaveAsync();
            await friend.SendAsync();
            await targetFriend.SendAsync();

            AddFriend(friend);
            target.AddFriend(targetFriend);

            await BroadcastRoomMsgAsync(string.Format(Language.StrMakeFriend, Name, target.Name));
            return true;
        }

        public bool IsFriend(uint idTarget)
        {
            return m_dicFriends.ContainsKey(idTarget);
        }

        public Friend GetFriend(uint idTarget)
        {
            return m_dicFriends.TryGetValue(idTarget, out var friend) ? friend : null;
        }

        public async Task<bool> DeleteFriend(uint idTarget, bool notify = false)
        {
            if (!IsFriend(idTarget) || !m_dicFriends.TryRemove(idTarget, out var target))
                return false;
            
            if (target.Online)
            {
                await target.User.DeleteFriend(Identity);
            }
            else
            {
                DbFriend targetFriend = await FriendRepository.GetAsync(Identity, idTarget);
                await using ServerDbContext ctx = new ServerDbContext();
                ctx.Remove(targetFriend);
                await ctx.SaveChangesAsync();
            }

            await target.DeleteAsync();

            await SendAsync(new MsgFriend
            {
                Identity = target.Identity,
                Name = target.Name,
                Action = MsgFriend.MsgFriendAction.RemoveFriend,
                Online = target.Online
            });

            if (notify)
                await BroadcastRoomMsgAsync(string.Format(Language.StrBreakFriend, Name, target.Name));
            return true;
        }

        public async Task SendAllFriendAsync()
        {
            foreach (var friend in m_dicFriends.Values)
            {
                await friend.SendAsync();
                if (friend.Online)
                {
                    await friend.User.SendAsync(new MsgFriend
                    {
                        Identity = Identity,
                        Name = Name,
                        Action = MsgFriend.MsgFriendAction.SetOnlineFriend,
                        Online = true
                    });
                }
            }
        }

        public async Task NotifyOfflineFriendAsync()
        {
            foreach (var friend in m_dicFriends.Values)
            {
                if (friend.Online)
                {
                    await friend.User.SendAsync(new MsgFriend
                    {
                        Identity = Identity,
                        Name = Name,
                        Action = MsgFriend.MsgFriendAction.SetOfflineFriend,
                        Online = true
                    });
                }
            }
        }

        public async Task SendToFriendsAsync(IPacket msg)
        {
            foreach (var friend in m_dicFriends.Values.Where(x => x.Online))
                await friend.User.SendAsync(msg);
        }

        #endregion

        #region Enemy

        private ConcurrentDictionary<uint, Enemy> m_dicEnemies = new ConcurrentDictionary<uint, Enemy>();

        public bool AddEnemy(Enemy friend)
        {
            return m_dicEnemies.TryAdd(friend.Identity, friend);
        }

        public async Task<bool> CreateEnemy(Character target)
        {
            if (IsEnemy(target.Identity))
                return false;

            Enemy enemy = new Enemy(this);
            if (!await enemy.CreateAsync(target))
                return false;

            await enemy.SaveAsync();
            await enemy.SendAsync();
            AddEnemy(enemy);
            return true;
        }

        public bool IsEnemy(uint idTarget)
        {
            return m_dicEnemies.ContainsKey(idTarget);
        }

        public Enemy GetEnemy(uint idTarget)
        {
            return m_dicEnemies.TryGetValue(idTarget, out var friend) ? friend : null;
        }

        public async Task<bool> DeleteEnemy(uint idTarget)
        {
            if (!IsFriend(idTarget) || !m_dicEnemies.TryRemove(idTarget, out var target))
                return false;

            await target.DeleteAsync();

            await SendAsync(new MsgFriend
            {
                Identity = target.Identity,
                Name = target.Name,
                Action = MsgFriend.MsgFriendAction.RemoveEnemy,
                Online = true
            });
            return true;
        }

        public async Task SendAllEnemiesAsync()
        {
            foreach (var enemy in m_dicEnemies.Values)
            {
                await enemy.SendAsync();
            }

            foreach (var enemy in await EnemyRepository.GetOwnEnemyAsync(Identity))
            {
                Character user = Kernel.RoleManager.GetUser(enemy.UserIdentity);
                if (user != null)
                    await user.SendAsync(new MsgFriend
                    {
                        Identity = Identity,
                        Name = Name,
                        Action = MsgFriend.MsgFriendAction.SetOnlineEnemy,
                        Online = true
                    });
            }
        }

        #endregion

        #region Trade Partner

        private Dictionary<uint, TradePartner> m_tradePartners = new Dictionary<uint, TradePartner>();

        public void AddTradePartner(TradePartner partner)
        {
            m_tradePartners.TryAdd(partner.Identity, partner);
        }

        public void RemoveTradePartner(uint idTarget)
        {
            if (m_tradePartners.ContainsKey(idTarget))
                m_tradePartners.Remove(idTarget);
        }

        public async Task<bool> CreateTradePartnerAsync(Character target)
        {
            if (IsTradePartner(target.Identity) || target.IsTradePartner(Identity))
            {
                await SendAsync(Language.StrTradeBuddyAlreadyAdded);
                return false;
            }

            DbBusiness business = new DbBusiness
            {
                User = GetDatabaseObject(),
                Business = target.GetDatabaseObject(),
                Date = DateTime.Now.AddDays(3)
            };

            if (!await BaseRepository.SaveAsync(business))
            {
                await SendAsync(Language.StrTradeBuddySomethingWrong);
                return false;
            }

            TradePartner me;
            TradePartner targetTp;
            AddTradePartner(me = new TradePartner(this, business));
            target.AddTradePartner(targetTp = new TradePartner(target, business));

            await me.SendAsync();
            await targetTp.SendAsync();

            await BroadcastRoomMsgAsync(string.Format(Language.StrTradeBuddyAnnouncePartnership, Name, target.Name));
            return true;
        }

        public async Task<bool> DeleteTradePartnerAsync(uint idTarget)
        {
            if (!IsTradePartner(idTarget))
                return false;

            TradePartner partner = GetTradePartner(idTarget);
            if (partner == null)
                return false;

            await partner.SendRemoveAsync();
            RemoveTradePartner(idTarget);
            await SendAsync(string.Format(Language.StrTradeBuddyBrokePartnership1, partner.Name));

            var delete = partner.DeleteAsync();
            Character target = Kernel.RoleManager.GetUser(idTarget);
            if (target != null)
            {
                partner = target.GetTradePartner(Identity);
                if (partner != null)
                {
                    await partner.SendRemoveAsync();
                    target.RemoveTradePartner(Identity);
                }

                await target.SendAsync(string.Format(Language.StrTradeBuddyBrokePartnership0, Name));
            }

            await delete;
            return true;
        }

        public async Task LoadTradePartnerAsync()
        {
            var tps = await DbBusiness.GetAsync(Identity);
            foreach (var tp in tps)
            {
                var db = new TradePartner(this, tp);
                AddTradePartner(db);
                await db.SendAsync();
            }
        }

        public TradePartner GetTradePartner(uint target)
        {
            return m_tradePartners.TryGetValue(target, out var result) ? result : null;
        }

        public bool IsTradePartner(uint target)
        {
            return m_tradePartners.ContainsKey(target);
        }

        public bool IsValidTradePartner(uint target)
        {
            return m_tradePartners.ContainsKey(target) && m_tradePartners[target].IsValid();
        }

        #endregion

        #region Syndicate

        public Syndicate Syndicate { get; set; }
        public SyndicateMember SyndicateMember => Syndicate?.QueryMember(Identity);
        public ushort SyndicateIdentity => Syndicate?.Identity ?? 0;
        public string SyndicateName => Syndicate?.Name ?? Language.StrNone;
        public SyndicateMember.SyndicateRank SyndicateRank => SyndicateMember?.Rank ?? SyndicateMember.SyndicateRank.None;

        public async Task<bool> CreateSyndicateAsync(string name, int price = 1000000)
        {
            if (Syndicate != null)
            {
                await SendAsync(Language.StrSynAlreadyJoined);
                return false;
            }

            // todo check string and name

            if (Kernel.SyndicateManager.GetSyndicate(name) != null)
            {
                await SendAsync(Language.StrSynNameInUse);
                return false;
            }

            if (!await SpendMoney(price))
            {
                await SendAsync(Language.StrNotEnoughMoney);
                return false;
            }

            Syndicate = new Syndicate();
            if (!await Syndicate.CreateAsync(name, price, this))
            {
                Syndicate = null;
                await AwardMoney(price);
                return false;
            }

            if (!Kernel.SyndicateManager.AddSyndicate(Syndicate))
            {
                await Syndicate.DeleteAsync();
                Syndicate = null;
                await AwardMoney(price);
                return false;
            }
            
            await Kernel.RoleManager.BroadcastMsgAsync(string.Format(Language.StrSynCreate, Name, name), MsgTalk.TalkChannel.Talk, Color.White);
            await SendSyndicateAsync();
            await Screen.SynchroScreenAsync();
            await Syndicate.BroadcastNameAsync();
            return true;
        }

        public async Task<bool> DisbandSyndicateAsync()
        {
            if (SyndicateIdentity == 0)
                return false;

            if (Syndicate.Leader.UserIdentity != Identity)
                return false;

            if (Syndicate.MemberCount > 1)
            {
                await SendAsync(Language.StrSynNoDisband);
                return false;
            }
            
            return await Syndicate.DisbandAsync(this);
        }

        public async Task SendSyndicateAsync()
        {
            if (Syndicate != null)
            {
                await SendAsync(new MsgSyndicateAttributeInfo
                {
                    Identity = SyndicateIdentity,
                    Rank = SyndicateRank,
                    MemberAmount = Syndicate.MemberCount,
                    Funds = Syndicate.Money,
                    PlayerDonation = SyndicateMember.Donation,
                    LeaderName = Syndicate.Leader.UserName
                });
                await SendAsync(Syndicate.Announce, MsgTalk.TalkChannel.Announce);
                await Syndicate.SendAsync(this);
            }
            else
            {
                await SendAsync(new MsgSyndicateAttributeInfo
                {
                    Rank = SyndicateMember.SyndicateRank.None
                });
            }
        }

        #endregion

        #region User Secondary Password

        public ulong SecondaryPassword
        {
            get => m_dbObject.LockKey;
            set => m_dbObject.LockKey = value;
        }

        public bool IsUnlocked()
        {
            return SecondaryPassword == 0 || VarData[0] != 0;
        }

        public void UnlockSecondaryPassword()
        {
            VarData[0] = 1;
        }

        public bool CanUnlock2ndPassword()
        {
            return VarData[1] <= 2;
        }

        public void Increment2ndPasswordAttempts()
        {
            VarData[1] += 1;
        }

        public async Task SendSecondaryPasswordInterface()
        {
            await GameAction.ExecuteActionAsync(100, this, null, null, string.Empty);
        }

        #endregion

        #region Administration

        public bool IsPm()
        {
            return Name.Contains("[PM]");
        }

        public bool IsGm()
        {
            return IsPm() || Name.Contains("[GM]");
        }

#endregion

        #region Screen

        public Screen Screen { get; }

        public async Task BroadcastRoomMsgAsync(string message, MsgTalk.TalkChannel channel = MsgTalk.TalkChannel.TopLeft, Color? color = null, bool self = true)
        {
            await BroadcastRoomMsgAsync(new MsgTalk(Identity, channel, color ?? Color.Red, message), self);
        }

        public override async Task BroadcastRoomMsgAsync(IPacket msg, bool self)
        {
            await Screen.BroadcastRoomMsgAsync(msg, self);
        }

        #endregion

        #region Map and Position

        public override GameMap Map { get; protected set; }

        /// <summary>
        /// The current map identity for the role.
        /// </summary>
        public override uint MapIdentity
        {
            get => m_idMap;
            set => m_idMap = value;
        }
        /// <summary>
        /// Current X position of the user in the map.
        /// </summary>
        public override ushort MapX
        {
            get => m_posX;
            set => m_posX = value;
        }
        /// <summary>
        /// Current Y position of the user in the map.
        /// </summary>
        public override ushort MapY
        {
            get => m_posY;
            set => m_posY = value;
        }

        public uint RecordMapIdentity
        {
            get => m_dbObject.MapID;
            set => m_dbObject.MapID = value;
        }

        public ushort RecordMapX
        {
            get => m_dbObject.X;
            set => m_dbObject.X = value;
        }

        public ushort RecordMapY
        {
            get => m_dbObject.Y;
            set => m_dbObject.Y = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task EnterMap()
        {
            Map = Kernel.MapManager.GetMap(m_idMap);
            if (Map != null)
            {
                await Map.AddAsync(this);
                await Map.SendMapInfoAsync(this);
                await Screen.SynchroScreenAsync();
            }
            else
            {
                await Log.WriteLog(LogLevel.Error, $"Invalid map {m_idMap} for user {Identity} {Name}");
                m_socket?.Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task LeaveMap()
        {
            BattleSystem.ResetBattle();
            await MagicData.AbortMagic(false);
            StopMining();
            
            if (Map != null)
                await Map.RemoveAsync(Identity);

            await Screen.ClearAsync();
        }

        public override Task ProcessOnMove()
        {
            StopMining();
            return base.ProcessOnMove();
        }

        public override Task ProcessOnAttack()
        {
            StopMining();
            return base.ProcessOnAttack();
        }

        public async Task SavePositionAsync()
        {
            if (!Map.IsRecordDisable())
            {
                m_dbObject.X = m_posX;
                m_dbObject.Y = m_posY;
                m_dbObject.MapID = m_idMap;
                await SaveAsync();
            }
        }

        public async Task SavePositionAsync(uint idMap, ushort x, ushort y)
        {
            if (!Map.IsRecordDisable())
            {
                m_dbObject.X = x;
                m_dbObject.Y = y;
                m_dbObject.MapID = idMap;
                await SaveAsync();
            }
        }

        public async Task<bool> FlyMap(uint idMap, int x, int y)
        {
            if (Map == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"FlyMap user not in map");
                return false;
            }

            if (idMap == 0)
                idMap = MapIdentity;

            GameMap newMap = Kernel.MapManager.GetMap(idMap);
            if (newMap == null || !newMap.IsValidPoint(x, y))
            {
                await Log.WriteLog(LogLevel.Warning, $"FlyMap user fly invalid position {idMap}[{x},{y}]");
                return false;
            }

            try
            {
                await LeaveMap();

                m_idMap = newMap.Identity;
                MapX = (ushort) x;
                MapY = (ushort) y;

                await SendAsync(new MsgAction
                {
                    Identity = Identity,
                    Command = newMap.MapDoc,
                    ArgumentX = MapX,
                    ArgumentY = MapY,
                    Action = MsgAction.ActionType.MapTeleport,
                    Direction = (ushort) Direction
                });

                await EnterMap();
            }
            catch
            {
                await Log.WriteLog(LogLevel.Error, "FlyMap error");
            }

            return true;
        }

        public Role QueryRole(uint idRole)
        {
            return Map.QueryAroundRole(this, idRole);
        }

        #endregion

        #region Movement

        public async Task<bool> SynPosition(ushort x, ushort y, int nMaxDislocation)
        {
            if (nMaxDislocation <= 0 || x == 0 && y == 0) // ignore in this condition
                return true;

            int nDislocation = GetDistance(x, y);
            if (nDislocation >= nMaxDislocation)
                return false;

            if (nDislocation <= 0)
                return true;

            if (IsGm())
                await SendAsync($"syn move: ({MapX},{MapY})->({x},{y})", MsgTalk.TalkChannel.Talk, Color.Red);

            if (!Map.IsValidPoint(x, y))
                return false;

            await ProcessOnMove();
            await JumpPosAsync(x, y);
            await Screen.BroadcastRoomMsgAsync(new MsgAction
            {
                Identity = Identity,
                Action = MsgAction.ActionType.Kickback,
                ArgumentX = x,
                ArgumentY = y,
                Command = (uint)((y << 16) | x),
                Direction = (ushort)Direction,
            });
            
            return true;
        }

        public async Task KickbackAsync()
        {
            await SendAsync(new MsgAction
            {
                Identity = Identity,
                ArgumentX = MapX,
                ArgumentY = MapY,
                Command = (uint) ((MapY << 16) | MapX),
                Direction = (ushort)Direction,
                Action = MsgAction.ActionType.Kickback
            });
        }

        #endregion

        #region Jar

        public async Task AddJarKillsAsync(int stcType)
        {
            Item jar = UserPackage.GetItemByType(Item.TYPE_JAR);
            if (jar != null)
            {
                if (jar.MaximumDurability == stcType)
                {
                    jar.Data += 1;
                    await jar.SaveAsync();

                    if (jar.Data % 50 == 0)
                    {
                        await jar.SendJarAsync();
                    }
                }
            }
        }

        #endregion

        #region Multiple Exp

        public bool HasMultipleExp => m_dbObject.ExperienceMultiplier > 1 && m_dbObject.ExperienceExpires >= DateTime.Now;

        public float ExperienceMultiplier =>
            !HasMultipleExp || m_dbObject.ExperienceMultiplier <= 0 ? 1f : m_dbObject.ExperienceMultiplier;

        public async Task SendMultipleExp()
        {
            if (RemainingExperienceSeconds > 0)
                await SynchroAttributesAsync(ClientUpdateType.DoubleExpTimer, RemainingExperienceSeconds, false);
        }

        public uint RemainingExperienceSeconds
        {
            get
            {
                DateTime now = DateTime.Now;
                if (m_dbObject.ExperienceExpires < now)
                {
                    m_dbObject.ExperienceMultiplier = 1;
                    m_dbObject.ExperienceExpires = null;
                    return 0;
                }

                return (uint)((m_dbObject.ExperienceExpires - now)?.TotalSeconds ?? 0);
            }
        }

        public async Task<bool> SetExperienceMultiplier(uint nSeconds, float nMultiplier = 2f)
        {
            m_dbObject.ExperienceExpires = DateTime.Now.AddSeconds(nSeconds);
            m_dbObject.ExperienceMultiplier = nMultiplier;
            await SendMultipleExp();
            return true;
        }

        #endregion

        #region Heaven Blessing

        public async Task SendBless()
        {
            if (IsBlessed)
            {
                DateTime now = DateTime.Now;
                await SynchroAttributesAsync(ClientUpdateType.HeavensBlessing, (uint)(HeavenBlessingExpires - now).TotalSeconds);

                if (Map != null && !Map.IsTrainingMap())
                    await SynchroAttributesAsync(ClientUpdateType.OnlineTraining, 0);
                else
                    await SynchroAttributesAsync(ClientUpdateType.OnlineTraining, 1);

                await AttachStatus(this, StatusSet.HEAVEN_BLESS, 0, (int)(HeavenBlessingExpires - now).TotalSeconds, 0, 0);
            }
        }

        /// <summary>
        /// This method will update the user blessing time.
        /// </summary>
        /// <param name="amount">The amount of minutes to be added.</param>
        /// <returns>If the heaven blessing has been added successfully.</returns>
        public async Task<bool> AddBlessing(uint amount)
        {
            DateTime now = DateTime.Now;
            if (m_dbObject.HeavenBlessing != null && m_dbObject.HeavenBlessing > now)
                m_dbObject.HeavenBlessing = m_dbObject.HeavenBlessing.Value.AddHours(amount);
            else
                m_dbObject.HeavenBlessing = now.AddHours(amount);

            await SendBless();
            return true;
        }

        public DateTime HeavenBlessingExpires => m_dbObject.HeavenBlessing ?? DateTime.MinValue;

        public bool IsBlessed => m_dbObject.HeavenBlessing > DateTime.Now;

        #endregion

        #region XP and Stamina

        public byte Energy { get; private set; } = DEFAULT_USER_ENERGY;

        public byte MaxEnergy => (byte) (IsBlessed ? 180 : 100);

        public byte XpPoints = 0;

        public async Task ProcXpVal()
        {
            if (!IsAlive)
            {
                await ClsXpVal();
                return;
            }

            IStatus pStatus = QueryStatus(StatusSet.START_XP);
            if (pStatus != null)
                return;

            if (XpPoints >= 100)
            {
                await BurstXp();
                await SetXp(0);
                m_xpPoints.Update();
            }
            else
            {
                if (Map != null && Map.IsBoothEnable())
                    return;
                await AddXp(1);
            }
        }

        public async Task<bool> BurstXp()
        {
            if (XpPoints < 100)
                return false;

            IStatus pStatus = QueryStatus(StatusSet.START_XP);
            if (pStatus != null)
                return true;

            await AttachStatus(this, StatusSet.START_XP, 0, 20, 0, 0);
            return true;
        }

        public async Task SetXp(byte nXp)
        {
            if (nXp > 100)
                return;
            await SetAttributesAsync(ClientUpdateType.XpCircle, nXp);
        }

        public async Task AddXp(byte nXp)
        {
            if (nXp <= 0 || !IsAlive || QueryStatus(StatusSet.START_XP) != null)
                return;
            await AddAttributesAsync(ClientUpdateType.XpCircle, nXp);
        }

        public async Task ClsXpVal()
        {
            XpPoints = 0;
            await StatusSet.DelObj(StatusSet.START_XP);
        }

        #endregion

        #region Attributes Set and Add

        public override async Task<bool> AddAttributesAsync(ClientUpdateType type, long value)
        {
            bool screen = false;
            switch (type)
            {
                case ClientUpdateType.Level:
                    if (value < 0)
                        return false;

                    screen = true;
                    value = Level = (byte)Math.Max(1, Math.Min(MAX_UPLEV, Level + value));
                    break;

                case ClientUpdateType.Experience:
                    if (value < 0)
                    {
                        Experience = Math.Max(0, Experience - (ulong) (value * -1));
                    }
                    else
                    {
                        Experience += (ulong) value;
                    }

                    value = (long) Experience;
                    break;

                case ClientUpdateType.Strength:
                    if (value < 0)
                        return false;

                    value = Strength = (ushort) Math.Max(0, Math.Min(ushort.MaxValue, Strength + value));
                    break;

                case ClientUpdateType.Agility:
                    if (value < 0)
                        return false;

                    value = Agility = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, Agility + value));
                    break;

                case ClientUpdateType.Vitality:
                    if (value < 0)
                        return false;

                    value = Vitality = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, Vitality + value));
                    break;

                case ClientUpdateType.Spirit:
                    if (value < 0)
                        return false;

                    value = Spirit = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, Spirit + value));
                    break;

                case ClientUpdateType.Atributes:
                    if (value < 0)
                        return false;

                    value = AttributePoints = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, AttributePoints + value));
                    break;

                case ClientUpdateType.XpCircle:
                    if (value < 0)
                    {
                        XpPoints = (byte)Math.Max(0, XpPoints - (value * -1));
                    }
                    else
                    {
                        XpPoints = (byte)Math.Max(0, XpPoints + value);
                    }

                    value = XpPoints;
                    break;

                case ClientUpdateType.Stamina:
                    if (value < 0)
                    {
                        Energy = (byte) Math.Max(0, Energy - (value * -1));
                    }
                    else
                    {
                        Energy = (byte)Math.Max(0, Math.Min(MaxEnergy, Energy + value));
                    }

                    value = Energy;
                    break;

                case ClientUpdateType.PkPoints:
                    value = PkPoints = (ushort) Math.Max(0, Math.Min(PkPoints + value, ushort.MaxValue));
                    await CheckPkStatusAsync((int) value);
                    break;

                default:
                    bool result = await base.AddAttributesAsync(type, value);
                    return result && await SaveAsync();
            }

            await SaveAsync();
            await SynchroAttributesAsync(type, value, screen);
            return true;
        }

        public override async Task<bool> SetAttributesAsync(ClientUpdateType type, long value)
        {
            bool screen = false;
            switch (type)
            {
                case ClientUpdateType.Level:
                    screen = true;
                    Level = (byte) Math.Max(1, Math.Min(MAX_UPLEV, value));
                    break;

                case ClientUpdateType.Experience:
                    Experience = (ulong) Math.Max(0, value);
                    break;

                case ClientUpdateType.XpCircle:
                    XpPoints = (byte) Math.Max(0, Math.Min(value, 100));
                    break;

                case ClientUpdateType.Stamina:
                    Energy = (byte)Math.Max(0, Math.Min(value, MaxEnergy));
                    break;

                case ClientUpdateType.Atributes:
                    AttributePoints = (ushort) Math.Max(0, Math.Min(ushort.MaxValue, value));
                    break;

                case ClientUpdateType.PkPoints:
                    PkPoints = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, value));
                    await CheckPkStatusAsync((int)value);
                    break;

                case ClientUpdateType.Mesh:
                    screen = true;
                    Mesh = (uint) value;
                    break;

                case ClientUpdateType.HairStyle:
                    screen = true;
                    Hairstyle = (ushort)value;
                    break;

                case ClientUpdateType.Strength:
                    value = Strength = (ushort) Math.Min(ushort.MaxValue, value);
                    break;

                case ClientUpdateType.Agility:
                    value = Agility = (ushort)Math.Min(ushort.MaxValue, value);
                    break;

                case ClientUpdateType.Vitality:
                    value = Vitality = (ushort)Math.Min(ushort.MaxValue, value);
                    break;

                case ClientUpdateType.Spirit:
                    value = Spirit = (ushort)Math.Min(ushort.MaxValue, value);
                    break;

                case ClientUpdateType.Class:
                    Profession = (byte) value;
                    break;

                case ClientUpdateType.Reborn:
                    Metempsychosis = (byte) value;
                    break;

                default:
                    bool result = await base.SetAttributesAsync(type, value);
                    return result && await SaveAsync();
            }

            await SaveAsync();
            await SynchroAttributesAsync(type, value, screen);
            return true;
        }

        /// <param name="value">The old value.</param>
        public async Task CheckPkStatusAsync(int value)
        {
            if (m_dbObject.KillPoints != value)
            {
                if (value > 99 && QueryStatus(StatusSet.BLACK_NAME) == null)
                {
                    await DetachStatus(StatusSet.RED_NAME);
                    await AttachStatus(this, StatusSet.BLACK_NAME, 0, int.MaxValue, 1, 0);
                }
                else if (value > 29 && QueryStatus(StatusSet.RED_NAME) == null)
                {
                    await DetachStatus(StatusSet.BLACK_NAME);
                    await AttachStatus(this, StatusSet.RED_NAME, 0, int.MaxValue, 1, 0);
                }
                else
                {
                    await DetachStatus(StatusSet.BLACK_NAME);
                    await DetachStatus(StatusSet.RED_NAME);
                }
            }
        }

        #endregion

        #region Mining

        private int m_mineCount = 0;

        public void StartMining()
        {
            m_mine.Startup(3);
            m_mineCount = 0;
        }

        public void StopMining()
        {
            m_mine.Clear();
        }

        public async Task DoMineAsync()
        {
            if (!IsAlive)
            {
                await SendAsync(Language.StrDead);
                StopMining();
                return;
            }

            if (UserPackage[Item.ItemPosition.RightHand]?.GetItemSubType() != 562)
            {
                await SendAsync(Language.StrMineWithPecker);
                StopMining();
                return;
            }

            if (UserPackage.IsPackFull())
            {
                await SendAsync(Language.StrYourBagIsFull);
                return;
            }

            float nChance = 15f + ((float)(WeaponSkill[562]?.Level ?? 0) / 2);
            if (await Kernel.ChanceCalcAsync(nChance))
            {
                uint idItem = await Kernel.MineManager.GetDropAsync(this);
                DbItemtype itemtype = Kernel.ItemManager.GetItemtype(idItem);
                if (itemtype == null)
                    return;

                await UserPackage.AwardItemAsync(idItem);
                await SendAsync(string.Format(Language.StrMineItemFound, itemtype.Name));

                m_mineCount++;
            }

            await BroadcastRoomMsgAsync(new MsgAction
            {
                Identity = Identity,
                Command = 0,
                ArgumentX = MapX,
                ArgumentY = MapY,
                Action = MsgAction.ActionType.MapMine
            }, true);
        }

        #endregion

        #region Timer

        public override async Task OnTimerAsync()
        {
            if (m_timeSync.ToNextTime())
            {
                _ = SendAsync(new MsgData(DateTime.Now));
            }

            try
            {
                if (m_pkDecrease.ToNextTime(PK_DEC_TIME) && PkPoints > 0)
                {
                    if (MapIdentity == 6001)
                    {
                        await AddAttributesAsync(ClientUpdateType.PkPoints, PKVALUE_DEC_ONCE_IN_PRISON);
                    }
                    else
                    {
                        await AddAttributesAsync(ClientUpdateType.PkPoints, PKVALUE_DEC_ONCE);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error pk decrease for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                foreach (var status in StatusSet.Status.Values)
                {
                    await status.OnTimer();

                    if (!status.IsValid && status.Identity != StatusSet.GHOST && status.Identity != StatusSet.DEAD)
                    {
                        await StatusSet.DelObj(status.Identity);

                        if (status.Identity == StatusSet.SUPERMAN || status.Identity == StatusSet.CYCLONE
                            && (QueryStatus(StatusSet.SUPERMAN) == null && QueryRole(StatusSet.CYCLONE) == null))
                        {
                            // Todo Superman Points
                            XpPoints = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error in status check for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (BattleSystem != null
                    && BattleSystem.IsActive()
                    && BattleSystem.NextAttack(await GetInterAtkRate()))
                {
                    await BattleSystem.ProcessAttackAsync();
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error in battle processing for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                await MagicData.OnTimerAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error in battle magic processing for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                for (int i = m_queuedActions.Count-1; i >= 0; i--)
                {
                    var action = m_queuedActions[i];
                    if (action.CanBeExecuted)
                    {
                        await GameAction.ExecuteActionAsync(action.Action, this, null, null, "");
                        m_queuedActions.Remove(action);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error in action queue for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            if (!IsAlive && !IsGhost() && m_ghost.IsActive() && m_ghost.IsTimeOut(4))
            {
                await SetGhost();
                m_ghost.Clear();
            }

            if (Team != null && !Team.IsLeader(Identity) && m_teamLeaderPos.ToNextTime())
            {
                await SendAsync(new MsgAction
                {
                    Action = MsgAction.ActionType.MapTeamLeaderStar,
                    Command = Team.Leader.Identity,
                    ArgumentX = Team.Leader.MapX,
                    ArgumentY = Team.Leader.MapY
                });
            }

            if (!IsAlive)
                return;
            
            try
            {
                if (m_energyTm.ToNextTime(ADD_ENERGY_STAND_SECS))
                {
                    if (Action == EntityAction.Sit)
                    {
                        await AddAttributesAsync(ClientUpdateType.Stamina, ADD_ENERGY_SIT);
                    }
                    else if (Action == EntityAction.Lie)
                    {
                        await AddAttributesAsync(ClientUpdateType.Stamina, ADD_ENERGY_LIE);
                    }
                    else
                    {
                        await AddAttributesAsync(ClientUpdateType.Stamina, ADD_ENERGY_STAND);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error updating energy for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (m_xpPoints.ToNextTime())
                {
                    await ProcXpVal();
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error updating xp value for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (m_autoHeal.ToNextTime() && IsAlive)
                {
                    await AddAttributesAsync(ClientUpdateType.Hitpoints, AUTOHEALLIFE_EACHPERIOD);
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error heal life for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (m_mine.IsActive() && m_mine.ToNextTime())
                    await DoMineAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error mine for user {Identity}:{Name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }
        }

        #endregion

        #region Socket

        public async Task SetLoginAsync()
        {
            m_dbObject.LoginTime = m_dbObject.LogoutTime = DateTime.Now;
            await SaveAsync();
        }

        public async Task OnDisconnectAsync()
        {
            if (Map?.IsRecordDisable() == false)
            {
                m_dbObject.MapID = m_idMap;
                m_dbObject.X = m_posX;
                m_dbObject.Y = m_posY;
            }

            m_dbObject.LogoutTime = DateTime.Now;
            m_dbObject.OnlineSeconds += (int)(m_dbObject.LogoutTime - m_dbObject.LoginTime).TotalSeconds;

            try
            {
                if (Booth != null)
                    await Booth.LeaveMap();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on booth disconnection");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {               
                await NotifyOfflineFriendAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on notifying friends disconnection");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (Team != null && Team.IsLeader(Identity))
                    await Team.Dismiss(this);
                else if (Team != null)
                    await Team.DismissMember(this);                
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on team dismiss");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                if (Trade != null)
                    await Trade.SendCloseAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on close trade");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            try
            {
                await LeaveMap();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on leave map");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            if (!m_IsDeleted)
                await SaveAsync();

            try
            {
                await using ServerDbContext context = new ServerDbContext();
                await context.LoginRcd.AddAsync(new DbGameLoginRecord
                {
                    AccountIdentity = Client.AccountIdentity,
                    UserIdentity = Identity,
                    LoginTime = m_dbObject.LoginTime,
                    LogoutTime = m_dbObject.LogoutTime,
                    ServerVersion = $"[{Kernel.SERVER_VERSION}]{Kernel.Version}",
                    IpAddress = Client.IPAddress,
                    MacAddress = "Unknown",
                    OnlineTime = (uint) (m_dbObject.LogoutTime - m_dbObject.LoginTime).TotalSeconds
                });
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Error on saving login rcd");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }
        }

        public override async Task SendAsync(IPacket msg)
        {
            if (m_socket != null)
            {
                if (await m_socket.SendAsync(msg) < 0)
                    await Kernel.RoleManager.LogoutUser(Identity);
            }
        }

        public override async Task SendSpawnToAsync(Character player)
        {
            await player.SendAsync(new MsgPlayer(this));
            
            if (Syndicate != null)
                await Syndicate.SendAsync(player);
        }

        public async Task SendWindowToAsync(Character player)
        {
            await player.SendAsync(new MsgPlayer(this)
            {
                WindowSpawn = 1
            });
        }

        #endregion

        #region Database

        public DbCharacter GetDatabaseObject() => m_dbObject;

        public async Task<bool> SaveAsync()
        {
            try
            {
                await using var db = new ServerDbContext();
                db.Update(m_dbObject);
                return await Task.FromResult(await db.SaveChangesAsync() != 0);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        #endregion

        #region Deletion

        private bool m_IsDeleted = false;

        public async Task DeleteCharacterAsync()
        {
            await BaseRepository.ScalarAsync($"INSERT INTO `cq_deluser` SELECT * FROM `cq_user` WHERE `id`={Identity};");
            await BaseRepository.DeleteAsync(m_dbObject);
            await Log.GmLog("delete_user", $"{Identity},{Name},{MapIdentity},{MapX},{MapY},{Silvers},{ConquerPoints},{Level},{Profession},{FirstProfession},{PreviousProfession}");

            foreach (var friend in m_dicFriends.Values)
                await friend.DeleteAsync();

            foreach (var enemy in m_dicEnemies.Values)
                await enemy.DeleteAsync();

            foreach (var tradePartner in m_tradePartners.Values)
                await tradePartner.DeleteAsync();

            DbPeerage peerage = Kernel.PeerageManager.GetUser(Identity);
            if (peerage != null)
                await BaseRepository.DeleteAsync(peerage);

            m_IsDeleted = true;
        }

        #endregion
    }

    /// <summary>Enumeration type for body types for player characters.</summary>
    public enum BodyType : ushort
    {
        AgileMale = 1003,
        MuscularMale = 1004,
        AgileFemale = 2001,
        MuscularFemale = 2002
    }

    /// <summary>Enumeration type for base classes for player characters.</summary>
    public enum BaseClassType : ushort
    {
        Trojan = 10,
        Warrior = 20,
        Archer = 40,
        Taoist = 100
    }

    public enum PkModeType
    {
        FreePk,
        Peace,
        Team,
        Capture
    }

    public enum RequestType
    {
        Friend,
        Syndicate,
        TeamApply,
        TeamInvite,
        Trade,
        Marriage,
        TradePartner,
        Guide
    }
}