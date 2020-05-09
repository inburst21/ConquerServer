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
using System.Drawing;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Core.Mathematics;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.World.Maps;
using Comet.Network.Packets;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

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
        private TimeOut m_xpPoints = new TimeOut(3000);

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

            /**
             * Removed the base class because we'll be inheriting role stuff.
             */
            m_dbObject = character;

            m_posX = character.X;
            m_posY = character.Y;
            m_idMap = character.MapID;

            Screen = new Screen(this);
            WeaponSkill = new WeaponSkill(this);
            UserPackage = new UserPackage(this);
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

        public string Mate
        {
            get => m_dbObject.Mate;
            set => m_dbObject.Mate = value;
        }

        #endregion
        
        #region Appearence

        public int Gender => Body == BodyType.AgileMale || Body == BodyType.MuscularMale ? 1 : 2;

        public override uint Mesh
        {
            get => m_dbObject.Mesh;
            set => m_dbObject.Mesh = value;
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

        #endregion

        #region Weapon Skill

        public WeaponSkill WeaponSkill { get; }

        public async Task AddWeaponSkillExpAsync(ushort type, int exp)
        {
            DbWeaponSkill skill = WeaponSkill[type];
            if (skill == null)
            {
                await WeaponSkill.CreateAsync(type, 0);
                if ((skill = WeaponSkill[type]) == null)
                    return;
            }

            if (skill.Level >= MAX_WEAPONSKILLLEVEL)
                return;

            // exp = (int)(exp * (1 + (VioletGem / 100.0d)));

            uint nIncreaseLev = 0;
            if (skill.Level > MASTER_WEAPONSKILLLEVEL)
            {
                int nRatio = (int)(100 - (skill.Level - MASTER_WEAPONSKILLLEVEL) * 20);
                if (nRatio < 10)
                    nRatio = 10;
                exp = Calculations.MulDiv(exp, nRatio, 100) / 2;
            }

            int nNewExp = (int)Math.Max(exp + skill.Experience, skill.Experience);

#if DEBUG
            if (IsPm())
                await SendAsync($"Add Weapon Skill exp: {exp}, CurExp: {nNewExp}");
#endif

            int nLevel = (int)skill.Level;
            if (nLevel >= 1 && nLevel < MAX_WEAPONSKILLLEVEL)
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
                    skill.Level += (byte)nIncreaseLev;
                    await SendAsync(new MsgWeaponSkill(skill));
                    await SendAsync(Language.StrWeaponSkillUp);
                }
                else
                {
                    await SendAsync(new MsgWeaponSkill(skill));
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
            SynchroAttributesAsync(ClientUpdateType.Money, Silvers).Forget();
        }

        public async Task<bool> SpendMoney(int amount, bool notify = false)
        {
            if (amount > Silvers)
            {
                if (notify)
                    SendAsync(Language.StrNotEnoughMoney, MsgTalk.TalkChannel.TopLeft, Color.Red).Forget();
                return false;
            }

            Silvers = (uint)(Silvers - amount);
            await SaveAsync();
            SynchroAttributesAsync(ClientUpdateType.Money, Silvers).Forget();
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
            Silvers = (uint)(ConquerPoints + amount);
            await SaveAsync();
            SynchroAttributesAsync(ClientUpdateType.ConquerPoints, ConquerPoints).Forget();
        }

        public async Task<bool> SpendConquerPoints(int amount, bool notify = false)
        {
            if (amount > ConquerPoints)
            {
                if (notify)
                    SendAsync(Language.StrNotEnoughEmoney, MsgTalk.TalkChannel.TopLeft, Color.Red).Forget();
                return false;
            }

            ConquerPoints = (uint)(ConquerPoints - amount);
            await SaveAsync();
            SynchroAttributesAsync(ClientUpdateType.ConquerPoints, ConquerPoints).Forget();
            return true;
        }

        #endregion

        #region Pk

        public PkModeType PkMode { get; set; }

        public ushort PkPoints
        {
            get => m_dbObject?.KillPoints ?? 0;
            set
            {
                if (m_dbObject.KillPoints != value)
                {
                    if (value > 99 && QueryStatus(StatusSet.BLACK_NAME) == null)
                    {
                        DetachStatus(StatusSet.RED_NAME).Forget();
                        AttachStatus(this, StatusSet.BLACK_NAME, 0, int.MaxValue, 1, 0).Forget();
                    }
                    else if (value > 29 && QueryStatus(StatusSet.RED_NAME) == null)
                    {
                        DetachStatus(StatusSet.BLACK_NAME).Forget();
                        AttachStatus(this, StatusSet.RED_NAME, 0, int.MaxValue, 1, 0).Forget();
                    }
                    else
                    {
                        DetachStatus(StatusSet.BLACK_NAME).Forget();
                        DetachStatus(StatusSet.RED_NAME).Forget();
                    }
                }

                m_dbObject.KillPoints = value;
            }
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

        public UserPackage UserPackage { get; }

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
                BroadcastRoomMsgAsync(msg, false).Forget();
        }

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
                    result += UserPackage[pos]?.MinAttack ?? 0;
                }

                result = (int) (result + (result * (1 + (DragonGemBonus / 100d))));
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
                    result += UserPackage[pos]?.MaxAttack ?? 0;
                }

                result = (int)(result + (result * (1 + (DragonGemBonus / 100d))));
                return result;
            }
        }

        public override int MagicAttack
        {
            get
            {
                int result = Spirit;
                for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
                {
                    result += UserPackage[pos]?.MagicAttack ?? 0;
                }

                result = (int)(result + (result * (1 + (PhoenixGemBonus/ 100d))));
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

        public override int AttackSpeed { get; } = 1000;

        public override int Accuracy
        {
            get
            {
                int result = Strength;
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
                    result += UserPackage[pos]?.DragonGemEffect ?? 0;
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

        /// <summary>
        /// 
        /// </summary>
        public override void EnterMap()
        {
            Map = Kernel.MapManager.GetMap(m_idMap);
            if (Map != null)
            {
                Map.AddAsync(this).Forget();
                Map.SendMapInfoAsync(this).Forget();
            }
            else
            {
                Log.WriteLog(LogLevel.Error, $"Invalid map {m_idMap} for user {Identity} {Name}").Forget();
                m_socket?.Disconnect();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void LeaveMap()
        {
            Map?.RemoveAsync(Identity).Forget();
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
            m_dbObject.X = x;
            m_dbObject.Y = y;
            m_dbObject.MapID = idMap;
            await SaveAsync();
        }

        public async Task<bool> FlyMap(uint idMap, int x, int y)
        {
            if (Map == null)
            {
                Log.WriteLog(LogLevel.Warning, $"FlyMap user not in map").Forget();
                return false;
            }

            if (idMap == 0)
                idMap = MapIdentity;

            GameMap newMap = Kernel.MapManager.GetMap(idMap);
            if (newMap == null || !newMap.IsValidPoint(x, y))
            {
                Log.WriteLog(LogLevel.Warning, $"FlyMap user fly invalid position {idMap}[{x},{y}]").Forget();
                return false;
            }

            try
            {
                LeaveMap();

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

                EnterMap();
            }
            catch
            {
                Log.WriteLog(LogLevel.Error, "FlyMap error").Forget();
            }

            return true;
        }

        #endregion

        #region Movement

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

        #region XP and Stamina

        public byte Energy { get; private set; } = DEFAULT_USER_ENERGY;

        public byte MaxEnergy => 100;

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
            if (nXp > 100 || QueryStatus(StatusSet.START_XP) != null)
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
                    break;

                case ClientUpdateType.Mesh:
                    screen = true;
                    Mesh = (uint) value;
                    break;

                case ClientUpdateType.HairStyle:
                    screen = true;
                    Hairstyle = (ushort)value;
                    break;

                default:
                    bool result = await base.SetAttributesAsync(type, value);
                    return result && await SaveAsync();
            }

            await SaveAsync();
            await SynchroAttributesAsync(type, value, screen);
            return true;
        }

        #endregion

        #region Timer

        public override async Task OnTimerAsync()
        {
            try
            {
                if (m_pkDecrease.ToNextTime(PK_DEC_TIME))
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
            catch
            {
                Log.WriteLog(LogLevel.Error, $"Error pk decrease for user {Identity}:{Name}").Forget();
            }

            try
            {
                foreach (var status in StatusSet.Status.Values)
                {
                    status.OnTimer();

                    if (!status.IsValid && status.Identity != StatusSet.GHOST && status.Identity != StatusSet.DEAD)
                    {
                        await StatusSet.DelObj(status.Identity);
                    }
                }
            }
            catch
            {
                Log.WriteLog(LogLevel.Error, $"Error in status check for user {Identity}:{Name}").Forget();
            }

            if (!IsAlive)
                return;

            try
            {
                if (m_energyTm.ToNextTime(ADD_ENERGY_STAND_SECS))
                {
                    // todo handle blessing
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
            catch
            {
                Log.WriteLog(LogLevel.Error, $"Error updating energy for user {Identity}:{Name}").Forget();
            }

            try
            {
                if (m_xpPoints.ToNextTime())
                {
                    await ProcXpVal();
                }
            }
            catch
            {
                Log.WriteLog(LogLevel.Error, $"Error updating xp value for user {Identity}:{Name}").Forget();
            }

            try
            {
                if (m_autoHeal.ToNextTime())
                {
                    await AddAttributesAsync(ClientUpdateType.Hitpoints, AUTOHEALLIFE_EACHPERIOD);
                }
            }
            catch
            {
                Log.WriteLog(LogLevel.Error, $"Error heal life for usre {Identity}:{Name}").Forget();
            }
        }

        #endregion

        #region Socket

        public override async Task SendAsync(IPacket msg)
        {
            if (m_socket != null)
                await m_socket.SendAsync(msg);
        }

        public override async Task SendSpawnToAsync(Character player)
        {
            await player.SendAsync(new MsgPlayer(this));
        }

        #endregion

        #region Database

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
}