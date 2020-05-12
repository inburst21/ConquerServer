// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Item.cs
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
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Shared;

#endregion

namespace Comet.Game.States.Items
{
    public sealed class Item
    {
        private readonly Character m_user;
        private DbItem m_dbItem;
        private DbItemtype m_dbItemtype;
        private DbItemAddition m_dbItemAddition;

        public Item()
        {
        }

        public Item(Character user)
        {
            m_user = user;
        }

        public async Task<bool> CreateAsync(DbItemtype type, ItemPosition position = ItemPosition.Inventory)
        {
            if (type == null)
                return false;

            m_dbItem = new DbItem
            {
                PlayerId = m_user.Identity,
                Type = type.Type,
                Position = (byte) position,
                Amount = type.Amount,
                AmountLimit = type.AmountLimit,
                Magic1 = type.Magic1,
                Magic2 = type.Magic2,
                Magic3 = type.Magic3,
                Color = 3
            };

            m_dbItemtype = type;
            m_dbItemAddition = Kernel.ItemManager.GetItemAddition(Type, Plus);

            return await SaveAsync();
        }

        public async Task<bool> CreateAsync(DbItem item)
        {
            if (item == null)
                return false;

            m_dbItem = item;
            m_dbItemtype = Kernel.ItemManager.GetItemtype(item.Type);

            if (m_dbItemtype == null)
                return false;

            m_dbItemAddition = Kernel.ItemManager.GetItemAddition(item.Type, item.Magic3);

            if (m_dbItem.Id == 0)
                await SaveAsync();
            return true;
        }

        #region Static

        public static int CalculateGemPercentage(SocketGem gem)
        {
            switch (gem)
            {
                case SocketGem.NormalTortoiseGem:
                    return 2;
                case SocketGem.RefinedTortoiseGem:
                    return 4;
                case SocketGem.SuperTortoiseGem:
                    return 6;
                case SocketGem.NormalDragonGem:
                case SocketGem.NormalPhoenixGem:
                case SocketGem.NormalFuryGem:
                    return 5;
                case SocketGem.RefinedDragonGem:
                case SocketGem.RefinedPhoenixGem:
                case SocketGem.NormalRainbowGem:
                case SocketGem.RefinedFuryGem:
                    return 10;
                case SocketGem.SuperDragonGem:
                case SocketGem.SuperPhoenixGem:
                case SocketGem.RefinedRainbowGem:
                case SocketGem.SuperFuryGem:
                case SocketGem.NormalMoonGem:
                    return 15;
                case SocketGem.SuperRainbowGem:
                    return 25;
                case SocketGem.NormalVioletGem:
                case SocketGem.RefinedMoonGem:
                    return 30;
                case SocketGem.RefinedVioletGem:
                case SocketGem.SuperMoonGem:
                case SocketGem.NormalKylinGem:
                    return 50;
                case SocketGem.RefinedKylinGem:
                case SocketGem.SuperVioletGem:
                    return 100;
                case SocketGem.SuperKylinGem:
                    return 200;
                default:
                    return 0;
            }
        }

        #endregion

        #region Attributes

        public uint Identity => m_dbItem.Id;

        public string Name => m_dbItemtype?.Name ?? Language.StrNone;

        public uint Type => m_dbItemtype?.Type ?? 0;

        /// <summary>
        ///     May be an NPC or Sash ID.
        /// </summary>
        public uint OwnerIdentity
        {
            get => m_dbItem.OwnerId;
            set => m_dbItem.OwnerId = value;
        }

        /// <summary>
        ///     The current owner of the equipment.
        /// </summary>
        public uint PlayerIdentity
        {
            get => m_dbItem.PlayerId;
            set
            {
                _ = Log.GmLog("owner_item", $"Item {Identity} has changed owner from '{PlayerIdentity}' to '{value}'");
                m_dbItem.PlayerId = value;
            }
        }

        public ushort Durability
        {
            get => m_dbItem.Amount;
            set => m_dbItem.Amount = value;
        }

