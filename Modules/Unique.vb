Imports System.Data.SqlClient

Public Class Unique
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Class RTSQL
        Partial Public Class Retreive
            Public Shared Function PGM_GetBarcodeFromVault(ByVal barcode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT ISNULL([bFromVault], 'False') FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", barcode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetItemReceivedTransfer(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT ISNULL([bTransferredIn], 'False') FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetItemConsumed(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT ISNULL([bConsumed], 'False') FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetItemDispatched(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [Dispatch] FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPalletID(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [iLine_ID] FROM [htbl_PalletBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPalletID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPalletDispatched(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [Dispatch] FROM [htbl_PalletBarcodes]
                                                        WHERE [iLine_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPalletDispatched: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPalletBoxBarcodes(ByVal palletID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [vUnqBarcode] FROM [ltbl_PalletBarcodes]
                                                        WHERE [iPallet_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", palletID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPalletDispatched: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetItemTransferredOut(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT ISNULL([bTransferredOut], 'false') FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetItemTransferredOut: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function ZECT_GetZectUnq(ByVal itemCode As String, ByVal jobNO As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [vUnqBarcode] FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + jobNO + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZectUnq: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function UI_GetPalletID(ByVal Unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELEcT [iLine_ID] FROM [htbl_PalletBarcodes]
                                                        WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", Unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPalletID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetSTUnqs(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([StockTake], ''), ISNULL([StockTake2], '') FROM [tbl_unqBarcodes]
                                                        WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetSTPalletLots(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
                                                    INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
                                                    WHERE h.[vUnqBarcode] = @UNQ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@UNQ", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        Dim _unq As String = Convert.ToString(sqlReader.Item(0))
                        ReturnData += Barcodes.GetItemLot(_unq) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No lot numbers were found for the scanned pallet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No lot numbers were found for the scanned pallet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSTPalletLots: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetSTPalletUnqs(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([StockTake], ''), ISNULL([StockTake2], '') FROM [htbl_PalletBarcodes]
                                                        WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSTPalletUnqs: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function UI_GetItemOnPallet(ByVal Unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLine_ID] FROM [ltbl_PalletBarcodes] WHERE [vUnqBarcode] = @UNQ AND [bOnPallet] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@UNQ", Unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This item is already on a pallet please use the pallet breakdown app."
                    Else
                        Return "1*Ok to add"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*This item is already on a pallet please use the pallet breakdown app."
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPalletID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetPalletContents(ByVal palletUnq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l 
                                                    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
                                                    WHERE h.[vUnqBarcode] = @UNQ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@UNQ", palletUnq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No items were found for this pallet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No items were found for this pallet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetPalletLots(ByVal palletUnq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l 
                                                    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
                                                    WHERE h.[vUnqBarcode] = @UNQ AND l.[bOnPallet] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@UNQ", palletUnq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        Dim unq As String = Convert.ToString(sqlReader.Item(0))
                        ReturnData += Barcodes.GetItemLot(unq) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No items were found for this pallet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No items were found for this pallet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function UI_SaveRT2DBarcode(ByVal unqCode As String, ByVal validateRef As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_unqBarcodes] ([vUnqBarcode]
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
                                                VALUES (@1, NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(), NULL, NULL, 0, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unqCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", validateRef))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_SaveRT2DBarcode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_SaveRT2DBarcodePallet(ByVal unqCode As String, ByVal validateRef As String, ByVal jobFrom As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [htbl_PalletBarcodes] ([vUnqBarcode]
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
                                                VALUES (@1, NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, @2, 0, 0, @3)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unqCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", validateRef))
                    sqlComm.Parameters.Add(New SqlParameter("@3", jobFrom))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_SaveRT2DBarcode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_SaveRMBarcodePallet(ByVal unqCode As String, ByVal validateRef As String, ByVal jobFrom As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [htbl_PalletBarcodes] ([vUnqBarcode]
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
                                                VALUES (@1, NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, @2, 0, 0, @3, 1)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unqCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", validateRef))
                    sqlComm.Parameters.Add(New SqlParameter("@3", jobFrom))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The pallet information could not be saved"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_SaveRMBarcodePallet: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GenericInsert(ByVal comm As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(comm, sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GenericInsert: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function UI_ClearUnqsDispatch(ByVal soNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_unqBarcodes] SET [Dispatch] = NULL, [DispatchDate] = NULL WHERE [Dispatch] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", soNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ClearUnqsDispatch: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function UI_ClearUnqPalletsDispatch(ByVal soNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_PalletBarcodes] SET [Dispatch] = NULL, [DispatchDate] = NULL WHERE [Dispatch] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", soNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ClearUnqPalletsDispatch: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function UI_UpdateTransFromVault(ByVal unqCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_unqBarcodes] SET [bFromVault] = 1 WHERE  [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unqCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateTransFromVault: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function

            Public Shared Function MBL_UpdateItemReceived(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [bTransferredIn] = 1, [bTransferredOut] = 0 
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemConsumed(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String, ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [bConsumed] = 1, [vJobConsumed] = @5
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@5", jobNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemReceivedAndConsumed(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [bConsumed] = 1, [bTransferredIn] = 1, [bTransferredOut] = 0 
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function

            Public Shared Function MBL_UpdateItemTransferredOut(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [bTransferredIn] = 0, [bTransferredOut] = 1 
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateBoxesDispatched(ByVal soNumber As String, ByVal clause As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [Dispatch] = @1, [DispatchDate] = GETDATE()
                                                    " + clause, sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", soNumber))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemDispatched(ByVal soNumber As String, ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [Dispatch] = @5, [DispatchDate] = GETDATE()
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@5", soNumber))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdatePalletDispatched(ByVal soNumber As String, ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [htbl_PalletBarcodes] SET [Dispatch] = @1, [DispatchDate] = GETDATE()
                                                    WHERE [iLine_ID] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", soNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lineID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemReceived: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function

            Public Shared Function MBL_UpdateItemST1(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [StockTake] = @5
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@5", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemST1: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemST2(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [StockTake2] = @5
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@5", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemST1: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdatePalletST1(ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [htbl_PalletBarcodes] SET [StockTake] = @2
                                                    WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@2", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePalletST1: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdatePalletST2(ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [htbl_PalletBarcodes] SET [StockTake2] = @2
                                                    WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@2", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePalletST2: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdatePalletSTBoth(ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [htbl_PalletBarcodes] SET [StockTake] = @2, [StockTake2] = @2
                                                    WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@2", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePalletSTBoth: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemSTBoth(ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal unq As String, ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [StockTake] = @5, [StockTake2] = @5
                                                    WHERE [vUnqBarcode] LIKE @1 AND [vUnqBarcode] LIKE @2 AND [vUnqBarcode] LIKE @3 AND [vUnqBarcode] LIKE @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + lot + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@3", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@4", "%" + unq + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@5", stNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemST1: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateItemSTBoth_Reversal(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_unqBarcodes] SET [StockTake] = '', [StockTake2] = ''
                                                    WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemSTBoth_Reversal: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function

            Public Shared Function MBL_UpdateRMPalletRemoved(ByVal hunq As String, ByVal lunq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE l SET l.[bOnPallet] = 0
                                                    FROM [ltbl_PalletBarcodes] l 
                                                    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
                                                    WHERE h.[vUnqBarcode] = @HUNQ AND l.[vUnqBarcode] = @LUNQ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@HUNQ", hunq))
                    sqlComm.Parameters.Add(New SqlParameter("@LUNQ", lunq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateRMPalletRemoved: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function

            Public Shared Function MBL_UpdateItemSTPalletBoth_Reversal(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [htbl_PalletBarcodes] SET [StockTake] = '', [StockTake2] = ''
                                                    WHERE [vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateItemSTBoth_Reversal: " + ex.ToString())
                    Return "-1*" + ex.Message
                End Try
            End Function
        End Class
    End Class
End Class
