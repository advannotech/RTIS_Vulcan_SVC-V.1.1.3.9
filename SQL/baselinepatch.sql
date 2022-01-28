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



 ------------------------sp_UI_InsertRMLink_fs----------------
IF (OBJECT_ID('sp_UI_InsertRMLink_fs') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertRMLink_fs]
GO
CREATE PROC [dbo].[sp_UI_InsertRMLink_fs]
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
	@FROM nvarchar(MAX),
	@TO nvarchar(MAX)
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM 
    [htbl_PalletBarcodes] WHERE [Printed] BETWEEN @FROM AND @TO AND [bRMPallet] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByItem]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByItem]
	@ItemCode VARCHAR(MAX)
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] 
	FROM [htbl_PalletBarcodes] WHERE [vUnqBarcode] LIKE '%' + @ItemCode + '%' AND [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByItemAndDate]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByItemAndDate]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByItemAndDate]
	@ItemCode VARCHAR(MAX),
	@FROM datetime,
	@TO datetime
AS
	SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM 
    [htbl_PalletBarcodes] WHERE [vUnqBarcode] LIKE '%' + @itemCode + '%' 
    AND [Printed] BETWEEN @FROM AND @TO AND [bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByLot]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByLot]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByLot]
	@LOT VARCHAR(MAX)
AS
	SELECT [iPallet_ID] FROM [ltbl_PalletBarcodes] l 
    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
    WHERE l.[vUnqBarcode] LIKE '%' + @LOT + '%' AND h.[bRMPallet] = 1
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPalletsByLotAndDate]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletsByLotAndDate]
GO

CREATE PROC [dbo].[sp_UI_GetPalletsByLotAndDate]
	@LOT varchar(max),
	@FROM datetime,
	@TO datetime
AS
SELECT [iPallet_ID] FROM [ltbl_PalletBarcodes] l 
INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
WHERE l.[vUnqBarcode] LIKE '%' + @LOT + '%' AND h.[Printed] BETWEEN @FROM AND @TO AND h.[bRMPallet] = 1
GO



IF (OBJECT_ID('[dbo].[UI_sp_GetPalletsByIDList]') IS NOT NULL)
	DROP PROC [dbo].[UI_sp_GetPalletsByIDList]
GO
CREATE PROC [dbo].[UI_sp_GetPalletsByIDList]
	@iLine_ID int
AS
SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM [htbl_PalletBarcodes]
WHERE [iLine_ID] IN (@iLine_ID)
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetPalletLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletLots]
GO

CREATE PROC [dbo].[sp_UI_GetPalletLots]
	@ID VARCHAR(MAX)
AS
	SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @ID
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetPalletLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletLines]
GO
CREATE PROC [dbo].[sp_UI_GetPalletLines]
	@ID VARCHAR(MAX)
AS
	SELECT l.[vUnqBarcode], l.[bOnPallet] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @ID
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletPrinSettings_Printer]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletPrinSettings_Printer]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletPrinSettings_Printer]
	@Printer VARCHAR(MAX)
AS
	UPDATE [tbl_RTSettings] SET [SettingValue] = @Printer WHERE [Setting_Name] = 'Pallet Printer'
GO



IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletPrinSettings_Label]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletPrinSettings_Label]
GO

CREATE PROC [dbo].[sp_UI_UpdatePalletPrinSettings_Label]
	@Label VARCHAR(MAX)
AS
	UPDATE [tbl_RTSettings] SET [SettingValue] = @Label WHERE [Setting_Name] = 'Pallet Label'
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPowderPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPowderPlanLines]
GO

CREATE PROC [dbo].[sp_UI_GetPowderPlanLines]
AS
SELECT [iLineID], '' AS [vAWCode], '' AS [CatalystCode], [vSlurryCode], [vPowderCode], '' AS [CoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit]
FROM [rtbl_Slurry_Powders]
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetZECT1PlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECT1PlanLines]
GO
CREATE PROC [dbo].[sp_UI_GetZECT1PlanLines]
AS
SELECT [iLineID], '' AS [vAWCode], [vCatalystCode], [vSlurryCode], '' AS [PowderCode], [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
FROM [rtbl_Slurry_Catalyst] 
WHERE [iZectLine] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetZECT2PlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECT2PlanLines]
GO
CREATE PROC [dbo].[sp_UI_GetZECT2PlanLines]
AS
SELECT [iLineID], '' AS [vAWCode], [vCatalystCode], [vSlurryCode], '' AS [PowderCode], [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
FROM [rtbl_Slurry_Catalyst] 
WHERE [iZectLine] = 2
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetAWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAWPlanLines]
GO
CREATE PROC [dbo].[sp_UI_GetAWPlanLines]
AS
SELECT [iLineID], [vAWCode], [vCatalystCode], '' AS [vSlurryCode], '' AS [PowderCode], '' AS [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
FROM [rtbl_Catalyst_AW]
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdateZECTPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateZECTPlanLines]
GO
CREATE PROC [dbo].[sp_UI_UpdateZECTPlanLines]
@Cataylst varchar(max),
@Slurry varchar(max),
@Coat varchar(max),
@User varchar(max),
@ZECTnum int,
@iLineID int
AS
UPDATE [rtbl_Slurry_Catalyst] 
SET [vCatalystCode] = @Cataylst,[vSlurryCode] = @Slurry,[vCoatNum] = @Coat
,[dtDateEdit] = GETDATE(),[vUserEdit] = @User,[iZectLine] = @ZECTnum 
WHERE [iLineID] = @iLineID
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdatePowderPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePowderPlanLines]
GO
CREATE PROC [dbo].[sp_UI_UpdatePowderPlanLines]
@Slurry varchar(max),
@Powder varchar(max),
@User varchar(max),
@ID int
AS
UPDATE [rtbl_Slurry_Powders] 
SET [vSlurryCode] = @Slurry,[vPowderCode] = @Powder
,[dtDateEdit] = GETDATE(),[vUserEdit] = @User
WHERE [iLineID] = @ID
GO


IF (OBJECT_ID('[dbo].[sp_UI_InsertZECTPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertZECTPlanLines]
GO
CREATE PROC [dbo].[sp_UI_InsertZECTPlanLines]
@Cataylst varchar(max),
@Slurry varchar(max),
@Coat varchar(max),
@User varchar(max),
@ZECTnum int
AS
INSERT INTO [rtbl_Slurry_Catalyst]([vCatalystCode],[vSlurryCode],[vCoatNum],[dtDateAdd],[vUserAdd],[iZectLine])
VALUES(@Cataylst,@Slurry,@Coat,GETDATE(),@User,@ZECTnum)
GO




IF (OBJECT_ID('[dbo].[sp_UI_InsertPowderPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertPowderPlanLines]
GO
CREATE PROC [dbo].[sp_UI_InsertPowderPlanLines]
@Slurry varchar(max),
@Powder varchar(max),
@User varchar(max)
AS
INSERT INTO [rtbl_Slurry_Powders]([vSlurryCode],[vPowderCode],[dtDateAdd],[vUserAdd])
VALUES(@Slurry,@Powder,GETDATE(),@User)
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPowderPrepWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPowderPrepWhes]
GO
CREATE PROC [dbo].[sp_MBL_GetPowderPrepWhes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_PPtFS] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetWaitingPowders]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetWaitingPowders]
GO
CREATE PROC [dbo].[sp_UI_GetWaitingPowders]
AS
SELECT [iLineID],[vItemCode],[vItemDesc],[vLotDesc],[dQty],[vUsername],[dtDateAdded], '' AS [ManufactureButton], ''  AS [EditButton]
FROM [tbl_RTIS_Powder_Prep] 
WHERE [bManufactured] = '0'
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPowderManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPowderManufactured]
GO
CREATE PROC [dbo].[sp_UI_GetPowderManufactured]
(
@iLineID int
)
AS
SELECT ISNULL([bManufactured], 0) 
FROM [tbl_RTIS_Powder_Prep] WHERE [iLineID] = @iLineID
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPowderPrepMF]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPowderPrepMF]
GO
CREATE PROC [dbo].[sp_UI_GetPowderPrepMF]
AS
SELECT [iLineID],[vItemCode],[vItemDesc],[vLotDesc],[dQty],[dtDateAdded]
FROM [tbl_RTIS_Powder_Prep] WHERE [bManufactured] = '0'
ORDER BY [dtDateAdded] DESC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPowderPrepRecords]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPowderPrepRecords]
GO
CREATE PROC [dbo].[sp_UI_GetPowderPrepRecords]
(
@dateFrom datetime,
@dateTo datetime
)
AS
SELECT [vItemCode], [vItemDesc], [vLotDesc], [dQty], [vUsername], [dtDateAdded], ISNULL([bManufactured], 0), [dtManufDate], [vUserManuf], ISNULL([bTransfered], 0)       
,[dtTransDate]
,[vUserTrans]
, ISNULL([bRecTrans], 0)
,[dtRecTrans]
,[vUserRec]
,[vUserEdited]
,[dtDateEdited]
,[vEditReason]
,[dOldQty] 
FROM [tbl_RTIS_Powder_Prep] 
WHERE [dtDateAdded] BETWEEN @dateFrom AND @dateTo
ORDER BY [dtDateAdded] DESC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPowderRMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPowderRMs]
GO
CREATE PROC [dbo].[sp_UI_GetPowderRMs]
(
@itemCode varchar(max),
@lotNumber varchar(max)
)
AS
SELECT h.[vItemCode], h.[vLotDesc],  SUM(l.[dWeightOut]) FROM [ltbl_RTIS_PGM_Trans] l 
INNER JOIN [htbl_RTIS_PGM_Manuf] h ON h.[iLineID] = l.[iHeaderID]
WHERE [ManufItem] = @itemCode AND [ManufBatch] = @lotNumber GROUP BY h.[vItemCode], h.[vLotDesc]
GO



IF (OBJECT_ID('[dbo].[sp_UI_editPowderQty]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_editPowderQty]
GO
CREATE PROC [dbo].[sp_UI_editPowderQty]
(
@LineID int,
@newQty decimal(16,3),
@oldQty decimal(16,3),
@UserName varchar(max),
@reason varchar(max)
)
AS
UPDATE [tbl_RTIS_Powder_Prep] 
SET [dQty] = @newQty, [dOldQty] = @oldQty, [vUserEdited] = @UserName, [dtDateEdited] = GETDATE(), [vEditReason] = @reason
WHERE [iLineID] = @LineID
GO


IF (OBJECT_ID('[dbo].[sp_UI_setPPManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setPPManufactured]
GO
CREATE PROC [dbo].[sp_UI_setPPManufactured]
(
@LineID int,
@UserName varchar(max)
)
AS
UPDATE [tbl_RTIS_Powder_Prep] SET [bManufactured] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @UserName WHERE [iLineID] = @LineID
GO


IF (OBJECT_ID('[dbo].[sp_UI_setPPManufacturedManual]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setPPManufacturedManual]
GO
CREATE PROC [dbo].[sp_UI_setPPManufacturedManual]
(
@LineID int,
@UserName varchar(max)
)
AS
UPDATE [tbl_RTIS_Powder_Prep] SET [bManufactured] = '1', [dtManufDateManual] = GETDATE(), [vUserManufManual] = @UserName WHERE [iLineID] = @LineID
GO


IF (OBJECT_ID('[dbo].[sp_UI_insertPowderPrep]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_insertPowderPrep]
GO
CREATE PROC [dbo].[sp_UI_insertPowderPrep]
(
@iStockID int,
@vItemCode varchar(max),
@vItemDesc varchar(max),
@iLotTrackingID int,
@vLotDesc varchar(max),
@dQty decimal(16,3),
@vUsername varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Powder_Prep] ([iStockID], [vItemCode], [vItemDesc], [iLotTrackingID], [vLotDesc], [dQty], [vUsername], [dtDateAdded], [bManufactured], [bTransfered])
VALUES (@iStockID, @vItemCode, @vItemDesc, @iLotTrackingID, @vLotDesc, @dQty, @vUsername, GETDATE(), 0, 0)
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWhseProcLookUp]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhseProcLookUp]
GO
CREATE PROC [dbo].[sp_UI_getWhseProcLookUp]
(
@vProcessName varchar(60)
)
AS
SELECT wm.[WhseLink], wm.[Name], '' AS [Remove] 
FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] wm ON wm.[WhseLink] = wl.[iWhseID]
WHERE wl.[vProcessName] = @vProcessName AND [bIsRec] = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWhseProcLookUpRec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhseProcLookUpRec]
GO
CREATE PROC [dbo].[sp_UI_getWhseProcLookUpRec]
(
@vProcessName varchar(60)
)
AS
SELECT wm.[WhseLink], wm.[Name], '' AS [Remove] 
FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] wm ON wm.[WhseLink] = wl.[iWhseID]
WHERE wl.[vProcessName] = @vProcessName AND [bIsRec] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_checkProcRefExists]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_checkProcRefExists]
GO
CREATE PROC [dbo].[sp_UI_checkProcRefExists]
(
@vProcessName varchar(60),
@iWhseID int
)
AS
SELECT [iLine_ID] FROM [tbl_WHTLocations]
WHERE [vProcessName] = @vProcessName AND [iWhseID] = @iWhseID AND [bIsRec] = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWhtProcesses]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtProcesses]
GO
CREATE PROC [dbo].[sp_UI_getWhtProcesses]
AS
SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames] WHERE ISNULL([bHasOutTransfer], 'false') = 1
GO

IF (OBJECT_ID('[dbo].[sp_UI_getWhtProcesses]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtProcesses]
GO
CREATE PROC [dbo].[sp_UI_getWhtProcesses]
AS
SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames] WHERE ISNULL([bHasOutTransfer], 'false') = 1
GO

IF (OBJECT_ID('[dbo].[sp_UI_getWhtProcessesRec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtProcessesRec]
GO
CREATE PROC [dbo].[sp_UI_getWhtProcessesRec]
AS
SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames] WHERE ISNULL([bHasRecTrans], 0) <> 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_PPtFS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_PPtFS]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_PPtFS]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_PPtFS] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_PP_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_PP_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_PP_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_FStMS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_FStMS]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_FStMS]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_FStMS] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_FS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_FS_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_FS_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO

IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_MStZect]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_MStZect]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_MStZect]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_MStZect] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_MS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_MS_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_MS_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Zect1]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Zect1]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Zect1]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Zect1] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Zect1_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Zect1_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Zect1_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Zect2]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Zect2]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Zect2]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Zect2] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Zect2_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Zect2_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Zect2_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Canning]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Canning]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Canning]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Canning] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_Can_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_Can_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_Can_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_AW]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_AW]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_AW] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_AW_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_AW_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_AW_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_PGM_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_PGM_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_PGM_Rec]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseLookUp_ToProd]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseLookUp_ToProd]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseLookUp_ToProd]
AS
SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_ToProd] wl
RIGHT JOIN [Cataler_SCN].[dbo].[WhseMst] w 
ON w.[WhseLink] = wl.[iWhse_Link]
GO

IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_PPtFS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_PPtFS]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_PPtFS]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] 
FROM [RTIS_WarehouseLookUp_PPtFS] 
WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_PP_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_PP_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_PP_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_FS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_FS_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_FS_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_MS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_MS_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_MS_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Zect1_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Zect1_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Zect1_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO

IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Zect2_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Zect2_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Zect2_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Can_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Can_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Can_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_AW_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_AW_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_AW_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_PGM_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_PGM_Rec]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_PGM_Rec]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_FStMS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_FStMS]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_FStMS]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_FStMS] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_MStZect]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_MStZect]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_MStZect]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_MStZect] WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Zect1]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Zect1]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Zect1]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Zect1] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Zect2]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Zect2]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Zect2]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Zect2] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_Can]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_Can]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_Can]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Canning] WHERE [iWhse_Link] = @iWhse_Link
GO




IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_AW]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_AW]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_AW] WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWarehouseExistes_ToProd]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWarehouseExistes_ToProd]
GO
CREATE PROC [dbo].[sp_UI_getWarehouseExistes_ToProd]
(
@iWhse_Link int
)
AS
SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_ToProd] WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef]
(
@vProcessName varchar(60),
@iWhseID int,
@bIsRec bit
)
AS
INSERT INTO [tbl_WHTLocations] ([vProcessName], [iWhseID], [bIsRec]) VALUES (@vProcessName, @iWhseID, @bIsRec)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_PPtFS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_PPtFS]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_PPtFS]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_PPtFS] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_PP_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_PP_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_PP_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_FS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_FS_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_FS_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_MS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_MS_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_MS_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Zect1_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect1_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect1_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Zect2_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect2_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect2_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO



IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Can_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Can_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Can_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_AW_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_AW_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_AW_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO

IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_PGM_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_PGM_Rec]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_PGM_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO



IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_FStMS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_FStMS]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_FStMS]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_FStMS] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_MStZect]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_MStZect]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_MStZect]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_MStZect] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Zect1]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect1]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect1]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_Zect1] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Zect2]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect2]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Zect2]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_Zect2] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO



IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_Canning]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_Canning]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_Canning]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_Canning] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO



IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_AW]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_AW]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_AW] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_addNewWhseCrossRef_ToProd]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_addNewWhseCrossRef_ToProd]
GO
CREATE PROC [dbo].[sp_UI_addNewWhseCrossRef_ToProd]
(
@iWhse_Link int,
@bEnabled bit
)
AS
INSERT INTO [RTIS_WarehouseLookUp_ToProd] ([iWhse_Link], [bEnabled]) VALUES (@iWhse_Link, @bEnabled)
GO


IF (OBJECT_ID('[dbo].[sp_UI_resetWhseCrossRef_PPtFS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_resetWhseCrossRef_PPtFS]
GO
CREATE PROC [dbo].[sp_UI_resetWhseCrossRef_PPtFS]
AS
UPDATE [RTIS_WarehouseLookUp_PPtFS] SET [bEnabled] = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_resetWhseCrossRef_FStMS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_resetWhseCrossRef_FStMS]
GO
CREATE PROC [dbo].[sp_UI_resetWhseCrossRef_FStMS]
AS
UPDATE [RTIS_WarehouseLookUp_FStMS] SET [bEnabled] = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_resetWhseCrossRef_MStZext]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_resetWhseCrossRef_MStZext]
GO
CREATE PROC [dbo].[sp_UI_resetWhseCrossRef_MStZext]
AS
UPDATE [RTIS_WarehouseLookUp_MStZect] SET [bEnabled] = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_PPtFS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_PPtFS]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_PPtFS]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_PPtFS] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_PP_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_PP_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_PP_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_FS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_FS_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_FS_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] SET [bEnabled] =@bEnabled  WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_MS_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_MS_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_MS_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_Zect1_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_Zect2_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_Zect2_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_Zect2_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_Can_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_Can_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_Can_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_AW_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_AW_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_AW_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_PGM_Rec]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_PGM_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_PGM_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__FStMS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_PGM_Rec]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_PGM_Rec]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_FStMS] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__MStZectS]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef__MStZectS]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef__MStZectS]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_MStZect] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__Zect1]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef__Zect1]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef__Zect1]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_Zect1] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__Zect2]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef__Zect2]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef__Zect2]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_Zect2] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__Canning]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef__Canning]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef__Canning]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_Canning] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO



IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef__AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef__AW]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef__AW]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_AW] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_updateWhseCrossRef_ToProd]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_updateWhseCrossRef_ToProd]
GO
CREATE PROC [dbo].[sp_UI_updateWhseCrossRef_ToProd]
(
@iWhse_Link int,
@bEnabled bit
)
AS
UPDATE [RTIS_WarehouseLookUp_ToProd] SET [bEnabled] = @bEnabled WHERE [iWhse_Link] = @iWhse_Link
GO


IF (OBJECT_ID('[dbo].[sp_UI_deleteNewWhseCrossRef]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_deleteNewWhseCrossRef]
GO
CREATE PROC [dbo].[sp_UI_deleteNewWhseCrossRef]
(
@vProcessName varchar(60),
@iWhseID int,
@bIsRec bit
)
AS
DELETE FROM [tbl_WHTLocations] WHERE [vProcessName] = @vProcessName AND [iWhseID] = @iWhseID AND [bIsRec] = @bIsRec
GO



IF (OBJECT_ID('[dbo].[sp_GetActiveModules]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetActiveModules]
GO
CREATE PROC [dbo].[sp_GetActiveModules]
AS
SELECT [iModule_ID], [vModule_Name] FROM [htbl_Modules] WHERE [bModuleActive] = 1 ORDER BY [Indx] ASC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetModuleUserPermission]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetModuleUserPermission]
GO
CREATE PROC [dbo].[sp_UI_GetModuleUserPermission]
(
@iModuleID int,
@vUser_Username varchar(200)
)
AS
SELECT p.[vPermission_Name], p.[vNestNode] 
FROM [ltbl_Module_Perms] p
INNER JOIN [ltbl_userRoleLines] rl ON p.[iPermission_ID] = rl.[iPermission_ID]
INNER JOIN [tbl_users] u ON u.[iRoleID] = rl.[iRole_ID]
WHERE rl.[bPermission_Active] = 1 AND p.[iModuleID] = @iModuleID AND u.[vUser_Username] = @vUser_Username AND p.[bUIPerm] = 1
ORDER BY p.[Indx]
GO




IF (OBJECT_ID('[dbo].[sp_PGM_GetModuleUserPermission]') IS NOT NULL)
	DROP PROC [dbo].[sp_PGM_GetModuleUserPermission]
GO
CREATE PROC [dbo].[sp_PGM_GetModuleUserPermission]
(
@vUser_Username varchar(200)
)
AS
SELECT p.[vPermission_Name]
FROM [ltbl_Module_Perms] p
INNER JOIN [ltbl_userRoleLines] rl ON p.[iPermission_ID] = rl.[iPermission_ID]
INNER JOIN [tbl_users] u ON u.[iRoleID] = rl.[iRole_ID]
WHERE rl.[bPermission_Active] = 1 AND u.[vUser_Username] = @vUser_Username AND p.[bPGMPerm] = 1
ORDER BY p.[Indx]
GO



IF (OBJECT_ID('[dbo].[sp_PGM_GetModuleUserPermission]') IS NOT NULL)
	DROP PROC [dbo].[sp_PGM_GetModuleUserPermission]
GO
CREATE PROC [dbo].[sp_PGM_GetModuleUserPermission]
(
@vUser_Username varchar(200)
)
AS
SELECT p.[vPermission_Name]
FROM [ltbl_Module_Perms] p
INNER JOIN [ltbl_userRoleLines] rl ON p.[iPermission_ID] = rl.[iPermission_ID]
INNER JOIN [tbl_users] u ON u.[iRoleID] = rl.[iRole_ID]
WHERE rl.[bPermission_Active] = 1 AND u.[vUser_Username] = @vUser_Username AND p.[bPGMPerm] = 1
ORDER BY p.[Indx]
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetActiveLabelTypes]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetActiveLabelTypes]
GO
CREATE PROC [dbo].[sp_UI_GetActiveLabelTypes]
(
@vUser_Username varchar(200)
)
AS
SELECT DISTINCT lt.[vLabel_Type_Name] 
FROM [tbl_labelTypes] lt
INNER JOIN [rtbl_LabelPermCom] lpc ON lpc.[iLabelID] = lt.[iLabel_ID]
INNER JOIN [ltbl_userRoleLines] rl ON rl.[iPermission_ID] = lpc.[iPermissionID]
INNER JOIN [htbl_userRoles] rh ON rl.[iRole_ID] = rh.[iRole_ID]
INNER JOIN [tbl_users] u ON u.[iRoleID] = rh.[iRole_ID]
WHERE rl.[bPermission_Active] = '1' AND u.[vUser_Username] = @vUser_Username
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetCompatiblelabels]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCompatiblelabels]
GO
CREATE PROC [dbo].[sp_UI_GetCompatiblelabels]
(
@vPermission_Name varchar(max)
)
AS
SELECT l.[vLabel_Type_Name] FROM [rtbl_LabelPermCom] lpc 
INNER JOIN [ltbl_Module_Perms] p ON lpc.[iPermissionID] = p.[iPermission_ID]
INNER JOIN [tbl_labelTypes] l ON lpc.[iLabelID] = l.[iLabel_ID]
WHERE p.[vPermission_Name] = @vPermission_Name
GO






IF (OBJECT_ID('[dbo].[sp_UI_GetPermissionHasLabel]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPermissionHasLabel]
GO
CREATE PROC [dbo].[sp_UI_GetPermissionHasLabel]
(
@vPermission_Name varchar(max)
)
AS
SELECT [bHasLabel] FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @vPermission_Name
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetPermID]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPermID]
GO
CREATE PROC [dbo].[sp_UI_GetPermID]
(
@vPermission_Name varchar(max)
)
AS
SELECT [iPermission_ID]
FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @vPermission_Name
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetPermLabelsNew]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPermLabelsNew]
GO
CREATE PROC [dbo].[sp_UI_GetPermLabelsNew]
(
@iPermID int
)
AS
 SELECT [vLabelName] FROM [rtbl_PermLabels]
 WHERE [iPermID] = @iPermID
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetGridOverridePassword]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetGridOverridePassword]
GO
CREATE PROC [dbo].[sp_UI_GetGridOverridePassword]
AS
SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'GRIDOVERRIDE'
GO



