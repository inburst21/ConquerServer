// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Program.cs
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
using System.Threading.Tasks;
using Comet.Account.Database;
using Comet.Account.Database.Models;
using Comet.Account.Database.Repositories;
using Comet.Shared;

#endregion

namespace Comet.Account
{
    /// <summary>
    ///     The account server accepts clients and authenticates players from the client's
    ///     login screen. If the player enters valid account credentials, then the server
    ///     will send login details to the game server and disconnect the client. The client
    ///     will reconnect to the game server with an access token from the account server.
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Log.DefaultFileName = "AccountServer";

            // Copyright notice may not be commented out. If adding your own copyright or
            // claim of ownership, you may include a second copyright above the existing
            // copyright. Do not remove completely to comply with software license. The
            // project name and version may be removed or changed.
            Console.Title = "Comet, Account Server";
            Console.WriteLine();
            await Log.WriteLog(LogLevel.Message, "  Comet: Account Server");
            await Log.WriteLog(LogLevel.Message, "  Copyright 2018 Gareth Jensen \"Spirited\"");
            await Log.WriteLog(LogLevel.Message, "  All Rights Reserved");
            Console.WriteLine();

            // Read configuration file and command-line arguments
            var config = new ServerConfiguration(args);
            if (!config.Valid)
            {
                Console.WriteLine("Invalid server configuration file");
                return;
            }

            // Initialize the database
            await Log.WriteLog(LogLevel.Message, "Initializing server...");
            ServerDbContext.Configuration = config.Database;
            if (!ServerDbContext.Ping())
            {
                await Log.WriteLog(LogLevel.Message, "Invalid database configuration");
                return;
            }

            // Recover caches from the database
            var tasks = new List<Task>();
            tasks.Add(RealmsRepository.LoadAsync());
#pragma warning disable VSTHRD103 // Chame métodos assíncronos quando estiver em um método assíncrono
            Task.WaitAll(tasks.ToArray());
#pragma warning restore VSTHRD103 // Chame métodos assíncronos quando estiver em um método assíncrono

            // Start the server listener
            await Log.WriteLog(LogLevel.Message, "Launching server listener...");
            var server = new Server(config);
            _ = server.StartAsync(config.Network.Port, config.Network.IPAddress).ConfigureAwait(false);

            // Output all clear and wait for user input
            await Log.WriteLog(LogLevel.Message, "Listening for new connections");
            Console.WriteLine();
            bool result = await CommandCenterAsync();
            if (!result)
                await Log.WriteLog(LogLevel.Error, "Account server has exited without success.");
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

                string[] full = text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                if (full.Length <= 0)
                    continue;

                switch (full[0].ToLower())
                {
                    case "newuser":
                        if (full.Length < 3)
                        {
                            Console.WriteLine(@"newuser username password [type] [vip]");
                            continue;
                        }

                        string username = full[1];
                        string salt = AccountsRepository.GenerateSalt();
                        string password = AccountsRepository.HashPassword(full[2], salt);
                        int type = 1;
                        int vip = 0;

                        if (full.Length >= 4)
                            int.TryParse(full[3], out type);
                        if (full.Length >= 5)
                            int.TryParse(full[4], out vip);

                        if (await AccountsRepository.FindAsync(username) != null)
                        {
                            Console.WriteLine(@"The required username is already in use.");
                            continue;
                        }

                        DbAccount account = new DbAccount
                        {
                            AuthorityID = (ushort) type,
                            Username = username,
                            Password = password,
                            IPAddress = "127.0.0.1",
                            Salt = salt,
                            StatusID = 2,
                            VipLevel = (byte) vip
                        };

                        await using (var db = new ServerDbContext())
                        {
                            db.Accounts.Add(account);
                            await db.SaveChangesAsync();
                        }

                        continue;
                }
            }
        }
    }
}