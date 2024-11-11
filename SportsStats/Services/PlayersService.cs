using SportsStats.Common;
using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.DTOObjects;
using SportsStats.Models.ServiceObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SportsStats.Services
{
    public class PlayersService
    {
        private static readonly PlayersService _ps = new PlayersService();

        public static PlayersService GetInstance()
        {
            if (_ps == null)
            {
                return new PlayersService();
            }
            return _ps;
        }

        public List<Player> GetPlayers(int? teamID = null, int? gameID = null, int? leagueID = null, DataCache dataCache = null)
        {
            var players = PlayerDataProvider.GetInstance().GetPlayers(teamID: teamID, gameID: gameID, leagueID: leagueID, dataCache: dataCache);			

			if (gameID.HasValue)
			{
				var dtoGame = GameDataProvider.GetInstance().GetGames(gameID: gameID, dataCache: dataCache).First();
				var game = ConvertObjects.ConvertType(dtoGame);
				var validTeams = new List<int>() { game.Team1ID, game.Team2ID };
				if (!players.Exists(p => p.TeamID == game.Team1ID))
				{
					//ADD TEAM PLAYER
					var dtoPlayer = new DTOPlayer()
					{
						Name = "Team 1 Player",
						TeamID = game.Team1ID
					};
					players.Add(dtoPlayer);
				}

				if (!players.Exists(p => p.TeamID == game.Team2ID))
				{
					//ADD TEAM PLAYER
					var dtoPlayer = new DTOPlayer()
					{
						Name = "Team 2 Player",
						TeamID = game.Team2ID
					};
					players.Add(dtoPlayer);
				}
			}

			return players
                    .Where(p => UserHelper.HasGetPermissions(playerID: p.ID, gameID: gameID))
                    .Select(p => new Player()
                    {
                        ID = p.ID,
                        Name = p.Name,
                        Number = p.Number,
						TeamID = p.TeamID
                    })
                    .ToList();
        }

        public Player GetPlayer(int playerID, DataCache dataCache = null)
        {
            if (UserHelper.HasGetPermissions(playerID: playerID))
            {
                var allPlayers = PlayerDataProvider.GetInstance().GetPlayers(dataCache: dataCache);
                var player = allPlayers.FirstOrDefault(t => t.ID == playerID);
                var teams = TeamsService.GetInstance().GetTeams(playerID: playerID, dataCache: dataCache);
                var games = GamesService.GetInstance().GetGames(playerID: playerID, dataCache: dataCache);

                var selectedPlayer = new Player()
                {
                    ID = player.ID,
                    Name = player.Name,
                    Number = player.Number,
                    Teams = teams,
                    Games = games
                };

                return selectedPlayer;
            }
            else
            {
                throw new UnauthorizedAccessException("Nope");
            }
        }

        public int AddPlayer(Player player, int? teamID = null, int? leagueID = null)
        {
            if (UserHelper.HasUpdatePermissions(leagueID: leagueID))
            {
                return PlayerDataProvider.GetInstance().AddPlayer(ConvertObjects.ConvertType(player), teamID, leagueID);
            }
            else
            {
                throw (new UnauthorizedAccessException("nope"));
            }
        }

        public void SavePlayer(Player player)
        {
            if (UserHelper.HasUpdatePermissions())
            {
                var dtoPlayer = ConvertObjects.ConvertType(player);
                PlayerDataProvider.GetInstance().SavePlayer(dtoPlayer);
                foreach(Team t in player.Teams)
                {
                    dtoPlayer.Number = t.PlayerNumber;
                    dtoPlayer.TeamID = t.ID;
                    PlayerDataProvider.GetInstance().SaveTeamPlayer(dtoPlayer);
                }
            }
        }
    }
}