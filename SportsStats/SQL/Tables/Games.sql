GO

/****** Object:  Table [dbo].[Games]    Script Date: 2/7/2017 8:30:33 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Games](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[GameDate] [datetime] NOT NULL,
	[Team1ID] [int] NOT NULL,
	[Team2ID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_Games_Deleted]  DEFAULT ((0))
) ON [PRIMARY]

GO


