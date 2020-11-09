using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Comet.Service
{
    public sealed class ServerWatch
    {
        public enum ServerType
        {
            Account,
            Game
        }

        public struct MaintenanceTime
        {
            public DayOfWeek DayOfWeek;
            public uint StartTime;
            public uint EndTime;
        }

        private List<MaintenanceTime> m_times = new List<MaintenanceTime>();

        private ServerWatch(ServiceConfiguration config, ServerType type)
        {
            List<MaintenanceConfiguration> timeList;
            switch (type)
            {
                case ServerType.Account:
                    timeList = config.AccountServer.Maintenance;
                    Path = config.AccountServer.Path.Replace("${CurrentDirectory}", Environment.CurrentDirectory);
                    Mutex = config.AccountServer.Mutex;
                    break;
                case ServerType.Game:
                    timeList = config.GameServer.Maintenance;
                    Path = config.GameServer.Path.Replace("${CurrentDirectory}", Environment.CurrentDirectory);
                    Mutex = config.GameServer.Mutex;
                    break;
                default:
                    throw new Exception($"Invalid {{ServerType}} {type}");
            }

            foreach (var time in timeList)
            {
                m_times.Add(new MaintenanceTime
                {
                    DayOfWeek = (DayOfWeek) (time.WeekDay%7),
                    StartTime = uint.Parse(time.From.Replace(":", "")),
                    EndTime = uint.Parse(time.To.Replace(":", ""))
                });
            }
        }

        public Process Process { get; private set; }
        public string Path { get; }
        public string Mutex { get; }

        public bool IsMaintenanceTime()
        {
            uint dwNow = uint.Parse(DateTime.Now.ToString("HHmm"));
            return m_times.Any(x => x.DayOfWeek == DateTime.Now.DayOfWeek && dwNow >= x.StartTime && dwNow < x.EndTime);
        }

        public async Task OnTimerAsync()
        {
            if (IsMaintenanceTime())
            {
                if (Process != null)
                {
                    
                }
            }
            else
            {
                if (Process == null)
                    Process = Process.Start(Path);
                else
                {
                    if (Process.HasExited)
                        Process = null;
                }
            }

            // todo: send a message to the server that a maintenance will be held soon
            // todo: heartbeat to check if the server is alive????
        }

        public static ServerWatch Create(ServiceConfiguration config, ServerType type)
        {
            return new ServerWatch(config, type);
        }
    }
}
