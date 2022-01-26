Imports System.Data.SqlClient

Public Class Zect
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
           "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTSQL
        Partial Public Class Retreive

#Region "UI"
            Public Shared Function UI_GetCatalystRaws(ByVal catalystCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCatalystRaws] @1", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalystCode))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCatalystRaws: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetZeckLinkExists(ByVal catalystCode As String, ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetZeckLinkExists] @1,@2", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalystCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetZeckLinkExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllZECTJobs(ByVal dateFrom As String, ByVal dateTo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllZECTJobs] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No ZECT jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllZECTJobs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GeZECTJobInPuts(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GeZECTJobInPuts] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
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
                        Return "0*No ZECT jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GeZECTJobInPuts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GeZECTJobOutPuts(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   EXEC [dbo].[sp_UI_GeZECTJobOutPuts] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
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
                        Return "0*No ZECT jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GeZECTJobOutPuts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GeZECTJobsToManufacture()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GeZECTJobsToManufacture] ", sqlConn)
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
                        Return "0*No ZECT jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GeZECTJobsToManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GeZECTPalletsToManufacture(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GeZECTPalletsToManufacture] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
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
                        Return "0*No ZECT jobs were found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GeZECTJobsToManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetZECTBatchTotal(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	EXEC [dbo].[sp_UI_GetZECTBatchTotal] @1", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetZECTBatchTotal: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetZECTRawMaterials(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("	EXEC [dbo].[sp_UI_GetZECTRawMaterials] @1", sqlConn)
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
#End Region

#Region "Tablet"
            Public Shared Function Zect_GetCatalystSlurries(ByVal itemCode As String, ByVal coatNum As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetCatalystSlurries] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + coatNum + "%"))
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetInsertedLineID(ByVal catalystCode As String, ByVal itemCode As String, ByVal lotNum As String, ByVal slurry As String, ByVal coatNum As String, ByVal zectLine As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetInsertedLineID] @1,@2,@3,@4,@5,@6", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalystCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@4", slurry))
                    sqlComm.Parameters.Add(New SqlParameter("@5", coatNum))
                    sqlComm.Parameters.Add(New SqlParameter("@6", zectLine))
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
                        Return "-1*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetInsertedLineID: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_CheckJobOnLine(ByVal whseCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_CheckJobOnLine] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*The is already a job running on this line, please close it and try again"
                    Else
                        Return "1*No jobs running"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No jobs running"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_CheckJobOnLine: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetJobID(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetJobID] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
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
                        Return "0*Job not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Job not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetJobID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function Zect_GetConfigTagInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetConfigTagInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLotExpiry: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetLabelInfo(ByVal itemCode As String, ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetLabelInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = jobNo + "|" + sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + sqlReader.Item(7)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLotExpiry: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetLabelInfo_ZectTage(ByVal itemCode As String, ByVal palletCode As String, ByVal palletNo As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetLabelInfo_ZectTage] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = palletCode + "|" + palletNo + "|" + sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + sqlReader.Item(7) + "~" + unq
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLotExpiry: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetReprintInfo_ZectTag(ByVal itemCode As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetReprintInfo_ZectTag] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLotExpiry: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetJobOpenInfo(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetJobOpenInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
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
                        Return "0*Job not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Job not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetJobOpenInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetJobOpen(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetJobOpen] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
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
                        Return "0*Job not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Job not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetJobOpen: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetLineJobOpen(ByVal unq As String, ByVal whse As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetLineJobOpen] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whse))
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
                        Return "0*Job not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Job not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetJobOpen: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetJobInformation(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetJobInformation] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
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
                        Return "0*Job not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Job not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetJobOpen: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetLastJobPallet(ByVal jobId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vPalletCode] FROM [tbl_RTIS_Zect_OutPut] WHERE [iJobID] = @1 ORDER BY [iLIneID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobId))
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
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetLastJobPallet: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetJobPallets(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetJobPallets] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetLastJobPallet: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetUerPermissions(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetUerPermissions] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetUerPermissions: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetUerPermissionsReOpen(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetUerPermissionsReOpen] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetUerPermissions: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetClosingJobInfo(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetClosingJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetClosingJobInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetReOpenJobInfo(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetReOpenJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetReOpenJobInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetReprintJobInfo(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetReprintJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No pallets printed yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No pallets printed yet"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetReprintJobInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetValidJobLots(ByVal itemCode As String, ByVal days As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetValidJobLots] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", Convert.ToInt32(days)))
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetValidReprintJobLots(ByVal itemCode As String, ByVal days As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetValidReprintJobLots] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", Convert.ToInt32(days)))
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetValidReprintJobLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetJobNumber(ByVal itemCode As String, ByVal lot As String, ByVal coat As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetJobNumber] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", coat))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whse))
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
                        Return "0*No job was found with this information!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetReprintJobNumber(ByVal itemCode As String, ByVal lot As String, ByVal coat As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetReprintJobNumber] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@3", coat))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whse))
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
                        Return "0*No job was found with this information!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetReprintJobNumber: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetreOpenLabelInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetreOpenLabelInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + sqlReader.Item(7)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetreOpenLabelInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetReprintLabelInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetReprintLabelInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + sqlReader.Item(7)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetreOpenLabelInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

