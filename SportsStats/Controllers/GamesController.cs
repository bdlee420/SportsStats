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
    public class GamesController : ApiController
    {
        public GamesResult GetGames(int leagueID)
        {
            try
            {
                return GetGamesResult(leagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public GameResult GetGame(int leagueID, int gameID)
        {
            try
            {
                return GetGameResult(leagueID, gameID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GameLogResult> GetGameLog(int gameID)
        {
            try
            {
                var gamelogs = GamesService.GetInstance().GetGameLog(gameID);
                return gamelogs.Select(g => ConvertObjects.ConvertType(g)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public GamesResult AddGame([FromBody] GameResultBase game)
        {
            GamesService.GetInstance().AddGame(ConvertObjects.ConvertType(game));
            return GetGamesResult(game.LeagueID);
        }

        [HttpPost]
        public GameResult UpdateGame([FromBody] GameResultBase game)
        {
            GamesService.GetInstance().UpdateGame(ConvertObjects.ConvertType(game));
            return GetGameResult(game.LeagueID, game.ID);
        }

        [ActionName("AddPlayer")]
        public int AddPlayer([FromBody] TeamPlayerSaveRequest saveRequest)
        {
            try
            {
                return PlayersService.GetInstance().AddPlayer(ConvertObjects.ConvertType(saveRequest.Player), saveRequest.TeamID, saveRequest.LeagueID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private GamesResult GetGamesResult(int leagueID)
        {
            var dataCache = new DataCache();
            var teams = TeamsService.GetInstance().GetTeams(leagueID: leagueID, showAll: true, dataCache: dataCache);
            var games = GamesService.GetInstance().GetGames(leagueID: leagueID, dataCache: dataCache);
            var gamesResult = new GamesResult()
            {
                Teams = teams.Select(t => new TeamsResult()
                {
                    ID = t.ID,
                    Name = t.Name
                }).ToList(),
                Games = games.Select(g => new GameResultBase()
                {
                    ID = g.ID,
                    GameDate = g.GameDate,
                    Team1ID = g.Team1ID,
                    Team1Name = teams.FirstOrDefault(tm1 => tm1.ID == g.Team1ID)?.Name,
                    Team1Score = g.Team1Score,
                    Team2ID = g.Team2ID,
                    Team2Name = teams.FirstOrDefault(tm2 => tm2.ID == g.Team2ID)?.Name,
                    Team2Score = g.Team2Score
                }).ToList()
            };
            return gamesResult;
        }

        private GameResult GetGameResult(int leagueID, int gameID)
        {
            try
            {
                var dataCache = new DataCache();
                var game = GamesService.GetInstance().GetGame(gameID, dataCache: dataCache);
                var teams = new List<int>() { game.Team1ID, game.Team2ID };
                var allTeams = TeamsService.GetInstance().GetTeams(leagueID: leagueID, validTeams: teams, dataCache: dataCache);
                var players = PlayersService.GetInstance().GetPlayers(gameID: gameID, leagueID: leagueID, dataCache: dataCache);
                var statTypes = GamesService.GetInstance().GetStatTypes(game.SportID, dataCache: dataCache).OrderBy(s => s.GridDisplayOrder);
                var gameState = BaseballService.GetInstance().GetExistingGameState(gameID, leagueID, dataCache: dataCache);

                var statTypesHash = statTypes
                    .Where(st => st.ShowGame)
                    .Select(s => s.ID)
                    .ToHashSet();

                foreach (var playerStat in game.PlayerStats)
                {
                    playerStat.Stats = playerStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                }

                foreach (var totalStat in game.TotalStats)
                {
                    totalStat.Stats = totalStat.Stats.Where(s => statTypesHash.Contains(s.StatType.ID)).ToList();
                }

                int? playerAtBat = gameState.TopOfInning ? gameState.Team1Player?.ID : gameState.Team2Player?.ID;

                var gameResult = new GameResult()
                {
                    ID = game.ID,
                    GameDate = game.GameDate,
                    StatTypes = statTypes.Select(s => ConvertObjects.ConvertType(s)).ToList(),
                    QuickStatTypes = statTypes
                        .Where(st => st.QuickButtonOrder > 0)
                        .Select(s => ConvertObjects.ConvertType(s))
                        .OrderBy(s => s.QuickButtonOrder)
                        .ToList(),
                    Team1ID = game.Team1ID,
                    Team1Score = game.Team1Score,
                    Team1Name = game.Team1Name,
                    Team2ID = game.Team2ID,
                    Team2Score = game.Team2Score,
                    Team2Name = game.Team2Name,
                    Team1PlayerStats = game.PlayerStats
                        .Where(p => p.TeamID == game.Team1ID)
                        .Select(g =>
                        {
                            var order = Convert.ToInt16(g.Stats.Where(s => s.StatType.ID == (int)CalculatedStatTypes.BattingOrder).Sum(s => s.Value));
                            return new PlayerStatsResult()
                            {
                                PlayerID = g.PlayerID,
                                PlayerName = players.FirstOrDefault(p => p.ID == g.PlayerID)?.Name,
                                PlayerNumber = players.FirstOrDefault(p => p.ID == g.PlayerID)?.Number ?? 0,
                                Order = order == 0 ? (int?)null : order,
                                AtBat = game.SportID == (int)SportsList.Baseball && g.PlayerID == playerAtBat,
                                IsActivePlayer = g.IsActivePlayer,
                                PlayerStats = g.Stats.OrderBy(s => s.StatType.GridDisplayOrder)
                                                     .Select(s => new StatResult()
                                                     {
                                                         StatTypeID = s.StatType.ID,
                                                         Name = s.StatType.Name,
                                                         DefaultShow = s.StatType.DefaultShow,
                                                         Value = s.Value,
                                                         ShowGame = s.StatType.ShowGame
                                                     }).ToList()
                            };
                        })
                        .OrderByDescending(p => p.IsActivePlayer)
                        .ToList(),
                    Team2PlayerStats = game.PlayerStats
                        .Where(p => p.TeamID == game.Team2ID)
                        .Select(g =>
                        {
                            var order = Convert.ToInt16(g.Stats.Where(s => s.StatType.ID == (int)CalculatedStatTypes.BattingOrder).Sum(s => s.Value));
                            return new PlayerStatsResult()
                            {
                                PlayerID = g.PlayerID,
                                PlayerName = players.FirstOrDefault(p => p.ID == g.PlayerID)?.Name,
                                PlayerNumber = players.FirstOrDefault(p => p.ID == g.PlayerID)?.Number ?? 0,
                                Order = order == 0 ? (int?)null : order,
                                AtBat = game.SportID == (int)SportsList.Baseball && g.PlayerID == playerAtBat,
                                IsActivePlayer = g.IsActivePlayer,
                                PlayerStats = g.Stats.OrderBy(s => s.StatType.GridDisplayOrder)
                                                     .Select(s => new StatResult()
                                                     {
                                                         StatTypeID = s.StatType.ID,
                                                         Name = s.StatType.Name,
                                                         DefaultShow = s.StatType.DefaultShow,
                                                         Value = s.Value,
                                                         ShowGame = s.StatType.ShowGame
                                                     }).ToList()
                            };
                        })
                        .OrderByDescending(p => p.IsActivePlayer)
                        .ToList(),
                    TotalTeam1Stats = game.TotalStats
                        .Where(ts => ts.TeamID == game.Team1ID)
                        .Select(g =>
                        {
                            return new PlayerStatsResult()
                            {
                                PlayerID = -1,
                                PlayerName = "Total",
                                PlayerNumber = 0,
                                PlayerStats = g.Stats.OrderBy(s => s.StatType.GridDisplayOrder)
                                                     .Select(s => new StatResult()
                                                     {
                                                         StatTypeID = s.StatType.ID,
                                                         Name = s.StatType.Name,
                                                         DefaultShow = s.StatType.DefaultShow,
                                                         Value = s.Value,
                                                         ShowGame = s.StatType.ShowGame
                                                     }).ToList()
                            };
                        })
                        .ToList(),
                    TotalTeam2Stats = game.TotalStats
                        .Where(ts => ts.TeamID == game.Team2ID)
                        .Select(g =>
                        {
                            return new PlayerStatsResult()
                            {
                                PlayerID = -1,
                                PlayerName = "Total",
                                PlayerNumber = 0,
                                PlayerStats = g.Stats.OrderBy(s => s.StatType.GridDisplayOrder)
                                                     .Select(s => new StatResult()
                                                     {
                                                         StatTypeID = s.StatType.ID,
                                                         Name = s.StatType.Name,
                                                         DefaultShow = s.StatType.DefaultShow,
                                                         Value = s.Value,
                                                         ShowGame = s.StatType.ShowGame
                                                     }).ToList()
                            };
                        })
                        .ToList(),
                    Teams = allTeams.Select(t => new TeamsResult()
                    {
                        ID = t.ID,
                        Name = t.Name
                    })
                    .ToList()
                };

                if (game.SportID == (int)SportsList.Baseball)
                {
                    gameResult.Team1PlayerStats = gameResult.Team1PlayerStats.OrderBy(p => p.Order == 0 || p.Order == null ? 99 : p.Order).ToList();
                    gameResult.Team2PlayerStats = gameResult.Team2PlayerStats.OrderBy(p => p.Order == 0 || p.Order == null ? 99 : p.Order).ToList();
                    gameResult.BaseballGameStateResult = ConvertObjects.ConvertType(BaseballService.GetInstance().GetExistingGameState(gameID, leagueID));
                }

                return gameResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}