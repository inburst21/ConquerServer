﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

            Helmet = user.Headgear?.Type ?? 0;
            HelmetColor = (ushort) (user.Headgear?.Color ?? Item.ItemColor.Orange);
            RightHand = user.RightHand?.Type ?? 0;
            LeftHand = user.LeftHand?.Type ?? 0;
            LeftHandColor = (ushort) (user.LeftHand?.Color ?? Item.ItemColor.Orange);
            Armor = user.Armor?.Type ?? 0;
            ArmorColor = (ushort) (user.Armor?.Color ?? Item.ItemColor.Orange);
            Garment = user.Garment?.Type ?? 0;

            Name = user.Name;
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

        public uint Status { get; set; }

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

        public uint Padding1 { get; set; }

        public uint NobilityRank { get; set; }
        public uint NobilityIdentity { get; set; }
        public uint NobilityPosition { get; set; }

        public ushort Padding2 { get; set; }

        public ushort HelmetColor { get; set; }
        public ushort ArmorColor { get; set; }
        public ushort LeftHandColor { get; set; }

        public string Name { get; set; }

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
            writer.Write(Identity);
            writer.Write(Mesh);

            if (StatuaryLife > 0)
            {
                writer.Write(StatuaryLife);
                writer.Write(StatuaryFrame);
                writer.Write(0u);
            }
            else
            {
                writer.Write(Status);
            }

            if (OwnerIdentity > 0)
            {
                writer.Write(OwnerIdentity);
            }
            else
            {
                writer.Write(SyndicateIdentity);
                writer.Write((byte) 0);
                writer.Write(SyndicatePosition);
            }

            writer.Write(Garment);
            writer.Write(Helmet);
            writer.Write(Armor);
            writer.Write(RightHand);
            writer.Write(LeftHand);
            writer.Write(Padding0);
            writer.Write(MonsterLife);
            writer.Write(MonsterLevel);
            writer.Write(MapX);
            writer.Write(MapY);
            writer.Write(Hairstyle);
            writer.Write(Direction);
            writer.Write(Pose);
            writer.Write(Metempsychosis);
            writer.Write(Level);
            writer.Write(Padding1);
            writer.Write(NobilityRank);
            writer.Write(Padding2);
            writer.Write(HelmetColor);
            writer.Write(ArmorColor);
            writer.Write(LeftHandColor);
            writer.Write(new List<string>
            {
                Name
            });

            return writer.ToArray();
        }

        
    }
}