IF (OBJECT_ID('[dbo].[sp_UI_DeletePermLabels]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeletePermLabels]
GO
CREATE PROC [dbo].[sp_UI_DeletePermLabels]
(
@iPermID int
)
AS
DELETE FROM [rtbl_PermLabels] WHERE [iPermID] = @iPermID
GO



IF (OBJECT_ID('[dbo].[sp_GetAllRoles]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetAllRoles]
GO
CREATE PROC [dbo].[sp_GetAllRoles]
AS
SELECT rh.[iRole_ID], rh.[vRole_Name], rh.[vRole_Desc], STUFF((SELECT ',' + CAST(p.[vPermission_Name] AS VARCHAR(MAX)) FROM [ltbl_userRoleLines] rl
INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rl.iPermission_ID
WHERE rh.iRole_ID = rl.[iRole_ID] AND rl.[bPermission_Active] = 1
FOR XML PATH('')),1,1,''), rh.[bRole_Active] FROM [htbl_userRoles] rh
GO



IF (OBJECT_ID('[dbo].[sp_GetActivePermissions]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetActivePermissions]
GO
CREATE PROC [dbo].[sp_GetActivePermissions]
AS
SELECT [vPermission_Name] FROM [ltbl_Module_Perms] WHERE [bPermissionActive] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetAddedRoleID]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAddedRoleID]
GO
CREATE PROC [dbo].[sp_UI_GetAddedRoleID]
(
@vRole_Name varchar(max)
)
AS
SELECT [iRole_ID] FROM [htbl_userRoles] WHERE [vRole_Name] = @vRole_Name
GO



IF (OBJECT_ID('[dbo].[sp_GetPermID]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetPermID]
GO
CREATE PROC [dbo].[sp_GetPermID]
(
@vPermission_Name varchar(max)
)
AS
SELECT [iPermission_ID]
FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @vPermission_Name
GO




IF (OBJECT_ID('[dbo].[sp_GetAllRolePerms]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetAllRolePerms]
GO
CREATE PROC [dbo].[sp_GetAllRolePerms]
(
@iRole_ID int
)
AS
SELECT p.[vPermission_Name]
,rp.[bPermission_Active]
FROM [ltbl_userRoleLines] rp 
INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rp.[iPermission_ID]
WHERE rp.[iRole_ID] = @iRole_ID
GO


IF (OBJECT_ID('[dbo].[sp_CheckRoleInUse]') IS NOT NULL)
	DROP PROC [dbo].[sp_CheckRoleInUse]
GO
CREATE PROC [dbo].[sp_CheckRoleInUse]
(
@iRole_ID int
)
AS
SELECT [iRoleID]
FROM [tbl_users] WHERE [iRoleID] = @iRole_ID
GO



IF (OBJECT_ID('[dbo].[sp_AddRoleHeader]') IS NOT NULL)
	DROP PROC [dbo].[sp_AddRoleHeader]
GO
CREATE PROC [dbo].[sp_AddRoleHeader]
(
@vRole_Name varchar(max),
@vRole_Desc varchar(max)
)
AS
INSERT INTO [htbl_userRoles] (
 [vRole_Name]
,[vRole_Desc]
,[bRole_Active]
,[dRole_Created]
)
VALUES (@vRole_Name, @vRole_Desc, 1, GETDATE())
GO



IF (OBJECT_ID('[dbo].[sp_AddRoleLine]') IS NOT NULL)
	DROP PROC [dbo].[sp_AddRoleLine]
GO
CREATE PROC [dbo].[sp_AddRoleLine]
(
@iRole_ID int,
@iPermission_ID int
)
AS
INSERT INTO [ltbl_userRoleLines] ([iRole_ID]
,[iPermission_ID]
,[bPermission_Active]
,[dPermission_Added]
)
VALUES (@iRole_ID, @iPermission_ID, 1, GETDATE())
GO



IF (OBJECT_ID('[dbo].[sp_UpdateRoleHeader]') IS NOT NULL)
	DROP PROC [dbo].[sp_UpdateRoleHeader]
GO
CREATE PROC [dbo].[sp_UpdateRoleHeader]
(
@iRole_ID int,
@vRole_Name varchar(max),
@vRole_Desc varchar(max)

)
AS
UPDATE [htbl_userRoles] SET [vRole_Name] = @vRole_Name, [vRole_Desc] = @vRole_Desc, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @iRole_ID
GO



IF (OBJECT_ID('[dbo].[sp_UpdateRolePermActive]') IS NOT NULL)
	DROP PROC [dbo].[sp_UpdateRolePermActive]
GO
CREATE PROC [dbo].[sp_UpdateRolePermActive]
(
@iRole_ID int,
@iPermission_ID int,
@bPermission_Active bit
)
AS
UPDATE [ltbl_userRoleLines] SET [bPermission_Active] = @bPermission_Active, [dPermission_Removed] =  GETDATE() WHERE [iRole_ID] = @iRole_ID AND [iPermission_ID] = @iPermission_ID
GO




IF (OBJECT_ID('[dbo].[sp_DeactivateRole]') IS NOT NULL)
	DROP PROC [dbo].[sp_DeactivateRole]
GO
CREATE PROC [dbo].[sp_DeactivateRole]
(
@iRole_ID int
)
AS
UPDATE [htbl_userRoles] SET [bRole_Active] = 0, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @iRole_ID
GO



IF (OBJECT_ID('[dbo].[sp_ActivateRole]') IS NOT NULL)
	DROP PROC [dbo].[sp_ActivateRole]
GO
CREATE PROC [dbo].[sp_ActivateRole]
(
@iRole_ID int
)
AS
UPDATE [htbl_userRoles] SET [bRole_Active] = 1, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @iRole_ID
GO



IF (OBJECT_ID('[dbo].[sp_RemoveRole]') IS NOT NULL)
	DROP PROC [dbo].[sp_RemoveRole]
GO
CREATE PROC [dbo].[sp_RemoveRole]
(
@iRole_ID int
)
AS
DELETE FROM [htbl_userRoles] WHERE [iRole_ID] = @iRole_ID
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetAllUsers]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllUsers]
GO
CREATE PROC [dbo].[sp_UI_GetAllUsers]
AS
 SELECT u.[iUser_ID]
,u.[vUser_Name]
,u.[vUser_Username]
,u.[vUser_PIN]
,u.[vUser_Password]
,r.[vRole_Name]
,u.[bUser_IsActive]
,u.[bHasAgent]
,u.[vAgentName]
FROM [tbl_users] u INNER JOIN [htbl_userRoles] r 
ON u.[iRoleID] = r.[iRole_ID]
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetUsersnames]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetUsersnames]
GO
CREATE PROC [dbo].[sp_UI_GetUsersnames]
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [bUser_IsActive] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_CheckUserLogon]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckUserLogon]
GO
CREATE PROC [dbo].[sp_UI_CheckUserLogon]
(
@vUser_Username varchar(200),
@vUser_Password varchar(50)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_Username] = @vUser_Username AND [vUser_Password] = @vUser_Password  AND [bUser_IsActive] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetActiveRoles]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetActiveRoles]
GO
CREATE PROC [dbo].[sp_UI_GetActiveRoles]
AS
SELECT [vRole_Name]
FROM [htbl_userRoles] WHERE [bRole_Active] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_CheckUsername]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckUsername]
GO
CREATE PROC [dbo].[sp_UI_CheckUsername]
(
@vUser_Username varchar(200)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_Username] = @vUser_Username
GO





IF (OBJECT_ID('[dbo].[sp_UI_CheckUserPin') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckUserPin]
GO
CREATE PROC [dbo].[sp_UI_CheckUserPin]
(
@vUser_PIN varchar(6)
)
AS
SELECT [vUser_PIN]
FROM [tbl_users] WHERE [vUser_PIN] = @vUser_PIN
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetRoleIDByName') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetRoleIDByName]
GO
CREATE PROC [dbo].[sp_UI_GetRoleIDByName]
(
@vRole_Name varchar(max)
)
AS
SELECT [iRole_ID]
FROM [htbl_userRoles] WHERE [vRole_Name] = @vRole_Name
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetRolePermsByName') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetRolePermsByName]
GO
CREATE PROC [dbo].[sp_UI_GetRolePermsByName]
(
@vRole_Name varchar(max)
)
AS
SELECT mp.[vPermission_Name] 
FROM [ltbl_userRoleLines] rl
INNER JOIN [ltbl_Module_Perms] mp ON mp.[iPermission_ID] = rl.[iPermission_ID]
INNER JOIN [htbl_userRoles] rh ON rl.[iRole_ID] = rh.[iRole_ID]
WHERE rh.[vRole_Name] = @vRole_Name AND [bPermission_Active] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetUsersname') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetUsersname]
GO
CREATE PROC [dbo].[sp_MBL_GetUsersname]
(
@vUser_PIN varchar(6)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_PIN] = @vUser_PIN
GO



IF (OBJECT_ID('[dbo].[sp_ZECT_GetUsersname') IS NOT NULL)
	DROP PROC [dbo].[sp_ZECT_GetUsersname]
GO
CREATE PROC [dbo].[sp_ZECT_GetUsersname]
(
@vUser_PIN varchar(6)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_PIN] = @vUser_PIN
GO



IF (OBJECT_ID('[dbo].[sp_AW_GetUsersname') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetUsersname]
GO
CREATE PROC [dbo].[sp_AW_GetUsersname]
(
@vUser_PIN varchar(6)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_PIN] = @vUser_PIN
GO



IF (OBJECT_ID('[dbo].[sp_Canning_GetUsersname') IS NOT NULL)
	DROP PROC [dbo].[sp_Canning_GetUsersname]
GO
CREATE PROC [dbo].[sp_Canning_GetUsersname]
(
@vUser_PIN varchar(6)
)
AS
SELECT [vUser_Username]
FROM [tbl_users] WHERE [vUser_PIN] = @vUser_PIN
GO



IF (OBJECT_ID('[dbo].[sp_UI_AddUser') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_AddUser]
GO
CREATE PROC [dbo].[sp_UI_AddUser]
(
@vUser_Name varchar(200),
@vUser_Username varchar(200),
@vUser_PIN varchar(6), 
@vUser_Password varchar(50), 
@bUser_IsActive bit,
@dUser_Created datetime, 
@iRoleID int, 
@bHasAgent bit, 
@vAgentName varchar(255)
)
AS
INSERT INTO [tbl_users] (
 [vUser_Name]
,[vUser_Username]
,[vUser_PIN]
,[vUser_Password]
,[bUser_IsActive]
,[dUser_Created]
,[iRoleID]
,[bHasAgent]
,[vAgentName]
)
VALUES (@vUser_Name, @vUser_Username, @vUser_PIN, @vUser_Password, 1, GETDATE(), @iRoleID, @bHasAgent, @vAgentName)
GO


IF (OBJECT_ID('[dbo].[sp_UI_RemoveUser') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemoveUser]
GO
CREATE PROC [dbo].[sp_UI_RemoveUser]
(
@iUser_ID int
)
AS
DELETE FROM [tbl_users] WHERE [iUser_ID] = @iUser_ID
GO



IF (OBJECT_ID('[dbo].[sp_UI_UpdateUser') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateUser]
GO
CREATE PROC [dbo].[sp_UI_UpdateUser]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 int,
@7 bit,
@8 varchar(max)

)
AS
UPDATE [tbl_users] SET [vUser_Name] = @2
,[vUser_Username] = @3
,[vUser_PIN] = @4
,[vUser_Password] = @5
,[dUser_Modified] = GETDATE()
,[iRoleID] = @6
,[bHasAgent] = @7
,[vAgentName] = @8
WHERE [iUser_ID] = @1
GO







IF (OBJECT_ID('[dbo].[sp_UI_ActivateUser') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ActivateUser]
GO
CREATE PROC [dbo].[sp_UI_ActivateUser]
(
@iUser_ID int
)
AS
UPDATE [tbl_users] SET [bUser_IsActive] = 1,[dUser_Modified] =  GETDATE() WHERE [iUser_ID] = @iUser_ID
GO



IF (OBJECT_ID('[dbo].[sp_UI_DeactivateUser') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeactivateUser]
GO
CREATE PROC [dbo].[sp_UI_DeactivateUser]
(
@iUser_ID int
)
AS
UPDATE [tbl_users] SET [bUser_IsActive] = 0,[dUser_Modified] =  GETDATE() WHERE [iUser_ID] = @iUser_ID
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



--------------------------------------------------------- PGM Planning ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[UI_GetVWPGMPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetVWPGMPlanLines]
GO

CREATE PROC [dbo].[UI_GetVWPGMPlanLines]
AS
	SELECT [iLineID], '' AS [CatalystCode], [vSlurryCode], '' AS [vPowderCode], [vPGMCode], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
	FROM [rtbl_VW_Slurry_PGM]
GO




