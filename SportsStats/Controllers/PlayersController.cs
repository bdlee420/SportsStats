using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Models.ServiceObjects;
using SportsStats.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Controllers
{
    public class PlayersController : ApiController
    {
        public List<PlayersResult> Get()
        {
            return GetPlayers();
        }

        public PlayerResult Get(int id)
        {
            var dataCache = new DataCache();
            var player = PlayersService.GetInstance().GetPlayer(id, dataCache);
            var sports = SportsService.GetInstance().GetSports();
            var leagues = LeaguesService.GetInstance().GetLeagues();
            var stats = StatsService.GetInstance().GetAllStats(playerID: id, dataCache: dataCache);
            var statTypes = GamesService.GetInstance().GetStatTypes().OrderBy(s => s.GridDisplayOrder);

            var playerResult = new PlayerResult()
            {
                ID = player.ID,
                Name = player.Name,
                Games = player.Games?.Select(g => new PlayerGameResult()
                {
                    ID = g.ID,
                    GameDate = g.GameDate,
                    Team1Score = g.Team1Score,
                    Team2Score = g.Team2Score
                }).ToList(),
                Teams = player.Teams?.Where(t => leagues.Exists(x => x.ID == t.LeagueID)).Select(t =>
                {
                    var league = leagues.First(l => l.ID == t.LeagueID);
                    var sport = sports.First(s => s.ID == league.SportID);
                    return new PlayerTeamResult()
                    {
                        ID = t.ID,
                        Name = t.Name,
                        LeagueID = t.LeagueID,
                        Number = t.PlayerNumber,
                        SportID = league.SportID,
                        LeagueName = string.Format("{0} - {1} {2} - {3}", sport.Name, league.StartDate.Year, league.Season.ToString(), league.Name)
                    };
                }).OrderBy(t => t.LeagueName).ToList(),
                HockeyStats = stats
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Hockey && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                BaseballStats = stats
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Baseball && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                BasketballStats = stats
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Basketball && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                HockeyStatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).Where(st => st.Sport == SportsList.Hockey).ToList(),
                BaseballStatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).Where(st => st.Sport == SportsList.Baseball).ToList(),
                BasketballStatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).Where(st => st.Sport == SportsList.Basketball).ToList(),
            };

            UpdateStatRow(playerResult.HockeyStats, player, leagues);
            UpdateStatRow(playerResult.BaseballStats, player, leagues);
            UpdateStatRow(playerResult.BasketballStats, player, leagues);

            return playerResult;
        }

        private void UpdateStatRow(List<PlayerStatsResult> stats, Player player, List<League> leagues)
        {
            foreach (var stat in stats)
            {
                var team = player.Teams.First(t => t.ID == stat.TeamID);
                var league = leagues.First(l => l.ID == stat.LeagueID);
                stat.LeagueName = string.Format("{0} {1} - {2}", league.StartDate.Year, league.Season.ToString(), team.Name);
                if (league.StartDate <= DateTime.Now && league.EndDate >= DateTime.Now)
                    stat.IsCurrentLeague = true;
            }
        }

        [HttpPost]
        public List<PlayersResult> AddPlayer([FromBody] PlayersResult player)
        {
            PlayersService.GetInstance().AddPlayer(ConvertObjects.ConvertType(player));
            return GetPlayers();
        }

        [HttpPost]
        public List<PlayersResult> SavePlayer([FromBody] PlayerResult player)
        {
            PlayersService.GetInstance().SavePlayer(ConvertObjects.ConvertType(player));
            return GetPlayers();
        }

        private List<PlayersResult> GetPlayers()
        {
            var players = PlayersService.GetInstance().GetPlayers();
            var playersResult = players.Select(t => new PlayersResult()
            {
                ID = t.ID,
                Name = t.Name,
            });
            return playersResult.ToList();
        }
    }
}