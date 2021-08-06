Imports System.Data.SqlClient
Imports Pastel.Evolution

Public Class PGM
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
           "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retreive

#Region "Tablet"
            Public Shared Function PGM_GetItemBatch(ByVal itemCode As String, ByVal lot As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration] FROM [htbl_RTIS_PGM_Manuf]
                                                    WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Insert new record"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckContainerUsed_Global(ByVal container As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [bTransferred] FROM [ltbl_RTIS_PGM_Manuf]
                                                    WHERE [vContainer] = @1 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                    'If ReturnData <> String.Empty Then

                    'Else
                    '    Return "1*True"
                    'End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckContainerUsed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckContainerUsed(ByVal headerID As String, ByVal container As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vContainer] FROM [ltbl_RTIS_PGM_Manuf]
                                                    WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*Container in use"
                    Else
                        Return "1*Insert new record"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckContainerUsed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_CheckContTransState(ByVal container As String, ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT TOP 1 ISNULL([bTransferred], 0) FROM [ltbl_RTIS_PGM_Manuf] WHERE [vContainer] = @1 AND [iHeaderID] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlComm.Parameters.Add(New SqlParameter("@2", headerID))
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
                        Return "0*The Container is already in use by this batch and has not been transferred out."
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckContRecState: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function PGM_CheckContRecState_Global(ByVal container As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT TOP 1 ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans] WHERE [vContainer] = @1 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
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
                        Return "1*True"
                    End If

                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The Container is already in use by this batch and has not been transferred out."
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckContRecState: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function PGM_CheckContRecState(ByVal container As String, ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT TOP 1 ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans] WHERE [vContainer] = @1 AND [iHeaderID] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlComm.Parameters.Add(New SqlParameter("@2", headerID))
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
                        Return "0*The Container is already in use by this batch and has not been transferred out."
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckContRecState: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function PGM_GetRemainderCaptured(ByVal itemCode As String, ByVal lot As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([bRemainderSet], 0) FROM [htbl_RTIS_PGM_Manuf]
	                                                WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
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
                        Return "1*false"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetRemainderCaptured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetBatchLines(ByVal itemCode As String, ByVal lot As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT pl.[vContainer], pl.[dWeightIn], ISNULL(pl.[bManuf], 0), ISNULL(pl.[bTransferred], 0) FROM [ltbl_RTIS_PGM_Manuf] pl 
	                                                INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	                                                WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2 AND ph.[vWhseCode] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
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
                        Return "0*No lines found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetHeaderID(ByVal itemCode As String, ByVal lot As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [htbl_RTIS_PGM_Manuf]
                                                    WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @3 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
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
                        Return "0*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetContOldInfo(ByVal cont As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 tl.[dWeightOut], tl.[vWhseTo], tl.[vDestWhse], tl.[iLineID], h.[vItemCode], h.[vLotDesc] FROM [ltbl_RTIS_PGM_Trans] tl 
                                                     INNER JOIN [htbl_RTIS_PGM_Manuf] h ON h.[iLineID] = tl.[iHeaderID]
                                                     WHERE [vContainer] = @1
                                                     ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", cont))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No previous information was found for this container"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetContOldInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_SelectVWTransferSlurries()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vSlurryCode] FROM [rtbl_VW_Slurry_PGM]", sqlConn)
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
                        Return "0*No slurries have been setup, please configure the slurries using the front end application"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_SelectVWTransferSlurries: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_SelectTTransferPowders()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vPowderCode] FROM [rtbl_T_Powder_PGM]", sqlConn)
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
                        Return "0*No powders have been setup, please configure the powders using the front end application"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_SelectTTransferPowders: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_SelectTTransferSlurries()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vSlurryCode] FROM [rtbl_T_Slurry_PGM]", sqlConn)
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
                        Return "0*No slurries have been setup, please configure the slurries using the front end application"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_SelectTTransferSlurries: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_SelectTTransferAW()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vCatalystCode] FROM [rtbl_T_Catalyst_PGM]", sqlConn)
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
                        Return "0*No catalysts have been setup, please configure the catalysts using the front end application"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_SelectTTransferAW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckItemAllowedVW(ByVal manufItem As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [rtbl_VW_Slurry_PGM] WHERE [vSlurryCode] = @1 AND [vPGMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", manufItem))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*The scanned raw material cannot be used to manufacture the selected item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemAllowedVW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckItemAllowedTP(ByVal manufItem As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [rtbl_T_Powder_PGM] WHERE [vPowderCode] = @1 AND [vPGMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", manufItem))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*The scanned raw material cannot be used to manufacture the selected item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemAllowedVW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckItemAllowedTS(ByVal manufItem As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [rtbl_T_Slurry_PGM] WHERE [vSlurryCode] = @1 AND [vPGMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", manufItem))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*The scanned raw material cannot be used to manufacture the selected item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemAllowedVW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckItemAllowedTAW(ByVal manufItem As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [rtbl_T_Catalyst_PGM] WHERE [vCatalystCode] = @1 AND [vPGMCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", manufItem))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*The scanned raw material cannot be used to manufacture the selected item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemAllowedVW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetVWWIPLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMVW'", sqlConn)
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
                        Return "-1*The setting PGMVW was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetVWWIPLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetVWDestLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMVW-WIP'", sqlConn)
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
                        Return "-1*The setting PGMVW was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetVWDestLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetTPWIPLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTP'", sqlConn)
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
                        Return "-1*The setting PGMTP was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTPWIPLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetTPDestLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTP-WIP'", sqlConn)
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
                        Return "-1*The setting PGMTP was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTPDestLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_GetTSWIPLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTS'", sqlConn)
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
                        Return "-1*The setting PGMTS was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTSWIPLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetTSDestLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTS-WIP'", sqlConn)
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
                        Return "-1*The setting PGMTS was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTSDestLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetTAWWIPLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTAW'", sqlConn)
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
                        Return "-1*The setting PGMTAW was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTAWWIPLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetTAWDestLoc()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'PGMTAW-WIP'", sqlConn)
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
                        Return "-1*The setting PGMTAW was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetTAWDestLoc: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetContainerInfo(ByVal container As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 ph.[vItemCode], ph.[vLotDesc], pl.[dWeightIn], ph.[iLineID] FROM [ltbl_RTIS_PGM_Manuf] pl 
	                                                INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
	                                                WHERE pl.[vContainer] = @1 AND ph.[vWhseCode] = @2 ORDER BY pl.[iLineID] DESC  ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlComm.Parameters.Add(New SqlParameter("@2", location))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No reference for the container was found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetItemLocQty(ByVal itemCode As String, ByVal lot As String, ByVal location As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [dWeightBal] FROM [htbl_RTIS_PGM_Manuf]
                                                    WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vPGMLoc] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
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
                        Return "-1*Lot not found for location: " + location
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemLocQty: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_CheckItemTransferred(ByVal container As String, ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT TOP 1 [bReceived] FROM [ltbl_RTIS_PGM_Trans] WHERE [vContainer] = @1 AND [iHeaderID] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlComm.Parameters.Add(New SqlParameter("@2", headerID))
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
                        Return "1*True"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetAllPGM(ByVal whseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vItemCode] FROM [htbl_RTIS_PGM_Manuf] WHERE [vWhseCode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseCode))
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
                        Return "0*No PGM items were found for this location"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetAllPGMItemHeaders(ByVal itemCode As String, ByVal whseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT [vLotDesc], [dConcentration], [dWeightIn], [dWeightOut], [dWeightBal]
                                                      FROM [htbl_RTIS_PGM_Manuf] WHERE [vItemCode] = @1 AND [vWhseCode] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM items were found for this location"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetAllPGMItemHeaders: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetAllPGMItemHeaderRows(ByVal itemCode As String, ByVal whseCode As String, ByVal rowCount As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT TOP " + rowCount + " [vLotDesc], [dConcentration], [dWeightIn], [dWeightOut], [dWeightBal]
                                                      FROM [htbl_RTIS_PGM_Manuf] WHERE [vItemCode] = @1 AND [vWhseCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM items were found for this location"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetAllPGMItemHeaders: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetAllPGMItemTransactions(ByVal itemCode As String, ByVal lotNumber As String, ByVal whseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT pl.[vUserAdded], pl.[dtDateAdded], pl.[vContainer],ph.[dConcentration], pl.[dWeightIn], pl.[dWeightOut], ph.[vLotDesc], pl.[dtDateAdded]  FROM [ltbl_RTIS_PGM_Manuf] pl 
                                                    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
                                                    WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2 AND  ph.[vWhseCode] = @3
                                                    UNION
                                                    SELECT pl.[vUserAdded],  pl.[dtDateAdded], pl.[vContainer],ph.[dConcentration], pl.[dWeightIn], pl.[dWeightOut], ph.[vLotDesc], pl.[dtDateAdded]  FROM [ltbl_RTIS_PGM_Trans] pl 
                                                    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
                                                    WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2 AND  ph.[vWhseCode] = @3
                                                    ORDER BY pl.[dtDateAdded] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
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
                        Return "0*No PGM items were found for this location"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_CheckItemTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetContainerInfoForRecTrans(ByVal container As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT  TOP 1 ph.[vItemCode], ph.[vLotDesc], pl.[dWeightOut], s.[Description_1], ISNULL([bReceived], 0), ph.[iLineID] FROM [ltbl_RTIS_PGM_Trans] pl 
	                                                INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
													INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[Code] = ph.[vItemCode]
	                                                WHERE pl.[vContainer] = @1 AND ph.[vItemCode] = @2 ORDER BY ph.[iLineID] DESC  ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No outgoing transfer reference found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetContainerInfoForRecTrans: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetPGMHeaderID(ByVal itemCode As String, ByVal lotNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [htbl_RTIS_PGM_Manuf] WHERE [vItemCode] = @1 AND [vLotDesc] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNo))
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
                        Return "-1*PGM batch not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetPGMHeaderID: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetPGMReceived(ByVal headerID As String, ByVal contNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT ISNULL([bReceived], 0) FROM [ltbl_RTIS_PGM_Trans]
													WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", contNo))
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
                        Return "-1*PGM batch not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetPGMReceived: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_GetCheckContainerManuf(ByVal headerID As String, ByVal contNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT ISNULL([bManuf], 0) FROM [ltbl_RTIS_PGM_Manuf] 
													WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", contNo))
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
                        Return "0*PGM container not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetCheckContainerManuf: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "UI"
            Public Shared Function UI_GetPGMJobLines()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	  SELECT [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
                                                      FROM [htbl_RTIS_PGM_Manuf]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM batches found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMJobLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMJobrows(ByVal rowCount As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	  SELECT TOP " + rowCount + " [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
                                                      FROM [htbl_RTIS_PGM_Manuf] ORDER BY [iLineID] DESC", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM batches found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMJobLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMJobsByDate(ByVal dateFrom As String, ByVal dateTo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	  SELECT [vItemCode], [vLotDesc], [vWhseCode], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [dtDateAdded]
                                                      FROM [htbl_RTIS_PGM_Manuf] WHERE [dtDateAdded] BETWEEN @1 AND @2 ORDER BY [iLineID] DESC, [dtDateAdded] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM batches found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMJobsByDate: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMContainers(ByVal itemCode As String, ByVal lotNumber As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	    SELECT pl.[vContainer], pl.[dWeightIn], pl.[vUserAdded], pl.[dtDateAdded], ISNULL( pl.[bManuf], 'false'), pl.[dtManufDate], pl.[vUserManuf], pl.[vUserEdited], pl.[dtDateEdited], pl.[vEditReason]
                                                        FROM [ltbl_RTIS_PGM_Manuf] pl
                                                        INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
                                                        WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PGM batches found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMJobLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMManufactureHeaders()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT ph.[vItemCode], ph.[vLotDesc], ph.[vWhseCode], ph.[dConcentration], SUM(pl.[dWeightIn]) AS [Qty Waiting], ISNULL(ph.[bRemainderSet], 1) , ISNULL(ph.[dRemainder], 0) FROM [ltbl_RTIS_PGM_Manuf] pl
                                                    INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
                                                    WHERE ISNULL([bManuf], 0) = 0 GROUP BY ph.[vItemCode], ph.[vLotDesc],ph.[vWhseCode], ph.[dConcentration], ph.[bRemainderSet], ph.[dRemainder]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No new PGM batches need to be manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMManufactureHeaders: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMManufactureContainers(ByVal itemCode As String, ByVal lotNumber As String, ByVal location As String, ByVal concentration As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	    SELECT pl.[vContainer], pl.[dWeightIn], pl.[vUserAdded], pl.[dtDateAdded], '' AS [Manuf], '' AS [Edit]
                                                        FROM [ltbl_RTIS_PGM_Manuf] pl
                                                        INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON ph.[iLineID] = pl.[iHeaderID]
                                                        WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2 AND ph.[vWhseCode] = @3 AND ph.[dConcentration] = @4 AND ISNULL(pl.[bManuf], 0) = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
                    sqlComm.Parameters.Add(New SqlParameter("@4", concentration.Replace(",", ".")))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No containers found for this PGM batche"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMManufactureContainers: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetPGMBatchTotal(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT SUM(l.[dWeightIn]) + ISNULL(h.[dRemainder], 0) AS [Total] FROM [ltbl_RTIS_PGM_Manuf] l
                                                            INNER JOIN [htbl_RTIS_PGM_Manuf] h ON h.[iLineID] = l.[iHeaderID]
                                                            WHERE ISNULL([bManuf], 0) = 0 AND [iHeaderID] = @1
                                                            GROUP BY h.[dRemainder]", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
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
                        Return "0*No new PGM batches need to be manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMBatchTotal: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            
#End Region

        End Class

        Partial Public Class Insert
            Public Shared Function PGM_AddWhseTransferLog(ByVal itemCode As String, ByVal lotNumber As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String, ByVal username As String, ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [stbl_WHTLog] ([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [dQtyTransfered], [dtDateTransfered], [vUsername], [vProcess])
                                                   VALUES (@1, @2, @3, @4, @5, GETDATE(), @6, @7)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlComm.Parameters.Add(New SqlParameter("@7", process))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_AddWhseTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_ManufactureHeader(ByVal itemCode As String, ByVal lotNumber As String, ByVal wIn As String, ByVal wBal As String, ByVal concentration As String, ByVal location As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [htbl_RTIS_PGM_Manuf] ([vItemCode], [vLotDesc], [dWeightIn], [dWeightOut], [dWeightBal], [dConcentration], [vWhseCode], [dtDateAdded], [vUserAdded]) OUTPUT INSERTED.iLineID
                                                   VALUES (@1, @2, @3, 0, @4, @5, @6, GETDATE(), @7)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", wIn))
                    sqlComm.Parameters.Add(New SqlParameter("@4", wBal))
                    sqlComm.Parameters.Add(New SqlParameter("@5", concentration))
                    sqlComm.Parameters.Add(New SqlParameter("@6", location))
                    sqlComm.Parameters.Add(New SqlParameter("@7", userName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnData
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_AddWhseTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_ManufactureLine(ByVal HeaderID As String, ByVal container As String, ByVal weightIn As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [ltbl_RTIS_PGM_Manuf] ([iHeaderID],[vContainer], [dWeightIn], [dWeightOut], [dtDateAdded], [vUserAdded], [bTransferred])
                                                                              VALUES (@1, @2, @3, 0, GETDATE(), @4, 0)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", HeaderID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", weightIn))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_AddWhseTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_AddTransferOutLine(ByVal HeaderID As String, ByVal container As String, ByVal weightOut As String, ByVal userName As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal manufItem As String, ByVal manufBatch As String, ByVal dsetWhse As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [ltbl_RTIS_PGM_Trans] ([iHeaderID],[vContainer], [dWeightIn], [dWeightOut], [dtDateAdded], [vUserAdded], [vWhseFrom], [vWhseTo], [ManufItem], [ManufBatch], [vDestWhse])
                                                                              VALUES (@1, @2, 0, @3, GETDATE(), @4, @5, @6,@7, @8, @9)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", HeaderID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", weightOut))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@5", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@6", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@7", manufItem))
                    sqlComm.Parameters.Add(New SqlParameter("@8", manufBatch))
                    sqlComm.Parameters.Add(New SqlParameter("@9", dsetWhse))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_AddTransferOutLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class

        Partial Public Class Update
            Public Shared Function PGM_updateContRec(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_updateHeader(ByVal itemCode As String, ByVal lotNumber As String, ByVal wIn As String, ByVal location As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @3, [dWeightBal] = [dWeightBal] + @3 WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", wIn))
                    sqlComm.Parameters.Add(New SqlParameter("@4", location))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_updateManufOut(ByVal cont As String, ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [bTransferred] = 1 WHERE [vContainer] = @1 AND [iHeaderID] = @2 AND [iLineID] = (SELECT TOP 1 [iLineID] FROM [ltbl_RTIS_PGM_Manuf] WHERE [vContainer] = @1 AND [iHeaderID] = @2 ORDER BY [iLineID] DESC)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", cont))
                    sqlComm.Parameters.Add(New SqlParameter("@2", headerID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateManufOut: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function PGM_updateHeaderQtyOut(ByVal itemCode As String, ByVal lotNumber As String, ByVal wOut As String, ByVal location As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_RTIS_PGM_Manuf] SET [dWeightOut] = [dWeightOut] + @3, [dWeightBal] = [dWeightBal] - @3 WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", wOut.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", location))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_updateContQtyHeader(ByVal itemCode As String, ByVal lotNumber As String, ByVal wIn As String, ByVal location As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @3, [dWeightBal] = [dWeightBal] + @3 WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", wIn))
                    sqlComm.Parameters.Add(New SqlParameter("@4", location))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateContQtyHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_updateContQtyLine(ByVal headerID As String, ByVal container As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @3 WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateContQtyLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_setBatchRemainder(ByVal remQty As String, ByVal itemCode As String, ByVal lot As String, ByVal con As String, ByVal whse As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  UPDATE [htbl_RTIS_PGM_Manuf] SET [dRemainder] = @1, [bRemainderSet] = 1, [dtRemainderSet] = GETDATE(), [vRemainderUser] = @6
                                                            WHERE [vItemCode] = @2 AND [vLotDesc] = @3 AND [dConcentration] = @4 AND [vWhseCode] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", remQty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", con))
                    sqlComm.Parameters.Add(New SqlParameter("@5", whse))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_setBatchRemainder: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


            Public Shared Function UI_updateContQtyLine(ByVal headerID As String, ByVal container As String, ByVal qty As String, ByVal username As String, ByVal reason As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [dWeightIn] = [dWeightIn] + @3, [vUserEdited] = @4, [dtDateEdited] = GETDATE(), [vEditReason] = @5  WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", reason))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateContQtyLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_updateContReceived_PPRec(ByVal container As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 WHERE [iLineID] = (SELECT TOP 1 [iLineID] FROM [ltbl_RTIS_PGM_Trans] WHERE [vContainer] = @1 ORDER BY [iLineID] DESC)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", container))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateContReceived_PPRec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_updateContReceived(ByVal headerID As String, ByVal container As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Trans] SET [bReceived] = 1 WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateContReceived: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function PGM_updateContManufactured(ByVal itemCode As String, ByVal lotNumber As String, ByVal container As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  UPDATE pl SET pl.[bManuf] = 1 FROM [ltbl_RTIS_PGM_Manuf] pl
                                                     INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
                                                     WHERE ph.[vItemCode] = @1 AND ph.[vLotDesc] = @2 AND pl.[vContainer] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", container))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_updateContManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_setPGMContainerManufactured(ByVal headerID As String, ByVal container As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @3 WHERE [iHeaderID] = @1 AND [vContainer] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setPGMContainerManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_setPGMBatchManufactured(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDate] = GETDATE(), [vUserManuf] = @2 WHERE [iHeaderID] = @1 AND ISNULL( [bManuf] , 0) = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setPGMBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_updateContainerRem(ByVal itemCode As String, ByVal lotNumber As String, ByVal location As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  UPDATE [htbl_RTIS_PGM_Manuf] SET [dRemainder] = @4, [dtRemainderUpdated] = GETDATE(), [vRemUpdateUser] = @5
                                                            WHERE [vItemCode] = @1 AND [vLotDesc] = @2 AND [vWhseCode] = @3 ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@3", location))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateContainerRem: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_setPGMBatchManufacturedManual(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_RTIS_PGM_Manuf] SET [bManuf] = '1', [dtManufDateManual] = GETDATE(), [vUserManufManual] = @2 WHERE [iHeaderID] = @1 AND ISNULL( [bManuf] , 0) = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setPGMBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class Evolutiom
        Partial Public Class Retreive

            Public Shared Function PGM_GetItemFVDescription(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [Description_1] FROM [StkItem] WHERE [Code] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()

                    Dim sqlReaderPGM As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReaderPGM.Read()
                        ReturnData = Convert.ToString(sqlReaderPGM.Item(0))
                    End While
                    sqlReaderPGM.Close()

                    sqlComm.Dispose()

                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Description found for PGM item"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Description found for PGM item"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemFVDescription: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function PGM_GetItemRecDescription(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [Description_1] FROM [StkItem] WHERE [Code] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()

                    Dim sqlReaderPGM As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReaderPGM.Read()
                        ReturnData = Convert.ToString(sqlReaderPGM.Item(0))
                    End While
                    sqlReaderPGM.Close()

                    sqlComm.Dispose()

                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Description found for PGM item"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Description found for PGM item"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemRecDescription: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function PGM_GetItemLots(ByVal code As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT l.[cLotDescription] FROM [_etblLotTrackingQty] lq
                                                 INNER JOIN [_etblLotTracking] l ON l.[idLotTracking] = lq.[iLotTrackingID] 
                                                 INNER JOIN [_etblLotStatus] ls ON l.[iLotStatusID] = ls.[idLotStatus]
                                                 INNER JOIN [WhseMst] w ON w.[WhseLink] = lq.[iWarehouseID]
                                                 INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
                                                 INNER JOIN [" + My.Settings.RTDB + "].[dbo].[htbl_RTIS_PGM_Manuf] rl ON rl.[vLotDesc] COLLATE Latin1_General_CI_AS = l.[cLotDescription]
												 WHERE s.[Code] = @1 AND w.[Code] = @2 AND lq.[fQtyOnHand] <> 0 AND rl.[vPGMLoc] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", code))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whse))
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
                        Return "-1*No lots were found for this item that had a qty above 0"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Insert
            Public Shared Function UI_InsertPGmForManufacture(ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String, ByVal project As String) As String
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_AddWhseTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class EvolutionSDK
        Public Shared Function Initialize() As String
            Try
                DatabaseContext.CreateCommonDBConnection(My.Settings.EvoComServer, My.Settings.EvoComDB, My.Settings.EvoComUser, My.Settings.EvoComPassword, False)
                DatabaseContext.SetLicense("DE10110181", "7725371")
                DatabaseContext.CreateConnection(My.Settings.EvoServer, My.Settings.EvoDB, My.Settings.EvoUser, My.Settings.EvoPassword, False)
                Return "1*Success"
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "Server Response Evolution SDK Error: " + ex.Message)
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function

        Public Shared Function CGetItem(ByVal ItemCode As String) As InventoryItem
            Initialize()
            Dim errorItem As InventoryItem = New InventoryItem()
            errorItem.Code = "ErrorItem"
            Try
                Dim thisitem As InventoryItem = New InventoryItem(InventoryItem.Find("Code='" & ItemCode & "'"))
                Return thisitem
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "CGetItem: " + ex.ToString)
                Return errorItem
            End Try
        End Function

        Public Shared Function CGetLot(ByVal lotNumber As String, ByVal stockId As String) As Lot
            Initialize()
            Dim errorLot As Lot = New Lot()
            errorLot.Code = "ErrorLot"
            Try
                Dim thisitem As Lot = New Lot(Lot.Find("cLotDescription='" & lotNumber & "' AND iStockID = '" & stockId & "'"))
                Return thisitem
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "CGetItem: " + ex.ToString)
                Return errorLot
            End Try
        End Function

        Public Shared Function CGetWhse(ByVal whseCode As String) As Warehouse
            Try
                Dim thisitem As Warehouse = New Warehouse(Warehouse.Find("Code='" & whseCode & "'"))
                Return thisitem
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "CGetItem: " + ex.ToString)
                Return Nothing
            End Try
        End Function

        Public Shared Function CTransferItem(ByVal OrderNo As String, ByVal FromWarehouseCode As String, ByVal ToWarehouseCode As String, ByVal ItemCode As String, ByVal Lot As String, ByVal Qty As String) As String
            Try
                Initialize()
                Dim thistransfer As New WarehouseTransfer()
                Dim invItem As New InventoryItem()
                invItem = CGetItem(ItemCode)
                thistransfer.Date = Now

                thistransfer.Description = "RTIS Warehouse Transfer from: " + FromWarehouseCode + " to: " + ToWarehouseCode
                thistransfer.ExtOrderNo = OrderNo
                thistransfer.FromWarehouse = CGetWhse(FromWarehouseCode)
                thistransfer.ToWarehouse = CGetWhse(ToWarehouseCode)
                thistransfer.InventoryItem = invItem
                thistransfer.Lot = CGetLot(Lot, invItem.ID)
                thistransfer.Quantity = Convert.ToDouble(Qty)
                thistransfer.Reference = OrderNo
                If thistransfer.Validate() Then
                    thistransfer.Post()
                    Return "1*Success"
                Else
                    Return "-1*The warehouse transfer could not be completed"
                End If


            Catch ex As Exception
                EventLog.WriteEntry("RTIS SVC", "CTransferItem: " + ex.ToString)
                Return "-1*" + ItemCode + " Reason: " + ex.Message
            End Try
        End Function
    End Class
End Class
