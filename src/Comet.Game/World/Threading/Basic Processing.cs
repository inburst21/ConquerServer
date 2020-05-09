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

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class SystemProcessor : TimerBase
    {
        public const string TITLE_FORMAT_S = @"[{0}] - Conquer Online Game Server - {1} - {2}";

        private TimeOut m_analytics = new TimeOut(60);

        private DateTime m_ServerStartTime;

        public SystemProcessor()
            : base(1000, "System Thread")
        {
        }

        public override Task OnStartAsync()
        {
            m_ServerStartTime = DateTime.Now;
            return base.OnStartAsync();
        }

        public override async Task<bool> OnElapseAsync()
        {
            DateTime now = DateTime.Now;
            Console.Title = string.Format(TITLE_FORMAT_S, Kernel.Configuration.ServerName, DateTime.Now.ToString("G"),
                Kernel.NetworkMonitor.UpdateStatsAsync(m_interval));

            if (m_analytics.ToNextTime())
            {
                var interval = now - m_ServerStartTime;
                await Log.WriteLog("GameAnalytics", LogLevel.Message, "=".PadLeft(64, '='));
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Server Start Time: {m_ServerStartTime:G}");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Total Online Time: {(int) interval.TotalDays} days, {interval.Hours} hours, {interval.Minutes} minutes, {interval.Seconds} seconds");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Online Players[{Kernel.RoleManager.OnlinePlayers}], Max Online Players[{Kernel.RoleManager.MaxOnlinePlayers}], Role Count[{Kernel.RoleManager.RolesCount}]");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Total Bytes Sent: {Kernel.NetworkMonitor.TotalBytesSent}, Total Packets Sent: {Kernel.NetworkMonitor.TotalPacketsSent}");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Total Bytes Recv: {Kernel.NetworkMonitor.TotalBytesRecv}, Total Packets Recv: {Kernel.NetworkMonitor.TotalPacketsRecv}");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"System Thread: {Kernel.SystemThread.ElapsedMilliseconds}ms");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"Generator Thread: {Kernel.GeneratorThread.ElapsedMilliseconds}ms");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, $"User Thread: {Kernel.UserThread.ElapsedMilliseconds}ms");
                await Log.WriteLog("GameAnalytics", LogLevel.Message, "=".PadLeft(64, '='));
            }

            return true;
        }
    }
}