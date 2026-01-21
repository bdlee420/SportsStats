using System;
using System.Collections.Generic;
using System.Linq;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Models.DTOObjects;
using SportsStats.Services;

namespace SportsStats.Common
{
    internal static class TeamHelper
    {
        internal static TeamResult GetTeamResult(int teamID, int leagueID)
        {
            var dataCache = new DataCache();
            var team = TeamsService.GetInstance().GetTeam(teamID, leagueID, dataCache: dataCache);
            var teams = TeamsService.GetInstance().GetTeams(leagueID: team.LeagueID, showAll: true, dataCache: dataCache);
            var players = PlayersService.GetInstance().GetPlayers(dataCache: dataCache);
            var statTypes = GamesService
                .GetInstance()
                .GetStatTypes(team.SportID)
                .Where(st => st.ShowTeam)
                .OrderBy(s => s.GridDisplayOrder);
            var playerStats = StatsService.GetInstance().GetAllStats(null, teamID, team.SportID, leagueID, dataCache: dataCache);
            var totalPlayer = new List<DTOPlayer> { new DTOPlayer { ID = -1, Name = "Total" } };
            var totalStats = StatsService.GetInstance().GetAllStats(totalPlayer, teamID, team.SportID, leagueID, dataCache: dataCache);

            var statTypesHash = statTypes
                     .Where(st => st.ShowTeam)
                     .Select(s => s.ID)
                     .ToHashSet();

            foreach (var playerStat in playerStats)
            {
                playerStat.Stats = playerStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                playerStat.PlayerName = CommonFunctions.TrimPlayerName(playerStat.PlayerName);
            }

            foreach (var totalStat in totalStats)
            {
                totalStat.Stats = totalStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                totalStat.PlayerName = CommonFunctions.TrimPlayerName(totalStat.PlayerName);
            }

            var teamResult = new TeamResult()
            {
                ID = team.ID,
                Name = team.Name,
                Games = team.Games.Select(g => new TeamGameResult()
                {
                    ID = g.ID,
                    GameDate = g.GameDate,
                    OtherTeamName = g.Team1ID == team.ID ? teams.First(t => t.ID == g.Team2ID).Name : teams.First(t => t.ID == g.Team1ID).Name,
                    DidWin = DidWin(g.Team1ID == team.ID, g.Team1Score, g.Team2Score),
                    HighScore = Math.Max(g.Team1Score, g.Team2Score),
                    LowScore = Math.Min(g.Team1Score, g.Team2Score),
                }).ToList(),
                Teams = teams.Select(t => new TeamsResult()
                {
                    ID = t.ID,
                    Name = t.Name
                }).ToList(),
                AvailablePlayers = players.Where(p => !team.Players.Exists(tp => tp.ID == p.ID)).Select(p => new PlayersResult()
                {
                    ID = p.ID,
                    Name = p.Name,
                }).ToList(),
                TeamPlayerStats = playerStats.Where(p => p.PlayerID != -1).Select(s => ConvertObjects.ConvertType(s)).ToList(),
                TeamTotalStats = totalStats.Where(p => p.PlayerID == -1).Select(s => ConvertObjects.ConvertType(s)).ToList(),
                StatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).ToList(),
            };

            // Calculate win/loss/tie record
            if (teamResult.Games != null)
            {
                teamResult.Wins = teamResult.Games.Count(g => g.DidWin == true);
                teamResult.Losses = teamResult.Games.Count(g => g.DidWin == false);
                teamResult.Ties = teamResult.Games.Count(g => g.DidWin == null);
                // Format record as W-L or W-L-T when ties exist
                teamResult.Record = teamResult.Ties > 0
                    ? string.Format("{0}-{1}-{2}", teamResult.Wins, teamResult.Losses, teamResult.Ties)
                    : string.Format("{0}-{1}", teamResult.Wins, teamResult.Losses);
            }
            return teamResult;
        }

        private static bool? DidWin(bool isTeam1, int team1score, int team2score)
        {
            if (team1score == team2score)
                return null;
            else if (team1score > team2score && isTeam1)
                return true;
            else if (team1score < team2score && !isTeam1)
                return true;
            else return false;
        }
    }
}
