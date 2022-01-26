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
	@CODE VARCHAR(MAX)
AS
SELECT DISTINCT l.[cLotDescription] FROM [_etblLotTracking] l
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
WHERE s.[Code] = @CODE
GO



------------------------sp_GetLabelInfo----------------
IF (OBJECT_ID('[sp_UI_GetLabelInfo_fg]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetLabelInfo_fg]
GO

CREATE PROC [dbo].[sp_UI_GetLabelInfo_fg]
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



IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPlanLines_Catalyst]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPlanLines_Catalyst]
GO
CREATE PROC [dbo].[sp_UI_GetSelectCSPlanLines_Catalyst]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like '18461-%' AND [Code] Not Like '%A&W%' AND [Code] Not Like '%AW%' 
ORDER BY [Code] ASC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPlanLines_Slurry]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPlanLines_Slurry]
GO
CREATE PROC [dbo].[sp_UI_GetSelectCSPlanLines_Slurry]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like 'TSP-%' OR [Code] Like 'VSP-%' 
ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPlanLines_AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPlanLines_AW]
GO
CREATE PROC [dbo].[sp_UI_GetSelectCSPlanLines_AW]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like '%A&W%' OR [Code] Like '%AW%' 
ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectSPPlanLines_Catalyst]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectSPPlanLines_Catalyst]
GO
CREATE PROC [dbo].[sp_UI_GetSelectSPPlanLines_Catalyst]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like 'TSP-%' OR [Code] Like 'VSP-%' 
ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectSPPlanLines_Slurry]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectSPPlanLines_Slurry]
GO
CREATE PROC [dbo].[sp_UI_GetSelectSPPlanLines_Slurry]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like 'TPP-%' 
ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectSPPlanLines_AW]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectSPPlanLines_AW]
GO
CREATE PROC [dbo].[sp_UI_GetSelectSPPlanLines_AW]
AS
SELECT [StockLink], [Code], [Description_1] 
FROM [StkItem] 
WHERE [Code] Like '18461-%' AND [Code] Not Like '%A&W%' AND [Code] Not Like '%AW%' 
ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_MBL_ValidatePPItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_ValidatePPItem]
GO
CREATE PROC [dbo].[sp_MBL_ValidatePPItem]
(
@Code varchar(400)
)
AS
SELECT [StockLink] FROM [StkItem] WHERE [Code] = @Code
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPPItemDesc]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPPItemDesc]
GO
CREATE PROC [dbo].[sp_MBL_GetPPItemDesc]
(
@Code varchar(400)
)
AS
SELECT [Description_1] FROM [StkItem] WHERE [Code] = @Code
GO



IF (OBJECT_ID('[dbo].[sp_MBL_ValidatePPLot]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_ValidatePPLot]
GO
CREATE PROC [dbo].[sp_MBL_ValidatePPLot]
(
@cLotDescription varchar(50)
)
AS
SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @cLotDescription
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetLotID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetLotID]
GO
CREATE PROC [dbo].[sp_MBL_GetLotID]
(
@cLotDescription varchar(50)
)
AS
SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @cLotDescription
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetPPItemInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPPItemInfo]
GO
CREATE PROC [dbo].[sp_MBL_GetPPItemInfo]
(
@Code varchar(400)
)
AS
SELECT [StockLink], [Description_1] FROM [StkItem] WHERE [Code] = @Code
GO



IF (OBJECT_ID('[dbo].[sp_UI_InsertPowderForManufacture]') IS NOT NULL)
	DROP PROC [dbo].[UI_InsertPowderForManufacture]
GO
CREATE PROC [dbo].[UI_InsertPowderForManufacture]
(
@sBOMItemCode varchar(500),
@fQtyToProduce float,
@sLotNumber varchar(500),
@sProjectCode varchar(500)
)
AS
INSERT INTO __SLtbl_MFPImports (sBOMItemCode, fQtyToProduce, sLotNumber, sProjectCode )
VALUES ( @sBOMItemCode, @fQtyToProduce, @sLotNumber, @sProjectCode)
GO





IF (OBJECT_ID('[dbo].[sp_UI_getWhseAllEvoWhses]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_getWhseAllEvoWhses]
GO
CREATE PROC [dbo].[sp_UI_getWhseAllEvoWhses]
AS
SELECT [WhseLink], [Name], '' AS [Add]
FROM [WhseMst]
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetEvoAgents') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetEvoAgents]
GO
CREATE PROC [dbo].[sp_UI_GetEvoAgents]
AS
SELECT [cAgentName] 
FROM [_rtblAgents]
GO












--------------------------------------------------------- Mixed Slurry ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[MBL_GetItemDesc]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetItemDesc]
GO

CREATE PROC [dbo].[MBL_GetItemDesc]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Description_1] FROM [StkItem]
    WHERE [Code] = @itemCode