#End Region

#Region "MBL"
            Public Shared Function MBL_GetJobOutPutItem(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_MBL_GetJobOutPutItem] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Job not found!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Job not found!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetJobOutPutItem: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_checkItemRM(ByVal catalyst As String, ByVal rm As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_checkItemRM] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalyst))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rm))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Invalid item for catalyst!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Invalid item for catalyst!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_checkItemRM: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region
        End Class
        Partial Public Class Insert
            Public Shared Function Zect_OpenJobOnLine(ByVal unq As String, ByVal itemCode As String, ByVal itemLot As String, ByVal slurryCode As String, ByVal slurryLot As String,
                                                       ByVal qty As String, ByVal coatNum As String, ByVal zectLine As String, ByVal username As String, ByVal checkSheet As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_Zect_OpenJobOnLine] @1,@2,@3,@4,@5,@6,@7,@8,@9,@10", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemLot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@5", slurryLot))
                    sqlComm.Parameters.Add(New SqlParameter("@6", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@7", coatNum))
                    sqlComm.Parameters.Add(New SqlParameter("@8", zectLine))
                    sqlComm.Parameters.Add(New SqlParameter("@9", username))
                    sqlComm.Parameters.Add(New SqlParameter("@10", checkSheet))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_OpenJobOnLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_LogRM(ByVal lineID As String, ByVal itemCode As String, ByVal itemLot As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_LogRM] @1,@2,@3,@4,@5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemLot))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_OpenJobOnLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_ManufacturePalletUnprinted(ByVal baseCode As String, ByVal itemCode As String, ByVal itemDesc As String, ByVal lotNumber As String, ByVal coatNum As String, ByVal slurryCode As String,
                                                                   ByVal qty As String, ByVal zectLine As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_ManufacturePalletUnprinted] @1,@2,@3,@4,@5,@6,@7,@8,@9", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", baseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@5", coatNum))
                    sqlComm.Parameters.Add(New SqlParameter("@6", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@7", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@8", zectLine))
                    sqlComm.Parameters.Add(New SqlParameter("@9", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_ManufacturePalletUnprinted: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_AddNewPallet(ByVal jobID As String, ByVal palletCode As String, ByVal palletNo As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_AddNewPallet] @1,@2,@3,@4,@5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", palletCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", palletNo))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_AddNewPallet: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_InsertRMLink(ByVal catalystCode As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertRMLink] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalystCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_OpenJobOnLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function ZECT_UpdateManufacturedQty(ByVal jobNo As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ZECT_UpdateManufacturedQty] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "ZECT_UpdateManufacturedQty: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function ZECT_UpdateJobClosed(ByVal jobNo As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ZECT_UpdateJobClosed] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "ZECT_UpdateJobClosed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function ZECT_UpdateJobReOpened(ByVal jobNo As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ZECT_UpdateJobReOpened] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "ZECT_UpdateJobClosed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateZectTransferred(ByVal lineID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_UpdateZectTransferred] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateZectTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateAWTransferred(ByVal lineID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_RTIS_AW] SET [bTrans] = 1, [dtTrans]= GETDATE(), [vUserTrans] = @2
                                                    WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateAWTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_UpdateZectLine_Printed(ByVal iLineID As String, ByVal userPrinted As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_UpdateZectLine_Printed] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", iLineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", userPrinted))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_UpdateAWLine_Printed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdatePalletManufactured(ByVal lineID As String, ByVal jobID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_UpdatePalletManufactured] @1,@2,@3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", jobID))
                    sqlComm.Parameters.Add(New SqlParameter("@3", userName))
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
            Public Shared Function UI_setZECTBatchManufactured(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setZECTBatchManufactured] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setZECTBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_setZECTBatchManufacturedManual(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setZECTBatchManufacturedManual] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setZECTBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_ManualCloseJob(ByVal lotNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ManualCloseZectJob] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return ReturnData
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_ManualCloseJob: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Delete
            Public Shared Function UI_DeleteRMLink(ByVal catalystCode As String, ByVal rmCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteRMLink] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalystCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", rmCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_OpenJobOnLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class
    Public Class Evoltion
        Partial Public Class Retreive
