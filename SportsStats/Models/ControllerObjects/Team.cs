using System;
using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
	public class GetTeamsResult
	{
		public List<TeamsResult> AvailableTeams { get; set; }
		public List<TeamsResult> Teams { get; set; }
	}
	public class TeamsResult
    {
		public int SportID { get; set; }
		public int LeagueID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class TeamResult
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<TeamGameResult> Games { get; set; }
        public List<TeamsResult> Teams { get; set; }
        public List<PlayersResult> AvailablePlayers { get; set; }
        public List<StatTypeResult> StatTypes { get; set; }
        public List<PlayerStatsResult> TeamPlayerStats { get; set; }
    }
    public class TeamPlayerResult
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
    public class TeamPlayerSaveRequest
    {
        public TeamPlayerResult Player { get; set; }
        public int TeamID { get; set; }
        public int LeagueID { get; set; }
    }
    public class TeamGameResult
    {
        public int ID { get; set; }
        public bool? DidWin { get; set; }
        public int HighScore { get; set; }
        public string OtherTeamName { get; set; }
        public int LowScore { get; set; }
        public DateTime GameDate { get; set; }
    }
}