IF (OBJECT_ID('[dbo].[UI_GetTOYOTAFSPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetTOYOTAFSPlanLines]
GO

CREATE PROC [dbo].[UI_GetTOYOTAFSPlanLines]
AS
	SELECT [iLineID], '' AS [CatalystCode], [vSlurryCode], '' AS [vPowderCode], [vPGMCode], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
	FROM [rtbl_T_Slurry_PGM]
GO




IF (OBJECT_ID('[dbo].[UI_GetTOYOTAPPPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetTOYOTAPPPlanLines]
GO

CREATE PROC [dbo].[UI_GetTOYOTAPPPlanLines]
AS
	SELECT [iLineID], '' AS [CatalystCode], '' AS [vSlurryCode], [vPowderCode], [vPGMCode], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
	FROM [rtbl_T_Powder_PGM]
GO




IF (OBJECT_ID('[dbo].[UI_GetTOYOTAPPPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetTOYOTAPPPlanLines]
GO

CREATE PROC [dbo].[UI_GetTOYOTAPPPlanLines]
AS
	SELECT [iLineID], '' AS [CatalystCode], '' AS [vSlurryCode], [vPowderCode], [vPGMCode], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] 
	FROM [rtbl_T_Powder_PGM]
GO




IF (OBJECT_ID('[dbo].[UI_GetTOYOTAAWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetTOYOTAAWPlanLines]
GO

CREATE PROC [dbo].[UI_GetTOYOTAAWPlanLines]
AS
SELECT [iLineID], [vCatalystCode], '' AS [vSlurryCode], '' AS [vPowderCode], [vPGMCode], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] FROM [rtbl_T_Catalyst_PGM]
GO




IF (OBJECT_ID('[dbo].[UI_UpdateVWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_UpdateVWPlanLines]
GO

CREATE PROC [dbo].[UI_UpdateVWPlanLines]
	@Slurry VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX),
	@ID VARCHAR(MAX)
AS
	UPDATE [rtbl_VW_Slurry_PGM] 
    SET [vSlurryCode] = @Slurry,[vPGMCode] = @PGMCode
    ,[dtDateEdit] = GETDATE(),[vUserEdit] = @User
    WHERE [iLineID] = @ID
GO




IF (OBJECT_ID('[dbo].[UI_UpdateTFSPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_UpdateTFSPlanLines]
GO

CREATE PROC [dbo].[UI_UpdateTFSPlanLines]
	@Slurry VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX),
	@ID VARCHAR(MAX)
AS
	UPDATE [rtbl_T_Slurry_PGM] 
    SET [vSlurryCode] = @Slurry,[vPGMCode] = @PGMCode
    ,[dtDateEdit] = GETDATE(),[vUserEdit] = @User
    WHERE [iLineID] = @ID
GO




IF (OBJECT_ID('[dbo].[UI_UpdateTPPPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_UpdateTPPPlanLines]
GO

CREATE PROC [dbo].[UI_UpdateTPPPlanLines]
	@Powder VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX),
	@ID VARCHAR(MAX)
AS
	UPDATE [rtbl_T_Powder_PGM] 
    SET [vPowderCode] = @Powder,[vPGMCode] = @PGMCode
    ,[dtDateEdit] = GETDATE(),[vUserEdit] = @User
    WHERE [iLineID] = @ID
GO



IF (OBJECT_ID('[dbo].[sp_UI_UpdateTAWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateTAWPlanLines]
GO

CREATE PROC [dbo].[sp_UI_UpdateTAWPlanLines]
	@Slurry VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX)
AS
INSERT INTO [rtbl_VW_Slurry_PGM]([vSlurryCode],[vPGMCode],[dtDateAdd],[vUserAdd])
VALUES(@Slurry,@PGMCode,GETDATE(),@User)
GO





IF (OBJECT_ID('[dbo].[UI_InsertTFSPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_InsertTFSPlanLines]
GO

CREATE PROC [dbo].[UI_InsertTFSPlanLines]
	@Slurry VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX)
AS
	INSERT INTO [rtbl_T_Slurry_PGM]([vSlurryCode],[vPGMCode],[dtDateAdd],[vUserAdd])
    VALUES(@Slurry,@PGMCode,GETDATE(),@User)
GO




IF (OBJECT_ID('[dbo].[UI_InsertTPPPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[UI_InsertTPPPlanLines]
GO

CREATE PROC [dbo].[UI_InsertTPPPlanLines]
	@Powder VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX)
AS
	INSERT INTO [rtbl_T_Powder_PGM]([vPowderCode],[vPGMCode],[dtDateAdd],[vUserAdd])
    VALUES(@Powder,@PGMCode,GETDATE(),@User)
GO




IF (OBJECT_ID('[dbo].[sp_UI_InsertTAWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertTAWPlanLines]
GO

CREATE PROC [dbo].[sp_UI_InsertTAWPlanLines]
	@Catalyst VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX)
AS
INSERT INTO [rtbl_T_Catalyst_PGM]([vCatalystCode],[vPGMCode],[dtDateAdd],[vUserAdd])
VALUES(@Catalyst,@PGMCode,GETDATE(),@User)
GO



IF (OBJECT_ID('[dbo].[sp_UI_InsertVWPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertVWPlanLines]
GO

CREATE PROC [dbo].[sp_UI_InsertVWPlanLines]
	@Slurry VARCHAR(MAX),
	@PGMCode VARCHAR(MAX),
	@User VARCHAR(MAX)
AS
INSERT INTO [rtbl_VW_Slurry_PGM]([vSlurryCode],[vPGMCode],[dtDateAdd],[vUserAdd])
VALUES(@Slurry,@PGMCode,GETDATE(),@User)
GO




--------------------------------------------------------- PGM Manufacture ---------------------------------------------------------------------------




IF (OBJECT_ID('[dbo].[UI_GetPGMMF]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMMF]
GO

CREATE PROC [dbo].[UI_GetPGMMF]
AS
	SELECT l.[iLineID],h.[vItemCode],h.[vLotDesc],l.[dWeightIn],l.[dtDateAdded],l.[vUserAdded]
    FROM [htbl_RTIS_PGM_Manuf] h
    LEFT JOIN [ltbl_RTIS_PGM_Manuf] l ON l.[iHeaderID] = h.[iLineID]
    WHERE (l.[bManuf] = '0' OR l.[bManuf] IS NULL)
GO




IF (OBJECT_ID('[dbo].[UI_GetPGMMF]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetPGMMF]
GO

CREATE PROC [dbo].[UI_GetPGMMF]
	@LineID INT,
	@UserName VARCHAR(MAX)
AS
	UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @UserName 
	WHERE [iLineID] = @LineID
GO




IF (OBJECT_ID('[dbo].[UI_setPGMContainerManufactured]') IS NOT NULL)
	DROP PROC [dbo].[UI_setPGMContainerManufactured]
GO

CREATE PROC [dbo].[UI_setPGMContainerManufactured]
	@headerID INT,
	@container VARCHAR(MAX),
	@UserName VARCHAR(MAX)
AS
DECLARE @res int
	UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @UserName 
	WHERE [iHeaderID] = @headerID AND [vContainer] = @container
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWhtProcesses') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtProcesses]
GO
CREATE PROC [dbo].[sp_UI_getWhtProcesses]
AS
SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames]
GO


IF (OBJECT_ID('[dbo].[sp_UI_getWhtFGRequests]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtFGRequests]
GO

CREATE PROC [dbo].[sp_UI_getWhtFGRequests]
@1 datetime,
@2 datetime
AS
SELECT fg.[iLineID], 
                    fg.[vItemCode], 
                    fg.[vLotNumber], 
                    fg.[vWarehouse_From], 
                    fg.[vWarehouse_To], 
                    fg.[dQtyTransfered], 
                    ISNULL(lq.[fQtyOnHand], 0) AS [QtyOnHand],
                    fg.[vUsername],
                    fg.[vProcess], 
                    fg.[dtDateTransfered],
                    CASE 
                    WHEN l.[cLotDescription] IS NULL THEN
	                    'The lot was not found in evolution, has the lot been manufactured.'		
                    WHEN lq.[fQtyOnHand] < fg.[dQtyTransfered] THEN
	                    'Insufficient quantity of this lot was found in the out going warehouse.'			
                    ELSE
	                    ''
                    END  AS [Warnings]
                    FROM [tbl_WHTFGRequests] fg
                    INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = fg.[vItemCode]
                    INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[Code] = fg.[vWarehouse_From]
                    LEFT JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l ON l.[cLotDescription] = fg.[vLotNumber]
                    LEFT JOIN   [Cataler_SCN].[dbo].[_etblLotTrackingQty] lq ON l.[idLotTracking] = lq.[iLotTrackingID] AND lq.[iWarehouseID] = w.[WhseLink] WHERE [dtDateTransfered] BETWEEN @1 AND @2 
                    ORDER BY [dtDateTransfered] DESC
GO



IF (OBJECT_ID('[dbo].[sp_UI_getWhtFGReports]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhtFGReports]
GO

CREATE PROC [dbo].[sp_UI_getWhtFGReports]
@1 datetime,
@2 datetime
AS
SELECT [iLineID]
      ,[vItemCode]
      ,[vLotNumber]
      ,[vWarehouse_From]
      ,[vWarehouse_To]
      ,[dQtyTransfered]
      ,[dtDateRequested]
      ,[vUserRequested]
      ,[dtDateTransfered]
      ,[vUsername]
      ,[vProcess] FROM [tbl_WHTFGRequests_Completed] WHERE [dtDateTransfered] BETWEEN @1 AND @2
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetFGTransferInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetFGTransferInfo]
GO

CREATE PROC [dbo].[sp_UI_GetFGTransferInfo]
@1 int
AS
SELECT [vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateTransfered], [vProcess] ,[vUsername]
FROM [tbl_WHTFGRequests] WHERE [iLineID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_SVC_GetPendingWhseTransfers]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_GetPendingWhseTransfers]
GO

CREATE PROC [dbo].[sp_SVC_GetPendingWhseTransfers]
AS
SELECT TOP 1000 [iLineID], [vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [vUsername] ,[vProcess] ,[vTransDesc], [dtDateTransfered] 
FROM [tbl_WHTPending] WHERE [vStatus] = 'Pending'
GO


IF (OBJECT_ID('[dbo].[sp_SVC_UpdateWhseTransferFailed]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_UpdateWhseTransferFailed]
GO

CREATE PROC [dbo].[sp_SVC_UpdateWhseTransferFailed]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [tbl_WHTPending] SET [vStatus] = 'Failed', [vFailureReason] = @2, [dtDateFailed] = GETDATE()
WHERE [iLineID] = @1
GO



IF (OBJECT_ID('[dbo].[sp_SVC_UpdateWhseTransferLine]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_UpdateWhseTransferLine]
GO

CREATE PROC [dbo].[sp_SVC_UpdateWhseTransferLine]
(
@1 varchar(max),
@2 decimal(16,3),
@3 varchar(max),
@4 int
)
AS
UPDATE [tbl_WHTPending] SET [vLotNumber] = @1, [dQtyTransfered] = @2, [vStatus] = @3 WHERE [iLineID] = @4
GO




IF (OBJECT_ID('[dbo].[sp_SVC_UpdateWhseTransferLinePending]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_UpdateWhseTransferLinePending]
GO

CREATE PROC [dbo].[sp_SVC_UpdateWhseTransferLinePending]
(
@1 varchar(max),
@2 decimal(16,3),
@3 varchar(max),
@4 int
)
AS
UPDATE [tbl_WHTPending] SET [vLotNumber] = @1, [dQtyTransfered] = @2, [vStatus] = @3, [vFailureReason] = NULL, [dtDateFailed] = NULL WHERE [iLineID] = @4
GO


IF (OBJECT_ID('[dbo].[sp_SVC_UpdateWhseTransfersFailedToPending]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_UpdateWhseTransfersFailedToPending]
GO

CREATE PROC [dbo].[sp_SVC_UpdateWhseTransfersFailedToPending]
AS
UPDATE [tbl_WHTPending] SET [vStatus] = 'Pending', [vFailureReason] = NULL, [dtDateFailed] = NULL WHERE [vStatus] = 'Failed'
GO


IF (OBJECT_ID('[dbo].[sp_SVC_InsertWHTLineCompleted]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_InsertWHTLineCompleted]
GO

CREATE PROC [dbo].[sp_SVC_InsertWHTLineCompleted]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 decimal(16,3),
@7 datetime,
@8 varchar(max),
@9 varchar(max),
@10 varchar(max)
)
AS
INSERT INTO [tbl_WHTCompleted] 
([iLineID], [vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateEntered], [dtDateTransfered], [vUsername], [vProcess], [vTransDesc]) 
VALUES 
(@1, @2, @3, @4, @5, @6, @7, GETDATE(), @8, @9, @10)
GO



IF (OBJECT_ID('[dbo].[sp_SVC_InsertFGWHTLineCompleted]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_InsertFGWHTLineCompleted]
GO

CREATE PROC [dbo].[sp_SVC_InsertFGWHTLineCompleted]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 decimal(16,3),
@7 datetime,
@8 varchar(max),
@9 varchar(max),
@10 varchar(max),
@11 varchar(max)
)
AS
INSERT INTO [tbl_WHTFGRequests_Completed] 
([iLineID], [vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateRequested], [vUserRequested], [dtDateTransfered], [vUsername], [vProcess], [vStatus])
VALUES
(@1, @2, @3, @4, @5, @6, @7, @8, GETDATE(), @9, @10, @11)
GO




IF (OBJECT_ID('[dbo].[sp_UI_DeleteFGWhseTransLineComplete]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteFGWhseTransLineComplete]
GO

CREATE PROC [dbo].[sp_UI_DeleteFGWhseTransLineComplete]
(
@1 int
)
AS
DELETE FROM [tbl_WHTFGRequests]  WHERE [iLineID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_SVC_DeleteWhseTransLineComplete]') IS NOT NULL)
	DROP PROC [dbo].[sp_SVC_DeleteWhseTransLineComplete]
GO

CREATE PROC [dbo].[sp_SVC_DeleteWhseTransLineComplete]
(
@1 int
)
AS
DELETE FROM [tbl_WHTPending]  WHERE [iLineID] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetRecWhse]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetRecWhse]
GO

CREATE PROC [dbo].[sp_MBL_GetRecWhse]
(
@1 varchar(60)
)
AS
SELECT w.[Code], w.[Name] FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhseID]
WHERE [vProcessName] = @1 AND [bIsRec] = 1
GO

IF (OBJECT_ID('[dbo].[sp_MBL_GetPPRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPPRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetPPRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetFSRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFSRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetFSRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetMSRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMSRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetMSRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetZect1RecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZect1RecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetZect1RecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetZect2RecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZect2RecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetZect2RecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetAWRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetAWRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetAWRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetCanningRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetCanningRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetCanningRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetPGMReceivingWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPGMReceivingWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetPGMReceivingWhses]
AS
SELECT w.[Code], w.[Name] FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhseID]
WHERE [vProcessName] = 'PGM' AND [bIsRec] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPGMRecWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPGMRecWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetPGMRecWhses]
AS
SELECT w.[Code], w.[Name] FROM [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetSlurryPowders]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryPowders]
GO

CREATE PROC [dbo].[sp_MBL_GetSlurryPowders]
(
@1 varchar(max)
)
AS
SELECT [vPowderCode] FROM [rtbl_Slurry_Powders] WHERE [vSlurryCode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPowderPrepWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPowderPrepWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetPowderPrepWhes]
AS
SELECT w.[Code], w.[Name] FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhseID]
WHERE [vProcessName] = 'Powder Prep' AND [bIsRec] = 0
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetPowderPrepWhes_OLD]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPowderPrepWhes_OLD]
GO

CREATE PROC [dbo].[sp_MBL_GetPowderPrepWhes_OLD]
AS
SELECT w.[Code] FROM [RTIS_WarehouseLookUp_PPtFS] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetFreshSlurryWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryWhes]
AS
SELECT w.[Code], w.[Name] FROM [tbl_WHTLocations] wl 
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhseID]
WHERE [vProcessName] = 'Fresh Slurry' AND [bIsRec] = 0
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetMixedSlurryWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetMixedSlurryWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetMixedSlurryWhes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_MStZect] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE w.[Code] NOT IN (SELECT w.[Code] 
FROM [RTIS_WarehouseLookUp_MStZect] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE wl.[iWhse_Link] !=w.WhseLink) AND wl.bEnabled=1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetToProdWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetToProdWhses]
GO

CREATE PROC [dbo].[sp_MBL_GetToProdWhses]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_ToProd] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetZect1Whes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZect1Whes]
GO

CREATE PROC [dbo].[sp_MBL_GetZect1Whes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_Zect1] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetZect2Whes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZect2Whes]
GO

CREATE PROC [dbo].[sp_MBL_GetZect2Whes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_Zect2] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetAWWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetAWWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetAWWhes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_AW] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetCanningWhes]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetCanningWhes]
GO

CREATE PROC [dbo].[sp_MBL_GetCanningWhes]
AS
SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_Canning] wl
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
WHERE [bEnabled] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_CheckPowderManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckPowderManufactured]
GO

CREATE PROC [dbo].[sp_MBL_CheckPowderManufactured]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT [bManufactured] FROM [tbl_RTIS_Powder_Prep] 
WHERE [vItemCode] = @1 AND [vLotDesc] = @2
GO



IF (OBJECT_ID('[dbo].[sp_MBL_CheckPowderReceieved]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckPowderReceieved]
GO

CREATE PROC [dbo].[sp_MBL_CheckPowderReceieved]
(
@1 varchar(max),
@2 varchar(max),
@3 decimal(16,3)
)
AS
SELECT ISNULL([bRecTrans], 'False') FROM [tbl_RTIS_Powder_Prep] 
WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [dQty] = @3
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetFreshSlurryManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryManufactured]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryManufactured]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT  TOP 1 ISNULL([bManuf], 0) FROM [tbl_RTIS_Fresh_Slurry] 
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetZectManufactured]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZectManufactured]
GO

CREATE PROC [dbo].[sp_MBL_GetZectManufactured]
(
@1 int
)
AS
SELECT ISNULL([bManuf], 0) FROM [tbl_RTIS_Zect]
WHERE [iLineID] = @1
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetZectTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetZectTransferred]
GO

CREATE PROC [dbo].[sp_MBL_GetZectTransferred]
(
@1 int
)
AS
SELECT ISNULL([bTrans], 0) FROM [tbl_RTIS_Zect]
WHERE [iLineID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetFreshSlurryTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetFreshSlurryTransferred]
GO

CREATE PROC [dbo].[sp_MBL_GetFreshSlurryTransferred]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 ISNULL([bTrans], 0) FROM [tbl_RTIS_Fresh_Slurry] 
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetSlurryTransferInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSlurryTransferInfo]
GO
CREATE PROC [dbo].[sp_MBL_GetSlurryTransferInfo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT [vLotNumber], [dDryWeight], s.[Description_1] FROM [tbl_RTIS_Fresh_Slurry] fs
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[Code] = [vItemCode]
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND ([bTrans] = 0 OR [bTrans] IS NULL) 
GO


IF (OBJECT_ID('[dbo].[sp_UI_InsertWhseTransfer]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertWhseTransfer]
GO
CREATE PROC [dbo].[sp_UI_InsertWhseTransfer]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3),
@6 varchar(max),
@7 varchar(max),
@8 varchar(max),
@9 varchar(max)
)
AS
INSERT INTO [tbl_WHTPending]
([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateTransfered], [vUsername], [vProcess], [vTransDesc], [vStatus])
VALUES
(@1, @2, @3, @4, @5, GETDATE(), @6, @7, @8, @9) 
GO


IF (OBJECT_ID('[dbo].[sp_UI_InsertFGWhseTransfer]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertFGWhseTransfer]
GO
CREATE PROC [dbo].[sp_UI_InsertFGWhseTransfer]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3),
@6 varchar(max),
@7 varchar(max)
)
AS
INSERT INTO [tbl_WHTFGRequests]
([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateTransfered], [vUsername], [vProcess])
VALUES
(@1, @2, @3, @4, CONVERT(DECIMAL,@5), GETDATE(), @6, @7) 
GO



IF (OBJECT_ID('[dbo].[sp_UI_whtTransferLog]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_whtTransferLog]
GO
CREATE PROC [dbo].[sp_UI_whtTransferLog]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3),
@6 varchar(max),
@7 varchar(max)
)
AS
INSERT INTO [stbl_WHTLog] ([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [vUsername], [vProcess], [dtDateTransfered])
VALUES (@1, @2, @3, @4, CONVERT(DECIMAL,@5), @6, @7, GETDATE())
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePowderTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePowderTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePowderTransferred]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)

)
AS
UPDATE [tbl_RTIS_Powder_Prep] SET [bTransfered] = 1, [vUserTrans] = @3, [dtTransDate] = GETDATE()  WHERE [vItemCode] = @1 AND [vLotDesc] = @2
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePowderTransferredIn]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePowderTransferredIn]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePowderTransferredIn]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 decimal(16,3)
)
AS
UPDATE [tbl_RTIS_Powder_Prep] SET [bRecTrans] = 1, [vUserRec] = @3, [dtRecTrans] = GETDATE()  WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [dQty] = @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateSlurryTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateSlurryTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdateSlurryTransferred]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
UPDATE [tbl_RTIS_Fresh_Slurry] SET [bTrans] = 1, [dtTrans] = GETDATE() 
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 AND [bManuf] = 1 AND ([bTrans] = 0 OR [bTrans] IS NULL)
GO




IF (OBJECT_ID('[dbo].[sp_MBL_UpdateMobileTankTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateMobileTankTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdateMobileTankTransferred]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_RTIS_MS_Decant] SET [bTransferred] = 1, [vUserTransferred] = @4, [dtTransferred] = GETDATE()
WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 AND [iLineID] = (
SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Decant] WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 ORDER BY [iLineID] DESC)
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateMobileTankReceived]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateMobileTankReceived]
GO
CREATE PROC [dbo].[sp_MBL_UpdateMobileTankReceived]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_RTIS_MS_Decant] SET [bReceived] = 1, [vUserReceived] = @4, [dtReceived] = GETDATE()
WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Decant] WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 ORDER BY [iLineID] DESC)
GO




