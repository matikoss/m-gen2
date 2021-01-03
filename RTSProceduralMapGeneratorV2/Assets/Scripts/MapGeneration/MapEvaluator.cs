using System;
using System.Collections.Generic;
using MapEntities;
using tools;
using UnityEngine;

namespace MapGeneration
{
    public class MapEvaluator
    {
        private void CalculateAvgDistanceFromRes(Map map)
        {
            DijkstraPathfinder dp = new DijkstraPathfinder();
            MapGraph tmpGraph = map.ToMapGraph();
            foreach (var player in map.Players)
            {
                var start = tmpGraph.MapNodes.IndexOf(new MapNode(map.Map1[player.StartingPosition]));
                MapNode playerSpawn = tmpGraph.MapNodes[start];
                var tmpNodesList = dp.dijkstraCalculateDistances(tmpGraph, playerSpawn);
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
    }
}