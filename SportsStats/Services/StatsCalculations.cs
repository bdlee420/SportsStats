using System;
using System.Collections.Generic;
using System.Linq;
using SportsStats.Models.ServiceObjects;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
    public class StatsCalculations
    {
        public static decimal GetValue(CalculatedStatTypes statType, List<GameStat> stats, List<StatStates> filterStatStates = null, decimal? gamesOverride = null)
        {
            Func<CalculatedStatTypes, decimal> GetValueFunc = (x) => GetValue(x, stats, filterStatStates, gamesOverride);
            Func<CalculatedStatTypes, List<GameStat>, decimal> GetValueFunc2 = (x, y) => GetValue(x, y, filterStatStates, gamesOverride);
            decimal games;
            switch (statType)
            {
                case CalculatedStatTypes.PPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Points) / games;
                case CalculatedStatTypes.RPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.TotalRebound) / games;
                case CalculatedStatTypes.OREBPercent:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    var oreb = GetValueFunc(CalculatedStatTypes.OREB);
                    return games == 0 ? 0 : oreb / games;
                case CalculatedStatTypes.TotalRebound:
                    oreb = GetValueFunc(CalculatedStatTypes.OREB);
                    var dreb = GetValueFunc(CalculatedStatTypes.DREB);
                    return dreb + oreb;
                case CalculatedStatTypes.APG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Assists) / games;
                case CalculatedStatTypes.SPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Steals) / games;
                case CalculatedStatTypes.BPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Blocks) / games;
                case CalculatedStatTypes.TPG:
                    games = GetValueFunc(CalculatedStatTypes.Games);
                    return games == 0 ? 0 : GetValueFunc(CalculatedStatTypes.Turnovers) / games;
                case CalculatedStatTypes.Points:
                    return (GetValueFunc(CalculatedStatTypes.TwoMade) * 2) + GetValueFunc(CalculatedStatTypes.FTMade) + (GetValueFunc(CalculatedStatTypes.ThreeMade) * 3);
                case CalculatedStatTypes.FGAttempt:
                    return GetValueFunc(CalculatedStatTypes.TwoMade) + GetValueFunc(CalculatedStatTypes.TwoMiss) + GetValueFunc(CalculatedStatTypes.ThreeMade) + GetValueFunc(CalculatedStatTypes.ThreeMiss);
                case CalculatedStatTypes.FTAttempt:
                    return GetValueFunc(CalculatedStatTypes.FTMade) + GetValueFunc(CalculatedStatTypes.FTMiss);
                case CalculatedStatTypes.ThreeAttempt:
                    return GetValueFunc(CalculatedStatTypes.ThreeMade) + GetValueFunc(CalculatedStatTypes.ThreeMiss);
                case CalculatedStatTypes.MaxPoints:
                    {
                        decimal maxPoints = 0;
                        foreach(var gameStats in stats.GroupBy(s=>s.GameID))
                        {
                            var points = GetValueFunc2(CalculatedStatTypes.Points, gameStats.ToList());
                            maxPoints = points > maxPoints ? points : maxPoints;
                        }
                        return maxPoints;
                    }
                case CalculatedStatTypes.MaxRebs:
                    {
                        decimal maxRebs = 0;
                        foreach (var gameStats in stats.GroupBy(s => s.GameID))
                        {
                            var rebs = GetValueFunc2(CalculatedStatTypes.TotalRebound, gameStats.ToList());
                            maxRebs = rebs > maxRebs ? rebs : maxRebs;
                        }
                        return maxRebs;
                    }
                case CalculatedStatTypes.FGMade:
                    {
                        return GetValueFunc(CalculatedStatTypes.TwoMade) + GetValueFunc(CalculatedStatTypes.ThreeMade);
                    }
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
                case CalculatedStatTypes.TwoPercent:
                    {
                        decimal total = GetValueFunc(CalculatedStatTypes.TwoMade) + GetValueFunc(CalculatedStatTypes.TwoMiss);
                        return total == 0 ? 0 : GetValueFunc(CalculatedStatTypes.TwoMade) / total;
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
                case CalculatedStatTypes.Games:
                    {
                        return gamesOverride.HasValue ? gamesOverride.Value : GetNonCalculatedValue(statType, stats, filterStatStates);
                    }
                default:
                    return GetNonCalculatedValue(statType, stats, filterStatStates);
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