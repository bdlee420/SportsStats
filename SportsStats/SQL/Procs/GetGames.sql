GO
/****** Object:  StoredProcedure [dbo].[GetGames]    Script Date: 2/9/2017 1:07:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetGames]
    @LeagueID   int = NULL,	
	@TeamID		int = NULL,
	@PlayerID	int = NULL,
    @GameID	    int = NULL
AS
BEGIN
	SET NOCOUNT ON;  

    IF @GameID is not null
		BEGIN	
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G	
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.ID = @GameID     
			AND			(@LeagueID is null OR L.ID = @LeagueID)  
			ORDER BY	G.GameDate
		END
	ELSE IF @PlayerID is not null
		BEGIN	
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G
			JOIN		TeamPlayers TP1
			ON			G.Team1ID = TP1.TeamID
			AND			TP1.PlayerID = @PlayerID
			JOIN		TeamPlayers TP2
			ON			G.Team2ID = TP2.TeamID
			AND			TP2.PlayerID = @PlayerID
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.Deleted = 0      
			AND			(@LeagueID is null OR L.ID = @LeagueID)        
			ORDER BY	G.GameDate
		END
	ELSE IF @TeamID is not null
		BEGIN
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G		
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		(G.Team1ID = @TeamID OR G.Team2ID = @TeamID)
			AND			(@LeagueID is null OR L.ID = @LeagueID)   
			AND			G.Deleted = 0
			ORDER BY	G.GameDate
		END
	ELSE
		BEGIN
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.Deleted = 0
			AND			(@LeagueID is null OR L.ID = @LeagueID)    
			ORDER BY	G.GameDate
		END
END
