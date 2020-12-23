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
        MsgUserInfo = 1006,
        MsgItemInfo = 1008,
        MsgItem = 1009,
        MsgTick = 1012,
        MsgName = 1015,
        MsgWeather,
        MsgFriend = 1019,
        MsgInteract = 1022,
        MsgTeam = 1023,
        MsgAllot = 1024,
        MsgWeaponSkill = 1025,
        MsgTeamMember = 1026,
        MsgGemEmbed = 1027,
        MsgFuse = 1028,
        MsgTeamAward = 1029,
        MsgData = 1033,
        MsgEnemyList = 1041,
        MsgMonsterTransform = 1042,
        MsgTeamRoll = 1043,
        MsgLoadMap = 1044,
        MsgTrade = 1056,
        // MsgAccount = 1051,
        MsgConnect = 1052,
        MsgConnectEx = 1055,
        MsgAccount = 1086,
        MsgPCNum = 1100,
        MsgMapItem = 1101,
        MsgPackage = 1102,
        MsgMagicInfo = 1103,
        MsgFlushExp = 1104,
        MsgMagicEffect = 1105,
        MsgSyndicateAttributeInfo = 1106,
        MsgSyndicate = 1107,
        MsgItemInfoEx = 1108,
        MsgNpcInfoEx = 1109,
        MsgMapInfo = 1110,
        MsgMessageBoard = 1111,
        MsgSynMemberInfo = 1112,
        MsgDice = 1113,
        MsgSyncAction = 1114,
        MsgNpcInfo = 2030,
        MsgNpc = 2031,
        MsgTaskDialog = 2032,
        MsgFriendInfo = 2033,
        MsgDataArray = 2036,
        MsgTrainingInfo = 2043,
        MsgTraining = 2044,
        MsgTradeBuddy = 2046,
        MsgTradeBuffyInfo = 2047,
        MsgEquipLock = 2048,
        MsgPigeon = 2050,
        MsgPigeonQuery = 2051,
        MsgPeerage = 2064,
        MsgGuide = 2065,
        MsgGuideInfo = 2066,
        MsgGuideContribute = 2067,

        MsgWalk = 1005,
        MsgAction = 1010,
        MsgPlayer = 1014,
        MsgUserAttrib = 1017
    }
}