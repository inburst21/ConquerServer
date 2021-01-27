// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgQualifyingInteractive.cs
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
using Comet.Shared;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgQualifyingInteractive : MsgBase<Client>
    {
        public InteractionType Interaction { get; set; }
        public int Option { get; set; }
        public uint Identity { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Profession { get; set; }
        public int Unknown40 { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            Interaction = (InteractionType) reader.ReadInt32();
            Option = reader.ReadInt32();
            Identity = reader.ReadUInt32();
            Name = reader.ReadString(16);
            Rank = reader.ReadInt32();
            Profession = reader.ReadInt32();
            Unknown40 = reader.ReadInt32();
            Points = reader.ReadInt32();
            Level = reader.ReadInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort)PacketType.MsgQualifyingRank);
            writer.Write((uint) Interaction);
            writer.Write(Option);
            writer.Write(Identity); 
            writer.Write(Name, 16);
            writer.Write(Rank);
            writer.Write(Profession);
            writer.Write(Unknown40);
            writer.Write(Points);
            writer.Write(Level);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = Kernel.RoleManager.GetUser(client.Character.Identity);
            if (user == null)
            {
                client.Disconnect();
                return;
            }

            switch (Interaction)
            {
                default:
                {
                    await client.SendAsync(this);
                    if (client.Character.IsPm())
                    {
                        await client.SendAsync(new MsgTalk(client.Identity, MsgTalk.TalkChannel.Service,
                            $"Missing packet {Type}, Action {Interaction}, Length {Length}"));
                    }

                    await Log.WriteLogAsync(LogLevel.Warning,
                        "Missing packet {0}, Action {1}, Length {2}\n{3}",
                        Type, Interaction, Length, PacketDump.Hex(Encode()));
                        break;
                }
            }
        }

        public enum InteractionType
        {
            Inscribe,
            Unsubscribe,
            Countdown,
            AcceptDlg,
            OpponentGaveUp,
            BuyArenaPoints,
            Match,
            YouAreKicked,
            StartTheFight,
            Dialog,
            EndDialog,
            ReJoin
        }
    }
}