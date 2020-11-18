// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgUserAttrib.cs
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
    public sealed class MsgUserAttrib : MsgBase<Client>
    {
        private readonly List<UserAttribute> Attributes = new List<UserAttribute>();

        public MsgUserAttrib(uint idRole, ClientUpdateType type, ulong value)
        {
            Type = PacketType.MsgUserAttrib;

            Identity = idRole;
            Amount++;
            Attributes.Add(new UserAttribute((uint) type, value));
        }

        public uint Identity { get; set; }

        public int Amount { get; set; }

        public void Append(ClientUpdateType type, ulong data)
        {
            Amount++;
            Attributes.Add(new UserAttribute((uint) type, data));
        }

        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Identity = reader.ReadUInt32();
            Amount = reader.ReadInt32();
            for (int i = 0; i < Amount; i++)
            {
                Attributes.Add(new UserAttribute(reader.ReadUInt32(), reader.ReadUInt64()));
            }
        }

        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write(Identity);
            Amount = Attributes.Count;
            writer.Write(Amount);
            for (int i = 0; i < Amount; i++)
            {
                writer.Write(Attributes[i].Type);
                writer.Write(Attributes[i].Data);
            }

            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            await Log.WriteLogAsync(LogLevel.Warning, "Unhandled MsgUserAttrib::Process call");
        }

        private readonly struct UserAttribute
        {
            public UserAttribute(uint type, ulong data)
            {
                Type = type;
                Data = data;
            }

            public readonly uint Type;
            public readonly ulong Data;
        }
    }

    public enum ClientUpdateType : byte
    {
        Hitpoints = 0,
        MaxHitpoints = 1,
        Mana = 2,
        MaxMana = 3,
        Money = 4,
        Experience = 5,
        PkPoints = 6,
        Class = 7,
        SizeAdd = 8,
        Stamina = 9,
        Atributes = 11,
        Mesh,
        Level,
        Spirit,
        Vitality,
        Strength,
        Agility,
        HeavensBlessing = 18,
        DoubleExpTimer = 19,
        CursedTimer = 21,
        Reborn = 23,
        StatusFlag = 26,
        HairStyle = 27,
        XpCircle = 28,
        LuckyTimeTimer = 29,
        ConquerPoints = 30,
        OnlineTraining = 32,
        ExtraBattlePower = 37,
        Merchant = 39,
        VipLevel = 40,
    }
}