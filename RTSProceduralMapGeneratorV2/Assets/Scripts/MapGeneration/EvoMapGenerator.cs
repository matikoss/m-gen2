using System.Collections.Generic;
using helpers;
using MapEntities;
using tools;
using UnityEngine;

namespace MapGeneration
{
    public class EvoMapGenerator
    {
        private List<EvoMapWrapper> listOfMaps;
        private float[,] elevationMap;
        private float[,] moistureMap;
        private int seed, width, height;
        private int startPopulationSize;
        private int numberOfPlayers;
        private PlayerRacesData pRaces;
        private ResourcesData startResources;

        public static readonly int RESOURCES_RADIUS = 30;
        public static readonly int SPAWN_BOUND = 6;

        public EvoMapGenerator(int seed, int width, int height, int startPopulationSize, int numberOfPlayers,
            PlayerRacesData races, ResourcesData startResources)
        {
            listOfMaps = new List<EvoMapWrapper>(startPopulationSize);
            this.seed = seed;
            this.width = width;
            this.height = height;
            this.startPopulationSize = startPopulationSize;
            this.numberOfPlayers = numberOfPlayers;
            this.pRaces = races;
            this.startResources = startResources;
            InitNoiseArrays();
            InitMaps();
        }

        public Map FindBest()
        {
            return listOfMaps[0].Map;
        }

        private void InitNoiseArrays()
        {
            this.elevationMap = CreateNoiseArray(seed, width, height, 70.0f, CalculateOctaves(width), 0.5f, 2,
                new Vector2(0, 0));
            this.moistureMap = CreateNoiseArray(seed, width, height, 25.0f, CalculateOctaves(width), 0.5f, 10,
                new Vector2(0, 0));
        }

        private void InitMaps()
        {
            for (int i = 0; i < startPopulationSize; i++)
            {
                float waterParam, mountainParam, treeParam;
                waterParam = GetRandomFloatInRange(0.0f, 0.50f);
                mountainParam = GetRandomFloatInRange(0.50f, 1.0f);
                treeParam = GetRandomFloatInRange(0.0f, 1.0f);
                MapParameters mp = new MapParameters(waterParam, mountainParam, treeParam);

                Vector2Int resourcesStart = GetRandomVector2D(0, width, 0, height);
                Vector2Int oneStart = GetRandomVector2D(0 + SPAWN_BOUND, width - SPAWN_BOUND, 0 + SPAWN_BOUND,
                    height - SPAWN_BOUND);
                Vector2Int twoStart = GetRandomVector2D(0 + SPAWN_BOUND, width - SPAWN_BOUND, 0 + SPAWN_BOUND,
                    height - SPAWN_BOUND);

                EvoMapWrapper eMap = CreateWrappedMap(seed, width, height, mp, resourcesStart, oneStart, twoStart);
                listOfMaps.Add(eMap);
            }
        }

        private EvoMapWrapper CreateWrappedMap(int seed, int width, int height, MapParameters parameters,
            Vector2 resourcesStart, Vector2Int p1Start, Vector2Int p2Start)
        {
            Map map = CreateMapFromParameters(seed, width, height, parameters.WaterParam, parameters.MountainParam,
                parameters.TreeParam);

            PlaceResourcesOnMap(map, CalculateResourcesRadius(), resourcesStart);
            PlacePlayersOnMap(map, p1Start, p2Start);
            EvoMapWrapper wrappedMap = new EvoMapWrapper(map, parameters, resourcesStart);
            return wrappedMap;
        }

