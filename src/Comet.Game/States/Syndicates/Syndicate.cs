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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States.Events;
using Comet.Network.Packets;
using Microsoft.EntityFrameworkCore;

namespace Comet.Game.States.Syndicates
{
    public sealed class Syndicate
    {
        private const int MEMBER_MIN_LEVEL = 15;
        private const int MAX_MEMBER_SIZE = 800;
        private const int DISBAND_MONEY = 100000;
        private const int SYNDICATE_ACTION_COST = 50000;
        private const int EXIT_MONEY = 20000;
        private const string DEFAULT_ANNOUNCEMENT_S = "This is a new guild.";
        private const int MAX_ALLY = 5;
        private const int MAX_ENEMY = 5;

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

        public uint Money
        {
            get => (uint)m_syndicate.Money;
            set => m_syndicate.Money = value;
        }


        public uint ConquerPoints
        {
            get => m_syndicate.ConquerPoints;
            set => m_syndicate.ConquerPoints = value;
        }

        public string Announce
        {
            get => m_syndicate.Announce;
            set => m_syndicate.Announce = value;
        }

        public DateTime AnnounceDate
        {
            get => m_syndicate.AnnounceDate;
            set => m_syndicate.AnnounceDate = value;
        }

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

            m_leader.Donation = investment / 2;
            await m_leader.SaveAsync();

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

        #region Disband

        public async Task<bool> DisbandAsync(Character user)
        {
            if (user.Identity != Leader.UserIdentity)
                return false;

            if (MemberCount > 1)
                return false;

            if (Money < DISBAND_MONEY)
            {
                return false;
            }

            user.Syndicate = null;

            if (m_dicMembers.TryRemove(user.Identity, out var member))
            {
                await BaseRepository.DeleteAsync(member);
            }

            ExitFromEvents();

            // additional clean up
            await new ServerDbContext().Database.ExecuteSqlRawAsync($"DELETE FROM `cq_synattr` WHERE `syn_id`={Identity}");

            await user.SendAsync(new MsgSyndicate
            {
                Identity = Identity,
                Mode = MsgSyndicate.SyndicateRequest.Disband
            });

            await user.Screen.SynchroScreenAsync();
            await Kernel.RoleManager.BroadcastMsgAsync(string.Format(Language.StrSynDestroy, Name));
            return await SoftDeleteAsync();
        }

        #endregion

        #region Member Management

        public async Task<bool> AppendMemberAsync(Character target, Character caller, JoinMode mode)
        {
            if ((mode == JoinMode.Invite || mode == JoinMode.Request) && caller == null)
            {
                return false;
            }

            if (target.SyndicateIdentity != 0)
                return false;

            if (target.Level < MEMBER_MIN_LEVEL)
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await caller.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            var newMember = new SyndicateMember();
            if (!await newMember.CreateAsync(target, this, SyndicateMember.SyndicateRank.Member))
                return false;

            if (!m_dicMembers.TryAdd(newMember.UserIdentity, newMember))
            {
                await newMember.DeleteAsync();
                return false;
            }

            target.Syndicate = this;

            await target.SendSyndicateAsync();
            await SendAsync(target);
            await SendRelationAsync(target);
            await target.Screen.SynchroScreenAsync();

            m_syndicate.Amount = (uint) MemberCount;
            await SaveAsync();

            switch (mode)
            {
                case JoinMode.Invite:
                    await SendAsync(string.Format(Language.StrSynInviteGuild, caller.SyndicateRank, caller.Name, target.Name));
                    break;
                case JoinMode.Request:
                    await SendAsync(string.Format(Language.StrSynJoinGuild, caller.SyndicateRank, caller.Name, target.Name));
                    break;
                case JoinMode.Recruitment:
                    break;
            }

            return true;
        }

