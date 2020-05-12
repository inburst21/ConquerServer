// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Battle System.cs
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
using System.Threading.Tasks;
using Comet.Core.Mathematics;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Microsoft.VisualStudio.Threading;

namespace Comet.Game.States
{
    public sealed class BattleSystem
    {
        private Role m_owner = null;

        private uint m_idTarget = 0;
        private bool m_bAutoAttack = false;

        public BattleSystem(Role role)
        {
            m_owner = role;
        }

        public async Task<bool> ProcessAttackAsync()
        {
            if (m_owner == null || m_idTarget == 0 || !IsBattleMaintain())
            {
                ResetBattle();
                return false;
            }

            Role target = Kernel.RoleManager.GetRole(m_idTarget);
            if (target == null)
            {
                ResetBattle();
                return false;
            }

            if (m_owner.IsImmunity(target))
            {
                ResetBattle();
                return false;
            }

            if (await IsTargetDodged(m_owner, target))
            {
                await m_owner.SendDamageMsgAsync(m_idTarget, 0);
                return false;
            }

            InteractionEffect effect = InteractionEffect.None;
            int damage = CalcPower(m_owner, target, ref effect);

            await m_owner.SendDamageMsgAsync(target.Identity, damage);
            m_owner.ProcessOnAttack();

            if (damage == 0)
                return true;

            if (!target.IsAlive)
            {
                uint dieWay = 1;
                if (damage > target.MaxLife / 3)
                    dieWay = 2;
            }

            return true;
        }

        public int CalcPower(Role attacker, Role target, ref InteractionEffect effect)
        {
            return 1;
        }

        public int CalcAttackPower(Role attacker, Role target, ref InteractionEffect effect)
        {
            return 1;
        }

        public int CalcMagicAttackPower(Role attacker, Role target, ref InteractionEffect effect)
        {
            return 1;
        }

        public async Task<bool> IsTargetDodged(Role attacker, Role target)
        {
            if (attacker == null || target == null || attacker.Identity == target.Identity)
                return false;

            int hitRate = attacker.Accuracy;
            if (attacker.QueryStatus(StatusSet.STAR_OF_ACCURACY) != null)
                hitRate = Calculations.AdjustData(hitRate, attacker.QueryStatus(StatusSet.STAR_OF_ACCURACY).Power);

            int dodge = target.Dodge;
            if (target.QueryStatus(StatusSet.DODGE) != null)
                dodge = Calculations.AdjustData(dodge, attacker.QueryStatus(StatusSet.DODGE).Power);

            if (target is Character)
                dodge += 40;

            hitRate = Math.Min(99, Math.Max(40, hitRate - dodge));

#if DEBUG
            if (attacker is Character atkUser && atkUser.IsPm())
                await atkUser.SendAsync($"Attacker({attacker.Name}), Target({target.Name}), Hit Rate: {hitRate}, Target Dodge: {dodge}");

            if (target is Character targetUser && targetUser.IsPm())
                await targetUser.SendAsync($"Attacker({attacker.Name}), Target({target.Name}), Hit Rate: {hitRate}, Target Dodge: {dodge}");
#endif

            return await Kernel.ChanceCalcAsync(hitRate);
        }

        public bool IsBattleMaintain()
        {
            if (m_idTarget == 0)
                return false;

            Role target = Kernel.RoleManager.GetRole(m_idTarget);
            if (target == null) return false;

            if (!target.IsAlive) return false;

            if (target.MapIdentity != m_owner.MapIdentity)
                return false;

            if (m_owner is Character && target is Character && m_owner.Map.IsPkDisable())
                return false;

            if (m_owner.IsWing && !target.IsWing && !m_owner.IsBowman)
                return false;

            if (m_owner.GetDistance(target) > m_owner.GetAttackRange(target.SizeAddition))
                return false;

            if (!m_owner.IsAttackable(target))
                return false;
            return true;
        }

        public void ResetBattle()
        {
            m_idTarget = 0;
            m_bAutoAttack = false;
        }

