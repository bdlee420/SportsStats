using System.Collections.Generic;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Models.ServiceObjects
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Roles Role { get; set; }
        public List<int> AdminLeagueIDs { get; set; }
        public List<int> Teams { get; set; }
        public List<int> Leagues { get; set; }
        public List<int> Players { get; set; }
        public List<int> Sports { get; set; }
        public bool HasOneTeam { get; set; }
        public bool HasOneLeague { get; set; }
        public int RequestedLeagueID { get; set;}
    }
}