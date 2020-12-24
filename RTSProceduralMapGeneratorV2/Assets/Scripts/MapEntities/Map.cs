using System;
using System.Collections;
using System.Collections.Generic;
using tools;
using UnityEngine;

namespace MapEntities
{
    public class Map
    {
        private Dictionary<Vector2Int, MapElement> map;
        private readonly int width, height;
        private int seed;
        private readonly int numberOfPlayers;
        private List<Player> players;

        public Map(int width, int height, int seed, int numberOfPlayers)
        {
            this.width = width;
            this.height = height;
            this.seed = seed;
            this.numberOfPlayers = numberOfPlayers;
            map = new Dictionary<Vector2Int, MapElement>();
            players = new List<Player>();
        }

        public Dictionary<Vector2Int, MapElement> Map1
        {
            get => map;
            set => map = value;
        }

        public int Width => width;

        public int Height => height;

        public int Seed
        {
            get => seed;
            set => seed = value;
        }

        public int NumberOfPlayers => numberOfPlayers;

        public List<Player> Players
        {
            get => players;
            set => players = value;
        }

        public MapGraph ToMapGraph()
        {
            if (map == null)
            {
                return null;
            }

            List<MapNode> nodes = new List<MapNode>();
            List<MapEdge> edges = new List<MapEdge>();

            Dictionary<Vector2Int, MapNode> tmpNodesDict = new Dictionary<Vector2Int, MapNode>();
            Vector2Int vecTmp;
            MapElement elemTmp;
            MapNode startNodeTmp;
            MapNode endNodeTmp;
            List<MapEdge> nodeEdgesContainer;

            foreach (MapElement element in map.Values)
            {
                if (element.Type == TileType.Empty || element.Type == TileType.Copper || element.Type == TileType.Spawn)
                {
                    startNodeTmp = new MapNode(element);
                    nodes.Add(startNodeTmp);
                    tmpNodesDict.Add(startNodeTmp.Element.Position, startNodeTmp);
                }
            }

            foreach (MapNode node in nodes)
            {
                nodeEdgesContainer = new List<MapEdge>();
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (!(i == 0 && j == 0))
                        {
                            try
                            {
                                endNodeTmp =
                                    tmpNodesDict[
                                        new Vector2Int(node.Element.Position.x + i, node.Element.Position.y + j)];
                            }
                            catch (Exception e)
                            {
                                endNodeTmp = null;
                            }

                            if (endNodeTmp != null)
                            {
                                nodeEdgesContainer.Add(new MapEdge(node, endNodeTmp, 1));
                            }
                        }
                    }
                }

                node.Edges = nodeEdgesContainer;
                edges.AddRange(nodeEdgesContainer);
            }

            return new MapGraph(nodes, edges);
        }
    }
}