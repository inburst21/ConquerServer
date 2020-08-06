// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Magic Processing.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Core.Mathematics;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.NPCs;
using Comet.Game.World.Maps;
using Comet.Shared;

#endregion

namespace Comet.Game.States.Magics
{
    public partial class MagicData
    {
        private const int _MAX_TARGET_NUM = 25;

        /// <summary>
        ///     This delay is a general one, to avoid people from spamming skills in low ping scenarios.
        /// </summary>
        private readonly TimeOutMS m_tDelay = new TimeOutMS(MAGIC_DELAY);

        private TimeOutMS m_tIntone = new TimeOutMS();
        private TimeOutMS m_tApply = new TimeOutMS();

        private Magic m_pMagic = null;

        private Point m_targetPos = default;
        private uint m_idTarget = 0;

        private bool m_autoAttack = false;
        private MagicState m_state = MagicState.None;

        private int m_autoAttackNum = 0;

        public async Task<(bool Success, ushort X, ushort Y)> CheckConditionAsync(Magic magic, uint idTarget, ushort x,
            ushort y)
        {
            if (!m_tDelay.IsTimeOut(MAGIC_DELAY - magic.Level * MAGIC_DECDELAY_PER_LEVEL) &&
                magic.Sort != MagicSort.Collide)
                return (false, x, y);

            if (!magic.IsReady())
                return (false, x, y);

            if (m_pOwner.Map.IsLineSkillMap() && magic.Sort != MagicSort.Line)
                return (false, x, y);

            if (!((magic.AutoActive & 1) == 1
                  || (magic.AutoActive & 4) == 4) && magic.Type != 6001)
            {
                if (!await Kernel.ChanceCalcAsync(magic.Percent))
                    return (false, x, y);
            }

            // todo handle region

            GameMap map = m_pOwner.Map;
            Character user = m_pOwner as Character;
            if (map.IsTrainingMap() && user != null)
            {
                if (user.Mana < magic.UseMana)
                    return (false, x, y);
                if (user.Energy < magic.UseStamina)
                    return (false, x, y);

                if (magic.UseItem > 0 && user.CheckWeaponSubType(magic.UseItem, magic.UseItemNum))
                    return (false, x, y);
            }

            if (magic.UseXp == BattleSystem.MagicType.Normal)
            {
                IStatus pStatus = m_pOwner.QueryStatus(StatusSet.START_XP);
                if (pStatus == null)
                    return (false, x, y);
            }

            if (magic.WeaponSubtype > 0 && user != null)
            {
                if (!user.CheckWeaponSubType(magic.WeaponSubtype))
                    return (false, x, y);
            }

            if (user != null && user.TransformationMesh != 0)
                return (false, x, y);

            if (m_pOwner.IsWing && magic.Sort == MagicSort.Transform)
                return (false, x, y);

            if (map.IsWingDisable() && magic.Sort == MagicSort.Attachstatus && magic.Status == StatusSet.FLY)
                return (false, x, y);

            Role role = null;
            if (magic.Ground == 0)
            {
                role = map.QueryAroundRole(m_pOwner, idTarget);
                if (role == null)
                    return (false, x, y);

                if (!role.IsAlive
                    && magic.Sort != MagicSort.Attachstatus
                    && magic.Sort != MagicSort.Detachstatus)
                    return (false, x, y);

                if (magic.Sort == MagicSort.Declife)
                {
                    if (role.Life * 100 / role.MaxLife >= 15)
                        return (false, x, y);
                }

                x = role.MapX;
                y = role.MapY;
            }

            if (HitByMagic() != 0 && !HitByWeapon() && m_pOwner.GetDistance(x, y) > magic.Distance + 1)
                return (false, x, y);

            if (m_pOwner.GetDistance(x, y) > m_pOwner.GetAttackRange(0) + magic.Distance + 1)
                return (false, x, y);

            // todo handle dynamic npc

            return (true, x, y);
        }

