USE Cataler_SCN
GO






IF (OBJECT_ID('[dbo].[sp_GetWhseStockQtys]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetWhseStockQtys]
GO

CREATE PROC [dbo].[sp_GetWhseStockQtys]
	@whseCode varchar(50)
AS
SELECT s.[Code], s.[Description_1], l.[cLotDescription], [fQtyOnHand] FROM [_etblLotTrackingQty] lq
INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
WHERE w.[Code] = @whseCode AND lq.[fQtyOnHand] <> 0
GO






IF (OBJECT_ID('[dbo].[sp_GetActivePOs]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetActivePOs]
GO

CREATE PROC [dbo].[sp_GetActivePOs]
	@supplier varchar(max)
AS
SELECT DISTINCT TOP 1000 [OrderNum] FROM [InvNum]
WHERE [cAccountName]=@supplier AND [AccountID] <>'' AND [DocType] = 5 AND ([DocState] = 1 OR [DocState] = 3) 
ORDER BY [OrderNum] DESC
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetEvoPOVendors]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetEvoPOVendors]
GO

CREATE PROC [dbo].[sp_UI_GetEvoPOVendors]
AS
SELEcT DISTINCT v.[DCLink], v.[Name], ISNULL(rv.[bSelected], 'false') AS [Selected] FROM [InvNum] i 
INNER JOIN [Vendor] v ON v.[DCLink] = i.[AccountID]
LEFT JOIN [CAT_RTIS].[dbo].[rtblEvoVendors] rv ON rv.[iVendorID] = v.[DCLink] ORDER BY v.[Name] ASC
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetVendorPOs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetVendorPOs]
GO

CREATE PROC [dbo].[sp_UI_GetVendorPOs]
	@vendorId varchar(max)
AS
SELECT DISTINCT TOP 15 [OrderNum] FROM [InvNum]
WHERE [AccountID] = @vendorId AND [DocType] = 5 AND ([DocState] = 1 OR [DocState] = 3) ORDER BY [OrderNum] DESC
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetPOLinesNew]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetPOLinesNew]
GO

CREATE PROC [dbo].[sp_UI_GetPOLinesNew]
	@orderNum varchar(max)
AS
SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], ''), il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable]	                                                                                                   
,ISNULL(rtp.[dPrintQty],0) AS [dPrintQty]                                                    
,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
,'0' AS [Back1]
,'0' AS [Back2]
,'' AS [Back3]
FROM [InvNum] i
INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
LEFT JOIN [CAT_RTIS].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vLotNumber] COLLATE Latin1_General_CI_AS = il.[cLotNumber] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
WHERE i.[OrderNum] = @orderNum AND i.[DocType] = '5' And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 1 AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @orderNum ORDER BY [DocVersion] DESC)       
UNION
SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], '') , il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable] 	                                                
,ISNULL(rtp.[dPrintQty], 0) AS [dPrintQty]                                                    
,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
,'0' AS [Back1]
,'0' AS [Back2]
,'' AS [Back3]
FROM [InvNum] i
INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
LEFT JOIN [CAT_RTIS].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
WHERE i.[OrderNum] = @orderNum AND i.[DocType] = '5' And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 0  AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @orderNum ORDER BY [DocVersion] DESC)
GO





IF (OBJECT_ID('[dbo].[sp_UI_ReprintPOLinesNew]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_ReprintPOLinesNew]
GO

CREATE PROC [dbo].[sp_UI_ReprintPOLinesNew]
	@orderNum varchar(max)
AS
SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], ''), il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable]	                                                                                                   
,ISNULL(rtp.[dPrintQty],0) AS [dPrintQty]                                                    
,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
,'0' AS [Back1]
,'0' AS [Back2]
,'' AS [Back3]
FROM [InvNum] i
INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
LEFT JOIN [CAT_RTIS].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vLotNumber] COLLATE Latin1_General_CI_AS = il.[cLotNumber] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
WHERE i.[OrderNum] = @orderNum AND i.[DocType] = '5' AND il.[cLotNumber]<>'' AND dPrintQty>0 And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 1 AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @orderNum ORDER BY [DocVersion] DESC)       
UNION
SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], '') , il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable] 	                                                
,ISNULL(rtp.[dPrintQty], 0) AS [dPrintQty]                                                    
,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
,'0' AS [Back1]
,'0' AS [Back2]
,'' AS [Back3]
FROM [InvNum] i
INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
LEFT JOIN [CAT_RTIS].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
WHERE i.[OrderNum] = @orderNum AND i.[DocType] = '5' AND il.[cLotNumber]<>'' AND dPrintQty>0 And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 0  AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @orderNum ORDER BY [DocVersion] DESC)
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetStockLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetStockLabelInfo]
GO

