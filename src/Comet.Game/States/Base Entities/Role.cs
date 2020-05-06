﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Role.cs
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
using Comet.Core.Mathematics;
using Comet.Game.Packets;
using Comet.Game.World.Maps;
using Comet.Network.Packets;
using Comet.Shared;

namespace Comet.Game.States.Base_Entities
{
    public abstract class Role
    {
        protected uint m_idMap;

        protected ushort m_posX,
            m_posY;

        protected uint m_maxLife = 0,
            m_maxMana = 0;

        #region Identity

        /// <summary>
        ///     The identity of the role in the world. This will be unique for ANY role in the world.
        /// </summary>
        public virtual uint Identity { get; protected set; }

        /// <summary>
        ///     The name of the role. May be empty or null for NPCs and Dynamic NPCs.
        /// </summary>
        public virtual string Name { get; set; }

        #endregion

        #region Appearence

        public virtual uint Mesh { get; set; }

        #endregion

        #region Level

        public virtual byte Level { get; set; }

        #endregion

        #region Life and Mana

        public virtual bool IsAlive => Life > 0;
        public virtual uint Life { get; set; }
        public virtual uint MaxLife => m_maxLife;
        public virtual uint Mana { get; set; }
        public virtual uint MaxMana => m_maxMana;
        public virtual bool IsGhost() => false;

        #endregion

        #region Map and Position

        public virtual GameMap Map { get; protected set; }

        /// <summary>
        ///     The current map identity for the role.
        /// </summary>
        public virtual uint MapIdentity
        {
            get => m_idMap;
            set => m_idMap = value;
        }

        /// <summary>
        ///     Current X position of the user in the map.
        /// </summary>
        public virtual ushort MapX
        {
            get => m_posX;
            set => m_posX = value;
        }

        /// <summary>
        ///     Current Y position of the user in the map.
        /// </summary>
        public virtual ushort MapY
        {
            get => m_posY;
            set => m_posY = value;
        }

        public virtual int GetDistance(Role role)
        {
            return GetDistance(role.MapX, role.MapY);
        }

        public virtual int GetDistance(int x, int y)
        {
            return ScreenCalculations.GetDistance(MapX, MapY, x, y);
        }

        /// <summary>
        /// </summary>
        public virtual void EnterMap()
        {
        }

        /// <summary>
        /// </summary>
        public virtual void LeaveMap()
        {
        }

        #endregion

        #region Movement

        public async Task JumpPosAsync(int x, int y)
        {
            if (x == MapX && y == MapY)
                return;

            if (Map == null || !Map.IsValidPoint(x, y))
                return;

            Character user = null;
            if (IsPlayer())
            {
                user = (Character)this;
                // we're trusting this
                if (GetDistance(x, y) > Screen.VIEW_SIZE || !Map.IsStandEnable(x, y))
                {
                    await user.SendAsync(Language.StrInvalidCoordinate, MsgTalk.TalkChannel.System, Color.Red);
                    await user.KickbackAsync();
                    return;
                }
            }

            Map.EnterBlock(this, x, y, MapX, MapY);

            Direction = (FacingDirection)ScreenCalculations.GetDirectionSector(MapX, MapY, x, y);

            m_posX = (ushort)x;
            m_posY = (ushort)y;

            ProcessAfterMove();
        }

        public async Task MoveTowardAsync(int direction, int mode)
        {
            direction %= 8;
            ushort newX = (ushort)(MapX + GameMap.WalkXCoords[direction]);
            ushort newY = (ushort)(MapY + GameMap.WalkYCoords[direction]);

            bool isRunning = mode >= (int)RoleMoveMode.MOVEMODE_RUN_DIR0 &&
                             mode <= (int)RoleMoveMode.MOVEMODE_RUN_DIR7;
            if (isRunning && IsAlive)
            {
                newX += (ushort)GameMap.WalkXCoords[direction];
                newY += (ushort)GameMap.WalkYCoords[direction];
            }

            Character user = this as Character;
            if (!IsAlive && user != null && !user.IsGhost())
            {
                await user.SendAsync(Language.StrDead, MsgTalk.TalkChannel.System, Color.Red);
                return;
            }

            if (!Map.IsStandEnable(newX, newY) && user != null)
            {
                await user.SendAsync(Language.StrInvalidCoordinate, MsgTalk.TalkChannel.System, Color.Red);
                return;
            }

            Map.EnterBlock(this, newX, newY, MapX, MapY);

            Direction = (FacingDirection)direction;

            m_posX = newX;
            m_posY = newY;

            ProcessAfterMove();
        }

