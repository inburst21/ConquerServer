// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Service - Worker.cs
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Comet.Network.RPC;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#endregion

namespace Comet.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ServiceConfiguration m_config;
        private List<ServerWatch> m_watches = new List<ServerWatch>();
        private RpcServerListener m_rpcServer;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            m_rpcServer = new RpcServerListener(new Remote(logger));

            m_config = new ServiceConfiguration();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            m_watches.Add(ServerWatch.Create(m_config, ServerWatch.ServerType.Account));
            m_watches.Add(ServerWatch.Create(m_config, ServerWatch.ServerType.Game));
            
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var watches in m_watches)
                {
                    await watches.OnTimerAsync();
                }

                await Task.Delay(1000, stoppingToken);
            }

            m_watches.Clear();
        }
    }

    internal class Remote : IRpcServerTarget
    {
        private ILogger<Worker> m_logger;

        public Remote(ILogger<Worker> logger)
        {
            m_logger = logger;
        }

        public void Connected(string agentName)
        {
            m_logger.LogInformation($"{agentName} has connected to the service RPC.");
        }


    }
}