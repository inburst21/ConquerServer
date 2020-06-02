// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Game Map.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Comet.Core.Mathematics;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.Packets;
using Comet.Game.States;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.States.NPCs;
using Comet.Network.Packets;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.World.Maps
{
    public sealed class GameMap
    {
        public const uint DEFAULT_LIGHT_RGB = 0xFFFFFF;

        public const int REGION_NONE = 0,
            REGION_CITY = 1,
            REGION_WEATHER = 2,
            REGION_STATUARY = 3,
            REGION_DESC = 4,
            REGION_GOBALDESC = 5,
            REGION_DANCE = 6, // data0: idLeaderRegion, data1: idMusic, 
            REGION_PK_PROTECTED = 7,
            REGION_FLAG_BASE = 8;

        public static readonly sbyte[] WalkXCoords = {0, -1, -1, -1, 0, 1, 1, 1, 0};
        public static readonly sbyte[] WalkYCoords = {1, 1, 0, -1, -1, -1, 0, 1, 0};

        private readonly DbMap m_dbMap;
        private GameMapData m_mapData;

        private GameBlock[,] m_blocks;

        private ConcurrentDictionary<uint, Character> m_users = new ConcurrentDictionary<uint, Character>();
        private ConcurrentDictionary<uint, Role> m_roles = new ConcurrentDictionary<uint, Role>();

        private List<Passway> m_passway = new List<Passway>();

        public GameMap(DbMap map)
        {
            m_dbMap = map;
        }

        public GameMap()
        {
            
        }

        public uint Identity => m_dbMap?.Identity ?? 0;
        public string Name => m_dbMap?.Name ?? "Invalid";
        public uint OwnerIdentity => m_dbMap?.OwnerIdentity ?? 0;
        public uint MapDoc => m_dbMap?.MapDoc ?? 0;
        public uint Type => m_dbMap?.Type ?? 0;

        public int Width => m_mapData?.Width ?? 0;
        public int Height => m_mapData?.Height ?? 0;

        public uint Light => m_dbMap?.Color ?? 0;

        public int BlocksX => Width / GameBlock.BLOCK_SIZE;
        public int BlocksY => Height / GameBlock.BLOCK_SIZE;

        public ulong Flag { get; set; }
        public int PlayerCount => m_users.Count;

        public async Task<bool> Initialize()
        {
            if (m_dbMap == null) return false;

            m_mapData = Kernel.MapManager.GetMapData(MapDoc);
            if (m_mapData == null)
            {
                Log.WriteLog(LogLevel.Warning, $"Could not load map {Identity}({MapDoc}): map data not found").Wait();
                return false;
            }

            m_blocks = new GameBlock[BlocksX, BlocksY];
            for (int y = 0; y < BlocksY; y++)
            {
                for (int x = 0; x < BlocksX; x++)
                {
                    m_blocks[x, y] = new GameBlock();
                }
            }

            List<DbPassway> passways = await PasswayRepository.GetAsync(Identity);
            foreach (var dbPassway in passways)
            {
                DbPortal portal = await PortalRepository.GetAsync(dbPassway.TargetMapId, dbPassway.TargetPortal);
                if (portal == null)
                {
                    await Log.WriteLog(LogLevel.Error, $"Could not find portal for passway [{dbPassway.Identity}]");
                    continue;
                }

                m_passway.Add(new Passway
                {
                    Index = (int)dbPassway.MapIndex,
                    TargetMap = dbPassway.TargetMapId,
                    TargetX = (ushort)portal.PortalX,
                    TargetY = (ushort)portal.PortalY
                });
            }

            return true;
        }

        #region Query Role

        public Role QueryAroundRole(Role sender, uint target)
        {
            int currentBlockX = GetBlockX(sender.MapX);
            int currentBlockY = GetBlockY(sender.MapY);
            return Query9Blocks(currentBlockX, currentBlockY).FirstOrDefault(x => x.Identity == target);
        }

        #endregion

        #region Role Management

        public async Task<bool> AddAsync(Role role)
        {
            if (m_roles.TryAdd(role.Identity, role))
            {
                EnterBlock(role, role.MapX, role.MapY);

                if (role is Character character)
                {
                    m_users.TryAdd(character.Identity, character);
                    await character.Screen.UpdateAsync();
                }
                else
                {
                    Kernel.RoleManager.AddRole(role);
                    foreach (var user in m_users.Values.Where(x =>
                        ScreenCalculations.GetDistance(x.MapX, x.MapY, role.MapX, role.MapY) <= Screen.VIEW_SIZE))
                    {
                        await user.Screen.SpawnAsync(role);
                    }
                }

                return true;
            }

            return false;
        }

        public async Task<bool> RemoveAsync(uint idRole)
        {
            if (m_roles.TryRemove(idRole, out var role))
            {
                m_users.TryRemove(idRole, out _);
                LeaveBlock(role);

                if (!(role is Character))
                    Kernel.RoleManager.RemoveRole(idRole);
                
                foreach (var user in Query9BlocksByPos(role.MapX, role.MapY).Where(x => x is Character)
                    .Cast<Character>())
                {
                    await user.Screen.RemoveAsync(idRole, true);
                }
            }

            return false;
        }

        #endregion

        #region Broadcasting

        public async Task SendMapInfoAsync(Character user)
        {
            // todo handle weather
            MsgAction action = new MsgAction
            {
                Action = MsgAction.ActionType.MapArgb,
                Identity = 1,
                Command = Light,
                ArgumentX = 0,
                ArgumentY = 0
            };
            await user.SendAsync(action);
            await user.SendAsync(new MsgMapInfo(Identity, MapDoc, Type));
        }

        public async Task BroadcastMsgAsync(IPacket msg, uint exclude = 0)
        {
            foreach (var user in m_users.Values)
            {
                if (user.Identity == exclude)
                    continue;

                await user.SendAsync(msg);
            }
        }

        public async Task BroadcastRoomMsgAsync(int x, int y, IPacket msg, uint exclude = 0)
        {
            foreach (var user in m_users.Values)
            {
                if (user.Identity == exclude ||
                    ScreenCalculations.GetDistance(x, y, user.MapX, user.MapY) > Screen.VIEW_SIZE)
                    continue;

                await user.SendAsync(msg);
            }
        }

        #endregion

        #region Blocks

        public void EnterBlock(Role role, int newX, int newY, int oldX = 0, int oldY = 0)
        {
            int currentBlockX = GetBlockX(newX);
            int currentBlockY = GetBlockY(newY);

            int oldBlockX = GetBlockX(oldX);
            int oldBlockY = GetBlockY(oldY);

            if (currentBlockX != oldBlockX || currentBlockY != oldBlockY)
            {
                if (GetBlock(oldBlockX, oldBlockY)?.RoleSet.ContainsKey(role.Identity) == true)
                    LeaveBlock(role);

                GetBlock(currentBlockX, currentBlockY)?.Add(role);
            }
        }

        public void LeaveBlock(Role role)
        {
            GetBlock(GetBlockX(role.MapX), GetBlockY(role.MapY))?.Remove(role);
        }


        public GameBlock GetBlock(int x, int y)
        {
            if (x < 0 || y < 0 || x >= BlocksX || y >= BlocksY)
                return null;
            return m_blocks[x, y];
        }

        public List<Role> Query9BlocksByPos(int x, int y)
        {
            return Query9Blocks(GetBlockX(x), GetBlockY(y));
        }

        public List<Role> Query9Blocks(int x, int y)
        {
            List<Role> result = new List<Role>();

            //Console.WriteLine(@"============== Query Block Begin =================");
            for (int aroundBlock = 0; aroundBlock < WalkXCoords.Length; aroundBlock++)
            {
                int viewBlockX = x + WalkXCoords[aroundBlock];
                int viewBlockY = y + WalkYCoords[aroundBlock];

                //Console.WriteLine($@"Block: {viewBlockX},{viewBlockY} [from: {viewBlockX*18},{viewBlockY*18}] [to: {viewBlockX*18+18},{viewBlockY*18+18}]");

                if (viewBlockX < 0 || viewBlockY < 0 || viewBlockX >= BlocksX || viewBlockY >= BlocksY)
                    continue;

                result.AddRange(GetBlock(viewBlockX, viewBlockY).RoleSet.Values);
            }

            //Console.WriteLine(@"============== Query Block End =================");
            return result;
        }

        #endregion

        #region Map Checks

        /// <summary>
        ///     Checks if the map is a pk field. Wont add pk points.
        /// </summary>
        public bool IsPkField()
        {
            return (Type & (uint) MapTypeFlags.PkField) != 0;
        }

        /// <summary>
        ///     Disable teleporting by skills or scrolls.
        /// </summary>
        public bool IsChgMapDisable()
        {
            return (Type & (uint) MapTypeFlags.ChangeMapDisable) != 0;
        }

        /// <summary>
        ///     Disable recording the map position into the database.
        /// </summary>
        public bool IsRecordDisable()
        {
            return (Type & (uint) MapTypeFlags.RecordDisable) != 0;
        }

        /// <summary>
        ///     Disable team creation into the map.
        /// </summary>
        public bool IsTeamDisable()
        {
            return (Type & (uint) MapTypeFlags.TeamDisable) != 0;
        }

        /// <summary>
        ///     Disable use of pk on the map.
        /// </summary>
        public bool IsPkDisable()
        {
            return (Type & (uint) MapTypeFlags.PkDisable) != 0;
        }

        /// <summary>
        ///     Disable teleporting by actions.
        /// </summary>
        public bool IsTeleportDisable()
        {
            return (Type & (uint) MapTypeFlags.TeleportDisable) != 0;
        }

        /// <summary>
        ///     Checks if the map is a syndicate map
        /// </summary>
        /// <returns></returns>
        public bool IsSynMap()
        {
            return (Type & (uint) MapTypeFlags.GuildMap) != 0;
        }

        /// <summary>
        ///     Checks if the map is a prision
        /// </summary>
        public bool IsPrisionMap()
        {
            return (Type & (uint) MapTypeFlags.PrisonMap) != 0;
        }

        /// <summary>
        ///     If the map enable the fly skill.
        /// </summary>
        public bool IsWingDisable()
        {
            return (Type & (uint) MapTypeFlags.WingDisable) != 0;
        }

        /// <summary>
        ///     Check if the map is in war.
        /// </summary>
        public bool IsWarTime()
        {
            return (Flag & 1) != 0;
        }

        /// <summary>
        ///     Check if the map is the training ground. [1039]
        /// </summary>
        public bool IsTrainingMap()
        {
            return Identity == 1039;
        }

        /// <summary>
        ///     Check if its the family (clan) map.
        /// </summary>
        public bool IsFamilyMap()
        {
            return (Type & (uint) MapTypeFlags.Family) != 0;
        }

        /// <summary>
        ///     If the map enables booth to be built.
        /// </summary>
        public bool IsBoothEnable()
        {
            return (Type & (uint) MapTypeFlags.BoothEnable) != 0;
        }

        public bool IsDeadIsland()
        {
            return (Type & (uint) MapTypeFlags.DeadIsland) != 0;
        }

        public bool IsPkGameMap()
        {
            return (Type & (uint) MapTypeFlags.PkGame) != 0;
        }

        public bool IsMineField()
        {
            return (Type & (uint) MapTypeFlags.MineField) != 0;
        }

        public bool IsSkillMap()
        {
            return (Type & (uint) MapTypeFlags.SkillMap) != 0;
        }

        public bool IsLineSkillMap()
        {
            return (Type & (ulong) MapTypeFlags.LineSkillOnly) != 0;
        }

        public bool IsDynamicMap()
        {
            return Identity > 999999;
        }

        #endregion

        #region Position Check

        public bool IsSuperPosition(int x, int y)
        {
            return GetBlock(GetBlockX(x), GetBlockY(y))?.RoleSet.Values.Any(a => a.MapX == x && a.MapY == y && a.IsAlive) != false;
        }

        public bool IsValidPoint(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public bool IsStandEnable(int x, int y)
        {
            return m_mapData[x, y].IsAccessible();
        }

        public bool IsMoveEnable(int x, int y)
        {
            return IsValidPoint(x, y) && IsStandEnable(x, y);
        }

        public bool IsLayItemEnable(int x, int y)
        {
            return Query9BlocksByPos(x, y).All(role => (!(role is MapItem) && !(role is BaseNpc)) || role.MapX != x || role.MapY != y);
        }

        public bool FindDropItemCell(int range, ref Point sender)
        {
            if (IsLayItemEnable(sender.X, sender.Y))
                return true;

            int size = range * 2 + 1;
            int bufSize = size ^ 2;

            for (int i = 0; i < 8; i++)
            {
                int newX = sender.X + WalkXCoords[i];
                int newY = sender.Y + WalkXCoords[i];
                if (IsLayItemEnable(newX, newY))
                {
                    sender.X = newX;
                    sender.Y = newY;
                    return true;
                }
            }

            Point pos = sender;
            var setItem = Query9BlocksByPos(sender.X, sender.Y).Where(x => x is MapItem && x.GetDistance(pos.X, pos.Y) <= range).Cast<MapItem>().ToList();
            int nMinRange = range + 1;
            bool ret = false;
            var posFree = new Point();
            for (int i = Math.Max(sender.X - range, 0); i <= sender.X + range && i < Width; i++)
            {
                for (int j = Math.Max(sender.Y - range, 0); j <= sender.Y + range && j < Height; j++)
                {
                    int idx = Pos2Index(i - (sender.X - range), j - (sender.Y - range), size, size);

                    if (idx >= 0 && idx < bufSize)
                        if (setItem.FirstOrDefault(x =>
                                Pos2Index(x.MapX - i + range, x.MapY - j + range, range, range) == idx) != null)
                            continue;

                    if (IsLayItemEnable(sender.X, sender.Y))
                    {
                        double nDistance = ScreenCalculations.GetDistance(i, j, sender.X, sender.Y);
                        if (nDistance < nMinRange)
                        {
                            nMinRange = (int)nDistance;
                            posFree.X = i;
                            posFree.Y = j;
                            ret = true;
                        }
                    }
                }
            }

            if (ret)
            {
                sender = posFree;
                return true;
            }
            return false;
        }

        #endregion

        #region Indexes

        public int Pos2Index(int x, int y, int cx, int cy)
        {
            return x + y * cx;
        }

        public int Index2X(int idx, int cx, int cy)
        {
            return idx % cy;
        }

        public int Index2Y(int idx, int cx, int cy)
        {
            return idx / cy;
        }

        #endregion

        #region Portals and Passages

        public bool GetRebornMap(ref uint idMap, ref Point target)
        {
            idMap = m_dbMap.RebornMap;
            GameMap targetMap = Kernel.MapManager.GetMap(idMap);
            if (targetMap == null)
            {
                Log.WriteLog(LogLevel.Error, $"Could not get reborn map [{Identity}]!").ConfigureAwait(false);
                return false;
            }

            target = new Point(m_dbMap.LinkX, m_dbMap.LinkY);
            return true;
        }

        public bool GetPassageMap(ref uint idMap, ref Point target, ref Point source)
        {
            if (!IsValidPoint(source.X, source.Y))
                return false;

            int idxPassage = m_mapData.GetPassage(source.X, source.Y);
            if (idxPassage < 0)
                return false;

            if (IsDynamicMap())
            {
                idMap = m_dbMap.LinkMap;
                target.X = m_dbMap.LinkX;
                target.Y = m_dbMap.LinkY;
                return true;
            }

            Passway passway = m_passway.FirstOrDefault(x => x.Index == idxPassage);
            idMap = passway.TargetMap;
            target = new Point(passway.TargetX, passway.TargetY);
            return true;
        }

        #endregion

        #region Ai Timer

        public async Task OnTimerAsync()
        {
            if (m_users.Count == 0)
                return;

            for (int x = 0; x < BlocksX; x++)
            {
                for (int y = 0; y < BlocksY; y++)
                {
                    GameBlock block = m_blocks[x, y];
                    if (block.IsActive)
                    {
                        foreach (var monster in block.RoleSet.Values.Where(z => z is Monster).Cast<Monster>())
                        {
                            await monster.OnTimerAsync();
                        }
                    }
                }
            }
        }

        #endregion

        #region Tiles

        public Tile this[int x, int y] => m_mapData[x, y];

        #endregion

        #region Static

        public static int GetBlockX(int x)
        {
            return x / GameBlock.BLOCK_SIZE;
        }

        public static int GetBlockY(int y)
        {
            return y / GameBlock.BLOCK_SIZE;
        }

        #endregion
    }

    public struct Passway
    {
        public int Index;
        public uint TargetMap;
        public ushort TargetX;
        public ushort TargetY;
    }

    [Flags]
    public enum MapTypeFlags
    {
        Normal = 0,
        PkField = 1, //0x1 1
        ChangeMapDisable = 1 << 1, //0x2 2
        RecordDisable = 1 << 2, //0x4 4 
        PkDisable = 1 << 3, //0x8 8
        BoothEnable = 1 << 4, //0x10 16
        TeamDisable = 1 << 5, //0x20 32
        TeleportDisable = 1 << 6, // 0x40 64
        GuildMap = 1 << 7, // 0x80 128
        PrisonMap = 1 << 8, // 0x100 256
        WingDisable = 1 << 9, // 0x200 512
        Family = 1 << 10, // 0x400 1024
        MineField = 1 << 11, // 0x800 2048
        PkGame = 1 << 12, // 0x1000 4098
        NeverWound = 1 << 13, // 0x2000 8196
        DeadIsland = 1 << 14, // 0x4000 16392
        SkillMap = 1 << 17, // 0x20000 65568
        LineSkillOnly = 1 << 18
    }
}