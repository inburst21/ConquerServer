using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comet.Game.Database.Repositories;
using Comet.Shared;

namespace Comet.Game.World.Threading
{
    public class WorldProcessing : TimerBase
    {
        private List<Generator> m_generators = new List<Generator>();

        public WorldProcessing()
            : base(500, "World Processing")
        {
        }

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
                foreach (var gen in m_generators)
                {
                    await gen.GenerateAsync();
                }
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
            return m_generators.Where(x => x.MapIdentity == idMap && x.MonsterName.Equals(monsterName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public List<Generator> GetByMonsterType(uint idType)
        {
            return m_generators.Where(x => x.RoleType == idType).ToList();
        }
    }
}
