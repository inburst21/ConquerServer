// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Events Processing.cs
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
using System.Linq;
using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.States.Events;
using Comet.Game.States.NPCs;
using Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class EventsProcessing : TimerBase
    {
        private TimeOut m_rankingBroadcast = new TimeOut(10);
        private List<GameEvent> m_events = new List<GameEvent>();

        public EventsProcessing()
            : base(500, "EventsProcessing")
        {
            m_rankingBroadcast.Update();
        }

        public override async Task<bool> OnElapseAsync()
        {
            await Kernel.PigeonManager.OnTimerAsync();

            bool ranking = m_rankingBroadcast.ToNextTime();
            foreach (var dynaNpc in Kernel.RoleManager.QueryRoleByType<DynamicNpc>())
            {
                if (dynaNpc.IsGoal())
                    continue;

                await dynaNpc.CheckFightTimeAsync();

                if(ranking
                    && dynaNpc.MapIdentity != 2058) 
                    await dynaNpc.BroadcastRankingAsync();
            }

            foreach (var @event in m_events)
            {
                if (@event.ToNextTime())
                    await @event.OnTimerAsync();
            }

            return true;
        }

        public override async Task OnStartAsync()
        {
            await RegisterEventAsync(new TimedGuildWar());

            await base.OnStartAsync();
        }

        public async Task<bool> RegisterEventAsync(GameEvent @event)
        {
            if (m_events.Any(x => x.Name.Equals(@event.Name, StringComparison.InvariantCultureIgnoreCase)))
                return false;
            if (await @event.CreateAsync())
            {
                m_events.Add(@event);
                return true;
            }
            return false;
        }

        public T GetEvent<T>() where T : GameEvent
        {
            return m_events.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
        }
    }
}