GO




IF (OBJECT_ID('[dbo].[MBL_GetFeshSlurryInfo]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetFeshSlurryInfo]
GO

CREATE PROC [dbo].[MBL_GetFeshSlurryInfo]
	@itemCode VARCHAR(MAX)
AS
	SELECT [StockLink], [AveUCst] FROM [StkItem] WHERE [Code] = @itemCode
GO




IF (OBJECT_ID('[dbo].[MBL_GetFeshSlurryLotID]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetFeshSlurryLotID]
GO

CREATE PROC [dbo].[MBL_GetFeshSlurryLotID]
	@lotNUmber VARCHAR(MAX),
	@stockID VARCHAR(MAX)
AS
	SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @lotNUmber AND [iStockID] = @stockID
GO




IF (OBJECT_ID('[dbo].[MBL_GetRecLotID]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetRecLotID]
GO

CREATE PROC [dbo].[MBL_GetRecLotID]
	@lotNUmber VARCHAR(MAX),
	@stockID VARCHAR(MAX)
AS
	SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @lotNUmber AND [iStockID] = @stockID
GO




IF (OBJECT_ID('[dbo].[MBL_GetRecLotID]') IS NOT NULL)
	DROP PROC [dbo].[MBL_GetRecLotID]
GO

CREATE PROC [dbo].[MBL_GetRecLotID]
	@lotNUmber VARCHAR(MAX),
	@stockID VARCHAR(MAX)
AS
	SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @lotNUmber AND [iStockID] = @stockID
GO




--------------------------------------------------------- PGM ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[PGM_GetItemFVDescription]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetItemFVDescription]
GO

CREATE PROC [dbo].[PGM_GetItemFVDescription]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Description_1] FROM [StkItem] WHERE [Code] = @itemCode
GO





IF (OBJECT_ID('[dbo].[PGM_GetItemRecDescription]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetItemRecDescription]
GO

CREATE PROC [dbo].[PGM_GetItemRecDescription]
	@itemCode VARCHAR(MAX)
AS
	SELECT [Description_1] FROM [StkItem] WHERE [Code] = @itemCode
GO




--=========================================================================================================--
	------------------------	CHECK BUG	--------------------------
--=========================================================================================================--


IF (OBJECT_ID('[dbo].[PGM_GetItemRecDescription]') IS NOT NULL)
	DROP PROC [dbo].[PGM_GetItemRecDescription]
GO

CREATE PROC [dbo].[PGM_GetItemRecDescription]
	@code VARCHAR(MAX),
	@whse VARCHAR(MAX)
AS
	--SELECT l.[cLotDescription] FROM [_etblLotTrackingQty] lq
 --   INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
 --   INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
 --   INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
 --   INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
 --   INNER JOIN [CAT_RTIS].[dbo].[htbl_RTIS_PGM_Manuf] rl ON rl.[vLotDesc] COLLATE Latin1_General_CI_AS = l.[cLotDescription]
	--WHERE s.[Code] = @code AND w.[Code] = @whse AND lq.[fQtyOnHand] <> 0 AND rl.[vPGMLoc] = @whse
GO




IF (OBJECT_ID('[dbo].[UI_InsertPGmForManufacture]') IS NOT NULL)
	DROP PROC [dbo].[UI_InsertPGmForManufacture]
GO

CREATE PROC [dbo].[UI_InsertPGmForManufacture]
	@itemCode VARCHAR(MAX),
	@qty FLOAT,
	@lotNumber VARCHAR(MAX),
	@project VARCHAR(MAX)
AS
	INSERT INTO __SLtbl_MFPImports (sBOMItemCode, fQtyToProduce, sLotNumber, sProjectCode ) 
	VALUES ( @itemCode, @qty, @lotNumber, @project)
GO




--------------------------------------------------------- PGM Planning ---------------------------------------------------------------------------





IF (OBJECT_ID('[dbo].[UI_GetSelectCSPPGMPlanLines_1]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetSelectCSPPGMPlanLines_1]
GO

CREATE PROC [dbo].[UI_GetSelectCSPPGMPlanLines_1]
AS
	SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [ItemGroup] IN (005) ORDER BY [Code] ASC
GO




IF (OBJECT_ID('[dbo].[UI_GetSelectCSPPGMPlanLines_2]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetSelectCSPPGMPlanLines_2]
GO

CREATE PROC [dbo].[UI_GetSelectCSPPGMPlanLines_2]
AS
	SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE  [ItemGroup] = 011 ORDER BY [Code] ASC
GO




IF (OBJECT_ID('[dbo].[UI_GetSelectCSPPGMPlanLines_3]') IS NOT NULL)
	DROP PROC [dbo].[UI_GetSelectCSPPGMPlanLines_3]
GO

CREATE PROC [dbo].[UI_GetSelectCSPPGMPlanLines_3]
AS
	SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [ItemGroup] = 010 ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPPGMPlanLines_Catalyst]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Catalyst]
GO

CREATE PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Catalyst]
AS
SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [ItemGroup] IN (005) ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPPGMPlanLines_Slurry]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Slurry]
GO

CREATE PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Slurry]
AS
SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE  [ItemGroup] = 011 ORDER BY [Code] ASC
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetSelectCSPPGMPlanLines_Powder]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Powder]
GO

CREATE PROC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Powder]
AS
SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [ItemGroup] = 010 ORDER BY [Code] ASC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetSelectPGMPlanLines]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetSelectPGMPlanLines]
GO

