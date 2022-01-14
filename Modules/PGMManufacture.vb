Imports System.Data.SqlClient

Public Class PGMManufacture

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTIS
        Partial Public Class Retrieve

            Public Shared Function UI_GetPGMMF() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetPGMMF]", sqlConn)
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
                        Return "0*No Lines found for PGM"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for PGM"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPGMMF: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

        End Class

        Partial Public Class Update

            Public Shared Function UI_setPGMManufactured(ByVal LineID As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_GetPGMMF] @1, @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", LineID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setPGMManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_setPGMContainerManufactured(ByVal headerID As String, ByVal container As String, ByVal UserName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[UI_setPGMContainerManufactured] @1, @2, @3", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", Convert.ToInt32(headerID)))
                    sqlComm.Parameters.Add(New SqlParameter("@2", container))
                    sqlComm.Parameters.Add(New SqlParameter("@3", UserName))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_setPGMContainerManufactured: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

        End Class
        Partial Public Class Insert



        End Class
    End Class
    Public Class Evolution
        Partial Public Class Retreive

        End Class
    End Class

End Class
