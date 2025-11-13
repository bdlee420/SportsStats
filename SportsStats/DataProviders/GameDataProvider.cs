using SportsStats.Common;
using SportsStats.Models.DTOObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SportsStats.DataProviders
{
    public class GameDataProvider : DataAccess
    {
        private static readonly GameDataProvider _dataProvider = new GameDataProvider();

        public static GameDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new GameDataProvider();
            }
            return _dataProvider;
        }

        public void AddGame(DTOGame game)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameDate", System.Data.SqlDbType.DateTime, game.GameDate));
            parameters.Add(CreateSqlParameter("@Team1ID", System.Data.SqlDbType.Int, game.Team1ID));
            parameters.Add(CreateSqlParameter("@Team2ID", System.Data.SqlDbType.Int, game.Team2ID));
            parameters.Add(CreateSqlParameter("@LeagueID", System.Data.SqlDbType.Int, game.LeagueID));
            SQLExecuteCommandParam("AddGame", parameters);
        }

        public void UpdateGame(DTOGame game)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", System.Data.SqlDbType.Int, game.ID));
            parameters.Add(CreateSqlParameter("@GameDate", System.Data.SqlDbType.DateTime, game.GameDate));
            parameters.Add(CreateSqlParameter("@Team1ID", System.Data.SqlDbType.Int, game.Team1ID));
            parameters.Add(CreateSqlParameter("@Team2ID", System.Data.SqlDbType.Int, game.Team2ID));
            SQLExecuteCommandParam("UpdateGame", parameters);
        }

        public void AddStat(DTOStat stat)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, stat.GameID));
            if (stat.PlayerID.HasValue)
                parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, stat.PlayerID));
            parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, stat.TeamID));
            parameters.Add(CreateSqlParameter("@StatTypeID", SqlDbType.Int, stat.StatTypeID));
            parameters.Add(CreateSqlParameter("@Value", SqlDbType.Int, stat.Value));
            parameters.Add(CreateSqlParameter("@Override", SqlDbType.Bit, stat.Override));
            if (stat.GroupID.HasValue)
                parameters.Add(CreateSqlParameter("@GroupID", SqlDbType.Int, stat.GroupID));
            parameters.Add(CreateSqlParameter("@States", SqlDbType.VarChar, stat.States == null ? null : String.Join(",", stat.States.Select(s => s))));
            SQLExecuteCommandParam("AddStat", parameters);
        }

        public List<DTOGame> GetGames(int? leagueID = null, int? playerID = null, int? teamID = null, int? gameID = null, DataCache dataCache = null)
        {
            var games = new List<DTOGame>();
            var dataTableResult = new DataTable();
            var cacheKey = new CacheKey(gameID, teamID, playerID, leagueID);
            if (dataCache != null && dataCache.GameResult != null && dataCache.GameResult.ContainsKey(cacheKey))
            {
                dataTableResult = dataCache.GameResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
                if (gameID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID.Value));
                }
                if (leagueID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID.Value));
                }
                if (playerID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, playerID.Value));
                }
                if (teamID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID.Value));
                }
                dataTableResult = SQLGetDataTable("GetGames", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataCache.GameResult.Add(cacheKey, dataTableResult);
            }
            foreach (DataRow dr in dataTableResult.Rows)
            {
                games.Add(new DTOGame()
                {
                    ID = (int)dr["ID"],
                    GameDate = (DateTime)dr["GameDate"],
                    Team1ID = (int)dr["Team1ID"],
                    Team2ID = (int)dr["Team2ID"],
                    SportID = (int)dr["SportID"],
                    LeagueID = (int)dr["LeagueID"]
                });
            }
            return games;
        }

        public int GetGameCount(int teamID, int leagueID)
        {
            var parameters = new List<SqlParameter>
            {
                CreateSqlParameter("@TeamID", SqlDbType.Int, teamID),
                CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID)
            };

            var result = SQLGetDataTable("GetGameCount", parameters);
            foreach (DataRow dr in result.Rows)
            {
                return (int)dr["GameCount"];
            }
            return 0;
        }

        public List<DTOStatExtended> GetStats(int? gameID = null, int? playerID = null, int? teamID = null, int? leagueID = null, DataCache dataCache = null)
        {
            var stats = new List<DTOStatExtended>();
            var dataSetResult = new DataSet();
            var dataTableResultStats = new DataTable();
            var dataTableResultStates = new DataTable();
            var cacheKey = new CacheKey(gameID, teamID, playerID, null);
            if (dataCache != null && dataCache.StatsResult != null && dataCache.StatsResult.ContainsKey(cacheKey))
            {
                dataTableResultStats = dataCache.StatsResult[cacheKey];
                dataTableResultStates = dataCache.StatesResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
                if (gameID.HasValue)
                    parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
                if (playerID.HasValue)
                    parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, playerID.Value));
                if (teamID.HasValue)
                    parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID.Value));
                if (leagueID.HasValue)
                    parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID.Value));
                dataSetResult = SQLGetDataSet("GetStats", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataTableResultStats = dataSetResult.Tables[0];
                dataTableResultStates = dataSetResult.Tables[1];
                dataCache.StatsResult.Add(cacheKey, dataTableResultStats);
                dataCache.StatesResult.Add(cacheKey, dataTableResultStates);
            }
            foreach (DataRow dr in dataTableResultStats.Rows)
            {
                var stat = new DTOStatExtended()
                {
                    ID = (int)dr["ID"],
                    GameID = (int)dr["GameID"],
                    LeagueID = (int)dr["LeagueID"],
                    SportID = (int)dr["SportID"],
                    GroupID = dr["GroupID"] == DBNull.Value ? (int?)null : (int)dr["GroupID"],
                    Value = (int)dr["Value"],
                    StatTypeID = (int)dr["StatTypeID"],
                    PlayerID = dr["PlayerID"] == DBNull.Value ? (int?)null : (int)dr["PlayerID"],
                    TeamID = (int)dr["TeamID"],
                    DefaultShow = (bool)dr["DefaultShow"],
                    DisplayName = dr["DisplayName"].ToString(),
                    IsCalculated = (bool)dr["IsCalculated"],
                    AutoGenerated = (bool)dr["IsCalculated"],
                    Name = dr["Name"].ToString(),
                };

                string filter = string.Format("GameID = {0} AND PlayerID = {1} AND StatID = {2}", stat.GameID, stat.PlayerID.HasValue ? stat.PlayerID.ToString() : "null", stat.ID);
                foreach (DataRow drState in dataTableResultStates.Select(filter))
                {
                    int stateID = (int)drState["StateID"];
                    stat.States.Add(stateID);
                }

                stats.Add(stat);
            }
            return stats;
        }

        public DTOStatGroup GetStatGroup(int gameID, int? playerID, int teamID, bool newGroup)
        {
            var statGroup = new DTOStatGroup();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
            if (playerID.HasValue)
                parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, playerID));
            parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID));
            parameters.Add(CreateSqlParameter("@New", SqlDbType.Bit, newGroup));
            var result = SQLGetDataTable("GetStatGroup", parameters);

            foreach (DataRow dr in result.Rows)
            {
                statGroup.GroupID = (int)dr["GroupID"];
            }

            return statGroup;
        }

        public List<DTOGameLog> GetGameLog(int gameID)
        {
            var gameLogs = new List<DTOGameLog>();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
            var result = SQLGetDataTable("GetGameLog", parameters);

            foreach (DataRow dr in result.Rows)
            {
                gameLogs.Add(new DTOGameLog()
                {
                    TeamName = dr["TeamName"].ToString(),
                    PlayerName = dr["PlayerName"].ToString(),
                    DisplayName = dr["DisplayName"].ToString(),
                    Value = (int)dr["Value"],
                    Timestamp = (DateTime)dr["Timestamp"]
                });
            }

            return gameLogs;
        }

        public List<DTOStatGroup> GetStatGroups(int gameID, int? playerID)
        {
            var statGroups = new List<DTOStatGroup>();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
            if (playerID.HasValue)
                parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, playerID));
            var result = SQLGetDataTable("GetStatGroups", parameters);

            foreach (DataRow dr in result.Rows)
            {
                statGroups.Add(new DTOStatGroup()
                {
                    GroupID = dr["GroupID"] == DBNull.Value ? (int?)null : (int)dr["GroupID"]
                });
            }

            return statGroups;
        }

        public List<DTOStatType> GetStatTypes(int? sportID, DataCache dataCache = null)
        {
            var statTypes = new List<DTOStatType>();
            var dataTableResult = new DataTable();
            var cacheKey = new CacheKey(null, null, null, null, sportID);
            if (dataCache != null && dataCache.StatTypesResult != null && dataCache.StatTypesResult.ContainsKey(cacheKey))
            {
                dataTableResult = dataCache.StatTypesResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
                if (sportID.HasValue)
                    parameters.Add(CreateSqlParameter("@SportID", SqlDbType.Int, sportID));
                dataTableResult = SQLGetDataTable("GetStatTypes", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataCache.StatTypesResult.Add(cacheKey, dataTableResult);
            }
            foreach (DataRow dr in dataTableResult.Rows)
            {
                statTypes.Add(new DTOStatType()
                {
                    SportID = (int)dr["SportID"],
                    StatTypeID = (int)dr["ID"],
                    Name = dr["Name"].ToString(),
                    DisplayName = dr["DisplayName"].ToString(),
                    DefaultShow = (bool)dr["DefaultShow"],
                    IsCalculated = (bool)dr["IsCalculated"],
                    GridDisplayOrder = (int)dr["GridDisplayOrder"],
                    SelectionDisplayOrder = (int)dr["SelectionDisplayOrder"],
                    ValueType = (int)dr["ValueType"],
                    QuickButtonOrder = dr["QuickButtonOrder"] != System.DBNull.Value ? (int)dr["QuickButtonOrder"] : -1,
                    QuickButtonText = dr["IsPositive"] != System.DBNull.Value ? dr["QuickButtonText"].ToString() : String.Empty,
                    IsPositive = dr["IsPositive"] != System.DBNull.Value ? (bool)dr["IsPositive"] : true,
                    AutoGenerated = (bool)dr["AutoGenerated"],
                    ShowGame = (bool)dr["ShowGame"],
                    ShowTeam = (bool)dr["ShowTeam"],
                    ShowPlayer = (bool)dr["ShowPlayer"],
                });
            }
            return statTypes;
        }

        public List<DTOState> GetStates(int sportID)
        {
            var statStates = new List<DTOState>();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@SportID", SqlDbType.Int, sportID));
            var result = SQLGetDataTable("GetStates", parameters);
            foreach (DataRow dr in result.Rows)
            {
                statStates.Add(new DTOState()
                {
                    ID = (int)dr["ID"],
                    Name = dr["Name"].ToString(),
                    DisplayOrder = (int)dr["DisplayOrder"],
                });
            }
            return statStates;
        }

        public List<DTOBattingOrder> GetBattingOrder(int gameID, int teamID)
        {
            var battingOrder = new List<DTOBattingOrder>();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID));
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
            var result = SQLGetDataTable("GetBattingOrder", parameters);
            foreach (DataRow dr in result.Rows)
            {
                battingOrder.Add(new DTOBattingOrder()
                {
                    PlayerID = (int)dr["PlayerID"],
                    BattingOrder = dr["BattingOrder"] == DBNull.Value ? 0 : (int)dr["BattingOrder"],
                });
            }
            return battingOrder;
        }

        public DTOBaseballGameState GetBaseballGameState(int gameID, DataCache dataCache = null)
        {
            var state = new DTOBaseballGameState();
            var dataTableResult = new DataTable();
            var cacheKey = new CacheKey(gameID, null, null, null, null);
            if (dataCache != null && dataCache.BaseballStateResult != null && dataCache.BaseballStateResult.ContainsKey(cacheKey))
            {
                dataTableResult = dataCache.BaseballStateResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
                parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID));
                dataTableResult = SQLGetDataTable("GetBaseballGameState", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataCache.BaseballStateResult.Add(cacheKey, dataTableResult);
            }
            if (dataTableResult.Rows.Count == 1)
            {
                var dr = dataTableResult.Rows[0];
                state.GameID = (int)dr["GameID"];
                state.Inning = (int)dr["Inning"];
                state.NumberOfOuts = (int)dr["Outs"];
                state.PlayerOnFirst = dr["FirstPlayerID"] == DBNull.Value ? (int?)null : (int)dr["FirstPlayerID"];
                state.PlayerOnSecond = dr["SecondPlayerID"] == DBNull.Value ? (int?)null : (int)dr["SecondPlayerID"];
                state.PlayerOnThird = dr["ThirdPlayerID"] == DBNull.Value ? (int?)null : (int)dr["ThirdPlayerID"];
                state.Team1PlayerID = dr["Team1PlayerID"] == DBNull.Value ? (int?)null : (int)dr["Team1PlayerID"];
                state.Team2PlayerID = dr["Team2PlayerID"] == DBNull.Value ? (int?)null : (int)dr["Team2PlayerID"];
                state.TopOfInning = (bool)dr["TopOfInning"];
            }
            return state;
        }

        public void SaveBaseballGameState(DTOBaseballGameState state)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, state.GameID));
            parameters.Add(CreateSqlParameter("@Inning", SqlDbType.Int, state.Inning));
            parameters.Add(CreateSqlParameter("@TopOfInning", SqlDbType.Bit, state.TopOfInning));
            parameters.Add(CreateSqlParameter("@FirstPlayerID", SqlDbType.Int, state.PlayerOnFirst));
            parameters.Add(CreateSqlParameter("@SecondPlayerID", SqlDbType.Int, state.PlayerOnSecond));
            parameters.Add(CreateSqlParameter("@ThirdPlayerID", SqlDbType.Int, state.PlayerOnThird));
            parameters.Add(CreateSqlParameter("@Outs", SqlDbType.Int, state.NumberOfOuts));
            parameters.Add(CreateSqlParameter("@Team1PlayerID", SqlDbType.Int, state.Team1PlayerID));
            parameters.Add(CreateSqlParameter("@Team2PlayerID", SqlDbType.Int, state.Team2PlayerID));
            SQLExecuteCommandParam("SaveBaseballGameState", parameters);
        }
    }
}