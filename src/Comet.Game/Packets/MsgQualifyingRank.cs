// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgQualifyingRank.cs
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgQualifyingRank : MsgBase<Client>
    {
        public QueryRankType RankType { get; set; }
        public ushort PageNumber { get; set; }
        public int RankingNum { get; set; }
        public int Count { get; set; }
        public List<PlayerDataStruct> Players = new List<PlayerDataStruct>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            RankType = (QueryRankType) reader.ReadUInt16();
            PageNumber = reader.ReadUInt16();
            RankingNum = reader.ReadInt32();
            Count = reader.ReadInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgQualifyingRank);
            writer.Write((ushort) RankType);
            writer.Write(PageNumber);
            writer.Write(RankingNum = Players.Count);
            writer.Write(Count = Players.Count);
            foreach (var player in Players)
            {
                writer.Write(player.Rank);
                writer.Write(player.Name, 16);
                writer.Write(player.Type);
                writer.Write(player.Points);
                writer.Write(player.Profession);
                writer.Write(player.Level);
                // writer.Write(player.Unknown);
            }
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            switch (RankType)
            {
                case QueryRankType.QualifierRank:
                {
                    List<DbArenic> players = await DbArenic.GetRankAsync(PageNumber * 10, 10);
                    int rank = PageNumber * 10;
                    foreach (var player in players)
                    {
                        Players.Add(new PlayerDataStruct
                        {
                            Rank = (ushort)rank++,
                            Name = player.User.Name,
                            Type = 0,
                            Level = player.User.Level,
                            Profession = player.User.Profession,
                            Points = player.User.AthleteHistoryHonorPoints,
                            Unknown = 0
                        });
                    }

                    await client.SendAsync(this);
                    break;
                }
                case QueryRankType.HonorHistory:
                {
                    List<DbCharacter> players = await DbCharacter.GetHonorRankAsync(PageNumber * 10, 10);
                    int rank = PageNumber * 10;
                    foreach (var player in players)
                    {
                        Players.Add(new PlayerDataStruct
                        {
                            Rank = (ushort) rank++,
                            Name = player.Name,
                            Type = 6004,
                            Level = player.Level,
                            Profession = player.Profession,
                            Points = player.AthleteHistoryHonorPoints,
                            Unknown = 0
                        });
                    }

                    await client.SendAsync(this);
                    break;
                }
            }
        }

        public struct PlayerDataStruct
        {
            public ushort Rank;
            public string Name;
            public ushort Type;
            public uint Points;
            public int Profession;
            public int Level;
            public int Unknown;
        }

        public enum QueryRankType : ushort
        {
            QualifierRank,
            HonorHistory
        }
    }
}