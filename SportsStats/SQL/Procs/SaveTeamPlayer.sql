GO
/****** Object:  StoredProcedure [dbo].[SavePlayer]    Script Date: 3/19/2017 12:25:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SaveTeamPlayer]
	@TeamID int,
	@PlayerID int,
	@PlayerNumber int
AS
BEGIN
	UPDATE	TeamPlayers
	SET		Number = @PlayerNumber
	WHERE	TeamID = @TeamID
	AND		PlayerID = @PlayerID
END
