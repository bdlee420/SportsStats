namespace SportsStats.Helpers
{
    public class Enums
    {
        public enum CalculatedStatTypes
        {
            Points = 1,
            DREB = 2,
            Steals = 3,
            Assists = 4,
            TwoMade = 6,
            TwoMiss = 7,
            FTMade = 8,
            FTMiss = 9,
            FGAttempt = 12,
            FGPercent = 13,
            FTPercent = 14,
            FTAttempt = 15,
            Blocks = 16,
            PPG = 18,
            Games = 19,
            RPG = 20,
            APG = 21,
            SPG = 22,
            BPG = 23,
            TPG = 24,
            ThreeMade = 31,
            ThreeMiss = 32,
            ThreePercent = 33,
            ThreeAttempt = 34,
            FGMade = 35,
            Turnovers = 40,
            AB = 50,
            Hits = 51,
            RBI = 52,
            Runs = 53,
            Single = 54,
            Double = 55,
            Triple = 56,
            HomeRun = 57,
            BB = 58,
            Strikeout = 59,
            FC = 60,
            AVG = 61,
            OBP = 62,
            SLG = 63,
            GroundOut = 64,
            FlyOut = 65,
            SwingMiss = 66,
            Foul = 67,
            SF = 70,
            TB = 71,
            BattingOrder = 72,
            AVGRISP = 73,
            AVG2OUT = 74,
            Contact = 75,
            HockeyGoal = 100,
            HockeyAssist = 101,
            HockeyPoints = 102,
            HockeyShootOutGoal = 104,
            MaxPoints = 105,
            MaxRebs = 106,
            IsActive = 200,
            IsInGame = 201,
            TwoPercent = 205,
            OREB = 210,
            TotalRebound = 215,
            OREBPercent = 220,
        }
        public enum ValueTypes
        {
            Integer = 1,
            Percentage = 2,
            Decimal = 3,
            Decimal1 = 4,
        }
        public enum Seasons
        {
            Winter = 1,
            Spring = 2,
            Summer = 3,
            Fall = 4
        }
        public enum SportsList
        {
            Basketball = 1,
            Baseball = 2,
            Hockey = 3
        }
        public enum Roles
        {
            Admin = 1,
            User = 2,
        }
        public enum StatStates
        {
            First = 1,
            Second = 2,
            Third = 3,
            Out1 = 4,
            Out2 = 5
        }
    }
}