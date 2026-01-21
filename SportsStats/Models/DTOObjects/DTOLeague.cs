using System;

namespace SportsStats.Models.DTOObjects
{
    public class DTOLeague
    {
        public int ID { get; set; }
        public int SportID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SeasonID { get; set; }
    }
}