CREATE PROC [dbo].[sp_UI_GetStockLabelInfo]
	@itemCode varchar(max)
AS
SELECT [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_MBL_CheckPOExists]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckPOExists]
GO

CREATE PROC [dbo].[sp_MBL_CheckPOExists]
	@OrderNum varchar(max)
AS
SELECT [OrderNum] FROM [InvNum]
WHERE [OrderNum] = @OrderNum
GO





IF (OBJECT_ID('[dbo].[sp_MBL_GetEvoPOLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetEvoPOLines]
GO

CREATE PROC [dbo].[sp_MBL_GetEvoPOLines]
	@OrderNum varchar(max)
AS
SELECT s.[Code], s.[Description_1], s.[Description_2], CAST(s.[bLotItem] AS INT), il.[cLotNumber], il.[fQuantity], il.[fQtyToProcess], 0, (il.[fQuantity] - il.[fQtyToProcess] - il.[fQtyProcessed]) AS [ViewableQty],
ISNULL(rtp.[dPrintQty], 0) FROM [_btblInvoiceLines] il
INNER JOIN [InvNum] i ON i.[AutoIndex] = il.[iInvoiceID]
INNER JOIN [StkItem] s ON il.[iStockCodeID] = s.[StockLink]
LEFT JOIN [CAT_RTIS].[dbo].[tblPOLines] rtp ON s.[Code] = rtp.[vItemCode] COLLATE Latin1_General_CI_AS AND il.[cLotNumber] = rtp.[vLotNumber] COLLATE Latin1_General_CI_AS AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
WHERE i.[OrderNum] = @OrderNum And (i.[DocState] = 1 OR i.[DocState] = 3) AND i.[DocFlag] = 1 
ORDER BY il.[cLotNumber] DESC
GO

------------------------sp_GetAllCanningCatalysts----------------
IF (OBJECT_ID('[sp_GetAllCanningCatalysts]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetAllCanningCatalysts]
GO

CREATE PROC [dbo].[sp_GetAllCanningCatalysts]
AS
SELECT [Code],[Description_1],[Description_2], '' FROM [StkItem]
WHERE [Code] LIKE '18450%'
GO

------------------------sp_GetAllCanningWRMs----------------
IF (OBJECT_ID('[sp_GetAllCanningWRMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetAllCanningWRMs]
GO

CREATE PROC [dbo].[sp_GetAllCanningWRMs]
AS
SELECT [Code],[Description_1], '' FROM [StkItem]
WHERE [Code] LIKE '18471%' OR [Code] LIKE '18461%' OR [Code] LIKE '%A&W%' OR ([Code] LIKE 'V%' AND [Code] NOT LIKE 'VSP%')
GO

--------------------------------------------------------- AW ---------------------------------------------------------------------------




IF (OBJECT_ID('[dbo].[sp_AW_GetLabelInfo_AWTags]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetLabelInfo_AWTags]
GO

CREATE PROC [dbo].[sp_AW_GetLabelInfo_AWTags]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
    FROM [StkItem] s
    LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [ucIICoatStage] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetReprintLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetReprintLabelInfo]
GO

CREATE PROC [dbo].[sp_AW_GetReprintLabelInfo]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
    FROM [StkItem] s
    LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [ucIICoatStage] = @itemCode
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetReprintInfo_AWTag]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetReprintInfo_AWTag]
GO

CREATE PROC [dbo].[sp_AW_GetReprintInfo_AWTag]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
    FROM [StkItem] s
    LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [ucIICoatStage] = @itemCode
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAllAWCatalysts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllAWCatalysts]
GO

CREATE PROC [dbo].[sp_UI_GetAllAWCatalysts]
AS
	SELECT [ucIICoatStage],[Description_1],[Description_2], '' FROM [StkItem]
    WHERE [ucIICoatStage] LIKE '%A&W%'
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetAllAWWRMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllAWWRMs]
GO

CREATE PROC [dbo].[sp_UI_GetAllAWWRMs]
AS
	SELECT [Code],[Description_1], '' FROM [StkItem]
    WHERE [ucIICoatStage] Like 'SOL-%' OR [ucIICoatStage] Like 'CHEM-1180%' OR [ucIICoatStage] Like 'CHEM-1640%' 
    OR [ItemGroup] = '005' OR [ItemGroup] = '009'
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetZectMFCode]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetZectMFCode]
GO

CREATE PROC [dbo].[sp_AW_GetZectMFCode]
	@baseCode VARCHAR(MAX)
AS
	SELECT [ucIICoatStage] FROM [StkItem] WHERE [Code] = @baseCode
GO





IF (OBJECT_ID('[dbo].[sp_AW_GetAWItemCode]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetAWItemCode]
GO

CREATE PROC [dbo].[sp_AW_GetAWItemCode]
	@baseCode VARCHAR(MAX)
AS
	SELECT [ucIICoatStage] FROM [StkItem] WHERE [ucIICoatStage] LIKE @baseCode AND [ucIICoatStage] LIKE '%A&W'
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetAWAvailablePGMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetAWAvailablePGMs]
GO

CREATE PROC [dbo].[sp_AW_GetAWAvailablePGMs]
	@itemCode VARCHAR(MAX),
	@whseCode VARCHAR(MAX)
AS
	SELECT DISTINCT aw.[vRMCode], l.[cLotDescription], lq.[fQtyOnHand] FROM [_etblLotTrackingQty] lq
    INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
    INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
	INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
    INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]  
	INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_AW_Raws] aw ON aw.[vRMCode] = s.[Code]                       
	WHERE aw.[vAWCode] = @itemCode AND w.[Code] = @whseCode AND lq.[fQtyOnHand]  <> 0
