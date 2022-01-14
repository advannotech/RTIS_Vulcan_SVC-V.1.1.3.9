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




------------------------ PO Receiving --------------------------------------


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

-----------------------------------------------------------------------------------------------------------------------------------------





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



--------------------------------------------------------- AW ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[sp_UI_GetAWCatalystRaws]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWCatalystRaws]
GO

CREATE PROC [dbo].[sp_UI_GetAWCatalystRaws]
	@catalystCode VARCHAR(MAX)
AS
SELECT [vRMCode], [vRMDesc], '' FROM [tbl_RTIS_AW_Raws] WHERE [vAWCode] = @catalystCode
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAWLinkExists]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWLinkExists]
GO

CREATE PROC [dbo].[sp_UI_GetAWLinkExists]
	@catalystCode VARCHAR(MAX),
	@rmCode VARCHAR(MAX)
AS
SELECT [vRMCode] FROM [tbl_RTIS_AW_Raws] WHERE [vAWCode] = @catalystCode AND [vRMCode] = @rmCode
GO




IF (OBJECT_ID('[dbo].[sp_UI_GeAWJobsToManufacture]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GeAWJobsToManufacture]
GO

CREATE PROC [dbo].[sp_UI_GeAWJobsToManufacture]
AS
 SELECT aj.[iLIneID], aj.[vJobUnq], aj.[vAWCode], aj.[vLotNumber], aj.[dQty], aj.[dQtyManuf], SUM(ao.dQty), aj.[dtStarted], aj.[vUserStarted],[bJobRunning] FROM [tbl_RTIS_AW_Jobs] aj
  INNER JOIN [tbl_RTIS_AW_OutPut] ao ON ao.[iJobID] = aj.[iLIneID]
  WHERE ISNULL(ao.[bManuf], 0) = 0 GROUP BY aj.[iLIneID], aj.[vJobUnq], aj.[vAWCode], aj.[vLotNumber], aj.[dQty], aj.[dQtyManuf], aj.[dtStarted], aj.[vUserStarted],[bJobRunning]
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetAWPalletsToManufacture]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWPalletsToManufacture]
GO

CREATE PROC [dbo].[sp_UI_GetAWPalletsToManufacture]
	@headerID VARCHAR(MAX)
AS
SELECT [iLIneID], [vPalletCode], [vPalletNo], [dQty], [dtDateRecorded], [vUserRecorded], '' 
FROM [tbl_RTIS_AW_OutPut] 
WHERE [iJobID] = @headerID AND ISNULL([bManuf], 0) = 0
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAWBatchTotal]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWBatchTotal]
GO

CREATE PROC [dbo].[sp_UI_GetAWBatchTotal]
	@headerID VARCHAR(MAX)
AS
	SELECT SUM([dQty]) AS [Total] FROM [tbl_RTIS_AW_OutPut]
    WHERE ISNULL([bManuf], 0) = 0 AND [iJobID] = @headerID
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAWRawMaterials]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWRawMaterials]
GO

CREATE PROC [dbo].[sp_UI_GetAWRawMaterials]
	@headerID VARCHAR(MAX)
AS
	SELECT [vCatalystCode], [vCatalystLot] FROM [tbl_RTIS_AW_Input] WHERE [iJobID] = @headerID
GO




IF (OBJECT_ID('[dbo].[sp_AW_CheckJobRunning]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_CheckJobRunning]
GO

CREATE PROC [dbo].[sp_AW_CheckJobRunning]
AS
	SELECT [vJobUnq] FROM [tbl_RTIS_AW_Jobs] WHERE [bJobRunning] = 1
GO





IF (OBJECT_ID('[dbo].[sp_AW_CheckSpecificJobRunning]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_CheckSpecificJobRunning]
GO

CREATE PROC [dbo].[sp_AW_CheckSpecificJobRunning]
	@jobNo VARCHAR(MAX)
AS
	SELECT [bJobRunning] FROM [tbl_RTIS_AW_Jobs] WHERE [bJobRunning] = 1 AND [vJobUnq] = @jobNo
GO






IF (OBJECT_ID('[dbo].[sp_AW_GetJobID]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetJobID]
GO

CREATE PROC [dbo].[sp_AW_GetJobID]
	@jobNo VARCHAR(MAX)
AS
	SELECT [iLIneID] FROM [tbl_RTIS_AW_Jobs] WHERE [bJobRunning] = 1 AND [vJobUnq] = @jobNo
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetJobInfo]
GO

CREATE PROC [dbo].[sp_AW_GetJobInfo]
	@jobNo VARCHAR(MAX)
AS
	SELECT [vAWCode], [vLotNumber], [vPGMCode], [vPGMLot], [dQty], [dQtyManuf]
    FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetLastJobPallet]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetLastJobPallet]
GO

CREATE PROC [dbo].[sp_AW_GetLastJobPallet]
	@jobNo VARCHAR(MAX)
AS
	 SELECT TOP 1 [vPalletCode] FROM [tbl_RTIS_AW_OutPut] WHERE [iJobID] = @jobNo ORDER BY [iLIneID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetValidReprintJobLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetValidReprintJobLots]
GO

CREATE PROC [dbo].[sp_AW_GetValidReprintJobLots]
	@itemCode VARCHAR(MAX),
	@days INT
AS
	 SELECT [vLotNumber] FROM [tbl_RTIS_AW_Jobs] WHERE [dtStarted] >= DATEADD(DAY, -(@days), GETDATE()) AND [vAWCode] = @itemCode
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetReprintJobNumber]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetReprintJobNumber]
GO

CREATE PROC [dbo].[sp_AW_GetReprintJobNumber]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX)
AS
	 SELECT [vJobUnq] froM [tbl_RTIS_AW_Jobs] WHERE [vAWCode] = @itemCode AND [vLotNumber] = @lot
GO



-------------------- sp_UI_UpdatePalletManufactured -------------
IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletManufactured]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletManufactured]
(
@iLIneID int,
@vUserManuf varchar(max)
)
AS
UPDATE [tbl_RTIS_Canning_Out] SET [bManuf] = 1, [dtDateManuf] = GETDATE(), [vUserManuf] = @vUserManuf
WHERE [iLIneID] = @iLIneID 
GO


