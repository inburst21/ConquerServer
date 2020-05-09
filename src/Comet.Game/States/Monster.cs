// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Monster.cs
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
using Comet.Game.Database.Models;
using Comet.Game.Packets;
using Comet.Game.States.BaseEntities;
using Comet.Game.World;
using Comet.Network.Packets;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.States
{
    public class Monster : Role
    {
        private readonly Generator m_generator;
        private readonly DbMonstertype m_dbMonster;

        private uint m_idRole = 0;

        public Monster(DbMonstertype type, uint identity, Generator generator)
        {
            m_dbMonster = type;
            m_generator = generator;
            m_idRole = identity;

            m_idMap = generator.MapIdentity;
        }
        
        #region Identity

        public override uint Identity
        {
            get => m_idRole;
            protected set { }
        }

        public override string Name
        {
            get => m_dbMonster.Name;
            set => m_dbMonster.Name = value;
        }

        #endregion

        #region Initialization

        public bool Initialize(uint idMap, ushort x, ushort y)
        {
            m_idMap = idMap;

            if ((Map = Kernel.MapManager.GetMap(idMap)) == null)
                return false;

            m_posX = x;
            m_posY = y;

            Life = MaxLife;

            return true;
        }

        #endregion

        #region Appearence

        public override uint Mesh
        {
            get => m_dbMonster.Lookface;
            set => m_dbMonster.Lookface = (ushort) value;
        }

        #endregion

        public override byte Level
        {
            get => (byte) (m_dbMonster?.Level ?? 0);
            set => m_dbMonster.Level = value;
        }

        public override uint Life
        {
            get;
            set;
        }

        public override uint MaxLife => (uint) (m_dbMonster?.Life ?? 1);

        #region Battle Attributes

        public override int BattlePower => 1;

        public override int MinAttack => m_dbMonster?.AttackMin ?? 0;

        public override int MaxAttack => m_dbMonster?.AttackMax ?? 0;

        public override int MagicAttack => m_dbMonster?.AttackMax ?? 0;

        public override int Defense => m_dbMonster?.Defence ?? 0;

        public override int MagicDefense => m_dbMonster?.MagicDef ?? 0;

        public override int Dodge => m_dbMonster?.AttackMin ?? 0;

        public override int AttackSpeed => m_dbMonster?.AttackSpeed ?? 1000;

        public override int Accuracy => (int) (m_dbMonster?.Dexterity ?? 0);

        #endregion

        #region Map and Movement

        /// <summary>
        ///     The current map identity for the role.
        /// </summary>
        public override uint MapIdentity
        {
            get => m_idMap;
            set => m_idMap = value;
        }

        /// <summary>
        ///     Current X position of the user in the map.
        /// </summary>
        public override ushort MapX
        {
            get => m_posX;
            set => m_posX = value;
        }

        /// <summary>
        ///     Current Y position of the user in the map.
        /// </summary>
        public override ushort MapY
        {
            get => m_posY;
            set => m_posY = value;
        }

        public override void EnterMap()
        {
            Map = Kernel.MapManager.GetMap(MapIdentity);
            Map?.AddAsync(this).Forget();
        }

        public override void LeaveMap()
        {
            Map?.RemoveAsync(Identity);
            IdentityGenerator.Monster.ReturnIdentity(Identity);
            m_generator.Generated--;
            if (Map != null)
            {
                Map.RemoveAsync(Identity).Forget();
                Kernel.RoleManager.RemoveRole(Identity);
            }
            Map = null;
        }

        #endregion

        #region Socket

        public override async Task BroadcastRoomMsgAsync(IPacket msg, bool self)
        {
            if (Map != null)
                await Map.BroadcastRoomMsgAsync(MapX, MapY, msg);
        }

        public override async Task SendSpawnToAsync(Character player)
        {
            await player.SendAsync(new MsgPlayer(this));
        }

        #endregion
    }
}