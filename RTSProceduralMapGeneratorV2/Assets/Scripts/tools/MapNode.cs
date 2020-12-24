using System;
using System.Collections.Generic;
using MapEntities;

namespace tools
{
    public class MapNode : IComparable<MapNode>
    {
        private MapElement element;
        private List<MapEdge> edges;
        private bool visited;
        private bool isSelected;
        private MapNode previousNode;
        private int distanceFromStart;

        public MapNode(MapElement element)
        {
            this.element = element;
            edges = new List<MapEdge>();
            visited = false;
            isSelected = false;
            previousNode = null;
            distanceFromStart = 0;
        }

        public MapElement Element
        {
            get => element;
            set => element = value;
        }

        public List<MapEdge> Edges
        {
            get => edges;
            set => edges = value;
        }

        public bool Visited
        {
            get => visited;
            set => visited = value;
        }

        public bool IsSelected
        {
            get => isSelected;
            set => isSelected = value;
        }

        public MapNode PreviousNode
        {
            get => previousNode;
            set => previousNode = value;
        }

        public int DistanceFromStart
        {
            get => distanceFromStart;
            set => distanceFromStart = value;
        }

        public int CompareTo(MapNode other)
        {
            if (distanceFromStart > other.distanceFromStart) return -1;
            if (distanceFromStart < other.distanceFromStart) return 1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as MapNode;
            if (other == null)
            {
                return false;
            }

            return element.Position.Equals(other.element.Position) && element.GetType() == other.element.GetType();
        }
    }
}