IF (OBJECT_ID('[dbo].[sp_MBL_UpdateLargeTankTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateLargeTankTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdateLargeTankTransferred]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [tbl_RTIS_MS_Main] SET [bTransferred] = 1, [vUserTransferred] = @5, [dtTransferred] = GETDATE()
WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4
AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4 ORDER BY [iLineID] DESC)
GO





IF (OBJECT_ID('[dbo].[sp_MBL_UpdateLargeTankTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateLargeTankTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdateLargeTankTransferred]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [tbl_RTIS_MS_Main] SET [bTransferred] = 1, [vUserTransferred] = @5, [dtTransferred] = GETDATE()
WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4
AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4 ORDER BY [iLineID] DESC)
GO



IF (OBJECT_ID('[dbo].[sp_MBL_setFreshSlurryReceived]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_setFreshSlurryReceived]
GO
CREATE PROC [dbo].[sp_MBL_setFreshSlurryReceived]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_RTIS_Fresh_Slurry] SET [bRecTrans] = 1, [dtRecTrans] = GETDATE(), [vUserRec] = @4
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 
AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 ORDER BY [iLineID] DESC)
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetCatalystRaws]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetCatalystRaws]
GO
CREATE PROC [dbo].[sp_UI_GetCatalystRaws]
(
@1 varchar(max)
)
AS
SELECT [vRMCode], [vRMDesc], '' FROM [tbl_RTIS_Zect_Raws] WHERE [vCatalystCode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetZeckLinkExists]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZeckLinkExists]
GO
CREATE PROC [dbo].[sp_UI_GetZeckLinkExists]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT [vRMCode] FROM [tbl_RTIS_Zect_Raws] WHERE [vCatalystCode] = @1 AND [vRMCode] = @2
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAllZECTJobs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllZECTJobs]
GO
CREATE PROC [dbo].[sp_UI_GetAllZECTJobs]
(
@1 datetime,
@2 datetime
)
AS
SELECT [iLIneID], [vJobUnq], [vCatalystCode], [vLotNumber], [dQty], [dQtyManuf], [dtStarted], [vZectLine], [bJobRunning] ,[vUserStarted] ,[dtStopped] ,[vUserStopped] ,[dtSReopened] ,[vUserReopened]
FROM [tbl_RTIS_Zect_Jobs] WHERE [dtStarted] BETWEEN @1 AND @2
ORDER BY [dtStarted] DESC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GeZECTJobInPuts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GeZECTJobInPuts]
GO
CREATE PROC [dbo].[sp_UI_GeZECTJobInPuts]
(
@1 int
)
AS
SELECT [vSlurryCode], [vSlurryLot], [dQty], [dtDateRecorded], [vUserRecorded]
FROM [tbl_RTIS_Zect_Input]
WHERE [iJobID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GeZECTJobOutPuts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GeZECTJobOutPuts]
GO
CREATE PROC [dbo].[sp_UI_GeZECTJobOutPuts]
(
@1 int
)
AS
SELECT [vPalletNo], [dQty], [vUserRecorded], [dtDateRecorded],ISNULL([bManuf], 0),[dtDateManuf],[vUserManuf]
FROM [tbl_RTIS_Zect_OutPut] WHERE [iJobID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GeZECTJobsToManufacture]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GeZECTJobsToManufacture]
GO
CREATE PROC [dbo].[sp_UI_GeZECTJobsToManufacture]
AS
SELECT zj.[iLIneID], zj.[vJobUnq], zj.[vCatalystCode], zj.[vLotNumber], zj.[dQty], zj.[dQtyManuf], SUM(zo.dQty) , zj.[vCoat], zj.[vZectLine], zj.[bJobRunning],[dtStarted] FROM [tbl_RTIS_Zect_Jobs] zj
  INNER JOIN [tbl_RTIS_Zect_OutPut] zo ON zo.[iJobID] = zj.[iLIneID]
  WHERE ISNULL(zo.[bManuf], 0) = 0 GROUP BY zj.[vJobUnq], zj.[vCatalystCode], zj.[vLotNumber], zj.[dQty], zj.[dQtyManuf] , zj.[vCoat], zj.[vZectLine], zj.[bJobRunning],[dtStarted],zj.[iLIneID]
GO




IF (OBJECT_ID('[dbo].[sp_UI_GeZECTPalletsToManufacture]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GeZECTPalletsToManufacture]
GO
CREATE PROC [dbo].[sp_UI_GeZECTPalletsToManufacture]
(
@1 int
)
AS
SELECT [iLIneID], [vPalletCode], [vPalletNo], [dQty], [dtDateRecorded], [vUserRecorded], '' FROM [tbl_RTIS_Zect_OutPut] WHERE [iJobID] = @1 AND ISNULL([bManuf], 0) = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetZECTBatchTotal]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECTBatchTotal]
GO
CREATE PROC [dbo].[sp_UI_GetZECTBatchTotal]
(
@1 int
)
AS
SELECT SUM([dQty]) AS [Total] FROM [tbl_RTIS_Zect_OutPut]
WHERE ISNULL([bManuf], 0) = 0 AND [iJobID] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetZECTRawMaterials]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetZECTRawMaterials]
GO
CREATE PROC [dbo].[sp_UI_GetZECTRawMaterials]
(
@1 int
)
AS
SELECT [vSlurryCode], [vSlurryLot] FROM [tbl_RTIS_Zect_Input] WHERE [iJobID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetCatalystSlurries]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetCatalystSlurries]
GO
CREATE PROC [dbo].[sp_Zect_GetCatalystSlurries]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT DISTINCT [vRMCode] FROM [tbl_RTIS_Zect_Raws]
WHERE [vCatalystCode] LIKE @1 AND [vCatalystCode] LIKE @2 AND ([vRMCode] LIKE 'TSP%' OR [vRMCode] LIKE 'VSP%')
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetInsertedLineID]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetInsertedLineID]
GO
CREATE PROC [dbo].[sp_Zect_GetInsertedLineID]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max)
)
AS
SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Zect]
WHERE [vCatalyst] = @1 AND [vItemCode] = @2 AND vLotNumber = @3 AND [vSlurry] = @4 AND [vCoatNum] = @5 AND [vZectLine] = @6 AND (bPrinted = 0 OR bPrinted IS NULL)
ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_Zect_CheckJobOnLine]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_CheckJobOnLine]
GO
CREATE PROC [dbo].[sp_Zect_CheckJobOnLine]
(
@1 varchar(max)
)
AS
SELECT [vJobUnq] FROM [tbl_RTIS_Zect_Jobs] WHERE [bJobRunning] = 1 AND [vZectLine] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetJobID]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobID]
GO
CREATE PROC [dbo].[sp_Zect_GetJobID]
(
@1 varchar(max)
)
AS
SELECT [iLIneID] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq]  = @1
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetJobOpenInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobOpenInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetJobOpenInfo]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode], [vLotNumber], [vSlurryCode], [vCoat], [bJobRunning] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetJobOpen]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobOpen]
GO
CREATE PROC [dbo].[sp_Zect_GetJobOpen]
(
@1 varchar(max)
)
AS
SELECT [bJobRunning] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetLineJobOpen]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetLineJobOpen]
GO
CREATE PROC [dbo].[sp_Zect_GetLineJobOpen]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT [bJobRunning] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1 AND [vZectLine] = @2
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetJobInformation]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobInformation]
GO
CREATE PROC [dbo].[sp_Zect_GetJobInformation]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode], [vLotNumber], [vCoat], [dQty], ISNULL([dQtyManuf], 0) FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetLastJobPallet]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetLastJobPallet]
GO
CREATE PROC [dbo].[sp_Zect_GetLastJobPallet]
(
@1 varchar(max)
)
AS
SELECT TOP 1 [vPalletCode] FROM [tbl_RTIS_Zect_OutPut] WHERE [iJobID] = @1 ORDER BY [iLIneID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetJobPallets]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobPallets]
GO
CREATE PROC [dbo].[sp_Zect_GetJobPallets]
(
@1 varchar(max)
)
AS
SELECT zo.[vPalletNo], zo.[vPalletCode], zj.[vCatalystCode], zj.[vLotNumber], zo.[dQty] FROM [tbl_RTIS_Zect_OutPut] zo
INNER JOIN [tbl_RTIS_Zect_Jobs] zj ON zj.[iLIneID] = zo.[iJobID] WHERE zj.[vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetUerPermissions]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetUerPermissions]
GO
CREATE PROC [dbo].[sp_Zect_GetUerPermissions]
(
@1 varchar(6)
)
AS
SELECT p.[vPermission_Name] FROM [tbl_users] u
INNER JOIN [htbl_userRoles] rh ON u.[iRoleID] = rh.[iRole_ID]
INNER JOIN [ltbl_userRoleLines] rl ON rl.[iRole_ID] = rh.[iRole_ID]
INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rl.[iPermission_ID]
WHERE u.[vUser_PIN] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetUerPermissionsReOpen]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetUerPermissionsReOpen]
GO
CREATE PROC [dbo].[sp_Zect_GetUerPermissionsReOpen]
(
@1 varchar(6)
)
AS
SELECT p.[vPermission_Name] FROM [tbl_users] u
INNER JOIN [htbl_userRoles] rh ON u.[iRoleID] = rh.[iRole_ID]
INNER JOIN [ltbl_userRoleLines] rl ON rl.[iRole_ID] = rh.[iRole_ID]
INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rl.[iPermission_ID]
WHERE u.[vUser_PIN] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetClosingJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetClosingJobInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetClosingJobInfo]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode], [vLotNumber], [vCoat], [dQty], [dQtyManuf] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetReOpenJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReOpenJobInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetReOpenJobInfo]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode], [vLotNumber], [vCoat], [dQty], [dQtyManuf] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetReprintJobInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReprintJobInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetReprintJobInfo]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode], [vLotNumber], [vCoat], [dQty], [dQtyManuf] FROM [tbl_RTIS_Zect_Jobs] WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetValidJobLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetValidJobLots]
GO
CREATE PROC [dbo].[sp_Zect_GetValidJobLots]
(
@1 varchar(max),
@2 datetime
)
AS
SELECT [vLotNumber] froM [tbl_RTIS_Zect_Jobs] WHERE [dtStarted] >= DATEADD(DAY, -@2, GETDATE()) AND [vCatalystCode] = @1
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetValidReprintJobLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetValidReprintJobLots]
GO
CREATE PROC [dbo].[sp_Zect_GetValidReprintJobLots]
(
@1 varchar(max),
@2 datetime
)
AS
SELECT [vLotNumber] froM [tbl_RTIS_Zect_Jobs] WHERE [dtStarted] >= DATEADD(DAY, -@2, GETDATE()) AND [vCatalystCode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetJobNumber]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetJobNumber]
GO
CREATE PROC [dbo].[sp_Zect_GetJobNumber]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELECT [vJobUnq] froM [tbl_RTIS_Zect_Jobs] WHERE [vCatalystCode] = @1 AND [vLotNumber] = @2 AND [vCoat] = @3 AND [vZectLine] = @4
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetReprintJobNumber]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReprintJobNumber]
GO
CREATE PROC [dbo].[sp_Zect_GetReprintJobNumber]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELECT [vJobUnq] froM [tbl_RTIS_Zect_Jobs] WHERE [vCatalystCode] = @1 AND [vLotNumber] = @2 AND [vCoat] = @3 AND [vZectLine] = @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetJobOutPutItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetJobOutPutItem]
GO
CREATE PROC [dbo].[sp_MBL_GetJobOutPutItem]
(
@1 varchar(max)
)
AS
SELECT [vCatalystCode]
FROM [tbl_RTIS_Zect_Jobs]
WHERE [vJobUnq] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_checkItemRM]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_checkItemRM]
GO
CREATE PROC [dbo].[sp_MBL_checkItemRM]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT [vRMCode] FROM [tbl_RTIS_Zect_Raws] WHERE [vCatalystCode] = @1 AND [vRMCode] = @2
GO



