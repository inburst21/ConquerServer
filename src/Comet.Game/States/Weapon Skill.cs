// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Weapon Skill.cs
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
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Shared;

#endregion

namespace Comet.Game.States
{
    public sealed class WeaponSkill
    {
        private readonly Character m_user;
        private readonly ConcurrentDictionary<ushort, DbWeaponSkill> m_skills;

        public WeaponSkill(Character user)
        {
            m_user = user;
            m_skills = new ConcurrentDictionary<ushort, DbWeaponSkill>();

            foreach (var skill in WeaponSkillRepository.GetAsync(user.Identity).Result)
            {
                m_skills.TryAdd((ushort) skill.Type, skill);
            }
        }

        public DbWeaponSkill this[ushort type] => m_skills.TryGetValue(type, out var item) ? item : null;

        public async Task<bool> CreateAsync(ushort type, byte level = 1)
        {
            if (m_skills.ContainsKey(type))
                return false;

            DbWeaponSkill skill = new DbWeaponSkill
            {
                Type = type,
                Experience = 0,
                Level = level,
                OwnerIdentity = m_user.Identity,
                OldLevel = 0,
                Unlearn = 0
            };

            if (await SaveAsync(skill))
            {
                await m_user.SendAsync(new MsgWeaponSkill(skill));
                return true;
            }

            return false;
        }

        public async Task<bool> SaveAsync(DbWeaponSkill skill)
        {
            try
            {
                using (var db = new ServerDbContext())
                {
                    if (skill.Identity == 0)
                        db.Add(skill);
                    else
                        db.Update(skill);
                    await db.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, "Cannot save Weaponskill");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                return false;
            }
        }

        public async Task SendAsync()
        {
            foreach (var skill in m_skills.Values)
                await m_user.SendAsync(new MsgWeaponSkill(skill));
        }
    }
}