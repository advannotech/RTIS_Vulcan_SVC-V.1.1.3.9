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
WHERE il.[fSystemQty] < il.[fCountQty] 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
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
WHERE (il.[fCountQty] = 0 OR il.[fCountQty] IS NULL OR il.[fCountQty] = '') 
AND (il.[fCountQty2] = 0 OR il.[fCountQty2] IS NULL OR il.[fCountQty2] = '') 
AND (il.[fCountQty] <> 0 OR il.[fCountQty2] <> 0 OR il.[fSystemQty] <> 0) 
AND il.[bOnST] = 1 
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
WHERE il.[fCountQty] = il.[fSystemQty] 
AND il.[fCountQty] <> 0 
AND il.[fSystemQty] <> 0 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
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
WHERE il.[fCountQty] <> il.[fCountQty2] 
AND il.[bOnST] = 1 
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
WHERE il.[bOnST] = 0 
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
FROM [RTIS_InvCountLines] il
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
				WHERE [vLotNumber] = TRIM(@lot)
				AND [bJobRunning] = 1)
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




--Archived stock take variance
IF (OBJECT_ID('[dbo].[vw_GetStockTakeVariancesArchives]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetStockTakeVariancesArchives]
GO




CREATE VIEW [dbo].[vw_GetStockTakeVariancesArchives]
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
, il.[bOnST] AS [gcOnST]
FROM [RTIS_InvCountArchiveLines] il
INNER JOIN[RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID]
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking]
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON il.[iBinLocationId] = b.[idBinLocation]
GO







--Archived Items below system count
IF (OBJECT_ID('[dbo].[vw_GetArchivedItemsBelowSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetArchivedItemsBelowSystemCount]
GO

CREATE VIEW [dbo].[vw_GetArchivedItemsBelowSystemCount]
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[fSystemQty] > il.[fCountQty]  
AND il.[fCountQty] <> 0 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
GO




--Archived Items Above system count
IF (OBJECT_ID('[dbo].[vw_GetArchivedItemsAboveSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetArchivedItemsAboveSystemCount]
GO

CREATE VIEW [dbo].[vw_GetArchivedItemsAboveSystemCount]
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[fSystemQty] < il.[fCountQty] 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
GO




--Uncounted Archived Items
IF (OBJECT_ID('[dbo].[vw_GetArchivedItemsUncounted]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetArchivedItemsUncounted]
GO

CREATE VIEW [dbo].[vw_GetArchivedItemsUncounted]
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE (il.[fCountQty] = 0 OR il.[fCountQty] IS NULL OR il.[fCountQty] = '') 
AND (il.[fCountQty2] = 0 OR il.[fCountQty2] IS NULL OR il.[fCountQty2] = '') 
AND (il.[fCountQty] <> 0 OR il.[fCountQty2] <> 0 OR il.[fSystemQty] <> 0) 
AND il.[bOnST] = 1 
GO






-- Archived Items equal to system count
IF (OBJECT_ID('[dbo].[vw_GetArchivedItemsEqualSystemCount]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetArchivedItemsEqualSystemCount]
GO

CREATE VIEW [dbo].[vw_GetArchivedItemsEqualSystemCount]
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[fCountQty] = il.[fSystemQty] 
AND il.[fCountQty] <> 0 
AND il.[fSystemQty] <> 0 
AND il.[fCountQty] = il.[fCountQty2] 
AND il.[bOnST] = 1 
GO






--Unqual scanner Archived quantities
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[fCountQty] <> il.[fCountQty2] 
AND il.[bOnST] = 1 
GO






--unlisted Archived Items
IF (OBJECT_ID('[dbo].[vw_GetArchivedUnlistedItems]') IS NOT NULL)
	DROP VIEW [dbo].[vw_GetArchivedUnlistedItems]
GO

CREATE VIEW [dbo].[vw_GetArchivedUnlistedItems]
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
FROM [RTIS_InvCountArchiveLines] il 
INNER JOIN [RTIS_InvCount] i ON i.[idInvCount] = [iInvCountID] 
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = il.[iStockID] 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
LEFT JOIN [Cataler_SCN].[dbo].[_btblBINLocation] b ON b.[idBinLocation] = il.[iBinLocationId] 
LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON il.[iLotTrackingID] = l.[idLotTracking] 
WHERE il.[bOnST] = 0 
GO



---Update Records from Permission Modules
BEGIN
	IF NOT EXISTS (SELECT [vPermission_Name] FROM [dbo].[ltbl_Module_Perms] WHERE [vPermission_Name] = 'ZECT Records')
	BEGIN
		UPDATE [dbo].[ltbl_Module_Perms]
		SET [vPermission_Name] = 'ZECT Records' 
		WHERE [vPermission_Name] = 'ZECT Jobs'
	END
END
GO

BEGIN
	IF NOT EXISTS (SELECT [vPermission_Name] FROM [dbo].[ltbl_Module_Perms] WHERE [vPermission_Name] = 'AW Records')
	BEGIN
		UPDATE [dbo].[ltbl_Module_Perms]
		SET [vPermission_Name] = 'AW Records' 
		WHERE [vPermission_Name] = 'AW Jobs'
	END
END
GO

BEGIN
	IF NOT EXISTS (SELECT [vPermission_Name] FROM [dbo].[ltbl_Module_Perms] WHERE [vPermission_Name] = 'PGM RM')
	BEGIN
		UPDATE [dbo].[ltbl_Module_Perms]
		SET [vPermission_Name] = 'PGM RM' 
		WHERE [vPermission_Name] = 'PGM Planning'
	END
END
GO

BEGIN
	IF NOT EXISTS (SELECT [vPermission_Name] FROM [dbo].[ltbl_Module_Perms] WHERE [vPermission_Name] = 'Production RM')
	BEGIN
		UPDATE [dbo].[ltbl_Module_Perms]
		SET [vPermission_Name] = 'Production RM' 
		WHERE [vPermission_Name] = 'Production Planning'
	END
END
GO







IF (OBJECT_ID('[dbo].[sp_UI_CheckCMSValue]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckCMSValue]
GO

CREATE PROC [dbo].[sp_UI_CheckCMSValue]
	@value VARCHAR(MAX),
	@valType VARCHAR(MAX)
AS
SELECT [vValType] FROM [COA].[tbl_CMS_Admin] WHERE [vValue] = @value AND [vValType] = @valType
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetCMSItems]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSItems]
GO

CREATE PROC [dbo].[sp_UI_GetCMSItems]
AS
SELECT [iLineID], [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'Item'
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetCMSUOMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSUOMs]
GO

CREATE PROC [dbo].[sp_UI_GetCMSUOMs]
AS
SELECT [iLineID], [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'UOM'
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSHeaders]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSHeaders]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSHeaders]
AS
SELECT s.[Code], s.[Description_1], CASE WHEN hc.[vStatus] IS NULL THEN 'NO' ELSE 'YES' END, ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[vUserCaptured], hc.[dtDateCreated], hc.[vUserApproved], hc.[dtDateApproved], ISNULL(hc.[iLineID], 0), hc.[iDocVersion],[vUserRejected],[dtRejected],[vReasons] FROM [Cataler_SCN].[dbo].[StkItem] s
LEFT JOIN [COA].[htbl_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
WHERE s.[ItemGroup] = '006' 
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetCMSItems_Add]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSItems_Add]
GO

CREATE PROC [dbo].[sp_UI_GetCMSItems_Add]
AS
SELECT [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'Item'
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetCMSUOMs_Add]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSUOMs_Add]
GO

CREATE PROC [dbo].[sp_UI_GetCMSUOMs_Add]
AS
SELECT [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'UOM'
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSApprovals]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSApprovals]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSApprovals]
AS
SELECT s.[Code], s.[Description_1], ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[iLineID], hc.[iDocVersion]  FROM [Cataler_SCN].[dbo].[StkItem] s
INNER JOIN [COA].[htbl_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
WHERE s.[ItemGroup] = '006' AND hc.[vStatus] <> 'Approved' AND hc.[vStatus] <> 'Rejected'
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSApprovalLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSApprovalLines]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSApprovalLines]
	@headerID VARCHAR(MAX)
AS
SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @headerID
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSApprovalLinesViww]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSApprovalLinesViww]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSApprovalLinesViww]
	@headerID VARCHAR(MAX)
