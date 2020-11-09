// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - GuildContest.cs
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
using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.States.BaseEntities;
using Comet.Game.World.Maps;
using Comet.Shared;

#endregion

namespace Comet.Game.States.Events
{
    public sealed class GuildContest : GameEvent
    {
        private const int MAP_ID = 2060;
        private const int MAX_DEATHS_PER_PLAYER = 20;
        private const int MIN_LEVEL = 40;
        private const int MIN_TIME_IN_SYNDICATE = 0; // in days

        private const int STARTUP_TIME = 1220000;
        private const int END_TIME = 4103000;

        private const int MAX_MONEY_REWARD = 60000000;
        private const int MAX_EMONEY_REWARD = 10750;

        private const int MAX_MONEY_REWARD_PER_USER = MAX_MONEY_REWARD / 15;
        private const int MAX_EMONEY_REWARD_PER_USER = MAX_EMONEY_REWARD / 15;

        private TimeOut m_updatePoints = new TimeOut(10);
        private TimeOut m_updateScreens = new TimeOut(10);

        public GuildContest(string name, int timeCheck = 1000)
            : base(name, timeCheck)
        {
        }

        public override EventType Identity { get; } = EventType.GuildContest;

        public GameMap Map { get; private set; }

        public override bool IsAllowedToJoin(Role sender)
        {
            if (!(sender is Character user))
                return false;

            if (sender.Level < MIN_LEVEL)
                return false;

            if (user.SyndicateIdentity == 0)
                return false;

            if ((DateTime.Now - user.SyndicateMember.JoinDate).TotalDays < MIN_TIME_IN_SYNDICATE)
                return false;

            return true;
        }

        public override async Task<bool> CreateAsync()
        {
            Map = Kernel.MapManager.GetMap(MAP_ID);
            if (Map == null)
            {
                await Log.WriteLog(LogLevel.Error, $"Could not start GuildContest, invalid mapid {MAP_ID}");
                return false;
            }
            return true;
        }

        public override Task OnTimerAsync()
        {
            return base.OnTimerAsync();
        }
    }
}