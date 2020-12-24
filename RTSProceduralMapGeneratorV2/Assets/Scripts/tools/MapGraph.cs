using System.Collections.Generic;

namespace tools
{
    public class MapGraph
    {
        private List<MapNode> mapNodes;
        private List<MapEdge> graphEdges;

        public MapGraph(List<MapNode> mapNodes, List<MapEdge> graphEdges)
        {
            this.mapNodes = mapNodes;
            this.graphEdges = graphEdges;
        }

        public List<MapNode> MapNodes => mapNodes;

        public List<MapEdge> GraphEdges => graphEdges;
    }
}