        public async Task<bool> QuitSyndicateAsync(Character target)
        {
            if (target.SyndicateRank == SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (!m_dicMembers.TryGetValue(target.Identity, out var member))
                return false;

            if (member.Donation < EXIT_MONEY)
            {
                await target.SendAsync(string.Format(Language.StrSynExitNotEnoughMoney, EXIT_MONEY));
                return false;
            }

            m_dicMembers.TryRemove(target.Identity, out _);

            target.Syndicate = null;

            await target.SendAsync(new MsgSyndicate
            {
                Identity = Identity,
                Mode = MsgSyndicate.SyndicateRequest.Disband
            });

            m_syndicate.Amount = (uint) MemberCount;
            await SaveAsync();

            await target.Screen.SynchroScreenAsync();

            await BaseRepository.SaveAsync(new DbSyndicateMemberHistory
            {
                UserIdentity = member.UserIdentity,
                JoinDate = member.JoinDate,
                LeaveDate = DateTime.Now,
                SyndicateIdentity = Identity,
                Rank = (ushort)member.Rank,
                Silver = member.Donation,
                ConquerPoints = 0,
                Guide = 0,
                PkPoints = 0
            });

            RemoveUserFromEvents(target.Identity);

            await member.DeleteAsync();
            await SendAsync(string.Format(Language.StrSynMemberExit, target.Name));
            return true;
        }

        public async Task<bool> KickoutMemberAsync(Character sender, string name)
        {
            if (sender.SyndicateRank < SyndicateMember.SyndicateRank.DeputyLeader)
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await sender.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            var member = QueryMember(name);
            if (member == null)
                return false;

            if (member.Rank == SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (!m_dicMembers.TryRemove(member.UserIdentity, out _))
                return false;

            Character target = member.User;
            if (target != null)
            {
                target.Syndicate = null;
                await target.SendAsync(new MsgSyndicate
                {
                    Identity = Identity,
                    Mode = MsgSyndicate.SyndicateRequest.Disband
                });
                await target.Screen.SynchroScreenAsync();
                await target.SendAsync(string.Format(Language.StrSynYouBeenKicked, sender.Name));
            }

            RemoveUserFromEvents(member.UserIdentity);

            await BaseRepository.SaveAsync(new DbSyndicateMemberHistory
            {
                UserIdentity = member.UserIdentity,
                JoinDate = member.JoinDate,
                LeaveDate = DateTime.Now,
                SyndicateIdentity = Identity,
                Rank = (ushort) member.Rank,
                Silver = member.Donation,
                ConquerPoints = 0,
                Guide = 0,
                PkPoints = 0
            });

            m_syndicate.Amount = (uint)MemberCount;
            await SaveAsync();
            await member.DeleteAsync();
            await SendAsync(string.Format(Language.StrSynMemberKickout, sender.SyndicateRank, sender.Name, member.UserName));
            return true;
        }

        #endregion

        #region Promote and Demote

        public Task<bool> PromoteAsync(Character sender, string target, SyndicateMember.SyndicateRank position)
        {
            Character user = Kernel.RoleManager.GetUser(target);
            if (user == null || user.SyndicateIdentity != sender.SyndicateIdentity)
                return Task.FromResult(false);
            return PromoteAsync(sender, user, position);
        }

        public Task<bool> PromoteAsync(Character sender, uint target, SyndicateMember.SyndicateRank position)
        {
            Character user = Kernel.RoleManager.GetUser(target);
            if (user == null || user.SyndicateIdentity != sender.SyndicateIdentity)
                return Task.FromResult(false);
            return PromoteAsync(sender, user, position);
        }

        public async Task<bool> PromoteAsync(Character sender, Character target, SyndicateMember.SyndicateRank position)
        {
            if (target.SyndicateRank == SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (target.SyndicateIdentity != Identity)
                return false;

            if (sender.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await sender.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            switch (position)
            {
                case SyndicateMember.SyndicateRank.DeputyLeader:
                    if (DeputyLeaderCount >= 5)
                        return false;
                    break;
            }

            if (position == SyndicateMember.SyndicateRank.GuildLeader) // abdicate
            {
                sender.SyndicateMember.Rank = SyndicateMember.SyndicateRank.Member;
                await sender.SendSyndicateAsync();
                await sender.Screen.SynchroScreenAsync();
                await sender.SyndicateMember.SaveAsync();

                await SendAsync(string.Format(Language.StrSynAbdicate, sender.Name, target.Name));
            }
            else
            {
                await SendAsync(string.Format(Language.StrSynPromoted, sender.SyndicateRank, sender.Name, target.Name,
                    position));
            }

            target.SyndicateMember.Rank = position;
            await target.SendSyndicateAsync();
            await target.Screen.SynchroScreenAsync();
            await target.SyndicateMember.SaveAsync();

            return true;
        }

        public Task<bool> DemoteAsync(Character sender, string name)
        {
            var member = QueryMember(name);
            if (member == null)
                return Task.FromResult(false);
            return DemoteAsync(sender, member);
        }

        public Task<bool> DemoteAsync(Character sender, uint target)
        {
            var member = QueryMember(target);
            if (member == null)
                return Task.FromResult(false);
            return DemoteAsync(sender, member);
        }

        public async Task<bool> DemoteAsync(Character sender, SyndicateMember member)
        {
            if (sender.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (member.Rank == SyndicateMember.SyndicateRank.GuildLeader)
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await sender.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            member.Rank = SyndicateMember.SyndicateRank.Member;
            if (member.User != null)
            {
                await member.User.SendSyndicateAsync();
                await member.User.Screen.SynchroScreenAsync();
            }
            await member.SaveAsync();
            return true;
        }

        #endregion

        #region Alliance

        public int AlliesCount => m_dicAllies.Count;
        

        public async Task<bool> CreateAllianceAsync(Character user, Syndicate target)
        {
            if (user.SyndicateIdentity != Identity)
                return false;

            if (user.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
            {
                await user.SendAsync(Language.StrSynYouNoLeader);
                return false;
            }

            if (IsAlly(target.Identity) || IsEnemy(target.Identity))
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await user.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            if (AlliesCount >= MAX_ALLY)
            {
                return false;
            }

            await BaseRepository.SaveAsync(new DbSyndicateAllies
            {
                AllyIdentity = Identity,
                AllyName = Name,
                SyndicateIdentity = target.Identity,
                SyndicateName = target.Name,
                EstabilishDate = DateTime.Now
            });

            await BaseRepository.SaveAsync(new DbSyndicateAllies
            {
                SyndicateIdentity = Identity,
                SyndicateName = Name,
                AllyIdentity = target.Identity,
                AllyName = target.Name,
                EstabilishDate = DateTime.Now
            });

            await AddAllyAsync(target);
            await target.AddAllyAsync(this);

            await SendAsync(string.Format(Language.StrSynAllyAdd, user.Name, target.Name));
            await target.SendAsync(string.Format(Language.StrSynAllyAdd, target.Leader.UserName, Name));

            return true;
        }

        public async Task<bool> DisbandAllianceAsync(Character user, Syndicate target)
        {
            if (user.SyndicateIdentity != Identity)
                return false;

            if (user.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
            {
                await user.SendAsync(Language.StrSynYouNoLeader);
                return false;
            }

            if (!IsAlly(target.Identity))
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await user.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            await SyndicateAllyRepository.DeleteAsync(Identity, target.Identity);

            await RemoveAllyAsync(target.Identity);
            await target.RemoveAllyAsync(Identity);

            await SendAsync(string.Format(Language.StrSynAllyRemove, user.Name, target.Name));
            await target.SendAsync(string.Format(Language.StrSynAllyRemoved, user.Name, target.Name));
            return true;
        }

        public async Task AddAllyAsync(Syndicate target)
        {
            m_dicAllies.TryAdd(target.Identity, target);
            await SendAsync(new MsgSyndicate
            {
                Identity = target.Identity,
                Mode = MsgSyndicate.SyndicateRequest.Ally
            });
            await SendAsync(new MsgName
            {
                Identity = target.Identity,
                Action = StringAction.SetAlly,
                Strings = new List<string>
                {
                    target.Name
                }
            });
        }

        public async Task RemoveAllyAsync(uint idAlly)
        {
            m_dicAllies.TryRemove((ushort) idAlly, out _);
            await SendAsync(new MsgSyndicate
            {
                Identity = idAlly,
                Mode = MsgSyndicate.SyndicateRequest.Unally
            });
        }

        public bool IsAlly(uint id)
        {
            return m_dicAllies.ContainsKey((ushort) id);
        }

        #endregion

        #region Enemies

        public int EnemyCount => m_dicEnemies.Count;

        public async Task<bool> AntagonizeAsync(Character user, Syndicate target)
        {
            if (user.SyndicateIdentity != Identity)
                return false;

            if (user.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
            {
                await user.SendAsync(Language.StrSynYouNoLeader);
                return false;
            }

            if (IsAlly(target.Identity) || IsEnemy(target.Identity))
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await user.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            if (EnemyCount >= MAX_ENEMY)
            {
                // ? message
                return false;
            }

            if (!await BaseRepository.SaveAsync(new DbSyndicateEnemy
            {
                SyndicateIdentity = Identity,
                SyndicateName = Name,
                EnemyIdentity = target.Identity,
                EnemyName = target.Name,
                EstabilishDate = DateTime.Now
            }))
                return false;

            m_dicEnemies.TryAdd(target.Identity, target);

            await SendAsync(new MsgSyndicate
            {
                Identity = target.Identity,
                Mode = MsgSyndicate.SyndicateRequest.Enemy
            });
            await SendAsync(new MsgName
            {
                Identity = target.Identity,
                Action = StringAction.SetEnemy,
                Strings = new List<string>
                {
                    target.Name
                }
            });

            await SendAsync(string.Format(Language.StrSynAddEnemy, user.Name, target.Name));
            await target.SendAsync(string.Format(Language.StrSynAddedEnemy, user.Name, Name));
            return true;
        }

        public async Task<bool> PeaceAsync(Character user, Syndicate target)
        {
            if (user.SyndicateIdentity != Identity)
                return false;

            if (user.SyndicateRank != SyndicateMember.SyndicateRank.GuildLeader)
            {
                await user.SendAsync(Language.StrSynYouNoLeader);
                return false;
            }

            if (!IsEnemy(target.Identity))
                return false;

            if (Money < SYNDICATE_ACTION_COST)
            {
                await user.SendAsync(string.Format(Language.StrSynNoMoney, SYNDICATE_ACTION_COST));
                return false;
            }

            m_dicEnemies.TryRemove(target.Identity, out _);
            await SyndicateEnemyRepository.DeleteAsync(Identity, target.Identity);

            await SendAsync(new MsgSyndicate
            {
                Identity = target.Identity,
                Mode = MsgSyndicate.SyndicateRequest.Unenemy
            });

            await SendAsync(string.Format(Language.StrSynRemoveEnemy, user.Name, target.Name));
            await target.SendAsync(string.Format(Language.StrSynRemovedEnemy, user.Name, Name));

            return true;
        }

        public bool IsEnemy(uint id)
        {
            return m_dicEnemies.ContainsKey((ushort)id);
        }

        #endregion

        #region Events

        public void ExitFromEvents()
        {
            Kernel.EventThread.GetEvent<TimedGuildWar>()?.UnsubscribeSyndicate(Identity);
        }

        public void RemoveUserFromEvents(uint idUser)
        {
            Kernel.EventThread.GetEvent<TimedGuildWar>()?.LeaveSyndicate(idUser, Identity);
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

        public async Task SendAsync(string message, uint idIgnore = 0u, Color? color = null)
        {
            await SendAsync(new MsgTalk(0, MsgTalk.TalkChannel.Guild, color ?? Color.White, message), idIgnore);
        }

        public async Task SendAsync(IPacket msg, uint exclude = 0u)
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

        public enum JoinMode
        {
            Invite,
            Request,
            Recruitment
        }
    }
}