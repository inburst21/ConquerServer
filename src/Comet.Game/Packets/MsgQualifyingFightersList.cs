// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgQualifyingFightersList.cs
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
    public sealed class MsgQualifyingFightersList : MsgBase<Client>
    {
        public int Page  {  get; set; }
        public int Unknown8 { get; set; }
        public int MatchesCount { get; set; }
        public int FightersNum { get; set; }
        public int Unknown20 { get; set; }
        public int Count { get; set; }
        public List<FightStruct> Fights { get; set; } = new List<FightStruct>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            Page = reader.ReadInt32(); // 4
            Unknown8 = reader.ReadInt32(); // 8
            MatchesCount = reader.ReadInt32(); // 12
            FightersNum = reader.ReadInt32(); // 16
            Unknown20 = reader.ReadInt32(); // 20
            Count = reader.ReadInt32(); // 24
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgQualifyingFightersList);
            writer.Write(Page);
            writer.Write(Unknown8);
            writer.Write(MatchesCount);
            writer.Write(FightersNum);
            writer.Write(Unknown20);
            writer.Write(Count = Fights.Count);
            foreach (var fight in Fights)
            {
                writer.Write(fight.Fighter0.Identity);
                writer.Write(fight.Fighter0.Mesh);
                writer.Write(fight.Fighter0.Name, 16);
                writer.Write(fight.Fighter0.Level);
                writer.Write(fight.Fighter0.Profession);
                //writer.Write(fight.Fighter0.Unknown);
                writer.Write(fight.Fighter0.Rank);
                writer.Write(fight.Fighter0.Points);
                writer.Write(fight.Fighter0.WinsToday);
                writer.Write(fight.Fighter0.LossToday);
                writer.Write(fight.Fighter0.CurrentHonor);
                writer.Write(fight.Fighter0.TotalHonor);

                writer.Write(fight.Fighter1.Identity);
                writer.Write(fight.Fighter1.Mesh);
                writer.Write(fight.Fighter1.Name, 16);
                writer.Write(fight.Fighter1.Level);
                writer.Write(fight.Fighter1.Profession);
                //writer.Write(fight.Fighter1.Unknown);
                writer.Write(fight.Fighter1.Rank);
                writer.Write(fight.Fighter1.Points);
                writer.Write(fight.Fighter1.WinsToday);
                writer.Write(fight.Fighter1.LossToday);
                writer.Write(fight.Fighter1.CurrentHonor);
                writer.Write(fight.Fighter1.TotalHonor);
            }
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            MatchesCount = 1;
            FightersNum = 2;
            Fights.Add(new FightStruct
            {
                Fighter0 = new FighterInfoStruct
                {
                    Identity = 1000506,
                    Mesh = 2012001,
                    Name = "Ninja",
                    Rank = 1,
                    Level = 6,
                    Points = 4000,
                    CurrentHonor = 5000,
                    TotalHonor = 6000,
                    WinsToday = 10,
                    LossToday = 9,
                    Profession = 55,
                    Unknown = 1
                },
                Fighter1 = new FighterInfoStruct
                {
                    Identity = 1000002,
                    Mesh = 591003,
                    Name = "Felipe[PM]",
                    Rank = 2,
                    Level = 140,
                    Points = 8000,
                    CurrentHonor = 5000,
                    TotalHonor = 6000,
                    WinsToday = 10,
                    LossToday = 9,
                    Profession = 55,
                    Unknown = 1
                }
            });
            await client.SendAsync(this);
        }

        public struct FightStruct
        {
            public FighterInfoStruct Fighter0;
            public FighterInfoStruct Fighter1;
        }

        public struct FighterInfoStruct
        {
            public uint Identity; // 0
            public uint Mesh; // 4
            public string Name; // 8
            public int Level; // 24
            public int Profession; // 28
            public int Unknown; // 32
            public int Rank; // 36
            public int Points; // 40
            public int WinsToday; // 44
            public int LossToday; // 48
            public int CurrentHonor; // 52
            public int TotalHonor; // 56
        }
    }
}