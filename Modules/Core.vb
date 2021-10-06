Imports System.Data.SqlClient
Public Class Core
    Public Class RTSQL
        Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
        "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
        Partial Public Class Retreive
            Public Shared Function GetActiveModules() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand("SELECT [iModule_ID], [vModule_Name] FROM [htbl_Modules] WHERE [bModuleActive] = 1 ORDER BY [Indx] ASC", sqlConn)
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

                    Dim sqlComm As New SqlCommand(" SELECT p.[vPermission_Name], p.[vNestNode] 
                                                    FROM [ltbl_Module_Perms] p
                                                    INNER JOIN [ltbl_userRoleLines] rl ON p.[iPermission_ID] = rl.[iPermission_ID]
                                                    INNER JOIN [tbl_users] u ON u.[iRoleID] = rl.[iRole_ID]
                                                    WHERE rl.[bPermission_Active] = 1 AND p.[iModuleID] = @1 AND u.[vUser_Username] = @2 AND p.[bUIPerm] = 1
                                                    ORDER BY p.[Indx]", sqlConn)

                    sqlComm.Parameters.Add(New SqlParameter("@1", moduleID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", userName))
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
                    Dim sqlComm As New SqlCommand(" SELECT p.[vPermission_Name]
                                                    FROM [ltbl_Module_Perms] p
                                                    INNER JOIN [ltbl_userRoleLines] rl ON p.[iPermission_ID] = rl.[iPermission_ID]
                                                    INNER JOIN [tbl_users] u ON u.[iRoleID] = rl.[iRole_ID]
                                                    WHERE rl.[bPermission_Active] = 1 AND u.[vUser_Username] = @1 AND p.[bPGMPerm] = 1
                                                    ORDER BY p.[Indx]", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userName))
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
                    Dim sqlComm As New SqlCommand(" SELECT DISTINCT lt.[vLabel_Type_Name] 
                                                    FROM [tbl_labelTypes] lt
                                                    INNER JOIN [rtbl_LabelPermCom] lpc ON lpc.[iLabelID] = lt.[iLabel_ID]
                                                    INNER JOIN [ltbl_userRoleLines] rl ON rl.[iPermission_ID] = lpc.[iPermissionID]
                                                    INNER JOIN [htbl_userRoles] rh ON rl.[iRole_ID] = rh.[iRole_ID]
                                                    INNER JOIN [tbl_users] u ON u.[iRoleID] = rh.[iRole_ID]
                                                    WHERE rl.[bPermission_Active] = '1' AND u.[vUser_Username] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userName))
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
                    Dim sqlComm As New SqlCommand(" SELECT l.[vLabel_Type_Name] FROM [rtbl_LabelPermCom] lpc 
                                                    INNER JOIN [ltbl_Module_Perms] p ON lpc.[iPermissionID] = p.[iPermission_ID]
                                                    INNER JOIN [tbl_labelTypes] l ON lpc.[iLabelID] = l.[iLabel_ID]
                                                    WHERE p.[vPermission_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permission))
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
                    Dim sqlComm As New SqlCommand("SELECT [bHasLabel] FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permissionName))
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
                    Dim sqlComm As New SqlCommand(" SELECT [iPermission_ID]
                                                    FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permName))
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
                    Dim sqlComm As New SqlCommand(" SELECT [vLabelName] FROM [rtbl_PermLabels]
                                                    WHERE [iPermID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permID))
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
                    Dim sqlComm As New SqlCommand("SELECT [SettingValue] FROM [tbl_RTSettings] WHERE [Setting_Name] = 'GRIDOVERRIDE'", sqlConn)
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [rtbl_PermLabels] WHERE [iPermID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permID))
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
                    Dim sqlComm As New SqlCommand("  SELECT rh.[iRole_ID], rh.[vRole_Name], rh.[vRole_Desc], STUFF((SELECT ',' + CAST(p.[vPermission_Name] AS VARCHAR(MAX)) FROM [ltbl_userRoleLines] rl
				                                     INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rl.iPermission_ID
				                                     WHERE rh.iRole_ID = rl.[iRole_ID] AND rl.[bPermission_Active] = 1
				                                     FOR XML PATH('')),1,1,''), rh.[bRole_Active] FROM [htbl_userRoles] rh", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [vPermission_Name] FROM [ltbl_Module_Perms] WHERE [bPermissionActive] = 1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [iRole_ID] FROM [htbl_userRoles] WHERE [vRole_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", roleName))
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
                    Dim sqlComm As New SqlCommand(" SELECT [iPermission_ID]
                                                    FROM [ltbl_Module_Perms] WHERE [vPermission_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", permName))
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
                    Dim sqlComm As New SqlCommand("SELECT p.[vPermission_Name]
		                                                 ,rp.[bPermission_Active]
	                                               FROM [ltbl_userRoleLines] rp 
                                                   INNER JOIN [ltbl_Module_Perms] p ON p.[iPermission_ID] = rp.[iPermission_ID]
                                                   WHERE rp.[iRole_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", roleID))
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
                    Dim sqlComm As New SqlCommand("SELECT [iRoleID]
	                                           FROM [tbl_users] WHERE [iRoleID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", roleID))
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
                    Dim sqlComm As New SqlCommand("INSERT INTO [htbl_userRoles] ([vRole_Name]
                                                                                ,[vRole_Desc]
                                                                                ,[bRole_Active]
                                                                                ,[dRole_Created]
                                                                                )
                                                VALUES (@1, @2, 1, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleName))
                    sqlComm.Parameters.Add(New SqlParameter("@2", RoleDesc))
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
                    Dim sqlComm As New SqlCommand("INSERT INTO [ltbl_userRoleLines] ([iRole_ID]
                                                                                ,[iPermission_ID]
                                                                                ,[bPermission_Active]
                                                                                ,[dPermission_Added]
                                                                                )
                                                VALUES (@1, @2, 1, GETDATE())", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", PermID))
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
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_userRoles] SET [vRole_Name] = @2, [vRole_Desc] = @3, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", roleName))
                    sqlComm.Parameters.Add(New SqlParameter("@3", roleDesc))
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
                    Dim sqlComm As New SqlCommand("UPDATE [ltbl_userRoleLines] SET [bPermission_Active] = @3, [dPermission_Removed] =  GETDATE() WHERE [iRole_ID] = @1 AND [iPermission_ID] = @2", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", permissionID))
                    sqlComm.Parameters.Add(New SqlParameter("@3", Active))
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
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_userRoles] SET [bRole_Active] = 0, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
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
                    Dim sqlComm As New SqlCommand("UPDATE [htbl_userRoles] SET [bRole_Active] = 1, [dRole_Modified] =  GETDATE() WHERE [iRole_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [htbl_userRoles] WHERE [iRole_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
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
                    Dim sqlComm As New SqlCommand("  SELECT u.[iUser_ID]
                                                ,u.[vUser_Name]
                                                ,u.[vUser_Username]
                                                ,u.[vUser_PIN]
                                                ,u.[vUser_Password]
	                                            ,r.[vRole_Name]
                                                ,u.[bUser_IsActive]
                                                ,u.[bHasAgent]
                                                ,u.[vAgentName]
	                                            FROM [tbl_users] u INNER JOIN [htbl_userRoles] r 
	                                            ON u.[iRoleID] = r.[iRole_ID]", sqlConn)
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
                    Dim sqlComm As New SqlCommand("  SELECT [vUser_Username]
	                                                 FROM [tbl_users] WHERE [bUser_IsActive] = 1", sqlConn)
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
            Public Shared Function UI_CheckUserLogon(ByVal username As String, ByVal password As String) As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    Dim sqlComm As New SqlCommand(" SELECT [vUser_Username]
	                                                FROM [tbl_users] WHERE [vUser_Username] = @1 AND [vUser_Password] = @2  AND [bUser_IsActive] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", username))
                    sqlComm.Parameters.Add(New SqlParameter("@2", password))
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
                    Dim sqlComm As New SqlCommand(" SELECT [vRole_Name]
                                                    FROM [htbl_userRoles] WHERE [bRole_Active] = 1", sqlConn)
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
                    Dim sqlComm As New SqlCommand("SELECT [vUser_Username]
                                                   FROM [tbl_users] WHERE [vUser_Username] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", username))
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
                    Dim sqlComm As New SqlCommand("SELECT [vUser_PIN]
                                                   FROM [tbl_users] WHERE [vUser_PIN] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", pin))
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
                    Dim sqlComm As New SqlCommand(" SELECT [iRole_ID]
                                                    FROM [htbl_userRoles] WHERE [vRole_Name] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", roleName))
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
                    Dim sqlComm As New SqlCommand("  SELECT mp.[vPermission_Name] FROM [ltbl_userRoleLines] rl
                                                     INNER JOIN [ltbl_Module_Perms] mp ON mp.[iPermission_ID] = rl.[iPermission_ID]
                                                     INNER JOIN [htbl_userRoles] rh ON rl.[iRole_ID] = rh.[iRole_ID]
                                                     WHERE rh.[vRole_Name] = @1 AND [bPermission_Active] = 1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", roleName))
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
                    Dim sqlComm As New SqlCommand("  SELECT [vUser_Username]
	                                                 FROM [tbl_users] WHERE [vUser_PIN] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                    Dim sqlComm As New SqlCommand("  SELECT [vUser_Username]
	                                                 FROM [tbl_users] WHERE [vUser_PIN] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                    Dim sqlComm As New SqlCommand("  SELECT [vUser_Username]
	                                                 FROM [tbl_users] WHERE [vUser_PIN] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                    Dim sqlComm As New SqlCommand("  SELECT [vUser_Username]
	                                                 FROM [tbl_users] WHERE [vUser_PIN] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userPin))
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
                    Dim sqlComm As New SqlCommand("INSERT INTO [tbl_users] ([vUser_Name]
                                                                                ,[vUser_Username]
                                                                                ,[vUser_PIN]
                                                                                ,[vUser_Password]
                                                                                ,[bUser_IsActive]
                                                                                ,[dUser_Created]
                                                                                ,[iRoleID]
                                                                                ,[bHasAgent]
                                                                                ,[vAgentName]
                                                                                )
                                                VALUES (@1, @2, @3, @4, 1, GETDATE(), @5, @6, @7)", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", name))
                    sqlComm.Parameters.Add(New SqlParameter("@2", username))
                    sqlComm.Parameters.Add(New SqlParameter("@3", pin))
                    sqlComm.Parameters.Add(New SqlParameter("@4", password))
                    sqlComm.Parameters.Add(New SqlParameter("@5", roleId))
                    sqlComm.Parameters.Add(New SqlParameter("@6", hasAgent))
                    sqlComm.Parameters.Add(New SqlParameter("@7", evoAgent))
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
                    Dim sqlComm As New SqlCommand("DELETE FROM [tbl_users] WHERE [iUser_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", RoleID))
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_users] SET [vUser_Name] = @2
                                                                                ,[vUser_Username] = @3
                                                                                ,[vUser_PIN] = @4
                                                                                ,[vUser_Password] = @5
                                                                                ,[dUser_Modified] = GETDATE()
                                                                                ,[iRoleID] = @6
                                                                                ,[bHasAgent] = @7
                                                                                ,[vAgentName] = @8
                                                                                WHERE [iUser_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", userID))
                    sqlComm.Parameters.Add(New SqlParameter("@2", name))
                    sqlComm.Parameters.Add(New SqlParameter("@3", userName))
                    sqlComm.Parameters.Add(New SqlParameter("@4", pin))
                    sqlComm.Parameters.Add(New SqlParameter("@5", password))
                    sqlComm.Parameters.Add(New SqlParameter("@6", roleID))
                    sqlComm.Parameters.Add(New SqlParameter("@7", hasAgent))
                    sqlComm.Parameters.Add(New SqlParameter("@8", evoAgent))
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_users] SET [bUser_IsActive] = 1,[dUser_Modified] =  GETDATE() WHERE [iUser_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", UserID))
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
                    Dim sqlComm As New SqlCommand("UPDATE [tbl_users] SET [bUser_IsActive] = 0,[dUser_Modified] =  GETDATE() WHERE [iUser_ID] = @1", sqlConn)
                    sqlComm.Parameters.Add(New SqlParameter("@1", UserID))
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
                    Dim sqlComm As New SqlCommand(" SELECT
                                                [cAgentName] 
                                                FROM [_rtblAgents]", sqlConn)
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
