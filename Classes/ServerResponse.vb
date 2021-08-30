Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports DevExpress.XtraReports.UI

Public Class ServerResponse
    Public Shared Sub Determine(ByVal ClientMsg As String, ByVal ClientSocket As Socket)
        Dim ClientRequest As String = ClientMsg.Split("@")(0)
        Dim ClientData As String = ClientMsg.Split("@")(1)
        Dim sep As String = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
        Dim project As String = My.Settings.Project

        Select Case ClientRequest

#Region "UI"

#Region "Core"

#Region "Core"
            Case "*TEST*"
                Server.Listener.SendResponse(ClientSocket, "1")
            Case "*GETUSERNAMES*"
                Try
                    'Dim f As String = String.Empty
                    'Dim fUp As String = f.Split("|")(3)
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.UI_GetUsersnames())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UILOGIN*"
                Try
                    Dim userName As String = ClientData.Split("|")(0)
                    Dim password As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.UI_CheckUserLogon(userName, password))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETACTIVEMODULES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.GetActiveModules())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETUSERPERMISSIONS*"
                Try
                    Dim moduleID As String = ClientData.Split("|")(1)
                    Dim userName As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetModuleUserPermission(moduleID, userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETACTIVELABElTYPES*"
                Try
                    Dim userName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetActiveLabelTypes(userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPERMISSIONHASLABEL*"
                Try
                    Dim permissionName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetPermissionHasLabel(permissionName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETACTIVELABELS*"
                Try
                    Dim permissionName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetCompatiblelabels(permissionName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPERMLABELSNEW*"
                Try
                    Dim permName As String = ClientData
                    Dim permID As String = Core.RTSQL.Retreive.UI_GetPermID(permName)
                    Select Case permID.Split("*")(0)
                        Case "1"
                            permID = permID.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetPermLabelsNew(permID))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, permID)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEMODULELABELCONFIGS*"
                Try
                    Dim permName As String = ClientData.Split("*")(0)
                    Dim labelInfo As String = ClientData.Split("*")(1)
                    Dim permID As String = Core.RTSQL.Retreive.UI_GetPermID(permName)
                    Select Case permID.Split("*")(0)
                        Case "1"
                            permID = permID.Remove(0, 2)
                            Dim deleted As String = Core.RTSQL.Delete.UI_DeletePermLabels(permID)
                            Select Case deleted.Split("*")(0)
                                Case "1"
                                    Dim query As String = "INSERT INTO [rtbl_PermLabels] ([vLabelName], [iPermID]) VALUES "
                                    For Each label As String In labelInfo.Split("|")
                                        If label <> String.Empty Then
                                            query += "('" + label + "','" + permID + "'),"
                                        End If
                                    Next
                                    query = query.Substring(0, query.Length - 1)
                                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Insert.UI_AddPermLabels(query))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, deleted)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, deleted)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, permID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, permID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETGRIDOVERRIDEPASSWORD*"
                Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.UI_GetGridOverridePassword())
#End Region

#Region "Role Management"
            Case "*GETALLROLES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Role_Managemet.RTSQL.Retreive.GetAllRoles)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETACTIVEPERMISSIONS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Role_Managemet.RTSQL.Retreive.GetActivePermissions())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDROLE*"
                Dim roleName As String = ClientData.Split("|")(0)
                Dim roleDesc As String = ClientData.Split("|")(1)
                Dim permissions As String() = ClientData.Split("|")(2).Split(",")
                Dim headerAdded As String = Role_Managemet.RTSQL.Insert.AddRoleHeader(roleName, roleDesc)
                Select Case headerAdded.Split("*")(0)
                    Case "1"
                        Dim roleID As String = Role_Managemet.RTSQL.Retreive.UI_GetAddedRoleID(roleName)
                        Select Case roleID.Split("*")(0)
                            Case "1"
                                roleID = roleID.Remove(0, 2)
                                Dim falied As Boolean = False
                                Dim failureReason As String = String.Empty
                                For Each permission As String In permissions
                                    If permission <> String.Empty Then
                                        Dim permKey As String = Role_Managemet.RTSQL.Retreive.GetPermID(permission)
                                        Select Case permKey.Split("*")(0)
                                            Case "1"
                                                permKey = permKey.Remove(0, 2)
                                                Dim added As String = Role_Managemet.RTSQL.Insert.AddRoleLine(roleID, permKey)
                                                Select Case added.Split("*")(0)
                                                    Case "1"

                                                    Case "-1"
                                                        failureReason = added
                                                        falied = True
                                                End Select
                                            Case "0"
                                                failureReason = roleID
                                                falied = True
                                            Case "-1"
                                                failureReason = roleID
                                                falied = True
                                        End Select
                                    End If
                                Next
                                If falied = False Then
                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                Else
                                    Server.Listener.SendResponse(ClientSocket, failureReason)
                                End If
                            Case "0"
                                Server.Listener.SendResponse(ClientSocket, roleID)
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, roleID)
                        End Select
                    Case "-1"
                        Server.Listener.SendResponse(ClientSocket, headerAdded)
                End Select
            Case "*UPDATEROLE*"
                Try
                    Dim roleID As String = ClientData.Split("|")(0)
                    Dim roleName As String = ClientData.Split("|")(1)
                    Dim roleDesc As String = ClientData.Split("|")(2)
                    Dim permissions As String() = ClientData.Split("|")(3).Split(",")
                    Dim perms As String = String.Empty

                    Dim headerUpdated As String = Role_Managemet.RTSQL.Update.UpdateRoleHeader(roleID, roleName, roleDesc)
                    Select Case headerUpdated.Split("*")(0)
                        Case "1"
                            perms = Role_Managemet.RTSQL.Retreive.GetAllRolePerms(roleID)
                            Select Case perms.Split("*")(0)
                                Case "1"
                                    perms = perms.Remove(0, 2)
                                    Dim allPerms As String() = perms.Split("~")

                                    Dim failed As Boolean = False
                                    Dim failureReason As String = String.Empty
                                    For Each permission As String In allPerms
                                        If permission <> String.Empty Then
                                            Dim permName As String = permission.Split("|")(0)
                                            Dim permActive As String = permission.Split("|")(1)
                                            If permissions.Contains(permName) = False Then
                                                Dim permID As String = Role_Managemet.RTSQL.Retreive.GetPermID(permName)
                                                Select Case permID.Split("*")(0)
                                                    Case "1"
                                                        permID = permID.Remove(0, 2)
                                                        Dim permDeactivated As String = Role_Managemet.RTSQL.Update.UpdateRolePermActive(roleID, permID, "0")
                                                        Select Case permDeactivated.Split("*")(0)
                                                            Case "1"

                                                            Case "-1"
                                                                failed = True
                                                                failureReason = permDeactivated
                                                        End Select
                                                    Case "0"
                                                        failed = True
                                                        failureReason = permID
                                                    Case "-1"
                                                        failed = True
                                                        failureReason = permID
                                                End Select
                                            End If
                                        End If
                                    Next

                                    If failed = False Then
                                        Dim failed2 As Boolean = False
                                        Dim failureReason2 As String = String.Empty
                                        For Each permission As String In permissions
                                            If permission <> String.Empty Then
                                                If perms.Contains(permission) = False Then
                                                    Dim permID As String = Role_Managemet.RTSQL.Retreive.GetPermID(permission)
                                                    Select Case permID.Split("*")(0)
                                                        Case "1"
                                                            permID = permID.Remove(0, 2)
                                                            Dim permissionAdded As String = Role_Managemet.RTSQL.Insert.AddRoleLine(roleID, permID)
                                                            Select Case permissionAdded.Split("*")(0)
                                                                Case "1"

                                                                Case "-1"
                                                                    failed2 = True
                                                                    failureReason2 = permissionAdded
                                                            End Select
                                                        Case "0"
                                                            failed2 = True
                                                            failureReason2 = permID
                                                        Case "-1"
                                                            failed2 = True
                                                            failureReason2 = permID
                                                    End Select
                                                Else
                                                    Dim allPerms2 As String() = perms.Split("~")
                                                    For Each perm As String In allPerms2
                                                        If perm <> String.Empty Then
                                                            Dim permName As String = perm.Split("|")(0)
                                                            Dim permActive As String = perm.Split("|")(1)
                                                            If permName = permission Then
                                                                If permActive <> "True" And permActive <> "true" And permActive <> "1" Then
                                                                    Dim permID As String = Role_Managemet.RTSQL.Retreive.GetPermID(permission)
                                                                    Select Case permID.Split("*")(0)
                                                                        Case "1"
                                                                            permID = permID.Remove(0, 2)
                                                                            Dim permActivated As String = Role_Managemet.RTSQL.Update.UpdateRolePermActive(roleID, permID, "1")
                                                                            Select Case permActivated.Split("*")(0)
                                                                                Case "1"

                                                                                Case "-1"
                                                                                    failed2 = True
                                                                                    failureReason2 = permID
                                                                            End Select
                                                                        Case "0"
                                                                            failed2 = True
                                                                            failureReason2 = permID
                                                                        Case "-1"
                                                                            failed2 = True
                                                                            failureReason2 = permID
                                                                    End Select
                                                                End If
                                                            End If
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        Next
                                        If failed2 = False Then
                                            Server.Listener.SendResponse(ClientSocket, "1*Success")
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, failureReason2)
                                        End If
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, failureReason)
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, perms)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, perms)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerUpdated)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVEROLE*"
                Try
                    Dim roleInUse As String = Role_Managemet.RTSQL.Retreive.CheckRoleInUse(ClientData)
                    Select Case roleInUse.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Role_Managemet.RTSQL.Delete.RemoveRole(ClientData))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, roleInUse)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, roleInUse)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TOGGLROLEACTIVE*"
                Try
                    Dim id As String = ClientData.Split("|")(0)
                    Dim active As String = ClientData.Split("|")(1)
                    Dim currentlyActive As Boolean = Convert.ToBoolean(active)
                    Select Case currentlyActive
                        Case True
                            Dim roleInUse As String = Role_Managemet.RTSQL.Retreive.CheckRoleInUse(id)
                            Select Case roleInUse.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, Role_Managemet.RTSQL.Update.DeactivateRole(ClientData.Split("|")(0)))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, roleInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, roleInUse)
                            End Select
                        Case False
                            Server.Listener.SendResponse(ClientSocket, Role_Managemet.RTSQL.Update.ActivateRole(ClientData.Split("|")(0)))
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "User Management"
            Case "*GETUSERS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.UI_GetAllUsers())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETACTIVEROLES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.UI_GetActiveRoles())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETEVOAGENTS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, User_Management.Evolution.Retreive.UI_GetEvoAgents())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETROLEPERMISSIONS*"
                Try
                    Dim roleName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.UI_GetRolePermsByName(roleName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDUSER*"
                Try
                    Dim name As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Dim pin As String = ClientData.Split("|")(2)
                    Dim password As String = ClientData.Split("|")(3)
                    Dim role As String = ClientData.Split("|")(4)
                    Dim hasAgent As String = ClientData.Split("|")(5)
                    Dim evoAgent As String = ClientData.Split("|")(6)
                    Dim checkUsername As String = User_Management.RTSQL.Retreive.UI_CheckUsername(username)
                    Select Case checkUsername.Split("*")(0)
                        Case "1"
                            Dim checkPin As String = User_Management.RTSQL.Retreive.UI_CheckUserPin(pin)
                            Select Case checkPin.Split("*")(0)
                                Case "1"
                                    Dim roleID As String = User_Management.RTSQL.Retreive.UI_GetRoleIDByName(role)
                                    Select Case roleID.Split("*")(0)
                                        Case "1"
                                            roleID = roleID.Remove(0, 2)
                                            Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Insert.UI_AddUser(name, username, pin, password, roleID, hasAgent, evoAgent))
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, roleID)
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, roleID)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, checkPin)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, checkPin)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, checkUsername)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, checkUsername)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVEUSER*"
                Try
                    Dim userID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Delete.UI_RemoveUser(userID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEUSER*"
                Try
                    Dim userID As String = ClientData.Split("|")(0)
                    Dim name As String = ClientData.Split("|")(1)
                    Dim userName As String = ClientData.Split("|")(2)
                    Dim pin As String = ClientData.Split("|")(3)
                    Dim password As String = ClientData.Split("|")(4)
                    Dim role As String = ClientData.Split("|")(5)
                    Dim hasAgent As String = ClientData.Split("|")(6)
                    Dim agent As String = ClientData.Split("|")(7)
                    Dim roleID As String = Role_Managemet.RTSQL.Retreive.UI_GetAddedRoleID(role)
                    Select Case roleID.Split("*")(0)
                        Case "1"
                            roleID = roleID.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Update.UI_UpdateUser(userID, name, userName, pin, password, roleID, hasAgent, agent))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, roleID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, roleID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TOGGLEUSERACTIVE*"
                Try
                    Dim userID As String = ClientData.Split("|")(0)
                    Dim currentlyActive As Boolean = Convert.ToBoolean(ClientData.Split("|")(1))
                    Select Case currentlyActive
                        Case True
                            Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Update.UI_DeactivateUser(userID))
                        Case False
                            Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Update.UI_ActivateUser(userID))
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Warehouse Transfers"

#Region "Warehouse Management"
            Case "*GETPROCESSLOCATIONS*"
                Try
                    Dim procName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWhseProcLookUp(procName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPROCESSLOCATIONSREC*"
                Try
                    Dim procName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWhseProcLookUpRec(procName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETALLEVOWHSESFORCONFIG*"
                Try
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWhseAllEvoWhses)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDPROCWHSEREF*"
                Try
                    Dim procName As String = ClientData.Split("|")(0)
                    Dim iwhseID As String = ClientData.Split("|")(1)
                    Dim isRec As Boolean = Convert.ToBoolean(ClientData.Split("|")(2))

                    Dim refExists As String = String.Empty
                    If isRec Then
                        refExists = WhseManagement.RTSQL.Retreive.UI_checkProcRefExistsRec(procName, iwhseID)
                    Else
                        refExists = WhseManagement.RTSQL.Retreive.UI_checkProcRefExists(procName, iwhseID)
                    End If
                    Select Case refExists.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef(procName, iwhseID, isRec))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, refExists)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVEPROCWHSEREF*"
                Try
                    Dim procName As String = ClientData.Split("|")(0)
                    Dim iwhseID As String = ClientData.Split("|")(1)
                    Dim isRec As Boolean = Convert.ToBoolean(ClientData.Split("|")(2))
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Delete.UI_deleteNewWhseCrossRef(procName, iwhseID, isRec))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
                'Pre wht rewrite
            Case "*GETPROCNAMES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWhtProcesses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETWHSELOOKUPSFORPROC*"
                Try
                    Dim procName As String = ClientData
                    Select Case procName
                        Case "PPtFS"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_PPtFS())
                        Case "FStMS"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_FStMS())
                        Case "MStZECT"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_MStZect())
                        Case "ZECT1"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Zect1())
                        Case "ZECT2"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Zect2())
                        Case "CAN"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Canning())
                        Case "AW"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_AW())
                        Case "ToProd"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_ToProd())
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETWAREHOUSESELECTED*"
                Try
                    Dim procName As String = ClientData.Split("|")(0)
                    Dim whseID As String = ClientData.Split("|")(1)
                    Dim selected As String = ClientData.Split("|")(2)
                    Select Case procName
                        Case "PPtFS"
                            Dim resetWareHouses As String = WhseManagement.RTSQL.Update.UI_resetWhseCrossRef_PPtFS()
                            Select Case resetWareHouses.Split("*")(0)
                                Case "1"
                                    Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_PPtFS(whseID)
                                    Select Case warehouseFound.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_PPtFS(whseID, selected))
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_PPtFS(whseID, selected))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, warehouseFound)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, resetWareHouses)
                            End Select

                        Case "FStMS"
                            Dim resetWareHouses As String = WhseManagement.RTSQL.Update.UI_resetWhseCrossRef_FStMS()
                            Select Case resetWareHouses.Split("*")(0)
                                Case "1"
                                    Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_FStMS(whseID)
                                    Select Case warehouseFound.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__FStMS(whseID, selected))
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_FStMS(whseID, selected))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, warehouseFound)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, resetWareHouses)
                            End Select
                        Case "MStZECT"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_MStZect(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__MStZectS(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_MStZect(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "ZECT1"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Zect1(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__Zect1(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Zect1(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "ZECT2"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Zect2(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__Zect2(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Zect2(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "CAN"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Can(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__Canning(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Canning(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "AW"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_AW(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef__AW(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_AW(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "ToProd"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_ToProd(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_ToProd(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_ToProd(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Warehouse Transfer Management (Receiving)"
            Case "*GETPROCNAMESREC*"
                Try
                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWhtProcessesRec())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETWHSELOOKUPSFORPROCREC*"
                Try
                    Dim procName As String = ClientData
                    Select Case procName
                        Case "PPtFS"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_PP_Rec())
                        Case "FStMS"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_FS_Rec())
                        Case "MStZECT"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_MS_Rec())
                        Case "ZECT1"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Zect1_Rec())
                        Case "ZECT2"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Zect2_Rec())
                        Case "CAN"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_Can_Rec())
                        Case "AW"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_AW_Rec())
                        Case "PGM"
                            Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Retreive.UI_getWarehouseLookUp_PGM_Rec())
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETWAREHOUSESELECTEDREC*"
                Try
                    Dim procName As String = ClientData.Split("|")(0)
                    Dim whseID As String = ClientData.Split("|")(1)
                    Dim selected As String = ClientData.Split("|")(2)
                    Select Case procName
                        Case "PPtFS"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_PP_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_PP_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_PP_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "FStMS"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_FS_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_FS_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_FS_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "MStZECT"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_MS_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_MS_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_MS_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "ZECT1"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Zect1_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_Zect1_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Zect1_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "ZECT2"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Zect2_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_Zect2_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Zect2_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "CAN"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_Can_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_Can_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_Can_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "AW"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_AW_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_AW_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_AW_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case "PGM"
                            Dim warehouseFound As String = WhseManagement.RTSQL.Retreive.UI_getWarehouseExistes_PGM_Rec(whseID)
                            Select Case warehouseFound.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Update.UI_updateWhseCrossRef_PGM_Rec(whseID, selected))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, WhseManagement.RTSQL.Insert.UI_addNewWhseCrossRef_PGM_Rec(whseID, selected))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, warehouseFound)
                            End Select
                        Case Else
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Warehouse Transfer Managemet"
            Case "*GETALLTRANSFERPROCESSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtProcesses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETWHTTRANSFERLINES*"
                Try
                    Dim status As String = ClientData.Split("|")(0)
                    Dim process As String = ClientData.Split("|")(1)
                    Dim rows As String = ClientData.Split("|")(2)
                    Dim transferredDate As String = String.Empty
                    Dim failedDate As String = String.Empty
                    Dim charIndex As Integer = 3
                    Dim transferredStart As String = ""
                    Dim transferredEnd As String = ""
                    Dim failedStart As String = ""
                    Dim failedEnd As String = ""

                    If ClientData.Split("|").Length = 5 Then
                        Select Case ClientData.Split("|")(charIndex).Substring(0, 1)
                            Case "t"
                                transferredStart = ClientData.Split("|")(3).Replace("t", "")
                                transferredEnd = ClientData.Split("|")(4).Replace("f", "")
                                'transferredDate = String.Format("AND [dtDateTransfered] BETWEEN '{0}' AND '{1}'", transferredStartDate.Replace("t", ""), transferredEndDate)
                            Case "f"
                                failedStart = ClientData.Split("|")(3).Replace("f", "")
                                failedEnd = ClientData.Split("|")(4).Replace("f", "")
                                'failedDate = String.Format("AND [dtDateFailed] BETWEEN '{0}' AND '{1}'", failedStartDate.Replace("f", ""), failedEndDate)
                        End Select
                    End If

                    If ClientData.Split("|").Length = 7 Then
                        For index = 1 To ClientData.Split("|").Length - 3
                            Select Case ClientData.Split("|")(charIndex).Substring(0, 1)
                                Case "t"
                                    transferredStart = ClientData.Split("|")(3).Replace("t", "").Replace("t", "")
                                    transferredEnd = ClientData.Split("|")(4).Replace("t", "").Replace("t", "")
                                    'transferredDate = String.Format("AND [dtDateTransfered] BETWEEN '{0}' AND '{1}'", transferredStartDate.Replace("t", ""), transferredEndDate)
                                Case "f"
                                    failedStart = ClientData.Split("|")(5).Replace("f", "")
                                    failedEnd = ClientData.Split("|")(6).Replace("f", "")
                                    'failedDate = String.Format("AND [dtDateFailed] BETWEEN '{0}' AND '{1}'", failedStartDate.Replace("f", ""), failedEndDate)
                            End Select
                            charIndex = charIndex + 1
                        Next
                    End If

                    'Dim tStart As Date = ""
                    'Dim tEnd As Date = ""
                    'Dim fStart As Date = ""
                    'Dim fEnd As Date = ""

                    'If transferredStart IsNot String.Empty Then
                    '    tStart = Convert.ToDateTime(transferredStart)
                    '    tEnd = Convert.ToDateTime(transferredEnd)
                    'End If

                    'If failedStart IsNot String.Empty Then
                    '    tStart = Convert.ToDateTime(failedStart)
                    '    tEnd = Convert.ToDateTime(failedEnd)
                    'End If

                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtLInes(status, process, rows, transferredStart, transferredEnd, failedStart, failedEnd))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETWHTRANSFERLINESALL*"
                Try
                    Dim process As String = ClientData.Split("|")(0)
                    Dim rows As String = ClientData.Split("|")(1)
                    Dim transferredDate As String = String.Empty
                    'Dim failedDate As String = String.Empty
                    Dim charIndex As Integer = 2
                    Dim transferredStartDate As String = ""
                    Dim transferredEndDate As String = ""

                    If ClientData.Split("|").Length = 4 Then
                        Select Case ClientData.Split("|")(charIndex).Substring(0, 1)
                            Case "t"
                                transferredStartDate = ClientData.Split("|")(2).Replace("t", "")
                                transferredEndDate = ClientData.Split("|")(3).Replace("t", "")
                                'transferredDate = String.Format("WHERE [dtDateTransfered] BETWEEN '{0}' AND '{1}'", transferredStartDate.Replace("t", ""), transferredEndDate.Replace("t", ""))
                        End Select
                    End If
                    Dim appendOperator As String = ""

                    'If transferredDate = "" Then
                    '    appendOperator = "WHERE"
                    'Else
                    '    appendOperator = "AND"
                    'End If

                    'process = String.Format(" {0} pr.[vProcName] LIKE '%{1}%'", appendOperator, process)
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtLInesAll(process, rows, transferredStartDate, transferredEndDate))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETWHTRANSFERLINESPOSTED*"
                Try
                    Dim process As String = ClientData.Split("|")(0)
                    Dim rows As String = ClientData.Split("|")(1)
                    Dim transferredDate As String = String.Empty
                    Dim failedDate As String = String.Empty
                    Dim charIndex As Integer = 2
                    Dim transferredStartDate As String = ""
                    Dim transferredEndDate As String = ""

                    If ClientData.Split("|").Length = 4 Then
                        Select Case ClientData.Split("|")(charIndex).Substring(0, 1)
                            Case "t"
                                transferredStartDate = ClientData.Split("|")(2).Replace("t", "")
                                transferredEndDate = ClientData.Split("|")(3).Replace("t", "")
                                'transferredDate = String.Format("WHERE [dtDateTransfered] BETWEEN '{0}' AND '{1}'", transferredStartDate.Replace("t", ""), transferredEndDate)
                        End Select
                    End If

                    Dim appendOperator As String = ""

                    If transferredDate = "" Then
                        appendOperator = "WHERE"
                    Else
                        appendOperator = "AND"
                    End If

                    'process = String.Format(" {0} pr.[vProcName] LIKE '%{1}%'", appendOperator, process)
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtLInesPosted(process, rows, transferredStartDate, transferredEndDate))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEWHSETRANSFERLINE*"
                Try
                    Dim lotNum As String = ClientData.Split("|")(0)
                    Dim qty As String = ClientData.Split("|")(1)
                    Dim status As String = ClientData.Split("|")(2)
                    Dim lineID As String = ClientData.Split("|")(3)
                    If status = "Pending" Then
                        Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Update.SVC_UpdateWhseTransferLinePending(lotNum, qty, status, lineID))
                    Else
                        Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Update.SVC_UpdateWhseTransferLine(lotNum, qty, status, lineID))
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*WHTSETFAILEDTOPENDING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Update.SVC_UpdateWhseTransfersFailedToPending())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PROCESSPENDINGWHTS*"
                Try
                    Dim failedMain As Boolean = False
                    Dim failureReasonMain As String = String.Empty

                    Dim WhseTransferLines As String = WhseTransfers.RTSQL.Retreive.SVC_GetPendingWhseTransfers()
                    Select Case WhseTransferLines.Split("*")(0)
                        Case "1"
                            WhseTransferLines = WhseTransferLines.Remove(0, 2)

                            Dim allWhseTransfers As String() = WhseTransferLines.Split("~")
                            For Each whseTransfer As String In allWhseTransfers
                                If whseTransfer <> String.Empty Then

                                    Dim lineID As String = whseTransfer.Split("|")(0)
                                    Dim code As String = whseTransfer.Split("|")(1)
                                    Dim lotNumber As String = whseTransfer.Split("|")(2)
                                    Dim whseFrom As String = whseTransfer.Split("|")(3)
                                    Dim whseTo As String = whseTransfer.Split("|")(4)
                                    Dim qty As String = whseTransfer.Split("|")(5)
                                    Dim username As String = whseTransfer.Split("|")(6)
                                    Dim proccess As String = whseTransfer.Split("|")(7)
                                    Dim transDesc As String = whseTransfer.Split("|")(8)
                                    Dim transDate As String = whseTransfer.Split("|")(9)

                                    Dim transferred As String = WhseTransfers.EvolutionSDK.CTransferItem("RTIS Auto transfer", whseFrom, whseTo, code, lotNumber, qty)
                                    Select Case transferred.Split("*")(0)
                                        Case "1"
                                            Dim completedInserted As String = WhseTransfers.RTSQL.Insert.SVC_InsertWHTLineCompleted(lineID, code, lotNumber, whseFrom, whseTo, qty, transDate, username, proccess, transDesc)
                                            Select Case completedInserted.Split("*")(0)
                                                Case "1"
                                                    Dim lineRemoved As String = WhseTransfers.RTSQL.Delete.SVC_DeleteWhseTransLineComplete(lineID)
                                                    Select Case lineRemoved.Split("*")(0)
                                                        Case "1"

                                                        Case "-1"
                                                            failedMain = True
                                                            failureReasonMain = lineRemoved
                                                            Exit For
                                                    End Select
                                                Case "-1"
                                                    failedMain = True
                                                    failureReasonMain = completedInserted
                                                    Exit For
                                            End Select
                                        Case "-1"
                                            Dim failureReason As String = transferred.Split("*")(1)
                                            Dim failureUpdated As String = WhseTransfers.RTSQL.Update.SVC_UpdateWhseTransferFailed(lineID, failureReason)
                                            Select Case failureUpdated.Split("*")(0)
                                                Case "1"
                                            'Line set as failed
                                                Case "-1"
                                                    failedMain = True
                                                    failureReasonMain = failureUpdated
                                                    Exit For
                                            End Select
                                    End Select
                                End If
                            Next


                            If failedMain = False Then
                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                            Else
                                Server.Listener.SendResponse(ClientSocket, failureReasonMain)
                            End If
                        Case "0"
                            'No Lines Found
                        Case "-1"
                            failedMain = True
                            failureReasonMain = WhseTransferLines
                        Case Else
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETFGWHTREQUESTS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0)
                    Dim dateTo As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtFGRequests(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*POSTFGWHT*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim userName As String = ClientData.Split("|")(1)
                    Dim whtInfo As String = WhseTransfers.RTSQL.Retreive.UI_GetFGTransferInfo(lineID)
                    Select Case whtInfo.Split("*")(0)
                        Case "1"
                            whtInfo = whtInfo.Remove(0, 2)
                            Dim itemCode As String = whtInfo.Split("|")(0)
                            Dim lot As String = whtInfo.Split("|")(1)
                            Dim whseFrom As String = whtInfo.Split("|")(2)
                            Dim whseTo As String = whtInfo.Split("|")(3)
                            Dim qty As String = whtInfo.Split("|")(4)
                            Dim dtEntered As String = whtInfo.Split("|")(5)
                            Dim process As String = whtInfo.Split("|")(6)
                            Dim userRequested As String = whtInfo.Split("|")(7)
                            Dim transferred As String = WhseTransfers.EvolutionSDK.CTransferItem("RTIS FG Transfer", whseFrom, whseTo, itemCode, lot, qty)
                            Select Case transferred.Split("*")(0)
                                Case "1"
                                    Dim completedInsert As String = WhseTransfers.RTSQL.Insert.SVC_InsertFGWHTLineCompleted(lineID, itemCode, lot, whseFrom, whseTo, qty, dtEntered, userRequested, userName, process, "Posted")
                                    Select Case completedInsert.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Delete.UI_DeleteFGWhseTransLineComplete(lineID))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, transferred)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, whtInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETFGWHTREPORTS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0)
                    Dim dateTo As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.RTSQL.Retreive.UI_getWhtFGReports(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Purchase Orders"

#Region "CMS"

#Region "Administration"
            Case "*ADDCMSVALUE*"
                Try
                    Dim value As String = ClientData.Split("|")(0)

                    Dim valueType As String = ClientData.Split("|")(1)
                    Dim valueFound As String = POReceiving.RTSQL.Retreive.UI_CheckCMSValue(value, valueType)
                    Select Case valueFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Insert.UI_AddCMSRecord(value, valueType))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, valueFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSITEMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetCMSItems())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSUOMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetCMSUOMs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*DELETECMSITEM*"
                Try
                    Dim id As Integer = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Delete.UI_CMSItem(id))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Management"
            Case "*GETITEMCMSHEADERS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSHeaders)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSITEMSADD*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetCMSItems_Add)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSUOMSADD*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetCMSUOMs_Add)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDCMSDOCUMENT*"
                Try
                    Dim headerInfo As String = ClientData.Split("*")(0)
                    Dim stockID As String = headerInfo.Split("|")(0)
                    Dim code As String = headerInfo.Split("|")(1)
                    Dim userName As String = headerInfo.Split("|")(2)
                    Dim lineInfo As String = ClientData.Split("*")(1)
                    Dim headerCreated As String = POReceiving.RTSQL.Insert.UI_AddCMSDocHeader(stockID, code, userName, "1")
                    Select Case headerCreated.Split("*")(0)
                        Case "1"
                            Dim headerID = headerCreated.Remove(0, 2)
                            Dim query As String = "INSERT INTO [COA].[ltbl_CMS_Docs] ([iHeaderID], [vItem], [vUnit], [vOperator], [dValue1], [dValue2], [vInspection]) VALUES "
                            For Each l As String In lineInfo.Split("~")
                                If l <> String.Empty Then
                                    query += $"('{headerID}', '{l.Split("|")(0)}','{l.Split("|")(1)}','{l.Split("|")(2)}','{l.Split("|")(3)}', '{l.Split("|")(4)}', '{l.Split("|")(5)}'),"
                                End If
                            Next
                            query = query.Substring(0, query.Length - 1)
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.ExecuteQuery(query))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerCreated)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETITEMCMSAPPROVALS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSApprovals)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSAPPROVALLINES*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSApprovalLines(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*APPROVECMSDOCUMENT*"
                Try
                    Dim itemCode = ClientData.split("|")(0)
                    Dim username As String = ClientData.split("|")(1)
                    Dim lineID As String = ClientData.split("|")(2)
                    Dim stockLink As String = ClientData.Split("|")(3)
                    Dim version As String = ClientData.Split("|")(4)

                    Dim sign As Image = New Bitmap(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + itemCode + ".png")
                    Dim memStrem = New MemoryStream()
                    sign.Save(memStrem, System.Drawing.Imaging.ImageFormat.Png)
                    Dim imageByteArry() As Byte = memStrem.ToArray()
                    Dim appoverd As String = POReceiving.RTSQL.Update.UI_UpdateCMSApproved(lineID, imageByteArry, username)
                    Select Case appoverd.Split("*")(0)
                        Case "1"
                            Dim headers As String = POReceiving.RTSQL.Retreive.UI_GetCMSHeadersToArchive(stockLink, version)
                            Select Case headers.Split("*")(0)
                                Case "1"
                                    headers = headers.Remove(0, 2)
                                    Dim headerQuery As String = String.Empty
                                    Dim linesQuery As String = String.Empty
                                    Dim delineQuery As String = String.Empty
                                    Dim deheadQuery As String = String.Empty
                                    Dim allHeaders As String() = headers.Split("~")
                                    For Each headID As String In allHeaders
                                        If headID <> String.Empty Then
                                            headerQuery += $"INSERT INTO [COA].[htbl_Archive_CMS_Docs]
                                                            SELECT *
                                                            FROM [COA].[htbl_CMS_Docs]
                                                            WHERE [iLineID] = {headID}" + Environment.NewLine

                                            linesQuery += $"INSERT INTO [COA].[ltbl_Archive_CMS_Docs]
                                                        SELECT [iHeaderID], [vItem], [vUnit], [vOperator],  [dValue1], [dValue2],[vInspection]
                                                        FROM [COA].[ltbl_CMS_Docs]
                                                        WHERE [iHeaderID] = {headID}" + Environment.NewLine

                                            deheadQuery += $"DELETE FROM [COA].[htbl_CMS_Docs] WHERE [iLineID] = {headID}" + Environment.NewLine
                                            delineQuery += $"DELETE FROM [COA].[ltbl_CMS_Docs] WHERE [iHeaderID] = {headID}" + Environment.NewLine
                                        End If
                                    Next

                                    Dim headersInserted As String = POReceiving.RTSQL.ExecuteQuery(headerQuery)
                                    Select Case headersInserted.Split("*")(0)
                                        Case "1"
                                            Dim linesInserted As String = POReceiving.RTSQL.ExecuteQuery(linesQuery)
                                            Select Case linesInserted.Split("*")(0)
                                                Case "1"
                                                    Dim headersDeleted As String = POReceiving.RTSQL.ExecuteQuery(deheadQuery)
                                                    Select Case headersDeleted.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.ExecuteQuery(delineQuery))
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, headersDeleted)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, linesInserted)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, headersInserted)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, headers)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, appoverd)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSAPPROVALIMAGE*"
                Try
                    Dim itemCode As String = ClientData
                    Dim imageeFound As String = POReceiving.RTSQL.Retreive.UI_GetCMSApprovalImagee(itemCode)
                    Server.Listener.SendResponseFile(ClientSocket, "1*Success", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + itemCode + ".png")
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSAPPROVALLINESVIEW*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSApprovalLinesViww(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REJECTCMSDOCUMENT*"
                Try
                    Dim lineID As String = ClientData.split("|")(0)
                    Dim reasons As String = ClientData.Split("|")(1)
                    Dim username As String = ClientData.split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Update.UI_UpdateCMSRejected(lineID, reasons, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSEDITLINES*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSApprovalLinesEdit(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CREATENEWDOCVERSION*"
                Try
                    Dim headerInfo As String = ClientData.Split("*")(0)
                    Dim stockID As String = headerInfo.Split("|")(0)
                    Dim code As String = headerInfo.Split("|")(1)
                    Dim userName As String = headerInfo.Split("|")(2)
                    Dim docVersion As Int32 = Convert.ToInt32(headerInfo.Split("|")(3))
                    Dim lineInfo As String = ClientData.Split("*")(1)
                    Dim headerCreated As String = POReceiving.RTSQL.Insert.UI_AddCMSDocHeader(stockID, code, userName, Convert.ToString(docVersion + 1))
                    Select Case headerCreated.Split("*")(0)
                        Case "1"
                            Dim headerID = headerCreated.Remove(0, 2)
                            Dim query As String = "INSERT INTO [COA].[ltbl_CMS_Docs] ([iHeaderID], [vItem], [vUnit], [vOperator], [dValue1], [dValue2], [vInspection]) VALUES "
                            For Each l As String In lineInfo.Split("~")
                                If l <> String.Empty Then
                                    query += $"('{headerID}', '{l.Split("|")(0)}','{l.Split("|")(1)}','{l.Split("|")(2)}','{l.Split("|")(3)}', '{l.Split("|")(4)}', '{l.Split("|")(5)}'),"
                                End If
                            Next
                            query = query.Substring(0, query.Length - 1)
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.ExecuteQuery(query))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerCreated)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*EDITEXISTINGDOCUMENT*"
                Try
                    Dim headerID As String = ClientData.Split("*")(0)
                    Dim lineInfo As String = ClientData.Split("*")(1)
                    Dim headerUpdated As String = POReceiving.RTSQL.Update.UI_UpdateCMSEdited(headerID)
                    Select Case headerUpdated.Split("*")(0)
                        Case "1"
                            Dim linesDeleted As String = POReceiving.RTSQL.Delete.UI_DeleteCMSDocLines(headerID)
                            Select Case linesDeleted.Split("*")(0)
                                Case "1"
                                    Dim query As String = "INSERT INTO [COA].[ltbl_CMS_Docs] ([iHeaderID], [vItem], [vUnit], [vOperator], [dValue1], [dValue2], [vInspection]) VALUES "
                                    For Each l As String In lineInfo.Split("~")
                                        If l <> String.Empty Then
                                            query += $"('{headerID}', '{l.Split("|")(0)}','{l.Split("|")(1)}','{l.Split("|")(2)}','{l.Split("|")(3)}', '{l.Split("|")(4)}', '{l.Split("|")(5)}'),"
                                        End If
                                    Next
                                    query = query.Substring(0, query.Length - 1)
                                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.ExecuteQuery(query))
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, linesDeleted)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerUpdated)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*DELETECMSDOCUMENT*"
                Try
                    Dim headerID As String = ClientData
                    Dim hDeleted As String = POReceiving.RTSQL.Delete.UI_DeleteCMSHeader(headerID)
                    Select Case hDeleted.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Delete.UI_DeleteCMSLines(headerID))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, hDeleted)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Archive"
            Case "*GETCMSARCHIVEHEADERS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetCMSArchiveHeaders)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSARCHIVELINES*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetItemCMSArchiveLines(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCMSARCHIVEIMAGE*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim imageeFound As String = POReceiving.RTSQL.Retreive.UI_GetCMSArchiveImage(lineID, itemCode)
                    Server.Listener.SendResponseFile(ClientSocket, "1*Success", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + itemCode + ".png")
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region
#End Region

#Region "PO Admin"
            Case "*GETEVOPOVENDORS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.Evolution.Retreive.UI_GetEvoPOVendors())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETVENDORLOOKUP*"
                Try
                    Dim vendorID As String = ClientData.Split("|")(0)
                    Dim vendorName As String = ClientData.Split("|")(1)
                    Dim viewable As String = ClientData.Split("|")(2)
                    Dim vendorFound As String = POReceiving.RTSQL.Retreive.UI_CheckRTVendor(vendorID)
                    Select Case vendorFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Update.UI_UpdateVendorLookup(vendorID, viewable))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Insert.UI_AddVendorLookup(vendorID, vendorName, viewable))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, vendorFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPOVENDORLINKS*"
                Try
                    Dim linkList As String = POReceiving.RTSQL.Retreive.UI_GetVendorPOLinks()
                    Select Case linkList.Split("*")(0)
                        Case "1"
                            linkList = linkList.Remove(0, 2)

                            Dim failed As Boolean = False
                            Dim failureReason As String = String.Empty
                            Dim allLinks As String() = linkList.Split("~")
                            Dim returnString As String = String.Empty
                            For Each link As String In allLinks
                                If link <> String.Empty Then
                                    Dim VendorID As String = link.Split("|")(0)
                                    Dim linkName As String = link.Split("|")(1)
                                    Dim linkPO As String = link.Split("|")(2)
                                    Dim linkDT As String = link.Split("|")(3)
                                    Dim linkPOs As String = POReceiving.Evolution.Retreive.UI_GetVendorPOs(VendorID)
                                    Select Case linkPOs.Split("*")(0)
                                        Case "1"
                                            linkPOs = linkPOs.Remove(0, 2)
                                            Dim allInfo As String = VendorID + "|" + linkName + "|" + linkPO + "|" + linkDT + "|" + linkPOs
                                            returnString += allInfo + "*"
                                        Case "0"
                                            failed = True
                                            failureReason = linkList
                                            Exit For
                                        Case "-1"
                                            failed = True
                                            failureReason = linkList
                                            Exit For
                                    End Select
                                End If
                            Next

                            If failed = False Then
                                Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                            Else
                                Server.Listener.SendResponse(ClientSocket, failureReason)
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, linkList)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, linkList)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETVENDORPOLINK*"
                Try
                    Dim vendorID As String = ClientData.Split("|")(0)
                    Dim supName As String = ClientData.Split("|")(1)
                    Dim orderNo As String = ClientData.Split("|")(2)
                    Dim refFound As String = POReceiving.RTSQL.Retreive.UI_CheckVendorPOLink(vendorID)
                    Select Case refFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Update.UI_UpdateVendorPOLink(vendorID, supName, orderNo))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Insert.UI_AddVendorPOLink(vendorID, supName, orderNo))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, refFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "PO Receiving"
            Case "*GETLINKEDVENDORS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetLinkedVendors())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETLINKEDPOS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Retreive.UI_GetLinkedPOs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETVENDORPOLINES*"
                Try
                    Dim vendorName As String = ClientData
                    Dim vendorPO As String = POReceiving.RTSQL.Retreive.UI_GetVendorPO(vendorName)
                    Select Case vendorPO.Split("*")(0)
                        Case "1"
                            vendorPO = vendorPO.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, POReceiving.Evolution.Retreive.UI_GetPOLinesNew(vendorPO, vendorName))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, vendorPO)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, vendorPO)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPOLINES*"
                Try
                    Dim orderNum As String = ClientData
                    Dim POVendor As String = POReceiving.RTSQL.Retreive.UI_GetPOVendor(orderNum)
                    Select Case POVendor.Split("*")(0)
                        Case "1"
                            POVendor = POVendor.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, POReceiving.Evolution.Retreive.UI_GetPOLinesNew(orderNum, POVendor))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, POVendor)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, POVendor)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PRINTPOLABELSAUTO*"
                Try
                    Dim OrderNO As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNum As String = ClientData.Split("|")(2)
                    Dim PrintQty As String = ClientData.Split("|")(3)
                    Dim userName As String = ClientData.Split("|")(4)
                    'Dim labelQty As String = ClientData.Split("|")(5)
                    Dim qtyPerLabel As String = ClientData.Split("|")(5)

                    Dim remQty As Double = Convert.ToDouble(PrintQty.Replace(",", sep).Replace(".", sep))
                    Dim qtyToPrint As Double = Convert.ToDouble(qtyPerLabel.Replace(",", sep).Replace(".", sep))
                    Dim exp = String.Empty
                    'Dim iLblQty As Integer = Convert.ToInt32(labelQty)

                    Dim returnString = String.Empty
                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty
                    While remQty >= qtyToPrint
                        Dim unqBar As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                        Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + exp.PadRight(6, " ") + "(10)" + lotNum.PadRight(20, " ") + "(30)" + qtyPerLabel.PadLeft(8, "0") + "(90)" + unqBar
                        Dim inserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, OrderNO)
                        Select Case inserted.Split("*")(0)
                            Case "1"
                                returnString += Barcode + "~"
                                remQty = remQty - qtyToPrint
                            Case "0"
                                failed = True
                                failureReason = inserted
                                Exit While
                            Case "-1"
                                failed = True
                                failureReason = inserted
                                Exit While
                        End Select
                        Thread.Sleep(1000)
                    End While

                    If failed = False Then
                        If remQty <> 0 Then
                            Dim unqLast As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                            Dim BarcodeLast = "(240)" + itemCode.PadRight(25, " ") + "(15)" + exp.PadRight(6, " ") + "(10)" + lotNum.PadRight(20, " ") + "(30)" + Convert.ToString(remQty).PadLeft(8, "0") + "(90)" + unqLast
                            Dim lastInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(BarcodeLast, OrderNO)
                            Select Case lastInserted.Split("*")(0)
                                Case "1"
                                    returnString += BarcodeLast
                                    Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, lastInserted)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, lastInserted)
                            End Select
                        Else
                            Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                        End If
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PRINTPOLABELSNOLOTAUTO*"
                Try
                    Dim OrderNO As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim PrintQty As String = ClientData.Split("|")(2)
                    Dim userName As String = ClientData.Split("|")(3)
                    Dim qtyPerLabel As String = ClientData.Split("|")(4)

                    Dim remQty As Double = Convert.ToDouble(PrintQty.Replace(",", sep).Replace(".", sep))
                    Dim qtyToPrint As Double = Convert.ToDouble(qtyPerLabel.Replace(",", sep).Replace(".", sep))

                    Dim returnString = String.Empty
                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty
                    While remQty >= qtyToPrint
                        Dim unqBar As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                        Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + "#NOLOT#".PadRight(20, " ") + "(30)" + qtyPerLabel.PadLeft(8, "0") + "(90)" + unqBar
                        Dim inserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, OrderNO)
                        Select Case inserted.Split("*")(0)
                            Case "1"
                                returnString += Barcode + "~"
                                remQty = remQty - qtyToPrint
                            Case "0"
                                failed = True
                                failureReason = inserted
                                Exit While
                            Case "-1"
                                failed = True
                                failureReason = inserted
                                Exit While
                        End Select
                        Thread.Sleep(1000)
                    End While

                    If failed = False Then
                        If remQty <> 0 Then
                            Dim unqLast As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                            Dim BarcodeLast = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + "#NOLOT#".PadRight(20, " ") + "(30)" + Convert.ToString(remQty).PadLeft(8, "0") + "(90)" + unqLast
                            Dim lastInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(BarcodeLast, OrderNO)
                            Select Case lastInserted.Split("*")(0)
                                Case "1"
                                    returnString += BarcodeLast
                                    Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, lastInserted)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, lastInserted)
                            End Select
                        Else
                            Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                        End If
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REPRINTPOLABELSLOT*"
                Try
                    Dim OrderNO As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNum As String = ClientData.Split("|")(2)
                    Dim PrintQty As String = ClientData.Split("|")(3)
                    Dim userName As String = ClientData.Split("|")(4)
                    Dim qtyPerLabel As String = ClientData.Split("|")(5)

                    Dim iPrintQty As Integer = Convert.ToInt32(PrintQty)
                    Dim exp = String.Empty

                    Dim returnString = String.Empty
                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty
                    For index = 1 To iPrintQty
                        Dim unqBar As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                        Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNum.PadRight(20, " ") + "(30)" + qtyPerLabel.PadLeft(8, "0") + "(90)" + unqBar
                        Dim inserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, OrderNO)
                        Select Case inserted.Split("*")(0)
                            Case "1"
                                returnString += Barcode + "~"
                            Case "0"
                                failed = True
                                failureReason = inserted
                                Exit For
                            Case "-1"
                                failed = True
                                failureReason = inserted
                                Exit For
                        End Select
                        Thread.Sleep(1000)
                    Next

                    If failed = False Then
                        Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REPRINTPOLABELSNOLOT*"
                Try
                    Dim OrderNO As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim PrintQty As String = ClientData.Split("|")(2)
                    Dim userName As String = ClientData.Split("|")(3)
                    Dim qtyPerLabel As String = ClientData.Split("|")(4)

                    Dim iPrintQty As Integer = Convert.ToInt32(PrintQty)
                    Dim exp = String.Empty

                    Dim returnString = String.Empty
                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty
                    For index = 1 To iPrintQty
                        Dim unqBar As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                        Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + "#NOLOT#".PadRight(20, " ") + "(30)" + qtyPerLabel.PadLeft(8, "0") + "(90)" + unqBar
                        Dim inserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, OrderNO)
                        Select Case inserted.Split("*")(0)
                            Case "1"
                                returnString += Barcode + "~"
                            Case "0"
                                failed = True
                                failureReason = inserted
                                Exit For
                            Case "-1"
                                failed = True
                                failureReason = inserted
                                Exit For
                        End Select
                        Thread.Sleep(1000)
                    Next

                    If failed = False Then
                        Server.Listener.SendResponse(ClientSocket, "1*" + returnString)
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPOLABELINFO*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.Evolution.Retreive.UI_GetStockLabelInfo(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVEINVALIDLABELS*"
                Try
                    Dim orderNum As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Delete.UI_DeleteInvalidLabels(orderNum))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*POSTPOLINES*"
                Try
                    Dim orderNum As String = ClientData.Split("*")(0)
                    Dim query As String = ClientData.Split("*")(1)
                    Dim allLines As String = ClientData.Split("*")(2)
                    Dim deleted As String = POReceiving.RTSQL.Delete.UI_DeletePOLines(orderNum)
                    Select Case deleted.Split("*")(0)
                        Case "1"
                            Dim rtLogged As String = POReceiving.RTSQL.Insert.UI_RTLogPO(query)
                            Select Case rtLogged.Split("*")(0)
                                Case "1"
                                    Dim processPoLines As String = POReceiving.EvolutionSDK.UI_ProccessPOLines(orderNum, allLines)
                                    Select Case processPoLines.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Update.UI_ValidateLabels(orderNum))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, processPoLines)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, rtLogged)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, deleted)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, deleted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*POSTPOLINESNEW*"
                Try
                    Dim orderNum As String = ClientData.Split("*")(0)
                    Dim allLines As String() = ClientData.Split("*")(1).Split("~")
                    Dim deleted As String = POReceiving.RTSQL.Delete.UI_DeletePOLines(orderNum)
                    Select Case deleted.Split("*")(0)
                        Case "1"
                            Dim query As String = "INSERT INTO [tblPOLines] ([vOrderNum]     ,[vItemCode], [vLotNumber]  , [dOrderQty]  , [dRecQty]   , [dPrintQty]  , [bValidated]    , [bScanned], [dtDateEntered], [vUserEntered]) VALUES"
                            For Each line As String In allLines
                                'orderNum + "|" +  code + "|" + lot + "|" + orderQty + "|" + recQty + "|" + printQty + "|" + validated + "|" + scanned + "|" + userName + "~";
                                If line <> String.Empty Then
                                    Dim order As String = line.Split("|")(0)
                                    Dim code As String = line.Split("|")(1)
                                    Dim lot As String = line.Split("|")(2)
                                    Dim orderQty As String = line.Split("|")(3)
                                    Dim recQty As String = line.Split("|")(4)
                                    Dim printQty As String = line.Split("|")(5)
                                    Dim validated As String = line.Split("|")(6)
                                    Dim scanned As String = line.Split("|")(7)
                                    Dim userName As String = line.Split("|")(8)
                                    query += " ('" + orderNum + "'  ,'" + code + "', '" + lot + "','" + orderQty.Replace(",", ".") + "', '" + recQty.Replace(",", ".") + "','" + printQty.Replace(",", ".") + "','" + validated + "','" + scanned + "', GETDATE(), '" + userName + "'),"
                                End If
                            Next

                            query = query.Substring(0, query.Length - 1)

                            Dim rtLogged As String = POReceiving.RTSQL.Insert.UI_RTLogPO(query)
                            Select Case rtLogged.Split("*")(0)
                                Case "1"
                                    Dim processPoLines As String = POReceiving.EvolutionSDK.UI_ProccessPOLinesNew(orderNum, allLines)
                                    Select Case processPoLines.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, POReceiving.RTSQL.Update.UI_ValidateLabels(orderNum))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, processPoLines)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, rtLogged)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, deleted)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, deleted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try


#End Region

#End Region

#Region "PGM"

#Region "PGM Planning"

            Case "*GETVWPGMPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Retrieve.UI_GetVWPGMPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTOYOTAFSPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Retrieve.UI_GetTOYOTAFSPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTOYOTAPPPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Retrieve.UI_GetTOYOTAPPPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTOYOTAAWPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Retrieve.UI_GetTOYOTAAWPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

            Case "*GETSELECTCSPPGMPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.Evolution.Retrieve.UI_GetSelectCSPPGMPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSELECTPGMPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.Evolution.Retrieve.UI_GetSelectPGMPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTVWPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Insert.UI_InsertVWPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTTFSPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Insert.UI_InsertTFSPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTTPPPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Insert.UI_InsertTPPPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTTAWPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Insert.UI_InsertTAWPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEVWPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Update.UI_UpdateVWPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATETFSPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Update.UI_UpdateTFSPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATETPPPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Update.UI_UpdateTPPPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATETAWPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMPlanning.RTSQL.Update.UI_UpdateTAWPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "PGM Manufacture"
            Case "*GETPGMHEADERSTOMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMManufactureHeaders())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMLINESTOMANUFACTURE*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Dim concentration As String = ClientData.Split("|")(3)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMManufactureContainers(itemCode, lotNumber, location, concentration))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREPGMCONTAINER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Dim concentration As String = ClientData.Split("|")(3)
                    Dim container As String = ClientData.Split("|")(4)
                    Dim qty As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)

                    Dim headerID As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, location)
                    Select Case headerID.Split("*")(0)
                        Case "1"
                            headerID = headerID.Remove(0, 2)
                            'Dim manufInserted As String = PGM.Evolutiom.Insert.UI_InsertPGmForManufacture(itemCode, lotNumber, qty, project)
                            'Select Case manufInserted.Split("*")(0)
                            '    Case "1"
                            '        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_setPGMContainerManufactured(headerID, container, username))
                            '    Case "-1"
                            '        Server.Listener.SendResponse(ClientSocket, manufInserted)
                            'End Select

                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture(itemCode, lotNumber, qty, project)
                            Select Case manufInserted.Split("*")(0)
                                Case "1"
                                    Dim setAsManuf As String = PGM.RTSQL.Update.UI_setPGMContainerManufactured(headerID, container, username)
                                    Select Case setAsManuf.Split("*")(0)
                                        Case "1"
                                            Dim spExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture()
                                            Select Case spExec.Split("*")(0)
                                                Case "1"
                                                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureItemError(itemCode, lotNumber, qty))
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, spExec)
                                            End Select
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREPGMBATCH*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Dim concentration As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim process As String = String.Empty
                    If location.Contains("VW") Then
                        process = "PGM-RVW"
                    ElseIf location.Contains("TY") Then
                        process = "PGM-RTY"
                    End If

                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations(process)
                    Select Case manufLocs.Split("*")(0)
                        Case "1"
                            manufLocs = manufLocs.Remove(0, 2)
                            Dim src As String = manufLocs.Split("|")(0)
                            Dim dest As String = manufLocs.Split("|")(1)

                            Dim headerID As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, location)
                            Select Case headerID.Split("*")(0)
                                Case "1"
                                    headerID = headerID.Remove(0, 2)
                                    Dim totalQty As String = PGM.RTSQL.Retreive.UI_GetPGMBatchTotal(headerID)
                                    Select Case totalQty.Split("*")(0)
                                        Case "1"
                                            totalQty = totalQty.Remove(0, 2)
                                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, totalQty, src, dest, lotNumber)
                                            Select Case manufInserted.Split("*")(0)
                                                Case "1"
                                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, totalQty, src, dest, lotNumber)
                                                    Select Case manufHeaderID.Split("*")(0)
                                                        Case "1"
                                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                            Select Case manufExec.Split("*")(0)
                                                                Case "1"
                                                                    manufExec = manufExec.Remove(0, 2)
                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_setPGMBatchManufactured(headerID, username))
                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    ElseIf manufExec = String.Empty Then
                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, totalQty, src, dest, lotNumber)
                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_setPGMBatchManufactured(headerID, username))
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                        End If
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    End If

                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                                            End Select
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, totalQty)
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, totalQty)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                    End Select


                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PGMEDITCONTAINERUI*"
                Try
                    Dim container As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim oldWeight As String = ClientData.Split("|")(3)
                    Dim newWeight As String = ClientData.Split("|")(4)
                    Dim whse As String = ClientData.Split("|")(5)
                    Dim reason As String = ClientData.Split("|")(6)
                    Dim username As String = ClientData.Split("|")(7)

                    Dim dOld As Double = Convert.ToDouble(oldWeight.Replace(",", sep).Replace(".", sep))
                    Dim dNew As Double = Convert.ToDouble(newWeight.Replace(",", sep).Replace(".", sep))
                    Dim diff As String = Convert.ToDouble(dNew - dOld)
                    Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, whse)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateContQtyHeader(itemCode, lotNumber, diff.Replace(",", "."), whse)
                            Select Case headerUpdated.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_updateContQtyLine(headerId, container, diff.Replace(",", "."), username, reason))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerUpdated)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*UPDATEPGMREMAINDER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Dim concentration As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_updateContainerRem(itemCode, lotNumber, location, qty, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*SETPGMMANUFACTUREDMANUAL*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Dim concentration As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)
                    Dim headerID As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, location)
                    Select Case headerID.Split("*")(0)
                        Case "1"
                            headerID = headerID.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.UI_setPGMBatchManufacturedManual(headerID, username))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try

#End Region

#Region "PGM Records"

            Case "*GETPGMJOBS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMJobLines())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMJOBROWS*"
                Try
                    Dim rowCount As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMJobrows(rowCount))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMJOBSBYDATE*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMJobsByDate(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMCONTAINERS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.UI_GetPGMContainers(itemCode, lotNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIMANUFPGMCONTAINER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim container As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.PGM_updateContManufactured(itemCode, lotNumber, container))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMMFLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PGMManufacture.RTIS.Retrieve.UI_GetPGMMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEPGMMANUFACTURED*"
                Try
                    Dim LineID As String = ClientData.Split("|")(0)
                    Dim UserName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, PGMManufacture.RTIS.Update.UI_setPGMManufactured(LineID, UserName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Powder Prep"

#Region "Powder Prep Manufacture"

            Case "*GETWAITINGPOWDERS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Retrieve.UI_GetWaitingPowders())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*EDITPOWDERQTY*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim oldWeight As String = ClientData.Split("|")(1)
                    Dim newWeight As String = ClientData.Split("|")(2)
                    Dim reason As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim manufactured As String = PowderPrep.RTSQL.Retrieve.UI_GetPowderManufactured(lineID)
                    Select Case manufactured.Split("*")(0)
                        Case "1"
                            manufactured = manufactured.Remove(0, 2)
                            If Convert.ToBoolean(manufactured) = False Then
                                Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Update.UI_editPowderQty(lineID, newWeight, oldWeight, username, reason))
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The powder you are trying to edit has already been manufactured")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, manufactured)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, manufactured)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREPOWDER*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations("Powder Prep")
                    Select Case manufLocs.Split("*")(0)
                        Case "1"
                            manufLocs = manufLocs.Remove(0, 2)
                            Dim src As String = manufLocs.Split("|")(0)
                            Dim dest As String = manufLocs.Split("|")(1)

                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, qty, src, dest, lotNumber)
                            Select Case manufInserted.Split("*")(0)
                                Case "1"
                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, qty, src, dest, lotNumber)
                                    Select Case manufHeaderID.Split("*")(0)
                                        Case "1"
                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                            Dim manufLines As String = PowderPrep.RTSQL.Retrieve.UI_GetPowderRMs(itemCode, lotNumber) 'CHANGE THIS FOR AW
                                            Select Case manufLines.Split("*")(0)
                                                Case "1"
                                                    manufLines = manufLines.Remove(0, 2)
                                                    Dim rawsFound As Boolean = False
                                                    Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                    If manufLines <> String.Empty Then
                                                        rawsFound = True
                                                        Dim allManufLines As String() = manufLines.Split("~")
                                                        For Each manufLine As String In allManufLines
                                                            If manufLine <> String.Empty Then
                                                                Dim rCode As String = manufLine.Split("|")(0)
                                                                Dim rLot As String = manufLine.Split("|")(1)
                                                                insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + lotNumber + "'),")
                                                            End If
                                                        Next
                                                        insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)
                                                    End If

                                                    Dim rawsInserted As String = String.Empty
                                                    If rawsFound Then
                                                        rawsInserted = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                    Else
                                                        rawsInserted = "1*Success"
                                                    End If
                                                    Select Case rawsInserted.Split("*")(0)
                                                        Case "1"
                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                            Select Case manufExec.Split("*")(0)
                                                                Case "1"
                                                                    manufExec = manufExec.Remove(0, 2)
                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                        Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Update.UI_setPPManufactured(lineID, username))
                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    ElseIf manufExec = String.Empty Then
                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                            Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Update.UI_setPPManufactured(lineID, username))
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                        End If
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    End If
                                                                    'manufExec = manufExec.Remove(0, 2)
                                                                    'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                    '    Dim setAsManuf As String = PowderPrep.RTSQL.Update.UI_setPPManufactured(lineID, username)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    'ElseIf manufExec = String.Empty Then
                                                                    '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                    'Else
                                                                    '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    'End If
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, manufLines)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSEPOWDER*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Update.UI_setPPManufacturedManual(lineID, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPOWDERPREPLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Retrieve.UI_GetPowderPrepMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEPPMANUFACTURED*"
                Try
                    Dim LineID As String = ClientData.Split("|")(0)
                    Dim UserName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Update.UI_setPPManufactured(LineID, UserName))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "Powder Prep Records"
            Case "*GETPOWDERPREPRECORDSBYDATE*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Retrieve.UI_GetPowderPrepRecords(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Fresh Slurry"
            Case "*GETALLFRESHSLURRIES*"
                Server.Listener.SendResponse(ClientSocket, FreshSlurry.Evolution.Retreive.UI_GetAllFreshSlurries())
            Case "*UIGETFRESHSLURRYRAWS*"
                Try
                    Dim slurryCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.Evolution.Retreive.UI_GetFreshSlurryRaws(slurryCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETALLFRESHSLURRYRMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.Evolution.Retreive.UI_GetAllFreshSlurryRMs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UISAVEFRESHSLURRYRM*"
                Try
                    Dim slurryCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Dim rmDesc As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Dim linkFound As String = FreshSlurry.RTSQL.Retreive.UI_GetFSLinkExists(slurryCode, rmCode)
                    Select Case linkFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Insert.UI_InsertRMLink(slurryCode, rmCode, rmDesc, username))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIREMOVEFRESHSLURRYRM*"
                Try
                    Dim slurryCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Delete.UI_DeleteRMLink(slurryCode, rmCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#Region "Inputs"

#End Region

#Region "Fresh Slurry Manufacture"
            Case "*GETWAITINGFREShSLURRIES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Retreive.UI_GetWaitingFreshSlurries())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREFRESHSLURRY*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations("Fresh Slurry")
                    Select Case manufLocs.Split("*")(0)
                        Case "1"
                            manufLocs = manufLocs.Remove(0, 2)
                            Dim src As String = manufLocs.Split("|")(0)
                            Dim dest As String = manufLocs.Split("|")(1)

                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, qty, src, dest, lotNumber)
                            Select Case manufInserted.Split("*")(0)
                                Case "1"
                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, qty, src, dest, lotNumber)
                                    Select Case manufHeaderID.Split("*")(0)
                                        Case "1"
                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                            Dim manufLines As String = FreshSlurry.RTSQL.Retreive.UI_GetFSRawMaterials(lineID)
                                            Select Case manufLines.Split("*")(0)
                                                Case "1"
                                                    manufLines = manufLines.Remove(0, 2)
                                                    Dim allManufLines As String() = manufLines.Split("~")
                                                    Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                    For Each manufLine As String In allManufLines
                                                        If manufLine <> String.Empty Then
                                                            Dim rCode As String = manufLine.Split("|")(0)
                                                            Dim rLot As String = manufLine.Split("|")(1)
                                                            insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + rLot + "'),")
                                                        End If
                                                    Next
                                                    insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)

                                                    Dim rawsInserted As String = String.Empty
                                                    If manufLines <> String.Empty Then
                                                        rawsInserted = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                    Else
                                                        rawsInserted = "1*Success"
                                                    End If
                                                    Select Case rawsInserted.Split("*")(0)
                                                        Case "1"
                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                            Select Case manufExec.Split("*")(0)
                                                                Case "1"
                                                                    manufExec = manufExec.Remove(0, 2)
                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                        Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.UI_setFSManufactured(lineID, username))
                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    ElseIf manufExec = String.Empty Then
                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                            Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.UI_setFSManufactured(lineID, username))
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                        End If
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    End If


                                                                    'manufExec = manufExec.Remove(0, 2)
                                                                    'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                    '    Dim setAsManuf As String = FreshSlurry.RTSQL.Update.UI_setFSManufactured(lineID, username)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    'ElseIf manufExec = String.Empty Then
                                                                    '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                    'Else
                                                                    '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    'End If
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, manufLines)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALCLOSEFRESHSLURRY*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.UI_setFSManufacturedManual(lineID, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Fresh Slurry Records"
            Case "*GETFRESHSLURRYRECORDSBYDATE*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Retreive.UI_GetFreshSlurryRecords(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Production Planning "

            Case "*GETPOWDERPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Retrieve.UI_GetPowderPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETZECT1PLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Retrieve.UI_GetZECT1PlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETZECT2PLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Retrieve.UI_GetZECT2PlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Retrieve.UI_GetAWPlanLines())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSELECTCSPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.Evolution.Retrieve.UI_GetSelectCSPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSELECTSPPLANNINGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.Evolution.Retrieve.UI_GetSelectSPPlanLines())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTAWPLANNING*"
                Try
                    Dim AWCode As String = ClientData.Split("|")(0)
                    Dim CatalystCode As String = ClientData.Split("|")(1)
                    Dim DT As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Insert.UI_InsertAWPlanLines(AWCode, CatalystCode, DT, username))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTZECTPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Insert.UI_InsertZECTPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*INSERTPOWDERPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Insert.UI_InsertPowderPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEAWPLANNING*"
                Try
                    Dim Id As String = ClientData.Split("|")(4)
                    Dim AWCode As String = ClientData.Split("|")(0)
                    Dim CatalystCode As String = ClientData.Split("|")(1)
                    Dim DT As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Update.UI_UpdateAWPlanLines(Id, AWCode, CatalystCode, DT, username))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEZECTPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Update.UI_UpdateZECTPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEPOWDERPLANNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, ProductionPlanning.RTSQL.Update.UI_UpdatePowderPlanLines(ClientData))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "Mixed Slurry"

#Region "View Records"
            Case "*GETALLMIXEDSLURRIS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurries(dateFrom, dateto))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRYTANKINFO*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurryTankInfo(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRYMOBILETANKINFO*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurryMobileTankInfo(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTANKSLURRIES*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryInputs(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTANKCHEMICALS*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim tankType As String = ClientData.Split("|")(1)
                    Select Case tankType
                        Case "TNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryChemicals(lineID))
                        Case "MTNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryChemicals_Mobile(lineID))
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CLOSEMIXEDSLURRYTANK*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim closeReason As String = ClientData.Split("|")(1)
                    Dim username As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_CloseMixedSlurryTank(lineID, closeReason, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CLOSEMIXEDSLURRYMOBILETANK*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim closeReason As String = ClientData.Split("|")(1)
                    Dim username As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_CloseMixedSlurryMobileTank(lineID, closeReason, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "NPL"
            Case "*GETNPLPERCENTAGES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetNPLPercentages())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*EDITNPLPERCENTAGE*"
                Try
                    Dim chemName As String = ClientData.Split("|")(0)
                    Dim percentage As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_EditNPLPercentage(chemName, percentage))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Manufacture"
            Case "*GETMIXEDSLURRYJOURNALID*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMSJournalID())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEDMIXEDSLURRYJOURNALID*"
                Try
                    Dim journalID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_EditMSJournalID(journalID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRIESFORMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurriesToManufacture())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRYTANKINFOMANUFACTURE*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurryTankInfoManufacture(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRYMOBILETANKINFOMANUFACTURE*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurryMobileTankInfoManufacture(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTANKSLURRIESMANUFACTURE*"
                Try
                    Dim lineID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryInputsManufacture(lineID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETDECATEDTANKSMANUFACTURE*"
                Try
                    Dim id As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetMixedSlurryDecantsManufacture(id))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETTANKCHEMICALSMANUFACTURE*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim tankType As String = ClientData.Split("|")(1)
                    Select Case tankType
                        Case "TNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryChemicalsManuf(lineID))
                        Case "MTNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryChemicalsManuf_Mobile(lineID))
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSETANK*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_CloseMixedSlurryTankManuf(lineID, "Manually Manufactured", username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSEBUFFTANK*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Dim linesClosed As String = MixedSlurry.RTSQL.Update.UI_CloseAllMobileTanksManuf(lineID, "Manually Manufactured", username)
                    Select Case linesClosed.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_CloseBufferTankManuf(lineID, "Manually Manufactured", username))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, linesClosed)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSEMOBILETANK*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_CloseMixedSlurryMobileTankManuf(lineID, "Manually Manufactured", username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            'Manufacture calls unused due to BBF
            Case "*MANUFACTUREMIXEDSLURRY*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim username As String = ClientData.Split("|")(2)
                    Dim iJournalContra As String = "84199"
                    Dim iJournalProject As String = "23"
                    Dim iJournalID As String = MixedSlurry.RTSQL.Retreive.UI_GetMSJournalID_Manuf()
                    Select Case iJournalID.Split("*")(0)
                        Case "1"
                            iJournalID = iJournalID.Remove(0, 2)
                            If iJournalID <> "0" Then
                                Dim queryList As List(Of String) = New List(Of String)
                                Dim AllFresh As String = MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryInputs_JournalOut(lineID)
                                Select Case AllFresh.Split("*")(0)
                                    Case "1"
                                        Dim Failed As Boolean = False
                                        Dim FailureReason As String = String.Empty
                                        For Each Fresh As String In AllFresh.Split("~")
                                            If Fresh <> String.Empty Then
                                                'Add Fresh Outs
                                                Dim freshInfo As String() = Fresh.Split("|")
                                                Dim trolly As String = freshInfo(0)
                                                Dim code As String = freshInfo(1)
                                                Dim desc As String = freshInfo(2)
                                                Dim lot As String = freshInfo(3)
                                                Dim qty As String = freshInfo(4)
                                                Dim solDate As String = freshInfo(5)
                                                Dim solidity As String = freshInfo(6)
                                                Dim wetWeight As Decimal = Convert.ToDecimal(qty.Replace(",", sep).Replace(".", sep))
                                                If solidity <> String.Empty Then
                                                    Dim dSol As Decimal = Convert.ToDecimal(solidity.Replace(",", sep).Replace(".", sep)) / 100
                                                    Dim dryWeight As String = Convert.ToString(wetWeight * dSol)
                                                    Dim stockInfo As String = MixedSlurry.Evolution.Rereive.MBL_GetFeshSlurryInfo(itemCode)
                                                    Select Case stockInfo.Split("*")(0)
                                                        Case "1"
                                                            stockInfo = stockInfo.Remove(0, 2)
                                                            Dim stockLink As String = stockInfo.Split("|")(0)
                                                            Dim cost As String = stockInfo.Split("|")(1)
                                                            Dim query As String = "INSERT INTO _etblInvJrBatchLines (iInvJrBatchID, iStockID, iWarehouseID, dTrDate, iTrCodeID,
                                                                    cReference, cDescription, fQtyIn, fQtyOut, fNewCost, iProjectID, bIsSerialItem, bIsLotItem, 
                                                                    iSNGroupID, iJobID, iLotID, cLotNumber, cLineNotes, iGLContraID, _etblInvJrBatchLines_iBranchID,
                                                                    iUnitsOfMeasureStockingID, iUnitsOfMeasureCategoryID, iUnitsOfMeasureID)
                                                                    VALUES(" + iJournalID + ", " + stockLink + ", '10', '" + solDate + "', 83,
                                                                    '', '– CS FRESH TO MIS', 0, " + dryWeight.Replace(",", ".") + ", " + cost.Replace(",", ".") + ", 23, 0, 1, 
                                                                    0, 0, 0, '" + lot + "', '', 84199, 0,
                                                                    0, 0, 0)
                                                                    "
                                                            queryList.Add(query)
                                                            'Dim lotID As String = MixedSlurry.Evolution.Rereive.MBL_GetFeshSlurryLotID(lot, stockLink)
                                                            'Select Case lotID.Split("*")(0)
                                                            '    Case "1"
                                                            '        lotID = lotID.Remove(0, 2)

                                                            '    Case Else
                                                            '        Failed = True
                                                            '        FailureReason = lotID
                                                            'End Select

                                                        Case Else
                                                            Failed = True
                                                            FailureReason = stockInfo
                                                    End Select
                                                Else
                                                    Failed = True
                                                    FailureReason = "0*The solidity for lot " + lot + " has not been captured."
                                                End If
                                            End If
                                        Next

                                        If Failed = False Then
                                            Dim mixInfo As String = MixedSlurry.RTSQL.Retreive.UI_GetAllMixedSlurryManufInfo(lineID)
                                            Select Case mixInfo.Split("*")(0)
                                                Case "1"
                                                    mixInfo = mixInfo.Remove(0, 2)
                                                    Dim allInfo As String() = mixInfo.Split("|")
                                                    Dim stockLink As String = allInfo(0)
                                                    Dim lotID As String = allInfo(1)
                                                    Dim lotNumber As String = allInfo(2)
                                                    Dim cost As String = allInfo(3)
                                                    Dim qty As String = allInfo(4)
                                                    Dim dtSol As String = allInfo(5)
                                                    Dim recQty As String = allInfo(6)
                                                    Dim recLot As String = allInfo(7)
                                                    Dim remQty As String = allInfo(8)
                                                    Dim remLot As String = allInfo(9)
                                                    Dim remSol As String = allInfo(10)
                                                    Dim recSol As String = allInfo(11)
                                                    If qty <> String.Empty Then
                                                        Dim query As String = "  INSERT INTO _etblInvJrBatchLines (iInvJrBatchID, iStockID, iWarehouseID, dTrDate, iTrCodeID,
                                                                        cReference, cDescription, fQtyIn, fQtyOut, fNewCost, iProjectID, bIsSerialItem, bIsLotItem, 
                                                                        iSNGroupID, iJobID, iLotID, cLotNumber, cLineNotes, iGLContraID, _etblInvJrBatchLines_iBranchID,
                                                                        iUnitsOfMeasureStockingID, iUnitsOfMeasureCategoryID, iUnitsOfMeasureID)
                                                                        VALUES(" + iJournalID + ", " + stockLink + ", '10', GETDATE(), 83,
                                                                        '', '– CS FRESH TO MIS',  " + qty.Replace(",", ".") + ",0, " + cost.Replace(",", ".") + ", 23, 0, 1, 
                                                                        0, 0, 0, '" + lotNumber + "', '', 84199, 0,
                                                                        0, 0, 0)
                                                                        "
                                                        Dim dSolRec As Decimal = Convert.ToDecimal(recSol.Replace(",", sep).Replace(".", sep)) / 100
                                                        Dim wetRec As Decimal = Convert.ToDecimal(recQty.Replace(",", sep).Replace(".", sep))
                                                        Dim dryWeightRec As String = Convert.ToString(wetRec * dSolRec)
                                                        Dim queryRec As String = "  INSERT INTO _etblInvJrBatchLines (iInvJrBatchID, iStockID, iWarehouseID, dTrDate, iTrCodeID,
                                                                        cReference, cDescription, fQtyIn, fQtyOut, fNewCost, iProjectID, bIsSerialItem, bIsLotItem, 
                                                                        iSNGroupID, iJobID, iLotID, cLotNumber, cLineNotes, iGLContraID, _etblInvJrBatchLines_iBranchID,
                                                                        iUnitsOfMeasureStockingID, iUnitsOfMeasureCategoryID, iUnitsOfMeasureID)
                                                                        VALUES(" + iJournalID + ", " + stockLink + ", '10', GETDATE(), 83,
                                                                        '', '– CS FRESH TO MIS',  0," + dryWeightRec.Replace(",", ".") + ", " + cost.Replace(",", ".") + ", 23, 0, 1, 
                                                                        0, 0, 0, '" + recLot + "', '', 84199, 0,
                                                                        0, 0, 0)
                                                                        "
                                                        Dim dSolRem As Decimal = Convert.ToDecimal(remSol.Replace(",", sep).Replace(".", sep)) / 100
                                                        Dim wetRem As Decimal = Convert.ToDecimal(remQty.Replace(",", sep).Replace(".", sep))
                                                        Dim dryWeightRem As String = Convert.ToString(wetRem * dSolRem)
                                                        Dim queryRem As String = "  INSERT INTO _etblInvJrBatchLines (iInvJrBatchID, iStockID, iWarehouseID, dTrDate, iTrCodeID,
                                                                        cReference, cDescription, fQtyIn, fQtyOut, fNewCost, iProjectID, bIsSerialItem, bIsLotItem, 
                                                                        iSNGroupID, iJobID, iLotID, cLotNumber, cLineNotes, iGLContraID, _etblInvJrBatchLines_iBranchID,
                                                                        iUnitsOfMeasureStockingID, iUnitsOfMeasureCategoryID, iUnitsOfMeasureID)
                                                                        VALUES(" + iJournalID + ", " + stockLink + ", '10', GETDATE(), 83,
                                                                        '', '– CS FRESH TO MIS', 0, " + dryWeightRem.Replace(",", ".") + ", " + cost.Replace(",", ".") + ", 23, 0, 1, 
                                                                        0, 0, 0, '" + remLot + "', '', 84199, 0,
                                                                        0, 0, 0)
                                                                        "
                                                        queryList.Add(query)
                                                        queryList.Add(queryRec)
                                                        queryList.Add(queryRem)
                                                        Dim manuf As String = MixedSlurry.RTSQL.ExecuteTransaction(queryList)
                                                        Select Case manuf.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.UI_UpdateMSManufactured(lineID, username))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, manuf)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "0*NO dry weight for the mixed slurry was found, has its solidity been set?")
                                                    End If
                                                    'If lotID <> String.Empty Then
                                                    '        Dim lotIDrec As String = MixedSlurry.Evolution.Rereive.MBL_GetRecLotID(recLot, stockLink)
                                                    'Select Case lotIDrec.Split("*")(0)
                                                    '    Case "1"
                                                    '        lotIDrec =  lotIDrec.Remove(0, 2)  
                                                    '        Dim lotIDRem As String = MixedSlurry.Evolution.Rereive.MBL_GetRemLotID(remLot, stockLink)
                                                    '            Select Case lotIDRem.Split("*")(0)
                                                    '                Case "1"
                                                    '                    lotIDRem =  lotIDRem.Remove(0, 2)  

                                                    '                Case Else
                                                    '                    Server.Listener.SendResponse(ClientSocket, lotIDRem)
                                                    '            End Select
                                                    '    Case Else
                                                    '        Server.Listener.SendResponse(ClientSocket, lotIDrec)
                                                    'End Select
                                                    'Else
                                                    '    Server.Listener.SendResponse(ClientSocket, "0*Lot " + lotNumber+ "wasnt found in evolution.")
                                                    'End If



                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, mixInfo)
                                            End Select
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, FailureReason)
                                        End If

                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, AllFresh)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*Please configure the mixed slurry journal ID")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, iJournalID)
                    End Select

                    'Dim NplValues As String = String.Empty
                    'Select Case NplValues.Split("*")(0)
                    '    Case "1"
                    '        NplValues = NplValues.Remove(0, 2)  
                    '        Dim AllNpls As String() = NplValues.Split("|")
                    '        Dim Pd_val As String = AllNpls(0)
                    '        Dim Pt_val As String = AllNpls(1)
                    '        Dim Rh_val As String = AllNpls(2)

                    '        Dim NplPers As String = String.Empty
                    '        Select Case NplPers.Split("*")(0)
                    '            Case "1"
                    '                Dim AllPercs As String() = NplValues.Split("|")
                    '                Dim Pd_Per As String = AllNpls(0)
                    '                Dim Pt_Per As String = AllNpls(0)
                    '                Dim Rh_Per As String = AllNpls(0)
                    '                Dim TotalFresh As String = String.Empty
                    '                Select Case TotalFresh.Split("*")(0)
                    '                    Case "1"
                    '                        TotalFresh = TotalFresh.Remove(0, 2)
                    '                        Dim dTotalFresh As Decimal = Convert.ToDecimal(TotalFresh.Replace(".", sep).Replace(",", sep))
                    '                        Dim AllFresh As String = String.Empty
                    '                        Select Case AllFresh.Split("*")(0)
                    '                            Case "1"
                    '                                AllFresh = AllFresh.Remove(0, 2)
                    '                                Dim NlQty As Decimal = 0
                    '                                If Convert.ToDecimal(Rh_val.Replace(".", sep).Replace(",", sep) > 0) Then
                    '                                    NlQty = Math.Round(dTotalFresh * Convert.ToDecimal(Rh_Per.Replace(".", sep).Replace(",", sep)), 5)
                    '                                ElseIf Convert.ToDecimal(Pt_val.Replace(".", sep).Replace(",", sep) > 0)
                    '                                    NlQty = Math.Round(dTotalFresh * Convert.ToDecimal(Pt_Per.Replace(".", sep).Replace(",", sep)), 5)
                    '                                Else
                    '                                    NlQty = Math.Round(dTotalFresh * Convert.ToDecimal(Pd_Per.Replace(".", sep).Replace(",", sep)), 5)
                    '                                End If

                    '                                'Create Journal Out Lines
                    '                                Dim Failed As Boolean = False
                    '                                Dim FailureReason As String = String.Empty
                    '                                For Each Fresh As String In AllFresh.Split("~")
                    '                                    If Fresh <> String.Empty Then
                    '                                        Dim lineAdded As String = String.Empty
                    '                                        Select Case lineAdded.Split("*")(0)
                    '                                            Case "1"
                    '                                                'Ignore
                    '                                            Case Else
                    '                                                Failed = True
                    '                                                FailureReason = lineAdded
                    '                                                Exit For
                    '                                        End Select
                    '                                    End If
                    '                                Next

                    '                                If Failed = False Then
                    '                                    Dim TankInfo As String = String.Empty
                    '                                    Select Case TankInfo.Split("*")(0)
                    '                                        Case "1"
                    '                                            TankInfo = TankInfo.Remove(0, 2)
                    '                                            Dim lineInserted As String = String.Empty
                    '                                            Select Case lineInserted.Split("*")(0)
                    '                                                Case "1"
                    '                                                    Dim nplInserted As String = String.Empty
                    '                                                    Select Case nplInserted.Split("*")(0)
                    '                                                        Case "1"
                    '                                                            'SetJustCleared = 0
                    '                                                            Server.Listener.SendResponse(ClientSocket, String.Empty)
                    '                                                        Case Else
                    '                                                            Server.Listener.SendResponse(ClientSocket, lineInserted)
                    '                                                    End Select
                    '                                                Case Else
                    '                                                    Server.Listener.SendResponse(ClientSocket, lineInserted)
                    '                                            End Select
                    '                                        Case Else
                    '                                            Server.Listener.SendResponse(ClientSocket, TankInfo)
                    '                                    End Select
                    '                                Else
                    '                                    Server.Listener.SendResponse(ClientSocket, FailureReason)
                    '                                End If
                    '                            Case Else
                    '                                Server.Listener.SendResponse(ClientSocket, AllFresh)
                    '                        End Select
                    '                    Case Else
                    '                        Server.Listener.SendResponse(ClientSocket, TotalFresh)
                    '                End Select
                    '            Case Else
                    '                Server.Listener.SendResponse(ClientSocket, NplPers)
                    '        End Select
                    '    Case Else
                    '        Server.Listener.SendResponse(ClientSocket, NplValues)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Chemical Management"
            Case "*GETEXISTINGCHEMICALS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.UI_GetAllChemicals())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDNEWCHEMICAL*"
                Try
                    Dim chemName As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.UI_InsertMixedSlurryChemical(chemName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*DELETECHEMICAL*"
                Try
                    Dim id As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Delete.UI_RemoveChemicalFromList(id))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "ZECT"

#Region "Inputs"
            Case "*UIGETALLZECTCATALYSTS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.UI_GetAllCatalysts())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETZECTCATALYSTRAWS*"
                Try
                    Dim catalystCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GetCatalystRaws(catalystCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETALLZECTRMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.UI_GetAllCatalystRMs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UISAVEZECTRM*"
                Try
                    Dim catalystCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Dim rmDesc As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Dim linkFound As String = Zect.RTSQL.Retreive.UI_GetZeckLinkExists(catalystCode, rmCode)
                    Select Case linkFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Insert.UI_InsertRMLink(catalystCode, rmCode, rmDesc, username))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIREMOVEZECTRM*"
                Try
                    Dim catalystCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Delete.UI_DeleteRMLink(catalystCode, rmCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Outputs"
            Case "*UIGETZECTJOBS*"
                Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GetAllZECTJobs(dateFrom, dateTo))
            Case "*UIGETZECTJOBINPUTS*"
                Dim headerID As String = ClientData
                Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GeZECTJobInPuts(headerID))
            Case "*UIGETZECTJOBOUTPUTS*"
                Dim headerID As String = ClientData
                Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GeZECTJobOutPuts(headerID))
#End Region

#Region "Manufacture"
            Case "*GETZECTHEADERSTOMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GeZECTJobsToManufacture())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETZECtPALLETLINES*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.UI_GeZECTPalletsToManufacture(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREZECTPALLET*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim lineID As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)

                    Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture(itemCode, lotNumber, qty, project)
                    Select Case manufInserted.Split("*")(0)
                        Case "1"
                            Dim setAsManuf As String = Zect.RTSQL.Update.UI_UpdatePalletManufactured(lineID, jobID, username)
                            Select Case setAsManuf.Split("*")(0)
                                Case "1"
                                    Dim spExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture()
                                    Select Case spExec.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureItemError(itemCode, lotNumber, qty))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, spExec)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, manufInserted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREZECTBATCH*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim Code As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Dim zectLine As String = ClientData.Split("|")(4)

                    Dim itemCode = Zect.Evoltion.Retreive.Zect_GetMFCode(Code)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Dim totalQty As String = Zect.RTSQL.Retreive.UI_GetZECTBatchTotal(jobID)
                            Select Case totalQty.Split("*")(0)
                                Case "1"
                                    totalQty = totalQty.Remove(0, 2)

                                    Dim process As String = String.Empty
                                    If zectLine.Contains("2") Then
                                        process = "ZECT 2"
                                    Else
                                        process = "ZECT 1"
                                    End If

                                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations(process)
                                    Select Case manufLocs.Split("*")(0)
                                        Case "1"
                                            manufLocs = manufLocs.Remove(0, 2)
                                            Dim src As String = manufLocs.Split("|")(0)
                                            Dim dest As String = manufLocs.Split("|")(1)

                                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, totalQty, src, dest, lotNumber)
                                            Select Case manufInserted.Split("*")(0)
                                                Case "1"
                                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, totalQty, src, dest, lotNumber)
                                                    Select Case manufHeaderID.Split("*")(0)
                                                        Case "1"
                                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                                            Dim manufLines As String = Zect.RTSQL.Retreive.UI_GetZECTRawMaterials(jobID)
                                                            Select Case manufLines.Split("*")(0)
                                                                Case "1"
                                                                    manufLines = manufLines.Remove(0, 2)
                                                                    Dim allManufLines As String() = manufLines.Split("~")
                                                                    Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                                    For Each manufLine As String In allManufLines
                                                                        If manufLine <> String.Empty Then
                                                                            Dim rCode As String = manufLine.Split("|")(0)
                                                                            Dim rLot As String = manufLine.Split("|")(1)
                                                                            insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + lotNumber + "'),")
                                                                        End If
                                                                    Next
                                                                    insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)

                                                                    Dim rawsInserted As String = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                                    Select Case rawsInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                                            Select Case manufExec.Split("*")(0)
                                                                                Case "1"
                                                                                    manufExec = manufExec.Remove(0, 2)
                                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                                        Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.UI_setZECTBatchManufactured(jobID, username))
                                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                                    ElseIf manufExec = String.Empty Then
                                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, totalQty, src, dest, lotNumber)
                                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.UI_setZECTBatchManufactured(jobID, username))
                                                                                        Else
                                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                                        End If
                                                                                    Else
                                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                                    End If


                                                                                    'manufExec = manufExec.Remove(0, 2)
                                                                                    'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                                    '    Dim setAsManuf As String = Zect.RTSQL.Update.UI_setZECTBatchManufactured(jobID, username)
                                                                                    '    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                                                                    'ElseIf manufExec = String.Empty Then
                                                                                    '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, totalQty, src, dest, lotNumber)
                                                                                    '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                                    'Else
                                                                                    '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                                    'End If
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufLines)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, totalQty)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, totalQty)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSEJOB*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.UI_setZECTBatchManufacturedManual(jobID, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "A&W"

#Region "Inputs"
            Case "*UIGETALLAWCATALYSTS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.UI_GetAllAWCatalysts())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETAWCATALYSTRAWS*"
                Try
                    Dim catalystCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GetAWCatalystRaws(catalystCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETALLAWRMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.UI_GetAllAWWRMs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UISAVEAWRM*"
                Try
                    Dim awCode As String = ClientData.Split("|")(0)
                    Dim awDesc As String = ClientData.Split("|")(1)
                    Dim rmCode As String = ClientData.Split("|")(2)
                    Dim rmDesc As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)
                    Dim linkFound As String = AW.RTSQL.Retrieve.UI_GetAWLinkExists(awCode, rmCode)
                    Select Case linkFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Insert.UI_InsertRMLink(awCode, awDesc, rmCode, rmDesc, username))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIREMOVEAWRM*"
                Try
                    Dim catalystCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Delete.UI_DeleteRMLink(catalystCode, rmCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Outputs"
            Case "*UIGETAWJOBS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GetAllAWJobs(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETAWJOBINPUTS*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GetAWJobInPuts(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETAWJOBOUTPUTS*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GetAWJobOutputs(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Manufacture"
            Case "*GETAWHEADERSTOMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GeAWJobsToManufacture())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWPALLETLINES*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.UI_GetAWPalletsToManufacture(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREAWPALLET*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim lineID As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)

                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations("A&W")
                    Select Case manufLocs.Split("*")(0)
                        Case "1"
                            manufLocs = manufLocs.Remove(0, 2)
                            Dim src As String = manufLocs.Split("|")(0)
                            Dim dest As String = manufLocs.Split("|")(1)
                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, qty, src, dest, lotNumber)
                            Select Case manufInserted.Split("*")(0)
                                Case "1"
                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, qty, src, dest, lotNumber)
                                    Select Case manufHeaderID.Split("*")(0)
                                        Case "1"
                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                            Dim manufLines As String = AW.RTSQL.Retrieve.UI_GetAWRawMaterials(jobID) 'CHANGE THIS FOR AW
                                            Select Case manufLines.Split("*")(0)
                                                Case "1"
                                                    manufLines = manufLines.Remove(0, 2)
                                                    Dim allManufLines As String() = manufLines.Split("~")
                                                    Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                    For Each manufLine As String In allManufLines
                                                        If manufLine <> String.Empty Then
                                                            Dim rCode As String = manufLine.Split("|")(0)
                                                            Dim rLot As String = manufLine.Split("|")(1)
                                                            insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + lotNumber + "'),")
                                                        End If
                                                    Next
                                                    insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)

                                                    Dim rawsInserted As String = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                    Select Case rawsInserted.Split("*")(0)
                                                        Case "1"
                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                            Select Case manufExec.Split("*")(0)
                                                                Case "1"
                                                                    manufExec = manufExec.Remove(0, 2)
                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                        Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.UI_UpdatePalletManufactured(lineID, jobID, username))
                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    ElseIf manufExec = String.Empty Then
                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.UI_UpdatePalletManufactured(lineID, jobID, username))
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                        End If
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    End If


                                                                    'manufExec = manufExec.Remove(0, 2)
                                                                    'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                    '    Dim setAsManuf As String = AW.RTSQL.Update.UI_UpdatePalletManufactured(lineID, jobID, username)
                                                                    '    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                                                    'ElseIf manufExec = String.Empty Then
                                                                    '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                    'Else
                                                                    '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    'End If
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, manufLines)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                            End Select

                        Case Else
                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTUREAWBATCH*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)

                    Dim totalQty As String = AW.RTSQL.Retrieve.UI_GetAWBatchTotal(jobID)
                    Select Case totalQty.Split("*")(0)
                        Case "1"
                            totalQty = totalQty.Remove(0, 2)
                            Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations("A&W")
                            Select Case manufLocs.Split("*")(0)
                                Case "1"
                                    manufLocs = manufLocs.Remove(0, 2)
                                    Dim src As String = manufLocs.Split("|")(0)
                                    Dim dest As String = manufLocs.Split("|")(1)
                                    Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, totalQty, src, dest, lotNumber)
                                    Select Case manufInserted.Split("*")(0)
                                        Case "1"
                                            Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, totalQty, src, dest, lotNumber)
                                            Select Case manufHeaderID.Split("*")(0)
                                                Case "1"
                                                    manufHeaderID = manufHeaderID.Remove(0, 2)
                                                    Dim manufLines As String = AW.RTSQL.Retrieve.UI_GetAWRawMaterials(jobID) 'CHANGE THIS FOR AW
                                                    Select Case manufLines.Split("*")(0)
                                                        Case "1"
                                                            manufLines = manufLines.Remove(0, 2)
                                                            Dim allManufLines As String() = manufLines.Split("~")
                                                            Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                            For Each manufLine As String In allManufLines
                                                                If manufLine <> String.Empty Then
                                                                    Dim rCode As String = manufLine.Split("|")(0)
                                                                    Dim rLot As String = manufLine.Split("|")(1)
                                                                    insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + lotNumber + "'),")
                                                                End If
                                                            Next
                                                            insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)

                                                            Dim rawsInserted As String = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                            Select Case rawsInserted.Split("*")(0)
                                                                Case "1"
                                                                    Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                                    Select Case manufExec.Split("*")(0)
                                                                        Case "1"
                                                                            manufExec = manufExec.Remove(0, 2)
                                                                            If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                                Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.UI_setAWBatchManufactured(jobID, username))
                                                                                'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                            ElseIf manufExec = String.Empty Then
                                                                                Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, totalQty, src, dest, lotNumber)
                                                                                If manufMessage.Contains("**SUCCESS**") Then
                                                                                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.UI_setAWBatchManufactured(jobID, username))
                                                                                Else
                                                                                    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                                End If
                                                                            Else
                                                                                Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                            End If


                                                                            'manufExec = manufExec.Remove(0, 2)
                                                                            'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                            '    Dim setAsManuf As String = AW.RTSQL.Update.UI_setAWBatchManufactured(jobID, username)
                                                                            '    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                                                            'ElseIf manufExec = String.Empty Then
                                                                            '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, totalQty, src, dest, lotNumber)
                                                                            '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                            'Else
                                                                            '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                            'End If
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, manufLines)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, manufInserted)
                                    End Select

                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, manufLocs)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, totalQty)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, totalQty)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSEAW*"
                Try
                    Dim jobID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.UI_setAWBatchManufacturedManual(jobID, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Canning"

#Region "Inputs"
            Case "*UIGETALLCANNINGCATALYSTS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Canning.Evolution.Retrieve.UI_GetAllCanningCatalysts())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETCANNINGCATALYSTRAWS*"
                Try
                    Dim catalystCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.UI_GetCanningCatalystRaws(catalystCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIGETALLCANNINGRMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Canning.Evolution.Retrieve.UI_GetAllCanningWRMs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UISAVECANNINGRM*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim itemDesc As String = ClientData.Split("|")(1)
                    Dim rmCode As String = ClientData.Split("|")(2)
                    Dim rmDesc As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)
                    Dim linkFound As String = Canning.RTSQL.Retrieve.UI_GetCanningLinkExists(itemCode, rmCode)
                    Select Case linkFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Insert.UI_InsertRMLink(itemCode, itemDesc, rmCode, rmDesc, username))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, linkFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UIREMOVECANNINGRM*"
                Try
                    Dim catalystCode As String = ClientData.Split("|")(0)
                    Dim rmCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Delete.UI_DeleteRMLink(catalystCode, rmCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Outputs"
            Case "*UIGETCANNINGRECORDS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0) + " 00:00:29.317"
                    Dim dateTo As String = ClientData.Split("|")(1) + " 23:59:29.317"
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.UI_GetCanningRecords(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Manufacture"
            Case "*GETCANNINGLINESTOMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.UI_GetCanningLinesToManufacture())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUFACTURECANNINGPALLET*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)


                    Dim manufLocs As String = AutoManufacture.RTIS.Retreive.UI_GetManufLocations("Canning")
                    Select Case manufLocs.Split("*")(0)
                        Case "1"
                            manufLocs = manufLocs.Remove(0, 2)
                            Dim src As String = manufLocs.Split("|")(0)
                            Dim dest As String = manufLocs.Split("|")(1)
                            Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture_H(itemCode, qty, src, dest, lotNumber)
                            Select Case manufInserted.Split("*")(0)
                                Case "1"
                                    Dim manufHeaderID As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderID(itemCode, qty, src, dest, lotNumber)
                                    Select Case manufHeaderID.Split("*")(0)
                                        Case "1"
                                            manufHeaderID = manufHeaderID.Remove(0, 2)
                                            Dim manufLines As String = Canning.RTSQL.Retrieve.UI_GetCanningRM(lineID)
                                            Select Case manufLines.Split("*")(0)
                                                Case "1"
                                                    manufLines = manufLines.Remove(0, 2)
                                                    Dim allManufLines As String() = manufLines.Split("~")
                                                    Dim insertQuery As String = "INSERT INTO [__SLtbl_AMPDetail] ([ifkAMPHeaderID], [sComponentItemCode], [sComponentWarehouseCode],[sComponentLotNumber]) VALUES"
                                                    For Each manufLine As String In allManufLines
                                                        If manufLine <> String.Empty Then
                                                            Dim rCode As String = manufLine.Split("|")(0)
                                                            Dim rLot As String = manufLine.Split("|")(1)
                                                            insertQuery += Convert.ToString("( '" + manufHeaderID + "', '" + rCode + "', '" + src + "', '" + lotNumber + "'),")
                                                        End If
                                                    Next
                                                    insertQuery = insertQuery.Remove(insertQuery.Length - 1, 1)

                                                    Dim rawsInserted As String = AutoManufacture.Evolution.UI_ExecuteGeneric(insertQuery)
                                                    Select Case rawsInserted.Split("*")(0)
                                                        Case "1"
                                                            Dim manufExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture_HL(manufHeaderID)
                                                            Select Case manufExec.Split("*")(0)
                                                                Case "1"
                                                                    manufExec = manufExec.Remove(0, 2)
                                                                    If (manufExec.Contains("rows affected") Or manufExec.Contains("Commands completed successfully.")) And manufExec <> String.Empty Then '
                                                                        Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Update.UI_UpdatePalletManufactured(lineID, username))
                                                                        'Server.Listener.SendResponse(ClientSocket, manufExec)
                                                                    ElseIf manufExec = String.Empty Then
                                                                        Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                        If manufMessage.Contains("**SUCCESS**") Then
                                                                            Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Update.UI_UpdatePalletManufactured(lineID, username))
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                        End If
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    End If


                                                                    'manufExec = manufExec.Remove(0, 2)
                                                                    'If manufExec.Contains("**SUCCESS**") And manufExec <> String.Empty Then
                                                                    '    Dim setAsManuf As String = Canning.RTSQL.Update.UI_UpdatePalletManufactured(lineID, username)
                                                                    '    Server.Listener.SendResponse(ClientSocket, setAsManuf)
                                                                    'ElseIf manufExec = String.Empty Then
                                                                    '    Dim manufMessage As String = AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureHeaderMSG(itemCode, qty, src, dest, lotNumber)
                                                                    '    Server.Listener.SendResponse(ClientSocket, manufMessage)
                                                                    'Else
                                                                    '    Server.Listener.SendResponse(ClientSocket, "0*" + manufExec)
                                                                    'End If
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, manufExec)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, rawsInserted)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, manufLines)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, manufHeaderID)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, manufInserted)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, manufLocs)
                    End Select

                    'Dim manufInserted As String = AutoManufacture.Evolution.Insert.UI_InsertAutorManufacture(itemCode, lotNumber, qty, project)
                    'Select Case manufInserted.Split("*")(0)
                    '    Case "1"
                    '        Dim setAsManuf As String = Canning.RTSQL.Update.UI_UpdatePalletManufactured(lineID, username)
                    '        Select Case setAsManuf.Split("*")(0)
                    '            Case "1"
                    '                Dim spExec As String = AutoManufacture.Evolution.Update.UI_ProccessAutorManufacture()
                    '                Select Case spExec.Split("*")(0)
                    '                    Case "1"
                    '                        Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureItemError(itemCode, lotNumber, qty))
                    '                    Case "-1"
                    '                        Server.Listener.SendResponse(ClientSocket, spExec)
                    '                End Select
                    '            Case "0"
                    '                Server.Listener.SendResponse(ClientSocket, setAsManuf)
                    '            Case "-1"
                    '                Server.Listener.SendResponse(ClientSocket, setAsManuf)
                    '        End Select
                    '    Case "-1"
                    '        Server.Listener.SendResponse(ClientSocket, manufInserted)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MANUALLYCLOSECANNING*"
                Try
                    Dim lineID As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Update.UI_UpdatePalletManufacturedManual(lineID, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "FG Printing"
            Case "*GETFGLABELINFO*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, FGPrinting.Evolution.Retreive.UI_GetLabelInfo(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEFGLABELS*"
                Try
                    Dim palletInfo As String = ClientData.Split("*")(0)
                    Dim itemCode As String = palletInfo.Split("|")(0)
                    Dim lotNumber As String = palletInfo.Split("|")(1)
                    Dim totalQty As String = palletInfo.Split("|")(2)
                    Dim fromPallet As String = palletInfo.Split("|")(3)

                    Dim quantities As String = ClientData.Split("*")(1)
                    Dim exp As String = String.Empty
                    Dim allQuantities As String() = quantities.Split("~")

                    Dim unqBar As String = "P" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                    Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + exp.PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + totalQty.PadLeft(8, "0") + "(90)" + unqBar
                    Dim palletSaved As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcodePallet(Barcode, "FG Printing: " + fromPallet, fromPallet)
                    Select Case palletSaved.Split("*")(0)
                        Case "1"
                            Dim palletID As String = Unique.RTSQL.Retreive.UI_GetPalletID(Barcode)
                            Select Case palletID.Split("*")(0)
                                Case "1"
                                    palletID = palletID.Remove(0, 2)
                                    Dim palletUnqs As String = String.Empty
                                    Dim insertUnq As String = "INSERT INTO [tbl_unqBarcodes] ([vUnqBarcode],[Receive],[Issue],[StockTake],[CycleCount],[Manuf],[Dispatch],[Printed],[DispatchDate],[Quarantine],[bValidated],[ValidateRef]) VALUES "
                                    Dim inserUnqPalletLines As String = "INSERT INTO [ltbl_PalletBarcodes] ([iPallet_ID], [vUnqBarcode], [bOnPallet]) VALUES"

                                    For Each qty As String In allQuantities
                                        If qty <> String.Empty Then
                                            Dim boxNumber As String = qty.Split("|")(0)
                                            Dim boxQty As String = qty.Split("|")(1)
                                            Dim unqLine As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "") + "_" + boxNumber
                                            Dim boxBarcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + exp.PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + boxQty.PadLeft(8, "0") + "(90)" + unqLine

                                            palletUnqs += boxBarcode + "~"
                                            insertUnq += "('" + boxBarcode + "', NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(), NULL, NULL, 1, '" + "FG Printing: " + fromPallet + "'),"
                                            inserUnqPalletLines += "('" + palletID + "','" + boxBarcode + "',1),"
                                        End If
                                    Next

                                    palletUnqs = palletUnqs.Substring(0, palletUnqs.Length - 1)
                                    insertUnq = insertUnq.Substring(0, insertUnq.Length - 1)
                                    inserUnqPalletLines = inserUnqPalletLines.Substring(0, inserUnqPalletLines.Length - 1)

                                    Dim unqsInserted As String = Unique.RTSQL.Insert.UI_GenericInsert(insertUnq)
                                    Select Case unqsInserted.Split("*")(0)
                                        Case "1"
                                            Dim palletLinesInserted As String = Unique.RTSQL.Insert.UI_GenericInsert(inserUnqPalletLines)
                                            Select Case palletLinesInserted.Split("*")(0)
                                                Case "1"
                                                    Server.Listener.SendResponse(ClientSocket, "1*" + Barcode + "*" + palletUnqs)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, palletLinesInserted)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, palletID)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, palletID)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, palletID)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, palletSaved)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Auto Manufacture"
            Case "*GETAUTOMANUFACTURERECORDS*"
                Try
                    Dim dateFrom As String = ClientData.Split("|")(0)
                    Dim dateTo As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Retreive.UI_GetAutoManufactureRecords(dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCONFIGLOCS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.RTIS.Retreive.UI_GetManufProcs())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMANUFWHSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Retreive.UI_GetManufWhses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEMANUFLOCATIONS*"
                Try
                    Dim process As String = ClientData.Split("|")(0)
                    Dim src As String = ClientData.Split("|")(1)
                    Dim dest As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Update.UI_UpdateProcessLoc(process, src, dest))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PROCESSAUTOMANUFACTURE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AutoManufacture.Evolution.Update.UI_ProccessAutorManufactureTables())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Dispatch"
            Case "*GETSOUNQBARCODES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Dispatch.RTSQL.Retreive.UI_GetSOUnqBarcodes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CLEARSOUNQS*"
                Try
                    Dim soNumber As String = ClientData
                    Dim palletsCleared As String = Unique.RTSQL.Update.UI_ClearUnqPalletsDispatch(soNumber)
                    Select Case palletsCleared.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.UI_ClearUnqsDispatch(soNumber))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, palletsCleared)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Stock Take"

#Region "Open"
            Case "*GETEVOSTOCKTAKES*"
                Server.Listener.SendResponse(ClientSocket, StockTake.Evolution.Retreive.UI_GetEvoStockTakes())
            Case "*IMPORTSTOCKITEMS*"
                Dim stNum As String = ClientData
                Dim checked As String = StockTake.RTSQL.Retreive.UI_CheckSTHeader(stNum)
                Select Case checked.Split("*")(0)
                    Case "1"
                        Dim headerImport As String = StockTake.RTSQL.Insert.UI_ImportEvoStockTakeHeader(stNum)
                        Select Case headerImport.Split("*")(0)
                            Case "1"
                                Dim linesImport As String = StockTake.RTSQL.Insert.UI_ImportEvoStockTakeLines(stNum)
                                Server.Listener.SendResponse(ClientSocket, linesImport)
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, headerImport)
                        End Select
                    Case "-1"
                        Server.Listener.SendResponse(ClientSocket, checked)
                End Select
            Case "*GETRTSTOCKTAKES*"
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_GetRTStockTakes())
            Case "*GETINVCOUNTLINETICKETS*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_getInvCountLineTickets(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REVERSESTOCKTAKETICKETRT2D*"
                Try
                    Dim headerID As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim ticketInfo As String = StockTake.RTSQL.Retreive.UI_getTicketInfo(ticketNo, headerID)
                    Select Case ticketInfo.Split("*")(0)
                        Case "1"
                            ticketInfo = ticketInfo.Remove(0, 2)
                            Dim ticketQty1 As String = ticketInfo.Split("|")(0)
                            Dim ticketQty2 As String = ticketInfo.Split("|")(1)
                            Dim ticketUnq As String = ticketInfo.Split("|")(2)
                            Dim unqsReset As String = Unique.RTSQL.Update.MBL_UpdateItemSTBoth_Reversal(ticketUnq)
                            Select Case unqsReset.Split("*")(0)
                                Case "1"
                                    Dim stQtysReversed As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeReverseTicketRT2D(headerID, ticketQty1, ticketQty2)
                                    Select Case stQtysReversed.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.UI_InvalidateSTTicket(headerID, ticketNo))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, stQtysReversed)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, unqsReset)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, ticketInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REVERSESTOCKTAKETICKETPALLET*"
                Try
                    Dim headerID As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim ticketInfo As String = StockTake.RTSQL.Retreive.UI_getTicketInfo(ticketNo, headerID)
                    Select Case ticketInfo.Split("*")(0)
                        Case "1"
                            ticketInfo = ticketInfo.Remove(0, 2)
                            Dim ticketQty1 As String = ticketInfo.Split("|")(0)
                            Dim ticketQty2 As String = ticketInfo.Split("|")(1)
                            Dim ticketUnq As String = ticketInfo.Split("|")(2)

                            Dim palletUnqs As String = StockTake.RTSQL.Retreive.MBL_GetAllPalletBarcodes(ticketUnq)
                            Select Case palletUnqs.Split("*")(0)
                                Case "1"
                                    palletUnqs = palletUnqs.Remove(0, 2)
                                    Dim allPalletUnqs As String() = palletUnqs.Split("~")
                                    Dim updateQuery As String = String.Empty
                                    For Each unqBarcode As String In allPalletUnqs
                                        If unqBarcode <> String.Empty Then
                                            updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '', [StockTake2] = '' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode)
                                        End If
                                    Next
                                    Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                    Select Case batchUpdated.Split("*")(0)
                                        Case "1"
                                            Dim unqsReset As String = Unique.RTSQL.Update.MBL_UpdateItemSTPalletBoth_Reversal(ticketUnq)
                                            Select Case unqsReset.Split("*")(0)
                                                Case "1"
                                                    Dim stQtysReversed As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeReverseTicketRT2D(headerID, ticketQty1, ticketQty2)
                                                    Select Case stQtysReversed.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.UI_InvalidateSTTicket(headerID, ticketNo))
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, stQtysReversed)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, unqsReset)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, palletUnqs)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, ticketInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REVERSESTOCKTAKETICKETRMPALLET*"
                Try
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim allTicketInfo As String = StockTake.RTSQL.Retreive.UI_getAllTicketInfo(ticketNo)
                    Select Case allTicketInfo.Split("*")(0)
                        Case "1"
                            Dim ticketInfoSplit As String() = allTicketInfo.Split("~")
                            For Each ticketInfo As String In ticketInfoSplit
                                ticketInfo = ticketInfo.Remove(0, 2)
                                Dim headerID As String = ticketInfo.Split("|")(0)
                                Dim ticketQty1 As String = ticketInfo.Split("|")(1)
                                Dim ticketQty2 As String = ticketInfo.Split("|")(2)
                                Dim ticketUnq As String = ticketInfo.Split("|")(3)
                                Dim palletUnqs As String = StockTake.RTSQL.Retreive.MBL_GetAllPalletBarcodes(ticketUnq)
                                Select Case palletUnqs.Split("*")(0)
                                    Case "1"
                                        palletUnqs = palletUnqs.Remove(0, 2)
                                        Dim allPalletUnqs As String() = palletUnqs.Split("~")
                                        Dim updateQuery As String = String.Empty
                                        For Each unqBarcode As String In allPalletUnqs
                                            If unqBarcode <> String.Empty Then
                                                updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '', [StockTake2] = '' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode)
                                            End If
                                        Next
                                        Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                        Select Case batchUpdated.Split("*")(0)
                                            Case "1"
                                                Dim unqsReset As String = Unique.RTSQL.Update.MBL_UpdateItemSTPalletBoth_Reversal(ticketUnq)
                                                Select Case unqsReset.Split("*")(0)
                                                    Case "1"
                                                        Dim stQtysReversed As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeReverseTicketRT2D(headerID, ticketQty1, ticketQty2)
                                                        Select Case stQtysReversed.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.UI_InvalidateSTTicket(headerID, ticketNo))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, stQtysReversed)
                                                                Exit For
                                                        End Select
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, unqsReset)
                                                        Exit For
                                                End Select
                                            Case Else
                                                Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                Exit For
                                        End Select
                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, palletUnqs)
                                        Exit For
                                End Select
                            Next
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, allTicketInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REVERSESTOCKTAKETICKET*"
                Try
                    Dim headerID As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim ticketInfo As String = StockTake.RTSQL.Retreive.UI_getTicketInfo(ticketNo, headerID)
                    Select Case ticketInfo.Split("*")(0)
                        Case "1"
                            ticketInfo = ticketInfo.Remove(0, 2)
                            Dim ticketQty1 As String = ticketInfo.Split("|")(0)
                            Dim ticketQty2 As String = ticketInfo.Split("|")(1)
                            Dim ticketUnq As String = ticketInfo.Split("|")(2)
                            Dim stInfo As String = StockTake.RTSQL.Retreive.UI_getStQtys(headerID)
                            Select Case stInfo.Split("*")(0)
                                Case "1"
                                    stInfo = stInfo.Remove(0, 2)
                                    Dim stQty1 As String = stInfo.Split("|")(0)
                                    Dim stQty2 As String = stInfo.Split("|")(1)
                                    Dim newQty1 As String = Convert.ToString(Convert.ToDecimal(stQty1.Replace(".", sep).Replace(",", sep)) - Convert.ToDecimal(ticketQty1.Replace(".", sep).Replace(",", sep)))
                                    Dim newQty2 As String = Convert.ToString(Convert.ToDecimal(stQty2.Replace(".", sep).Replace(",", sep)) - Convert.ToDecimal(ticketQty2.Replace(".", sep).Replace(",", sep)))
                                    Dim stQtysReversed As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeReverseTicket(headerID, newQty1, newQty2)
                                    Select Case stQtysReversed.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.UI_InvalidateSTTicket(headerID, ticketNo))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, stQtysReversed)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, stInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, ticketInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETEXPORTFORMATS*"
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_GetExportFormats())
            Case "*GETEXPORTFORMATSTRING*"
                Dim name As String = ClientData
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_GetExportFormatString(name))
            Case "*SAVEEXPORTFORMAT*"
                Try
                    Dim name As String = ClientData.Split("*")(0)
                    Dim format As String = ClientData.Split("*")(1)
                    Dim delimiter As String = ClientData.Split("*")(2)
                    Dim formatExists As String = StockTake.RTSQL.Retreive.UI_CheckExportFormatExists(name)
                    Select Case formatExists.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Insert.UI_InsertExportFormat(name, format, delimiter))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, formatExists)
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "-1*Unexpected server error")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEEXPORTFORMAT*"
                Try
                    Dim name As String = ClientData.Split("*")(0)
                    Dim format As String = ClientData.Split("*")(1)
                    Dim delimiter As String = ClientData.Split("*")(2)
                    Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.UI_UpdateExportFormat(name, format, delimiter))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*DELETEEXPORTFORMAT*"
                Try
                    Dim name As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Delete.UI_RemoveExportLayout(name))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSTREPORTINFO*"
                Try
                    Dim stNum As String = ClientData
                    Dim invLinesCount As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCount(stNum)
                    Select Case invLinesCount.Split("*")(0)
                        Case "1"
                            invLinesCount = invLinesCount.Remove(0, 2)
                            Dim Count1Lines As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCount1(stNum)
                            Select Case Count1Lines.Split("*")(0)
                                Case "1"
                                    Count1Lines = Count1Lines.Remove(0, 2)
                                    Dim countLines2 As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCount2(stNum)
                                    Select Case countLines2.Split("*")(0)
                                        Case "1"
                                            countLines2 = countLines2.Remove(0, 2)
                                            Dim totalItems As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCountAll(stNum)
                                            Select Case totalItems.Split("*")(0)
                                                Case "1"
                                                    totalItems = totalItems.Remove(0, 2)
                                                    Server.Listener.SendResponse(ClientSocket, "1*" + invLinesCount + "|" + Count1Lines + "|" + countLines2 + "|" + totalItems)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, totalItems)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, countLines2)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, Count1Lines)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, invLinesCount)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSTREPORTINFOARCHIVE*"
                Try
                    Dim stNum As String = ClientData
                    Dim invLinesCount As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCountArchive(stNum)
                    Select Case invLinesCount.Split("*")(0)
                        Case "1"
                            invLinesCount = invLinesCount.Remove(0, 2)
                            Dim Count1Lines As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCount1Archive(stNum)
                            Select Case Count1Lines.Split("*")(0)
                                Case "1"
                                    Count1Lines = Count1Lines.Remove(0, 2)
                                    Dim countLines2 As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCount2Archive(stNum)
                                    Select Case countLines2.Split("*")(0)
                                        Case "1"
                                            countLines2 = countLines2.Remove(0, 2)
                                            Dim totalItems As String = StockTake.RTSQL.Retreive.UI_GetSTLinesCountAllArchive(stNum)
                                            Select Case totalItems.Split("*")(0)
                                                Case "1"
                                                    totalItems = totalItems.Remove(0, 2)
                                                    Server.Listener.SendResponse(ClientSocket, "1*" + invLinesCount + "|" + Count1Lines + "|" + countLines2 + "|" + totalItems)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, totalItems)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, countLines2)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, Count1Lines)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, invLinesCount)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ARCHIVESTOCKTAKEANDVALIDATION*"
                Dim stNum As String = ClientData
                Dim validationMoved As String = StockTake.RTSQL.Insert.UI_ArchiveRTStockTakeTicketLines(stNum)
                Select Case validationMoved.Split("*")(0)
                    Case "1"
                        Dim validationRemoved As String = StockTake.RTSQL.Delete.UI_RemovePostArchiveTicketLines(stNum)
                        Select Case validationRemoved.Split("*")(0)
                            Case "1"
                                Dim linesMoved As String = StockTake.RTSQL.Insert.UI_ArchiveRTStockTakeLines(stNum)
                                Select Case linesMoved.Split("*")(0)
                                    Case "1"
                                        Dim linesRemoved As String = StockTake.RTSQL.Delete.UI_RemovePostArchiveLines(stNum)
                                        Select Case linesRemoved.Split("*")(0)
                                            Case "1"
                                                Dim archived As String = StockTake.RTSQL.Update.UI_ArchiveStockTakeHeader(stNum)
                                                Server.Listener.SendResponse(ClientSocket, archived)
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, linesRemoved)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, linesMoved)
                                End Select
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, validationRemoved)
                        End Select
                    Case "-1"
                        Server.Listener.SendResponse(ClientSocket, validationMoved)
                End Select
#End Region

#Region "Archive"
            Case "*GETARCHIVEDRTSTOCKTAKES*"
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_GetRTArchiveStockTakes())
            Case "*GETINVCOUNTLINETICKETSARCHIVE*"
                Try
                    Dim headerID As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.UI_getInvCountLineTickets_Archive(headerID))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVESTOCKTAKEWITHVALIDATION*"
                Dim stNum As String = ClientData
                Dim validationRemoved As String = StockTake.RTSQL.Delete.UI_RemoveArchiveTicketLines(stNum)
                Select Case validationRemoved.Split("*")(0)
                    Case "1"
                        Dim linesRemoved As String = StockTake.RTSQL.Delete.UI_RemoveArchiveRTStockTakeLines(stNum)
                        Select Case linesRemoved.Split("*")(0)
                            Case "1"
                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Delete.UI_RemoveArchiveRTStockTakeHead(stNum))
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, linesRemoved)
                        End Select
                    Case "-1"
                        Server.Listener.SendResponse(ClientSocket, validationRemoved)
                End Select
#End Region
#End Region

#Region "Palletizing"
            Case "*GETPALLETPRINTSETTINGS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Palletizing.RTIS.Retreive.UI_GetPalletPrintSettings())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEPALLETPRINTSETTINGS*"
                Try
                    Dim printer As String = ClientData.Split("|")(0)
                    Dim label As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Palletizing.RTIS.Update.UI_UpdatePalletPrinSettings(printer, label))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SENDPALLETLABELTOSERVER*"
                Try
                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETITEMCODES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Palletizing.Evolution.Retreive.UI_GetItemCodes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETITEMLOTS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, Palletizing.Evolution.Retreive.UI_GetItemLots(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETLINESALL*"
                Try
                    Dim dateSearch As Boolean = Convert.ToBoolean(ClientData.Split("|")(0))
                    Dim dateFrom As String = ClientData.Split("|")(1)
                    Dim dateTo As String = ClientData.Split("|")(2)
                    Dim headerInfo As String = String.Empty
                    If dateSearch Then
                        headerInfo = Palletizing.RTIS.Retreive.UI_GetPalletsByDate(dateFrom, dateTo)
                    Else
                        headerInfo = Palletizing.RTIS.Retreive.UI_GetPallets()
                    End If

                    Select Case headerInfo.Split("*")(0)
                        Case "1"
                            headerInfo = headerInfo.Remove(0, 2)
                            Dim sendList As String = String.Empty
                            Dim allHeaderInfo As List(Of String) = headerInfo.Split("~").ToList()
                            For Each header As String In allHeaderInfo
                                If header <> String.Empty Then
                                    Dim id As String = header.Split("|")(0)
                                    Dim _date As String = header.Split("|")(1)
                                    Dim palletNum As String = header.Split("|")(2)
                                    Dim itemCode As String = Barcodes.GetItemCode(palletNum)
                                    Dim lotNumbers As String = Palletizing.RTIS.Retreive.UI_GetPalletLots(id)
                                    Select Case lotNumbers.Split("*")(0)
                                        Case "1"
                                            lotNumbers = lotNumbers.Remove(0, 2)
                                            sendList += id + "|" + palletNum + "|" + itemCode + "|" + _date + "|" + lotNumbers + "~"
                                        Case "0"
                                            'Ignor
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, lotNumbers)
                                            Exit For
                                    End Select
                                End If
                            Next

                            If sendList <> String.Empty Then
                                Server.Listener.SendResponse(ClientSocket, "1*" + sendList)
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*NO pallets were found with this item loaded")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerInfo)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETLINESBYITEM*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim dateSearch As Boolean = Convert.ToBoolean(ClientData.Split("|")(1))
                    Dim dateFrom As String = ClientData.Split("|")(2)
                    Dim dateTo As String = ClientData.Split("|")(3)
                    Dim headerInfo As String = String.Empty
                    If dateSearch Then
                        headerInfo = Palletizing.RTIS.Retreive.UI_GetPalletsByItemAndDate(itemCode, dateFrom, dateTo)
                    Else
                        headerInfo = Palletizing.RTIS.Retreive.UI_GetPalletsByItem(itemCode)
                    End If

                    Select Case headerInfo.Split("*")(0)
                        Case "1"
                            headerInfo = headerInfo.Remove(0, 2)
                            Dim sendList As String = String.Empty
                            Dim allHeaderInfo As List(Of String) = headerInfo.Split("~").ToList()
                            For Each header As String In allHeaderInfo
                                If header <> String.Empty Then
                                    Dim id As String = header.Split("|")(0)
                                    Dim _date As String = header.Split("|")(1)
                                    Dim palletNum As String = header.Split("|")(2)
                                    Dim lotNumbers As String = Palletizing.RTIS.Retreive.UI_GetPalletLots(id)
                                    Select Case lotNumbers.Split("*")(0)
                                        Case "1"
                                            lotNumbers = lotNumbers.Remove(0, 2)
                                            sendList += id + "|" + palletNum + "|" + itemCode + "|" + _date + "|" + lotNumbers + "~"
                                        Case "0"
                                            'Ignor
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, lotNumbers)
                                            Exit For
                                    End Select
                                End If
                            Next

                            If sendList <> String.Empty Then
                                Server.Listener.SendResponse(ClientSocket, "1*" + sendList)
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*NO pallets were found with this item loaded")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerInfo)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETLINESBYLOT*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim dateSearch As Boolean = Convert.ToBoolean(ClientData.Split("|")(2))
                    Dim dateFrom As String = ClientData.Split("|")(3)
                    Dim dateTo As String = ClientData.Split("|")(4)

                    Dim headerIDs As String = String.Empty
                    If dateSearch Then
                        headerIDs = Palletizing.RTIS.Retreive.UI_GetPalletsByLotAndDate(lotNumber, dateFrom, dateTo)
                    Else
                        headerIDs = Palletizing.RTIS.Retreive.UI_GetPalletsByLot(lotNumber)
                    End If

                    Select Case headerIDs.Split("*")(0)
                        Case "1"
                            headerIDs = headerIDs.Remove(0, 2)
                            headerIDs = headerIDs.Substring(0, headerIDs.Length - 1)
                            Dim allIDs As String() = headerIDs.Split("~")
                            Dim inIds As String = "(" + String.Join(",", allIDs) + ")"
                            Dim headerInfo As String = Palletizing.RTIS.Retreive.UI_GetPalletsByIDList(inIds)
                            Select Case headerInfo.Split("*")(0)
                                Case "1"
                                    headerInfo = headerInfo.Remove(0, 2)
                                    Dim sendList As String = String.Empty
                                    Dim allHeaderInfo As List(Of String) = headerInfo.Split("~").ToList()
                                    For Each header As String In allHeaderInfo
                                        If header <> String.Empty Then
                                            Dim id As String = header.Split("|")(0)
                                            Dim _date As String = header.Split("|")(1)
                                            Dim palletNum As String = header.Split("|")(2)
                                            Dim lotNumbers As String = Palletizing.RTIS.Retreive.UI_GetPalletLots(id)
                                            Select Case lotNumbers.Split("*")(0)
                                                Case "1"
                                                    lotNumbers = lotNumbers.Remove(0, 2)
                                                    sendList += id + "|" + palletNum + "|" + itemCode + "|" + _date + "|" + lotNumbers + "~"
                                                Case "0"
                                                    'Ignor
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, lotNumbers)
                                                    Exit For
                                            End Select
                                        End If
                                    Next

                                    If sendList <> String.Empty Then
                                        Server.Listener.SendResponse(ClientSocket, "1*" + sendList)
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*NO pallets were found with this item loaded")
                                    End If
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, headerInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerIDs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETLINES*"
                Try
                    Dim id As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Palletizing.RTIS.Retreive.UI_GetPalletLines(id))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REPRINTPALLETLABEL*"
                Try
                    Dim palletCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = Barcodes.GetItemCode(palletCode)
                    Dim allLots As String = String.Empty
                    Dim label As String = String.Empty
                    Dim printer As String = String.Empty
                    Dim allUnqs As String = Unique.RTSQL.Retreive.MBL_GetPalletLots(palletCode)
                    Select Case allUnqs.Split("*")(0)
                        Case "1"
                            allUnqs = allUnqs.Remove(0, 2)
                            Dim Unqs As String() = allUnqs.Split("~")
                            For Each unq As String In Unqs
                                If unq <> String.Empty Then
                                    allLots += unq + ","
                                End If
                            Next
                            allLots = allLots.Substring(0, allLots.Length - 1)

                            Dim printSettings As String = Palletizing.RTIS.Retreive.MBL_GetPalletPrintSettings()
                            Select Case printSettings.Split("*")(0)
                                Case "1"
                                    printSettings = printSettings.Remove(0, 2)
                                    Dim allSettings As String() = printSettings.Split("~")
                                    For Each setting As String In allSettings
                                        If setting <> String.Empty Then
                                            Dim settingInfo As String() = setting.Split("|")
                                            If settingInfo(0) = "Pallet Label" Then
                                                label = settingInfo(1)
                                            End If

                                            If settingInfo(0) = "Pallet Printer" Then
                                                printer = settingInfo(1)
                                            End If
                                        End If
                                    Next

                                    Dim itemInfo As String = Palletizing.Evolution.Retreive.MBL_GetItemInfo(itemCode)
                                    Select Case itemInfo.Split("*")(0)
                                        Case "1"
                                            itemInfo = itemInfo.Remove(0, 2)
                                            Dim allItemInfo As String() = itemInfo.Split("|")
                                            Dim rep As New XtraReport()
                                            rep.LoadLayout(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\" + label)

                                            For Each item As DevExpress.XtraReports.Parameters.Parameter In rep.Parameters
                                                If item.Name = "_barcode" Then
                                                    item.Value = allItemInfo(0)
                                                End If

                                                If item.Name = "_RT2D" Then
                                                    item.Value = palletCode
                                                End If

                                                If item.Name = "_Date" Then
                                                    item.Value = DateTime.Now.ToString()
                                                End If

                                                If item.Name = "_Qty" Then
                                                    item.Value = "1"
                                                End If

                                                If item.Name = "_SimpleCode" Then
                                                    item.Value = allItemInfo(1)
                                                End If

                                                If item.Name = "_LotNumbers" Then
                                                    item.Value = allLots
                                                End If

                                                If item.Name = "_ItemCode" Then
                                                    item.Value = itemCode
                                                End If

                                                If item.Name = "_Desc1" Then
                                                    item.Value = allItemInfo(2)
                                                End If

                                                If item.Name = "_Desc2" Then
                                                    item.Value = allItemInfo(3)
                                                End If

                                                If item.Name = "_Desc3" Then
                                                    item.Value = allItemInfo(4)
                                                End If
                                            Next

                                            rep.CreateDocument()
                                            Dim rpt As ReportPrintTool = New ReportPrintTool(rep)
                                            rpt.PrinterSettings.Copies = Convert.ToInt16(1)
                                            rpt.Print(printer)
                                            Server.Listener.SendResponse(ClientSocket, "1*SUCCESS")
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, itemInfo)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, printSettings)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, allUnqs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "Old"

#Region "Slurry Manufacture (To be retired)"

            Case "*GETFRESHSLURRYLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Retreive.UI_GetFreshSlurryMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEFSMANUFACTURED*"
                Try
                    Dim LineID As String = ClientData.Split("|")(0)
                    Dim UserName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.UI_setFSManufactured(LineID, UserName))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region
#Region "FG Manufacture (To be retired)"

            Case "*GETZECT1FGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FGManufacture.RTIS.Retrieve.UI_GetZECT1FGMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETZECT2FGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FGManufacture.RTIS.Retrieve.UI_GetZECT2FGMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWFGLINES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, FGManufacture.RTIS.Retrieve.UI_GetAWFGMF())

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

            Case "*UPDATEZECTALLFGMANUFACTURED*"
                Try
                    Dim LineID As String = ClientData.Split("|")(0)
                    Dim UserName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FGManufacture.RTIS.Update.UI_setZECTALLFGManufactured(LineID, UserName))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEAWFGMANUFACTURED*"
                Try
                    Dim LineID As String = ClientData.Split("|")(0)
                    Dim UserName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FGManufacture.RTIS.Update.UI_setAWFGManufactured(LineID, UserName))

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#End Region

#End Region

#Region "PGM"

            Case "*PGMLOGIN*"
                Try
                    Dim userPin As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.MBL_GetUsersname(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMUSERPERMS*"
                Try
                    Dim userName As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Core.RTSQL.Retreive.PGM_GetModuleUserPermission(userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#Region "Transfer From Vault"
            Case "*PGMGETFVITEMINFO*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, PGM.Evolutiom.Retreive.PGM_GetItemFVDescription(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PGMTRANSFERFROMVAULT*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim WhseFrom As String = ClientData.Split("|")(2)
                    Dim WhseTo As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim procCode As String = ClientData.Split("|")(6)
                    Dim unq As String = ClientData.Split("|")(7)
                    Dim transferDescription As String = "PGM Transfer from Vault"

                    Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                    Select Case whseFromOK.Split("*")(0)
                        Case "1"
                            Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                            Select Case whseToOK.Split("*")(0)
                                Case "1"

                                    Dim tranferredIn As String = Unique.RTSQL.Retreive.MBL_GetItemTransferredOut(itemCode, lotNumber, qty, unq)
                                    Select Case tranferredIn.Split("*")(0)
                                        Case "1"
                                            tranferredIn = tranferredIn.Remove(0, 2)
                                            If Convert.ToBoolean(tranferredIn) = False Then
                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode, transferDescription, "Pending")
                                                Select Case transferSaved.Split("*")(0)
                                                    Case "1"
                                                        Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                                        Select Case transferLogged.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemTransferredOut(itemCode, lotNumber, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                End Select

                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "0*The item has already been received!")
                                            End If
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, tranferredIn)
                                    End Select

                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, whseToOK)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, whseFromOK)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Receiving"
            Case "*PGMGETWHSESREC*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetPGMReceivingWhses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PGMGETFROMWHSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetPGMRecWhses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PGMGETRECITEMINFO*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, PGM.Evolutiom.Retreive.PGM_GetItemRecDescription(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PGMRECEIVETRANS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim WhseFrom As String = ClientData.Split("|")(2)
                    Dim WhseTo As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim procCode As String = ClientData.Split("|")(6)
                    Dim unq As String = ClientData.Split("|")(7)
                    Dim transferDescription As String = "PGM Receiving Transfer"

                    Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                    Select Case whseFromOK.Split("*")(0)
                        Case "1"
                            Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                            Select Case whseToOK.Split("*")(0)
                                Case "1"

                                    Dim tranferredIn As String = Unique.RTSQL.Retreive.MBL_GetItemReceivedTransfer(itemCode, lotNumber, qty, unq)
                                    Select Case tranferredIn.Split("*")(0)
                                        Case "1"
                                            tranferredIn = tranferredIn.Remove(0, 2)
                                            If Convert.ToBoolean(tranferredIn) = False Then
                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode, transferDescription, "Pending")
                                                Select Case transferSaved.Split("*")(0)
                                                    Case "1"
                                                        Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                                        Select Case transferLogged.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemReceived(itemCode, lotNumber, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                End Select

                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "0*The item has already been received!")
                                            End If
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, tranferredIn)
                                    End Select

                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, whseToOK)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, whseFromOK)
                    End Select


                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Manufacture"
            Case "*PGMGETREMCAPTURED*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetRemainderCaptured(itemCode, lot, location))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMGETBATCHLINES*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim location As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetBatchLines(itemCode, lot, location))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMMANUFACTURECONTAINER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim container As String = ClientData.Split("|")(2)
                    Dim weight As String = ClientData.Split("|")(3)
                    Dim concentration As String = ClientData.Split("|")(4)
                    Dim location As String = ClientData.Split("|")(5)
                    Dim userName As String = ClientData.Split("|")(6)

                    Dim containerUsed As String = PGM.RTSQL.Retreive.PGM_CheckContainerUsed_Global(container)
                    Select Case containerUsed.Split("*")(0)
                        Case "1"
                            containerUsed = containerUsed.Remove(0, 2)
                            If containerUsed = String.Empty Then
#Region "Insert or Update Batch"
                                Dim batchFound As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                                Select Case batchFound.Split("*")(0)
                                    Case "1"
                                        Dim headerId As String = batchFound.Remove(0, 2)
                                        Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateHeader(itemCode, lot, weight.Replace(",", "."), location)
                                        Select Case headerUpdated.Split("*")(0)
                                            Case "1"
                                                Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerId, container, weight.Replace(",", "."), userName))
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                        End Select
                                    Case "0"
                                        Dim headerID As String = PGM.RTSQL.Insert.PGM_ManufactureHeader(itemCode, lot, weight.Replace(",", "."), weight.Replace(",", "."), concentration.Replace(",", "."), location, userName)
                                        Select Case headerID.Split("*")(0)
                                            Case "1"
                                                headerID = headerID.Remove(0, 2)
                                                Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerID, container, weight.Replace(",", "."), userName))
                                            Case Else
                                                Server.Listener.SendResponse(ClientSocket, headerID)
                                        End Select
                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, batchFound)
                                End Select
#End Region
                            ElseIf Convert.ToBoolean(containerUsed) Then
                                Dim containerRec = PGM.RTSQL.Retreive.PGM_CheckContRecState_Global(container)
                                Select Case containerRec.Split("*")(0)
                                    Case "1"
                                        containerRec = containerRec.Remove(0, 2)
                                        If Convert.ToBoolean(containerRec) Then
#Region "Insert or Update Batch"
                                            Dim batchFound As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                                            Select Case batchFound.Split("*")(0)
                                                Case "1"
                                                    Dim headerId As String = batchFound.Remove(0, 2)
                                                    Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateHeader(itemCode, lot, weight.Replace(",", "."), location)
                                                    Select Case headerUpdated.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerId, container, weight.Replace(",", "."), userName))
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                    End Select
                                                Case "0"
                                                    Dim headerID As String = PGM.RTSQL.Insert.PGM_ManufactureHeader(itemCode, lot, weight.Replace(",", "."), weight.Replace(",", "."), concentration.Replace(",", "."), location, userName)
                                                    Select Case headerID.Split("*")(0)
                                                        Case "1"
                                                            headerID = headerID.Remove(0, 2)
                                                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerID, container, weight.Replace(",", "."), userName))
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, headerID)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, batchFound)
                                            End Select
#End Region
                                        Else
                                            'REFILL
                                            Server.Listener.SendResponse(ClientSocket, "2*The container has already been used in this batch and has not been received, would you like to refill it?" + Environment.NewLine + "This will automatically transfer the previous contents of the container")
                                        End If
                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, containerRec)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The container is in use and has not been transferred out")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, containerUsed)
                    End Select

                    'Dim batchFound As String = PGM.RTSQL.Retreive.PGM_GetItemBatch(itemCode, lot, location)
                    'Select Case batchFound.Split("*")(0)
                    '    Case "1"
                    '        Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                    '        Select Case headerId.Split("*")(0)
                    '            Case "1"
                    '                headerId = headerId.Remove(0, 2)

                    '            Case "-1"
                    '                Server.Listener.SendResponse(ClientSocket, headerId)
                    '        End Select
                    '    Case "0"
                    '        'Add new
                    '        Dim headerAdded As String = PGM.RTSQL.Insert.PGM_ManufactureHeader(itemCode, lot, weight.Replace(",", "."), weight.Replace(",", "."), concentration.Replace(",", "."), location, userName)
                    '        Select Case headerAdded.Split("*")(0)
                    '            Case "1"
                    '                Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                    '                Select Case headerId.Split("*")(0)
                    '                    Case "1"
                    '                        headerId = headerId.Remove(0, 2)
                    '                        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerId, container, weight.Replace(",", "."), userName))
                    '                    Case "-1"
                    '                        Server.Listener.SendResponse(ClientSocket, headerId)
                    '                End Select
                    '            Case "-1"
                    '                Server.Listener.SendResponse(ClientSocket, headerAdded + Environment.NewLine + "Data Returned: " + ClientData)
                    '        End Select
                    '    Case Else
                    '        Server.Listener.SendResponse(ClientSocket, batchFound)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMREFILLCONTAINER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim container As String = ClientData.Split("|")(2)
                    Dim weight As String = ClientData.Split("|")(3)
                    Dim concentration As String = ClientData.Split("|")(4)
                    Dim location As String = ClientData.Split("|")(5)
                    Dim userName As String = ClientData.Split("|")(6)

                    Dim oldContInfo As String = PGM.RTSQL.Retreive.PGM_GetContOldInfo(container)
                    Select Case oldContInfo.Split("*")(0)
                        Case "1"
                            oldContInfo = oldContInfo.Remove(0, 2)
                            Dim oldContQty As String = oldContInfo.Split("|")(0)
                            Dim oldContTo As String = oldContInfo.Split("|")(1)
                            Dim oldContDest As String = oldContInfo.Split("|")(2)
                            Dim oldContID As String = oldContInfo.Split("|")(3)
                            Dim oldItemCode As String = oldContInfo.Split("|")(4)
                            Dim oldLot As String = oldContInfo.Split("|")(5)
                            Dim transferred As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(oldItemCode, oldLot, oldContTo, oldContDest, oldContQty.Replace(",", "."), userName, "PGM")
                            Select Case transferred.Split("*")(0)
                                Case "1"
                                    Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lot, oldContTo, oldContDest, oldContQty.Replace(",", "."), userName, "PGM")
                                    Select Case transferLogged.Split("*")(0)
                                        Case "1"
                                            Dim received As String = PGM.RTSQL.Update.PGM_updateContRec(oldContID)
                                            Select Case received.Split("*")(0)
                                                Case "1"
#Region "Insert or Update Batch"
                                                    Dim batchFound As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                                                    Select Case batchFound.Split("*")(0)
                                                        Case "1"
                                                            Dim headerId As String = batchFound.Remove(0, 2)
                                                            Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateHeader(itemCode, lot, weight.Replace(",", "."), location)
                                                            Select Case headerUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerId, container, weight.Replace(",", "."), userName))
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                            End Select
                                                        Case "0"
                                                            Dim headerID As String = PGM.RTSQL.Insert.PGM_ManufactureHeader(itemCode, lot, weight.Replace(",", "."), weight.Replace(",", "."), concentration.Replace(",", "."), location, userName)
                                                            Select Case headerID.Split("*")(0)
                                                                Case "1"
                                                                    headerID = headerID.Remove(0, 2)
                                                                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerID, container, weight.Replace(",", "."), userName))
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                                            End Select
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, batchFound)
                                                    End Select
#End Region

                                                    'Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateHeader(itemCode, lot, weight.Replace(",", "."), location)
                                                    'Select Case headerUpdated.Split("*")(0)
                                                    '    Case "1"
                                                    '        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Insert.PGM_ManufactureLine(headerId, container, weight.Replace(",", "."), userName))
                                                    '    Case "-1"
                                                    '        Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                    'End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, transferLogged)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, transferLogged)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, oldContInfo)
                    End Select

                    'Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lot, location)
                    'Select Case headerId.Split("*")(0)
                    '    Case "1"
                    '        headerId = headerId.Remove(0, 2)


                    '    Case Else
                    '        Server.Listener.SendResponse(ClientSocket, headerId)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMEDITCONTAINERWEIGHT*"
                Try
                    Dim container As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim oldWeight As String = ClientData.Split("|")(3)
                    Dim newWeight As String = ClientData.Split("|")(4)
                    Dim whse As String = ClientData.Split("|")(5)

                    Dim dOld As Double = Convert.ToDouble(oldWeight.Replace(",", sep).Replace(".", sep))
                    Dim dNew As Double = Convert.ToDouble(newWeight.Replace(",", sep).Replace(".", sep))
                    Dim diff As String = Convert.ToDouble(dNew - dOld)
                    Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, whse)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim pgmManuf As String = PGM.RTSQL.Retreive.PGM_GetCheckContainerManuf(headerId, container)
                            Select Case pgmManuf.Split("*")(0)
                                Case "1"
                                    pgmManuf = pgmManuf.Remove(0, 2)
                                    If Convert.ToBoolean(pgmManuf) = False Then
                                        Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateContQtyHeader(itemCode, lotNumber, diff.Replace(",", "."), whse)
                                        Select Case headerUpdated.Split("*")(0)
                                            Case "1"
                                                Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.PGM_updateContQtyLine(headerId, container, diff.Replace(",", ".")))
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*Container already manufacturd and cannot be edited")
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, pgmManuf)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, pgmManuf)
                            End Select

                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMSETREMAINDER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim concentration As String = ClientData.Split("|")(2)
                    Dim whse As String = ClientData.Split("|")(3)
                    Dim remainder As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    'PGM_setBatchRemainder
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.PGM_setBatchRemainder(remainder, itemCode, lot, concentration, whse, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
#End Region

#Region "Transfer To WIP"
            Case "*PGMGETTRANSFERITEMS*"
                Try
                    Dim transferType As String = ClientData
                    Select Case transferType
                        Case "VW"
                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_SelectVWTransferSlurries())
                        Case "TPP"
                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_SelectTTransferPowders())
                        Case "TSP"
                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_SelectTTransferSlurries())
                        Case "TAW"
                            Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_SelectTTransferAW())
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Invalid transfer type selected")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
            Case "*PGMTRANSFERTOWIP*"
                Try
                    Dim container As String = ClientData.Split("|")(0)
                    Dim product As String = ClientData.Split("|")(1)
                    Dim batch As String = ClientData.Split("|")(2)
                    Dim whseFrom As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim transferType As String = ClientData.Split("|")(5)
                    Dim whseTo As String = String.Empty '"WIP-FS"
                    Dim whseDest As String = String.Empty

                    Dim procCode As String = "PGM"
                    Dim transferDescription As String = "PGM Transfer to IT"
                    'Select Case procCode
                    '    Case "PPtFS"
                    '        transferDescription = "Powder Prep Transfer from IT"
                    '    Case "FStMS"
                    '        transferDescription = "Fresh Slurry Transfer from IT"
                    '    Case "MStZECT"
                    '        transferDescription = "MIxed Slurry Transfer from IT"
                    '    Case "AW"
                    '        transferDescription = "AW Transfer from IT"
                    '    Case "Canning"
                    '        transferDescription = "Canning Transfer from IT"
                    'End Select
                    Select Case transferType
                        Case "VW"
                            whseTo = PGM.RTSQL.Retreive.PGM_GetVWWIPLoc()
                            whseDest = PGM.RTSQL.Retreive.PGM_GetVWDestLoc()
                            'whseTo = "WIP-FS"
                        Case "TPP"
                            whseTo = PGM.RTSQL.Retreive.PGM_GetTPWIPLoc()
                            whseDest = PGM.RTSQL.Retreive.PGM_GetTPDestLoc()
                            'whseTo = "WIP-PP"
                        Case "TSP"
                            whseTo = PGM.RTSQL.Retreive.PGM_GetTSWIPLoc()
                            whseDest = PGM.RTSQL.Retreive.PGM_GetTSDestLoc()
                            'hseTo = "WIP-FS"
                        Case "TAW"
                            whseTo = PGM.RTSQL.Retreive.PGM_GetTAWWIPLoc()
                            whseDest = PGM.RTSQL.Retreive.PGM_GetTAWDestLoc()
                            'whseTo = "WIP-AW"
                    End Select

                    Select Case whseTo.Split("*")(0)
                        Case "1"
                            whseTo = whseTo.Remove(0, 2)
                            Select Case whseDest.Split("*")(0)
                                Case "1"
                                    whseDest = whseDest.Remove(0, 2)
                                    Dim containerInfo As String = PGM.RTSQL.Retreive.PGM_GetContainerInfo(container, whseFrom)
                                    Select Case containerInfo.Split("*")(0)
                                        Case "1"
                                            containerInfo = containerInfo.Remove(0, 2)
                                            Dim itemCode As String = containerInfo.Split("|")(0)
                                            Dim lotNumber As String = containerInfo.Split("|")(1)
                                            Dim qty As String = containerInfo.Split("|")(2)

                                            Dim itemAllowed As String = String.Empty
                                            Select Case transferType
                                                Case "VW"
                                                    itemAllowed = PGM.RTSQL.Retreive.PGM_CheckItemAllowedVW(product, itemCode)
                                                Case "TPP"
                                                    itemAllowed = PGM.RTSQL.Retreive.PGM_CheckItemAllowedTP(product, itemCode)
                                                Case "TSP"
                                                    itemAllowed = PGM.RTSQL.Retreive.PGM_CheckItemAllowedTS(product, itemCode)
                                                Case "TAW"
                                                    itemAllowed = PGM.RTSQL.Retreive.PGM_CheckItemAllowedTAW(product, itemCode)
                                            End Select

                                            Select Case itemAllowed.Split("*")(0)
                                                Case "1"
                                                    Dim headerId As String = PGM.RTSQL.Retreive.PGM_GetHeaderID(itemCode, lotNumber, whseFrom)
                                                    Select Case headerId.Split("*")(0)
                                                        Case "1"
                                                            headerId = headerId.Remove(0, 2)
                                                            Dim itemTransferred As String = PGM.RTSQL.Retreive.PGM_CheckItemTransferred(container, headerId)
                                                            Select Case itemTransferred.Split("*")(0)
                                                                Case "1"
                                                                    itemTransferred = itemTransferred.Remove(0, 2)
                                                                    If Convert.ToBoolean(itemTransferred) = True Then
                                                                        Dim transferAdded As String = PGM.RTSQL.Insert.PGM_AddTransferOutLine(headerId, container, qty.Replace(",", "."), username, whseFrom, whseTo, product, batch, whseDest)
                                                                        Select Case transferAdded.Split("*")(0)
                                                                            Case "1"
                                                                                Dim manufLineUpdated As String = PGM.RTSQL.Update.PGM_updateManufOut(container, headerId)
                                                                                Select Case manufLineUpdated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Dim headerUpdated As String = PGM.RTSQL.Update.PGM_updateHeaderQtyOut(itemCode, lotNumber, qty, whseFrom)
                                                                                        Select Case headerUpdated.Split("*")(0)
                                                                                            Case "1"
                                                                                                'Dim transferred As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(itemCode, lotNumber, whseFrom, whseTo, qty.Replace(",", "."), username, "PGM")
                                                                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, whseFrom, whseTo, qty, username, procCode, transferDescription, "Pending")

                                                                                                Select Case transferSaved.Split("*")(0)
                                                                                                    Case "1"
                                                                                                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, batch, whseFrom, whseTo, qty.Replace(",", "."), username, "PGM"))
                                                                                                    Case "-1"
                                                                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                                                                End Select
                                                                                            Case "-1"
                                                                                                Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                                                        End Select
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, manufLineUpdated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, transferAdded)
                                                                        End Select
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "0*The container scanned has an unreceived outgoing transfer")
                                                                    End If
                                                                Case "0"
                                                                    Server.Listener.SendResponse(ClientSocket, itemTransferred)
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, itemTransferred)
                                                            End Select
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, headerId)
                                                    End Select
                                                Case "0"
                                                    Server.Listener.SendResponse(ClientSocket, itemAllowed)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, itemAllowed)
                                            End Select
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, containerInfo)
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, containerInfo)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, whseDest)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, whseTo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, "-1*" + ex.Message)
                End Try
#End Region

#Region "Transaction Viewer"
            Case "*PGMGETITEMLIST*"
                Try
                    Dim whseCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetAllPGM(whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMTRANSACTIONSHEADER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetAllPGMItemHeaders(itemCode, whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMTRANSACTIONSHEADERROWS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Dim rowCount As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetAllPGMItemHeaderRows(itemCode, whseCode, rowCount))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETITEMTRANSACTIONSBATCHSPEC*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim batch As String = ClientData.Split("|")(1)
                    Dim whseCode As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetAllPGMItemTransactions(itemCode, batch, whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
                'Case "*GETITEMTRANSACTIONS*"
                '    Try
                '        Dim itemCode As String = ClientData.Split("|")(0)
                '        Dim whseCode As String = ClientData.Split("|")(1)
                '        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Retreive.PGM_GetAllPGMItemTransactions(itemCode, whseCode))
                '    Catch ex As Exception
                '        Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                'End Try

#End Region

#End Region

#Region "MBL"

#Region "General"
            Case "*GETOUTTRANSDESC*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, WhseTransfers.Evolution.Retreive.GetItemDesc(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "PO Receiving"
            Case "*GETUSERNAME*"
                Try
                    Dim userPin As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.MBL_GetUsersname(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CREATEPODB*"
                Try
                    Dim orderNum As String = ClientData.Split("|")(0)
                    Dim scannerID As String = ClientData.Split("|")(1)
                    Dim poExists As String = POReceiving.Evolution.Retreive.MBL_CheckPOExists(orderNum)
                    Select Case poExists.Split("*")(0)
                        Case "1"
                            If Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC") = False Then
                                Directory.CreateDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC")
                            End If

                            If File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3") Then
                                File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3")
                            End If

                            Dim created As String = SQLite.Create.createPODB(orderNum)
                            Select Case created.Split("*")(0)
                                Case "1"
                                    Dim poLines As String = POReceiving.Evolution.Retreive.MBL_GetEvoPOLines(orderNum)
                                    Select Case poLines.Split("*")(0)
                                        Case "1"
                                            poLines = poLines.Remove(0, 2)
                                            poLines = poLines.Substring(0, poLines.Length - 1)
                                            Dim inserted As String = SQLite.Insert.inserPOLines(orderNum, poLines)
                                            Select Case inserted.Split("*")(0)
                                                Case "1"
                                                    Dim unqPOLines As String = POReceiving.RTSQL.Retreive.MBL_CheckPOUnqLines(orderNum)
                                                    Select Case unqPOLines.Split("*")(0)
                                                        Case "1"
                                                            unqPOLines = unqPOLines.Remove(0, 2)
                                                            unqPOLines = unqPOLines.Substring(0, unqPOLines.Length - 1)
                                                            Dim unqInserted As String = SQLite.Insert.inserPOLinesUnq(orderNum, unqPOLines)
                                                            Select Case unqInserted.Split("*")(0)
                                                                Case "1"
                                                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, unqInserted)
                                                            End Select
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, unqPOLines)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, inserted)
                                            End Select

#Region "Unused"
                                                                                                        'Dim failed As Boolean = False
                                                                                                        'Dim failureReason As String = String.Empty
                                                                                                        'poLines = poLines.Remove(0, 2)
                                                                                                        'Dim allLines As String() = poLines.Split("~")
                                                                                                        'For Each line As String In allLines
                                                                                                        '    If line <> String.Empty Then
                                                                                                        '        Dim code As String = line.Split("|")(0)
                                                                                                        '        Dim desc As String = line.Split("|")(1)
                                                                                                        '        Dim desc2 As String = line.Split("|")(2)
                                                                                                        '        Dim isLot As String = line.Split("|")(3)
                                                                                                        '        Dim lotNum As String = line.Split("|")(4)
                                                                                                        '        Dim qty As String = line.Split("|")(5)
                                                                                                        '        Dim qtyToProc As String = line.Split("|")(6)
                                                                                                        '        Dim qtyProc As String = line.Split("|")(7)
                                                                                                        '        Dim viewableQty As String = line.Split("|")(8)
                                                                                                        '        Dim printQty As String = line.Split("|")(9)

                                                                                                        '        Dim inserted As String = SQLite.Insert.inserPOLine(orderNum, code, desc, desc2, isLot, lotNum, qty, qtyToProc, qtyProc, viewableQty, printQty)
                                                                                                        '        Select Case inserted.Split("*")(0)
                                                                                                        '            Case "1"

                                                                                                        '            Case "-1"
                                                                                                        '                failureReason = inserted
                                                                                                        '                failed = True
                                                                                                        '                Exit For
                                                                                                        '        End Select
                                                                                                        '    End If
                                                                                                        'Next

                                                                                                        'If failed = False Then
                                                                                                        '    Dim unqPOLines As String = POReceiving.RTSQL.Retreive.MBL_CheckPOUnqLines(orderNum)
                                                                                                        '    Select Case unqPOLines.Split("*")(0)
                                                                                                        '        Case "1"
                                                                                                        '            unqPOLines = unqPOLines.Remove(0, 2)
                                                                                                        '            Dim allUnq As String() = unqPOLines.Split("~")

                                                                                                        '            Dim failed2 As Boolean = False
                                                                                                        '            Dim failureReason2 As String = String.Empty
                                                                                                        '            For Each unq As String In allUnq
                                                                                                        '                If unq <> String.Empty Then
                                                                                                        '                    Dim barcode As String = unq.Split("*")(0)
                                                                                                        '                    Dim rec As String = unq.Split("*")(1)
                                                                                                        '                    Dim valid As String = unq.Split("*")(2)
                                                                                                        '                    Dim inserted As String = SQLite.Insert.insertUnq(orderNum, barcode, rec, valid)
                                                                                                        '                    Select Case inserted.Split("*")(0)
                                                                                                        '                        Case "1"

                                                                                                        '                        Case "-1"
                                                                                                        '                            failureReason2 = inserted
                                                                                                        '                            failed2 = True
                                                                                                        '                            Exit For
                                                                                                        '                    End Select
                                                                                                        '                End If
                                                                                                        '            Next

                                                                                                        '            If failed2 = False Then
                                                                                                        '                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                                                        '            Else
                                                                                                        '                Server.Listener.SendResponse(ClientSocket, failureReason2)
                                                                                                        '            End If
                                                                                                        '        Case "-1"
                                                                                                        '            Server.Listener.SendResponse(ClientSocket, unqPOLines)
                                                                                                        '    End Select
                                                                                                        'Else
                                                                                                        '    Server.Listener.SendResponse(ClientSocket, failureReason)
                                                                                                        'End If
#End Region
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, poLines)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, created)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, poExists)
                    End Select
                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETOFFLINEPODATABASE*"
                Try
                    Dim orderNum As String = ClientData
                    Server.Listener.SendResponseFile(ClientSocket, "1*Success", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3")
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MBLPOSTPOLINESINGLE*"
                Try
                    Dim orderNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNum As String = ClientData.Split("|")(2)
                    Dim toPrcocess As String = ClientData.Split("|")(3)
                    Dim posted As String = POReceiving.EvolutionSDK.MBL_UpdatePOItemRec(orderNum, itemCode, lotNum, toPrcocess)
                    Server.Listener.SendResponse(ClientSocket, posted)
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*MBLPOSTPOLINESUNQ*"
                Try
                    Dim orderNum As String = ClientData.Split("*")(0)
                    Dim unqs As String = ClientData.Split("*")(1)

                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty

                    Dim allUnq As String() = unqs.Split("~")
                    For Each unq As String In allUnq
                        If unq <> String.Empty Then
                            Dim updated As String = POReceiving.RTSQL.Update.MBL_SetUnqReceived(orderNum, unq)
                            Select Case updated.Split("*")(0)
                                Case "1"

                                Case "-1"
                                    failed = True
                                    failureReason = updated
                            End Select
                        End If
                    Next

                    If failed = False Then
                        Server.Listener.SendResponse(ClientSocket, "1*Success")
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

            Case "*MBLPOSTPOLINES*"
                Try
                    Dim orderNum As String = ClientData.Split("*")(0)
                    Dim orderLines As String = ClientData.Split("*")(1)
                    Dim unqLines As String = ClientData.Split("*")(2)
                    Dim allOrderLines As String() = orderLines.Split("~")

                    Dim failed As Boolean = False
                    Dim failureReason As String = String.Empty
                    For Each line As String In allOrderLines
                        If line <> String.Empty Then
                            Dim itemCode As String = line.Split("|")(0)
                            Dim lotNum As String = line.Split("|")(1)
                            Dim toPrcocess As String = line.Split("|")(2)
                            Dim posted As String = POReceiving.EvolutionSDK.MBL_UpdatePOItemRec(orderNum, itemCode, lotNum, toPrcocess)
                            Select Case posted.Split("*")(0)
                                Case "1"

                                Case "-1"
                                    failed = True
                                    failureReason = posted
                            End Select
                        End If
                    Next

                    If failed = False Then
                        Dim allUnq As String() = unqLines.Split("~")

                        Dim failed2 As Boolean = False
                        Dim failureReason2 As String = String.Empty
                        For Each unq As String In allUnq
                            If unq <> String.Empty Then
                                Dim updated As String = POReceiving.RTSQL.Update.MBL_SetUnqReceived(orderNum, unq)
                                Select Case updated.Split("*")(0)
                                    Case "1"

                                    Case "-1"
                                        failed2 = True
                                        failureReason2 = updated
                                End Select
                            End If
                        Next

                        If failed2 = False Then
                            Server.Listener.SendResponse(ClientSocket, "1*Success")
                        Else
                            Server.Listener.SendResponse(ClientSocket, failureReason2)
                        End If
                    Else
                        Server.Listener.SendResponse(ClientSocket, failureReason)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Mixed Slurry"

#Region "Start Mix"
            Case "*CHECKMIXEDSLURRYTANKINUSE*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)

                    Select Case tankType
                        Case "BTNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckMixedSlurryBufferTankInUse(tankType, tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.Evolution.Rereive.MBL_GetItemDesc(itemCode))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case "TNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckMixedSlurryTankInUse(tankType, tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.Evolution.Rereive.MBL_GetItemDesc(itemCode))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CLOSEBUFFERSLURRY*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)

                    Dim bufferSlurriesClosed As String = MixedSlurry.RTSQL.Update.MBL_closeBufferSlurries(tankType, tankCode, itemCode, username)
                    Select Case bufferSlurriesClosed.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.Evolution.Rereive.MBL_GetItemDesc(itemCode))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, bufferSlurriesClosed)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*STARTMIXEDSLURRY*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim itemDesc As String = ClientData.Split("|")(3)
                    Dim lotNumber As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.MBL_InsertMixedSlurryRecord(tankType, tankCode, itemCode, itemDesc, lotNumber, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Enter Remaining"
            Case "*CHECkMIXEDSLURRYREM*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_CheckMixedSlurryRemENtered(tankType, tankCode, itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETMIXEDSLURRYREMAINING*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim oldLot As String = ClientData.Split("|")(3)
                    Dim oldSolid As String = ClientData.Split("|")(4)
                    Dim wetWeight As String = ClientData.Split("|")(5)
                    Dim userName As String = ClientData.Split("|")(6)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_setMixedSlurryRemaining(tankType, tankCode, itemCode, wetWeight.Replace(",", "."), oldLot, oldSolid, userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Enter Recovered"
            Case "*CHECkMIXEDSLURRYREC*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_CheckMixedSlurryRecEntered(tankType, tankCode, itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETMIXEDSLURRYRECOVERED*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim oldLot As String = ClientData.Split("|")(3)
                    Dim oldSolid As String = ClientData.Split("|")(4)
                    Dim wetWeight As String = ClientData.Split("|")(5)
                    Dim userName As String = ClientData.Split("|")(6)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_setMixedSlurryRecovered(tankType, tankCode, itemCode, wetWeight.Replace(",", "."), oldLot, oldSolid, userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Add Fresh Slurry"
            Case "*CHECKTANKAVAILABLE*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Select Case tankType
                        Case "BTNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryInBufferTank(tankType, tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Dim headerID As String = tankInUse.Remove(0, 2)
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetTankFreshSlurries(headerID))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case "TNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryInTank(tankType, tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Dim headerID As String = tankInUse.Remove(0, 2)
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetTankFreshSlurries(headerID))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETFSTROLLEYINFOMS*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim trolleyNo As String = ClientData.Split("|")(3)
                    Dim slurrycode As String = ClientData.Split("|")(4)

                    Dim headerID As String = MixedSlurry.RTSQL.Retreive.MBL_GetMixedSlurryHeaderID(tankType, tankCode, itemCode)
                    Select Case headerID.Split("*")(0)
                        Case "1"
                            headerID = headerID.Remove(0, 2)
                            Dim trolleyInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetTrolleyInfo(trolleyNo, slurrycode)
                            Select Case trolleyInfo.Split("*")(0)
                                Case "1"
                                    trolleyInfo = trolleyInfo.Remove(0, 2)
                                    Dim lotNumber As String = trolleyInfo.Split("|")(0)
                                    Dim description As String = trolleyInfo.Split("|")(1)
                                    Server.Listener.SendResponse(ClientSocket, "1*" + lotNumber + "|" + description)
                                    'Dim trolleyAlreadyUsed As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryAlreadyInTank(headerID, trolleyNo, slurrycode, lotNumber)
                                    'Select Case trolleyAlreadyUsed.Split("*")(0)
                                    '    Case "1"

                                    '    Case "0"
                                    '        Server.Listener.SendResponse(ClientSocket, trolleyAlreadyUsed)
                                    '    Case "-1"
                                    '        Server.Listener.SendResponse(ClientSocket, trolleyAlreadyUsed)
                                    'End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, trolleyInfo)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, trolleyInfo)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDFRESHSLURRYTOMIX*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim itemDesc As String = ClientData.Split("|")(3)
                    Dim trolleyCode As String = ClientData.Split("|")(4)
                    Dim trolleyItem As String = ClientData.Split("|")(5)
                    Dim trolleyDesc As String = ClientData.Split("|")(6)
                    Dim trolleyLot As String = ClientData.Split("|")(7)
                    Dim wetWeight As String = ClientData.Split("|")(8)
                    Dim userName As String = ClientData.Split("|")(9)

                    Dim headerID As String = String.Empty
                    Select Case tankType
                        Case "BTNK"
                            headerID = MixedSlurry.RTSQL.Retreive.MBL_GetBufferTankSlurryID(tankType, tankNo, itemCode)
                        Case "TNK"
                            headerID = MixedSlurry.RTSQL.Retreive.MBL_GetTankSlurryID(tankType, tankNo, itemCode)
                        Case Else
                            headerID = "0*Please scan a valid slurry tank"
                    End Select

                    Select Case headerID.Split("*")(0)
                        Case "1"
                            headerID = headerID.Remove(0, 2)
                            Dim trolleyTolerance As String = MixedSlurry.RTSQL.Retreive.MBL_GetTrolleyTolerance()
                            Select Case trolleyTolerance.Split("*")(0)
                                Case "1"
                                    trolleyTolerance = trolleyTolerance.Remove(0, 2)
                                    Dim dTolerance As Double = Convert.ToDouble(trolleyTolerance.Replace(".", sep).Replace(",", sep))
                                    Dim freshSlurryInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetFreshSlurryTrolleyInfo(trolleyCode, trolleyItem)
                                    Select Case freshSlurryInfo.Split("*")(0)
                                        Case "1"
                                            freshSlurryInfo = freshSlurryInfo.Remove(0, 2)
                                            Dim trolleyID As String = freshSlurryInfo.Split("|")(0)
                                            Dim trolleyWeight As String = freshSlurryInfo.Split("|")(1)
                                            Dim totalDecanted As String = freshSlurryInfo.Split("|")(2)
                                            Dim totalDecntedWeight As Double = Convert.ToDouble(totalDecanted.Replace(".", sep).Replace(",", sep))

                                            Dim dEnteredWeight As Double = Convert.ToDouble(wetWeight.Replace(".", sep).Replace(",", sep))
                                            Dim dTrolleyWeight As Double = Convert.ToDouble(trolleyWeight.Replace(".", sep).Replace(",", sep))

                                            If dTrolleyWeight >= (totalDecntedWeight + dEnteredWeight) Then
                                                Dim allowedTol As Double = (dTolerance / 100) * dTrolleyWeight
                                                Dim diff As Double = dTrolleyWeight - dEnteredWeight

                                                Dim actualWeight As String = String.Empty
                                                Dim actualTolerance As String = String.Empty
                                                If diff <= allowedTol And totalDecntedWeight = 0 Then
                                                    'If weight entered is within the tolerance it is not a partial tank.
                                                    actualWeight = dTrolleyWeight
                                                    actualTolerance = trolleyTolerance
                                                Else
                                                    'If weight is not within the tolerance.
                                                    actualWeight = wetWeight
                                                    actualTolerance = "0"
                                                End If

                                                Dim slurryDecanted = MixedSlurry.RTSQL.Update.MBL_UpdateFreshSlurryDecantQty(trolleyID, wetWeight)
                                                Select Case slurryDecanted.Split("*")(0)
                                                    Case "1"
                                                        Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.MBL_InsertFreshSlurryRecord(headerID, tankNo, trolleyCode, trolleyItem, trolleyDesc, trolleyLot, actualWeight.Replace(",", "."), actualTolerance.Replace(",", "."), userName))
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, slurryDecanted)
                                                End Select
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, $"0*The quantity entered would exceed the amount of slurry in the container{Environment.NewLine}Remaining slurry weight: {Convert.ToString(dTrolleyWeight - totalDecntedWeight)}{Environment.NewLine}Attempted Weight: {wetWeight}")
                                            End If
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, freshSlurryInfo)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, trolleyTolerance)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Decant"
            Case "*CHECKBUFFERTANKFORDECANT*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)

                    Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryAvailableToDecant(tankType, tankCode, itemCode)
                    Select Case tankInUse.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, tankInUse)
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, tankInUse)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, tankInUse)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKDECANTTANK*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    If tankType = "MTNK" Then
                        Dim mobileTankAvailabe As String = MixedSlurry.RTSQL.Retreive.MBL_CheckMobileTankAvailable(tankCode, itemCode)
                        Select Case mobileTankAvailabe.Split("*")(0)
                            Case "1"
                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                            Case "0"
                                Server.Listener.SendResponse(ClientSocket, mobileTankAvailabe)
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, mobileTankAvailabe)
                        End Select
                    Else
                        Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDDECANTSLURRYLINE*"
                Try
                    Dim mainTankType As String = ClientData.Split("|")(0)
                    Dim mainTank As String = ClientData.Split("|")(1)
                    Dim mainItem As String = ClientData.Split("|")(2)
                    Dim mobileTank As String = ClientData.Split("|")(3)
                    Dim mobileItem As String = ClientData.Split("|")(4)
                    Dim wetWeight As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)

                    Dim headerInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetMixedSlurryHeaderInfo(mainTankType, mainTank, mainItem)
                    Select Case headerInfo.Split("*")(0)
                        Case "1"
                            headerInfo = headerInfo.Remove(0, 2)
                            Dim headerID As String = headerInfo.Split("|")(0)
                            Dim itemDesc As String = headerInfo.Split("|")(1)
                            Dim lotNumber As String = headerInfo.Split("|")(2)
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.MBL_InsertMobileSlurryRecord(headerID, mobileTank, mobileItem, itemDesc, lotNumber, wetWeight.Replace(",", "."), username))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, headerInfo)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Zonen and Charging"
            Case "*GETALLZACCHEMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetAllChemicals())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKSLURRYTANKZANDC*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Select Case tankType
                        Case "MTNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryInMobileTankZAC(tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case "TNK"
                            Dim tankInUse As String = MixedSlurry.RTSQL.Retreive.MBL_CheckSlurryTankZAC(tankType, tankCode, itemCode)
                            Select Case tankInUse.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInUse)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETZACCHEMS*"
                Try
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetZacChems())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATEZACCHEM*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim chemical As String = ClientData.Split("|")(4)
                    Dim qty As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)
                    Select Case tankType
                        Case "MTNK"
                            Dim headerID As String = MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankSlurryID(tankCode, itemCode)
                            Select Case headerID.Split("*")(0)
                                Case "1"
                                    headerID = headerID.Remove(0, 2)
                                    Dim chemicalLineFound As String = MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankChemicalID(headerID, chemical)
                                    Select Case chemicalLineFound.Split("*")(0)
                                        Case "1"
                                            'Update
                                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_updateMobileTankChemical(headerID, chemical, qty, username))
                                        Case "0"
                                            'Add
                                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.MBL_InsertMixedSlurryChemicalMobile(headerID, chemical, qty, username))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, chemicalLineFound)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                            End Select

                        Case "TNK"
                            Dim headerID As String = MixedSlurry.RTSQL.Retreive.MBL_GetTankSlurryID(tankType, tankCode, itemCode)
                            Select Case headerID.Split("*")(0)
                                Case "1"
                                    headerID = headerID.Remove(0, 2)
                                    Dim chemicalLineFound As String = MixedSlurry.RTSQL.Retreive.MBL_GetTankChemicalID(headerID, chemical)
                                    Select Case chemicalLineFound.Split("*")(0)
                                        Case "1"
                                            'Update
                                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_updateMTankChemical(headerID, chemical, qty, username))
                                        Case "0"
                                            'Add
                                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Insert.MBL_InsertMixedSlurryChemical(headerID, chemical, qty, username))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, chemicalLineFound)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerID)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Solidity"
            Case "*GETTANKINFOSOL*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Select Case tankType
                        Case "MTNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankInfoSolidity(tankCode, itemCode))
                        Case "TNK"
                            Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetSlurryTankInfoSolidity(tankType, tankCode, itemCode))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETMIXEDSLURRYSOLIDITY*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim weight As String = ClientData.Split("|")(3)
                    Dim solidity As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Select Case tankType
                        Case "MTNK"
                            Dim tankInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankSolidityInfo(tankCode, itemCode)
                            Select Case tankInfo.Split("*")(0)
                                Case "1"
                                    tankInfo = tankInfo.Remove(0, 2)
                                    Dim tankID As String = tankInfo.Split("|")(0)
                                    Dim lotNumber As String = tankInfo.Split("|")(1)

                                    Dim dWet As Double = Convert.ToDouble(weight.Replace(",", sep).Replace(".", sep))
                                    Dim dSol As Double = Convert.ToDouble(solidity.Replace(",", sep).Replace(".", sep)) / 100
                                    Dim dryWeight As String = Convert.ToString(dWet * dSol)
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_setMobileTankSolidity(solidity.Replace(",", "."), weight.Replace(",", "."), dryWeight.Replace(",", "."), username, tankID))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInfo)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInfo)
                            End Select
                        Case "TNK"
                            Dim tankInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetSlurryTankSolidityInfo(tankType, tankCode, itemCode)
                            Select Case tankInfo.Split("*")(0)
                                Case "1"
                                    tankInfo = tankInfo.Remove(0, 2)
                                    Dim tankID As String = tankInfo.Split("|")(0)
                                    Dim lotNumber As String = tankInfo.Split("|")(1)

                                    Dim dWet As Double = Convert.ToDouble(weight.Replace(",", sep).Replace(".", sep))
                                    Dim dSol As Double = Convert.ToDouble(solidity.Replace(",", sep).Replace(".", sep)) / 100
                                    Dim dryWeight As String = Convert.ToString(dWet * dSol)
                                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Update.MBL_setTankSolidity(solidity.Replace(",", "."), weight.Replace(",", "."), dryWeight.Replace(",", "."), username, tankID))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, tankInfo)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, tankInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Powder Prep"

#Region "Manufacture"
            Case "*VALIDATEPPITEM*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNum As String = ClientData.Split("|")(1)
                    Dim itemFound As String = PowderPrep.Evolution.Retreive.MBL_ValidatePPItem(itemCode)
                    Select Case itemFound.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, PowderPrep.Evolution.Retreive.MBL_GetPPItemDesc(itemCode)) 'MBL_GetPPItemDesc
                            'Server.Listener.SendResponse(ClientSocket, PowderPrep.Evolution.Retreive.MBL_ValidatePPLot(lotNum))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PREPPOWDER*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNum As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim userName As String = ClientData.Split("|")(3)
                    qty = qty.Replace(",", sep).Replace(".", sep)
                    Dim dqty As Double = Convert.ToDouble(qty) / 10000
                    qty = Convert.ToString(dqty).Replace(",", ".")
                    Dim itemInfo As String = PowderPrep.Evolution.Retreive.MBL_GetPPItemInfo(itemCode)
                    Select Case itemInfo.Split("*")(0)
                        Case "1"
                            itemInfo = itemInfo.Remove(0, 2)
                            Dim itemId As String = itemInfo.Split("|")(0)
                            Dim itemDesc As String = itemInfo.Split("|")(1)
                            Server.Listener.SendResponse(ClientSocket, PowderPrep.RTSQL.Insert.UI_insertPowderPrep(itemId, itemCode, itemDesc, 0, lotNum, qty, userName))
                            'Dim lotID As String = PowderPrep.Evolution.Retreive.MBL_GetLotID(lotNum)
                            'Select Case lotID.Split("*")(0)
                            '    Case "1"
                            '        lotID = lotID.Remove(0, 2)

                            '    Case "-1"
                            '        Server.Listener.SendResponse(ClientSocket, lotID)
                            'End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Fresh Slurry"

#Region "Manufacture"
            Case "*CHECKSLURYYLOTEXISTS*"
                Try
                    Dim lotNumber As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Retreive.MBL_CheckLotNumberUsed(lotNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*VALIDATEFSRM*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim rCode As String = ClientData.Split("|")(1)
                    Dim rmLink As String = FreshSlurry.RTSQL.Retreive.MBL_GetSlurryRMLink(itemCode, rCode)
                    Select Case rmLink.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, PowderPrep.Evolution.Retreive.MBL_GetPPItemDesc(rCode))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, rmLink)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKSLURRYINUSEANDRMREQ*"
                Try
                    Dim slurryTank As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim slurryInUse As String = FreshSlurry.RTSQL.Retreive.MBL_GetFreshSlurryInUse(slurryTank)
                    Select Case slurryInUse.Split("*")(0)
                        Case "1"
                            Dim slurryRMs As String = FreshSlurry.RTSQL.Retreive.MBL_GetSlurryReqRM(itemCode)
                            Select Case slurryRMs.Split("*")(0)
                                Case "1"
                                    slurryRMs = slurryRMs.Remove(0, 2)
                                    Dim rmReq As Boolean = False
                                    If slurryRMs <> String.Empty Then
                                        rmReq = True
                                    End If

                                    Dim itemDesc As String = WhseTransfers.Evolution.Retreive.GetItemDesc(itemCode)
                                    Select Case itemDesc.Split("*")(0)
                                        Case "1"
                                            itemDesc = itemDesc.Remove(0, 2)
                                            Server.Listener.SendResponse(ClientSocket, "1*" + itemDesc + "|" + Convert.ToString(rmReq))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, slurryRMs)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, slurryRMs)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, slurryInUse)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, slurryInUse)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CLOSESLURRY*"
                Try
                    Dim slurryTank As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim username As String = ClientData.Split("|")(3)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.MBL_InvalidateSlurry(slurryTank, itemCode, lotNumber, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDNEWFRESHSLURRY*"
                Try
                    Dim trolleyCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lot As String = ClientData.Split("|")(2)
                    Dim wetWeight As String = ClientData.Split("|")(3)
                    Dim userName As String = ClientData.Split("|")(4)
                    Dim itemDesc As String = FreshSlurry.Evolution.Retreive.MBL_GetItemDesc(itemCode)
                    Select Case itemDesc.Split("*")(0)
                        Case "1"
                            itemDesc = itemDesc.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Insert.MBL_InsertFreshSlurry(trolleyCode, itemCode, lot, wetWeight, userName, itemDesc))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemDesc)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDNEWFRESHSLURRYWRAW*"
                Try
                    Dim trolleyCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lot As String = ClientData.Split("|")(2)
                    Dim wetWeight As String = ClientData.Split("|")(3)
                    Dim userName As String = ClientData.Split("|")(4)
                    Dim rCode As String = ClientData.Split("|")(5)
                    Dim rLot As String = ClientData.Split("|")(6)
                    Dim rQty As String = ClientData.Split("|")(7)
                    Dim itemDesc As String = FreshSlurry.Evolution.Retreive.MBL_GetItemDesc(itemCode)
                    Select Case itemDesc.Split("*")(0)
                        Case "1"
                            itemDesc = itemDesc.Remove(0, 2)
                            Dim fsHeaderInserted As String = FreshSlurry.RTSQL.Insert.MBL_InsertFreshSlurry(trolleyCode, itemCode, lot, wetWeight, userName, itemDesc)
                            Select Case fsHeaderInserted.Split("*")(0)
                                Case "1"
                                    Dim headerId As String = FreshSlurry.RTSQL.Retreive.MBL_GetSlurryHeaderID(trolleyCode, itemCode, lot)
                                    Select Case headerId.Split("*")(0)
                                        Case "1"
                                            headerId = headerId.Remove(0, 2)
                                            Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Insert.MBL_InsertFreshSlurryRM(headerId, rCode, rLot, rQty, userName))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, headerId)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, fsHeaderInserted)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemDesc)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETSLURRYNONMANUFACTURED*"
                Try
                    Dim trolleyCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Retreive.MBL_GetSlurryLotNonManufactured(trolleyCode, itemCode))
                    'Dim trolleyFound As String = String.Empty
                    'Select Case trolleyFound.Split("*")(0)
                    '    Case "1"
                    '        trolleyFound = trolleyFound.Remove(0, 2)
                    '        If Convert.ToString(trolleyFound) = False Then
                    '            Server.Listener.SendResponse()
                    '        Else
                    '            Server.Listener.SendResponse(ClientSocket, "-1*No valid slurry found or slurry already manufactured!")
                    '        End If
                    '    Case "-1"
                    '        Server.Listener.SendResponse(ClientSocket, trolleyFound)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SETSLURRYSOLIDITY*"
                Try
                    Dim trolley As String = ClientData.Split("|")(0)
                    Dim item As String = ClientData.Split("|")(1)
                    Dim sol As String = ClientData.Split("|")(2)
                    Dim userName As String = ClientData.Split("|")(3)
                    Dim lot As String = FreshSlurry.RTSQL.Retreive.MBL_GetSlurryLotNonManufacturedSaveSol(trolley, item)
                    Select Case lot.Split("*")(0)
                        Case "1"
                            lot = lot.Remove(0, 2)
                            Dim wetWeight As String = FreshSlurry.RTSQL.Retreive.MBL_GetSlurryWeightNonManufacturedSaveSol(trolley, item, lot)
                            Select Case wetWeight.Split("*")(0)
                                Case "1"
                                    wetWeight = wetWeight.Remove(0, 2)
                                    Dim dSol As Decimal = Convert.ToDecimal(sol.Replace(",", sep).Replace(".", sep))
                                    dSol = dSol / 100
                                    Dim wetWeightd As Decimal = Convert.ToDecimal(wetWeight.Replace(",", sep).Replace(".", sep))
                                    Dim actualSol As String = Convert.ToString(dSol).Replace(",", ".")
                                    Dim dryWeight As String = Convert.ToString(wetWeightd * actualSol)
                                    Server.Listener.SendResponse(ClientSocket, FreshSlurry.RTSQL.Update.MBL_setSlurrySolidity(trolley, item, lot, sol, userName, dryWeight))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, wetWeight)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, lot)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "ZECT"
            Case "*CHECKZECTJOBRUNNING*"
                Try
                    Dim zectJob As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)

                    Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetLineJobOpen(zectJob, whseCode)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) = True Then
                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is not currently running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CONSUMERMRT2DZECT*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)
                    Dim unq As String = ClientData.Split("|")(5)

                    Dim jobOutPut As String = Zect.RTSQL.Retreive.MBL_GetJobOutPutItem(jobNo)
                    Select Case jobOutPut.Split("*")(0)
                        Case "1"
                            jobOutPut = jobOutPut.Remove(0, 2)
                            Dim rmAllowed As String = Zect.RTSQL.Retreive.MBL_checkItemRM(jobOutPut, itemCode)
                            Select Case rmAllowed.Split("*")(0)
                                Case "1"
                                    Dim consumed As String = Unique.RTSQL.Retreive.MBL_GetItemConsumed(itemCode, lotNumber, qty, unq)
                                    Select Case consumed.Split("*")(0)
                                        Case "1"
                                            consumed = consumed.Remove(0, 2)
                                            If Convert.ToBoolean(consumed) = False Then
                                                Dim tranferredIn As String = Unique.RTSQL.Retreive.MBL_GetItemReceivedTransfer(itemCode, lotNumber, qty, unq)
                                                Select Case tranferredIn.Split("*")(0)
                                                    Case "1"
                                                        tranferredIn = tranferredIn.Remove(0, 2)
                                                        If Convert.ToBoolean(tranferredIn) = True Then
                                                            Dim headerID As String = Zect.RTSQL.Retreive.Zect_GetJobID(jobNo)
                                                            Select Case headerID.Split("*")(0)
                                                                Case "1"
                                                                    headerID = headerID.Remove(0, 2)
                                                                    Dim rmRecorded As String = Zect.RTSQL.Insert.Zect_LogRM(headerID, itemCode, lotNumber, qty, username)
                                                                    Select Case rmRecorded.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemConsumed(itemCode, lotNumber, qty, unq, jobNo))
                                                                        Case "-1"
                                                                            Server.Listener.SendResponse(ClientSocket, rmRecorded)
                                                                    End Select
                                                                Case "0"
                                                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The item has not yet been received!")
                                                        End If
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, tranferredIn)
                                                End Select
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*The item has already been consumed!")
                                            End If
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, consumed)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, rmAllowed)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobOutPut)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "A&W"
            Case "*MBLGETAWJOBINFO*"
                Try
                    Dim jobNumber As String = ClientData
                    Dim jobRunning As String = AW.RTSQL.Retrieve.MBL_GetJobRunning(jobNumber)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) = True Then
                                Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.MBL_GetJobInfo(jobNumber))
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job you have scanned is not currently running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ISSUEAWJOBRM*"
                Try
                    Dim jobNumber As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNUmber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim unq As String = ClientData.Split("|")(4)
                    Dim userName As String = ClientData.Split("|")(5)

                    Dim ZectJob As String = unq.Split("_")(0)
                    Dim palletNumber As String = unq.Split("_")(1)

                    Dim awJobID As String = AW.RTSQL.Retrieve.MBL_GetJobID(jobNumber)
                    Select Case awJobID.Split("*")(0)
                        Case "1"
                            awJobID = awJobID.Remove(0, 2)
                            Dim unqFoundOnJob As String = AW.RTSQL.Retrieve.MBL_GetUnqOnJob(unq)
                            Select Case unqFoundOnJob.Split("*")(0)
                                Case "1"
                                    Dim inserted As String = AW.RTSQL.Insert.UI_InsertNewAWJobRawMaterial(awJobID, itemCode, lotNUmber, qty, userName, palletNumber, ZectJob, unq)
                                    Select Case inserted.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemConsumed(itemCode, lotNUmber, qty, unq, jobNumber))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, inserted)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, unqFoundOnJob)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, unqFoundOnJob)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, awJobID)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, awJobID)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Receiving Transfers"
            Case "*GETVALIDRECWAREHOUSES*"
                Try
                    Dim process As String = ClientData
                    Select Case process
                        Case "PPtFS"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetRecWhse("Powder Prep"))
                        Case "FStMS"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetRecWhse("Fresh Slurry"))
                        Case "MStZECT"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetMSRecWhses())
                        Case "ZECT1"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetZect1RecWhses())
                        Case "ZECT2"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetZect2RecWhses())
                        Case "Canning"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetCanningRecWhses())
                        Case "AW"
                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetAWRecWhses())
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFROMITRT2D*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim WhseFrom As String = ClientData.Split("|")(2)
                    Dim WhseTo As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim procCode As String = ClientData.Split("|")(6)
                    Dim unq As String = ClientData.Split("|")(7)

                    Dim transferDescription As String = String.Empty
                    Select Case procCode
                        Case "PPtFS"
                            transferDescription = "Powder Prep Transfer from IT"
                        Case "FStMS"
                            transferDescription = "Fresh Slurry Transfer from IT"
                        Case "MStZECT"
                            transferDescription = "MIxed Slurry Transfer from IT"
                        Case "AW"
                            transferDescription = "AW Transfer from IT"
                        Case "Canning"
                            transferDescription = "Canning Transfer from IT"
                    End Select

                    Dim tranferredIn As String = Unique.RTSQL.Retreive.MBL_GetItemReceivedTransfer(itemCode, lotNumber, qty, unq)
                    Select Case tranferredIn.Split("*")(0)
                        Case "1"
                            tranferredIn = tranferredIn.Remove(0, 2)
                            If Convert.ToBoolean(tranferredIn) = False Then
                                Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                                Select Case whseFromOK.Split("*")(0)
                                    Case "1"
                                        Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                                        Select Case whseToOK.Split("*")(0)
                                            Case "1"
                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode, transferDescription, "Pending")
                                                Select Case transferSaved.Split("*")(0)
                                                    Case "1"
                                                        Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                                        Select Case transferLogged.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemReceived(itemCode, lotNumber, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseToOK)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*The item has already been received!")
                            End If
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, tranferredIn)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPGMRECTRANSINFO*"
                Try
                    Dim container As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim itemInfo As String = PGM.RTSQL.Retreive.PGM_GetContainerInfoForRecTrans(container, itemCode)
                    Select Case itemInfo.Split("*")(0)
                        Case "1"
                            If Convert.ToBoolean(itemInfo.Split("|")(4)) = False Then
                                Server.Listener.SendResponse(ClientSocket, itemInfo)
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Item Already received")
                            End If
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFROMITPGM*"
                Try
                    Dim containerNO As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim WhseFrom As String = ClientData.Split("|")(3)
                    Dim WhseTo As String = ClientData.Split("|")(4)
                    Dim qty As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)
                    Dim procCode As String = ClientData.Split("|")(7)

                    Dim transferDescription As String = String.Empty
                    Select Case procCode
                        Case "PPtFS"
                            transferDescription = "Powder Prep Transfer from IT"
                        Case "FStMS"
                            transferDescription = "Fresh Slurry Transfer from IT"
                        Case "MStZECT"
                            transferDescription = "MIxed Slurry Transfer from IT"
                        Case "AW"
                            transferDescription = "AW Transfer from IT"
                        Case "Canning"
                            transferDescription = "Canning Transfer from IT"
                    End Select

                    'Select Case procCode
                    '    Case "PPtFS"
                    '        transferDescription = "Powder Prep Transfer from IT"
                    '    Case "FStMS"
                    '        transferDescription = "Fresh Slurry Transfer from IT"
                    '    Case "MStZECT"
                    '        transferDescription = "MIxed Slurry Transfer from IT"
                    'End Select

                    Dim headerID As String = PGM.RTSQL.Retreive.PGM_GetPGMHeaderID(itemCode, lotNumber)
                    Select Case headerID.Split("*")(0)
                        Case "1"
                            headerID = headerID.Remove(0, 2)
                            Dim received As String = PGM.RTSQL.Retreive.PGM_GetPGMReceived(headerID, containerNO)
                            Select Case received.Split("*")(0)
                                Case "1"
                                    received = received.Remove(0, 2)
                                    If Convert.ToString(received) = False Then
                                        Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                                        Select Case whseFromOK.Split("*")(0)
                                            Case "1"
                                                Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                                                Select Case whseToOK.Split("*")(0)
                                                    Case "1"
                                                        'Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode)
                                                        Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode, transferDescription, "Pending")

                                                        Select Case transferSaved.Split("*")(0)
                                                            Case "1"
                                                                Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                                                Select Case transferLogged.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, PGM.RTSQL.Update.PGM_updateContReceived_PPRec(containerNO))
                                                                    Case "-1"
                                                                        Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                                End Select
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, whseToOK)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "-1*Item Already received")
                                    End If
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, received)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, headerID)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFROMITFSBCD*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim WhseFrom As String = ClientData.Split("|")(2)
                    Dim WhseTo As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim procCode As String = ClientData.Split("|")(6)

                    Dim transferDescription As String = String.Empty
                    Select Case procCode
                        Case "PPtFS"
                            transferDescription = "Powder Prep Transfer from IT"
                        Case "FStMS"
                            transferDescription = "Fresh Slurry Transfer from IT"
                        Case "MStZECT"
                            transferDescription = "MIxed Slurry Transfer from IT"
                    End Select

                    Dim transferredIn As String = Transfers.RTSQL.Retreive.MBL_CheckPowderReceieved(itemCode, lotNumber, qty)
                    Select Case transferredIn.Split("*")(0)
                        Case "1"
                            transferredIn = transferredIn.Remove(0, 2)
                            If Convert.ToBoolean(transferredIn) = False Then
                                Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                                Select Case whseFromOK.Split("*")(0)
                                    Case "1"
                                        Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                                        Select Case whseToOK.Split("*")(0)
                                            Case "1"
                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode)
                                                Select Case transferSaved.Split("*")(0)
                                                    Case "1"
                                                        Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                                        Select Case transferLogged.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_UpdatePowderTransferredIn(itemCode, lotNumber, username, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseToOK)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Powder already received")
                            End If
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, transferredIn)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETFRESHSLURRYINFO*"
                Try
                    Dim trolleycode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, MixedSlurry.RTSQL.Retreive.MBL_GetFreshSlurryInfoRecTrans(trolleycode, itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFROMITMSBCD*"
                Try
                    Dim trolleycode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim WhseFrom As String = ClientData.Split("|")(3)
                    Dim WhseTo As String = ClientData.Split("|")(4)
                    Dim qty As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)
                    Dim procCode As String = ClientData.Split("|")(7)

                    Dim transferDescription As String = String.Empty
                    Select Case procCode
                        Case "PPtFS"
                            transferDescription = "Powder Prep Transfer from IT"
                        Case "FStMS"
                            transferDescription = "Fresh Slurry Transfer from IT"
                        Case "MStZECT"
                            transferDescription = "MIxed Slurry Transfer from IT"
                    End Select

                    Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseFrom)
                    Select Case whseFromOK.Split("*")(0)
                        Case "1"
                            Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, WhseTo)
                            Select Case whseToOK.Split("*")(0)
                                Case "1"
                                    Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, procCode, transferDescription, "Pending")
                                    Select Case transferSaved.Split("*")(0)
                                        Case "1"
                                            Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, procCode)
                                            Select Case transferLogged.Split("*")(0)
                                                Case "1"
                                                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_setFreshSlurryReceived(trolleycode, itemCode, lotNumber, username))
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, transferLogged)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, transferSaved)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, whseToOK)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, whseFromOK)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Outgoing Transfers"

#Region "Powder Prep"
            Case "*VALIDATEPPITEMWHT*"
                Try
                    Dim slurryCode As String = ClientData.Split("|")(0)
                    Dim powderCode As String = ClientData.Split("|")(1)
                    Dim lotNum As String = ClientData.Split("|")(2)
                    Dim powderFound As String = Transfers.Evolution.Retreive.MBL_ValidatePPItem(powderCode)
                    Select Case powderFound.Split("*")(0)
                        Case "1"
                            Dim slurryFound As String = Transfers.Evolution.Retreive.MBL_ValidatePPItem(slurryCode)
                            Select Case slurryFound.Split("*")(0)
                                Case "1"

                                    Dim slurryPowders As String = Transfers.RTSQL.Retreive.MBL_GetSlurryPowders(slurryCode)
                                    Select Case slurryPowders.Split("*")(0)
                                        Case "1"
                                            slurryPowders = slurryPowders.Remove(0, 2)
                                            Dim allSlurryPowders As String() = slurryPowders.Split("~")

                                            Dim approved As Boolean = False
                                            For Each powder As String In allSlurryPowders
                                                If powder = powderCode Then
                                                    approved = True
                                                    Exit For
                                                End If
                                            Next

                                            If approved = True Then
                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*Incorrect powder for slurry!")
                                            End If
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, slurryPowders)
                                    End Select

                                    'Dim lotFound As String = Transfers.Evolution.Retreive.MBL_ValidatePPLot(lotNum)
                                    'Select Case lotFound.Split("*")(0)
                                    '    Case "1"

                                    '    Case "-1"
                                    '        Server.Listener.SendResponse(ClientSocket, lotFound)
                                    'End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, slurryFound)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, powderFound)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPPTFSWAREHOUSE*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetPowderPrepWhes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFROMPOWDERPREP*"
                Try
                    Dim orderNum As String = "RTISTRANSFER"
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim whseFrom As String = ClientData.Split("|")(3)
                    Dim userName As String = ClientData.Split("|")(4)

                    qty = qty.Replace(",", sep).Replace(".", sep)
                    Dim dqty As Double = Convert.ToDouble(qty) / 10000
                    qty = Convert.ToString(dqty)

                    Dim procCode As String = "PPtFS"
                    Dim transferDescription As String = "Powder Prep Transfer to IT"


                    Dim whseTo As String = Transfers.RTSQL.Retreive.MBL_GetPowderPrepWhes()
                    Select Case whseTo.Split("*")(0)
                        Case "1"
                            whseTo = whseTo.Remove(0, 2)
                            Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                            Select Case whseFromOK.Split("*")(0)
                                Case "1"
                                    Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                    Select Case whseToOK.Split("*")(0)
                                        Case "1"
                                            'Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(itemCode, lot, whseFrom, whseTo, qty, userName, "PPtFS")
                                            Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lot, whseFrom, whseTo, qty, userName, procCode, transferDescription, "Pending")

                                            'Dim transferComplete As String = Transfers.EvolutionSDK.CTransferItem(orderNum, whseFrom, whseTo, itemCode, lot, qty)
                                            Select Case transferSaved.Split("*")(0)
                                                Case "1"
                                                    Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lot, whseFrom, whseTo, qty, userName, "PPtFS")
                                                    Select Case transferRecorded.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_UpdatePowderTransferred(itemCode, lot, userName))
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, transferSaved)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, whseToOK)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, whseFromOK)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, whseTo)
                    End Select

                    'Dim checkManufactured As String = Transfers.RTSQL.Retreive.MBL_CheckPowderManufactured(itemCode, lot)
                    'Select Case checkManufactured.Split("*")(0)
                    '    Case "1"
                    '        checkManufactured = checkManufactured.Remove(0, 2)
                    '        If Convert.ToBoolean(checkManufactured) = True Then

                    '        Else
                    '            Server.Listener.SendResponse(ClientSocket, "-1*CANNOT TRANSFER ITEM" + Environment.NewLine + "THE POWDER YOU HAVE SCANNED HAS NOT BEEN MANUFACTURED!")
                    '        End If
                    '    Case "-1"
                    '        Server.Listener.SendResponse(ClientSocket, checkManufactured)
                    'End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Fresh Slurry"
            Case "*GEFSTRANSFERINFO*"
                Try
                    Dim trolley As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim slurryTrans As String = Transfers.RTSQL.Retreive.MBL_GetFreshSlurryTransferred(trolley, itemCode)
                    slurryTrans = slurryTrans.Remove(0, 2)
                    If Convert.ToBoolean(slurryTrans) = False Then
                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetSlurryTransferInfo(trolley, itemCode))
                    Else
                        Server.Listener.SendResponse(ClientSocket, "-1*Slurry Already Transferred out!")
                    End If
                    'Dim slurryManuf As String = Transfers.RTSQL.Retreive.MBL_GetFreshSlurryManufactured(trolley, itemCode)
                    'Select Case slurryManuf.Split("*")(0)
                    '    Case "1"
                    '        slurryManuf = slurryManuf.Remove(0, 2)
                    '        If Convert.ToBoolean(slurryManuf) = True Then

                    '        Else
                    '            Server.Listener.SendResponse(ClientSocket, "-1*Slurry Not Manufactured!")
                    '        End If
                    '    Case "-1"
                    '        Server.Listener.SendResponse(ClientSocket, slurryManuf)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERFRESHSLURRY*"
                Try
                    Dim orderNum As String = "RTISTRANSFER"
                    Dim trolley As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim whseFrom As String = ClientData.Split("|")(2)
                    Dim userName As String = ClientData.Split("|")(3)

                    Dim slurryTrans As String = Transfers.RTSQL.Retreive.MBL_GetFreshSlurryTransferred(trolley, itemCode)
                    slurryTrans = slurryTrans.Remove(0, 2)
                    If Convert.ToBoolean(slurryTrans) = False Then
                        Dim slurryInfo As String = Transfers.RTSQL.Retreive.MBL_GetSlurryTransferInfo(trolley, itemCode)
                        Select Case slurryInfo.Split("*")(0)
                            Case "1"
                                slurryInfo = slurryInfo.Remove(0, 2)
                                Dim lot As String = slurryInfo.Split("|")(0)
                                Dim qty As String = slurryInfo.Split("|")(1)

                                Dim whseTo As String = Transfers.RTSQL.Retreive.MBL_GetFreshSlurryWhes()
                                Select Case whseTo.Split("*")(0)
                                    Case "1"
                                        whseTo = whseTo.Remove(0, 2)
                                        Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                        Select Case whseFromOK.Split("*")(0)
                                            Case "1"
                                                Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                                Select Case whseToOK.Split("*")(0)
                                                    Case "1"
                                                        Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertFGWhseTransfer(itemCode, lot, whseFrom, whseTo, qty, userName, "FStMS")
                                                        'Dim transferComplete As String = Transfers.EvolutionSDK.CTransferItem(orderNum, whseFrom, whseTo, itemCode, lot, qty)
                                                        Select Case transferComplete.Split("*")(0)
                                                            Case "1"
                                                                Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lot, whseFrom, whseTo, qty, userName, "FStMS")
                                                                Select Case transferRecorded.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_UpdateSlurryTransferred(trolley, itemCode, lot))
                                                                    Case "-1"
                                                                        Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                                End Select
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, whseToOK)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                        End Select

                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseTo)
                                End Select
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, slurryInfo)
                        End Select
                    Else
                        Server.Listener.SendResponse(ClientSocket, "-1*Slurry Already Transferred out!")
                    End If

                    'Dim slurryManuf As String = Transfers.RTSQL.Retreive.MBL_GetFreshSlurryManufactured(trolley, itemCode)
                    'Select Case slurryManuf.Split("*")(0)
                    '    Case "1"
                    '        slurryManuf = slurryManuf.Remove(0, 2)
                    '        If Convert.ToBoolean(slurryManuf) = True Then

                    '        Else
                    '            Server.Listener.SendResponse(ClientSocket, "-1*Slurry Not Manufactured!")
                    '        End If
                    '    Case "-1"
                    '        Server.Listener.SendResponse(ClientSocket, slurryManuf)
                    'End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Mixed Slurry"
            Case "*GETMIXEDSLURRYWAREHOUSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetMixedSlurryWhes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMIXEDSLURRYTRANSFERINFO*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Select Case tankType
                        Case "MTNK"
                            Dim slurryInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankTransferInfo(tankCode, itemCode)
                            Select Case slurryInfo.Split("*")(0)
                                Case "1"
                                    slurryInfo = slurryInfo.Remove(0, 2)
                                    Dim lotNum As String = slurryInfo.Split("|")(0)
                                    Dim dryWeight As String = slurryInfo.Split("|")(1)
                                    Dim transferred As String = slurryInfo.Split("|")(2)
                                    Dim itemDesc As String = slurryInfo.Split("|")(3)
                                    If Convert.ToBoolean(transferred) = False Then
                                        Server.Listener.SendResponse(ClientSocket, "1*" + lotNum + "|" + dryWeight + "|" + itemDesc)
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "-1*Slurry already transferred")
                                    End If

                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, slurryInfo)
                            End Select
                        Case "TNK"
                            Dim slurryInfo As String = MixedSlurry.RTSQL.Retreive.MBL_GetTankTransferInfo(tankType, tankCode, itemCode)
                            Select Case slurryInfo.Split("*")(0)
                                Case "1"
                                    slurryInfo = slurryInfo.Remove(0, 2)
                                    Dim lotNum As String = slurryInfo.Split("|")(0)
                                    Dim dryWeight As String = slurryInfo.Split("|")(1)
                                    Dim transferred As String = slurryInfo.Split("|")(2)
                                    Dim itemDesc As String = slurryInfo.Split("|")(3)
                                    If Convert.ToBoolean(transferred) = False Then
                                        Server.Listener.SendResponse(ClientSocket, "1*" + lotNum + "|" + dryWeight + "|" + itemDesc)
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "-1*Slurry already transferred")
                                    End If

                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, slurryInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERMIXEDSLURRYNEW*"
                Try
                    Dim tankType As String = ClientData.Split("|")(0)
                    Dim tankCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNum As String = ClientData.Split("|")(3)
                    Dim dryWeight As String = ClientData.Split("|")(4)
                    Dim whseFrom As String = ClientData.Split("|")(5)
                    Dim whseTo As String = ClientData.Split("|")(6)
                    Dim userName As String = ClientData.Split("|")(7)

                    Select Case tankType
                        Case "MTNK"
                            Dim transferred As String = MixedSlurry.RTSQL.Retreive.MBL_GetMobileTankTransferd(tankCode, itemCode, lotNum)
                            Select Case transferred.Split("*")(0)
                                Case "1"
                                    transferred = transferred.Remove(0, 2)
                                    If Convert.ToBoolean(transferred) = False Then
                                        Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                        Select Case whseFromOK.Split("*")(0)
                                            Case "1"
                                                Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                                Select Case whseToOK.Split("*")(0)
                                                    Case "1"
                                                        Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNum, whseFrom, whseTo, dryWeight.Replace(".", sep), userName, "MStZECT", "Mixed Slurry Transfer to in Transit", "Pending")
                                                        'Dim transferComplete As String = Transfers.EvolutionSDK.CTransferItem(orderNum, whseFrom, whseTo, itemCode, lotNum, dryWeight.Replace(".", sep).Replace(",", sep)) '"1*Success" '
                                                        Select Case transferComplete.Split("*")(0)
                                                            Case "1"
                                                                Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNum, whseFrom, whseTo, dryWeight.Replace(",", "."), userName, "FStMS")
                                                                Select Case transferRecorded.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_UpdateMobileTankTransferred(tankCode, itemCode, lotNum, userName))
                                                                    Case "-1"
                                                                        Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                                End Select
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, whseToOK)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*The tank has already been transferred out")
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                            End Select
                        Case "TNK"
                            Dim transferred As String = MixedSlurry.RTSQL.Retreive.MBL_GetLargeTankTransferd(tankType, tankCode, itemCode, lotNum)
                            Select Case transferred.Split("*")(0)
                                Case "1"
                                    transferred = transferred.Remove(0, 2)
                                    If Convert.ToBoolean(transferred) = False Then
                                        Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                        Select Case whseFromOK.Split("*")(0)
                                            Case "1"
                                                Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                                Select Case whseToOK.Split("*")(0)
                                                    Case "1"
                                                        Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNum, whseFrom, whseTo, dryWeight.Replace(".", sep), userName, "MStZECT", "Mixed Slurry Transfer to in Transit", "Pending")
                                                        Select Case transferComplete.Split("*")(0)
                                                            Case "1"
                                                                Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNum, whseFrom, whseTo, dryWeight.Replace(",", "."), userName, "FStMS")
                                                                Select Case transferRecorded.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Update.MBL_UpdateLargeTankTransferred(tankType, tankCode, itemCode, lotNum, userName))
                                                                    Case "-1"
                                                                        Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                                End Select
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, whseToOK)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*The tank has already been transferred out")
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, transferred)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, "0*Please scan a valid slurry tank")
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "To Prod"
            Case "*GETTOPRODWAREHOUSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetToProdWhses())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERITEMTOPROD*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim WhseFrom As String = ClientData.Split("|")(2)
                    Dim WhseTo As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim unq As String = ClientData.Split("|")(6)

                    Dim bTransferred As String = Unique.RTSQL.Retreive.MBL_GetItemTransferredOut(itemCode, lotNumber, qty, unq)
                    Select Case bTransferred.Split("*")(0)
                        Case "1"
                            bTransferred = bTransferred.Remove(0, 2)
                            If Convert.ToBoolean(bTransferred) = False Then
                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNumber, WhseFrom, WhseTo, qty, username, "ToProd", "Transfer to production", "Pending")
                                Select Case transferSaved.Split("*")(0)
                                    Case "1"
                                        Dim logged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNumber, WhseFrom, WhseTo, qty.Replace(",", "."), username, "ToProd")
                                        Select Case logged.Split("*")(0)
                                            Case "1"
                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemTransferredOut(itemCode, lotNumber, qty, unq))
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, logged)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Item already transferred out!")
                            End If
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, bTransferred)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Zect"
            Case "*GEtZECTTRANSFERWAREHOUSES*"
                Try
                    Dim zectWhse As String = ClientData
                    If zectWhse.Contains("1") Then
                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetZect1Whes())
                    ElseIf zectWhse.Contains("2") Then
                        Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetZect2Whes())
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERZECTITEM*"
                Try
                    Dim orderNum As String = "RTISTRANSFER"
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNum As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim whseFrom As String = ClientData.Split("|")(3)
                    Dim whseTo As String = ClientData.Split("|")(4)
                    Dim userName As String = ClientData.Split("|")(5)
                    Dim unq As String = ClientData.Split("|")(6) '12 '

                    Dim proccessName As String = String.Empty
                    If whseFrom.Contains("1") Then
                        proccessName = "ZECT1"
                    Else
                        proccessName = "ZECT2"
                    End If

                    Dim checkTransfferredOut As String = Unique.RTSQL.Retreive.MBL_GetItemTransferredOut(itemCode, lotNum, qty, unq)
                    Select Case checkTransfferredOut.Split("*")(0)
                        Case "1"
                            checkTransfferredOut = checkTransfferredOut.Remove(0, 2)
                            If Convert.ToBoolean(checkTransfferredOut) = False Then
                                Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                Select Case whseFromOK.Split("*")(0)
                                    Case "1"
                                        Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                        Select Case whseToOK.Split("*")(0)
                                            Case "1"
                                                Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNum, whseFrom, whseTo, qty.Replace(".", sep), userName, proccessName, "ZECT Transfer to in Transit", "Pending")
                                                Select Case transferComplete.Split("*")(0)
                                                    Case "1"
                                                        Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNum, whseFrom, whseTo, qty.Replace(",", "."), userName, proccessName)
                                                        Select Case transferRecorded.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemTransferredOut(itemCode, lotNum, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseToOK)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The item scanned has already been transferred out")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "A&W"
            Case "*GETAWTRANSFERWAREHOUSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetAWWhes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERAWITEM*"
                Try
                    Dim orderNum As String = "RTISTRANSFER"
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNum As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim whseFrom As String = ClientData.Split("|")(3)
                    Dim whseTo As String = ClientData.Split("|")(4)
                    Dim userName As String = ClientData.Split("|")(5)
                    Dim unq As String = ClientData.Split("|")(6) '12 '
                    Dim proccessName As String = "AW"

                    Dim checkTransfferredOut As String = Unique.RTSQL.Retreive.MBL_GetItemTransferredOut(itemCode, lotNum, qty, unq)
                    Select Case checkTransfferredOut.Split("*")(0)
                        Case "1"
                            checkTransfferredOut = checkTransfferredOut.Remove(0, 2)
                            If Convert.ToBoolean(checkTransfferredOut) = False Then
                                Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                Select Case whseFromOK.Split("*")(0)
                                    Case "1"
                                        Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                        Select Case whseToOK.Split("*")(0)
                                            Case "1"
                                                Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNum, whseFrom, whseTo, qty.Replace(".", sep), userName, proccessName, "A&W Transfer to in Transit", "Pending")
                                                Select Case transferComplete.Split("*")(0)
                                                    Case "1"
                                                        Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNum, whseFrom, whseTo, qty.Replace(",", "."), userName, proccessName)
                                                        Select Case transferRecorded.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemTransferredOut(itemCode, lotNum, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseToOK)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The item scanned has already been transferred out")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Canning"
            Case "*GETCANNINGTRANSFERWAREHOUSES*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Transfers.RTSQL.Retreive.MBL_GetCanningWhes())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*TRANSFERCANNINGITEM*"
                Try
                    Dim orderNum As String = "RTISTRANSFER"
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNum As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim whseFrom As String = ClientData.Split("|")(3)
                    Dim whseTo As String = ClientData.Split("|")(4)
                    Dim userName As String = ClientData.Split("|")(5)
                    Dim unq As String = ClientData.Split("|")(6) '12 '
                    Dim proccessName As String = "Canning"

                    Dim checkTransfferredOut As String = Unique.RTSQL.Retreive.MBL_GetItemTransferredOut(itemCode, lotNum, qty, unq)
                    Select Case checkTransfferredOut.Split("*")(0)
                        Case "1"
                            checkTransfferredOut = checkTransfferredOut.Remove(0, 2)
                            If Convert.ToBoolean(checkTransfferredOut) = False Then
                                Dim whseFromOK = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseFrom)
                                Select Case whseFromOK.Split("*")(0)
                                    Case "1"
                                        Dim whseToOK As String = Transfers.RTSQL.Retreive.MBL_CheckWhseStockAso(itemCode, whseTo)
                                        Select Case whseToOK.Split("*")(0)
                                            Case "1"
                                                Dim transferComplete As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(itemCode, lotNum, whseFrom, whseTo, qty.Replace(".", sep), userName, proccessName, "Canning Transfer to in Transit", "Pending")
                                                Select Case transferComplete.Split("*")(0)
                                                    Case "1"
                                                        Dim transferRecorded As String = Transfers.RTSQL.Insert.UI_whtTransferLog(itemCode, lotNum, whseFrom, whseTo, qty.Replace(",", "."), userName, proccessName)
                                                        Select Case transferRecorded.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemTransferredOut(itemCode, lotNum, qty, unq))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferRecorded)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferComplete)
                                                End Select
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, whseToOK)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, whseFromOK)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The item scanned has already been transferred out")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, checkTransfferredOut)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Dispatch"
            Case "*GETSOLINES*"
                Try
                    Dim soNumber As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Dispatch.EvolutionSDK.MBL_GetSOLines(soNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESOLINELOT*"
                Try
                    Dim soNumber As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim Qty As String = ClientData.Split("|")(3)
                    Dim unq As String = ClientData.Split("|")(4)
                    If unq.Substring(0, 1) = "P" Then
                        'Palltet Barcode
                        Dim palletID As String = Unique.RTSQL.Retreive.MBL_GetPalletID(itemCode, lotNumber, Qty, unq) '
                        Select Case palletID.Split("*")(0)
                            Case "1"
                                palletID = palletID.Remove(0, 2)
                                Dim palletDispatched As String = Unique.RTSQL.Retreive.MBL_GetPalletDispatched(palletID)
                                Select Case palletDispatched.Split("*")(0)
                                    Case "1"
                                        palletDispatched = palletDispatched.Remove(0, 2)
                                        If palletDispatched = String.Empty Then
                                            Dim palletUnqs As String = Unique.RTSQL.Retreive.MBL_GetPalletBoxBarcodes(palletID)
                                            Select Case palletUnqs.Split("*")(0)
                                                Case "1"
                                                    palletUnqs = palletUnqs.Remove(0, 2)
                                                    Dim whereClause As String = "WHERE"
                                                    Dim allPallets = palletUnqs.Split("~")
                                                    For Each pallet As String In allPallets
                                                        If pallet <> String.Empty Then
                                                            whereClause += "[vUnqBarcode] = '" + pallet + "' OR "
                                                        End If
                                                    Next
                                                    whereClause = whereClause.Substring(0, whereClause.Length - 4)

                                                    Dim updated As String = Dispatch.EvolutionSDK.MBL_UpdateSOLot(soNumber, itemCode, lotNumber, Qty)
                                                    Select Case updated.Split("*")(0)
                                                        Case "1"
                                                            Dim boxesUpdated As String = Unique.RTSQL.Update.MBL_UpdateBoxesDispatched(soNumber, whereClause)
                                                            Select Case boxesUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletDispatched(soNumber, palletID))
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, boxesUpdated)
                                                            End Select
                                                        Case "0"
                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                    End Select

                                                Case "0"
                                                    Server.Listener.SendResponse(ClientSocket, palletUnqs)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, palletUnqs)
                                            End Select
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*This pallet has already been dispatched on SO: " + palletDispatched)
                                        End If
                                    Case "0"
                                        Server.Listener.SendResponse(ClientSocket, palletDispatched)
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, palletDispatched)
                                End Select
                            Case "0"
                                Server.Listener.SendResponse(ClientSocket, palletID)
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, palletID)
                        End Select
                    Else
                        Dim unqFound As String = Unique.RTSQL.Retreive.MBL_GetItemDispatched(itemCode, lotNumber, Qty, unq)
                        Select Case unqFound.Split("*")(0)
                            Case "1"
                                unqFound = unqFound.Remove(0, 2)
                                If unqFound = String.Empty Then
                                    Dim updated As String = Dispatch.EvolutionSDK.MBL_UpdateSOLot(soNumber, itemCode, lotNumber, Qty)
                                    Select Case updated.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemDispatched(soNumber, itemCode, lotNumber, Qty, unq))
                                        Case "0"
                                            Server.Listener.SendResponse(ClientSocket, updated)
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, updated)
                                    End Select
                                Else
                                    Server.Listener.SendResponse(ClientSocket, "-1*This ")
                                End If
                            Case "0"
                                Server.Listener.SendResponse(ClientSocket, unqFound)
                            Case "-1"
                                Server.Listener.SendResponse(ClientSocket, unqFound)
                        End Select
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESOLINENOLOT*"
                Try
                    Dim soNumber As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim Qty As String = ClientData.Split("|")(3)
                    Dim unq As String = ClientData.Split("|")(4)
                    Server.Listener.SendResponse(ClientSocket, Dispatch.EvolutionSDK.MBL_UpdateSONoLot(soNumber, itemCode, Qty))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Stock Takes"

#Region "General"
            Case "*EVOGETSTNUMBERS*"
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.MBL_GetRTStockTakes())
            Case "*EVOGETWHDETAILS*"
                Dim stNum = ClientData
                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Retreive.MBL_GetStockTakeWarehosesBySTNum(stNum))
            Case "*GETLOTEXISTSINEVO*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, StockTake.Evolution.Retreive.MBL_GetLotInEvo(lotNumber, itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETITEMONSTOCKTAKE*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)

                    Dim onStockTake As String = String.Empty
                    If lotNumber = "#NOLOT#" Then
                        onStockTake = StockTake.RTSQL.Retreive.MBL_GetSTLineIDNoLot(stNum, itemCode, whseCode)
                    Else
                        onStockTake = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whseCode, lotNumber)
                        Server.Listener.SendResponse(ClientSocket, onStockTake)
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDLOTFORINVESTIGATION*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)
                    Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Insert.MBL_AddItemForInvestigation(stNum, itemCode, lotNumber, qty, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDITEMTOSTOCKTAKE*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim invCountId As String = StockTake.RTSQL.Retreive.MBL_GetStockTakIDBySTNum(stNum)
                    Select Case invCountId.Split("*")(0)
                        Case "1"
                            invCountId = invCountId.Remove(0, 2)
                            Dim itemInfo As String = StockTake.Evolution.Retreive.MBL_GetItemInfoForST(itemCode)
                            Select Case itemInfo.Split("*")(0)
                                Case "1"
                                    itemInfo = itemInfo.Remove(0, 2)
                                    Dim stockID As String = itemInfo.Split("|")(0)
                                    Dim barcode As String = itemInfo.Split("|")(1)
                                    Dim whseID As String = StockTake.Evolution.Retreive.MBL_GetEvoWhseID(whseCode)
                                    Select Case whseID.Split("*")(0)
                                        Case "1"
                                            whseID = whseID.Remove(0, 2)
                                            Dim lotID As String = String.Empty
                                            Dim isLot As String = 0
                                            If lotNumber = "#NOLOT#" Then
                                                lotID = 0
                                            Else
                                                lotID = StockTake.Evolution.Retreive.MBL_GetLotTrackingID(lotNumber, stockID)
                                                Select Case lotID.Split("*")(0)
                                                    Case "1"
                                                        isLot = 1
                                                        lotID = lotID.Remove(0, 2)
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, lotID)
                                                        Exit Sub
                                                End Select
                                            End If

                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Insert.MBL_AddItemToST(invCountId, barcode, isLot, lotID, stockID, whseID))
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, whseID)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, itemInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, invCountId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ADDITEMTOSTOCKTAKEFRESHSLURRY*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim tankLot As String = ClientData.Split("|")(3)
                    Dim tankQty As String = ClientData.Split("|")(4)
                    Dim trolleyNumber As String = ClientData.Split("|")(4)
                    Dim scannerID As String = ClientData.Split("|")(5)
                    Dim isSingleScanner As String = ClientData.Split("|")(6)
                    Dim isRecount As String = ClientData.Split("|")(7)
                    Dim trolleyCode As String = "TRO_" + trolleyNumber + "$" + itemCode
                    Dim invCountId As String = StockTake.RTSQL.Retreive.MBL_GetStockTakIDBySTNum(stNum)
                    Select Case invCountId.Split("*")(0)
                        Case "1"
                            invCountId = invCountId.Remove(0, 2)
                            Dim itemInfo As String = StockTake.Evolution.Retreive.MBL_GetItemInfoForST(itemCode)
                            Select Case itemInfo.Split("*")(0)
                                Case "1"
                                    itemInfo = itemInfo.Remove(0, 2)
                                    Dim stockID As String = itemInfo.Split("|")(0)
                                    Dim barcode As String = itemInfo.Split("|")(1)
                                    Dim whseID As String = StockTake.Evolution.Retreive.MBL_GetEvoWhseID(whseCode)
                                    Select Case whseID.Split("*")(0)
                                        Case "1"
                                            whseID = whseID.Remove(0, 2)
                                            Dim lotID As String = String.Empty
                                            Dim isLot As String = 0
                                            If tankLot = "#NOLOT#" Then
                                                lotID = 0
                                            Else
                                                lotID = StockTake.Evolution.Retreive.MBL_GetLotTrackingID(tankLot, stockID)
                                                Select Case lotID.Split("*")(0)
                                                    Case "1"
                                                        isLot = 1
                                                        lotID = lotID.Remove(0, 2)
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, lotID)
                                                        Exit Sub
                                                End Select
                                            End If

                                            Dim addedToST = StockTake.RTSQL.Insert.MBL_AddItemToST(invCountId, barcode, isLot, lotID, stockID, whseID)
                                            Select Case addedToST.Split("*")(0)
                                                Case "1"
                                                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, tankLot)
                                                    Select Case headerId.Split("*")(0)
                                                        Case "1"
                                                            headerId = headerId.Remove(0, 2)
                                                            If Convert.ToBoolean(isRecount) = False Then
                                                                Dim trolleyTicketInfo As String = StockTake.RTSQL.Retreive.MBL_getSlurryTicketInfo(headerId, trolleyCode)
                                                                Select Case trolleyTicketInfo.Split("*")(0)
                                                                    Case "1"
                                                                        trolleyTicketInfo = trolleyTicketInfo.Remove(0, 2)
                                                                        Dim trolleyQty1 As Decimal = Convert.ToDecimal(trolleyTicketInfo.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                                        Dim trolleyQty2 As Decimal = Convert.ToDecimal(trolleyTicketInfo.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                                        If Convert.ToBoolean(isSingleScanner) = False Then
                                                                            If CLng(scannerID) Mod 2 = 0 Then
                                                                                'Even
                                                                                If trolleyQty2 = 0 Then
                                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                                Else
                                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                                End If
                                                                            Else
                                                                                'Odd
                                                                                If trolleyQty1 = 0 Then
                                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                                Else
                                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                                End If
                                                                            End If
                                                                        Else
                                                                            If trolleyQty1 = 0 And trolleyQty2 = 0 Then
                                                                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                            Else
                                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                            End If
                                                                        End If
                                                                    Case "0"
                                                                        'Trolley not yet scanned
                                                                        Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, trolleyTicketInfo)
                                                                End Select
                                                            Else
                                                                Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, trolleyCode)
                                                                Select Case invalidatedLineFound.Split("*")(0)
                                                                    Case "1"
                                                                        invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                                        If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                                            Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                                        End If
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                                End Select
                                                            End If
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, headerId)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, addedToST)
                                            End Select
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, whseID)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, itemInfo)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, invCountId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "RT2D"
            Case "*CHECKRT2DITEMST*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim unq As String = ClientData.Split("|")(4)
                    Dim whsecode As String = ClientData.Split("|")(5)
                    Dim scannerID As String = ClientData.Split("|")(6)
                    Dim isSingleScanner As String = ClientData.Split("|")(7)
                    Dim isRecount As String = ClientData.Split("|")(8)

                    Dim headerId As String = String.Empty
                    If lotNumber = "#NOLOT#" Then
                        headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDNoLot(stNum, itemCode, whsecode)
                    Else
                        headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    End If

                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unq
                            Dim stUnqs As String = Unique.RTSQL.Retreive.MBL_GetSTUnqs(itemCode, lotNumber, qty, unq)
                            Select Case stUnqs.Split("*")(0)
                                Case "1"
                                    stUnqs = stUnqs.Remove(0, 2)
                                    Dim stUnq1 = stUnqs.Split("|")(0)
                                    Dim stUnq2 = stUnqs.Split("|")(1)
                                    'If Convert.ToBoolean(isRecount) = False Then
                                    If Convert.ToBoolean(isSingleScanner) = False Then
                                        If CLng(scannerID) Mod 2 = 0 Then
                                            'Even
                                            If stUnq2 <> stNum Then
                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                            End If
                                        Else
                                            'Odd
                                            If stUnq1 <> stNum Then
                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                            End If
                                        End If
                                    Else
                                        If stUnq1 <> stNum And stUnq2 <> stNum Then
                                            Server.Listener.SendResponse(ClientSocket, "1*Success")
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                        End If
                                    End If
                                    'Else
                                    If stUnq1 <> stNum And stUnq2 <> stNum Then
                                        Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                        Select Case invalidatedLineFound.Split("*")(0)
                                            Case "1"
                                                invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                End If
                                            Case "0"
                                                Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                    End If
                                    'End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, stUnqs)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, stUnqs)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKTAKERT2D*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim unq As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = ClientData.Split("|")(8)
                    Dim isRecount As String = ClientData.Split("|")(9)
                    Dim username As String = ClientData.Split("|")(10)

                    Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unq
                    Dim headerId As String = String.Empty
                    If Convert.ToBoolean(isRecount) = False Then
                        'Non recount boxes
#Region "Normal Count"
                        If lotNumber <> "#NOLOT#" Then
                            'Lot items
                            headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                            Select Case headerId.Split("*")(0)
                                Case "1"
                                    headerId = headerId.Remove(0, 2)
                                    Dim ticketCounts As String = StockTake.RTSQL.Retreive.MBL_checkTicketCounts(headerId, ticketNo)
                                    Select Case ticketCounts.Split("*")(0)
                                        Case "1"
#Region "Update Ticket"
                                            'update existing ticket count
                                            ticketCounts = ticketCounts.Remove(0, 2)
                                            Dim count1 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                            Dim count2 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                            Dim ticketUnq As String = ticketCounts.Split("|")(2)
                                            If Convert.ToBoolean(isSingleScanner) = True Then
                                                If CLng(scannerID) Mod 2 = 0 Then
                                                    'Even
                                                    If ticketUnq.Contains(itemCode) And ticketUnq.Contains(lotNumber) And ticketUnq.Contains(qty) And ticketUnq.Contains(unq) Then
                                                        If count1 <> 0 And count2 = 0 Then
                                                            Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount2_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                            Select Case ticketUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                    Select Case updated.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST2(itemCode, lotNumber, qty, unq, stNum))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                        End If
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                    End If
                                                Else
                                                    'Odd
                                                    If ticketUnq.Contains(itemCode) And ticketUnq.Contains(lotNumber) And ticketUnq.Contains(qty) And ticketUnq.Contains(unq) Then
                                                        If count1 = 0 And count2 <> 0 Then
                                                            Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                            Select Case ticketUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                    Select Case updated.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST1(itemCode, lotNumber, qty, unq, stNum))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                        End If
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                    End If
                                                End If
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                            End If
#End Region
                                        Case "0"
#Region "Insert Ticket"
                                            Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                            Select Case ticketRefFound.Split("*")(0)
                                                Case "1"
                                                    Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, RT2D)
                                                    Select Case ticketUnq.Split("*")(0)
                                                        Case "1"
                                                            ticketUnq = ticketUnq.Remove(0, 2)
                                                            If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                                If Convert.ToBoolean(isSingleScanner) = True Then
                                                                    If CLng(scannerID) Mod 2 = 0 Then
                                                                        'Even
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST2(itemCode, lotNumber, qty, unq, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                        End Select
                                                                    Else
                                                                        'Odd
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST1(itemCode, lotNumber, qty, unq, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                        End Select
                                                                    End If
                                                                Else
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                            Select Case updated.Split("*")(0)
                                                                                Case "1"
                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemSTBoth(itemCode, lotNumber, qty, unq, stNum))
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                            End If
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                            End Select
                                            'insert new ticket count

#End Region
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, ticketCounts)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                            End Select
                        Else
                            'Non Lot Items
                            headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDNoLot(stNum, itemCode, whsecode)
                            Select Case headerId.Split("*")(0)
                                Case "1"
                                    headerId = headerId.Remove(0, 2)
                                    Dim ticketCounts As String = StockTake.RTSQL.Retreive.MBL_checkTicketCounts(headerId, ticketNo)
                                    Select Case ticketCounts.Split("*")(0)
                                        Case "1"
#Region "Update Tickets"
                                            ticketCounts = ticketCounts.Remove(0, 2)
                                            Dim count1 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                            Dim count2 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                            Dim ticketUnq As String = ticketCounts.Split("|")(2)
                                            If Convert.ToBoolean(isSingleScanner) = False Then
                                                If CLng(scannerID) Mod 2 = 0 Then
                                                    'Even
                                                    If ticketUnq.Contains(itemCode) And ticketUnq.Contains(lotNumber) And ticketUnq.Contains(qty) And ticketUnq.Contains(unq) Then
                                                        If count1 <> 0 And count2 = 0 Then
                                                            Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount2_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                            Select Case ticketUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2(stNum, itemCode, whsecode, qty)
                                                                    Select Case updated.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST2(itemCode, lotNumber, qty, unq, stNum))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                        End If
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                    End If
                                                Else
                                                    'Odd
                                                    If ticketUnq.Contains(itemCode) And ticketUnq.Contains(lotNumber) And ticketUnq.Contains(qty) And ticketUnq.Contains(unq) Then
                                                        If count1 = 0 And count2 <> 0 Then
                                                            Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                            Select Case ticketUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem(stNum, itemCode, whsecode, qty)
                                                                    Select Case updated.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST1(itemCode, lotNumber, qty, unq, stNum))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                        End If
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                    End If
                                                End If
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                            End If
#End Region
                                        Case "0"
#Region "Insert Tickets"
                                            Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                            Select Case ticketRefFound.Split("*")(0)
                                                Case "1"
                                                    Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, RT2D)
                                                    Select Case ticketUnq.Split("*")(0)
                                                        Case "1"
                                                            ticketUnq = ticketUnq.Remove(0, 2)
                                                            If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                                If Convert.ToBoolean(isSingleScanner) = False Then
                                                                    If CLng(scannerID) Mod 2 = 0 Then
                                                                        'Even
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2(stNum, itemCode, whsecode, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST2(itemCode, lotNumber, qty, unq, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                        End Select
                                                                    Else
                                                                        'Odd
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem(stNum, itemCode, whsecode, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemST1(itemCode, lotNumber, qty, unq, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                        End Select
                                                                    End If
                                                                Else
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth(stNum, itemCode, whsecode, qty)
                                                                            Select Case updated.Split("*")(0)
                                                                                Case "1"
                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemSTBoth(itemCode, lotNumber, qty, unq, stNum))
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                            End If
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                    End Select
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                            End Select

#End Region
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, ticketCounts)
                                    End Select
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                            End Select
                        End If

#End Region
                    Else
#Region "Recount"
                        If lotNumber <> "#NOLOT#" Then
                            headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                            Select Case headerId.Split("*")(0)
                                Case "1"
                                    headerId = headerId.Remove(0, 2)
                                    Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                    Select Case invalidatedRecordFound.Split("*")(0)
                                        Case "1"
                                            invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                            If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                Select Case ticketfound.Split("*")(0)
                                                    Case "1"
                                                        Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                        Select Case ticketInserted.Split("*")(0)
                                                            Case "1"
                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                Select Case updated.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemSTBoth(itemCode, lotNumber, qty, unq, stNum))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                End Select
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                        End Select
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                End Select
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                            End Select
                        Else
                            headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDNoLot(stNum, itemCode, whsecode)
                            Select Case headerId.Split("*")(0)
                                Case "1"
                                    headerId = headerId.Remove(0, 2)
                                    Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                    Select Case invalidatedRecordFound.Split("*")(0)
                                        Case "1"
                                            invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                            If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                Select Case ticketfound.Split("*")(0)
                                                    Case "1"
                                                        Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "RT2D", RT2D)
                                                        Select Case ticketInserted.Split("*")(0)
                                                            Case "1"
                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth(stNum, itemCode, whsecode, qty)
                                                                Select Case updated.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateItemSTBoth(itemCode, lotNumber, qty, unq, stNum))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                End Select
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                        End Select
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                End Select
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, headerId)
                            End Select
                        End If

#End Region
                    End If
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "FG Pallets"
            Case "*CHECKPALLETST*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim unq As String = ClientData.Split("|")(4)
                    Dim whsecode As String = ClientData.Split("|")(5)
                    Dim scannerID As String = ClientData.Split("|")(6)
                    Dim isSingleScanner As String = ClientData.Split("|")(7)
                    Dim isRecount As String = ClientData.Split("|")(8)

                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unq
                            Dim stUnqs As String = Unique.RTSQL.Retreive.MBL_GetSTPalletUnqs(RT2D)
                            Select Case stUnqs.Split("*")(0)
                                Case "1"
                                    stUnqs = stUnqs.Remove(0, 2)
                                    Dim stUnq1 = stUnqs.Split("|")(0)
                                    Dim stUnq2 = stUnqs.Split("|")(1)
                                    If Convert.ToBoolean(isRecount) = False Then
                                        If Convert.ToBoolean(isSingleScanner) = False Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If stUnq2 <> stNum Then
                                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                End If
                                            Else
                                                'Odd
                                                If stUnq1 <> stNum Then
                                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                End If
                                            End If
                                        Else
                                            If stUnq1 <> stNum And stUnq2 <> stNum Then
                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                            End If
                                        End If
                                    Else
                                        If stUnq1 <> stNum And stUnq2 <> stNum Then
                                            Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                            Select Case invalidatedLineFound.Split("*")(0)
                                                Case "1"
                                                    invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                    If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                        Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                    End If
                                                Case "0"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                            End Select
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                        End If
                                    End If
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, stUnqs)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKTAKEPALLET*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim unq As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = ClientData.Split("|")(8)
                    Dim isRecount As String = ClientData.Split("|")(9)
                    Dim username As String = ClientData.Split("|")(10)

                    Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unq
                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim palletUnqs As String = StockTake.RTSQL.Retreive.MBL_GetAllPalletBarcodes(RT2D)
                            Select Case palletUnqs.Split("*")(0)
                                Case "1"
                                    palletUnqs = palletUnqs.Remove(0, 2)
                                    Dim allPalletUnqs As String() = palletUnqs.Split("~")
                                    If Convert.ToBoolean(isRecount) = False Then
#Region "Normal Count"
                                        Dim ticketCounts As String = StockTake.RTSQL.Retreive.MBL_checkTicketCounts(headerId, ticketNo)
                                        Select Case ticketCounts.Split("*")(0)
                                            Case "1"
#Region "Update"
                                                ticketCounts = ticketCounts.Remove(0, 2)
                                                Dim count1 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                Dim count2 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                Dim ticketUnq As String = ticketCounts.Split("|")(2)
                                                If Convert.ToBoolean(isSingleScanner) = True Then
                                                    If CLng(scannerID) Mod 2 = 0 Then
                                                        'Even
                                                        If ticketUnq = RT2D Then
                                                            If count1 <> 0 And count2 = 0 Then
                                                                'ADD HERE
                                                                Dim updateQuery As String = String.Empty
                                                                For Each unqBarcode As String In allPalletUnqs
                                                                    If unqBarcode <> String.Empty Then
                                                                        updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                    End If
                                                                Next
                                                                Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                Select Case batchUpdated.Split("*")(0)
                                                                    Case "1"
                                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount2_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                                        Select Case ticketUpdated.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST2(RT2D, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                                        End Select
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                End Select
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                        End If
                                                    Else
                                                        'Odd
                                                        If ticketUnq = RT2D Then
                                                            If count1 = 0 And count2 <> 0 Then
                                                                Dim updateQuery As String = String.Empty
                                                                For Each unqBarcode As String In allPalletUnqs
                                                                    If unqBarcode <> String.Empty Then
                                                                        updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                    End If
                                                                Next
                                                                Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                Select Case batchUpdated.Split("*")(0)
                                                                    Case "1"
                                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                                        Select Case ticketUpdated.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST1(RT2D, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                End Select
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                                        End Select
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                End Select
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 1")
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                        End If
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                                End If
#End Region
                                            Case "0"
#Region "Insert"
                                                Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                                Select Case ticketRefFound.Split("*")(0)
                                                    Case "1"
                                                        Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, RT2D)
                                                        Select Case ticketUnq.Split("*")(0)
                                                            Case "1"
                                                                ticketUnq = ticketUnq.Remove(0, 2)
                                                                If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                                    If Convert.ToBoolean(isSingleScanner) = True Then
                                                                        If CLng(scannerID) Mod 2 = 0 Then
                                                                            'Even
                                                                            Dim updateQuery As String = String.Empty
                                                                            For Each unqBarcode As String In allPalletUnqs
                                                                                If unqBarcode <> String.Empty Then
                                                                                    updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                                End If
                                                                            Next
                                                                            Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                            Select Case batchUpdated.Split("*")(0)
                                                                                Case "1"
                                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "FG Pallet", RT2D)
                                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                                        Case "1"
                                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                            Select Case updated.Split("*")(0)
                                                                                                Case "1"
                                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST2(RT2D, stNum))
                                                                                                Case Else
                                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                                            End Select
                                                                                        Case "-1"
                                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                                    End Select
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                            End Select
                                                                        Else
                                                                            'Odd
                                                                            Dim updateQuery As String = String.Empty
                                                                            For Each unqBarcode As String In allPalletUnqs
                                                                                If unqBarcode <> String.Empty Then
                                                                                    updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                                End If
                                                                            Next
                                                                            Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                            Select Case batchUpdated.Split("*")(0)
                                                                                Case "1"
                                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "FG Pallet", RT2D)
                                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                                        Case "1"
                                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                            Select Case updated.Split("*")(0)
                                                                                                Case "1"
                                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST1(RT2D, stNum))
                                                                                                Case Else
                                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                                            End Select
                                                                                        Case "-1"
                                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                                    End Select
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                            End Select
                                                                        End If
                                                                    Else
                                                                        Dim updateQuery As String = String.Empty
                                                                        For Each unqBarcode As String In allPalletUnqs
                                                                            If unqBarcode <> String.Empty Then
                                                                                updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}', [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                            End If
                                                                        Next
                                                                        Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                        Select Case batchUpdated.Split("*")(0)
                                                                            Case "1"
                                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "FG Pallet", RT2D)
                                                                                Select Case ticketInserted.Split("*")(0)
                                                                                    Case "1"
                                                                                        Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                        Select Case updated.Split("*")(0)
                                                                                            Case "1"
                                                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletSTBoth(RT2D, stNum))
                                                                                            Case Else
                                                                                                Server.Listener.SendResponse(ClientSocket, updated)
                                                                                        End Select
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                                End Select
                                                                            Case Else
                                                                                Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                        End Select
                                                                    End If
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                                End If


                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                        End Select

                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                                End Select

                                            Case Else
                                                Server.Listener.SendResponse(ClientSocket, ticketCounts)
                                        End Select
#End Region

#End Region

                                    Else

#Region "Recount"
                                        Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                        Select Case ticketRefFound.Split("*")(0)
                                            Case "1"
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                            Dim updateQuery As String = String.Empty
                                                            For Each unqBarcode As String In allPalletUnqs
                                                                If unqBarcode <> String.Empty Then
                                                                    updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}', [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                End If
                                                            Next
                                                            Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                            Select Case batchUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "FG Pallet", RT2D)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth(stNum, itemCode, whsecode, qty)
                                                                            Select Case updated.Split("*")(0)
                                                                                Case "1"
                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletSTBoth(RT2D, stNum))
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
                                            Case Else
                                                Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                        End Select

#End Region
                                    End If
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, palletUnqs)
                            End Select

                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "RM Pallets"
            Case "*CHECKRMPALLETST*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim qty As String = ClientData.Split("|")(2)
                    Dim unq As String = ClientData.Split("|")(3)
                    Dim whsecode As String = ClientData.Split("|")(4)
                    Dim scannerID As String = ClientData.Split("|")(5)
                    Dim isSingleScanner As String = ClientData.Split("|")(6)
                    Dim isRecount As String = ClientData.Split("|")(7)
                    Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + "#PALLET#".PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unq
                    Dim lotNumbers As String = Unique.RTSQL.Retreive.MBL_GetSTPalletLots(RT2D)
                    Select Case lotNumbers.Split("*")(0)
                        Case "1"
                            lotNumbers = lotNumbers.Remove(0, 2)
                            Dim lotInString As String = String.Empty
                            For Each lot As String In lotNumbers.Split("~")
                                lotInString = lotInString + "'" + lot + "',"
                            Next
                            lotInString = "(" + lotInString.Substring(0, lotInString.Length - 1) + ")"
                            Dim lotIds As String = StockTake.Evolution.Retreive.MBL_GetLotIds(lotInString, itemCode)
                            Select Case lotIds.Split("*")(0)
                                Case "1"
                                    Dim headerIds As String = StockTake.RTSQL.Retreive.MBL_GetSTLineIDsRMPallet(stNum, itemCode, whsecode, lotNumbers)
                                    Select Case headerIds.Split("*")(0)
                                        Case "1"
                                            headerIds = headerIds.Remove(0, 2)
                                            Dim lotCount As String() = lotIds.Split("~")
                                            Dim allHeaderIDs As String() = headerIds.Split("~")
                                            Dim headerInString As String = String.Empty
                                            For Each header As String In allHeaderIDs
                                                If header <> String.Empty Then
                                                    headerInString = headerInString + "'" + header + "',"
                                                End If
                                            Next
                                            headerInString = "(" + headerInString.Substring(0, headerInString.Length - 1) + ")"
                                            If lotCount.Count = allHeaderIDs.Count Then
                                                Dim stUnqs As String = Unique.RTSQL.Retreive.MBL_GetSTPalletUnqs(RT2D)
                                                Select Case stUnqs.Split("*")(0)
                                                    Case "1"
                                                        stUnqs = stUnqs.Remove(0, 2)
                                                        Dim stUnq1 = stUnqs.Split("|")(0)
                                                        Dim stUnq2 = stUnqs.Split("|")(1)
                                                        If Convert.ToBoolean(isRecount) = False Then
                                                            If Convert.ToBoolean(isSingleScanner) = False Then
                                                                If CLng(scannerID) Mod 2 = 0 Then
                                                                    If stUnq2 <> stNum Then
                                                                        Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                    End If
                                                                Else
                                                                    If stUnq1 <> stNum Then
                                                                        Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                    End If
                                                                End If
                                                            Else
                                                                If stUnq1 <> stNum And stUnq2 <> stNum Then
                                                                    Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                End If
                                                            End If
                                                        Else
                                                            If stUnq1 <> stNum And stUnq2 <> stNum Then
                                                                Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecountRMPallet(headerInString, RT2D)
                                                                Select Case invalidatedLineFound.Split("*")(0)
                                                                    Case "1"
                                                                        invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                                        If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                                            Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                        Else
                                                                            Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                                        End If
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                                End Select
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, stUnqs)
                                                End Select
                                            Else
                                                Server.Listener.SendResponse(ClientSocket, "-1*Not all items on this pallet were found on the stock take")
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, headerIds)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, lotIds)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, lotNumbers)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKTAKERMPALLET*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim unq As String = ClientData.Split("|")(3)
                    Dim whsecode As String = ClientData.Split("|")(4)
                    Dim scannerID As String = ClientData.Split("|")(5)
                    Dim isSingleScanner As String = ClientData.Split("|")(6)
                    Dim isRecount As String = ClientData.Split("|")(7)
                    Dim username As String = ClientData.Split("|")(8)
                    Dim RT2D = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + "PALLET".PadRight(20, " ") + "(30)" + "1".PadLeft(8, "0") + "(90)" + unq

                    Dim lotNumbers As String = Unique.RTSQL.Retreive.MBL_GetSTPalletLots(RT2D)
                    Select Case lotNumbers.Split("*")(0)
                        Case "1"
                            lotNumbers = lotNumbers.Remove(0, 2)
                            Dim allLotNumbers As String() = lotNumbers.Split("~")
                            For Each lotInfo As String In allLotNumbers
                                If lotInfo <> String.Empty Then
                                    Dim lotNumber As String = lotInfo.Split("|")(0)
                                    Dim qty As String = lotInfo.Split("|")(1)
                                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                                    Dim palletUnqs As String = StockTake.RTSQL.Retreive.MBL_GetAllPalletBarcodes_RM(RT2D)
                                    Select Case palletUnqs.Split("*")(0)
                                        Case "1"
                                            palletUnqs = palletUnqs.Remove(0, 2)
                                            Dim allPalletUnqs As String() = palletUnqs.Split("~")
                                            If Convert.ToBoolean(isRecount) = False Then
#Region "Normal Count"
                                                Dim ticketCounts As String = StockTake.RTSQL.Retreive.MBL_checkTicketCounts(headerId, ticketNo)
                                                Select Case ticketCounts.Split("*")(0)
                                                    Case "1"
#Region "Update"
                                                        ticketCounts = ticketCounts.Remove(0, 2)
                                                        Dim count1 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                        Dim count2 As Decimal = Convert.ToDecimal(ticketCounts.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                        Dim ticketUnq As String = ticketCounts.Split("|")(2)
                                                        If Convert.ToBoolean(isSingleScanner) = True Then
                                                            If CLng(scannerID) Mod 2 = 0 Then
                                                                If ticketUnq = RT2D Then
                                                                    If count1 <> 0 And count2 = 0 Then
                                                                        Dim updateQuery As String = String.Empty
                                                                        For Each unqBarcode As String In allPalletUnqs
                                                                            If unqBarcode <> String.Empty Then
                                                                                updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                            End If
                                                                        Next
                                                                        Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                        Select Case batchUpdated.Split("*")(0)
                                                                            Case "1"
                                                                                Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount2_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                                                Select Case ticketUpdated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                        Select Case updated.Split("*")(0)
                                                                                            Case "1"
                                                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST2(RT2D, stNum))
                                                                                            Case Else
                                                                                                Server.Listener.SendResponse(ClientSocket, updated)
                                                                                                Exit For
                                                                                        End Select
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                                                        Exit For
                                                                                End Select
                                                                            Case Else
                                                                                Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                                Exit For
                                                                        End Select
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                                        Exit For
                                                                    End If
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                                    Exit For
                                                                End If
                                                            Else
                                                                If ticketUnq = RT2D Then
                                                                    If count1 = 0 And count2 <> 0 Then
                                                                        Dim updateQuery As String = String.Empty
                                                                        For Each unqBarcode As String In allPalletUnqs
                                                                            If unqBarcode <> String.Empty Then
                                                                                updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                            End If
                                                                        Next
                                                                        Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                        Select Case batchUpdated.Split("*")(0)
                                                                            Case "1"
                                                                                Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount_Lot(headerId, ticketNo, qty, username, itemCode, lotNumber, unq)
                                                                                Select Case ticketUpdated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Dim updated = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                        Select Case updated.Split("*")(0)
                                                                                            Case "1"
                                                                                                Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST1(RT2D, stNum))
                                                                                            Case Else
                                                                                                Server.Listener.SendResponse(ClientSocket, updated)
                                                                                                Exit For
                                                                                        End Select
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                                                        Exit For
                                                                                End Select
                                                                            Case Else
                                                                                Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                                Exit For
                                                                        End Select
                                                                    Else
                                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 1")
                                                                        Exit For
                                                                    End If
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                                    Exit For
                                                                End If
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                                            Exit For
                                                        End If
#End Region
                                                    Case "0"
#Region "Insert"
                                                        If Convert.ToBoolean(isSingleScanner) = True Then
                                                            If CLng(scannerID) Mod 2 = 0 Then
                                                                'Even
                                                                Dim updateQuery As String = String.Empty
                                                                For Each unqBarcode As String In allPalletUnqs
                                                                    If unqBarcode <> String.Empty Then
                                                                        updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                    End If
                                                                Next

                                                                Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                Select Case batchUpdated.Split("*")(0)
                                                                    Case "1"
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "RM Pallet", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST2(RT2D, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, updated)
                                                                                        Exit For
                                                                                End Select
                                                                            Case Else
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                                Exit For
                                                                        End Select
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                        Exit For
                                                                End Select
                                                            Else
                                                                'Odd
                                                                Dim updateQuery As String = String.Empty
                                                                For Each unqBarcode As String In allPalletUnqs
                                                                    If unqBarcode <> String.Empty Then
                                                                        updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                    End If
                                                                Next

                                                                Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                Select Case batchUpdated.Split("*")(0)
                                                                    Case "1"
                                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "RM Pallet", RT2D)
                                                                        Select Case ticketLogInserted.Split("*")(0)
                                                                            Case "1"
                                                                                Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                                Select Case updated.Split("*")(0)
                                                                                    Case "1"
                                                                                        Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletST1(RT2D, stNum))
                                                                                    Case Else
                                                                                        Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                                        Exit For
                                                                                End Select
                                                                            Case Else
                                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                                Exit For
                                                                        End Select
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                        Exit For
                                                                End Select
                                                            End If
                                                        Else
                                                            Dim updateQuery As String = String.Empty
                                                            For Each unqBarcode As String In allPalletUnqs
                                                                If unqBarcode <> String.Empty Then
                                                                    updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}', [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                End If
                                                            Next

                                                            Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                            Select Case batchUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "RM Pallet", RT2D)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty)
                                                                            Select Case updated.Split("*")(0)
                                                                                Case "1"
                                                                                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletSTBoth(RT2D, stNum))
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, updated)
                                                                                    Exit For
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                            Exit For
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                    Exit For
                                                            End Select
                                                        End If
#End Region
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketCounts)
                                                        Exit For
                                                End Select
#End Region
                                            Else
#Region "Recount"
                                                Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                                Select Case ticketRefFound.Split("*")(0)
                                                    Case "1"
                                                        Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, RT2D)
                                                        Select Case invalidatedRecordFound.Split("*")(0)
                                                            Case "1"
                                                                If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                                    Dim updateQuery As String = String.Empty
                                                                    For Each unqBarcode As String In allPalletUnqs
                                                                        If unqBarcode <> String.Empty Then
                                                                            updateQuery += String.Format("UPDATE [tbl_unqBarcodes] SET [StockTake] = '{1}', [StockTake2] = '{1}' WHERE [vUnqBarcode] ='{0}' " + Environment.NewLine, unqBarcode, stNum)
                                                                        End If
                                                                    Next
                                                                    Dim batchUpdated As String = StockTake.RTSQL.ExecuteQuery(updateQuery)
                                                                    Select Case batchUpdated.Split("*")(0)
                                                                        Case "1"
                                                                            Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "RM Pallet", RT2D)
                                                                            Select Case ticketInserted.Split("*")(0)
                                                                                Case "1"
                                                                                    Dim updated As String = StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth(stNum, itemCode, whsecode, qty)
                                                                                    Select Case updated.Split("*")(0)
                                                                                        Case "1"
                                                                                            Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdatePalletSTBoth(RT2D, stNum))
                                                                                        Case Else
                                                                                            Server.Listener.SendResponse(ClientSocket, updated)
                                                                                    End Select
                                                                                Case Else
                                                                                    Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                            End Select
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, batchUpdated)
                                                                    End Select
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                                End If
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                                Exit For
                                                        End Select
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                                        Exit For
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, palletUnqs)
                                            Exit For
                                    End Select
                                End If
                            Next
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, lotNumbers)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Fresh Slurry"
            Case "*GETFSLOTSTOCKTAKE*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim trolleyNumber As String = ClientData.Split("|")(1)
                    'MBL_getFreshSlurryInfo_CheckAddLot
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getFreshSlurryInfo(trolleyNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As String = tankInfo.Split("|")(2)
                            If Convert.ToBoolean(tankReceived) = False Then
                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Slurry trolley is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKFRESHSLURRYTROLLEY*"
                'Add Check for dry weight
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim trolleyNumber As String = ClientData.Split("|")(2)
                    Dim whsecode As String = ClientData.Split("|")(3)
                    Dim scannerID As String = ClientData.Split("|")(4)
                    Dim isSingleScanner As String = ClientData.Split("|")(5)
                    Dim isRecount As String = ClientData.Split("|")(6)

                    Dim trolleyCode As String = "TRO_" + trolleyNumber + "$" + itemCode
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getFreshSlurryInfo(trolleyNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As String = tankInfo.Split("|")(2)
                            If Convert.ToBoolean(tankReceived) = False Then
                                Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, tankLot)
                                Select Case headerId.Split("*")(0)
                                    Case "1"
                                        headerId = headerId.Remove(0, 2)
                                        If Convert.ToBoolean(isRecount) = False Then
                                            Dim trolleyTicketInfo As String = StockTake.RTSQL.Retreive.MBL_getSlurryTicketInfo(headerId, trolleyCode)
                                            Select Case trolleyTicketInfo.Split("*")(0)
                                                Case "1"
                                                    trolleyTicketInfo = trolleyTicketInfo.Remove(0, 2)
                                                    Dim trolleyQty1 As Decimal = Convert.ToDecimal(trolleyTicketInfo.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                    Dim trolleyQty2 As Decimal = Convert.ToDecimal(trolleyTicketInfo.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                    If Convert.ToBoolean(isSingleScanner) = False Then
                                                        If CLng(scannerID) Mod 2 = 0 Then
                                                            'Even
                                                            If trolleyQty2 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        Else
                                                            'Odd
                                                            If trolleyQty1 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        End If
                                                    Else
                                                        If trolleyQty1 = 0 And trolleyQty2 = 0 Then
                                                            Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                        End If
                                                    End If
                                                Case "0"
                                                    'Trolley not yet scanned
                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, trolleyTicketInfo)
                                            End Select
                                        Else
                                            Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, trolleyCode)
                                            Select Case invalidatedLineFound.Split("*")(0)
                                                Case "1"
                                                    invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                    If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                        Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                    End If
                                                Case "0"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                            End Select
                                        End If
                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, headerId)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Slurry trolley is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKTAKEFRESHSLURRY*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim trolleyNumber As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(9))
                    Dim username As String = ClientData.Split("|")(10)

                    Dim trolleyCode As String = "TRO_" + trolleyNumber + "$" + itemCode
                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))

                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If ticketUnq = trolleyCode Then
                                                    If count1 <> 0 And count2 = 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount2_FreshSlurry(headerId, ticketNo, qty, username, trolleyCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            Else
                                                'Odd
                                                If ticketUnq = trolleyCode Then
                                                    If count1 = 0 And count2 <> 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateSTCount_FreshSlurry(headerId, ticketNo, qty, username, trolleyCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted") 'Put eror here
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert New Ticket"
                                                Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getFreshSlurryUnqTicket(headerId, trolleyCode) 'Change to allow normal scanning after ticket is invalidated
                                                Select Case ticketUnq.Split("*")(0)
                                                    Case "1"
                                                        ticketUnq = ticketUnq.Remove(0, 2)
                                                        If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                            If isSingleScanner = True Then
                                                                If CLng(scannerID) Mod 2 = 0 Then
                                                                    'Even
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "Fresh Slurry", trolleyCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                Else
                                                                    'Odd
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "Fresh Slurry", trolleyCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "Fresh Slurry", trolleyCode)
                                                                Select Case ticketInserted.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                End Select
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                End Select
#End Region
                                            Else
#Region "Recount"
                                                'Server.Listener.SendResponse(ClientSocket, "-1*CATscan cannot recount this item as it has not been countd yet")
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, trolleyCode)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                            Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                            Select Case ticketfound.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "Fresh Slurry", trolleyCode)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Mixed Slurry"

#Region "Tank"
            Case "*GETMSLOTSTOCKTAKE*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim tankNumber As String = ClientData.Split("|")(1)
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getMixedSlurryTankInfo_CheckLot(tankNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As Boolean = Convert.ToBoolean(tankInfo.Split("|")(2))
                            Dim tankClosed As Boolean = Convert.ToBoolean(tankInfo.Split("|")(3))
                            If Convert.ToBoolean(tankReceived) = False And Convert.ToBoolean(tankClosed) = False Then
                                If tankQty <> String.Empty Then
                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                Else
                                    Server.Listener.SendResponse(ClientSocket, "-1*No dry weight found for this tank please use the manual count function!")
                                End If
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Slurry trolley is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKMIXEDSLURRYTANKST*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim tankNumber As String = ClientData.Split("|")(2)
                    Dim whsecode As String = ClientData.Split("|")(3)
                    Dim scannerID As String = ClientData.Split("|")(4)
                    Dim isSingleScanner As String = ClientData.Split("|")(5)
                    Dim isRecount As String = ClientData.Split("|")(6)

                    Dim tankCode As String = "TNK_" + tankNumber + "$" + itemCode
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getMixedSlurryTankInfo(tankNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As Boolean = Convert.ToBoolean(tankInfo.Split("|")(2))
                            Dim tankClosed As Boolean = Convert.ToBoolean(tankInfo.Split("|")(3))
                            If Convert.ToBoolean(tankReceived) = False And Convert.ToBoolean(tankClosed) = False Then
                                If tankQty <> String.Empty Then
                                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, tankLot)
                                    Select Case headerId.Split("*")(0)
                                        Case "1"
                                            headerId = headerId.Remove(0, 2)
                                            If Convert.ToBoolean(isRecount) = False Then
                                                Dim tankTicketInfo As String = StockTake.RTSQL.Retreive.MBL_getSlurryTicketInfo(headerId, tankCode)
                                                Select Case tankTicketInfo.Split("*")(0)
                                                    Case "1"
                                                        tankTicketInfo = tankTicketInfo.Remove(0, 2)
                                                        Dim tankQty1 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                        Dim tankQty2 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                        If Convert.ToBoolean(isSingleScanner) = False Then
                                                            If CLng(scannerID) Mod 2 = 0 Then
                                                                'Even
                                                                If tankQty2 = 0 Then
                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                End If
                                                            Else
                                                                'Odd
                                                                If tankQty1 = 0 Then
                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                End If
                                                            End If
                                                        Else
                                                            If tankQty1 = 0 And tankQty2 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        End If
                                                    Case "0"
                                                        'Trolley not yet scanned
                                                        Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, tankTicketInfo)
                                                End Select
                                            Else
                                                Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, tankCode)
                                                Select Case invalidatedLineFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                            Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                        End If
                                                    Case "0"
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                End Select
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, headerId)
                                    End Select
                                Else
                                    Server.Listener.SendResponse(ClientSocket, "-1*No dry weight found for this tank please use the manual count function!")
                                End If
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*This slurry tank is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKMIXEDSLURRYTANK*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim tankNumber As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(9))
                    Dim username As String = ClientData.Split("|")(10)

                    Dim tankCode As String = "TNK_" + tankNumber + "$" + itemCode
                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))
                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If ticketUnq = tankCode Then
                                                    If count1 <> 0 And count2 = 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount2(headerId, ticketNo, qty, username, tankCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            Else
                                                'Odd
                                                If ticketUnq = tankCode Then
                                                    If count1 = 0 And count2 <> 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount(headerId, ticketNo, qty, username, tankCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted")
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert New Ticket"
                                                Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, tankCode) 'Change to allow normal scanning after ticket is invalidated
                                                Select Case ticketUnq.Split("*")(0)
                                                    Case "1"
                                                        ticketUnq = ticketUnq.Remove(0, 2)
                                                        If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                            If isSingleScanner = True Then
                                                                If CLng(scannerID) Mod 2 = 0 Then
                                                                    'Even
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                Else
                                                                    'Odd
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                Select Case ticketInserted.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                End Select
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                End Select
#End Region
                                            Else
#Region "Recount"
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, tankCode)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                            Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                            Select Case ticketfound.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select

                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Mobile Tank"
            Case "*GETMBLMSLOTSTOCKTAKE*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim tankNumber As String = ClientData.Split("|")(1)
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getMixedSlurryMobileTankInfo_CheckLot(tankNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As Boolean = Convert.ToBoolean(tankInfo.Split("|")(2))
                            Dim tankClosed As Boolean = Convert.ToBoolean(tankInfo.Split("|")(3))
                            If Convert.ToBoolean(tankReceived) = False And Convert.ToBoolean(tankClosed) = False Then
                                If tankQty <> String.Empty Then
                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                Else
                                    Server.Listener.SendResponse(ClientSocket, "-1*No dry weight found for this tank please use the manual count function!")
                                End If
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*Slurry trolley is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*CHECKMIXEDSLURRYMOBILETANKST*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim tankNumber As String = ClientData.Split("|")(2)
                    Dim whsecode As String = ClientData.Split("|")(3)
                    Dim scannerID As String = ClientData.Split("|")(4)
                    Dim isSingleScanner As String = ClientData.Split("|")(5)
                    Dim isRecount As String = ClientData.Split("|")(6)

                    Dim tankCode As String = "MTNK_" + tankNumber + "$" + itemCode
                    Dim tankInfo As String = StockTake.RTSQL.Retreive.MBL_getMixedSlurryMobileTankInfo(tankNumber, itemCode)
                    Select Case tankInfo.Split("*")(0)
                        Case "1"
                            tankInfo = tankInfo.Remove(0, 2)
                            Dim tankLot As String = tankInfo.Split("|")(0)
                            Dim tankQty As String = tankInfo.Split("|")(1)
                            Dim tankReceived As Boolean = Convert.ToBoolean(tankInfo.Split("|")(2))
                            Dim tankClosed As Boolean = Convert.ToBoolean(tankInfo.Split("|")(3))
                            If Convert.ToBoolean(tankReceived) = False And Convert.ToBoolean(tankClosed) = False Then
                                If tankQty <> String.Empty Then
                                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, tankLot)
                                    Select Case headerId.Split("*")(0)
                                        Case "1"
                                            headerId = headerId.Remove(0, 2)
                                            If Convert.ToBoolean(isRecount) = False Then
                                                Dim tankTicketInfo As String = StockTake.RTSQL.Retreive.MBL_getSlurryTicketInfo(headerId, tankCode)
                                                Select Case tankTicketInfo.Split("*")(0)
                                                    Case "1"
                                                        tankTicketInfo = tankTicketInfo.Remove(0, 2)
                                                        Dim tankQty1 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                        Dim tankQty2 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                        If Convert.ToBoolean(isSingleScanner) = False Then
                                                            If CLng(scannerID) Mod 2 = 0 Then
                                                                'Even
                                                                If tankQty2 = 0 Then
                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                End If
                                                            Else
                                                                'Odd
                                                                If tankQty1 = 0 Then
                                                                    Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                                Else
                                                                    Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                                End If
                                                            End If
                                                        Else
                                                            If tankQty1 = 0 And tankQty2 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        End If
                                                    Case "0"
                                                        'Trolley not yet scanned
                                                        Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, tankTicketInfo)
                                                End Select
                                            Else
                                                Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, tankCode)
                                                Select Case invalidatedLineFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                            Server.Listener.SendResponse(ClientSocket, "1*" + tankLot + "|" + tankQty)
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                        End If
                                                    Case "0"
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                End Select
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, headerId)
                                    End Select
                                Else
                                    Server.Listener.SendResponse(ClientSocket, "-1*No dry weight found for this tank please use the manual count function!")
                                End If
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*This slurry tank is empty in CATscan")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, tankInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKMIXEDSLURRYMOBILETANK*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim tankNumber As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(9))
                    Dim username As String = ClientData.Split("|")(10)

                    Dim tankCode As String = "MTNK_" + tankNumber + "$" + itemCode
                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))
                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If ticketUnq = tankCode Then
                                                    If count1 <> 0 And count2 = 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount2(headerId, ticketNo, qty, username, tankCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            Else
                                                'Odd
                                                If ticketUnq = tankCode Then
                                                    If count1 = 0 And count2 <> 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount(headerId, ticketNo, qty, username, tankCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted")
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert New Ticket"
                                                Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, tankCode) 'Change to allow normal scanning after ticket is invalidated
                                                Select Case ticketUnq.Split("*")(0)
                                                    Case "1"
                                                        ticketUnq = ticketUnq.Remove(0, 2)
                                                        If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                            If isSingleScanner = True Then
                                                                If CLng(scannerID) Mod 2 = 0 Then
                                                                    'Even
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                Else
                                                                    'Odd
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "Mixed Slurry Tank", tankCode)
                                                                Select Case ticketInserted.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                End Select
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                End Select
#End Region
                                            Else
#Region "Recount"
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, tankCode)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                            Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                            Select Case ticketfound.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "Mixed Slurry Mobile Tank", tankCode)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "PGM"
            Case "*CHECKPGMSTOCKTAKE*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim contNumber As String = ClientData.Split("|")(2)
                    Dim whsecode As String = ClientData.Split("|")(3)
                    Dim scannerID As String = ClientData.Split("|")(4)
                    Dim isSingleScanner As String = ClientData.Split("|")(5)
                    Dim isRecount As String = ClientData.Split("|")(6)

                    Dim contCode As String = "CONT_" + contNumber + "$" + itemCode
                    Dim contInfo As String = StockTake.RTSQL.Retreive.MBL_getPGMContainerInfo(contNumber, itemCode)
                    Select Case contInfo.Split("*")(0)
                        Case "1"
                            contInfo = contInfo.Remove(0, 2)
                            Dim contLot As String = contInfo.Split("|")(0)
                            Dim contQty As String = contInfo.Split("|")(1)
                            Dim contReceived As Boolean = Convert.ToBoolean(contInfo.Split("|")(2))
                            If Convert.ToBoolean(contReceived) = False Then
                                Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, contLot)
                                Select Case headerId.Split("*")(0)
                                    Case "1"
                                        headerId = headerId.Remove(0, 2)
                                        If Convert.ToBoolean(isRecount) = False Then
                                            Dim tankTicketInfo As String = StockTake.RTSQL.Retreive.MBL_getSlurryTicketInfo(headerId, contCode)
                                            Select Case tankTicketInfo.Split("*")(0)
                                                Case "1"
                                                    tankTicketInfo = tankTicketInfo.Remove(0, 2)
                                                    Dim tankQty1 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                                    Dim tankQty2 As Decimal = Convert.ToDecimal(tankTicketInfo.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                                    If Convert.ToBoolean(isSingleScanner) = False Then
                                                        If CLng(scannerID) Mod 2 = 0 Then
                                                            'Even
                                                            If tankQty2 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        Else
                                                            'Odd
                                                            If tankQty1 = 0 Then
                                                                Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                                            Else
                                                                Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                            End If
                                                        End If
                                                    Else
                                                        If tankQty1 = 0 And tankQty2 = 0 Then
                                                            Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*Item already scanned on this stock take")
                                                        End If
                                                    End If
                                                Case "0"
                                                    'Trolley not yet scanned
                                                    Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, tankTicketInfo)
                                            End Select
                                        Else
                                            Dim invalidatedLineFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, contCode)
                                            Select Case invalidatedLineFound.Split("*")(0)
                                                Case "1"
                                                    invalidatedLineFound = invalidatedLineFound.Remove(0, 2)
                                                    If Convert.ToBoolean(invalidatedLineFound) = False Then
                                                        Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The system cannot recount this item as it has already assigned to a ticket")
                                                    End If
                                                Case "0"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, invalidatedLineFound)
                                            End Select
                                        End If
                                    Case "0"
                                        'item not on stock take, dont do ticket check
                                        Server.Listener.SendResponse(ClientSocket, "1*" + contLot + "|" + contQty)
                                    Case Else
                                        Server.Listener.SendResponse(ClientSocket, headerId)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "-1*PGM Container is empty in CATscan, no manufacture has been recorded to it")
                            End If
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, contInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*UPDATESTOCKTAKEPGM*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim contNumber As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(9))
                    Dim username As String = ClientData.Split("|")(10)

                    Dim contCode As String = "CONT_" + contNumber + "$" + itemCode
                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))
                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If ticketUnq = contCode Then
                                                    If count1 <> 0 And count2 = 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount2(headerId, ticketNo, qty, username, contCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            Else
                                                'Odd
                                                If ticketUnq = contCode Then
                                                    If count1 = 0 And count2 <> 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount(headerId, ticketNo, qty, username, contCode)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted")
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert New Ticket"
                                                Dim ticketUnq As String = StockTake.RTSQL.Retreive.MBL_getUnqcodeTicket(headerId, contCode) 'Change to allow normal scanning after ticket is invalidated
                                                Select Case ticketUnq.Split("*")(0)
                                                    Case "1"
                                                        ticketUnq = ticketUnq.Remove(0, 2)
                                                        If ticketNo = ticketUnq Or ticketUnq = String.Empty Then
                                                            If isSingleScanner = True Then
                                                                If CLng(scannerID) Mod 2 = 0 Then
                                                                    'Even
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "PGM Container", contCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                Else
                                                                    'Odd
                                                                    Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "PGM Container", contCode)
                                                                    Select Case ticketLogInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                                    End Select
                                                                End If
                                                            Else
                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "PGM Container", contCode)
                                                                Select Case ticketInserted.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                End Select
                                                            End If
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "-1*The barcode scanned is already associated with another ticket")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketUnq)
                                                End Select
#End Region
                                            Else
#Region "Recount"
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkRT2dForRecount(headerId, contCode)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        invalidatedRecordFound = invalidatedRecordFound.Remove(0, 2)
                                                        If Convert.ToBoolean(invalidatedRecordFound) = False Then
                                                            Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                            Select Case ticketfound.Split("*")(0)
                                                                Case "1"
                                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "PGM Container", contCode)
                                                                    Select Case ticketInserted.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                        Case Else
                                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                    End Select
                                                                Case Else
                                                                    Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                            End Select
                                                        Else
                                                            Server.Listener.SendResponse(ClientSocket, "0*The item cannot be recounted as there is still a valid counted quantity for this item")
                                                        End If
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Powder Prep"
            Case "*UPDATEPOWDERPREPSTOCKTAKE*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim unq As String = ClientData.Split("|")(5)
                    Dim whsecode As String = ClientData.Split("|")(6)
                    Dim scannerID As String = ClientData.Split("|")(7)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(9))
                    Dim username As String = ClientData.Split("|")(10)

                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))
                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If ticketUnq = unq Then
                                                    If count1 <> 0 And count2 = 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount2(headerId, ticketNo, qty, username, unq)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            Else
                                                'Odd
                                                If ticketUnq = unq Then
                                                    If count1 = 0 And count2 <> 0 Then
                                                        Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount(headerId, ticketNo, qty, username, unq)
                                                        Select Case ticketUpdated.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                        End Select
                                                    Else
                                                        Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                    End If
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket is not associated with this barcode")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted")
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert New Ticket"
                                                If isSingleScanner = True Then
                                                    If CLng(scannerID) Mod 2 = 0 Then
                                                        'Even
                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "Powder", unq)
                                                        Select Case ticketLogInserted.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                        End Select
                                                    Else
                                                        'Odd
                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "Powder", unq)
                                                        Select Case ticketLogInserted.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                        End Select
                                                    End If
                                                Else
                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "Powder", unq)
                                                    Select Case ticketInserted.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                    End Select
                                                End If
#End Region
                                            Else
#Region "Recount"
                                                Dim invalidatedRecordFound As String = StockTake.RTSQL.Retreive.MBL_checkPowderPrepForRecount(headerId, unq)
                                                Select Case invalidatedRecordFound.Split("*")(0)
                                                    Case "1"
                                                        Dim ticketfound As String = StockTake.RTSQL.Retreive.MBL_checkTicketNumber(headerId, ticketNo)
                                                        Select Case ticketfound.Split("*")(0)
                                                            Case "1"
                                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "Powder", unq)
                                                                Select Case ticketInserted.Split("*")(0)
                                                                    Case "1"
                                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                                    Case Else
                                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                                End Select
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketfound)
                                                        End Select
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, invalidatedRecordFound)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Manual"
            Case "*SAVESTOCKTAKEMANUAL*"
                Try
                    Dim stNum As String = ClientData.Split("|")(0)
                    Dim ticketNo As String = ClientData.Split("|")(1)
                    Dim itemCode As String = ClientData.Split("|")(2)
                    Dim lotNumber As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim whsecode As String = ClientData.Split("|")(5)
                    Dim scannerID As String = ClientData.Split("|")(6)
                    Dim isSingleScanner As String = Convert.ToBoolean(ClientData.Split("|")(7))
                    Dim isRecount As Boolean = Convert.ToBoolean(ClientData.Split("|")(8))
                    Dim username As String = ClientData.Split("|")(9)

                    Dim headerId = StockTake.RTSQL.Retreive.MBL_GetSTLineIDLot(stNum, itemCode, whsecode, lotNumber)
                    Select Case headerId.Split("*")(0)
                        Case "1"
                            headerId = headerId.Remove(0, 2)
                            Dim ticketRecord As String = StockTake.RTSQL.Retreive.MBL_getTicketUpdateInfo(headerId, ticketNo)
                            Select Case ticketRecord.Split("*")(0)
                                Case "1"
                                    ticketRecord = ticketRecord.Remove(0, 2)
                                    Dim count1 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(0).Replace(",", sep).Replace(".", sep))
                                    Dim count2 As Decimal = Convert.ToDecimal(ticketRecord.Split("|")(1).Replace(",", sep).Replace(".", sep))
                                    Dim ticketUnq As String = ticketRecord.Split("|")(2)
                                    Dim ticketValid As Boolean = Convert.ToBoolean(ticketRecord.Split("|")(3))
                                    If ticketValid = True And isRecount = False Then
#Region "Update Ticket"
                                        If isSingleScanner = True Then
                                            If CLng(scannerID) Mod 2 = 0 Then
                                                'Even
                                                If count1 <> 0 And count2 = 0 Then
                                                    Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount2(headerId, ticketNo, qty, username, "NA - Manual Count")
                                                    Select Case ticketUpdated.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                    End Select
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 2")
                                                End If
                                            Else
                                                'Odd
                                                If count1 = 0 And count2 <> 0 Then
                                                    Dim ticketUpdated As String = StockTake.RTSQL.Update.MBL_UpdateTicketSTCount(headerId, ticketNo, qty, username, "NA - Manual Count")
                                                    Select Case ticketUpdated.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, ticketUpdated)
                                                    End Select
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted on count 1")
                                                End If
                                            End If
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, "-1*The ticket has already been counted this stock take")
                                        End If
#End Region
                                    ElseIf ticketValid = False And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*Cannot recount on this ticket as it has already been ivaliadted")
                                    ElseIf ticketValid = False And isRecount = False Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket has been invalidated, please use the recount function")
                                    ElseIf ticketValid = True And isRecount = True Then
                                        Server.Listener.SendResponse(ClientSocket, "-1*The current ticket for this item is still valid and cannot be recounted")
                                    End If
                                Case "0"
                                    Dim ticketRefFound As String = StockTake.RTSQL.Retreive.MBL_checkTicketRef(stNum, ticketNo)
                                    Select Case ticketRefFound.Split("*")(0)
                                        Case "1"
                                            If isRecount = False Then
#Region "Insert Ticket"
                                                If isSingleScanner = True Then
                                                    If CLng(scannerID) Mod 2 = 0 Then
                                                        'Even
                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog2(headerId, ticketNo, qty, username, "Manual", "NA - Manual Count")
                                                        Select Case ticketLogInserted.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem2_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                        End Select
                                                    Else
                                                        'Odd
                                                        Dim ticketLogInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLog(headerId, ticketNo, qty, username, "Manual", "NA - Manual Count")
                                                        Select Case ticketLogInserted.Split("*")(0)
                                                            Case "1"
                                                                Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItem_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                            Case Else
                                                                Server.Listener.SendResponse(ClientSocket, ticketLogInserted)
                                                        End Select
                                                    End If
                                                Else
                                                    Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogBoth(headerId, ticketNo, qty, username, "Manual", "NA - Manual Count")
                                                    Select Case ticketInserted.Split("*")(0)
                                                        Case "1"
                                                            Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                        Case Else
                                                            Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                    End Select
                                                End If
#End Region
                                            Else
#Region "Recount"
                                                Dim ticketInserted As String = StockTake.RTSQL.Insert.MBL_InsertTicketLogRecount(headerId, ticketNo, qty, username, "Manual", "NA - Manual Count")
                                                Select Case ticketInserted.Split("*")(0)
                                                    Case "1"
                                                        Server.Listener.SendResponse(ClientSocket, StockTake.RTSQL.Update.MBL_UpdateRTStockTakeItemBoth_Lot(stNum, itemCode, whsecode, lotNumber, qty))
                                                    Case Else
                                                        Server.Listener.SendResponse(ClientSocket, ticketInserted)
                                                End Select
#End Region
                                            End If
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, ticketRefFound)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, ticketRecord)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, headerId)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Palletizing"
            Case "*GETPALLETITMDESC*"
                Try
                    Dim unq As String = ClientData.Split("|")(0)
                    Dim itemCode As String = Barcodes.GetItemCode(unq)
                    Dim unqOnPallet As String = Unique.RTSQL.Retreive.UI_GetItemOnPallet(unq)
                    Select Case unqOnPallet.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Palletizing.Evolution.Retreive.MBL_GetItemDesc(itemCode))
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, unqOnPallet)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*SAVEPALLETITEMS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim palletUnqs As String() = ClientData.Split("|")(1).Split("~")
                    Dim lotNumber As String = "#PALLET#"
                    Dim exp As String = String.Empty
                    Dim qty As String = "1"
                    Dim label As String = String.Empty
                    Dim printer As String = String.Empty
                    Dim lotNumbers As String = String.Empty

                    Dim printSettings As String = Palletizing.RTIS.Retreive.MBL_GetPalletPrintSettings()
                    Select Case printSettings.Split("*")(0)
                        Case "1"
                            Dim allSettings As String() = printSettings.Split("~")
                            For Each setting As String In allSettings
                                If setting <> String.Empty Then
                                    Dim settingInfo As String() = setting.Split("|")
                                    If settingInfo(0) = "Pallet Label" Then
                                        label = settingInfo(1)
                                    End If

                                    If settingInfo(0) = "Pallet Printer" Then
                                        printer = settingInfo(1)
                                    End If
                                End If
                            Next

                            Dim unqBar As String = "P" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                            Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + exp.PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unqBar
                            Dim palletSaved As String = Unique.RTSQL.Insert.UI_SaveRMBarcodePallet(Barcode, "Palletizing", "")
                            Select Case palletSaved.Split("*")(0)
                                Case "1"
                                    Dim palletID As String = palletSaved.Remove(0, 2)
                                    Dim inserUnqPalletLines As String = "INSERT INTO [ltbl_PalletBarcodes] ([iPallet_ID], [vUnqBarcode], [bOnPallet]) VALUES"
                                    For Each unq As String In palletUnqs
                                        If unq <> String.Empty Then
                                            inserUnqPalletLines += "('" + palletID + "','" + unq + "',1),"
                                            lotNumbers += Barcodes.GetItemLot(unq) + ","
                                        End If
                                    Next
                                    inserUnqPalletLines = inserUnqPalletLines.Substring(0, inserUnqPalletLines.Length - 1)
                                    lotNumbers = lotNumbers.Substring(0, lotNumbers.Length - 1)
                                    Dim palletLinesInserted As String = Unique.RTSQL.Insert.UI_GenericInsert(inserUnqPalletLines)
                                    Select Case palletLinesInserted.Split("*")(0)
                                        Case "1"
                                            Dim itemInfo As String = Palletizing.Evolution.Retreive.MBL_GetItemInfo(itemCode)

                                            Select Case itemInfo.Split("*")(0)
                                                Case "1"
                                                    itemInfo = itemInfo.Remove(0, 2)
                                                    Dim allItemInfo As String() = itemInfo.Split("|")
                                                    Dim rep As New XtraReport()
                                                    rep.LoadLayout(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\" + label)

                                                    For Each item As DevExpress.XtraReports.Parameters.Parameter In rep.Parameters
                                                        If item.Name = "_barcode" Then
                                                            item.Value = allItemInfo(0)
                                                        End If

                                                        If item.Name = "_RT2D" Then
                                                            item.Value = Barcode
                                                        End If

                                                        If item.Name = "_Date" Then
                                                            item.Value = DateTime.Now.ToString()
                                                        End If

                                                        If item.Name = "_Qty" Then
                                                            item.Value = qty
                                                        End If

                                                        If item.Name = "_SimpleCode" Then
                                                            item.Value = allItemInfo(1)
                                                        End If

                                                        If item.Name = "_LotNumbers" Then
                                                            item.Value = lotNumbers
                                                        End If

                                                        If item.Name = "_ItemCode" Then
                                                            item.Value = itemCode
                                                        End If

                                                        If item.Name = "_Desc1" Then
                                                            item.Value = allItemInfo(2)
                                                        End If

                                                        If item.Name = "_Desc2" Then
                                                            item.Value = allItemInfo(3)
                                                        End If

                                                        If item.Name = "_Desc3" Then
                                                            item.Value = allItemInfo(4)
                                                        End If
                                                    Next

                                                    rep.CreateDocument()
                                                    Dim rpt As ReportPrintTool = New ReportPrintTool(rep)
                                                    rpt.PrinterSettings.Copies = Convert.ToInt16(1)
                                                    rpt.Print(printer)
                                                    Server.Listener.SendResponse(ClientSocket, "1*SUCCESS")
                                                Case Else
                                                    Server.Listener.SendResponse(ClientSocket, itemInfo)
                                            End Select
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, palletLinesInserted)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, palletSaved)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, printSettings)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETPALLETINFO*"
                Try
                    Dim palletNo As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Retreive.MBL_GetPalletContents(palletNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REMOVEITEMFROMPALLET*"
                Try
                    Dim palletCode As String = ClientData.Split("|")(0)
                    Dim remCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Unique.RTSQL.Update.MBL_UpdateRMPalletRemoved(palletCode, remCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*PRINTNEWPALLETLABEL*"
                Try
                    Dim palletCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = Barcodes.GetItemCode(palletCode)
                    Dim allLots As String = String.Empty
                    Dim label As String = String.Empty
                    Dim printer As String = String.Empty
                    Dim allUnqs As String = Unique.RTSQL.Retreive.MBL_GetPalletLots(palletCode)
                    Select Case allUnqs.Split("*")(0)
                        Case "1"
                            allUnqs = allUnqs.Remove(0, 2)
                            Dim Unqs As String() = allUnqs.Split("~")
                            For Each unq As String In Unqs
                                If unq <> String.Empty Then
                                    allLots += unq + ","
                                End If
                            Next
                            allLots = allLots.Substring(0, allLots.Length - 1)

                            Dim printSettings As String = Palletizing.RTIS.Retreive.MBL_GetPalletPrintSettings()
                            Select Case printSettings.Split("*")(0)
                                Case "1"
                                    Dim allSettings As String() = printSettings.Split("~")
                                    For Each setting As String In allSettings
                                        If setting <> String.Empty Then
                                            Dim settingInfo As String() = setting.Split("|")
                                            If settingInfo(0) = "Pallet Label" Then
                                                label = settingInfo(1)
                                            End If

                                            If settingInfo(0) = "Pallet Printer" Then
                                                printer = settingInfo(1)
                                            End If
                                        End If
                                    Next

                                    Dim itemInfo As String = Palletizing.Evolution.Retreive.MBL_GetItemInfo(itemCode)
                                    Select Case itemInfo.Split("*")(0)
                                        Case "1"
                                            itemInfo = itemInfo.Remove(0, 2)
                                            Dim allItemInfo As String() = itemInfo.Split("|")
                                            Dim rep As New XtraReport()
                                            rep.LoadLayout(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\" + label)

                                            For Each item As DevExpress.XtraReports.Parameters.Parameter In rep.Parameters
                                                If item.Name = "_barcode" Then
                                                    item.Value = allItemInfo(0)
                                                End If

                                                If item.Name = "_RT2D" Then
                                                    item.Value = palletCode
                                                End If

                                                If item.Name = "_Date" Then
                                                    item.Value = DateTime.Now.ToString()
                                                End If

                                                If item.Name = "_Qty" Then
                                                    item.Value = "1"
                                                End If

                                                If item.Name = "_SimpleCode" Then
                                                    item.Value = allItemInfo(1)
                                                End If

                                                If item.Name = "_LotNumbers" Then
                                                    item.Value = allLots
                                                End If

                                                If item.Name = "_ItemCode" Then
                                                    item.Value = itemCode
                                                End If

                                                If item.Name = "_Desc1" Then
                                                    item.Value = allItemInfo(2)
                                                End If

                                                If item.Name = "_Desc2" Then
                                                    item.Value = allItemInfo(3)
                                                End If

                                                If item.Name = "_Desc3" Then
                                                    item.Value = allItemInfo(4)
                                                End If
                                            Next

                                            rep.CreateDocument()
                                            Dim rpt As ReportPrintTool = New ReportPrintTool(rep)
                                            rpt.PrinterSettings.Copies = Convert.ToInt16(1)
                                            rpt.Print(printer)
                                            Server.Listener.SendResponse(ClientSocket, "1*SUCCESS")
                                        Case Else
                                            Server.Listener.SendResponse(ClientSocket, itemInfo)
                                    End Select
                                Case Else
                                    Server.Listener.SendResponse(ClientSocket, printSettings)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, allUnqs)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Unused"
            Case "*CHECKSLURRYINUSE*"
                Try
                    Dim slurryTank As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim slurryInUse As String = FreshSlurry.RTSQL.Retreive.MBL_GetFreshSlurryInUse(slurryTank)
                    Select Case slurryInUse.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, WhseTransfers.Evolution.Retreive.GetItemDesc(itemCode))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, slurryInUse)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, slurryInUse)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region
#End Region

#Region "ZECT"

            Case "*ZECTLOGIN*"
                Try
                    Dim userPin As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.ZECT_GetUsersname(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#Region "Open Job"
            'Case "*GETMFITEMCODEZECT*"
            '    Try
            '        Dim mfCode As String = ClientData.Split("|")(0)
            '        Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetItemCodeFromMFCode(mfCode))
            '    Catch ex As Exception
            '        Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
            '    End Try
            Case "*ZECTGETCOATSLURRIES*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim coatNum As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetCatalystSlurries(itemCode, coatNum))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETSLURRYLOTSONHANDFROMTANK*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetItemLotsFromTanks(itemCode, whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETSLURRYLOTTANK*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetSlurryTanks(itemCode, lotNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*OPENZECTJOBCARDWITHSLURRYTANK*"
                Try
                    Dim checkSheet As String = ClientData.Split("|")(0)
                    Dim coatNum As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim slurry As String = ClientData.Split("|")(3)
                    Dim slurryLot As String = ClientData.Split("|")(4)
                    Dim qty As String = ClientData.Split("|")(5)
                    Dim whseTo As String = ClientData.Split("|")(6)
                    Dim whseFrom As String = ClientData.Split("|")(7)
                    Dim username As String = ClientData.Split("|")(8)

                    Dim slurryTankType As String = ClientData.Split("|")(9)
                    Dim slurryTank As String = ClientData.Split("|")(10)
                    Dim slurryWet As String = ClientData.Split("|")(11)
                    Dim slurryDry As String = ClientData.Split("|")(12)

                    Dim checkJobRunning As String = Zect.RTSQL.Retreive.Zect_CheckJobOnLine(whseTo)
                    Select Case checkJobRunning.Split("*")(0)
                        Case "1"
                            Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(checkSheet, coatNum)
                            Select Case itemCode.Split("*")(0)
                                Case "1"
                                    itemCode = itemCode.Remove(0, 2)
                                    Dim unqCode As String = "ZECT_" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                                    Dim jobStarted As String = Zect.RTSQL.Insert.Zect_OpenJobOnLine(unqCode, itemCode, lotNumber, slurry, slurryLot, qty, coatNum, whseTo, username, checkSheet)
                                    Select Case jobStarted.Split("*")(0)
                                        Case "1"
                                            Dim headerID As String = Zect.RTSQL.Retreive.Zect_GetJobID(unqCode)
                                            Select Case headerID.Split("*")(0)
                                                Case "1"
                                                    headerID = headerID.Remove(0, 2)
                                                    Dim rmLogged As String = Zect.RTSQL.Insert.Zect_LogRM(headerID, slurry, slurryLot, slurryDry, username)
                                                    Select Case rmLogged.Split("*")(0)
                                                        Case "1"
                                                            'Do Transfer
                                                            Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(slurry, slurryLot, whseFrom, whseTo, slurryDry, username, "Zect", "Zect Receiving", "Pending")
                                                            Select Case transferSaved.Split("*")(0)
                                                                Case "1"
                                                                    Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(slurry, slurryLot, whseFrom, whseTo, slurryDry.Replace(",", "."), username, "Zect")
                                                                    Select Case transferLogged.Split("*")(0)
                                                                        Case "1"
                                                                            Select Case slurryTankType
                                                                                Case "TNK"
                                                                                    Dim tankReceived As String = Transfers.RTSQL.Update.MBL_UpdateLargeTankReceived(slurryTankType, slurryTank, slurry, slurryLot, username)
                                                                                    Select Case tankReceived.Split("*")(0)
                                                                                        Case "1"
                                                                                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetLabelInfo(itemCode, unqCode))
                                                                                        Case "-1"
                                                                                            Server.Listener.SendResponse(ClientSocket, tankReceived)
                                                                                    End Select
                                                                                Case "MTNK"
                                                                                    Dim tankReceived As String = Transfers.RTSQL.Update.MBL_UpdateMobileTankReceived(slurryTank, slurry, slurryLot, username)
                                                                                    Select Case tankReceived.Split("*")(0)
                                                                                        Case "1"
                                                                                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetLabelInfo(itemCode, unqCode))
                                                                                        Case "-1"
                                                                                            Server.Listener.SendResponse(ClientSocket, tankReceived)
                                                                                    End Select
                                                                            End Select
                                                                        Case "-1"
                                                                            Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                            End Select

                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, rmLogged)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, headerID)
                                            End Select

                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, jobStarted)
                                    End Select

                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, itemCode)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, checkJobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, checkJobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Reprint Job Tags"
            Case "*ZECTREPRINTGETCOATS*"
                Try
                    Dim itemCode As String = ClientData + "%"
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetRePrintCatalystCoats(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETREPRINTJOBLOTS*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim lotPeriod As String = ClientData.Split("|")(2)
                    Dim whse As String = ClientData.Split("|")(3)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetValidReprintJobLots(itemCode, lotPeriod, whse))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTREPRINTGETJOBNO*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim whse As String = ClientData.Split("|")(2)
                    Dim lot As String = ClientData.Split("|")(3)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetReprintJobNumber(itemCode, lot, coat, whse))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETREPRINTJOBINFO*"
                Try
                    Dim jobNo As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetReprintJobInfo(jobNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETREREPRINTJOBLABElINFO*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetReprintLabelInfo(itemCode))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Add Slurry"
            Case "*ZECTGETJOBSLURRY*"
                Try
                    Dim jobNo As String = ClientData
                    Dim jobInfo As String = Zect.RTSQL.Retreive.Zect_GetJobOpenInfo(jobNo)
                    Select Case jobInfo.Split("*")(0)
                        Case "1"
                            jobInfo = jobInfo.Remove(0, 2)
                            Dim itemCode As String = jobInfo.Split("|")(0)
                            Dim lotNumber As String = jobInfo.Split("|")(1)
                            Dim slurryCode As String = jobInfo.Split("|")(2)
                            Dim coat As String = jobInfo.Split("|")(3)
                            Dim jobOpen As String = jobInfo.Split("|")(4)
                            If Convert.ToBoolean(jobOpen) = True Then
                                Server.Listener.SendResponse(ClientSocket, "1*" + itemCode + "|" + lotNumber + "|" + slurryCode + "|" + coat)
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobInfo)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobInfo)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETADDSLURRYLOTSFROMTANK*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetItemLotsFromTank_AddSlurry(itemCode, whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTSLURRYADDITIONFROMTANK*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim tankNumber As String = ClientData.Split("|")(1)
                    Dim slurry As String = ClientData.Split("|")(2)
                    Dim slurryLot As String = ClientData.Split("|")(3)
                    Dim qty As String = ClientData.Split("|")(4)
                    Dim whseTo As String = ClientData.Split("|")(5)
                    Dim whseFrom As String = ClientData.Split("|")(6)
                    Dim username As String = ClientData.Split("|")(7)
                    Dim tankType As String = ClientData.Split("|")(8)

                    Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetJobOpen(jobNo)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) Then
                                Dim headerID As String = Zect.RTSQL.Retreive.Zect_GetJobID(jobNo)
                                Select Case headerID.Split("*")(0)
                                    Case "1"
                                        headerID = headerID.Remove(0, 2)
                                        Dim rmLogged As String = Zect.RTSQL.Insert.Zect_LogRM(headerID, slurry, slurryLot, qty, username)
                                        Select Case rmLogged.Split("*")(0)
                                            Case "1"
                                                'Do Transfer
                                                Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(slurry, slurryLot, whseFrom, whseTo, qty, username, "Zect", "Zect Receiving", "Pending")
                                                Select Case transferSaved.Split("*")(0)
                                                    Case "1"
                                                        Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(slurry, slurryLot, whseFrom, whseTo, qty.Replace(",", "."), username, "Zect")
                                                        Select Case transferLogged.Split("*")(0)
                                                            Case "1"
                                                                Select Case tankType
                                                                    Case "TNK"
                                                                        Dim tankReceived As String = Transfers.RTSQL.Update.MBL_UpdateLargeTankReceived(tankType, tankNumber, slurry, slurryLot, username)
                                                                        Select Case tankReceived.Split("*")(0)
                                                                            Case "1"
                                                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, tankReceived)
                                                                        End Select
                                                                    Case "MTNK"
                                                                        Dim tankReceived As String = Transfers.RTSQL.Update.MBL_UpdateMobileTankReceived(tankNumber, slurry, slurryLot, username)
                                                                        Select Case tankReceived.Split("*")(0)
                                                                            Case "1"
                                                                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                                                                            Case "-1"
                                                                                Server.Listener.SendResponse(ClientSocket, tankReceived)
                                                                        End Select
                                                                End Select
                                                            Case "-1"
                                                                Server.Listener.SendResponse(ClientSocket, transferLogged)
                                                        End Select
                                                    Case "-1"
                                                        Server.Listener.SendResponse(ClientSocket, transferSaved)
                                                End Select

                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, rmLogged)
                                        End Select
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, headerID)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "Print Tags"
            Case "*ZECTGETJOBINFO*"
                Try
                    Dim jobNo As String = ClientData
                    Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetJobOpen(jobNo)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) Then
                                Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetJobInformation(jobNo))
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTLOGPALLETPRINTED*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim Code As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim whse As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)

                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetMFCode(Code)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetJobOpen(jobNo)
                            Select Case jobRunning.Split("*")(0)
                                Case "1"
                                    jobRunning = jobRunning.Remove(0, 2)
                                    If Convert.ToBoolean(jobRunning) Then
                                        Dim headerID As String = Zect.RTSQL.Retreive.Zect_GetJobID(jobNo)
                                        Select Case headerID.Split("*")(0)
                                            Case "1"
                                                headerID = headerID.Remove(0, 2)

                                                Dim failed As Boolean = False
                                                Dim failureReason As String = String.Empty
                                                Dim palletNumber As String = String.Empty
                                                Dim palletCode As String = String.Empty

                                                Dim lastPalletCode As String = Zect.RTSQL.Retreive.Zect_GetLastJobPallet(headerID)
                                                Select Case lastPalletCode.Split("*")(0)
                                                    Case "1"
                                                        lastPalletCode = lastPalletCode.Remove(0, 2)
                                                        Dim lastPalletNumber As String = lastPalletCode.Split("_")(2)

                                                        palletNumber = Convert.ToString(Convert.ToInt32(lastPalletNumber) + 1).PadLeft(3, "0")
                                                        palletCode = jobNo + "_" + palletNumber
                                                    Case "0"
                                                        'generate new
                                                        palletNumber = "1".PadLeft(3, "0")
                                                        palletCode = jobNo + "_" + palletNumber
                                                    Case "-1"
                                                        failed = True
                                                        failureReason = lastPalletCode
                                                End Select

                                                If failed = False Then
                                                    Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + palletCode
                                                    Dim palletLogged As String = Zect.RTSQL.Insert.Zect_AddNewPallet(headerID, palletCode, palletNumber, qty, username)
                                                    Select Case palletLogged.Split("*")(0)
                                                        Case "1"
                                                            Dim unqInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, jobNo)
                                                            Select Case unqInserted.Split("*")(0)
                                                                Case "1"
                                                                    Dim headerUpdated As String = Zect.RTSQL.Update.ZECT_UpdateManufacturedQty(jobNo, qty)
                                                                    Select Case headerUpdated.Split("*")(0)
                                                                        Case "1"
                                                                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetLabelInfo_ZectTage(itemCode, palletCode, palletNumber, Barcode))
                                                                        Case "-1"
                                                                            Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                                    End Select
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, unqInserted)
                                                            End Select
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, palletLogged)
                                                    End Select
                                                Else
                                                    Server.Listener.SendResponse(ClientSocket, failureReason)
                                                End If
                                            Case "-1"
                                                Server.Listener.SendResponse(ClientSocket, headerID)
                                        End Select
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, jobRunning)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, jobRunning)
                            End Select
                        Case Else
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETREPRINTPALLETS*"
                Try
                    Dim jobNo As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetJobPallets(jobNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETTAGREPRINTINFO*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim palletCode As String = ClientData.Split("|")(1)
                    Dim palletNo As String = ClientData.Split("|")(2)
                    palletNo = palletCode + "_" + palletNo
                    Dim unq As String = Unique.RTSQL.Retreive.ZECT_GetZectUnq(itemCode, palletNo)
                    Select Case unq.Split("*")(0)
                        Case "1"
                            unq = unq.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetReprintInfo_ZectTag(itemCode, unq))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, unq)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

#End Region

#Region "Close Job"
            Case "*ZECTGETJOBRUNNING*"
                Try
                    Dim jobNo As String = ClientData
                    Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetJobOpen(jobNo)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) Then
                                Server.Listener.SendResponse(ClientSocket, "1*Success")
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETUSERPERMS*"
                Try
                    Dim userPin As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetUerPermissions(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETCLOSINGJOBINFO*"
                Try
                    Dim jobNo As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetClosingJobInfo(jobNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTCLOSEJOB*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.ZECT_UpdateJobClosed(jobNo, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Re-Open Job"
            Case "*ZECTREOPENCHECKJOBONLINE*"
                Try
                    Dim whseCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_CheckJobOnLine(whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTREOPENCHECkJOBSTATE*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Dim jobsRunning As String = Zect.RTSQL.Retreive.Zect_CheckJobOnLine(whseCode)
                    Select Case jobsRunning.Split("*")(0)
                        Case "1"
                            Dim jobRunning As String = Zect.RTSQL.Retreive.Zect_GetJobOpen(jobNo)
                            Select Case jobRunning.Split("*")(0)
                                Case "1"
                                    jobRunning = jobRunning.Remove(0, 2)
                                    If Convert.ToBoolean(jobRunning) = False Then
                                        Server.Listener.SendResponse(ClientSocket, "1*Success")
                                    Else
                                        Server.Listener.SendResponse(ClientSocket, "0*The job scanned is already running")
                                    End If
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, jobRunning)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, jobRunning)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobsRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobsRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETUSERPERMSREOPEN*"
                Try
                    Dim userPin As String = ClientData.Split("|")(0)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetUerPermissionsReOpen(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETREOPENJOBINFO*"
                Try
                    Dim jobNo As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetReOpenJobInfo(jobNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTREOPENJOB*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.ZECT_UpdateJobReOpened(jobNo, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETREOPENCATALYSTCOATS*"
                Try
                    Dim itemCode As String = ClientData + "%"
                    Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetReOpenCatalystCoats(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*ZECTGETVALIDJOBLOTS*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim lotPeriod As String = ClientData.Split("|")(2)
                    Dim whse As String = ClientData.Split("|")(3)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetValidJobLots(itemCode, lotPeriod, whse))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETREOPENJOBNUMBER*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim whse As String = ClientData.Split("|")(2)
                    Dim lot As String = ClientData.Split("|")(3)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetJobNumber(itemCode, lot, coat, whse))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETREOPENJOBLABElINFO*"
                Try
                    Dim catalyst As String = ClientData.Split("|")(0)
                    Dim coat As String = ClientData.Split("|")(1)
                    Dim itemCode As String = Zect.Evoltion.Retreive.Zect_GetItemCode(catalyst, coat)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetreOpenLabelInfo(itemCode))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Old"
            'Case "*GETCATALYSTCOATS*"
            '    Try
            '        Dim itemCode As String = ClientData + "%"
            '        Server.Listener.SendResponse(ClientSocket, Zect.Evoltion.Retreive.Zect_GetCatalystCoats(itemCode))
            '    Catch ex As Exception
            '        Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
            '    End Try
            Case "*GETCOATSLURRIES*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim coatNum As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Retreive.Zect_GetCatalystSlurries(itemCode, coatNum))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
'            Case "*PRINTANDMANUFACTUREZECT*"
'                Try
'                    Dim baseCode As String = ClientData.Split("|")(0)
'                    Dim itemCode As String = ClientData.Split("|")(1)
'                    Dim lotNum As String = ClientData.Split("|")(2)
'                    Dim coatNum As String = ClientData.Split("|")(3)
'                    Dim slurryCode As String = ClientData.Split("|")(4)
'                    Dim qty As String = ClientData.Split("|")(5)
'                    Dim zectLine As String = ClientData.Split("|")(6)
'                    Dim userName As String = ClientData.Split("|")(7)

'                    Dim itemDesc As String = Zect.Evoltion.Retreive.Zect_GetItemDescription(itemCode)
'                    Select Case itemDesc.Split("*")(0)
'                        Case "1"
'                            itemDesc = itemDesc.Remove(0, 2)
'                            Dim labelPrintInfo As String = Zect.RTSQL.Retreive.Zect_GetLabelInfo(itemCode)
'                            Select Case labelPrintInfo.Split("*")(0)
'                                Case "1"
'                                    labelPrintInfo = labelPrintInfo.Remove(0, 2)
'                                    Dim barcode As String = labelPrintInfo.Split("|")(0)
'                                    Dim simpleCode As String = labelPrintInfo.Split("|")(1)
'                                    Dim bin As String = labelPrintInfo.Split("|")(2)
'                                    Dim desc1 As String = labelPrintInfo.Split("|")(3)
'                                    Dim desc2 As String = labelPrintInfo.Split("|")(4)
'                                    Dim desc3 As String = labelPrintInfo.Split("|")(5)
'                                    Dim group As String = labelPrintInfo.Split("|")(6)

'                                    Dim insreted As String = Zect.RTSQL.Insert.Zect_ManufacturePalletUnprinted(baseCode, itemCode, itemDesc, lotNum, coatNum, slurryCode, qty, zectLine, userName)
'                                    Select Case insreted.Split("*")(0)
'                                        Case "1"
'                                            Dim lineId As String = Zect.RTSQL.Retreive.Zect_GetInsertedLineID(baseCode, itemCode, lotNum, slurryCode, coatNum, zectLine)
'                                            Select Case lineId.Split("*")(0)
'                                                Case "1"
'                                                    lineId = lineId.Remove(0, 2)
'                                                    Dim unqBar As String = Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
'                                                    Dim RT2d As String = "(91)" + lineId + "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNum.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + unqBar

'                                                    Dim UnqInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(RT2d, "Zect :" + zectLine)
'                                                    Select Case UnqInserted.Split("*")(0)
'                                                        Case "1"
'                                                            Dim prntRep As XtraReport
'                                                            For Each report As XtraReport In Labels.zectList
'                                                                If report.Name.Contains(zectLine) Then
'                                                                    prntRep = report
'                                                                End If
'                                                            Next

'                                                            For Each item As DevExpress.XtraReports.Parameters.Parameter In prntRep.Parameters
'#Region "Label Parameters"
'                                                                If item.Name = "_barcode" Then
'                                                                    item.Value = barcode
'                                                                End If
'                                                                If item.Name = "_bin" Then
'                                                                    item.Value = bin
'                                                                End If
'                                                                If item.Name = "_Date" Then
'                                                                    item.Value = DateTime.Now
'                                                                End If
'                                                                If item.Name = "_Description1" Then
'                                                                    item.Value = desc1
'                                                                End If
'                                                                If item.Name = "_Description2" Then
'                                                                    item.Value = desc2
'                                                                End If
'                                                                If item.Name = "_Description3" Then
'                                                                    item.Value = desc3
'                                                                End If
'                                                                If item.Name = "_Group" Then
'                                                                    item.Value = group
'                                                                End If
'                                                                If item.Name = "_ItemCode" Then
'                                                                    item.Value = itemCode
'                                                                End If
'                                                                If item.Name = "_Lot" Then
'                                                                    item.Value = lotNum
'                                                                End If
'                                                                If item.Name = "_Qty" Then
'                                                                    item.Value = qty
'                                                                End If
'                                                                If item.Name = "_RT2D" Then
'                                                                    item.Value = RT2d
'                                                                End If
'                                                                If item.Name = "_Serial" Then
'                                                                    item.Value = "No serial"
'                                                                End If
'                                                                If item.Name = "_SimpleCode" Then
'                                                                    item.Value = simpleCode
'                                                                End If
'                                                                If item.Name = "_ZectLine" Then
'                                                                    item.Value = zectLine
'                                                                End If
'                                                                If item.Name = "_CatalystCode" Then
'                                                                    item.Value = baseCode
'                                                                End If
'                                                                If item.Name = "_Coat" Then
'                                                                    item.Value = coatNum
'                                                                End If
'                                                                If item.Name = "_Slurry" Then
'                                                                    item.Value = slurryCode
'                                                                End If
'#End Region
'                                                            Next
'                                                            prntRep.CreateDocument()
'                                                            Dim rpt As ReportPrintTool = New ReportPrintTool(prntRep)
'                                                            rpt.PrinterSettings.Copies = Convert.ToInt16(1)
'                                                            rpt.Print(My.Settings.Zect1Printer)
'                                                            Server.Listener.SendResponse(ClientSocket, Zect.RTSQL.Update.Zect_UpdateZectLine_Printed(lineId, userName))
'                                                        Case "-1"
'                                                            Server.Listener.SendResponse(ClientSocket, UnqInserted)
'                                                    End Select
'                                                Case "-1"
'                                                    Server.Listener.SendResponse(ClientSocket, lineId)
'                                            End Select
'                                        Case "-1"
'                                            Server.Listener.SendResponse(ClientSocket, insreted)
'                                    End Select
'                                Case "-1"
'                                    Server.Listener.SendResponse(ClientSocket, labelPrintInfo)
'                            End Select
'                        Case "-1"
'                            Server.Listener.SendResponse(ClientSocket, itemDesc)
'                    End Select

'                Catch ex As Exception
'                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
'                End Try
#End Region

#End Region

#Region "A&W"
            Case "*AWLOGIN*"
                Try
                    Dim userPin As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.AW_GetUsersname(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#Region "Open Job"
            Case "*GETAWJOBRUNNING*"
                Try
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_CheckJobRunning())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWITEMCODE*"
                Try
                    Dim itemCode As String = ClientData
                    Dim mfZectCode As String = AW.Evolution.Retrieve.AW_GetZectMFCode(itemCode)
                    Select Case mfZectCode.Split("*")(0)
                        Case "1"
                            mfZectCode = mfZectCode.Remove(0, 2)
                            Dim baseCode As String = mfZectCode.Replace("-1st COAT", String.Empty).Replace("-2nd COAT", String.Empty).Replace("-3rd COAT", String.Empty).Replace("-4rd COAT", String.Empty).Replace(" - 1st COAT", String.Empty).Replace(" - 2nd COAT", String.Empty).Replace(" - 3rd COAT", String.Empty).Replace(" - 4rd COAT", String.Empty).Replace(" ", String.Empty)
                            Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.AW_GetAWItemCode(baseCode))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, mfZectCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWITEMPGMS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim whseCode As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.AW_GetAWAvailablePGMs(itemCode, whseCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*OPENAWJOB*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim PGM As String = ClientData.Split("|")(2)
                    Dim PGMLot As String = ClientData.Split("|")(3)
                    Dim jobQty As String = ClientData.Split("|")(4)
                    Dim username As String = ClientData.Split("|")(5)
                    Dim unqCode As String = "A&W_" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                    Dim jobStarted As String = AW.RTSQL.Insert.UI_InsertNewAWJob(unqCode, itemCode, lotNumber, PGM, PGMLot, jobQty, username)
                    Select Case jobStarted.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.AW_GetLabelInfo(itemCode, unqCode))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobStarted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*OPENAWJOBFROMIT*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim PGM As String = ClientData.Split("|")(2)
                    Dim PGMLot As String = ClientData.Split("|")(3)
                    Dim PGMQty As String = ClientData.Split("|")(4)
                    Dim jobQty As String = ClientData.Split("|")(5)
                    Dim username As String = ClientData.Split("|")(6)
                    Dim whseWIP As String = ClientData.Split("|")(7)
                    Dim whseIT As String = ClientData.Split("|")(8)
                    Dim unqCode As String = "A&W_" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                    Dim jobStarted As String = AW.RTSQL.Insert.UI_InsertNewAWJob(unqCode, itemCode, lotNumber, PGM, PGMLot, jobQty, username)
                    Select Case jobStarted.Split("*")(0)
                        Case "1"
                            Dim transferSaved As String = Transfers.RTSQL.Insert.UI_InsertWhseTransfer(PGM, PGMLot, whseIT, whseWIP, PGMQty.Replace(",", "."), username, "A&W", "A&W Open Job", "Pending")
                            Select Case transferSaved.Split("*")(0)
                                Case "1"
                                    Dim transferLogged As String = Transfers.RTSQL.Insert.UI_whtTransferLog(PGM, PGMLot, whseIT, whseWIP, PGMQty.Replace(",", "."), username, "A&W")
                                    Select Case transferLogged.Split("*")(0)
                                        Case "1"
                                            Server.Listener.SendResponse(ClientSocket, AW.Evolution.Retrieve.AW_GetLabelInfo(itemCode, unqCode))
                                        Case "-1"
                                            Server.Listener.SendResponse(ClientSocket, transferLogged)
                                    End Select
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, transferSaved)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobStarted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Reprint Job Tags"
            Case "*GETAWJOBLOTS*"
                Try
                    Dim baseCode As String = ClientData.Split("|")(0)
                    Dim lotPeriod As String = ClientData.Split("|")(1)
                    Dim itemCode As String = AW.Evolution.Retrieve.AW_GetAWItemCode(baseCode)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetValidReprintJobLots(itemCode, lotPeriod))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*AWGETREPRINTJOBINFO*"
                Try
                    Dim baseCode As String = ClientData.Split("|")(0)
                    Dim lot As String = ClientData.Split("|")(1)

                    Dim itemCode As String = AW.Evolution.Retrieve.AW_GetAWItemCode(baseCode)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Dim jobNo As String = AW.RTSQL.Retrieve.AW_GetReprintJobNumber(itemCode, lot)
                            Select Case jobNo.Split("*")(0)
                                Case "1"
                                    jobNo = jobNo.Remove(0, 2)
                                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetReprintJobInfo(jobNo))
                                Case "0"
                                    Server.Listener.SendResponse(ClientSocket, jobNo)
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, jobNo)
                            End Select
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETREREPRINTAWJOBLABElINFO*"
                Try
                    Dim baseCode As String = ClientData.Split("|")(0)
                    Dim itemCode As String = AW.Evolution.Retrieve.AW_GetAWItemCode(baseCode)
                    Select Case itemCode.Split("*")(0)
                        Case "1"
                            itemCode = itemCode.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetReprintLabelInfo(itemCode))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, itemCode)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Print Product Tags"
            Case "*GETAWJOBINFO*"
                Try
                    Dim jobNumber As String = ClientData
                    Dim jobRunning As String = AW.RTSQL.Retrieve.AW_CheckSpecificJobRunning(jobNumber)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) = True Then
                                Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetJobInfo(jobNumber))
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*AWLOGPALLETPRINTED*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim itemCode As String = ClientData.Split("|")(1)
                    Dim lotNumber As String = ClientData.Split("|")(2)
                    Dim qty As String = ClientData.Split("|")(3)
                    Dim username As String = ClientData.Split("|")(4)

                    Dim jobRunning As String = AW.RTSQL.Retrieve.AW_CheckSpecificJobRunning(jobNo)
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            If Convert.ToBoolean(jobRunning) Then
                                Dim headerID As String = AW.RTSQL.Retrieve.AW_GetJobID(jobNo)
                                Select Case headerID.Split("*")(0)
                                    Case "1"
                                        headerID = headerID.Remove(0, 2)

                                        Dim failed As Boolean = False
                                        Dim failureReason As String = String.Empty
                                        Dim palletNumber As String = String.Empty
                                        Dim palletCode As String = String.Empty

                                        Dim lastPalletCode As String = AW.RTSQL.Retrieve.AW_GetLastJobPallet(headerID)
                                        Select Case lastPalletCode.Split("*")(0)
                                            Case "1"
                                                lastPalletCode = lastPalletCode.Remove(0, 2)
                                                Dim lastPalletNumber As String = lastPalletCode.Split("_")(2)

                                                palletNumber = Convert.ToString(Convert.ToInt32(lastPalletNumber) + 1).PadLeft(3, "0")
                                                palletCode = jobNo + "_" + palletNumber
                                            Case "0"
                                                'generate new
                                                palletNumber = "1".PadLeft(3, "0")
                                                palletCode = jobNo + "_" + palletNumber
                                            Case "-1"
                                                failed = True
                                                failureReason = lastPalletCode
                                        End Select

                                        If failed = False Then
                                            Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qty.PadLeft(8, "0") + "(90)" + palletCode
                                            Dim palletLogged As String = AW.RTSQL.Insert.AW_AddNewPallet(headerID, palletCode, palletNumber, qty, username)
                                            Select Case palletLogged.Split("*")(0)
                                                Case "1"
                                                    Dim unqInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, jobNo)
                                                    Select Case unqInserted.Split("*")(0)
                                                        Case "1"
                                                            Dim headerUpdated As String = AW.RTSQL.Update.AW_UpdateManufacturedQty(jobNo, qty)
                                                            Select Case headerUpdated.Split("*")(0)
                                                                Case "1"
                                                                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetLabelInfo_AWTags(itemCode, palletCode, palletNumber, Barcode))
                                                                Case "-1"
                                                                    Server.Listener.SendResponse(ClientSocket, headerUpdated)
                                                            End Select
                                                        Case "-1"
                                                            Server.Listener.SendResponse(ClientSocket, unqInserted)
                                                    End Select
                                                Case "-1"
                                                    Server.Listener.SendResponse(ClientSocket, palletLogged)
                                            End Select
                                        Else
                                            Server.Listener.SendResponse(ClientSocket, failureReason)
                                        End If
                                    Case "-1"
                                        Server.Listener.SendResponse(ClientSocket, headerID)
                                End Select
                            Else
                                Server.Listener.SendResponse(ClientSocket, "0*The job scanned is currently not running")
                            End If
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*AWGETREPRINTPALLETS*"
                Try
                    Dim jobNo As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetJobPallets(jobNo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*AWGETTAGREPRINTINFO*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim palletCode As String = ClientData.Split("|")(1)
                    Dim unq As String = AW.RTSQL.Retrieve.AW_GetAWUnq(itemCode, palletCode)
                    Select Case unq.Split("*")(0)
                        Case "1"
                            unq = unq.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetReprintInfo_AWTag(itemCode, unq))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, unq)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "Close Job"
            Case "*GETAWJOBINFOCJ*"
                Try
                    Dim jobNumber As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetJobInfo_CJ(jobNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*AWCLOSEJOB*"
                Try
                    Dim jobNo As String = ClientData.Split("|")(0)
                    Dim username As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.AW_UpdateJobClosed(jobNo, username))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#Region "ReOpen Job"
            Case "*GETAWREOPENJOBLOTS*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotPeriod As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetValidReopenJobLots(itemCode, lotPeriod))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETMANUALREOPENINFO*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim jobNumber As String = AW.RTSQL.Retrieve.AW_GetReprintJobNumber_RO(itemCode, lotNumber)
                    Select Case jobNumber.Split("*")(0)
                        Case "1"
                            jobNumber = jobNumber.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetJobInfo_RO(jobNumber) + "|" + jobNumber)
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobNumber)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobNumber)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETAWJOBINFORO*"
                Try
                    Dim jobNumber As String = ClientData
                    Dim jobRunning As String = AW.RTSQL.Retrieve.AW_CheckJobRunning_RO()
                    Select Case jobRunning.Split("*")(0)
                        Case "1"
                            jobRunning = jobRunning.Remove(0, 2)
                            Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Retrieve.AW_GetJobInfo_RO(jobNumber))
                        Case "0"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, jobRunning)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REOPENAWJOB*"
                Try
                    Dim jobNumber As String = ClientData.Split("|")(0)
                    Dim userName As String = ClientData.Split("|")(1)
                    Server.Listener.SendResponse(ClientSocket, AW.RTSQL.Update.AW_UpdateJobReOpened(jobNumber, userName))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

#End Region

#Region "Canning"
            Case "*CANNINGLOGIN*"
                Try
                    Dim userPin As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, User_Management.RTSQL.Retreive.Canning_GetUsersname(userPin))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCANNINGPRODUCTS*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.UI_GetCanningProducts(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCANNINGRAWDESC*"
                Try
                    Dim itemCode As String = ClientData
                    Server.Listener.SendResponse(ClientSocket, Canning.Evolution.Retrieve.MBL_GetItemDesc(itemCode))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*RECORDCANNINGOUT*"
                Try
                    Dim rmCode As String = ClientData.Split("|")(0)
                    Dim rmDesc As String = ClientData.Split("|")(1)
                    Dim rmQty As String = ClientData.Split("|")(2)
                    Dim rmUnq As String = ClientData.Split("|")(3)
                    Dim itemCode As String = ClientData.Split("|")(4)
                    Dim itemDesc As String = ClientData.Split("|")(5)
                    Dim lotNumber As String = ClientData.Split("|")(6)
                    Dim qtyOut As String = ClientData.Split("|")(7)
                    Dim userName As String = ClientData.Split("|")(8)

                    Dim jobCode As String = rmUnq.Split("_")(0) + "_" + rmUnq.Split("_")(1)
                    Dim palletNo As String = rmUnq.Split("_")(2)

                    Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qtyOut.PadLeft(8, "0") + "(90)" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                    Dim barcodeInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, "Canning")
                    Select Case barcodeInserted.Split("*")(0)
                        Case "1"
                            Dim inserted As String = Canning.RTSQL.Insert.UI_InsertCanningRecordk(jobCode, rmUnq, rmCode, rmDesc, rmQty, lotNumber, itemCode, itemDesc, qtyOut, userName, palletNo)
                            Select Case inserted.Split("*")(0)
                                Case "1"
                                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.Canning_GetLabelInfo(itemCode, Barcode))
                                Case "-1"
                                    Server.Listener.SendResponse(ClientSocket, inserted)
                            End Select
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, barcodeInserted)
                    End Select

                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try

            Case "*GETCANNINGREPRINTLIST*"
                Try
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.Canning_GetReprintItemList())
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCANNINGREPRINTLOTLIST*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim dateFrom As String = ClientData.Split("|")(1)
                    Dim dateTo As String = ClientData.Split("|")(2)
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.Canning_GetReprintLotList(itemCode, dateFrom, dateTo))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*GETCANNINGREPRINTPALLETLIST*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)                    '
                    Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.Canning_GetPalletList(itemCode, lotNumber))
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
            Case "*REPRINTCANNINGLABEL*"
                Try
                    Dim itemCode As String = ClientData.Split("|")(0)
                    Dim lotNumber As String = ClientData.Split("|")(1)
                    Dim qtyOut As String = ClientData.Split("|")(2)
                    Dim Barcode = "(240)" + itemCode.PadRight(25, " ") + "(15)" + "".PadRight(6, " ") + "(10)" + lotNumber.PadRight(20, " ") + "(30)" + qtyOut.PadLeft(8, "0") + "(90)" + Convert.ToString(DateTime.Now.ToString("dd-MM-yy hh:mm:ss")).Replace(":", "").Replace(" ", "").Replace("-", "").Replace("/", "")
                    Dim barcodeInserted As String = Unique.RTSQL.Insert.UI_SaveRT2DBarcode(Barcode, "Canning")
                    Select Case barcodeInserted.Split("*")(0)
                        Case "1"
                            Server.Listener.SendResponse(ClientSocket, Canning.RTSQL.Retrieve.Canning_GetLabelInfo(itemCode, Barcode))
                        Case "-1"
                            Server.Listener.SendResponse(ClientSocket, barcodeInserted)
                    End Select
                Catch ex As Exception
                    Server.Listener.SendResponse(ClientSocket, ExHandler.returnErrorEx(ex))
                End Try
#End Region

            Case Else
                Server.Listener.SendResponse(ClientSocket, "0*No correct response for request: " + Environment.NewLine + Environment.NewLine + ClientRequest)
        End Select
    End Sub
End Class
