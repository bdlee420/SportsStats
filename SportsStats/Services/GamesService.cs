using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using SportsStats.Common;
using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.ServiceObjects;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
    public class GamesService
    {
        private static readonly GamesService _service = new GamesService();

        public static GamesService GetInstance()
        {
            if (_service == null)
            {
                return new GamesService();
            }
            return _service;
        }

        public List<Game> GetGames(int? leagueID = null, int? playerID = null, int? teamID = null, DataCache dataCache = null)
        {
            if (UserHelper.HasGetPermissions(teamID: teamID, leagueID: leagueID))
            {
                var games = GameDataProvider.GetInstance().GetGames(leagueID: leagueID, playerID: playerID, teamID: teamID, dataCache: dataCache);
                var stats = GameDataProvider.GetInstance().GetStats(leagueID: leagueID, playerID: playerID, dataCache: dataCache);
                var gameStats = stats.Select(s => ConvertObjects.ConvertType2(s)).ToList();

                return games.Select(g =>
                {
                    var game = ConvertObjects.ConvertType(g);
                    return new Game()
                    {
                        ID = g.ID,
                        GameDate = g.GameDate,
                        Team1ID = g.Team1ID,
                        Team1Score = GetTeamScore(gameStats, game, true),
                        Team2ID = g.Team2ID,
                        Team2Score = GetTeamScore(gameStats, game, false)
                    };
                }).ToList();
            }
            else
            {
                return null;
            }
        }

        private int GetTeamScore(List<GameStat> gameStats, Game game, bool isTeam1)
        {
            int teamID = isTeam1 ? game.Team1ID : game.Team2ID;
            var teamGameStats = gameStats.Where(gs => gs.TeamID == teamID && gs.GameID == game.ID).ToList();
            int teamscore = 0;
            if (game.SportID == (int)SportsList.Basketball)
            {
                var fgMadeTeam = teamGameStats.Where(s => s.StatType.ID == (int)CalculatedStatTypes.TwoMade).Sum(s => s.Value);
                var ftMadeTeam = teamGameStats.Where(s => s.StatType.ID == (int)CalculatedStatTypes.FTMade).Sum(s => s.Value);
                var threeMadeTeam = teamGameStats.Where(s => s.StatType.ID == (int)CalculatedStatTypes.ThreeMade).Sum(s => s.Value);

                teamscore = (fgMadeTeam * 2) + (threeMadeTeam * 3) + ftMadeTeam;
            }
            else if (game.SportID == (int)SportsList.Baseball)
            {
                var test = teamGameStats.Where(gs => gs.StatType.ID == (int)CalculatedStatTypes.Runs).ToList();
                teamscore = teamGameStats.Where(gs => gs.StatType.ID == (int)CalculatedStatTypes.Runs).Sum(f => f.Value);
            }
            else if (game.SportID == (int)SportsList.Hockey)
            {
                teamscore = teamGameStats.Where(gs => gs.TeamID == teamID && gs.StatType.ID == (int)CalculatedStatTypes.HockeyGoal).Sum(f => f.Value);

                var team1shootoutgoals = gameStats.Where(gs => gs.GameID == game.ID && gs.TeamID == game.Team1ID && gs.StatType.ID == (int)CalculatedStatTypes.HockeyShootOutGoal).Sum(f => f.Value);
                var team2shootoutgoals = gameStats.Where(gs => gs.GameID == game.ID && gs.TeamID == game.Team2ID && gs.StatType.ID == (int)CalculatedStatTypes.HockeyShootOutGoal).Sum(f => f.Value);

                if (team1shootoutgoals > team2shootoutgoals && isTeam1)
                    teamscore++;

                if (team2shootoutgoals > team1shootoutgoals && !isTeam1)
                    teamscore++;
            }

            return teamscore;
        }

        public List<StatType> GetStatTypes(int? sportID = null, DataCache dataCache = null)
        {
            return GameDataProvider.GetInstance().GetStatTypes(sportID, dataCache)
                .Select(s => ConvertObjects.ConvertType(s)).ToList();
        }

        public Game GetGame(int gameID, DataCache dataCache = null)
        {
            var dtoGame = GameDataProvider.GetInstance().GetGames(gameID: gameID, dataCache: dataCache).First();
            var game = ConvertObjects.ConvertType(dtoGame);
            var validTeams = new List<int>() { game.Team1ID, game.Team2ID };
            var teams = TeamsService.GetInstance().GetTeams(validTeams: validTeams, dataCache: dataCache);

            if (UserHelper.HasGetPermissions(game))
            {
                var stats = GameDataProvider.GetInstance().GetStats(gameID: gameID, dataCache: dataCache);
                var statTypes = GameDataProvider.GetInstance().GetStatTypes(game.SportID, dataCache: dataCache);
                var players = PlayersService.GetInstance().GetPlayers(gameID: gameID, leagueID: game.LeagueID, dataCache: dataCache);
                var gameStats = stats.Select(s => ConvertObjects.ConvertType2(s)).ToList();
                var gameTotalStats = new List<PlayerStats>();
                var playerStats = new List<PlayerStats>();

                foreach (var player in players)
                {
                    var teamTotalStats = gameTotalStats.FirstOrDefault(ts => ts.TeamID == player.TeamID);
                    if (teamTotalStats == null)
                    {
                        teamTotalStats = new PlayerStats
                        {
                            TeamID = player.TeamID,
                            PlayerName = "TOTAL",
                            GameID = gameID,
                            PlayerID = -1,
                            Stats = new List<Stat>()
                        };
                        gameTotalStats.Add(teamTotalStats);
                    }

                    var playerStat = new PlayerStats()
                    {
                        GameID = gameID,
                        PlayerID = player.ID,
                        PlayerName = player.Name,
                        TeamID = player.TeamID,
                        Stats = new List<Stat>()
                    };

                    foreach (var statType in statTypes.Where(st => !st.IsCalculated))
                    {
                        var existingStat = gameStats.FirstOrDefault(s =>
                            s.StatType.ID == statType.StatTypeID &&
                            s.PlayerID == player.ID &&
                            s.TeamID == player.TeamID);

                        var existingTeamTotalStat = teamTotalStats.Stats.FirstOrDefault(s => s.StatType.ID == statType.StatTypeID);
                        if (existingTeamTotalStat == null)
                        {
                            existingTeamTotalStat = new Stat()
                            {
                                GameID = gameID,
                                TeamID = teamTotalStats.TeamID,
                                Value = 0,
                                StatType = ConvertObjects.ConvertType(statType)
                            };
                            teamTotalStats.Stats.Add(existingTeamTotalStat);
                        }

                        var stat = new GameStat()
                        {
                            GameID = gameID,
                            PlayerID = player.ID,
                            TeamID = player.TeamID,
                            Value = 0,
                            StatType = ConvertObjects.ConvertType(statType)
                        };

                        if (existingStat != null)
                        {
                            stat.Value = gameStats.Where(s =>
                                s.StatType.ID == statType.StatTypeID &&
                                s.PlayerID == player.ID &&
                                s.TeamID == player.TeamID).Sum(s => s.Value);

                            existingTeamTotalStat.Value += stat.Value;
                        }

                        stat.StatType.GridDisplayOrder = statType.GridDisplayOrder;
                        stat.StatType.SelectionDisplayOrder = statType.SelectionDisplayOrder;
                        playerStat.Stats.Add(ConvertObjects.ConvertType(stat));
                    }

                    playerStat.IsActivePlayer = false;
                    playerStat.IsInGame = false;
                    if (playerStat.Stats.Any())
                    {
                        playerStat.IsActivePlayer = playerStat.Stats
                            .Where(s => s.StatType.ID == (int)CalculatedStatTypes.IsActive)
                            .Sum(s => s.Value) > 0;

                        playerStat.IsInGame = playerStat.Stats
                            .Where(s => s.StatType.ID == (int)CalculatedStatTypes.IsInGame)
                            .Sum(s => s.Value) > 0;
                    }

                    playerStats.Add(playerStat);
                }

                int team1score = 0;
                int team2score = 0;

                team1score = GetTeamScore(gameStats, game, true);
                team2score = GetTeamScore(gameStats, game, false);

                var selectedGame = new Game()
                {
                    ID = game.ID,
                    GameDate = game.GameDate,
                    Team1ID = game.Team1ID,
                    Team1Score = team1score,
                    Team1Name = teams.First(t => t.ID == game.Team1ID).Name,
                    Team2ID = game.Team2ID,
                    Team2Score = team2score,
                    Team2Name = teams.First(t => t.ID == game.Team2ID).Name,
                    PlayerStats = playerStats,
                    TotalStats = gameTotalStats,
                    SportID = game.SportID
                };

                foreach (var ps in selectedGame.PlayerStats)
                {
                    var playerGameStats = gameStats.Where(g => g.PlayerID == ps.PlayerID).ToList();
                    foreach (var calcStat in statTypes.Where(st => st.IsCalculated))
                    {
                        ps.Stats.Add(new Stat()
                        {
                            StatType = ConvertObjects.ConvertType(calcStat),
                            Value = StatsCalculations.GetValue((CalculatedStatTypes)calcStat.StatTypeID, playerGameStats)
                        });
                    }
                }

                //Handle Total Stats

                var totalGameStats = gameTotalStats
                    .SelectMany(g => g.Stats)
                    .Select(s => ConvertObjects.ConvertType2(s))
                    .ToList();

                foreach (var ts in selectedGame.TotalStats)
                {
                    var teamGameStats = totalGameStats
                        .Where(tgs => tgs.TeamID == ts.TeamID)
                        .ToList();

                    foreach (var calcStat in statTypes.Where(st => st.IsCalculated))
                    {
                        ts.Stats.Add(new Stat()
                        {
                            StatType = ConvertObjects.ConvertType(calcStat),
                            Value = StatsCalculations.GetValue((CalculatedStatTypes)calcStat.StatTypeID, teamGameStats)
                        });
                    }
                }

                return selectedGame;
            }
            else
            {
                return null;
            }
        }

        public List<GameLog> GetGameLog(int gameID, DataCache dataCache = null)
        {
            var game = GameDataProvider.GetInstance().GetGameLog(gameID: gameID);
            return game.Select(g => ConvertObjects.ConvertType(g)).ToList();
        }

        public void AddGame(Game game)
        {
            if (UserHelper.HasUpdatePermissions(leagueID: game.LeagueID))
            {
                GameDataProvider.GetInstance().AddGame(ConvertObjects.ConvertType(game));
            }
        }

        public void UpdateGame(Game game)
        {
            if (UserHelper.HasUpdatePermissions(leagueID: game.LeagueID))
            {
                GameDataProvider.GetInstance().UpdateGame(ConvertObjects.ConvertType(game));
            }
        }

        public void AddStat(PlayerGameStat stat, BaseballGameState state = null)
        {
            if (UserHelper.HasUpdatePermissions(leagueID: stat.LeagueID))
            {
                stat.States = new List<int>();
                if (state != null)
                {
                    if (state.PlayerOnFirst != null)
                        stat.States.Add((int)StatStates.First);
                    if (state.PlayerOnSecond != null)
                        stat.States.Add((int)StatStates.Second);
                    if (state.PlayerOnThird != null)
                        stat.States.Add((int)StatStates.Third);
                    if (state.NumberOfOuts == 1)
                        stat.States.Add((int)StatStates.Out1);
                    if (state.NumberOfOuts == 2)
                        stat.States.Add((int)StatStates.Out2);
                }
                var gameStat = ConvertObjects.ConvertType(stat);
                GameDataProvider.GetInstance().AddStat(gameStat);
            }
        }
    }
}