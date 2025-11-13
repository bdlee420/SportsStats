using System;
using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
    public class PlayersResult
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
    public class PlayerResult
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public List<PlayerTeamResult> Teams { get; set; }
        public List<PlayerGameResult> Games { get; set; }
        public List<PlayerStatsResult> HockeyStats { get; set; }
        public List<PlayerStatsResult> BaseballStats { get; set; }
        public List<PlayerStatsResult> BasketballStats { get; set; }
        public List<PlayerStatsResult> TotalBasketballStats { get; set; }
        public List<StatTypeResult> HockeyStatTypes { get; set; }
        public List<StatTypeResult> BaseballStatTypes { get; set; }
        public List<StatTypeResult> BasketballStatTypes { get; set; }
    }
    public class PlayerTeamResult
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int LeagueID { get; set; }
        public int SportID { get; set; }
        public string LeagueName { get; set; }
    }
    public class PlayerGameResult
    {
        public int ID { get; set; }
        public bool IsTeam1 { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public DateTime GameDate { get; set; }
    }
}