------------------------sp_UI_UpdatePalletManufacturedManual----------------
IF (OBJECT_ID('[sp_UI_UpdatePalletManufacturedManual]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletManufacturedManual]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletManufacturedManual]
(
@iLIneID int,
@vUserManufManual varchar(max)
)
AS
UPDATE [tbl_RTIS_Canning_Out] SET [bManuf] = 1, [dtManufDateManual] = GETDATE(), [vUserManufManual] = @vUserManufManual
WHERE [iLIneID] = @iLIneID
GO


------------------------sp_UI_GetSOUnqBarcodes----------------
IF (OBJECT_ID('[sp_UI_GetSOUnqBarcodes]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSOUnqBarcodes]
GO

CREATE PROC [dbo].[sp_UI_GetSOUnqBarcodes]
AS
SELECT DISTINCT [Dispatch] FROM [tbl_unqBarcodes] WHERE [Dispatch] IS NOT NULL AND [Dispatch] <> ''
GO


------------------------sp_UI_GetZECT1FGMF----------------
IF (OBJECT_ID('[sp_UI_GetZECT1FGMF]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECT1FGMF]
GO

CREATE PROC [dbo].[sp_UI_GetZECT1FGMF]
AS
SELECT [iLineID],[vItemCode],[vItemDesc],[vLotNumber],[vCoatNum],[vSlurry],[dPalletQty],[dtDateEntered],[vUserEntered], ISNULL([bPrinted], 0)
FROM [tbl_RTIS_Zect] WHERE ([bManuf] = '0' OR [bManuf] IS NULL) AND [vZectLine] = '1'
GO


	------------------------sp_UI_GetZECT2FGMF----------------
IF (OBJECT_ID('[sp_UI_GetZECT2FGMF]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECT2FGMF]
GO

CREATE PROC [dbo].[sp_UI_GetZECT2FGMF]
AS
SELECT [iLineID],[vItemCode],[vItemDesc],[vLotNumber],[vCoatNum],[vSlurry],[dPalletQty],[dtDateEntered],[vUserEntered],[bPrinted]
FROM [tbl_RTIS_Zect] WHERE ([bManuf] = '0' OR [bManuf] IS NULL) AND [vZectLine] = '2'
GO
 


 	------------------------sp_UI_GetAWFGMF----------------
--IF (OBJECT_ID('[sp_UI_GetAWFGMF]') IS NOT NULL)
--	DROP PROC [dbo].[sp_UI_GetAWFGMF]
--GO

--CREATE PROC [dbo].[sp_UI_GetAWFGMF]
--AS
--SELECT [iLineID],[vItemCode],[vItemDesc],[vLotNumber], '' AS [vCoatNum], '' AS [vSlurry],[dNewPalletQty],[dtDateEntered],[vUserEntered],ISNULL( [bPrinted], 0)
--FROM [tbl_RTIS_AW] WHERE ([bManuf] = '0' OR [bManuf] IS NULL)
--GO
 


 ------------------------sp_UI_setZECTALLFGManufactured----------------
IF (OBJECT_ID('[sp_UI_setZECTALLFGManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setZECTALLFGManufactured]
GO

CREATE PROC [dbo].[sp_UI_setZECTALLFGManufactured]
(
@iLineID int,
@vUserManuf varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @vUserManuf WHERE [iLineID] = @iLineID
GO



 ------------------------sp_MBL_GetFreshSlurryInUse----------------
IF (OBJECT_ID('[sp_MBL_GetFreshSlurryInUse]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryInUse]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryInUse]
(
 @vTrolleyCode varchar(max)
)
AS
SELECT TOP 1 [vItemCode], [vLotNumber], [dWetWeight] 
FROM [tbl_RTIS_Fresh_Slurry] 
WHERE [vTrolleyCode] = @vTrolleyCode AND [dSolidity] IS NULL 
ORDER BY [iLineID] DESC
GO


 ------------------------sp_MBL_GetFreshSlurryWhes----------------
IF (OBJECT_ID('[sp_MBL_GetFreshSlurryWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryWhes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_FStMS] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


 ------------------------sp_MBL_GetSlurryLotNonManufactured----------------
IF (OBJECT_ID('[sp_MBL_GetSlurryLotNonManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryLotNonManufactured]
GO

CREATE PROC [dbo].[sp_MBL_GetSlurryLotNonManufactured]
(
 @vTrolleyCode varchar(max),
 @vItemCode varchar(max)
)
AS
SELECT [vLotNumber], s.[Description_1] FROM [tbl_RTIS_Fresh_Slurry] fs
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = fs.[vItemCode]
WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode
GO



 ------------------------sp_MBL_GetSlurryLotNonManufacturedSaveSol----------------
IF (OBJECT_ID('[sp_MBL_GetSlurryLotNonManufacturedSaveSol]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryLotNonManufacturedSaveSol]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryLotNonManufacturedSaveSol]
(
 @vTrolleyCode varchar(max),
 @vItemCode varchar(max)
)
AS
SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_Fresh_Slurry]
WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode ORDER BY [iLineID] DESC
GO


 ------------------------sp_MBL_GetSlurryWeightNonManufacturedSaveSol----------------
IF (OBJECT_ID('[sp_MBL_GetSlurryWeightNonManufacturedSaveSol]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryWeightNonManufacturedSaveSol]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryWeightNonManufacturedSaveSol]
(
 @vTrolleyCode varchar(max),
 @vItemCode varchar(max),
 @vLotNumber varchar(max)
)
AS
SELECT TOP 1 [dWetWeight] FROM [tbl_RTIS_Fresh_Slurry]
WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode AND [vLotNumber] = @vLotNumber  ORDER BY [iLineID] DESC
GO


 ------------------------sp_UI_GetWaitingFreshSlurries----------------
IF (OBJECT_ID('[sp_UI_GetWaitingFreshSlurries]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetWaitingFreshSlurries]
GO
CREATE PROC [dbo].[sp_UI_GetWaitingFreshSlurries]
AS
SELECT [iLineID], [vTrolleyCode], [vItemCode], [vLotNumber], [dWetWeight], [dSolidity], [dDryWeight], [vUserEntered], [dtDateEntered], '', '' , '' 
FROM [tbl_RTIS_Fresh_Slurry] WHERE ISNULL([dSolidity], 0) <> 0 AND ISNULL([bManuf], 0) = 0
GO



 ------------------------sp_UI_GetFreshSlurryMF----------------
IF (OBJECT_ID('[sp_UI_GetFreshSlurryMF]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetFreshSlurryMF]
GO
CREATE PROC [dbo].[sp_UI_GetFreshSlurryMF]
AS
SELECT [iLineID],[vTrolleyCode],'' AS [Tank],[vItemCode],[vItemDesc],[vLotNumber],[dWetWeight],[dSolidity],[dDryWeight],[dtDateSol],ISNULL([bManuf], 0)
FROM [tbl_RTIS_Fresh_Slurry] WHERE ([bManuf] = '0' OR [bManuf] IS NULL) AND [dSolidity] is not null
ORDER BY [dtDateSol] DESC
GO


 ------------------------sp_UI_GetFreshSlurryRecords----------------
IF (OBJECT_ID('[sp_UI_GetFreshSlurryRecords]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetFreshSlurryRecords]
GO
CREATE PROC [dbo].[sp_UI_GetFreshSlurryRecords]
(
@dateFrom datetime,
@dateTo datetime
)
AS
SELECT [vTrolleyCode],
[vItemCode],[vItemDesc],
[vLotNumber],[dWetWeight],
[dDryWeight],[dSolidity],[dtDateSol],
[vUserSol],[dtDateEntered],[vUserEntered],
ISNULL([bManuf], 0),
[dtManufDate],
[vUserManuf],
ISNULL([bTrans], 0),
[dtTrans],
ISNULL([bRecTrans], 0),
[dtRecTrans] ,
[vUserRec] 
FROM [tbl_RTIS_Fresh_Slurry]
WHERE [dtDateEntered] BETWEEN @dateFrom AND @dateTo
ORDER BY [dtDateEntered] DESC
GO


 ------------------------sp_UI_GetCanningCatalystRaws----------------
IF (OBJECT_ID('[sp_UI_GetCanningCatalystRaws]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningCatalystRaws]
GO
CREATE PROC [dbo].[sp_UI_GetCanningCatalystRaws]
(
@vItemCode varchar(max)
)
AS
SELECT [vRMCode], [vRMDesc], '' FROM [tbl_RTIS_Canning_Raws] WHERE [vItemCode] = @vItemCode
GO


 ------------------------sp_UI_GetCanningLinkExists----------------
IF (OBJECT_ID('[sp_UI_GetCanningLinkExists]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningLinkExists]
GO
CREATE PROC [dbo].[sp_UI_GetCanningLinkExists]
(
@vItemCode varchar(max),
@vRMCode varchar(max)
)
AS
SELECT [vRMCode] FROM [tbl_RTIS_Canning_Raws] WHERE [vItemCode] = @vItemCode AND [vRMCode] = @vRMCode
GO



 ------------------------sp_UI_GetCanningRecords----------------
IF (OBJECT_ID('[sp_UI_GetCanningRecords]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningRecords]
GO
CREATE PROC [dbo].[sp_UI_GetCanningRecords]
(
@dateFrom datetime,
@dateTo datetime
)
AS
SELECT 
[vCanningCode]
,[vCanningDesc]
,[vQty]
,[vRMCode]
,[vRMDesc]
,[vRMQty]
,[vLotNumber]
,[vOldJobCode]
,[vPalletNo]      
,[vUserAdded]
,[dtDateAdded]
FROM [tbl_RTIS_Canning_Out] 
WHERE [dtDateAdded] BETWEEN @dateFrom AND @dateTo
ORDER BY [dtDateAdded] DESC
GO





 ------------------------sp_UI_GetCanningLinesToManufacture----------------
IF (OBJECT_ID('[sp_UI_GetCanningLinesToManufacture]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningLinesToManufacture]
GO
CREATE PROC [dbo].[sp_UI_GetCanningLinesToManufacture]
(
@vItemCode varchar(max),
@vRMCode varchar(max)
)
AS
SELECT [iLIneID],[vCanningCode], [vCanningDesc], [vLotNumber],[vQty],[vUserAdded], [dtDateAdded]
FROM [tbl_RTIS_Canning_Out] WHERE ISNULL([bManuf], 0) = 0
GO




 ------------------------sp_UI_GetCanningRM----------------
IF (OBJECT_ID('[sp_UI_GetCanningRM]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningRM]
GO
CREATE PROC [dbo].[sp_UI_GetCanningRM]
(
@iLIneID int
)
AS
SELECT [vRMCode], [vLotNumber] FROM [tbl_RTIS_Canning_Out] WHERE [iLIneID] = @iLIneID
GO


 ------------------------sp_UI_GetCanningProducts----------------
IF (OBJECT_ID('[sp_UI_GetCanningProducts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCanningProducts]
GO
CREATE PROC [dbo].[sp_UI_GetCanningProducts]
(
@vRMCode varchar(max)
)
AS
SELECT [vItemCode] , [vItemDesc], '' FROM [tbl_RTIS_Canning_Raws] WHERE [vRMCode] = @vRMCode
GO



 ------------------------sp_Canning_GetReprintItemList----------------
IF (OBJECT_ID('[sp_Canning_GetReprintItemList]') IS NOT NULL)
	DROP PROC [dbo].[sp_Canning_GetReprintItemList]
GO
CREATE PROC [dbo].[sp_Canning_GetReprintItemList]
AS
SELECT DISTINCT [vCanningCode] FROM [tbl_RTIS_Canning_Out]
GO



 ------------------------sp_Canning_GetReprintLotList----------------
IF (OBJECT_ID('sp_Canning_GetReprintLotList') IS NOT NULL)
	DROP PROC [dbo].[sp_Canning_GetReprintLotList]
GO
CREATE PROC [dbo].[sp_Canning_GetReprintLotList]
(
@vCanningCode varchar(max),
@dateFrom datetime,
@dateTo datetime
)
AS
SELECT DISTINCT [vLotNumber] FROM [tbl_RTIS_Canning_Out] WHERE [vCanningCode] = @vCanningCode AND [dtDateAdded] BETWEEN @dateFrom AND @dateTo
GO


 ------------------------sp_Canning_GetPalletList----------------
IF (OBJECT_ID('sp_Canning_GetPalletList') IS NOT NULL)
	DROP PROC [dbo].[sp_Canning_GetPalletList]
GO
CREATE PROC [dbo].[sp_Canning_GetPalletList]
(
@vCanningCode varchar(max),
@vLotNumber varchar(max)
)
AS
SELECT [vCanningCode], [vLotNumber], [vQty], [vRMCode], [vPalletNo], [vRMDesc] 
FROM [tbl_RTIS_Canning_Out] 
WHERE [vCanningCode] = @vCanningCode AND [vLotNumber] = @vLotNumber
GO

 ------------------------sp_UI_InsertCanningRecordk----------------
IF (OBJECT_ID('sp_UI_InsertCanningRecordk') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertCanningRecordk]
GO
CREATE PROC [dbo].[sp_UI_InsertCanningRecordk]
(
@vOldJobCode varchar(max), 
@vPalletCode varchar(max),
@vRMCode varchar(max), 
@vRMDesc varchar(max),
@vRMQty varchar(max),
@vLotNumber varchar(max),
@vCanningCode varchar(max),
@vCanningDesc varchar(max), 
@vQty varchar(max), 
@vUserAdded varchar(max), 
@dtDateAdded datetime, 
@vPalletNo varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Canning_Out] 
([vOldJobCode], 
[vPalletCode], 
[vRMCode], 
[vRMDesc], 
[vRMQty], 
[vLotNumber], 
[vCanningCode], 
[vCanningDesc],
[vQty],
[vUserAdded], 
[dtDateAdded], 
[vPalletNo])
VALUES (@vOldJobCode, @vPalletCode, @vRMCode, @vRMDesc, @vRMQty, @vLotNumber, @vCanningCode, @vCanningDesc, @vQty , @vUserAdded, GETDATE(), @vPalletNo)
GO




 ------------------------sp_UI_DeleteRMLink----------------
IF (OBJECT_ID('sp_UI_DeleteRMLink') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteRMLink]
GO
CREATE PROC [dbo].[sp_UI_DeleteRMLink]
(
@vitemCode varchar(max),
@vRMCode varchar(max)
)
AS
DELETE FROM [tbl_RTIS_Canning_Raws] WHERE [vitemCode] = @vitemCode AND [vRMCode] = @vRMCode
GO




 ------------------------sp_UI_GetFSLinkExists----------------
IF (OBJECT_ID('sp_UI_GetFSLinkExists') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetFSLinkExists]
GO
CREATE PROC [dbo].[sp_UI_GetFSLinkExists]
(
@vSlurryCode varchar(max),
@vRMCode varchar(max)
)
AS
SELECT [vRMCode] FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @vSlurryCode AND [vRMCode] = @vRMCode
GO


 ------------------------sp_MBL_GetSlurryReqRM----------------
IF (OBJECT_ID('sp_MBL_GetSlurryReqRM') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryReqRM]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryReqRM]
(
@vSlurryCode varchar(max)
)
AS
SELECT TOP 1 [vRMCode] FROM  [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @vSlurryCode
GO


 ------------------------sp_MBL_CheckLotNumberUsed----------------
IF (OBJECT_ID('sp_MBL_CheckLotNumberUsed') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckLotNumberUsed]
GO
CREATE PROC [dbo].[sp_MBL_CheckLotNumberUsed]
(
@vLotNumber varchar(max)
)
AS
SELECT [vLotNumber] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vLotNumber] = @vLotNumber
GO

 ------------------------sp_MBL_GetSlurryRMLink----------------
IF (OBJECT_ID('sp_MBL_GetSlurryRMLink') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryRMLink]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryRMLink]
(
@vSlurryCode varchar(max),
@vRMCode varchar(max)
)
AS
SELECT [iLineID] FROM  [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @vSlurryCode AND [vRMCode] = @vRMCode
GO



 ------------------------sp_MBL_GetSlurryHeaderID----------------
IF (OBJECT_ID('sp_MBL_GetSlurryHeaderID') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryHeaderID]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryHeaderID]
(
@vTrolleyCode varchar(max),
@vItemCode varchar(max),
@vLotNumber varchar(max)
)
AS
SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode AND [vLotNumber] = @vLotNumber
ORDER BY [iLineID] DESC
GO



 ------------------------sp_UI_GetFSRawMaterials----------------
IF (OBJECT_ID('sp_MBL_GetSlurryHeaderID') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryHeaderID]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryHeaderID]
(
@iSlurryID int
)
AS
SELECT [vPowderCode], [vPowderLot] FROM [tbl_RTIS_Fresh_Slurry_Input] WHERE [iSlurryID] = @iSlurryID
ORDER BY [iLineID] DESC
GO


 ------------------------sp_MBL_InsertFreshSlurry----------------
IF (OBJECT_ID('sp_MBL_InsertFreshSlurry') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertFreshSlurry]
GO
CREATE PROC [dbo].[sp_MBL_InsertFreshSlurry]
(
@vTrolleyCode varchar(max), 
@vItemCode varchar(max),
@vLotNumber varchar(max),
@dWetWeight varchar(max), 
@vUserEntered varchar(max),
@vItemDesc varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Fresh_Slurry] ([vTrolleyCode], [vItemCode], [vLotNumber], [dWetWeight], [vUserEntered], [dtDateEntered], [vItemDesc])
VALUES
(
@vTrolleyCode,
@vItemCode, 
@vLotNumber,
@dWetWeight, 
@vUserEntered, 
GETDATE(),
@vItemDesc
)
GO



 ------------------------sp_UI_InsertRMLink----------------
IF (OBJECT_ID('sp_UI_InsertRMLink') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertRMLink]
GO
CREATE PROC [dbo].[sp_UI_InsertRMLink]
(
@vSlurryCode varchar(max), 
@vRMCode varchar(max), 
@vRMDesc varchar(max), 
@vUserAdded varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Fresh_Slurry_Raws] 
([vSlurryCode], [vRMCode], [vRMDesc], [vUserAdded], [dtDateAdded])
VALUES (@vSlurryCode, @vRMCode , @vRMDesc, @vUserAdded,GETDATE())
GO



 ------------------------sp_MBL_InsertFreshSlurryRM----------------
IF (OBJECT_ID('sp_MBL_InsertFreshSlurryRM') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertFreshSlurryRM]
GO
CREATE PROC [dbo].[sp_MBL_InsertFreshSlurryRM]
(
@iSlurryID int, 
@vPowderCode varchar(max),
@vPowderLot varchar(max), 
@dQty decimal(16,3), 
@vUserRecorded varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Fresh_Slurry_Input] ([iSlurryID], [vPowderCode], [vPowderLot], [dQty], [dtDateRecorded], [vUserRecorded])
VALUES (@iSlurryID, @vPowderCode, @vPowderLot, @dQty, GETDATE(), @vUserRecorded)
GO


 ------------------------sp_MBL_InvalidateSlurry----------------
IF (OBJECT_ID('sp_MBL_InvalidateSlurry') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InvalidateSlurry]
GO
CREATE PROC [dbo].[sp_MBL_InvalidateSlurry]
(
@vTrolleyCode varchar(max), 
@vItemCode varchar(max), 
@vLotNumber varchar(max), 
@vUserSol varchar(max)
)
AS
 UPDATE [tbl_RTIS_Fresh_Slurry] SET [dSolidity] = 0, [dDryWeight] = 0, [dtDateSol] = GETDATE()
, [vUserSol] = @vUserSol, [bManuf] = 1, [dtManufDate] = GETDATE(), [bTrans] = 1, [dtTrans] = GETDATE(), [vUserManuf] = @vUserSol, 
[bRecTrans] = 1, dtRecTrans = GETDATE(), [vUserRec] = @vUserSol
WHERE [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode AND [vLotNumber] = @vLotNumber AND [dSolidity] IS NULL
GO




 ------------------------sp_MBL_setSlurrySolidity----------------
IF (OBJECT_ID('sp_MBL_setSlurrySolidity') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_setSlurrySolidity]
GO
CREATE PROC [dbo].[sp_MBL_setSlurrySolidity]
(
@vTrolleyCode varchar(max), 
@vItemCode varchar(max), 
@vLotNumber varchar(max), 
@dSolidity decimal(16,3), 
@vUserSol varchar(max),
@dDryWeight decimal(16,3)

)
AS
UPDATE [tbl_RTIS_Fresh_Slurry] SET [dSolidity] = @dSolidity, [dDryWeight] = @dDryWeight, [vUserSol] = @vUserSol, [dtDateSol] =GETDATE()
WHERE [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode AND [vLotNumber] = @vLotNumber
AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @vTrolleyCode AND [vItemCode] = @vItemCode AND [vLotNumber] = @vLotNumber ORDER BY [iLineID] DESC)
GO



 ------------------------sp_UI_setFSManufactured----------------
IF (OBJECT_ID('sp_UI_setFSManufactured') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setFSManufactured]
GO
CREATE PROC [dbo].[sp_UI_setFSManufactured]
(
@iLineID int,
@vUserManuf varchar(max)
)
AS
UPDATE [tbl_RTIS_Fresh_Slurry] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @vUserManuf WHERE [iLineID] = @iLineID
GO





 ------------------------sp_UI_setFSManufacturedManual----------------
IF (OBJECT_ID('sp_UI_setFSManufacturedManual') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setFSManufacturedManual]
GO
CREATE PROC [dbo].[sp_UI_setFSManufacturedManual]
(
@iLineID int,
@vUserManuf varchar(max)
)
AS
UPDATE [tbl_RTIS_Fresh_Slurry] 
SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @vUserManuf, [bManualManuf] =1 WHERE [iLineID] = @iLineID
GO


 ------------------------sp_UI_DeleteRMLink----------------
IF (OBJECT_ID('sp_UI_DeleteRMLink') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteRMLink]
GO
CREATE PROC [dbo].[sp_UI_DeleteRMLink]
(
@vSlurryCode varchar(max),
@vRMCode varchar(max)
)
AS
DELETE FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @vSlurryCode AND [vRMCode] = @vRMCode
GO


 ------------------------sp_UI_GetFreshSlurryRaws----------------
IF (OBJECT_ID('[sp_UI_GetFreshSlurryRaws]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetFreshSlurryRaws]
GO
CREATE PROC [dbo].[sp_UI_GetFreshSlurryRaws]
(
@vSlurryCode varchar(max)
)
AS
SELECT [vRMCode], [vRMDesc], '' 
FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @vSlurryCode
GO


IF (OBJECT_ID('[dbo].[sp_AW_GetReprintJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetReprintJobInfo]
GO

CREATE PROC [dbo].[sp_AW_GetReprintJobInfo]
	@jobNo VARCHAR(MAX)
AS
	 SELECT [vAWCode], [vLotNumber], [dQty], [dQtyManuf], [vPGMCode], [vPGMLot]  
	 FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetJobPallets]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetJobPallets]
GO

CREATE PROC [dbo].[sp_AW_GetJobPallets]
	@jobNo VARCHAR(MAX)
AS
	SELECT ao.[vPalletNo], ao.[vPalletCode], aj.[vAWCode], aj.[vLotNumber], ao.[dQty] FROM [tbl_RTIS_AW_OutPut] ao
	INNER JOIN [tbl_RTIS_AW_Jobs] aj ON aj.[iLIneID] = ao.[iJobID] WHERE aj.[vJobUnq] = @jobNo
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetAWUnq]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetAWUnq]
GO

CREATE PROC [dbo].[sp_AW_GetAWUnq]
	@itemCode VARCHAR(MAX),
	@jobNo VARCHAR(MAX)
AS
	SELEcT [vUnqBarcode] FROM [tbl_unqBarcodes]
    WHERE [vUnqBarcode] LIKE @itemCode AND [vUnqBarcode] LIKE @jobNo
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetJobInfo_CJ]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetJobInfo_CJ]
GO

CREATE PROC [dbo].[sp_AW_GetJobInfo_CJ]
	@jobNo VARCHAR(MAX)
AS
	SELECT [vAWCode], [vLotNumber], [vPGMCode], [vPGMLot], [dQty], [dQtyManuf]
    FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO





IF (OBJECT_ID('[dbo].[sp_AW_CheckJobRunning_RO]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_CheckJobRunning_RO]
GO

CREATE PROC [dbo].[sp_AW_CheckJobRunning_RO]
AS
	SELECT [vJobUnq] FROM [tbl_RTIS_AW_Jobs] WHERE [bJobRunning] = 1
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetJobInfo_RO]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetJobInfo_RO]
GO

CREATE PROC [dbo].[sp_AW_GetJobInfo_RO]
	@jobNo VARCHAR(MAX)
AS
	SELECT [vAWCode], [vLotNumber], [vPGMCode], [vPGMLot], [dQty], [dQtyManuf]
    FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetValidReopenJobLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetValidReopenJobLots]
GO

CREATE PROC [dbo].[sp_AW_GetValidReopenJobLots]
	@itemCode VARCHAR(MAX),
	@days INT
AS
	SELECT [vLotNumber] 
	FROM [tbl_RTIS_AW_Jobs] 
	WHERE [dtStarted] >= DATEADD(DAY, -(@days), GETDATE()) AND [vAWCode] = @itemCode AND ISNULL([bJobRunning], 0) = 0
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetReprintJobNumber_RO]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetReprintJobNumber_RO]
GO

CREATE PROC [dbo].[sp_AW_GetReprintJobNumber_RO]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX)
AS
	SELECT [vJobUnq] froM [tbl_RTIS_AW_Jobs] WHERE [vAWCode] = @itemCode AND [vLotNumber] = @lot
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetJobInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetJobInfo]
	@jobNo VARCHAR(MAX)
AS
	SELECT [vAWCode], [vLotNumber]
    FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetJobRunning]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetJobRunning]
GO

CREATE PROC [dbo].[sp_MBL_GetJobRunning]
	@jobNo VARCHAR(MAX)
AS
	SELECT [bJobRunning] FROM [tbl_RTIS_AW_Jobs] WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetJobID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetJobID]
GO

CREATE PROC [dbo].[sp_MBL_GetJobID]
	@jobNo VARCHAR(MAX)
AS
	SELECT [iLIneID] FROM [tbl_RTIS_AW_Jobs] WHERE [bJobRunning] = 1 AND [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetUnqOnJob]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetUnqOnJob]
GO

CREATE PROC [dbo].[sp_MBL_GetUnqOnJob]
	@unq VARCHAR(MAX)
AS
	SELECT [vUnq] FROM [tbl_RTIS_AW_Input] WHERE [vUnq] = @unq
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetAllAWJobs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllAWJobs]
GO

CREATE PROC [dbo].[sp_UI_GetAllAWJobs]
	@dateFrom VARCHAR(MAX),
	@dateTo VARCHAR(MAX)
AS
	SELECT [iLIneID], [vJobUnq], [vAWCode], [vLotNumber], [vPGMCode], [vPGMLot], [dQty], [dQtyManuf], [dtStarted] ,[vUserStarted] ,[dtStopped] ,[vUserStopped] ,[dtSReopened] ,[vUserReopened], [bJobRunning]
    FROM [tbl_RTIS_AW_Jobs] WHERE [dtStarted] BETWEEN @dateFrom AND @dateTo ORDER BY [dtStarted] DESC
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAWJobInPuts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWJobInPuts]
GO

CREATE PROC [dbo].[sp_UI_GetAWJobInPuts]
	@headerID VARCHAR(MAX)
AS
	SELECT [vCatalystCode], [vCatalystLot], [dQty], [dtDateRecorded], [vUserRecorded]
    FROM [tbl_RTIS_AW_Input]
    WHERE [iJobID] = @headerID
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetAWJobOutputs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWJobOutputs]
GO

CREATE PROC [dbo].[sp_UI_GetAWJobOutputs]
	@headerID VARCHAR(MAX)
AS
	SELECT [vPalletNo], [dQty], [vUserRecorded], [dtDateRecorded],ISNULL([bManuf], 0),[dtDateManuf],[vUserManuf]
    FROM [tbl_RTIS_AW_OutPut] WHERE [iJobID] = @headerID
GO





IF (OBJECT_ID('[dbo].[sp_UI_InsertRMLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertRMLink]
GO

CREATE PROC [dbo].[sp_UI_InsertRMLink]
	@awCode VARCHAR(MAX),
	@awDesc VARCHAR(MAX),
	@rmCode VARCHAR(MAX),
	@rmDesc VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
	INSERT INTO [tbl_RTIS_AW_Raws] ([vAWCode], [vAWDesc], [vRMCode], [vRMDesc], [vUserAdded], [dtDateAdded])
    VALUES (@awCode, @awDesc, @rmCode, @rmDesc, @username, GETDATE())
GO





IF (OBJECT_ID('[dbo].[sp_UI_InsertNewAWJob]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertNewAWJob]
GO

CREATE PROC [dbo].[sp_UI_InsertNewAWJob]
	@jobNumber VARCHAR(MAX),
	@code VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@PGM VARCHAR(MAX),
	@PGMLot VARCHAR(MAX),
	@qty DECIMAL,
	@username VARCHAR(MAX)
AS
	INSERT INTO [tbl_RTIS_AW_Jobs] ([vJobUnq], [vAWCode], [vLotNumber], [vPGMCode], [vPGMLot], [dQty], [vUserStarted], [dtStarted], [bJobRunning], [dQtyManuf])
    VALUES (@jobNumber, @code, @lotNumber, @PGM, @PGMLot, @qty, @username, GETDATE(), 1, 0)
GO





IF (OBJECT_ID('[dbo].[sp_AW_AddNewPallet]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_AddNewPallet]
GO

CREATE PROC [dbo].[sp_AW_AddNewPallet]
	@jobID VARCHAR(MAX),
	@palletCode VARCHAR(MAX),
	@palletNo VARCHAR(MAX),
	@qty DECIMAL,
	@username VARCHAR(MAX)
AS
	INSERT INTO [tbl_RTIS_AW_OutPut] ([iJobID], [vPalletCode], [vPalletNo], [dQty], [vUserRecorded], [dtDateRecorded])
	VALUES (@jobID, @palletCode, @palletNo, @qty, @username, GETDATE())
GO




IF (OBJECT_ID('[dbo].[sp_UI_InsertNewAWJobRawMaterial]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertNewAWJobRawMaterial]
GO

CREATE PROC [dbo].[sp_UI_InsertNewAWJobRawMaterial]
	@jobID VARCHAR(MAX),
	@code VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@qty DECIMAL,
	@username VARCHAR(MAX),
	@palletNo VARCHAR(MAX),
	@ZectJob VARCHAR(MAX),
	@palletUnq VARCHAR(MAX)
AS
	INSERT INTO [tbl_RTIS_AW_Input] ([iJobID], [vCatalystCode], [vCatalystLot], [dQty], [dtDateRecorded], [vUserRecorded], [vPalletNo], [vZectJobNo], [vUnq])
    VALUES (@jobID, @code, @lotNumber, @qty, GETDATE(), @username, @palletNo, @ZectJob, @palletUnq)
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteRMLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteRMLink]
GO

CREATE PROC [dbo].[sp_UI_DeleteRMLink]
	@awCode VARCHAR(MAX),
	@rmCode VARCHAR(MAX)
AS
	DELETE FROM [tbl_RTIS_AW_Raws] WHERE [vAWCode] = @awCode AND [vRMCode] = @rmCode
GO




IF (OBJECT_ID('[dbo].[sp_AW_UpdateManufacturedQty]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_UpdateManufacturedQty]
GO

CREATE PROC [dbo].[sp_AW_UpdateManufacturedQty]
	@jobNo VARCHAR(MAX),
	@qty DECIMAL
AS
	UPDATE [tbl_RTIS_AW_Jobs] SET [dQtyManuf] = ISNULL([dQtyManuf], 0) + @qty
    WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_AW_UpdateJobClosed]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_UpdateJobClosed]
GO

CREATE PROC [dbo].[sp_AW_UpdateJobClosed]
	@jobNo VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
	UPDATE [tbl_RTIS_AW_Jobs] SET [bJobRunning] =0, [vUserStopped] = @username, [dtStopped] = GETDATE()
    WHERE [vJobUnq] = @jobNo
GO




IF (OBJECT_ID('[dbo].[sp_AW_UpdateJobReOpened]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_UpdateJobReOpened]
GO

CREATE PROC [dbo].[sp_AW_UpdateJobReOpened]
	@jobNo VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
	UPDATE [tbl_RTIS_AW_Jobs] SET [bJobRunning] =1, [vUserReopened] = @username, [dtSReopened] = GETDATE()
    WHERE [vJobUnq] = @jobNo

GO




IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletManufactured]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletManufactured]
	@lineID VARCHAR(MAX),
	@jobID VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	UPDATE [tbl_RTIS_AW_OutPut] SET [bManuf] = 1, [dtDateManuf] = GETDATE(), [vUserManuf] = @userName
    WHERE [iLIneID] = @lineID AND [iJobID] = @jobID
GO




IF (OBJECT_ID('[dbo].[sp_UI_setAWBatchManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setAWBatchManufactured]
GO

CREATE PROC [dbo].[sp_UI_setAWBatchManufactured]
	@headerID VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	UPDATE [tbl_RTIS_AW_OutPut] SET [bManuf] = '1', [dtDateManuf] = GETDATE(), [vUserManuf] = @userName 
	WHERE [iJobID] = @headerID AND ISNULL( [bManuf] , 0) = 0
GO




IF (OBJECT_ID('[dbo].[sp_UI_setAWBatchManufacturedManual]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setAWBatchManufacturedManual]
GO

CREATE PROC [dbo].[sp_UI_setAWBatchManufacturedManual]
	@headerID VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	UPDATE [tbl_RTIS_AW_OutPut] SET [bManuf] = '1', [dtManufDateManual] = GETDATE(), [vUserManufManual] = @headerID 
	WHERE [iJobID] = @headerID AND ISNULL( [bManuf] , 0) = 0
GO


--------------------------------------------------------- Palletizing ---------------------------------------------------------------------------


IF (OBJECT_ID('[dbo].[sp_UI_GetPalletPrintSettings]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletPrintSettings]
GO

CREATE PROC [dbo].[sp_UI_GetPalletPrintSettings]
AS
	IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Printer')) 
    BEGIN
	    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) 
	    VALUES ('Pallet Printer', 'Please Select A Printer')
    END
    IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Label')) 
    BEGIN
	    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) 
	    VALUES ('Pallet Label', 'Please Select A Label')
    END
    SELECT [Setting_Name], [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Printer' OR [Setting_Name] = 'Pallet Label'
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletPrintSettings]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletPrintSettings]
GO

CREATE PROC [dbo].[sp_MBL_GetPalletPrintSettings]
AS
	IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Printer')) 
    BEGIN
	    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) 
	    VALUES ('Pallet Printer', 'Please Select A Printer')
    END
    IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Label')) 
    BEGIN
	    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) 
	    VALUES ('Pallet Label', 'Please Select A Label')
    END
    SELECT [Setting_Name], [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'Pallet Printer' OR [Setting_Name] = 'Pallet Label'
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetPallets]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPallets]
GO

CREATE PROC [dbo].[sp_UI_GetPallets]
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM 
    [htbl_PalletBarcodes] WHERE [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByDate]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByDate]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByDate]
	@from VARCHAR(MAX),
	@_to VARCHAR(MAX)
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM 
    [htbl_PalletBarcodes] WHERE [Printed] BETWEEN @from AND @_to AND [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByItem]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByItem]
	@itemCode VARCHAR(MAX)
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] 
	FROM [htbl_PalletBarcodes] WHERE [vUnqBarcode] LIKE '%' + @ItemCode + '%' AND [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByItemAndDate]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByItemAndDate]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByItemAndDate]
	@itemCode VARCHAR(MAX),
	@from VARCHAR(MAX),
	@_to VARCHAR(MAX)
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM 
    [htbl_PalletBarcodes] WHERE [vUnqBarcode] LIKE '%' + @itemCode + '%' 
    AND [Printed] BETWEEN @from AND @_to AND [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByLot]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByLot]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByLot]
	@lot VARCHAR(MAX)
AS
	SELECT [iPallet_ID] FROM [ltbl_PalletBarcodes] l 
    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
    WHERE l.[vUnqBarcode] LIKE '%' + @lot + '%' AND h.[bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByLotAndDate]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByLotAndDate]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByLotAndDate]
	@lot VARCHAR(MAX),
	@from VARCHAR(MAX),
	@_to VARCHAR(MAX)
AS
	SELECT [iPallet_ID] FROM [ltbl_PalletBarcodes] l 
    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
    WHERE l.[vUnqBarcode] LIKE '%' + @lot + '%' AND h.[Printed] BETWEEN @from AND @_to AND h.[bRMPallet] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPalletLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletLots]
GO

CREATE PROC [dbo].[sp_UI_GetPalletLots]
	@palletId VARCHAR(MAX)
AS
	SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @palletId
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetPalletLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletLines]
GO

CREATE PROC [dbo].[sp_UI_GetPalletLines]
	@palletId VARCHAR(MAX)
AS
	SELECT l.[vUnqBarcode], l.[bOnPallet] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @palletId
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletPrinSettings_1]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletPrinSettings_1]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletPrinSettings_1]
	@printer VARCHAR(MAX)
AS
	UPDATE [tbl_RTSettings] SET [SettingValue] = @printer WHERE [Setting_Name] = 'Pallet Printer'
GO



IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletPrinSettings_2]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletPrinSettings_2]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletPrinSettings_2]
	@label VARCHAR(MAX)
AS
	UPDATE [tbl_RTSettings] SET [SettingValue] = @label WHERE [Setting_Name] = 'Pallet Label'
GO



--------------------------------------------------------- Mixed Slurry ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[sp_MBL_CheckMixedSlurryBufferTankInUse]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckMixedSlurryBufferTankInUse]
GO

CREATE PROC [dbo].[sp_MBL_CheckMixedSlurryBufferTankInUse]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bBufferClosed], 0) = 0
    ORDER BY [iLineID] DESC
GO





IF (OBJECT_ID('[dbo].[sp_MBL_CheckMixedSlurryTankInUse]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckMixedSlurryTankInUse]
GO

CREATE PROC [dbo].[sp_MBL_CheckMixedSlurryTankInUse]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bReceived], 0) = 0
	ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckMixedSlurryRemENtered]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckMixedSlurryRemENtered]
GO

CREATE PROC [dbo].[sp_MBL_CheckMixedSlurryRemENtered]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vUserRemaining] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckMixedSlurryRecEntered]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckMixedSlurryRecEntered]
GO

CREATE PROC [dbo].[sp_MBL_CheckMixedSlurryRecEntered]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vUserRecovered] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryInBufferTank]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryInBufferTank]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryInBufferTank]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bBufferClosed], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetTankFreshSlurries]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetTankFreshSlurries]
GO

