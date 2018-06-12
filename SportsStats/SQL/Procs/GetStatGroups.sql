GO
/****** Object:  StoredProcedure [dbo].[GetStatGroups]    Script Date: 3/21/2017 10:55:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetStatGroups]	
	@GameID     int,
	@PlayerID   int = null
AS
BEGIN
	SET NOCOUNT ON;

	select  distinct 
            groupID 
    FROM    statgroups 
    WHERE   playerID = @PlayerID 
	AND		gameID = @GameID	

END
