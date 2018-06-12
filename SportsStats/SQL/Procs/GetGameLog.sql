GO
/****** Object:  StoredProcedure [dbo].[GetGameLog]    Script Date: 4/13/2017 4:16:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetGameLog]	
	@GameID	    int
AS
BEGIN
	SET NOCOUNT ON;

    SELECT      T.name as TeamName, 
                P.name as PlayerName, 
                ST.DisplayName, 
                S.Value
    FROM        Stats s 
    JOIN        Players P
    ON          S.PlayerID = P.ID
    JOIN        StatTypes ST
    ON          S.StatTypeID = ST.ID
    JOIN        Teams T
    ON          T.ID = s.TeamID
    WHERE       s.gameID = @GameID 
    AND         s.StatTypeID <> 72
    ORDER BY    s.ID
END

