// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Game Action.cs
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

using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Shared;

namespace Comet.Game.States
{
    public static class GameAction
    {
        public static async Task<bool> ExecuteActionAsync(uint idAction, Character user, Role role, Item item, string input)
        {
            const int _MAX_ACTION_I = 64;
            const int _DEADLOCK_CHECK_I = 5;

            if (idAction == 0)
                return false;

            int actionCount = 0;
            uint idNext = idAction, idOld = idAction;
            while (idNext > 0)
            {
                if (actionCount++ > _MAX_ACTION_I)
                {
                    await Log.WriteLog(LogLevel.Error, $"Error: too many game action, from: {idAction}, last action: {idNext}");
                    return false;
                }

                DbAction action = Kernel.EventManager.GetAction(idNext);
                if (action == null)
                {
                    await Log.WriteLog(LogLevel.Error, $"Error: invalid game action: {idNext}");
                    return false;
                }

                bool result = false;
                switch ((TaskActionType) action.Type)
                {
                    
                }

                idNext = result ? action.IdNext : action.IdNextfail;
            }
            return true;
        }

        private static string FormatParam(DbAction action, Character user, Role role, Item item, string input)
        {
            string result = action.Param;

            return result;
        }
    }

    public enum TaskActionType
    {
        // System
        ActionSysFirst = 100,
        ActionMenutext = 101,
        ActionMenulink = 102,
        ActionMenuedit = 103,
        ActionMenupic = 104,
        ActionMenubutton = 110,
        ActionMenulistpart = 111,
        ActionMenucreate = 120,
        ActionRand = 121,
        ActionRandaction = 122,
        ActionChktime = 123,
        ActionPostcmd = 124,
        ActionBrocastmsg = 125,
        ActionMessagebox = 126,
        ActionExecutequery = 127,
        ActionSysLimit = 199,

        //NPC
        ActionNpcFirst = 200,
        ActionNpcAttr = 201,
        ActionNpcErase = 205,
        ActionNpcModify = 206,
        ActionNpcResetsynowner = 207,
        ActionNpcFindNextTable = 208,
        ActionNpcAddTable = 209,
        ActionNpcDelTable = 210,
        ActionNpcDelInvalid = 211,
        ActionNpcTableAmount = 212,
        ActionNpcSysAuction = 213,
        ActionNpcDressSynclothing = 214,
        ActionNpcTakeoffSynclothing = 215,
        ActionNpcAuctioning = 216,
        ActionNpcLimit = 299,

        // Map
        ActionMapFirst = 300,
        ActionMapMovenpc = 301,
        ActionMapMapuser = 302,
        ActionMapBrocastmsg = 303,
        ActionMapDropitem = 304,
        ActionMapSetstatus = 305,
        ActionMapAttrib = 306,
        ActionMapRegionMonster = 307,
        ActionMapChangeweather = 310,
        ActionMapChangelight = 311,
        ActionMapMapeffect = 312,
        ActionMapCreatemap = 313,
        ActionMapFireworks = 314,
        ActionMapLimit = 399,

        // Lay item
        ActionItemonlyFirst = 400,
        ActionItemRequestlaynpc = 401,
        ActionItemCountnpc = 402,
        ActionItemLaynpc = 403,
        ActionItemDelthis = 498,
        ActionItemonlyLimit = 499,
        
        // Item
        ActionItemFirst = 500,
        ActionItemAdd = 501,
        ActionItemDel = 502,
        ActionItemCheck = 503,
        ActionItemHole = 504,
        ActionItemRepair = 505,
        ActionItemMultidel = 506,
        ActionItemMultichk = 507,
        ActionItemLeavespace = 508,
        ActionItemUpequipment = 509,
        ActionItemEquiptest = 510,
        ActionItemEquipexist = 511,
        ActionItemEquipcolor = 512,
        ActionItemRemoveAny = 513,
        ActionItemCheckrand = 516,
        ActionItemModify = 517,
        ActionItemJarCreate = 528,
        ActionItemJarVerify = 529,
        ActionItemLimit = 599,

        // Dyn NPCs
        ActionNpconlyFirst = 600,
        ActionNpconlyCreatenewPet = 601,
        ActionNpconlyDeletePet = 602,
        ActionNpconlyMagiceffect = 603,
        ActionNpconlyMagiceffect2 = 604,
        ActionNpconlyLimit = 699,

        // Syndicate
        ActionSynFirst = 700,
        ActionSynCreate = 701,
        ActionSynDestroy = 702,
        ActionSynAttr = 717,
        ActionSynChangeLeader = 709,
        ActionSynLimit = 799,

        //Monsters
        ActionMstFirst = 800,
        ActionMstDropitem = 801,
        ActionMstMagic = 802,
        ActionMstRefinery = 803,
        ActionMstLimit = 899,

        //Family
        ActionFamilyFirst = 900,
        ActionFamilyCreate = 901,
        ActionFamilyDestroy = 902,
        ActionFamilyAttr = 917,
        ActionFamilyUplev = 918,
        ActionFamilyBpuplev = 919,
        ActionFamilyLimit = 999,

