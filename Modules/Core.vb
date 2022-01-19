Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Timers

Public Class Core
    Public Class RTSQL
        Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
        "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
        Partial Public Class Retreive
            Public Shared Function GetActiveModules() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetActiveModules]", sqlConn)
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
                        Return "0*No modules were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No modules were found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "GetActiveModules: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetModuleUserPermission(ByVal moduleID As String, ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)

                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetModuleUserPermission] @iModuleID, @vUser_Username", sqlConn)

                    sqlComm.Parameters.Add(New SqlParameter("@iModuleID", moduleID))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", userName))
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
                        Return "1*"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "GetModuleUserPermission: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function PGM_GetModuleUserPermission(ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_PGM_GetModuleUserPermission] @vUser_Username", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", userName))
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
                        Return "0*This user has no PGM permissions"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*This user has no PGM permissions"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "GetModuleUserPermission: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetActiveLabelTypes(ByVal userName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_GetActiveLabelTypes] @vUser_Username", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", userName))
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
                        Return "1*"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetActiveLabelTypes: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetCompatiblelabels(ByVal permission As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetCompatiblelabels] @vPermission_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vPermission_Name", permission))
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
                        Return "1*No labels's were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No labels's were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetCompatiblelabels: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPermissionHasLabel(ByVal permissionName) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPermissionHasLabel] @vPermission_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vPermission_Name", permissionName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Could not find permission"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Could not find permission"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPermissionHasLabel: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPermID(ByVal permName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_GetPermID] @vPermission_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vPermission_Name", permName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No key was found for the role " + permName
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No key was found for the role " + permName
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPermID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetPermLabelsNew(ByVal permID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetPermLabelsNew] @iPermID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iPermID", permID))
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
                        Return "0*No labels's were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No labels's were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetPermLabelsNew: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetGridOverridePassword() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetGridOverridePassword]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "-1*Setting missing: GRIDOVERRIDE"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "-1*Setting missing: GRIDOVERRIDE"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "UI_CheckUserTracking: " + ex.ToString())
                        Return "-1*" + ex.Message
                    End If
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function UI_AddPermLabels(ByVal command As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(command, sqlConn)
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeletePermLabels: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Delete
            Public Shared Function UI_DeletePermLabels(ByVal permID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeletePermLabels] @iPermID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iPermID", permID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_DeletePermLabels: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class
End Class

Public Class Role_Managemet
    Public Class RTSQL
        Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
        Partial Public Class Retreive
            Public Shared Function GetAllRoles() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_GetAllRoles]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + Convert.ToString(sqlReader.Item(4)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No roles were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No roles were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "GetAllRoles: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function GetActivePermissions() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetActivePermissions] ", sqlConn)
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
                        Return "0*No active permissions were found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No active permissions were found"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "GetActivePermissions: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetAddedRoleID(ByVal roleName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAddedRoleID] @vRole_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Name", roleName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*Role not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Role not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAddedRoleID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function GetPermID(ByVal permName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_GetPermID] @vPermission_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vPermission_Name", permName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> String.Empty Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No key was found for the role " + permName
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No key was found for the role " + permName
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "GetPermID: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function GetAllRolePerms(ByVal roleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_GetAllRolePerms] @iRole_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", roleID))
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
                        Return "0*No permissions were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No permissions were pulled"
                    Else
                        EventLog.WriteEntry("RTIS SVC", "GetAllRolePerms: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function CheckRoleInUse(ByVal roleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_CheckRoleInUse] @iRole_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", roleID))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*The selected role is in use and cannot be deactivated or removed"
                    Else
                        Return "1*Not in use"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*Not in use"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "Server Response RT Error: " + ex.Message)
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class
        Partial Public Class Insert
            Public Shared Function AddRoleHeader(ByVal RoleName As String, ByVal RoleDesc As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AddRoleHeader] @vRole_Name, @vRole_Desc", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Name", RoleName))
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Desc", RoleDesc))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AddRoleHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function AddRoleLine(ByVal RoleID As String, ByVal PermID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_AddRoleLine] @iRole_ID, @iPermission_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@iPermission_ID", PermID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "AddRoleLine: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function UpdateRoleHeader(ByVal RoleID As String, ByVal roleName As String, ByVal roleDesc As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UpdateRoleHeader] @iRole_ID, @vRole_Name, @vRole_Desc", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Name", roleName))
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Desc", roleDesc))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*D"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS SVC", "UpdateRoleHeader: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UpdateRolePermActive(ByVal RoleID As String, ByVal permissionID As String, ByVal Active As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UpdateRolePermActive] @iRole_ID, @iPermission_ID, @bPermission_Active", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@iPermission_ID", permissionID))
                    sqlComm.Parameters.Add(New SqlParameter("@bPermission_Active", Active))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*D"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UpdateRolePermActive: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function DeactivateRole(ByVal RoleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_DeactivateRole] @iRole_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*D"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "DeactivateRole: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function ActivateRole(ByVal RoleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_ActivateRole] @iRole_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*A"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "ActivateRole: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Delete
            Public Shared Function RemoveRole(ByVal RoleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_RemoveRole] @iRole_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iRole_ID", RoleID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "RemoveRole: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class
