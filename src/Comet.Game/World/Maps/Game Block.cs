﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Game Block.cs
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

using System.Collections.Concurrent;
using Comet.Game.States.Base_Entities;

#endregion

namespace Comet.Game.World.Maps
{
    /// <summary>
    ///     A block is a set of the map which will hold a collection with all entities in an area. This will help us
    ///     iterating over a limited number of roles when trying to process AI and movement. Instead of iterating a list with
    ///     thousand roles in the entire map, we'll just iterate the blocks around us.
    /// </summary>
    public class GameBlock
    {
        /// <summary>
        ///     The width/height of a view block.
        /// </summary>
        public const int BLOCK_SIZE = 18;

        /// <summary>
        ///     Collection of roles currently inside of this block.
        /// </summary>
        public ConcurrentDictionary<uint, Role> RoleSet = new ConcurrentDictionary<uint, Role>();

        public bool IsActive => RoleSet.Count > 0;

        public bool Add(Role role)
        {
            return RoleSet.TryAdd(role.Identity, role);
        }

        public bool Remove(Role role)
        {
            return RoleSet.TryRemove(role.Identity, out _);
        }

        public bool Remove(uint role)
        {
            return RoleSet.TryRemove(role, out _);
        }
    }
}