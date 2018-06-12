using System.Collections.Generic;

namespace SportsStats.Models.ServiceObjects
{
    public class Team
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int SportID { get; set; }
        public int PlayerNumber { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public List<Game> Games { get; set; }
    }
}