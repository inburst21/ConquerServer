// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Client.cs
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

#define BLOWFISH

#region References

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Comet.Game.World.Security;
using Comet.Network.Packets;
using Comet.Network.Security;
using Comet.Network.Sockets;
using Comet.Shared;

#endregion

namespace Comet.Game.States
{
    /// <summary>
    ///     Client encapsules the accepted client socket's actor and game server state.
    ///     The class should be initialized by the server's Accepted method and returned
    ///     to be passed along to the Receive loop and kept alive. Contains all world
    ///     interactions with the player.
    /// </summary>
    public sealed class Client : TcpServerActor
    {
        // Fields and Properties 
        public Character Character = null;
        public Creation Creation = null;

        /// <summary>
        ///     Instantiates a new instance of <see cref="Client" /> using the Accepted event's
        ///     resulting socket and preallocated buffer. Initializes all account server
        ///     states, such as the cipher used to decrypt and encrypt data.
        /// </summary>
        /// <param name="socket">Accepted remote client socket</param>
        /// <param name="buffer">Preallocated buffer from the server listener</param>
        /// <param name="partition">Packet processing partition</param>
        public Client(Socket socket, Memory<byte> buffer, uint partition)
            : base(socket, buffer, new Blowfish(), partition)
        {
            Exchange = new NetDragonDHKeyExchange();

            Footer = Encoding.ASCII.GetBytes("TQServer");
        }

        // Client unique identifier
        public uint Identity => Character?.Identity ?? 0;
        public uint AccountIdentity { get; set; }
        public byte VipLevel { get; set; }
        public ushort AuthorityLevel { get; set; }

        public NetDragonDHKeyExchange Exchange { get; }

        public override Task<int> SendAsync(byte[] packet)
        {
#if BLOWFISH
            byte[] buffer = new byte[packet.Length + 8];
            System.Buffer.BlockCopy(packet, 0, buffer, 0, packet.Length);
            System.Buffer.BlockCopy(Footer, 0, buffer, buffer.Length - 8, Footer.Length);
            Kernel.NetworkMonitor.Send(buffer.Length);
            return base.SendAsync(buffer);
#else
            Kernel.NetworkMonitor.Send(packet.Length);
            return base.SendAsync(packet);
#endif
        }
    }
}