CREATE PROC [dbo].[sp_MBL_GetTankFreshSlurries]
	@headerID VARCHAR(MAX)
AS
	SELECT
    s.[vTrolleyCode]
    ,s.[vItemCode]
    ,s.[vItemDesc]
    ,s.[vLotNumber]
    ,s.[dWeight]
    FROM [tbl_RTIS_MS_Slurries] s
    WHERE s.[iHeaderID] = @headerID
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryInTank]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryInTank]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryInTank]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bReceived], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMixedSlurryHeaderID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMixedSlurryHeaderID]
GO

CREATE PROC [dbo].[sp_MBL_GetMixedSlurryHeaderID]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bReceived], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetTrolleyInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetTrolleyInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetTrolleyInfo]
	@trolleyNo VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], s.[Description_1]
    FROM [tbl_RTIS_Fresh_Slurry] ms
    INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = ms.[vItemCode]
    WHERE [vTrolleyCode] = @trolleyNo AND [vItemCode] = @itemCode AND [dSolidity] IS NOT NULL AND ISNULL([bRecTrans], 0) = 1 
	ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryAlreadyInTank]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryAlreadyInTank]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryAlreadyInTank]
	@headerId VARCHAR(MAX),
	@trolleyCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [tbl_RTIS_MS_Slurries] WHERE [iHeaderID] = @headerId 
	AND [vTrolleyCode] = @trolleyCode 
	AND [vItemCode] = @itemCode 
	AND [vLotNumber] = @lotNumber
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetBufferTankSlurryID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetBufferTankSlurryID]
GO

