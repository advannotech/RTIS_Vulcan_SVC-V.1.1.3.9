Imports System.Data.SqlClient
Imports System.IO
Imports System.Threading
Imports Pastel.Evolution

Public Class POReceiving
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Shared sep As String = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
    Public Class RTSQL
        Partial Public Class Retreive

            'GET SELECTED POs FROM CATscan

            Public Shared Function UI_GetSelectedPOs(ByVal VendorName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vOrderNum] FROM tbl_POLink WHERE vVendorName=@1 ORDER BY [vOrderNum] ASC", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", VendorName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "1*No POs found~"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No POs found~"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetSelectedPOs: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function



            Public Shared Function UI_CheckCMSValue(ByVal value As String, ByVal valType As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CheckCMSValue] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", value))
                    sqlComm.Parameters.Add(New SqlParameter("@2", valType))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*This value was already found in the CMS records"
                    Else
                        Return "1*OK to add"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*OK to add"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOVendor: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSItems() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSItems]", sqlConn)
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
                        Return "0*CMS items were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS items were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCMSItems: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSUOMs() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSUOMs]", sqlConn)
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
                        Return "0*CMS UOMs were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS UOMs were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCMSUOMs: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetItemCMSHeaders() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSHeaders]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) +
                        "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS headers were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS headers were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSHeaders: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSItems_Add() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSItems_Add]", sqlConn)
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
                        Return "0*No CMS items were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No CMS items were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCMSItems: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSUOMs_Add() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSUOMs_Add]", sqlConn)
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
                        Return "0*No CMS items were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No CMS items were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCMSUOMs_Add: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetItemCMSApprovals() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSApprovals]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS headers were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS headers were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovals: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetItemCMSApprovalLines(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSApprovalLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS Lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS Lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovalLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetItemCMSApprovalLinesViww(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSApprovalLinesViww] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS Lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS Lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovalLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetItemCMSApprovalLinesEdit(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSApprovalLinesEdit] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS Lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS Lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovalLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSApprovalImagee(ByVal itemCode As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSApprovalImagee] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()

                    Dim sign As Image
                    Dim imgStream As New MemoryStream(CType(sqlComm.ExecuteScalar, Byte()))
                    Dim retImage As Image = Image.FromStream(imgStream)
                    retImage.Save(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + itemCode + ".png")

                    sqlComm.Dispose()
                    sqlConn.Close()

                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovalLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetCMSHeadersToArchive(ByVal stockLink As String, ByVal docVersion As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSHeadersToArchive] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", stockLink))
                    sqlComm.Parameters.Add(New SqlParameter("@2", docVersion))
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
                        Return "0*CMS headers were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS headers were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCMSHeadersToArchive: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSArchiveHeaders() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSArchiveHeaders]", sqlConn)
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
                        Return "0*CMS headers were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS headers were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSHeaders: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCMSArchiveImage(ByVal lineID As String, ByVal itemCode As String) As String
                Try
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCMSArchiveImage] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlConn.Open()

                    Dim sign As Image
                    Dim imgStream As New MemoryStream(CType(sqlComm.ExecuteScalar, Byte()))
                    Dim retImage As Image = Image.FromStream(imgStream)
                    retImage.Save(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + itemCode + ".png")

                    sqlComm.Dispose()
                    sqlConn.Close()

                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSApprovalLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetItemCMSArchiveLines(ByVal headerID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetItemCMSArchiveLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", headerID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*CMS Lines were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*CMS Lines were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetItemCMSArchiveLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckRTVendor(ByVal vendorID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CheckRTVendor] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", vendorID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Vendors were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Vendors were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetEvoPOVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetVendorPOLinks() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetVendorPOLinks]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Vendors were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Vendors were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetEvoPOVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckVendorPOLink(ByVal vendorID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CheckVendorPOLink] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", vendorID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No References were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No References were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_CheckVendorPOLink: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetLinkedVendors() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetLinkedVendors]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No linked vendors were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No linked vendors were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLinkedVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetLinkedPOs() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetLinkedPOs]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No linked orders were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No linked orders were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetLinkedVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function



            Public Shared Function UI_GetVendorPO(ByVal vendorName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetVendorPO] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", vendorName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PO found for vendor"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No PO found for vendor"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetVendorPO: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPOVendor(ByVal PONumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPOVendor] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", PONumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No PO found for vendor"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No PO found for vendor"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOVendor: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckPOUnqLines(ByVal OrderNum As String)
                Try
                    Dim ReturnData As String = ""
                    Dim ReturnQuery As String = "INSERT INTO [UNQ] ([Barcode], [Receive], [bValidated]) VALUES "
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_CheckPOUnqLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", OrderNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "*" + Convert.ToString(sqlReader.Item(1)) + "*" + Convert.ToString(sqlReader.Item(2)) + "~"

                        ReturnQuery &= "('" + Convert.ToString(sqlReader.Item(0)) + "','" + Convert.ToString(sqlReader.Item(1)) + "','" + Convert.ToString(sqlReader.Item(2)) + "'),"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*" + ReturnQuery
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function UI_AddCMSRecord(ByVal value As String, ByVal valType As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_AddCMSRecord] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", value))
                    sqlComm.Parameters.Add(New SqlParameter("@2", valType))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_AddCMSDocHeader(ByVal id As String, ByVal code As String, ByVal username As String, ByVal version As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" INSERT INTO [COA].[htbl_CMS_Docs] ([iStockID],[vItemCode],[dtDateCreated], [vUserCaptured],[vStatus],[iDocVersion]) OUTPUT INSERTED.iLineID VALUES (@1, @2, GETDATE(), @3, 'Waiting Approval', @4)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", code))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlComm.Parameters.Add(New SqlParameter("@4", version))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No headerID was returned"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddCMSDocHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_AddVendorLookup(ByVal id As String, ByVal name As String, ByVal viewable As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_AddVendorLookup] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", name))
                    sqlComm.Parameters.Add(New SqlParameter("@3", viewable))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_InsertPOLines(ByVal query As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(query, sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertPOLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_AddVendorPOLink(ByVal id As String, ByVal name As String, ByVal orderNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_AddVendorPOLink] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", name))
                    sqlComm.Parameters.Add(New SqlParameter("@3", orderNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


            'LINK PO TO VENDOR
            Public Shared Function UI_LinkPOtoVendor(ByVal id As String, ByVal name As String, ByVal orderNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_LinkPOtoVendor] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", name))
                    sqlComm.Parameters.Add(New SqlParameter("@3", orderNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_RTLogPO(ByVal query As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(query, sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddRTPoRef: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function UI_UpdateCMSApproved(ByVal lineID As String, ByVal image() As Byte, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateCMSApproved] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", image))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateCMSApproved: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateCMSRejected(ByVal lineID As String, ByVal reason As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateCMSRejected] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", reason))
                    sqlComm.Parameters.Add(New SqlParameter("@3", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateCMSRejected: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateCMSEdited(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateCMSEdited] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateCMSEdited: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateVendorLookup(ByVal id As String, ByVal viewable As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateVendorLookup] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", viewable))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateVendorPOLink(ByVal id As String, ByVal name As String, ByVal orderNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateVendorPOLink] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", name))
                    sqlComm.Parameters.Add(New SqlParameter("@3", orderNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddVendorLookup: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ValidateLabels(ByVal validRef As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_ValidateLabels] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", validRef))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ValidateLabels: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_SetUnqReceived(ByVal poRec As String, ByVal barcode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_SetUnqReceived] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", poRec))
                    sqlComm.Parameters.Add(New SqlParameter("@2", barcode))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_ValidateLabels: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Delete
            Public Shared Function UI_CMSItem(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CMSItem] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_CMSItem: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeletePOLines(ByVal orderNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeletePOLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", orderNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeletePOLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeleteCMSDocLines(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteCMSDocLiness] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteCMSDocLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeleteInvalidLabels(ByVal orderNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteInvalidLabels] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", orderNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteInvalidLabels: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeleteCMSHeader(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteCMSHeader] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteCMSHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeleteCMSLines(ByVal id As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteCMSLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeleteCMSLines: " + ex.ToString())
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
        Partial Public Class Retreive

            'GET ACTIVE POs FROM SAGE
            Public Shared Function GetActivePOs(ByVal supplier As String) As String
                Try

                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetActivePOs] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", supplier))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No active po were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No active po were found"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "GetActivePOnumber: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetEvoPOVendors() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetEvoPOVendors]", sqlConn)
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
                        Return "0*No Vendors were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Vendors were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetEvoPOVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function





            Public Shared Function UI_GetVendorPOs(ByVal vendorID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetVendorPOs] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", vendorID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "1*No POs found~"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No POs found~"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetEvoPOVendors: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function



            Public Shared Function UI_GetPOLinesNew(ByVal orderNum As String, ByVal vendorName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPOLinesNew] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", orderNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) +
                         "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "~" '+ Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "~" '+ "|" + Convert.ToString(sqlReader.Item(18)) + "|" + Convert.ToString(sqlReader.Item(19)) + "|" + Convert.ToString(sqlReader.Item(20)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) +
                         "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "~" '+ Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "~" '+ "|" + Convert.ToString(sqlReader.Item(18)) + "|" + Convert.ToString(sqlReader.Item(19)) + "|" + Convert.ToString(sqlReader.Item(20)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + orderNum + "*" + vendorName + "*" + ReturnData
                    Else
                        Return "0*No Lines found for purchase order"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for purchase order"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_ReprintPOLinesNew(ByVal orderNum As String, ByVal vendorName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_ReprintPOLinesNew] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", orderNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) +
                         "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "~" '+ Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "~" '+ "|" + Convert.ToString(sqlReader.Item(18)) + "|" + Convert.ToString(sqlReader.Item(19)) + "|" + Convert.ToString(sqlReader.Item(20)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) +
                         "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "~" '+ Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "~" '+ "|" + Convert.ToString(sqlReader.Item(18)) + "|" + Convert.ToString(sqlReader.Item(19)) + "|" + Convert.ToString(sqlReader.Item(20)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + orderNum + "*" + vendorName + "*" + ReturnData
                    Else
                        Return "0*No Lines found for purchase order"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for purchase order"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPOLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetStockLabelInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetStockLabelInfo] @1", sqlConn)
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
            Public Shared Function MBL_CheckPOExists(ByVal OrderNum As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_CheckPOExists] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", OrderNum))
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
                        Return "0*PO not found!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetEvoPOLines(ByVal OrderNum As String)
                Try
                    Dim ReturnData As String = ""
                    Dim returnQuery As String = "INSERT INTO [PO] ([ItemCode], [Description], [Description2], [bLotItem], [LotNumber], [OrderQty], [RecQty], [ProcQty], [ViewableQty], [PrintQty], [UserScanned], [DTUserScanned]) VALUES "
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetEvoPOLines] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", OrderNum))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) +
                        "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"

                        returnQuery &= "('" + Convert.ToString(sqlReader.Item(0)) + "','" + Convert.ToString(sqlReader.Item(1)) + "','" + Convert.ToString(sqlReader.Item(2)) + "','" + Convert.ToString(sqlReader.Item(3)) + "','" + Convert.ToString(sqlReader.Item(4)) +
                         "','" + Convert.ToString(sqlReader.Item(5)).Replace(",", ".") + "','" + Convert.ToString(sqlReader.Item(6)).Replace(",", ".") + "','" + Convert.ToString(sqlReader.Item(7)).Replace(",", ".") + "','" + Convert.ToString(sqlReader.Item(8)).Replace(",", ".") + "','" + Convert.ToString(sqlReader.Item(9)).Replace(",", ".") +
                         "','', '' ),"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    'Return "1*" + ReturnData
                    Return "1*" + returnQuery
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOExists: " + ex.ToString())
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
        Public Shared Function CGetWhse(ByVal WHSEid As String) As Warehouse
            Try
                Dim thisitem As Warehouse = New Warehouse(Warehouse.Find("WhseLink='" & WHSEid & "'"))
                Return thisitem
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "CGetItem: " + ex.ToString)
                Return Nothing
            End Try
        End Function

        'Public Shared Sub UI_CreateCr()
        '    Try
        '        Initialize()


        '        Dim cr As CreditNote = New CreditNote()
        '        cr.AccountID = 20 '7726
        '        cr.ExternalOrderNo = "CRTRY"
        '        cr.InvoiceNumber = "IC100315"
        '        'Dim acc As DrCrAccount = DrCrAccount()
        '        Dim crl As OrderDetail = New OrderDetail()
        '        Dim invID As Integer = InventoryItem.Find("Code='18461-0C080'")
        '        crl.InventoryItem = New InventoryItem(invID)
        '        crl.UnitCostPrice = crl.InventoryItem.UnitCost
        '        crl.UnitSellingPrice = crl.InventoryItem.SellingPrice1
        '        Dim whseID As Integer = Warehouse.Find("Code='IT-FGCTOY'")
        '        crl.Warehouse = New Warehouse(whseID)
        '        crl.Quantity = 100
        '        crl.ToProcess = 50

        '        Dim crl2 As OrderDetail = New OrderDetail()
        '        Dim invID2 As Integer = InventoryItem.Find("Code='18450-0C100'")
        '        crl2.InventoryItem = New InventoryItem(invID2)
        '        crl2.UnitCostPrice = crl.InventoryItem.UnitCost
        '        crl2.UnitSellingPrice = crl.InventoryItem.SellingPrice1
        '        Dim whseID2 As Integer = Warehouse.Find("Code='IT-FGCTOY'")
        '        crl2.Warehouse = New Warehouse(whseID2)
        '        crl2.Quantity = 200
        '        crl2.ToProcess = 150


        '        cr.Detail.Add(crl)
        '        cr.Detail.Add(crl2)
        '        cr.Save()
        '    Catch ex As Exception

        '    End Try


        'End Sub

        Public Shared Function UI_ProccessPOLines(ByVal PONum As String, ByVal POInfo As String)
            Try
                Initialize()
                Dim POLines As String() = POInfo.Split("~")
                Dim poId As String = 109784
                Dim OrderID As Integer = PurchaseOrder.Find("OrderNum = '" & PONum & "' AND (DocState=1 OR DocState=3) AND DocFlag = 1")
                Dim Order As New PurchaseOrder(OrderID)
                For Each poLine As String In POLines
                    If poLine <> String.Empty Then
                        Dim itemCode As String = poLine.Split("|")(0)
                        Dim lotDesc As String = poLine.Split("|")(1)
                        Dim orderQty As String = poLine.Split("|")(2)
                        If lotDesc <> String.Empty Then
                            Dim lotFound As Boolean = False
                            Dim oriDetail As OrderDetail
                            Dim lotDetail As OrderDetail
                            For Each d As OrderDetail In Order.Detail
                                If IsNothing(d.Lot) = False Then
                                    If d.InventoryItem.Code = itemCode And d.Lot.Code = lotDesc Then
                                        lotFound = True
                                        lotDetail = d
                                        'ElseIf d.InventoryItem.Code And d.Lot.Code = String.Empty Then
                                        '    oriDetail = d
                                    End If
                                Else
                                    If d.InventoryItem.Code = itemCode Then
                                        oriDetail = d
                                    End If
                                End If

                            Next

                            If lotFound = True Then
                                'oriDetail.Quantity = oriDetail.Quantity - orderQty
                                lotDetail.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep)) 'lotDetail.Quantity ' + orderQty
                                'oriDetail.Save()
                                'lotDetail.Save()
                                ' Order.Save()
                            Else
                                Dim newd As New OrderDetail '= d
                                newd.InventoryItem = oriDetail.InventoryItem
                                newd.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep))
                                newd.ToProcess = 0
                                newd.TaxMode = oriDetail.TaxMode
                                newd.Warehouse = CGetWhse(oriDetail.WarehouseID)
                                newd.PriceListNameID = oriDetail.PriceListNameID
                                newd.UnitCostPrice = oriDetail.UnitCostPrice
                                newd.UnitSellingPrice = oriDetail.UnitSellingPrice
                                newd.Project = oriDetail.Project
                                'newd.Warehouse = oriDetail.Warehouse

                                Dim newLot As Lot = New Lot()
                                newLot.Code = lotDesc
                                newLot.Branch = oriDetail.InventoryItem.Branch
                                newLot.BranchID = oriDetail.InventoryItem.BranchID
                                newLot.InventoryItem = oriDetail.InventoryItem
                                newd.Lot = newLot

                                Order.Detail.Add(newd)
                                'Order.Save()
                            End If
                        Else
                            For Each d As OrderDetail In Order.Detail
                                If d.InventoryItem.Code = itemCode And IsNothing(d.Lot) Then
                                    d.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep))
                                    'd.Save()
                                    'Order.Save()
                                End If
                            Next
                        End If
                    End If
                Next
                Order.Save()
                Return "1*Success"

