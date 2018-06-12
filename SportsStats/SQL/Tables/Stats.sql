GO

/****** Object:  Table [dbo].[Stats]    Script Date: 2/7/2017 8:32:21 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Stats](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StatTypeID] [int] NOT NULL,
	[Value] [int] NOT NULL,
	[GameID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[TeamID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Stats_Deleted]  DEFAULT ((0))
) ON [PRIMARY]

GO


