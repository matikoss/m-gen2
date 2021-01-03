using System;
using System.Collections.Generic;
using MapEntities;
using tools;
using UnityEngine;

namespace MapGeneration
{
    public class EvoMapWrapper : IComparable<EvoMapWrapper>
    {
        private Map map;
        private float rating;
        private MapParameters parameters;
        private List<MapNode> CalculatedDistancesForPlayerOne;
        private List<MapNode> CalculatedDistancesForPlayerTwo;
        private Vector2 resourcesStart;

        public EvoMapWrapper(Map map, MapParameters parameters, Vector2 resourcesStart)
        {
            this.map = map;
            this.parameters = parameters;
            rating = 0;
            CalculatedDistancesForPlayerOne = null;
            CalculatedDistancesForPlayerTwo = null;
            this.resourcesStart = resourcesStart;
        }

        public Map Map
        {
            get => map;
            set => map = value;
        }

        public float Rating
        {
            get => rating;
            set => rating = value;
        }

        public MapParameters Parameters
        {
            get => parameters;
            set => parameters = value;
        }

        public int CompareTo(EvoMapWrapper other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return rating.CompareTo(other.rating);
        }
    }
}