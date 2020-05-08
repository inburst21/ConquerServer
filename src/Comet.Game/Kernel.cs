// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Kernel.cs
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

using System.Collections.Generic;
using System.Runtime.Caching;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Routines;
using Comet.Game.World.Managers;
using Comet.Game.World.Threading;
using Comet.Shared;

#endregion

namespace Comet.Game
{
    /// <summary>
    ///     Kernel for the server, acting as a central core for pools of models and states
    ///     initialized by the server. Used in database repositories to load data into memory
    ///     from essential tables or tables which require heavy post-processing. Used in the
    ///     server packet process methods for tracking client and world states.
    /// </summary>
    public static class Kernel
    {
        // State caches
        public static MemoryCache Logins = MemoryCache.Default;
        public static List<uint> Registration = new List<uint>();

        public static ServerConfiguration.GameNetworkConfiguration Configuration;

        public static MapManager MapManager = new MapManager();
        public static RoleManager RoleManager = new RoleManager();
        public static ItemManager ItemManager = new ItemManager();
        public static PeerageManager PeerageManager = new PeerageManager();

        public static NetworkMonitor NetworkMonitor = new NetworkMonitor();

        public static SystemProcessor SystemThread = new SystemProcessor(1000);

        /// <summary>
        ///     Returns the next random number from the generator.
        /// </summary>
        /// <param name="minValue">The least legal value for the Random number.</param>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public static Task<int> NextAsync(int minValue, int maxValue)
        {
            return Services.Randomness.NextAsync(minValue, maxValue);
        }

        public static async Task<bool> StartupAsync()
        {
            MapManager.LoadData();
            MapManager.LoadMaps();

            await ItemManager.InitializeAsync();
            await RoleManager.InitializeAsync();
            await PeerageManager.InitializeAsync();

            await SystemThread.StartAsync();
            return true;
        }

        // Background services
        public static class Services
        {
            public static RandomnessService Randomness = new RandomnessService();
        }
    }
}