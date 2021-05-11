Imports System.Data.SqlClient

Public Class AutoManufacture
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTIS
        Partial Public Class Retreive
            Public Shared Function UI_GetManufProcs() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vProcName]
      ,[vSrcWhseCode]
      ,[vDestWhseCode]
      FROM [rtbl_ManufLocs]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No manufactre processes were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No manufactre processes were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetManufProcs: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetManufLocations(ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vSrcWhseCode], [vDestWhseCode] FROM [rtbl_ManufLocs] WHERE [vProcName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", process))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No manufacture locations wre found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No manufacture locations wre found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAutUI_GetManufLocationsoManufactureRecords: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class
    End Class
    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function UI_GetAutoManufactureRecords(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [ipkMFPImportID]
      ,[sBOMItemCode]
      ,[fQtyToProduce]
      ,[sLotNumber]
      ,[sProjectCode]
      ,[iStatusID]
      ,[dCreationDate]
      ,[dSyncDate]
      ,[sError]
      FROM [__SLtbl_MFPImports] WHERE [dCreationDate] BETWEEN @1 AND @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAutoManufactureRecords: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAutoManufactureItemError(ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("   SELECT [sError] FROM [__SLtbl_MFPImports] WHERE [ipkMFPImportID] = (SELECT TOP 1 [ipkMFPImportID] FROM [__SLtbl_MFPImports] WHERE [sBOMItemCode]= @1 AND [sLotNumber] = @2 AND [fQtyToProduce] = @3)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*" + ReturnData
                    Else
                        Return "1*"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAutoManufactureRecords: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetManufWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [Code]
      FROM [WhseMst]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Warehouses were found!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Warehouses were found!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetManufWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAutoManufactureHeaderID(ByVal itemCode As String, ByVal qty As String, ByVal src As String, ByVal dest As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("   SELECT TOP 1 [ipkAMPHeaderID] FROM [__SLtbl_AMPHeader] WHERE [sFinishedItemCode] = @1 AND [fQtyToManufacture] = @2 AND [sSourceWarehouseCode] = @3 AND [sDestinationWarehouseCode] = @4 AND [sDestinationLotNumber] = @5  ORDER BY [ipkAMPHeaderID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@3", src))
                    sqlComm.Parameters.Add(New SqlParameter("@4", dest))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No header data was found with the specified parameters when manufacturing"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No header data was found with the specified parameters when manufacturing"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAutoManufactureHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetAutoManufactureHeaderMSG(ByVal itemCode As String, ByVal qty As String, ByVal src As String, ByVal dest As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("   SELECT TOP 1 [sFeedback] FROM [__SLtbl_AMPHeader] WHERE [sFinishedItemCode] = @1 AND [fQtyToManufacture] = @2 AND [sSourceWarehouseCode] = @3 AND [sDestinationWarehouseCode] = @4 AND [sDestinationLotNumber] = @5  ORDER BY [ipkAMPHeaderID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@3", src))
                    sqlComm.Parameters.Add(New SqlParameter("@4", dest))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*" + ReturnData
                    Else
                        Return "0*No message was found with the specified parameters when manufacturing"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No message was found with the specified parameters when manufacturing"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAutoManufactureHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function UI_InsertAutorManufacture(ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String, ByVal project As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("INSERT INTO __SLtbl_MFPImports (sBOMItemCode, fQtyToProduce, sLotNumber, sProjectCode ) VALUES ( @1, @2, @3, @4)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", project))

                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderForManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertAutorManufacture_H(ByVal itemCode As String, ByVal qty As String, ByVal src As String, ByVal dest As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [__SLtbl_AMPHeader] ([sFinishedItemCode], [fQtyToManufacture], [sSourceWarehouseCode], [sDestinationWarehouseCode], [sDestinationLotNumber] ) VALUES ( @1, @2, @3, @4, @5)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@3", src))
                    sqlComm.Parameters.Add(New SqlParameter("@4", dest))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotNumber))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertAutorManufacture_H: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertAutorManufacture_L(ByVal hID As String, ByVal itemCode As String, ByVal src As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES ( @1, @2, @3, @4)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", hID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", src))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotNumber))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertAutorManufacture_L: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Update
            Public Shared Function UI_ProccessAutorManufactureTables() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("exec __SLsp_AutoManufactureFromTable", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ProccessAutorManufactureTables: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ProccessAutorManufacture() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("exec __SLtbl_MFPImport", sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ProccessAutorManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ProccessAutorManufacture_HL(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("exec __SLsp_AutoManufactureFromTable @1, 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ProccessAutorManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_UpdateProcessLoc(ByVal proc As String, ByVal src As String, ByVal dest As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [rtbl_ManufLocs] SET [vSrcWhseCode] = @2, [vDestWhseCode] = @3 WHERE [vProcName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", proc))
                    sqlComm.Parameters.Add(New SqlParameter("@2", src))
                    sqlComm.Parameters.Add(New SqlParameter("@3", dest))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateProcessLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Public Shared Function UI_ExecuteGeneric(ByVal query As String) As String
            Try
                Dim ReturnData As String = ""
                Dim sqlConn As New SqlConnection(EvoString)
                Dim sqlComm As New SqlCommand(query, sqlConn)
                sqlConn.Open()
                sqlComm.ExecuteNonQuery()
                sqlComm.Dispose()
                sqlConn.Close()
                Return "1*Success"
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ExecuteGeneric: " + ex.ToString())
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
    End Class
End Class
