// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Game Map Data.cs
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
using System.IO;
using System.Text;
using Comet.Shared;

#endregion

namespace Comet.Game.World.Maps
{
    public class GameMapData
    {
        public const int MAP_NONE = 0;
        public const int MAP_TERRAIN = 1;
        public const int MAP_TERRAIN_PART = 2;
        public const int MAP_SCENE = 3;
        public const int MAP_COVER = 4;
        public const int MAP_ROLE = 5;
        public const int MAP_HERO = 6;
        public const int MAP_PLAYER = 7;
        public const int MAP_PUZZLE = 8;
        public const int MAP_3DSIMPLE = 9;
        public const int MAP_3DEFFECT = 10;
        public const int MAP_2DITEM = 11;
        public const int MAP_3DNPC = 12;
        public const int MAP_3DOBJ = 13;
        public const int MAP_3DTRACE = 14;
        public const int MAP_SOUND = 15;
        public const int MAP_2DREGION = 16;
        public const int MAP_3DMAGICMAPITEM = 17;
        public const int MAP_3DITEM = 18;
        public const int MAP_3DEFFECTNEW = 19;

        private readonly uint m_idDoc;

        private readonly List<PassageData> m_passageData = new List<PassageData>();
        private Tile[,] m_cell;

        public GameMapData(uint idMapDoc)
        {
            m_idDoc = idMapDoc;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Tile this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    return default;

                return m_cell[x, y];
            }
        }

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                var stream = File.OpenRead(path);
                BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);

                LoadData(reader);
                LoadPassageData(reader);
                LoadLayerData(reader);

                reader.Close();
                reader.Dispose();
            }
            else
            {
                Log.WriteLog(LogLevel.Warning, $"Map data for file {m_idDoc} '{path}' has not been found.").Wait();
            }
        }

        private void LoadData(BinaryReader reader)
        {
            uint version = reader.ReadUInt32();
            uint data = reader.ReadUInt32();
            reader.ReadBytes(260); // jump ??? why
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            m_cell = new Tile[Width, Height];
            for (int y = 0; y < Height; y++)
            {
                uint checkSum = 0, tmp = 0;
                for (int x = 0; x < Width; x++)
                {
                    short access = reader.ReadInt16();
                    short surface = reader.ReadInt16();
                    short elevation = reader.ReadInt16();

                    checkSum += (uint) ((uint) access * (surface + y + 1) +
                                        (elevation + 2) * (x + 1 + surface));

                    m_cell[x, y] = new Tile(elevation, access, surface);
                }

                tmp = reader.ReadUInt32();
                if (checkSum != tmp)
                {
                    Log.WriteLog(LogLevel.Error, $"Invalid checksum for block of cells (mapdata: {m_idDoc}), y: {y}")
                        .Wait();
                }
            }
        }

        private void LoadPassageData(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                int index = reader.ReadInt32();

                m_passageData.Add(new PassageData(x, y, index));
            }
        }

        private void LoadLayerData(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                int type = reader.ReadInt32();

                switch (type)
                {
                    case MAP_COVER:
                        reader.ReadChars(416);
                        break;

                    case MAP_TERRAIN:
                        string file = new string(reader.ReadChars(260));
                        int startX = reader.ReadInt32();
                        int startY = reader.ReadInt32();

                        if (File.Exists(file))
                        {
                            var memory = new MemoryStream(File.ReadAllBytes(file));
                            var scenery = new BinaryReader(memory);

                            memory.Seek(332, SeekOrigin.Current);
                            int width = scenery.ReadInt32();
                            int height = scenery.ReadInt32();
                            memory.Seek(4, SeekOrigin.Current);
                            int sceneOffsetX = scenery.ReadInt32();
                            int sceneOffsetY = scenery.ReadInt32();
                            memory.Seek(4, SeekOrigin.Current);

                            for (int y = 0; y < height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    short mask = (short) scenery.ReadInt32();
                                    short terrain = (short) scenery.ReadInt32();
                                    short altitude = (short) scenery.ReadInt32();

                                    int posX = startX + sceneOffsetX + x - width;
                                    int posY = startY + sceneOffsetY + y - height;

                                    m_cell[x, y] = new Tile(altitude, mask, terrain);
                                }
                            }

                            memory.Close();
                            scenery.Close();
                            memory.Dispose();
                            scenery.Dispose();
                        }

                        break;

                    case MAP_SOUND:
                        reader.ReadChars(276);
                        break;

                    case MAP_3DEFFECT:
                        reader.ReadChars(72);
                        break;

                    case MAP_3DEFFECTNEW:
                        reader.ReadChars(276);
                        break;
                }
            }
        }
    }

    public struct PassageData
    {
        public PassageData(int x, int y, int index)
        {
            X = x;
            Y = y;
            Index = index;
        }

        public int X;
        public int Y;
        public int Index;
    }
}