        public virtual void ProcessOnMove()
        {
        }

        public virtual void ProcessAfterMove()
        {
            Action = EntityAction.Stand;
        }

        public virtual void ProcessOnAttack()
        {
        }

        #endregion

        #region Action and Direction

        public virtual FacingDirection Direction { get; protected set; }

        public virtual EntityAction Action { get; protected set; }

        public async Task SetDirectionAsync(FacingDirection direction, bool sync = true)
        {
            Direction = direction;
            if (sync)
            {
                await BroadcastRoomMsgAsync(new MsgAction
                {
                    Identity = Identity,
                    Action = MsgAction.ActionType.CharacterDirection,
                    Direction = (ushort)direction,
                    ArgumentX = MapX,
                    ArgumentY = MapY
                }, sync);
            }
        }

        public async Task SetActionAsync(EntityAction action, bool sync = true)
        {
            Action = action;
            if (sync)
            {
                await BroadcastRoomMsgAsync(new MsgAction
                {
                    Identity = Identity,
                    Action = MsgAction.ActionType.CharacterEmote,
                    Command = (ushort)action,
                    ArgumentX = MapX,
                    ArgumentY = MapY,
                    Direction = (ushort)Direction
                }, sync);
            }
        }

        #endregion

        #region Role Type

        public bool IsPlayer()
        {
            return Identity >= PLAYER_ID_FIRST && Identity < PLAYER_ID_LAST;
        }

        public bool IsMonster()
        {
            return Identity >= MONSTERID_FIRST && Identity < MONSTERID_LAST;
        }

        public bool IsNpc()
        {
            return Identity >= SYSNPCID_FIRST && Identity < SYSNPCID_LAST;
        }

        public bool IsDynaNpc()
        {
            return Identity >= DYNANPCID_FIRST && Identity < DYNANPCID_LAST;
        }

        public bool IsPet()
        {
            return Identity >= PETID_FIRST && Identity < PETID_LAST;
        }

        public bool IsTrap()
        {
            return Identity >= TRAPID_FIRST && Identity < TRAPID_LAST;
        }

        public bool IsMapItem()
        {
            return Identity >= MAPITEM_FIRST && Identity < MAPITEM_LAST;
        }

        public bool IsFurniture()
        {
            return Identity >= SCENE_NPC_MIN && Identity < SCENE_NPC_MAX;
        }

        #endregion

        #region Synchronization

        public virtual async Task<bool> AddAttributesAsync(ClientUpdateType type, long value)
        {
            long currAttr = 0;
            switch (type)
            {
                case ClientUpdateType.Hitpoints:
                    currAttr = Math.Min(MaxLife, Math.Max(Life + value, 0));
                    break;

                case ClientUpdateType.Mana:
                    currAttr = Math.Min(MaxMana, Math.Max(Mana + value, 0));
                    break;

                default:
                    await Log.WriteLog(LogLevel.Warning, $"Role::AddAttributes {type} not handled");
                    return false;
            }

            await SynchroAttributesAsync(type, currAttr);
            return true;
        }

        public virtual async Task<bool> SetAttributesAsync(ClientUpdateType type, long value)
        {
            switch (type)
            {
                case ClientUpdateType.Hitpoints:
                    value = Life = (uint)Math.Max(0, Math.Min(MaxLife, value));
                    break;

                case ClientUpdateType.Mana:
                    value = Mana = (uint)Math.Max(0, Math.Min(MaxMana, value));
                    break;

                default:
                    await Log.WriteLog(LogLevel.Warning, $"Role::SetAttributes {type} not handled");
                    return false;
            }

            await SynchroAttributesAsync(type, value);
            return true;
        }

        public async Task SynchroAttributesAsync(ClientUpdateType type, long value, bool screen = false)
        {
            MsgUserAttrib msg = new MsgUserAttrib(Identity, type, (ulong)value);
            if (this is Character && !screen)
                await SendAsync(msg);

            if (screen)
            {
                if (this is Character user)
                    await user.BroadcastRoomMsgAsync(msg, false);
                else
                    Map?.BroadcastRoomMsgAsync(MapX, MapY, msg, Identity);
            }
        }

        #endregion

        #region Timers

        public virtual Task OnTimerAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Socket

        public async Task SendAsync(string message, MsgTalk.TalkChannel channel = MsgTalk.TalkChannel.System, Color? color = null)
        {
            await SendAsync(new MsgTalk(Identity, channel, color ?? Color.White, message));
        }

