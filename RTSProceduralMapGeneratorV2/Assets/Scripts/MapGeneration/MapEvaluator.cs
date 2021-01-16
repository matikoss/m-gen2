using System;
using System.Collections.Generic;
using MapEntities;
using tools;
using UnityEngine;

namespace MapGeneration
{
    public class MapEvaluator
    {
        private float[,] elevationMap;
        private float[,] moistureMap;

        private readonly float PLAYER_DISTANCE_WEIGHT = 1.0f;
        private readonly float RESOURCES_AVG_WEIGHT = 1.2f;
        private readonly float RESOURCES_MIN_MAX_WEIGHT = 1.0f;
        private readonly float HEIGHT_GRADE_WEIGHT = 1.1f;
        private readonly float HUMIDITY_GRADE_WEIGHT = 1.1f;
        private readonly float ABSOLUTE_DISTANCE_WEIGHT = 1.0f;

        public MapEvaluator(float[,] elevationMap, float[,] moistureMap)
        {
            this.elevationMap = elevationMap;
            this.moistureMap = moistureMap;
        }

        public void Evaluate(EvoMapWrapper wrappedMap)
        {
            PrepareMap(wrappedMap);
            int distanceBetweenPlayers = CalculateDistanceBetweenPlayers(wrappedMap);
            bool resourcesAvailability = CheckIfAllResourcesAvailable2(wrappedMap.Map.Players);
            int[] minMaxP1 = CalculateMinMaxDistanceFromResources(wrappedMap, 0);
            int[] minMaxP2 = CalculateMinMaxDistanceFromResources(wrappedMap, 1);
            float heightGrade = 1.0f - (Mathf.Abs(wrappedMap.P1SpawnHeight - wrappedMap.P2SpawnHeight));
            float humidityGrade = 1.0f - (Mathf.Abs(wrappedMap.P1SpawnHumidity - wrappedMap.P2SpawnHumidity));
            float resourcesAvailabilityGrade = 0.0f;
            if (resourcesAvailability)
            {
                resourcesAvailabilityGrade = 1.0f;
            }

            float minMaxGrade = 1.0f - (Mathf.Abs(minMaxP1[0] - minMaxP2[0]) / (wrappedMap.Map.Width * 1.0f) +
                                        Mathf.Abs(minMaxP1[1] - minMaxP2[1]) / (wrappedMap.Map.Width * 1.0f));

            float playerDistanceGrade = 0.0f;
            if (distanceBetweenPlayers != Int32.MaxValue)
            {
                playerDistanceGrade = distanceBetweenPlayers / (wrappedMap.Map.Width * 1.0f);
            }

            if (playerDistanceGrade > 1.0f)
            {
                playerDistanceGrade = 1.0f;
            }

            int absoluteDistance = CalculateAbsoluteDistanceBetweenPlayers(wrappedMap);

            float absoluteDistanceGrade = absoluteDistance / (1.0f * wrappedMap.Map.Width / 2);
            if (absoluteDistanceGrade > 1.0f)
            {
                absoluteDistanceGrade = 1.0f;
            }


            float finalGrade = heightGrade * HEIGHT_GRADE_WEIGHT +
                               humidityGrade * HUMIDITY_GRADE_WEIGHT +
                               resourcesAvailabilityGrade +
                               minMaxGrade * RESOURCES_MIN_MAX_WEIGHT +
                               playerDistanceGrade * PLAYER_DISTANCE_WEIGHT +
                               absoluteDistanceGrade * ABSOLUTE_DISTANCE_WEIGHT;

            if (finalGrade < 0)
            {
                finalGrade = 0;
            }

            wrappedMap.Rating = finalGrade;
        }

        private void PrepareMap(EvoMapWrapper wrappedMap)
        {
            CalculateDistancesOnMap(wrappedMap);
            CalculateAvgDistanceFromRes(wrappedMap);
            AssignSpawnHeights(wrappedMap, elevationMap);
            AssignSpawnHumidity(wrappedMap, moistureMap);
        }

        private void CalculateDistancesOnMap(EvoMapWrapper wrappedMap)
        {
            DijkstraPathfinder dp = new DijkstraPathfinder();
            MapGraph graph = wrappedMap.Map.ToMapGraph();
            foreach (var player in wrappedMap.Map.Players)
            {
                var start = graph.MapNodes.IndexOf(new MapNode(wrappedMap.Map.Map1[player.StartingPosition]));
                MapNode playerSpawn = graph.MapNodes[start];
                var distanceList = dp.dijkstraCalculateDistances(graph, playerSpawn);
                if (player.ID == 0)
                {
                    wrappedMap.CalculatedDistancesForPlayerOne = distanceList;
                }
                else
                {
                    wrappedMap.CalculatedDistancesForPlayerTwo = distanceList;
                }
            }
        }

