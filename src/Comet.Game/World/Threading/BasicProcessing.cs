// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Basic Processing.cs
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
using System.Threading.Tasks;
using Comet.Core;
using Comet.Shared;
using Comet.Shared.Models;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class SystemProcessor : TimerBase
    {
        public const string TITLE_FORMAT_S = @"[{0}] - Conquer Online Game Server [{9}] - {1} - Players: {3} (max:{4}) - {2} - Threads[S:{5:0000},G:{6:0000},U:{7:0000},A:{8:0000}]ms";

#if !DEBUG
        private TimeOut m_analytics = new TimeOut(300);
#else
        private TimeOut m_analytics = new TimeOut(3600);
#endif

        private TimeOut m_apiSync = new TimeOut(60);

        private DateTime m_serverStartTime;

        public SystemProcessor()
            : base(1000, "System Thread")
        {
        }

        public override Task OnStartAsync()
        {
            m_serverStartTime = DateTime.Now;
            m_analytics.Update();
            m_apiSync.Update();

            return base.OnStartAsync();
        }

        public override async Task<bool> OnElapseAsync()
        {
            Console.Title = string.Format(TITLE_FORMAT_S, Kernel.Configuration.ServerName, DateTime.Now.ToString("G"),
                Kernel.NetworkMonitor.UpdateStatsAsync(m_interval), Kernel.RoleManager.OnlinePlayers, Kernel.RoleManager.MaxOnlinePlayers,
                Kernel.SystemThread.ElapsedMilliseconds, Kernel.WorldThread.ElapsedMilliseconds, Kernel.UserThread.ElapsedMilliseconds,
                //Kernel.SystemThread.ElapsedMilliseconds, Kernel.GeneratorThread.ElapsedMilliseconds, Kernel.UserThread.ElapsedMilliseconds,
                Kernel.WorldThread.ElapsedMilliseconds, Kernel.Version);
                //Kernel.AiThread.ElapsedMilliseconds);

            if (m_analytics.ToNextTime())
            {
                await DoAnalyticsAsync();
            }

            foreach (var exchange in Kernel.Exchange.Values)
            {
                if (exchange.RequestExpire < DateTime.Now)
                {
                    Kernel.Exchange.TryRemove(exchange.GUID, out _);
                    exchange.Client.Disconnect();
                }
            }

#if USE_API
            try
            {
                if (m_apiSync.ToNextTime())
                {
                    await Kernel.Api.PostAsync(new ServerInformation
                    {
                        ServerName = Kernel.Configuration.ServerName,
                        ServerStatus = ServerInformation.RealmStatus.Online,
                        PlayerAmount = Kernel.RoleManager.OnlinePlayers,
                        MaxPlayerAmount = Kernel.RoleManager.MaxOnlinePlayers

                    }, MyApi.SYNC_INFORMATION_URL);
                }
            }
            catch
            {
                await Log.WriteLogAsync(LogLevel.Debug, "Failed to Write to the API.");
            }
#endif
            return true;
        }

        public async Task DoAnalyticsAsync()
        {
            var interval = DateTime.Now - m_serverStartTime;
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, "=".PadLeft(64, '='));
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Server Start Time: {m_serverStartTime:G}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Total Online Time: {(int)interval.TotalDays} days, {interval.Hours} hours, {interval.Minutes} minutes, {interval.Seconds} seconds");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Online Players[{Kernel.RoleManager.OnlinePlayers}], Max Online Players[{Kernel.RoleManager.MaxOnlinePlayers}], Distinct Players[{Kernel.RoleManager.OnlineUniquePlayers}], Role Count[{Kernel.RoleManager.RolesCount}]");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Total Bytes Sent: {Kernel.NetworkMonitor.TotalBytesSent:N0}, Total Packets Sent: {Kernel.NetworkMonitor.TotalPacketsSent:N0}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Total Bytes Recv: {Kernel.NetworkMonitor.TotalBytesRecv:N0}, Total Packets Recv: {Kernel.NetworkMonitor.TotalPacketsRecv:N0}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"System Thread: {Kernel.SystemThread.ElapsedMilliseconds:N0}ms");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Generator Thread: {Kernel.WorldThread.ElapsedMilliseconds:N0}ms");
            //await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Generator Thread: {Kernel.GeneratorThread.ElapsedMilliseconds:N0}ms");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"User Thread: {Kernel.UserThread.ElapsedMilliseconds:N0}ms");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Ai Thread: {Kernel.WorldThread.ElapsedMilliseconds:N0}ms ({Kernel.WorldThread.ProcessedMonsters} AI Agents)");
            //await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Ai Thread: {Kernel.AiThread.ElapsedMilliseconds:N0}ms ({Kernel.AiThread.ProcessedMonsters} AI Agents)");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"Identities Remaining: ");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"\tMonster: {IdentityGenerator.Monster.IdentitiesCount()}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"\tFurniture: {IdentityGenerator.Furniture.IdentitiesCount()}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"\tMapItem: {IdentityGenerator.MapItem.IdentitiesCount()}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, $"\tTraps: {IdentityGenerator.Traps.IdentitiesCount()}");
            await Log.WriteLogAsync("GameAnalytics", LogLevel.Message, "=".PadLeft(64, '='));
        }
    }
}