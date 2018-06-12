GO
/****** Object:  StoredProcedure [dbo].[GetPlayers]    Script Date: 2/7/2017 9:45:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetPlayers]	
	@TeamID	int		= NULL,
	@GameID int		= NULL,
	@PlayerID int	= NULL,
	@LeagueID int	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @TeamID is not null
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						TP.Number,
						TP.TeamID
			FROM		Players P
			JOIN		TeamPlayers TP
			ON			P.ID = TP.PlayerID
			JOIN		LeagueTeams LT
			ON			LT.TeamID = TP.TeamID
			WHERE		TP.TeamID = @TeamID
			AND			TP.leagueID = @LeagueID
			AND			(@LeagueID is null OR LT.LeagueID = @LeagueID)
			AND			P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END
	ELSE IF @GameID is not null
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						TP.Number,
						TP.TeamID
			FROM		Players P
			JOIN		TeamPlayers TP
			ON			P.ID = TP.PlayerID
			JOIN		Games G
			ON			G.Team1ID = TP.TeamID 
			OR			G.Team2ID = TP.TeamID
			WHERE		G.ID = @GameID
			AND			TP.leagueID = @LeagueID
			AND			P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END
	ELSE
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						0 as Number,
						0 as TeamID
			FROM		Players P
			WHERE		P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END

END
