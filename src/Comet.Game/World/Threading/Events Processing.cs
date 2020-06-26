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

using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.States.NPCs;
using Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class EventsProcessing : TimerBase
    {
        private TimeOut m_rankingBroadcast = new TimeOut(10);

        public EventsProcessing()
            : base(500, "EventsProcessing")
        {
            m_rankingBroadcast.Update();
        }

        public override async Task<bool> OnElapseAsync()
        {
            bool ranking = m_rankingBroadcast.ToNextTime();

            foreach (var dynaNpc in Kernel.RoleManager.QueryRoleByType<DynamicNpc>())
            {
                if (dynaNpc.IsGoal())
                    continue;

                await dynaNpc.CheckFightTimeAsync();

                if(ranking) 
                    await dynaNpc.BroadcastRankingAsync();
            }

            return true;
        }
    }
}