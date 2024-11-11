GO
/****** Object:  StoredProcedure [dbo].[GetUser]    Script Date: 3/24/2017 9:14:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetUser]	
    @UserName   varchar(50),
	@Password	varchar(50) = null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @UserID int = NULL
	DECLARE @RoleID int = NULL

	SELECT  @UserID = UserID, 
			@RoleID = RoleID
	FROM	Users
	WHERE	UserName = @UserName
	AND		(@Password is null OR Password = @Password)

	IF @UserID is not null
		BEGIN
			UPDATE Users SET LastLogin = GetDate()
		END

	SELECT  @RoleID as RoleID

	SELECT	LeagueID,
			RoleID
	FROM	UserLeagueAccess
	WHERE	UserID = @UserID

	SELECT  TeamID, 
			LeagueID,
			SportID
	FROM	UserTeamAccess UTA
	JOIN	Leagues L
	ON		L.ID = UTA.LeagueID
	WHERE	UserID = @UserID
	UNION
	SELECT	LT.TeamID, 
			LT.LeagueID,
			L.SportID
	FROM	LeagueTeams LT
	JOIN	UserLeagueAccess ULA
	ON		ULA.LeagueID = LT.LeagueID
	JOIN	Leagues L
	ON		L.ID = LT.LeagueID
	WHERE	ULA.UserID = @UserID

	SELECT	ID as PlayerID
	FROM	Players P
	JOIN	TeamPlayers TP
	ON		P.ID = TP.PlayerID
	JOIN	UserTeamAccess UTA
	ON		UTA.TeamID = TP.TeamID
	WHERE	UTA.UserID = @UserID
	UNION
	SELECT	ID as PlayerID
	FROM	Players P
	JOIN	TeamPlayers TP
	ON		P.ID = TP.PlayerID
	JOIN	LeagueTeams LT
	ON		LT.TeamID = TP.TeamID
	JOIN	UserLeagueAccess ULA
	ON		ULA.LeagueID = LT.LeagueID
	WHERE	ULA.UserID = @UserID

END
