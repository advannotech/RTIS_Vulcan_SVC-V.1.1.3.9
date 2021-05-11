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
            Public Shared Function UI_CheckCMSValue(ByVal value As String, ByVal valType As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [vValType] FROM [COA].[tbl_CMS_Admin] WHERE [vValue] = @1 AND [vValType] = @2", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'Item'", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        'Dim sendValueList As New List(Of Integer)
                        'Dim cmsItem As String = Convert.ToString(sqlReader.Item(1)) 
                        'For Each c As Char In cmsItem
                        '    sendValueList.Add(Convert.toInt32(c)) 
                        'Next
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
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'UOM'", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT s.[Code], s.[Description_1], CASE WHEN hc.[vStatus] IS NULL THEN 'NO' ELSE 'YES' END, ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[vUserCaptured], hc.[dtDateCreated], hc.[vUserApproved], hc.[dtDateApproved], ISNULL(hc.[iLineID], 0), hc.[iDocVersion],[vUserRejected],[dtRejected],[vReasons] FROM [Cataler_SCN].[dbo].[StkItem] s
                                                            LEFT JOIN [COA].[htbl_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
                                                            WHERE s.[ItemGroup] = '006' ", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10))+ 
                        "|" + Convert.ToString(sqlReader.Item(11))   + "|" + Convert.ToString(sqlReader.Item(12))   + "|" + Convert.ToString(sqlReader.Item(13)) + "~"
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
                    Dim sqlComm As New SqlCommand("SELECT [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'Item'", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [vValue] FROM [COA].[tbl_CMS_Admin] WHERE [vValType] = 'UOM'", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT s.[Code], s.[Description_1], ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[iLineID], hc.[iDocVersion]  FROM [Cataler_SCN].[dbo].[StkItem] s
                                                            INNER JOIN [COA].[htbl_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
                                                            WHERE s.[ItemGroup] = '006' AND hc.[vStatus] <> 'Approved' AND hc.[vStatus] <> 'Rejected'", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
                                                            FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
                                                            FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
                                                            FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [imApprovalSignature]
                                                            FROM [COA].[htbl_CMS_Docs] WHERE [vItemCode] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [iLineID] FROM [COA].[htbl_CMS_Docs] WHERE [iStockID] = @1 AND [iDocVersion] < @2", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT s.[Code], s.[Description_1], CASE WHEN hc.[vStatus] IS NULL THEN 'NO' ELSE 'YES' END, ISNULL(hc.[vStatus], 'Waiting CMS'), s.[StockLink], hc.[vUserCaptured], hc.[dtDateCreated], hc.[vUserApproved], hc.[dtDateApproved], ISNULL(hc.[iLineID], 0), hc.[iDocVersion] FROM [Cataler_SCN].[dbo].[StkItem] s
                                                            RIGHT JOIN [COA].[htbl_Archive_CMS_Docs] hc ON s.[StockLink] = hc.[iStockID]
                                                            WHERE s.[ItemGroup] = '006' ", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [imApprovalSignature]
                                                            FROM [COA].[htbl_Archive_CMS_Docs] WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [vItem] ,[vUnit] ,[vOperator] ,[dValue1] ,[dValue2] ,[vInspection]
                                                            FROM [COA].[ltbl_Archive_CMS_Docs] WHERE [iHeaderID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELEcT [iVendorID] FROM [rtblEvoVendors]
                                                        WHERE [iVendorID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT ven.[iVendorID], ven.[vVendorName], ISNULL([vOrderNum], '- Select Order -') AS [vOrderNum], [dtDateUpdated], '' AS [POs]
                                                    FROM [tbl_POLink] link 
                                                    RIGHT JOIN [rtblEvoVendors] ven ON ven.[iVendorID] = link.[iVendorID]
                                                    WHERE ven.[vVendorName] <> '' AND ven.[bSelected] = 1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELEcT [iVendorID] FROM [tbl_POLink]
                                                        WHERE [iVendorID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vVendorName] FROM [tbl_POLink]", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT DISTINCT [vOrderNum] FROM [tbl_POLink]", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT [vOrderNum] FROM [tbl_POLink]
                                                    WHERE [vVendorName] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT [vVendorName] FROM [tbl_POLink]
                                                    WHERE [vOrderNum] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT [vUnqBarcode], [Receive], [bValidated] FROM [tbl_unqBarcodes]
                                                    WHERE [ValidateRef] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" INSERT INTO [COA].[tbl_CMS_Admin] ([vValue],[vValType]) VALUES (@1, @2)", sqlConn)
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
                    Dim sqlComm As New SqlCommand("INSERT INTO [rtblEvoVendors] ([iVendorID]
                                                                                ,[vVendorName]
                                                                                ,[bSelected]
                                                                                )
                                                   VALUES (@1, @2, @3)", sqlConn)
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
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_POLink] ([iVendorID]
                                                                                ,[vVendorName]
                                                                                ,[vOrderNum]
                                                                                ,[dtDateUpdated]
                                                                                )
                                                   VALUES (@1, @2, @3, GETDATE())", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [COA].[htbl_CMS_Docs] SET [imApprovalSignature] = @2, [dtDateApproved] = GETDATE(), [vUserApproved] = @3, [vStatus] = 'Approved'  WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [COA].[htbl_CMS_Docs] SET [vReasons] = @2, [dtRejected] = GETDATE(), [vUserRejected] = @3, [vStatus] = 'Rejected'  WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" UPDATE [COA].[htbl_CMS_Docs] SET [vStatus] = 'Waiting Approval', [vReasons] = NULL, [dtRejected] = NULL,  [vUserRejected] = NULL
                                                    WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [rtblEvoVendors] SET [bSelected]= @2 WHERE [iVendorID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_POLink] SET [vVendorName] = @2, [vOrderNum] = @3, [dtDateUpdated] = GETDATE() WHERE [iVendorID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_unqBarcodes] SET [bValidated] = 1 WHERE [ValidateRef] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_unqBarcodes] SET [Receive] = @1 WHERE [vUnqBarcode] = @2", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [COA].[tbl_CMS_Admin] WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [tblPOLines] WHERE [vOrderNum] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" DELETE FROM [COA].[ltbl_CMS_Docs]
                                                    WHERE [iHeaderID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_unqBarcodes] WHERE [ValidateRef] = @1 AND [bValidated] = 0", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [COA].[htbl_CMS_Docs] WHERE [iLineID] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = @1", sqlConn)
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
            Public Shared Function UI_GetEvoPOVendors() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand(" SELEcT DISTINCT v.[DCLink], v.[Name], ISNULL(rv.[bSelected], 'false') AS [Selected] FROM [InvNum] i 
                                                    INNER JOIN [Vendor] v ON v.[DCLink] = i.[AccountID]
                                                    LEFT JOIN [" + My.Settings.RTDB + "].[dbo].[rtblEvoVendors] rv ON rv.[iVendorID] = v.[DCLink] ORDER BY v.[Name] ASC", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT [OrderNum] FROM [InvNum]
                                                    WHERE [AccountID] = @1 AND [DocType] = 5 AND ([DocState] = 1 OR [DocState] = 3)", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], ''), il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable]	                                                                                                   
                                                    ,ISNULL(rtp.[dPrintQty],0) AS [dPrintQty]                                                    
                                                    ,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
                                                    ,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
	                                                ,'0' AS [Back1]
	                                                ,'0' AS [Back2]
	                                                ,'' AS [Back3]
                                                    FROM [InvNum] i
                                                    INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
                                                    INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
                                                    LEFT JOIN [" + My.Settings.RTDB + "].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vLotNumber] COLLATE Latin1_General_CI_AS = il.[cLotNumber] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
                                                    WHERE i.[OrderNum] = @1 AND i.[DocType] = '5' And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 1 AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @1 ORDER BY [DocVersion] DESC)       
                                                    UNION
                                                    SELECT s.[Code], s.[Description_1], s.[Description_2], ISNULL(il.[cLotNumber], '') , il.[fQuantity] AS [OrderQty], il.[fQtyToProcess], 'false' AS [Receive],  '' AS [Print], s.[bLotItem] AS [LotLine], 'true' AS [Viewable] 	                                                
                                                    ,ISNULL(rtp.[dPrintQty], 0) AS [dPrintQty]                                                    
                                                    ,ISNULL(rtp.[bValidated], 'True') AS [bValidated]
                                                    ,ISNULL(rtp.[bScanned], 'False') AS [bScanned]
	                                                ,'0' AS [Back1]
	                                                ,'0' AS [Back2]
	                                                ,'' AS [Back3]
                                                    FROM [InvNum] i
                                                    INNER JOIN [_btblInvoiceLines] il ON i.[AutoIndex] = il.[iInvoiceID]
                                                    INNER JOIN [StkItem] s ON s.[StockLink] = il.[iStockCodeID] 
                                                    LEFT JOIN [" + My.Settings.RTDB + "].[dbo].[tblPOLines] rtp ON rtp.[vItemCode] COLLATE Latin1_General_CI_AS = s.[Code] AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
                                                    WHERE i.[OrderNum] = @1 AND i.[DocType] = '5' And (i.[DocState] = 1 OR i.[DocState] = 3) AND [bLotItem] = 0  AND i.[AutoIndex] = (SELECT TOP 1 [AutoIndex] FROM [InvNum] WHERE [OrderNum] = @1 ORDER BY [DocVersion] DESC)      ", sqlConn)

                    ',ISNULL(rtp.[dLastOrderQty], 0) AS [dLastOrderQty]
                    ',ISNULL(rtp.[dLastRecQty],0) AS [dLastRecQty]
                    ',ISNULL(rtp.[dLastPrintQty], 0) AS [dLastPrintQty]
                    ',ISNULL(rtp.[dOrderQty], il.[fQuantity] - il.[fQtyProcessed]) AS [dOrderQty]
                    ' ,ISNULL(rtp.[dRecQty], il.[fQtyToProcess]) AS [dRecQty]

                    ',ISNULL(rtp.[dLastOrderQty], 0) AS [dLastOrderQty]
                    ',ISNULL(rtp.[dLastRecQty], 0) AS [dLastRecQty]
                    ',ISNULL(rtp.[dLastPrintQty], 0) AS [dLastPrintQty]
                    ',ISNULL(rtp.[dOrderQty], 0) AS [dOrderQty]                                                   
                    ',ISNULL(rtp.[dRecQty], 0) AS [dRecQty]

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
                    Dim sqlComm As New SqlCommand(" SELECT [Bar_Code], [cSimpleCode], b.[cBinLocationName], [Description_1], [Description_2], [Description_3], [ItemGroup] 
                                                FROM [StkItem] s
                                                LEFT JOIN [_btblBINLocation] b ON s.[iBinLocationID] = b.[idBinLocation] WHERE [Code] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT [OrderNum] FROM [InvNum]
                                                    WHERE [OrderNum] = @1", sqlConn)
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
                    Dim sqlComm As New SqlCommand(" SELECT s.[Code], s.[Description_1], s.[Description_2], CAST(s.[bLotItem] AS INT), il.[cLotNumber], il.[fQuantity], il.[fQtyToProcess], 0, (il.[fQuantity] - il.[fQtyToProcess] - il.[fQtyProcessed]) AS [ViewableQty],
                                                    ISNULL(rtp.[dPrintQty], 0) FROM [_btblInvoiceLines] il
                                                    INNER JOIN [InvNum] i ON i.[AutoIndex] = il.[iInvoiceID]
                                                    INNER JOIN [StkItem] s ON il.[iStockCodeID] = s.[StockLink]
                                                    LEFT JOIN [" + My.Settings.RTDB + "].[dbo].[tblPOLines] rtp ON s.[Code] = rtp.[vItemCode] COLLATE Latin1_General_CI_AS AND il.[cLotNumber] = rtp.[vLotNumber] COLLATE Latin1_General_CI_AS AND rtp.[vOrderNum] COLLATE Latin1_General_CI_AS = [OrderNum]
                                                    WHERE i.[OrderNum] = @1 And (i.[DocState] = 1 OR i.[DocState] = 3) AND i.[DocFlag] = 1 
                                                    ORDER BY il.[cLotNumber] DESC", sqlConn)
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
