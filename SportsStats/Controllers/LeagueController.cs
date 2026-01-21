using System;
using System.Linq;
using System.Web.Http;
using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;

namespace SportsStats.Controllers
{
    public class LeagueController : ApiController
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
        
        [ActionName("AddTeam")]
        public GetTeamsResult AddTeam([FromBody] TeamsResult team)
        {
            try
            {
                if (team.ID == 0)
                {
                    team.Name = team.Name.Trim();
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
                return TeamHelper.GetTeamResult(game.Team1ID, game.LeagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private GetTeamsResult GetTeamsResult(int leagueID, int sportID)
        {
            var user = UserService.GetInstance().GetUser();
            var isLeagueAdmin = user.AdminLeagueIDs.Contains(leagueID);
            var result = new GetTeamsResult();
            var dataCache = new DataCache();
            var allTeams = TeamsService.GetInstance().GetTeams(sportID: sportID, leagueID: leagueID, dataCache: dataCache, showAll: isLeagueAdmin);
            var teams = TeamsService.GetInstance().GetTeams(leagueID: leagueID, dataCache: dataCache, showAll: isLeagueAdmin);
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
    }
}