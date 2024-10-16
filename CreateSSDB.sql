USE [master]
GO
/****** Object:  Database [SportsStats]    Script Date: 10/15/2024 10:48:51 AM ******/
CREATE DATABASE [SportsStats]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SportsStats', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\SportsStats.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SportsStats_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\SportsStats_log.ldf' , SIZE = 2304KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SportsStats] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SportsStats].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SportsStats] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SportsStats] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SportsStats] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SportsStats] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SportsStats] SET ARITHABORT OFF 
GO
ALTER DATABASE [SportsStats] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SportsStats] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SportsStats] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SportsStats] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SportsStats] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SportsStats] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SportsStats] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SportsStats] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SportsStats] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SportsStats] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SportsStats] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SportsStats] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SportsStats] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SportsStats] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SportsStats] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SportsStats] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SportsStats] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SportsStats] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SportsStats] SET  MULTI_USER 
GO
ALTER DATABASE [SportsStats] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SportsStats] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SportsStats] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SportsStats] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [SportsStats] SET DELAYED_DURABILITY = DISABLED 
GO
USE [SportsStats]
GO
/****** Object:  User [website]    Script Date: 10/15/2024 10:48:51 AM ******/
CREATE USER [website] FOR LOGIN [website] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [website]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [website]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [website]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [website]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [website]
GO
ALTER ROLE [db_datareader] ADD MEMBER [website]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [website]
GO
ALTER ROLE [db_denydatareader] ADD MEMBER [website]
GO
ALTER ROLE [db_denydatawriter] ADD MEMBER [website]
GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
      Item NVARCHAR(1000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END
 
      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)
           
            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
           
            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END
 
      RETURN
END

