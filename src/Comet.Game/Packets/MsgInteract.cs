// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgInteract.cs
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
using System.Threading.Tasks;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgInteract : MsgBase<Client>
    {
        public MsgInteract()
        {
            Type = PacketType.MsgInteract;

            Timestamp = Environment.TickCount;
        }

        public int Timestamp { get; set; }
        public uint SenderIdentity { get; set; }
        public uint TargetIdentity { get; set; }
        public ushort PosX { get; set; }
        public ushort PosY { get; set; }
        public MsgInteractType Action { get; set; }
        public int Data { get; set; }

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
            Timestamp = reader.ReadInt32();
            SenderIdentity = reader.ReadUInt32();
            TargetIdentity = reader.ReadUInt32();
            PosX = reader.ReadUInt16();
            PosY = reader.ReadUInt16();
            Action = (MsgInteractType)reader.ReadUInt32();
            Data = reader.ReadInt32();
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
            writer.Write(Timestamp);
            writer.Write(SenderIdentity);
            writer.Write(TargetIdentity);
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write((ushort)Action);
            writer.Write(Data);
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
            switch (Action)
            {
                case MsgInteractType.Attack:
                case MsgInteractType.Shoot:
                    break;
                case MsgInteractType.MagicAttack:
                    break;
                case MsgInteractType.Court:
                    break;
                case MsgInteractType.Marry:
                    break;
            }
        }
    }

    public enum MsgInteractType : uint
    {
        None = 0,
        Steal = 1,
        Attack = 2,
        Heal = 3,
        Poison = 4,
        Assassinate = 5,
        Freeze = 6,
        Unfreeze = 7,
        Court = 8,
        Marry = 9,
        Divorce = 10,
        PresentMoney = 11,
        PresentItem = 12,
        SendFlowers = 13,
        Kill = 14,
        JoinGuild = 15,
        AcceptGuildMember = 16,
        KickoutGuildMember = 17,
        PresentPower = 18,
        QueryInfo = 19,
        RushAttack = 20,
        Unknown21 = 21,
        AbortMagic = 22,
        ReflectWeapon = 23,
        MagicAttack = 24,
        Unknown = 25,
        ReflectMagic = 26,
        Dash = 27,
        Shoot = 28,
        Quarry = 29,
        Chop = 30,
        Hustle = 31,
        Soul = 32,
        AcceptMerchant = 33,
        IncreaseJar = 36,
        PresentEmoney = 39,
        CounterKill = 43,
        CounterKillSwitch = 44,
        FatalStrike = 45,
        InteractRequest = 46,
        InteractConfirm,
        Interact,
        InteractUnknown,
        InteractStop,
        AzureDmg = 55
    }

    [Flags]
    public enum InteractionEffect : ushort
    {
        None = 0x0,
        Block = 0x1, // 1
        Penetration = 0x2, // 2
        CriticalStrike = 0x4, // 4
        Breakthrough = 0x2, // 8
        MetalResist = 0x10, // 16
        WoodResist = 0x20, // 32
        WaterResist = 0x40, // 64
        FireResist = 0x80, // 128
        EarthResist = 0x100,
    }
}