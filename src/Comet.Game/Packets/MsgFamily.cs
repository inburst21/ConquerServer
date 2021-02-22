// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgFamily.cs
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
using Comet.Shared;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgFamily : MsgBase<Client>
    {
        public FamilyAction Action;
        public uint Identity;
        public uint Unknown;
        public List<string> Strings = new List<string>();
        public List<object> Objects = new List<object>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Action = (FamilyAction) reader.ReadUInt32();
            Identity = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            Strings = reader.ReadStrings();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) PacketType.MsgFamily);
            writer.Write((int) Action);
            writer.Write(Identity);
            writer.Write(Unknown);
            if (Objects.Count == 0)
            {
                writer.Write(Strings);
            }
            else
            {
                int idx = 0;
                foreach (var obj in Objects)
                {
                    if (obj is MemberListStruct member)
                    {
                        writer.Write(member.Name, 16);
                        writer.Write((int) member.Level);
                        writer.Write(member.Rank);
                        writer.Write((ushort) (member.Online ? 1 : 0));
                        writer.Write(member.Profession);
                        writer.Write(member.Donation);
                    }
                    else if (obj is RelationListStruct relation)
                    {
                        writer.Write(idx + 101);
                        writer.Write(relation.Name, 16);
                        writer.Write(relation.LeaderName, 16);
                    }
                    idx++;
                }
            }
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = Kernel.RoleManager.GetUser(client.Character?.Identity ?? 0);
            if (user == null)
            {
                client.Disconnect();
                return;
            }

            switch (Action)
            {
                case FamilyAction.Query:
                {
                    if (user.Family == null)
                        return;

                    await user.SendFamilyAsync();
                    break;
                }

                case FamilyAction.QueryOccupy:
                {
                    if (user.Family == null)
                        return;

                    await user.SendFamilyOccupyAsync();
                    break;
                }

                default:
                {
                    if (user.IsPm())
                    {
                        await client.SendAsync(new MsgTalk(client.Identity, MsgTalk.TalkChannel.Service,
                            $"Missing packet {Type}, Action {Action}, Length {Length}"));
                    }

                    await Log.WriteLogAsync(LogLevel.Warning,
                        "Missing packet {0}, Action {1}, Length {2}\n{3}",
                        Type, Action, Length, PacketDump.Hex(Encode()));
                        break;
                }
            }
        }

        public struct MemberListStruct
        {
            public string Name;
            public byte Level;
            public ushort Rank;
            public bool Online;
            public ushort Profession;
            public uint Donation;
        }

        public struct RelationListStruct
        {
            public string Name;
            public string LeaderName;
        }

        public enum FamilyAction : byte
        {
            Query = 1,
            QueryMemberList = 4,
            Recruit = 9,
            AcceptRecruit = 10,
            Join = 11,
            AcceptJoinRequest = 12,
            SendEnemy = 13,
            AddEnemy = 14,
            DeleteEnemy = 15,
            SendAlly = 16,
            AddAlly = 17,
            AcceptAlliance = 18,
            DeleteAlly = 20,
            Abdicate = 21,
            KickOut = 22,
            Quit = 23,
            Announce = 24,
            SetAnnouncement = 25,
            Dedicate = 26,
            QueryOccupy = 29
        }
    }
}