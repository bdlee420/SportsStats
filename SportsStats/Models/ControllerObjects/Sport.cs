using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
    public class SportsResults
    {
        public List<SportsResult> CurrentLeagues { get; set; }
        public List<SportsResult> OldLeagues { get; set; }
    }
    public class SportsResult
    {
        public SportsLeague SportsLeagueSelection { get; set; }
        public string Name { get; set; }
        public bool IsCurrentLeague { get; set; }
    }
    public class SportsLeague
    {
        public int SportID { get; set; }
        public int LeagueID { get; set; }
        public string Name { get; set; }
        public int SeasonID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
    }
}