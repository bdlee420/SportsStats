GO

/****** Object:  Table [dbo].[Users]    Script Date: 3/11/2017 12:27:05 PM ******/
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


