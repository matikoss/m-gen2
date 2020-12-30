using System;
using System.Collections.Generic;
using System.Globalization;
using MapEntities;
using tools;
using UnityEngine;
using WyrmsunMapExporting;

namespace MapGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        private static readonly float WATER_PARAM = 0.25f;
        private static readonly float DIRT_PARAM = 0.27f;
        private static readonly float MOUNTAIN_PARAM = 0.75f;
        private static readonly float TREE_PARAM = 0.6f;

        public NoiseArrayGenerator noiseGenerator;
        public MapVisualizer mapVisualizer;
        private Map map;
        private WyrmsunMapExporter wme;
        private DijkstraPathfinder dp;

        public MapGenerator()
        {
            noiseGenerator = new NoiseArrayGenerator();
            wme = new WyrmsunMapExporter();
            dp = new DijkstraPathfinder();
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void GenerateMap(int seed)
        {
            map = new Map(128, 128, seed, 2);
            GenerateSymmetricMap(MapConstants.MAP_SMALL_SIZE, MapConstants.MAP_SMALL_SIZE, seed, WATER_PARAM,
                MOUNTAIN_PARAM, TREE_PARAM);
            mapVisualizer.DrawMap(map);
            List<string> p2 = new List<string> {"person", "computer"};
            List<string> p3 = new List<string> {"person", "computer", "computer"};
            dijkstraTest(map);
            calculateAvgDistanceFromRes(map, map.Players);
            wme.ExportMapToFile(map, p2, "mapaTestowaZapis");
        }

        private void GenerateAssymetricMap(int width, int height, int seed, float waterParameter,
            float mountainParameter,
            float treeParameter)
        {
            float[,] elevationNoise =
                noiseGenerator.GenerateNoiseArray(width, height, seed, 70.0f, 7, 0.5f, 2, new Vector2(0, 0));
            float[,] moistureNoise =
                noiseGenerator.GenerateNoiseArray(width, height, seed, 25.0f, 7, 0.5f, 10, new Vector2(0, 0));
            AssignMapTileToNoise(elevationNoise, moistureNoise, waterParameter, mountainParameter, treeParameter);
            PlacePlayers();
            CreateSpawnPoints();
            PlaceResources(map, 30);
        }

        private void GenerateSymmetricMap(int width, int height, int seed, float waterParameter,
            float mountainParameter,
            float treeParameter)
        {
            float[,] elevationNoise =
                noiseGenerator.GenerateNoiseArray(width, height, seed, 70.0f, 7, 0.5f, 2, new Vector2(0, 0));
            float[,] moistureNoise =
                noiseGenerator.GenerateNoiseArray(width, height, seed, 25.0f, 7, 0.5f, 10, new Vector2(0, 0));

            AssignMapTileToNoise(elevationNoise, moistureNoise, waterParameter, mountainParameter, treeParameter);

            PlaceResources(map, 30);
            MapElement newElement;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width - y; x++)
                {
                    MapElement current = map.Map1[new Vector2Int(x, y)];
                    if (current.Type == TileType.Water)
                    {
                        newElement = new MapElement(TileType.Water, new Vector2Int(height - 1 - x, width - 1 - y));
                    }
                    else if (current.Type == TileType.Tree)
                    {
                        newElement = new MapElement(TileType.Tree, new Vector2Int(height - 1 - x, width - 1 - y));
                    }
                    else if (current.Type == TileType.Mountain)
                    {
                        newElement = new MapElement(TileType.Mountain, new Vector2Int(height - 1 - x, width - 1 - y));
                    }
                    else if (current.Type == TileType.Copper)
                    {
                        newElement = new MapElement(TileType.Copper, new Vector2Int(height - 1 - x, width - 1 - y));
                    }
                    else
                    {
                        newElement = new MapElement(TileType.Empty, new Vector2Int(height - 1 - x, width - 1 - y));
                    }

                    map.Map1.Remove(new Vector2Int(height - 1 - x, width - 1 - y));
                    map.Map1.Add(new Vector2Int(height - 1 - x, width - 1 - y), newElement);
                }
            }

            PlacePlayersOnSymmetricMap(map);
        }

        private bool PlacePlayersOnSymmetricMap(Map map)
        {
            DijkstraPathfinder dp = new DijkstraPathfinder();
            MapGraph graph = map.ToMapGraph();
            bool placed = false;
            for (int y = 10; y < (map.Height / 2); y++)
            {
                for (int x = 10; x < (map.Width / 2); x++)
                {
                    var startIndex = graph.MapNodes.IndexOf(new MapNode(map.Map1[new Vector2Int(x, y)]));
                    var endIndex =
                        graph.MapNodes.IndexOf(
                            new MapNode(map.Map1[new Vector2Int(map.Height - 1 - x, map.Width - 1 - y)]));
                    if (startIndex == -1 || endIndex == -1)
                    {
                        continue;
                    }

                    MapNode start = graph.MapNodes[startIndex];
                    MapNode end = graph.MapNodes[endIndex];
                    dp.shortesPath(graph, start, end);
                    if (dp.pathDistance != int.MaxValue)
                    {
                        List<Player> players = new List<Player>()
                        {
                            new Player(0, new Vector2Int(x, y), "dwarf", "goldhoof-clan"),
                            new Player(1, new Vector2Int(map.Height - 1 - x, map.Width - 1 - y), "goblin",
                                "dreadskull-tribe")
                        };
                        map.Players = players;
                        foreach (var p in map.Players)
                        {
                            if (map.Map1.ContainsKey(p.StartingPosition))
                            {
                                map.Map1.Remove(p.StartingPosition);
                            }

                            map.Map1.Add(p.StartingPosition, new MapElement(TileType.Spawn, p.StartingPosition));
                            CreatePlayerSpawn(p.StartingPosition, map);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void PlacePlayers()
        {
            List<Player> players = new List<Player>()
            {
                new Player(0, new Vector2Int(20, 20), "dwarf", "goldhoof-clan"),
                new Player(1, new Vector2Int(100, 100), "goblin", "dreadskull-tribe")
            };
            map.Players = players;
            foreach (var p in map.Players)
            {
                if (map.Map1.ContainsKey(p.StartingPosition))
                {
                    map.Map1.Remove(p.StartingPosition);
                }

                map.Map1.Add(p.StartingPosition, new MapElement(TileType.Spawn, p.StartingPosition));
            }
        }

        private void CreateSpawnPoints()
        {
            foreach (var p in map.Players)
            {
                for (int i = -2; i <= 2; i++)
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            var v = new Vector2Int(p.StartingPosition.x + i, p.StartingPosition.y + j);
                            map.Map1.Remove(v);
                            if (i == 2 && j == 1)
                            {
                                map.Map1.Add(v, new MapElement(TileType.Copper, v));
                            }
                            else
                            {
                                map.Map1.Add(v, new MapElement(TileType.Empty, v));
                            }
                        }
                    }
                }
            }
        }

        private void CreatePlayerSpawn(Vector2Int spawnPosition, Map map)
        {
            for (int i = -5; i <= 5; i++)
            {
                for (int j = -5; j <= 5; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        var v = new Vector2Int(spawnPosition.x + i, spawnPosition.y + j);
                        map.Map1.Remove(v);
                        if (i == 4 && j == 4)
                        {
                            map.Map1.Add(v, new MapElement(TileType.Copper, v));
                        }
                        else
                        {
                            map.Map1.Add(v, new MapElement(TileType.Empty, v));
                        }
                    }
                }
            }
        }

        private void PlaceResources(Map map, int r)
        {
            PoissonSampler ps = new PoissonSampler(map.Width, map.Height, r);
            foreach (var sample in ps.GetSamples())
            {
                var tile = map.Map1[sample];
                if (tile.Type == TileType.Empty)
                {
                    map.Map1.Remove(sample);
                    map.Map1.Add(sample, new MapElement(TileType.Copper, sample));
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                continue;
                            }

                            Vector2Int v = new Vector2Int(sample.x + i, sample.y + j);
                            if ((v.x >= 0 && v.x < map.Width) && (v.y >= 0 && v.y < map.Height))
                            {
                                map.Map1.Remove(v);
                                map.Map1.Add(v, new MapElement(TileType.Empty, v));
                            }
                        }
                    }
                }
            }
        }

        private void dijkstraTest(Map map)
        {
            MapGraph graph = map.ToMapGraph();
            Vector2Int p1 = map.Players[0].StartingPosition;
            Vector2Int p2 = map.Players[1].StartingPosition;
            var startIndex = graph.MapNodes.IndexOf(new MapNode(map.Map1[p1]));
            var endIndex = graph.MapNodes.IndexOf(new MapNode(map.Map1[p2]));
            MapNode start = graph.MapNodes[startIndex];
            MapNode end = graph.MapNodes[endIndex];
            dp.shortesPath(graph, start, end);
            Debug.Log(dp.pathDistance);
        }

        private void calculateAvgDistanceFromRes(Map map, List<Player> players)
        {
            MapGraph tmpGraph = map.ToMapGraph();
            foreach (var player in players)
            {
                List<MapNode> tmpNodesList = new List<MapNode>();
                var start = tmpGraph.MapNodes.IndexOf(new MapNode(map.Map1[player.StartingPosition]));
                MapNode playerSpawn = tmpGraph.MapNodes[start];
                tmpNodesList = dp.dijkstraCalculateDistances(tmpGraph, playerSpawn);
                List<MapNode> resources = new List<MapNode>();
                foreach (var node in tmpNodesList)
                {
                    if (node.Element.Type == TileType.Copper)
                    {
                        resources.Add(node);
                    }
                }

                double sum = 0;
                foreach (var res in resources)
                {
                    sum += res.DistanceFromStart;
                }

                player.AvgDistanceFromResources = sum / resources.Count;
                if (player.AvgDistanceFromResources < 0 && player.AvgDistanceFromResources > (map.Height + map.Width))
                {
                    player.AvgDistanceFromResources = double.MinValue;
                }

                Debug.Log(player.AvgDistanceFromResources);
            }
        }

        private bool CheckIfAllResourcesAvailable(List<Player> players)
        {
            foreach (var p in players)
            {
                if (Math.Abs(p.AvgDistanceFromResources - Double.MinValue) < 0.1)
                {
                    return false;
                }
            }

            return true;
        }

        private void AssignMapTileToNoise(float[,] elevationNoise, float[,] moistureNoise,
            float waterParameter, float mountainParameter, float treeParameter)
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (elevationNoise[x, y] <= waterParameter)
                    {
                        map.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Water, new Vector2Int(x, y)));
                    }
                    else if (elevationNoise[x, y] >= mountainParameter)
                    {
                        map.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Mountain, new Vector2Int(x, y)));
                    }
                    else if (elevationNoise[x, y] > waterParameter
                             && elevationNoise[x, y] < mountainParameter
                             && moistureNoise[x, y] > treeParameter)
                    {
                        map.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Tree, new Vector2Int(x, y)));
                    }
                    else
                    {
                        map.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Empty, new Vector2Int(x, y)));
                    }
                }
            }
        }
    }
}