        public ushort MaximumDurability
        {
            get
            {
                ushort result = OriginalMaximumDurability;
                switch (SocketOne)
                {
                    case SocketGem.NormalKylinGem:
                    case SocketGem.RefinedKylinGem:
                    case SocketGem.SuperKylinGem:
                        result += (ushort) (OriginalMaximumDurability * (CalculateGemPercentage(SocketOne) / 100.0d));
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalKylinGem:
                    case SocketGem.RefinedKylinGem:
                    case SocketGem.SuperKylinGem:
                        result += (ushort) (OriginalMaximumDurability * (CalculateGemPercentage(SocketTwo) / 100.0d));
                        break;
                }

                return result;
            }
            set => m_dbItem.AmountLimit = value;
        }

        public ushort OriginalMaximumDurability => m_dbItem.AmountLimit;

        public SocketGem SocketOne
        {
            get => (SocketGem) m_dbItem.Gem1;
            set => m_dbItem.Gem1 = (byte) value;
        }

        public SocketGem SocketTwo
        {
            get => (SocketGem) m_dbItem.Gem2;
            set => m_dbItem.Gem2 = (byte) value;
        }

        public ItemPosition Position
        {
            get => (ItemPosition) m_dbItem.Position;
            set => m_dbItem.Position = (byte) value;
        }

        public byte Plus
        {
            get => m_dbItem.Magic3;
            set => m_dbItem.Magic3 = value;
        }

        public ItemEffect Effect
        {
            get => (ItemEffect) m_dbItem.Magic1;
            set => m_dbItem.Magic1 = (byte) value;
        }

        public byte ReduceDamage
        {
            get => m_dbItem.ReduceDmg;
            set => m_dbItem.ReduceDmg = value;
        }

        public byte Enchantment
        {
            get => m_dbItem.AddLife;
            set => m_dbItem.AddLife = value;
        }

        public ItemColor Color
        {
            get => (ItemColor) m_dbItem.Color;
            set => m_dbItem.Color = (byte) value;
        }

        public bool IsBound
        {
            get => (m_dbItem.Monopoly & 3) != 0;
            set
            {
                if (value)
                {
                    m_dbItem.Monopoly |= 3;
                }
                else
                {
                    int monopoly = m_dbItem.Monopoly;
                    monopoly &= ~3;
                    m_dbItem.Monopoly = (byte) monopoly;
                }
            }
        }

        #endregion

        #region Requirements

        public int RequiredLevel => m_dbItemtype?.ReqLevel ?? 0;

        public int RequiredProfession => m_dbItemtype?.ReqProfession ?? 0;

        public int RequiredGender => m_dbItemtype?.ReqSex ?? 0;

        public int RequiredWeaponSkill => m_dbItemtype?.ReqWeaponskill ?? 0;

        public int RequiredForce => m_dbItemtype?.ReqForce ?? 0;

        public int RequiredAgility => m_dbItemtype?.ReqSpeed ?? 0;

        public int RequiredVitality => m_dbItemtype?.ReqHealth ?? 0;

        public int RequiredSpirit => m_dbItemtype?.ReqSoul ?? 0;

        #endregion

        #region Battle Attributes

        public int BattlePower
        {
            get
            {
#if BATTLE_POWER
                if (!IsEquipment() || IsGarment() || IsGourd())
                    return 0;

                int ret = (int) Type % 5;
                if (m_user != null && m_user.Map.IsSkillMap())
                {
                    ret += Math.Max(5, (int) Plus);
                    ret += SocketOne != SocketGem.NoSocket ? 1 : 0;
                    ret += (int) SocketOne % 10 == 3 ? 1 : 0;
                }
                else
                {
                    ret += Plus;
                    ret += SocketOne != SocketGem.NoSocket ? 1 : 0;
                    ret += (int) SocketOne % 10 == 3 ? 1 : 0;
                    ret += SocketTwo != SocketGem.NoSocket ? 1 : 0;
                    ret += (int) SocketTwo % 10 == 3 ? 1 : 0;
                }

                if ((IsBackswordType() || IsWeaponTwoHand()) && m_user?.UserPackage[ItemPosition.LeftHand] == null)
                    ret *= 2;

                return ret;
#else
                return 0;
#endif
            }
        }

