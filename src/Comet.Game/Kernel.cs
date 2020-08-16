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

using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Routines;
using Comet.Game.World.Managers;
using Comet.Game.World.Threading;
using Comet.Network.Security;
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
        public const int SERVER_VERSION = 5017;
        public static readonly string Version;

        // State caches
        public static MemoryCache Logins = MemoryCache.Default;
        public static List<uint> Registration = new List<uint>();

        public static MyApi Api;

        public static ServerConfiguration.GameNetworkConfiguration Configuration;

        public static MapManager MapManager = new MapManager();
        public static RoleManager RoleManager = new RoleManager();
        public static ItemManager ItemManager = new ItemManager();
        public static PeerageManager PeerageManager = new PeerageManager();
        public static MagicManager MagicManager = new MagicManager();
        public static EventManager EventManager = new EventManager();
        public static SyndicateManager SyndicateManager = new SyndicateManager();
        public static MineManager MineManager = new MineManager();
        public static PigeonManager PigeonManager = new PigeonManager();

        public static NetworkMonitor NetworkMonitor = new NetworkMonitor();

        public static SystemProcessor SystemThread = new SystemProcessor();
        public static UserProcessor UserThread = new UserProcessor();
        public static GeneratorProcessor GeneratorThread = new GeneratorProcessor();
        public static AiProcessor AiThread = new AiProcessor();
        public static AutomaticActionsProcessing AutomaticActions = new AutomaticActionsProcessing();
        public static EventsProcessing EventThread = new EventsProcessing();

        static Kernel()
        {
            Version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString() ?? "Error";
        }

        /// <summary>
        /// Returns the next random number from the generator.
        /// </summary>
        /// <param name="maxValue">One greater than the greatest legal return value.</param>
        public static Task<int> NextAsync(int maxValue)
        {
            return NextAsync(0, maxValue);
        }

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
            await MapManager.LoadDataAsync();
            await MapManager.LoadMapsAsync();

            await ItemManager.InitializeAsync();
            await RoleManager.InitializeAsync();
            await MagicManager.InitializeAsync();
            await PeerageManager.InitializeAsync();
            await SyndicateManager.InitializeAsync();
            await EventManager.InitializeAsync();
            await MineManager.InitializeAsync();
            await PigeonManager.InitializeAsync();

            await SystemThread.StartAsync();
            await UserThread.StartAsync();
            await GeneratorThread.StartAsync();
            await AiThread.StartAsync();
            await AutomaticActions.StartAsync();
            await EventThread.StartAsync();

            Blowfish.InitialKey = Encoding.ASCII.GetBytes("DR654dt34trg4UI6");
            return true;
        }

        public static async Task<bool> CloseAsync()
        {
            await RoleManager.KickoutAllAsync("Server is now closing");

            SystemThread.CloseRequest = true;
            UserThread.CloseRequest = true;
            GeneratorThread.CloseRequest = true;
            AiThread.CloseRequest = true;
            AutomaticActions.CloseRequest = true;
            EventThread.CloseRequest = true;

            for (int i = 0; i < 5; i++)
            {
                await Log.WriteLog(LogLevel.Message, $"Server will shutdown in {5-i} seconds...");
                Thread.Sleep(1000);
            }
            return true;
        }

        // Background services
        public static class Services
        {
            public static RandomnessService Randomness = new RandomnessService();
        }

        public static async Task<bool> ChanceCalcAsync(int chance, int outOf)
        {
            return await NextAsync(outOf) < chance;
        }

        /// <summary>
        /// Calculates the chance of success based in a rate.
        /// </summary>
        /// <param name="chance">Rate in percent.</param>
        /// <returns>True if the rate is successful.</returns>
        public static async Task<bool> ChanceCalcAsync(double chance)
        {
            const int DIVISOR_I = 10000;
            const int MAX_VALUE = 100 * DIVISOR_I;
            try
            {
                return await NextAsync(0, MAX_VALUE) <= chance * DIVISOR_I;
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Chance Calc error!");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                return false;
            }
        }
    }
}