// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Shared - Log.cs
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace Comet.Shared
{
    public enum LogLevel
    {
        Message,
        Debug,
        Warning,
        Error,
        Exception,
        Deadloop
    }

    internal enum LogFolder
    {
        SystemLog,
        GameLog
    }

    internal struct LogFile
    {
        public LogFolder Folder;
        public string Path;
        public string Filename;
        public DateTime Date;
        public object Handle;
    }

    public static class Log
    {
        private static readonly Dictionary<string, LogFile> Files = new Dictionary<string, LogFile>();

        public static string DefaultFileName = "Server";

        static Log()
        {
            RefreshFolders();
        }

        public static async Task WriteLog(LogLevel level, string message, params object[] values)
        {
            await WriteLog(DefaultFileName, level, message, values);
        }

        public static async Task WriteLog(string file, LogLevel level, string message, params object[] values)
        {
            RefreshFolders();

            DateTime now = DateTime.Now;
            message = string.Format(message, values);
            message = $"{DateTime.Now:HH:mm:ss.fff} [{level.ToString()}] - {message}";

            await WriteToFile(file, LogFolder.SystemLog, message);

            switch (level)
            {
                case LogLevel.Message:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Exception:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
            }

            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static async Task GmLog(string file, string message, params object[] values)
        {
            RefreshFolders();

            if (values.Length > 0)
                message = string.Format(message, values);

            message = $"{DateTime.Now:HHmmss.fff} - {message}";

            await WriteToFile(file, LogFolder.GameLog, message);
        }

        private static async Task WriteToFile(string file, LogFolder eFolder, string value)
        {
            DateTime now = DateTime.Now;
            if (!Files.TryGetValue(file, out var fileHandle))
                Files.Add(file, fileHandle = CreateHandle(file, eFolder));

            if (fileHandle.Date.Year != now.Year
                || fileHandle.Date.DayOfYear != now.DayOfYear)
            {
                fileHandle.Date = now;
                fileHandle.Path =
                    $".\\{LogFolder.SystemLog.ToString()}\\{now.Year:0000}\\{now.Month:00}\\{now.Day:00}\\{file}.log";
            }

            using StreamWriter writer = new StreamWriter(fileHandle.Path, true);
            await writer.WriteLineAsync(value);
        }

        private static LogFile CreateHandle(string file, LogFolder folder)
        {
            if (Files.ContainsKey(file))
                return Files[file];

            DateTime now = DateTime.Now;
            LogFile logFile = new LogFile
            {
                Date = now,
                Filename = $"{now:YYYYMMdd)} - {file}.log",
                Handle = new object(),
                Path = $".\\{folder.ToString()}\\{now.Year:0000}\\{now.Month:00}\\{now.Day:00}\\{file}.log",
                Folder = folder
            };

            return logFile;
        }

        private static void RefreshFolders()
        {
            DateTime now = DateTime.Now;
            foreach (var eVal in Enum.GetValues(typeof(LogFolder)).Cast<LogFolder>())
            {
                if (!Directory.Exists($".\\{eVal.ToString()}\\{now.Year:0000}\\{now.Month:00}\\{now.Day:00}"))
                    Directory.CreateDirectory($".\\{eVal.ToString()}\\{now.Year:0000}\\{now.Month:00}\\{now.Day:00}");
            }
        }
    }
}