        public int Life
        {
            get
            {
                int result = m_dbItemtype?.Life ?? 0;
                result += Enchantment;
                result += m_dbItemAddition?.Life ?? 0;
                return result;
            }
        }

        public int Mana => m_dbItemtype?.Mana ?? 0;

        public int MinAttack
        {
            get
            {
                int result = m_dbItemtype?.AttackMin ?? 0;
                result += m_dbItemAddition?.AttackMin ?? 0;
                return result;
            }
        }

        public int MaxAttack
        {
            get
            {
                int result = m_dbItemtype?.AttackMax ?? 0;
                result += m_dbItemAddition?.AttackMax ?? 0;
                return result;
            }
        }

        public int MagicAttack
        {
            get
            {
                int result = m_dbItemtype?.MagicAtk ?? 0;
                result += m_dbItemAddition?.MagicAtk ?? 0;
                return result;
            }
        }

        public int Defense
        {
            get
            {
                int result = m_dbItemtype?.Defense ?? 0;
                result += m_dbItemAddition?.Defense ?? 0;
                return result;
            }
        }

        public int MagicDefense
        {
            get
            {
                if (Position == ItemPosition.Armor || Position == ItemPosition.Headwear)
                {
                    return m_dbItemAddition?.MagicDef ?? 0;
                }

                return m_dbItemtype?.MagicDef ?? 0;
            }
        }

        public int MagicDefenseBonus
        {
            get
            {
                if (Position == ItemPosition.Armor || Position == ItemPosition.Headwear)
                {
                    return m_dbItemAddition?.MagicDef ?? 0;
                }

                return 0;
            }
        }

        public int Accuracy
        {
            get
            {
                int result = m_dbItemtype?.Dexterity ?? 0;
                result += m_dbItemAddition?.Dexterity ?? 0;
                return result;
            }
        }

        public int Dodge
        {
            get
            {
                int result = m_dbItemtype?.Dodge ?? 0;
                result += m_dbItemAddition?.Dodge ?? 0;
                return result;
            }
        }

        public int Blessing => m_dbItem?.ReduceDmg ?? 0;

        public int DragonGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalDragonGem:
                    case SocketGem.RefinedDragonGem:
                    case SocketGem.SuperDragonGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalDragonGem:
                    case SocketGem.RefinedDragonGem:
                    case SocketGem.SuperDragonGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int PhoenixGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalPhoenixGem:
                    case SocketGem.RefinedPhoenixGem:
                    case SocketGem.SuperPhoenixGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalPhoenixGem:
                    case SocketGem.RefinedPhoenixGem:
                    case SocketGem.SuperPhoenixGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int RainbowGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalRainbowGem:
                    case SocketGem.RefinedRainbowGem:
                    case SocketGem.SuperRainbowGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalRainbowGem:
                    case SocketGem.RefinedRainbowGem:
                    case SocketGem.SuperRainbowGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int VioletGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalVioletGem:
                    case SocketGem.RefinedVioletGem:
                    case SocketGem.SuperVioletGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalVioletGem:
                    case SocketGem.RefinedVioletGem:
                    case SocketGem.SuperVioletGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int FuryGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalFuryGem:
                    case SocketGem.RefinedFuryGem:
                    case SocketGem.SuperFuryGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalFuryGem:
                    case SocketGem.RefinedFuryGem:
                    case SocketGem.SuperFuryGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int MoonGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalMoonGem:
                    case SocketGem.RefinedMoonGem:
                    case SocketGem.SuperMoonGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalMoonGem:
                    case SocketGem.RefinedMoonGem:
                    case SocketGem.SuperMoonGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int TortoiseGemEffect
        {
            get
            {
                int result = 0;
                switch (SocketOne)
                {
                    case SocketGem.NormalTortoiseGem:
                    case SocketGem.RefinedTortoiseGem:
                    case SocketGem.SuperTortoiseGem:
                        result += CalculateGemPercentage(SocketOne);
                        break;
                }

                switch (SocketTwo)
                {
                    case SocketGem.NormalTortoiseGem:
                    case SocketGem.RefinedTortoiseGem:
                    case SocketGem.SuperTortoiseGem:
                        result += CalculateGemPercentage(SocketTwo);
                        break;
                }

                return result;
            }
        }

