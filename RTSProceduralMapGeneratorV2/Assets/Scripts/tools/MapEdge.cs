namespace tools
{
    public class MapEdge
    {
        private int distance;
        private MapNode startNode;
        private MapNode endNode;

        public MapEdge(MapNode start, MapNode end, int dist)
        {
            startNode = start;
            endNode = end;
            distance = dist;
        }

        public int Distance => distance;

        public MapNode StartNode => startNode;

        public MapNode EndNode => endNode;
    }
}