GO
/****** Object:  StoredProcedure [dbo].[GetTeams]    Script Date: 2/7/2017 11:25:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSports]	
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT		S.ID,
				S.Name
	FROM		Sports S
	ORDER BY	S.Name

END
