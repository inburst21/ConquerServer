// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - TimedGuildWar.cs
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
using Comet.Game.States.Syndicates;
using Comet.Game.World.Maps;

namespace Comet.Game.States.Events
{
    public sealed class TimedGuildWar : GameEvent
    {
        private ConcurrentDictionary<uint, SyndicateData> m_synData = new ConcurrentDictionary<uint, SyndicateData>();

        private const int MAP_ID = 0;

        public TimedGuildWar()
            : base("GuildWar", 800)
        {
        }

        public override EventType Identity { get; } = EventType.TimedGuildWar;

        public GameMap Map { get; private set; }

        public override async Task<bool> CreateAsync()
        {
            Map = Kernel.MapManager.GetMap(MAP_ID);
            if (Map == null)
                return false;
            return true;
        }

        public void AddPoints(int amount, uint synId)
        {
            Syndicate syn = Kernel.SyndicateManager.GetSyndicate((int) synId);
            if (syn == null || syn.Deleted)
                return;

            if (!m_synData.TryGetValue(synId, out var synData))
            {
                synData = new SyndicateData
                {
                    Identity = syn.Identity,
                    Name = syn.Name
                };
                m_synData.TryAdd(syn.Identity, synData);
            }

            synData.Points = Math.Max(0, synData.Points + amount);

            if (amount > 0)
            {
                synData.TotalPoints += amount;

                foreach (var user in Kernel.RoleManager.QueryUserSetByMap(Map.Identity)
                    .Where(x => x.SyndicateIdentity == synId))
                {
                    if (!synData.Players.TryGetValue(user.Identity, out var userData))
                    {
                        userData = new UserData
                        {
                            Identity = user.Identity,
                            Name = user.Name,
                            Kills = 0,
                            Deaths = 0,
                            Points = 0
                        };
                        synData.Players.Add(userData.Identity, userData);
                    }

                    userData.Points += amount;
                }
            }
        }

        private class UserData
        {
            public uint Identity { get; set; }
            public string Name { get; set; }
            public long Points { get; set; }
            public int Kills { get; set; }
            public int Deaths { get; set; }
        }

        private class SyndicateData
        {
            public uint Identity { get; set; }
            public string Name { get; set; }
            public long Points { get; set; }
            public long TotalPoints { get; set; }

            public Dictionary<uint, UserData> Players = new Dictionary<uint, UserData>();
        }
    }
}