        public int AttackRange => m_dbItemtype?.AtkRange ?? 1;

        #endregion

        #region Change Data

        public bool ChangeType(uint newType)
        {
            DbItemtype itemtype = Kernel.ItemManager.GetItemtype(newType);
            if (itemtype == null)
            {
                _ = Log.WriteLog(LogLevel.Error, $"ChangeType() Invalid itemtype id {newType}");
                return false;
            }

            m_dbItem.Type = itemtype.Type;
            m_dbItemtype = itemtype;

            m_dbItemAddition = Kernel.ItemManager.GetItemAddition(newType, m_dbItem.Magic3);
            return true;
        }

        public bool ChangeAddition(byte level)
        {
            DbItemAddition add = null;
            if (level > 0)
            {
                add = Kernel.ItemManager.GetItemAddition(Type, level);
                if (add == null)
                    return false;
            }

            Plus = level;
            m_dbItemAddition = add;
            return true;
        }

        #endregion

        #region Query info

        public bool IsHoldEnable()
        {
            return IsWeaponOneHand() || IsWeaponTwoHand() || IsWeaponProBased() || IsBow() || IsShield() ||
                   IsArrowSort();
        }

        public bool IsBow()
        {
            return IsBow(Type);
        }

        public ItemPosition GetPosition()
        {
            if (IsHelmet())
                return ItemPosition.Headwear;
            if (IsNeck())
                return ItemPosition.Necklace;
            if (IsRing())
                return ItemPosition.Ring;
            if (IsBangle())
                return ItemPosition.Ring;
            if (IsWeapon())
                return ItemPosition.RightHand;
            if (IsShield())
                return ItemPosition.LeftHand;
            if (IsArrowSort())
                return ItemPosition.LeftHand;
            if (IsArmor())
                return ItemPosition.Armor;
            if (IsShoes())
                return ItemPosition.Boots;
            if (IsGourd())
                return ItemPosition.Gourd;
            if (IsGarment())
                return ItemPosition.Garment;
            return ItemPosition.Inventory;
        }

        public bool IsArmor()
        {
            return Type / 10000 == 13;
        }

        public bool IsMedicine()
        {
            return Type >= 1000000 && Type <= 1049999 || Type == 725065 || Type == 725066;
        }

        public bool IsEquipEnable()
        {
            return IsEquipment() || IsArrowSort();
        }

        public bool IsBackswordType()
        {
            return GetItemtype() == 421;
        }

        public int GetItemtype()
        {
            return GetItemtype(Type);
        }

        public bool IsEquipment()
        {
            return !IsArrowSort() && (int) GetItemSort() < 7 || IsShield() || GetItemSubType() == 2100;
        }

        public int GetItemSubType()
        {
            return GetItemSubType(Type);
        }

        public ItemSort GetItemSort()
        {
            return GetItemSort(Type);
        }

        public bool IsArrowSort()
        {
            return IsArrowSort(Type);
        }

        public bool IsHelmet()
        {
            return Type >= 110000 && Type < 120000 || Type >= 140000 && Type < 150000;
        }

        public bool IsNeck()
        {
            return Type >= 120000 && Type < 123000;
        }

        public bool IsRing()
        {
            return Type >= 150000 && Type < 152000;
        }

        public bool IsBangle()
        {
            return Type >= 152000 && Type < 153000;
        }

        public bool IsShoes()
        {
            return Type >= 160000 && Type < 161000;
        }

        public bool IsGourd()
        {
            return Type >= 2100000 && Type < 2200000;
        }

        public bool IsGarment()
        {
            return Type >= 170000 && Type < 200000;
        }

        public bool IsWeaponOneHand()
        {
            return GetItemSort() == ItemSort.ItemsortWeaponSingleHand;
        } // single hand use

        public bool IsWeaponTwoHand()
        {
            return GetItemSort() == ItemSort.ItemsortWeaponDoubleHand;
        } // two hand use

        public bool IsWeaponProBased()
        {
            return GetItemSort() == ItemSort.ItemsortWeaponProfBased;
        } // professional hand use

