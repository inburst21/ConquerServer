// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - ServerProcessor.cs
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

#endregion

namespace Comet.Game.World
{
    public class ServerProcessor : BackgroundService
    {
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern Int32 GetCurrentWin32ThreadId();

        protected readonly ProcessThread[] m_Thread;
        protected readonly Task[] m_BackgroundTasks;
        protected readonly Channel<Func<Task>>[] m_Channels;
        protected readonly Partition[] m_Partitions;
        protected CancellationToken m_CancelReads;
        protected CancellationToken m_CancelWrites;

        public readonly int Count;

        public ServerProcessor(int processorCount)
        {
            Count = processorCount;

            m_Thread = new ProcessThread[Count];
            m_BackgroundTasks = new Task[Count];
            m_Channels = new Channel<Func<Task>>[Count];
            m_Partitions = new Partition[Count];
            m_CancelReads = new CancellationToken();
            m_CancelWrites = new CancellationToken();
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            for (int i = 0; i < Count; i++)
            {
                m_Partitions[i] = new Partition { ID = (uint)i, Weight = 0 };
                m_Channels[i] = Channel.CreateUnbounded<Func<Task>>();
                m_BackgroundTasks[i] = DequeueAsync(i, m_Channels[i]);
            }

            return Task.WhenAll(m_BackgroundTasks);
        }

        public void Queue(int partition, Func<Task> task)
        {
            if (!m_CancelWrites.IsCancellationRequested)
            {
                m_Channels[partition].Writer.TryWrite(task);
            }
        }

        public void Clear(int partition)
        {
            if (!m_CancelWrites.IsCancellationRequested)
            {
                
            }
        }

        private async Task DequeueAsync(int partition, Channel<Func<Task>> channel)
        {
            m_Thread[partition] = (from ProcessThread entry in Process.GetCurrentProcess().Threads
                                   where entry.Id == GetCurrentWin32ThreadId()
                                   select entry).First();
            while (!m_CancelReads.IsCancellationRequested)
            {
                var action = await channel.Reader.ReadAsync(m_CancelReads);
                if (action != null)
                {
                    await action.Invoke();
                }
            }
        }

        /// <summary>
        ///     Triggered when the application host is stopping the background task with a
        ///     graceful shutdown. Requests that writes into the channel stop, and then reads
        ///     from the channel stop.
        /// </summary>
        public new async Task StopAsync(CancellationToken cancellationToken)
        {
            m_CancelWrites = new CancellationToken(true);
            foreach (var channel in m_Channels)
                await channel.Reader.Completion;
            m_CancelReads = new CancellationToken(true);
        }

        /// <summary>
        ///     Selects a partition for the client actor based on partition weight. The
        ///     partition with the least popluation will be chosen first. After selecting a
        ///     partition, that partition's weight will be increased by one.
        /// </summary>
        public uint SelectPartition()
        {
            uint partition = m_Partitions.Aggregate((aggr, next) =>
                next.Weight.CompareTo(aggr.Weight) < 0 ? next : aggr).ID;
            Interlocked.Increment(ref m_Partitions[partition].Weight);
            return partition;
        }

        /// <summary>
        ///     Deslects a partition after the client actor disconnects.
        /// </summary>
        /// <param name="partition">The partition id to reduce the weight of</param>
        public void DeselectPartition(uint partition)
        {
            Interlocked.Decrement(ref m_Partitions[partition].Weight);
        }

        protected class Partition
        {
            public uint ID;
            public int Weight;
        }
    }
}