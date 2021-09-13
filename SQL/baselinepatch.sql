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


-----------------------------------------------------------------------------------------------------




--Items below system count
IF (OBJECT_ID('[dbo].[vw_GetItemsBelowSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetItemsBelowSystemCount]
GO

CREATE VIEW [dbo].[vw_GetItemsBelowSystemCount]
AS
SELECT s.[Code] AS [ItemCode],
i.[cInvCountNo],
s.[Description_1] AS [Desc] ,
b.[cBinLocationName] AS Bin ,
ROUND(il.[fCountQty], 5)  AS [Counted], 
ROUND(il.[fCountQty2], 5) AS [Counted2],
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE]  ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[fSystemQty] > il.[fCountQty]  
AND il.[fCountQty] <> 0 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
--ORDER BY s.[Code]
GO




--Items Above system count
IF (OBJECT_ID('[dbo].[vw_GetItemsAboveSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetItemsAboveSystemCount]
GO

CREATE VIEW [dbo].[vw_GetItemsAboveSystemCount]
AS
SELECT s.[Code] AS [ItemCode], 
i.[cInvCountNo],
s.[Description_1] AS [Desc] , 
b.[cBinLocationName] AS Bin ,
ROUND(il.[fCountQty], 5)  AS [Counted], 
ROUND([fCountQty2], 5) AS [Counted2] ,
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE] ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
--WHERE i.[cInvCountNo] = 'STK0182' 
WHERE il.[fSystemQty] < il.[fCountQty] 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
--ORDER BY s.[Code]
GO




--Uncounted Items
IF (OBJECT_ID('[dbo].[vw_GetItemsUncounted]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetItemsUncounted]
GO

CREATE VIEW [dbo].[vw_GetItemsUncounted]
AS
SELECT s.[Code] AS [ItemCode] ,  
i.[cInvCountNo],
s.[Description_1] AS [Desc] , 
b.[cBinLocationName] AS Bin ,
'0'  AS [Counted],  
ROUND([fCountQty2], 5) AS [Counted2] ,
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE] ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
--WHERE i.[cInvCountNo] = 'STK0182' 
WHERE (il.[fCountQty] = 0 OR il.[fCountQty] IS NULL OR il.[fCountQty] = '') 
AND (il.[fCountQty2] = 0 OR il.[fCountQty2] IS NULL OR il.[fCountQty2] = '') 
AND (il.[fCountQty] <> 0 OR il.[fCountQty2] <> 0 OR il.[fSystemQty] <> 0) 
AND il.[bOnST] = 1 
--ORDER BY s.[Code]
GO






--Items equal to system count
IF (OBJECT_ID('[dbo].[vw_GetItemsEqualSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetItemsEqualSystemCount]
GO

CREATE VIEW [dbo].[vw_GetItemsEqualSystemCount]
AS
SELECT s.[Code] AS [ItemCode],  
i.[cInvCountNo],
s.[Description_1] AS [Desc], 
b.[cBinLocationName] AS Bin ,
ROUND(il.[fCountQty], 5)  AS [Counted], 
ROUND([fCountQty2], 5) AS [Counted2] ,
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE] ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
--WHERE i.[cInvCountNo] = 'STK0140' 
WHERE il.[fCountQty] = il.[fSystemQty] 
AND il.[fCountQty] <> 0 
AND il.[fSystemQty] <> 0 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
--ORDER BY s.[Code]
GO






--Unqual scanner quantities
IF (OBJECT_ID('[dbo].[vw_GetUnequalScannerQty]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetUnequalScannerQty]
GO

CREATE VIEW [dbo].[vw_GetUnequalScannerQty]
AS
SELECT s.[Code] AS [ItemCode], 
i.[cInvCountNo],
s.[Description_1] AS [Desc] , 
b.[cBinLocationName] AS Bin ,
ROUND(il.[fCountQty], 5)  AS [Counted], 
ROUND([fCountQty2], 5) AS [Counted2] ,
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE] ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
--WHERE i.[cInvCountNo] = 'STK0182' 
WHERE il.[fCountQty] <> il.[fCountQty2] 
AND il.[bOnST] = 1 
--ORDER BY s.[Code]
GO






--unlisted Items
IF (OBJECT_ID('[dbo].[vw_GetUnlistedItems]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetUnlistedItems]
GO

CREATE VIEW [dbo].[vw_GetUnlistedItems]
AS
SELECT s.[Code] AS [ItemCode], 
i.[cInvCountNo],
s.[Description_1] AS [Desc], 
b.[cBinLocationName] AS Bin ,
ROUND(il.[fCountQty], 5)  AS [Counted], 
ROUND([fCountQty2], 5) AS [Counted2] ,
ROUND(il.[fSystemQty], 5) AS [System], 
ROUND(CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup])) AS VARCHAR(50)), 5) AS [Variance] ,
w.[Code] AS [WHSE] ,
i.[dPrepared] 
FROM [RTIS_InvCountLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
--WHERE i.[cInvCountNo] = 'STK0182' 
WHERE il.[bOnST] = 0 
--ORDER BY s.[Code]
GO






--Stock take variance
IF (OBJECT_ID('[dbo].[vw_GetStockTakeVariances]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetStockTakeVariances]
GO

CREATE VIEW [dbo].[vw_GetStockTakeVariances]
AS
SELECT
il.[idInvCountLines] AS [gclineID]
,i.[cInvCountNo]
,s.[Code] AS [gcItemCode]
, s.[Bar_Code] AS [gcBarcode]
, s.[Description_1] AS [gcItemDesc]
, b.[cBinLocationName] AS [gcBin]
, l.[cLotDescription] AS [gcLot]
, ROUND(il.[fCountQty], 4) AS [gcCounted]
, ROUND(il.[fCountQty2], 4) AS [gcCounted2]
, ROUND(il.[fSystemQty], 4) AS [gcSystem]
, CAST([dbo].[fn_CalculateVariance]([dbo].[fn_GetDifference](il.[fCountQty],il.[fSystemQty]), [dbo].[fn_GetTolerance](s.[ItemGroup]))AS VARCHAR(50)) AS [gcVarience]
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
GO



------------------------------------------------------------------------------------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[sp_GetAllTransfers]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetAllTransfers]
GO

CREATE PROC [dbo].[sp_GetAllTransfers]
	@transStart varchar(50),
	@transEnd varchar(50),
	@process varchar(50),
	@rows int
AS
SELECT TOP(@rows) wt.[iLineID]
,[vItemCode]
,[vLotNumber]
,[vWarehouse_From]
,[vWarehouse_To]
,[dQtyTransfered]
,[dtDateTransfered]
,[vUsername]
,pr.[vDisplayName]
,[vTransDesc]
,'' AS [Save]
,[vStatus]
,[vFailureReason]
,[dtDateFailed]
,'false' AS [bChanged]
FROM [tbl_WHTPending] wt
INNER JOIN [tbl_ProcNames] pr ON wt.[vProcess]  = [vProcName]
WHERE wt.[vProcess] LIKE '%' + @process + '%'
AND CAST([dtDateTransfered] AS DATE) BETWEEN CAST(@transStart AS DATE) AND CAST(@transEnd AS DATE)
UNION
SELECT TOP(@rows) wt.[iLineID]
,[vItemCode]
,[vLotNumber]
,[vWarehouse_From]
,[vWarehouse_To]
,[dQtyTransfered] 
,[dtDateTransfered]
,[vUsername]
,pr.[vDisplayName]
,[vTransDesc]
,'' AS [Save]
,'Posted' [vStatus]
,'' AS [vFailureReason]
,NULL AS [dtDateFailed]
,'false' AS [bChanged]
FROM [tbl_WHTCompleted] wt
INNER JOIN [tbl_ProcNames] pr ON wt.[vProcess]  = [vProcName]
WHERE wt.[vProcess] LIKE '%' + @process + '%'
AND CAST([dtDateTransfered] AS DATE) BETWEEN CAST(@transStart AS DATE) AND CAST(@transEnd AS DATE)
ORDER BY [dtDateTransfered] DESC
GO





IF (OBJECT_ID('[dbo].[sp_GetTransfers]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetTransfers]
GO

CREATE PROC [dbo].[sp_GetTransfers]
	@transStart varchar(50),
	@transEnd varchar(50),
	@failedStart varchar(50),
	@failedEnd varchar(50),
	@process varchar(50),
	@status varchar(50),
	@rows int
AS
SELECT TOP (@rows)  wt.[iLineID]
,[vItemCode]
,[vLotNumber]
,[vWarehouse_From]
,[vWarehouse_To]
,[dQtyTransfered]
,[dtDateTransfered]
,[vUsername]
,pr.[vDisplayName]
,[vTransDesc]
,'' AS [Save]
,[vStatus]
,[vFailureReason]
,[dtDateFailed]
,'false' AS [bChanged]
FROM [tbl_WHTPending]  wt
INNER JOIN [tbl_ProcNames] pr ON wt.[vProcess]  = [vProcName]
WHERE [vStatus] LIKE @status AND [vProcess] LIKE '%' + @process + '%'
AND CAST([dtDateTransfered] AS DATE) BETWEEN CAST(@transStart AS DATE) AND CAST(@transEnd AS DATE)
OR CAST([dtDateFailed] AS DATE) BETWEEN CAST(@failedStart AS DATE) AND CAST(@failedEnd AS DATE)
ORDER BY [dtDateTransfered] DESC
GO







IF (OBJECT_ID('[dbo].[sp_GetPostedTransfers]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetPostedTransfers]
GO

CREATE PROC [dbo].[sp_GetPostedTransfers]
	@transStart varchar(50),
	@transEnd varchar(50),
	@process varchar(50),
	@rows int
AS
SELECT TOP (@rows) wt.[iLineID]
,wt.[vItemCode]
,wt.[vLotNumber]
,wt.[vWarehouse_From]
,wt.[vWarehouse_To]
,wt.[dQtyTransfered] 
,wt.[dtDateTransfered]
,wt.[vUsername]
,pr.[vDisplayName]
,wt.[vTransDesc]
,'' AS [Save]
,'Posted' AS  [vStatus]
,'' AS [vFailureReason]
,NULL AS[dtDateFailed]
,'false' AS [bChanged]
FROM [tbl_WHTCompleted] wt
INNER JOIN [tbl_ProcNames] pr ON wt.[vProcess]  = [vProcName]
WHERE wt.[vProcess] LIKE '%' + @process + '%'
AND CAST([dtDateTransfered] AS DATE) BETWEEN CAST(@transStart AS DATE) AND CAST(@transEnd AS DATE)
ORDER BY [dtDateTransfered] DESC
GO







IF (OBJECT_ID('[dbo].[sp_ManualCloseZectJob]') IS NOT NULL)
	DROP PROC [dbo].[sp_ManualCloseZectJob]
GO

CREATE PROC [dbo].[sp_ManualCloseZectJob]
	@lot varchar(50)
AS
	IF EXISTS (SELECT * FROM [dbo].[tbl_RTIS_Zect_Jobs]
				WHERE [vLotNumber] = TRIM(@lot))
	BEGIN
		UPDATE [dbo].[tbl_RTIS_Zect_Jobs]
		SET [bJobRunning] = 0
		WHERE [vLotNumber] = TRIM(@lot) 

		SELECT 1 AS RETVAL
	END
	ELSE 
		SELECT 0 AS RETVAL
GO







IF (OBJECT_ID('[dbo].[sp_ManualCloseAWJob]') IS NOT NULL)
	DROP PROC [dbo].[sp_ManualCloseAWJob]
GO

CREATE PROC [dbo].[sp_ManualCloseAWJob]
	@lot varchar(50)
AS
	IF EXISTS (SELECT * FROM [dbo].[tbl_RTIS_AW_Jobs]
				WHERE [vLotNumber] = TRIM(@lot))
	BEGIN
		UPDATE [dbo].[tbl_RTIS_AW_Jobs]
		SET [bJobRunning] = 0
		WHERE [vLotNumber] = TRIM(@lot) 

		SELECT 1 AS RETVAL
	END
	ELSE 
		SELECT 0 AS RETVAL
GO




-- DELETE duplicate permission modules
WITH cte AS (
    SELECT 
        [iLine_ID],
		[iRole_ID],
		[iPermission_ID],
		[bPermission_Active],
		[dPermission_Added],
		[dPermission_Removed],
        ROW_NUMBER() OVER (
            PARTITION BY 
                [iRole_ID],
				[iPermission_ID]
            ORDER BY 
                [iRole_ID],
				[iPermission_ID]
        ) row_num
     FROM 
        [dbo].[ltbl_userRoleLines]
)
DELETE FROM cte
WHERE row_num > 1;





























































