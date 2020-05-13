// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
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

        public RoleManager()
        {

        }

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
        }

        public async Task<bool> LoginUserAsync(Client user)
        {
            if (m_userSet.TryGetValue(user.Character.Identity, out var concurrent))
            {
                await Log.WriteLog(LogLevel.Message,
                    $"User {user.Character.Identity} {user.Character.Name} tried to login an already connected client.");

                if (user.IPAddress != concurrent.Client.IPAddress)
                {
                    await concurrent.SendAsync(Language.StrAnotherLoginSameIp, MsgTalk.TalkChannel.Talk);
                }
                else
                {
                    await concurrent.SendAsync(Language.StrAnotherLoginOtherIp, MsgTalk.TalkChannel.Talk);
                }

                user.Disconnect();
                await KickoutAsync(user.Character.Identity, "logged twice");
                return false;
            }

            if (m_userSet.Count > Kernel.Configuration.MaxConn && user.AccountIdentity >= 10000 &&
                !user.Character.IsGm())
            {
                await user.SendAsync(new MsgConnectEx(MsgConnectEx.RejectionCode.ServerFull));
                await KickoutAsync(user.Character.Identity, "server is full");
                return false;
            }

            m_userSet.TryAdd(user.Character.Identity, user.Character);
            m_roleSet.TryAdd(user.Character.Identity, user.Character);

            await user.Character.SetLoginAsync();

#if DEBUG
            await user.Character.SendAsync($"Server is running in DEBUG mode. Version: [{Kernel.SERVER_VERSION}]{Kernel.Version}");
#endif

            if (OnlinePlayers > MaxOnlinePlayers)
                MaxOnlinePlayers = OnlinePlayers;
            return true;
        }

        public bool LogoutUser(uint idUser)
        {
            if (m_userSet.TryRemove(idUser, out var user))
            {
                m_roleSet.TryRemove(idUser, out _);
                try
                {
                    user.OnDisconnectAsync().Forget();
                    user.Client.Disconnect();
                }
                catch
                {
                    // just in case the user is already disconnected and we receive an exception
                }

                return true;
            }

            return false;
        }

        public async Task KickoutAsync(uint idUser, string reason = "")
        {
            if (m_userSet.TryGetValue(idUser, out var user))
            {
                await user.SendAsync(string.Format(Language.StrKickout, reason), MsgTalk.TalkChannel.Talk, Color.White);
                user.Client.Disconnect();

                _ = Log.WriteLog(LogLevel.Message, $"User {user.Name} has been kicked: {reason}");
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
            m_mapItemSet.TryRemove(idRole, out _);
            return m_roleSet.TryGetValue(idRole, out var role) ? role : null;
        }

        /// <summary>
        ///     Attention, DO NOT USE to remove <see cref="Character" />.
        /// </summary>
        public bool RemoveRole(uint idRole)
        {
            return m_roleSet.TryRemove(idRole, out _);
        }

        public async Task OnUserTimerAsync()
        {
            foreach (var user in m_userSet.Values)
            {
                try
                {
                    await user.OnTimerAsync();
                }
                catch (Exception ex)
                {
                    await Log.WriteLog("OnUserTimer", LogLevel.Exception, $"Exception thrown: {ex.Message}\n{ex}");
                }
            }
        }

        public async Task OnRoleTimerAsync()
        {
            
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
            return (uint) (prof << (32 + level));
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
            return m_dicLevExp.Count;
        }
    }
}