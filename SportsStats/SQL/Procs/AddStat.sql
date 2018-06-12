GO
/****** Object:  StoredProcedure [dbo].[AddStat]    Script Date: 3/30/2017 8:24:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[AddStat]
	@TeamID		int,
	@PlayerID	int = null,
	@GameID		int,
	@Value		int = 0,
	@StatTypeID int,
	@GroupID	int = null,
	@States		varchar(500) = null,
	@Override	bit = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @ID int = null

	SELECT	@ID = ID 
	FROM	[Stats] 
	WHERE	GameID		= @GameID
	AND		PlayerID	= @PlayerID
	AND		TeamID		= @TeamID
	AND		StatTypeID  = @StatTypeID

	--Each event could have its own state. Only increment existing if there are no states attached.
	IF @ID is null OR (@Override = 0 AND (@States is not null OR @GroupID is not null))
		BEGIN
		   INSERT INTO dbo.[Stats] (StatTypeID, Value, GameID, PlayerID, TeamID, StatGroup, Deleted)
		   SELECT	@StatTypeID,
					@Value,
					@GameID,
					@PlayerID,
					@TeamID,
					@GroupID,
					0
			SET @ID = @@IDENTITY
		END	
   ELSE
	BEGIN
		if(@Override = 1)
			BEGIN
				UPDATE	Stats
				SET		Value	= @Value
				WHERE	ID		= @ID
			END
		ELSE
			BEGIN
				UPDATE	Stats
				SET		Value	= Value + @Value
				WHERE	ID		= @ID
			END
	END

	IF @States is not null AND @States <> ''
		BEGIN
			INSERT INTO StatStates 
			(
					StatID, 
					StateID,
					GameID
			) 
			SELECT	@ID, 
					Item,
					@GameID
			FROM	dbo.SplitString(@States, ',')
		END
			
END
