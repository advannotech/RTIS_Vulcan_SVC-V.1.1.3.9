Imports System.Data.SqlClient

Public Class PGMPlanning

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retrieve

            Public Shared Function UI_GetVWPGMPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetVWPGMPlanLines]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for VW PGM"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for VW PGM"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetVWPGMPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetTOYOTAFSPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetTOYOTAFSPlanLines]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for Toyota fresh slurry"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for Toyota fresh slurry"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetTOYOTAFSPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetTOYOTAPPPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetTOYOTAPPPlanLines]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for Toyota powder prep"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for Toyota powder prep"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetTOYOTAPPPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function


            Public Shared Function UI_GetTOYOTAAWPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetTOYOTAAWPlanLines]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for Toyota A&W"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for Toyota A&W"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetTOYOTAAWPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function


        End Class

        Partial Public Class Update

            Public Shared Function UI_UpdateVWPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_UpdateVWPlanLines] @Slurry, @PGMCode, @User, @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", Line.Split("|")(4)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateTFSPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_UpdateTFSPlanLines] @Slurry, @PGMCode, @User, @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", Line.Split("|")(4)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateTPPPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_UpdateTPPPlanLines] @Powder, @PGMCode, @User, @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Powder", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", Line.Split("|")(4)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_UpdateTAWPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_UpdateTAWPlanLines] @Catalyst, @PGMCode, @User, @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Catalyst", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", Line.Split("|")(4)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertPowderPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class
        Partial Public Class Insert

            Public Shared Function UI_InsertVWPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_UpdateTAWPlanLines] @Slurry, @PGMCode, @User", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertVWPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertTFSPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_InsertTFSPlanLines] @Slurry, @PGMCode, @User", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertTFSPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertTPPPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_InsertTPPPlanLines] @Powder, @Powder, @Powder", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Powder", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertTFSPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertTAWPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_InsertTAWPlanLines] @Catalyst, @PGMCode, @User", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Catalyst", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@PGMCode", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertTFSPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class
    End Class
    Public Class Evolution
        Partial Public Class Retrieve

            Public Shared Function UI_GetSelectCSPPGMPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlCommCatalyst As New SqlCommand("EXEC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Catalyst]", sqlConn)
                    Dim sqlCommSlurry As New SqlCommand("EXEC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Slurry]", sqlConn)
                    Dim sqlCommPowder As New SqlCommand("EXEC [dbo].[sp_UI_GetSelectCSPPGMPlanLines_Powder]", sqlConn)
                    sqlConn.Open()

                    Dim sqlReaderCatalyst As SqlDataReader = sqlCommCatalyst.ExecuteReader()
                    While sqlReaderCatalyst.Read()
                        ReturnData &= Convert.ToString(sqlReaderCatalyst.Item(0)) + "|" + Convert.ToString(sqlReaderCatalyst.Item(1)) + "|" + Convert.ToString(sqlReaderCatalyst.Item(2)) + "|||||~"
                    End While
                    sqlReaderCatalyst.Close()

                    Dim sqlReaderSlurry As SqlDataReader = sqlCommSlurry.ExecuteReader()
                    While sqlReaderSlurry.Read()
                        ReturnData &= Convert.ToString(sqlReaderSlurry.Item(0)) + "|||" + Convert.ToString(sqlReaderSlurry.Item(1)) + "|" + Convert.ToString(sqlReaderSlurry.Item(2)) + "|||~"
                    End While
                    sqlReaderSlurry.Close()

                    Dim sqlReaderPowder As SqlDataReader = sqlCommPowder.ExecuteReader()
                    While sqlReaderPowder.Read()
                        ReturnData &= Convert.ToString(sqlReaderPowder.Item(0)) + "|||||" + Convert.ToString(sqlReaderPowder.Item(1)) + "|" + Convert.ToString(sqlReaderPowder.Item(2)) + "|~"
                    End While
                    sqlReaderPowder.Close()

                    sqlCommCatalyst.Dispose()
                    sqlCommSlurry.Dispose()
                    sqlCommPowder.Dispose()

                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for select csp"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for select csp"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetSelectCSPPGMPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetSelectPGMPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlCommPGM As New SqlCommand("EXEC  [dbo].[sp_UI_GetSelectPGMPlanLines]", sqlConn)
                    sqlConn.Open()

                    Dim sqlReaderPGM As SqlDataReader = sqlCommPGM.ExecuteReader()
                    While sqlReaderPGM.Read()
                        ReturnData &= Convert.ToString(sqlReaderPGM.Item(0)) + "|" + Convert.ToString(sqlReaderPGM.Item(1)) + "|" + Convert.ToString(sqlReaderPGM.Item(2)) + "|~"
                    End While
                    sqlReaderPGM.Close()

                    sqlCommPGM.Dispose()

                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for PGM solutions"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for PGM solutions"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetSelectPGMPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

        End Class
    End Class

End Class
