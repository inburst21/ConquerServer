// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Map Manager.cs
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.World.Maps;
using Comet.Shared;
using Microsoft.VisualStudio.Threading;

#endregion

namespace Comet.Game.World.Managers
{
    public sealed class MapManager
    {
        private readonly ConcurrentDictionary<uint, GameMapData> m_mapData =
            new ConcurrentDictionary<uint, GameMapData>();

        private readonly ConcurrentDictionary<uint, GameMap> m_maps = new ConcurrentDictionary<uint, GameMap>();

        public void LoadData()
        {
            var stream = File.OpenRead(@".\ini\GameMap.dat");
            BinaryReader reader = new BinaryReader(stream);

            int mapDataCount = reader.ReadInt32();
            _ = Log.WriteLog(LogLevel.Debug, $"Loading {mapDataCount} maps...");

            for (int i = 0; i < mapDataCount; i++)
            {
                uint idMap = reader.ReadUInt32();
                int length = reader.ReadInt32();
                string name = new string(reader.ReadChars(length));
                uint puzzle = reader.ReadUInt32();

                GameMapData mapData = new GameMapData(idMap);
                mapData.Load(name);

                _ = Log.WriteLog(LogLevel.Debug, $"Map [{idMap}] loaded...");
                m_mapData.TryAdd(idMap, mapData);
            }

            reader.Close();
            stream.Close();
            reader.Dispose();
            stream.Dispose();
        }

        public async Task LoadMaps()
        {
            var jtf = new JoinableTaskFactory(new JoinableTaskContext());
            List<DbMap> maps = jtf.Run(MapsRepository.GetAsync);
            foreach (var dbmap in maps)
            {
                GameMap map = new GameMap(dbmap);
                if (await map.Initialize())
                    m_maps.TryAdd(map.Identity, map);
            }
        }

        public GameMap GetMap(uint idMap)
        {
            return m_maps.TryGetValue(idMap, out var value) ? value : null;
        }

        public GameMapData GetMapData(uint idDoc)
        {
            return m_mapData.TryGetValue(idDoc, out var map) ? map : null;
        }

        public ConcurrentDictionary<uint, GameMap> GameMaps => m_maps;
    }
}