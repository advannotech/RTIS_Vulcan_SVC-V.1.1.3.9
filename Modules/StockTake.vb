﻿Imports System.Data.SqlClient
Imports System.Threading

Public Class StockTake
    Public Class RTSQL
        Public Shared sep As String = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
        Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
            "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
        Partial Public Class Retreive

#Region "UI"
            Public Shared Function UI_GetRTStockTakes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [cInvCountNo] ,[cDescription]
                                                    FROM [RTIS_InvCount] WHERE [iStatus] = 1", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Stock Takes were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Stock Takes were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetRTStockTakes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckSTHeader(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [cInvCountNo]
                                                    FROM [RTIS_InvCount] WHERE [iStatus] = 1 AND [cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "~"
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*The stock take already exists please archive and delete it before trying to create it again"
                    Else
                        Return "1*No Stock Takes were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No Stock Takes were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_CheckSTHeader: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getInvCountLineTickets(ByVal headerID) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT [iLineID],[vTicketNo],[dCountQty],[vCountUser] ,[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2],[vBarcodeType],[bCountValid],[bRecountTicket]
                                                     FROM [RTIS_InvCountLines_Tickets]
                                                     WHERE [iHeaderID] =@1 AND [bCountValid] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString() + "|" + sqlReader.Item(3).ToString() + "|" + sqlReader.Item(4).ToString() + "|" + sqlReader.Item(5).ToString() +
                         "|" + sqlReader.Item(6).ToString() + "|" + sqlReader.Item(7).ToString() + "|" + sqlReader.Item(8).ToString() + "|" + sqlReader.Item(9).ToString() + "|" + sqlReader.Item(10).ToString() + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No tickets were found for this item"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No tickets were found for this item"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_getInvCountLineTickets: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getInvCountLineTickets_Archive(ByVal headerID) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT [iLineID],[vTicketNo],[dCountQty],[vCountUser] ,[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2],[vBarcodeType],[bCountValid],[bRecountTicket]
                                                     FROM [RTIS_InvCountArchiveLines_Tickets]
                                                     WHERE [iHeaderID] =@1 AND [bCountValid] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString() + "|" + sqlReader.Item(3).ToString() + "|" + sqlReader.Item(4).ToString() + "|" + sqlReader.Item(5).ToString() +
                         "|" + sqlReader.Item(6).ToString() + "|" + sqlReader.Item(7).ToString() + "|" + sqlReader.Item(8).ToString() + "|" + sqlReader.Item(9).ToString() + "|" + sqlReader.Item(10).ToString() + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No tickets were found for this item"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No tickets were found for this item"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_getInvCountLineTickets_Archive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getTicketInfo(ByVal ticketNo As String, ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([dCountQty],0), ISNULL([dCountQty2], 0), [vUnqBarcode] FROM [RTIS_InvCountLines_Tickets]
                                                    WHERE [vTicketNo] = @1 AND [iHeaderID] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString()
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No infromation was found for the selected ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No infromation was found for the selected ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_getTicketInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            
            Public Shared Function UI_getAllTicketInfo(ByVal ticketNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [iHeaderID], ISNULL([dCountQty],0), ISNULL([dCountQty2], 0), [vUnqBarcode] FROM [RTIS_InvCountLines_Tickets]
                                                    WHERE [vTicketNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", ticketNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString() + "|" + sqlReader.Item(3).ToString() + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No infromation was found for the selected ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No infromation was found for the selected ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_getTicketInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_getStQtys(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [fCountQty] , [fCountQty2] FROM [RTIS_InvCountLines]
                                                    WHERE [idInvCountLines] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString()
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No infromation was found for the selected ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No infromation was found for the selected ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_getTicketInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetExportFormats() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vFormatName]
                                                FROM [tbl_ExportFormats] ", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "~"
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No export formats found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No export formats found"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetExportFormats: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetExportFormatString(ByVal name As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vFormatString], [vDelimeter]
                                                FROM [tbl_ExportFormats] WHERE [vFormatName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~" + Convert.ToString(sqlReader.Item(1))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*format string not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*format string not found"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_CheckExportFormatExists: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckExportFormatExists(ByVal name As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vFormatName]
                                                FROM [tbl_ExportFormats] WHERE [vFormatName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "-1*A format with this name already exists"
                    Else
                        Return "1*Ok to create"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Ok to create"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_CheckExportFormatExists: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCount(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
                                                     INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                     WHERE i.[cInvCountNo] = @1 AND [bOnST] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCount: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCountArchive(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
                                                     INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                     WHERE i.[cInvCountNo] = @1 AND [bOnST] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCountArchive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetSTLinesCountAll(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
                                                     INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                     WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCountAll: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCountAllArchive(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
                                                     INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                     WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCountAllArchive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCount1(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
                                                    INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                    WHERE i.[cInvCountNo] = @1 AND [fCountQty] <> 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCount1: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCount1Archive(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
                                                    INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                    WHERE i.[cInvCountNo] = @1 AND [fCountQty] <> 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCount1Archive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCount2(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT COUNT([iStockID]) FROM [RTIS_InvCountLines] il
                                                    INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                    WHERE i.[cInvCountNo] = @1 AND [fCountQty2] <> 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCount2: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetSTLinesCount2Archive(ByVal stNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT COUNT([iStockID]) FROM [RTIS_InvCountArchiveLines] il
                                                    INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                    WHERE i.[cInvCountNo] = @1 AND [fCountQty2] <> 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No lines found for the selected stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lines found for the selected stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetSTLinesCount2Archive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetRTArchiveStockTakes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [cInvCountNo],[cDescription]
                                                FROM [RTIS_InvCount] WHERE [iStatus] = 0", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Stock Takes were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Stock Takes were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetRTArchiveStockTakes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "MBL"
            Public Shared Function MBL_GetRTStockTakes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [cInvCountNo]
                                                FROM [RTIS_InvCount] WHERE [iStatus] = 1", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "~"
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Stock Takes were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Stock Takes were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetRTStockTakes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetStockTakeWarehosesBySTNum(ByVal stockTakeNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT w.[Code], w.[Name] FROM [" + My.Settings.RTDB + "].[dbo].[RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = il.[iWarehouseID] 
                                               INNER JOIN [" + My.Settings.RTDB + "].[dbo].[RTIS_InvCount] i ON i.[idInvCount] = il.[iInvCountID]
                                               WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockTakeNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0) + "|" + sqlReader.Item(1) + "~"
                    While sqlReader.Read()
                        ReturnData &= sqlReader.Item(0) + "|" + sqlReader.Item(1) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No warehouses were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No warehouses were found"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetStockTakIDBySTNum(ByVal stockTakeNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [idInvCount] FROM [RTIS_InvCount] WHERE [cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockTakeNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No id was found for this stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No id was found for this stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakIDBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSTLineIDNoLot(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[idInvCountLines] FROM [RTIS_InvCountLines] l
                                                    INNER JOIN [RTIS_InvCount] h ON h.[idInvCount] = l.[iInvCountID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[StockLink] = l.[iStockID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = l.[iWarehouseID] 
                                                    WHERE h.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[bLotItem] = 0", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Item not found on stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Item not found on stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSTLineIDLot(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[idInvCountLines] FROM [RTIS_InvCountLines] l
                                                    INNER JOIN [RTIS_InvCount] h ON h.[idInvCount] = l.[iInvCountID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[StockLink] = l.[iStockID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = l.[iWarehouseID] 
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_etblLotTracking] lo ON lo.[idLotTracking] = l.[iLotTrackingID]
                                                    WHERE h.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND lo.[cLotDescription] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Item not found on stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Item not found on stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetSTLineIDsRMPallet(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String, ByVal lotNumbers As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand($" SELECT l.[idInvCountLines] FROM [RTIS_InvCountLines] l
                                                    INNER JOIN [RTIS_InvCount] h ON h.[idInvCount] = l.[iInvCountID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s ON s.[StockLink] = l.[iStockID]
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = l.[iWarehouseID] 
                                                    INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_etblLotTracking] lo ON lo.[idLotTracking] = l.[iLotTrackingID]
                                                    WHERE h.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND lo.[cLotDescription] IN " + lotNumbers + "", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Item not found on stock take"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Item not found on stock take"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_checkRT2dForRecount(ByVal headerID As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [bCountValid] FROM [RTIS_InvCountLines_Tickets]
                                                    WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_checkRT2dForRecountRMPallet(ByVal headerIDs As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand($" SELECT TOP 1 [bCountValid] FROM [RTIS_InvCountLines_Tickets]
                                                    WHERE [iHeaderID] IN {headerIDs} AND [vUnqBarcode] = @UNQ ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@UNQ", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_checkPowderPrepForRecount(ByVal headerID As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [bCountValid] FROM [RTIS_InvCountLines_Tickets]
                                                    WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND ISNULL([bCountValid], 0) = 0 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The system cannot recount this item as it has not been counted yet"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_checkTicketCounts(ByVal headerID As String, ByVal ticket As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([dCountQty], 0), ISNULL([dCountQty2],0), [vUnqBarcode] FROM [RTIS_InvCountLines_Tickets] WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [bCountValid] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticket))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString()
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No counts found for this ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No counts found for this ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_checkTicketNumber(ByVal headerID As String, ByVal ticket As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vTicketNo] FROM [RTIS_InvCountLines_Tickets] WHERE [iHeaderID] = @1 AND [vTicketNo] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticket))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0)
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This ticket has already been used in the stock take"
                    Else
                        Return "1*Ok to use ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Ok to use ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_checkTicketNumber: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_getFreshSlurryInfo(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 0)  FROM [tbl_RTIS_Fresh_Slurry]
                                                    WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getFreshSlurryInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getFreshSlurryInfo_CheckAddLot(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bRecTrans], 0)  FROM [tbl_RTIS_Fresh_Slurry]
                                                    WHERE [vTrolleyCode] = @1 AND [vItemCode] = @2 ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getFreshSlurryInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getSlurryTicketInfo(ByVal headerID As String, ByVal tankCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT ISNULL([dCountQty], 0), ISNULL([dCountQty2],0) FROM [RTIS_InvCountLines_Tickets] WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND [bCountValid] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tankCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString()
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No counts found for this ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No counts found for this ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetStockTakeWarehosesBySTNum: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            'Public Shared Function MBL_getFreshSlurryTicketUpdateInfo(ByVal headerID As String, ByVal ticketNumber As String) As String
            '    Try
            '        Dim ReturnData As String = ""
            '        Dim sqlConn As New SqlConnection(RTString)
            '        Dim sqlComm As New SqlCommand("  SELECT TOP 1 ISNULL([dCountQty], 0), ISNULL([dCountQty2], 0), [vUnqBarcode], [bCountValid] FROM [RTIS_InvCountLines_Tickets]
            '                                         WHERE [iHeaderID] = @1 AND [vTicketNo] = @2", sqlConn)
            '        sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
            '        sqlComm.Parameters.Add(New SqlParameter("@2", ticketNumber))
            '        sqlConn.Open()
            '        Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
            '        While sqlReader.Read()
            '            ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString() + "|" + sqlReader.Item(3).ToString()
            '        End While
            '        sqlReader.Close()
            '        sqlComm.Dispose()
            '        sqlConn.Close()

            '        If ReturnData <> "" Then
            '            Return "1*" + ReturnData
            '        Else
            '            Return "0*No counts found for this ticket"
            '        End If
            '    Catch ex As Exception
            '        If ex.Message = "Invalid attempt to read when no data is present." Then
            '            Return "0*No counts found for this ticket"
            '        Else
            '            EventLog.WriteEntry("RTIS SVC", "MBL_getFreshSlurryTicketUpdateInfo: " + ex.ToString())
            '            Return ExHandler.returnErrorEx(ex)
            '        End If
            '    End Try
            'End Function
            Public Shared Function MBL_getTicketUpdateInfo(ByVal headerID As String, ByVal ticketNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 ISNULL([dCountQty], 0), ISNULL([dCountQty2], 0), [vUnqBarcode], [bCountValid] FROM [RTIS_InvCountLines_Tickets]
                                                     WHERE [iHeaderID] = @1 AND [vTicketNo] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString() + "|" + sqlReader.Item(1).ToString() + "|" + sqlReader.Item(2).ToString() + "|" + sqlReader.Item(3).ToString()
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No counts found for this ticket"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No counts found for this ticket"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_getFreshSlurryTicketUpdateInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_getFreshSlurryUnqTicket(ByVal headerID As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vTicketNo] FROM [RTIS_InvCountLines_Tickets]
                                                     WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2 AND [bCountValid] = 1 ORDER BY [iLineID] DESC", sqlConn) ' AND [bCountValid] = 1
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString()
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
                        EventLog.WriteEntry("RTIS SVC", "MBL_getFreshSlurryUnqTicket: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_getUnqcodeTicket(ByVal headerID As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vTicketNo] FROM [RTIS_InvCountLines_Tickets]
                                                     WHERE [iHeaderID] = @1 AND [vUnqBarcode] = @2  AND [bCountValid] = 1 ORDER BY [iLineID] DESC", sqlConn) ' 
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = sqlReader.Item(0).ToString()
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
                        EventLog.WriteEntry("RTIS SVC", "MBL_getUnqcodeTicket: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_getMixedSlurryTankInfo(ByVal tankNo As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bReceived], 0), ISNULL([bManualclose], 0)  FROM [tbl_RTIS_MS_Main]
                                                     WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vTankType] = 'TNK' ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getMixedSlurryTankInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getMixedSlurryTankInfo_CheckLot(ByVal tankNo As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT TOP 1 [vLotNumber], [dDryWeight], ISNULL([bReceived], 0), ISNULL([bManualclose], 0)  FROM [tbl_RTIS_MS_Main]
                                                     WHERE [vTankCode] = @1 AND [vItemCode] = @2 AND [vTankType] = 'TNK' ORDER BY [iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getMixedSlurryTankInfo_CheckLot: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getMixedSlurryMobileTankInfo_CheckLot(ByVal tankNo As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("     SELECT TOP 1 m.[vLotNumber], md.[dDryWeight], ISNULL(md.[bReceived], 0), ISNULL(md.[bManualclose], 0)  FROM [tbl_RTIS_MS_Decant] md 
                                                        INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = md.[iHeaderID]
                                                        WHERE md.[vTankCode] = @1 AND md.[vItemCode] = @2 ORDER BY md.[iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getMixedSlurryMobileTankInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getMixedSlurryMobileTankInfo(ByVal tankNo As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("     SELECT TOP 1 m.[vLotNumber], md.[dDryWeight], ISNULL(md.[bReceived], 0), ISNULL(md.[bManualclose], 0)  FROM [tbl_RTIS_MS_Decant] md 
                                                        INNER JOIN [tbl_RTIS_MS_Main] m ON m.[iLineID] = md.[iHeaderID]
                                                        WHERE md.[vTankCode] = @1 AND md.[vItemCode] = @2 ORDER BY md.[iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*The slurry you have scanned does not exist in CATscan"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getMixedSlurryMobileTankInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_checkTicketRef(ByVal stockTakeNumber As String, ByVal ticketNo As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("     SELECT t.[vTicketNo] FROM [RTIS_InvCountLines_Tickets] t 
                                                        INNER JOIN [RTIS_InvCountLines] il ON il.[idInvCountLines] = t.[iHeaderID]
                                                        INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                        WHERE i.[cInvCountNo] = @1 AND t.[vTicketNo] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockTakeNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "-1*This ticket has already been used"
                    Else
                        Return "1*ticket has not been used yet"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getMixedSlurryMobileTankInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_getPGMContainerInfo(ByVal contNo As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("     SELECT TOP 1 ph.[vLotDesc], pl.[dWeightIn], ISNULL(pt.[bReceived], 0)  FROM [ltbl_RTIS_PGM_Manuf] pl --
                                                        INNER JOIN [htbl_RTIS_PGM_Manuf] ph ON pl.[iHeaderID] = ph.[iLineID]
                                                        LEFT JOIN [ltbl_RTIS_PGM_Trans] pt ON pt.[iHeaderID] = ph.[iLineID] 
                                                        WHERE pl.[vContainer] = @1 AND ph.[vItemCode] = @2
                                                        ORDER BY pl.[iLineID] DESC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", contNo))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*New container barcode scanned, no manufacture has been recorded to it."
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_getPGMContainerInfo: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetAllPalletBarcodes(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
                                                      INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
                                                      WHERE h.[vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += sqlReader.Item(0) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No items were found on this pallet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No items were found on this pallet"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetAllPalletBarcodes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAllPalletBarcodes_RM(ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("   SELECT l.[vUnqBarcode] FROM [ltbl_PalletBarcodes] l
                                                      INNER JOIN [htbl_PalletBarcodes] h ON h.[iLine_ID] = l.[iPallet_ID]
                                                      WHERE h.[vUnqBarcode] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", unq))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData += sqlReader.Item(0) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No items were found on this pallet"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No items were found on this pallet"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetAllPalletBarcodes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Insert
            Public Shared Function UI_ImportEvoStockTakeHeader(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [RTIS_InvCount] SELECT i.[idInvCount],i.[cInvCountNo],i.[cDescription],i.[dPrepared]
                                                    ,i.[cWarehouses],i.[cBinLocations],'1',i.[_btblInvCount_iBranchID],i.[_btblInvCount_dCreatedDate] 
                                                    FROM [" + My.Settings.EvoDB + "].[dbo].[_btblInvCount] i WHERE [cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeHeader: " + ex.ToString())
                    Return "-1*Cannot import stock take header: " + ex.Message
                End Try
            End Function
            Public Shared Function UI_ImportEvoStockTakeLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_InvCountLines] 
                                               SELECT il.[iInvCountID], il.[cBarcode], 0 AS [fCountQty], 0 AS [fCountQty2], il.[fSystemQty], il.[bSerialItem], il.[tSerialList]
                                               ,il.[bLotItem], il.[iLotTrackingID], il.[iBinLocationId], il.[iStockID], il.[iWarehouseID], il.[_btblInvCountLines_dCreatedDate] , 0 AS [Counted],  1 AS [bOnST]
                                               FROM [" + My.Settings.EvoDB + "].[dbo].[_btblInvCountLines] il INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_btblInvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                               WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function MBL_InsertTicketLog(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal userName As String, ByVal barcodeType As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_InvCountLines_Tickets] ([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
                                                   VALUES (@1, @2, @3, @4, GETDATE(), @5, @6, 1, 0)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@5", barcodeType))
                    sqlComm.Parameters.Add(New SqlParameter("@6", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_InsertTicketLog: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function MBL_InsertTicketLog2(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal userName As String, ByVal barcodeType As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_InvCountLines_Tickets] ([iHeaderID], [vTicketNo], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
                                                   VALUES (@1, @2, @3, @4, GETDATE(), @5, @6, 1, 0)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@5", barcodeType))
                    sqlComm.Parameters.Add(New SqlParameter("@6", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_InsertTicketLog: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function MBL_InsertTicketLogBoth(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal userName As String, ByVal barcodeType As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  INSERT INTO [RTIS_InvCountLines_Tickets] 
                                                    ([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
                                                    vALUES
                                                    (@1, @2, @3, @4, GETDATE(), @3, @4, GETDATE(), @5, @6, 1, 0)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@5", barcodeType))
                    sqlComm.Parameters.Add(New SqlParameter("@6", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_InsertTicketLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_InsertTicketLogRecount(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal userName As String, ByVal barcodeType As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  INSERT INTO [RTIS_InvCountLines_Tickets] 
                                                    ([iHeaderID], [vTicketNo], [dCountQty], [vCountUser], [dtDateCounted], [dCountQty2], [vCountUser2], [dtDateCounted2] ,[vBarcodeType],[vUnqBarcode],[bCountValid] ,[bRecountTicket])
                                                    vALUES
                                                    (@1, @2, @3, @4, GETDATE(), @3, @4, GETDATE(), @5, @6, 1, 1)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@5", barcodeType))
                    sqlComm.Parameters.Add(New SqlParameter("@6", unq))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_InsertTicketLog: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function UI_InsertExportFormat(ByVal name As String, ByVal format As String, ByVal delimiter As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_ExportFormats] ([vFormatName], [vFormatString], [vDelimeter]) VALUES (@1, @2, @3)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlComm.Parameters.Add(New SqlParameter("@2", format))
                    sqlComm.Parameters.Add(New SqlParameter("@3", delimiter))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_InsertExportFormat: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ArchiveRTStockTakeTicketLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [RTIS_InvCountArchiveLines_Tickets] 
                                                    SELECT [iHeaderID] ,[vTicketNo],[dCountQty],[vCountUser],[dtDateCounted],[dCountQty2],[vCountUser2],[dtDateCounted2] 
                                                    ,[vBarcodeType],[vUnqBarcode],[bCountValid],[bRecountTicket]
                                                    FROM [RTIS_InvCountLines_Tickets] it
                                                    INNER JOIN [RTIS_InvCountLines] il ON il.[idInvCountLines] = it.[iHeaderID]
                                                    INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
                                                    WHERE ih.[cInvCountNo] = @1 ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ArchiveRTStockTakeValidationLines: " + ex.ToString())
                    Return "-1*Cannot archive stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function UI_ArchiveRTStockTakeLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [RTIS_InvCountArchiveLines] 
                                                SELECT il.[idInvCountLines],il.[iInvCountID], il.[cBarcode], il.[fCountQty],  il.[fCountQty2], il.[fSystemQty], il.[bSerialItem], il.[tSerialList]
                                                ,il.[bLotItem], il.[iLotTrackingID], il.[iBinLocationId], il.[iStockID], il.[iWarehouseID], il.[_btblInvCountLines_dCreatedDate], il.[bOnST] 
                                                FROM [RTIS_InvCountLines] il INNER JOIN [RTIS_InvCount] i ON il.[iInvCountID] = i.[idInvCount]
                                                WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ArchiveRTStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_AddItemForInvestigation(ByVal stockTake As String, ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [RTIS_InvCountLines_Enquiry] ([vStNumber], [vItemCode], [vLotDescription], [dQty1], [vUserAdded], [dtAdded])
                                                    VALUES (@1, @2, @3, @4, @5, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockTake))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@5", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_AddItemForInvestigation: " + ex.ToString())
                    Return "-1*Cannot add item for investigation: " + ex.Message
                End Try
            End Function

            Public Shared Function MBL_AddItemToST(ByVal invCountID As String, ByVal barcode As String, ByVal blotItem As String, ByVal lotID As String, ByVal stockID As String, ByVal whseID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [RTIS_InvCountLines] ([iInvCountID], [cBarcode], [fCountQty] ,[fCountQty2] ,[fSystemQty] ,[bSerialItem] ,[tSerialList] ,[bLotItem], [iLotTrackingID], [iBinLocationId], [iStockID], [iWarehouseID], [_btblInvCountLines_dCreatedDate], [bIsCounted], [bOnST])
                                                   VALUES                           (@1           , @2        , 0           , 0           , 0           , 0            , NULL         , @3      , @4              , 0               , @5        , @6            , NULL                             ,0, 0)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", invCountID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", barcode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", blotItem))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotID))
                    sqlComm.Parameters.Add(New SqlParameter("@5", stockID))
                    sqlComm.Parameters.Add(New SqlParameter("@6", whseID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_InsertTicketLog: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
        End Class

        Partial Public Class Update
            Public Shared Function MBL_UpdateSTCount_Lot(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal itemCode As String, ByVal lotNumber As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND ([vUnqBarcode] LIKE @5 AND  [vUnqBarcode] LIKE @6 AND [vUnqBarcode] LIKE @7 AND [vUnqBarcode] LIKE @8)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@6", "%" + lotNumber + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@7", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@8", "%" + unq + "%"))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateSTCount2_Lot(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal itemCode As String, ByVal lotNumber As String, ByVal unq As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND ([vUnqBarcode] LIKE @5 AND  [vUnqBarcode] LIKE @6 AND [vUnqBarcode] LIKE @7 AND [vUnqBarcode] LIKE @8)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", "%" + itemCode + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@6", "%" + lotNumber + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@7", "%" + qty + "%"))
                    sqlComm.Parameters.Add(New SqlParameter("@8", "%" + unq + "%"))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function MBL_UpdateSTCount_FreshSlurry(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", trolleyCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateSTCount2_FreshSlurry(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", trolleyCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateTicketSTCount(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty] = @3, [vCountUser] = @4, [dtDateCounted] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", trolleyCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateTicketSTCount2(ByVal headerID As String, ByVal ticketNo As String, ByVal qty As String, ByVal username As String, ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [dCountQty2] = @3, [vCountUser2] = @4, [dtDateCounted2] = GETDATE() 
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2 AND [vUnqBarcode] = @5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlComm.Parameters.Add(New SqlParameter("@5", trolleyCode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ImportEvoStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItem_Lot(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String, ByVal lotNumber As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty] = il.[fCountQty] + @5 FROM [RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_etblLotTracking] l
                                               ON l.[idLotTracking] = il.[iLotTrackingID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2_Lot: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItem2_Lot(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String, ByVal lotNumber As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty2] = il.[fCountQty2] + @5 FROM [RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_etblLotTracking] l
                                               ON l.[idLotTracking] = il.[iLotTrackingID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2_Lot: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItemBoth_Lot(ByVal stNum As String, ByVal itemCode As String, ByVal whseCode As String, ByVal lotNumber As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty] = il.[fCountQty] + @5 , il.[fCountQty2] = il.[fCountQty2] + @5 FROM [RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[_etblLotTracking] l
                                               ON l.[idLotTracking] = il.[iLotTrackingID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3 AND l.[cLotDescription] = @4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNum))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2_Lot: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItem(ByVal stNumber As String, ByVal itemcode As String, ByVal whseCode As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty] = il.[fCountQty] + @4, [bIsCounted] = 1 FROM  il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItem2(ByVal stNumber As String, ByVal itemcode As String, ByVal whseCode As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty2] = il.[fCountQty2] + @4, [bIsCounted] = 1 FROM [RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeItemBoth(ByVal stNumber As String, ByVal itemcode As String, ByVal whseCode As String, ByVal qty As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE il SET il.[fCountQty] = il.[fCountQty] + @4, il.[fCountQty2] = il.[fCountQty2] + @4, [bIsCounted] = 1 FROM [RTIS_InvCountLines] il
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[StkItem] s 
                                               ON s.[StockLink] = il.[iStockID]
                                               INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w
                                               ON il.[iWarehouseID] = w.[WhseLink]
                                               INNER JOIN [RTIS_InvCount] i 
                                               ON i.[idInvCount] = il.[iInvCountID]
                                               WHERE i.[cInvCountNo] = @1 AND s.[Code] = @2 AND w.[Code] = @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeItem2: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeReverseTicket(ByVal lineID As String, ByVal qty1 As String, ByVal qty2 As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_InvCountLines] SET [fCountQty] = @2, [fCountQty2] = @3 WHERE [idInvCountLines] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty1.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty2.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeReverseTicket: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function MBL_UpdateRTStockTakeReverseTicketRT2D(ByVal lineID As String, ByVal qty1 As String, ByVal qty2 As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [RTIS_InvCountLines] SET [fCountQty] = [fCountQty] - @2, [fCountQty2] = [fCountQty2] - @3 WHERE [idInvCountLines] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", qty1.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty2.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "MBL_UpdateRTStockTakeReverseTicket: " + ex.ToString())
                    Return "0"
                End Try
            End Function
            Public Shared Function UI_InvalidateSTTicket(ByVal headerID As String, ByVal ticketNo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCountLines_Tickets] SET [bCountValid] = 0
                                                    WHERE [iHeaderID] = @1 AND [vTicketNo] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", ticketNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_InvalidateSTTicket: " + ex.ToString())
                    Return "-1*Cannot import stock take lines: " + ex.Message
                End Try
            End Function
            Public Shared Function UI_UpdateExportFormat(ByVal name As String, ByVal format As String, ByVal delimiter As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [tbl_ExportFormats] SET [vFormatString] = @2, [vDelimeter] = @3 WHERE [vFormatName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlComm.Parameters.Add(New SqlParameter("@2", format))
                    sqlComm.Parameters.Add(New SqlParameter("@3", delimiter))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_UpdateExportFormat: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ArchiveStockTakeHeader(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" UPDATE [RTIS_InvCount] SET [iStatus] = 0 WHERE [cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_ArchiveStockTakeHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Delete
            Public Shared Function UI_RemoveExportLayout(ByVal name) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_ExportFormats] WHERE [vFormatName] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemoveExportLayout: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_RemovePostArchiveTicketLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  DELETE it FROM [RTIS_InvCountLines_Tickets] it
                                                     INNER JOIN [RTIS_InvCountLines] il ON it.[iHeaderID] = il.[idInvCountLines]
                                                     INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
                                                     WHERE ih.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemovePostArchiveValidationLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_RemovePostArchiveLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" DELETE il FROM [RTIS_InvCountLines] il INNER JOIN [RTIS_InvCount] i
                                                ON il.[iInvCountID] = i.[idInvCount] WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemovePostArchiveLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_RemoveArchiveTicketLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  DELETE it FROM [RTIS_InvCountArchiveLines_Tickets] it
                                                     INNER JOIN [RTIS_InvCountArchiveLines] il ON it.[iHeaderID] = il.[idInvCountLines]
                                                     INNER JOIN [RTIS_InvCount] ih ON ih.[idInvCount] = il.[iInvCountID]
                                                     WHERE ih.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemoveArchiveTicketLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_RemoveArchiveRTStockTakeLines(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" DELETE il FROM [RTIS_InvCountArchiveLines] il INNER JOIN [RTIS_InvCount] i
                                                ON il.[iInvCountID] = i.[idInvCount] WHERE i.[cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemoveArchiveRTStockTakeLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_RemoveArchiveRTStockTakeHead(ByVal stNo) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("DELETE FROM [RTIS_InvCount] WHERE [cInvCountNo] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stNo))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UI_RemoveArchiveRTStockTakeHead: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Public Shared Function ExecuteQuery(ByVal query As String) As String
            Try
                Dim ReturnData As String = ""
                Dim sqlConn As New SqlConnection(RTString)
                Dim sqlComm As New SqlCommand(query, sqlConn)
                sqlConn.Open()
                sqlComm.ExecuteNonQuery()
                sqlComm.Dispose()
                sqlConn.Close()
                Return "1*SUCCESS"
            Catch ex As Exception
                EventLog.WriteEntry("RTIS SVC", "ExecuteQuery: " + ex.ToString())
                Return "-1*Cannot execute: " + ex.Message
            End Try
        End Function


    End Class
    Public Class Evolution
        Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
           "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
        Partial Public Class Retreive
            Public Shared Function UI_GetEvoStockTakes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [cInvCountNo],[cDescription]
                                                    FROM [_btblInvCount]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + " - " + Convert.ToString(sqlReader.Item(1)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Stock Takes were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Stock Takes were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetEvoStockTakes: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetEvoWhseID(ByVal whseCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [WhseLink]
                                                    FROM [WhseMst] WHERE [Code] = @1", sqlConn)
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
                        Return "1*" + ReturnData
                    Else
                        Return "0*No code was foun for this warehouse"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No code was foun for this warehouse"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetEvoWhseID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetLotTrackingID(ByVal lotDesc As String, ByVal stockID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [idLotTracking]
                                                    FROM [_etblLotTracking] WHERE [cLotDescription] = @1 AND [iStockID] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@2", stockID))
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
                        Return "0*lot number was not found in evolution : " + Environment.NewLine + lotDesc
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*lot number was not found in evolution : " + Environment.NewLine + lotDesc
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetLotTrackingID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetItemInfoForST(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT [StockLink],[Bar_Code]
                                                    FROM [StkItem] WHERE [Code] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
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

            Public Shared Function MBL_GetLotInEvo(ByVal lotDesc As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELECT l.[idLotTracking] 
                                                    FROM [_etblLotTracking] l INNER JOIN [StkItem] s ON l.[iStockID] = s.[StockLink]
                                                    WHERE l.[cLotDescription] = @1 AND s.[Code] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "0*lot number was not found in evolution : " + Environment.NewLine + lotDesc
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*lot number was not found in evolution : " + Environment.NewLine + lotDesc
                    Else
                        EventLog.WriteEntry("RTIS SVC", "MBL_GetLotTrackingID: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetLotIds(ByVal lotNumbers As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand($" SELECT l.[idLotTracking] FROM [_etblLotTracking] l
                                                    INNER JOIN [StkItem] s ON s.[StockLink] = l.[iStockID]
                                                    WHERE l.[cLotDescription] IN {lotNumbers} AND s.[Code] = @ITEM", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@ITEM", itemCode))
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
                        Return "0*No lot numbers were found for this item in evolution"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No lot numbers were found for this item in evolution"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_GetEvoStockTakes: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
        End Class
    End Class
End Class
