GO

/****** Object:  Table [dbo].[Players]    Script Date: 2/7/2017 8:31:35 AM ******/
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


