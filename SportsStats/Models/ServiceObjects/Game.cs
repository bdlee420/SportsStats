using System;
using System.Collections.Generic;

namespace SportsStats.Models.ServiceObjects
{
    public class Game
    {
        public int ID { get; set; }
        public int Team1ID { get; set; }
        public string Team1Name { get; set; }
        public int Team1Score { get; set; }
        public int Team2ID { get; set; }
        public string Team2Name { get; set; }
        public int Team2Score { get; set; }
        public List<PlayerStats> PlayerStats { get; set; }
        public DateTime GameDate { get; set; }
        public int SportID { get; set; }
		public int LeagueID { get; set; }

		public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", ID, Team1ID, Team1Score, Team2ID, Team2Score, GameDate);
        }
    }
}