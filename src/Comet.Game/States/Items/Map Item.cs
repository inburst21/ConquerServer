// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Map Item.cs
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.Database.Models;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.World;
using Comet.Game.World.Maps;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.States.Items
{
    public sealed class MapItem : Role
    {
        /// <summary>
        /// Timer to keep the object alive in the map.
        /// </summary>
        private TimeOut m_tAlive = new TimeOut();
        /// <summary>
        /// Time to lock object so non-teammates cannot pick it up.
        /// </summary>
        private TimeOut m_tProtection = new TimeOut();

        private MapItemInfo m_info = default;
        private Item m_itemInfo = null;
        private DbItemtype m_itemtype = null;

        private uint m_idOwner = 0;
        private uint m_moneyAmount = 0;

        public MapItem(uint idRole)
        {
            Identity = idRole;
        }

        #region Creation

        public bool Create(GameMap map, Point pos, uint idType, uint idOwner, byte nPlus, byte nDmg,
            short nDura)
        {
            m_itemtype = Kernel.ItemManager.GetItemtype(idType);
            return m_itemtype != null && Create(map, pos, m_itemtype, idOwner, nPlus, nDmg, nDura);
        }

        public bool Create(GameMap map, Point pos, DbItemtype idType, uint idOwner, byte nPlus,
            byte nDmg,
            short nDura)
        {
            if (map == null || idType == null) return false;

            m_tAlive = new TimeOut(_DISAPPEAR_TIME);
            m_tAlive.Startup(_DISAPPEAR_TIME);

            m_idMap = map.Identity;
            Map = map;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            m_info.Addition = nPlus;
            m_info.ReduceDamage = nDmg;
            m_info.MaximumDurability = nDura;

            m_itemtype = idType;
            m_info.Type = m_itemtype.Type;

            if (idOwner != 0)
            {
                m_idOwner = idOwner;
                m_tProtection = new TimeOut(_MAPITEM_PRIV_SECS);
                m_tProtection.Startup(_MAPITEM_PRIV_SECS);
                m_tProtection.Update();
            }

            return true;
        }

        public async Task<bool> Create(GameMap map, Point pos, Item pInfo, uint idOwner)
        {
            if (map == null || pInfo == null) return false;

            int nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;
            if (pInfo.Itemtype != null)
                nAliveSecs = (int)(pInfo.Itemtype.Price / _MAPITEM_ALIVESECS_PERPRICE + _MAPITEM_USERMIN_ALIVESECS);

            if (nAliveSecs > _MAPITEM_USERMAX_ALIVESECS)
                nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;

            m_tAlive = new TimeOut(nAliveSecs);
            m_tAlive.Update();

            m_idMap = map.Identity;
            Map = map;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            m_itemInfo = pInfo;
            m_info.Type = pInfo.Type;
            m_itemInfo.OwnerIdentity = 0;
            await m_itemInfo.ChangeOwnerAsync(0, Item.ChangeOwnerType.DropItem);
            m_itemInfo.Position = Item.ItemPosition.Floor;
            return true;
        }

        public bool CreateMoney(GameMap map, Point pos, uint dwMoney, uint idOwner)
        {
            if (map == null || Identity == 0) return false;

            int nAliveSecs = _MAPITEM_MONSTER_ALIVESECS;
            if (idOwner == 0)
            {
                nAliveSecs = (int)(dwMoney / _MAPITEM_ALIVESECS_PERPRICE + _MAPITEM_USERMIN_ALIVESECS);
                if (nAliveSecs > _MAPITEM_USERMAX_ALIVESECS)
                    nAliveSecs = _MAPITEM_USERMAX_ALIVESECS;
            }

            m_tAlive = new TimeOut(nAliveSecs);
            m_tAlive.Update();

            m_idMap = map.Identity;
            Map = map;
            MapX = (ushort)pos.X;
            MapY = (ushort)pos.Y;

            uint idType;
            if (dwMoney < _ITEM_SILVER_MAX)
                idType = 1090000;
            else if (dwMoney < _ITEM_SYCEE_MAX)
                idType = 1090010;
            else if (dwMoney < _ITEM_GOLD_MAX)
                idType = 1090020;
            else if (dwMoney < _ITEM_GOLDBULLION_MAX)
                idType = 1091000;
            else if (dwMoney < _ITEM_GOLDBAR_MAX)
                idType = 1091010;
            else
                idType = 1091020;

            m_moneyAmount = dwMoney;

            m_info.Type = idType;

            if (idOwner != 0)
            {
                m_idOwner = idOwner;
                m_tProtection = new TimeOut(_MAPITEM_PRIV_SECS);
                m_tProtection.Startup(_MAPITEM_PRIV_SECS);
                m_tProtection.Update();
            }

            return true;
        }

        #endregion

        #region Identity

        public uint ItemIdentity => m_itemInfo?.Identity ?? 0;

        public uint Itemtype
        {
            get => m_info.Type;
            private set => m_info.Type = value;
        }

        public uint OwnerIdentity => m_idOwner;

        public uint Money => m_moneyAmount;

        public bool IsPrivate()
        {
            return m_tProtection.IsActive() && !m_tProtection.IsTimeOut();
        }

        public bool IsMoney()
        {
            return m_moneyAmount > 0;
        }

        #endregion

        #region Generation

        public async Task GenerateRandomInfoAsync()
        {
            Item.ItemSort sort = Item.GetItemSort(Itemtype);
            if (sort == Item.ItemSort.ItemsortWeaponSingleHand
                || sort == Item.ItemSort.ItemsortWeaponDoubleHand
                || sort == Item.ItemSort.ItemsortFinery
                || sort == Item.ItemSort.ItemsortWeaponShield)
            {
                string message = string.Empty;

                if (Item.GetQuality(Itemtype) == 6)
                {
                    message += $"Refined[{Itemtype}]";
                }
                else if (Item.GetQuality(Itemtype) == 7)
                {
                    message += $"Unique[{Itemtype}]";
                }
                else if (Item.GetQuality(Itemtype) == 8)
                {
                    message += $"Elite[{Itemtype}]";
                }
                else if (Item.GetQuality(Itemtype) == 9)
                {
                    message += $"Super[{Itemtype}]";
                }

                if (sort == Item.ItemSort.ItemsortWeaponSingleHand
                    && sort == Item.ItemSort.ItemsortWeaponDoubleHand
                    && await Kernel.ChanceCalcAsync(1, 200)) // socketed item
                {
                    message += "(SocketNum: 1)";
                    m_info.SocketNum = 1;

                    if (await Kernel.ChanceCalcAsync(20))
                    {
                        message += "(SocketNum: 2)";
                        m_info.SocketNum = 2;
                    }
                }

                if (await Kernel.ChanceCalcAsync(1, 300))
                {
                    message += "(Addition: +1)";
                    m_info.Addition = 1;
                }

                if (await Kernel.ChanceCalcAsync(1, 500))
                {
                    message += "(ReduceDamage: -3%)";
                    m_info.ReduceDamage = 3;
                }
                else if (await Kernel.ChanceCalcAsync(1, 750))
                {
                    message += "(ReduceDamage: -5%)";
                    m_info.ReduceDamage = 5;
                }

#if DEBUG
                if (!string.IsNullOrEmpty(message))
                {
                    await Log.WriteLog("dropitem", LogLevel.Debug, $"MapItem[{Identity}; {MapX}, {MapY}] {message}");
                }
#endif
            }
        }

        public async Task<Item> GetInfo(Character owner)
        {
            if (m_itemInfo == null)
            {
                m_itemInfo = new Item(owner);

                await m_itemInfo.CreateAsync(m_itemtype);

                m_itemInfo.Color = m_info.Color;

                m_itemInfo.ChangeAddition(m_info.Addition);
                m_itemInfo.ReduceDamage = m_info.ReduceDamage;

                if (m_info.SocketNum > 0)
                    m_itemInfo.SocketOne = Item.SocketGem.EmptySocket;
                if (m_info.SocketNum > 1)
                    m_itemInfo.SocketTwo = Item.SocketGem.EmptySocket;
            }

            m_itemInfo.Position = Item.ItemPosition.Inventory;
            await m_itemInfo.ChangeOwnerAsync(owner.Identity, Item.ChangeOwnerType.PickupItem);
            await m_itemInfo.SaveAsync();
            return m_itemInfo;
        }

        #endregion

        #region Battle

        public override bool IsImmunity(Role target)
        {
            return true;
        }

        #endregion

        #region Map

        public void SetAliveTimeout(int durationSecs)
        {
            m_tAlive.Startup(durationSecs);
        }

        public bool CanDisappear()
        {
            return m_tAlive.IsTimeOut();
        }

        public async Task Disappear()
        {
            if (m_itemInfo != null)
                await m_itemInfo.DeleteAsync(Item.ChangeOwnerType.DeleteDroppedItem);

            await LeaveMap();
        }

        public override async Task EnterMap()
        {
            Map = Kernel.MapManager.GetMap(MapIdentity);
            if (Map != null)
                await Map.AddAsync(this);
        }

        public override async Task LeaveMap()
        {
            IdentityGenerator.MapItem.ReturnIdentity(Identity);
            if (Map != null)
            {
                var msg = new MsgMapItem
                {
                    Identity = Identity,
                    MapX = MapX,
                    MapY = MapY,
                    Itemtype = Itemtype,
                    Mode = DropType.DisappearItem
                };
                await Map.BroadcastRoomMsgAsync(MapX, MapY, msg);
                await Map.RemoveAsync(Identity);
                Kernel.RoleManager.RemoveRole(Identity);
            }
            Map = null;
        }

        #endregion

        #region Socket

        public override async Task SendSpawnToAsync(Character player)
        {
            uint type = Itemtype;
            if (Item.GetItemSort(type) == Item.ItemSort.ItemsortWeaponShield
                || Item.GetItemSubType(type) >= 111 && Item.GetItemSubType(type) < 120
                || Item.GetItemSubType(type) >= 130 && Item.GetItemSubType(type) < 140)
                type += 300;

            await player.SendAsync(new MsgMapItem
            {
                Identity = Identity,
                MapX = MapX,
                MapY = MapY,
                Itemtype = type,
                Mode = DropType.LayItem
            });
        }

        #endregion

        #region Constants

        private const uint _ITEM_SILVER_MIN = 1;
        private const uint _ITEM_SILVER_MAX = 9;
        private const uint _ITEM_SYCEE_MIN = 10;
        private const uint _ITEM_SYCEE_MAX = 99;
        private const uint _ITEM_GOLD_MIN = 100;
        private const uint _ITEM_GOLD_MAX = 999;
        private const uint _ITEM_GOLDBULLION_MIN = 1000;
        private const uint _ITEM_GOLDBULLION_MAX = 1999;
        private const uint _ITEM_GOLDBAR_MIN = 2000;
        private const uint _ITEM_GOLDBAR_MAX = 4999;
        private const uint _ITEM_GOLDBARS_MIN = 5000;
        private const uint _ITEM_GOLDBARS_MAX = 10000000;

        private const int _PICKUP_TIME = 30;
        private const int _DISAPPEAR_TIME = 60;
        private const int _MAPITEM_ONTIMER_SECS = 5;
        private const int _MAPITEM_MONSTER_ALIVESECS = 60;
        private const int _MAPITEM_USERMAX_ALIVESECS = 90;
        private const int _MAPITEM_USERMIN_ALIVESECS = 60;

        private const int _MAPITEM_ALIVESECS_PERPRICE = 1000 / (_MAPITEM_USERMAX_ALIVESECS - _MAPITEM_USERMIN_ALIVESECS); 

        private const int _MAPITEM_PRIV_SECS = 30;
        private const int _PICKMAPITEMDIST_LIMIT = 0;

        #endregion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 2, Size = 12)]
        private struct MapItemInfo
        {
            public uint Type { get; set; }
            public short MaximumDurability { get; set; }
            public byte ReduceDamage { get; set; }
            public byte Addition { get; set; }
            public byte SocketNum { get; set; }
            public Item.ItemColor Color { get; set; }
        }
    }
}