        public bool IsWeapon()
        {
            return IsWeaponOneHand() || IsWeaponTwoHand() || IsWeaponProBased();
        }

        public bool IsOther()
        {
            return GetItemSort() == ItemSort.ItemsortUsable;
        }

        public bool IsFinery()
        {
            return !IsArrowSort() && GetItemSort() == ItemSort.ItemsortFinery;
        }

        public bool IsShield()
        {
            return IsShield(Type);
        }

        public bool IsExpend()
        {
            return IsExpend(Type);
        }

        public int GetQuality()
        {
            return GetQuality(Type);
        }

        public static bool IsShield(uint nType)
        {
            return nType / 1000 == 900;
        }

        public static bool IsExpend(uint type)
        {
            return IsArrowSort(type)
                   || GetItemSort(type) == ItemSort.ItemsortUsable
                   || GetItemSort(type) == ItemSort.ItemsortUsable2
                   || GetItemSort(type) == ItemSort.ItemsortUsable3;
        }

        public static int GetQuality(uint type)
        {
            return (int) (type % 10);
        }

        public static bool IsBow(uint type)
        {
            return GetItemtype(type) == 500;
        }

        public static bool IsArrowSort(uint type)
        {
            return GetItemtype(type) == 1050;
        }

        public static ItemSort GetItemSort(uint type)
        {
            return (ItemSort) (type % 10000000 / 100000);
        }

        public static int GetItemtype(uint type)
        {
            if (GetItemSort(type) == ItemSort.ItemsortWeaponSingleHand
                || GetItemSort(type) == ItemSort.ItemsortWeaponDoubleHand)
                return (int) (type % 100000 / 1000 * 1000);
            return (int) (type % 100000 / 10000 * 10000);
        }

        public static int GetItemSubType(uint type)
        {
            return (int) (type % 1000000 / 1000);
        }

        public static int GetLevel(uint type)
        {
            return (int) type % 1000 / 10;
        }

        #endregion

        #region Database

        public async Task<bool> SaveAsync()
        {
            try
            {
                await using var db = new ServerDbContext();
                if (m_dbItem.Id == 0)
                    db.Add(m_dbItem);
                else
                    db.Update(m_dbItem);
                return await db.SaveChangesAsync() != 0;
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Problem when saving item!");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                return false;
            }
        }

        public async Task<bool> DeleteAsync()
        {
            try
            {
                await using var db = new ServerDbContext();
                db.Remove(m_dbItem);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Problem when Delete item!");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                return false;
            }
        }

        #endregion

        #region Enums

        public enum ItemSort
        {
            ItemsortFinery = 1,
            ItemsortMount = 3,
            ItemsortWeaponSingleHand = 4,
            ItemsortWeaponDoubleHand = 5,
            ItemsortWeaponProfBased = 6,
            ItemsortUsable = 7,
            ItemsortWeaponShield = 9,
            ItemsortUsable2 = 10,
            ItemsortUsable3 = 12,
            ItemsortAccessory = 3,
            ItemsortTwohandAccessory = 35,
            ItemsortOnehandAccessory = 36,
            ItemsortBowAccessory = 37,
            ItemsortShieldAccessory = 38
        }

        public enum ItemEffect : ushort
        {
            None = 0,
            Poison = 0xC8,
            Life = 0xC9,
            Mana = 0xCA,
            Shield = 0xCB,
            Horse = 0x64
        }

        public enum SocketGem : byte
        {
            NormalPhoenixGem = 1,
            RefinedPhoenixGem = 2,
            SuperPhoenixGem = 3,

            NormalDragonGem = 11,
            RefinedDragonGem = 12,
            SuperDragonGem = 13,

            NormalFuryGem = 21,
            RefinedFuryGem = 22,
            SuperFuryGem = 23,

            NormalRainbowGem = 31,
            RefinedRainbowGem = 32,
            SuperRainbowGem = 33,

            NormalKylinGem = 41,
            RefinedKylinGem = 42,
            SuperKylinGem = 43,

            NormalVioletGem = 51,
            RefinedVioletGem = 52,
            SuperVioletGem = 53,

