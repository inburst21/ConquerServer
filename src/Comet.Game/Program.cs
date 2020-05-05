// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Program.cs
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
using System.Threading;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Packets;
using Comet.Network.RPC;
using Comet.Shared;

#endregion

namespace Comet.Game
{
    /// <summary>
    ///     The game server listens for authentication players with a valid access token from
    ///     the account server, and hosts the game world. The game world in this project has
    ///     been simplified into a single server executable. For an n-server distributed
    ///     systems implementation of a Conquer Online server, see Chimera.
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.DefaultFileName = "GameServer";

            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = "Comet, Game Server";
            Console.WriteLine();
            await Log.WriteLog(LogLevel.Message, "  Comet: Game Server");
            await Log.WriteLog(LogLevel.Message, "  Copyright 2018 Gareth Jensen \"Spirited\"");
            await Log.WriteLog(LogLevel.Message, "  All Rights Reserved");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);
            if (!config.Valid)
            {
                await Log.WriteLog(LogLevel.Message, "Invalid server configuration file");
                return;
            }

            // Initialize the database
            await Log.WriteLog(LogLevel.Message, "Initializing server...");
            MsgConnect.StrictAuthentication = config.Authentication.StrictAuthPass;
            ServerDbContext.Configuration = config.Database;
            if (!ServerDbContext.Ping())
            {
                await Log.WriteLog(LogLevel.Message, "Invalid database configuration");
                return;
            }

            // Recover caches from the database
            var tasks = new List<Task>();
            Task.WaitAll(tasks.ToArray());

            // Start background services
            tasks = new List<Task>();
            tasks.Add(Kernel.Services.Randomness.StartAsync(CancellationToken.None));
            Task.WaitAll(tasks.ToArray());

            // Start the RPC server listener
            await Log.WriteLog(LogLevel.Message, "Launching server listeners...");
            var rpcserver = new RpcServerListener(new Remote());
            await rpcserver.StartAsync(config.RpcNetwork.Port, config.RpcNetwork.IPAddress)
                .ConfigureAwait(false);

            // Start the game server listener
            var server = new Server(config);
            await server.StartAsync(config.GameNetwork.Port, config.GameNetwork.IPAddress)
                .ConfigureAwait(false);

            // Output all clear and wait for user input
            await Log.WriteLog(LogLevel.Message, "Listening for new connections");
            Console.WriteLine();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}