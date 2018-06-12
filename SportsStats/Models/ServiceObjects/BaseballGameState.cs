using System.Collections.Generic;

namespace SportsStats.Models.ServiceObjects
{
    public class AtBatStates
    {
        public BaseballGameState OriginalState { get; set; }
        public BaseballGameState NewState { get; set; }
    }
        public class BaseballGameState
    {       
        public int GameID { get; set; }
        public int Inning { get; set; }
        public bool TopOfInning { get; set; }
        public PlayerBase PlayerOnFirst { get; set; }
        public PlayerBase PlayerOnSecond { get; set; }
        public PlayerBase PlayerOnThird { get; set; }
        public int NumberOfOuts { get; set; }
        public List<PlayerBase> RunnersScored { get; set; }
        public List<PlayerBase> RunnersOut { get; set; }
        public bool PotentialAdjustment { get; set; }
        public bool NextAtBat { get; set; }
        public bool ChangeState { get; set; }
        public bool InningChanged { get; set; }
        public PlayerBase Team1Player { get; set; }
        public PlayerBase Team2Player { get; set; }
    }
    public class NextBatter
    {
        public int TeamID { get; set; }
        public int? PlayerID { get; set; }        
    }
}