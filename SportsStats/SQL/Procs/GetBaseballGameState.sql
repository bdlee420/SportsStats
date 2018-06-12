GO
/****** Object:  StoredProcedure [dbo].[GetBaseballGameState]    Script Date: 3/22/2017 4:09:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetBaseballGameState]
	@GameID int
AS
BEGIN
	SELECT	GameID,
			Inning,
			TopOfInning,
			FirstPlayerID,
			SecondPlayerID,
			ThirdPlayerID,
			Outs,
            Team1PlayerID,
            Team2PlayerID
	FROM	BaseballGameState
	WHERE	GameID = @GameID
END
