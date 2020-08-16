// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgLoginProofA.cs
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
using Comet.Network.Security;

#endregion

namespace Comet.Game.Packets
{
    public class MsgLoginProofA : MsgBase<Client>
    {
        public byte[] Padding { get; set; }
        public int Size { get; set; }
        public int JunkSize { get; set; }
        public byte[] Junk { get; set; }
        public int PublicKeyLength { get; set; }
        public string Key { get; set; }

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Padding = reader.ReadBytes(7);
            Size = reader.ReadInt32();
            JunkSize = reader.ReadInt32();
            Junk = reader.ReadBytes(JunkSize);
            PublicKeyLength = reader.ReadInt32();
            Key = reader.ReadString(PublicKeyLength);
        }

        public override async Task ProcessAsync(Client client)
        {
            client.Exchange.Respond(Key, client);
            client.Exchanged = true;
        }
    }
}