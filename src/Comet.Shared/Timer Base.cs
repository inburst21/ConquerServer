// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Core - TimerBase.cs
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

using System;
using System.Threading.Tasks;
using System.Timers;
using Comet.Shared;

namespace Comet.Core
{
    public abstract class TimerBase
    {
        private Timer m_timer;
        private string m_name;
        protected int m_interval = 1000;

        protected TimerBase(int intervalMs, string name)
        {
            m_name = name;
            m_interval = intervalMs;

            m_timer = new Timer
            {
                Interval = intervalMs,
                AutoReset = false
            };
            m_timer.Elapsed += TimerOnElapse;
            m_timer.Disposed += TimerOnDisposed;
        }

        public string Name => m_name;

        public bool StopOnException { get; set; }

        public async Task StartAsync()
        {
            await OnStartAsync();

            m_timer.Start();
        }

        private async void TimerOnDisposed(object sender, EventArgs e)
        {
            await OnCloseAsync();
        }

        private async void TimerOnElapse(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (await OnElapseAsync())
                    m_timer.Start();
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Error, $"Error on thread {m_name}");
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
                if (!StopOnException)
                    m_timer.Start();
            }
        }

        public virtual async Task OnStartAsync()
        {
            await Log.WriteLog(LogLevel.Message, $"Timer [{m_name}] has started");
        }

        public virtual async Task<bool> OnElapseAsync()
        {
            await Log.WriteLog(LogLevel.Message, $"Timer [{m_name}] has elapsed at {DateTime.Now}");
            return true;
        }

        public virtual async Task OnCloseAsync()
        {
            await Log.WriteLog(LogLevel.Message, $"Timer [{m_name}] has finished");
        }
    }
}