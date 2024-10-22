using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SportsStats.Controllers
{
    public class TeamsController : ApiController
    {
        public GetTeamsResult GetTeams(int leagueID, int sportID)
        {
            try
            {
                return GetTeamsResult(leagueID, sportID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TeamResult GetTeam(int teamID, int leagueID)
        {
            try
            {
                return GetTeamResult(teamID, leagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ActionName("AddTeam")]
        public GetTeamsResult AddTeam([FromBody] TeamsResult team)
        {
            try
            {
                if (team.ID == 0)
                {
                    TeamsService.GetInstance().AddTeam(ConvertObjects.ConvertType(team));
                }
                else
                {
                    TeamsService.GetInstance().AddLeagueTeam(team.ID, team.LeagueID);
                }
                return GetTeamsResult(team.LeagueID, team.SportID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ActionName("AddPlayer")]
        public TeamResult AddPlayer([FromBody] TeamPlayerSaveRequest saveRequest)
        {
            try
            {
                PlayersService.GetInstance().AddPlayer(ConvertObjects.ConvertType(saveRequest.Player), saveRequest.TeamID, saveRequest.LeagueID);
                return GetTeamResult(saveRequest.TeamID, saveRequest.LeagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ActionName("AddGame")]
        public TeamResult AddGame([FromBody] GameResultBase game)
        {
            try
            {
                if (game.Team1ID == 0 || game.Team2ID == 0)
                {
                    throw new Exception("Invalid Team");
                }
                GamesService.GetInstance().AddGame(ConvertObjects.ConvertType(game));
                return GetTeamResult(game.Team1ID, game.LeagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private GetTeamsResult GetTeamsResult(int leagueID, int sportID)
        {
            var result = new GetTeamsResult();
            var dataCache = new DataCache();
            var allTeams = TeamsService.GetInstance().GetTeams(sportID: sportID, dataCache: dataCache);
            var teams = TeamsService.GetInstance().GetTeams(leagueID: leagueID, dataCache: dataCache);
            result.Teams = teams.Select(t => new TeamsResult()
            {
                ID = t.ID,
                Name = t.Name
            }).ToList();

            result.AvailableTeams = allTeams
                .Where(t => !teams.Any(t1 => t1.ID == t.ID))
                .GroupBy(t => new { t.ID, t.Name })
                .Select(t => new TeamsResult()
                {
                    ID = t.Key.ID,
                    Name = t.Key.Name
                }).ToList();

            return result;
        }

        private TeamResult GetTeamResult(int teamID, int leagueID)
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
            var playerStats = StatsService.GetInstance().GetAllStats(teamID, team.SportID, leagueID, dataCache: dataCache);

            var statTypesHash = statTypes
                     .Where(st => st.ShowTeam)
                     .Select(s => s.ID)
                     .ToHashSet();

            foreach (var playerStat in playerStats)
            {
                playerStat.Stats = playerStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                playerStat.PlayerName = CommonFunctions.TrimPlayerName(playerStat.PlayerName);
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
                TeamPlayerStats = playerStats.Select(s => ConvertObjects.ConvertType(s)).ToList(),
                StatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).ToList(),
            };
            return teamResult;
        }

        private bool? DidWin(bool isTeam1, int team1score, int team2score)
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
    public static class Extensions
    {
        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }
    }
}