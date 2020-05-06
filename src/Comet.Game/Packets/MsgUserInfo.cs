// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgUserInfo.cs
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
using Comet.Game.Database.Models;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    /// <remarks>Packet Type 1006</remarks>
    /// <summary>
    ///     Message defining character information, used to initialize the client interface
    ///     and game state. Character information is loaded from the game database on login
    ///     if a character exists.
    /// </summary>
    public sealed class MsgUserInfo : MsgBase<Client>
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="MsgUserInfo" /> using data fetched
        ///     from the database and stored in <see cref="DbCharacter" />.
        /// </summary>
        /// <param name="character">Character info from the database</param>
        public MsgUserInfo(Character character)
        {
            Type = PacketType.MsgUserInfo;
            Identity = character.Identity;
            Mesh = (uint) (character.Mesh + character.Avatar * 10000);
            Hairstyle = character.Hairstyle;
            Silver = character.Silvers;
            Jewels = character.ConquerPoints;
            Experience = character.Experience;
            Strength = character.Strength;
            Agility = character.Agility;
            Vitality = character.Vitality;
            Spirit = character.Spirit;
            AttributePoints = character.AttributePoints;
            HealthPoints = (ushort) character.Life;
            ManaPoints = (ushort) character.Mana;
            KillPoints = character.PkPoints;
            Level = character.Level;
            CurrentClass = character.Profession;
            PreviousClass = character.PreviousProfession;
            Rebirths = character.Metempsychosis;
            CharacterName = character.Name;
            SpouseName = "None";
            HasName = true;
        }

        // Packet Properties
        public uint Identity { get; set; }
        public uint Mesh { get; set; }
        public ushort Hairstyle { get; set; }
        public uint Silver { get; set; }
        public uint Jewels { get; set; }
        public ulong Experience { get; set; }
        public ushort Strength { get; set; }
        public ushort Agility { get; set; }
        public ushort Vitality { get; set; }
        public ushort Spirit { get; set; }
        public ushort AttributePoints { get; set; }
        public ushort HealthPoints { get; set; }
        public ushort ManaPoints { get; set; }
        public ushort KillPoints { get; set; }
        public byte Level { get; set; }
        public byte CurrentClass { get; set; }
        public byte PreviousClass { get; set; }
        public byte Rebirths { get; set; }
        public bool HasName { get; set; }
        public string CharacterName { get; set; }
        public string SpouseName { get; set; }

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
            writer.Write(Hairstyle);
            writer.Write(Silver);
            writer.Write(Jewels);
            writer.Write(Experience);
            writer.Write((ulong) 0);
            writer.Write((ulong) 0);
            writer.Write(Strength);
            writer.Write(Agility);
            writer.Write(Vitality);
            writer.Write(Spirit);
            writer.Write(AttributePoints);
            writer.Write(HealthPoints);
            writer.Write(ManaPoints);
            writer.Write(KillPoints);
            writer.Write(Level);
            writer.Write(CurrentClass);
            writer.Write(PreviousClass);
            writer.Write(Rebirths);
            writer.Write(HasName);
            writer.Write(new List<string>
            {
                CharacterName,
                SpouseName
            });
            return writer.ToArray();
        }
    }
}