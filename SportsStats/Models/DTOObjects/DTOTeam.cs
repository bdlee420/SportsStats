using System.Collections.Generic;

namespace SportsStats.Models.DTOObjects
{
    public class DTOTeam
    {
        public int ID { get; set; }
        public int SportID { get; set; }
        public int LeagueID { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public List<int> Players { get; set; }
    }
}