CREATE PROC [dbo].[sp_UI_GetSelectPGMPlanLines]
AS
SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [ItemGroup] IN (002,009) ORDER BY [Code] ASC
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetAllFreshSlurries]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllFreshSlurries]
GO

CREATE PROC [dbo].[sp_UI_GetAllFreshSlurries]
AS
SELECT [Code],[Description_1],[Description_2], '' FROM [StkItem]
WHERE [ItemGroup] LIKE '%011%'
GO



IF (OBJECT_ID('[dbo].[sp_GetItemDesc]') IS NOT NULL)
	DROP PROC [dbo].[sp_GetItemDesc]
GO

CREATE PROC [dbo].[sp_GetItemDesc]
(
@1 varchar(400)
)
AS
SELECT [Description_1] FROM [StkItem] WHERE [Code] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_CheckWhseStockAso]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_CheckWhseStockAso]
GO

CREATE PROC [dbo].[sp_MBL_CheckWhseStockAso]
(
@1 varchar(400),
@2 varchar(20)
)
AS
SELECT [IdWhseStk] FROM [WhseStk] wstk
INNER JOIN [WhseMst] w ON w.[WhseLink] = wstk.[WHWhseID]
INNER JOIN [StkItem] s ON s.[StockLink] = wstk.[WHStockLink]
WHERE s.[Code] = @1 AND w.[Code] = @2
GO



