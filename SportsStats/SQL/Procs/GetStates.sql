GO
/****** Object:  StoredProcedure [dbo].[AddGame]    Script Date: 3/17/2017 2:15:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStates]
	@SportID int
AS
BEGIN
	SELECT	Name,
			ID,
			DisplayOrder
	FROM	States
	WHERE	SportID = @SportID
END
