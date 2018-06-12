/****** Object:  StoredProcedure [dbo].[AddPlayer]    Script Date: 2/7/2017 8:34:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddPlayer]
	@PlayerID int = NULL OUTPUT,
	@Name varchar(500) = NULL,
	@Number	int,	
	@TeamID	int = NULL,
	@LeagueID int = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name is not null AND @PlayerID is null
		BEGIN
		   INSERT INTO dbo.Players (name)
		   SELECT @Name
		   SET @PlayerID = @@IDENTITY
		END	

   IF @TeamID is not null AND @LeagueID is not null
	BEGIN
		INSERT INTO dbo.TeamPlayers (TeamID, PlayerID, LeagueID, Number)
		SELECT @TeamID, @PlayerID, @LeagueID, @Number
	END
END

GO