IF (OBJECT_ID('[dbo].[sp_MBL_ValidatePPItem]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_ValidatePPItem]
GO

CREATE PROC [dbo].[sp_MBL_ValidatePPItem]
(
@1 varchar(400)
)
AS
SELECT [StockLink] FROM [StkItem] WHERE [Code] = @1
GO




IF (OBJECT_ID('[dbo].[sp_MBL_ValidatePPLot]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_ValidatePPLot]
GO

CREATE PROC [dbo].[sp_MBL_ValidatePPLot]
(
@1 varchar(50)
)
AS
SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetLotID]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetLotID]
GO

CREATE PROC [dbo].[sp_MBL_GetLotID]
(
@1 varchar(50)
)
AS
SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @1
GO



IF (OBJECT_ID('[dbo].[sp_MBL_GetPPItemInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetPPItemInfo]
GO

CREATE PROC [dbo].[sp_MBL_GetPPItemInfo]
(
@1 varchar(400)
)
AS
SELECT [StockLink], [Description_1] FROM [StkItem] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetConfigTagInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetConfigTagInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetConfigTagInfo]
(
@1 varchar(400)
)
AS
SELECT [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetLabelInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetLabelInfo]
(
@1 varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetLabelInfo_ZectTage]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetLabelInfo_ZectTage]
GO
CREATE PROC [dbo].[sp_Zect_GetLabelInfo_ZectTage]
(
@1 varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetReprintInfo_ZectTag]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReprintInfo_ZectTag]
GO
CREATE PROC [dbo].[sp_Zect_GetReprintInfo_ZectTag]
(
@1 varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO

IF (OBJECT_ID('[dbo].[sp_Zect_GetreOpenLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetreOpenLabelInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetreOpenLabelInfo]
(
@1 varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetReprintLabelInfo]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReprintLabelInfo]
GO
CREATE PROC [dbo].[sp_Zect_GetReprintLabelInfo]
(
@1 varchar(400)
)
AS
SELECT [Code], [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
FROM [StkItem] s
LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1
GO



IF (OBJECT_ID('[dbo].[sp_UI_GetAllCatalysts]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllCatalysts]
GO
CREATE PROC [dbo].[sp_UI_GetAllCatalysts]
AS
Select [Code],[Description_1],[Description_2], '' FROM [StkItem]
WHERE [Code] Like '18461%' OR ([Code] LIKE 'V%' AND [Code] NOT LIKE 'VS%')
GO


IF (OBJECT_ID('[dbo].[sp_UI_GetAllCatalystRMs]') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetAllCatalystRMs]
GO
CREATE PROC [dbo].[sp_UI_GetAllCatalystRMs]
AS
SELECT [Code],[Description_1], '' FROM [StkItem]
WHERE [ItemGroup] LIKE '%007%' OR [ItemGroup] LIKE '%011%' OR [ItemGroup] LIKE '%005%'
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetMFCode]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetMFCode]
GO
CREATE PROC [dbo].[sp_Zect_GetMFCode]
(
@1 varchar(30)
)
AS
SELECT [Code] FROM [StkItem] WHERE [ucIICoatStage] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetItemCodeFromMFCode]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetItemCodeFromMFCode]
GO
CREATE PROC [dbo].[sp_Zect_GetItemCodeFromMFCode]
(
@1 varchar(400)
)
AS
SELECT [ucIICoatStage] FROM [StkItem] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetItemCode]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetItemCode]
GO
CREATE PROC [dbo].[sp_Zect_GetItemCode]
(
@1 varchar(400),
@2 varchar(400)
)
AS
SELECT [Code] FROM [StkItem] WHERE [Code] LIKE @1 AND [Code] LIKE @2
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetItemLotsFromTanks]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetItemLotsFromTanks]
GO
CREATE PROC [dbo].[sp_Zect_GetItemLotsFromTanks]
(
@1 varchar(400),
@2 varchar(400)
)
AS
SELECT DISTINCT 'Large Tank', rl.vTankCode, l.[cLotDescription], 'TNK', rl.[dWetWeight], rl.[dDryWeight] FROM [_etblLotTrackingQty] lq
INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] rl ON rl.[vLotNumber] COLLATE Latin1_General_CI_AS = l.[cLotDescription] AND rl.[vItemCode] = s.[Code]
WHERE s.[Code] = @1 AND w.[Code] = @2 AND rl.[vTankType] = 'TNK'  AND lq.[fQtyOnHand]  > 0 AND rl.[bTransferred] = 1 AND rl.[dSolidity] >0 
UNION
SELECT DISTINCT 'Mobile Tank', rd.vTankCode, l.[cLotDescription], 'MTNK', rd.[dFinalWetWeight], rd.[dDryWeight] FROM [_etblLotTrackingQty] lq
INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] rl ON rl.[vLotNumber] COLLATE Latin1_General_CI_AS = l.[cLotDescription] AND rl.[vItemCode] = s.[Code]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Decant] rd ON rd.[iHeaderID] = rl.[iLineID] 
WHERE s.[Code] = @1 AND w.[Code] = @2 AND rl.[vTankType] = 'BTNK'  AND lq.[fQtyOnHand]  > 0 AND rd.[bTransferred] = 1 AND rd.[dSolidity] >0
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetSlurryTanks]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetSlurryTanks]
GO
CREATE PROC [dbo].[sp_Zect_GetSlurryTanks]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT m.[iLineID], m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0)
FROM [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] m
LEFT JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
WHERE m.[vTankType] = 'TNK' AND m.[vItemCode] = @1 AND m.[vLotNumber] = @2
GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber] 													  
UNION
SELECT 0, 'MTNK_' + d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0) FROM [CAT_RTIS].[dbo].[tbl_RTIS_MS_Decant] d
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] m ON m.[iLineID] = d.[iHeaderID]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Slurries] s ON m.[iLineID] = s.[iHeaderID]
WHERE d.[vItemCode] = @1 AND d.[vLotNumber] = @2
GROUP BY d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber] 
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetReOpenCatalystCoats]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReOpenCatalystCoats]
GO
CREATE PROC [dbo].[sp_Zect_GetReOpenCatalystCoats]
(
@1 varchar(400)
)
AS
SELECT [Code] FROM [StkItem]
WHERE [Code] LIKE @1
GO



