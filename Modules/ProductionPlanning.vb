Imports System.Data.SqlClient

Public Class ProductionPlanning

    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
    "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
    "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"

    Public Class RTSQL
        Partial Public Class Retrieve

            Public Shared Function UI_GetPowderPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], '' AS [vAWCode], '' AS [CatalystCode], [vSlurryCode], [vPowderCode], '' AS [CoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] FROM [rtbl_Slurry_Powders]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for powder prep"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for powder prep"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPowderPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetZECT1PlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], '' AS [vAWCode], [vCatalystCode], [vSlurryCode], '' AS [PowderCode], [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] FROM [rtbl_Slurry_Catalyst] WHERE [iZectLine] = 1", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for zect 1"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for zect 1"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetZECT1PlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetZECT2PlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], '' AS [vAWCode], [vCatalystCode], [vSlurryCode], '' AS [PowderCode], [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] FROM [rtbl_Slurry_Catalyst] WHERE [iZectLine] = 2", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for zect 2"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for zect 2"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetZECT2PlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetAWPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iLineID], [vAWCode], [vCatalystCode], '' AS [vSlurryCode], '' AS [PowderCode], '' AS [vCoatNum], [dtDateAdd], [vUserAdd], [dtDateEdit], [vUserEdit] FROM [rtbl_Catalyst_AW]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + Convert.ToString(sqlReader.Item(1)) + "|" + Convert.ToString(sqlReader.Item(2)) + "|" + Convert.ToString(sqlReader.Item(3)) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "|" + Convert.ToString(sqlReader.Item(9)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for zect 2"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for zect 2"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAWPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

        End Class

        Partial Public Class Update

            Public Shared Function UI_UpdateZECTPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [rtbl_Slurry_Catalyst] 
                                                   SET [vCatalystCode] = @Cataylst,[vSlurryCode] = @Slurry,[vCoatNum] = @Coat
                                                   ,[dtDateEdit] = GETDATE(),[vUserEdit] = @User,[iZectLine] = @ZECTnum 
                                                   WHERE [iLineID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Cataylst", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@Coat", Line.Split("|")(2)))
                    sqlComm.Parameters.Add(New SqlParameter("@DT", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(4)))
                    sqlComm.Parameters.Add(New SqlParameter("@ZECTnum", Line.Split("|")(5).Replace("ZECT", "")))
                    sqlComm.Parameters.Add(New SqlParameter("@ID", Line.Split("|")(6)))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertZECTPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_UpdatePowderPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [rtbl_Slurry_Powders] 
                                                   SET [vSlurryCode] = @Slurry,[vPowderCode] = @Powder
                                                   ,[dtDateEdit] = GETDATE(),[vUserEdit] = @User
                                                   WHERE [iLineID] = @ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@Powder", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@DT", Line.Split("|")(2)))
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

            Public Shared Function UI_UpdateAWPlanLines(ByVal id As String, ByVal AWCode As String, ByVal AWCatalystCode As String, ByVal DT As String, ByVal Username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("UPDATE [rtbl_Catalyst_AW]
                                                   SET [vAWCode] = @2 , [vCatalystCode] = @3
                                                   ,[dtDateEdit] = GETDATE(),[vUserEdit] = @5
                                                   WHERE [iLineID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", id))
                    sqlComm.Parameters.Add(New SqlParameter("@2", AWCode))
                    sqlComm.Parameters.Add(New SqlParameter("@3", AWCatalystCode))
                    'sqlComm.Parameters.Add(New SqlParameter("@4", DT))
                    sqlComm.Parameters.Add(New SqlParameter("@5", Username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateAWPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Insert

            Public Shared Function UI_InsertAWPlanLines(ByVal AWCode As String, ByVal CatalystCode As String, ByVal DT As String, ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [rtbl_Catalyst_AW]([vAWCode],[vCatalystCode],[dtDateAdd],[vUserAdd])
                                                    VALUES(@1, @2, GETDATE(), @4)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", AWCode))
                    sqlComm.Parameters.Add(New SqlParameter("@2", CatalystCode))
                    'sqlComm.Parameters.Add(New SqlParameter("@3", DT))
                    sqlComm.Parameters.Add(New SqlParameter("@4", username))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertAWPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertZECTPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [rtbl_Slurry_Catalyst]([vCatalystCode],[vSlurryCode],[vCoatNum],[dtDateAdd],[vUserAdd],[iZectLine])
                                                    VALUES(@Cataylst,@Slurry,@Coat,GETDATE(),@User,@ZECTnum)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Cataylst", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@Coat", Line.Split("|")(2)))
                    sqlComm.Parameters.Add(New SqlParameter("@DT", Line.Split("|")(3)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(4)))
                    sqlComm.Parameters.Add(New SqlParameter("@ZECTnum", Line.Split("|")(5).Replace("ZECT", "")))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*SUCCESS"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_InsertZECTPlanLines: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function UI_InsertPowderPlanLines(ByVal Line As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("INSERT INTO [rtbl_Slurry_Powders]([vSlurryCode],[vPowderCode],[dtDateAdd],[vUserAdd])
                                                   VALUES(@Slurry,@Powder,GETDATE(),@User)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@Slurry", Line.Split("|")(0)))
                    sqlComm.Parameters.Add(New SqlParameter("@Powder", Line.Split("|")(1)))
                    sqlComm.Parameters.Add(New SqlParameter("@DT", Line.Split("|")(2)))
                    sqlComm.Parameters.Add(New SqlParameter("@User", Line.Split("|")(3)))
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
    End Class
    Public Class Evolution
        Partial Public Class Retrieve

            Public Shared Function UI_GetSelectCSPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlCommCatalyst As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like '18461-%' AND [Code] Not Like '%A&W%' AND [Code] Not Like '%AW%' ORDER BY [Code] ASC", sqlConn)
                    Dim sqlCommSlurry As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like 'TSP-%' OR [Code] Like 'VSP-%' ORDER BY [Code] ASC", sqlConn)
                    Dim sqlCommAW As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like '%A&W%' OR [Code] Like '%AW%' ORDER BY [Code] ASC", sqlConn)
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

                    Dim sqlAWReader As SqlDataReader = sqlCommAW.ExecuteReader()
                    While sqlAWReader.Read()
                        ReturnData &= Convert.ToString(sqlAWReader.Item(0)) + "|||||" + Convert.ToString(sqlAWReader.Item(1)) + "|" + Convert.ToString(sqlAWReader.Item(2)) + "|~"
                    End While
                    sqlAWReader.Close()

                    'OLD
                    'Dim sqlReaderCatalyst As SqlDataReader = sqlCommCatalyst.ExecuteReader()
                    'While sqlReaderCatalyst.Read()
                    '    ReturnData &= Convert.ToString(sqlReaderCatalyst.Item(0)) + "|" + Convert.ToString(sqlReaderCatalyst.Item(1)) + "|" + Convert.ToString(sqlReaderCatalyst.Item(2)) + "|||~"
                    'End While
                    'sqlReaderCatalyst.Close()

                    'Dim sqlReaderSlurry As SqlDataReader = sqlCommSlurry.ExecuteReader()
                    'While sqlReaderSlurry.Read()
                    '    ReturnData &= Convert.ToString(sqlReaderSlurry.Item(0)) + "|||" + Convert.ToString(sqlReaderSlurry.Item(1)) + "|" + Convert.ToString(sqlReaderSlurry.Item(2)) + "|~"
                    'End While
                    'sqlReaderSlurry.Close()

                    sqlReaderCatalyst.Close()
                    sqlCommCatalyst.Dispose()
                    sqlReaderSlurry.Close()
                    sqlCommSlurry.Dispose()
                    sqlAWReader.Close()
                    sqlCommAW.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for select cs"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for select cs"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetSelectCSPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

            Public Shared Function UI_GetSelectSPPlanLines() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlCommCatalyst As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like 'TSP-%' OR [Code] Like 'VSP-%' ORDER BY [Code] ASC", sqlConn)
                    Dim sqlCommSlurry As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like 'TPP-%' ORDER BY [Code] ASC", sqlConn)
                    Dim sqlCommAW As New SqlCommand("SELECT [StockLink], [Code], [Description_1] FROM [StkItem] WHERE [Code] Like '18461-%' AND [Code] Not Like '%A&W%' AND [Code] Not Like '%AW%' ORDER BY [Code] ASC", sqlConn)
                    sqlConn.Open()

                    Dim sqlReaderAW As SqlDataReader = sqlCommAW.ExecuteReader()
                    While sqlReaderAW.Read()
                        ReturnData &= Convert.ToString(sqlReaderAW.Item(0)) + "|||||" + Convert.ToString(sqlReaderAW.Item(1)) + "|" + Convert.ToString(sqlReaderAW.Item(2)) + "|~"
                    End While
                    sqlReaderAW.Close()

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



                    sqlCommCatalyst.Dispose()
                    sqlCommSlurry.Dispose()

                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No Lines found for select sp"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Lines found for select sp"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetSelectSPPlanLines: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try

            End Function

        End Class
    End Class

End Class
