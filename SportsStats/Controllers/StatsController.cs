using SportsStats.Common;
using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Models.ServiceObjects;
using SportsStats.Services;
using System.Linq;
using System.Web.Http;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Controllers
{
    public class StatsController : ApiController
    {
        public StatsResult GetStats(int sportID, int? playerID, int gameID, int teamID, int? groupID, int leagueID)
        {
            var statsResult = GetStatTypes(sportID, playerID, gameID, teamID, groupID, leagueID);
            if (sportID == (int)SportsList.Baseball)
            {
                statsResult.BaseballGameStateResult = ConvertObjects.ConvertType(BaseballService.GetInstance().GetExistingGameState(gameID, statsResult.LeagueID));
                statsResult.BaseballGameStateResult.IsStatsPage = true;
            }
            return statsResult;
        }

        [HttpPost]
        public void SaveOrder([FromBody]StatRequest statRequest)
        {
            var gameStat = ConvertObjects.ConvertType(statRequest);
            gameStat.Override = true;
            GamesService.GetInstance().AddStat(gameStat);
        }

        [HttpPost]
        public BaseballGameStateResult AddStat([FromBody]StatRequest statRequest)
        {
            var dataCache = new DataCache();
            var gameStat = ConvertObjects.ConvertType(statRequest);

            AtBatStates gameStates = null;
            if (statRequest.IsBaseball)
                gameStates = BaseballService.GetInstance().GetGameStates(gameStat, statRequest.IsLatestGroup, dataCache);

            NextBatterResult nextBatter = null;
            bool dataSaved = false;
            if (gameStates == null || (!gameStates.NewState.PotentialAdjustment && !gameStates.NewState.InningChanged))
            {
                dataSaved = true;
                GamesService.GetInstance().AddStat(gameStat, gameStates?.OriginalState);
                if (statRequest.IsBaseball && gameStates.NewState.ChangeState)
                {
                    nextBatter = ConvertObjects.ConvertType(BaseballService.GetInstance().SaveNewGameState(gameStates.NewState, false, dataCache: dataCache));
                    //Create new at bat group for player
                    StatsService.GetInstance().CreateNewStatGroup((int)SportsList.Baseball, gameStat.GameID, gameStat.PlayerID, gameStat.TeamID);
                }
            }

            if (statRequest.IsBaseball)
            {
                var gameStateResult = ConvertObjects.ConvertType(gameStates.NewState);
                gameStateResult.GameStat = statRequest;
                gameStateResult.NextBatter = nextBatter;
                gameStateResult.DataSaved = dataSaved;
                gameStateResult.IsStatsPage = true;
                return gameStateResult;
            }
            else
            {
                return new BaseballGameStateResult() { DataSaved = true };
            }
        }

        [HttpPost]
        public NextBatterResult SaveNewGameState([FromBody]BaseballGameStateResult gameState)
        {
            var dataCache = new DataCache();
            if (gameState.GameStat != null)
            {
                var originalState = BaseballService.GetInstance().GetExistingGameState(gameID: gameState.GameID, leagueID: gameState.LeagueID, dataCache: dataCache);
                GamesService.GetInstance().AddStat(ConvertObjects.ConvertType(gameState.GameStat), originalState);
                int rbis = 0;
                foreach (var player in gameState.RunnersScored)
                {
                    rbis++;
                    GamesService.GetInstance().AddStat(ConvertObjects.ConvertType(new StatRequest()
                    {
                        GameID = gameState.GameID,
                        PlayerID = player.ID,
                        StatTypeID = (int)CalculatedStatTypes.Runs,
                        TeamID = gameState.GameStat.TeamID,
                        Value = 1,
                        GroupID = gameState.GameStat.GroupID,
                    }));
                }

                if (rbis > 0)
                {
                    GamesService.GetInstance().AddStat(ConvertObjects.ConvertType(new StatRequest()
                    {
                        GameID = gameState.GameID,
                        PlayerID = gameState.GameStat.PlayerID,
                        StatTypeID = (int)CalculatedStatTypes.RBI,
                        TeamID = gameState.GameStat.TeamID,
                        Value = rbis,
                        GroupID = gameState.GameStat.GroupID,
                    }), originalState);
                }
            }
            bool manualAdjustment = gameState.GameStat == null;
            var result = BaseballService.GetInstance().SaveNewGameState(ConvertObjects.ConvertType(gameState), manualAdjustment, dataCache: dataCache);
            if (gameState.GameStat != null)
            {
                //Create new at bat group for player
                StatsService.GetInstance().CreateNewStatGroup((int)SportsList.Baseball, gameState.GameID, gameState.GameStat.PlayerID, gameState.GameStat.TeamID);
            }
            return ConvertObjects.ConvertType(result);
        }

        [HttpPost]
        public bool NewGroup([FromBody]StatRequest statRequest)
        {
            return StatsService.GetInstance().CreateNewStatGroup((int)SportsList.Baseball, statRequest.GameID, statRequest.PlayerID, statRequest.TeamID);
        }

        private bool IsValidStatWithGroup(Models.ServiceObjects.StatType statType, int? groupID, int? statGroupID)
        {
            if (statType.ID == (int)CalculatedStatTypes.BattingOrder || statType.ID == (int)CalculatedStatTypes.Runs)
                return true;

            if (groupID == statGroupID)
                return true;

            return false;
        }

        private StatsResult GetStatTypes(int sportID, int? playerID, int gameID, int teamID, int? groupID, int leagueID)
        {
            var dataCache = new DataCache();
            var statTypes = GamesService.GetInstance().GetStatTypes(sportID, dataCache: dataCache);
            var playerStats = StatsService.GetInstance().GetStats(sportID, gameID, playerID, teamID, leagueID, dataCache: dataCache);
            var game = GamesService.GetInstance().GetGame(gameID, dataCache: dataCache);

            int? statGroupID = null;
            if (groupID.HasValue)
            {
                statGroupID = groupID.Value;
            }
            else
            {
                statGroupID = StatsService.GetInstance().GetStatGroup(sportID, gameID, playerID, teamID)?.GroupID;
            }
            var statGroups = StatsService.GetInstance().GetStatGroups(sportID, gameID, playerID);
            var states = StatsService.GetInstance().GetStates(sportID);

            var statTypesResult = statTypes
                .Where(st => !st.IsCalculated)
                .Select(t => new StatTypesResult()
                {
                    ID = t.ID,
                    Name = t.Name,
                    DisplayName = t.DisplayName,
                    DefaultShow = t.DefaultShow,
                    CurrentValue = playerStats.Stats
                                    .Where(ps => ps.StatType?.ID == t.ID && IsValidStatWithGroup(ps.StatType, statGroupID, ps.GroupID))
                                    .Sum(ps => ps.Value),
                    GridDisplayOrder = t.GridDisplayOrder,
                    SelectionDisplayOrder = t.SelectionDisplayOrder,
                    AutoGenerated = t.AutoGenerated
                })
                .OrderBy(s => s.SelectionDisplayOrder);

            var statGroupsResult = statGroups.Select(s => new StatGroupResult()
            {
                GroupID = s.GroupID
            }).ToList();

            int i = 1;
            foreach (var result in statGroupsResult)
            {
                result.GroupName = "AB " + i++;
            }

            return new StatsResult()
            {
                GroupID = statGroupID,
                PlayerID = playerStats.PlayerID,
                PlayerNumber = playerStats.PlayerNumber,
                PlayerName = playerStats.PlayerName,
                StatTypes = statTypesResult.ToList(),
                StatGroups = statGroupsResult,
                GameResult = ConvertObjects.ConvertType2(game),
                LeagueID = game.LeagueID
            };
        }
    }
}