GO
/****** Object:  StoredProcedure [dbo].[GetUser]    Script Date: 3/24/2017 9:14:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].GetLeagueID	
    @GameID   int
AS
BEGIN
	SET NOCOUNT ON;
	SELECT LeagueID as ID FROM Games WHERE ID = @GameID
END
