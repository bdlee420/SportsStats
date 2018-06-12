GO
/****** Object:  StoredProcedure [dbo].[GetTeams]    Script Date: 3/19/2017 9:16:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetTeams]	
    @LeagueID   int = NULL,
	@PlayerID	int	= NULL,
    @TeamID	    int	= NULL,
	@SportID    int	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @PlayerID is not null
		BEGIN
			SELECT		T.ID,
						TP.Number, 
						T.Name, 
						L.Name, 
						L.SportID,
						L.ID as LeagueID
			FROM		TeamPlayers TP
			JOIN		Teams T
			ON			TP.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = TP.LeagueID
			WHERE		TP.PlayerID = @PlayerID
			AND         (L.ID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name			
		END
    ELSE IF @TeamID is not null
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T		
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
			WHERE		T.ID = @TeamID
			AND         (LT.LeagueID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name
		END
	ELSE IF @SportID is not null
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T		
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
			WHERE		L.SportID = @SportID
			ORDER BY	T.Name
		END
	ELSE
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
            WHERE       (LT.LeagueID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name
		END

END
