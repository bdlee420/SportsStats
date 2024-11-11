using SportsStats.Common;
using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.ServiceObjects;
using System.Collections.Generic;
using System.Linq;

namespace SportsStats.Services
{
	public class TeamsService
	{
		private static readonly TeamsService _service = new TeamsService();

		public static TeamsService GetInstance()
		{
			if (_service == null)
			{
				return new TeamsService();
			}
			return _service;
		}

		public List<Team> GetTeams(List<int> validTeams = null, int? leagueID = null, int? playerID = null, int? sportID = null, bool showAll = false, DataCache dataCache = null)
		{
			var teams = TeamDataProvider.GetInstance().GetTeams(leagueID: leagueID, playerID: playerID, sportID: sportID, dataCache: dataCache);
			return teams
				.Where(t => UserHelper.HasGetPermissions(teamID: t.ID) || showAll || (validTeams?.Contains(t.ID) ?? false))
				.Select(t => new Team()
				{
					ID = t.ID,
					Name = t.Name,
					LeagueID = t.LeagueID,
					SportID = t.SportID,
					PlayerNumber = t.PlayerNumber
				})
				.ToList();
		}

		public Team GetTeam(int teamID, int leagueID, DataCache dataCache = null)
		{
			var team = TeamDataProvider.GetInstance().GetTeams(teamID: teamID, leagueID: leagueID, dataCache: dataCache).First();
			var players = PlayerDataProvider.GetInstance().GetPlayers(teamID: teamID, leagueID: leagueID, dataCache: dataCache);
			var games = GamesService.GetInstance().GetGames(teamID: teamID, leagueID: leagueID, dataCache: dataCache);

			var selectedTeam = new Team()
			{
				ID = team.ID,
				Name = team.Name,
				Players = players.Select(p => new Player()
				{
					ID = p.ID,
					Name = p.Name,
					Number = p.Number
				}).ToList(),
				Games = games,
				LeagueID = team.LeagueID,
				SportID = team.SportID
			};

			return selectedTeam;
		}

		public void AddTeam(Team team)
		{
			if (UserHelper.HasUpdatePermissions(team.ID, team.LeagueID))
			{
				TeamDataProvider.GetInstance().AddTeam(ConvertObjects.ConvertType(team));
			}
		}

		public void AddLeagueTeam(int teamID, int leagueID)
		{
			if (UserHelper.HasUpdatePermissions(teamID, leagueID))
			{
				TeamDataProvider.GetInstance().AddLeagueTeam(leagueID, teamID);
			}
		}
	}
}