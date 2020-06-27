// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - SyndicateMember.cs
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

using System;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Shared;

namespace Comet.Game.States.Syndicates
{
    public sealed class SyndicateMember
    {
        private DbSyndicateAttr m_attr;

        public uint UserIdentity => m_attr.UserIdentity;
        public string UserName { get; private set; }
        public uint SyndicateIdentity => m_attr.SynId;
        public int Donation
        {
            get => (int) m_attr.Proffer;
            set => m_attr.Proffer = value;
        }

        public string SyndicateName { get; private set; }

        public byte Level { get; private set; }

        public SyndicateRank Rank
        {
            get => (SyndicateRank) m_attr.Rank;
            set => m_attr.Rank = (ushort) value;
        }

        public DateTime JoinDate => m_attr.JoinDate;

        public Character User => Kernel.RoleManager.GetUser(UserIdentity);
        public bool IsOnline => User != null;

        public async Task<bool> CreateAsync(DbSyndicateAttr attr, Syndicate syn)
        {
            if (attr == null || syn == null || m_attr != null)
                return false;

            m_attr = attr;

            DbCharacter user = await CharactersRepository.FindByIdentityAsync(attr.UserIdentity);
            if (user == null)
                return false;

            UserName = user.Name;
            SyndicateName = syn.Name;
            Level = user.Level;
            return true;
        }

        public async Task<bool> CreateAsync(Character user, Syndicate syn, SyndicateRank rank)
        {
            if (user == null || syn == null || m_attr != null)
                return false;

            m_attr = new DbSyndicateAttr
            {
                UserIdentity = user.Identity,
                SynId = syn.Identity,
                Arsenal = 0,
                Emoney = 0,
                EmoneyTotal = 0,
                Exploit = 0,
                Guide = 0,
                GuideTotal = 0,
                JoinDate = DateTime.Now,
                Pk = 0,
                PkTotal = 0,
                Proffer = 0,
                ProfferTotal = 0,
                Rank = (ushort) rank
            };

            if (!await BaseRepository.SaveAsync(m_attr))
                return false;

            UserName = user.Name;
            SyndicateName = syn.Name;
            Level = user.Level;

            await Log.GmLog("syndicate", $"User [{user.Identity}, {user.Name}] has joined [{syn.Identity}, {syn.Name}]");
            return true;
        }

        public enum SyndicateRank : ushort
        {
            GuildLeader = 100,
            DeputyLeader = 90,
            Member = 50,
            None = 0
        }

        public async Task<bool> SaveAsync()
        {
            return await BaseRepository.SaveAsync(m_attr);
        }

        public async Task<bool> DeleteAsync()
        {
            return await BaseRepository.DeleteAsync(m_attr);
        }
    }
}