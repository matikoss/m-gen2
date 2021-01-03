using WyrmsunMapExporting;

namespace helpers
{
    public class PlayerRacesData
    {
        public WyrmsunRace PlayerOneRace;
        public WyrmsunRace PlayerTwoRace;

        public PlayerRacesData(WyrmsunRace playerOneRace, WyrmsunRace playerTwoRace)
        {
            PlayerOneRace = playerOneRace;
            PlayerTwoRace = playerTwoRace;
        }
    }
}