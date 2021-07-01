USE CAT_RTIS
GO




IF (OBJECT_ID('[dbo].[tbl_Tolerance]') IS NOT NULL)
	DROP TABLE [dbo].[tbl_Tolerance]

CREATE TABLE [dbo].[tbl_Tolerance]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[itemGroup] VARCHAR(20) NOT NULL,
	[tolerance] DECIMAL(18,7) NOT NULL
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
RETURNS  DECIMAL(18,7)
BEGIN
RETURN (SELECT [tolerance] FROM [dbo].[tbl_Tolerance] WHERE [itemGroup] = @itemGroup)
END
GO





IF (OBJECT_ID('[dbo].[fn_CalculateVariance]') IS NOT NULL)
	DROP FUNCTION [dbo].[fn_CalculateVariance]
GO

CREATE FUNCTION [dbo].[fn_CalculateVariance](@diff FLOAT, @tolerance DECIMAL(18,5))
RETURNS DECIMAL(18,5)
BEGIN
DECLARE @variance DECIMAL(18,5)

	IF ABS(@diff) >= @tolerance
		SET @variance = 0
	ELSE
		SET @variance = @diff
RETURN @variance
END
GO




IF (OBJECT_ID('[dbo].[fn_GetDifference]') IS NOT NULL)
	DROP FUNCTION [dbo].[fn_GetDifference]
GO

CREATE FUNCTION [dbo].[fn_GetDifference](@count1 DECIMAL(18,5), @sysCount DECIMAL(18,5))
RETURNS DECIMAL(18,5)
BEGIN
DECLARE @diff DECIMAL(18,5)

	IF @count1 >= @sysCount
		SET @diff = @count1 - @sysCount
	ELSE
		SET @diff = @sysCount - @count1
RETURN @diff
END
GO





SELECT [dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](0, 6.999), null)





select ABS(-6) AS result



BEGIN
IF 0.00005 >= 0.00001
	SELECT 'YES'
ELSE 
	SELECT 'NO'
END





if ABS(0.00005) <= null
	select 'yes'
else
	select 'no'









