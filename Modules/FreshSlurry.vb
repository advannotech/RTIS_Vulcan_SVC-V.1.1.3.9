Imports System.Data.SqlClient

Public Class FreshSlurry

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
"; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retreive

            Public Shared Function MBL_GetFreshSlurryInUse(ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vItemCode], [vLotNumber], [dWetWeight] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @1 AND [dSolidity] IS NULL 
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= trolleyCode + "|" + Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*" + ReturnData
                    Else
                        Return "1*No unfinished slurry found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No unfinished slurry found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInUse: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("    SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_FStMS] wl
                                                       INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
                                                       WHERE [bEnabled] = 1", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No warehouse set for powder prep!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for powder prep!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryLotNonManufactured(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("    SELECT [vLotNumber], s.[Description_1] FROM [tbl_RTIS_Fresh_Slurry] fs
                                                       INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[Code] = fs.[vItemCode]
                                                       WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @1 AND [vItemCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryLotNonManufacturedSaveSol(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_Fresh_Slurry]
                                                    WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryWeightNonManufacturedSaveSol(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [dWetWeight] FROM [tbl_RTIS_Fresh_Slurry]
                                                    WHERE ([bManuf] = 0 OR [bManuf] IS NULL) AND [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3  ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetWaitingFreshSlurries() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT [iLineID], [vTrolleyCode], [vItemCode], [vLotNumber], [dWetWeight], [dSolidity], [dDryWeight], [vUserEntered], [dtDateEntered], '', '' , '' 
                                                     FROM [tbl_RTIS_Fresh_Slurry] WHERE ISNULL([dSolidity], 0) <> 0 AND ISNULL([bManuf], 0) = 0", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryMF: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetFreshSlurryMF() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID],[vTrolleyCode],'' AS [Tank],[vItemCode],[vItemDesc],[vLotNumber],[dWetWeight],[dSolidity],[dDryWeight],[dtDateSol],ISNULL([bManuf], 0)
                                                   FROM [tbl_RTIS_Fresh_Slurry] WHERE ([bManuf] = '0' OR [bManuf] IS NULL) AND [dSolidity] is not null", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryMF: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetFreshSlurryRecords(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vTrolleyCode],
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
                                                WHERE [dtDateEntered] BETWEEN @1 AND @2
                                                ORDER BY [dtDateEntered] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "|" + Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "|" + Convert.ToString(sqlReader.Item(18)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No records found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No records found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryRecords: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function
            Public Shared Function UI_GetFSLinkExists(ByVal slurryCode As String, ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vRMCode] FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @1 AND [vRMCode] = @2", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*This item is already approved for this type of slurry"
                    Else
                        Return "1*No link found, add one"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFSLinkExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_GetSlurryReqRM(ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT TOP 1 [vRMCode] FROM  [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryReqRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_CheckLotNumberUsed(ByVal lotNumber As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vLotNumber] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vLotNumber] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*This lot number has already been used"
                    Else
                        Return "1*Ok to use"
                    End If
                Catch ex As Exception
                   If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Ok to use"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckLotNumberUsed: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryRMLink(ByVal itemCode As String, ByVal rCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM  [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @1 AND [vRMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*This powder is not allowed for the slurry you are starting!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryReqRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryHeaderID(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNumber))
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
                        Return "0*The headerID for this slurry could not be found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The headerID for this slurry could not be found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetFSRawMaterials(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT [vPowderCode], [vPowderLot] FROM [tbl_RTIS_Fresh_Slurry_Input] WHERE [iSlurryID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetZECTBatchTotal: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function MBL_InsertFreshSlurry(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal wetWeight As String, ByVal userName As String, ByVal desc As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_RTIS_Fresh_Slurry] ([vTrolleyCode], [vItemCode], [vLotNumber], [dWetWeight], [vUserEntered], [dtDateEntered], [vItemDesc])
                                                                               VALUES (@1, @2, @3, @4, @5, GETDATE(), @6)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", wetWeight.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@5", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@6", desc))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertFreshSlurry: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_InsertFreshSlurryRM(ByVal headerID As String, ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  INSERT INTO [tbl_RTIS_Fresh_Slurry_Input] ([iSlurryID], [vPowderCode], [vPowderLot], [dQty], [dtDateRecorded], [vUserRecorded])
                                                     VALUES (@1, @2, @3, @4, GETDATE(), @5)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@5", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertFreshSlurryRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_InsertRMLink(ByVal slurryCode As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_RTIS_Fresh_Slurry_Raws] ([vSlurryCode], [vRMCode], [vRMDesc], [vUserAdded], [dtDateAdded])
                                                   VALUES (@1, @2, @3, @4, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertRMLink: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function MBL_InvalidateSlurry(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_Fresh_Slurry] SET [dSolidity] = 0, [dDryWeight] = 0, [dtDateSol] = GETDATE()
                                                    , [vUserSol] = @4, [bManuf] = 1, [dtManufDate] = GETDATE(), [bTrans] = 1, [dtTrans] = GETDATE(), [vUserManuf] = @4, 
                                                    [bRecTrans] = 1, dtRecTrans = GETDATE(), [vUserRec] = @4
                                                    WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 AND [dSolidity] IS NULL ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setSlurrySolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_setSlurrySolidity(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal solidity As String, ByVal userName As String, ByVal dryWeight As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_Fresh_Slurry] SET [dSolidity] = @4, [dDryWeight] = @6, [vUserSol] = @5, [dtDateSol] = GETDATE()
                                                                               WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3
                    AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [tbl_RTIS_Fresh_Slurry] WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3 ORDER BY [iLineID] DESC)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", solidity.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@5", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@6", dryWeight.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setSlurrySolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function



            Public Shared Function UI_setFSManufactured(ByVal LineID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_Fresh_Slurry] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @2 WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", LineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setFSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


            Public Shared Function UI_setFSManufacturedManual(ByVal LineID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_Fresh_Slurry] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @2, [bManualManuf] =1 WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", LineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setFSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


        End Class
        Partial Public Class Delete
            Public Shared Function UI_DeleteRMLink(ByVal slurryCode As String, ByVal rmCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @1 AND [vRMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", slurryCode))
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
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function MBL_GetItemDesc(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [Description_1] FROM [StkItem]
                                                    WHERE [Code] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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

            Public Shared Function UI_GetAllFreshSlurries()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("   SELECT [Code],[Description_1],[Description_2], '' FROM [StkItem]
                                                      WHERE [ItemGroup] LIKE '%011%'", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    Dim builder As New Text.StringBuilder()
                    builder.Append(ReturnData)
                    While sqlReader.Read()
                        builder.Append(Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "~")
                    End While
                    ReturnData = builder.ToString()
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return If(ReturnData <> String.Empty, DirectCast("1*" + ReturnData, Object), DirectCast("0*Fresh slurries were found!", Object))
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllFreshSlurries: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetFreshSlurryRaws(ByVal slurryCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vRMCode], [vRMDesc], '' FROM [tbl_RTIS_Fresh_Slurry_Raws] WHERE [vSlurryCode] = @1", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", slurryCode))
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
                        Return "0*No raw materials found for the selected slurry"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryRaws: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllFreshSlurryRMs()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("   SELECT [Code],[Description_1], '' FROM [StkItem]
                                                      WHERE [ItemGroup] LIKE '%010%'", sqlConn)
                    'WHERE [Code] LIKE '18471%' OR [Code] LIKE 'TSP%' OR [Code] LIKE 'VSP%' OR [Code] LIKE '18461%' OR [Code] LIKE '%COAT%' OR [Code] LIKE '%Coat%'
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllFreshSlurryRMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class

        Partial Public Class Insert
            Public Shared Function UI_InsertFreshSlurryForManufacture(ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String, ByVal project As String) As String
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
        End Class
    End Class
End Class
