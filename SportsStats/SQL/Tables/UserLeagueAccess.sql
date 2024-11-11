GO

/****** Object:  Table [dbo].[Players]    Script Date: 2/7/2017 8:31:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserLeagueAccess](
	[UserID] [int] NOT NULL,
	[LeagueID] [int] NOT NULL,
	[RoleID] [int] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