CREATE PROC [dbo].[sp_MBL_GetBufferTankSlurryID]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bBufferClosed], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetTankSlurryID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetTankSlurryID]
GO

CREATE PROC [dbo].[sp_MBL_GetTankSlurryID]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bReceived], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetTrolleyTolerance]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetTrolleyTolerance]
GO

CREATE PROC [dbo].[sp_MBL_GetTrolleyTolerance]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings]
    WHERE [Setting_Name] = 'FSTol'
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetFreshSlurryTrolleyInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryTrolleyInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryTrolleyInfo]
	@trolleyCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLIneID], [dWetWeight], [dTotalDecantedWeight] FROM [tbl_RTIS_Fresh_Slurry]
    WHERE [vTrolleyCode] = @trolleyCode AND [vItemCode] = @itemCode ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryAvailableToDecant]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryAvailableToDecant]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryAvailableToDecant]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bBufferClosed], 0) = 0 AND [dRemainingWeight] IS NOT NULL AND [dRecoveredWeight] IS NOT NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckMobileTankAvailable]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckMobileTankAvailable]
GO

CREATE PROC [dbo].[sp_MBL_CheckMobileTankAvailable]
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_MS_Decant]
    WHERE [vTankCode] = @tankCode AND [vItemCode]= @itemCode AND ISNULL([bReceived], 0) = 0
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMixedSlurryHeaderInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMixedSlurryHeaderInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetMixedSlurryHeaderInfo]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID], [vDescription], [vLotNumber] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bBufferClosed], 0) = 0 AND [dRemainingWeight] IS NOT NULL AND [dRecoveredWeight] IS NOT NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryInMobileTankZAC]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryInMobileTankZAC]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryInMobileTankZAC]
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [vItemDesc] FROM [tbl_RTIS_MS_Decant]
    WHERE [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_CheckSlurryTankZAC]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckSlurryTankZAC]