IF (OBJECT_ID('[dbo].[sp_Zect_OpenJobOnLine]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_OpenJobOnLine]
GO
CREATE PROC [dbo].[sp_Zect_OpenJobOnLine]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 decimal(16,3),
@7 varchar(max),
@8 varchar(max),
@9 varchar(max),
@10 varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Zect_Jobs] ([vJobUnq], [vCatalystCode], [vLotNumber], [vSlurryCode], [vSlurryLot], [dQty], [vCoat], [vZectLine], [vUserStarted], [dtStarted], [bJobRunning], [vScanSheet])
VALUES (@1, @2, @3, @4, @5, @6, @7, @8, @9, GETDATE(), 1, @10)
GO


IF (OBJECT_ID('[dbo].[sp_Zect_LogRM]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_LogRM]
GO
CREATE PROC [dbo].[sp_Zect_LogRM]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 decimal(16,3),
@5 varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Zect_Input] ([iJobID], [vSlurryCode], [vSlurryLot], [dQty], [dtDateRecorded], [vUserRecorded])
VALUES (@1, @2, @3, @4, GETDATE(), @5)
GO




IF (OBJECT_ID('[dbo].[sp_Zect_ManufacturePalletUnprinted]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_ManufacturePalletUnprinted]
GO
CREATE PROC [dbo].[sp_Zect_ManufacturePalletUnprinted]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max),
@7 decimal(16,3),
@8 varchar(max),
@9 varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Zect] ([vCatalyst], [vItemCode], [vItemDesc], [vLotNumber], [vCoatNum], [vSlurry], [dPalletQty], [vZectLine], [dtDateEntered], [vUserEntered])
VALUES (@1, @2, @3, @4, @5, @6, @7, @8, GETDATE(), @9)
GO




IF (OBJECT_ID('[dbo].[sp_Zect_AddNewPallet]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_AddNewPallet]
GO
CREATE PROC [dbo].[sp_Zect_AddNewPallet]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 decimal(16,3),
@5 varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Zect_OutPut] ([iJobID], [vPalletCode], [vPalletNo], [dQty], [vUserRecorded], [dtDateRecorded])
VALUES (@1, @2, @3, @4, @5, GETDATE())
GO


IF (OBJECT_ID('[dbo].[sp_UI_InsertRMLink]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertRMLink]
GO
CREATE PROC [dbo].[sp_UI_InsertRMLink]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
INSERT INTO [tbl_RTIS_Zect_Raws] ([vCatalystCode], [vRMCode], [vRMDesc], [vUserAdded], [dtDateAdded])
VALUES (@1, @2, @3, @4, GETDATE())
GO


IF (OBJECT_ID('[dbo].[sp_ZECT_UpdateManufacturedQty]') IS NOT NULL)
	DROP PROC [dbo].[sp_ZECT_UpdateManufacturedQty]
GO
CREATE PROC [dbo].[sp_ZECT_UpdateManufacturedQty]
(
@1 varchar(max),
@2 decimal(16,3)
)
AS
UPDATE [tbl_RTIS_Zect_Jobs] SET [dQtyManuf] = ISNULL([dQtyManuf], 0) + @2
WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_ZECT_UpdateJobClosed]') IS NOT NULL)
	DROP PROC [dbo].[sp_ZECT_UpdateJobClosed]
GO
CREATE PROC [dbo].[sp_ZECT_UpdateJobClosed]
(
@1 varchar(max),
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect_Jobs] SET [bJobRunning] =0, [vUserStopped] = @2, [dtStopped] = GETDATE()
WHERE [vJobUnq] = @1
GO


IF (OBJECT_ID('[dbo].[sp_ZECT_UpdateJobReOpened]') IS NOT NULL)
	DROP PROC [dbo].[sp_ZECT_UpdateJobReOpened]
GO
CREATE PROC [dbo].[sp_ZECT_UpdateJobReOpened]
(
@1 varchar(max),
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect_Jobs] SET [bJobRunning] =1, [vUserReopened] = @2, [dtSReopened] = GETDATE()
WHERE [vJobUnq] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateZectTransferred]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateZectTransferred]
GO
CREATE PROC [dbo].[sp_MBL_UpdateZectTransferred]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect] SET [bTrans] = 1, [dtTrans]= GETDATE(), [vUserTrans] = @2
WHERE [iLineID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_UpdateZectLine_Printed') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_UpdateZectLine_Printed]
GO
CREATE PROC [dbo].[sp_Zect_UpdateZectLine_Printed]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect] SET [bPrinted] = 1, [dtPrinted] = GETDATE(), [vUserPrinted] = @2
WHERE [iLineID] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdatePalletManufactured') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdatePalletManufactured]
GO
CREATE PROC [dbo].[sp_UI_UpdatePalletManufactured]
(
@1 int,
@2 int,
@3 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect_OutPut] SET [bManuf] = 1, [dtDateManuf] = GETDATE(), [vUserManuf] = @3
WHERE [iLIneID] = @1 AND [iJobID] = @2
GO



IF (OBJECT_ID('[dbo].[sp_UI_setZECTBatchManufactured') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setZECTBatchManufactured]
GO
CREATE PROC [dbo].[sp_UI_setZECTBatchManufactured]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect_OutPut] SET [bManuf] = '1', [dtDateManuf] = GETDATE(), [vUserManuf] = @2 WHERE [iJobID] = @1 AND ISNULL( [bManuf] , 0) = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_setZECTBatchManufacturedManual') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_setZECTBatchManufacturedManual]
GO
CREATE PROC [dbo].[sp_UI_setZECTBatchManufacturedManual]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [tbl_RTIS_Zect_OutPut] SET [bManuf] = '1', [dtManufDateManual] = GETDATE(), [vUserManufManual] = @2 WHERE [iJobID] = @1 AND ISNULL( [bManuf] , 0) = 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_DeleteRMLink') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_DeleteRMLink]
GO
CREATE PROC [dbo].[sp_UI_DeleteRMLink]
(
@1 varchar(max),
@2 varchar(max)
)
AS
DELETE FROM [tbl_RTIS_Zect_Raws] WHERE [vCatalystCode] = @1 AND [vRMCode] = @2
GO



IF (OBJECT_ID('[dbo].[sp_PGM_GetBarcodeFromVault') IS NOT NULL)
	DROP PROC [dbo].[sp_PGM_GetBarcodeFromVault]
GO
CREATE PROC [dbo].[sp_PGM_GetBarcodeFromVault]
(
@1 varchar(max)
)
AS
SELEcT ISNULL([bFromVault], 'False') FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] = @1



