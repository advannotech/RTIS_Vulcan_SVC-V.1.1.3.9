USE CAT_RTIS
GO




IF (OBJECT_ID('[dbo].[tbl_Tolerance]') IS NOT NULL)
	DROP TABLE [dbo].[tbl_Tolerance]

CREATE TABLE [dbo].[tbl_Tolerance]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[itemGroup] VARCHAR(20) NOT NULL,
	[tolerance] DECIMAL(18,9) NOT NULL
)


INSERT INTO [dbo].[tbl_Tolerance]([itemGroup],[tolerance])
VALUES ('002', 0.00001),
		('009', 0.05),
		('010', 0.02),
		('011', 0.02)
GO




IF (OBJECT_ID('[dbo].[fn_GetTolerance]') IS NOT NULL)
	DROP FUNCTION [dbo].[fn_GetTolerance]
GO

CREATE FUNCTION [dbo].[fn_GetTolerance](@itemGroup VARCHAR(20))
RETURNS  DECIMAL(18,9)
BEGIN
RETURN (SELECT [tolerance] FROM [dbo].[tbl_Tolerance] WHERE [itemGroup] = @itemGroup)
END
GO





IF (OBJECT_ID('[dbo].[fn_CalculateVariance]') IS NOT NULL)
	DROP FUNCTION [dbo].[fn_CalculateVariance]
GO

CREATE FUNCTION [dbo].[fn_CalculateVariance](@diff FLOAT, @tolerance DECIMAL(18,9))
RETURNS FLOAT
BEGIN
DECLARE @variance FLOAT

	IF @diff < @tolerance
		SET @variance = 0
	ELSE
		SET @variance = @diff
RETURN @variance
END
GO




IF (OBJECT_ID('[dbo].[fn_GetDifference]') IS NOT NULL)
	DROP FUNCTION [dbo].[fn_GetDifference]
GO

CREATE FUNCTION [dbo].[fn_GetDifference](@count1 DECIMAL(18,9), @sysCount DECIMAL(18,9))
RETURNS DECIMAL(18,9)
BEGIN
DECLARE @diff DECIMAL(18,9)

	SET @diff = @count1 - @sysCount
RETURN @diff
END
GO







--select [dbo].[fn_CalculateVariance](0.002, 0.00001)





select [dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](-0.019,-0.019), -0.02) AS Variance







--select [dbo].[fn_GetDifference](1303.699,1303.697)











