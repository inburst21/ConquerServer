// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - PacketTypes.cs
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

namespace Comet.Network.Packets
{
    /// <summary>
    ///     Packet types for the Conquer Online game client across all server projects.
    ///     Identifies packets by an unsigned short from offset 2 of every packet sent to
    ///     the server.
    /// </summary>
    public enum PacketType : ushort
    {
        MsgRegister = 1001,
        MsgTalk = 1004,
        MsgWalk,
        MsgUserInfo,
        MsgItemInfo = 1008,
        MsgItem,
        MsgAction,
        MsgTick = 1012,
        MsgPlayer = 1014,
        MsgName,
        MsgWeather,
        MsgUserAttrib,
        MsgFriend = 1019,
        MsgInteract = 1022,
        MsgTeam,
        MsgAllot,
        MsgWeaponSkill,
        MsgTeamMember,
        MsgGemEmbed,
        MsgFuse,
        MsgTeamAward,
        MsgEnemyList = 1041,
        MsgMonsterTransform,
        MsgTeamRoll,
        MsgLoadMap,
        MsgTrade = 1056,
        MsgAccount = 1051,
        MsgConnect,
        MsgConnectEx = 1055,
        MsgMapItem = 1101,
        MsgPackage,
        MsgMagicInfo,
        MsgFlushExp,
        MsgMagicEffect,
        MsgSyndicateAttributeInfo,
        MsgSyndicate,
        MsgItemInfoEx,
        MsgNpcInfoEx,
        MsgMapInfo,
        MsgMessageBoard,
        MsgSynMemberInfo = 1112,
        MsgDice = 1113,
        MsgSyncAction,
        MsgNpcInfo = 2030,
        MsgNpc,
        MsgTaskDialog,
        MsgFriendInfo,
        MsgDataArray = 2036,
        MsgPigeon = 2050,
        MsgPigeonQuery = 2051,
        MsgPeerage = 2064,
    }
}