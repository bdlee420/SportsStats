using System;
using System.Collections.Generic;
using System.Linq;
using SportsStats.Models.ServiceObjects;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
    public class StatsCalculations
    {
        public static decimal GetValue(CalculatedStatTypes statTypeID, List<GameStat> stats, List<StatStates> filterStatStates = null)
        {
            Func<CalculatedStatTypes, decimal> GetValueFunc = (x) => GetValue(x, stats, filterStatStates);
            decimal games;
            switch (statTypeID)
            {
                case CalculatedStatTypes.PPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Points) / GetValueFunc(CalculatedStatTypes.Games);
                case CalculatedStatTypes.RPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Rebounds) / GetValueFunc(CalculatedStatTypes.Games);
                case CalculatedStatTypes.Points:
                    return (GetValueFunc(CalculatedStatTypes.FGMade) * 2) + GetValueFunc(CalculatedStatTypes.FTMade) + (GetValueFunc(CalculatedStatTypes.ThreeMade) * 3);
                case CalculatedStatTypes.FGAttempt:
                    return GetValueFunc(CalculatedStatTypes.FGMade) + GetValueFunc(CalculatedStatTypes.FGMiss) + GetValueFunc(CalculatedStatTypes.ThreeMade) + GetValueFunc(CalculatedStatTypes.ThreeMiss);
                case CalculatedStatTypes.FTAttempt:
                    return GetValueFunc(CalculatedStatTypes.FTMade) + GetValueFunc(CalculatedStatTypes.FTMiss);
                case CalculatedStatTypes.ThreeAttempt:
                    return GetValueFunc(CalculatedStatTypes.ThreeMade) + GetValueFunc(CalculatedStatTypes.ThreeMiss);
                case CalculatedStatTypes.FGPercent:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.FGAttempt);
                        return total == 0 ? 0 : GetValueFunc(CalculatedStatTypes.FGMade) / total;
                    }
                case CalculatedStatTypes.ThreePercent:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.ThreeAttempt);
                        return total == 0 ? 0 : GetValueFunc(CalculatedStatTypes.ThreeMade) / total;
                    }
                case CalculatedStatTypes.FTPercent:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.FTAttempt);
                        return total == 0 ? 0 : GetValueFunc(CalculatedStatTypes.FTMade) / total;
                    }
                case CalculatedStatTypes.Hits:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.Single) +
                            GetValueFunc(CalculatedStatTypes.Double) +
                            GetValueFunc(CalculatedStatTypes.Triple) +
                            GetValueFunc(CalculatedStatTypes.HomeRun);
                        return total;
                    }
                case CalculatedStatTypes.AB:
                    {
                        decimal hitsTotal = GetValueFunc(CalculatedStatTypes.Hits);
                        decimal outsTotal = GetValueFunc(CalculatedStatTypes.Strikeout) +
                            GetValueFunc(CalculatedStatTypes.GroundOut) +
                            GetValueFunc(CalculatedStatTypes.FC) +
                            GetValueFunc(CalculatedStatTypes.FlyOut);
                        return hitsTotal + outsTotal;
                    }
                case CalculatedStatTypes.AVG:
                    {
                        decimal hitsTotal = GetValueFunc(CalculatedStatTypes.Hits);
                        decimal abTotal = GetValueFunc(CalculatedStatTypes.AB);
                        return abTotal == 0 ? 0 : hitsTotal / abTotal;
                    }
                case CalculatedStatTypes.AVGRISP:
                    {
                        filterStatStates = new List<StatStates>() { StatStates.Second, StatStates.Third };
                        decimal hitsTotal = GetValueFunc(CalculatedStatTypes.Hits);
                        decimal abTotal = GetValueFunc(CalculatedStatTypes.AB);
                        return abTotal == 0 ? 0 : hitsTotal / abTotal;
                    }
                case CalculatedStatTypes.AVG2OUT:
                    {
                        filterStatStates = new List<StatStates>() { StatStates.Out2 };
                        decimal hitsTotal = GetValueFunc(CalculatedStatTypes.Hits);
                        decimal abTotal = GetValueFunc(CalculatedStatTypes.AB);
                        return abTotal == 0 ? 0 : hitsTotal / abTotal;
                    }
                case CalculatedStatTypes.SLG:
                    {
                        decimal tbTotal = GetValueFunc(CalculatedStatTypes.TB);
                        decimal abTotal = GetValueFunc(CalculatedStatTypes.AB);
                        return abTotal == 0 ? 0 : tbTotal / abTotal;
                    }
                case CalculatedStatTypes.OBP:
                    {
                        decimal reachedTotal = GetValueFunc(CalculatedStatTypes.Hits) + GetValueFunc(CalculatedStatTypes.BB);
                        decimal plateAppearanceTotal = GetValueFunc(CalculatedStatTypes.AB) + GetValueFunc(CalculatedStatTypes.SF);
                        return plateAppearanceTotal == 0 ? 0 : reachedTotal / plateAppearanceTotal;
                    }
                case CalculatedStatTypes.Contact:
                    {
                        decimal contact = GetValueFunc(CalculatedStatTypes.Hits) +
                            GetValueFunc(CalculatedStatTypes.GroundOut) +
                            GetValueFunc(CalculatedStatTypes.FlyOut) +
                            GetValueFunc(CalculatedStatTypes.Foul) +
                            GetValueFunc(CalculatedStatTypes.FC) +
                            GetValueFunc(CalculatedStatTypes.SF);
                        decimal miss = GetValueFunc(CalculatedStatTypes.SwingMiss);
                        return contact + miss == 0 ? 0 : (contact) / (contact + miss);
                    }
                case CalculatedStatTypes.TB:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.Single) +
                           2 * GetValueFunc(CalculatedStatTypes.Double) +
                           3 * GetValueFunc(CalculatedStatTypes.Triple) +
                           4 * GetValueFunc(CalculatedStatTypes.HomeRun);
                        return total;
                    }
                case CalculatedStatTypes.HockeyPoints:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.HockeyGoal) +
                           GetValueFunc(CalculatedStatTypes.HockeyAssist);
                        return total;
                    }
                default:
                    return GetNonCalculatedValue((CalculatedStatTypes)statTypeID, stats, filterStatStates);
            }
        }

        private static decimal GetNonCalculatedValue(CalculatedStatTypes type, List<GameStat> gameStats, List<StatStates> filterStatStates)
        {
            decimal value = 0;
            var stats = gameStats.Where(s => s.StatType.ID == (int)type);
            if (filterStatStates != null && filterStatStates.Any())
            {
                foreach (var stat in stats)
                {
                    foreach (var statState in stat.StatStates)
                    {
                        if (filterStatStates.Contains(statState))
                        {
                            value += stat.Value;
                        }
                    }
                }
            }
            else
            {
                value = stats.Sum(s => s.Value);
            }
            return value;
        }
    }
}