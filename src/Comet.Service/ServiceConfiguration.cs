// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Service - ServiceConfiguration.cs
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

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Comet.Service
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            new ConfigurationBuilder()
                .AddJsonFile("ServiceConfig.json")
                .Build()
                .Bind(this);
        }

        public SystemConfiguration SystemConfiguration { get; set; }
        public AccountServerConfiguration AccountServer { get; set; }
        public GameServerConfiguration GameServer { get; set; }
    }

    public class SystemConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AccountServerConfiguration
    {
        public string Mutex { get; set; }
        public string Path { get; set; }
        public List<MaintenanceConfiguration> Maintenance { get; set; }
    }

    public class GameServerConfiguration
    {
        public string Mutex { get; set; }
        public string Path { get; set; }
        public List<MaintenanceConfiguration> Maintenance { get; set; }
    }

    public class MaintenanceConfiguration
    {
        public int WeekDay  {  get;  set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}