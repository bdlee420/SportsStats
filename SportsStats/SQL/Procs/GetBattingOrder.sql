GO
/****** Object:  StoredProcedure [dbo].[GetBattingOrder]    Script Date: 3/21/2017 1:04:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetBattingOrder]	
	@TeamID int = null,
	@GameID int = null
AS
BEGIN
	SELECT		TP.PlayerID, 
				SUM(S.Value) as BattingOrder
	FROM		TeamPlayers TP
	JOIN		Stats S
	ON			S.PlayerID = TP.PlayerID	
	AND			S.GameID = @GameID 
	AND			S.StatTypeID = 72
	AND			S.Value <> 0
	WHERE		TP.TeamID = @TeamID
	GROUP BY	TP.PlayerID
	ORDER BY	BattingOrder
END