#Region "UI"
            Public Shared Function UI_GetAllCatalysts()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllCatalysts] ", sqlConn)


                    'Select Case [ucIICoatStage],[Description_1],[Description_2], '' FROM [StkItem]
                    '                                  WHERE [ucIICoatStage] Like '18461%' OR ([ucIICoatStage] LIKE 'V%' AND [ucIICoatStage] NOT LIKE 'VS%')
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllCatalysts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetAllCatalystRMs()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_GetAllCatalystRMs]", sqlConn)
                    'SELECT [Code],[Description_1], '' FROM [StkItem]
                    'WHERE [ItemGroup] LIKE '%007%' OR [ItemGroup] LIKE '%011%'
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllCatalystRMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function Zect_GetMFCode(ByVal catalyst As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_Zect_GetMFCode] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalyst))
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
                        Return "0*No code was found for the catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetMFCode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetItemCodeFromMFCode(ByVal catalyst As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetItemCodeFromMFCode] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", catalyst))
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
                        Return "0*No code was found for the catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetItemCodeFromMFCode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region
            Public Shared Function Zect_GetItemCode(ByVal catalyst As String, ByVal coatNum As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetItemCode] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", "%" + catalyst + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@2", "%" + coatNum + "%"))
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
                        Return "0*No code was found for the catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function Zect_GetItemLotsFromTanks(ByVal code As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetItemLotsFromTanks] @1,@2", sqlConn)

                    sqlComm.Parameters.Add(New SqlParameter("@1", code))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whse))
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
                        Return "0*No lots were found for this item that had a qty above 0"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetSlurryTanks(ByVal code As String, ByVal lot As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetSlurryTanks] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", code))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
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
                        Return "0*No tanks were found for this item lot that have not already been received"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function Zect_GetReOpenCatalystCoats(ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_Zect_GetReOpenCatalystCoats] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetRePrintCatalystCoats(ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_ZZect_GetReOpenCatalystCoats] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        Return "0*No coats found for catalyst!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetRePrintCatalystCoats: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Zect_GetItemDescription(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_Zect_GetItemDescription] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
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
                        Return "-1*Item not found in evolution!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Zect_GetItemDescription: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function Zect_GetItemLotsFromTank_AddSlurry(ByVal code As String, ByVal whse As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Zect_GetItemLotsFromTank_AddSlurry] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", code))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whse))
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
                        Return "0*No lots were found for this item that had a qty above 0"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetItemLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class
End Class
