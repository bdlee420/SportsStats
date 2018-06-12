﻿GO
/****** Object:  StoredProcedure [dbo].[GetStatTypes]    Script Date: 3/21/2017 10:59:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetStatTypes]	
	@SportID	int = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		ID, 
				Name,
				DisplayName,
				DefaultShow,
				IsCalculated,
                SelectionDisplayOrder,
                GridDisplayOrder,
                ValueType,
				QuickButtonOrder,
				QuickButtonText,
				IsPositive,
				AutoGenerated,
				SportID
	FROM		StatTypes
	WHERE		SportID = @SportID OR @SportID is null
END

