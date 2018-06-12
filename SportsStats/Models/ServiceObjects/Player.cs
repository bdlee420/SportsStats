using System.Collections.Generic;

namespace SportsStats.Models.ServiceObjects
{
    public class Player : PlayerBase
    {
        public List<Team> Teams { get; set; }
        public List<Game> Games { get; set; }
		public int TeamID { get; set; }
    }
    public class PlayerBase
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }        
    }
}