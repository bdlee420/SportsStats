using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.ServiceObjects;
using System.Collections.Generic;
using System.Linq;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
    public class LeaguesService
    {
        private static readonly LeaguesService _service = new LeaguesService();
        public static LeaguesService GetInstance()
        {
            if (_service == null)
            {
                return new LeaguesService();
            }
            return _service;
        }

        public List<League> GetLeagues(int? sportID = null)
        {
            var leagues = LeagueDataProvider.GetInstance().GetLeagues(sportID);

                return leagues
                .Where(s=> UserHelper.HasGetPermissions(leagueID: s.ID))
                .Select(s => new League()
                {
                    ID = s.ID,
                    Name = s.Name,
                    EndDate = s.EndDate,
                    StartDate = s.StartDate,
                    Season = (Seasons)s.SeasonID,
                    SportID = s.SportID,
                })
                .ToList();
        }

        public int GetLeagueID(int gameID)
        {
            return LeagueDataProvider.GetInstance().GetLeagueId(gameID);
        }
    }
}