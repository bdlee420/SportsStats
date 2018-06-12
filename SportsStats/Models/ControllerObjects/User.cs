using System.Collections.Generic;

namespace SportsStats.Models.ControllerObjects
{
    public class UserResult
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public List<int> Teams { get; set; }
        public List<int> Leagues { get; set; }
        public List<int> Sports { get; set; }
        public bool RememberMe { get; set; }
        public bool HasOneTeam { get; set; }
        public bool HasOneLeague { get; set; }
    }
}