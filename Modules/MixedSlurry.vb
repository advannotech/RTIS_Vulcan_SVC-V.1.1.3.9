Imports System.Data.SqlClient
Public Class MixedSlurry

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTSQL
        Partial Public Class Retreive

#Region "MBL"

#Region "Start Mix"
            Public Shared Function MBL_CheckMixedSlurryBufferTankInUse(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*" + ReturnData
                    Else
                        Return "1*Tank not in use"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Tank not in use"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfoRecTrans: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckMixedSlurryTankInUse(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bReceived], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*Warning, Mixed Slurry Lot " + ReturnData + " is currently assigned to tank " + tankCode + ". To start a new mix please close off current mix"
                    Else
                        Return "1*Tank not in use"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Tank not in use"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfoRecTrans: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Enter Remainder"
            Public Shared Function MBL_CheckMixedSlurryRemENtered(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vUserRemaining] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*The remainder for this tank has already been entered"
                    Else
                        Return "1*Remainer not entered"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Remainer not entered"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfoRecTrans: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Enter Recovered"
            Public Shared Function MBL_CheckMixedSlurryRecEntered(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vUserRecovered] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*The recovery for this tank has already been entered"
                    Else
                        Return "1*recovery not entered"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*recovery not entered"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfoRecTrans: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Add Fresh Slurry"
            Public Shared Function MBL_CheckSlurryInBufferTank(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInBufferTank: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function  
            Public Shared Function MBL_GetTankFreshSlurries(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT
                                                    s.[vTrolleyCode]
                                                    ,s.[vItemCode]
                                                    ,s.[vItemDesc]
                                                    ,s.[vLotNumber]
                                                    ,s.[dWeight]
                                                    FROM [tbl_RTIS_MS_Slurries] s
                                                    WHERE s.[iHeaderID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "1*"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetTankFreshSlurries: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckSlurryInTank(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bReceived], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn) '
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInBufferTank: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMixedSlurryHeaderID(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMixedSlurryHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetTrolleyInfo(ByVal trolleyNo As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vLotNumber], s.[Description_1]
                                                     FROM [tbl_RTIS_Fresh_Slurry] ms
                                                     INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[Code] = ms.[vItemCode]
                                                     WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 AND [dSolidity] IS NOT NULL AND ISNULL([bRecTrans], 0) = 1 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No unused slurry found for tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No unused slurry found for tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckSlurryAlreadyInTank(ByVal headerId As String, ByVal trolleyCode As String, ByVal itemCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID] FROM [tbl_RTIS_MS_Slurries] WHERE [iHeaderID] = @1 AND [vTrolleyCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerId))
                    sqlComm.Parameters.Add(New SqlParameter("@2", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This trolley has already been scanned into the tank"
                    Else
                        Return "1*Trolley not yet used"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Trolley not yet used"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMixedSlurryHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetBufferTankSlurryID(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetBufferTankSlurryID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetTankSlurryID(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bReceived], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetTankSlurryID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetTrolleyTolerance()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [SettingValue] FROM [tbl_RTSettings]
                                                    WHERE [Setting_Name] = 'FSTol'", sqlConn)
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
                        Return "-1*No tolerance found for trolley!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetTrolleyTolerance: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryTrolleyInfo(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLIneID], [dWetWeight], [dTotalDecantedWeight] FROM [tbl_RTIS_Fresh_Slurry]
                                                    WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No fresh slurry found for trolley!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No fresh slurry found for trolley!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Decant"
            Public Shared Function MBL_CheckSlurryAvailableToDecant(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0 AND [dRemainingWeight] IS NOT NULL AND [dRecoveredWeight] IS NOT NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No valid slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No valid slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInBufferTank: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckMobileTankAvailable(ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber] FROM [tbl_RTIS_MS_Decant]
                                                    WHERE [vTankCode] = @1 AND [vItemCode]= @2 AND ISNULL([bReceived], 0) = 0
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*Warning, Mixed Slurry Lot " + ReturnData + " is currently assigned to tank " + tankCode + ". To start a new mix please close off current mix"
                    Else
                        Return "1*The tank is empty"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*The tank is empty"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInBufferTank: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMixedSlurryHeaderInfo(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID], [vDescription], [vLotNumber] FROM [tbl_RTIS_MS_Main]
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0 AND [dRemainingWeight] IS NOT NULL AND [dRecoveredWeight] IS NOT NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No valid slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No valid slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInBufferTank: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Zonen and Charging"
            Public Shared Function MBL_CheckSlurryInMobileTankZAC(ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [vItemDesc] FROM [tbl_RTIS_MS_Decant]
                                                    WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryInMobileTankZAC: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckSlurryTankZAC(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
                                                     WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                     ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckSlurryTankZAC: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZacChems() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vChemicalName] FROM [tbl_RTIS_MS_Chemical_List]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + +"~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No chemiclas  were pulled!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No chemiclas  were pulled!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZacChems: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function 
            Public Shared Function MBL_GetMobileTankSlurryID(ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Decant]
                                                    WHERE [vTankCode] = @1 AND [vItemCode] = @2
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*No slurry is currently in this tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMobileTankSlurryID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMobileTankChemicalID(ByVal tankID As String, ByVal chemical As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SElECT [iLineID] FROM [tbl_RTIS_MS_Chemicals] WHERE [iMTNKID] = @1 AND [vChemicalName] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
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
                        Return "0*Chemical not found add new"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Chemical not found add new"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMobileTankChemicalID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetTankChemicalID(ByVal tankID As String, ByVal chemical As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SElECT [iLineID] FROM [tbl_RTIS_MS_Chemicals] WHERE [iTNKID] = @1 AND [vChemicalName] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
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
                        Return "0*Chemical not found add new"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Chemical not found add new"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetTankChemicalID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAllChemicals() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vChemicalName]
                                                    FROM [tbl_RTIS_MS_Chemical_List]", sqlConn)
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
                        Return "0*No chemicals were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No chemicals were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllChemicals: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region        

#Region "Solidity"
            Public Shared Function MBL_GetMobileTankInfoSolidity(ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [vItemDesc] FROM [tbl_RTIS_MS_Decant]
                                                    WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMobileTankInfoSolidity: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryTankInfoSolidity(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vLotNumber], [vDescription] FROM [tbl_RTIS_MS_Main]
                                                     WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                     ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryTankInfoSolidity: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMobileTankSolidityInfo(ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [iLineID], [vLotNumber] FROM [tbl_RTIS_MS_Decant]
                                                    WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMobileTankSolidityInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryTankSolidityInfo(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1  [iLineID], [vLotNumber] FROM [tbl_RTIS_MS_Main]
                                                     WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bTransferred], 0) = 0 AND ISNULL([bReceived], 0) = 0 AND [dSolidity] IS NULL
                                                     ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No slurry is currently in this tank or the slurry is invalid for zonen and charging"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryTankSolidityInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Rec Transfer"
            Public Shared Function MBL_GetFreshSlurryInfoRecTrans(ByVal trolleyCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 'False'), s.[Description_1] FROM [tbl_RTIS_Fresh_Slurry] ms
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[Code] = [vItemCode]
                                                    WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No trolley information found!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No trolley information found!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInfoRecTrans: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Outgoing Transfer"
            Public Shared Function MBL_GetMobileTankTransferInfo(ByVal tankNo As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bTransferred], 0), [vItemDesc]
                                                    FROM [tbl_RTIS_MS_Decant] WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [dSolidity] IS NOT NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No valid mixed slurry found for tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No valid mixed slurry found for tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMixedSlurryInfoForTransfer: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMobileTankTransferd(ByVal tankNo As String, ByVal itemCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	SELECT TOP 1 ISNULL([bTransferred], 0) FROM [tbl_RTIS_MS_Decant]
	                                                WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vLotNumber] = @3
	                                                ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
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

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No mixed slurry found for tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No mixed slurry found for tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMobileTankTransferd: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetLargeTankTransferd(ByVal tankTye As String, ByVal tankNo As String, ByVal itemCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	  SELECT TOP 1 ISNULL([bTransferred], 0) FROM [tbl_RTIS_MS_Main]
                                                      WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [vLotNumber] = @4 
                                                      ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankTye))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
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
                        Return "0*No mixed slurry found for tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No mixed slurry found for tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetLargeTankTransferd: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetTankTransferInfo(ByVal tankType As String, ByVal tankNo As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bTransferred], 0), [vDescription]
                                                    FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [dSolidity] IS NOT NULL
                                                    ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No valid mixed slurry found for tank"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No valid mixed slurry found for tank"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMixedSlurryInfoForTransfer: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#End Region

#Region "UI"

#Region "Auto Manufacture"
            Public Shared Function UI_GetMixedSlurriesToManufacture() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT m.[iLineID], m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[dtUserStarted]
                                                      FROM [tbl_RTIS_MS_Main] m
                                                      LEFT JOIN [tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
                                                      WHERE m.[bManufactured] = 0
                                                      GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], m.[dtUserStarted], m.[bTransferred], m.[bReceived], m.[bManualclose]", sqlConn)

 '                   SELECT m.[iLineID], m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[dtUserStarted]
 '                                                     FROM [tbl_RTIS_MS_Main] m
 '                                                     LEFT JOIN [tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
 '                                                     WHERE m.[vTankType] = 'TNK' AND m.[bManufactured] = 0
 '                                                     GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], m.[dtUserStarted], m.[bTransferred], m.[bReceived], m.[bManualclose]
 '                                                     UNION 
 'SELECT  d.[iLineID], 'MTNK_' + d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[dtUserStarted]
 'FROM [tbl_RTIS_MS_Decant] d 
 'INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = d.[iHeaderID]
 'LEFT JOIN [tbl_RTIS_MS_Slurries] s ON m.[iLineID] = s.[iHeaderID] WHERE d.[bManufactured] = 0
 'GROUP BY d.[iLineID],d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], m.[dtUserStarted], d.[bTransferred], d.[bReceived], d.[bManualclose]
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No mixed slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No mixed slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurries: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetMixedSlurryTankInfoManufacture(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight]  , ISNULL(m.[bManualclose], 0) ,m.[vUserClosed],m.[dtDateClosed] ,m.[vReasonClosed], ISNULL(m.[dWetWeight],0) , ISNULL(m.[dDryWeight],0) , ISNULL(m.[dSolidity],0) 
                                                      FROM [tbl_RTIS_MS_Main] m
                                                      LEFT JOIN [tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
                                                      WHERE m.[iLineID] = @1
                                                      GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], m.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], m.[bManualclose] ,m.[vUserClosed],m.[dtDateClosed] ,m.[vReasonClosed], m.[dWetWeight], m.[dDryWeight], m.[dSolidity]", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) 
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No information found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMixedSlurryTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetMixedSlurryDecantsManufacture(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("     SELECT md.[vTankCode], md.[dFinalWetWeight], md.[dSolidity], md.[dDryWeight],
  STUFF(
  (SELECT ',' + CAST(mc.[iLineID] AS VARCHAR(MAX)) + '#' + CAST(mc.[vChemicalName] AS VARCHAR(MAX)) + '#' + CAST(mc.[dQty] AS VARCHAR(MAX)) FROM [tbl_RTIS_MS_Chemicals] mc WHERE mc.[iMTNKID] = md.[iLineID] FOR XML PATH('')),1,1,'') AS [Chems]
  FROM [tbl_RTIS_MS_Decant] md WHERE [iHeaderID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No information found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMixedSlurryTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryInputsManufacture(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT s.[vTrolleyCode], s.[vItemCode], s.[vItemDesc], s.[vLotNumber], s.[dWeight] FROM [tbl_RTIS_MS_Slurries] s 
                                                    INNER JOIN [tbl_RTIS_MS_Main] m ON s.[iHeaderID] = m.[iLineID]
                                                    WHERE m.[iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No fresh slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No fresh slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryInputs: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryChemicalsManuf(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID], [vChemicalName], [dQty] FROM [tbl_RTIS_MS_Chemicals]
                                                    WHERE [iTNKID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
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
                        Return "0*No zonen and charging chemicals were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No zonen and charging chemicals were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryChemicals: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryChemicalsManuf_Mobile(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID], [vChemicalName], [dQty] FROM [tbl_RTIS_MS_Chemicals]
                                                    WHERE [iMTNKID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
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
                        Return "0*No zonen and charging chemicals were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No zonen and charging chemicals were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryChemicals_Mobile: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function       
            Public Shared Function UI_GetMixedSlurryMobileTankInfoManufacture(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("    SELECT TOP 1 d.iHeaderID, 'MTNK_' + d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), d.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], ISNULL(d.[bManualclose], 0) ,d.[vUserClosed],d.[dtDateClosed] ,d.[vReasonClosed], ISNULL(d.[dFinalWetWeight], 0), ISNULL(d.[dDryWeight], 0), ISNULL(d.[dSolidity], 0) FROM [tbl_RTIS_MS_Decant] d
 INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = d.[iHeaderID]
 LEFT JOIN [tbl_RTIS_MS_Slurries] s ON m.[iLineID] = s.[iHeaderID]
 WHERE d.[iLineID] = @1
 GROUP BY d.[iLineID], d.iHeaderID, d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], d.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], ISNULL(d.[bManualclose], 0) ,d.[vUserClosed],d.[dtDateClosed] ,d.[vReasonClosed], d.[dFinalWetWeight], d.[dDryWeight], d.[dSolidity]
 ORDER BY d.[iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No information found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMixedSlurryTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
             Public Shared Function UI_GetAllMixedSlurryInputs_JournalOut(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT s.[vTrolleyCode], s.[vItemCode], s.[vItemDesc], s.[vLotNumber], s.[dWeight], fs.[dtDateSol], fs.[dSolidity] FROM [tbl_RTIS_MS_Slurries] s 
                                                    INNER JOIN [tbl_RTIS_MS_Main] m ON s.[iHeaderID] = m.[iLineID]
													INNER JOIN [tbl_RTIS_Fresh_Slurry] fs ON fs.[vLotNumber] = s.[vLotNumber]
                                                    WHERE m.[iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No fresh slurry inputs found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No fresh slurry inputs found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryInputs_JournalOut: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryManufInfo(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT s.[StockLink], l.[idLotTracking], m.[vLotNumber], s.[AveUCst],
  Case
  WHEN m.[vTankType] = 'BTNK' THEN (SELECT SUM (ISNULL([dDryWeight], 0)) FROM [tbl_RTIS_MS_Decant] WHERE [iHeaderID] = m.[iLineID])
  ELSE m.[dDryWeight]
  END AS [Qty],
  Case
  WHEN m.[vTankType] = 'BTNK' THEN (SELECT TOP 1 [dtSol] FROM [tbl_RTIS_MS_Decant] WHERE [iHeaderID] = m.[iLineID] ORDER BY [dtSol])
  ELSE m.[dtSol]
  END AS [dtSol]
	        ,[dRecoveredWeight]
			,[vUserRecoveredLot]
	        ,[dRemainingWeight]
	        ,[vUserRemainingLot]
      ,[vRemainingSolidity]
      ,[vRecoveredSolidity]
  FROM [tbl_RTIS_MS_Main] m 
  INNER JOIN ["+My.Settings.EvoDB+"].[dbo].[StkItem] s ON s.[Code] = m.[vItemCode]
  LEFT JOIN ["+My.Settings.EvoDB+"].[dbo].[_etblLotTracking] l ON l.[cLotDescription] = m.[vLotNumber] AND l.[iStockID] = s.[StockLink]
  WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9))+ "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information was found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return"0*No information was found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryManufInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetMSJournalID_Manuf() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'MSJournalID')) 
                                    BEGIN
	                                    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) VALUES ('MSJournalID', '0')
                                    END
                                    SELECT SettingValue
                                                    FROM [tbl_RTSettings] WHERE [Setting_Name] = 'MSJournalID'", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No journal ID wasfound"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No journal ID wasfound"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMSJournalID_Manuf: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Chemical Management"
             Public Shared Function UI_GetAllChemicals() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID]
                                                    ,[vChemicalName]
                                                    FROM [tbl_RTIS_MS_Chemical_List]", sqlConn)
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
                        Return "0*No chemicals were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No chemicals were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllChemicals: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "NPL"
            Public Shared Function UI_GetNPLPercentages() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vChemicalName], [dPercentage]
                                                    FROM [tbl_RTIS_MS_NPL]", sqlConn)
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
                        Return "0*No NPL records were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No NPL records were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetNPLPercentages: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Journal ID"
             Public Shared Function UI_GetMSJournalID() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" IF(NOT EXISTS (SELECT * FROM [tbl_RTSettings] WHERE [Setting_Name] = 'MSJournalID')) 
                                    BEGIN
	                                    INSERT INTO [tbl_RTSettings] ([Setting_Name], [SettingValue]) VALUES ('MSJournalID', '0')
                                    END
                                    SELECT SettingValue
                                                    FROM [tbl_RTSettings] WHERE [Setting_Name] = 'MSJournalID'", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No journal ID wasfound"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No journal ID wasfound"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMSJournalID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            #End Region

            Public Shared Function UI_GetAllMixedSlurries(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT m.[iLineID], m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[dtUserStarted], ISNULL(m.[bTransferred], 0), ISNULL(m.[bReceived], 0), ISNULL(m.[bManualclose], 0)
                                                      FROM [tbl_RTIS_MS_Main] m
                                                      LEFT JOIN [tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
                                                      WHERE m.[vTankType] = 'TNK' AND m.[dtUserStarted] BETWEEN @1 AND @2
                                                      GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], m.[dtUserStarted], m.[bTransferred], m.[bReceived], m.[bManualclose]
                                                      UNION 
 SELECT  d.[iLineID], 'MTNK_' + d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[dtUserStarted], ISNULL(d.[bTransferred], 0), ISNULL(d.[bReceived], 0), ISNULL(d.[bManualclose], 0) 
 FROM [tbl_RTIS_MS_Decant] d 
 INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = d.[iHeaderID]
 LEFT JOIN [tbl_RTIS_MS_Slurries] s ON m.[iLineID] = s.[iHeaderID] WHERE m.[dtUserStarted] BETWEEN @1 AND @2
 GROUP BY d.[iLineID],d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], m.[dtUserStarted], d.[bTransferred], d.[bReceived], d.[bManualclose]", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No mixed slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No mixed slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurries: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetMixedSlurryTankInfo(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT m.[vTankType] + '_' + m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), m.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight]  , ISNULL(m.[bManualclose], 0) ,m.[vUserClosed],m.[dtDateClosed] ,m.[vReasonClosed], ISNULL(m.[dWetWeight],0) , ISNULL(m.[dDryWeight],0) , ISNULL(m.[dSolidity],0) 
                                                      FROM [tbl_RTIS_MS_Main] m
                                                      LEFT JOIN [tbl_RTIS_MS_Slurries] s ON s.[iHeaderID] = m.[iLineID]
                                                      WHERE m.[iLineID] = @1
                                                      GROUP BY m.[iLineID], m.[vTankType] , m.[vTankCode], m.[vItemCode], m.[vDescription], m.[vLotNumber], m.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], m.[bManualclose] ,m.[vUserClosed],m.[dtDateClosed] ,m.[vReasonClosed], m.[dWetWeight], m.[dDryWeight], m.[dSolidity]", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) 
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No information found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMixedSlurryTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetMixedSlurryMobileTankInfo(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("    SELECT TOP 1 d.iHeaderID, 'MTNK_' + d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], ISNULL( COUNT( s.[iLineID]), 0), d.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], ISNULL(d.[bManualclose], 0) ,d.[vUserClosed],d.[dtDateClosed] ,d.[vReasonClosed], ISNULL(d.[dFinalWetWeight], 0), ISNULL(d.[dDryWeight], 0), ISNULL(d.[dSolidity], 0) FROM [tbl_RTIS_MS_Decant] d
 INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = d.[iHeaderID]
 LEFT JOIN [tbl_RTIS_MS_Slurries] s ON m.[iLineID] = s.[iHeaderID]
 WHERE d.[iLineID] = @1
 GROUP BY d.[iLineID], d.iHeaderID, d.[vTankCode], d.[vItemCode], m.[vDescription], d.[vLotNumber], d.[vZonenCharging], m.[dRemainingWeight], m.[dRecoveredWeight], ISNULL(d.[bManualclose], 0) ,d.[vUserClosed],d.[dtDateClosed] ,d.[vReasonClosed], d.[dFinalWetWeight], d.[dDryWeight], d.[dSolidity]
 ORDER BY d.[iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No information found for the selected mixed slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No information found for the selected mixed slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetMixedSlurryTankInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryInputs(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT s.[vTrolleyCode], s.[vItemCode], s.[vItemDesc], s.[vLotNumber], s.[dWeight] FROM [tbl_RTIS_MS_Slurries] s 
                                                    INNER JOIN [tbl_RTIS_MS_Main] m ON s.[iHeaderID] = m.[iLineID]
                                                    WHERE m.[iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No fresh slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No fresh slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryInputs: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryChemicals(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID], [vChemicalName], [dQty] FROM [tbl_RTIS_MS_Chemicals]
                                                    WHERE [iTNKID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
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
                        Return "0*No fresh slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No fresh slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryChemicals: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAllMixedSlurryChemicals_Mobile(ByVal lineId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iLineID], [vChemicalName], [dQty] FROM [tbl_RTIS_MS_Chemicals]
                                                    WHERE [iMTNKID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineId))
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
                        Return "0*No fresh slurries found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No fresh slurries found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllMixedSlurryChemicals_Mobile: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function       
#End Region     
        End Class
        Partial Public Class Insert
#Region "MBL"
#Region "Start Mix"
            Public Shared Function MBL_InsertMixedSlurryRecord(ByVal tankType As String, ByVal tankCode As String, ByVal itemCode As String, ByVal itemDesc As String, ByVal lotNum As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  INSERT INTO [tbl_RTIS_MS_Main] ([vTankType], [vTankCode], [vItemCode], [vDescription], [vLotNumber], [vUserStarted], [dtUserStarted])
                                                     VALUES (@1, @2, @3, @4, @5, @6, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", itemDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertMixedSlurryRecord: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Add Fresh Slurry"
            Public Shared Function MBL_InsertFreshSlurryRecord(ByVal headerID As String, ByVal tankCode As String, ByVal trolleyCode As String, ByVal itemCode As String, ByVal itemDesc As String, ByVal lotnumber As String, ByVal weight As String, ByVal tolerance As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INtO [tbl_RTIS_MS_Slurries] ([iHeaderID],[vTankCode]
                                                    ,[vTrolleyCode]
                                                    ,[vItemCode]
                                                    ,[vItemDesc]
                                                    ,[vLotNumber]
                                                    ,[dWeight]
                                                    ,[vTrolleyTol]
                                                    ,[dtDateEntered]
                                                    ,[vUserEntered])
                                                    VALUES (@1, @2, @3, @4, @5, @6, @7, @8, GETDATE(), @9)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@5", itemDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@6", lotnumber))
                    sqlComm.Parameters.Add(New SqlParameter("@7", weight))
                    sqlComm.Parameters.Add(New SqlParameter("@8", tolerance))
                    sqlComm.Parameters.Add(New SqlParameter("@9", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertFreshSlurryRecord: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            
#Region "Decant"
           Public Shared Function MBL_InsertMobileSlurryRecord(ByVal headerID As String, ByVal tankCode As String, ByVal itemcode As String, ByVal itemDesc As String, ByVal lotnumber As String, ByVal weight As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [tbl_RTIS_MS_Decant] ([iHeaderID], [vTankCode], [vItemCode], [vItemDesc], [vLotNumber], [dWetWeight], [vUserDecanted], [dtDateDecanted]) 
	                                                VALUES (@1, @2, @3, @4, @5, @6, @7, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", itemDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lotnumber))
                    sqlComm.Parameters.Add(New SqlParameter("@6", weight))
                    sqlComm.Parameters.Add(New SqlParameter("@7", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertMobileSlurryRecord: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function 
#End Region

#Region "Zonen and Charging"
            Public Shared Function MBL_InsertMixedSlurryChemicalMobile(ByVal tankId As String, ByVal chemical As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [tbl_RTIS_MS_Chemicals] ([iTNKID], [iMTNKID], [vChemicalName], [dQty], [vUserAdded], [dtAdded])
                                                    VALUES (0, @1, @2, @3, @4, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankId))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertMixedSlurryChemical: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_InsertMixedSlurryChemical(ByVal tankId As String, ByVal chemical As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [tbl_RTIS_MS_Chemicals] ([iTNKID], [iMTNKID], [vChemicalName], [dQty], [vUserAdded], [dtAdded])
                                                    VALUES (@1, 0, @2, @3, @4, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankId))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertMixedSlurryChemical: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region     
            #End Region
           
#Region "UI"
            Public Shared Function UI_InsertMixedSlurryChemical(ByVal name As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [tbl_RTIS_MS_Chemical_List] ([vChemicalName]) VALUES (@Name)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Name", name))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertMixedSlurryChemical: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

        End Class
        Partial Public Class Update
#Region "MBL"

#Region "Start Mix"
            Public Shared Function MBL_closeBufferSlurries(ByVal tankType As String, ByVal tankCode As String, ByVal itemcode As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [bBufferClosed] = 1, [vUserBufferClosed] = @4, [dtBufferClosed] = GETDATE()
                                                    WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND ISNULL([bBufferClosed], 0) = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setMSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            
#Region "Enter Remainder"
            Public Shared Function MBL_setMixedSlurryRemaining(ByVal tankType As String, ByVal tankCode As String, ByVal itemcode As String, ByVal remWeight As String, ByVal remLot As String, ByVal remSol As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_MS_Main] SET [dRemainingWeight] = @4, [vUserRemainingLot] = @5, [vRemainingSolidity] = @6, [vUserRemaining] = @7, [dtRemaining] = GETDATE()
                                                   WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [iLineID] = (  SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 ORDER BY [iLineID] DESC)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", remWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@5", remLot))
                    sqlComm.Parameters.Add(New SqlParameter("@6", remSol))
                    sqlComm.Parameters.Add(New SqlParameter("@7", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setMixedSlurryRemaining: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Enter Recovered"
            Public Shared Function MBL_setMixedSlurryRecovered(ByVal tankType As String, ByVal tankCode As String, ByVal itemcode As String, ByVal remWeight As String, ByVal remLot As String, ByVal remSol As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_MS_Main] SET [dRecoveredWeight] = @4, [vUserRecoveredLot] = @5, [vRecoveredSolidity] = @6, [vUserRecovered] = @7, [dtRecovered] = GETDATE()
                                                   WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 AND [iLineID] = (  SELECT TOP 1 [iLineID] FROM [tbl_RTIS_MS_Main] WHERE [vTankType] = @1 AND [vTankCode] = @2 AND [vItemCode] = @3 ORDER BY [iLineID] DESC)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", remWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@5", remLot))
                    sqlComm.Parameters.Add(New SqlParameter("@6", remSol))
                    sqlComm.Parameters.Add(New SqlParameter("@7", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setMixedSlurryRemaining: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Add Fresh Slurry"
            Public Shared Function MBL_UpdateFreshSlurryDecantQty(ByVal trolleyID As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_RTIS_Fresh_Slurry] SET [dTotalDecantedWeight] = [dTotalDecantedWeight] + @QTY WHERE [iLIneID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@ID", trolleyID))
                    sqlComm.Parameters.Add(New SqlParameter("@QTY", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateFreshSlurryDecantQty: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Zonen and Charging"
            Public Shared Function MBL_updateMobileTankChemical(ByVal tankId As String, ByVal chemical As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   UPDATE [tbl_RTIS_MS_Chemicals] SET [dQty] = @3, [vUserUpdated] = @4, [dtUpdated] = GETDATE()
                                                      WHERE [iMTNKID] = @1 AND [vChemicalName] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankId))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_updateMobileTankChemical: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_updateMTankChemical(ByVal tankId As String, ByVal chemical As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   UPDATE [tbl_RTIS_MS_Chemicals] SET [dQty] = @3, [vUserUpdated] = @4, [dtUpdated] = GETDATE()
                                                      WHERE [iTNKID] = @1 AND [vChemicalName] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankId))
                    sqlComm.Parameters.Add(New SqlParameter("@2", chemical))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_updateMTankChemical: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            
#Region "Solidity"
            Public Shared Function MBL_setMobileTankSolidity(ByVal solidity As String, ByVal wetWeight As String, ByVal dryWeight As String, ByVal username As String, ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Decant] SET [dSolidity] = @1, [dFinalWetWeight] = @2, [dDryWeight] = @3, [vUserSol] = @4, [dtSol] = GETDATE()
                                                    WHERE [iLineID] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", solidity))
                    sqlComm.Parameters.Add(New SqlParameter("@2", wetWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@3", dryWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lineID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setMobileTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_setTankSolidity(ByVal solidity As String, ByVal wetWeight As String, ByVal dryWeight As String, ByVal username As String, ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [dSolidity] = @1, [dWetWeight] = @2, [dDryWeight] = @3, [vUserSol] = @4, [dtSol] = GETDATE()
                                                    WHERE [iLineID] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", solidity))
                    sqlComm.Parameters.Add(New SqlParameter("@2", wetWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@3", dryWeight))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", lineID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            
#End Region          

#Region "UI"

#Region "NPL"
            Public Shared Function UI_EditNPLPercentage(ByVal name As String, ByVal percentage As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_NPL] SET [dPercentage] = @PER WHERE [vChemicalName] = @NAME", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@NAME", name))
                    sqlComm.Parameters.Add(New SqlParameter("@PER", percentage.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_EditNPLPercentage: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Journal ID"
            Public Shared Function UI_EditMSJournalID(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTSettings] SET [SettingValue] = @ID WHERE [Setting_Name] = 'MSJournalID'", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@ID", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_EditMSJournalID: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateMSManufactured(ByVal id As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [bManufactured] = 1, [vUserManufactured] = @USER, [dtManufactured] = GETDATE() WHERE [iLineID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@ID", id))
                    sqlComm.Parameters.Add(New SqlParameter("@USER", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateMSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            Public Shared Function UI_CloseMixedSlurryTank(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_CloseMixedSlurryMobileTank(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Decant] SET [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_CloseMixedSlurryTankManuf(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [bManufactured] = 1, [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_CloseBufferTankManuf(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Main] SET [bBufferClosed] = 1, [vUserBufferClosed] = @3, [dtBufferClosed] = GETDATE(), [bManufactured] = 1, [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_CloseMixedSlurryMobileTankManuf(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Decant] SET [bManufactured] = 1, [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_CloseAllMobileTanksManuf(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_MS_Decant] SET [bManufactured] = 1, [bManualclose] = 1, [vUserClosed] = @3, [vReasonClosed] = @2, [dtDateClosed] = GETDATE(), [bReceived] = 1 ,[vUserReceived]= @3 ,[dtReceived] = GETDATE()
                                                    WHERE [iHeaderID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_setTankSolidity: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region          
        End Class
        Public Partial Class Delete
#Region "UI"

#Region "Chemical Management"
            Public Shared Function UI_RemoveChemicalFromList(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" DELETE FROM [tbl_RTIS_MS_Chemical_List] WHERE [iLineID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@ID", lineID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_RemoveChemicalFromList: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#End Region
        End Class

        Public Shared Function ExecuteTransaction(ByVal queryList As List(Of String)) As String
            Try
                Dim sqlConn As New SqlConnection(EvoString)  
                sqlConn.Open()
                Dim sqlTrans As SqlTransaction = sqlConn.BeginTransaction(IsolationLevel.ReadCommitted)
                 Try
                   
                    For Each query as string in queryList
                        Dim sqlComm As New SqlCommand(query, sqlConn, sqlTrans)
                        sqlComm.ExecuteNonQuery()
                    Next
                    sqlTrans.Commit()    
                     sqlConn.Close()
                    Return "1*"
                Catch ex2 As Exception
                    sqlTrans.Rollback()
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setMSManufactured: " + ex2.ToString())
                    Return ExHandler.returnErrorEx(ex2)
                End Try
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setMSManufactured: " + ex.ToString())
                Return ExHandler.returnErrorEx(ex)
            End Try
               
            End Function
    End Class

    Public Class Evolution
        Partial Public Class Rereive
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

             Public Shared Function MBL_GetFeshSlurryInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [StockLink], [AveUCst] FROM [StkItem] WHERE [Code] = @CODE", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@CODE", itemCode))
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
                        Return "0*No item information was found for one of the fresh slurries"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No item information was found for one of the fresh slurries"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFeshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetFeshSlurryLotID(ByVal lotNUmber As String, ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @LOT AND [iStockID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@LOT", lotNUmber))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", stockID))
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
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFeshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetRecLotID(ByVal lotNUmber As String, ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @LOT AND [iStockID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@LOT", lotNUmber))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", stockID))
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
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFeshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetRemLotID(ByVal lotNUmber As String, ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("SELECT [idLotTracking] FROM [_etblLotTracking] WHERE [cLotDescription] = @LOT AND [iStockID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@LOT", lotNUmber))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", stockID))
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
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*"+ lotNUmber +" was not found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFeshSlurryInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class
    End Class
End Class
