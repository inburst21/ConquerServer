// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - WorldProcessor.cs
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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#endregion

namespace Comet.Game.World
{
    public sealed class WorldProcessor : ServerProcessor
    {
        public WorldProcessor(int processorCount)
            : base(processorCount)
        {
        }

        protected override async Task DequeueAsync(int partition, Channel<Func<Task>> channel)
        {
            m_Thread[partition] = (from ProcessThread entry in Process.GetCurrentProcess().Threads
                where entry.Id == GetCurrentWin32ThreadId()
                select entry).First();

            while (!m_CancelReads.IsCancellationRequested)
            {
                await Kernel.GeneratorManager.OnTimerAsync(partition);
                await Task.Delay(1000, m_CancelReads);
            }
        }
    }
}