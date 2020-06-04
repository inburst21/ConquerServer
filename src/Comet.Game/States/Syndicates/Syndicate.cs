// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Syndicate.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Network.Packets;

namespace Comet.Game.States.Syndicates
{
    public sealed class Syndicate
    {
        private const string DEFAULT_ANNOUNCEMENT_S = "This is a new guild.";

        private DbSyndicate m_syndicate;
        private SyndicateMember m_leader;
        private ConcurrentDictionary<uint, SyndicateMember> m_dicMembers = new ConcurrentDictionary<uint, SyndicateMember>();
        private ConcurrentDictionary<ushort, Syndicate> m_dicAllies = new ConcurrentDictionary<ushort, Syndicate>();
        private ConcurrentDictionary<ushort, Syndicate> m_dicEnemies = new ConcurrentDictionary<ushort, Syndicate>();

        public Syndicate()
        {
            
        }

        public ushort Identity => m_syndicate.Identity;
        public string Name => m_syndicate.Name;
        public int MemberCount => m_dicMembers.Count;
        public uint Money => (uint) m_syndicate.Money;

        public string Announce => m_syndicate.Announce;
        public DateTime AnnounceDate => m_syndicate.AnnounceDate;

        public bool Deleted => m_syndicate.DelFlag != null;

        public SyndicateMember Leader => m_leader;

        #region Position Count

        public int DeputyLeaderCount => m_dicMembers.Values.Count(x => x.Rank == SyndicateMember.SyndicateRank.DeputyLeader);

        #endregion

        #region Create

        public async Task<bool> CreateAsync(DbSyndicate syn)
        {
            m_syndicate = syn;

            if (Deleted)
                return true;

            List<DbSyndicateAttr> members = await SyndicateAttrRepository.GetAsync(Identity);
            foreach (var dbMember in members)
            {
                SyndicateMember member = new SyndicateMember();
                if (!await member.CreateAsync(dbMember, this))
                    continue;

                if (m_dicMembers.TryAdd(member.UserIdentity, member))
                {
                    if (member.Rank == SyndicateMember.SyndicateRank.GuildLeader)
                        m_leader = member;
                }
            }

            if (MemberCount == 0)
                m_syndicate.DelFlag = DateTime.Now;

            m_syndicate.Amount = (uint) MemberCount;
            await SaveAsync();
            return true;
        }

        public async Task<bool> CreateAsync(string name, int investment, Character leader)
        {
            if (Kernel.SyndicateManager.GetSyndicate(name) != null)
            {
                await leader.SendAsync(Language.StrSynNameInUse);
                return false;
            }

            m_syndicate = new DbSyndicate
            {
                Amount = 1,
                Announce = DEFAULT_ANNOUNCEMENT_S,
                AnnounceDate = DateTime.Now,
                CastleIdentity = 0,
                CastleLevel = 0,
                ConquerPoints = 0,
                CreationDate = DateTime.Now,
                CurrentBossLevel = 0,
                EmoneyPrize = 0,
                LeaderIdentity = leader.Identity,
                LeaderName = leader.Name,
                Money = (ulong) (investment/2),
                MoneyPrize = 0,
                Name = name,
                ReqClass = 0,
                ReqMetempsychosis = 0,
                ReqLevel = 0
            };

            if (!await SaveAsync())
                return false;

            m_leader = new SyndicateMember();
            if (!await m_leader.CreateAsync(leader, this, SyndicateMember.SyndicateRank.GuildLeader))
            {
                await DeleteAsync();
                return false;
            }

            m_dicMembers.TryAdd(m_leader.UserIdentity, m_leader);
            return true;
        }

