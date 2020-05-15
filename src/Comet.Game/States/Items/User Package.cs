// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - User Package.cs
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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.World;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.States.Items
{
    public sealed class UserPackage
    {
        public const int MAX_INVENTORY_CAPACITY = 40;

        private readonly Character m_user;

        private readonly ConcurrentDictionary<Item.ItemPosition, Item> m_dicEquipment;
        private readonly ConcurrentDictionary<uint, Item> m_dicInventory;
        private readonly ConcurrentDictionary<uint, List<Item>> m_dicWarehouses;

        public UserPackage(Character user)
        {
            m_user = user;

            m_dicEquipment = new ConcurrentDictionary<Item.ItemPosition, Item>();
            m_dicInventory = new ConcurrentDictionary<uint, Item>();
            m_dicWarehouses = new ConcurrentDictionary<uint, List<Item>>();
        }

        public Item this[Item.ItemPosition position]
        {
            get => m_dicEquipment.TryGetValue(position, out var item) ? item : null;
            set
            {
                // todo make equip call
            }
        }

        public Item this[uint uid] => m_dicInventory.TryGetValue(uid, out var item) ? item : null;

        public Item this[Item.ItemPosition pos, uint uid] =>
            m_dicEquipment.TryGetValue(pos, out var item) && item.Identity == uid ? item : null;

        public Item this[uint storage, uint uid] => m_dicWarehouses.TryGetValue(storage, out var items)
            ? items.FirstOrDefault(x => x.Identity == uid)
            : null;

        public async Task<bool> CreateAsync()
        {
            var items = await ItemRepository.GetAsync(m_user.Identity);
            foreach (var dbItem in items)
            {
                Item item = new Item(m_user);
                if (!await item.CreateAsync(dbItem))
                {
                    await Log.WriteLog(LogLevel.Error, $"Failed to load item {dbItem.Id} to user {m_user.Identity}");
                    continue;
                }

                if (item.Position >= Item.ItemPosition.EquipmentBegin &&
                    item.Position <= Item.ItemPosition.EquipmentEnd)
                {
                    if (!m_dicEquipment.TryAdd(item.Position, item))
                        m_dicInventory.TryAdd(item.Identity, item);
                }
                else if (item.Position == Item.ItemPosition.Chest
                         || item.Position == Item.ItemPosition.Storage)
                {
                    if (m_dicWarehouses.TryGetValue(item.OwnerIdentity, out var list))
                    {
                        list.Add(item);
                    }
                    else
                    {
                        m_dicWarehouses.TryAdd(item.OwnerIdentity, new List<Item> {item});
                    }
                }
                else if (item.Position == Item.ItemPosition.Inventory)
                {
                    if (!m_dicInventory.TryAdd(item.Identity, item))
                        await Log.WriteLog(LogLevel.Warning,
                            $"Failed to insert inventory item {item.Identity}: duplicate???");
                }
                else
                {
                    await Log.WriteLog(LogLevel.Warning,
                        $"Item {item.Identity} on '{item.Position}' cannot be loaded (unhandled)");
                }
            }

            return true;
        }

        public async Task<bool> AwardItemAsync(uint type, Item.ItemPosition pos = Item.ItemPosition.Inventory)
        {
            DbItemtype itemtype = Kernel.ItemManager.GetItemtype(type);
            if (itemtype == null)
                return false;

            Item item = new Item(m_user);
            if (!await item.CreateAsync(itemtype, pos))
                return false;

            return m_dicInventory.TryAdd(item.Identity, item) && await AddItem(item);
        }

        public async Task<bool> UseItemAsync(uint idItem, Item.ItemPosition position)
        {
            if (!TryItem(idItem, position))
                return false;

            Item item = this[idItem];
            if (item == null)
                return false;

            if (item.IsEquipEnable())
                return await EquipItemAsync(item, position);

            if (item.IsMedicine())
            {
                if (item.Life > 0 && m_user.Life == m_user.MaxLife ||
                    item.Mana > 0 && m_user.MaxMana == m_user.Mana)
                    return false;

                if (!await SpendItem(item))
                    return false;

                if (item.Life > 0)
                    await m_user.AddAttributesAsync(ClientUpdateType.Hitpoints, item.Life);
                if (item.Mana > 0)
                    await m_user.AddAttributesAsync(ClientUpdateType.Mana, item.Mana);

                return false;
            }

            return false;
        }

        public async Task<bool> EquipItemAsync(Item item, Item.ItemPosition position)
        {
            if (item == null)
                return false;

            if (position == Item.ItemPosition.Inventory)
            {
                if (m_user.IsWing && !item.IsArrowSort())
                    return false;

                position = item.GetPosition();
                if (this[Item.ItemPosition.RightHand] != null
                    && this[Item.ItemPosition.LeftHand] == null
                    && item.IsWeaponOneHand()
                    && !item.IsBackswordType())
                    position = Item.ItemPosition.LeftHand;
            }

            switch (position)
            {
                case Item.ItemPosition.RightHand:
                    if (!item.IsHoldEnable())
                        return false;

                    if (item.IsWeaponTwoHand())
                    {
                        if (!(this[Item.ItemPosition.RightHand] != null
                              && this[Item.ItemPosition.LeftHand] != null
                              && !IsPackSpare(2)))
                        {
                            await UnequipAsync(Item.ItemPosition.RightHand);

                            if (this[Item.ItemPosition.LeftHand] != null &&
                                !this[Item.ItemPosition.LeftHand].IsArrowSort() &&
                                item.IsBow() &&
                                this[Item.ItemPosition.LeftHand]?.IsShield() == true)
                                await UnequipAsync(Item.ItemPosition.LeftHand);
                        }
                    }
                    else if (item.IsWeaponOneHand() || item.IsWeaponProBased())
                    {
                        Item pLeft = this[Item.ItemPosition.LeftHand];
                        if (!(pLeft != null && pLeft.IsArrowSort() && !IsPackSpare(2)))
                        {
                            await UnequipAsync(Item.ItemPosition.RightHand);
                            if (pLeft != null && pLeft.IsArrowSort())
                                await UnequipAsync(Item.ItemPosition.LeftHand);
                        }
                    }

                    break;

                case Item.ItemPosition.LeftHand:
                    if (!item.IsHoldEnable())
                        return false;

                    Item pRight = this[Item.ItemPosition.RightHand];
                    if (pRight == null)
                        return false;

                    if (item.IsBackswordType() || pRight.IsBackswordType())
                        return false;

                    if (pRight.IsBackswordType() && item.IsShield())
                        return false;

                    if (pRight.IsWeaponOneHand() && !pRight.IsBackswordType() &&
                        (item.IsWeaponOneHand() || item.IsShield())
                        || pRight.IsBow() && item.IsArrowSort())
                    {
                        await UnequipAsync(Item.ItemPosition.LeftHand);
                    }
                    else if (pRight.IsWeaponProBased())
                    {
                        if (pRight.GetItemSubType() == item.GetItemSubType())
                        {
                            if (item.IsShield())
                                return false;

                            await UnequipAsync(Item.ItemPosition.LeftHand);
                        }
                    }

                    break;

                default:
                    await UnequipAsync(position);
                    break;
            }

            if (!await RemoveFromInventoryAsync(item))
                return false;

            if (!m_dicEquipment.TryAdd(position, item))
            {
                await AddItem(item);
                return false;
            }

            item.Position = position;
            await m_user.SendAsync(new MsgItem(item.Identity, MsgItem.ItemActionType.InventoryEquip, (uint) position));
            await m_user.SendAsync(new MsgItemInfo(item));

            await item.SaveAsync();

            switch (position)
            {
                case Item.ItemPosition.Headwear:
                case Item.ItemPosition.RightHand:
                case Item.ItemPosition.LeftHand:
                case Item.ItemPosition.Armor:
                case Item.ItemPosition.Garment:
                    await m_user.Screen.BroadcastRoomMsgAsync(new MsgPlayer(m_user), false);
                    break;
            }

            return true;
        }

        public async Task<bool> UnequipAsync(Item.ItemPosition position, RemovalType mode = RemovalType.RemoveOnly)
        {
            Item item = this[position];
            if (item == null)
                return false;

            if (position == Item.ItemPosition.RightHand
                && this[Item.ItemPosition.LeftHand] != null)
            {
                if (!IsPackSpare(2))
                    return false;

                if (!await UnequipAsync(Item.ItemPosition.LeftHand))
                    return false;
            }
            else
            {
                if (!IsPackSpare(1))
                    return false;
            }

            m_dicEquipment.TryRemove(position, out _);

            item.Position = Item.ItemPosition.Inventory;
            await m_user.SendAsync(new MsgItem(item.Identity, MsgItem.ItemActionType.EquipmentRemove, (uint) position));
            await m_user.SendAsync(new MsgItemInfo(item));

            if (mode == RemovalType.Delete)
                await item.DeleteAsync();
            else
            {
                await AddItem(item);
                await item.SaveAsync();
            }

            switch (position)
            {
                case Item.ItemPosition.Headwear:
                case Item.ItemPosition.RightHand:
                case Item.ItemPosition.LeftHand:
                case Item.ItemPosition.Armor:
                case Item.ItemPosition.Garment:
                    await m_user.Screen.BroadcastRoomMsgAsync(new MsgPlayer(m_user), false);
                    break;
            }

            return true;
        }

        public bool TryItem(uint idItem, Item.ItemPosition position)
        {
            Item item = this[idItem];
            if (item == null)
                return false;

            if (item.RequiredLevel > m_user.Level)
                return false;

            if (item.RequiredGender > 0 && item.RequiredGender != m_user.Gender)
                return false;

            if (item.Durability == 0 && !item.IsExpend())
                return false;

            if (m_user.Metempsychosis > 0 && m_user.Level >= 70)
                return true;

            if (item.RequiredProfession > 0)
            {
                int nRequireProfSort = item.RequiredProfession % 1000 / 10;
                int nRequireProfLevel = item.RequiredProfession % 10;
                int nProfSort = m_user.ProfessionSort;
                int nProfLevel = m_user.ProfessionLevel;

                if (nRequireProfSort == 19)
                {
                    if (nProfSort < 10 || position == Item.ItemPosition.LeftHand)
                        return false;
                }
                else
                {
                    if (nRequireProfSort != nProfSort)
                        return false;
                }

                if (nProfLevel < nRequireProfLevel)
                    return false;
            }

            if (item.RequiredForce > m_user.Strength
                || item.RequiredAgility > m_user.Agility
                || item.RequiredVitality > m_user.Vitality
                || item.RequiredSpirit > m_user.Spirit)
                return false;

            if (item.RequiredWeaponSkill > 0 &&
                m_user.WeaponSkill[(ushort) item.GetItemtype()]?.Level < item.RequiredWeaponSkill)
                return false;

            return true;
        }

        public async Task<bool> AddItem(Item item)
        {
            if (IsPackFull())
                return false;

            item.PlayerIdentity = m_user.Identity;
            item.Position = Item.ItemPosition.Inventory;
            m_dicInventory.TryAdd(item.Identity, item);
            await m_user.SendAsync(new MsgItemInfo(item));
            return true;
        }

        public async Task<bool> RemoveFromInventoryAsync(uint idItem, RemovalType mode = RemovalType.RemoveOnly)
        {
            return m_dicInventory.TryGetValue(idItem, out var item) && await RemoveFromInventoryAsync(item, mode);
        }

        public async Task<bool> RemoveFromInventoryAsync(Item item, RemovalType mode = RemovalType.RemoveOnly)
        {
            if (!m_dicInventory.TryRemove(item.Identity, out _))
                return false;

            switch (mode)
            {
                case RemovalType.DropItem:
                    item.Position = Item.ItemPosition.Floor;
                    break;

                case RemovalType.Delete:
                    await Log.GmLog("delete_item",
                        $"{item.Identity}, {item.Name}, {item.PlayerIdentity}\r\n\t{item.ToJson()}");
                    await item.DeleteAsync();
                    break;
            }

            await m_user.SendAsync(new MsgItem(item.Identity, MsgItem.ItemActionType.InventoryRemove));
            return true;
        }

        public async Task<bool> SpendItem(uint type, int nNum = 1, bool bSynchro = true)
        {
            return await SpendItem(GetItemByType(type), nNum, bSynchro);
        }

        public async Task<bool> SpendItem(Item item, int nNum = 1, bool bSynchro = true)
        {
            if (item == null)
                return false;

            return item.Position == Item.ItemPosition.Inventory &&
                   await RemoveFromInventoryAsync(item.Identity, RemovalType.Delete);
        }

        public int MeteorAmount()
        {
            int amount = 0;
            foreach (var item in m_dicInventory.Values)
            {
                switch (item.Type)
                {
                    case Item.TYPE_METEOR:
                    case Item.TYPE_METEORTEAR:
                        amount++;
                        break;
                    case Item.TYPE_METEOR_SCROLL:
                        amount += 10;
                        break;
                }
            }

            return amount;
        }

        public int DragonBallAmount(bool bound = true, bool onlyBound = false)
        {
            int amount = 0;
            foreach (var item in m_dicInventory.Values)
            {
                if (!bound && item.IsBound)
                    continue;
                if (!item.IsBound && onlyBound)
                    continue;

                switch (item.Type)
                {
                    case Item.TYPE_DRAGONBALL:
                        amount++;
                        break;
                    case Item.TYPE_DRAGONBALL_SCROLL:
                        amount += 10;
                        break;
                }
            }

            return amount;
        }

        public bool SpendMeteors(int amount)
        {
            if (amount > MeteorAmount())
                return false;

            List<Item> items = new List<Item>();
            int meteor = MultiGetItem(Item.TYPE_METEOR, Item.TYPE_METEORTEAR, 0, ref items);
            int meteorScroll =
                MultiGetItem(Item.TYPE_METEOR_SCROLL, Item.TYPE_METEOR_SCROLL, 0, ref items);

            int taken = 0;
            if (meteor >= amount)
            {
                foreach (var item in items.OrderBy(x => x.Type).Reverse())
                {
                    if (item.Type == Item.TYPE_METEOR || item.Type == Item.TYPE_METEORTEAR)
                    {
                        _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                        items.Remove(item);
                        taken++;
                    }

                    if (taken >= amount)
                        break;
                }

                return taken >= amount;
            }

            if (amount % 10 > 0 && meteor % 10 >= amount % 10)
            {
                int take = amount % 10;
                foreach (var item in items.OrderBy(x => x.Type).Reverse())
                {
                    if (item.Type == Item.TYPE_METEOR || item.Type == Item.TYPE_METEORTEAR)
                    {
                        _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                        items.Remove(item);
                        taken++;
                        take--;
                    }

                    if (take == 0)
                        break;
                }
            }

            int amountMS = (int) Math.Ceiling(amount / 10f);
            int returnMeteor = 0;
            if (amountMS * 10 > amount)
                returnMeteor = amountMS * 10 - amount;

            foreach (var item in items.Where(x => x.Type == Item.TYPE_METEOR_SCROLL).Reverse())
            {
                _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                items.Remove(item);
                taken += 10;

                if (taken >= amount)
                {
                    while (returnMeteor > 0)
                    {
                        _ = AwardItemAsync(Item.TYPE_METEOR);
                        returnMeteor--;
                    }

                    return true;
                }
            }

            return true;
        }

        public bool SpendDragonBalls(int amount, bool allowBound = false)
        {
            if (amount > DragonBallAmount())
                return false;

            List<Item> items = new List<Item>();
            int db = MultiGetItem(Item.TYPE_DRAGONBALL, Item.TYPE_DRAGONBALL, 0, ref items);
            int dbScroll = MultiGetItem(Item.TYPE_DRAGONBALL_SCROLL, Item.TYPE_DRAGONBALL_SCROLL, 0,
                ref items);

            int taken = 0;
            if (db >= amount)
            {
                foreach (var item in items.OrderBy(x => x.Type).Reverse())
                {
                    if (!allowBound && item.IsBound)
                        continue;

                    if (item.Type == Item.TYPE_DRAGONBALL)
                    {
                        _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                        items.Remove(item);
                        taken++;
                    }

                    if (taken >= amount)
                        break;
                }

                return taken >= amount;
            }

            if (amount % 10 > 0 && db % 10 >= amount % 10)
            {
                int take = amount % 10;
                foreach (var item in items.OrderBy(x => x.Type).Reverse())
                {
                    if (!allowBound && item.IsBound)
                        continue;

                    if (item.Type == Item.TYPE_DRAGONBALL)
                    {
                        _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                        items.Remove(item);
                        taken++;
                        take--;
                    }

                    if (take == 0)
                        break;
                }
            }

            int amountDbs = (int) Math.Ceiling(amount / 10f);

            if (dbScroll < amountDbs)
                return false;

            int returnDb = 0;
            if (amountDbs * 10 > amount)
                returnDb = amountDbs * 10 - amount;

            foreach (var item in items.Where(x => x.Type == Item.TYPE_DRAGONBALL_SCROLL).Reverse())
            {
                if (!allowBound && item.IsBound)
                    continue;

                _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
                items.Remove(item);
                taken += 10;

                if (taken >= amount)
                {
                    while (returnDb > 0)
                    {
                        _ = AwardItemAsync(Item.TYPE_DRAGONBALL);
                        returnDb--;
                    }

                    return true;
                }
            }

            return true;
        }

        public bool MultiSpendItem(uint idFirst, uint idLast, int nNum, bool dontAllowBound = false)
        {
            int temp = nNum;
            List<Item> items = new List<Item>();
            if (MultiGetItem(idFirst, idLast, nNum, ref items, dontAllowBound) < nNum)
                return false;

            foreach (var item in items)
            {
                if (dontAllowBound && item.IsBound)
                    continue;

                nNum--;
                _ = RemoveFromInventoryAsync(item, RemovalType.Delete);
            }

            if (nNum > 0)
                _ = Log.WriteLog(LogLevel.Error,
                    $"Something went wrong when MultiSpendItem({idFirst}, {idLast}, {temp}) {nNum} left???");
            return nNum == 0;
        }

        public int MultiGetItem(uint idFirst, uint idLast, int nNum, ref List<Item> pItems, bool dontAllowBound = false)
        {
            int nAmount = 0;
            foreach (var item in m_dicInventory
                .Values
                .Where(x => x.Type >= idFirst && x.Type <= idLast)
                .OrderBy(x => x.Type))
            {
                if (dontAllowBound && item.IsBound)
                    continue;

                pItems.Add(item);
                nAmount += 1;

                if (nNum > 0 && nAmount >= nNum)
                    return nAmount;
            }

            return nAmount;
        }

        public bool MultiCheckItem(uint idFirst, uint idLast, int nNum, bool dontAllowBound = false)
        {
            int nAmount = 0;
            foreach (var item in m_dicInventory.Values.Where(x => x.Type >= idFirst && x.Type <= idLast))
            {
                if (dontAllowBound && item.IsBound)
                    continue;

                nAmount += 1;
                if (nAmount >= nNum)
                    return true;
            }

            return nAmount >= nNum;
        }

        public async Task<int> RandDropItem(int nMapType, int nChance)
        {
            if (m_user == null)
                return 0;
            int nDropNum = 0;
            foreach (var item in m_dicInventory.Values)
            {
                if (await Kernel.ChanceCalcAsync(nChance))
                {
                    if (item.IsNeverDropWhenDead())
                        continue;

                    switch (nMapType)
                    {
                        case 0: // NONE
                            break;
                        case 1: // PK
                        case 2: // SYN
                            if (!item.IsArmor())
                                continue;
                            break;
                        case 3: // PRISON
                            break;
                    }

                    var pos = new Point(m_user.MapX, m_user.MapY);
                    if (m_user.Map.FindDropItemCell(5, ref pos))
                    {
                        if (!await RemoveFromInventoryAsync(item.Identity, RemovalType.DropItem))
                            continue;

                        item.PlayerIdentity = 0;
                        item.OwnerIdentity = m_user.Identity;

                        var pMapItem = new MapItem((uint) IdentityGenerator.MapItem.GetNextIdentity);
                        if (pMapItem.Create(m_user.Map, pos, item, m_user.Identity))
                        {
                            await pMapItem.EnterMap();
                            await Log.GmLog("drop_item",
                                $"{m_user.Name}({m_user.Identity}) drop item:[id={item.Identity}, type={item.Type}], dur={item.Durability}, max_dur={item.MaximumDurability}\n\t{item.ToJson()}");
                        }
                        else
                        {
                            IdentityGenerator.MapItem.ReturnIdentity(pMapItem.Identity);
                        }

                        nDropNum++;
                    }
                }
            }

            return nDropNum;
        }

        public async Task<int> RandDropItem(int nDropNum)
        {
            if (m_user == null)
                return 0;

            var temp = new List<Item>();
            foreach (var item in m_dicInventory.Values)
            {
                if (item.IsNeverDropWhenDead())
                    continue;
                temp.Add(item);
            }

            int nTotalItemCount = temp.Count;
            if (nTotalItemCount == 0)
                return 0;
            int nRealDropNum = 0;
            for (int i = 0; i < Math.Min(nDropNum, nTotalItemCount); i++)
            {
                int nIdx = await Kernel.NextAsync(nTotalItemCount);
                Item item;
                try
                {
                    item = temp[nIdx];
                }
                catch
                {
                    continue;
                }

                var pos = new Point(m_user.MapX, m_user.MapY);
                if (m_user.Map.FindDropItemCell(5, ref pos))
                {
                    if (!await RemoveFromInventoryAsync(item.Identity, RemovalType.DropItem))
                        continue;

                    item.PlayerIdentity = 0;
                    item.OwnerIdentity = m_user.Identity;

                    var pMapItem = new MapItem((uint) IdentityGenerator.MapItem.GetNextIdentity);
                    if (pMapItem.Create(m_user.Map, pos, item, m_user.Identity))
                    {
                        await pMapItem.EnterMap();
                        await Log.GmLog("drop_item",
                            $"{m_user.Name}({m_user.Identity}) drop item:[id={item.Identity}, type={item.Type}], dur={item.Durability}, max_dur={item.MaximumDurability}\n\t{item.ToJson()}");
                    }
                    else
                    {
                        IdentityGenerator.MapItem.ReturnIdentity(pMapItem.Identity);
                    }

                    nRealDropNum++;
                }
            }

            return nRealDropNum;
        }

        public async Task<bool> RandDropEquipment(Character attacker)
        {
            if (m_dicEquipment.Count == 0)
                return false;

            List<Item> items = m_dicEquipment.Values.Where(x => !x.IsArrowSort() && !x.IsGourd() && !x.IsGourd()).ToList();
            Item item = items[await Kernel.NextAsync(items.Count) % items.Count];

            if (!await UnequipAsync(item.Position))
                return false;

            if (await m_user.DropItem(item.Identity, m_user.MapX, m_user.MapY, true) && attacker != null)
            {
                await Kernel.RoleManager.BroadcastMsgAsync(string.Format(Language.StrDropEquipment, m_user.Name), MsgTalk.TalkChannel.Talk, Color.Red);
            }
            return true;
        }

        public int InventoryCount => m_dicInventory.Count;

        public bool IsPackSpare(int size)
        {
            return m_dicInventory.Count + size <= MAX_INVENTORY_CAPACITY;
        }

        public bool IsPackFull()
        {
            return m_dicInventory.Count >= MAX_INVENTORY_CAPACITY;
        }

        /// <summary>
        ///     Sent only on login!!!
        /// </summary>
        public async Task SendAsync()
        {
            foreach (var item in m_dicEquipment.Values)
            {
                await m_user.SendAsync(new MsgItemInfo(item));
            }

            foreach (var item in m_dicInventory.Values)
            {
                await m_user.SendAsync(new MsgItemInfo(item));
            }
        }

        public Item GetItemByType(uint type)
        {
            return m_dicInventory.Values.FirstOrDefault(x => x.Type == type);
        }

        public enum RemovalType
        {
            /// <summary>
            ///     Item will be removed and disappear, but wont be deleted.
            /// </summary>
            RemoveAndDisappear,

            /// <summary>
            ///     Item will be internally removed only. No client interaction and also wont be deleted.
            /// </summary>
            RemoveOnly,

            /// <summary>
            ///     Item will be removed and deleted.
            /// </summary>
            Delete,

            /// <summary>
            ///     Item will be set to floor and will be updated. No delete.
            /// </summary>
            DropItem
        }
    }
}