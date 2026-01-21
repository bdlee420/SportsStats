using System;
using System.Collections.Generic;
using System.Web.Http;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;

namespace SportsStats.Controllers
{
    public class LeaguesController : ApiController
    {
        [HttpGet]
        public List<Season> GetSeasons()
        {
            try
            {
                return new List<Season>
                {
                    new Season { ID = 1, Name = "Winter" },
                    new Season { ID = 2, Name = "Spring" },
                    new Season { ID = 3, Name = "Summer" },
                    new Season { ID = 4, Name = "Fall" }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public SportsResults GetLeagues()
        {
            try
            {
                return GetLeaguesResult();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public IHttpActionResult AddLeague([FromBody] SportsLeague league)
        {
            try
            {
                if (!UserHelper.HasAdminPermissions())
                {
                    return Unauthorized();
                }

                var id = LeaguesService.GetInstance().AddLeague(new SportsStats.Models.ServiceObjects.League
                {
                    Name = league.Name,
                    SportID = league.SportID,
                    StartDate = league.StartDate,
                    EndDate = league.EndDate,
                    Season = (SportsStats.Helpers.Enums.Seasons)league.SeasonID
                });
                return Ok(id);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
       
        private SportsResults GetLeaguesResult()
        {
            var sportsResults = new SportsResults();
            var currentLeagues = new List<SportsResult>();
            var oldLeagues = new List<SportsResult>();
            var sports = SportsService.GetInstance().GetSports();

            foreach (var sport in sports)
            {
                var leagues = LeaguesService.GetInstance().GetLeagues(sport.ID);
                foreach (var league in leagues)
                {
                    int endYear = league.EndDate.Year % 100;
                    string year = league.StartDate.Year == league.EndDate.Year ? league.StartDate.Year.ToString() : $"{league.StartDate.Year}-{endYear}";
                    string name = $"{sport.Name}: {year} {league.Season} - {league.Name}";
                    //string name = string.Format("{0} - {1}-{2} - {3}", sport.Name, league.StartDate.Year, league.Season.ToString(), league.Name);
                    bool isCurrent = league.StartDate <= DateTime.Now && league.EndDate >= DateTime.Now;
                    if (isCurrent)
                    {
                        currentLeagues.Add(new SportsResult()
                        {
                            Name = name,
                            SportsLeagueSelection = new SportsLeague()
                            {
                                LeagueID = league.ID,
                                SportID = sport.ID
                            }
                        });
                    }      
                    else
                    {
                        oldLeagues.Add(new SportsResult()
                        {
                            Name = name,
                            SportsLeagueSelection = new SportsLeague()
                            {
                                LeagueID = league.ID,
                                SportID = sport.ID
                            }
                        });
                    }        
                }
            }

            sportsResults.OldLeagues = oldLeagues;
            sportsResults.CurrentLeagues = currentLeagues;

            return sportsResults;
        }       
    }

    public class Season
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}