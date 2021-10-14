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


















































