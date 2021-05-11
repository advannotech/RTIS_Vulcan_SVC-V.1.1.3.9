Imports System.Data.SqlClient

Public Class WhseManagement

    Public Class RTSQL
        Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
        Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

        Partial Public Class Retreive
            Public Shared Function UI_getWhseProcLookUp(ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT wm.[WhseLink], wm.[Name], '' AS [Remove] FROM [tbl_WHTLocations] wl 
	                                                INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] wm ON wm.[WhseLink] = wl.[iWhseID]
	                                                WHERE wl.[vProcessName] = @1 AND [bIsRec] = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", process))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhseProcLookUp: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWhseProcLookUpRec(ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT wm.[WhseLink], wm.[Name], '' AS [Remove] FROM [tbl_WHTLocations] wl 
	                                                INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] wm ON wm.[WhseLink] = wl.[iWhseID]
	                                                WHERE wl.[vProcessName] = @1 AND [bIsRec] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", process))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhseProcLookUp: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWhseAllEvoWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("	SELECT [WhseLink], [Name], '' AS [Add] FROM [WhseMst]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No evolution warehouses were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No evolution warehouses were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhseAllEvoWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_checkProcRefExists(ByVal procName As String, ByVal whseID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLine_ID] FROM [tbl_WHTLocations]
                                                    WHERE [vProcessName] = @1 AND [iWhseID] = @2 AND [bIsRec] = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", procName))
                    sqlComm.Parameters.Add(New SqlParameter("@2", Convert.ToInt32(whseID)))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This warehouse is already associated with the selected process"
                    Else
                        Return "1*No ref found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No ref found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_checkProcRefExists: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_checkProcRefExistsRec(ByVal procName As String, ByVal whseID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLine_ID] FROM [tbl_WHTLocations]
                                                    WHERE [vProcessName] = @1 AND [iWhseID] = @2 AND [bIsRec] =1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", procName))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This warehouse is already associated with the selected process"
                    Else
                        Return "1*No ref found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No ref found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_checkProcRefExists: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWhtProcesses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames] WHERE ISNULL([bHasOutTransfer], 'false') = 1", sqlConn)
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
                        Return "0*No processes were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No processes were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtProcesses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWhtProcessesRec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT [vDisplayName], [vProcName] FROM [tbl_ProcNames] WHERE ISNULL([bHasRecTrans], 0) <> 0", sqlConn)
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
                        Return "0*No processes were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No processes were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtProcessesRec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_PPtFS() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_PPtFS] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_PPtFS: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseLookUp_PP_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_PP_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseLookUp_FStMS() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_FStMS] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_FStMS: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseLookUp_FS_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_FS_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseLookUp_MStZect() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_MStZect] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_MStZect: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_MS_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "RTIS_WhseLookUp_MS_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Zect1() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Zect1] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Zect1_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Zect1_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Zect2() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Zect2] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Zect2: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Zect2_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Zect2_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Canning() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_Canning] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Canning: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_Can_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_Can_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_AW() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_AW] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_AW: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_AW_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_AW_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_PGM_Rec() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_PGM_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseLookUp_ToProd() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT w.[WhseLink], w.[Name], ISNULL(wl.[bEnabled],0) FROM [RTIS_WarehouseLookUp_ToProd] wl
                                                    RIGHT JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w 
                                                    ON w.[WhseLink] = wl.[iWhse_Link]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseLookUp_ToProd: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWarehouseExistes_PPtFS(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_PPtFS] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

#Region "Receiving Transfer Management"
            Public Shared Function UI_getWarehouseExistes_PP_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_PP_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_FS_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_FS_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_MS_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_MS_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Zect1_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Zect2_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect2_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Can_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Can_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_AW_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_AW_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_PGM_Rec(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_PGM_Rec: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

            Public Shared Function UI_getWarehouseExistes_FStMS(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_FStMS] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_MStZect(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_MStZect] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Zect1(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Zect1] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Zect2(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Zect2] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_Can(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_Canning] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_AW(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_AW] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWarehouseExistes_ToProd(ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [bEnabled] FROM [RTIS_WarehouseLookUp_ToProd] WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse lookups were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse lookups were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWarehouseExistes_Zect1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class

        Partial Public Class Insert
            Public Shared Function UI_addNewWhseCrossRef(ByVal procName As String, ByVal whseID As String, ByVal isRec As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_WHTLocations] ([vProcessName], [iWhseID], [bIsRec]) VALUES (@1, @2, @3)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", procName))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@3", isRec))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_addNewWhseCrossRef_PPtFS(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_PPtFS] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_PPtFS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

#Region "Receiving Transfer Management"

            Public Shared Function UI_addNewWhseCrossRef_PP_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_PP_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_FS_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_FS_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_MS_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_MS_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Zect1_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Zect1_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Zect2_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Zect2_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Can_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Can_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_AW_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_AW_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_PGM_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_PGM_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

            Public Shared Function UI_addNewWhseCrossRef_FStMS(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_FStMS] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_FStMS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_MStZect(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_MStZect] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_MStZect: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Zect1(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_Zect1] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Zect1: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Zect2(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_Zect2] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Zect2: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_Canning(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_Canning] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_Canning: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_AW(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_AW] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_AW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_addNewWhseCrossRef_ToProd(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_WarehouseLookUp_ToProd] ([iWhse_Link], [bEnabled]) VALUES (@1, @2)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_addNewWhseCrossRef_ToProd: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Update

            Public Shared Function UI_resetWhseCrossRef_PPtFS() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_PPtFS] SET [bEnabled] = 0", sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_resetWhseCrossRef_PPtFS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_resetWhseCrossRef_FStMS() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_FStMS] SET [bEnabled] = 0", sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_resetWhseCrossRef_FStMS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_resetWhseCrossRef_MStZext() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_MStZect] SET [bEnabled] = 0", sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_resetWhseCrossRef_MStZext: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_updateWhseCrossRef_PPtFS(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_PPtFS] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_PPtFS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

#Region "Receiving transfer management"

            Public Shared Function UI_updateWhseCrossRef_PP_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_PP_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_PPtFS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_FS_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_FS_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_FS_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_MS_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_MS_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_MS_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_Zect1_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Zect1_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_Zect1_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_Zect2_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Zect2_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_Zect2_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_Can_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_Canning_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_Can_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_AW_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_AW_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_AW_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_PGM_Rec(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [Rec_Transfers].[RTIS_WhseLookUp_PGM_Rec] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_PGM_Rec: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

            Public Shared Function UI_updateWhseCrossRef__FStMS(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_FStMS] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__FStMS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef__MStZectS(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_MStZect] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__MStZectS: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef__Zect1(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_Zect1] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__Zect1: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef__Zect2(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_Zect2] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__Zect2: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef__Canning(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_Canning] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__Canning: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef__AW(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_AW] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef__AW: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_updateWhseCrossRef_ToProd(ByVal whseID As String, ByVal selected As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_WarehouseLookUp_ToProd] SET [bEnabled] = @2 WHERE [iWhse_Link] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", selected))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_updateWhseCrossRef_ToProd: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Delete
            Public Shared Function UI_deleteNewWhseCrossRef(ByVal procName As String, ByVal whseID As String, ByVal isRec As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_WHTLocations] WHERE [vProcessName] = @1 AND [iWhseID] = @2 AND [bIsRec] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", procName))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseID))
                    sqlComm.Parameters.Add(New SqlParameter("@3", isRec))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_deleteNewWhseCrossRef: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

End Class
