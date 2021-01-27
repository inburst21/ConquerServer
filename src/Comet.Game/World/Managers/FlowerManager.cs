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

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Packets;

namespace Comet.Game.World.Managers
{
    public static class FlowerManager
    {
        public static async Task<List<FlowerRankingStruct>> GetFlowerRankingAsync(MsgFlower.FlowerType type, int from = 0, int limit = 10)
        {
            DataTable query = await BaseRepository.SelectAsync($"CALL QueryFlowerRanking({(int) type},{limit},{from})");
            List<FlowerRankingStruct> result = new List<FlowerRankingStruct>();

            foreach (DataRow row in query.Rows)
            {
                var item = new FlowerRankingStruct
                {
                    Identity = uint.Parse(row["id"]?.ToString() ?? "0"),
                    Name = row["name"].ToString(),
                    Profession = ushort.Parse(row["profession"]?.ToString() ?? "0"),

                };

                switch (type)
                {
                    case MsgFlower.FlowerType.RedRose:
                        item.Value = uint.Parse(row["rose"]?.ToString() ?? "0");
                        break;
                    case MsgFlower.FlowerType.WhiteRose:
                        item.Value = uint.Parse(row["lily"]?.ToString() ?? "0");
                        break;
                    case MsgFlower.FlowerType.Orchid:
                        item.Value = uint.Parse(row["orchid"]?.ToString() ?? "0");
                        break;
                    case MsgFlower.FlowerType.Tulip:
                        item.Value = uint.Parse(row["tulip"]?.ToString() ?? "0");
                        break;
                }

                result.Add(item);
            }

            return result;
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