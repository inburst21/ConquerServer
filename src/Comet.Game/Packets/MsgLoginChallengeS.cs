// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgLoginChallengeS.cs
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
using Comet.Game.States;
using Comet.Game.World.Security;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgLoginChallengeS : MsgBase<Client>
    {
        // Local-Scope Constant Declarations:
        private const int PADDING_LENGTH = 11;
        private const int JUNK_LENGTH = 12;
        private const int PRIMATIVE_ROOT_LENGTH = 128;
        private const int PRIMARY_KEY_LENGTH = 128;
        private const int GENERATOR_LENGTH = 2;
        private const int PACKET_LENGTH = 333- PADDING_LENGTH;

        public MsgLoginChallengeS(string key, byte[] encryptionIV, byte[] decryptionIV)
        {
            Random rand = new Random();

            PaddingBytes = new byte[PADDING_LENGTH];
            rand.NextBytes(PaddingBytes);

            PacketLength = PACKET_LENGTH;

            JunkLength = JUNK_LENGTH;
            Junk = new byte[JUNK_LENGTH];
            rand.NextBytes(Junk);

            EncryptIVSize = encryptionIV.Length;
            EncryptIv = encryptionIV;

            DecryptIVSize = decryptionIV.Length;
            DecryptIV = decryptionIV;

            PSize = PRIMATIVE_ROOT_LENGTH;
            P = NetDragonDHKeyExchange.PRIMATIVE_ROOT;

            GSize = GENERATOR_LENGTH;
            G = NetDragonDHKeyExchange.GENERATOR;

            ASize = PRIMARY_KEY_LENGTH;
            A = key;
        }

        public byte[] PaddingBytes { get; set; }
        public int PacketLength { get; set; }
        public int JunkLength { get; set; }
        public byte[] Junk { get; set; }
        public int EncryptIVSize { get; set; }
        public byte[] EncryptIv { get; set; }
        public int DecryptIVSize { get; set; }
        public byte[] DecryptIV { get; set; }
        public int PSize { get; set; }
        public string P { get; set; }
        public int GSize { get; set; }
        public string G { get; set; }
        public int ASize { get; set; }
        public string A { get; set; }


        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.BaseStream.Position = 0;
            writer.Write(PaddingBytes);
            writer.Write(PacketLength);
            writer.Write(JunkLength);
            writer.Write(Junk);
            writer.Write(EncryptIVSize);
            writer.Write(EncryptIv);
            writer.Write(DecryptIVSize);
            writer.Write(DecryptIV);
            writer.Write(PSize);
            writer.Write(P, PSize);
            writer.Write(GSize);
            writer.Write(G, GSize);
            writer.Write(ASize);
            writer.Write(A, ASize);

            return writer.ToArray();
        }
    }
}