using System;

namespace WyrmsunMapExporting
{
    public class WyrmsunRace
    {
        public readonly int raceId;
        public readonly string raceName;
        public readonly string raceParam;

        public WyrmsunRace(int id, string raceName, string raceParam)
        {
            raceId = id;
            this.raceName = raceName;
            this.raceParam = raceParam;
        }

        public string GetDefaultFaction()
        {
            if (raceParam == null)
            {
                return "";
            }

            if (raceParam == WyrmRaceTypes.GERMANS)
            {
                return "ingaevone-tribe";
            }

            if (raceParam == WyrmRaceTypes.GOBLINS)
            {
                return "dreadskull-tribe";
            }

            return "goldhoof-clan";
        }
    }
}