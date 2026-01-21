GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddLeague]
	@SportID int,
	@Name varchar(100),
	@StartDate datetime,
	@EndDate datetime,
	@SeasonID int
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Leagues (SportID, Name, StartDate, EndDate, SeasonID)
    VALUES (@SportID, @Name, @StartDate, @EndDate, @SeasonID)

    SELECT SCOPE_IDENTITY() AS NewID
END
GO