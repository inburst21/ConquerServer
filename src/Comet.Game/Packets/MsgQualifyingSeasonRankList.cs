// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgQualifyingSeasonRankList.cs
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
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgQualifyingSeasonRankList : MsgBase<Client>
    {
        public int Count { get; set; }
        public List<QualifyingSeasonRankStruct> Members = new List<QualifyingSeasonRankStruct>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            Count = reader.ReadInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) PacketType.MsgQualifyingSeasonRankList);
            writer.Write(Count = Members.Count);
            foreach (var member in Members)
            {
                writer.Write(member.Identity);
                writer.Write(member.Name, 16);
                writer.Write(member.Mesh);
                writer.Write(member.Level);
                writer.Write(member.Profession);
                // writer.Write(member.Unknown);
                writer.Write(member.Rank);
                writer.Write(member.Score);
                writer.Write(member.Win);
                writer.Write(member.Lose);
            }
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Members.Add(new QualifyingSeasonRankStruct
            {
                Identity = 1000506,
                Name = "Ninja",
                Rank = 1,
                Level = 6,
                Win = 100,
                Lose = 70,
                Profession = 55,
                Mesh = 2012001,
                Score = 5898
            });
            Members.Add(new QualifyingSeasonRankStruct
            {
                Identity = 1000002,
                Name = "Felipe[PM]",
                Rank = 2,
                Level = 140,
                Win = 100,
                Lose = 70,
                Profession = 15,
                Mesh = 591003,
                Score = 5546
            });
            Members.Add(new QualifyingSeasonRankStruct
            {
                Identity = 1000003,
                Name = "Archer[PM]",
                Rank = 3,
                Level = 140,
                Win = 100,
                Lose = 70,
                Profession = 45,
                Mesh = 12001,
                Score = 4896
            });
            Members.Add(new QualifyingSeasonRankStruct
            {
                Identity = 1000505,
                Name = "Fire[PM]",
                Rank = 4,
                Level = 140,
                Win = 100,
                Lose = 70,
                Profession = 145,
                Mesh = 11003,
                Score = 4654
            });
            await client.SendAsync(this);
        }

        public struct QualifyingSeasonRankStruct
        {
            public uint Identity { get; set; }
            public string Name { get; set; }
            public uint Mesh { get; set; }
            public int Level { get; set; }
            public int Profession { get; set; }
            public int Unknown { get; set; }
            public int Rank { get; set; }
            public int Score { get; set; }
            public int Win { get; set; }
            public int Lose { get; set; }
        }
    }
}