        public async Task<bool> ProcessMagicAttackAsync(ushort usMagicType, uint idTarget, ushort x, ushort y,
            byte ucAutoActive = 0)
        {
            switch (m_state)
            {
                case MagicState.Intone:
                    await AbortMagic(true);
                    break;
                case MagicState.Delay:
                    return false;
                case MagicState.Launch:
                    return false;
            }

            if (!Magics.TryGetValue(usMagicType, out Magic magic)
                && (ucAutoActive == 0 || (magic.AutoActive & ucAutoActive) != 0))
            {
                await Log.GmLog("cheat", $"invalid magic type: {usMagicType}, user[{m_pOwner.Name}][{m_pOwner.Identity}]");
                return false;
            }

            var result = await CheckConditionAsync(magic, idTarget, x, y);
            if (!result.Success)
            {
                if (magic.Sort == MagicSort.Collide)
                    await ProcessCollideFail(x, y, (int) idTarget);

                await AbortMagic(true);
                return false;
            }

            if (!magic.IsReady())
                return false;

            if (magic.Ground > 0 && magic.Sort != MagicSort.Atkstatus)
                m_idTarget = 0;
            else
                m_idTarget = idTarget;

            m_targetPos = new Point(x, y);

            Character user = m_pOwner as Character;
            GameMap map = m_pOwner.Map;
            if (user != null  && !map.IsTrainingMap())
            {
                if (magic.UseMana > 0)
                    await user.AddAttributesAsync(ClientUpdateType.Mana, magic.UseMana * -1);
                if (magic.UseStamina > 0)
                    await user.AddAttributesAsync(ClientUpdateType.Stamina, magic.UseStamina * -1);
                if (magic.UseItem > 0)
                    await user.SpendEquipItem(magic.UseItem, Math.Max(magic.UseItemNum, 1), true);
            }

            if (magic.UseXp == BattleSystem.MagicType.Normal && user != null)
            {
                IStatus pStatus = m_pOwner.QueryStatus(StatusSet.START_XP);
                if (pStatus == null)
                {
                    await AbortMagic(true);
                    return false;
                }

                await user.DetachStatus(StatusSet.START_XP);
                await user.ClsXpVal();
            }

            if (!IsWeaponMagic(magic.Type))
            {
                await m_pOwner.BroadcastRoomMsgAsync(new MsgInteract
                {
                    Action = MsgInteractType.MagicAttack,
                    TargetIdentity = idTarget,
                    SenderIdentity = m_pOwner.Identity,
                    PosX = m_pOwner.MapX,
                    PosY = m_pOwner.MapY
                }, true);
            }

            m_pMagic = magic; // for auto attack!

            if (magic.UseMana != 0)
            {
                if (!map.IsTrainingMap() && user != null)
                    await user.DecEquipmentDurability(false, (int) HitByMagic(), (ushort)m_pMagic.UseItemNum);

                if (await Kernel.ChanceCalcAsync(7) && user != null)
                    await user.SendGemEffect();
            }

            if (magic.IntoneSpeed <= 0)
            {
                if (!await LaunchAsync(magic))
                {
                    // fail? cancel
                    m_tApply.Clear();
                    ResetDelay();
                }
                else
                {
                    if (m_pOwner.Map.IsTrainingMap() && m_pMagic != null)
                    {
                        SetAutoAttack();
                        m_tDelay.Startup(Math.Max(1500, m_pMagic.DelayMs));
                        m_state = MagicState.Delay;
                        return true;
                    }

                    if (m_tApply == null)
                        m_tApply = new TimeOutMS(0);
                    if (m_pMagic == null)
                        return false;
                    
                    m_tApply.Startup((int)m_pMagic.StepSeconds);
                    m_state = MagicState.Launch;
                }
            }
            else
            {
                m_state = MagicState.Intone;
                m_tIntone.Startup((int) magic.IntoneSpeed);
            }

            return true;
        }

        #region Processing

