﻿using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SportsStats.Controllers
{
    public class SportsController : ApiController
    {
        public SportsResults Get()
        {
            try
            {
                return GetSportsLeagues();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        private SportsResults GetSportsLeagues()
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
                    string name = string.Format("{0} - {1} {2} - {3}", sport.Name, league.StartDate.Year, league.Season.ToString(), league.Name);
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
}