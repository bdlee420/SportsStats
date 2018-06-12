GO
/****** Object:  StoredProcedure [dbo].[GetStatGroup]    Script Date: 4/13/2017 12:57:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetStatGroup]	
	@GameID	    int,
    @PlayerID	int = null,
    @TeamID	    int,
    @New        bit
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @GroupID int = null

    IF @New = 0
        BEGIN
	        SELECT		Distinct TOP 1 @GroupID = GroupID
	        FROM		StatGroups
	        WHERE		GameID      = @GameID
            AND         PlayerID    = @PlayerID
            AND         TeamID      = @TeamID
            ORDER BY    GroupID DESC
        END

    IF @GroupID is null OR @New = 1
        BEGIN

            SELECT		Distinct TOP 1 @GroupID = GroupID
	        FROM		StatGroups SG
	        WHERE		GameID      = @GameID
            AND         PlayerID    = @PlayerID
            AND         TeamID      = @TeamID
            AND         NOT EXISTS (SELECT * FROM Stats S WHERE S.StatGroup = SG.GroupID)

            IF @GroupID is null
                BEGIN
                    INSERT INTO StatGroups
                    (
                        GameID,
                        PlayerID,
                        TeamID
                    )
                    SELECT 
                        @GameID,
                        @PlayerID,
                        @TeamID

                    SET @GroupID = @@IDENTITY
                END
        END

	SELECT @GroupID as GroupID
END

