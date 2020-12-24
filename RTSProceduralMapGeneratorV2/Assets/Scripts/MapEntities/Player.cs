using UnityEngine;

namespace MapEntities
{
    public class Player
    {
        private int id;
        private Vector2Int startingPosition;
        private string race;
        private string faction;

        private double avgDistanceFromResources;
        
        public Player(int id, Vector2Int startingPosition, string race, string faction)
        {
            this.id = id;
            this.startingPosition = startingPosition;
            this.race = race;
            this.faction = faction;
            avgDistanceFromResources = 0;
        }

        public int ID
        {
            get => id;
            set => id = value;
        }

        public Vector2Int StartingPosition
        {
            get => startingPosition;
            set => startingPosition = value;
        }

        public string Race
        {
            get => race;
            set => race = value;
        }

        public string Faction
        {
            get => faction;
            set => faction = value;
        }

        public double AvgDistanceFromResources
        {
            get => avgDistanceFromResources;
            set => avgDistanceFromResources = value;
        }
    }
}