using SportsStats.Models.DTOObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SportsStats.DataProviders
{
    public class LeagueDataProvider : DataAccess
    {
        private static readonly LeagueDataProvider _dataProvider = new LeagueDataProvider();
        public static LeagueDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new LeagueDataProvider();
            }
            return _dataProvider;
        }
        public List<DTOLeagues> GetLeagues(int? sportID = null)
        {
            var leagues = new List<DTOLeagues>();
            var parameters = new List<SqlParameter>();
            if (sportID.HasValue)
            {
                parameters.Add(CreateSqlParameter("@SportID", SqlDbType.Int, sportID.Value));
            }
            var result = SQLGetDataTable("GetLeagues", parameters);
            foreach (DataRow dr in result.Rows)
            {
                leagues.Add(new DTOLeagues()
                {
                    ID = (int)dr["ID"],
                    Name = dr["Name"].ToString(),
                    StartDate = (DateTime)dr["StartDate"],
                    EndDate = (DateTime)dr["EndDate"],
                    SeasonID = (int)dr["SeasonID"],
                    SportID = (int)dr["SportID"],
                });
            }
            return leagues;
        }
        public int GetLeagueId(int gameID)
        {
            var parameters = new List<SqlParameter>
            {
                CreateSqlParameter("@GameID", SqlDbType.Int, gameID)
            };

            var result = SQLGetDataTable("GetLeagueID", parameters);
            foreach (DataRow dr in result.Rows)
            {
                return (int)dr["ID"];
            }
            return 0;
        }
    }
}