GO

CREATE PROC [dbo].[sp_MBL_CheckSlurryTankZAC]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetZacChems]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZacChems]
GO

CREATE PROC [dbo].[sp_MBL_GetZacChems]
AS
	SELECT [vChemicalName] FROM [tbl_RTIS_MS_Chemical_List]
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMobileTankSlurryID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMobileTankSlurryID]
GO

CREATE PROC [dbo].[sp_MBL_GetMobileTankSlurryID]
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Decant]
    WHERE [vTankCode] = @tankCode AND [vItemCode] = @itemCode
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMobileTankChemicalID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMobileTankChemicalID]
GO

CREATE PROC [dbo].[sp_MBL_GetMobileTankChemicalID]
	@tankID VARCHAR(MAX),
	@chemical VARCHAR(MAX)
AS
	SElECT [iLineID] FROM [tbl_RTIS_MS_Chemicals] WHERE [iMTNKID] = @tankID AND [vChemicalName] = @chemical
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetTankChemicalID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetTankChemicalID]
GO

CREATE PROC [dbo].[sp_MBL_GetTankChemicalID]
	@tankID VARCHAR(MAX),
	@chemical VARCHAR(MAX)
AS
	SElECT [iLineID] FROM [tbl_RTIS_MS_Chemicals] WHERE [iTNKID] = @tankID AND [vChemicalName] = @chemical
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetAllChemicals]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetAllChemicals]
GO

CREATE PROC [dbo].[sp_MBL_GetAllChemicals]
AS
	SELECT [vChemicalName]
    FROM [tbl_RTIS_MS_Chemical_List]
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMobileTankSlurryID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMobileTankSlurryID]
GO

CREATE PROC [dbo].[sp_MBL_GetMobileTankSlurryID]
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [vItemDesc] FROM [tbl_RTIS_MS_Decant]
    WHERE [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetSlurryTankInfoSolidity]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryTankInfoSolidity]
GO

CREATE PROC [dbo].[sp_MBL_GetSlurryTankInfoSolidity]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetMobileTankSolidityInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMobileTankSolidityInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetMobileTankSolidityInfo]
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID], [vLotNumber] FROM [tbl_RTIS_MS_Decant]
    WHERE [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetSlurryTankSolidityInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryTankSolidityInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetSlurryTankSolidityInfo]
	@tankType VARCHAR(MAX),
	@tankCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1  [iLineID], [vLotNumber] FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankType AND [vTankCode] = @tankCode AND [vItemCode] = @itemCode AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetFreshSlurryInfoRecTrans]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryInfoRecTrans]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryInfoRecTrans]
	@trolleyCode VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 'False'), s.[Description_1] FROM [tbl_RTIS_Fresh_Slurry] ms
    INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = [vItemCode]
    WHERE [vTrolleyCode] = @trolleyCode AND [vItemCode] = @itemCode ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[MBL_GetMobileTankTransferInfo]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetMobileTankTransferInfo]
