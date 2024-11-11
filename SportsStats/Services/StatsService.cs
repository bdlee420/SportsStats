using SportsStats.Common;
using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.ServiceObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
    public class StatsService
    {
        private static readonly StatsService _service = new StatsService();

        public static StatsService GetInstance()
        {
            if (_service == null)
            {
                return new StatsService();
            }
            return _service;
        }

        public PlayerStats GetStats(int sportID, int gameID, int? playerID, int teamID, int leagueID, DataCache dataCache = null)
        {
            var stats = GameDataProvider.GetInstance().GetStats(gameID, playerID: playerID, teamID: teamID, dataCache: dataCache);
            var player = PlayersService.GetInstance().GetPlayers(teamID: teamID, gameID: gameID, leagueID: leagueID, dataCache: dataCache).Where(p => p.ID == playerID).First();

            return new PlayerStats()
            {
                PlayerName = player.Name,
                TeamID = teamID,
                GameID = gameID,
                PlayerID = player.ID,
                PlayerNumber = player.Number,
                Stats = stats.Select(s => new Stat()
                {
                    StatType = ConvertObjects.ConvertType(s),
                    GroupID = s.GroupID,
                    Value = s.Value
                }).ToList()
            };
        }

        public StatGroup GetStatGroup(int sportID, int gameID, int? playerID, int teamID)
        {

            var statGroup = new StatGroup();
            if (sportID == (int)SportsList.Baseball && UserHelper.HasUpdatePermissions())
            {
                statGroup = ConvertObjects.ConvertType(GameDataProvider.GetInstance().GetStatGroup(gameID, playerID, teamID, false));
            }
            return statGroup;
        }

        public bool CreateNewStatGroup(int sportID, int gameID, int? playerID, int teamID)
        {
            if (UserHelper.HasUpdatePermissions())
            {
                var statGroup = new StatGroup();
                if (sportID == (int)SportsList.Baseball)
                {
                    statGroup = ConvertObjects.ConvertType(GameDataProvider.GetInstance().GetStatGroup(gameID, playerID, teamID, true));
                }
                return true;
            }
            else
            {
                throw new UnauthorizedAccessException("nope");
            }
        }

        public List<StatGroup> GetStatGroups(int sportID, int gameID, int? playerID)
        {
            if (UserHelper.HasGetPermissions(gameID: gameID))
            {
                var statGroups = new List<StatGroup>();
                if (sportID == (int)SportsList.Baseball)
                {
                    var dtoStatGroups = GameDataProvider.GetInstance().GetStatGroups(gameID, playerID);
                    statGroups = dtoStatGroups.Select(sg => new StatGroup() { GroupID = sg.GroupID }).ToList();
                }
                return statGroups;
            }
            else
            {
                throw new UnauthorizedAccessException("nope");
            }
        }

        public List<PlayerStats> GetAllStats(int? teamID = null, int? sportID = null, int? leagueID = null, int? playerID = null, DataCache dataCache = null)
        {
            var stats = GameDataProvider.GetInstance().GetStats(teamID: teamID, leagueID: leagueID, playerID: playerID, dataCache: dataCache);
            var players = PlayerDataProvider.GetInstance().GetPlayers(teamID: teamID, leagueID: leagueID, playerID: playerID, dataCache: dataCache);
            List<Team> teamList = new List<Team>();
            if (teamID.HasValue)
            {
                teamList.Add(new Team()
                {
                    ID = teamID.Value,
                    LeagueID = leagueID.Value,
                    SportID = sportID.Value
                });
            }
            else
            {
                teamList = TeamsService.GetInstance().GetTeams(leagueID: leagueID, playerID: playerID, showAll: true, dataCache: dataCache);
            }
            var statTypes = GameDataProvider.GetInstance().GetStatTypes(sportID, dataCache: dataCache);
            var gameStats = stats.Select(s => ConvertObjects.ConvertType2(s)).ToList();

            var playerStats = new List<PlayerStats>();
            foreach (var player in players)
            {
                foreach (var team in teamList)
                {
                    var playerStat = new PlayerStats()
                    {
                        TeamID = team.ID,
                        LeagueID = team.LeagueID,
                        SportID = team.SportID,
                        PlayerName = player.Name,
                        PlayerNumber = teamID.HasValue ? player.Number : team.PlayerNumber,
                        GameID = 0,
                        PlayerID = player.ID,
                        Stats = new List<Stat>()
                    };

                    foreach (var statType in statTypes.Where(st => !st.IsCalculated && st.SportID == team.SportID))
                    {
                        var stat = stats.Where(s => s.StatTypeID == statType.StatTypeID && s.PlayerID == player.ID && s.LeagueID == team.LeagueID).Sum(s => s.Value);
                        playerStat.Stats.Add(new Stat()
                        {
                            Value = stat,
                            StatType = ConvertObjects.ConvertType(statType)
                        });
                    }

                    playerStats.Add(playerStat);
                }
            }

            foreach (var playerStat in playerStats)
            {
                var playerGameStats = gameStats.Where(g => g.PlayerID == playerStat.PlayerID && g.LeagueID == playerStat.LeagueID).ToList();
                foreach (var calcStat in statTypes.Where(st => st.IsCalculated && st.SportID == playerStat.SportID))
                {
                    playerStat.Stats.Add(new Stat()
                    {
                        StatType = ConvertObjects.ConvertType(calcStat),
                        Value = StatsCalculations.GetValue((CalculatedStatTypes)calcStat.StatTypeID, playerGameStats)
                    });
                }
            }
            return playerStats;
        }

        public List<State> GetStates(int sportID)
        {
            var statStates = GameDataProvider.GetInstance().GetStates(sportID);
            return statStates.Select(s => ConvertObjects.ConvertType(s)).ToList();
        }

        public PlayerStats GetPlayerStats(int playerID, DataCache dataCache = null)
        {
            var stats = GameDataProvider.GetInstance().GetStats(playerID: playerID, dataCache: dataCache);
            var player = PlayersService.GetInstance().GetPlayer(playerID, dataCache: dataCache);

            return new PlayerStats()
            {
                PlayerName = player.Name,
                PlayerID = player.ID,
                PlayerNumber = player.Number,
                Stats = stats.Select(s => new Stat()
                {
                    StatType = ConvertObjects.ConvertType(s),
                    GroupID = s.GroupID,
                    Value = s.Value,
                    LeagueID = s.LeagueID,
                    GameID = s.GameID,
                    TeamID = s.TeamID,
                    Sport = (SportsList)s.SportID
                }).ToList()
            };
        }
    }
}