        public virtual async Task SendAsync(IPacket msg)
        {
            await Log.WriteLog(LogLevel.Warning, $"{GetType().Name} - {Identity} has no SendAsync handler");
        }

        public virtual async Task SendSpawnToAsync(Character player)
        {
            await Log.WriteLog(LogLevel.Warning, $"{GetType().Name} - {Identity} has no SendSpawnToAsync handler");
        }

        public virtual async Task BroadcastRoomMsgAsync(IPacket msg, bool self)
        {
            if (Map != null)
                await Map.BroadcastRoomMsgAsync(MapX, MapY, msg, (self ? Identity : 0));
        }

        #endregion

        #region Comparison

        public override bool Equals(object obj)
        {
            return obj is Role role && role.Identity == Identity;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Constants

        public const int SCENEID_FIRST = 1;
        public const int SYSNPCID_FIRST = 1;
        public const int SYSNPCID_LAST = 99999;
        public const int DYNANPCID_FIRST = 100000;
        public const int DYNANPCID_LAST = 199999;
        public const int SCENE_NPC_MIN = 200000;
        public const int SCENE_NPC_MAX = 299999;
        public const int SCENEID_LAST = 299999;

        public const int NPCSERVERID_FIRST = 400001;
        public const int MONSTERID_FIRST = 400001;
        public const int MONSTERID_LAST = 499999;
        public const int PETID_FIRST = 500001;
        public const int PETID_LAST = 599999;
        public const int NPCSERVERID_LAST = 699999;

        public const int CALLPETID_FIRST = 700001;
        public const int CALLPETID_LAST = 799999;

        public const int MAPITEM_FIRST = 800001;
        public const int MAPITEM_LAST = 899999;

        public const int TRAPID_FIRST = 900001;
        public const int MAGICTRAPID_FIRST = 900001;
        public const int MAGICTRAPID_LAST = 989999;
        public const int SYSTRAPID_FIRST = 990001;
        public const int SYSTRAPID_LAST = 999999;
        public const int TRAPID_LAST = 999999;

        public const int DYNAMAP_FIRST = 1000000;

        public const int PLAYER_ID_FIRST = 1000000;
        public const int PLAYER_ID_LAST = 1999999999;

        public const byte MAX_UPLEV = 140;

        public const int ADD_ENERGY_STAND_SECS = 2;
        public const int ADD_ENERGY_STAND = 3;
        public const int ADD_ENERGY_SIT = 10;
        public const int ADD_ENERGY_LIE = ADD_ENERGY_SIT / 2;
        public const int DEFAULT_USER_ENERGY = 70;
        public const int KEEP_STAND_MS = 1500;
        public const int MIN_SUPERMAP_KILLS = 25;
        public const int VETERAN_DIFF_LEVEL = 20;
        public const int HIGHEST_WATER_WIZARD_PROF = 135;
        public const int SLOWHEALLIFE_MS = 1000;
        public const int AUTOHEALLIFE_TIME = 10;
        public const int AUTOHEALLIFE_EACHPERIOD = 6;
        public const int TICK_SECS = 10;
        public const int MAX_PKLIMIT = 10000;
        public const int PILEMONEY_CHANGE = 5000;
        public const int ADDITIONALPOINT_NUM = 3;
        public const int PK_DEC_TIME = 180;
        public const int PKVALUE_DEC_ONCE = -1;
        public const int PKVALUE_DEC_ONCE_IN_PRISON = -3;
        public const int USER_ATTACK_SPEED = 800;
        public const int POISONDAMAGE_INTERVAL = 2;

        public const int MASTER_WEAPONSKILLLEVEL = 12;
        public const int MAX_WEAPONSKILLLEVEL = 20;

        #endregion
    }

    public enum FacingDirection : byte
    {
        SouthWest = 0,
        West = 1,
        NorthWest = 2,
        North = 3,
        NorthEast = 4,
        East = 5,
        SouthEast = 6,
        South = 7
    }

    public enum EntityAction : ushort
    {
        Dance1 = 1,
        Dance2 = 2,
        Dance3 = 3,
        Dance4 = 4,
        Dance5 = 5,
        Dance6 = 6,
        Dance7 = 7,
        Dance8 = 8,
        Stand = 100,
        Happy = 150,
        Angry = 160,
        Sad = 170,
        Wave = 190,
        Bow = 200,
        Kneel = 210,
        Cool = 230,
        Sit = 250,
        Lie = 270
    }
}