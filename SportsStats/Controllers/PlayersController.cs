using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Models.DTOObjects;
using SportsStats.Models.ServiceObjects;
using SportsStats.Services;
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
            var playerStatsByLeague = StatsService.GetInstance().GetAllStats(null, playerID: id, dataCache: dataCache);
            var totalPlayer = new List<DTOPlayer> { new DTOPlayer { ID = -1, Name = "Total" } };
            var totalBasketballStats = StatsService.GetInstance().GetAllStats(totalPlayer, playerID: id, sportID: (int)SportsList.Basketball, dataCache: dataCache);
            var statTypes = GamesService.GetInstance().GetStatTypes().Where(st => st.ShowTeam).OrderBy(s => s.GridDisplayOrder);

            var statTypesHash = statTypes
                     .Where(st => st.ShowTeam)
                     .Select(s => s.ID)
                     .ToHashSet();

            foreach (var playerStat in playerStatsByLeague)
            {
                playerStat.Stats = playerStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                playerStat.PlayerName = CommonFunctions.TrimPlayerName(playerStat.PlayerName);
            }

            foreach (var basketballStat in totalBasketballStats)
            {
                basketballStat.Stats = basketballStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                basketballStat.PlayerName = CommonFunctions.TrimPlayerName(basketballStat.PlayerName);
            }

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
                HockeyStats = playerStatsByLeague
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Hockey && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                BaseballStats = playerStatsByLeague
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Baseball && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                BasketballStats = playerStatsByLeague
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Basketball && leagues.Exists(l => l.ID == s.LeagueID))
                    .OrderBy(s => leagues.First(l => l.ID == s.LeagueID).StartDate)
                    .ToList(),
                TotalBasketballStats = totalBasketballStats
                    .Select(s => ConvertObjects.ConvertType(s))
                    .Where(s => s.Sport == SportsList.Basketball && (s.LeagueID == 0 || leagues.Exists(l => l.ID == s.LeagueID)))
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

                int endYear = league.EndDate.Year % 100;
                string year = league.StartDate.Year == league.EndDate.Year ? league.StartDate.Year.ToString() : $"{league.StartDate.Year}-{endYear}";
                string name = $"{year} {league.Season} - {league.Name}";

                stat.LeagueNameFull = name;
                stat.LeagueName = CommonFunctions.TrimLeagueName(name);
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