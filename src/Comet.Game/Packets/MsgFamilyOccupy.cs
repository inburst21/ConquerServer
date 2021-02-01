// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgFamilyOccupy.cs
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

using System.IO;
using System.Threading.Tasks;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgFamilyOccupy : MsgBase<Client>
    {
        public enum FamilyPromptType
        {
            RequestNpc = 6, // Npc Click Client -> Server -> Client
            AnnounceWarBegin = 7, // Call to war Server -> Client
            AnnounceWarAccept = 8 // Answer Ok to annouce Client -> Server
        }

        public FamilyPromptType Action; // 4
        public uint Identity; // 8
        public uint RequestNpc; // 12
        public uint SubAction; // 16
        public string OccupyName; // 20
        public string CityName; // 56
        public uint OccupyDays; // 96
        public uint DailyPrize; // 100
        public uint WeeklyPrize; // 104
        public uint GoldFee; // 120

        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Action = (FamilyPromptType) reader.ReadInt32();
            Identity = reader.ReadUInt32();
            RequestNpc = reader.ReadUInt32();
            SubAction = reader.ReadUInt32();
            OccupyName = reader.ReadString(16);
            reader.BaseStream.Seek(20, SeekOrigin.Current);
            CityName = reader.ReadString(16);
            reader.BaseStream.Seek(24, SeekOrigin.Current);
            OccupyDays = reader.ReadUInt32();
            DailyPrize = reader.ReadUInt32();
            WeeklyPrize = reader.ReadUInt32();
            reader.BaseStream.Seek(12, SeekOrigin.Current);
            GoldFee = reader.ReadUInt32();
        }

        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort) (Type = PacketType.MsgFamilyOccupy));
            writer.Write((int) Action);
            writer.Write(Identity);
            writer.Write(RequestNpc);
            writer.Write(SubAction);
            writer.Write(OccupyName, 16);
            writer.BaseStream.Seek(20, SeekOrigin.Current);
            writer.Write(CityName, 16);
            writer.BaseStream.Seek(24, SeekOrigin.Current);
            writer.Write(OccupyDays);
            writer.Write(DailyPrize);
            writer.Write(WeeklyPrize);
            writer.BaseStream.Seek(12, SeekOrigin.Current);
            writer.Write(GoldFee);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;
            if (user == null)
                return;

            switch (Action)
            {
                case FamilyPromptType.RequestNpc:
                {
                    switch (RequestNpc)
                    {
                        case 10026:
                        {
                            DailyPrize = 722458;
                            WeeklyPrize = 722454;
                            break;
                        }

                        case 10027:
                        {
                            DailyPrize = 722458;
                            WeeklyPrize = 722454;
                            break;
                        }

                        case 10028:
                        {
                            DailyPrize = 722478;
                            WeeklyPrize = 722474;
                            break;
                        }

                        case 10029:
                        {
                            DailyPrize = 722478;
                            WeeklyPrize = 722474;
                            break;
                        }

                        case 10030:
                        {
                            DailyPrize = 722473;
                            WeeklyPrize = 722469;
                            break;
                        }

                        case 10031:
                        {
                            DailyPrize = 722473;
                            WeeklyPrize = 722469;
                            break;
                        }

                        case 10032:
                        {
                            DailyPrize = 722463;
                            WeeklyPrize = 722459;
                            break;
                        }

                        case 10033:
                        {
                            DailyPrize = 722463;
                            WeeklyPrize = 722459;
                            break;
                        }

                        case 10034:
                        {
                            DailyPrize = 722468;
                            WeeklyPrize = 722464;
                            break;
                        }

                        case 10035:
                        {
                            DailyPrize = 722468;
                            WeeklyPrize = 722464;
                            break;
                        }
                    }

                    await user.SendAsync(this);
                    break;
                }
            }
        }
    }
}