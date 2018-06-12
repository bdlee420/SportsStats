GO
/****** Object:  StoredProcedure [dbo].[AddTeam]    Script Date: 8/20/2017 10:34:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddLeagueTeam]
	@TeamID int,
    @LeagueID int
AS
BEGIN
	SET NOCOUNT ON;


   INSERT INTO dbo.LeagueTeams( leagueID, teamID )
   SELECT  @LeagueID, @TeamID
END