        private async Task<bool> LaunchAsync(Magic magic)
        {
            bool result = false;
            try
            {
                if (magic == null)
                    return false;

                if (!m_pOwner.IsAlive) 
                    return false;

                magic.Use();

                switch (magic.Sort)
                {
                    case MagicSort.Attack:
                        result = await ProcessAttackAsync(magic);
                        break;
                    case MagicSort.Recruit:
                        result = await ProcessRecruitAsync(magic);
                        break;
                    case MagicSort.Fan:
                        result = await ProcessFanAsync(magic);
                        break;
                    case MagicSort.Bomb:
                        result = await ProcessBombAsync(magic);
                        break;
                    case MagicSort.Attachstatus:
                        result = await ProcessAttachAsync(magic);
                        break;
                    case MagicSort.Detachstatus:
                        result = await ProcessDetachAsync(magic);
                        break;
                    case MagicSort.Dispatchxp:
                        result = await ProcessDispatchXpAsync(magic);
                        break;
                    case MagicSort.Line:
                        result = await ProcessLineAsync(magic);
                        break;
                    case MagicSort.Atkstatus:
                        result = await ProcessAttackStatusAsync(magic);
                        break;
                    case MagicSort.Transform:
                        result = await ProcessTransformAsync(magic);
                        break;
                    case MagicSort.Addmana:
                        result = await ProcessAddManaAsync(magic);
                        break;

                    default:
                        await Log.WriteLog(LogLevel.Warning, $"MagicProcessing::LaunchAsync {magic.Sort} not handled!!!");
                        result = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error ocurred on MagicProcessing::LaunchAsync");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }

            if (m_autoAttack)
                m_autoAttackNum++;

            return result;
        }
        
        private async Task<bool> ProcessAttackAsync(Magic magic)
        {
            if (magic == null || m_pOwner == null || m_idTarget == 0)
                return false;

            Role targetRole = m_pOwner.Map.QueryAroundRole(m_pOwner, m_idTarget);
            if (targetRole == null
                || m_pOwner.GetDistance(targetRole) > magic.Distance
                || !targetRole.IsAlive
                || !targetRole.IsAttackable(m_pOwner))
                return false;

            if (m_pOwner.IsImmunity(targetRole))
                return false;

            if (m_pMagic.FloorAttr > 0)
            {
                int nAttr = targetRole.Map[targetRole.MapX, targetRole.MapY].Elevation;
                if (nAttr != m_pMagic.FloorAttr)
                    return false;
            }

            var byMagic = HitByMagic();
            var result = await m_pOwner.BattleSystem.CalcPower(byMagic, m_pOwner, targetRole);
            int power = result.Damage;

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY
            };
            msg.Append(targetRole.Identity, power, true);
            await m_pOwner.BroadcastRoomMsgAsync(msg, true);

            await CheckCrime(targetRole);

            Character user = m_pOwner as Character;
            int totalExp = 0;
            if (power > 0)
            {
                int lifeLost = (int) Math.Min(targetRole.MaxLife, power);
                await targetRole.BeAttack(byMagic, m_pOwner, power, true);
                totalExp = lifeLost;

                if (user != null && targetRole is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }
            }

            if (totalExp > 0)
            {
                await AwardExpOfLife(targetRole, totalExp);
            }

            if (!targetRole.IsAlive)
            {
                int nBonusExp = (int)(targetRole.MaxLife * 20 / 100);
                await m_pOwner.BattleSystem.OtherMemberAwardExp(targetRole, nBonusExp);
                await m_pOwner.Kill(targetRole, GetDieMode());
            }
            else if (user != null)
                await user.SendWeaponMagic2(targetRole);

            return true;
        }

        private async Task<bool> ProcessRecruitAsync(Magic magic)
        {
            if (magic == null || m_pOwner == null || m_idTarget == 0)
                return false;

            List<Role> setTarget = new List<Role>();
            // todo team
            Role targetRole = m_pOwner.Map.QueryAroundRole(m_pOwner, m_idTarget);
            if (targetRole == null
                || m_pOwner.GetDistance(targetRole) > magic.Distance
                || !targetRole.IsAlive)
                return false;

            setTarget.Add(targetRole);

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY
            };

            foreach (var target in setTarget)
            {
                if (!target.IsAlive)
                    continue;

                int power = magic.Power;
                if (power == Calculations.ADJUST_FULL)
                    power = (int) (target.MaxLife - target.Life);

                msg.Append(target.Identity, power, false);

                if (power > 0)
                {
                    await target.AddAttributesAsync(ClientUpdateType.Hitpoints, power);
                    // todo broadcast team life
                }
            }

            await m_pOwner.BroadcastRoomMsgAsync(msg, true);
            await AwardExpOfLife(targetRole, AWARDEXP_BY_TIMES, true);
            return true;
        }

        private async Task<bool> ProcessFanAsync(Magic magic)
        {
            int nRange = (int)m_pMagic.Distance + 2;
            const int nWidth = DEFAULT_MAGIC_FAN + 30;
            long nExp = 0;

            List<Role> setTarget = new List<Role>();
            var targets = m_pOwner.Map.Query9BlocksByPos(m_pOwner.MapX, m_pOwner.MapY);
            foreach (var target in targets)
            {
                if (target.Identity == m_pOwner.Identity)
                    continue;

                Point posThis = new Point(target.MapX, target.MapY);
                if (!ScreenCalculations.IsInFan(posThis, new Point(m_pOwner.MapX, m_pOwner.MapY), nRange, nWidth,
                    m_targetPos))
                    continue;

                if (target.IsAttackable(m_pOwner)
                    && !m_pOwner.IsImmunity(target))
                    setTarget.Add(target);
            }

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };

            Character user = m_pOwner as Character;
            var byMagic = HitByMagic();
            bool bMagic2Dealt = false;
            foreach (var target in setTarget)
            {
                var result = await m_pOwner.BattleSystem.CalcPower(byMagic, m_pOwner, target);

                if (msg.Count < _MAX_TARGET_NUM)
                    msg.Append(target.Identity, result.Damage, true);

                int lifeLost = (int)Math.Min(target.Life, result.Damage);
                await target.BeAttack(byMagic, m_pOwner, lifeLost, true);

                if (user != null && target is Monster monster)
                {
                    nExp += user.AdjustExperience(monster, lifeLost, false);
                    if (!monster.IsAlive)
                    {
                        int nBonusExp = (int)(monster.MaxLife * 20 / 100d);

                        if (user.Team != null)
                            await user.Team.AwardMemberExp(user.Identity, target, nBonusExp);

                        nExp += user.AdjustExperience(monster, nBonusExp, false);
                    }
                }

                if (user != null && target is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }

                if (!target.IsAlive)
                    await m_pOwner.Kill(target, GetDieMode());
                else if (!bMagic2Dealt && await Kernel.ChanceCalcAsync(5d) && user != null)
                {
                    await user.SendWeaponMagic2(target);
                    bMagic2Dealt = true;
                }
            }

