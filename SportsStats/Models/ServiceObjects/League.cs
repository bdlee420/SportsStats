using System;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Models.ServiceObjects
{
    public class League
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SportID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Seasons Season { get; set; }
    }
}