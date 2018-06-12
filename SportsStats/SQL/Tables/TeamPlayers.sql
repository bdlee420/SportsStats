GO

/****** Object:  Table [dbo].[TeamPlayers]    Script Date: 2/7/2017 8:33:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TeamPlayers](
	[TeamID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[Deleted] [bit] NULL,
	[Number] [int] NOT NULL CONSTRAINT [DF_TeamPlayers_Number]  DEFAULT ((0))
) ON [PRIMARY]

GO


