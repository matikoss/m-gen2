﻿using System;
using System.Collections.Generic;
using System.IO;
using MapEntities;
using UnityEngine;

namespace WyrmsunMapExporting
{
    public class WyrmsunMapExporter
    {
        public void ExportMapToFile(Map map, List<string> playerTypes, string mapName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CreateSmpFile(1, playerTypes, dir, mapName, map);
            CreateSmsFile(map, 1000, 1000, 1000, dir, mapName);
        }

        private void CreateSmpFile(int pNum, List<string> pTypes, string dir, string mapName, Map map)
        {
            var fileName = mapName + ".smp";
            using (StreamWriter file = new StreamWriter(Path.Combine(dir, fileName)))
            {
                file.Write(WyrmsunMapTemplates.HEADER);
                file.Write(WyrmsunMapTemplates.PlayerTypes2P(pTypes.ToArray()));
                file.Write(WyrmsunMapTemplates.PresentMap(mapName, pNum, map.Width, map.Height, 1));
            }
        }

        private void CreateSmsFile(Map map, int startLumber, int startCopper, int startStone, string dir,
            string mapName)
        {
            var fileName = mapName + ".sms";
            using (StreamWriter file = new StreamWriter(Path.Combine(dir, fileName)))
            {
                file.Write(WyrmsunMapTemplates.HEADER);
                file.Write(WyrmsunMapTemplates.PLAYER_HEADER);
                foreach (var player in map.Players)
                {
                    file.Write(WyrmsunMapTemplates.PlayerData(player.ID, player.StartingPosition, startLumber,
                        startCopper, startStone, player.Race, player.Faction, "passive"));
                }

                file.Write(WyrmsunMapTemplates.PlayerData(31, new Vector2Int(0, 0), 0,
                    0, 0, "neutral", "ai-passive"));
                file.Write("\n");
                file.Write(WyrmsunMapTemplates.LoadTileset("scripts/tilesets/conifer_forest_summer.lua"));
                file.Write(WyrmsunMapTemplates.TILES_MAP);
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        var vec = new Vector2Int(x, y);
                        var tile = map.Map1[vec];
                        var resource = 0;
                        string type = WyrmsunTileTypes.GRASS;
                        if (tile.Type == TileType.Tree)
                        {
                            type = WyrmsunTileTypes.PINE_TREE;
                            resource = 400;
                        }
                        else if (tile.Type == TileType.Mountain)
                        {
                            type = WyrmsunTileTypes.ROCK;
                            resource = 10000;
                        }
                        else if (tile.Type == TileType.Water)
                        {
                            type = WyrmsunTileTypes.SHALLOW_WATER;
                            resource = 0;
                        }
                        else
                        {
                            type = WyrmsunTileTypes.GRASS;
                            resource = 0;
                        }

                        file.Write(WyrmsunMapTemplates.GetTerrainTileString(type, x, y, resource, 0));
                    }
                }

                file.Write("\n");
                file.Write(WyrmsunMapTemplates.DEFAULT_HEADER);
                file.Write("\n");
                file.Write(WyrmsunMapTemplates.UNITS_HEADER);
                foreach (var t in map.Map1.Values)
                {
                    if (t.Type == TileType.Copper)
                    {
                        file.Write(WyrmsunMapTemplates.CreateResource(WyrmUnitsTypes.BIG_COPPER, 31, t.Position,
                            50000));
                    }
                }

                foreach (var p in map.Players)
                {
                    if (p.Race == "dwarf")
                    {
                        file.Write(WyrmsunMapTemplates.CreateUnit(WyrmUnitsTypes.DWARF_HALL, p.ID,
                            p.StartingPosition));
                    }
                    else if (p.Race == "goblin")
                    {
                        file.Write(WyrmsunMapTemplates.CreateUnit(WyrmUnitsTypes.GOBLIN_HALL, p.ID,
                            p.StartingPosition));
                    }
                }

                file.Write("\n");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}