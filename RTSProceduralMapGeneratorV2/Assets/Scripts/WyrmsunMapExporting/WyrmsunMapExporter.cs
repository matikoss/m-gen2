using System;
using System.Collections.Generic;
using System.IO;
using MapEntities;
using UnityEngine;

namespace WyrmsunMapExporting
{
    public class WyrmsunMapExporter
    {
        private bool IsTestMode;
        private readonly string MAP_DEFAULT_NAME = "MapaTestowa";

        public WyrmsunMapExporter()
        {
            IsTestMode = false;
        }

        public void ExportMapToFile(Map map, string mapName, bool isTestMode)
        {
            this.IsTestMode = isTestMode;
            if (mapName == "")
            {
                mapName = MAP_DEFAULT_NAME;
            }

            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            int startLumber = map.Players[1].StartWood;
            int startCopper = map.Players[1].StartCopper;
            int startStone = map.Players[1].StartStone;
            CreateSmpFile(1, dir, mapName, map);
            CreateSmsFile(map, startLumber, startCopper, startStone, dir, mapName);
        }
        
        public void ExportMapToFile(Map map, string mapName, bool isTestMode, string dirName)
        {
            this.IsTestMode = isTestMode;
            if (mapName == "")
            {
                mapName = MAP_DEFAULT_NAME;
            }

            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + dirName;
            int startLumber = map.Players[1].StartWood;
            int startCopper = map.Players[1].StartCopper;
            int startStone = map.Players[1].StartStone;
            CreateSmpFile(1, dir, mapName, map);
            CreateSmsFile(map, startLumber, startCopper, startStone, dir, mapName);
        }

        private void CreateSmpFile(int pNum, string dir, string mapName, Map map)
        {
            List<string> playerTypes = new List<string>();
            foreach (var player in map.Players)
            {
                if (player.PlayerType == PlayerTypeEnum.Person)
                {
                    playerTypes.Add(WyrmsunMapTemplates.PLAYER_PERSON);
                }
                else
                {
                    playerTypes.Add(WyrmsunMapTemplates.PLAYER_COMPUTER);
                }
            }

            var fileName = mapName + ".smp";
            using (StreamWriter file = new StreamWriter(Path.Combine(dir, fileName)))
            {
                file.Write(WyrmsunMapTemplates.HEADER);
                if (IsTestMode)
                {
                    file.Write(WyrmsunMapTemplates.PlayerTypes3P(playerTypes.ToArray()));
                }
                else
                {
                    file.Write(WyrmsunMapTemplates.PlayerTypes2P(playerTypes.ToArray()));
                }

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
                        startCopper, startStone, player.Race, player.Faction, WyrmAITypes.LAND_ATTACK));
                }

                file.Write(WyrmsunMapTemplates.PlayerData(63, new Vector2Int(0, 0), 0,
                    0, 0, "neutral", WyrmAITypes.PASSIVE));
                file.Write("\n");
                file.Write(WyrmsunMapTemplates.LoadTileset("scripts/tilesets/conifer_forest_summer.lua"));
                file.Write(WyrmsunMapTemplates.TILES_MAP);
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
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
                        file.Write(WyrmsunMapTemplates.CreateResource(WyrmUnitsTypes.BIG_COPPER, 63, t.Position,
                            5000));
                    }
                    else if (t.Type == TileType.SmallCopper)
                    {
                        file.Write(WyrmsunMapTemplates.CreateResource(WyrmUnitsTypes.SMALL_COPPER, 63, t.Position,
                            400));
                    }
                    else if (t.Type == TileType.StonePile)
                    {
                        file.Write(WyrmsunMapTemplates.CreateResource(WyrmUnitsTypes.SMALL_STONE, 63, t.Position,
                            400));
                    }
                    else if (t.Type == TileType.WoodPile)
                    {
                        file.Write(WyrmsunMapTemplates.CreateResource(WyrmUnitsTypes.SMALL_WOOD, 63, t.Position,
                            400));
                    }
                }

                foreach (var p in map.Players)
                {
                    if (IsTestMode && p.PlayerType == PlayerTypeEnum.Person)
                    {
                        file.Write(WyrmsunMapTemplates.CreateUnit(WyrmUnitsTypes.DWARF_WORKER, p.ID,
                            new Vector2Int(p.StartingPosition.x - 5, p.StartingPosition.y - 5)));
                        continue;
                    }

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
                    else if (p.Race == "germanic")
                    {
                        file.Write(WyrmsunMapTemplates.CreateUnit(WyrmUnitsTypes.GERMAN_HALL, p.ID,
                            p.StartingPosition));
                    }
                }

                file.Write("\n\n\n\n");
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