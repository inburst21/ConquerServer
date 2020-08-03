// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgTalk.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    /// <remarks>Packet Type 1004</remarks>
    /// <summary>
    ///     Message defining a chat message from one player to the other, or from the system
    ///     to a player. Used for all chat systems in the game, including messages outside of
    ///     the game world state, such as during character creation or to tell the client to
    ///     continue logging in after connect.
    /// </summary>
    public sealed class MsgTalk : MsgBase<Client>
    {
        // Static messages
        public const string SYSTEM = "SYSTEM";
        public const string ALLUSERS = "ALLUSERS";
        public static readonly MsgTalk LoginOk = new MsgTalk(0, TalkChannel.Login, "ANSWER_OK");
        public static readonly MsgTalk LoginInvalid = new MsgTalk(0, TalkChannel.Login, "Invalid login");
        public static readonly MsgTalk LoginNewRole = new MsgTalk(0, TalkChannel.Login, "NEW_ROLE");
        public static readonly MsgTalk RegisterOk = new MsgTalk(0, TalkChannel.Register, "ANSWER_OK");
        public static readonly MsgTalk RegisterInvalid = new MsgTalk(0, TalkChannel.Register, "Invalid character");
        public static readonly MsgTalk RegisterNameTaken = new MsgTalk(0, TalkChannel.Register, "Character name taken");

        public static readonly MsgTalk RegisterTryAgain =
            new MsgTalk(0, TalkChannel.Register, "Error, please try later");

        /// <summary>
        ///     Instantiates a new instance of <see cref="MsgTalk" /> using the recipient's
        ///     character ID, a destination channel, and text to display. By default, sends
        ///     from "SYSTEM" to "ALLUSERS".
        /// </summary>
        /// <param name="characterID">Character's identifier</param>
        /// <param name="channel">Destination channel to send the text on</param>
        /// <param name="text">Text to be displayed in the client</param>
        public MsgTalk(uint characterID, TalkChannel channel, string text)
        {
            Type = PacketType.MsgTalk;
            Color = Color.White;
            Channel = channel;
            Style = TalkStyle.Normal;
            CharacterID = characterID;
            SenderName = SYSTEM;
            RecipientName = ALLUSERS;
            Suffix = string.Empty;
            Message = text;
        }

        /// <summary>
        ///     Instantiates a new instance of <see cref="MsgTalk" /> using the recipient's
        ///     character ID, a destination channel, a text color, and text to display. By
        ///     default, sends from "SYSTEM" to "ALLUSERS".
        /// </summary>
        /// <param name="characterID">Character's identifier</param>
        /// <param name="channel">Destination channel to send the text on</param>
        /// <param name="color">Color text is to be displayed in</param>
        /// <param name="text">Text to be displayed in the client</param>
        public MsgTalk(uint characterID, TalkChannel channel, Color color, string text)
        {
            Type = PacketType.MsgTalk;
            Color = color;
            Channel = channel;
            Style = TalkStyle.Normal;
            CharacterID = characterID;
            SenderName = SYSTEM;
            RecipientName = ALLUSERS;
            Suffix = string.Empty;
            Message = text;
        }

        /// <summary>
        ///     Instantiates a new instance of <see cref="MsgTalk" /> using the recipient's
        ///     character ID, a destination channel, a text color, sender and recipient's name,
        ///     and text to display.
        /// </summary>
        /// <param name="characterID">Character's identifier</param>
        /// <param name="channel">Destination channel to send the text on</param>
        /// <param name="color">Color text is to be displayed in</param>
        /// <param name="recipient">Name the message displays it is to</param>
        /// <param name="sender">Name the message displays it is from</param>
        /// <param name="text">Text to be displayed in the client</param>
        public MsgTalk(uint characterID, TalkChannel channel, Color color,
            string recipient, string sender, string text)
        {
            Type = PacketType.MsgTalk;
            Color = color;
            Channel = channel;
            Style = TalkStyle.Normal;
            CharacterID = characterID;
            SenderName = sender;
            RecipientName = recipient;
            Suffix = string.Empty;
            Message = text;
        }

        // Packet Properties
        public Color Color { get; set; }
        public TalkChannel Channel { get; set; }
        public TalkStyle Style { get; set; }
        public uint CharacterID { get; set; }
        public uint RecipientMesh { get; set; }
        public string RecipientName { get; set; }
        public uint SenderMesh { get; set; }
        public string SenderName { get; set; }
        public string Suffix { get; set; }
        public string Message { get; set; }

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
            Type = (PacketType) reader.ReadUInt16();
            Color = Color.FromArgb(reader.ReadInt32());
            Channel = (TalkChannel) reader.ReadUInt16();
            Style = (TalkStyle) reader.ReadUInt16();
            CharacterID = reader.ReadUInt32();
            RecipientMesh = reader.ReadUInt32();
            SenderMesh = reader.ReadUInt32();

            var strings = reader.ReadStrings();
            if (strings.Count > 3)
            {
                SenderName = strings[0];
                RecipientName = strings[1];
                Suffix = strings[2];
                Message = strings[3];
            }
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
            writer.Write((ushort) Type);
            writer.Write(Color.FromArgb(0, Color).ToArgb());
            writer.Write((ushort) Channel);
            writer.Write((ushort) Style);
            writer.Write(CharacterID);
            writer.Write(RecipientMesh);
            writer.Write(SenderMesh);
            writer.Write(new List<string>
            {
                SenderName,
                RecipientName,
                Suffix,
                Message
            });
            return writer.ToArray();
        }

        /// <summary>
        ///     Enumeration for defining the channel text is printed to. Can also print to
        ///     separate states of the client such as character registration, and can be
        ///     used to change the state of the client or deny a login.
        /// </summary>
        public enum TalkChannel : ushort
        {
            Talk = 2000,
            Whisper,
            Action,
            Team,
            Guild,
            Spouse = 2006,
            System,
            Yell,
            Friend,
            Center = 2011,
            TopLeft,
            Ghost,
            Service,
            Tip,
            World = 2021,
            Register = 2100,
            Login,
            Shop,
            Vendor = 2104,
            Website,
            GuildWarRight1 = 2108,
            GuildWarRight2,
            Offline,
            Announce,
            TradeBoard = 2201,
            FriendBoard,
            TeamBoard,
            GuildBoard,
            OthersBoard,
            Broadcast = 2500,
            Monster = 2600
        }

        /// <summary>
        ///     Enumeration type for controlling how text is stylized in the client's chat
        ///     area. By default, text appears and fades overtime. This can be overridden
        ///     with multiple styles, hard-coded into the client.
        /// </summary>
        [Flags]
        public enum TalkStyle : ushort
        {
            Normal = 0,
            Scroll = 1 << 0,
            Flash = 1 << 1,
            Blast = 1 << 2
        }
    }
}