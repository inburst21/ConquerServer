﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Game Npc.cs
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

using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Packets;

namespace Comet.Game.States.NPCs
{
    public sealed class Npc : BaseNpc
    {
        private DbNpc m_dbNpc;

        public Npc(DbNpc npc) 
            : base(npc.Id)
        {
            m_dbNpc = npc;

            m_idMap = npc.Mapid;
            m_posX = npc.Cellx;
            m_posY = npc.Celly;

            Name = npc.Name;
        }

        #region Map and Position



        #endregion

        #region Socket

        public override async Task SendSpawnToAsync(Character player)
        {
            await player.SendAsync(new MsgNpcInfo
            {
                Identity = Identity,
                Lookface = m_dbNpc.Lookface,
                Sort = m_dbNpc.Sort,
                PosX = MapX,
                PosY = MapY,
                Name = Name,
                NpcType = m_dbNpc.Type
            });
        }

        #endregion
    }
}