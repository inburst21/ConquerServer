// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - NetDragonDHKeyExchange.cs
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

using Comet.Game.Packets;
using Comet.Game.States;
using Comet.Network.Security;

#endregion

namespace Comet.Game.World.Security
{
    public class NetDragonDHKeyExchange : DiffieHellmanKeyExchange
    {
        // Global-Scope Constant Declarations:
        public const string PRIMATIVE_ROOT = "E7A69EBDF105F2A6BBDEAD7E798F76A209AD73FB466431E2E7352ED262F8C558F10BEFEA977DE9E21DCEE9B04D245F300ECCBBA03E72630556D011023F9E857F";

        public const string GENERATOR = "05";

        // Local-Scope Variable Declarations:
        private static string _serverRequestKey;
        private byte[] _decryptionIv;
        private byte[] _encryptionIv;

        /// <summary>
        /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
        /// other to jointly establish a shared secret key over an insecure communications channel. This 
        /// key can then be used to encrypt subsequent communications using a symmetric key cipher. This
        /// class controls communication between the client and server using NetDragon Websoft's exchange
        /// request packet. Once keys are set, Blowfish will be reinitialized and the message server will 
        /// begin to receive packets.
        /// </summary>
        static NetDragonDHKeyExchange()
        {
            DiffieHellmanKeyExchange exchange = new DiffieHellmanKeyExchange(PRIMATIVE_ROOT, GENERATOR);
            _serverRequestKey = exchange.GenerateRequest();
        }

        /// <summary>
        /// The Diffie–Hellman key exchange method allows two parties that have no prior knowledge of each 
        /// other to jointly establish a shared secret key over an insecure communications channel. This 
        /// key can then be used to encrypt subsequent communications using a symmetric key cipher. This
        /// class controls communication between the client and server using NetDragon Websoft's exchange
        /// request packet. Once keys are set, Blowfish will be reinitialized and the message server will 
        /// begin to receive packets.
        /// </summary>
        public NetDragonDHKeyExchange()
            : base(PRIMATIVE_ROOT, GENERATOR)
        {
            _decryptionIv = new byte[8];
            _encryptionIv = new byte[8];
        }

        /// <summary>
        /// This method creates an exchange request packet which includes the server's public key and the 
        /// client's decryption and encryption initialization vectors (if they are initialized). It returns
        /// the created packet to be sent to the client.
        /// </summary>
        public MsgLoginChallengeS Request()
        {
            return new MsgLoginChallengeS(_serverRequestKey, _encryptionIv, _decryptionIv);
        }

        /// <summary>
        /// This method processes the client's response packet and responds back by configuring the client's
        /// remote Blowfish cipher implementation. The server computes the secret exchange key using the
        /// client's public key, then transfers that key to the Blowfish cipher. The client's decryption and
        /// encryption IVs are reset.
        /// </summary>
        /// <param name="publicKey">The client's public key from the exchange response.</param>
        /// <param name="cipher">The client's remote Blowfish cipher implementation.</param>
        public void Respond(string publicKey, Client user)
        {
            user.Cipher.GenerateKeys(new object[] {GenerateResponse(publicKey)});
            user.Cipher.SetDecryptionIV(_decryptionIv);
            user.Cipher.SetEncryptionIV(_encryptionIv);
        }
    }
}