End Class

Public Class User_Management
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
           "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
       "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Class RTSQL
        Partial Public Class Retreive
            Public Shared Function UI_GetAllUsers() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetAllUsers]", sqlConn)
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0)) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    While sqlReader.Read()
                        ReturnData &= Convert.ToString(sqlReader.Item(0)) + "|" + sqlReader.Item(1) + "|" + sqlReader.Item(2) + "|" + sqlReader.Item(3) + "|" + Convert.ToString(sqlReader.Item(4)) + "|" + Convert.ToString(sqlReader.Item(5)) + "|" + Convert.ToString(sqlReader.Item(6)) + "|" + Convert.ToString(sqlReader.Item(7)) + "|" + Convert.ToString(sqlReader.Item(8)) + "~"
                    End While
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No users were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No users were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetAllUsers: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetUsersnames() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetUsersnames]", sqlConn)
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
                        Return "0*No users were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No users were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetUsersnames: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_CheckUserLogon(ByVal username As String, ByVal pin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC  [dbo].[sp_UI_CheckUserLogon] @vUser_Username, @vUser_Password", sqlConn)
                    pin = pin.Replace(" ", "+")

                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", username))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Password", pin))

                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = Convert.ToString(sqlReader.Item(0))
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No users were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No users were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "CheckUserLogon: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function

            Public Shared Function UI_GetActiveRoles() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_UI_GetActiveRoles]", sqlConn)
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
                        Return "0*No roles were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No roles were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetActiveRoles: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckUsername(ByVal username As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CheckUsername] @vUser_Username", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", username))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*A user with the username " + username + " already exists"
                    Else
                        Return "1*No users were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No users were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_CheckUsername: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_CheckUserPin(ByVal pin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_CheckUserPin] @vUser_PIN", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", pin))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "0*A user with the pin " + pin + " already exists"
                    Else
                        Return "1*No users were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "1*No users were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_CheckUserPin: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetRoleIDByName(ByVal roleName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetRoleIDByName] @vRole_Name ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Name", roleName))
                    sqlConn.Open()
                    Dim sqlReader As SqlDataReader = sqlComm.ExecuteReader()
                    sqlReader.Read()
                    ReturnData = sqlReader.Item(0)
                    sqlReader.Close()
                    sqlComm.Dispose()
                    sqlConn.Close()

                    If ReturnData <> "" Then
                        Return "1*" + ReturnData
                    Else
                        Return "0*No ID was found for the role " + roleName
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No ID was found for the role " + roleName
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetRoleIDByName: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function UI_GetRolePermsByName(ByVal roleName As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_UI_GetRolePermsByName] @vRole_Name", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vRole_Name", roleName))
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
                        Return "0*No permissions were found for the role " + roleName
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No permissions were found for the role " + roleName
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetRolePermsByName: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
            Public Shared Function MBL_GetUsersname(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_MBL_GetUsersname] @vUser_PIN", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", userPin))
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
                        Return "-1*User not found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_GetUsersname: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function ZECT_GetUsersname(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("  EXEC [dbo].[sp_ZECT_GetUsersname] @vUser_PIN", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", userPin))
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
                        Return "0*User not found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "ZECT_GetUsersname: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function

            Public Shared Function AW_GetUsersname(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" EXEC [dbo].[sp_AW_GetUsersname] @vUser_PIN", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", userPin))
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
                        Return "0*User not found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "AW_GetUsersname: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function Canning_GetUsersname(ByVal userPin As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_Canning_GetUsersname] @vUser_PIN", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", userPin))
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
                        Return "0*User not found"
                    End If
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "Canning_GetUsersname: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class


        Partial Public Class Insert
            Public Shared Function UI_AddUser(ByVal name As String, ByVal username As String, ByVal pin As String, ByVal password As String, ByVal roleId As String, ByVal hasAgent As String, ByVal evoAgent As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_AddUser] @vUser_Name, @vUser_Username, @vUser_PIN, @vUser_Password, 1, GETDATE(), @iRoleID, @bHasAgent, @vAgentName", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Name", name))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", username))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", pin))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Password", password))
                    sqlComm.Parameters.Add(New SqlParameter("@iRoleID", roleId))
                    sqlComm.Parameters.Add(New SqlParameter("@bHasAgent", hasAgent))
                    sqlComm.Parameters.Add(New SqlParameter("@vAgentName", evoAgent))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_AddUser: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Delete
            Public Shared Function UI_RemoveUser(ByVal RoleID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_RemoveUser] @iUser_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iUser_ID", RoleID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_RemoveUser: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
        Partial Public Class Update
            Public Shared Function UI_UpdateUser(ByVal userID As String, ByVal name As String, ByVal userName As String, ByVal pin As String, ByVal password As String, ByVal roleID As String, ByVal hasAgent As String, ByVal evoAgent As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_UpdateUser]@iUser_ID, @vUser_Name, @vUser_Username, @vUser_PIN, @vUser_Password, GETDATE(), @iRoleID,@bHasAgent,@vAgentName", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iUser_ID", userID))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Name", name))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Username", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_PIN", pin))
                    sqlComm.Parameters.Add(New SqlParameter("@vUser_Password", password))
                    sqlComm.Parameters.Add(New SqlParameter("@iRoleID", roleID))
                    sqlComm.Parameters.Add(New SqlParameter("@bHasAgent", hasAgent))
                    sqlComm.Parameters.Add(New SqlParameter("@vAgentName", evoAgent))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateUser: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_ActivateUser(ByVal UserID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_ActivateUser] @iUser_ID", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iUser_ID", UserID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateUser: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
            Public Shared Function UI_DeactivateUser(ByVal UserID As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_DeactivateUser] @iUser_ID ", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@iUser_ID", UserID))
                    sqlConn.Open()
                    sqlComm.ExecuteNonQuery()
                    sqlComm.Dispose()
                    sqlConn.Close()
                    Return "1*Success"
                Catch ex As Exception
                    EventLog.WriteEntry("RTIS Vulcan SVC", "UI_UpdateUser: " + ex.ToString())
                    Return ExHandler.returnErrorEx(ex)
                End Try
            End Function
        End Class
    End Class

    Public Class Evolution
        Partial Public Class Retreive
            Public Shared Function UI_GetEvoAgents() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(EvoString)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetEvoAgents] ", sqlConn)
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
                        Return "0*No Agents's were pulled"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*No Agents's were pulled"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "UI_GetEvoAgents: " + ex.ToString())
                        Return ExHandler.returnErrorEx(ex)
                    End If
                End Try
            End Function
        End Class
    End Class
End Class