#Region "Old"
                'Dim stockItem As InventoryItem = CGetItem(itemCode)
                'If stockItem.Code <> "ErrorItem" Then
                '    If lotDesc <> String.Empty Then
                '        'Items with lots
                '        Dim lot As Lot = CGetLot(lotDesc, stockItem.ID)
                '        If lot.Code <> "ErrorLot" Then

                '        Else
                '            Dim newLot As Lot = New Lot()
                '            newLot.Code = lotDesc
                '            newLot.Branch = stockItem.Branch
                '            newLot.BranchID = stockItem.BranchID
                '            newLot.InventoryItem = stockItem
                '            newLot.Save()
                '        End If
                '    Else
                '        'Items without lots
                '    End If
                'Else

                'End If

                'For Each d As OrderDetail In Order.Detail
                '    If d.LotID = 0 Then
                '        If d.InventoryItem.Code = "CHEM-1240" Then
                '            Dim newd As New OrderDetail '= d
                '            newd.InventoryItem = d.InventoryItem
                '            newd.Quantity = d.Quantity
                '            newd.ToProcess = 0
                '            newd.TaxMode = d.TaxMode
                '            newd.Warehouse = CGetWhse(1)
                '            newd.PriceListNameID = d.PriceListNameID
                '            newd.UnitCostPrice = d.UnitCostPrice
                '            newd.UnitSellingPrice = d.UnitSellingPrice

                '            Dim newLot As Lot = New Lot()
                '            newLot.Code = "Patrick"
                '            newLot.Branch = d.InventoryItem.Branch
                '            newLot.BranchID = d.InventoryItem.BranchID
                '            newLot.InventoryItem = d.InventoryItem
                '            newd.Lot = newLot
                '            Order.Detail.Add(newd)
                '            Order.Save()
                '        End If
                '    End If
                'Next
