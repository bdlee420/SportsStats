using SportsStats.Models.DTOObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SportsStats.DataProviders
{
    public class UserDataProvider : DataAccess
    {
        private static readonly UserDataProvider _dataProvider = new UserDataProvider();
        public static UserDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new UserDataProvider();
            }
            return _dataProvider;
        }
        public DTOUser GetUser(DTOUser user)
        {
            DTOUser userObject = new DTOUser();
            var parameters = new List<SqlParameter>();
            parameters.Add(CreateSqlParameter("@UserName", SqlDbType.VarChar, user.UserName));
            if (!String.IsNullOrEmpty(user.Password))
                parameters.Add(CreateSqlParameter("@Password", SqlDbType.VarChar, user.Password));
            var dataSet = SQLGetDataSet("GetUser", parameters);
            if (dataSet.Tables.Count == 4)
            {
                var userInfo = dataSet.Tables[0];
                userObject.UserName = user.UserName;
                userObject.RoleID = userInfo.Rows[0]["RoleID"] == DBNull.Value ? 0 : (int)userInfo.Rows[0]["RoleID"];
                userObject.Teams = new List<int>();
                userObject.Leagues = new List<int>();
                userObject.Players = new List<int>();
                userObject.Sports = new List<int>();
                userObject.AdminLeagueIDs = new List<int>();
                var leagues = dataSet.Tables[1];
                foreach (DataRow dr in leagues.Rows)
                {
                    var leagueID = (int)dr["LeagueID"];
                    var roleID = (int)dr["RoleID"];
                    if (!userObject.AdminLeagueIDs.Contains(leagueID) && roleID == 1)
                        userObject.AdminLeagueIDs.Add(leagueID);
                }
                var teams = dataSet.Tables[2];
                foreach (DataRow dr in teams.Rows)
                {
                    var teamID = (int)dr["TeamID"];
                    var leagueID = (int)dr["LeagueID"];
                    var sportID = (int)dr["SportID"];
                    if (!userObject.Teams.Contains(teamID))
                        userObject.Teams.Add(teamID);
                    if (!userObject.Leagues.Contains(leagueID))
                        userObject.Leagues.Add(leagueID);
                    if (!userObject.Sports.Contains(sportID))
                        userObject.Sports.Add(sportID);
                }
                var players = dataSet.Tables[3];
                foreach (DataRow dr in players.Rows)
                {
                    userObject.Players.Add((int)dr["PlayerID"]);
                }
            }
            return userObject;
        }
    }
}