GO
/****** Object:  Table [dbo].[BaseballGameState]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BaseballGameState](
	[GameID] [int] NOT NULL,
	[Inning] [int] NOT NULL,
	[TopOfInning] [bit] NOT NULL,
	[FirstPlayerID] [int] NULL,
	[SecondPlayerID] [int] NULL,
	[ThirdPlayerID] [int] NULL,
	[Outs] [int] NOT NULL,
	[Team1PlayerID] [int] NULL,
	[Team2PlayerID] [int] NULL,
 CONSTRAINT [PK_BaseballGameState] PRIMARY KEY CLUSTERED 
(
	[GameID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Games]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Games](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GameDate] [datetime] NOT NULL,
	[Team1ID] [int] NOT NULL,
	[Team2ID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Games_Deleted]  DEFAULT ((0)),
	[LeagueID] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Leagues]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Leagues](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SportID] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[SeasonID] [int] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LeagueTeams]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeagueTeams](
	[LeagueID] [int] NOT NULL,
	[TeamID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Players]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Players](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Players_Deleted]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Sports]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sports](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[States]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[States](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SportID] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StatGroups]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatGroups](
	[GroupID] [int] IDENTITY(1,1) NOT NULL,
	[GameID] [int] NOT NULL,
	[PlayerID] [int] NULL,
	[TeamID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Stats]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stats](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StatTypeID] [int] NOT NULL,
	[Value] [int] NOT NULL,
	[GameID] [int] NOT NULL,
	[PlayerID] [int] NULL,
	[TeamID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Stats_Deleted]  DEFAULT ((0)),
	[StatGroup] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StatStates]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatStates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StateID] [int] NOT NULL,
	[StatID] [int] NOT NULL,
	[GameID] [int] NULL,
 CONSTRAINT [PK_StatStates_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StatTypes]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StatTypes](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[DisplayName] [varchar](50) NULL,
	[DefaultShow] [bit] NOT NULL CONSTRAINT [DF_StatTypes_DefaultShow]  DEFAULT ((0)),
	[IsCalculated] [bit] NOT NULL CONSTRAINT [DF_StatTypes_IsCalculated]  DEFAULT ((0)),
	[SelectionDisplayOrder] [int] NOT NULL CONSTRAINT [DF_StatTypes_DisplayOrder]  DEFAULT ((0)),
	[GridDisplayOrder] [int] NOT NULL CONSTRAINT [DF_StatTypes_GridDisplayOrder]  DEFAULT ((0)),
	[ValueType] [int] NOT NULL CONSTRAINT [DF_StatTypes_ValueType]  DEFAULT ((1)),
	[SportID] [int] NULL,
	[QuickButtonOrder] [int] NULL,
	[IsPositive] [bit] NULL,
	[QuickButtonText] [varchar](10) NULL,
	[AutoGenerated] [bit] NOT NULL CONSTRAINT [DF_StatTypes_AutoGenerated]  DEFAULT ((0)),
	[ShowGame] [bit] NOT NULL CONSTRAINT [DF_StatTypes_ShowGame]  DEFAULT ((0)),
	[ShowTeam] [bit] NOT NULL CONSTRAINT [DF_StatTypes_ShowTeam]  DEFAULT ((0)),
	[ShowPlayer] [bit] NOT NULL CONSTRAINT [DF_StatTypes_ShowPlayer]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TeamPlayers]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeamPlayers](
	[TeamID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[Deleted] [bit] NULL,
	[Number] [int] NOT NULL CONSTRAINT [DF_TeamPlayers_Number]  DEFAULT ((0)),
	[LeagueID] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Teams]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Teams](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Teams_Deleted]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[LastLogin] [datetime] NULL,
	[RoleID] [int] NOT NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserTeamAccess]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTeamAccess](
	[UserID] [int] NOT NULL,
	[TeamID] [int] NOT NULL,
	[LeagueID] [int] NULL
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[AddGame]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddGame]
	@GameDate datetime,
	@Team1ID int,
	@Team2ID int,
	@LeagueID int
AS
BEGIN
	SET NOCOUNT ON;

   INSERT INTO dbo.Games (GameDate, Team1ID, Team2ID, LeagueID)
   SELECT @GameDate, @Team1ID, @Team2ID, @LeagueID
END


GO
/****** Object:  StoredProcedure [dbo].[AddLeagueTeam]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddLeagueTeam]
	@TeamID int,
    @LeagueID int
AS
BEGIN
	SET NOCOUNT ON;


   INSERT INTO dbo.LeagueTeams( leagueID, teamID )
   SELECT  @LeagueID, @TeamID
END
GO
/****** Object:  StoredProcedure [dbo].[AddPlayer]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddPlayer]
	@PlayerID int = NULL OUTPUT,
	@Name varchar(500) = NULL,
	@Number	int,	
	@TeamID	int = NULL,
	@LeagueID int = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name is not null AND @PlayerID is null
		BEGIN
		   INSERT INTO dbo.Players (name)
		   SELECT @Name
		   SET @PlayerID = @@IDENTITY
		END	

   IF @TeamID is not null AND @LeagueID is not null
	BEGIN
		INSERT INTO dbo.TeamPlayers (TeamID, PlayerID, LeagueID, Number)
		SELECT @TeamID, @PlayerID, @LeagueID, @Number
	END
END


GO
/****** Object:  StoredProcedure [dbo].[AddStat]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddStat]
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

GO
/****** Object:  StoredProcedure [dbo].[AddTeam]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[AddTeam]
	@Name varchar(500),
    @LeagueID int
AS
BEGIN
	SET NOCOUNT ON;

   INSERT INTO dbo.Teams (name)
   SELECT @Name

   INSERT INTO dbo.LeagueTeams( leagueID, teamID )
   SELECT  @LeagueID, @@identity
END
GO
/****** Object:  StoredProcedure [dbo].[GetBaseballGameState]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetBaseballGameState]
	@GameID int
AS
BEGIN
	SELECT	GameID,
			Inning,
			TopOfInning,
			FirstPlayerID,
			SecondPlayerID,
			ThirdPlayerID,
			Outs,
            Team1PlayerID,
            Team2PlayerID
	FROM	BaseballGameState
	WHERE	GameID = @GameID
END

GO
/****** Object:  StoredProcedure [dbo].[GetBattingOrder]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetBattingOrder]	
	@TeamID int = null,
	@GameID int = null
AS
BEGIN
	SELECT		TP.PlayerID, 
				SUM(S.Value) as BattingOrder
	FROM		TeamPlayers TP
	JOIN		Stats S
	ON			S.PlayerID = TP.PlayerID	
	AND			S.GameID = @GameID 
	AND			S.StatTypeID = 72
	AND			S.Value <> 0
	WHERE		TP.TeamID = @TeamID
	GROUP BY	TP.PlayerID
	ORDER BY	BattingOrder
END

GO
/****** Object:  StoredProcedure [dbo].[GetGameLog]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetGameLog]	
	@GameID	    int
AS
BEGIN
	SET NOCOUNT ON;

    SELECT      T.name as TeamName, 
                P.name as PlayerName, 
                ST.DisplayName, 
                S.Value
    FROM        Stats s 
    JOIN        Players P
    ON          S.PlayerID = P.ID
    JOIN        StatTypes ST
    ON          S.StatTypeID = ST.ID
    JOIN        Teams T
    ON          T.ID = s.TeamID
    WHERE       s.gameID = @GameID 
    AND         s.StatTypeID <> 72
    ORDER BY    s.ID
END


GO
/****** Object:  StoredProcedure [dbo].[GetGames]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetGames]
    @LeagueID   int = NULL,	
	@TeamID		int = NULL,
	@PlayerID	int = NULL,
    @GameID	    int = NULL
AS
BEGIN
	SET NOCOUNT ON;  

    IF @GameID is not null
		BEGIN	
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G	
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.ID = @GameID     
			AND			(@LeagueID is null OR L.ID = @LeagueID)  
			ORDER BY	G.GameDate
		END
	ELSE IF @PlayerID is not null
		BEGIN	
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G
			JOIN		TeamPlayers TP1
			ON			G.Team1ID = TP1.TeamID
			AND			TP1.PlayerID = @PlayerID
			JOIN		TeamPlayers TP2
			ON			G.Team2ID = TP2.TeamID
			AND			TP2.PlayerID = @PlayerID
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.Deleted = 0      
			AND			(@LeagueID is null OR L.ID = @LeagueID)        
			ORDER BY	G.GameDate
		END
	ELSE IF @TeamID is not null
		BEGIN
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G		
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		(G.Team1ID = @TeamID OR G.Team2ID = @TeamID)
			AND			(@LeagueID is null OR L.ID = @LeagueID)   
			AND			G.Deleted = 0
			ORDER BY	G.GameDate
		END
	ELSE
		BEGIN
			SELECT		G.ID, 
						G.GameDate,
						G.Team1ID,
						G.Team2ID,
                        L.SportID,
						L.ID as LeagueID
			FROM		Games G
            JOIN        Leagues L
            ON          L.ID = G.LeagueID
			WHERE		G.Deleted = 0
			AND			(@LeagueID is null OR L.ID = @LeagueID)    
			ORDER BY	G.GameDate
		END
END

GO
/****** Object:  StoredProcedure [dbo].[GetLeagues]    Script Date: 10/15/2024 10:48:51 AM ******/
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

