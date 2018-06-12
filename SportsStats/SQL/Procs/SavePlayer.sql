GO
/****** Object:  StoredProcedure [dbo].[AddPlayer]    Script Date: 3/17/2017 3:12:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SavePlayer]
	@PlayerID int,
	@Name varchar(500)
AS
BEGIN
	UPDATE Players
	SET	Name = @Name
	WHERE ID = @PlayerID
END