IF (OBJECT_ID('[dbo].[sp_Zect_GetReOpenCatalystCoats]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetReOpenCatalystCoats]
GO
CREATE PROC [dbo].[sp_Zect_GetReOpenCatalystCoats]
(
@1 varchar(400)
)
AS
SELECT [Code] FROM [StkItem]
WHERE [Code] LIKE @1
GO




IF (OBJECT_ID('[dbo].[sp_Zect_GetItemDescription]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetItemDescription]
GO
CREATE PROC [dbo].[sp_Zect_GetItemDescription]
(
@1 varchar(400)
)
AS
SELECT [Description_1] FROM [StkItem] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_Zect_GetItemLotsFromTank_AddSlurry]') IS NOT NULL)
	DROP PROC [dbo].[sp_Zect_GetItemLotsFromTank_AddSlurry]
GO
CREATE PROC [dbo].[sp_Zect_GetItemLotsFromTank_AddSlurry]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT DISTINCT 'Large Tank', rl.vTankCode, l.[cLotDescription], 'TNK', rl.[dWetWeight], rl.[dDryWeight] FROM [_etblLotTrackingQty] lq
INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] rl ON rl.[vLotNumber] COLLATE Latin1_General_CI_AS = l.[cLotDescription] AND rl.[vItemCode] = s.[Code]
WHERE s.[Code] = @1 AND w.[Code] = @2 AND rl.[vTankType] = 'TNK'  AND lq.[fQtyOnHand]  > 0 AND rl.[bTransferred] = 1 AND rl.[dSolidity] >0
UNION
SELECT DISTINCT 'Mobile Tank', rd.vTankCode, l.[cLotDescription], 'MTNK', rd.[dFinalWetWeight], rd.[dDryWeight] FROM [_etblLotTrackingQty] lq
INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Main] rl ON rl.[vLotNumber] COLLATE Latin1_General_CI_AS = l.[cLotDescription] AND rl.[vItemCode] = s.[Code]
INNER JOIN [CAT_RTIS].[dbo].[tbl_RTIS_MS_Decant] rd ON rd.[iHeaderID] = rl.[iLineID] 
WHERE s.[Code] = @1 AND w.[Code] = @2 AND rl.[vTankType] = 'BTNK'  AND lq.[fQtyOnHand]  > 0 AND rd.[bTransferred] = 1  AND rd.[dSolidity] >0
GO




IF (OBJECT_ID('[dbo].[sp_UI_GetEvoStockTakes') IS NOT NULL)
	DROP PROC [dbo].[sp_UI_GetEvoStockTakes]
GO
CREATE PROC [dbo].[sp_UI_GetEvoStockTakes]
(
@1 varchar(max)
)
AS
SELECT [cInvCountNo],[cDescription]
FROM [_btblInvCount]
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetEvoWhseID') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetEvoWhseID]
GO
CREATE PROC [dbo].[sp_MBL_GetEvoWhseID]
(
@1 varchar(max)
)
AS
SELECT [WhseLink]
FROM [WhseMst] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetLotTrackingID') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetLotTrackingID]
GO
CREATE PROC [dbo].[sp_MBL_GetLotTrackingID]
(
@1 varchar(max),
@2 int
)
AS
SELECT [idLotTracking]
FROM [_etblLotTracking] WHERE [cLotDescription] = @1 AND [iStockID] = @2
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetItemInfoForST') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetItemInfoForST]
GO
CREATE PROC [dbo].[sp_MBL_GetItemInfoForST]
(
@1 varchar(max)
)
AS
SELECT [StockLink],[Bar_Code]
FROM [StkItem] WHERE [Code] = @1
GO


IF (OBJECT_ID('[dbo].[sp_MBL_GetLotInEvo') IS NOT NULL)
	DROP PROC [dbo].[sp_MBL_GetLotInEvo]
GO
CREATE PROC [dbo].[sp_MBL_GetLotInEvo]
(
@1 varchar(max),
@2 varchar(max)
)
AS
SELECT l.[idLotTracking] 
FROM [_etblLotTracking] l INNER JOIN [StkItem] s ON l.[iStockID] = s.[StockLink]
WHERE l.[cLotDescription] = @1 AND s.[Code] = @2
GO








 





















































