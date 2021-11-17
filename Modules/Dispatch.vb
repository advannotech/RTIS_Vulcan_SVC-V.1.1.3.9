Imports System.Data.SqlClient
Imports System.Threading
Imports Pastel.Evolution

Public Class Dispatch
    Public Shared RTString As String = "Data Source=" + My.Settings.RTServer + "; Initial Catalog=" + My.Settings.RTDB +
         "; user ID=" + My.Settings.RTUser + "; password=" + My.Settings.RTPassword + ";Max Pool Size=99999;"
    Public Shared EvoString As String = "Data Source=" + My.Settings.EvoServer + "; Initial Catalog=" + My.Settings.EvoDB +
      "; user ID=" + My.Settings.EvoUser + "; password=" + My.Settings.EvoPassword + ";Max Pool Size=99999;"
    Public Shared sep As String = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator

    Public Class RTSQL
        Partial Public Class Retreive
            Public Shared Function UI_GetSOUnqBarcodes() As String
                Try
                    Dim ReturnData As String = ""
                    Dim sqlConn As New SqlConnection(RTString)
                    'Dim sqlComm As New SqlCommand(" SELECT DISTINCT [Dispatch] FROM [tbl_unqBarcodes] WHERE [Dispatch] IS NOT NULL AND [Dispatch] <> ''", sqlConn)
                    Dim sqlComm As New SqlCommand("EXEC [dbo].[sp_UI_GetSOUnqBarcodes]", sqlConn)
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
                        Return "0*Barcode Not found"
                    End If
                Catch ex As Exception
                    If ex.Message = "Invalid attempt to read when no data is present." Then
                        Return "0*Barcode Not found"
                    Else
                        EventLog.WriteEntry("RTIS Vulcan SVC", "PGM_GetBarcodeFromVault: " + ex.ToString())
                        Return "-1*" + ex.Message
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

        Public Shared Function MBL_GetSOLines(ByVal PONum As String)
            Try
                Dim init As String = Initialize()
                Select Case init.Split("*")(0)
                    Case "1"
                        Dim OrderID As Integer = SalesOrder.Find("OrderNum = '" & PONum & "' AND (DocState=1 OR DocState=3)")
                        Dim Order As New SalesOrder(OrderID)

                        Dim returnData As String = String.Empty
                        For Each d As OrderDetail In Order.Detail
                            If IsNothing(d.Lot) = False Then
                                Dim detail As String = d.InventoryItem.Code + "|" + d.InventoryItem.Description + "|" + d.Lot.Code + "|" + d.Quantity.ToString() + "|" + d.Processed.ToString() + "|" + d.ToProcess.ToString() + "|" + d.InventoryItem.IsLotTracked.ToString() + "~"
                                returnData += detail
                            Else
                                Dim detail As String = d.InventoryItem.Code + "|" + d.InventoryItem.Description + "||" + d.Quantity.ToString() + "|" + d.Processed.ToString() + "|" + d.ToProcess.ToString() + "|" + d.InventoryItem.IsLotTracked.ToString() + "~"
                                returnData += detail
                            End If
                        Next

                        If returnData <> String.Empty Then
                            Return "1*" + returnData
                        Else
                            Return "0*SO Not found!"
                        End If
                    Case "-1"
                        Return init
                    Case Else
                        Return init
                End Select
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "FunctionMBL_GetSOLines: " + ex.ToString)
                Return ExHandler.returnErrorEx(ex)
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
        Public Shared Function MBL_UpdateSOLot(ByVal SONum As String, ByVal Code As String, ByVal Lot As String, ByVal Qty As String)
            Try
                Dim init As String = Initialize()
                Select Case init.Split("*")(0)
                    Case "1"
                        Dim OrderID As Integer = SalesOrder.Find("OrderNum = '" & SONum & "' AND (DocState=1 OR DocState=3)")
                        Dim Order As New SalesOrder(OrderID)

                        Dim lotFound As Boolean = False
                        Dim oriDetail As OrderDetail
                        Dim lotDetail As OrderDetail

                        For Each d As OrderDetail In Order.Detail
                            If IsNothing(d.Lot) = False Then
                                If d.InventoryItem.Code = Code And d.Lot.Code = Lot Then
                                    lotFound = True
                                    lotDetail = d
                                End If
                            Else
                                If d.InventoryItem.Code = Code Then
                                    oriDetail = d
                                End If
                            End If
                        Next

                        If lotFound = True Then
                            If IsNothing(oriDetail) = False Then
                                oriDetail.Quantity = oriDetail.Quantity - Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                                lotDetail.Quantity += Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                                lotDetail.ToProcess += Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            Else
                                lotDetail.ToProcess += Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            End If
                            Order.Save()
                            Return "1*Success"
                        Else
                            Dim newd As New OrderDetail '= d
                            newd.InventoryItem = oriDetail.InventoryItem
                            newd.Quantity = Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            newd.ToProcess = Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            newd.TaxMode = oriDetail.TaxMode
                            newd.Warehouse = CGetWhse(oriDetail.WarehouseID)
                            newd.PriceListNameID = oriDetail.PriceListNameID
                            newd.UnitCostPrice = oriDetail.UnitCostPrice
                            newd.UnitSellingPrice = oriDetail.UnitSellingPrice
                            'newd.Project = oriDetail.Project

                            Dim newLot As Lot = New Lot()
                            newLot.Code = Lot
                            newLot.Branch = oriDetail.InventoryItem.Branch
                            newLot.BranchID = oriDetail.InventoryItem.BranchID
                            newLot.InventoryItem = oriDetail.InventoryItem
                            newd.Lot = newLot

                            Order.Detail.Add(newd)
                            oriDetail.Quantity = oriDetail.Quantity - Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            Order.Save()
                            Return "1*Success"
                        End If
                    Case "-1"
                        Return init
                    Case Else
                        Return init
                End Select
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateSOLot: " + ex.ToString)
                Return "-1*" + ex.Message 'ExHandler.returnErrorEx(ex)
            End Try
        End Function

        Public Shared Function MBL_UpdateSONoLot(ByVal SONum As String, ByVal Code As String, ByVal Qty As String)
            Try
                Dim init As String = Initialize()
                Select Case init.Split("*")(0)
                    Case "1"
                        Dim OrderID As Integer = SalesOrder.Find("OrderNum = '" & SONum & "' AND (DocState=1 OR DocState=3)")
                        Dim Order As New SalesOrder(OrderID)

                        Dim itemFound As Boolean = False
                        Dim oriDetail As OrderDetail

                        For Each d As OrderDetail In Order.Detail
                            If d.InventoryItem.Code = Code And IsNothing(d.Lot) Then
                                oriDetail = d
                            End If
                        Next

                        If itemFound Then
                            oriDetail.ToProcess += Convert.ToDouble(Qty.Replace(".", sep).Replace(",", sep))
                            oriDetail.Save()
                            Order.Save()
                            Return "1*Success"
                        Else
                            Return "-1*ITEM NOT FOUND ON SO!"
                        End If
                    Case "-1"
                        Return init
                    Case Else
                        Return init
                End Select
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "MBL_UpdateSOLot: " + ex.ToString)
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
    End Class
End Class
