GO

/****** Object:  Table [dbo].[BaseballGameState]    Script Date: 3/21/2017 10:44:46 PM ******/
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