GO

CREATE PROC [dbo].[MBL_GetMobileTankTransferInfo]
	@tankNo VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bTransferred], 0), [vItemDesc]
    FROM [tbl_RTIS_MS_Decant] WHERE [vTankCode] = @tankNo AND [vItemCode] = @itemCode AND [dSolidity] IS NOT NULL
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[MBL_GetMobileTankTransferd]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetMobileTankTransferd]
GO

CREATE PROC [dbo].[MBL_GetMobileTankTransferd]
	@tankNo VARCHAR(MAX),
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX)
AS
	SELECT TOP 1 ISNULL([bTransferred], 0) FROM [tbl_RTIS_MS_Decant]
	WHERE [vTankCode] = @tankNo AND [vItemCode] = @itemCode AND [vLotNumber] = @lotNumber
	ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[MBL_GetLargeTankTransferd]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetLargeTankTransferd]
GO

CREATE PROC [dbo].[MBL_GetLargeTankTransferd]
	@tankTye VARCHAR(MAX),
	@tankNo VARCHAR(MAX),
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX)
AS
	SELECT TOP 1 ISNULL([bTransferred], 0) FROM [tbl_RTIS_MS_Main]
    WHERE [vTankType] = @tankTye AND [vTankCode] = @tankNo 
	AND [vItemCode] = @itemCode AND [vLotNumber] = @lotNumber 
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[MBL_GetTankTransferInfo]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetTankTransferInfo]
GO

CREATE PROC [dbo].[MBL_GetTankTransferInfo]
	@tankTye VARCHAR(MAX),
	@tankNo VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bTransferred], 0), [vDescription]
    FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @tankTye AND [vTankCode] = @tankNo 
	AND [vItemCode] = @itemCode AND [dSolidity] IS NOT NULL
    ORDER BY [iLineID] DESC
GO



--------------------------------------------------------- PGM ---------------------------------------------------------------------------




IF (OBJECT_ID('[dbo].[PGM_GetItemBatch]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetItemBatch]
GO

CREATE PROC [dbo].[PGM_GetItemBatch]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	SELECT [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration] FROM [htbl_RTIS_PGM_Manuf]
    WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lot AND [vWhseCode] = @location
GO



IF (OBJECT_ID('[dbo].[PGM_CheckContainerUsed_Global]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckContainerUsed_Global]
GO

CREATE PROC [dbo].[PGM_CheckContainerUsed_Global]
	@container VARCHAR(MAX)
AS
	SELECT TOP 1 [bTransferred] FROM [ltbl_RTIS_PGM_Manuf]
    WHERE [vContainer] = @container ORDER BY [iLineID] DESC
GO



IF (OBJECT_ID('[dbo].[PGM_CheckContainerUsed]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckContainerUsed]
GO

CREATE PROC [dbo].[PGM_CheckContainerUsed]
	@headerID VARCHAR(MAX),
	@container VARCHAR(MAX)
AS
	SELECT [vContainer] FROM [ltbl_RTIS_PGM_Manuf]
    WHERE [iHeaderID] = @headerID AND [vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_CheckContTransState]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckContTransState]
GO

CREATE PROC [dbo].[PGM_CheckContTransState]
	@container VARCHAR(MAX),
	@headerID VARCHAR(MAX)
AS
	SELECT TOP 1 ISNULL([bTransferred], 0) FROM [ltbl_RTIS_PGM_Manuf] 
	WHERE [vContainer] = @container AND [iHeaderID] = @headerID ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_CheckContRecState_Global]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckContRecState_Global]
GO

CREATE PROC [dbo].[PGM_CheckContRecState_Global]
	@container VARCHAR(MAX)
AS
	SELECT TOP 1 ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans] 
	WHERE [vContainer] = @container ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_CheckContRecState]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckContRecState]
GO

CREATE PROC [dbo].[PGM_CheckContRecState]
	@container VARCHAR(MAX),
	@headerID VARCHAR(MAX)
AS
	SELECT TOP 1 ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans] 
	WHERE [vContainer] = @headerID AND [iHeaderID] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_GetRemainderCaptured]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetRemainderCaptured]
GO

CREATE PROC [dbo].[PGM_GetRemainderCaptured]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	SELECT ISNULL([bRemainderSet], 0) FROM [htbl_RTIS_PGM_Manuf]
	WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lot AND [vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[PGM_GetBatchLines]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetBatchLines]
GO

CREATE PROC [dbo].[PGM_GetBatchLines]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	SELECT pl.[vContainer], pl.[dWeightIn], ISNULL(pl.[bManuf], 0), ISNULL(pl.[bTransferred], 0) FROM [ltbl_RTIS_PGM_Manuf] pl 
	INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lot AND ph.[vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[PGM_GetHeaderID]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetHeaderID]
GO

CREATE PROC [dbo].[PGM_GetHeaderID]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	SELECT TOP 1 [iLineID] FROM [htbl_RTIS_PGM_Manuf]
    WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lot AND [vWhseCode] = @location ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_GetContOldInfo]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetContOldInfo]
GO

CREATE PROC [dbo].[PGM_GetContOldInfo]
	@cont VARCHAR(MAX)
AS
	SELECT TOP 1 tl.[dWeightOut], tl.[vWhseTo], tl.[vDestWhse], tl.[iLineID], h.[vItemCode], h.[vLotDesc] FROM [ltbl_RTIS_PGM_Trans] tl 
    INNER JOIN [htbl_RTIS_PGM_Manuf] h ON h.[iLineID] = tl.[iHeaderID]
    WHERE [vContainer] = @cont
    ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_SelectVWTransferSlurries]') IS NOT NULL)
	DROP PROC [dbo].[PGM_SelectVWTransferSlurries]
GO

CREATE PROC [dbo].[PGM_SelectVWTransferSlurries]
AS
	SELECT DISTINCT [vSlurryCode] FROM [rtbl_VW_Slurry_PGM]
GO




IF (OBJECT_ID('[dbo].[PGM_SelectTTransferPowders]') IS NOT NULL)
	DROP PROC [dbo].[PGM_SelectTTransferPowders]
GO

CREATE PROC [dbo].[PGM_SelectTTransferPowders]
AS
	SELECT DISTINCT [vPowderCode] FROM [rtbl_T_Powder_PGM]
GO




IF (OBJECT_ID('[dbo].[PGM_SelectTTransferSlurries]') IS NOT NULL)
	DROP PROC [dbo].[PGM_SelectTTransferSlurries]
GO

CREATE PROC [dbo].[PGM_SelectTTransferSlurries]
AS
	SELECT DISTINCT [vSlurryCode] FROM [rtbl_T_Slurry_PGM]
GO




IF (OBJECT_ID('[dbo].[PGM_SelectTTransferAW]') IS NOT NULL)
	DROP PROC [dbo].[PGM_SelectTTransferAW]
GO

CREATE PROC [dbo].[PGM_SelectTTransferAW]
AS
	SELECT DISTINCT [vCatalystCode] FROM [rtbl_T_Catalyst_PGM]
GO




IF (OBJECT_ID('[dbo].[PGM_SelectTTransferAW]') IS NOT NULL)
	DROP PROC [dbo].[PGM_SelectTTransferAW]
GO

CREATE PROC [dbo].[PGM_SelectTTransferAW]
AS
	SELECT DISTINCT [vCatalystCode] FROM [rtbl_T_Catalyst_PGM]
GO




