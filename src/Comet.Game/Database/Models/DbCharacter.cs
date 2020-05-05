// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbCharacter.cs
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
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Comet.Game.Database.Models
{
    /// <summary>
    ///     Character information associated with a player. Every player account is permitted
    ///     a single character on the server. Contains the character's defining look and features,
    ///     level and attribute information, location, etc.
    /// </summary>
    [Table("character")]
    public class DbCharacter
    {
        // Column Properties
        public virtual uint CharacterID { get; set; }
        public virtual uint AccountID { get; set; }
        public virtual string Name { get; set; }
        public virtual uint Mesh { get; set; }
        public virtual ushort Avatar { get; set; }
        public virtual ushort Hairstyle { get; set; }
        public virtual uint Silver { get; set; }
        public virtual uint Jewels { get; set; }
        public virtual byte CurrentClass { get; set; }
        public virtual byte PreviousClass { get; set; }
        public virtual byte Rebirths { get; set; }
        public virtual byte Level { get; set; }
        public virtual ulong Experience { get; set; }
        public virtual uint MapID { get; set; }
        public virtual ushort X { get; set; }
        public virtual ushort Y { get; set; }
        public virtual uint Virtue { get; set; }
        public virtual ushort Strength { get; set; }
        public virtual ushort Agility { get; set; }
        public virtual ushort Vitality { get; set; }
        public virtual ushort Spirit { get; set; }
        public virtual ushort AttributePoints { get; set; }
        public virtual ushort HealthPoints { get; set; }
        public virtual ushort ManaPoints { get; set; }
        public virtual ushort KillPoints { get; set; }
        public virtual DateTime Registered { get; set; }
    }
}