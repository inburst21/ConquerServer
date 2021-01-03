﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgSyndicate.cs
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
using Comet.Game.States.Syndicates;
using Comet.Network.Packets;
using Comet.Shared;
using Microsoft.EntityFrameworkCore.Query;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgSyndicate : MsgBase<Client>
    {
        public MsgSyndicate()
        {
            Type = PacketType.MsgSyndicate;
        }

        public SyndicateRequest Mode { get; set; }

        public uint Identity { get; set; }

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
            Mode = (SyndicateRequest) reader.ReadUInt32();
            Identity = reader.ReadUInt32();
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
            writer.Write((uint) Mode);
            writer.Write(Identity);
            return writer.ToArray();
        }

        /// <summary>
        ///     Process can be invoked by a packet after decode has been called to structure
        ///     packet fields and properties. For the server implementations, this is called
        ///     in the packet handler after the message has been dequeued from the server's
        ///     <see cref="PacketProcessor{TClient}" />.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;

            switch (Mode)
            {
                case SyndicateRequest.JoinRequest:
                    if (Identity == 0 || user.Syndicate != null)
                        return;

                    Character leader = Kernel.RoleManager.GetUser(Identity);
                    if (leader == null)
                        return;

                    if (leader.SyndicateIdentity == 0 ||
                        leader.SyndicateRank < SyndicateMember.SyndicateRank.DeputyLeader)
                        return;

                    leader.SetRequest(RequestType.Syndicate, user.Identity);
                    Identity = user.Identity;
                    await leader.SendAsync(this);

                    break;

                case SyndicateRequest.InviteRequest:
                    if (Identity == 0 || user.Syndicate == null)
                        return;

                    Character target = Kernel.RoleManager.GetUser(Identity);
                    if (target == null)
                        return;

                    if (user.SyndicateIdentity == 0
                        || user.SyndicateRank < SyndicateMember.SyndicateRank.DeputyLeader)
                        return;

                    if (user.QueryRequest(RequestType.Syndicate) == Identity)
                    {
                        user.PopRequest(RequestType.Syndicate);
                        await user.Syndicate.AppendMemberAsync(target, user, Syndicate.JoinMode.Request);
                    }
                    break;

                case SyndicateRequest.Quit: // 3
                    if (user.SyndicateIdentity == 0)
                        return;

                    await user.Syndicate.QuitSyndicateAsync(user);
                    break;

                case SyndicateRequest.Query: // 6
                    Syndicate queryTarget = Kernel.SyndicateManager.GetSyndicate((int) Identity);
                    if (queryTarget == null)
                        return;

                    await queryTarget.SendAsync(user);
                    break;

                case SyndicateRequest.DonateSilvers:
                    if (user.Syndicate == null)
                        return;

                    int amount = (int) Identity;
                    if (!await user.SpendMoneyAsync(amount))
                    {
                        await user.SendAsync(Language.StrNotEnoughMoney);
                        return;
                    }

                    user.Syndicate.Money += amount;
                    await user.Syndicate.SaveAsync();
                    user.SyndicateMember.Donation += (int) Identity;
                    await user.Syndicate.SaveAsync();
                    await user.SendSyndicateAsync();

                    await user.Syndicate.SendAsync(string.Format(Language.StrSynDonateMoney, user.SyndicateRank, user.Name, Identity));
                    break;

                case SyndicateRequest.Refresh: // 12
                    if (user.Syndicate != null)
                        await user.SendSyndicateAsync();
                    break;

                default:
                    await Log.WriteLogAsync(LogLevel.Warning, $"Type: {Type}, Subtype: {Mode} not handled");
                    break;
            }
        }

        public enum SyndicateRequest : uint
        {
            JoinRequest = 1,
            InviteRequest = 2,
            Quit = 3,
            Query = 6,
            Ally = 7,
            Unally = 8,
            Enemy = 9,
            Unenemy = 10,
            DonateSilvers = 11,
            Refresh = 12,
            Disband = 19,
            DonateConquerPoints = 20,
            Bulletin = 27,
            SendRequest = 28,
            AcceptRequest = 29,
            Discharge = 30,
            Resign = 32,
            Discharge2 = 33,
            PaidPromote = 34,
            Promote = 37
        }
    }
}