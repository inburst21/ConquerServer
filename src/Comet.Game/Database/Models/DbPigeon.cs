﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbPigeon.cs
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
    [Table("ad_log")]
    public class DbPigeon
    {
        [Key] [Column("id")] public virtual uint Identity { get; set; }
        [Column("user_id")] public virtual uint UserIdentity { get; set; }
        [Column("user_name")] public virtual string UserName { get; set; }
        [Column("time")] public virtual DateTime Time { get; set; }
        [Column("addition")] public virtual ushort Addition { get; set; }
        [Column("words")] public virtual string Message { get; set; }
    }
}