        private Map CreateMapFromParameters(int seed, int width, int height, float waterParameter,
            float mountainParameter,
            float treeParameter)
        {
            Map createdMap = new Map(width, height, seed, numberOfPlayers);
            for (int y = 0; y < createdMap.Height; y++)
            {
                for (int x = 0; x < createdMap.Width; x++)
                {
                    if (elevationMap[x, y] <= waterParameter)
                    {
                        createdMap.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Water, new Vector2Int(x, y)));
                    }
                    else if (elevationMap[x, y] >= mountainParameter)
                    {
                        createdMap.Map1.Add(new Vector2Int(x, y),
                            new MapElement(TileType.Mountain, new Vector2Int(x, y)));
                    }
                    else if (elevationMap[x, y] > waterParameter
                             && elevationMap[x, y] < mountainParameter
                             && moistureMap[x, y] > treeParameter)
                    {
                        createdMap.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Tree, new Vector2Int(x, y)));
                    }
                    else
                    {
                        createdMap.Map1.Add(new Vector2Int(x, y), new MapElement(TileType.Empty, new Vector2Int(x, y)));
                    }
                }
            }

            return createdMap;
        }

        private void PlaceResourcesOnMap(Map map, int r, Vector2 start)
        {
            PoissonSampler ps = new PoissonSampler(map.Width, map.Height, r);
            foreach (var sample in ps.GetSamples(start))
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

        private void PlacePlayersOnMap(Map map, Vector2Int p1Start, Vector2Int p2Start)
        {
            List<Player> players = new List<Player>()
            {
                new Player(0, p1Start, pRaces.PlayerOneRace.raceParam,
                    pRaces.PlayerOneRace.GetDefaultFaction(), PlayerTypeEnum.Person, startResources.startWood,
                    startResources.startCopper, startResources.startStone),
                new Player(1, p2Start, pRaces.PlayerTwoRace.raceParam,
                    pRaces.PlayerTwoRace.GetDefaultFaction(), PlayerTypeEnum.Computer, startResources.startWood,
                    startResources.startCopper, startResources.startStone)
            };
            map.Players = players;

            foreach (var p in map.Players)
            {
                if (map.Map1.ContainsKey(p.StartingPosition))
                {
                    map.Map1.Remove(p.StartingPosition);
                }

                map.Map1.Add(p.StartingPosition, new MapElement(TileType.Spawn, p.StartingPosition));
                PreparePlayerSpawn(p.StartingPosition, map);
            }
        }

        private void PreparePlayerSpawn(Vector2Int spawnPosition, Map map)
        {
            for (int i = -6; i <= 6; i++)
            {
                for (int j = -6; j <= 6; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        var v = new Vector2Int(spawnPosition.x + i, spawnPosition.y + j);
                        map.Map1.Remove(v);
                        if (i == 4 && j == 4)
                        {
                            map.Map1.Add(v, new MapElement(TileType.Copper, v));
                        }
                        else if (i > -5 && i < -2 && j > -4 && j < -1)
                        {
                            map.Map1.Add(v, new MapElement(TileType.SmallCopper, v));
                        }
                        else if (i > 1 && i < 4 && j < -2 && j > -6)
                        {
                            map.Map1.Add(v, new MapElement(TileType.StonePile, v));
                        }
                        else if (i > -3 && i < 1 && j > 2 && j < 5)
                        {
                            map.Map1.Add(v, new MapElement(TileType.WoodPile, v));
                        }
                        else
                        {
                            map.Map1.Add(v, new MapElement(TileType.Empty, v));
                        }
                    }
                }
            }
        }

        private float GetRandomFloatInRange(float start, float end)
        {
            return Random.Range(start, end);
        }

        private Vector2Int GetRandomVector2D(int xStart, int xEnd, int yStart, int yEnd)
        {
            return new Vector2Int(Random.Range(xStart, xEnd), Random.Range(yStart, yEnd));
        }

        private float[,] CreateNoiseArray(int seed, int width, int height, float scale, int octaves, float persistance,
            float lacunarity, Vector2 offset)
        {
            NoiseArrayGenerator noiseGenerator = new NoiseArrayGenerator();
            return noiseGenerator.GenerateNoiseArray(width, height, seed, scale, octaves, persistance, lacunarity,
                offset);
        }

        private int CalculateOctaves(int mapSize)
        {
            if (mapSize == 128)
            {
                return 7;
            }
            else
            {
                return 6;
            }
        }

        private int CalculateResourcesRadius()
        {
            int baseRadius = RESOURCES_RADIUS;
            float radiusFull = baseRadius * (width / 128f);
            return Mathf.CeilToInt(radiusFull);
        }
    }
}