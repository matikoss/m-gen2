using System;
using System.Collections.Generic;

namespace tools
{
    public class DijkstraPathfinder
    {
        public int pathDistance;
        public List<MapNode> path;

        public DijkstraPathfinder()
        {
            pathDistance = 0;
            path = new List<MapNode>();
        }

        public void shortesPath(MapGraph graph, MapNode start, MapNode end)
        {
            List<MapNode> received = dijkstraCalculateDistances(graph, start);
            pathDistance = end.DistanceFromStart;
            MapNode tmp = end;
            int step = 0;
            while (tmp.PreviousNode != null && step <= received.Count)
            {
                path.Add(tmp.PreviousNode);
                tmp = tmp.PreviousNode;
                step++;
            }

            if (step >= received.Count)
            {
                pathDistance = Int32.MaxValue;
                path.Clear();
            }
        }

        public List<MapNode> dijkstraCalculateDistances(MapGraph graph, MapNode start)
        {
            List<MapNode> tmpNodes = graph.MapNodes;
            PriorityQ<MapNode> pq = new PriorityQ<MapNode>();
            MapNode tmp;
            foreach (var node in tmpNodes)
            {
                node.DistanceFromStart = Int32.MaxValue;
                node.PreviousNode = null;
                if (node.Equals(start))
                {
                    node.DistanceFromStart = 0;
                }
            }

            foreach (var node in tmpNodes)
            {
                pq.AddToQueue(node);
            }

            while (!pq.isEmpty())
            {
                tmp = pq.PopFromQueue();
                List<MapNode> changed = new List<MapNode>();
                foreach (var edge in tmp.Edges)
                {
                    if (edge.EndNode.DistanceFromStart > tmp.DistanceFromStart + edge.Distance)
                    {
                        edge.EndNode.DistanceFromStart = tmp.DistanceFromStart + edge.Distance;
                        edge.EndNode.PreviousNode = tmp;
                        changed.Add(edge.EndNode);
                    }
                    // pq.UpdatePriority(edge.EndNode, edge.EndNode.DistanceFromStart);
                }
                pq.RestoreQueue();
                // pq.RestoreQueueV2(changed);
            }

            return tmpNodes;
        }
    }
}