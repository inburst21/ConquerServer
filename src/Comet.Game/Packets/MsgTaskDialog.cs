﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgTaskDialog.cs
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
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgTaskDialog : MsgBase<Client>
    {
        public MsgTaskDialog()
        {
            Type = PacketType.MsgTaskDialog;
            Text = string.Empty;
        }

        public uint TaskIdentity { get; set; }
        public ushort Data { get; set; }
        public byte OptionIndex { get; set; }
        public TaskInteraction InteractionType { get; set; }
        public string Text { get; set; }

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
            TaskIdentity = reader.ReadUInt32();
            Data = reader.ReadUInt16();
            OptionIndex = reader.ReadByte();
            InteractionType = (TaskInteraction) reader.ReadByte();
            List<string> strings = reader.ReadStrings();
            Text = strings.Count > 0 ? strings[0] : "";
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
            writer.Write(TaskIdentity);
            writer.Write(Data);
            writer.Write(OptionIndex);
            writer.Write((byte) InteractionType);
            writer.Write(new List<string> { Text } );
            return writer.ToArray();
        }

        public enum TaskInteraction : byte
        {
            ClientRequest = 0,
            Dialog = 1,
            Option = 2,
            Input = 3,
            Avatar = 4,
            LayNpc = 5,
            MessageBox = 6,
            Finish = 100,
            Answer = 101,
            TextInput = 102,
            UpdateWindow = 112
        }
    }
}