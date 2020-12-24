// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgRank.cs
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
using Castle.Components.DictionaryAdapter;
using Comet.Game.States;
using Comet.Network.Packets;
using Comet.Shared;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgRank : MsgBase<Client>
    {
        public enum RequestType
        {
            None,
            RequestRank,
            QueryInfo,
            QueryIcon = 5
        }

        public enum RankType : byte
        {
            Flower,
            ChiDragon,
            ChiPhoenix,
            ChiTiger,
            ChiTurtle
        }

        public RequestType Mode { get; set; }
        public uint Identity { get; set; }
        public RankType RankMode { get; set; }
        public byte Subtype { get; set; }
        public ushort Data1 { get; set; }
        public ushort Data2 { get; set; }
        public ushort PageNumber { get; set; }

        public List<string> Strings { get; set; } = new List<string>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16(); // 0
            Type = (PacketType) reader.ReadUInt16(); // 2
            Mode = (RequestType) reader.ReadUInt32(); // 4
            Identity = reader.ReadUInt32(); // 8
            RankMode = (RankType) reader.ReadByte(); // 12
            Subtype = reader.ReadByte(); // 13
            PageNumber = reader.ReadUInt16();
            //Data2 = reader.ReadUInt16();
            //PageNumber = reader.ReadUInt16();
            //Strings = reader.ReadStrings();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) PacketType.MsgRank);
            writer.Write((uint) Mode);
            writer.Write(Identity);
            writer.Write((byte) RankMode);
            writer.Write(Subtype);
            writer.Write(PageNumber);
            //writer.Write(Data2);
            //writer.Write(PageNumber);
            writer.Write((ushort) Strings.Count);
            foreach (var t in Strings)
            {
                writer.Write(t);
            }
            // writer.Write(Strings);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;
            switch (Mode)
            {
                case RequestType.QueryInfo:
                {
                    if (user.Gender != 2)
                        return;

                    await user.SendAsync(new MsgFlower
                    {
                        Mode = MsgFlower.RequestMode.QueryIcon,
                        Strings = new List<string>
                        {
                            $"{user.FlowerRed} 0 {user.FlowerWhite} 0 {user.FlowerOrchid} 0 {user.FlowerTulip} 0"
                        }
                    });

                    await user.SendAsync(new MsgRank
                    {
                        Mode = RequestType.QueryIcon
                    });

                    break;
                }
                default:
                {
                    await Log.WriteLogAsync(LogLevel.Error, $"Unhandled MsgRank:{Mode}");
                    await Log.WriteLogAsync(LogLevel.Debug, PacketDump.Hex(Encode()));
                    return;
                }
            }
        }
    }
}