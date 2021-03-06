Imports System.Data.SqlClient
Imports Pastel.Evolution

Public Class Transfers

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retreive

#Region "General"
            Public Shared Function MBL_CheckWhseStockAso(ByVal itemcode As String, ByVal whsecode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_CheckWhseStockAso] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemcode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", whsecode))
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
                        Return "-1*The item scanned is not associated with this warehouse!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*The item scanned is not associated with this warehouse!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckWhseStockAso: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

#Region "Receiving Transfers"
            Public Shared Function MBL_GetRecWhse(ByVal procName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetRecWhse] @1", sqlConn)
                    sqlComm.Parameters.Add(new SqlParameter("@1",procName))
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
                        Return "-1*No warehouse set for powder prep!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for powder prep!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPowderPrepWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPPRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_MBL_GetPPRecWhses]", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPPRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFSRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC MBL_GetFSRecWhses ", sqlConn)
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
                        Return "-1*No warehouses set for fresh slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouses set for fresh slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFSRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMSRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetMSRecWhses] ", sqlConn)
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
                        Return "-1*No warehouses set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouses set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMSRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZect1RecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_GetZect1RecWhses]", sqlConn)
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
                        Return "-1*No warehouses set for zect 1!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for zect 1!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZect1RecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZect2RecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetZect2RecWhses] ", sqlConn)
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
                        Return "-1*No warehouse set for zect 2!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for zect 2!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZect2RecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAWRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetAWRecWhses]", sqlConn)
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
                        Return "-1*No warehouse set for AW!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for AW!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetAWRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetCanningRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetCanningRecWhses] ", sqlConn)
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
                        Return "-1*No warehouse set for canning!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for canning!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetCanningRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPGMReceivingWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetPGMReceivingWhses]", sqlConn)
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
                        Return "0*No receivable warehouse were found!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No receivable warehouse were found!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPGMRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPGMRecWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetPGMRecWhses]", sqlConn)
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
                        Return "-1*No warehouse set for AW!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for AW!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPGMRecWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

#End Region

