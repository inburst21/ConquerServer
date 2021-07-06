// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Configuration.cs
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

using Microsoft.Extensions.Configuration;

#endregion

namespace Comet.Account.Database
{
    /// <summary>
    ///     Defines the configuration file structure for the Account Server. App Configuration
    ///     files are copied to the build output directory on successful build, containing all
    ///     default configuration settings for the server, only if the file is newer than the
    ///     file bring replaced.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        ///     Instantiates a new instance of <see cref="ServerConfiguration" /> with command-line
        ///     arguments from the user and a configuration file for the application. Builds the
        ///     configuration file and binds to this instance of the ServerConfiguration class.
        /// </summary>
        /// <param name="args">Command-line arguments from the user</param>
        public ServerConfiguration(string[] args)
        {
            new ConfigurationBuilder()
                .AddJsonFile("Comet.Account.config")
                .AddCommandLine(args)
                .Build()
                .Bind(this);
        }

        // Properties and fields
        public DatabaseConfiguration Database { get; set; }
        public NetworkConfiguration Network { get; set; }
        public NetworkConfiguration RealmNetwork { get; set; }

        /// <summary>
        ///     Returns true if the server configuration is valid after reading.
        /// </summary>
        public bool Valid =>
            Database != null &&
            Network != null;

        /// <summary>
        ///     Encapsulates database configuration for Entity Framework.
        /// </summary>
        public class DatabaseConfiguration
        {
            public string Hostname { get; set; }
            public string Schema { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Port { get; set; } = 3306;
        }

        /// <summary>
        ///     Encapsulates network configuration for the server listener.
        /// </summary>
        public class NetworkConfiguration
        {
            public string IPAddress { get; set; }
            public int Port { get; set; }
            public int MaxConn { get; set; }
        }
    }
}