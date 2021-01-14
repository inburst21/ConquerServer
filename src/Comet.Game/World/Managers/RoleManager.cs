﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Role Manager.cs
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
using Comet.Core;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.States.Magics;
using Comet.Network.Packets;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.World.Managers
{
    public sealed class RoleManager
    {
        private readonly ConcurrentDictionary<uint, Character> m_userSet = new ConcurrentDictionary<uint, Character>();
        private readonly ConcurrentDictionary<uint, Role> m_roleSet = new ConcurrentDictionary<uint, Role>();
        private readonly ConcurrentDictionary<uint, MapItem> m_mapItemSet = new ConcurrentDictionary<uint, MapItem>();

        private readonly Dictionary<byte, DbLevelExperience> m_dicLevExp = new Dictionary<byte, DbLevelExperience>();
        private readonly Dictionary<uint, DbPointAllot> m_dicPointAllot = new Dictionary<uint, DbPointAllot>();
        private readonly Dictionary<uint, DbMonstertype> m_dicMonstertype = new Dictionary<uint, DbMonstertype>();
        private readonly Dictionary<uint, List<DbMonsterMagic>> m_dicMonsterMagics = new Dictionary<uint, List<DbMonsterMagic>>();
        private Dictionary<uint, DbSuperman> m_superman = new Dictionary<uint, DbSuperman>();

        private readonly List<DbRebirth> m_dicRebirths = new List<DbRebirth>();
        private readonly List<MagicTypeOp> m_magicOps = new List<MagicTypeOp>();

        private TimeOutMS m_userUpdate = new TimeOutMS(500);

        private bool m_isShutdown = false;

        public RoleManager()
        {

        }

        public int OnlineUniquePlayers => m_userSet.Values.Select(x => x.Client.IPAddress).Distinct().Count();
        public int OnlinePlayers => m_userSet.Count;
        public int RolesCount => m_roleSet.Count;

        public int MaxOnlinePlayers { get; private set; }

        public async Task InitializeAsync()
        {
            foreach (var auto in await PointAllotRepository.GetAsync())
            {
                m_dicPointAllot.TryAdd(AllotIndex(auto.Profession, auto.Level), auto);
            }

            foreach (var mob in await MonsterypeRepository.GetAsync())
            {
                m_dicMonstertype.TryAdd(mob.Id, mob);
            }

            foreach (var lev in await LevelExperienceRepository.GetAsync())
            {
                m_dicLevExp.TryAdd(lev.Level, lev);
            }

            m_dicRebirths.AddRange(await RebirthRepository.GetAsync());

            foreach (var dbOp in await MagictypeOpRepository.GetAsync())
            {
                m_magicOps.Add(new MagicTypeOp(dbOp));
            }

            foreach (var magic in await DbMonsterMagic.GetAsync())
            {
                if (!m_dicMonsterMagics.ContainsKey(magic.OwnerIdentity))
                    m_dicMonsterMagics.TryAdd(magic.OwnerIdentity, new List<DbMonsterMagic>());
                m_dicMonsterMagics[magic.OwnerIdentity].Add(magic);
            }

            m_superman = (await DbSuperman.GetAsync()).ToDictionary(superman => superman.UserIdentity);

            m_userUpdate.Update();
        }

        public async Task<bool> LoginUserAsync(Client user)
        {
            if (m_isShutdown)
            {
                await user.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ServerDown));
                user.Disconnect();
                return false;
            }

            if (m_userSet.TryGetValue(user.Character.Identity, out var concurrent))
            {
                await Log.WriteLogAsync(LogLevel.Message,
                    $"User {user.Character.Identity} {user.Character.Name} tried to login an already connected client.");

                if (user.IPAddress != concurrent.Client.IPAddress)
                {
                    await concurrent.SendAsync(Language.StrAnotherLoginSameIp, MsgTalk.TalkChannel.Talk);
                }
                else
                {
                    await concurrent.SendAsync(Language.StrAnotherLoginOtherIp, MsgTalk.TalkChannel.Talk);
                }

                concurrent.Client.Disconnect();
                user.Disconnect();
                //await KickOutAsync(user.Character.Identity, "logged twice");
                return false;
            }

            if (m_userSet.Count > Kernel.Configuration.MaxConn && user.AccountIdentity >= 10000 &&
                !user.Character.IsGm())
            {
                await user.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ServerFull));
                await Log.WriteLogAsync(LogLevel.Warning, $"{user.Character.Name} tried to login and server is full.");
                user.Disconnect();
                return false;
            }

            m_userSet.TryAdd(user.Character.Identity, user.Character);
            m_roleSet.TryAdd(user.Character.Identity, user.Character);

            await user.Character.SetLoginAsync();

            await Log.WriteLogAsync(LogLevel.Message, $"{user.Character.Name} has logged in.");

            if (OnlinePlayers > MaxOnlinePlayers)
                MaxOnlinePlayers = OnlinePlayers;
            return true;
        }

        public void ForceLogoutUser(uint idUser)
        {
            m_userSet.TryRemove(idUser, out _);
            m_roleSet.TryRemove(idUser, out _);
        }

        public async Task KickOutAsync(uint idUser, string reason = "")
        {
            if (m_userSet.TryGetValue(idUser, out var user))
            {
                await user.SendAsync(string.Format(Language.StrKickout, reason), MsgTalk.TalkChannel.Talk, Color.White);
                user.Client.Disconnect();
                await Log.WriteLogAsync(LogLevel.Message, $"User {user.Name} has been kicked: {reason}");
            }
        }

        public async Task KickOutAllAsync(string reason = "", bool isShutdown = false)
        {
            if (isShutdown)
                m_isShutdown = true;

            foreach (var user in m_userSet.Values)
            {
                await user.SendAsync(string.Format(Language.StrKickout, reason), MsgTalk.TalkChannel.Talk, Color.White);
                user.Client.Disconnect();

                await Log.WriteLogAsync(LogLevel.Message, $"User {user.Name} has been kicked (kickoutall): {reason}");
            }
        }

        public Character GetUser(uint idUser)
        {
            return m_userSet.TryGetValue(idUser, out var client) ? client : null;
        }

        public Character GetUser(string name)
        {
            return m_userSet.Values.FirstOrDefault(x => x.Name == name);
        }

        public List<Character> QueryUserSetByMap(uint idMap)
        {
            return m_userSet.Values.Where(x => x.MapIdentity == idMap).ToList();
        }

        /// <summary>
        ///     Attention, DO NOT USE to add <see cref="Character" />.
        /// </summary>
        public bool AddRole(Role role)
        {
            if (role is MapItem item)
                m_mapItemSet.TryAdd(role.Identity, item);
            return m_roleSet.TryAdd(role.Identity, role);
        }

        public Role GetRole(uint idRole)
        {
            return m_roleSet.TryGetValue(idRole, out var role) ? role : null;
        }

        public T GetRole<T>(uint idRole) where T : Role
        {
            return m_roleSet.TryGetValue(idRole, out var role) ? role as T : null;
        }

        /// <summary>
        ///     Attention, DO NOT USE to remove <see cref="Character" />.
        /// </summary>
        public bool RemoveRole(uint idRole)
        {
            m_mapItemSet.TryRemove(idRole, out _);
            return m_roleSet.TryRemove(idRole, out _);
        }
        
        public async Task OnUserTimerAsync()
        {
            foreach (var (_, value) in m_userSet)
            {
                try
                {
                    if (value != null)
                    {
                        await value.OnBattleTimerAsync();
                        if (m_userUpdate.ToNextTime())
                            await value.OnTimerAsync();
                    }
                }
                catch (Exception ex)
                {
                    await Log.WriteLogAsync("OnUserTimer", LogLevel.Exception, $"Exception thrown: {ex.Message}\n{ex}");
                }
            }
        }

        public async Task OnRoleTimerAsync()
        {
            foreach (var item in m_mapItemSet.Values)
            {
                if (item.CanDisappear())
                {
                    await item.DisappearAsync();
                    m_mapItemSet.TryRemove(item.Identity, out _);
                }
            }

            foreach (var trap in m_roleSet.Values.Where(x => x is MapTrap).Cast<MapTrap>())
            {
                await trap.OnTimerAsync();
            }
        }
        
        public async Task BroadcastMsgAsync(string message, MsgTalk.TalkChannel channel = MsgTalk.TalkChannel.System,
            Color? color = null)
        {
            foreach (var user in m_userSet.Values)
            {
                await user.SendAsync(message, channel, color);
            }
        }

        public async Task BroadcastMsgAsync(IPacket msg, uint ignore = 0)
        {
            foreach (var user in m_userSet.Values)
            {
                if (user.Identity == ignore) continue;
                await user.SendAsync(msg);
            }
        }

        public DbPointAllot GetPointAllot(ushort profession, ushort level)
        {
            return m_dicPointAllot.TryGetValue(AllotIndex(profession, level), out var point) ? point : null;
        }

        private uint AllotIndex(ushort prof, ushort level)
        {
            return (uint) ((prof << 16) + level);
        }

        public DbMonstertype GetMonstertype(uint type)
        {
            return m_dicMonstertype.TryGetValue(type, out var mob) ? mob : null;
        }

        public DbLevelExperience GetLevelExperience(byte level)
        {
            return m_dicLevExp.TryGetValue(level, out var value) ? value : null;
        }

        public int GetLevelLimit()
        {
            return m_dicLevExp.Count + 1;
        }

        public DbRebirth GetRebirth(int profNow, int profNext, int currMete)
        {
            profNow = (profNow / 10 * 1000) + profNow % 10;
            profNext = (profNext / 10 * 1000) + profNext % 10;
            return m_dicRebirths.FirstOrDefault(x => x.NeedProfession == profNow && x.NewProfession == profNext && x.Metempsychosis == currMete);
        }

        public MagicTypeOp GetMagictypeOp(MagicTypeOp.MagictypeOperation op, int profNow, int profNext, int metempsychosis)
        {
            return m_magicOps.FirstOrDefault(x => x.ProfessionAgo == profNow && x.ProfessionNow == profNext && x.RebirthTime == metempsychosis && x.Operation == op);
        }

        public List<T> QueryRoleByMap<T>(uint idMap) where T : Role
        {
            return m_roleSet.Values.Where(x => x.MapIdentity == idMap && x is T).Cast<T>().ToList();
        }

        public List<T> QueryRoleByType<T>() where T : Role
        {
            return m_roleSet.Values.Where(x => x is T).Cast<T>().ToList();
        }

        public List<DbMonsterMagic> GetMonsterMagics(uint roleType)
        {
            return m_dicMonsterMagics.TryGetValue(roleType, out var result) ? result : new List<DbMonsterMagic>();
        }

        public Task AddOrUpdateSupermanAsync(uint idUser, int amount)
        {
            if (!m_superman.TryGetValue(idUser, out var superman))
            {
                m_superman.Add(idUser, superman = new DbSuperman
                {
                    UserIdentity = idUser
                });
            }

            superman.Amount = (uint) amount;
            return BaseRepository.SaveAsync(superman);
        }

        public int GetSupermanPoints(uint idUser)
        {
            return (int) (m_superman.TryGetValue(idUser, out var value) ? value.Amount : 0);
        }

        public int GetSupermanRank(uint idUser)
        {
            int result = 1;
            foreach (var super in m_superman.Values.OrderByDescending(x => x.Amount))
            {
                if (super.UserIdentity == idUser)
                    return result;
                result++;
            }
            
            return result;
        }
    }
}