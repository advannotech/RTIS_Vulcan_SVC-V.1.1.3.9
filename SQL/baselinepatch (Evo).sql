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





































