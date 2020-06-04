// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgSyndicateAttributeInfo.cs
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

using Comet.Game.States;
using Comet.Game.States.Syndicates;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgSyndicateAttributeInfo : MsgBase<Client>
    {
        public MsgSyndicateAttributeInfo()
        {
            Type = PacketType.MsgSyndicateAttributeInfo;
            LeaderName = Language.StrNone;
        }

        public uint Identity { get; set; }
        public int PlayerDonation { get; set; }
        public uint Funds { get; set; }
        public int MemberAmount { get; set; }
        public SyndicateMember.SyndicateRank Rank { get; set; }
        public string LeaderName { get; set; }

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
            writer.Write(Identity);
            writer.Write(PlayerDonation);
            writer.Write(Funds);
            writer.Write(MemberAmount);
            writer.Write((byte) Rank);
            writer.Write(LeaderName, 16);
            return writer.ToArray();
        }
    }
}