GO
/****** Object:  StoredProcedure [dbo].[GetTeams]    Script Date: 2/7/2017 11:25:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetLeagues]	
	@SportID int = null
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT		L.ID,
				L.Name,
				L.StartDate,
				L.EndDate,
				L.SeasonID,
				L.SportID
	FROM		Leagues L
	WHERE		@SportID is null OR L.SportID = @SportID
	ORDER BY	L.StartDate desc

END