GO
/****** Object:  StoredProcedure [dbo].[GetPlayers]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPlayers]	
	@TeamID	int		= NULL,
	@GameID int		= NULL,
	@PlayerID int	= NULL,
	@LeagueID int	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @TeamID is not null
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						TP.Number,
						TP.TeamID
			FROM		Players P
			JOIN		TeamPlayers TP
			ON			P.ID = TP.PlayerID
			JOIN		LeagueTeams LT
			ON			LT.TeamID = TP.TeamID
			WHERE		TP.TeamID = @TeamID
			AND			TP.leagueID = @LeagueID
			AND			(@LeagueID is null OR LT.LeagueID = @LeagueID)
			AND			P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END
	ELSE IF @GameID is not null
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						TP.Number,
						TP.TeamID
			FROM		Players P
			JOIN		TeamPlayers TP
			ON			P.ID = TP.PlayerID
			JOIN		Games G
			ON			G.Team1ID = TP.TeamID 
			OR			G.Team2ID = TP.TeamID
			WHERE		G.ID = @GameID
			AND			TP.leagueID = @LeagueID
			AND			P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END
	ELSE
		BEGIN
			SELECT		P.ID, 
						P.Name, 
						0 as Number,
						0 as TeamID
			FROM		Players P
			WHERE		P.Deleted = 0
			AND			(@PlayerID is null OR P.ID = @PlayerID)
			ORDER BY	P.Name
		END

END

GO
/****** Object:  StoredProcedure [dbo].[GetSports]    Script Date: 10/15/2024 10:48:51 AM ******/
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

