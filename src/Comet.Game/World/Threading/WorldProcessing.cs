﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - WorldProcessing.cs
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
using Comet.Game.Database.Repositories;
using Comet.Shared;

#endregion

namespace Comet.Game.World.Threading
{
    public class WorldProcessing : TimerBase
    {
        private List<Generator> m_generators = new List<Generator>();

        public WorldProcessing()
            : base(1000, "World Processing")
        {
        }

        public int ProcessedMonsters { get; private set; }

        public override async Task OnStartAsync()
        {
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
                ProcessedMonsters = 0;
                foreach (var map in Kernel.MapManager.GameMaps.Values)
                {
                    ProcessedMonsters += await map.OnTimerAsync();
                }

                await Kernel.RoleManager.OnRoleTimerAsync();

                foreach (var gen in m_generators) await gen.GenerateAsync();
            }
            catch (Exception ex)
            {
                await Log.WriteLogAsync(LogLevel.Exception, ex.ToString());
            }

            return true;
        }

        public async Task<bool> AddGeneratorAsync(Generator generator)
        {
            try
            {
                m_generators.Add(generator);
            }
            catch (Exception e)
            {
                await Log.WriteLogAsync(LogLevel.Exception, e.ToString());
                return false;
            }

            return true;
        }

        public Generator GetGenerator(uint idGen)
        {
            return m_generators.FirstOrDefault(x => x.Identity == idGen);
        }

        public List<Generator> GetGenerators(uint idMap, string monsterName)
        {
            return m_generators.Where(x =>
                x.MapIdentity == idMap &&
                x.MonsterName.Equals(monsterName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public List<Generator> GetByMonsterType(uint idType)
        {
            return m_generators.Where(x => x.RoleType == idType).ToList();
        }
    }
}