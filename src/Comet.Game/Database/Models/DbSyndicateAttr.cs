// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbSyndicateAttr.cs
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
    [Table("cq_synattr")]
    public class DbSyndicateAttr
    {
        [Key] [Column("id")] public virtual uint Id { get; set; }
        [Column("user_id")] public virtual uint UserIdentity { get; set; }
        [Column("syn_id")] public virtual uint SynId { get; set; }
        [Column("rank")] public virtual ushort Rank { get; set; }
        [Column("proffer")] public virtual long Proffer { get; set; }
        [Column("proffer_total")] public virtual ulong ProfferTotal { get; set; }
        [Column("emoney")] public virtual uint Emoney { get; set; }
        [Column("emoney_total")] public virtual uint EmoneyTotal { get; set; }
        [Column("pk")] public virtual int Pk { get; set; }
        [Column("pk_total")] public virtual uint PkTotal { get; set; }
        [Column("guide")] public virtual uint Guide { get; set; }
        [Column("guide_total")] public virtual uint GuideTotal { get; set; }
        [Column("exploit")] public virtual uint Exploit { get; set; }
        [Column("arsenal")] public virtual uint Arsenal { get; set; }
        [Column("expiration")] public virtual DateTime? Expiration { get; set; }
        [Column("join_date")] public virtual DateTime JoinDate { get; set; }
    }
}