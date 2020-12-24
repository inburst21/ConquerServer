// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgPlayer.cs
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

#define NO_COLOR

#region References

using System.Collections.Generic;
using Comet.Game.States;
using Comet.Game.States.Items;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgPlayer : MsgBase<Client>
    {
        public MsgPlayer(Character user)
        {
            Type = PacketType.MsgPlayer;

            Identity = user.Identity;
            Mesh = user.Mesh;

            MapX = user.MapX;
            MapY = user.MapY;

            Status = (uint) user.StatusFlag;

            Hairstyle = user.Hairstyle;
            Direction = (byte) user.Direction;
            Pose = (byte) user.Action;
            Metempsychosis = user.Metempsychosis;
            Level = user.Level;

            SyndicateIdentity = user.SyndicateIdentity;
            SyndicatePosition = (byte) user.SyndicateRank;

            NobilityRank = (uint) user.NobilityRank;
            NobilityIdentity = 0; // SharedBattlePower
            NobilityPosition = (uint) user.NobilityPosition;

            Helmet = user.Headgear?.Type ?? 0;
            HelmetColor = (ushort) (user.Headgear?.Color ?? Item.ItemColor.None);
            RightHand = user.RightHand?.Type ?? 0;
            LeftHand = user.LeftHand?.Type ?? 0;
            LeftHandColor = (ushort) (user.LeftHand?.Color ?? Item.ItemColor.None);
            Armor = user.Armor?.Type ?? 0;
            ArmorColor = (ushort) (user.Armor?.Color ?? Item.ItemColor.None);
            Garment = user.Garment?.Type ?? 0;

            Name = user.Name;
            Mate = user.MateName;
        }

        public MsgPlayer(Monster monster)
        {
            Type = PacketType.MsgPlayer;

            Identity = monster.Identity;

            Mesh = monster.Mesh;

            MapX = monster.MapX;
            MapY = monster.MapY;

            Status = (uint) monster.StatusFlag;

            Direction = (byte) monster.Direction;
            Pose = (byte) monster.Action;

            MonsterLevel = monster.Level;
            if (monster.Life > ushort.MaxValue)
            {
                MonsterLife = (ushort) (ushort.MaxValue / monster.Life * 100);
            }
            else
            {
                MonsterLife = (ushort) monster.Life;
            }

            Name = monster.Name;
            Mate = "";
        }

        public uint Identity { get; set; }
        public uint Mesh { get; set; }

        #region Union

        #region Struct

        public ushort SyndicateIdentity { get; set; }
        public byte SyndicatePosition { get; set; }

        #endregion

        public uint OwnerIdentity { get; set; }

        #endregion

        #region Union

        public ulong Status { get; set; }

        #region Struct

        public ushort StatuaryLife { get; set; }
        public ushort StatuaryFrame { get; set; }

        #endregion

        #endregion
       
        public uint Garment { get; set; }
        public uint Helmet { get; set; }
        public uint Armor { get; set; }
        public uint RightHand { get; set; }
        public uint LeftHand { get; set; }

        public uint Padding0 { get; set; }

        public ushort MonsterLife { get; set; }
        public ushort MonsterLevel { get; set; }

        public ushort MapX { get; set; }
        public ushort MapY { get; set; }
        public ushort Hairstyle { get; set; }
        public byte Direction { get; set; }
        public byte Pose { get; set; }
        public ushort Metempsychosis { get; set; }
        public ushort Level { get; set; }
        public bool WindowSpawn { get; set; }
        public uint SharedBattlePower { get; set; }
        public uint FlowerRanking { get; set; }

        public uint NobilityRank { get; set; }
        public uint NobilityIdentity { get; set; }
        public uint NobilityPosition { get; set; }

        public ushort Padding2 { get; set; }

        public ushort HelmetColor { get; set; }
        public ushort ArmorColor { get; set; }
        public ushort LeftHandColor { get; set; }
        public uint QuizPoints { get; set; }

        public string Name { get; set; }
        public string Mate { get; set; }

        /// <summary>
        ///     Encodes the packet structure defined by this message class into a byte packet
        ///     that can be sent to the client. Invoked automatically by the client's send
        ///     method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write(Identity); // 8
            writer.Write(Mesh); // 4

            if (StatuaryLife > 0)
            {
                writer.Write(StatuaryLife); // 16
                writer.Write(StatuaryFrame); // 18
                writer.Write(0u); // 20
            }
            else
            {
                writer.Write(Status); // 16
            }

            if (OwnerIdentity > 0)
            {
                writer.Write(OwnerIdentity); // 12
            }
            else
            {
                writer.Write(SyndicateIdentity); // 12
                writer.Write((byte)0); // 14
                writer.Write(SyndicatePosition); // 15
            }

            writer.Write(Garment); // 24
            writer.Write(Helmet); // 28
            writer.Write(Armor); // 32
            writer.Write(RightHand); // 36
            writer.Write(LeftHand); // 40
            writer.Write(Padding0); // 44
            writer.Write(MonsterLife); // 48
            writer.Write(MonsterLevel); // 50
            writer.Write(MapX); // 52
            writer.Write(MapY); // 54
            writer.Write(Hairstyle); // 56
            writer.Write(Direction); // 58
            writer.Write(Pose); // 59
            writer.Write(Metempsychosis); // 60
            writer.Write(Level); // 62
            writer.Write(WindowSpawn); // 64 
            writer.Write(SharedBattlePower); // 65 
            writer.Write(FlowerRanking); // 69
            writer.Write(NobilityRank); // 73
            writer.Write(NobilityPosition); // 77
            writer.Write(0); // 81
            writer.Write(HelmetColor); // 85
            writer.Write(ArmorColor); // 87
            writer.Write(LeftHandColor); // 89
            writer.Write(QuizPoints); // 91
            writer.Write(new List<string> // 95
            {
                Name,
                Mate
            });

            return writer.ToArray();
        }
    }
}