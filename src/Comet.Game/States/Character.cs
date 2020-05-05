// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Character.cs
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

using Comet.Game.Database.Models;

#endregion

namespace Comet.Game.States
{
    /// <summary>
    ///     Character class defines a database record for a player's character. This allows
    ///     for easy saving of character information, as well as means for wrapping character
    ///     data for spawn packet maintenance, interface update pushes, etc.
    /// </summary>
    public class Character : DbCharacter
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="Character" /> using a database fetched
        ///     <see cref="DbCharacter" />. Copies attributes over to the base class of this
        ///     class, which will then be used to save the character from the game world.
        /// </summary>
        /// <param name="character">Database character information</param>
        public Character(DbCharacter character)
        {
            base.AccountID = character.AccountID;
            base.Agility = character.Agility;
            base.AttributePoints = character.AttributePoints;
            base.Avatar = character.Avatar;
            base.CharacterID = character.CharacterID;
            base.CurrentClass = character.CurrentClass;
            base.Experience = character.Experience;
            base.Hairstyle = character.Hairstyle;
            base.HealthPoints = character.HealthPoints;
            base.Jewels = character.Jewels;
            base.KillPoints = character.KillPoints;
            base.Level = character.Level;
            base.ManaPoints = character.ManaPoints;
            base.MapID = character.MapID;
            base.Mesh = character.Mesh;
            base.Name = character.Name;
            base.PreviousClass = character.PreviousClass;
            base.Rebirths = character.Rebirths;
            base.Registered = character.Registered;
            base.Silver = character.Silver;
            base.Spirit = character.Spirit;
            base.Strength = character.Strength;
            base.Virtue = character.Virtue;
            base.Vitality = character.Vitality;
            base.X = character.X;
            base.Y = character.Y;
        }
    }

    /// <summary>Enumeration type for body types for player characters.</summary>
    public enum BodyType : ushort
    {
        AgileMale = 1003,
        MuscularMale = 1004,
        AgileFemale = 2001,
        MuscularFemale = 2002
    }

    /// <summary>Enumeration type for base classes for player characters.</summary>
    public enum BaseClassType : ushort
    {
        Trojan = 10,
        Warrior = 20,
        Archer = 40,
        Taoist = 100
    }
}