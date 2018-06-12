using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
    public class LeagueResult
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<TeamsResult> Teams { get; set; }
    }
}