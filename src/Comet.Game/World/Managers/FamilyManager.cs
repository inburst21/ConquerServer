﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - FamilyManager.cs
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
using System.Linq;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.States.Families;

#endregion

namespace Comet.Game.World.Managers
{
    public sealed class FamilyManager
    {
        private ConcurrentDictionary<uint, Family> m_dicFamilies = new ConcurrentDictionary<uint, Family>();

        public async Task<bool> InitializeAsync()
        {
            var dbFamilies = await DbFamily.GetAsync();
            foreach (var dbFamily in dbFamilies)
            {
                var family = await Family.CreateAsync(dbFamily);
                if (family != null)
                    m_dicFamilies.TryAdd(family.Identity, family);
            }

            foreach (var family in m_dicFamilies.Values)
            {
                family.LoadRelations();
            }

            return true;
        }

        public bool AddFamily(Family family)
        {
            return m_dicFamilies.TryAdd(family.Identity, family);
        }

        public Family GetFamily(uint idFamily)
        {
            return m_dicFamilies.TryGetValue((ushort)idFamily, out var family) ? family : null;
        }

        public Family GetFamily(string name)
        {
            return m_dicFamilies.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Find the family a user is in.
        /// </summary>
        public Family FindByUser(uint idUser)
        {
            return m_dicFamilies.Values.FirstOrDefault(x => x.GetMember(idUser) != null);
        }
    }
}