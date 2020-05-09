// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Context.cs
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

using Comet.Game.Database.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Comet.Game.Database
{
    /// <summary>
    ///     Server database client context implemented using Entity Framework Core, an open
    ///     source object-relational mapping framework for ADO.NET. Substitutes in MySQL
    ///     support through a third-party framework provided by Pomelo Foundation.
    /// </summary>
    public class ServerDbContext : DbContext
    {
        // Connection String Configuration
        public static ServerConfiguration.DatabaseConfiguration Configuration;

        // Table Definitions
        public virtual DbSet<DbCharacter> Characters { get; set; }
        public virtual DbSet<DbMap> Maps { get; set; }
        public virtual DbSet<DbItemAddition> ItemAdditions { get; set; }
        public virtual DbSet<DbItem> Items { get; set; }
        public virtual DbSet<DbItemtype> Itemtypes { get; set; }
        public virtual DbSet<DbPointAllot> PointAllot { get; set; }
        public virtual DbSet<DbWeaponSkill> WeaponSkills { get; set; }
        public virtual DbSet<DbPeerage> Peerage { get; set; }
        public virtual DbSet<DbMonstertype> Monstertype { get; set; }
        public virtual DbSet<DbGenerator> Generator { get; set; }
        public virtual DbSet<DbPassway> Passway { get; set; }
        public virtual DbSet<DbPortal> Portal { get; set; }

        /// <summary>
        ///     Configures the database to be used for this context. This method is called
        ///     for each instance of the context that is created. For this project, the MySQL
        ///     connector will be initialized with a connection string from the server's
        ///     configuration file.
        /// </summary>
        /// <param name="options">Builder to create the context</param>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseLazyLoadingProxies(false);
            options.UseMySql($"server={Configuration.Hostname};database={Configuration.Schema};user={Configuration.Username};password={Configuration.Password}");
        }

        /// <summary>
        ///     Typically called only once when the first instance of the context is created.
        ///     Allows for model building before the context is fully initialized, and used
        ///     to initialize composite keys and relationships.
        /// </summary>
        /// <param name="builder">Builder for creating models in the context</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbCharacter>(e => e.HasKey(x => x.Identity));
        }

        /// <summary>
        ///     Tests that the database connection is alive and configured.
        /// </summary>
        public static bool Ping()
        {
            try
            {
                using (ServerDbContext ctx = new ServerDbContext())
                    return ctx.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }
    }
}