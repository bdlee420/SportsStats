namespace SportsStats.Models.DTOObjects
{
    public class DTOBaseballGameState
    {
        public int GameID { get; set; }
        public int Inning { get; set; }
        public bool TopOfInning { get; set; }
        public int? PlayerOnFirst { get; set; }
        public int? PlayerOnSecond { get; set; }
        public int? PlayerOnThird { get; set; }
        public int NumberOfOuts { get; set; }
        public int? Team1PlayerID { get; set; }
        public int? Team2PlayerID { get; set; }
    }    
}