            NormalMoonGem = 61,
            RefinedMoonGem = 62,
            SuperMoonGem = 63,

            NormalTortoiseGem = 71,
            RefinedTortoiseGem = 72,
            SuperTortoiseGem = 73,

            NormalThunderGem = 101,
            RefinedThunderGem = 102,
            SuperThunderGem = 103,

            NormalGloryGem = 121,
            RefinedGloryGem = 122,
            SuperGloryGem = 123,

            NoSocket = 0,
            EmptySocket = 255
        }

        public enum ItemPosition : ushort
        {
            Inventory = 0,
            EquipmentBegin = 1,
            Headwear = 1,
            Necklace = 2,
            Armor = 3,
            RightHand = 4,
            LeftHand = 5,
            Ring = 6,
            Gourd = 7,
            Boots = 8,
            Garment = 9,
            AttackTalisman = 10,
            DefenceTalisman = 11,
            Steed = 12,
            RightHandAccessory = 15,
            LeftHandAccessory = 16,
            SteedArmor = 17,
            Crop = 18,
            EquipmentEnd = Garment,

            AltHead = 21,
            AltNecklace = 22,
            AltArmor = 23,
            AltWeaponR = 24,
            AltWeaponL = 25,
            AltRing = 26,
            AltBottle = 27,
            AltBoots = 28,
            AltGarment = 29,
            AltFan = 30,
            AltTower = 31,
            AltSteed = 32,

            UserLimit = 199,

            /// <summary>
            ///     Warehouse.
            /// </summary>
            Storage = 201,

            /// <summary>
            ///     ??????????????????????? Three kind of storages?
            /// </summary>
            Trunk = 202,

            /// <summary>
            ///     I'll use this for sashes.
            /// </summary>
            Chest = 203,

            Detained = 250,
            Floor = 254
        }

        public enum ItemColor : byte
        {
            None,
            Black = 2,
            Orange = 3,
            LightBlue = 4,
            Red = 5,
            Blue = 6,
            Yellow = 7,
            Purple = 8,
            White = 9
        }

        #endregion

        #region Constants

        //
        public const uint TYPE_DRAGONBALL = 1088000;
        public const uint TYPE_METEOR = 1088001;
        public const uint TYPE_METEORTEAR = 1088002;
        public const uint TYPE_TOUGHDRILL = 1200005;

        public const uint TYPE_STARDRILL = 1200006;

        //
        public const uint TYPE_DRAGONBALL_SCROLL = 720028; // Amount 10
        public const uint TYPE_METEOR_SCROLL = 720027; // Amount 10

        public const uint TYPE_METEORTEAR_PACK = 723711; // Amount 5

        //
        public const uint TYPE_STONE1 = 730001;
        public const uint TYPE_STONE2 = 730002;
        public const uint TYPE_STONE3 = 730003;
        public const uint TYPE_STONE4 = 730004;
        public const uint TYPE_STONE5 = 730005;
        public const uint TYPE_STONE6 = 730006;
        public const uint TYPE_STONE7 = 730007;

        public const uint TYPE_STONE8 = 730008;

        //
        public const uint TYPE_MOUNT_ID = 300000;

        //
        public const uint TYPE_EXP_BALL = 723700;

        public static readonly int[] BowmanArrows =
        {
            1050000, 1050001, 1050002, 1050020, 1050021, 1050022, 1050023, 1050030, 1050031, 1050032, 1050033, 1050040,
            1050041, 1050042, 1050043, 1050050, 1050051, 1050052
        };

        public const uint IRON_ORE = 1072010;
        public const uint COPPER_ORE = 1072020;
        public const uint EUXINITE_ORE = 1072031;
        public const uint SILVER_ORE = 1072040;
        public const uint GOLD_ORE = 1072050;

        public const uint OBLIVION_DEW = 711083;
        public const uint MEMORY_AGATE = 720828;

        public const uint PERMANENT_STONE = 723694;
        public const uint BIGPERMANENT_STONE = 723695;

        public const int LOTTERY_TICKET = 710212;
        public const uint SMALL_LOTTERY_TICKET = 711504;

        #endregion
    }
}