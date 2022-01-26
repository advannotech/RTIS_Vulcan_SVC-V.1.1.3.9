Imports System.Data.SqlClient
Imports System.Threading
Imports Pastel.Evolution

Public Class WhseTransfers

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
     "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Shared sep As String = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator

    Public Class RTSQL
        Partial Public Class Retreive
            Public Shared Function GetWhseStockQtys(ByVal whseCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetWhseStockQtys] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*No stock items were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No stock items were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "Server Response Evolution Error: " + ex.Message)
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function UI_getWhtProcesses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_getWhtProcesses] ", sqlConn)
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
            Public Shared Function UI_getWhtLInes(ByVal status As String, ByVal process As String, ByVal rows As String, transStart As String, transEnd As String, failedStart As String, failedEnd As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetTransfers] @1,@2,@3,@4,@5,@6,@7", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", transStart))
                    sqlComm.Parameters.Add(New SqlParameter("@2", transEnd))
                    sqlComm.Parameters.Add(New SqlParameter("@3", failedStart))
                    sqlComm.Parameters.Add(New SqlParameter("@4", failedEnd))
                    sqlComm.Parameters.Add(New SqlParameter("@5", process))
                    sqlComm.Parameters.Add(New SqlParameter("@6", status))
                    sqlComm.Parameters.Add(New SqlParameter("@7", rows))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse transfer lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse transfer lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtProcesses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWhtFGRequests(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_getWhtFGRequests] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
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
                        Return "0*No FG transfer requests were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No FG transfer requests were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtFGRequests: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getWhtFGReports(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_getWhtFGReports] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@2", dateTo))
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
                        Return "0*No FG transfer records were found for the specified date range"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No FG transfer records were found for the specified date range"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtFGReports: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetFGTransferInfo(ByVal iLineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_UI_GetFGTransferInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", iLineID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No FG transfer request was found with the specified ID"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No FG transfer request was found with the specified ID"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFGTransferInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWhtLInesAll(ByVal process As String, ByVal rows As String, transStart As String, transEnd As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetAllTransfers] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", transStart))
                    sqlComm.Parameters.Add(New SqlParameter("@2", transEnd))
                    sqlComm.Parameters.Add(New SqlParameter("@3", process))
                    sqlComm.Parameters.Add(New SqlParameter("@4", rows))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse transfer lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse transfer lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtProcesses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_getWhtLInesPosted(ByVal process As String, ByVal rows As String, transStart As String, transEnd As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetPostedTransfers] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", transStart))
                    sqlComm.Parameters.Add(New SqlParameter("@2", transEnd))
                    sqlComm.Parameters.Add(New SqlParameter("@3", process))
                    sqlComm.Parameters.Add(New SqlParameter("@4", rows))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouse transfer lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouse transfer lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_getWhtProcesses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function SVC_GetPendingWhseTransfers() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_SVC_GetPendingWhseTransfers] ", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Pending Lines!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Pending Lines!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_GetPendingWhseTransfers: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class

        Partial Public Class Update
            Public Shared Function SVC_UpdateWhseTransferFailed(ByVal id As String, ByVal reason As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_SVC_UpdateWhseTransferFailed] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_UpdateWhseTransferFailed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function SVC_UpdateWhseTransferLine(ByVal lot As String, ByVal qty As String, ByVal status As String, ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_SVC_UpdateWhseTransferLine] @1, @2, @3, @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@3", status))
                    sqlComm.Parameters.Add(New SqlParameter("@4", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_UpdateWhseTransferFailed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function SVC_UpdateWhseTransferLinePending(ByVal lot As String, ByVal qty As String, ByVal status As String, ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_SVC_UpdateWhseTransferLinePending] @1, @2, @3, @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@3", status))
                    sqlComm.Parameters.Add(New SqlParameter("@4", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_UpdateWhseTransferFailed: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function SVC_UpdateWhseTransfersFailedToPending() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_SVC_UpdateWhseTransfersFailedToPending] ", sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_UpdateWhseTransfersFailedToPending: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Insert
            Public Shared Function SVC_InsertWHTLineCompleted(ByVal id As String, ByVal code As String, ByVal lotnumber As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String,
                                                              ByVal dtEntered As String, ByVal username As String, ByVal process As String, ByVal transDesc As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_SVC_InsertWHTLineCompleted] @1, @2, @3, @4, @5, @6, @7, @8, @9, @10 ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", code))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotnumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@5", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@6", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@7", dtEntered))
                    sqlComm.Parameters.Add(New SqlParameter("@8", username))
                    sqlComm.Parameters.Add(New SqlParameter("@9", process))
                    sqlComm.Parameters.Add(New SqlParameter("@10", transDesc))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_InsertWHTLineCompleted: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function SVC_InsertFGWHTLineCompleted(ByVal id As String, ByVal code As String, ByVal lotnumber As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String,
                                                             ByVal dtEntered As String, ByVal userRequested As String, ByVal username As String, ByVal process As String, ByVal status As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_SVC_InsertFGWHTLineCompleted] @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11 ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", code))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotnumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@5", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@6", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@7", dtEntered))
                    sqlComm.Parameters.Add(New SqlParameter("@8", userRequested))
                    sqlComm.Parameters.Add(New SqlParameter("@9", username))
                    sqlComm.Parameters.Add(New SqlParameter("@10", process))
                    sqlComm.Parameters.Add(New SqlParameter("@11", status))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_InsertFGWHTLineCompleted: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Delete
            Public Shared Function UI_DeleteFGWhseTransLineComplete(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_DeleteFGWhseTransLineComplete] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteFGWhseTransLineComplete: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function SVC_DeleteWhseTransLineComplete(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_SVC_DeleteWhseTransLineComplete] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "SVC_DeleteWhseTransLineComplete: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function GetItemDesc(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_GetItemDesc] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        Return "-1*Item not found in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Item not found in evolution"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "Server Response Evolution Error: " + ex.Message)
                        Return "-1*" + ex.Message
                    End If
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
                errorItem.Description = ex.Message
                Return errorItem
            End Try
        End Function

        Public Shared Function CGetLot(ByVal lotNumber As String, ByVal stockId As String) As Lot
            Initialize()
            Dim errorLot As Lot = New Lot()
            errorLot.Code = lotNumber
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
                EventLog.WriteEntry("RTIS Vulcan SVC", "CGetWhse: " + ex.ToString)
                Return Nothing
            End Try
        End Function

        Public Shared Function CTransferItem(ByVal OrderNo As String, ByVal FromWarehouseCode As String, ByVal ToWarehouseCode As String, ByVal ItemCode As String, ByVal Lot As String, ByVal Qty As String) As String
            Try
                Initialize()
                Dim thistransfer As New WarehouseTransfer()
                Dim invItem As New InventoryItem()
                invItem = CGetItem(ItemCode)
                If invItem.Code <> "ErrorItem" Then
                    thistransfer.Date = Now

                    thistransfer.Description = "RTIS Warehouse Transfer from: " + FromWarehouseCode + " to: " + ToWarehouseCode
                    thistransfer.ExtOrderNo = OrderNo
                    thistransfer.FromWarehouse = CGetWhse(FromWarehouseCode)
                    thistransfer.ToWarehouse = CGetWhse(ToWarehouseCode)
                    thistransfer.InventoryItem = invItem
                    thistransfer.Lot = CGetLot(Lot, invItem.ID)
                    thistransfer.Quantity = Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                    thistransfer.Reference = OrderNo
                    If thistransfer.Validate() Then
                        thistransfer.Post()
                        Return "1*Success"
                    Else
                        Return "-1*The warehouse transfer could not be completed"
                    End If
                Else
                    Return invItem.Description
                End If
            Catch ex As Exception
                If ex.Message.Contains("Negative lot tracking quantity not allowed!") Then
                    EventLog.WriteEntry("RTIS SVC", "CTransferItem: " + ex.ToString)
                    Return "-1*Insufficient qty for lot"
                Else
                    EventLog.WriteEntry("RTIS SVC", "CTransferItem: " + ex.ToString)
                    Return "-1*Reason: " + ex.Message
                End If

            End Try
        End Function
    End Class
End Class