        private int CalculateAbsoluteDistanceBetweenPlayers(EvoMapWrapper wrappedMap)
        {
            Vector2Int p1Position = wrappedMap.Map.Players[0].StartingPosition;
            Vector2Int p2Position = wrappedMap.Map.Players[1].StartingPosition;
            return (int) Mathf.Sqrt(Mathf.Pow(p2Position.x - p1Position.x, 2) +
                                    Mathf.Pow(p2Position.y - p1Position.y, 2));
        }

        // private void CalculateAvgDistanceFromRes(Map map)
        // {
        //     DijkstraPathfinder dp = new DijkstraPathfinder();
        //     MapGraph tmpGraph = map.ToMapGraph();
        //     foreach (var player in map.Players)
        //     {
        //         var start = tmpGraph.MapNodes.IndexOf(new MapNode(map.Map1[player.StartingPosition]));
        //         MapNode playerSpawn = tmpGraph.MapNodes[start];
        //         var tmpNodesList = dp.dijkstraCalculateDistances(tmpGraph, playerSpawn);
        //         List<MapNode> resources = new List<MapNode>();
        //         foreach (var node in tmpNodesList)
        //         {
        //             if (node.Element.Type == TileType.Copper)
        //             {
        //                 resources.Add(node);
        //             }
        //         }
        //
        //         double sum = 0;
        //         foreach (var res in resources)
        //         {
        //             sum += res.DistanceFromStart;
        //         }
        //
        //         player.AvgDistanceFromResources = sum / resources.Count;
        //         if (player.AvgDistanceFromResources < 0 && player.AvgDistanceFromResources > (map.Height + map.Width))
        //         {
        //             player.AvgDistanceFromResources = double.MinValue;
        //         }
        //     }
        // }

        private int CalculateDistanceBetweenPlayers(EvoMapWrapper wrappedMap)
        {
            List<MapNode> distances = wrappedMap.CalculatedDistancesForPlayerOne1;

            foreach (var mapNode in distances)
            {
                if (mapNode.Element.Type == TileType.Spawn && mapNode.DistanceFromStart != 0)
                {
                    return mapNode.DistanceFromStart;
                }
            }

            return 0;
        }

        private void CalculateAvgDistanceFromRes(EvoMapWrapper wrappedMap)
        {
            foreach (var player in wrappedMap.Map.Players)
            {
                List<MapNode> resources = new List<MapNode>();
                if (player.ID == 0)
                {
                    foreach (var mapNode in wrappedMap.CalculatedDistancesForPlayerOne1)
                    {
                        if (mapNode.Element.Type == TileType.Copper)
                        {
                            resources.Add(mapNode);
                        }
                    }
                }
                else
                {
                    foreach (var mapNode in wrappedMap.CalculatedDistancesForPlayerTwo1)
                    {
                        if (mapNode.Element.Type == TileType.Copper)
                        {
                            resources.Add(mapNode);
                        }
                    }
                }

                double sum = 0;
                foreach (var res in resources)
                {
                    sum += res.DistanceFromStart;
                }

                player.AvgDistanceFromResources = sum / resources.Count;
                if (player.AvgDistanceFromResources < 0 &&
                    player.AvgDistanceFromResources > (wrappedMap.Map.Height + wrappedMap.Map.Width))
                {
                    player.AvgDistanceFromResources = double.MinValue;
                }
            }
        }

        private int[] CalculateMinMaxDistanceFromResources(EvoMapWrapper wrappedMap, int playerId)
        {
            int min = Int32.MaxValue;
            int max = Int32.MinValue;
            List<MapNode> nodesToCheck;
            if (playerId == 0)
            {
                nodesToCheck = wrappedMap.CalculatedDistancesForPlayerOne;
            }
            else
            {
                nodesToCheck = wrappedMap.CalculatedDistancesForPlayerTwo;
            }

            foreach (var node in nodesToCheck)
            {
                if (node.Element.Type == TileType.Copper)
                {
                    if (node.DistanceFromStart < min)
                    {
                        min = node.DistanceFromStart;
                    }

                    if (node.DistanceFromStart > max)
                    {
                        max = node.DistanceFromStart;
                    }
                }
            }

            return new[] {min, max};
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

        private bool CheckIfAllResourcesAvailable2(List<Player> players)
        {
            foreach (var p in players)
            {
                if (p.AvgDistanceFromResources < 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void AssignSpawnHeights(EvoMapWrapper wrappedMap, float[,] heightMap)
        {
            foreach (var player in wrappedMap.Map.Players)
            {
                float height = heightMap[player.StartingPosition.x, player.StartingPosition.y];
                if (player.ID == 0)
                {
                    wrappedMap.P1SpawnHeight = height;
                }
                else
                {
                    wrappedMap.P2SpawnHeight = height;
                }
            }
        }

        private void AssignSpawnHumidity(EvoMapWrapper wrappedMap, float[,] humidityMap)
        {
            foreach (var player in wrappedMap.Map.Players)
            {
                float humidity = humidityMap[player.StartingPosition.x, player.StartingPosition.y];
                if (player.ID == 0)
                {
                    wrappedMap.P1SpawnHumidity = humidity;
                }
                else
                {
                    wrappedMap.P2SpawnHumidity = humidity;
                }
            }
        }
    }
}