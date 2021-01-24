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

        private float heightRating,
            humidityRating,
            resAvailRating,
            avgResRating,
            minMaxRating,
            secondResourceRating,
            pDistanceRating,
            absDistRating;

        private MapParameters parameters;
        private List<MapNode> calculatedDistancesForPlayerOne;
        private List<MapNode> calculatedDistancesForPlayerTwo;
        private Vector2 resourcesStart;
        private float p1SpawnHeight, p2SpawnHeight;
        private float p1SpawnHumidity, p2SpawnHumidity;

        public EvoMapWrapper(Map map, MapParameters parameters, Vector2 resourcesStart)
        {
            this.map = map;
            this.parameters = parameters;
            rating = 0f;
            humidityRating = 0f;
            resAvailRating = 0f;
            avgResRating = 0f;
            minMaxRating = 0f;
            pDistanceRating = 0f;
            absDistRating = 0f;
            calculatedDistancesForPlayerOne = null;
            calculatedDistancesForPlayerTwo = null;
            p1SpawnHeight = 0f;
            p2SpawnHeight = 0f;
            p1SpawnHumidity = 0f;
            p2SpawnHumidity = 0f;
            secondResourceRating = 0f;
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

        public List<MapNode> CalculatedDistancesForPlayerOne1
        {
            get => calculatedDistancesForPlayerOne;
            set => calculatedDistancesForPlayerOne = value;
        }

        public List<MapNode> CalculatedDistancesForPlayerTwo1
        {
            get => calculatedDistancesForPlayerTwo;
            set => calculatedDistancesForPlayerTwo = value;
        }

        public Vector2 ResourcesStart
        {
            get => resourcesStart;
            set => resourcesStart = value;
        }

        public List<MapNode> CalculatedDistancesForPlayerOne
        {
            get => calculatedDistancesForPlayerOne;
            set => calculatedDistancesForPlayerOne = value;
        }

        public List<MapNode> CalculatedDistancesForPlayerTwo
        {
            get => calculatedDistancesForPlayerTwo;
            set => calculatedDistancesForPlayerTwo = value;
        }

        public float P1SpawnHeight
        {
            get => p1SpawnHeight;
            set => p1SpawnHeight = value;
        }

        public float P2SpawnHeight
        {
            get => p2SpawnHeight;
            set => p2SpawnHeight = value;
        }

        public float P1SpawnHumidity
        {
            get => p1SpawnHumidity;
            set => p1SpawnHumidity = value;
        }

        public float P2SpawnHumidity
        {
            get => p2SpawnHumidity;
            set => p2SpawnHumidity = value;
        }

        public float HeightRating
        {
            get => heightRating;
            set => heightRating = value;
        }

        public float HumidityRating
        {
            get => humidityRating;
            set => humidityRating = value;
        }

        public float ResAvailRating
        {
            get => resAvailRating;
            set => resAvailRating = value;
        }

        public float AvgResRating
        {
            get => avgResRating;
            set => avgResRating = value;
        }

        public float MINMaxRating
        {
            get => minMaxRating;
            set => minMaxRating = value;
        }

        public float PDistanceRating
        {
            get => pDistanceRating;
            set => pDistanceRating = value;
        }

        public float AbsDistRating
        {
            get => absDistRating;
            set => absDistRating = value;
        }

        public float SecondResourceRating
        {
            get => secondResourceRating;
            set => secondResourceRating = value;
        }
    }
}