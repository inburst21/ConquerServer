﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgNobility.cs
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
    public sealed class MsgPeerage : MsgBase<Client>
    {
        public MsgPeerage()
        {
            Type = PacketType.MsgPeerage;
            Data = 0;
        }

        public MsgPeerage(NobilityAction action, ushort maxPages, ushort maxPerPage)
        {
            Type = PacketType.MsgPeerage;
            Action = action;
            DataLow2 = maxPages;
            DataHigh1 = maxPerPage;
        }

        public NobilityAction Action { get; set; }
        public ulong Data { get; set; }

        public uint DataHigh
        {
            get => (uint) (Data - (Data >> 32));
            set => Data = (ulong) value << 32 | DataLow;
        }

        public ushort DataLow1
        {
            get => (ushort) (DataHigh - (DataHigh >> 16));
            set => DataHigh = (uint) value << 16 | DataHigh2;
        }

        public ushort DataLow2
        {
            get => (ushort) (DataHigh >> 16);
            set => DataHigh = ((uint)DataHigh1 << 16 | value);
        }

        public uint DataLow
        {
            get => (uint) (Data >> 32);
            set => Data = (ulong) DataHigh << 32 | value;
        }

        public ushort DataHigh1
        {
            get => (ushort)(DataLow - (DataLow >> 16));
            set => DataLow = (uint)value << 16 | DataLow2;
        }

        public ushort DataHigh2
        {
            get => (ushort)(DataLow >> 16);
            set => DataLow = ((uint)DataLow1 << 16 | value);
        }
        
        public List<string> Strings = new List<string>();

        /// <summary>
        ///     Decodes a byte packet into the packet structure defined by this message class.
        ///     Should be invoked to structure data from the client for processing. Decoding
        ///     follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            Action = (NobilityAction) reader.ReadUInt32();
            Data = reader.ReadUInt64();
            Strings = reader.ReadStrings();
        }

        /// <summary>
        ///     Encodes the packet structure defined by this message class into a byte packet
        ///     that can be sent to the client. Invoked automatically by the client's send
        ///     method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort)Type);
            writer.Write((uint) Action);
            writer.Write(Data);
            writer.Write(Strings);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
#if NOBILITY
            Character user = client.Character;

            switch (Action)
            {
                case NobilityAction.Donate:
                    if (user.Level < 70)
                    {
                        return;
                    }

                    if (Data < 3000000)
                    {
                        return;
                    }

                    if (Data <= user.Silvers)
                    {
                        if (!await user.SpendMoney((int) Data, true))
                            return;
                    }
                    else
                    {
                        if (!await user.SpendConquerPoints((int)(Data/50000), true))
                            return;
                    }

                    await Kernel.PeerageManager.Donate(user, Data);
                    break;
                case NobilityAction.List:
                    await Kernel.PeerageManager.SendRankingAsync(user, DataLow1);
                    break;
                case NobilityAction.QueryRemainingSilver:
                    break;
            }
#endif
        }
    }

    public enum NobilityAction : uint
    {
        None,
        Donate,
        List,
        Info,
        QueryRemainingSilver
    };

    public enum NobilityRank : byte
    {
        Serf,
        Knight,
        Baron = 3,
        Earl = 5,
        Duke = 7,
        Prince = 9,
        King = 12
    };
}