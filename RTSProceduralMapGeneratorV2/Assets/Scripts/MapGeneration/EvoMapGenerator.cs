using System.Collections.Generic;
using System.Linq;
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

        public static readonly float MUTATION_RATE = 0.03f;
        public static readonly float CROSSOVER_RATE = 0.9f;

        public static readonly int AFTER_SELECTION_SIZE = 25;
        public static readonly float EVO_END_GRADE = 7.3f;
        public static readonly int EVO_NUMBER = 5;

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

        public EvoMapWrapper FindBest()
        {
            EvoGeneration();
            Debug.Log("Avg distance grade: " + listOfMaps[0].AvgResRating);
            Debug.Log("Height grade: " + listOfMaps[0].HeightRating);
            Debug.Log("Humidity grade: " + listOfMaps[0].HumidityRating);
            Debug.Log("Min max resources grade: " + listOfMaps[0].MINMaxRating);
            Debug.Log("Second resource distance grade " + listOfMaps[0].SecondResourceRating);
            Debug.Log("Player distance grade: " + listOfMaps[0].PDistanceRating);
            Debug.Log("Absolute distance grade: " + listOfMaps[0].AbsDistRating);
            Debug.Log("Resources availability grade: " + listOfMaps[0].ResAvailRating);
            Debug.Log("Final grade:" + listOfMaps[0].Rating);
            return listOfMaps[0];
        }

        public void EvoGeneration()
        {
            MapEvaluator mapEvaluator = new MapEvaluator(elevationMap, moistureMap);
            FirstRun(mapEvaluator);
            Debug.Log(listOfMaps[0].Rating);
            int evoCount = 0;
            while (listOfMaps[0].Rating < EVO_END_GRADE && evoCount < EVO_NUMBER)
            {
                SelectFirstNBest(AFTER_SELECTION_SIZE);
                CrossoverPhase();
                MutationPhase();
                foreach (var wMap in listOfMaps)
                {
                    if (wMap.Rating == 0)
                    {
                        mapEvaluator.Evaluate(wMap);
                    }
                }

                listOfMaps.Sort();
                listOfMaps.Reverse();
                evoCount++;
                Debug.Log(listOfMaps[0].Rating);
            }
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
                treeParam = GetRandomFloatInRange(0.2f, 0.8f);
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

        private void FirstRun(MapEvaluator mapEvaluator)
        {
            foreach (var wMap in listOfMaps)
            {
                mapEvaluator.Evaluate(wMap);
            }

            listOfMaps.Sort();
            listOfMaps.Reverse();
        }

        private void SelectFirstNBest(int n)
        {
            List<EvoMapWrapper> newGeneration = new List<EvoMapWrapper>();
            for (int i = 0; i < n; i++)
            {
                newGeneration.Add(listOfMaps[i]);
            }

            listOfMaps = newGeneration;
        }

        private void CrossoverPhase()
        {
            List<EvoMapWrapper> children = new List<EvoMapWrapper>();
            while (children.Count < (startPopulationSize - AFTER_SELECTION_SIZE))
            {
                int parent1 = Random.Range(0, listOfMaps.Count - 1);
                int parent2 = Random.Range(0, listOfMaps.Count - 1);
                while (parent1 == parent2)
                {
                    parent2 = Random.Range(0, listOfMaps.Count - 1);
                }

                EvoMapWrapper parent1Map = listOfMaps[parent1];
                EvoMapWrapper parent2Map = listOfMaps[parent2];
                CrossOver(parent1Map, parent2Map, children);
            }

            listOfMaps.AddRange(children);
        }

        private void MutationPhase()
        {
            foreach (var map in listOfMaps)
            {
                Mutate(map);
            }
        }

        private void CrossOver(EvoMapWrapper map1, EvoMapWrapper map2, List<EvoMapWrapper> childrenList)
        {
            if (YesNoFromRatio(CROSSOVER_RATE))
            {
                int crossOverBound = Random.Range(1, 6);
                MapParameters mapParameters = map1.Parameters;
                Vector2 resourcesStart = map1.ResourcesStart;
                Vector2Int p1Spawn = map1.Map.Players[0].StartingPosition;
                Vector2Int p2Spawn = map1.Map.Players[1].StartingPosition;
                if (crossOverBound == 1)
                {
                    mapParameters.MountainParam = map2.Parameters.WaterParam;
                    mapParameters.TreeParam = map2.Parameters.TreeParam;
                    resourcesStart = map2.ResourcesStart;
                    p1Spawn = map2.Map.Players[0].StartingPosition;
                    p2Spawn = map2.Map.Players[1].StartingPosition;
                }
                else if (crossOverBound == 2)
                {
                    mapParameters.TreeParam = map2.Parameters.TreeParam;
                    resourcesStart = map2.ResourcesStart;
                    p1Spawn = map2.Map.Players[0].StartingPosition;
                    p2Spawn = map2.Map.Players[1].StartingPosition;
                }
                else if (crossOverBound == 3)
                {
                    resourcesStart = map2.ResourcesStart;
                    p1Spawn = map2.Map.Players[0].StartingPosition;
                    p2Spawn = map2.Map.Players[1].StartingPosition;
                }
                else if (crossOverBound == 4)
                {
                    p1Spawn = map2.Map.Players[0].StartingPosition;
                    p2Spawn = map2.Map.Players[1].StartingPosition;
                }
                else if (crossOverBound == 5)
                {
                    p2Spawn = map2.Map.Players[1].StartingPosition;
                }

                EvoMapWrapper childMap =
                    CreateWrappedMap(seed, width, height, mapParameters, resourcesStart, p1Spawn, p2Spawn);
                childrenList.Add(childMap);
            }
        }

        private void Mutate(EvoMapWrapper mapToMutate)
        {
            if (YesNoFromRatio(MUTATION_RATE))
            {
                mapToMutate.Rating = 0.0f;
                MapParameters mapParameters = mapToMutate.Parameters;
                Vector2 resourcesStart = mapToMutate.ResourcesStart;
                Vector2Int p1Spawn = mapToMutate.Map.Players[0].StartingPosition;
                Vector2Int p2Spawn = mapToMutate.Map.Players[1].StartingPosition;

                int mutationTarget = Random.Range(1, 6);
                if (mutationTarget == 1)
                {
                    mapParameters.WaterParam = GetRandomFloatInRange(0.0f, 0.50f);
                }
                else if (mutationTarget == 2)
                {
                    mapParameters.MountainParam = GetRandomFloatInRange(0.50f, 1.00f);
                }
                else if (mutationTarget == 3)
                {
                    mapParameters.TreeParam = GetRandomFloatInRange(0.0f, 1.0f);
                }
                else if (mutationTarget == 4)
                {
                    Vector2Int newResourcesStart =
                        GetRandomVector2D(0, mapToMutate.Map.Width, 0, mapToMutate.Map.Width);
                    resourcesStart = newResourcesStart;
                }
                else if (mutationTarget == 5)
                {
                    p1Spawn = GetRandomVector2D(0 + SPAWN_BOUND, width - SPAWN_BOUND, 0 + SPAWN_BOUND,
                        height - SPAWN_BOUND);
                }
                else if (mutationTarget == 6)
                {
                    p2Spawn = GetRandomVector2D(0 + SPAWN_BOUND, width - SPAWN_BOUND, 0 + SPAWN_BOUND,
                        height - SPAWN_BOUND);
                }

                mapToMutate = CreateWrappedMap(seed, width, height, mapParameters, resourcesStart, p1Spawn, p2Spawn);
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
            float radiusFull = baseRadius * (width / MapConstants.MAX_MAP_SIZE);
            return Mathf.CeilToInt(radiusFull);
        }

        private bool YesNoFromRatio(float rate)
        {
            int percent = (int) (100 * rate);
            return Random.Range(0, 100) < percent;
        }
    }
}