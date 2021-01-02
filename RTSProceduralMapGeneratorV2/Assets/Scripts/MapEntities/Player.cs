using UnityEngine;

namespace MapEntities
{
    public class Player
    {
        private int id;
        private Vector2Int startingPosition;
        private string race;
        private string faction;
        private int startWood;
        private int startCopper;
        private int startStone;
        private PlayerTypeEnum playerType;

        private double avgDistanceFromResources;

        public Player(int id, Vector2Int startingPosition, string race, string faction, PlayerTypeEnum playerType)
        {
            this.id = id;
            this.startingPosition = startingPosition;
            this.race = race;
            this.faction = faction;
            this.playerType = playerType;
            avgDistanceFromResources = 0;
            startWood = 2000;
            startCopper = 2000;
            startStone = 2000;
        }

        public Player(int id, Vector2Int startingPosition, string race, string faction, PlayerTypeEnum playerType,
            int startWood, int startCopper,
            int startStone)
        {
            this.id = id;
            this.startingPosition = startingPosition;
            this.race = race;
            this.faction = faction;
            this.playerType = playerType;
            avgDistanceFromResources = 0;

            this.startWood = startWood;
            this.startCopper = startCopper;
            this.startStone = startStone;
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

        public int StartWood
        {
            get => startWood;
            set => startWood = value;
        }

        public int StartCopper
        {
            get => startCopper;
            set => startCopper = value;
        }

        public int StartStone
        {
            get => startStone;
            set => startStone = value;
        }

        public PlayerTypeEnum PlayerType => playerType;

        public void SetResources(int wood, int copper, int stone)
        {
            startWood = wood;
            startCopper = copper;
            startStone = stone;
        }
    }
}