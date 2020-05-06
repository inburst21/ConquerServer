// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DbItemtype.cs
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comet.Game.Database.Models
{
    [Table("cq_itemtype")]
    public class DbItemtype
    {
        [Key] [Column("id")] public virtual uint Type { get; set; }
        [Column("name")] public virtual string Name { get; set; }
        [Column("req_profession")] public virtual byte ReqProfession { get; set; }
        [Column("req_weaponskill")] public virtual byte ReqWeaponskill { get; set; }
        [Column("req_level")] public virtual byte ReqLevel { get; set; }
        [Column("req_sex")] public virtual byte ReqSex { get; set; }
        [Column("req_force")] public virtual ushort ReqForce { get; set; }
        [Column("req_speed")] public virtual ushort ReqSpeed { get; set; }
        [Column("req_health")] public virtual ushort ReqHealth { get; set; }
        [Column("req_soul")] public virtual ushort ReqSoul { get; set; }
        [Column("monopoly")] public virtual byte Monopoly { get; set; }
        [Column("weight")] public virtual ushort Weight { get; set; }
        [Column("price")] public virtual uint Price { get; set; }
        [Column("id_action")] public virtual uint IdAction { get; set; }
        [Column("attack_max")] public virtual ushort AttackMax { get; set; }
        [Column("attack_min")] public virtual ushort AttackMin { get; set; }
        [Column("defense")] public virtual short Defense { get; set; }
        [Column("dexterity")] public virtual short Dexterity { get; set; }
        [Column("dodge")] public virtual short Dodge { get; set; }
        [Column("life")] public virtual short Life { get; set; }
        [Column("mana")] public virtual short Mana { get; set; }
        [Column("amount")] public virtual ushort Amount { get; set; }

        [Column("amount_limit")] public virtual ushort AmountLimit { get; set; }

        //[Column("weapon_req")] public virtual uint RequireWeaponType { get; set; } // 2014-12-14
        [Column("ident")] public virtual byte Ident { get; set; }
        [Column("gem1")] public virtual byte Gem1 { get; set; }
        [Column("gem2")] public virtual byte Gem2 { get; set; }
        [Column("magic1")] public virtual byte Magic1 { get; set; }
        [Column("magic2")] public virtual byte Magic2 { get; set; }
        [Column("magic3")] public virtual byte Magic3 { get; set; }
        [Column("magic_atk")] public virtual ushort MagicAtk { get; set; }
        [Column("magic_def")] public virtual ushort MagicDef { get; set; }
        [Column("atk_range")] public virtual ushort AtkRange { get; set; }
        [Column("atk_speed")] public virtual ushort AtkSpeed { get; set; }
        [Column("fray_mode")] public virtual byte FrayMode { get; set; }
        [Column("repair_mode")] public virtual byte RepairMode { get; set; }
        [Column("type_mask")] public virtual byte TypeMask { get; set; }
        [Column("emoney_price")] public virtual uint EmoneyPrice { get; set; }
        [Column("emoney2_price")] public virtual uint BoundEmoneyPrice { get; set; } // 2014-12-14
        [Column("crit_strike")] public virtual uint CritStrike { get; set; }
        [Column("skill_crit_strike")] public virtual uint SkillCritStrike { get; set; }
        [Column("immunity")] public virtual uint Immunity { get; set; }
        [Column("penetration")] public virtual uint Penetration { get; set; }
        [Column("block")] public virtual uint Block { get; set; }
        [Column("breakthrough")] public virtual uint Breakthrough { get; set; }
        [Column("counteraction")] public virtual uint Counteraction { get; set; }
        [Column("detoxication")] public virtual uint Detoxication { get; set; }
        [Column("stack_limit")] public virtual ushort StackLimit { get; set; }
        [Column("resist_metal")] public virtual uint ResistMetal { get; set; }
        [Column("resist_wood")] public virtual uint ResistWood { get; set; }
        [Column("resist_water")] public virtual uint ResistWater { get; set; }
        [Column("resist_fire")] public virtual uint ResistFire { get; set; }
        [Column("resist_earth")] public virtual uint ResistEarth { get; set; }
        [Column("phase")] public virtual byte Phase { get; set; }

        [Column("meteor_num")] public virtual uint MeteorAmount { get; set; } // 2014-12-14
        //[Column("honor_price")] public virtual uint HonorPrice { get; set; } // 2014-12-14
        //[Column("life_time")] public virtual uint LifeTime { get; set; } // 2014-12-25
    }
}