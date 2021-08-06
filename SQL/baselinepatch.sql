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

CREATE FUNCTION [dbo].[fn_CalculateVariance](@diff DECIMAL(18,5), @tolerance DECIMAL(18,5))
RETURNS FLOAT
BEGIN
DECLARE @variance DECIMAL(18,5), @start INT

	IF (@diff BETWEEN -(@tolerance) AND @tolerance) AND (@diff NOT IN (-(@tolerance),@tolerance))
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

	SET @diff = @count1 - @sysCount
RETURN @diff 
END
GO



-- query from CATscan UI, you'll find it on DirectQueries.cs

SELECT
il.[idInvCountLines] AS [gclineID]
,s.[Code] AS [gcItemCode]
, s.[Bar_Code] AS [gcBarcode]
, s.[Description_1] AS [gcItemDesc]
, b.[cBinLocationName] AS [gcBin]
, l.[cLotDescription] AS [gcLot]
, ROUND(il.[fCountQty], 5) AS [gcCounted]
, ROUND(il.[fCountQty2], 5) AS [gcCounted2]
, ROUND(il.[fSystemQty], 5) AS [gcSystem]
, CASE WHEN(il.[fCountQty] = il.[fCountQty2])  
THEN CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50))
ELSE 'SV: ' + CAST(CAST([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]) AS FLOAT) AS VARCHAR(50)) END AS [gcVarience]
, w.[Code] AS [gcWhseCode]
, w.[Name] AS [gcWhseName]
, il.[bIsCounted] AS [gcIsCounted]
, il.[bOnST] AS [gcOnST]
FROM[RTIS_InvCountLines] il
INNER JOIN[RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID]
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking]
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON il.[iBinLocationId] = b.[idBinLocation]
WHERE i.[cInvCountNo] = 'STK0131'  
ORDER BY
CASE WHEN(il.[fSystemQty] - il.[fCountQty]) < 0 THEN il.[fCountQty]
WHEN(il.[fCountQty] - il.[fSystemQty]) < 0 THEN il.[fCountQty] END DESC



-------------------------------------------------------------------------------------



SELECT * FROM [RTIS_InvCountLines]
WHERE [iInvCountID] = 131


-- update the variance expression
UPDATE [RTIS_InvCountLines]
SET [fCountQty] = 1.95123,
[fCountQty2] = 1.95123,
[fSystemQty] = 2
WHERE [idInvCountLines] = 340




----------------



--IF (@diff BETWEEN -(@tolerance) AND @tolerance) AND (@diff NOT IN (-(@tolerance),@tolerance))
IF (0.05 BETWEEN -(0.05) AND 0.05) AND (0.05 NOT IN (-(0.05),0.05))
	SELECT 'Unacceptable'
ELSE
	SELECT 'Acceptable'




