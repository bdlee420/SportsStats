using System;

namespace SportsStats.Models.ControllerObjects
{
    public class GameLogResult
    {
        public string TeamName { get; set; }
        public string PlayerName { get; set; }
        public string DisplayName { get; set; }
        public int Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}