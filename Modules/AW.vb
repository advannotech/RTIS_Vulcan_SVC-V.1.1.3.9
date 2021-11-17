﻿Imports System.Data.SqlClient
Public Class AW

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retrieve
#Region "UI"
            Public Shared Function UI_GetAWCatalystRaws(ByVal catalystCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWCatalystRaws] @1", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWCatalystRaws: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAWLinkExists(ByVal catalystCode As String, ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWLinkExists] @1,@2", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWLinkExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GeAWJobsToManufacture()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GeAWJobsToManufacture]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GeAWJobsToManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAWPalletsToManufacture(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWPalletsToManufacture] @1", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWPalletsToManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetAWBatchTotal(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWBatchTotal] @1", sqlConn)
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
                        Return "0*No quantity found for the selected batch"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWBatchTotal: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAWRawMaterials(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWRawMaterials] @1", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWRawMaterials: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Tablet"
            Public Shared Function AW_CheckJobRunning()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_AW_CheckJobRunning]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*There is already a job running!"
                    Else
                        Return "1*no jobs found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_CheckJobRunning: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_CheckSpecificJobRunning(ByVal jobNo)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_CheckSpecificJobRunning] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*the specified job is not currently running"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_CheckSpecificJobRunning: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetJobID(ByVal jobNo)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetJobID] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*the specified job is not currently running"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetJobID: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetJobInfo(ByVal jobNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The scanned job was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetJobInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetLastJobPallet(ByVal jobId As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetLastJobPallet] @1", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetLastJobPallet: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetLabelInfo_AWTags(ByVal itemCode As String, ByVal palletCode As String, ByVal palletNo As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetLabelInfo_AWTags] @1", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetLabelInfo_AWTage: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetValidReprintJobLots(ByVal itemCode As String, ByVal days As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetValidReprintJobLots] @1, @2", sqlConn)
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
                        Return "0*No lots found for item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetValidReprintJobLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetReprintJobNumber(ByVal itemCode As String, ByVal lot As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetReprintJobNumber] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetReprintJobNumber: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetReprintJobInfo(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetReprintJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + jobNo
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
                        Return "0*No job was found with this information!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetReprintJobInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetReprintLabelInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetReprintLabelInfo] @1", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetReprintLabelInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetJobPallets(ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetJobPallets] @1", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetJobPallets: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetAWUnq(ByVal itemCode As String, ByVal jobNO As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetAWUnq] @1, @2", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetAWUnq: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function AW_GetReprintInfo_AWTag(ByVal itemCode As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetReprintInfo_AWTag] @1", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetReprintInfo_AWTag: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function AW_GetJobInfo_CJ(ByVal jobNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetJobInfo_CJ] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The scanned job was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetJobInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_CheckJobRunning_RO()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_CheckJobRunning_RO]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*There is already a job running!"
                    Else
                        Return "1*no jobs found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_CheckJobRunning_RO: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetJobInfo_RO(ByVal jobNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetJobInfo_RO] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The scanned job was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetJobInfo_RO: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetValidReopenJobLots(ByVal itemCode As String, ByVal days As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetValidReopenJobLots] @1, @2", sqlConn)
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
                        Return "0*No lots found for item!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetValidReopenJobLots: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetReprintJobNumber_RO(ByVal itemCode As String, ByVal lot As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetReprintJobNumber_RO] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lot))
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetReprintJobNumber_RO: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "MBL"
            Public Shared Function MBL_GetJobInfo(ByVal jobNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetJobInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*The scanned job was not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetJobInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetJobRunning(ByVal jobNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetJobRunning] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*no jobs have been found for A&W"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetJobRunning: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetJobID(ByVal jobNo)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetJobID] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
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
                        Return "0*the specified job is not currently running"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetJobID: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetUnqOnJob(ByVal unq As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetUnqOnJob] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*The pallet has already been scanned as a raw material"
                    Else
                        Return "1*Not yet scanned"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetUnqOnJob: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllAWJobs(ByVal dateFrom As String, ByVal dateTo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllAWJobs] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
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

            Public Shared Function UI_GetAWJobInPuts(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWJobInPuts] @1", sqlConn)
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
                        Return "0*No inputs were found for this job!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWJobInPuts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAWJobOutputs(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAWJobOutputs] @1", sqlConn)
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
                        Return "0*No outputs were found for this job!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWJobOutputs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Insert