            await m_pOwner.BroadcastRoomMsgAsync(msg, true);

            await AwardExp(nExp, nExp, false, magic);
            return true;
        }

        private async Task<bool> ProcessBombAsync(Magic magic)
        {
            if (magic == null || m_pOwner == null)
                return false;

            List<Role> setTarget = new List<Role>();

            var result = await CollectTargetBomb(0, (int) magic.Range);

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = (ushort) result.Center.X,
                MapY = (ushort) result.Center.Y,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };

            long battleExp = 0;
            long exp = 0;
            Character user = m_pOwner as Character;
            foreach (var target in result.Roles)
            {
                var atkResult = await m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, target);
                int lifeLost = (int) Math.Min(atkResult.Damage, target.Life);
                
                await target.BeAttack(HitByMagic(), m_pOwner, atkResult.Damage, true);

                if (user != null && target is Monster monster)
                {
                    exp += lifeLost;
                    battleExp += user.AdjustExperience(target, lifeLost, false);
                    if (!monster.IsAlive)
                    {
                        int nBonusExp = (int)(monster.MaxLife * 20 / 100d);
                        if (user.Team != null)
                            await user.Team.AwardMemberExp(user.Identity, target, nBonusExp);
                        battleExp += user.AdjustExperience(monster, nBonusExp, false);
                    }
                }

                if (user != null && target is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }

                if (!target.IsAlive)
                    await m_pOwner.Kill(target, GetDieMode());

