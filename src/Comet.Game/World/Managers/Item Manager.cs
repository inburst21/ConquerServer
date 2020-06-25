// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Item Manager.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.States.Items;

#endregion

namespace Comet.Game.World.Managers
{
    public sealed class ItemManager
    {
        private Dictionary<uint, DbItemtype> m_dicItemtype;
        private Dictionary<ulong, DbItemAddition> m_dicItemAddition;

        public async Task InitializeAsync()
        {
            m_dicItemtype = new Dictionary<uint, DbItemtype>();
            foreach (var item in await ItemtypeRepository.GetAsync())
            {
                m_dicItemtype.TryAdd(item.Type, item);
            }

            m_dicItemAddition = new Dictionary<ulong, DbItemAddition>();
            foreach (var addition in await ItemAdditionRepository.GetAsync())
            {
                m_dicItemAddition.TryAdd(AdditionKey(addition.TypeId, addition.Level), addition);
            }
        }

        public DbItemtype GetItemtype(uint type)
        {
            return m_dicItemtype.TryGetValue(type, out var item) ? item : null;
        }

        public DbItemAddition GetItemAddition(uint type, byte level)
        {
            return m_dicItemAddition.TryGetValue(AdditionKey(type, level), out var item) ? item : null;
        }

        private ulong AdditionKey(uint type, byte level)
        {
            uint key = type;
            Item.ItemSort sort = Item.GetItemSort(type);
            if (sort == Item.ItemSort.ItemsortWeaponSingleHand)
            {
                key = type / 100000 * 100000 + type % 1000 + 44000 - type % 10;
            }
            else if (sort == Item.ItemSort.ItemsortWeaponDoubleHand && !Item.IsBow(type))
            {
                key = type / 100000 * 100000 + type % 1000 + 55000 - type % 10;
            }
            else if (Item.GetItemSubType(type) >= 130 && Item.GetItemSubType(type) < 140)
            {
                key = type / 1000 * 1000 + (type % 100 / 10 * 10);
            }
            else
            {
                key = type / 1000 * 1000 + (type % 1000 - type % 10);
            }

            return key << (32 + level);
        }
    }
}