AS
SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @headerID
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSApprovalLinesEdit]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSApprovalLinesEdit]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSApprovalLinesEdit]
	@headerID VARCHAR(MAX)
AS
SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @headerID
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetCMSApprovalImagee]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSApprovalImagee]
GO

CREATE PROC [dbo].[sp_UI_GetCMSApprovalImagee]
	@itemCode VARCHAR(MAX)
AS
SELECT [imApprovalSignature]
FROM [COA].[htbl_CMS_Docs] WHERE [vItemCode] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetCMSHeadersToArchive]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSHeadersToArchive]
GO

CREATE PROC [dbo].[sp_UI_GetCMSHeadersToArchive]
	@stockLink VARCHAR(MAX),
	@docVersion VARCHAR(MAX)
AS
SELECT [iLineID] FROM [COA].[htbl_CMS_Docs] WHERE [iStockID] = @stockLink AND [iDocVersion] < @docVersion
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetCMSArchiveHeaders]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSArchiveHeaders]
GO

CREATE PROC [dbo].[sp_UI_GetCMSArchiveHeaders]
AS
SELECT s.[Code], s.[Description_1], CASE WHEN hc.[vStatus] IS NULL THEN 'NO' ELSE 'YES' END, ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[vUserCaptured], hc.[dtDateCreated], hc.[vUserApproved], hc.[dtDateApproved], ISNULL(hc.[iLineID], 0), hc.[iDocVersion] FROM [Cataler_SCN].[dbo].[StkItem] s
RIGHT JOIN [COA].[htbl_Archive_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
WHERE s.[ItemGroup] = '006' 
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetCMSArchiveImage]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCMSArchiveImage]
GO

