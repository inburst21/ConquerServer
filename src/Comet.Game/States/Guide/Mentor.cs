// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Mentor.cs
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

namespace Comet.Game.States.Guide
{
    public sealed class Mentor
    {
        private Character m_owner;

        /// <summary>
        /// Used to create new relation.
        /// </summary>
        public Mentor()
        {
            
        }

        /// <summary>
        /// Used to initialize existing relation.
        /// </summary>
        /// <param name="owner"></param>
        public Mentor(Character owner)
        {
            m_owner = owner;
        }

        public async Task<bool> CreateAsync(Character userGuide, Character userStudent)
        {

            return true;
        }

        public async Task<bool> InitializeAsync()
        {
            return true;
        }
    }
}