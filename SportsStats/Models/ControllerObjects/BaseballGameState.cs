using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
    public class BaseballGameStateResult
    {
		public int LeagueID { get; set; }
		public int GameID { get; set; }
        public int Inning { get; set; }
        public bool TopOfInning { get; set; }
        public PlayersResult PlayerOnFirst { get; set; }
        public PlayersResult PlayerOnSecond { get; set; }
        public PlayersResult PlayerOnThird { get; set; }
        public int NumberOfOuts { get; set; }
        public List<PlayersResult> RunnersScored { get; set; }
        public List<PlayersResult> RunnersOut { get; set; }
        public StatRequest GameStat { get; set; }
        public bool DataSaved { get; set; }
        public bool NextAtBat { get; set; }
        public bool InningChanged { get; set; }
        public PlayersResult Team1Player { get; set; }
        public PlayersResult Team2Player { get; set; }
        public NextBatterResult NextBatter { get; set; }
        public bool IsStatsPage { get; set; }
    }
    public class NextBatterResult
    {
        public int TeamID { get; set; }
        public int? PlayerID { get; set; }
    }
    public class PlayerOrder
    {
        public int PlayerID { get; set; }
        public int Order { get; set; }
    }
}