CREATE PROC [dbo].[sp_UI_GetCMSArchiveImage]
	@lineID VARCHAR(MAX)
AS
SELECT [imApprovalSignature]
FROM [COA].[htbl_Archive_CMS_Docs] WHERE [iLineID] = @lineID
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetItemCMSArchiveLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCMSArchiveLines]
GO

CREATE PROC [dbo].[sp_UI_GetItemCMSArchiveLines]
	@headerID VARCHAR(MAX)
AS
SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
FROM [COA].[ltbl_Archive_CMS_Docs] WHERE [iHeaderID] = @headerID
GO





IF (OBJECT_ID('[dbo].[sp_UI_CheckRTVendor]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckRTVendor]
GO

CREATE PROC [dbo].[sp_UI_CheckRTVendor]
	@vendorID VARCHAR(MAX)
AS
SELEcT [iVendorID] FROM [rtblEvoVendors]
WHERE [iVendorID] = @vendorID
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetVendorPOLinks]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetVendorPOLinks]
GO

CREATE PROC [dbo].[sp_UI_GetVendorPOLinks]
AS
SELECT ven.[iVendorID], ven.[vVendorName], ISNULL([vOrderNum], '- Not Linked -') AS [vOrderNum], [dtDateUpdated], '' AS [POs]
FROM [tbl_POLink] link 
RIGHT JOIN [rtblEvoVendors] ven ON ven.[iVendorID] = link.[iVendorID]
WHERE ven.[vVendorName] <> '' AND ven.[bSelected] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_CheckVendorPOLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckVendorPOLink]
GO

CREATE PROC [dbo].[sp_UI_CheckVendorPOLink]
	@vendorID VARCHAR(MAX)
AS
SELECT [iVendorID] FROM [tbl_POLink]
WHERE [iVendorID] = @vendorID
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetLinkedVendors]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetLinkedVendors]
GO

