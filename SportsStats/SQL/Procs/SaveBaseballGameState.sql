GO
/****** Object:  StoredProcedure [dbo].[GetBaseballGameState]    Script Date: 3/19/2017 8:31:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SaveBaseballGameState]
	@GameID				int,
	@Inning				int,
	@TopOfInning		int,
	@FirstPlayerID		int = NULL,
	@SecondPlayerID		int = NULL,
	@ThirdPlayerID		int = NULL,
	@Outs				int,
	@Team1PlayerID		int = NULL,
	@Team2PlayerID		int = NULL
AS
BEGIN
	IF EXISTS (SELECT* FROM	BaseballGameState WHERE	GameID = @GameID)
		BEGIN	
			UPDATE	BaseballGameState
			SET		Inning = @Inning,
					TopOfInning = @TopOfInning,
					FirstPlayerID = @FirstPlayerID,
					SecondPlayerID = @SecondPlayerID,
					ThirdPlayerID = @ThirdPlayerID,					
					Outs = @Outs,
					Team1PlayerID = @Team1PlayerID,
					Team2PlayerID = @Team2PlayerID
			WHERE   GameID = @GameID
		END
	ELSE
		BEGIN
			INSERT INTO BaseballGameState
			(
					GameID,
					Inning,
					TopOfInning,
					FirstPlayerID,
					SecondPlayerID,
					ThirdPlayerID,
					Outs,
					Team1PlayerID,
					Team2PlayerID
			)
			SELECT	@GameID,
					@Inning,
					@TopOfInning,
					@FirstPlayerID,
					@SecondPlayerID,
					@ThirdPlayerID,
					@Outs,
					@Team1PlayerID,
					@Team2PlayerID
		END
	
END