        //User
        ActionUserFirst = 1000,
        ActionUserAttr = 1001,
        ActionUserFull = 1002, // Fill the user attributes. param is the attribute name. life/mana/xp/sp
        ActionUserChgmap = 1003, // Mapid Mapx Mapy savelocation
        ActionUserRecordpoint = 1004, // Records the user location, so he can be teleported back there later.
        ActionUserHair = 1005,
        ActionUserChgmaprecord = 1006,
        ActionUserChglinkmap = 1007,
        ActionUserTransform = 1008,
        ActionUserIspure = 1009,
        ActionUserTalk = 1010,
        ActionUserMagic = 1020,
        ActionUserWeaponskill = 1021,
        ActionUserLog = 1022,
        ActionUserBonus = 1023,
        ActionUserDivorce = 1024,
        ActionUserMarriage = 1025,
        ActionUserSex = 1026,
        ActionUserEffect = 1027,
        ActionUserTaskmask = 1028,
        ActionUserMediaplay = 1029,
        ActionUserSupermanlist = 1030,
        ActionUserAddTitle = 1031,
        ActionUserRemoveTitle = 1032,
        ActionUserCreatemap = 1033,
        ActionUserEnterHome = 1034,
        ActionUserEnterMateHome = 1035,
        ActionUserChkinCard2 = 1036,
        ActionUserChkoutCard2 = 1037,
        ActionUserFlyNeighbor = 1038,
        ActionUserUnlearnMagic = 1039,
        ActionUserRebirth = 1040,
        ActionUserWebpage = 1041,
        ActionUserBbs = 1042,
        ActionUserUnlearnSkill = 1043,
        ActionUserDropMagic = 1044,
        ActionUserFixAttr = 1045,
        ActionUserOpenDialog = 1046,
        ActionUserPointAllot = 1047,
        ActionUserExpMultiply = 1048,
        ActionUserDelWpgBadge = 1049,
        ActionUserChkWpgBadge = 1050,
        ActionUserTakestudentexp = 1051,
        ActionUserWhPassword = 1052,
        ActionUserSetWhPassword = 1053,
        ActionUserOpeninterface = 1054,
        ActionUserVarCompare = 1060,
        ActionUserVarDefine = 1061,
        ActionUserVarCalc = 1064,
        ActionUserStcCompare = 1073,
        ActionUserStcOpe = 1074,
        ActionUserTaskManager = 1080,
        ActionUserTaskOpe = 1081,
        ActionUserAttachStatus = 1082,
        ActionUserGodTime = 1083,
        ActionUserExpballExp = 1086,
        ActionUserStatusCreate = 1096,
        ActionUserStatusCheck = 1098,

        //User -> Team
        ActionTeamBroadcast = 1101,
        ActionTeamAttr = 1102,
        ActionTeamLeavespace = 1103,
        ActionTeamItemAdd = 1104,
        ActionTeamItemDel = 1105,
        ActionTeamItemCheck = 1106,
        ActionTeamChgmap = 1107,
        ActionTeamChkIsleader = 1501,

        // User -> General
        ActionGeneralLottery = 1508,

        ActionUserLimit = 1999,

        //Events
        ActionEventFirst = 2000,
        ActionEventSetstatus = 2001,
        ActionEventDelnpcGenid = 2002,
        ActionEventCompare = 2003,
        ActionEventCompareUnsigned = 2004,
        ActionEventChangeweather = 2005,
        ActionEventCreatepet = 2006,
        ActionEventCreatenewNpc = 2007,
        ActionEventCountmonster = 2008,
        ActionEventDeletemonster = 2009,
        ActionEventBbs = 2010,
        ActionEventErase = 2011,
        ActionEventTeleport = 2012,
        ActionEventMassaction = 2013,
        ActionEventLimit = 2099,

        //Traps
        ActionTrapFirst = 2100,
        ActionTrapCreate = 2101,
        ActionTrapErase = 2102,
        ActionTrapCount = 2103,
        ActionTrapAttr = 2104,
        ActionTrapLimit = 2199,

        // Detained Item
        ActionDetainFirst = 2200,
        ActionDetainDialog = 2205,
        ActionDetainLimit = 2299,

        //Wanted
        ActionWantedFirst = 3000,
        ActionWantedNext = 3001,
        ActionWantedName = 3002,
        ActionWantedBonuty = 3003,
        ActionWantedNew = 3004,
        ActionWantedOrder = 3005,
        ActionWantedCancel = 3006,
        ActionWantedModifyid = 3007,
        ActionWantedSuperadd = 3008,
        ActionPolicewantedNext = 3010,
        ActionPolicewantedOrder = 3011,
        ActionPolicewantedCheck = 3012,
        ActionWantedLimit = 3099,

        //Magic
        ActionMagicFirst = 4000,
        ActionMagicAttachstatus = 4001,
        ActionMagicAttack = 4002,
        ActionMagicLimit = 4099
    }
}