CREATE PROC [dbo].[sp_UI_GetLinkedVendors]
AS
SELECT DISTINCT [vVendorName] FROM [tbl_POLink]
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetLinkedPOs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetLinkedPOs]
GO

CREATE PROC [dbo].[sp_UI_GetLinkedPOs]
AS
SELECT DISTINCT [vOrderNum] FROM [tbl_POLink]
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetVendorPO]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetVendorPO]
GO

CREATE PROC [dbo].[sp_UI_GetVendorPO]
	@vendorName VARCHAR(MAX)
AS
SELECT [vOrderNum] FROM [tbl_POLink]
WHERE [vVendorName] = @vendorName
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetPOVendor]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPOVendor]
GO

CREATE PROC [dbo].[sp_UI_GetPOVendor]
	@PONumber VARCHAR(MAX)
AS
SELECT [vVendorName] FROM [tbl_POLink]
WHERE [vOrderNum] = @PONumber
GO






IF (OBJECT_ID('[dbo].[sp_MBL_CheckPOUnqLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckPOUnqLines]
GO

CREATE PROC [dbo].[sp_MBL_CheckPOUnqLines]
	@OrderNum VARCHAR(MAX)
AS
SELECT [vUnqBarcode], [Receive], [bValidated] FROM [tbl_unqBarcodes]
WHERE [ValidateRef] = @OrderNum
GO






IF (OBJECT_ID('[dbo].[sp_UI_AddCMSRecord]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_AddCMSRecord]
GO

CREATE PROC [dbo].[sp_UI_AddCMSRecord]
	@value VARCHAR(MAX),
	@valType VARCHAR(MAX)
AS
INSERT INTO [COA].[tbl_CMS_Admin] ([vValue],[vValType]) VALUES (@value, @valType)
GO






IF (OBJECT_ID('[dbo].[sp_UI_AddCMSDocHeader]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_AddCMSDocHeader]
GO

CREATE PROC [dbo].[sp_UI_AddCMSDocHeader]
	@id VARCHAR(MAX),
	@code VARCHAR(MAX),
	@username VARCHAR(MAX),
	@version VARCHAR(MAX)
AS
INSERT INTO [COA].[htbl_CMS_Docs] ([iStockID],[vItemCode],[dtDateCreated], [vUserCaptured],[vStatus],[iDocVersion]) 
OUTPUT INSERTED.iLineID 
VALUES (@id, @code, GETDATE(), @username, 'Waiting Approval', @version)
GO






IF (OBJECT_ID('[dbo].[sp_UI_AddVendorLookup]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_AddVendorLookup]
GO

CREATE PROC [dbo].[sp_UI_AddVendorLookup]
	@id VARCHAR(MAX),
	@name VARCHAR(MAX),
	@viewable VARCHAR(MAX)
AS
INSERT INTO [rtblEvoVendors] ([iVendorID]
                            ,[vVendorName]
                            ,[bSelected]
                            )
VALUES (@id, @name, @viewable)
GO





IF (OBJECT_ID('[dbo].[sp_UI_AddVendorPOLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_AddVendorPOLink]
GO

CREATE PROC [dbo].[sp_UI_AddVendorPOLink]
	@id VARCHAR(MAX),
	@name VARCHAR(MAX),
	@orderNum VARCHAR(MAX)
AS
IF NOT EXISTS
(   SELECT DISTINCT [vOrderNum],[vVendorName]
	FROM    [tbl_POLink] 
	WHERE   [vVendorName] =@id
	AND		[vOrderNum] IS NOT NULL
	AND		[vOrderNum] =@name
)BEGIN
	INSERT [tbl_POLink] ([iVendorID], [vVendorName],[vOrderNum],[dtDateUpdated])
	VALUES (@id, @name, @orderNum, GETDATE())
END
GO




IF (OBJECT_ID('[dbo].[sp_UI_LinkPOtoVendor]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_LinkPOtoVendor]
GO

CREATE PROC [dbo].[sp_UI_LinkPOtoVendor]
	@id VARCHAR(MAX),
	@name VARCHAR(MAX),
	@orderNum VARCHAR(MAX)
AS
INSERT INTO [tbl_POLink] ([iVendorID]
    ,[vVendorName]
    ,[vOrderNum]
    ,[dtDateUpdated]
    )
VALUES (@id, @name, @orderNum, GETDATE())
GO





IF (OBJECT_ID('[dbo].[sp_UI_UpdateCMSApproved]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateCMSApproved]
GO

CREATE PROC [dbo].[sp_UI_UpdateCMSApproved]
	@lineID VARCHAR(MAX),
	@image VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
UPDATE [COA].[htbl_CMS_Docs] 
SET [imApprovalSignature] = @image, [dtDateApproved] = GETDATE(), [vUserApproved] = @username, [vStatus] = 'Approved'  
WHERE [iLineID] = @lineID
GO





IF (OBJECT_ID('[dbo].[sp_UI_UpdateCMSRejected]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateCMSRejected]
GO

CREATE PROC [dbo].[sp_UI_UpdateCMSRejected]
	@lineID VARCHAR(MAX),
	@reason VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
UPDATE [COA].[htbl_CMS_Docs] 
SET [vReasons] = @reason, [dtRejected] = GETDATE(), [vUserRejected] = @username, [vStatus] = 'Rejected'  
WHERE [iLineID] = @lineID
GO





IF (OBJECT_ID('[dbo].[sp_UI_UpdateCMSEdited]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateCMSEdited]
GO

CREATE PROC [dbo].[sp_UI_UpdateCMSEdited]
	@id VARCHAR(MAX)
AS
UPDATE [COA].[htbl_CMS_Docs] 
SET [vStatus] = 'Waiting Approval', [vReasons] = NULL, [dtRejected] = NULL,  [vUserRejected] = NULL
WHERE [iLineID] = @id
GO




IF (OBJECT_ID('[dbo].[sp_UI_UpdateVendorLookup]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateVendorLookup]
GO

CREATE PROC [dbo].[sp_UI_UpdateVendorLookup]
	@id VARCHAR(MAX),
	@viewable VARCHAR(MAX)
AS
UPDATE [rtblEvoVendors] SET [bSelected]= @viewable WHERE [iVendorID] = @id
GO





IF (OBJECT_ID('[dbo].[sp_UI_UpdateVendorPOLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateVendorPOLink]
GO

CREATE PROC [dbo].[sp_UI_UpdateVendorPOLink]
	@id VARCHAR(MAX),
	@name VARCHAR(MAX),
	@orderNum VARCHAR(MAX)
AS
UPDATE [tbl_POLink] 
SET [vVendorName] = @name, [vOrderNum] = @orderNum, [dtDateUpdated] = GETDATE() 
WHERE [iVendorID] = @id
GO





IF (OBJECT_ID('[dbo].[sp_UI_ValidateLabels]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ValidateLabels]
GO

CREATE PROC [dbo].[sp_UI_ValidateLabels]
	@validRef VARCHAR(MAX)
AS
UPDATE [tbl_unqBarcodes] SET [bValidated] = 1 WHERE [ValidateRef] = @validRef
GO




IF (OBJECT_ID('[dbo].[sp_MBL_SetUnqReceived]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_SetUnqReceived]
GO

CREATE PROC [dbo].[sp_MBL_SetUnqReceived]
	@poRec VARCHAR(MAX),
	@barcode VARCHAR(MAX)
AS
UPDATE [tbl_unqBarcodes] SET [Receive] = @poRec WHERE [vUnqBarcode] = @barcode
GO





IF (OBJECT_ID('[dbo].[sp_UI_CMSItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CMSItem]
GO

CREATE PROC [dbo].[sp_UI_CMSItem]
	@id VARCHAR(MAX)
AS
DELETE FROM [COA].[tbl_CMS_Admin] WHERE [iLineID] = @id
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeletePOLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeletePOLines]
GO

CREATE PROC [dbo].[sp_UI_DeletePOLines]
	@orderNum VARCHAR(MAX)
AS
DELETE FROM [tblPOLines] WHERE [vOrderNum] = @orderNum
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteCMSDocLiness]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteCMSDocLiness]
GO

CREATE PROC [dbo].[sp_UI_DeleteCMSDocLiness]
	@id VARCHAR(MAX)
AS
DELETE FROM [COA].[ltbl_CMS_Docs]
WHERE [iHeaderID] = @id
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteInvalidLabels]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteInvalidLabels]
GO

CREATE PROC [dbo].[sp_UI_DeleteInvalidLabels]
	@orderNum VARCHAR(MAX)
AS
DELETE FROM [tbl_unqBarcodes] WHERE [ValidateRef] = @orderNum AND [bValidated] = 0
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteCMSHeader]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteCMSHeader]
GO

CREATE PROC [dbo].[sp_UI_DeleteCMSHeader]
	@id VARCHAR(MAX)
AS
DELETE FROM [COA].[htbl_CMS_Docs] WHERE [iLineID] = @id
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteCMSLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteCMSLines]
GO

CREATE PROC [dbo].[sp_UI_DeleteCMSLines]
	@id VARCHAR(MAX)
AS
DELETE FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @id
GO





IF (OBJECT_ID('[dbo].[sp_UI_DeleteCMSLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteCMSLines]
GO

CREATE PROC [dbo].[sp_UI_DeleteCMSLines]
	@id VARCHAR(MAX)
AS
DELETE FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @id
GO








--Add Reprint Module---
USE [CAT_RTIS]
GO
BEGIN
   IF NOT EXISTS (SELECT * FROM [ltbl_Module_Perms] 
                   WHERE [iPermission_ID] = '1052')
   BEGIN
INSERT INTO [dbo].[ltbl_Module_Perms]
           ([vPermission_Name]
           ,[bPermissionActive]
           ,[bHasLabel]
           ,[iModuleID]
           ,[bIsNested]
           ,[vNestNode]
           ,[Indx]
           ,[bUIPerm]
           ,[bPGMPerm])
     VALUES
	 ('PO Reprinting',
	 'True',
	 'True',
	 2,
	 'False',
	 NULL,
	 2,
	 'True',
	 'False'
	 )
   END
END

--Add Reprint Module User Role---

USE [CAT_RTIS]
GO
BEGIN
   IF NOT EXISTS (SELECT * FROM [ltbl_userRoleLines] 
                   WHERE [iPermission_ID] = 1052)
   BEGIN
INSERT INTO [dbo].[ltbl_userRoleLines]
           ([iRole_ID]
           ,[iPermission_ID]
           ,[bPermission_Active]
           ,[dPermission_Added]
           ,[dPermission_Removed])
     VALUES
           (
		   2,
		   1052,
		   'True',
		   '2021-05-17 15:27:06.747'
           ,NULL
		   )
   END
END

---------Add Label Perm Com for PO Reprinting Module------------
USE [CAT_RTIS]
GO
BEGIN
   IF NOT EXISTS (SELECT * FROM [rtbl_LabelPermCom] 
                   WHERE [iLabelID] = 1
				   AND [iPermissionID]=1052)
   BEGIN
INSERT INTO [dbo].[rtbl_LabelPermCom]
           ([iLabelID]
           ,[iPermissionID])
     VALUES
           (1,
           1052
		   )
   END
END

----------Add Perm Label For Reprint Module-------------
USE [CAT_RTIS]
GO
BEGIN
   IF NOT EXISTS (SELECT * FROM [rtbl_PermLabels] 
                   WHERE [iPermID] = 1052)
   BEGIN
INSERT INTO [rtbl_PermLabels]
           ([vLabelName]
           ,[iPermID])
     VALUES
           (
		   'Stock_GRV Label.repx'
		   ,1052
           )
   END
END


USE [CAT_RTIS]
GO












