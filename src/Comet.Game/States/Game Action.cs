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

using System;
using System.Drawing;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.States.NPCs;
using Comet.Game.World.Maps;
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
            int deadLookCount = 0;
            uint idNext = idAction, idOld = idAction;
            while (idNext > 0)
            {
                if (actionCount++ > _MAX_ACTION_I)
                {
                    await Log.WriteLog(LogLevel.Error, $"Error: too many game action, from: {idAction}, last action: {idNext}");
                    return false;
                }

                if (idAction == idOld && deadLookCount++ >= _DEADLOCK_CHECK_I)
                {
                    await Log.WriteLog(LogLevel.Error, $"Error: dead loop detected, from: {idAction}, last action: {idNext}");
                    return false;
                }
                else
                {
                    deadLookCount = 0;
                }

                DbAction action = Kernel.EventManager.GetAction(idNext);
                if (action == null)
                {
                    await Log.WriteLog(LogLevel.Error, $"Error: invalid game action: {idNext}");
                    return false;
                }

                string param = FormatParam(action, user, role, item, input);
                if (user?.IsPm() == true)
                {
                    await user.SendAsync(
                        $"{action.Identity}: [{action.IdNext},{action.IdNextfail}]. type[{action.Type}], data[{action.Data}], param:[{param}].",
                        MsgTalk.TalkChannel.System,
                        Color.White);
                }

                bool result = false;
                switch ((TaskActionType) action.Type)
                {
                    case TaskActionType.ActionMenutext: result = await ExecuteActionMenuText(action, param, user, role, item, input); break;
                    case TaskActionType.ActionMenulink: result = await ExecuteActionMenuLink(action, param, user, role, item, input); break;
                    case TaskActionType.ActionMenuedit: result = await ExecuteActionMenuEdit(action, param, user, role, item, input); break;
                    case TaskActionType.ActionMenupic: result = await ExecuteActionMenuPic(action, param, user, role, item, input); break;
                    case TaskActionType.ActionMenucreate: result = await ExecuteActionMenuCreate(action, param, user, role, item, input); break;
                    case TaskActionType.ActionBrocastmsg: result = await ExecuteActionBrocastmsg(action, param, user, role, item, input); break;
                    case TaskActionType.ActionRand: result = await ExecuteActionMenuRand(action, param, user, role, item, input); break;
                    case TaskActionType.ActionRandaction: result = await ExecuteActionMenuRandAction(action, param, user, role, item, input); break;
                    case TaskActionType.ActionChktime: result = await ExecuteActionMenuChkTime(action, param, user, role, item, input); break;

                    case TaskActionType.ActionItemAdd: result = await ExecuteActionItemAdd(action, param, user, role, item, input); break;

                    case TaskActionType.ActionUserAttr: result = await ExecuteUserAttr(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserFull: result = await ExecuteUserFull(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserChgmap: result = await ExecuteUserChgMap(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserRecordpoint: result = await ExecuteUserRecordpoint(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserHair: result = await ExecuteUserHair(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserChgmaprecord: result = await ExecuteUserChgmaprecord(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserTransform: result = await ExecuteUserTransform(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserTalk: result = await ExecuteActionUserTalk(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserMagic: result = await ExecuteActionUserMagic(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserWeaponskill: result = await ExecuteActionUserWeaponSkill(action, param, user, role, item, input); break;
                    case TaskActionType.ActionUserLog: result = await ExecuteActionUserLog(action, param, user, role, item, input); break;

                    default:
                        await Log.WriteLog(LogLevel.Warning, $"GameAction::ExecuteActionAsync unhandled action type {action.Type} for action: {action.Identity}");
                        break;
                }

                idOld = idAction;
                idNext = result ? action.IdNext : action.IdNextfail;
            }
            return true;
        }

        #region Action
        
        private static async Task<bool> ExecuteActionMenuText(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Action[{action.Identity}] type 101 non character");
                return false;
            }

            await user.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.Dialog,
                Text = param,
                Data = (ushort) action.Data
            });
            return true;
        }

        private static async Task<bool> ExecuteActionMenuLink(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Action[{action.Identity}] type 101 non character");
                return false;
            }

            uint task = 0;
            int align = 0;
            string[] parsed = param.Split(' ');
            if (parsed.Length > 1)
                uint.TryParse(parsed[1], out task);
            if (parsed.Length > 2)
                int.TryParse(parsed[2], out align);

            await user.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.Option,
                Text = parsed[0],
                OptionIndex = user.PushTaskId(task),
                Data = (ushort) align
            });
            return true;
        }

        private static async Task<bool> ExecuteActionMenuEdit(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] paramStrings = SplitParam(param, 3);
            if (paramStrings.Length < 3)
            {
                await Log.WriteLog(LogLevel.Error, $"Invalid input param length for {action.Identity}, param: {param}");
                return false;
            }

            await user.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.Input,
                OptionIndex = user.PushTaskId(uint.Parse(paramStrings[1])),
                Data = ushort.Parse(paramStrings[0]),
                Text = paramStrings[2]
            });

            return true;
        }

        private static async Task<bool> ExecuteActionMenuPic(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParam = SplitParam(param);

            ushort x = ushort.Parse(splitParam[0]);
            ushort y = ushort.Parse(splitParam[1]);

            await user.SendAsync(new MsgTaskDialog
            {
                TaskIdentity = (uint) ((x << 16) | y),
                InteractionType = MsgTaskDialog.TaskInteraction.Avatar,
                Data = ushort.Parse(splitParam[2])
            });
            return true;
        }

        private static async Task<bool> ExecuteActionMenuCreate(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            await user.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.Finish
            });
            return true;
        }

        private static async Task<bool> ExecuteActionMenuRand(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            string[] paramSplit = SplitParam(param, 2);

            int x = int.Parse(paramSplit[0]);
            int y = int.Parse(paramSplit[1]);
            double chance = 0.01;
            if (x > y)
                chance = 99;
            else
            {
                chance = x / (double) y;
                chance *= 100;
            }

            return await Kernel.ChanceCalcAsync(chance);
        }

        private static async Task<bool> ExecuteActionMenuRandAction(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            string[] paramSplit = SplitParam(param);
            if (paramSplit.Length == 0)
                return false;
            uint taskId = uint.Parse(paramSplit[await Kernel.NextAsync(0, paramSplit.Length) % paramSplit.Length]);
            return await ExecuteActionAsync(taskId, user, role, item, input);
        }

        private static async Task<bool> ExecuteActionMenuChkTime(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            string[] paramSplit = SplitParam(param);

            DateTime actual = DateTime.Now;
            var nCurWeekDay = (int)actual.DayOfWeek;
            int nCurHour = actual.Hour;
            int nCurMinute = actual.Minute;

            switch (action.Data)
            {
                #region Complete date (yyyy-mm-dd hh:mm yyyy-mm-dd hh:mm)

                case 0:
                    {
                        if (paramSplit.Length < 4)
                            return false;

                        string[] time0 = paramSplit[1].Split(':');
                        string[] date0 = paramSplit[0].Split('-');
                        string[] time1 = paramSplit[3].Split(':');
                        string[] date1 = paramSplit[2].Split('-');

                        var dTime0 = new DateTime(int.Parse(date0[0]), int.Parse(date0[1]),int.Parse(date0[2]), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(int.Parse(date1[0]), int.Parse(date1[1]),int.Parse(date1[2]), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        return dTime0 <= actual && dTime1 >= actual;
                    }

                #endregion

                #region On Year date (mm-dd hh:mm mm-dd hh:mm)

                case 1:
                    {
                        if (paramSplit.Length < 4)
                            return false;

                        string[] time0 = paramSplit[1].Split(':');
                        string[] date0 = paramSplit[0].Split('-');
                        string[] time1 = paramSplit[3].Split(':');
                        string[] date1 = paramSplit[2].Split('-');

                        var dTime0 = new DateTime(DateTime.Now.Year, int.Parse(date0[1]),int.Parse(date0[2]), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(DateTime.Now.Year, int.Parse(date1[1]), int.Parse(date1[2]), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        return dTime0 <= actual && dTime1 >= actual;
                    }

                #endregion

                #region Day of the month (dd hh:mm dd hh:mm)

                case 2:
                    {
                        if (paramSplit.Length < 4)
                            return false;

                        string[] time0 = paramSplit[1].Split(':');
                        string date0 = paramSplit[0];
                        string[] time1 = paramSplit[3].Split(':');
                        string date1 = paramSplit[2];

                        var dTime0 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(date0), int.Parse(time0[0]), int.Parse(time0[1]), 0);
                        var dTime1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, int.Parse(date1), int.Parse(time1[0]), int.Parse(time1[1]), 59);

                        return dTime0 <= actual && dTime1 >= actual;
                    }

                #endregion

                #region Day of the week (dw hh:mm dw hh:mm)

                case 3:
                    {
                        if (paramSplit.Length < 4)
                            return false;

                        string[] time0 = paramSplit[1].Split(':');
                        string[] time1 = paramSplit[3].Split(':');

                        int nDay0 = int.Parse(paramSplit[0]);
                        int nDay1 = int.Parse(paramSplit[2]);
                        int nHour0 = int.Parse(time0[0]);
                        int nHour1 = int.Parse(time1[0]);
                        int nMinute0 = int.Parse(time0[1]);
                        int nMinute1 = int.Parse(time1[1]);

                        int timeNow = nCurWeekDay * 24 * 60 + nCurHour * 60 + nCurMinute;
                        int from = nDay0 * 24 * 60 + nHour0 * 60 + nMinute0;
                        int to = nDay1 * 24 * 60 + nHour1 * 60 + nMinute1;

                        return timeNow >= from && timeNow <= to;
                    }

                #endregion

                #region Hour check (hh:mm hh:mm)

                case 4:
                    {
                        if (paramSplit.Length < 2)
                            return false;

                        string[] time0 = paramSplit[0].Split(':');
                        string[] time1 = paramSplit[1].Split(':');

                        int nHour0 = int.Parse(time0[0]);
                        int nHour1 = int.Parse(time1[0]);
                        int nMinute0 = int.Parse(time0[1]);
                        int nMinute1 = int.Parse(time1[1]);

                        int timeNow = nCurHour * 60 + nCurMinute;
                        int from = nHour0 * 60 + nMinute0;
                        int to = nHour1 * 60 + nMinute1;

                        return timeNow >= from && timeNow <= to;
                    }

                #endregion

                #region Minute check (mm mm)

                case 5:
                    {
                        if (paramSplit.Length < 2)
                            return false;

                        return nCurMinute >= int.Parse(paramSplit[0]) && nCurMinute <= int.Parse(paramSplit[1]);
                    }

                    #endregion
            }

            return false;
        }
        
        private static async Task<bool> ExecuteActionBrocastmsg(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            await Kernel.RoleManager.BroadcastMsgAsync(param, (MsgTalk.TalkChannel) action.Data, Color.White);
            return true;
        }

        #endregion

        #region Item

        private static async Task<bool> ExecuteActionItemAdd(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user?.UserPackage == null)
                return false;

            if (!user.UserPackage.IsPackSpare(1))
                return false;

            DbItemtype itemtype = Kernel.ItemManager.GetItemtype(action.Data);
            if (itemtype == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid itemtype: {action.Identity}, {action.Type}, {action.Data}");
                return false;
            }

            string[] splitParam = SplitParam(param);
            DbItem newItem = Item.CreateEntity(action.Data);
            newItem.PlayerId = user.Identity;

            for (int i = 0; i < splitParam.Length; i++)
            {
                if (!int.TryParse(splitParam[i], out var value))
                    continue;

                switch (i)
                {
                    case 0: // amount
                        newItem.Amount = (ushort) Math.Min(value, ushort.MaxValue);
                        break;
                    case 1: // amount limit
                        newItem.AmountLimit = (ushort)Math.Min(value, ushort.MaxValue);
                        break;
                    case 2: // socket progress
                        newItem.Data = (uint) Math.Min(value, ushort.MaxValue);
                        break;
                    case 3: // gem 1
                        if (Enum.IsDefined(typeof(Item.SocketGem), (byte) value))
                            newItem.Gem1 = (byte) value;
                        break;
                    case 4: // gem 2
                        if (Enum.IsDefined(typeof(Item.SocketGem), (byte)value))
                            newItem.Gem2 = (byte)value;
                        break;
                    case 5: // effect magic 1
                        if (Enum.IsDefined(typeof(Item.ItemEffect), (ushort)value))
                            newItem.Magic1 = (byte)value;
                        break;
                    case 6: // magic 2
                        newItem.Magic2 = (byte) value;
                        break;
                    case 7: // magic 3
                        newItem.Magic3 = (byte) value;
                        break;
                    case 8: // reduce dmg
                        newItem.ReduceDmg = (byte) Math.Min(byte.MaxValue, value);
                        break;
                    case 9: // add life
                        newItem.AddLife = (byte)Math.Min(byte.MaxValue, value);
                        break;
                    case 10: // plunder
                        newItem.Plunder = (uint) value;
                        break;
                    case 11: // color
                        if (Enum.IsDefined(typeof(Item.ItemColor), (byte)value))
                            newItem.Color = (byte)value;
                        break;
                }
            }
            
            item = new Item(user);
            if (!await item.CreateAsync(newItem))
                return false;

            return await user.UserPackage.AddItem(item);
        }

        #endregion

        #region User

        private static async Task<bool> ExecuteUserAttr(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] parsedParam = SplitParam(param);
            if (parsedParam.Length < 3)
            {
                await Log.WriteLog(LogLevel.Error, $"GameAction::ExecuteUserAttr[{action.Identity}] invalid param num {param}");
                return false;
            }

            string type = "", opt = "", value = "", last = "";
            type = parsedParam[0];
            opt = parsedParam[1];
            value = parsedParam[2];
            if (parsedParam.Length > 3)
                last = parsedParam[3];

            switch (type.ToLower())
            {
                #region Force (>, >=, <, <=, =, +=, set)

                case "force":
                case "strength":
                    int forceValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Strength > forceValue;
                    if (opt.Equals(">="))
                        return user.Strength >= forceValue;
                    if (opt.Equals("<"))
                        return user.Strength < forceValue;
                    if (opt.Equals("<="))
                        return user.Strength <= forceValue;
                    if (opt.Equals("="))
                        return user.Strength == forceValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Strength, forceValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Strength, forceValue);
                        return true;
                    }
                    break;

                #endregion

                #region Speed (>, >=, <, <=, =, +=, set)

                case "agility":
                case "speed":
                    int speedValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Agility > speedValue;
                    if (opt.Equals(">="))
                        return user.Agility >= speedValue;
                    if (opt.Equals("<"))
                        return user.Agility < speedValue;
                    if (opt.Equals("<="))
                        return user.Agility <= speedValue;
                    if (opt.Equals("="))
                        return user.Agility == speedValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Agility, speedValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Agility, speedValue);
                        return true;
                    }
                    break;

                #endregion

                #region Health (>, >=, <, <=, =, +=, set)

                case "vitality":
                case "health":
                    int healthValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Vitality > healthValue;
                    if (opt.Equals(">="))
                        return user.Vitality >= healthValue;
                    if (opt.Equals("<"))
                        return user.Vitality < healthValue;
                    if (opt.Equals("<="))
                        return user.Vitality <= healthValue;
                    if (opt.Equals("="))
                        return user.Vitality == healthValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Vitality, healthValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Vitality, healthValue);
                        return true;
                    }
                    break;

                #endregion

                #region Soul (>, >=, <, <=, =, +=, set)

                case "spirit":
                case "soul":
                    int soulValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Spirit > soulValue;
                    if (opt.Equals(">="))
                        return user.Spirit >= soulValue;
                    if (opt.Equals("<"))
                        return user.Spirit < soulValue;
                    if (opt.Equals("<="))
                        return user.Spirit <= soulValue;
                    if (opt.Equals("="))
                        return user.Spirit == soulValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Spirit, soulValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Spirit, soulValue);
                        return true;
                    }
                    break;

                #endregion

                #region Attribute Points (>, >=, <, <=, =, +=, set)

                case "attr_points":
                case "attr":
                    int attrValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.AttributePoints > attrValue;
                    if (opt.Equals(">="))
                        return user.AttributePoints >= attrValue;
                    if (opt.Equals("<"))
                        return user.AttributePoints < attrValue;
                    if (opt.Equals("<="))
                        return user.AttributePoints <= attrValue;
                    if (opt.Equals("="))
                        return user.AttributePoints == attrValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Atributes, attrValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Atributes, attrValue);
                        return true;
                    }
                    break;

                #endregion    

                #region Level (>, >=, <, <=, =, +=, set)

                case "level":
                    int levelValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Level > levelValue;
                    if (opt.Equals(">="))
                        return user.Level >= levelValue;
                    if (opt.Equals("<"))
                        return user.Level < levelValue;
                    if (opt.Equals("<="))
                        return user.Level <= levelValue;
                    if (opt.Equals("="))
                        return user.Level == levelValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Level, levelValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Level, levelValue);
                        return true;
                    }
                    break;

                #endregion

                #region Metempsychosis (>, >=, <, <=, =, +=, set)

                case "metempsychosis":
                    int metempsychosisValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Metempsychosis > metempsychosisValue;
                    if (opt.Equals(">="))
                        return user.Metempsychosis >= metempsychosisValue;
                    if (opt.Equals("<"))
                        return user.Metempsychosis < metempsychosisValue;
                    if (opt.Equals("<="))
                        return user.Metempsychosis <= metempsychosisValue;
                    if (opt.Equals("="))
                        return user.Metempsychosis == metempsychosisValue;
                    if (opt.Equals("+="))
                    {
                        await user.AddAttributesAsync(ClientUpdateType.Reborn, metempsychosisValue);
                        return true;
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Reborn, metempsychosisValue);
                        return true;
                    }
                    break;

                #endregion

                #region Money (>, >=, <, <=, =, +=, set)

                case "money":
                case "silver":
                    int moneyValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.Silvers > moneyValue;
                    if (opt.Equals(">="))
                        return user.Silvers >= moneyValue;
                    if (opt.Equals("<"))
                        return user.Silvers < moneyValue;
                    if (opt.Equals("<="))
                        return user.Silvers <= moneyValue;
                    if (opt.Equals("="))
                        return user.Silvers == moneyValue;
                    if (opt.Equals("+="))
                    {
                        return await user.ChangeMoney(moneyValue);
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.Money, moneyValue);
                        return true;
                    }
                    break;

                #endregion

                #region Emoney (>, >=, <, <=, =, +=, set)

                case "emoney":
                case "e_money":
                case "cps":
                    int emoneyValue = int.Parse(value);
                    if (opt.Equals(">"))
                        return user.ConquerPoints > emoneyValue;
                    if (opt.Equals(">="))
                        return user.ConquerPoints >= emoneyValue;
                    if (opt.Equals("<"))
                        return user.ConquerPoints < emoneyValue;
                    if (opt.Equals("<="))
                        return user.ConquerPoints <= emoneyValue;
                    if (opt.Equals("="))
                        return user.ConquerPoints == emoneyValue;
                    if (opt.Equals("+="))
                    {
                        return await user.ChangeConquerPoints(emoneyValue);
                    }
                    if (opt.Equals("set"))
                    {
                        await user.SetAttributesAsync(ClientUpdateType.ConquerPoints, emoneyValue);
                        return true;
                    }
                    break;

                #endregion

                #region Rankshow (>, >=, <, <=, =)

                case "rank_show":
                    int rankShowValue = int.Parse(value);
                    // hummmm
                    break;

                #endregion


            }

            return false;
        }

        private static async Task<bool> ExecuteUserFull(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            if (param.Equals("life", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetAttributesAsync(ClientUpdateType.Hitpoints, user.MaxLife);
                return true;
            }
            if (param.Equals("mana", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetAttributesAsync(ClientUpdateType.Mana, user.MaxMana);
                return true;
            }
            if (param.Equals("xp", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetXp(100);
                await user.BurstXp();
                return true;
            }
            if (param.Equals("sp", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetAttributesAsync(ClientUpdateType.Stamina, user.MaxEnergy);
                return true;
            }
            return false;
        }

        private static async Task<bool> ExecuteUserChgMap(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] paramStrings = SplitParam(param);
            if (paramStrings.Length < 3)
            {
                await Log.WriteLog(LogLevel.Warning, $"Action {action.Identity}:{action.Type} invalid param length: {param}");
                return false;
            }

            if (!uint.TryParse(paramStrings[0], out var idMap)
                || !ushort.TryParse(paramStrings[1], out var x)
                || !ushort.TryParse(paramStrings[2], out var y))
                return false;

            GameMap map = Kernel.MapManager.GetMap(idMap);
            if (map == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid map identity {idMap} for action {action.Identity}");
                return false;
            }

            return await user.FlyMap(idMap, x, y);
        }

        private static async Task<bool> ExecuteUserRecordpoint(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] paramStrings = SplitParam(param);
            if (paramStrings.Length < 3)
            {
                await Log.WriteLog(LogLevel.Warning, $"Action {action.Identity}:{action.Type} invalid param length: {param}");
                return false;
            }

            if (!uint.TryParse(paramStrings[0], out var idMap)
                || !ushort.TryParse(paramStrings[1], out var x)
                || !ushort.TryParse(paramStrings[2], out var y))
                return false;

            if (idMap == 0)
            {
                await user.SavePositionAsync(user.MapIdentity, user.MapX, user.MapY);
                return true;
            }

            GameMap map = Kernel.MapManager.GetMap(idMap);
            if (map == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid map identity {idMap} for action {action.Identity}");
                return false;
            }

            await user.SavePositionAsync(idMap, x, y);
            return true;
        }

        private static async Task<bool> ExecuteUserHair(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParams = SplitParam(param);

            if (splitParams.Length < 2)
            {
                await Log.WriteLog(LogLevel.Warning,
                    $"Action {action.Identity}:{action.Type} has not enough argments: {param}");
                return false;
            }

            int value = int.Parse(splitParams[1]);
            if (splitParams[0].Equals("style", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetAttributesAsync(ClientUpdateType.HairStyle, (ushort)(value + (user.Hairstyle - user.Hairstyle % 100)));
                return true;
            }
            if (splitParams[0].Equals("hair", StringComparison.InvariantCultureIgnoreCase))
            {
                await user.SetAttributesAsync(ClientUpdateType.HairStyle, (ushort) (user.Hairstyle % 100 + value * 100));
                return true;
            }
            return false;
        }

        private static async Task<bool> ExecuteUserChgmaprecord(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            await user.FlyMap(user.RecordMapIdentity, user.RecordMapX, user.RecordMapY);
            return true;
        }

        private static async Task<bool> ExecuteUserTransform(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParam = SplitParam(param);

            if (splitParam.Length < 4)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid param count for {action.Identity}:{action.Type}, {param}");
                return false;
            }

            uint transformation = uint.Parse(splitParam[2]);
            int time = int.Parse(splitParam[3]);
            return await user.Transform(transformation, time, true);
        }

        private static async Task<bool> ExecuteActionUserTalk(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
            {
                await Log.WriteLog(LogLevel.Warning, $"Action type 1010 {action.Identity} no user");
                return false;
            }

            await user.SendAsync(param, (MsgTalk.TalkChannel) action.Data, Color.White);
            return true;
        }

        private static async Task<bool> ExecuteActionUserMagic(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParam = SplitParam(param);
            if (splitParam.Length < 2)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid ActionUserMagic param length: {action.Identity}, {param}");
                return false;
            }

            switch (splitParam[0].ToLowerInvariant())
            {
                case "check":
                    if (splitParam.Length >= 3)
                        return user.MagicData.CheckLevel(ushort.Parse(splitParam[1]), ushort.Parse(splitParam[2]));
                    return user.MagicData.CheckType(ushort.Parse(splitParam[1]));

                case "learn":
                    if (splitParam.Length >= 3)
                        return await user.MagicData.Create(ushort.Parse(splitParam[1]), byte.Parse(splitParam[2]));
                    return await user.MagicData.Create(ushort.Parse(splitParam[1]), 0);

                case "uplev":
                    return await user.MagicData.UpLevelByTask(ushort.Parse(splitParam[1]));

                case "addexp":
                    return await user.MagicData.AwardExp(ushort.Parse(splitParam[1]), 0, int.Parse(splitParam[2]));

                default:
                    await Log.WriteLog(LogLevel.Warning, $"[ActionType: {action.Type}] Unknown {splitParam[0]} param {action.Identity}");
                    return false;
            }
        }

        private static async Task<bool> ExecuteActionUserWeaponSkill(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParam = SplitParam(param);

            if (splitParam.Length < 3)
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid param amount: {param} [{action.Identity}]");
                return false;
            }

            if (!ushort.TryParse(splitParam[1], out var type)
                || !int.TryParse(splitParam[2], out var value))
            {
                await Log.WriteLog(LogLevel.Warning, $"Invalid weapon skill type {param} for action {action.Identity}");
                return false;
            }

            switch (splitParam[0].ToLowerInvariant())
            {
                case "check":
                    return user.WeaponSkill[type]?.Level >= value;

                case "learn":
                    return await user.WeaponSkill.CreateAsync(type, (byte) value);

                case "addexp":
                    await user.AddWeaponSkillExpAsync(type, value);
                    return true;

                default:
                    await Log.WriteLog(LogLevel.Warning, $"ExecuteActionUserWeaponSkill {splitParam[0]} invalid {action.Identity}");
                    return false;
            }
        }

        private static async Task<bool> ExecuteActionUserLog(DbAction action, string param, Character user, Role role, Item item, string input)
        {
            if (user == null)
                return false;

            string[] splitParam = SplitParam(param, 2);

            if (splitParam.Length < 2)
            {
                await Log.WriteLog(LogLevel.Warning, $"ExecuteActionUserLog length {action.Identity}, {param}");
                return false;
            }

            string file = splitParam[0];
            string message = splitParam[1];

            if (file.StartsWith("gmlog/"))
                file = file.Remove(0, "gmlog/".Length);

            await Log.GmLog(file, message);
            return true;
        }

        #endregion

        private static string FormatParam(DbAction action, Character user, Role role, Item item, string input)
        {
            string result = action.Param;

            if (user != null)
            {
                result = result.Replace("%user_name", user.Name)
                    .Replace("%user_id", user.Identity.ToString())
                    .Replace("%user_lev", user.Level.ToString())
                    .Replace("%user_mate", user.Mate)
                    .Replace("%user_pro", user.Profession.ToString())
                    .Replace("%user_map_id", user.Map.Identity.ToString())
                    .Replace("%user_map_name", user.Map.Name)
                    .Replace("%user_map_x", user.MapX.ToString())
                    .Replace("%user_map_y", user.MapY.ToString())
                    .Replace("%map_owner_id", user.Map.OwnerIdentity.ToString())
                    .Replace("%user_nobility_rank", ((int)user.NobilityRank).ToString())
                    .Replace("%user_nobility_position", user.NobilityPosition.ToString());

                if (result.Contains("%levelup_exp"))
                {
                    DbLevelExperience db = Kernel.RoleManager.GetLevelExperience(user.Level);
                    result = result.Replace("%levelup_exp", db != null ? db.Exp.ToString() : "0");
                }
            }
            else
            {
                result = result.Replace("%user_name", "None")
                    .Replace("%user_id", "0")
                    .Replace("%user_lev", "0")
                    .Replace("%user_mate", "None")
                    .Replace("%user_pro", "0")
                    .Replace("%user_map_id", "0")
                    .Replace("%user_map_name", "None")
                    .Replace("%user_map_x", "0")
                    .Replace("%user_map_y", "0")
                    .Replace("%map_owner_id", "0")
                    .Replace("%user_nobility_rank", "0")
                    .Replace("%user_nobility_position", "0")
                    .Replace("%levelup_exp", "0");
            }

            if (role != null)
            {
                if (role is BaseNpc npc)
                {
                    result = result.Replace("%data0", npc.GetData("data0").ToString())
                        .Replace("%data1", npc.GetData("data1").ToString())
                        .Replace("%data2", npc.GetData("data2").ToString())
                        .Replace("%data3", npc.GetData("data3").ToString())
                        .Replace("%npc_ownerid", npc.OwnerIdentity.ToString());
                }
                else if (role is Monster monster)
                {

                }

                result = result.Replace("%map_owner_id", role.Map.OwnerIdentity.ToString());
            }

            if (item != null)
            {
                result = result.Replace("%item_data", item.Identity.ToString())
                    .Replace("%item_name", item.Name)
                    .Replace("%item_type", item.Type.ToString())
                    .Replace("%item_id", item.Identity.ToString());
            }

            result = result.Replace("%map_name", user?.Map?.Name ?? role?.Map?.Name ?? Language.StrNone);
            return result;
        }

        private static string[] SplitParam(string param, int count = 0)
        {
            return count > 0 ? param.Split(new[] {' '}, count, StringSplitOptions.RemoveEmptyEntries) : param.Split(' ');
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