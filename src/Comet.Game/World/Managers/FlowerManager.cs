// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - FlowerManager.cs
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Database.Models;
using Comet.Game.Packets;

#endregion

namespace Comet.Game.World.Managers
{
    public class FlowerManager
    {
        private ConcurrentDictionary<uint, DbFlower> m_dicFlowers = new ConcurrentDictionary<uint, DbFlower>();

        public async Task<bool> InitializeAsync()
        {
            m_dicFlowers = new ConcurrentDictionary<uint, DbFlower>((await DbFlower.GetAsync()).ToDictionary(x => x.UserId));
            return true;
        }

        public List<FlowerRankingStruct> GetFlowerRankingAsync(MsgFlower.FlowerType type, int from = 0, int limit = 10)
        {
            int position = 1;
            switch (type)
            {
                case MsgFlower.FlowerType.RedRose:
                    return m_dicFlowers.Values.Where(x => x.RedRose > 0).OrderByDescending(x => x.RedRose).Skip(from)
                        .Take(limit).Select(x => new FlowerRankingStruct
                        {
                            Identity = x.UserId,
                            Name = x.User.Name,
                            Profession = x.User.Profession,
                            Value = x.RedRose,
                            Position = position++
                        }).ToList();

                case MsgFlower.FlowerType.WhiteRose:
                    return m_dicFlowers.Values.Where(x => x.WhiteRose > 0).OrderByDescending(x => x.WhiteRose)
                        .Skip(from).Take(limit).Select(x => new FlowerRankingStruct
                        {
                            Identity = x.UserId,
                            Name = x.User.Name,
                            Profession = x.User.Profession,
                            Value = x.WhiteRose,
                            Position = position++
                        }).ToList();

                case MsgFlower.FlowerType.Orchid:
                    return m_dicFlowers.Values.Where(x => x.Orchids > 0).OrderByDescending(x => x.Orchids).Skip(from)
                        .Take(limit).Select(x => new FlowerRankingStruct
                        {
                            Identity = x.UserId,
                            Name = x.User.Name,
                            Profession = x.User.Profession,
                            Value = x.Orchids,
                            Position = position++
                        }).ToList();

                case MsgFlower.FlowerType.Tulip:
                    return m_dicFlowers.Values.Where(x => x.Tulips > 0).OrderByDescending(x => x.Tulips).Skip(from)
                        .Take(limit).Select(x => new FlowerRankingStruct
                        {
                            Identity = x.UserId,
                            Name = x.User.Name,
                            Profession = x.User.Profession,
                            Value = x.Tulips,
                            Position = position++
                        }).ToList();
            }

            return new List<FlowerRankingStruct>();
        }

        public DbFlower QueryFlowers(in uint userIdentity)
        {
            if (m_dicFlowers.TryGetValue(userIdentity, out var value))
                return value;
            return m_dicFlowers.TryAdd(userIdentity, value = new DbFlower {UserId = userIdentity}) ? value : null;
        }

        public async Task DailyResetAsync()
        {
            await BaseRepository.DeleteAsync(m_dicFlowers.Values.ToList());
            m_dicFlowers.Clear();
        }

        public struct FlowerRankingStruct
        {
            public int Position;
            public uint Identity;
            public string Name;
            public ushort Profession;
            public uint Value;
        }
    }
}