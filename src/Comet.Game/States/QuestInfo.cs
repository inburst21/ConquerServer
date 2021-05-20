using Comet.Shared;
using Microsoft.Extensions.Configuration.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comet.Game.States
{
    public sealed class QuestInfo
    {
        private static Dictionary<int, string> m_questInfoType = new();
        private static Dictionary<int, QuestInfo> m_questInfo = new();

        public static async Task InitializeAsync()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "ini", "QuestInfo.ini");
            if (!File.Exists(path))
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"File '{path}' is missing");
                return;
            }

            IniConfigurationSource source = new();
            source.Path = path;
            source.ReloadOnChange = false;
            IniConfigurationProvider reader = new(source);
            try
            {
                reader.Load(new FileStream(path, FileMode.Open, FileAccess.Read));
            }
            catch
            {
                await Log.WriteLogAsync(LogLevel.Error, $"An error ocurred while reading '{path}'. Check for duplicated values or parsing errors! Startup will continue.");
                return;
            }

            if (reader.TryGet("TaskType:Num", out var strTaskTypeNum) 
                && int.TryParse(strTaskTypeNum, out var taskTypeNum))
            {
                for (int i = 1; i <= taskTypeNum; i++)
                {
                    if (!reader.TryGet($"TaskType:TypeId{i}", out var strId)
                        || !int.TryParse(strId, out var id)
                        || !reader.TryGet($"TaskType:TypeName{i}", out var name))
                        continue;

                    if (m_questInfoType.ContainsKey(id))
                        continue;

                    m_questInfoType.Add(id, name);
                }
            }

            if (!reader.TryGet("TotalMission:TotalMission", out var strTotalMission)
                || !int.TryParse(strTotalMission, out var totalMission))
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"No total mission found.");
                return;
            }

            for (int i = 1; i <= totalMission; i++)
            {
                QuestInfo questInfo = new();

            }
        }

        public int TypeId { get; set; }
        public string TaskNameColor { get; set; }
        public int CompleteFlag { get; set; }
        public int ActivityType { get; set; }
        public int MissionId { get; set; }
        public string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public bool Auto { get; set; }
        public bool First { get; set; }
        public int[] PreQuest { get; set; }
        public uint MapId { get; set; }
        public int[] Profession { get; set; }
        public int Sex { get; set; }
        public int FinishTime { get; set; }
        public int ActivityBeginTime { get; set; }
        public int ActivityEndTime { get; set; }
        public NpcInfo BeginNpc { get; set; }
        public NpcInfo EndNpc { get; set; }
        public string Prize { get; set; }
        public string IntentionDesp { get; set; }
        public string IntentAmount { get; set; }
        public string[] Intent { get; set; }
        public string Content { get; set; }

        public struct NpcInfo
        {
            public uint Id;
            public uint Map;
            public ushort X;
            public ushort Y;
            public string Name;
            public string MapName;
        }
    }    
}