#End Region
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function

        Public Shared Function UI_ProccessPOLinesNew(ByVal PONum As String, ByVal POLines As String())
            Try
                Initialize()
                Dim poId As String = 109784
                Dim OrderID As Integer = PurchaseOrder.Find("OrderNum = '" & PONum & "' AND (DocState=1 OR DocState=3) AND DocFlag = 1")
                Dim Order As New PurchaseOrder(OrderID)
                For Each poLine As String In POLines
                    If poLine <> String.Empty Then
                        Dim itemCode As String = poLine.Split("|")(1)
                        Dim lotDesc As String = poLine.Split("|")(2)
                        Dim orderQty As String = poLine.Split("|")(3)
                        If lotDesc <> String.Empty Then
                            Dim lotFound As Boolean = False
                            Dim oriDetail As OrderDetail
                            Dim lotDetail As OrderDetail
                            For Each d As OrderDetail In Order.Detail
                                If IsNothing(d.Lot) = False Then
                                    If d.InventoryItem.Code = itemCode And d.Lot.Code = lotDesc Then
                                        lotFound = True
                                        lotDetail = d
                                        'ElseIf d.InventoryItem.Code And d.Lot.Code = String.Empty Then
                                        '    oriDetail = d
                                    End If
                                Else
                                    If d.InventoryItem.Code = itemCode Then
                                        oriDetail = d
                                    End If
                                End If

                            Next

                            If lotFound = True Then
                                'oriDetail.Quantity = oriDetail.Quantity - orderQty
                                lotDetail.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep)) 'lotDetail.Quantity ' + orderQty
                                'oriDetail.Save()
                                'lotDetail.Save()
                                ' Order.Save()
                            Else
                                Dim newd As New OrderDetail '= d
                                newd.InventoryItem = oriDetail.InventoryItem
                                newd.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep))
                                newd.ToProcess = 0
                                newd.TaxMode = oriDetail.TaxMode
                                newd.Warehouse = CGetWhse(oriDetail.WarehouseID)
                                newd.PriceListNameID = oriDetail.PriceListNameID
                                newd.UnitCostPrice = oriDetail.UnitCostPrice
                                newd.UnitSellingPrice = oriDetail.UnitSellingPrice
                                newd.Project = oriDetail.Project
                                'newd.Warehouse = oriDetail.Warehouse

                                Dim newLot As Lot = New Lot()
                                newLot.Code = lotDesc
                                newLot.Branch = oriDetail.InventoryItem.Branch
                                newLot.BranchID = oriDetail.InventoryItem.BranchID
                                newLot.InventoryItem = oriDetail.InventoryItem
                                newd.Lot = newLot

                                Order.Detail.Add(newd)
                                'Order.Save()
                            End If
                        Else
                            For Each d As OrderDetail In Order.Detail
                                If d.InventoryItem.Code = itemCode And IsNothing(d.Lot) Then
                                    d.Quantity = Convert.ToDouble(orderQty.Replace(".", sep).Replace(",", sep))
                                    'd.Save()
                                    'Order.Save()
                                End If
                            Next
                        End If
                    End If
                Next
                Order.Save()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
        Public Shared Function MBL_UpdatePOItemRec(ByVal ordeNum As String, ByVal itemCode As String, ByVal lotNum As String, ByVal toPoccess As String)
            Try
                Initialize()
                Dim OrderID As Integer = PurchaseOrder.Find("OrderNum = '" & ordeNum & "' AND (DocState=1 OR DocState=3) AND DocFlag = 1")
                Dim Order As New PurchaseOrder(OrderID)
                If lotNum <> String.Empty Then
                    For Each d As OrderDetail In Order.Detail
                        If IsNothing(d.Lot) = False Then
                            If d.InventoryItem.Code = itemCode And d.Lot.Code = lotNum Then
                                Dim dToProc As Double = Convert.ToDouble(toPoccess.Replace(",", sep).Replace(".", sep))
                                d.ToProcess = d.ToProcess + dToProc
                            End If
                        End If
                    Next
                Else
                    For Each d As OrderDetail In Order.Detail
                        If IsNothing(d.Lot) = True Then
                            If d.InventoryItem.Code = itemCode Then
                                Dim dToProc As Double = Convert.ToDouble(toPoccess.Replace(",", sep).Replace(".", sep))
                                d.ToProcess = d.ToProcess + dToProc
                            End If
                        End If
                    Next
                End If
                Order.Save()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
    End Class
End Class
