GO

/****** Object:  Table [dbo].[Players]    Script Date: 2/7/2017 8:31:35 AM ******/
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


