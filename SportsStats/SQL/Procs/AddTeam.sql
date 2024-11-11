GO
/****** Object:  StoredProcedure [dbo].[AddTeam]    Script Date: 8/20/2017 10:34:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddTeam]
	@Name varchar(500),
    @LeagueID int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ExistingTeamID int = null
	SELECT @ExistingTeamID = ID FROM Teams WHERE Name = @Name

	IF @ExistingTeamID IS NULL
		BEGIN
		   INSERT INTO dbo.Teams (name)
		   SELECT @Name

		   INSERT INTO dbo.LeagueTeams( leagueID, teamID )
		   SELECT  @LeagueID, @@identity
		END
	ELSE
		INSERT INTO dbo.LeagueTeams( leagueID, teamID )
		SELECT @LeagueID, @ExistingTeamID
END