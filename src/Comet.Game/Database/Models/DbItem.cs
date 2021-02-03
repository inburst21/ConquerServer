﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbItem.cs
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
    [Table("cq_item")]
    public class DbItem
    {
        /// <summary>
        ///     The unique identification of the item.
        /// </summary>
        [Key]
        [Column("id")]
        public virtual uint Id { get; set; }

        /// <summary>
        ///     The itemtype of the item.
        /// </summary>
        [Column("type")]
        public virtual uint Type { get; set; }

        /// <summary>
        ///     The owner (shop or player) where the player got the item from.
        /// </summary>
        [Column("owner_id")]
        public virtual uint OwnerId { get; set; }

        /// <summary>
        ///     The player who actually owns the item.
        /// </summary>
        [Column("player_id")]
        public virtual uint PlayerId { get; set; }

        /// <summary>
        ///     The actual durability of the item.
        /// </summary>
        [Column("amount")]
        public virtual ushort Amount { get; set; }

        /// <summary>
        ///     The max amount of the durability of the item.
        /// </summary>
        [Column("amount_limit")]
        public virtual ushort AmountLimit { get; set; }

        /// <summary>
        ///     Not sure yet.
        /// </summary>
        [Column("ident")]
        public virtual byte Ident { get; set; }

        /// <summary>
        ///     The actual position of the item. +200 for warehouses.
        /// </summary>
        [Column("position")]
        public virtual byte Position { get; set; }

        /// <summary>
        ///     The gem on socket 1. 255 for open hole.
        /// </summary>
        [Column("gem1")]
        public virtual byte Gem1 { get; set; }

        /// <summary>
        ///     The gem on socket 2. 255 for open hole.
        /// </summary>
        [Column("gem2")]
        public virtual byte Gem2 { get; set; }

        /// <summary>
        ///     The item effect.
        /// </summary>
        [Column("magic1")]
        public virtual byte Magic1 { get; set; }

        /// <summary>
        ///     Not sure yet.
        /// </summary>
        [Column("magic2")]
        public virtual byte Magic2 { get; set; }

        /// <summary>
        ///     The item plus.
        /// </summary>
        [Column("magic3")]
        public virtual byte Magic3 { get; set; }

        /// <summary>
        ///     Socket progress.
        /// </summary>
        [Column("data")]
        public virtual uint Data { get; set; }

        /// <summary>
        ///     The item blessing.
        /// </summary>
        [Column("reduce_dmg")]
        public virtual byte ReduceDmg { get; set; }

        /// <summary>
        ///     Item enchantment.
        /// </summary>
        [Column("add_life")]
        public virtual byte AddLife { get; set; }

        /// <summary>
        ///     The green attribute. Not used tho.
        /// </summary>
        [Column("anti_monster")]
        public virtual byte AntiMonster { get; set; }

        /// <summary>
        ///     Not sure yet.
        /// </summary>
        [Column("chk_sum")]
        public virtual uint ChkSum { get; set; }

        /// <summary>
        ///     Item locking timestamp. If 0, item is not locked, if timestamp is set 1 item is locked. If it has a timestamp, it's
        ///     the unlock time.
        /// </summary>
        [Column("plunder")]
        public virtual DateTime? Plunder { get; set; }


        /// <summary>
        ///     Forbbiden or not?
        /// </summary>
        [Column("SpecialFlag")]
        public virtual uint Specialflag { get; set; }

        /// <summary>
        ///     The color of the item.
        /// </summary>
        [Column("color")]
        public virtual uint Color { get; set; }

        /// <summary>
        ///     The progress of the plus.
        /// </summary>
        [Column("Addlevel_exp")]
        public virtual uint AddlevelExp { get; set; }

        /// <summary>
        ///     The kind of item. (Bound, Quest, etc)
        /// </summary>
        [Column("monopoly")]
        public virtual byte Monopoly { get; set; }

        [Column("syndicate")] public virtual uint Syndicate { get; set; }

        [Column("del_time")] public virtual DateTime? DeleteTime { get; set; }

        [Column("save_time")] public virtual DateTime? SaveTime { get; set; }

        [Column("accumulate_num")] public virtual uint AccumulateNum { get; set; }

    }
}