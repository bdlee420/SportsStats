using System;

namespace SportsStats.Models.DTOObjects
{
    public class DTOGame
    {
        public int ID { get; set; }
        public int Team1ID { get; set; }
        public int Team1Score { get; set; }
        public int Team2ID { get; set; }  
        public int Team2Score { get; set; }
        public DateTime GameDate { get; set; }
        public int SportID { get; set; }
		public int LeagueID { get; set; }
	}
}