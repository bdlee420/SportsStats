using System.Collections.Generic;

namespace SportsStats.Models.DTOObjects
{
    public class DTOUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public List<int> Teams { get; set; }
        public List<int> Leagues { get; set; }
        public List<int> Players { get; set; }
        public List<int> Sports { get; set; }
    }
}