using System.Collections.Generic;

namespace SportsStats.Models.DTOObjects
{
    public class DTOStat
    {
        public int ID { get; set; }
        public int StatTypeID { get; set; }
        public int Value { get; set; }
        public int? PlayerID { get; set; }
        public int TeamID { get; set; }
        public int GameID { get; set; }
        public int? GroupID { get; set; }
        public List<int> States { get; set; }
        public bool Override { get; set; }
    }

    public class DTOStatExtended : DTOStatType
    {
        public DTOStatExtended()
        {
            States = new List<int>();
        }
        public int ID { get; set; }
        public int Value { get; set; }
        public int? PlayerID { get; set; }
        public int TeamID { get; set; }
        public int GameID { get; set; }
        public int LeagueID { get; set; }
        public int SportID { get; set; }
        public int? GroupID { get; set; }
        public List<int> States { get; set; }
    }

    public class DTOStatGroup
    {
        public int? GroupID { get; set; }
    }
}