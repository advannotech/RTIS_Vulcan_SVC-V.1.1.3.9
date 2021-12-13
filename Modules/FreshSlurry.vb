Imports System.Data.SqlClient

Public Class FreshSlurry

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
"; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retreive

            Public Shared Function MBL_GetFreshSlurryInUse(ByVal trolleyCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetFreshSlurryInUse] @vTrolleyCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= trolleyCode + "|" + Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*" + ReturnData
                    Else
                        Return "1*No unfinished slurry found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No unfinished slurry found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryInUse: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetFreshSlurryWhes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetFreshSlurryWhes]", sqlConn)
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
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetFreshSlurryWhes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryLotNonManufactured(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryLotNonManufactured] @vTrolleyCode, @vItemCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
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
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryLotNonManufacturedSaveSol(ByVal trolleyCode As String, ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryLotNonManufacturedSaveSol] @vTrolleyCode, @vItemCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
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
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryWeightNonManufacturedSaveSol(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryWeightNonManufacturedSaveSol] @vTrolleyCode, @vItemCode, @vLotNumber", sqlConn)

                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lot))
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
                        Return "-1*The slurry you have scanned is either invalid or is already manufactured"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckPOUnqLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetWaitingFreshSlurries() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetWaitingFreshSlurries]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryMF: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetFreshSlurryMF() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetFreshSlurryMF]", sqlConn)
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
                        Return "0*No Lines found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryMF: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetFreshSlurryRecords(ByVal dateFrom As String, ByVal dateTo As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetFreshSlurryRecords] @dateFrom, @dateTo", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@dateFrom", dateFrom))
                    sqlComm.Parameters.Add(New SqlParameter("@dateTo", dateTo))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "|" + Convert.ToString(sqlReader.Item(10)) + "|" + Convert.ToString(sqlReader.Item(11)) + "|" + Convert.ToString(sqlReader.Item(12)) + "|" + Convert.ToString(sqlReader.Item(13)) + "|" + Convert.ToString(sqlReader.Item(14)) + "|" + Convert.ToString(sqlReader.Item(15)) + "|" + Convert.ToString(sqlReader.Item(16)) + "|" + Convert.ToString(sqlReader.Item(17)) + "|" + Convert.ToString(sqlReader.Item(18)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No records found for fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No records found for fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryRecords: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function
            Public Shared Function UI_GetFSLinkExists(ByVal slurryCode As String, ByVal rmCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetFSLinkExists] @vSlurryCode, @vRMCode", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData = Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*This item is already approved for this type of slurry"
                    Else
                        Return "1*No link found, add one"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFSLinkExists: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function MBL_GetSlurryReqRM(ByVal itemCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryReqRM] @vSlurryCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", itemCode))
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
                        Return "1*"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryReqRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_CheckLotNumberUsed(ByVal lotNumber As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_CheckLotNumberUsed] @vLotNumber", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lotNumber))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0))
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    If ReturnData <> String.Empty Then
                        Return "0*This lot number has already been used"
                    Else
                        Return "1*Ok to use"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Ok to use"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_CheckLotNumberUsed: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetSlurryRMLink(ByVal itemCode As String, ByVal rCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryRMLink] @vSlurryCode, @vRMCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rCode))
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
                        Return "0*This powder is not allowed for the slurry you are starting!"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryReqRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_GetSlurryHeaderID(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lotNumber As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryHeaderID] @vTrolleyCode, @vItemCode, @vLotNumber", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lotNumber))
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
                        Return "0*The headerID for this slurry could not be found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*The headerID for this slurry could not be found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetSlurryHeaderID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetFSRawMaterials(ByVal headerID As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetSlurryHeaderID] @iSlurryID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iSlurryID", headerID))
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
        End Class
        Partial Public Class Insert
            Public Shared Function MBL_InsertFreshSlurry(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal wetWeight As String, ByVal userName As String, ByVal desc As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_InsertFreshSlurry] @vTrolleyCode,@vItemCode,@vLotNumber,@dWetWeight,@vUserEntered,@vItemDesc)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@dWetWeight", wetWeight.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserEntered", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemDesc", desc))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertFreshSlurry: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function MBL_InsertFreshSlurryRM(ByVal headerID As String, ByVal itemCode As String, ByVal lot As String, ByVal qty As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_InsertFreshSlurryRM] @iSlurryID, @vPowderCode, @vPowderLot, @dQty,@vUserRecorded", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iSlurryID", headerID))
                    sqlComm.Parameters.Add(New SqlParameter("@vPowderCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vPowderLot", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@dQty", qty.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserRecorded", userName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_InsertFreshSlurryRM: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_InsertRMLink(ByVal slurryCode As String, ByVal rmCode As String, ByVal rmDesc As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertRMLink] @vSlurryCode, @vRMCode, @vRMDesc, @vUserAdded", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMDesc", rmDesc))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserAdded", username))
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
        End Class
        Partial Public Class Update
            Public Shared Function MBL_InvalidateSlurry(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_InvalidateSlurry] @vTrolleyCode, @vItemCode, @vLotNumber, @vUserSol ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserSol", userName))
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
            Public Shared Function MBL_setSlurrySolidity(ByVal trolleyCode As String, ByVal itemCode As String, ByVal lot As String, ByVal solidity As String, ByVal userName As String, ByVal dryWeight As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_setSlurrySolidity] @vTrolleyCode, @vItemCode, @vLotNumber, @dSolidity, @vUserSol, @dDryWeight", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vTrolleyCode", trolleyCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vLotNumber", lot))
                    sqlComm.Parameters.Add(New SqlParameter("@dSolidity", solidity.Replace(",", ".")))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserSol", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@dDryWeight", dryWeight.Replace(",", ".")))
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



            Public Shared Function UI_setFSManufactured(ByVal LineID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setFSManufactured] @iLineID, @vUserManuf", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUserManuf", LineID))
                    sqlComm.Parameters.Add(New SqlParameter("@iLineID", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setFSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


            Public Shared Function UI_setFSManufacturedManual(ByVal LineID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_setFSManufacturedManual] @iLineID, @vUserManuf", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iLineID", LineID))
                    sqlComm.Parameters.Add(New SqlParameter("@vUserManuf", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setFSManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function


        End Class
        Partial Public Class Delete
            Public Shared Function UI_DeleteRMLink(ByVal slurryCode As String, ByVal rmCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeleteRMLink] @vSlurryCode, @vRMCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", slurryCode))
                    sqlComm.Parameters.Add(New SqlParameter("@vRMCode", rmCode))
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
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function MBL_GetItemDesc(ByVal itemCode As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_MBL_GetItemDesc] @Code", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Code", itemCode))
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

            Public Shared Function UI_GetAllFreshSlurries()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllFreshSlurries]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    Dim builder As New Text.StringBuilder()
                    builder.Append(ReturnData)
                    While sqlReader.Read()
                        builder.Append(Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "~")
                    End While
                    ReturnData = builder.ToString()
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return If(ReturnData <> String.Empty, DirectCast("1*" + ReturnData, Object), DirectCast("0*Fresh slurries were found!", Object))
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllFreshSlurries: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_GetFreshSlurryRaws(ByVal slurryCode As String)
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetFreshSlurryRaws] @vSlurryCode", sqlConn)
                    sqlConn.Open()
                    sqlComm.Parameters.Add(New SqlParameter("@vSlurryCode", slurryCode))
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
                        Return "0*No raw materials found for the selected slurry"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetFreshSlurryRaws: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_GetAllFreshSlurryRMs()
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllFreshSlurryRMs]", sqlConn)
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
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllFreshSlurryRMs: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class

        Partial Public Class Insert
            Public Shared Function UI_InsertFreshSlurryForManufacture(ByVal itemCode As String, ByVal lotNumber As String, ByVal qty As String, ByVal project As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_InsertFreshSlurryForManufacture] @sBOMItemCode, @fQtyToProduce, @sLotNumber, @sProjectCode", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@sBOMItemCode", itemCode))
                    sqlComm.Parameters.Add(New SqlParameter("@sLotNumber", lotNumber))
                    sqlComm.Parameters.Add(New SqlParameter("@fQtyToProduce", qty))
                    sqlComm.Parameters.Add(New SqlParameter("@sProjectCode", project))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderForManufacture: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class
End Class
