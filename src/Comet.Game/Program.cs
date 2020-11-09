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
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Comet.Game.Database;
using Comet.Game.Packets;
using Comet.Game.States.Items;
using Comet.Network.RPC;
using Comet.Network.Security;
using Comet.Shared;
using Comet.Shared.Models;

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
            Console.Title = @"Comet, Game Server";
            Console.WriteLine();
            await Log.WriteLog(LogLevel.Message, "  Comet: Game Server");
            await Log.WriteLog(LogLevel.Message, "  Copyright 2018-2020 Gareth Jensen \"Spirited\"");
            await Log.WriteLog(LogLevel.Message, "  All Rights Reserved");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);
            if (!config.Valid)
            {
                await Log.WriteLog(LogLevel.Error, "Invalid server configuration file");
                return;
            }

            Kernel.Configuration = config.GameNetwork;

            // Initialize the database
            await Log.WriteLog(LogLevel.Message, "Initializing server...");
            MsgConnect.StrictAuthentication = config.Authentication.StrictAuthPass;
            ServerDbContext.Configuration = config.Database;
            if (!await ServerDbContext.PingAsync())
            {
                await Log.WriteLog(LogLevel.Error, "Invalid database configuration");
                return;
            }

            if (!await Kernel.StartupAsync())
            {
                await Log.WriteLog(LogLevel.Error, "Could not load database related stuff");
                return;
            }

            // Recover caches from the database
            var tasks = new List<Task>();
            Task.WaitAll(tasks.ToArray());

            // Start background services
            tasks = new List<Task>();
            tasks.Add(Kernel.Services.Randomness.StartAsync(CancellationToken.None));
            tasks.Add(DiffieHellman.ProbablePrimes.StartAsync(CancellationToken.None));
            Task.WaitAll(tasks.ToArray());

            // await ConvertItemsAsync();

            // Start the RPC server listener
            await Log.WriteLog(LogLevel.Message, "Launching server listeners...");
            var rpcserver = new RpcServerListener(new Remote());
            _ = rpcserver.StartAsync(config.RpcNetwork.Port, config.RpcNetwork.IPAddress)
                .ConfigureAwait(false);

            // Start the game server listener
            var server = new Server(config);
            _ = server.StartAsync(config.GameNetwork.Port, config.GameNetwork.IPAddress)
                .ConfigureAwait(false);

#if !DEBUG && USE_API
            Kernel.Api = new MyApi(Kernel.Configuration.ServerName, config.ApiAuth.Username, config.ApiAuth.Password);
            await Kernel.Api.PostAsync(new ServerInformation
            {
                ServerName = Kernel.Configuration.ServerName,
                ServerStatus = ServerInformation.RealmStatus.Online,
                PlayerAmount = 0,
                MaxPlayerAmount = Kernel.Configuration.MaxConn
            }, MyApi.SYNC_INFORMATION_URL);
#endif

            // Output all clear and wait for user input
            await Log.WriteLog(LogLevel.Message, "Listening for new connections");
            Console.WriteLine();

            bool result = await CommandCenterAsync();
            if (!result)
                await Log.WriteLog(LogLevel.Error, "Game server has exited without success.");

#if !DEBUG && USE_API
            await Kernel.Api.PostAsync(new ServerInformation
            {
                ServerName = Kernel.Configuration.ServerName,
                ServerStatus = ServerInformation.RealmStatus.Offline,
                PlayerAmount = 0,
                MaxPlayerAmount = Kernel.Configuration.MaxConn

            }, MyApi.SYNC_INFORMATION_URL);
#endif

            await Kernel.CloseAsync();
        }

        private static async Task<bool> CommandCenterAsync()
        {
            while (true)
            {
                string text = Console.ReadLine();

                if (string.IsNullOrEmpty(text))
                    continue;

                if (text == "exit")
                {
                    await Log.WriteLog(LogLevel.Warning, "Server will shutdown...");
                    return true;
                }

                string[] full = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (full.Length <= 0)
                    continue;

                switch (full[0].ToLower())
                {
                    case "/players":

                        break;

                    case "/analytics":
                    {
                        await Kernel.SystemThread.DoAnalyticsAsync();
                        break;
                    }
                }
            }
        }

        private static async Task ConvertItemsAsync()
        {
            await using ServerDbContext ctx = new ServerDbContext();
            await using StreamWriter writer = new StreamWriter("cq_item_convert.sql", false, Encoding.ASCII);

            await writer.WriteLineAsync($"##############################################################################");
            await writer.WriteLineAsync($"# ");
            await writer.WriteLineAsync($"# Players items exportation and converting tool");
            await writer.WriteLineAsync($"# {Environment.CurrentDirectory}");
            await writer.WriteLineAsync($"# {Environment.UserName} - {DateTime.Now:U}");
            await writer.WriteLineAsync($"# ");
            await writer.WriteLineAsync($"##############################################################################");

            int count = 0;
            DataTable oldItems = await ctx.SelectAsync("SELECT * FROM cq_item_old");
            foreach (DataRow row in oldItems.Rows)
            {
                uint type = uint.Parse(row["type"].ToString());
                byte newColor = byte.Parse(row["color"].ToString());
                if (Item.IsShield(type) || Item.IsArmor(type) || Item.IsHelmet(type))
                {
                    uint oldType = type;
                    int color = (int)(type % 1000 / 100);
                    if (color > 1)
                        newColor = (byte) Math.Max(3, color);
                    type = (uint)(type - color * 100);
                    _ = Log.GmLog($"ItemColorChangeType", $"PlayerId: {row["player_id"]}, OwnerId: {row["owner_id"]}, OldType: {oldType}, NewType: {type}");
                }
                
                Dictionary<string, string> kvp = new Dictionary<string, string>();
                kvp.Add("`id`", row["id"].ToString());
                kvp.Add("`type`", type.ToString());
                kvp.Add("`owner_id`", row["owner_id"].ToString());
                kvp.Add("`player_id`", row["player_id"].ToString());
                kvp.Add("`amount`", row["amount"].ToString());
                kvp.Add("`amount_limit`", row["amount_limit"].ToString());
                kvp.Add("`ident`", row["ident"].ToString());
                kvp.Add("`position`", row["position"].ToString());
                kvp.Add("`gem1`", row["gem1"].ToString());
                kvp.Add("`gem2`", row["gem2"].ToString());
                kvp.Add("`magic1`", row["magic1"].ToString());
                kvp.Add("`magic2`", row["magic2"].ToString());
                kvp.Add("`magic3`", row["magic3"].ToString());
                kvp.Add("`data`", row["data"].ToString());
                kvp.Add("`reduce_dmg`", row["reduce_dmg"].ToString());
                kvp.Add("`add_life`", row["add_life"].ToString());
                kvp.Add("`anti_monster`", row["anti_monster"].ToString());
                kvp.Add("`chk_sum`", row["chk_sum"].ToString());
                kvp.Add("`SpecialFlag`", row["SpecialFlag"].ToString());
                kvp.Add("`color`", newColor.ToString());
                kvp.Add("`Addlevel_exp`", row["Addlevel_exp"].ToString());
                kvp.Add("`monopoly`", row["monopoly"].ToString());

                string query = $"INSERT INTO `cq_item` ({string.Join(",", kvp.Keys)}) VALUES ({string.Join(",", kvp.Values)});";

                await writer.WriteLineAsync(query);
                count++;
            }

            writer.Close();
            await Log.WriteLog(LogLevel.Debug, $"Converted items: {count}");
        }
    }
}