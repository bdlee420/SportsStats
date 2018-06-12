using SportsStats.Common;
using SportsStats.Models.DTOObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SportsStats.DataProviders
{
    public class PlayerDataProvider : DataAccess
    {
        private static readonly PlayerDataProvider _dataProvider = new PlayerDataProvider();

        public static PlayerDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new PlayerDataProvider();
            }
            return _dataProvider;
        }

        public int AddPlayer(DTOPlayer player, int? teamID, int? leagueID)
        {
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(player.Name))
            {
                parameters.Add(CreateSqlParameter("@Name", SqlDbType.VarChar, player.Name));
            }
            parameters.Add(CreateSqlParameter("@Number", SqlDbType.Int, player.Number));
            if (teamID.HasValue)
            {
                parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID.Value));
            }
			if (leagueID.HasValue)
			{
				parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID.Value));
			}
			var outputParam = new SqlParameter();            
            if (player.ID > 0)
            {
                outputParam = CreateSqlParameter("@PlayerID", SqlDbType.Int, player.ID);
            }
            else
            {
                outputParam = CreateSqlParameter("@PlayerID", SqlDbType.Int);
            }
            outputParam.Direction = ParameterDirection.InputOutput;
            parameters.Add(outputParam);
            SQLExecuteCommandParam("AddPlayer", parameters);
            return Convert.ToInt32(outputParam.Value);
        }

        public void SavePlayer(DTOPlayer player)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@Name", SqlDbType.VarChar, player.Name));
            parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, player.ID));
            SQLExecuteCommandParam("SavePlayer", parameters);
        }

        public void SaveTeamPlayer(DTOPlayer player)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, player.TeamID));
            parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, player.ID));
            parameters.Add(CreateSqlParameter("@PlayerNumber", SqlDbType.Int, player.Number));
            SQLExecuteCommandParam("SaveTeamPlayer", parameters);
        }

        public List<DTOPlayer> GetPlayers(int? teamID = null, int? gameID = null, int? playerID = null, int? leagueID = null, DataCache dataCache = null)
        {
            var players = new List<DTOPlayer>();
            var dataTableResult = new DataTable();
            var cacheKey = new CacheKey(null, teamID, playerID, null);
            if (dataCache != null && dataCache.PlayersResult != null && dataCache.PlayersResult.ContainsKey(cacheKey))
            {
                dataTableResult = dataCache.PlayersResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
                if (teamID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID.Value));
                }
                if (gameID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@GameID", SqlDbType.Int, gameID.Value));
                }
                if (playerID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@PlayerID", SqlDbType.Int, playerID.Value));
                }
                if (leagueID.HasValue)
                {
                    parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID.Value));
                }
                dataTableResult = SQLGetDataTable("GetPlayers", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataCache.PlayersResult.Add(cacheKey, dataTableResult);
            }
            foreach (DataRow dr in dataTableResult.Rows)
            {
                players.Add(new DTOPlayer()
                {
                    ID = (int)dr["ID"],
                    Name = dr["Name"].ToString(),
                    Number = (int)dr["Number"],
                    TeamID = (int)dr["TeamID"]
                });
            }
            return players;
        }
    }
}