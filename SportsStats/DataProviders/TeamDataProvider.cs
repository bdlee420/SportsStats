﻿using SportsStats.Common;
using SportsStats.Models.DTOObjects;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SportsStats.DataProviders
{
    public class TeamDataProvider : DataAccess
    {
        private static readonly TeamDataProvider _dataProvider = new TeamDataProvider();
        public static TeamDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new TeamDataProvider();
            }
            return _dataProvider;
        }
        public void AddTeam(DTOTeam team)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@Name", SqlDbType.VarChar, team.Name));
            parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, team.LeagueID));
            SQLExecuteCommandParam("AddTeam", parameters);
        }
		public void AddLeagueTeam(int leagueID, int teamID)
		{
			var parameters = new List<SqlParameter>();
			parameters.Add(CreateSqlParameter("@TeamID", SqlDbType.Int, teamID));
			parameters.Add(CreateSqlParameter("@LeagueID", SqlDbType.Int, leagueID));
			SQLExecuteCommandParam("AddLeagueTeam", parameters);
		}
		public List<DTOTeam> GetTeams(int? leagueID = null, int? playerID = null, int? teamID = null, int? sportID = null, DataCache dataCache = null)
        {
            var teams = new List<DTOTeam>();
            var dataTableResult = new DataTable();
            var cacheKey = new CacheKey(null, teamID, playerID, leagueID);
            if (dataCache != null && dataCache.TeamsResult != null && dataCache.TeamsResult.ContainsKey(cacheKey))
            {
                dataTableResult = dataCache.TeamsResult[cacheKey];
            }
            else
            {
                var parameters = new List<SqlParameter>();
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
				if (sportID.HasValue)
				{
					parameters.Add(CreateSqlParameter("@SportID", SqlDbType.Int, sportID.Value));
				}
				dataTableResult = SQLGetDataTable("GetTeams", parameters);
                if (dataCache == null)
                    dataCache = new DataCache();
                dataCache.TeamsResult.Add(cacheKey, dataTableResult);
            }
            foreach (DataRow dr in dataTableResult.Rows)
            {
                teams.Add(new DTOTeam()
                {
                    ID = (int)dr["ID"],
                    Name = dr["Name"].ToString(),
                    LeagueID = (int)dr["LeagueID"],
                    SportID = (int)dr["SportID"],
                    PlayerNumber = (int)dr["Number"],
                });
            }
            return teams;
        }
    }
}