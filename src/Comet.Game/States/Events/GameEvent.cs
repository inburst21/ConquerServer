// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - GameEvent.cs
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
using Comet.Game.States.BaseEntities;

#endregion

namespace Comet.Game.States.Events
{
    public abstract class GameEvent
    {
        protected enum EventStage
        {
            Idle,
            Running,
            Ending
        }

        public enum EventType
        {
            None,
            TimedGuildWar,
            GuildPk,
            GuildContest
        }

        private TimeOutMS m_eventCheck;

        protected GameEvent(string name, int timeCheck = 1000)
        {
            Name = name;
            m_eventCheck = new TimeOutMS(timeCheck);
        }

        public virtual EventType Identity { get; } = EventType.None;

        public string Name { get; }

        public bool ToNextTime() => m_eventCheck.ToNextTime();

        public virtual bool IsAllowedToJoin(Role sender)
        {
            return true;
        }

        public virtual Task<bool> CreateAsync()
        {
            return Task.FromResult(true);
        }

        public virtual Task OnEnterAsync(Character sender)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnExitAsync(Character sender)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnMoveAsync(Character sender)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnAttackAsync(Character sender)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnReviveAsync(Character sender, bool selfRevive)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnTimerAsync()
        {
            return Task.CompletedTask;
        }
    }
}