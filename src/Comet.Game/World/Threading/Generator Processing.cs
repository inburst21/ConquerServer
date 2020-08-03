﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Generator Processing.cs
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
using System.Threading.Tasks;
using Comet.Game.Database.Repositories;
using Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public sealed class GeneratorProcessor : TimerBase
    {
        private List<Generator> m_generators = new List<Generator>();

        public GeneratorProcessor()
            : base(5000, "Generator Thread")
        {
        }

        public override async Task OnStartAsync()
        {
            // todo make first generation

            foreach (var dbGen in await GeneratorRepository.GetAsync())
            {
                Generator gen = new Generator(dbGen);
                if (gen.CanBeProcessed)
                    m_generators.Add(gen);
            }

            await base.OnStartAsync();
        }

        public override async Task<bool> OnElapseAsync()
        {
            try
            {
                foreach (var gen in m_generators)
                {
                    await gen.GenerateAsync();
                }
            }
            catch (Exception ex)
            {
                await Log.WriteLog(LogLevel.Exception, ex.ToString());
            }
            return true;
        }
    }
}