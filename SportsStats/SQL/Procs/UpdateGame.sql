/****** Object:  StoredProcedure [dbo].[UpdateGame]    Script Date: 2/7/2017 8:34:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateGame]
	@GameID		int,
	@GameDate	datetime,
	@Team1ID	int,
	@Team2ID	int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE	Games
	SET		Team1ID		= @Team1ID,
			Team2ID		= @Team2ID,
			GameDate	= @GameDate
	WHERE	ID = @GameID
END

GO