#Region "Transfer Out"
            Public Shared Function MBL_GetSlurryPowders(ByVal slurryCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_GetSlurryPowders] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", slurryCode))
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
                        Return "-1*No powders found for slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No powders found for slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryPowders: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPowderPrepWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetPowderPrepWhes] ", sqlConn)
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
                        Return "-1*No warehouse set for powder prep!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for powder prep!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPowderPrepWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetPowderPrepWhes_OLD() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetPowderPrepWhes_OLD] ", sqlConn)
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
                        Return "-1*No warehouse set for powder prep!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for powder prep!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetPowderPrepWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetFreshSlurryWhes] ", sqlConn)
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
                        Return "-1*No warehouse set for fresh slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for fresh slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetMixedSlurryWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)


                    'Dim sqlComm As New SqlCommand("    SELECT w.[Code], w.[Name] FROM [RTIS_WarehouseLookUp_MStZect] wl
                    '                                   INNER JOIN [" + My.Settings.EvoDB + "].[dbo].[WhseMst] w ON w.[WhseLink] = wl.[iWhse_Link]
                    '                                   WHERE [bEnabled] = 1", sqlConn)

                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetMixedSlurryWhes] ", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetMixedSlurryWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetToProdWhses() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetToProdWhses]", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetToProdWhses: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZect1Whes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetZect1Whes] ", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZect1Whes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZect2Whes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetZect2Whes] ", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZect2Whes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAWWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetAWWhes]", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetAWWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetCanningWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetCanningWhes] ", sqlConn)
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
                        Return "-1*No warehouse set for mixed slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No warehouse set for mixed slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetCanningWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckPowderManufactured(ByVal itemCode As String, ByVal lotNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_CheckPowderManufactured] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
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
                        Return "-1*No meanufacture record found for the scanned item!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No meanufacture record found for the scanned item!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPowderManufactured: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_CheckPowderReceieved(ByVal itemCode As String, ByVal lotNum As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_MBL_CheckPowderReceieved] @1,@2,@3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", qty.Replace(",", ".")))
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
                        Return "-1*No record found for the scanned powder!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No record found for the scanned powder!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPowderManufactured: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryManufactured(ByVal trolleyCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_GetFreshSlurryManufactured] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*No powders found for slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No powders found for slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryTransferInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetZectManufactured(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_MBL_GetZectManufactured] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
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
                        Return "-1*No pallet found with this ID!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No pallet found with this ID!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZectManufactured: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAWManufactured(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT ISNULL([bManuf], 0) FROM [tbl_RTIS_AW]
                                                     WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
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
                        Return "-1*No pallet found with this ID!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No pallet found with this ID!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZectManufactured: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetAWTransferred(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  SELECT ISNULL([bTrans], 0) FROM [tbl_RTIS_AW]
                                                     WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
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
                        Return "-1*No pallet found with this ID!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No pallet found with this ID!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetAWTransferred: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetZectTransferred(ByVal lineID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetZectTransferred] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lineID))
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
                        Return "-1*No pallet found with this ID!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No pallet found with this ID!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetZectTransferred: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryTransferred(ByVal trolleyCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_GetFreshSlurryTransferred] @1,@2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
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
                        Return "-1*No powders found for slurry!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*No powders found for slurry!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryTransferInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetSlurryTransferInfo(ByVal trolleyCode As String, ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryTransferInfo] @1, @2", sqlConn)
                    ' --AND [bManuf] = 1 
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Slurry not found!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Slurry not found!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryTransferInfo: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
#End Region

        End Class

        Partial Public Class Insert
            Public Shared Function UI_InsertWhseTransfer(ByVal itemCode As String, ByVal lotNum As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String, ByVal username As String, ByVal process As String, ByVal transDesc As String, ByVal ststus As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_InsertWhseTransfer] @1, @2, @3, @4, @5, @6, @7, @8, @9", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlComm.Parameters.Add(New SqlParameter("@7", process))
                    sqlComm.Parameters.Add(New SqlParameter("@8", transDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@9", ststus))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_whtTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertFGWhseTransfer(ByVal itemCode As String, ByVal lotNum As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String, ByVal username As String, ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_InsertFGWhseTransfer] @1, @2, @3, @4, @5, @6, @7 ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlComm.Parameters.Add(New SqlParameter("@7", process))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_whtTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


            Public Shared Function UI_whtTransferLog(ByVal itemCode As String, ByVal lotNum As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal qty As String, ByVal username As String, ByVal process As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_whtTransferLog] @1, @2, @3, @4, @5, @6, @7", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@5", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlComm.Parameters.Add(New SqlParameter("@7", process))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_whtTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_whtTransferLogNew(ByVal itemCode As String, ByVal lotNum As String, ByVal whseFrom As String, ByVal whseTo As String, ByVal process As String, ByVal unq As String, ByVal qty As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_WHTLog] ([vItemCode], [vLotNumber], [vWarehouse_From], [vWarehouse_To], [vProcess], [vUnq], [dQtyTransfered], [dtDateTransfered], [vUsernameTransfered])
                                                                               VALUES (@1, @2, @3, @4, @5, @6, @7, GETDATE(), @8)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", whseFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@4", whseTo))
                    sqlComm.Parameters.Add(New SqlParameter("@5", process))
                    sqlComm.Parameters.Add(New SqlParameter("@6", unq))
                    sqlComm.Parameters.Add(New SqlParameter("@7", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@6", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_whtTransferLog: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class

        Partial Public Class Update
            Public Shared Function MBL_UpdatePowderTransferred(ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_MBL_UpdatePowderTransferred] @1 ,@2 ,@3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePowderTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_UpdatePowderTransferredIn(ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String, ByVal qty As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_UpdatePowderTransferredIn] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@3", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@4", qty.Replace(",", ".")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePowderTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_UpdateSlurryTransferred(ByVal trolley As String, ByVal itemCode As String, ByVal lotNum As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_UpdateSlurryTransferred] @1,@2,@3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", trolley))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNum))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdatePowderTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function



            Public Shared Function MBL_UpdateMobileTankTransferred(ByVal tank As String, ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_UpdateMobileTankTransferred] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tank))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateMobileTankTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_UpdateMobileTankReceived(ByVal tank As String, ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_UpdateMobileTankReceived] @1,@2,@3,@4", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tank))
                    sqlComm.Parameters.Add(New SqlParameter("@2", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@4", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateMobileTankReceived: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_UpdateLargeTankTransferred(ByVal tankType As String, ByVal tank As String, ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_MBL_UpdateLargeTankTransferred] @1,@2,@3,@4,@5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tank))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@5", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateLargeTankTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_UpdateLargeTankReceived(ByVal tankType As String, ByVal tank As String, ByVal itemCode As String, ByVal lotNum As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_UpdateLargeTankTransferred] @1,@2,@3,@4,@5", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", tankType))
                    sqlComm.Parameters.Add(New SqlParameter("@2", tank))
                    sqlComm.Parameters.Add(New SqlParameter("@3", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@4", lotNum))
                    sqlComm.Parameters.Add(New SqlParameter("@5", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateLargeTankTransferred: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_setFreshSlurryReceived(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_setFreshSlurryReceived] @1,@2,@3,@4", sqlConn)
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
        End Class
    End Class
    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function MBL_ValidatePPItem(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_ValidatePPItem] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
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
                        Return "-1*Item not found in evolution!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Item not found in evolution!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_ValidatePPItem: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_ValidatePPLot(ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_ValidatePPLot] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotNumber))
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
                        Return "-1*Lot not found in evolution!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Lot not found in evolution!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_ValidatePPLot: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetLotID(ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_GetLotID] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", lotNumber))
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
                        Return "-1*Lot not found in evolution!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Lot not found in evolution!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_ValidatePPLot: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function MBL_GetPPItemInfo(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_GetPPItemInfo] @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", itemCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Item not found in evolution!"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Item not found in evolution!"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_ValidatePPItem: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
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
                If ex.Message.Contains("Negative lot tracking quantity not allowed!") Then
                    EventLog.WriteEntry("RTIS SVC", "CTransferItem: " + ex.ToString)
                    Return "-1*Insufficient qty for lot"
                Else
                    EventLog.WriteEntry("RTIS SVC", "CTransferItem: " + ex.ToString)
                    Return "-1*" + ItemCode + " Reason: " + ex.Message
                End If

            End Try
        End Function
    End Class
End Class
