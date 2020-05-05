// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - MsgAccount.cs
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

using System.Text;
using System.Threading.Tasks;
using Comet.Account.Database.Repositories;
using Comet.Account.States;
using Comet.Network.Packets;
using Comet.Network.Security;
using Comet.Shared.Models;

#endregion

namespace Comet.Account.Packets
{
    #region References

    using static MsgConnectEx;

    #endregion

    /// <remarks>Packet Type 1051</remarks>
    /// <summary>
    ///     Message containing login credentials from the login screen. This is the first
    ///     packet sent to the account server from the client on login. The server checks the
    ///     encrypted password against the hashed password in the database, the responds with
    ///     <see cref="MsgConnectEx" /> with either a pass or fail.
    /// </summary>
    public sealed class MsgAccount : MsgBase<Client>
    {
        // Packet Properties
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Realm { get; private set; }

        /// <summary>
        ///     Process can be invoked by a packet after decode has been called to structure
        ///     packet fields and properties. For the server implementations, this is called
        ///     in the packet handler after the message has been dequeued from the server's
        ///     <see cref="PacketProcessor" />.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            // Fetch account info from the database
            client.Account = await AccountsRepository.FindAsync(Username).ConfigureAwait(false);
            if (client.Account == null || !AccountsRepository.CheckPassword(
                    Password, client.Account.Password, client.Account.Salt))
            {
                await client.SendAsync(new MsgConnectEx(RejectionCode.InvalidPassword));
                client.Socket.Disconnect(false);
                return;
            }

            // Connect to the game server
            if (!Kernel.Realms.TryGetValue(Realm, out var server) || !server.Rpc.Online)
            {
                await client.SendAsync(new MsgConnectEx(RejectionCode.ServerDown));
                client.Socket.Disconnect(false);
                return;
            }

            // Get an access token from the server
            var args = new TransferAuthArgs
            {
                AccountID = client.Account.AccountID,
                AuthorityID = client.Account.AuthorityID,
                AuthorityName = client.Account.Authority.AuthorityName,
                IPAddress = client.IPAddress,
                VipLevel = client.Account.VipLevel
            };

            ulong token = await server.Rpc.CallAsync<ulong>("TransferAuth", args);
            await client.SendAsync(new MsgConnectEx(server.GameIPAddress, server.GamePort, token));
        }

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
            Username = reader.ReadString(16);
            Password = DecryptPassword(reader.ReadBytes(16));
            Realm = reader.ReadString(16);
        }

        /// <summary>
        ///     Decrypts the password from read in packet bytes for the <see cref="Decode" />
        ///     method. Trims the end of the password string of null terminators.
        /// </summary>
        /// <param name="buffer">Bytes from the packet buffer</param>
        /// <returns>Returns the decrypted password string.</returns>
        private string DecryptPassword(byte[] buffer)
        {
            var rc5 = new RC5();
            var password = new byte[16];
            rc5.Decrypt(buffer, password);
            return Encoding.ASCII.GetString(password).Trim('\0');
        }
    }
}