                if (msg.Count < _MAX_TARGET_NUM)
                    msg.Append(target.Identity, atkResult.Damage, true);
            }

            await m_pOwner.Map.BroadcastRoomMsgAsync(result.Center.X, result.Center.Y, msg);

            await CheckCrime(result.Roles.ToDictionary(x => x.Identity, x => x));
            await AwardExp(0, battleExp, exp, magic);
            return true;
        }

        private async Task<bool> ProcessAttachAsync(Magic magic)
        {
            if (magic == null)
                return false;

            var target = Kernel.RoleManager.GetRole(m_idTarget);
            if (target == null)
                return false;

            /*
             * 64 can only be used on dead players
             */
            if (!target.IsAlive && magic.Target != 64 && !(target is Character))
                return false;

            int power = magic.Power;
            int secs = (int) magic.StepSeconds;
            int times = (int) magic.ActiveTimes;
            int status = (int) magic.Status;
            byte level = (byte) magic.Level;

            if (power < 0)
            {
                await Log.WriteLog(LogLevel.Warning, $"Error magic type invalid power {magic.Type} {magic.Power}");
                return false;
            }

            if (secs <= 0)
                secs = int.MaxValue;

            int damage = 1;
            switch (status)
            {
                case StatusSet.FLY:
                    if (target.Identity != m_pOwner.Identity)
                        return false;
                    if (!target.IsBowman || !target.IsAlive)
                        return false;
                    if (target.Map.IsWingDisable())
                        return false;
                    break;

                case StatusSet.LUCKY_ABSORB:
                    status = StatusSet.LUCKY_DIFFUSE;
                    damage = 0;

                    if (target is Character user)
                    {
                        // todo check booth
                    }
                    break;
            }

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };
            msg.Append(target.Identity, damage, damage != 0);
            await m_pOwner.BroadcastRoomMsgAsync(msg, true);

            await CheckCrime(target);

            await target.AttachStatus(m_pOwner, status, power, secs, times, level);

            if (power >= Calculations.ADJUST_PERCENT)
            {
                var powerTimes = (power - 30000) - 100;
                switch (status)
                {
                    case StatusSet.STAR_OF_ACCURACY:
                        await target.SendAsync(string.Format(Language.StrAccuracyActiveP, secs, powerTimes));
                        break;
                    case StatusSet.DODGE:
                        await target.SendAsync(string.Format(Language.StrDodgeActiveP, secs, powerTimes));
                        break;
                    case StatusSet.STIG:
                        await target.SendAsync(string.Format(Language.StrStigmaActiveP, secs, powerTimes));
                        break;
                    case StatusSet.SHIELD:
                        await target.SendAsync(string.Format(Language.StrShieldActiveP, secs, powerTimes));
                        break;
                }
            }
            else
            {
                switch (status)
                {
                    case StatusSet.STAR_OF_ACCURACY:
                        await target.SendAsync(string.Format(Language.StrAccuracyActiveT, secs, power));
                        break;
                    case StatusSet.DODGE:
                        await target.SendAsync(string.Format(Language.StrDodgeActiveT, secs, power));
                        break;
                    case StatusSet.STIG:
                        await target.SendAsync(string.Format(Language.StrStigmaActiveT, secs, power));
                        break;
                    case StatusSet.SHIELD:
                        await target.SendAsync(string.Format(Language.StrShieldActiveT, secs, power));
                        break;
                }
            }

            if (m_pOwner is Character player)
                await AwardExp(0, 0, AWARDEXP_BY_TIMES, magic);
            return true;
        }

        private async Task<bool> ProcessDetachAsync(Magic magic)
        {
            if (magic == null) return false;

            Role target = Kernel.RoleManager.GetRole(m_idTarget);
            if (target == null)
                return false;

            int power = magic.Power;
            int secs = (int)magic.StepSeconds;
            int times = (int)magic.ActiveTimes;
            int status = (int)magic.Status;
            byte level = (byte)magic.Level;

            if (!target.IsAlive && target.IsPlayer())
            {
                if (status != 0)
                    return false;

                if (target.Map.IsPkField())
                    return false;
            }

            if (status == 0 && target is Character user)
            {
                await user.Reborn(false, true);
                await user.Map.SendMapInfoAsync(user);
            }
            else
            {
                // todo handle any other skill
            }

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = (ushort) magic.Type,
                MagicLevel = magic.Level
            };
            msg.Append(target.Identity, power, true);
            await target.BroadcastRoomMsgAsync(msg, true);

            if (power > 0)
            {
                int lifeLost = (int) Math.Min(target.Life, Math.Max(0, Calculations.AdjustData(target.Life, power)));
                await target.BeAttack(HitByMagic(), m_pOwner, lifeLost, true);
                await target.AddAttributesAsync(ClientUpdateType.Hitpoints, lifeLost * -1);
            }

            return true;
        }

        private async Task<bool> ProcessDispatchXpAsync(Magic magic)
        {
            if (magic == null)
                return false;

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = (ushort)magic.Type,
                MagicLevel = magic.Level
            };
            if (m_pOwner is Character user && user.Team != null)
            {
                foreach (Character member in user.Team.Members.Where(x => x.Identity != user.Identity && x.IsAlive))
                {
                    if (m_pOwner.GetDistance(member) > Screen.VIEW_SIZE * 2)
                        continue;

                    msg.Append(member.Identity, DISPATCHXP_NUMBER, true);
                    await member.SetXp(DISPATCHXP_NUMBER);
                    await member.BurstXp();
                    await member.SendAsync(string.Format(Language.StrDispatchXp, user.Name));
                }
            }

            await m_pOwner.BroadcastRoomMsgAsync(msg, true);
            await AwardExp(0, 0, AWARDEXP_BY_TIMES, magic);
            return true;
        }

        private async Task<bool> ProcessLineAsync(Magic magic)
        {
            if (magic == null || m_pOwner == null)
                return false;

            var allTargets = m_pOwner.Map.Query9BlocksByPos(m_pOwner.MapX, m_pOwner.MapY);

            List<Point> setPoint = new List<Point>();
            var pos = new Point(m_pOwner.MapX, m_pOwner.MapY);
            ScreenCalculations.DDALine(pos.X, pos.Y, m_targetPos.X, m_targetPos.Y, (int)m_pMagic.Range, ref setPoint);

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = (ushort)m_targetPos.X,
                MapY = (ushort)m_targetPos.Y,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };
            long exp = 0;
            long battleExp = 0;
            Character user = m_pOwner as Character;

            Tile userTile = m_pOwner.Map[m_pOwner.MapX, m_pOwner.MapY];
            foreach (var point in setPoint)
            {
                if (msg.Count >= _MAX_TARGET_NUM)
                {
                    await m_pOwner.BroadcastRoomMsgAsync(msg, true);
                    msg.ClearTargets();
                }

                Tile targetTile = m_pOwner.Map[point.X, point.Y];
                if (userTile.Elevation - targetTile.Elevation > 26)
                    continue;

                Role target = allTargets.FirstOrDefault(x => x.MapX == point.X && x.MapY == point.Y);
                if (target == null || target.Identity == m_pOwner.Identity)
                    continue;

                if (m_pOwner.IsImmunity(target)
                    || !target.IsAttackable(m_pOwner))
                    continue;

                var result = await m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, target);
                int lifeLost = (int)Math.Min(result.Damage, target.Life);

                await target.BeAttack(HitByMagic(), m_pOwner, result.Damage, true);

                if (user != null && target is Monster monster)
                {
                    exp += lifeLost;
                    battleExp += user.AdjustExperience(target, lifeLost, false);
                    if (!monster.IsAlive)
                    {
                        int nBonusExp = (int)(monster.MaxLife * 20 / 100d);
                        if (user.Team != null)
                            await user.Team.AwardMemberExp(user.Identity, target, nBonusExp);
                        battleExp += user.AdjustExperience(monster, nBonusExp, false);
                    }
                }

                if (user != null && target is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }

                if (!target.IsAlive)
                    await m_pOwner.Kill(target, GetDieMode());

                msg.Append(target.Identity, result.Damage, true);
            }

            if (msg.Count > 0)
                await m_pOwner.BroadcastRoomMsgAsync(msg, true);

            await CheckCrime(allTargets.ToDictionary(x => x.Identity, x => x));
            await AwardExp(0, battleExp, exp, magic);
            return true;
        }

        private async Task<bool> ProcessAttackStatusAsync(Magic magic)
        {
            if (magic == null)
                return false;

            Role target = Kernel.RoleManager.GetRole(m_idTarget);
            if (target == null)
                return false;

            if (target.MapIdentity != m_pOwner.MapIdentity
                || m_pOwner.GetDistance(target) > magic.Distance + target.SizeAddition)
                return false;

            if (!target.IsAttackable(m_pOwner) || m_pOwner.IsImmunity(target))
                return false;

            int power = 0;
            InteractionEffect effect = InteractionEffect.None;

            if (HitByWeapon())
            {
                switch (magic.Status)
                {
                    case 0:
                        break;
                    default:
                        var result = await m_pOwner.BattleSystem.CalcPower(HitByMagic(), m_pOwner, target);
                        power = result.Damage;
                        effect = result.effect;
                        break;
                }
            }

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = (ushort) magic.Type,
                MagicLevel = magic.Level
            };
            msg.Append(target.Identity, power, true);
            await m_pOwner.BroadcastRoomMsgAsync(msg, true);

            long battleExp = 0;
            int exp = 0;
            Character user = m_pOwner as Character;
            if (power > 0)
            {
                int lifeLost = (int) Math.Max(0, Math.Min(target.Life, power));

                await target.BeAttack(HitByMagic(), m_pOwner, power, true);

                if (user != null && target is Monster monster)
                {
                    exp += lifeLost;
                    battleExp += user.AdjustExperience(target, lifeLost, false);
                    if (!monster.IsAlive)
                    {
                        int nBonusExp = (int)(monster.MaxLife * 20 / 100d);

                        if (user.Team != null)
                            await user.Team.AwardMemberExp(user.Identity, target, nBonusExp);

                        battleExp += user.AdjustExperience(monster, nBonusExp, false);
                    }
                }

                if (user != null && target is DynamicNpc dynaNpc && dynaNpc.IsAwardScore())
                {
                    dynaNpc.AddSynWarScore(user.Syndicate, lifeLost);
                }
            }
            
            await AwardExp(0, battleExp, exp, magic);

            if (!target.IsAlive)
                await target.BeKill(m_pOwner);

            return true;
        }

        private async Task<bool> ProcessTransformAsync(Magic magic)
        {
            if (magic == null || m_pOwner == null || !(m_pOwner is Character user))
                return false;

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };
            await m_pOwner.BroadcastRoomMsgAsync(msg, true);
            await user.Transform((uint) magic.Power, (int) magic.StepSeconds, true);
            await AwardExp(0, 0, AWARDEXP_BY_TIMES, magic);
            return true;
        }

        private async Task<bool> ProcessAddManaAsync(Magic magic)
        {
            if (magic == null)
                return false;

            Role target = null;
            if (magic.Target == 2) // self
            {
                target = m_pOwner;
            }
            else if (magic.Target == 1) // target
            {
                target = Kernel.RoleManager.GetRole(m_idTarget);
            }
            else // unhandled
            {
                await Log.WriteLog(LogLevel.Warning, $"Add mana unhandled target {magic.Target}");
                return false;
            }

            if (target.Identity != m_pOwner.Identity
                && (target.MapIdentity != m_pOwner.MapIdentity || m_pOwner.GetDistance(target) > magic.Distance))
                return false;

            int addMana = (int) Math.Max(0, Math.Min(target.MaxMana - target.Mana, magic.Power));

            MsgMagicEffect msg = new MsgMagicEffect
            {
                AttackerIdentity = m_pOwner.Identity,
                MapX = m_pOwner.MapX,
                MapY = m_pOwner.MapY,
                MagicIdentity = magic.Type,
                MagicLevel = magic.Level
            };
            msg.Append(target.Identity, addMana, true);
            await target.BroadcastRoomMsgAsync(msg, true);

            await target.AddAttributesAsync(ClientUpdateType.Mana, addMana);

            await AwardExp(0, 0, Math.Max(addMana, AWARDEXP_BY_TIMES), magic);
            return true;
        }

        private async Task<bool> ProcessCollideFail(ushort x, ushort y, int nDir)
        {
            ushort nTargetX = (ushort)(x + GameMap.WalkXCoords[nDir]);
            ushort nTargetY = (ushort)(y + GameMap.WalkYCoords[nDir]);

            if (!m_pOwner.Map.IsStandEnable(nTargetX, nTargetY))
            {
                if (m_pOwner is Character owner)
                {
                    await owner.SendAsync(Language.StrInvalidMsg);
                    await Kernel.RoleManager.KickoutAsync(owner.Identity, "INVALID COORDINATES ProcessCollideFail");
                }
                return false;
            }

            MsgInteract pMsg = new MsgInteract
            {
                SenderIdentity = m_pOwner.Identity,
                TargetIdentity = 0,
                PosX = nTargetX,
                PosY = nTargetY,
                Action = MsgInteractType.Dash,
                Data = (nDir * 0x01000000)
            };

            await m_pOwner.BroadcastRoomMsgAsync(pMsg, true);
            if (m_pOwner is Character character)
            {
                await character.ProcessOnMove();
                await character.MoveTowardAsync(nDir, (int) RoleMoveMode.MOVEMODE_COLLIDE);
            }
            return true;
        }

        #endregion

        #region Collect Targets

        private Task<(List<Role> Roles, Point Center)> CollectTargetBomb(int nLockType, int nRange)
        {
            List<Role> targets = new List<Role>();

            Point center = new Point(m_targetPos.X, m_targetPos.Y);
            if (m_pMagic.Ground == 1)
            {
                center.X = m_pOwner.MapX;
                center.Y = m_pOwner.MapY;
            }
            else if (m_idTarget != 0)
            {
                Role target = m_pOwner.Map.QueryAroundRole(m_pOwner, m_idTarget);
                if (target != null)
                {
                    center.X = target.MapX;
                    center.Y = target.MapY;
                }
            }

            var setRoles = m_pOwner.Map.Query9BlocksByPos(center.X, center.Y);

            foreach (var target in setRoles)
            {
                if (target.Identity == m_pOwner.Identity)
                    continue;

                if (target.GetDistance(center.X, center.Y) > nRange)
                    continue;

                if (m_pOwner.IsImmunity(target) || !target.IsAttackable(m_pOwner))
                    continue;

                targets.Add(target);
            }

            return Task.FromResult((targets, center));
        }

        #endregion

        #region Experience

        public async Task<bool> AwardExpOfLife(Role pTarget, int nLifeLost, bool bMagicRecruit = false)
        {
            Character pOwner = m_pOwner as Character;
            if ((pTarget.IsMonster()) && pOwner != null) // todo check if dynamic npc
            {
                int exp = nLifeLost;
                long battleExp = pOwner.AdjustExperience(pTarget, nLifeLost, false);

                if (!pTarget.IsAlive && !bMagicRecruit)
                {
                    int nBonusExp = (int)(pTarget.MaxLife * (5 / 100));
                    battleExp += nBonusExp;
                    if (!pOwner.Map.IsTrainingMap() && nBonusExp > 0)
                        await pOwner.SendAsync(string.Format(Language.StrKillingExperience, nBonusExp));
                }

                await AwardExp(0, (int)battleExp, exp);
            }
            return true;
        }

        public async Task<bool> AwardExp(int nType, long nBattleExp, long nExp, Magic pMagic = null)
        {
            if (pMagic == null)
                return await AwardExp(nBattleExp, nExp, true, m_pMagic);
            return await AwardExp(nBattleExp, nExp, true, pMagic);
        }

        public async Task<bool> AwardExp(long nBattleExp, long nExp, bool bIgnoreFlag, Magic pMagic = null)
        {
            if (nBattleExp <= 0 && nExp == 0) return false;

            if (pMagic == null)
                pMagic = m_pMagic;

            if (m_pOwner.Map.IsTrainingMap())
            {
                if (nBattleExp > 0)
                {
                    if (m_pOwner.IsBowman)
                        nBattleExp /= 2;
                    nBattleExp = Calculations.CutTrail(1, Calculations.MulDiv(nBattleExp, 10, 100));
                }
            }

            if (nBattleExp > 0 && m_pOwner is Character user)
                await user.AwardBattleExp(nBattleExp, true);

            if (pMagic == null)
                return false;

            if (!CheckAwardExpEnable((ushort)((m_pOwner as Character)?.Profession ?? 0)))
                return false;

            if (m_pOwner.Map.IsTrainingMap() && m_autoAttackNum % 10 != 0)
                return true;

            if (pMagic.NeedExp > 0
                && (pMagic.AutoActive & 16) == 0
                || bIgnoreFlag)
            {
                if (m_pOwner is Character owner)
                    nExp = (int)(nExp * ((1 + (owner.MoonGemBonus / 100f))));

                pMagic.Experience += (uint)nExp;

                if ((pMagic.AutoActive & 8) == 0)
                    await pMagic.SendAsync();

                await UpLevelMagic(true, pMagic);
                await pMagic.SaveAsync();
                return true;
            }

            if (pMagic.NeedExp == 0
                && pMagic.Target == 4)
            {
                if (m_pOwner is Character owner)
                    nExp = (int)(nExp * ((1 + (owner.MoonGemBonus / 100f))));

                pMagic.Experience += (uint)nExp;

                if ((pMagic.AutoActive & 8) == 0)
                    await pMagic.SendAsync();
                await UpLevelMagic(true, pMagic);

                await pMagic.SaveAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpLevelMagic(bool synchro, Magic pMagic)
        {
            if (pMagic == null)
                return false;
            
            int nNeedExp = pMagic.NeedExp;

            if (!(nNeedExp > 0
                  && (pMagic.Experience >= nNeedExp
                      || pMagic.OldLevel > 0
                      && pMagic.Level >= pMagic.OldLevel / 2
                      && pMagic.Level < pMagic.OldLevel)))
                return false;

            ushort nNewLevel = (ushort)(pMagic.Level + 1);
            pMagic.Experience = 0;
            pMagic.Level = nNewLevel;
            await pMagic.SendAsync();
            return true;
        }

        public bool CheckAwardExpEnable(ushort dwProf)
        {
            if (m_pMagic == null)
                return false;
            return m_pOwner.Level >= m_pMagic.NeedLevel
                   && m_pMagic.NeedExp > 0
                   && m_pOwner.MapIdentity != 1005
                /*&& CheckProfession(dwProf, m_pMagic.NeedProf)*/;
        }

        #endregion

        #region OnTimer

        public async Task OnTimerAsync()
        {
            if (m_pMagic == null)
            {
                m_state = MagicState.None;
                return;
            }

            switch (m_state)
            {
                case MagicState.Intone: // intone
                    {
                        if (m_tIntone != null && !m_tIntone.IsTimeOut())
                            return;

                        if (m_tIntone != null && m_tIntone.IsTimeOut() && !await LaunchAsync(m_pMagic))
                        {
                            m_tApply.Clear();
                            ResetDelay();
                        }

                        m_state = MagicState.Launch;

                        if (m_tApply == null)
                            m_tApply = new TimeOutMS((int)m_pMagic.StepSeconds);

                        m_tApply.Startup((int)m_pMagic.StepSeconds);
                        break;
                    }
                case MagicState.Launch: // launch{
                    {
                        if (!m_tApply.IsActive() || m_tApply.TimeOver())
                        {
                            if (m_pMagic.Sort == MagicSort.Attack && m_idTarget != 0)
                            {
                                var pTarget = m_pOwner.Map.QueryAroundRole(m_pOwner, m_idTarget);
                                if (pTarget != null && !pTarget.IsAlive && pTarget.IsAttackable(m_pOwner))
                                    await m_pOwner.Kill(pTarget, (uint)GetDieMode());
                            }
                            ResetDelay();
                            m_state = MagicState.None;
                            await AbortMagic(false);
                        }
                        break;
                    }
                case MagicState.Delay: // delay
                    {
                        if (m_pOwner.Map.IsTrainingMap()
                            && m_tDelay.IsActive()
                            && m_pMagic.Sort != MagicSort.Atkstatus)
                        {
                            if (m_tDelay.IsTimeOut())
                            {
                                m_state = MagicState.None;
                                if (!await m_pOwner.ProcessMagicAttack(m_pMagic.Type, m_idTarget, (ushort)m_targetPos.X,
                                        (ushort)m_targetPos.Y,
                                        0))
                                    m_state = MagicState.Delay;
                            }
                            return;
                        }

                        if (!m_tDelay.IsActive())
                        {
                            m_state = MagicState.None;
                            await AbortMagic(true);
                            return;
                        }

                        if (m_autoAttack && m_tDelay.ToNextTime())
                        {
                            if ((m_tDelay.IsActive() && !m_tDelay.TimeOver()))
                                return;

                            m_state = MagicState.None;
                            await m_pOwner.ProcessMagicAttack(m_pMagic.Type, m_idTarget, (ushort)m_targetPos.X, (ushort)m_targetPos.Y,
                                0);

                            if (m_idTarget != 0 && m_pOwner.Map.QueryAroundRole(m_pOwner, m_idTarget)?.IsPlayer() == true)
                                await AbortMagic(false);
                        }

                        if (m_tDelay.IsActive() && m_tDelay.TimeOver())
                        {
                            m_state = MagicState.None;
                            await AbortMagic(false);
                        }
                        break;
                    }
            }
        }

        #endregion

        private void ResetDelay()
        {
            if (m_pMagic == null) return;
            m_state = MagicState.Delay;
            m_tDelay.Update();
            m_pMagic.SetDelay();
        }

        private void SetAutoAttack()
        {
            m_autoAttack = true;
        }

        private void BreakAutoAttack()
        {
            m_autoAttack = false;
        }

        public async Task<bool> AbortMagic(bool bSynchro)
        {
            if (m_state == MagicState.Launch)
            {
                //m_tApply.Clear();
                return false;
            }

            BreakAutoAttack();
            m_pMagic = null;

            if (m_state == MagicState.Delay)
            {
                //m_tDelay.Clear();
                return false;
            }

            if (m_state == MagicState.Intone)
            {
                m_tIntone.Clear();
            }

            m_state = MagicState.None;

            if (bSynchro && m_pOwner is Character)
            {
                await m_pOwner.SendAsync(new MsgAction
                {
                    Identity = m_pOwner.Identity,
                    Action = MsgAction.ActionType.AbortMagic
                });
            }

            return true;
        }

        public enum MagicState
        {
            None = 0,
            Intone = 1,
            Launch = 2,
            Delay = 3
        }
    }
}