﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbSyndicate.cs
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

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Game.Database.Models
{
    [Table("cq_syndicate")]
    public class DbSyndicate
    {
        [Key] [Column("id")] public virtual ushort Identity { get; set; }
        [Column("creation_date")] public virtual DateTime CreationDate { get; set; }
        [Column("name")] public virtual string Name { get; set; }
        [Column("announce")] public virtual string Announce { get; set; }
        [Column("announce_date")] public virtual DateTime AnnounceDate { get; set; }
        [Column("leader_id")] public virtual uint LeaderIdentity { get; set; }
        [Column("leader_name")] public virtual string LeaderName { get; set; }
        [Column("money")] public virtual ulong Money { get; set; }
        [Column("emoney")] public virtual uint ConquerPoints { get; set; }
        [Column("del_flag")] public virtual byte DelFlag { get; set; }
        [Column("amount")] public virtual uint Amount { get; set; }
        [Column("totem_pole")] public virtual int TotemPole { get; set; }
        [Column("last_totem")] public virtual DateTime? LastTotem { get; set; }
        [Column("req_lev")] public virtual byte ReqLevel { get; set; }
        [Column("req_class")] public virtual byte ReqClass { get; set; }
        [Column("req_metempsychosis")] public virtual byte ReqMetempsychosis { get; set; }
        [Column("money_prize")] public virtual long MoneyPrize { get; set; }
        [Column("emoney_prize")] public virtual uint EmoneyPrize { get; set; }
        [Column("castle_id")] public virtual uint CastleIdentity { get; set; }
        [Column("castle_level")] public virtual uint CastleLevel { get; set; }
        [Column("current_boss_level")] public virtual uint CurrentBossLevel { get; set; }
    }
}