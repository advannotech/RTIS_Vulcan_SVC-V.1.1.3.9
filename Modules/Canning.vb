Imports System.Data.SqlClient
Public Class Canning

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTSQL
        Partial Public Class Retrieve
#Region "UI"
            Public Shared Function UI_GetCanningCatalystRaws(ByVal catalystCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    'Dim sqlComm As New SqlCommand("SELECT [vRMCode], [vRMDesc], '' FROM [tbl_RTIS_Canning_Raws] WHERE [vItemCode] = @1", sqlConn)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningCatalystRaws] @vItemCode", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", catalystCode))
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No raw materials found for catalyst"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCanningCatalystRaws: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetCanningLinkExists(ByVal catalystCode As String, ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    'Dim sqlComm As New SqlCommand("SELECT [vRMCode] FROM [tbl_RTIS_Canning_Raws] WHERE [vItemCode] = @1 AND [vRMCode] = @2", sqlConn)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningLinkExists] @vItemCode, @vRMCode", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", catalystCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*This item is already approved for this type of catalyst"
                    Else
                        Return "1*No link found, add one"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCanningLinkExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetCanningRecords(ByVal dateFrom As String, ByVal dateTo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningRecords] @dateFrom, @dateTo", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@dateFrom", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@dateTo", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No A&W jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllAWJobs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetCanningLinesToManufacture()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningLinesToManufacture]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Canning products need to be manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCanningLineToManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetCanningRM(ByVal lineID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningRM] @iLIneID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iLIneID", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    Dim builder As New Text.StringBuilder()
                    builder.Append(ReturnData)
                    While sqlReader.Read()
                        builder.Append(Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~")
                    End While
                    ReturnData = builder.ToString()
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return If(ReturnData <> String.Empty, DirectCast("1*" + ReturnData, Object), DirectCast("1*", Object))
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCanningRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Tablet"
            Public Shared Function UI_GetCanningProducts(ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCanningProducts] @vRMCode", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No raw materials found for catalyst"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCanningProducts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Canning_GetLabelInfo(ByVal itemCode As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Canning_GetLabelInfo] @Code", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Code", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + sqlReader.Item(7) + "~" + unq
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No item info was pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No item info was pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Canning_GetLabelInfo_AWTags: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function Canning_GetReprintItemList() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Canning_GetReprintItemList]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No items were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No items were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Canning_GetReprintItemList: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function Canning_GetReprintLotList(ByVal itemCode As String, ByVal dateFrom As String, ByVal dateTo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Canning_GetReprintLotList] @vCanningCode, @dateFrom, @dateTo", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vCanningCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@dateFrom", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@dateTo", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lots were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Canning_GetReprintLotList: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Canning_GetPalletList(ByVal itemCode As String, ByVal lotNumber As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Canning_GetPalletList] @vCanningCode, @vLotNumber", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vCanningCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pllets were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Canning_GetPalletList: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Insert
#Region "UI"
            Public Shared Function UI_InsertRMLink(ByVal itemCode As String, ByVal itemDesc As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_RTIS_Canning_Raws] ([vItemCode], [vItemDesc], [vRMCode], [vRMDesc], [vUserAdded], [dtDateAdded])
                                                   VALUES (@1, @2, @3, @4, @5, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@3", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertRMLink: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Tablet"
            Public Shared Function UI_InsertCanningRecordk(ByVal jobNo As String, ByVal palletCode As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal rmQty As String, ByVal lotNumber As String, ByVal canningCode As String, ByVal canningDesc As String, ByVal qty As String, ByVal username As String, ByVal palletNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  INSERT INTO [tbl_RTIS_Canning_Out] ([vOldJobCode], [vPalletCode], [vRMCode], [vRMDesc], [vRMQty], [vLotNumber], [vCanningCode], [vCanningDesc], [vQty], [vUserAdded], [dtDateAdded], [vPalletNo])
                                                     VALUES (@1, @2, @3, @4, @5, @6, @7, @8, @9 , @10, GETDATE(), @11)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", palletCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@5", rmQty))
                    sqlComm.Parameters.Add(New SqlParameter("@6", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@7", canningCode))
                    sqlComm.Parameters.Add(New SqlParameter("@8", canningDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@9", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@10", username))
                    sqlComm.Parameters.Add(New SqlParameter("@11", palletNo))

                    sqlComm.Parameters.Add(New SqlParameter("@vOldJobCode", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@vPalletCode", palletCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMDesc", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMQty", rmQty))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@vCanningCode", canningCode))
                    sqlComm.Parameters.Add(New SqlParameter("vCanningDesc", canningDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@vQty", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserAdded", username))
                    sqlComm.Parameters.Add(New SqlParameter("@vPalletNo", palletNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertCanningRecordk: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
        End Class

        Partial Public Class Delete
#Region "UI"
            Public Shared Function UI_DeleteRMLink(ByVal itemCode As String, ByVal rmCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_RTIS_Canning_Raws] WHERE [vitemCode] = @1 AND [vRMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteRMLink: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Update
            Public Shared Function UI_UpdatePalletManufactured(ByVal lineID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdatePalletManufactured] @iLIneID, @vUserManuf", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iLIneID", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserManuf", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdatePalletManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_UpdatePalletManufacturedManual(ByVal lineID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdatePalletManufacturedManual] @iLIneID, @vUserManufManual", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iLIneID", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserManufManual", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdatePalletManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class


    Public Class Evolution
        Partial Public Class Retrieve

#Region "UI"
            Public Shared Function UI_GetAllCanningCatalysts()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    'Dim sqlComm As New SqlCommand("   SELECT [Code],[Description_1],[Description_2], '' FROM [StkItem]
                    '                                  WHERE [Code] LIKE '18450%'", sqlConn)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetAllCanningCatalysts]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No A&W catalysts found !"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllCanningCatalysts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllCanningWRMs()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    'Dim sqlComm As New SqlCommand("   SELECT [Code],[Description_1], '' FROM [StkItem]
                    '                                  WHERE [Code] LIKE '18471%' OR [Code] LIKE '18461%' OR [Code] LIKE '%A&W%' OR ([Code] LIKE 'V%' AND [Code] NOT LIKE 'VSP%')", sqlConn)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetAllCanningWRMs]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllCanningWRMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Tablet"
            Public Shared Function MBL_GetItemDesc(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    'Dim sqlComm As New SqlCommand(" SELECT [Description_1] FROM [StkItem]
                    '                                WHERE [Code] = @1", sqlConn)
                    'sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetItemDesc: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region
        End Class
    End Class

End Class
