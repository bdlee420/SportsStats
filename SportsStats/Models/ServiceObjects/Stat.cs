using System.Collections.Generic;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Models.ServiceObjects
{
    public class PlayerStats
    {
        public int StatGroup { get; set; }
        public int ID { get; set; }
        public int? PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int PlayerNumber { get; set; }
        public int TeamID { get; set; }
        public int GameID { get; set; }
        public int LeagueID { get; set; }
        public int SportID { get; set; }
        public bool IsActivePlayer { get; set; }
        public bool IsInGame { get; set; }
        public List<Stat> Stats { get; set; }        
    }

    public class GameStat
    {
        public int LeagueID { get; set; }
        public int GameID { get; set; }
        public int TeamID { get; set; }
        public int? PlayerID { get; set; }
        public StatType StatType { get; set; }
        public int Value { get; set; }
        public List<StatStates> StatStates { get; set; }
    }

    public class Stat
    {
        public SportsList Sport { get; set; }
        public int LeagueID { get; set; }
        public int TeamID { get; set; }
        public int GameID { get; set; }
        public StatType StatType { get; set; }
        public decimal Value { get; set; }
        public int? GroupID { get; set; }
        public List<StatStates> StatStates { get; set; }

        public override string ToString()
        {
            return StatType.DisplayName;
        }
    }

    public class PlayerGameStat
    {
        public int StatTypeID { get; set; }
        public int? PlayerID { get; set; }
		public int LeagueID { get; set; }
		public int GameID { get; set; }
        public int TeamID { get; set; }
        public int Value { get; set; }
        public int? GroupID { get; set; }
        public List<int> States { get; set; }
        public bool Override { get; set; }
    }

    public class StatGroup
    {
        public int? GroupID { get; set; }
    }
}