GO
/****** Object:  StoredProcedure [dbo].[GetStates]    Script Date: 10/15/2024 10:48:51 AM ******/
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

GO
/****** Object:  StoredProcedure [dbo].[GetStatGroup]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStatGroup]	
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


GO
/****** Object:  StoredProcedure [dbo].[GetStatGroups]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStatGroups]	
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

GO
/****** Object:  StoredProcedure [dbo].[GetStats]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStats]	
	@GameID int = null,
	@PlayerID int = null,
	@TeamID int = null,
	@LeagueID int = null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	S.ID, 
			S.StatTypeID,
			S.Value,
			S.GameID,
			S.PlayerID,
			S.TeamID,
			G.LeagueID,
			L.SportID,
			ST.DefaultShow,
			ST.DisplayName,
			ST.SelectionDisplayOrder,
            ST.GridDisplayOrder,
			ST.AutoGenerated,
			ST.ValueType,
			ST.IsCalculated,
			ST.Name,
			ST.ID as StatTypeID,
			S.StatGroup as GroupID
	FROM	dbo.[Stats] S
	JOIN	dbo.[StatTypes] ST
	ON		S.StatTypeID = ST.ID
	JOIN	Games G
	ON		G.ID = S.GameID
	JOIN	Leagues L 
	ON		L.ID = G.LeagueID
	WHERE	S.Deleted = 0
	AND		(@GameID is null OR S.GameID = @GameID)
	AND		(@PlayerID is null OR S.PlayerID = @PlayerID)
	AND		(@TeamID is null OR S.TeamID = @TeamID)
	AND		(@LeagueID is null OR G.LeagueID = @LeagueID)
    
    SELECT  SS.StatID,
            SS.GameID,
            S.PlayerID,
            SS.StateID,
			G.LeagueID
    FROM    StatStates SS
    JOIN    Stats S
    ON      SS.StatID = S.ID
	JOIN	Games G
	ON		G.ID = S.GameID
    WHERE   S.Deleted = 0
	AND		(@GameID is null OR S.GameID = @GameID)
	AND		(@PlayerID is null OR S.PlayerID = @PlayerID)
	AND		(@TeamID is null OR S.TeamID = @TeamID)
	AND		(@LeagueID is null OR G.LeagueID = @LeagueID)

END


GO
/****** Object:  StoredProcedure [dbo].[GetStatTypes]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetStatTypes]	
	@SportID	int = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		ID, 
				Name,
				DisplayName,
				DefaultShow,
				IsCalculated,
                SelectionDisplayOrder,
                GridDisplayOrder,
                ValueType,
				QuickButtonOrder,
				QuickButtonText,
				IsPositive,
				AutoGenerated,
				SportID,
				ShowGame,
				ShowTeam,
				ShowPlayer
	FROM		StatTypes
	WHERE		SportID = @SportID OR @SportID is null
END


GO
/****** Object:  StoredProcedure [dbo].[GetTeams]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetTeams]	
    @LeagueID   int = NULL,
	@PlayerID	int	= NULL,
    @TeamID	    int	= NULL,
	@SportID    int	= NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF @PlayerID is not null
		BEGIN
			SELECT		T.ID,
						TP.Number, 
						T.Name, 
						L.Name, 
						L.SportID,
						L.ID as LeagueID
			FROM		TeamPlayers TP
			JOIN		Teams T
			ON			TP.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = TP.LeagueID
			WHERE		TP.PlayerID = @PlayerID
			AND         (L.ID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name			
		END
    ELSE IF @TeamID is not null
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T		
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
			WHERE		T.ID = @TeamID
			AND         (LT.LeagueID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name
		END
	ELSE IF @SportID is not null
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T		
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
			WHERE		L.SportID = @SportID
			ORDER BY	T.Name
		END
	ELSE
		BEGIN
			SELECT		T.ID, 
						T.Name,
                        LT.LeagueID,
						L.SportID,
						0 as Number
			FROM		Teams T
			JOIN		LeagueTeams LT
			ON			LT.TeamID = T.ID
			JOIN		Leagues L
			ON			L.ID = LT.LeagueID
            WHERE       (LT.LeagueID = @LeagueID OR @LeagueID is null)
			ORDER BY	T.Name
		END

END

GO
/****** Object:  StoredProcedure [dbo].[GetUser]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUser]	
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
			UPDATE Users SET LastLogin = GetDate() WHERE UserID = @UserID
		END

	SELECT  @RoleID as RoleID

	SELECT  TeamID, 
			LeagueID,
			SportID
	FROM	UserTeamAccess UTA
	JOIN	Leagues L
	ON		L.ID = UTA.LeagueID
	WHERE	UserID = @UserID

	SELECT	ID as PlayerID
	FROM	Players P
	JOIN	TeamPlayers TP
	ON		P.ID = TP.PlayerID
	JOIN	UserTeamAccess UTA
	ON		UTA.TeamID = TP.TeamID
	WHERE	UTA.UserID = @UserID

END

GO
/****** Object:  StoredProcedure [dbo].[SaveBaseballGameState]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SaveBaseballGameState]
	@GameID				int,
	@Inning				int,
	@TopOfInning		int,
	@FirstPlayerID		int = NULL,
	@SecondPlayerID		int = NULL,
	@ThirdPlayerID		int = NULL,
	@Outs				int,
	@Team1PlayerID		int = NULL,
	@Team2PlayerID		int = NULL
AS
BEGIN
	IF EXISTS (SELECT* FROM	BaseballGameState WHERE	GameID = @GameID)
		BEGIN	
			UPDATE	BaseballGameState
			SET		Inning = @Inning,
					TopOfInning = @TopOfInning,
					FirstPlayerID = @FirstPlayerID,
					SecondPlayerID = @SecondPlayerID,
					ThirdPlayerID = @ThirdPlayerID,					
					Outs = @Outs,
					Team1PlayerID = @Team1PlayerID,
					Team2PlayerID = @Team2PlayerID
			WHERE   GameID = @GameID
		END
	ELSE
		BEGIN
			INSERT INTO BaseballGameState
			(
					GameID,
					Inning,
					TopOfInning,
					FirstPlayerID,
					SecondPlayerID,
					ThirdPlayerID,
					Outs,
					Team1PlayerID,
					Team2PlayerID
			)
			SELECT	@GameID,
					@Inning,
					@TopOfInning,
					@FirstPlayerID,
					@SecondPlayerID,
					@ThirdPlayerID,
					@Outs,
					@Team1PlayerID,
					@Team2PlayerID
		END
	
END

GO
/****** Object:  StoredProcedure [dbo].[SavePlayer]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SavePlayer]
	@PlayerID int,
	@Name varchar(500)
AS
BEGIN
	UPDATE Players
	SET	Name = @Name
	WHERE ID = @PlayerID
END

GO
/****** Object:  StoredProcedure [dbo].[SaveTeamPlayer]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SaveTeamPlayer]
	@TeamID int,
	@PlayerID int,
	@PlayerNumber int
AS
BEGIN
	UPDATE	TeamPlayers
	SET		Number = @PlayerNumber
	WHERE	TeamID = @TeamID
	AND		PlayerID = @PlayerID
END

GO
/****** Object:  StoredProcedure [dbo].[UpdateGame]    Script Date: 10/15/2024 10:48:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateGame]
	@GameID		int,
	@GameDate	datetime,
	@Team1ID	int,
	@Team2ID	int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE	Games
	SET		Team1ID		= @Team1ID,
			Team2ID		= @Team2ID,
			GameDate	= @GameDate
	WHERE	ID = @GameID
END

GO
USE [master]
GO
ALTER DATABASE [SportsStats] SET  READ_WRITE 
GO
