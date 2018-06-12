GO

/****** Object:  Table [dbo].[Teams]    Script Date: 2/7/2017 8:33:24 AM ******/
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


