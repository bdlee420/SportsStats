USE [SportsStats]
GO

/****** Object:  Table [dbo].[StatTypes]    Script Date: 2/27/2017 5:36:55 PM ******/
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
	[QuickButtonText] [varchar](10) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


