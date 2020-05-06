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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Comet.Game.Database.Models
{
    /// <summary>
    ///     Character information associated with a player. Every player account is permitted
    ///     a single character on the server. Contains the character's defining look and features,
    ///     level and attribute information, location, etc.
    /// </summary>
    [Table("cq_user")]
    public class DbCharacter
    {
        // Column Properties
        [Key]
        [Column("id")] public virtual uint Identity { get; set; }
        [Column("account_id")] public virtual uint AccountIdentity { get; set; }
        [Column("name")] public virtual string Name { get; set; }
        [Column("mate")] public virtual string Mate { get; set; }
        [Column("lookface")] public virtual uint Mesh { get; set; }
        [Column("hair")] public virtual ushort Hairstyle { get; set; }
        [Column("money")] public virtual uint Silver { get; set; }
        [Column("emoney")] public virtual uint ConquerPoints { get; set; }
        [Column("profession")] public virtual byte Profession { get; set; }
        [Column("old_prof")] public virtual byte PreviousProfession { get; set; }
        [Column("first_prof")] public virtual byte FirstProfession { get; set; }
        [Column("metempsychosis")] public virtual byte Rebirths { get; set; }
        [Column("level")] public virtual byte Level { get; set; }
        [Column("exp")] public virtual ulong Experience { get; set; }
        [Column("recordmap_id")] public virtual uint MapID { get; set; }
        [Column("recordx")] public virtual ushort X { get; set; }
        [Column("recordy")] public virtual ushort Y { get; set; }
        [Column("virtue")] public virtual uint Virtue { get; set; }
        [Column("strength")] public virtual ushort Strength { get; set; }
        [Column("speed")] public virtual ushort Agility { get; set; }
        [Column("health")] public virtual ushort Vitality { get; set; }
        [Column("soul")] public virtual ushort Spirit { get; set; }
        [Column("additional_point")] public virtual ushort AttributePoints { get; set; }
        [Column("life")] public virtual ushort HealthPoints { get; set; }
        [Column("mana")] public virtual ushort ManaPoints { get; set; }
        [Column("pk")] public virtual ushort KillPoints { get; set; }
        [Column("creation_date")] public virtual DateTime Registered { get; set; }
    }
}