        public async Task LoadRelationAsync()
        {
            var dbAllies = await SyndicateAllyRepository.GetAsync(Identity);
            foreach (var ally in dbAllies)
            {
                // validate alliance
                if ((await SyndicateAllyRepository.GetAsync(ally.AllyIdentity)).All(x => x.AllyIdentity != ally.SyndicateIdentity))
                {
                    await BaseRepository.DeleteAsync(ally);
                    continue;
                }

                Syndicate syndicate = Kernel.SyndicateManager.GetSyndicate((int) ally.AllyIdentity);
                if (syndicate == null || syndicate.Deleted)
                {
                    // invalid ally
                    await BaseRepository.DeleteAsync(ally);
                    continue;
                }

                m_dicAllies.TryAdd(syndicate.Identity, syndicate);
            }

            var dbEnemies = await SyndicateEnemyRepository.GetAsync(Identity);
            foreach (var enemy in dbEnemies)
            {
                Syndicate syndicate = Kernel.SyndicateManager.GetSyndicate((int)enemy.EnemyIdentity);
                if (syndicate == null || syndicate.Deleted)
                {
                    // invalid ally
                    await BaseRepository.DeleteAsync(enemy);
                    continue;
                }

                m_dicEnemies.TryAdd(syndicate.Identity, syndicate);
            }
        }

        #endregion

        #region Query

        public SyndicateMember QueryMember(uint id)
        {
            return m_dicMembers.TryGetValue(id, out var member) ? member : null;
        }

        public SyndicateMember QueryMember(string member)
        {
            return m_dicMembers.Values.FirstOrDefault(x => x.UserName == member);
        }

        #endregion

        #region Socket

        public async Task SendMembersAsync(int page, Character target)
        {
            const int MAX_PER_PAGE_I = 10;
            int startAt = page+1; // just in case I need to change the value in runtime
            int current = 0;

            MsgName msg = new MsgName
            {
                Identity = Identity,
                Action = StringAction.MemberList
            };

            foreach (var member in m_dicMembers.Values
                .OrderByDescending(x => x.IsOnline)
                .ThenByDescending(x => x.Rank)
                .ThenByDescending(x => x.Level))
            {
                if (current - startAt >= MAX_PER_PAGE_I)
                    break;
                if (current++ < startAt)
                    continue;

                msg.Strings.Add($"{member.UserName} {member.Level} {(member.IsOnline ? "1" : "0")}");
            }

            await target.SendAsync(msg);
        }

        public async Task SendRelationAsync(Character target)
        {
            foreach (var ally in m_dicAllies.Values)
            {
                await target.SendAsync(new MsgSyndicate
                {
                    Mode = MsgSyndicate.SyndicateRequest.Ally,
                    Identity = ally.Identity
                });
                await target.SendAsync(new MsgName
                {
                    Identity = ally.Identity,
                    Action = StringAction.SetAlly,
                    Strings = new List<string>
                    {
                        ally.Name
                    }
                });
            }

            foreach (var enemy in m_dicEnemies.Values)
            {
                await target.SendAsync(new MsgSyndicate
                {
                    Mode = MsgSyndicate.SyndicateRequest.Enemy,
                    Identity = enemy.Identity
                });
                await target.SendAsync(new MsgName
                {
                    Identity = enemy.Identity,
                    Action = StringAction.SetEnemy,
                    Strings = new List<string>
                    {
                        enemy.Name
                    }
                });
            }
        }

        public async Task BroadcastNameAsync()
        {
            await Kernel.RoleManager.BroadcastMsgAsync(new MsgName
            {
                Action = StringAction.Guild,
                Identity = Identity,
                Strings = new List<string>
                {
                    Name
                }
            });
        }

        public async Task SendAsync(Character user)
        {
            await user.SendAsync(new MsgName
            {
                Action = StringAction.Guild,
                Identity = Identity,
                Strings = new List<string>
                {
                    Name
                }
            });
        }

        public async Task SendAsync(IPacket msg, uint exclude = 0)
        {
            foreach (var player in m_dicMembers.Values)
            {
                if (exclude == player.UserIdentity || !player.IsOnline)
                    continue;
                await player.User.SendAsync(msg);
            }
        }

        #endregion

        #region Database

        public async Task<bool> SaveAsync()
        {
            return await BaseRepository.SaveAsync(m_syndicate);
        }

        public async Task<bool> SoftDeleteAsync()
        {
            m_syndicate.Amount = 0;
            m_syndicate.DelFlag = DateTime.Now;
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync()
        {
            return await BaseRepository.DeleteAsync(m_syndicate);
        }

        #endregion
    }
}