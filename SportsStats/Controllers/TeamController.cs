using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;

namespace SportsStats.Controllers
{
    public class TeamController : ApiController
    {      
        public TeamResult GetTeam(int teamID, int leagueID)
        {
            try
            {
                return TeamHelper.GetTeamResult(teamID, leagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [ActionName("UpdateTeam")]
        [HttpPost]
        public TeamResult UpdateTeam([FromBody] TeamsResult team)
        {
            try
            {
                // Validate
                if (team.ID == 0)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));

                TeamsService.GetInstance().UpdateTeam(ConvertObjects.ConvertType(team));
                return TeamHelper.GetTeamResult(team.ID, team.LeagueID);
            }
            catch (HttpResponseException)
            {
                throw;
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
                return TeamHelper.GetTeamResult(saveRequest.TeamID, saveRequest.LeagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}