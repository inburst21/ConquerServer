﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgGuideContribute.cs
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

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgGuideContribute : MsgBase<Client>
    {
        public enum RequestType
        {
            Show = 5,
            ClaimExperience = 1,
            ClaimSomeExperience = 2,
            ClaimHeavenBlessing = 3,
            ClaimPlus = 4
        }

        public MsgGuideContribute()
        {
            Type = PacketType.MsgGuideContribute;
        }

        public RequestType Mode;
        public uint Identity;
        public byte[] Padding = new byte[12];
        public uint Experience;
        public ushort HeavenBlessing;
        public ushort Composing;
        public uint Unknown;

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Mode = (RequestType) reader.ReadUInt32();
            Identity = reader.ReadUInt32();
            Padding = reader.ReadBytes(12);
            Experience = reader.ReadUInt32();
            HeavenBlessing = reader.ReadUInt16();
            Composing = reader.ReadUInt16();
            Unknown = reader.ReadUInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write((int) Mode);
            writer.Write(Identity);
            writer.Write(Padding);
            writer.Write(Experience);
            writer.Write(HeavenBlessing);
            writer.Write(Composing);
            writer.Write(Unknown);
            return writer.ToArray();
        }

        public override Task ProcessAsync(Client client)
        {
            return client.SendAsync(this);
        }
    }
}