IF (OBJECT_ID('[dbo].[PGM_CheckItemAllowedVW]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckItemAllowedVW]
GO

CREATE PROC [dbo].[PGM_CheckItemAllowedVW]
	@manufItem VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [rtbl_VW_Slurry_PGM] WHERE [vSlurryCode] = @manufItem AND [vPGMCode] = @itemCode
GO




IF (OBJECT_ID('[dbo].[PGM_CheckItemAllowedTP]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckItemAllowedTP]
GO

CREATE PROC [dbo].[PGM_CheckItemAllowedTP]
	@manufItem VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [rtbl_T_Powder_PGM] WHERE [vPowderCode] = @manufItem AND [vPGMCode] = @itemCode
GO




IF (OBJECT_ID('[dbo].[PGM_CheckItemAllowedTS]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckItemAllowedTS]
GO

CREATE PROC [dbo].[PGM_CheckItemAllowedTS]
	@manufItem VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [rtbl_T_Slurry_PGM] WHERE [vSlurryCode] = @manufItem AND [vPGMCode] = @itemCode
GO




IF (OBJECT_ID('[dbo].[PGM_CheckItemAllowedTAW]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckItemAllowedTAW]
GO

CREATE PROC [dbo].[PGM_CheckItemAllowedTAW]
	@manufItem VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [rtbl_T_Catalyst_PGM] WHERE [vCatalystCode] = @manufItem AND [vPGMCode] = @itemCode
GO




IF (OBJECT_ID('[dbo].[PGM_GetVWWIPLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetVWWIPLoc]
GO

CREATE PROC [dbo].[PGM_GetVWWIPLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMVW'
GO




IF (OBJECT_ID('[dbo].[PGM_GetVWDestLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetVWDestLoc]
GO

CREATE PROC [dbo].[PGM_GetVWDestLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMVW-WIP'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTPWIPLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTPWIPLoc]
GO

CREATE PROC [dbo].[PGM_GetTPWIPLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTP'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTPDestLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTPDestLoc]
GO

CREATE PROC [dbo].[PGM_GetTPDestLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTP-WIP'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTSWIPLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTSWIPLoc]
GO

CREATE PROC [dbo].[PGM_GetTSWIPLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTS'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTSDestLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTSDestLoc]
GO

CREATE PROC [dbo].[PGM_GetTSDestLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTS-WIP'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTAWWIPLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTAWWIPLoc]
GO

CREATE PROC [dbo].[PGM_GetTAWWIPLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTAW'
GO




IF (OBJECT_ID('[dbo].[PGM_GetTAWDestLoc]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetTAWDestLoc]
GO

CREATE PROC [dbo].[PGM_GetTAWDestLoc]
AS
	SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTAW-WIP'
GO




IF (OBJECT_ID('[dbo].[PGM_GetContainerInfo]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetContainerInfo]
GO

CREATE PROC [dbo].[PGM_GetContainerInfo]
	@container VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	SELECT TOP 1 ph.[vItemCode], ph.[vLotDesc], pl.[dWeightIn], ph.[iLineID] FROM [ltbl_RTIS_PGM_Manuf] pl 
	INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	WHERE pl.[vContainer] = @container AND ph.[vWhseCode] = @location ORDER BY pl.[iLineID] DESC
GO



--=========================================================================================================--
	------------------------	CHECK BUG	--------------------------
--=========================================================================================================--


IF (OBJECT_ID('[dbo].[PGM_GetItemLocQty]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetItemLocQty]
GO

CREATE PROC [dbo].[PGM_GetItemLocQty]
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	--SELECT [dWeightBal] FROM [htbl_RTIS_PGM_Manuf]
 --   WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lot AND [vPGMLoc] = @location
GO





IF (OBJECT_ID('[dbo].[PGM_CheckItemTransferred]') IS NOT NULL)
	DROP PROC [dbo].[PGM_CheckItemTransferred]
GO

CREATE PROC [dbo].[PGM_CheckItemTransferred]
	@container VARCHAR(MAX),
	@headerID VARCHAR(MAX)
AS
	SELECT TOP 1 [bReceived] FROM [ltbl_RTIS_PGM_Trans] 
	WHERE [vContainer] = @container AND [iHeaderID] = @headerID ORDER BY [iLineID] DESC
GO





IF (OBJECT_ID('[dbo].[PGM_GetAllPGM]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetAllPGM]
GO

CREATE PROC [dbo].[PGM_GetAllPGM]
	@whseCode VARCHAR(MAX)
AS
	SELECT DISTINCT [vItemCode] FROM [htbl_RTIS_PGM_Manuf] WHERE [vWhseCode] = @whseCode
GO




IF (OBJECT_ID('[dbo].[PGM_GetAllPGMItemHeaders]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetAllPGMItemHeaders]
GO

CREATE PROC [dbo].[PGM_GetAllPGMItemHeaders]
	@itemCode VARCHAR(MAX),
	@whseCode VARCHAR(MAX)
AS
	SELECT [vLotDesc], [dConcentration], [dWeightIn], [dWeightOut], [dWeightBal]
    FROM [htbl_RTIS_PGM_Manuf] WHERE [vItemCode] = @itemCode AND [vWhseCode] = @whseCode
GO




IF (OBJECT_ID('[dbo].[PGM_GetAllPGMItemHeaderRows]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetAllPGMItemHeaderRows]
GO

CREATE PROC [dbo].[PGM_GetAllPGMItemHeaderRows]
	@itemCode VARCHAR(MAX),
	@whseCode VARCHAR(MAX),
	@rowCount INT
AS
	SELECT TOP(@rowCount) [vLotDesc], [dConcentration], [dWeightIn], [dWeightOut], [dWeightBal]
    FROM [htbl_RTIS_PGM_Manuf] 
	WHERE [vItemCode] = @itemCode AND [vWhseCode] = @rowCount ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_GetAllPGMItemTransactions]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetAllPGMItemTransactions]
GO

CREATE PROC [dbo].[PGM_GetAllPGMItemTransactions]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@whseCode VARCHAR(MAX)
AS
	SELECT pl.[vUserAdded], pl.[dtDateAdded], pl.[vContainer],ph.[dConcentration], pl.[dWeightIn], pl.[dWeightOut], ph.[vLotDesc], pl.[dtDateAdded]  FROM [ltbl_RTIS_PGM_Manuf] pl 
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
    WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lotNumber AND  ph.[vWhseCode] = @whseCode
    UNION
    SELECT pl.[vUserAdded],  pl.[dtDateAdded], pl.[vContainer],ph.[dConcentration], pl.[dWeightIn], pl.[dWeightOut], ph.[vLotDesc], pl.[dtDateAdded]  FROM [ltbl_RTIS_PGM_Trans] pl 
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
    WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lotNumber AND  ph.[vWhseCode] = @whseCode
    ORDER BY pl.[dtDateAdded] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_GetContainerInfoForRecTrans]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetContainerInfoForRecTrans]
GO

CREATE PROC [dbo].[PGM_GetContainerInfoForRecTrans]
	@container VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT  TOP 1 ph.[vItemCode], ph.[vLotDesc], pl.[dWeightOut], s.[Description_1], ISNULL([bReceived], 0), ph.[iLineID] FROM [ltbl_RTIS_PGM_Trans] pl 
	INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = ph.[vItemCode]
	WHERE pl.[vContainer] = @container AND ph.[vItemCode] = @itemCode ORDER BY ph.[iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_GetContainerInfoForRecTrans]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetContainerInfoForRecTrans]
GO

CREATE PROC [dbo].[PGM_GetContainerInfoForRecTrans]
	@container VARCHAR(MAX),
	@itemCode VARCHAR(MAX)
AS
	SELECT  TOP 1 ph.[vItemCode], ph.[vLotDesc], pl.[dWeightOut], s.[Description_1], ISNULL([bReceived], 0), ph.[iLineID] FROM [ltbl_RTIS_PGM_Trans] pl 
	INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = ph.[vItemCode]
	WHERE pl.[vContainer] = @container AND ph.[vItemCode] = @itemCode ORDER BY ph.[iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[PGM_GetPGMHeaderID]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetPGMHeaderID]
GO

CREATE PROC [dbo].[PGM_GetPGMHeaderID]
	@itemCode VARCHAR(MAX),
	@lotNo VARCHAR(MAX)
AS
	SELECT [iLineID] FROM [htbl_RTIS_PGM_Manuf] WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lotNo
GO




IF (OBJECT_ID('[dbo].[PGM_GetPGMReceived]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetPGMReceived]
GO

CREATE PROC [dbo].[PGM_GetPGMReceived]
	@headerID VARCHAR(MAX),
	@contNo VARCHAR(MAX)
AS
	SELECT ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans]
	WHERE [iHeaderID] = @headerID AND [vContainer] = @contNo
GO




IF (OBJECT_ID('[dbo].[PGM_GetCheckContainerManuf]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetCheckContainerManuf]
GO

CREATE PROC [dbo].[PGM_GetCheckContainerManuf]
	@headerID VARCHAR(MAX),
	@contNo VARCHAR(MAX)
AS
	SELECT ISNULL([bManuf], 0) FROM [ltbl_RTIS_PGM_Manuf] 
	WHERE [iHeaderID] = @headerID AND [vContainer] = @contNo
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMJobLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMJobLines]
GO

CREATE PROC [dbo].[UI_GetPGMJobLines]
AS
	SELECT [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
    FROM [htbl_RTIS_PGM_Manuf]
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMJobrows]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMJobrows]
GO

CREATE PROC [dbo].[UI_GetPGMJobrows]
	@rowCount INT
AS
	SELECT TOP(@rowCount) [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
    FROM [htbl_RTIS_PGM_Manuf] ORDER BY [iLineID] DESC
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMJobsByDate]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMJobsByDate]
GO

CREATE PROC [dbo].[UI_GetPGMJobsByDate]
	@dateFrom VARCHAR(MAX),
	@dateTo VARCHAR(MAX)
AS
	SELECT [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
    FROM [htbl_RTIS_PGM_Manuf] 
	WHERE [dtDateAdded] BETWEEN @dateFrom AND @dateTo ORDER BY [iLineID] DESC, [dtDateAdded] DESC
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMContainers]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMContainers]
GO

CREATE PROC [dbo].[UI_GetPGMContainers]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX)
AS
	SELECT pl.[vContainer], pl.[dWeightIn], pl.[vUserAdded], pl.[dtDateAdded], ISNULL( pl.[bManuf], 'false'), pl.[dtManufDate], pl.[vUserManuf], pl.[vUserEdited], pl.[dtDateEdited], pl.[vEditReason]
    FROM [ltbl_RTIS_PGM_Manuf] pl
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
    WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lotNumber
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMManufactureHeaders]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMManufactureHeaders]
GO

CREATE PROC [dbo].[UI_GetPGMManufactureHeaders]
AS
	SELECT ph.[vItemCode], ph.[vLotDesc], ph.[vWhseCode], ph.[dConcentration], SUM(pl.[dWeightIn]) AS [Qty Waiting], ISNULL(ph.[bRemainderSet], 1) , ISNULL(ph.[dRemainder], 0) FROM [ltbl_RTIS_PGM_Manuf] pl
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
    WHERE ISNULL([bManuf], 0) = 0 GROUP BY ph.[vItemCode], ph.[vLotDesc],ph.[vWhseCode], ph.[dConcentration], ph.[bRemainderSet], ph.[dRemainder]
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMManufactureContainers]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMManufactureContainers]
GO

CREATE PROC [dbo].[UI_GetPGMManufactureContainers]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@location VARCHAR(MAX),
	@concentration VARCHAR(MAX)
AS
	SELECT pl.[vContainer], pl.[dWeightIn], pl.[vUserAdded], pl.[dtDateAdded], '' AS [Manuf], '' AS [Edit]
    FROM [ltbl_RTIS_PGM_Manuf] pl
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
    WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lotNumber AND ph.[vWhseCode] = @location 
	AND ph.[dConcentration] = @concentration AND ISNULL(pl.[bManuf], 0) = 0
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMBatchTotal]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMBatchTotal]
GO

CREATE PROC [dbo].[UI_GetPGMBatchTotal]
	@headerID VARCHAR(MAX)
AS
	SELECT SUM(l.[dWeightIn]) + ISNULL(h.[dRemainder], 0) AS [Total] FROM [ltbl_RTIS_PGM_Manuf] l
    INNER JOIN [htbl_RTIS_PGM_Manuf] h ON h.[iLineID] = l.[iHeaderID]
    WHERE ISNULL([bManuf], 0) = 0 AND [iHeaderID] = @headerID
    GROUP BY h.[dRemainder]
GO


IF (OBJECT_ID('[dbo].[PGM_AddWhseTransferLog]') IS NOT NULL)
	DROP PROC [dbo].[PGM_AddWhseTransferLog]
GO

CREATE PROC [dbo].[PGM_AddWhseTransferLog]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@whseFrom VARCHAR(MAX),
	@whseTo VARCHAR(MAX),
	@qty DECIMAL,
	@username VARCHAR(MAX),
	@process VARCHAR(MAX)
AS
	INSERT INTO [stbl_WHTLog] ([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateTransfered], [vUsername], [vProcess])
    VALUES (@itemCode, @lotNumber, @whseFrom, @whseTo, @qty, GETDATE(), @username, @process)
GO




IF (OBJECT_ID('[dbo].[PGM_ManufactureHeader]') IS NOT NULL)
	DROP PROC [dbo].[PGM_ManufactureHeader]
GO

CREATE PROC [dbo].[PGM_ManufactureHeader]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@wIn VARCHAR(MAX),
	@wBal VARCHAR(MAX),
	@concentration VARCHAR(MAX),
	@location VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	INSERT INTO [htbl_RTIS_PGM_Manuf] ([vItemCode], [vLotDesc], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [vWhseCode], [dtDateAdded], [vUserAdded]) OUTPUT INSERTED.iLineID
    VALUES (@itemCode, @lotNumber, @wIn, 0, @wBal, @concentration, @location, GETDATE(), @userName)
GO




IF (OBJECT_ID('[dbo].[PGM_ManufactureHeader]') IS NOT NULL)
	DROP PROC [dbo].[PGM_ManufactureHeader]
GO

CREATE PROC [dbo].[PGM_ManufactureHeader]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@wIn VARCHAR(MAX),
	@wBal VARCHAR(MAX),
	@concentration VARCHAR(MAX),
	@location VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	INSERT INTO [htbl_RTIS_PGM_Manuf] ([vItemCode], [vLotDesc], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [vWhseCode], [dtDateAdded], [vUserAdded]) OUTPUT INSERTED.iLineID
    VALUES (@itemCode, @lotNumber, @wIn, 0, @wBal, @concentration, @location, GETDATE(), @userName)
GO




IF (OBJECT_ID('[dbo].[PGM_ManufactureLine]') IS NOT NULL)
	DROP PROC [dbo].[PGM_ManufactureLine]
GO

CREATE PROC [dbo].[PGM_ManufactureLine]
	@HeaderID VARCHAR(MAX),
	@container VARCHAR(MAX),
	@weightIn VARCHAR(MAX),
	@userName VARCHAR(MAX)
AS
	INSERT INTO [ltbl_RTIS_PGM_Manuf] ([iHeaderID],[vContainer], [dWeightIn], [dWeightOut], [dtDateAdded], [vUserAdded], [bTransferred])
    VALUES (@HeaderID, @container, @weightIn, 0, GETDATE(), @userName, 0)
GO




IF (OBJECT_ID('[dbo].[PGM_AddTransferOutLine]') IS NOT NULL)
	DROP PROC [dbo].[PGM_AddTransferOutLine]
GO

CREATE PROC [dbo].[PGM_AddTransferOutLine]
	@HeaderID VARCHAR(MAX),
	@container VARCHAR(MAX),
	@weightOut VARCHAR(MAX),
	@userName VARCHAR(MAX),
	@whseFrom VARCHAR(MAX),
	@whseTo VARCHAR(MAX),
	@manufItem VARCHAR(MAX),
	@manufBatch VARCHAR(MAX),
	@dsetWhse VARCHAR(MAX)
AS
	INSERT INTO [ltbl_RTIS_PGM_Trans] ([iHeaderID],[vContainer], [dWeightIn], [dWeightOut], [dtDateAdded], [vUserAdded], [vWhseFrom], [vWhseTo], [ManufItem], [ManufBatch], [vDestWhse])
    VALUES (@HeaderID, @container, 0, @weightOut, GETDATE(), @userName, @whseFrom, @whseTo,@manufItem, @manufBatch, @dsetWhse)
GO




IF (OBJECT_ID('[dbo].[PGM_updateContRec]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContRec]
GO

CREATE PROC [dbo].[PGM_updateContRec]
	@lineID VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 WHERE [iLineID] = @lineID
GO



IF (OBJECT_ID('[dbo].[PGM_updateHeader]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateHeader]
GO

CREATE PROC [dbo].[PGM_updateHeader]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@wIn VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	UPDATE [htbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @wIn, [dWeightBal] = [dWeightBal] + @wIn 
	WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lotNumber AND [vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[PGM_updateManufOut]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateManufOut]
GO

CREATE PROC [dbo].[PGM_updateManufOut]
	@cont VARCHAR(MAX),
	@headerID VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] SET [bTransferred] = 1 
	WHERE [vContainer] = @cont AND [iHeaderID] = @headerID 
	AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [ltbl_RTIS_PGM_Manuf] 
	WHERE [vContainer] = @cont AND [iHeaderID] = @headerID ORDER BY [iLineID] DESC)
GO




IF (OBJECT_ID('[dbo].[PGM_updateHeaderQtyOut]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateHeaderQtyOut]
GO

CREATE PROC [dbo].[PGM_updateHeaderQtyOut]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@wOut VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	UPDATE [htbl_RTIS_PGM_Manuf] 
	SET [dWeightOut] = [dWeightOut] + @wOut, [dWeightBal] = [dWeightBal] - @wOut 
	WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lotNumber AND [vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[PGM_updateContQtyHeader]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContQtyHeader]
GO

CREATE PROC [dbo].[PGM_updateContQtyHeader]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@wIn VARCHAR(MAX),
	@location VARCHAR(MAX)
AS
	UPDATE [htbl_RTIS_PGM_Manuf] 
	SET [dWeightIn] = [dWeightIn] + @wIn, [dWeightBal] = [dWeightBal] + @wIn 
	WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lotNumber AND [vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[PGM_updateContQtyLine]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContQtyLine]
GO

CREATE PROC [dbo].[PGM_updateContQtyLine]
	@headerID VARCHAR(MAX),
	@container VARCHAR(MAX),
	@qty DECIMAL
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @qty 
	WHERE [iHeaderID] = @headerID AND [vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_setBatchRemainder]') IS NOT NULL)
	DROP PROC [dbo].[PGM_setBatchRemainder]
GO

CREATE PROC [dbo].[PGM_setBatchRemainder]
	@remQty VARCHAR(MAX),
	@itemCode VARCHAR(MAX),
	@lot VARCHAR(MAX),
	@con VARCHAR(MAX),
	@whse VARCHAR(MAX),
	@username VARCHAR(MAX)
AS
	UPDATE [htbl_RTIS_PGM_Manuf] 
	SET [dRemainder] = @remQty, [bRemainderSet] = 1, [dtRemainderSet] = GETDATE(), [vRemainderUser] = @username
    WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lot AND [dConcentration] = @con AND [vWhseCode] = @whse
GO




IF (OBJECT_ID('[dbo].[UI_updateContQtyLine]') IS NOT NULL)
	DROP PROC [dbo].[UI_updateContQtyLine]
GO

CREATE PROC [dbo].[UI_updateContQtyLine]
	@headerID VARCHAR(MAX),
	@container VARCHAR(MAX),
	@qty DECIMAL,
	@username VARCHAR(MAX),
	@reason VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] 
	SET [dWeightIn] = [dWeightIn] + @qty, [vUserEdited] = @username, [dtDateEdited] = GETDATE(), [vEditReason] = @reason  
	WHERE [iHeaderID] = @headerID AND [vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_updateContReceived_PPRec]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContReceived_PPRec]
GO

CREATE PROC [dbo].[PGM_updateContReceived_PPRec]
	@container VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 WHERE [iLineID] = (SELECT TOP 1 [iLineID] 
	FROM [ltbl_RTIS_PGM_Trans] 
	WHERE [vContainer] = @container ORDER BY [iLineID] DESC)
GO




IF (OBJECT_ID('[dbo].[PGM_updateContReceived]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContReceived]
GO

CREATE PROC [dbo].[PGM_updateContReceived]
	@headerID VARCHAR(MAX),
	@container VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 
	WHERE [iHeaderID] = @headerID AND [vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_updateContManufactured]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContManufactured]
GO

CREATE PROC [dbo].[PGM_updateContManufactured]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@container VARCHAR(MAX)
AS
	UPDATE pl SET pl.[bManuf] = 1 FROM [ltbl_RTIS_PGM_Manuf] pl
    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
    WHERE ph.[vItemCode] = @itemCode AND ph.[vLotDesc] = @lotNumber AND pl.[vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[PGM_updateContManufactured]') IS NOT NULL)
	DROP PROC [dbo].[PGM_updateContManufactured]
GO

CREATE PROC [dbo].[PGM_updateContManufactured]
	@itemCode VARCHAR(MAX),
	@container VARCHAR(MAX),
	@UserName VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @UserName 
	WHERE [iHeaderID] = @itemCode AND [vContainer] = @container
GO




IF (OBJECT_ID('[dbo].[UI_setPGMBatchManufactured]') IS NOT NULL)
	DROP PROC [dbo].[UI_setPGMBatchManufactured]
GO

CREATE PROC [dbo].[UI_setPGMBatchManufactured]
	@itemCode VARCHAR(MAX),
	@UserName VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] 
	SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @UserName 
	WHERE [iHeaderID] = @itemCode AND ISNULL( [bManuf] , 0) = 0
GO




IF (OBJECT_ID('[dbo].[UI_updateContainerRem]') IS NOT NULL)
	DROP PROC [dbo].[UI_updateContainerRem]
GO

CREATE PROC [dbo].[UI_updateContainerRem]
	@itemCode VARCHAR(MAX),
	@lotNumber VARCHAR(MAX),
	@location VARCHAR(MAX),
	@qty DECIMAL,
	@UserName VARCHAR(MAX)
AS
	UPDATE [htbl_RTIS_PGM_Manuf] 
	SET [dRemainder] = @qty, [dtRemainderUpdated] = GETDATE(), [vRemUpdateUser] = @UserName
    WHERE [vItemCode] = @itemCode AND [vLotDesc] = @lotNumber AND [vWhseCode] = @location
GO




IF (OBJECT_ID('[dbo].[UI_setPGMBatchManufacturedManual]') IS NOT NULL)
	DROP PROC [dbo].[UI_setPGMBatchManufacturedManual]
GO

CREATE PROC [dbo].[UI_setPGMBatchManufacturedManual]
	@headerID VARCHAR(MAX),
	@UserName VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] 
	SET [bManuf] = '1', [dtManufDateManual] = GETDATE(), [vUserManufManual] = @UserName 
	WHERE [iHeaderID] = @headerID AND ISNULL( [bManuf] , 0) = 0
GO
























































































































































































































































