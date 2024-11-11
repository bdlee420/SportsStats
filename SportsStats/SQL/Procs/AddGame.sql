/****** Object:  StoredProcedure [dbo].[AddGame]    Script Date: 2/7/2017 8:34:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddGame]
	@GameDate datetime,
	@Team1ID int,
	@Team2ID int,
	@LeagueID int
AS
BEGIN
	SET NOCOUNT ON;

	if @Team1ID = 0 OR @Team2ID = 0
		BEGIN
			return
		END

   INSERT INTO dbo.Games (GameDate, Team1ID, Team2ID, LeagueID)
   SELECT @GameDate, @Team1ID, @Team2ID, @LeagueID
END