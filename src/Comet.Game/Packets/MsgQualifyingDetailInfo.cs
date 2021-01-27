// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgQualifyingDetailInfo.cs
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

using System.Threading.Tasks;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgQualifyingDetailInfo : MsgBase<Client>
    {
        public int Ranking { get; set; }
        public int Unknown { get; set; }
        public ArenaStatus Status { get; set; }
        public int TotalWins { get; set; }
        public int TotalLoses { get; set; }
        public int TodayWins { get; set; }
        public int TodayLoses { get; set; }
        public int TotalHonor { get; set; }
        public int CurrentHonor { get; set; }
        public int Points { get; set; }

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Ranking = reader.ReadInt32();
            Unknown = reader.ReadInt32();
            Status = (ArenaStatus) reader.ReadInt32();
            TotalWins = reader.ReadInt32();
            TotalLoses = reader.ReadInt32();
            TodayWins = reader.ReadInt32();
            TodayLoses = reader.ReadInt32();
            TotalHonor = reader.ReadInt32();
            CurrentHonor = reader.ReadInt32();
            Points = reader.ReadInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) PacketType.MsgQualifyingDetailInfo);
            writer.Write(Ranking); // 4
            writer.Write(0); // 8
            writer.Write((int) Status); // 12
            writer.Write(198500); // Activity
            writer.Write((byte) 20); // 20 Triumph Today 20
            writer.Write((byte) 9); // 21 Triumph Today 9
            writer.Write((ushort) 0); // 22
            writer.Write(TotalWins); // 24
            writer.Write(TotalLoses); // 28
            writer.Write(TodayWins); // Season Wins
            writer.Write(TodayLoses); // Season Loss
            writer.Write(6464998); // History Honor
            writer.Write(CurrentHonor); // 44
            writer.Write(Points); // 48
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Ranking = 1;
            Status = ArenaStatus.NotSignedUp;
            TodayWins = 9;
            TodayLoses = 20;
            TotalWins = 100;
            TotalLoses = 85;
            TotalHonor = 500000;
            CurrentHonor = 150000;
            Points = 4806;
            await client.SendAsync(this);
        }

        public enum ArenaStatus
        {
            NotSignedUp,
            WaitingForOpponent,
            WaitingInactive
        }
    }
}