IF (OBJECT_ID('[dbo].[sp_MBL_GetItemReceivedTransfer') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemReceivedTransfer]
GO
CREATE PROC [dbo].[sp_MBL_GetItemReceivedTransfer]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELEcT ISNULL([bTransferredIn], 'False') FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetItemConsumed') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemConsumed]
GO
CREATE PROC [dbo].[sp_MBL_GetItemConsumed]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELEcT ISNULL([bConsumed], 'False') FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetItemDispatched') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemDispatched]
GO
CREATE PROC [dbo].[sp_MBL_GetItemDispatched]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELEcT [Dispatch] FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletID') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletID]
GO
CREATE PROC [dbo].[sp_MBL_GetPalletID]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELEcT [iLine_ID] FROM [htbl_PalletBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletDispatched') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletDispatched]
GO
CREATE PROC [dbo].[sp_MBL_GetPalletDispatched]
(
@1 int
)
AS
SELEcT [Dispatch] FROM [htbl_PalletBarcodes]
WHERE [iLine_ID] = @1
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletBoxBarcodes') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletBoxBarcodes]
GO
CREATE PROC [dbo].[sp_MBL_GetPalletBoxBarcodes]
(
@1 int
)
AS
SELEcT [vUnqBarcode] FROM [ltbl_PalletBarcodes]
WHERE [iPallet_ID] = @1
GO




IF (OBJECT_ID('[dbo].[sp_MBL_GetItemTransferredOut') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemTransferredOut]
GO
CREATE PROC [dbo].[sp_MBL_GetItemTransferredOut]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELEcT ISNULL([bTransferredOut], 'false') FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_ZECT_GetZectUnq') IS NOT NULL)
	DROP PROC [dbo].[sp_ZECT_GetZectUnq]
GO
CREATE PROC [dbo].[sp_ZECT_GetZectUnq]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELEcT [vUnqBarcode] FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetPalletID') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPalletID]
GO
CREATE PROC [dbo].[sp_UI_GetPalletID]
(
@1 varchar(max)
)
AS
SELEcT [iLine_ID] FROM [htbl_PalletBarcodes]
WHERE [vUnqBarcode] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetSTUnqs') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSTUnqs]
GO
CREATE PROC [dbo].[sp_MBL_GetSTUnqs]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELECT ISNULL([StockTake], ''), ISNULL([StockTake2], '') FROM [tbl_unqBarcodes]
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO





IF (OBJECT_ID('[dbo].[sp_MBL_GetSTPalletLots') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSTPalletLots]
GO
CREATE PROC [dbo].[sp_MBL_GetSTPalletLots]
(
@UNQ varchar(max)
)
AS
SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
WHERE h.[vUnqBarcode] = @UNQ
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetSTPalletUnqs') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSTPalletUnqs]
GO
CREATE PROC [dbo].[sp_MBL_GetSTPalletUnqs]
(
@1 varchar(max)
)
AS
SELECT ISNULL([StockTake], ''), ISNULL([StockTake2], '') FROM [htbl_PalletBarcodes]
WHERE [vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetItemOnPallet') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemOnPallet]
GO
CREATE PROC [dbo].[sp_UI_GetItemOnPallet]
(
@UNQ varchar(max)
)
AS
SELECT [iLine_ID] FROM [ltbl_PalletBarcodes] WHERE [vUnqBarcode] = @UNQ AND [bOnPallet] = 1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletContents') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletContents]
GO
CREATE PROC [dbo].[sp_MBL_GetPalletContents]
(
@UNQ varchar(max)
)
AS
SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l 
INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
WHERE h.[vUnqBarcode] = @UNQ
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPalletLots') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPalletLots]
GO
CREATE PROC [dbo].[sp_MBL_GetPalletLots]
(
@UNQ varchar(max)
)
AS
SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l 
INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
WHERE h.[vUnqBarcode] = @UNQ AND l.[bOnPallet] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_SaveRT2DBarcode') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_SaveRT2DBarcode]
GO
CREATE PROC [dbo].[sp_UI_SaveRT2DBarcode]
(
@1 varchar(max),
@2 varchar(max)
)
AS
INSERT INTO [tbl_unqBarcodes] (
[vUnqBarcode]
,[Receive]
,[Issue]
,[StockTake]
,[CycleCount]
,[Manuf]
,[Dispatch]
,[Printed]
,[DispatchDate]
,[Quarantine]
,[bValidated]
,[ValidateRef])
VALUES (@1, NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(), NULL, NULL, 0, @2)
GO



IF (OBJECT_ID('[dbo].[sp_UI_SaveRT2DBarcodePallet') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_SaveRT2DBarcodePallet]
GO
CREATE PROC [dbo].[sp_UI_SaveRT2DBarcodePallet]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
INSERT INTO [htbl_PalletBarcodes] ([vUnqBarcode]
,[StockTake]
,[CycleCount]
,[Dispatch]
,[Printed]
,[DispatchDate]
,[Quarantine]
,[bValidated]
,[ValidateRef]
,[bTransferredOut]
,[bTransferredIn]
,[vJobFrom])
VALUES (@1, NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, @2, 0, 0, @3)
GO


IF (OBJECT_ID('[dbo].[sp_UI_SaveRMBarcodePallet') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_SaveRMBarcodePallet]
GO
CREATE PROC [dbo].[sp_UI_SaveRMBarcodePallet]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
INSERT INTO [htbl_PalletBarcodes] (
[vUnqBarcode]
,[StockTake]
,[CycleCount]
,[Dispatch]
,[Printed]
,[DispatchDate]
,[Quarantine]
,[bValidated]
,[ValidateRef]
,[bTransferredOut]
,[bTransferredIn]
,[vJobFrom]
,[bRMPallet])
OUTPUT INSERTED.iLine_ID
VALUES (@1, NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, @2, 0, 0, @3, 1)
GO



IF (OBJECT_ID('[dbo].[sp_UI_ClearUnqsDispatch') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ClearUnqsDispatch]
GO
CREATE PROC [dbo].[sp_UI_ClearUnqsDispatch]
(
@1 varchar(20)
)
AS
UPDATE [tbl_unqBarcodes] SET [Dispatch] = NULL, [DispatchDate] = NULL WHERE [Dispatch] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_ClearUnqPalletsDispatch') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ClearUnqPalletsDispatch]
GO
CREATE PROC [dbo].[sp_UI_ClearUnqPalletsDispatch]
(
@1 varchar(20)
)
AS
UPDATE [htbl_PalletBarcodes] SET [Dispatch] = NULL, [DispatchDate] = NULL WHERE [Dispatch] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_UpdateTransFromVault') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateTransFromVault]
GO
CREATE PROC [dbo].[sp_UI_UpdateTransFromVault]
(
@1 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bFromVault] = 1 WHERE  [vUnqBarcode] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemReceived') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemReceived]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemReceived]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bTransferredIn] = 1, [bTransferredOut] = 0 
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemConsumed') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemConsumed]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemConsumed]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bConsumed] = 1, [vJobConsumed] = @5
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemReceivedAndConsumed') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemReceivedAndConsumed]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemReceivedAndConsumed]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bConsumed] = 1, [bTransferredIn] = 1, [bTransferredOut] = 0 
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemTransferredOut') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemTransferredOut]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemTransferredOut]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bTransferredIn] = 0, [bTransferredOut] = 1 
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateBoxesDispatched') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateBoxesDispatched]
GO
CREATE PROC [dbo].[sp_MBL_UpdateBoxesDispatched]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [bTransferredIn] = 0, [bTransferredOut] = 1 
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO

IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemDispatched') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemDispatched]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemDispatched]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(20)
)
AS
UPDATE [tbl_unqBarcodes] SET [Dispatch] = @5, [DispatchDate] = GETDATE()
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePalletDispatched') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePalletDispatched]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePalletDispatched]
(
@1 varchar(20),
@2 int
)
AS
UPDATE [htbl_PalletBarcodes] SET [Dispatch] = @1, [DispatchDate] = GETDATE()
WHERE [iLine_ID] = @2
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemST1') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemST1]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemST1]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(20)
)
AS
UPDATE [tbl_unqBarcodes] SET [StockTake] = @5
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemST2') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemST2]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemST2]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [StockTake2] = @5
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePalletST1') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePalletST1]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePalletST1]
(
@1 varchar(max),
@2 varchar(20)
)
AS
UPDATE [htbl_PalletBarcodes] SET [StockTake] = @2
WHERE [vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePalletST2') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePalletST2]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePalletST2]
(
@1 varchar(max),
@2 varchar(max)
)
AS
UPDATE [htbl_PalletBarcodes] SET [StockTake2] = @2
    WHERE [vUnqBarcode] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdatePalletSTBoth') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdatePalletSTBoth]
GO
CREATE PROC [dbo].[sp_MBL_UpdatePalletSTBoth]
(
@1 varchar(max),
@2 varchar(max)
)
AS
UPDATE [htbl_PalletBarcodes] SET [StockTake] = @2, [StockTake2] = @2
WHERE [vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemSTBoth') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemSTBoth]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemSTBoth]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [StockTake] = @5, [StockTake2] = @5
WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemSTBoth_Reversal') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemSTBoth_Reversal]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemSTBoth_Reversal]
(
@1 varchar(max)
)
AS
UPDATE [tbl_unqBarcodes] SET [StockTake] = '', [StockTake2] = ''
WHERE [vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRMPalletRemoved') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRMPalletRemoved]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRMPalletRemoved]
(
@HUNQ varchar(max),
@LUNQ varchar(max)
)
AS
UPDATE l SET l.[bOnPallet] = 0
FROM [ltbl_PalletBarcodes] l 
INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
WHERE h.[vUnqBarcode] = @HUNQ AND l.[vUnqBarcode] = @LUNQ
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateItemSTPalletBoth_Reversal') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateItemSTPalletBoth_Reversal]
GO
CREATE PROC [dbo].[sp_MBL_UpdateItemSTPalletBoth_Reversal]
(
@1 varchar(max)
)
AS
UPDATE [htbl_PalletBarcodes] SET [StockTake] = '', [StockTake2] = ''
WHERE [vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetRTStockTakes') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetRTStockTakes]
GO
CREATE PROC [dbo].[sp_UI_GetRTStockTakes]
AS
SELECT [cInvCountNo] ,[cDescription]
FROM [RTIS_InvCount] WHERE [iStatus] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_CheckSTHeader') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckSTHeader]
GO
CREATE PROC [dbo].[sp_UI_CheckSTHeader]
(
@1 varchar(50)
)
AS
SELECT [cInvCountNo]
FROM [RTIS_InvCount] WHERE [iStatus] = 1 AND [cInvCountNo] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_getInvCountLineTickets') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getInvCountLineTickets]
GO
CREATE PROC [dbo].[sp_UI_getInvCountLineTickets]
(
@1 int
)
AS
SELECT [iLineID],[vTicketNo],[dCountQty],[vCountUser] ,[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2],[vBarcodeType],[bCountValid],[bRecountTicket]
FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] =@1 AND [bCountValid] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_getInvCountLineTickets_Archive') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getInvCountLineTickets_Archive]
GO
CREATE PROC [dbo].[sp_UI_getInvCountLineTickets_Archive]
(
@1 int
)
AS
SELECT [iLineID],[vTicketNo],[dCountQty],[vCountUser] ,[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2],[vBarcodeType],[bCountValid],[bRecountTicket]
FROM [RTIS_InvCountArchiveLines_Tickets]
WHERE [iHeaderID] =@1 AND [bCountValid] = 1
GO



IF (OBJECT_ID('[dbo].[sp_UI_getTicketInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getTicketInfo]
GO
CREATE PROC [dbo].[sp_UI_getTicketInfo]
(
@1 varchar(100),
@2 int
)
AS
SELECT ISNULL([dCountQty],0), ISNULL([dCountQty2], 0), [vUnqBarcode] FROM [RTIS_InvCountLines_Tickets]
WHERE [vTicketNo] = @1 AND [iHeaderID] = @2
GO



IF (OBJECT_ID('[dbo].[sp_UI_getAllTicketInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getAllTicketInfo]
GO
CREATE PROC [dbo].[sp_UI_getAllTicketInfo]
(
@1 varchar(100)
)
AS
SELECT [iHeaderID], ISNULL([dCountQty],0), ISNULL([dCountQty2], 0), [vUnqBarcode] FROM [RTIS_InvCountLines_Tickets]
WHERE [vTicketNo] = @1
GO

IF (OBJECT_ID('[dbo].[sp_UI_getStQtys') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getStQtys]
GO
CREATE PROC [dbo].[sp_UI_getStQtys]
(
@1 int
)
AS
SELECT [fCountQty] , [fCountQty2] FROM [RTIS_InvCountLines]
WHERE [idInvCountLines] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetExportFormats') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetExportFormats]
GO
CREATE PROC [dbo].[sp_UI_GetExportFormats]
AS
SELECT [vFormatName]
FROM [tbl_ExportFormats] 
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetExportFormatString') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetExportFormatString]
GO
CREATE PROC [dbo].[sp_UI_GetExportFormatString]
(
@1 varchar(max)
)
AS
SELECT [vFormatString], [vDelimeter]
FROM [tbl_ExportFormats] WHERE [vFormatName] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_CheckExportFormatExists') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_CheckExportFormatExists]
GO
CREATE PROC [dbo].[sp_UI_CheckExportFormatExists]
(
@1 varchar(max)
)
AS
SELECT [vFormatName]
FROM [tbl_ExportFormats] WHERE [vFormatName] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCount') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCount]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCount]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [bOnST] = 1
GO

IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCountArchive') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCountArchive]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCountArchive]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [bOnST] = 1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCountAll') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCountAll]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCountAll]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCountAllArchive') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCountAllArchive]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCountAllArchive]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1
GO

IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCount1') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCount1]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCount1]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [fCountQty] <> 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCount1Archive') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCount1Archive]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCount1Archive]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [fCountQty] <> 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCount2') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCount2]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCount2]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [fCountQty2] <> 0
GO

IF (OBJECT_ID('[dbo].[sp_UI_GetSTLinesCount2Archive') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSTLinesCount2Archive]
GO
CREATE PROC [dbo].[sp_UI_GetSTLinesCount2Archive]
(
@1 varchar(max)
)
AS
SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND [fCountQty2] <> 0
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetRTArchiveStockTakes') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetRTArchiveStockTakes]
GO
CREATE PROC [dbo].[sp_UI_GetRTArchiveStockTakes]
AS
SELECT [cInvCountNo],[cDescription]
FROM [RTIS_InvCount] WHERE [iStatus] = 0
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetRTStockTakes') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetRTStockTakes]
GO
CREATE PROC [dbo].[sp_MBL_GetRTStockTakes]
AS
SELECT [cInvCountNo]
FROM [RTIS_InvCount] WHERE [iStatus] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetStockTakeWarehosesBySTNum') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetStockTakeWarehosesBySTNum]
GO
CREATE PROC [dbo].[sp_MBL_GetStockTakeWarehosesBySTNum]
(
@1 varchar(max)
)
AS
SELECT DISTINCT w.[Code], w.[Name] FROM [dbo].[RTIS_InvCountLines] il
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
INNER JOIN [dbo].[RTIS_InvCount] i ON i.[idInvCount] = il.[iInvCountID]
WHERE i.[cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetStockTakIDBySTNum') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetStockTakIDBySTNum]
GO
CREATE PROC [dbo].[sp_MBL_GetStockTakIDBySTNum]
(
@1 varchar(max)
)
AS
SELECT [idInvCount] FROM [RTIS_InvCount] WHERE [cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetSTLineIDNoLot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSTLineIDNoLot]
GO
CREATE PROC [dbo].[sp_MBL_GetSTLineIDNoLot]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
SELECT l.[idInvCountLines] FROM [RTIS_InvCountLines] l
INNER JOIN [RTIS_InvCount] h ON h.[idInvCount] = l.[iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = l.[iWarehouseID] 
WHERE h.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[bLotItem] = 0
GO





IF (OBJECT_ID('[dbo].[sp_MBL_GetSTLineIDLot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetSTLineIDLot]
GO
CREATE PROC [dbo].[sp_MBL_GetSTLineIDLot]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max)
)
AS
SELECT l.[idInvCountLines] FROM [RTIS_InvCountLines] l
INNER JOIN [RTIS_InvCount] h ON h.[idInvCount] = l.[iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w ON w.[WhseLink] = l.[iWarehouseID] 
INNER JOIN [Cataler_SCN].[dbo].[_etblLotTracking] lo ON lo.[idLotTracking] = l.[iLotTrackingID]
WHERE h.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND lo.[cLotDescription] = @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_checkRT2dForRecount') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_checkRT2dForRecount]
GO
CREATE PROC [dbo].[sp_MBL_checkRT2dForRecount]
(
@1 int,
@2 varchar(max)
)
AS
SELECT TOP 1 [bCountValid] FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_checkPowderPrepForRecount') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_checkPowderPrepForRecount]
GO
CREATE PROC [dbo].[sp_MBL_checkPowderPrepForRecount]
(
@1 int,
@2 varchar(max)
)
AS
SELECT TOP 1 [bCountValid] FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND ISNULL([bCountValid], 0) = 0 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_checkTicketNumber') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_checkTicketNumber]
GO
CREATE PROC [dbo].[sp_MBL_checkTicketNumber]
(
@1 int,
@2 varchar(max)
)
AS
SELECT [vTicketNo] FROM [RTIS_InvCountLines_Tickets] WHERE [iHeaderID] = @1 AND [vTicketNo] = @2
GO



IF (OBJECT_ID('[dbo].[sp_MBL_getFreshSlurryInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getFreshSlurryInfo]
GO
CREATE PROC [dbo].[sp_MBL_getFreshSlurryInfo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 0)  FROM [tbl_RTIS_Fresh_Slurry]
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getFreshSlurryInfo_CheckAddLot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getFreshSlurryInfo_CheckAddLot]
GO
CREATE PROC [dbo].[sp_MBL_getFreshSlurryInfo_CheckAddLot]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 0)  FROM [tbl_RTIS_Fresh_Slurry]
WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getSlurryTicketInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getSlurryTicketInfo]
GO
CREATE PROC [dbo].[sp_MBL_getSlurryTicketInfo]
(
@1 int,
@2 varchar(max)
)
AS
SELECT ISNULL([dCountQty], 0), ISNULL([dCountQty2],0) FROM [RTIS_InvCountLines_Tickets] WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND [bCountValid] = 1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getTicketUpdateInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getTicketUpdateInfo]
GO
CREATE PROC [dbo].[sp_MBL_getTicketUpdateInfo]
(
@1 int,
@2 varchar(max)
)
AS
SELECT TOP 1 ISNULL([dCountQty], 0), ISNULL([dCountQty2], 0), [vUnqBarcode], [bCountValid] FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getFreshSlurryUnqTicket') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getFreshSlurryUnqTicket]
GO
CREATE PROC [dbo].[sp_MBL_getFreshSlurryUnqTicket]
(
@1 int,
@2 varchar(max)
)
AS
SELECT TOP 1 [vTicketNo] FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND [bCountValid] = 1 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getUnqcodeTicket') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getUnqcodeTicket]
GO
CREATE PROC [dbo].[sp_MBL_getUnqcodeTicket]
(
@1 int,
@2 varchar(max)
)
AS
SELECT TOP 1 [vTicketNo] FROM [RTIS_InvCountLines_Tickets]
WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2  AND [bCountValid] = 1 ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getMixedSlurryTankInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getMixedSlurryTankInfo]
GO
CREATE PROC [dbo].[sp_MBL_getMixedSlurryTankInfo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bReceived], 0), ISNULL([bManualclose], 0)  FROM [tbl_RTIS_MS_Main]
WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vTankType] = 'TNK' ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getMixedSlurryTankInfo_CheckLot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getMixedSlurryTankInfo_CheckLot]
GO
CREATE PROC [dbo].[sp_MBL_getMixedSlurryTankInfo_CheckLot]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bReceived], 0), ISNULL([bManualclose], 0)  FROM [tbl_RTIS_MS_Main]
WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vTankType] = 'TNK' ORDER BY [iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getMixedSlurryMobileTankInfo_CheckLot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getMixedSlurryMobileTankInfo_CheckLot]
GO
CREATE PROC [dbo].[sp_MBL_getMixedSlurryMobileTankInfo_CheckLot]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 m.[vLotNumber], md.[dDryWeight], ISNULL(md.[bReceived], 0), ISNULL(md.[bManualclose], 0)  FROM [tbl_RTIS_MS_Decant] md 
INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = md.[iHeaderID]
WHERE md.[vTankCode] = @1 AND md.[vItemCode] = @2 ORDER BY md.[iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getMixedSlurryMobileTankInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getMixedSlurryMobileTankInfo]
GO
CREATE PROC [dbo].[sp_MBL_getMixedSlurryMobileTankInfo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 m.[vLotNumber], md.[dDryWeight], ISNULL(md.[bReceived], 0), ISNULL(md.[bManualclose], 0)  FROM [tbl_RTIS_MS_Decant] md 
INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = md.[iHeaderID]
WHERE md.[vTankCode] = @1 AND md.[vItemCode] = @2 ORDER BY md.[iLineID] DESC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_checkTicketRef') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_checkTicketRef]
GO
CREATE PROC [dbo].[sp_MBL_checkTicketRef]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT t.[vTicketNo] FROM [RTIS_InvCountLines_Tickets] t 
INNER JOIN [RTIS_InvCountLines] il ON il.[idInvCountLines] = t.[iHeaderID]
INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1 AND t.[vTicketNo] = @2
GO


IF (OBJECT_ID('[dbo].[sp_MBL_getPGMContainerInfo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_getPGMContainerInfo]
GO
CREATE PROC [dbo].[sp_MBL_getPGMContainerInfo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT TOP 1 ph.[vLotDesc], pl.[dWeightIn], ISNULL(pt.[bReceived], 0)  FROM [ltbl_RTIS_PGM_Manuf] pl --
INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
LEFT JOIN [ltbl_RTIS_PGM_Trans] pt ON pt.[iHeaderID] = ph.[iLineID] 
WHERE pl.[vContainer] = @1 AND ph.[vItemCode] = @2
ORDER BY pl.[iLineID] DESC
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetAllPalletBarcodes') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetAllPalletBarcodes]
GO
CREATE PROC [dbo].[sp_MBL_GetAllPalletBarcodes]
(
@1 varchar(max)
)
AS
SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
WHERE h.[vUnqBarcode] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetAllPalletBarcodes_RM') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetAllPalletBarcodes_RM]
GO
CREATE PROC [dbo].[sp_MBL_GetAllPalletBarcodes_RM]
(
@1 varchar(max)
)
AS
SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
WHERE h.[vUnqBarcode] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_ImportEvoStockTakeHeader') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ImportEvoStockTakeHeader]
GO
CREATE PROC [dbo].[sp_UI_ImportEvoStockTakeHeader]
(
@1 varchar(max)
)
AS
INSERT INTO [RTIS_InvCount] SELECT i.[idInvCount],i.[cInvCountNo],i.[cDescription],i.[dPrepared]
,i.[cWarehouses],i.[cBinLocations],'1',i.[_btblInvCount_iBranchID],i.[_btblInvCount_dCreatedDate] 
FROM [Cataler_SCN].[dbo].[_btblInvCount] i WHERE [cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_ImportEvoStockTakeLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ImportEvoStockTakeLines]
GO
CREATE PROC [dbo].[sp_UI_ImportEvoStockTakeLines]
(
@1 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines] 
SELECT il.[iInvCountID], il.[cBarcode], 0 AS [fCountQty], 0 AS [fCountQty2], il.[fSystemQty], il.[bSerialItem], il.[tSerialList]
,il.[bLotItem], il.[iLotTrackingID], il.[iBinLocationId], il.[iStockID], il.[iWarehouseID], il.[_btblInvCountLines_dCreatedDate] , 0 AS [Counted],  1 AS [bOnST]
FROM [Cataler_SCN].[dbo].[_btblInvCountLines] il INNER JOIN [Cataler_SCN].[dbo].[_btblInvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_InsertTicketLog') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertTicketLog]
GO
CREATE PROC [dbo].[sp_MBL_InsertTicketLog]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines_Tickets] ([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
VALUES (@1, @2, @3, @4, GETDATE(), @5, @6, 1, 0)
GO


IF (OBJECT_ID('[dbo].[sp_MBL_InsertTicketLog2') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertTicketLog2]
GO
CREATE PROC [dbo].[sp_MBL_InsertTicketLog2]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines_Tickets] ([iHeaderID], [vTicketNo], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
VALUES (@1, @2, @3, @4, GETDATE(), @5, @6, 1, 0)
GO



IF (OBJECT_ID('[dbo].[sp_MBL_InsertTicketLogBoth') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertTicketLogBoth]
GO
CREATE PROC [dbo].[sp_MBL_InsertTicketLogBoth]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines_Tickets] 
([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
vALUES
(@1, @2, @3, @4, GETDATE(), @3, @4, GETDATE(), @5, @6, 1, 0)
GO




IF (OBJECT_ID('[dbo].[sp_MBL_InsertTicketLogRecount') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_InsertTicketLogRecount]
GO
CREATE PROC [dbo].[sp_MBL_InsertTicketLogRecount]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines_Tickets] 
([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
vALUES
(@1, @2, @3, @4, GETDATE(), @3, @4, GETDATE(), @5, @6, 1, 1)
GO



IF (OBJECT_ID('[dbo].[sp_UI_InsertExportFormat') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InsertExportFormat]
GO
CREATE PROC [dbo].[sp_UI_InsertExportFormat]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
INSERT INTO [tbl_ExportFormats] ([vFormatName], [vFormatString], [vDelimeter]) VALUES (@1, @2, @3)
GO



IF (OBJECT_ID('[dbo].[sp_UI_ArchiveRTStockTakeTicketLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ArchiveRTStockTakeTicketLines]
GO
CREATE PROC [dbo].[sp_UI_ArchiveRTStockTakeTicketLines]
(
@1 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountArchiveLines_Tickets] 
SELECT [iHeaderID] ,[vTicketNo],[dCountQty],[vCountUser],[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2] 
,[vBarcodeType],[vUnqBarcode],[bCountValid],[bRecountTicket]
FROM [RTIS_InvCountLines_Tickets] it
INNER JOIN [RTIS_InvCountLines] il ON il.[idInvCountLines] = it.[iHeaderID]
INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
WHERE ih.[cInvCountNo] = @1 
GO



IF (OBJECT_ID('[dbo].[sp_UI_ArchiveRTStockTakeLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ArchiveRTStockTakeLines]
GO
CREATE PROC [dbo].[sp_UI_ArchiveRTStockTakeLines]
(
@1 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountArchiveLines] 
SELECT il.[idInvCountLines],il.[iInvCountID], il.[cBarcode], il.[fCountQty],  il.[fCountQty2], il.[fSystemQty], il.[bSerialItem], il.[tSerialList]
,il.[bLotItem], il.[iLotTrackingID], il.[iBinLocationId], il.[iStockID], il.[iWarehouseID], il.[_btblInvCountLines_dCreatedDate], il.[bOnST] 
FROM [RTIS_InvCountLines] il INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
WHERE i.[cInvCountNo] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_AddItemForInvestigation') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_AddItemForInvestigation]
GO
CREATE PROC [dbo].[sp_MBL_AddItemForInvestigation]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 decimal(16,3),
@5 varchar(max)
)
AS
INSERT INTO [RTIS_InvCountLines_Enquiry] ([vStNumber], [vItemCode], [vLotDescription], [dQty1], [vUserAdded], [dtAdded])
VALUES (@1, @2, @3, @4, @5, GETDATE())
GO


IF (OBJECT_ID('[dbo].[sp_MBL_AddItemToST') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_AddItemToST]
GO
CREATE PROC [dbo].[sp_MBL_AddItemToST]
(
@1 int,
@2 varchar(max),
@3 varchar(max),
@4 int,
@5 int,
@6 int
)
AS
INSERT INTO [RTIS_InvCountLines] ([iInvCountID], [cBarcode], [fCountQty] ,[fCountQty2] ,[fSystemQty] ,[bSerialItem] ,[tSerialList] ,[bLotItem], [iLotTrackingID], [iBinLocationId], [iStockID], [iWarehouseID], [_btblInvCountLines_dCreatedDate], [bIsCounted], [bOnST])
VALUES                           (@1           , @2        , 0           , 0           , 0           , 0            , NULL         , @3      , @4              , 0               , @5        , @6            , NULL                             ,0, 0)
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateSTCount_Lot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateSTCount_Lot]
GO
CREATE PROC [dbo].[sp_MBL_UpdateSTCount_Lot]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max),
@7 varchar(max),
@8 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND ([vUnqBarcode] LIKE @5 AND  [vUnqBarcode] LIKE @6 AND [vUnqBarcode] LIKE @7 AND [vUnqBarcode] LIKE @8)
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateSTCount2_Lot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateSTCount2_Lot]
GO
CREATE PROC [dbo].[sp_MBL_UpdateSTCount2_Lot]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max),
@6 varchar(max),
@7 varchar(max),
@8 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND ([vUnqBarcode] LIKE @5 AND  [vUnqBarcode] LIKE @6 AND [vUnqBarcode] LIKE @7 AND [vUnqBarcode] LIKE @8)
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateSTCount_FreshSlurry') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateSTCount_FreshSlurry]
GO
CREATE PROC [dbo].[sp_MBL_UpdateSTCount_FreshSlurry]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateSTCount2_FreshSlurry') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateSTCount2_FreshSlurry]
GO
CREATE PROC [dbo].[sp_MBL_UpdateSTCount2_FreshSlurry]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateTicketSTCount') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateTicketSTCount]
GO
CREATE PROC [dbo].[sp_MBL_UpdateTicketSTCount]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateTicketSTCount2') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateTicketSTCount2]
GO
CREATE PROC [dbo].[sp_MBL_UpdateTicketSTCount2]
(
@1 int,
@2 varchar(max),
@3 decimal(16,3),
@4 varchar(max),
@5 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRTStockTakeItem_Lot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRTStockTakeItem_Lot]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRTStockTakeItem_Lot]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3)
)
AS
UPDATE il SET il.[fCountQty] = il.[fCountQty] + @5 FROM [RTIS_InvCountLines] il
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s 
ON s.[StockLink] = il.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w
ON il.[iWarehouseID] = w.[WhseLink]
INNER JOIN [RTIS_InvCount] i 
ON i.[idInvCount] = il.[iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l
ON l.[idLotTracking] = il.[iLotTrackingID]
WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRTStockTakeItem2_Lot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRTStockTakeItem2_Lot]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRTStockTakeItem2_Lot]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3)
)
AS
UPDATE il SET il.[fCountQty2] = il.[fCountQty2] + @5 FROM [RTIS_InvCountLines] il
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s 
ON s.[StockLink] = il.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w
ON il.[iWarehouseID] = w.[WhseLink]
INNER JOIN [RTIS_InvCount] i 
ON i.[idInvCount] = il.[iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l
ON l.[idLotTracking] = il.[iLotTrackingID]
WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRTStockTakeItemBoth_Lot') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRTStockTakeItemBoth_Lot]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRTStockTakeItemBoth_Lot]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max),
@4 varchar(max),
@5 decimal(16,3)
)
AS
UPDATE il SET il.[fCountQty] = il.[fCountQty] + @5 , il.[fCountQty2] = il.[fCountQty2] + @5 FROM [RTIS_InvCountLines] il
INNER JOIN [Cataler_SCN].[dbo].[StkItem] s 
ON s.[StockLink] = il.[iStockID]
INNER JOIN [Cataler_SCN].[dbo].[WhseMst] w
ON il.[iWarehouseID] = w.[WhseLink]
INNER JOIN [RTIS_InvCount] i 
ON i.[idInvCount] = il.[iInvCountID]
INNER JOIN [Cataler_SCN].[dbo].[_etblLotTracking] l
ON l.[idLotTracking] = il.[iLotTrackingID]
WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4
GO



IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRTStockTakeReverseTicket') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRTStockTakeReverseTicket]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRTStockTakeReverseTicket]
(
@1 bigint,
@2 float,
@3 float
)
AS
UPDATE [RTIS_InvCountLines] SET [fCountQty] = @2, [fCountQty2] = @3 WHERE [idInvCountLines] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_UpdateRTStockTakeReverseTicketRT2D') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_UpdateRTStockTakeReverseTicketRT2D]
GO
CREATE PROC [dbo].[sp_MBL_UpdateRTStockTakeReverseTicketRT2D]
(
@1 bigint,
@2 float,
@3 float
)
AS
UPDATE [RTIS_InvCountLines] SET [fCountQty] = [fCountQty] - @2, [fCountQty2] = [fCountQty2] - @3 WHERE [idInvCountLines] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_InvalidateSTTicket') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_InvalidateSTTicket]
GO
CREATE PROC [dbo].[sp_UI_InvalidateSTTicket]
(
@1 int,
@2 varchar(max)
)
AS
UPDATE [RTIS_InvCountLines_Tickets] SET [bCountValid] = 0
WHERE [iHeaderID] = @1 AND [vTicketNo] = @2
GO



IF (OBJECT_ID('[dbo].[sp_UI_UpdateExportFormat') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_UpdateExportFormat]
GO
CREATE PROC [dbo].[sp_UI_UpdateExportFormat]
(
@1 varchar(max),
@2 varchar(max),
@3 varchar(max)
)
AS
UPDATE [tbl_ExportFormats] SET [vFormatString] = @2, [vDelimeter] = @3 WHERE [vFormatName] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_ArchiveStockTakeHeader') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ArchiveStockTakeHeader]
GO
CREATE PROC [dbo].[sp_UI_ArchiveStockTakeHeader]
(
@1 varchar(max)
)
AS
UPDATE [RTIS_InvCount] SET [iStatus] = 0 WHERE [cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_RemoveExportLayout') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemoveExportLayout]
GO
CREATE PROC [dbo].[sp_UI_RemoveExportLayout]
(
@1 varchar(max)
)
AS
DELETE FROM [tbl_ExportFormats] WHERE [vFormatName] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_RemovePostArchiveTicketLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemovePostArchiveTicketLines]
GO
CREATE PROC [dbo].[sp_UI_RemovePostArchiveTicketLines]
(
@1 varchar(max)
)
AS
DELETE it FROM [RTIS_InvCountLines_Tickets] it
INNER JOIN [RTIS_InvCountLines] il ON it.[iHeaderID] = il.[idInvCountLines]
INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
WHERE ih.[cInvCountNo] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_RemovePostArchiveLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemovePostArchiveLines]
GO
CREATE PROC [dbo].[sp_UI_RemovePostArchiveLines]
(
@1 varchar(max)
)
AS
DELETE il FROM [RTIS_InvCountLines] il INNER JOIN [RTIS_InvCount] i
ON il.[iInvCountID] = i.[idInvCount] WHERE i.[cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_RemoveArchiveTicketLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemoveArchiveTicketLines]
GO
CREATE PROC [dbo].[sp_UI_RemoveArchiveTicketLines]
(
@1 varchar(max)
)
AS
DELETE it FROM [RTIS_InvCountArchiveLines_Tickets] it
INNER JOIN [RTIS_InvCountArchiveLines] il ON it.[iHeaderID] = il.[idInvCountLines]
INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
WHERE ih.[cInvCountNo] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_RemoveArchiveRTStockTakeLines') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemoveArchiveRTStockTakeLines]
GO
CREATE PROC [dbo].[sp_UI_RemoveArchiveRTStockTakeLines]
(
@1 varchar(max)
)
AS
DELETE il FROM [RTIS_InvCountArchiveLines] il INNER JOIN [RTIS_InvCount] i
ON il.[iInvCountID] = i.[idInvCount] WHERE i.[cInvCountNo] = @1
GO


IF (OBJECT_ID('[dbo].[sp_UI_RemoveArchiveRTStockTakeHead') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_RemoveArchiveRTStockTakeHead]
GO
CREATE PROC [dbo].[sp_UI_RemoveArchiveRTStockTakeHead]
(
@1 varchar(max)
)
AS
DELETE FROM [RTIS_InvCount] WHERE [cInvCountNo] = @1
GO





























































































































































































































































































































































































































































































































 





















 









 



































































