Imports System.Data.SQLite
Imports System.IO

Public Class SQLite

    Public Class Create
        Public Shared Function createPODB(ByVal orderNum As String)
            Try

                Dim dbPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3"
                Dim conn As New SQLiteConnection("Data Source =" + dbPath)
                Dim comm As New SQLiteCommand()
                conn.Open()
                comm.CommandText = "CREATE TABLE [UNQ] (
                                    [Barcode] NVARCHAR(128),
                                    [Receive] NVARCHAR(128),
                                    [bValidated] BIT
                                    )"
                comm.Connection = conn
                comm.ExecuteNonQuery()
                comm.CommandText = "CREATE TABLE [PO] (
                                    [ItemCode] NVARCHAR(128),
                                    [Description] NVARCHAR(128),
                                    [Description2] NVARCHAR(128),
                                    [bLotItem] INT,
                                    [LotNumber] NVARCHAR(128),
                                    [OrderQty] DECIMAL(16,3),
                                    [RecQty] DECIMAL(16,3),
                                    [ProcQty] DECIMAL(16,3),
                                    [ViewableQty] DECIMAL(16,3),
                                    [PrintQty] DECIMAL(16,3), 
                                    [UserScanned] NVARCHAR(128),
                                    [DTUserScanned] DATETIME
                                    )"
                comm.ExecuteNonQuery()
                comm.Dispose()
                conn.Close()
                conn.Dispose()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
    End Class

    Public Class Insert
        Public Shared Function inserPOLine(ByVal orderNum As String, ByVal itemCode As String, ByVal desc As String, ByVal desc2 As String, ByVal isLot As String, ByVal lotNum As String, ByVal qty As String, ByVal qtyToProc As String,
                                           ByVal qtyProc As String, ByVal viewableQty As String, ByVal printQty As String)
            Try
                Dim dbPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3"
                Dim conn As New SQLiteConnection("Data Source =" + dbPath)
                Dim comm As New SQLiteCommand()
                comm.CommandText = "INSERT INTO [PO] ([ItemCode], [Description], [Description2], [bLotItem], [LotNumber], [OrderQty], [RecQty], [ProcQty], [ViewableQty], [PrintQty], [UserScanned], [DTUserScanned]) VALUES
                                                     (@1        , @2           ,  @3           , @4        , @5         , @6        , @7      , @8       , @9           , @10       , NULL         , NULL)"
                comm.Parameters.Add(New SQLiteParameter("@1", itemCode))
                comm.Parameters.Add(New SQLiteParameter("@2", desc))
                comm.Parameters.Add(New SQLiteParameter("@3", desc2))
                comm.Parameters.Add(New SQLiteParameter("@4", isLot))
                comm.Parameters.Add(New SQLiteParameter("@5", lotNum))
                comm.Parameters.Add(New SQLiteParameter("@6", qty))
                comm.Parameters.Add(New SQLiteParameter("@7", qtyToProc))
                comm.Parameters.Add(New SQLiteParameter("@8", qtyProc))
                comm.Parameters.Add(New SQLiteParameter("@9", viewableQty))
                comm.Parameters.Add(New SQLiteParameter("@10", printQty))
                comm.Connection = conn
                conn.Open()
                comm.ExecuteNonQuery()
                comm.Dispose()
                conn.Close()
                conn.Dispose()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try


        End Function

        Public Shared Function inserPOLines(ByVal orderNum As String, ByVal query As String)
            Try
                Dim dbPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3"
                Dim conn As New SQLiteConnection("Data Source =" + dbPath)
                Dim comm As New SQLiteCommand()
                comm.CommandText = query
                comm.Connection = conn
                conn.Open()
                comm.ExecuteNonQuery()
                comm.Dispose()
                conn.Close()
                conn.Dispose()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function

        Public Shared Function inserPOLinesUnq(ByVal orderNum As String, ByVal query As String)
            Try
                Dim dbPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3"
                Dim conn As New SQLiteConnection("Data Source =" + dbPath)
                Dim comm As New SQLiteCommand()
                comm.CommandText = query
                comm.Connection = conn
                conn.Open()
                comm.ExecuteNonQuery()
                comm.Dispose()
                conn.Close()
                conn.Dispose()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function

        Public Shared Function insertUnq(ByVal orderNum As String, ByVal barcode As String, ByVal rec As String, ByVal valid As String)
            Try
                Dim dbPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\RSC\\" + orderNum + ".db3"
                Dim conn As New SQLiteConnection("Data Source =" + dbPath)
                Dim comm As New SQLiteCommand()
                comm.CommandText = "INSERT INTO [UNQ] ([Barcode], [Receive], [bValidated]) VALUES
                                                      (@1        , @2      , @3          )"
                comm.Parameters.Add(New SQLiteParameter("@1", barcode))
                comm.Parameters.Add(New SQLiteParameter("@2", rec))
                comm.Parameters.Add(New SQLiteParameter("@3", valid))
                comm.Connection = conn
                conn.Open()
                comm.ExecuteNonQuery()
                comm.Dispose()
                conn.Close()
                conn.Dispose()
                Return "1*Success"
            Catch ex As Exception
                Return ExHandler.returnErrorEx(ex)
            End Try
        End Function
    End Class

End Class
