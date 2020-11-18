// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgGuide.cs
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
    public sealed class MsgGuide : MsgBase<Client>
    {
        public enum Request
        {
            RequestApprentice = 1,
            RequestMentor = 2,
            LeaveMentor = 3,
            ExpellApprentice = 4,
            AcceptRequestApprentice = 8,
            AcceptRequestMentor = 9,
            DumpApprentice = 18,
            DumpMentor = 19
        }

        public MsgGuide()
        {
            Type = PacketType.MsgGuide;
        }

        public Request Action;
        public uint Identity;
        public uint Param;
        public uint Param2;
        public bool Online;
        public string Name;

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Action = (Request) reader.ReadUInt32();
            Identity = reader.ReadUInt32();
            Param = reader.ReadUInt32();
            Param2 = reader.ReadUInt32();
            Online = reader.ReadBoolean();
            Name = reader.ReadString(MAX_NAME_SIZE);
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write((uint) Action);
            writer.Write(Identity);
            writer.Write(Param);
            writer.Write(Param2);
            writer.Write(Online);
            writer.Write(Name, MAX_NAME_SIZE);
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;
            switch (Action)
            {
                default:
                    if (user.IsPm())
                        await user.SendAsync($"Unhandled MsgGuide:{Action}", MsgTalk.TalkChannel.Talk);
                    break;
            }
        }
    }
}