GO




IF (OBJECT_ID('[dbo].[sp_AW_GetLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_AW_GetLabelInfo]
GO

CREATE PROC [dbo].[sp_AW_GetLabelInfo]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
    FROM [StkItem] s
    LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [ucIICoatStage] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_MBL_GetItemDesc]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemDesc]
GO

CREATE PROC [dbo].[sp_MBL_GetItemDesc]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Description_1] FROM [StkItem]
    WHERE [Code] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_MBL_GetItemInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetItemInfo]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Bar_Code],[cSimpleCode],[Description_1],[Description_2],[Description_3] FROM [StkItem] WHERE [Code] = @itemCode
GO





IF (OBJECT_ID('[dbo].[sp_UI_GetItemCodes]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemCodes]
GO

CREATE PROC [dbo].[sp_UI_GetItemCodes]
AS
	SELECT DISTINCT [Code] FROM [StkItem]
    WHERE [Code] NOT LIKE 'TSP%'AND [Code] NOT LIKE 'VSP%'
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetItemLots]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetItemLots]
GO

CREATE PROC [dbo].[sp_UI_GetItemLots]
	@itemCode VARCHAR(MAX)
AS
	SELECT DISTINCT l.[cLotDescription] FROM [_etblLotTracking] l
    INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
    WHERE s.[Code] = @itemCode
GO
















------------------------sp_GetLabelInfo----------------
IF (OBJECT_ID('[sp_UI_GetLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetLabelInfo]
GO

CREATE PROC [dbo].[sp_GetLabelInfo]
(
@Code varchar(400)
)
AS
SELECT [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @Code
GO



 ------------------------sp_Canning_GetLabelInfo----------------
IF (OBJECT_ID('[sp_Canning_GetLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Canning_GetLabelInfo]
GO
CREATE PROC [dbo].[sp_Canning_GetLabelInfo]
(
@Code varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @Code
GO

 ------------------------sp_MBL_GetItemDesc----------------
IF (OBJECT_ID('[sp_MBL_GetItemDesc]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemDesc]
GO
CREATE PROC [dbo].[sp_MBL_GetItemDesc]
(
@Code varchar(400)
)
AS
SELECT [Description_1] FROM [StkItem]
WHERE [Code] = @Code
GO













