        public int AdjustDrop(int nDrop, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            if (nAtkLev - nDefLev > 0)
            {
                int nDeltaLev = nAtkLev - nDefLev;
                if (1 < nAtkLev && nAtkLev <= 19)
                {
                    if (nDeltaLev < 3)
                    {
                    }
                    else if (3 <= nDeltaLev && nDeltaLev < 6)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (19 < nAtkLev && nAtkLev <= 49)
                {
                    if (nDeltaLev < 5)
                    {
                    }
                    else if (5 <= nDeltaLev && nDeltaLev < 10)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (49 < nAtkLev && nAtkLev <= 85)
                {
                    if (nDeltaLev < 4)
                    {
                    }
                    else if (4 <= nDeltaLev && nDeltaLev < 8)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (85 < nAtkLev && nAtkLev <= 112)
                {
                    if (nDeltaLev < 3)
                    {
                    }
                    else if (3 <= nDeltaLev && nDeltaLev < 6)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (112 < nAtkLev && nAtkLev <= 120)
                {
                    if (nDeltaLev < 2)
                    {
                    }
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (120 < nAtkLev && nAtkLev <= 130)
                {
                    if (nDeltaLev < 2)
                    {
                    }
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
                else if (130 < nAtkLev && nAtkLev <= 140)
                {
                    if (nDeltaLev < 2)
                    {
                    }
                    else if (2 <= nDeltaLev && nDeltaLev < 4)
                        nDrop = nDrop / 5;
                    else
                        nDrop = nDrop / 10;
                }
            }

            return Calculations.CutTrail(0, nDrop);
        }

        public int GetNameType(int nAtkLev, int nDefLev)
        {
            int nDeltaLev = nAtkLev - nDefLev;

            if (nDeltaLev >= 3)
                return NAME_GREEN;
            if (nDeltaLev >= 0)
                return NAME_WHITE;
            if (nDeltaLev >= -5)
                return NAME_RED;
            return NAME_BLACK;
        }

        public int CalcDamageUser2Monster(int nAtk, int nDef, int nAtkLev, int nDefLev)
        {
            int nDamage = nAtk - nDef;

            if (GetNameType(nAtkLev, nDefLev) != NAME_GREEN)
                return Calculations.CutTrail(0, nDamage);

            int nDeltaLev = nAtkLev - nDefLev;
            if (nDeltaLev >= 3
                && nDeltaLev <= 5)
                nAtk = (int)(nAtk * 1.5);
            else if (nDeltaLev > 5
                     && nDeltaLev <= 10)
                nAtk *= 2;
            else if (nDeltaLev > 10
                     && nDeltaLev <= 20)
                nAtk = (int)(nAtk * 2.5);
            else if (nDeltaLev > 20)
                nAtk *= 3;

            return Calculations.CutTrail(0, nAtk - nDef);
        }

        public int CalcDamageMonster2User(int nAtk, int nDef, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            int nDamage = nAtk; //(int) (nAtk - nDef*0.6);

            int nNameType = GetNameType(nAtkLev, nDefLev);

            if (nNameType == NAME_RED)
                nDamage = (int)(nAtk * 1.5f - nDef);
            else if (nNameType == NAME_BLACK)
            {
                int nDeltaLev = nDefLev - nAtkLev;
                if (nDeltaLev >= -10 && nDeltaLev <= -5)
                    nAtk *= 2;
                else if (nDeltaLev >= -20 && nDeltaLev < -10)
                    nAtk = (int)(nAtk * 3.5f);
                else if (nDeltaLev < -20)
                    nAtk *= 5;
                //nDamage = nAtk - nDef;
            }

            return Calculations.CutTrail(0, nDamage);
        }

        public int AdjustMinDamageUser2Monster(int nDamage, Role pAtker, Role pTarget)
        {
            int nMinDamage = 1;
            nMinDamage += pAtker.Level / 10;

            if (!(pAtker is Character))
                return Calculations.CutTrail(nMinDamage, nDamage);

            var pUser = (Character) pAtker;
            Item pItem = pUser.UserPackage[Item.ItemPosition.RightHand];
            if (pItem != null)
                nMinDamage += pItem.GetQuality();

            return Calculations.CutTrail(nMinDamage, nDamage);
        }

        public int AdjustMinDamageMonster2User(int nDamage, Role pAtker, Role pTarget)
        {
            int nMinDamage = 7;

            if (nDamage >= nMinDamage
                || pTarget.Level <= 15)
                return nDamage;

            if (!(pTarget is Character pUser))
                return Calculations.CutTrail(nMinDamage, nDamage);

            for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin; pos <= Item.ItemPosition.EquipmentEnd; pos++)
            {
                Item item = pUser.UserPackage[pos];
                if (item == null)
                    continue;
                switch (item.Position)
                {
                    case Item.ItemPosition.Necklace:
                    case Item.ItemPosition.Headwear:
                    case Item.ItemPosition.Armor:
                        nMinDamage -= item.GetQuality() / 5;
                        break;
                }
            }

            nMinDamage = Calculations.CutTrail(1, nMinDamage);
            return Calculations.CutTrail(nMinDamage, nDamage);
        }

        public long AdjustExp(long nDamage, int nAtkLev, int nDefLev)
        {
            if (nAtkLev > 120)
                nAtkLev = 120;

            long nExp = nDamage;

            int nNameType = NAME_WHITE;
            int nDeltaLev = nAtkLev - nDefLev;
            if (nNameType == NAME_GREEN)
            {
                if (nDeltaLev >= 3 && nDeltaLev <= 5)
                    nExp = nExp * 70 / 100;
                else if (nDeltaLev > 5
                         && nDeltaLev <= 10)
                    nExp = nExp * 20 / 100;
                else if (nDeltaLev > 10
                         && nDeltaLev <= 20)
                    nExp = nExp * 10 / 100;
                else if (nDeltaLev > 20)
                    nExp = nExp * 5 / 100;
            }
            else if (nNameType == NAME_RED)
            {
                nExp = (int)(nExp * 1.3f);
            }
            else if (nNameType == NAME_BLACK)
            {
                if (nDeltaLev >= -10
                    && nDeltaLev < -5)
                    nExp = (int)(nExp * 1.5f);
                else if (nDeltaLev >= -20
                         && nDeltaLev < -10)
                    nExp = (int)(nExp * 1.8f);
                else if (nDeltaLev < -20)
                    nExp = (int)(nExp * 2.3f);
            }

            return Calculations.CutTrail(0, nExp);
        }

        public const int NAME_GREEN = 0,
            NAME_WHITE = 1,
            NAME_RED = 2,
            NAME_BLACK = 3;
    }
}