#Region "UI"
            Public Shared Function UI_InsertRMLink(ByVal awCode As String, ByVal awDesc As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertRMLink] @1,@2,@3,@4,@5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", awCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", awDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@3", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
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
#End Region

#Region "Tablet"
            Public Shared Function UI_InsertNewAWJob(ByVal jobNumber As String, ByVal code As String, ByVal lotNumber As String, ByVal PGM As String, ByVal PGMLot As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertNewAWJob] @1,@2,@3,@4,@5,@6,@7", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", code))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", PGM))
                    sqlComm.Parameters.Add(New SqlParameter("@5", PGMLot))
                    sqlComm.Parameters.Add(New SqlParameter("@6", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@7", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertNewAWJob: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function AW_AddNewPallet(ByVal jobID As String, ByVal palletCode As String, ByVal palletNo As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_AddNewPallet] @1, @2, @3, @4, @5", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_AddNewPallet: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "MBL"
            Public Shared Function UI_InsertNewAWJobRawMaterial(ByVal jobID As String, ByVal code As String, ByVal lotNumber As String, ByVal qty As String, ByVal username As String, ByVal palletNo As String, ByVal ZectJob As String, ByVal palletUnq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertNewAWJob] @1, @2, @3, @4, @5, @6, @7, @8", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", code))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlComm.Parameters.Add(New SqlParameter("@6", palletNo))
                    sqlComm.Parameters.Add(New SqlParameter("@7", ZectJob))
                    sqlComm.Parameters.Add(New SqlParameter("@8", palletUnq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertNewAWJobRawMaterial: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Delete
#Region "UI"
            Public Shared Function UI_DeleteRMLink(ByVal awCode As String, ByVal rmCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteRMLink] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", awCode))
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
            Public Shared Function AW_UpdateManufacturedQty(ByVal jobNo As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_UpdateManufacturedQty] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_UpdateManufacturedQty: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_UpdateJobClosed(ByVal jobNo As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_UpdateJobClosed] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_UpdateJobClosed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_UpdateJobReOpened(ByVal jobNo As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_UpdateJobReOpened] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", jobNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_UpdateJobReOpened: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdatePalletManufactured(ByVal lineID As String, ByVal jobID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdatePalletManufactured] @1, @2", sqlConn)
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
            Public Shared Function UI_setAWBatchManufactured(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setAWBatchManufactured] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setAWBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_setAWBatchManufacturedManual(ByVal headerID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setAWBatchManufacturedManual] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setAWBatchManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_ManualCloseJob(ByVal lotNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ManualCloseAWJob] @1", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_ManualCloseJob: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retrieve

#Region "UI"
            Public Shared Function UI_GetAllAWCatalysts()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllAWCatalysts]", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllAWCatalysts: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllAWWRMs()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllAWWRMs]", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllAWWRMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
#End Region

#Region "Tablet"
            Public Shared Function AW_GetZectMFCode(ByVal baseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", baseCode))
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
                        Return "0*No A&W version of this product was found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetZectMFCode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetAWItemCode(ByVal baseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetAWItemCode] @1", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", baseCode + "%"))
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
                        Return "0*No A&W version of this product was found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetAWItemCode: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetAWAvailablePGMs(ByVal itemCode As String, ByVal whseCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetAWAvailablePGMs] @1, @2", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whseCode))
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
                        Return "0*No available raw materials found for this product!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetAWAvailablePGMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AW_GetLabelInfo(ByVal itemCode As String, ByVal jobNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AW_GetLabelInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + sqlReader.Item(4) + "|" + sqlReader.Item(5) + "|" + sqlReader.Item(6) + "|" + jobNo
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetLabelInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

        End Class
    End Class
End Class
