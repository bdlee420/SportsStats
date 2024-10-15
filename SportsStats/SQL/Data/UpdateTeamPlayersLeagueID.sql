--10/4/2017
UPDATE TP SET TP.LeagueID = (select LeagueID from leagueTeams where teamID = TP.teamID) FROM TeamPlayers TP