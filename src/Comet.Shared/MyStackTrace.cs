// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Shared - MyStackTrace.cs
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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;

#endregion

namespace Comet.Shared
{
    public class MyStackTrace
    {
        public static async Task DoStackTrace()
        {
            var result = new Dictionary<int, string[]>();

            var pid = Process.GetCurrentProcess().Id;

            using (var dataTarget = DataTarget.AttachToProcess(pid, true))
            {
                ClrInfo runtimeInfo = dataTarget.ClrVersions[0];
                var runtime = runtimeInfo.CreateRuntime();

                foreach (var t in runtime.Threads)
                    result.Add(
                        t.ManagedThreadId,
                        t.EnumerateStackTrace().Select(f =>
                        {
                            if (f.Method != null)
                                return $"{f.Thread?.OSThreadId} [{f.Thread?.CurrentAppDomain.Name}] > {f.Method.Type.Name}.{f.Method.Name}";
                            return null;
                        }).ToArray()
                    );
            }

            foreach (var res in result.Where(x => x.Value != null))
            {
                await Log.WriteLogAsync("deadlock", LogLevel.Deadloop, "");
                await Log.WriteLogAsync("deadlock", LogLevel.Deadloop, $"Stack: {res.Key}");
                foreach (var str in res.Value) await Log.WriteLogAsync("deadlock", LogLevel.Deadloop, $"\t{res}");
            }
        }
    }
}