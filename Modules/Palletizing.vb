Imports System.Data.SqlClient

Public Class Palletizing
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
  "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTIS
        Partial Public Class Retreive
            Public Shared Function UI_GetPalletPrintSettings() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletPrintSettings]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallet print settings were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallet print settings were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPalletPrintSettings: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPalletPrintSettings() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetPalletPrintSettings]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallet print settings were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallet print settings were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPalletPrintSettings: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPallets() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPallets]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets with this item loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this item loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItem: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletsByDate(ByVal from As String, ByVal _to As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPallets] @from, @_to", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@from", from))
                    sqlComm.Parameters.Add(New SqlParameter("@_to", _to))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets with this item loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this item loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItemAndDate: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletsByItem(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletsByItem] @itemCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@itemCode", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets with this item loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this item loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItem: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletsByItemAndDate(ByVal itemCode As String, ByVal from As String, ByVal _to As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletsByItemAndDate] @itemCode, @from, @_to", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@itemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@from", from))
                    sqlComm.Parameters.Add(New SqlParameter("@_to", _to))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets with this item loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this item loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItemAndDate: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletsByLot(ByVal lot As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletsByLot] @lot", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@lot ", lot))
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
                        Return "0*No pallets with this lot loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this lot loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItem: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletsByLotAndDate(ByVal lot As String, ByVal from As String, ByVal _to As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iPallet_ID] FROM [ltbl_PalletBarcodes] l 
                                                    INNER JOIN [htbl_PalletBarcodes] h ON l.[iPallet_ID] = h.[iLine_ID]
                                                    WHERE l.[vUnqBarcode] LIKE '%' + @LOT + '%' AND h.[Printed] BETWEEN @FROM AND @TO AND h.[bRMPallet] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@LOT", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@FROM", from))
                    sqlComm.Parameters.Add(New SqlParameter("@TO", _to))
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
                        Return "0*No pallets with this lot loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this lot loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByItem: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletLots(ByVal palletId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletLots] @palletId", sqlConn)
                    'Dim sqlComm As New SqlCommand(" SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@palletId", palletId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        Dim unq As String = Convert.ToString(sqlReader.Item(0))
                        ReturnData += Barcodes.GetItemLot(unq) + ","
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
            Public Shared Function UI_GetPalletsByIDList(ByVal ids As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand($"  SELECT [iLine_ID], [Printed], [vUnqBarcode] FROM [htbl_PalletBarcodes]
                                                     WHERE [iLine_ID] IN {ids}", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets with this lot loaded were found."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets with this lot loaded were found."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetPalletsByIDList: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPalletLines(ByVal palletId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPalletLines] @palletId", sqlConn)
                    'Dim sqlComm As New SqlCommand(" SELECT l.[vUnqBarcode], l.[bOnPallet] FROM [ltbl_PalletBarcodes] l WHERE l.[iPallet_ID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@palletId", palletId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        Dim unq As String = Convert.ToString(sqlReader.Item(0))
                        ReturnData += Barcodes.GetItemLot(unq) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
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

        Partial Public Class Update
            Public Shared Function UI_UpdatePalletPrinSettings(ByVal printer As String, ByVal label As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTSettings] SET [SettingValue] = @VALUE WHERE [Setting_Name] = 'Pallet Printer'", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@VALUE", printer))

                    Dim sqlComm2 As New SqlCommand(" UPDATE [tbl_RTSettings] SET [SettingValue] = @VALUE2 WHERE [Setting_Name] = 'Pallet Label'", sqlConn)
                    sqlComm2.Parameters.Add(New SqlParameter("@VALUE2", label))

                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm2.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdatePalletPrinSettings: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function MBL_GetItemDesc(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetItemDesc] @Code", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Code", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "1*"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetItemInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [Bar_Code],[cSimpleCode],[Description_1],[Description_2],[Description_3] FROM [StkItem] WHERE [Code] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*This item was not found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*This item was not found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetEvoStockTakes: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function UI_GetItemCodes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCodes]", sqlConn)
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
                        Return "0*No item codes were found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No item codes were found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetItemCodes: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function UI_GetItemLots(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemLots] @itemCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@itemCode", itemCode))
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
                        Return "0*No lots were found for this item in evolution."
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lots were found for this item in evolution."
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetItemLots: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

        End Class
    End Class
End Class
