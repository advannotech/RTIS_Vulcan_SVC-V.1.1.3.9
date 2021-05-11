Public Class Barcodes

    Public Shared Function GetItemCode(ByVal barcode As String) As String
        Try
            Dim itemCode As String = String.Empty
            Barcode = Barcode.Replace("(", "*")
            Barcode = Barcode.Replace(")", "~")
            Dim Items As String() = Barcode.Split("*")
            For Each item As String In Items
                If item <> String.Empty Then
                    If item.Split("~")(0) = "240" Then
                        itemcode = item.Split("~")(1).TrimEnd()
                        Exit For
                    End If
                End If
            Next
            Return If(itemCode <> String.Empty, itemCode, "None")
        Catch ex As Exception
            Return "NONE"
        End Try       
    End Function

    Public Shared Function GetItemLot(ByVal barcode As String) As String
        Try
            Dim itemlot As String = String.Empty
            Barcode = Barcode.Replace("(", "*")
            Barcode = Barcode.Replace(")", "~")
            Dim Items As String() = Barcode.Split("*")
            For Each item As String In Items
                If item <> String.Empty Then
                    If item.Split("~")(0) = "10" Then
                        itemlot = item.Split("~")(1).TrimEnd()
                        Exit For
                    End If
                End If
            Next
            Return If(itemlot <> String.Empty, itemlot, "None")
        Catch ex As Exception
            Return "NONE"
        End Try       
    End Function

    Public Shared Function GetItemQty(ByVal barcode As String) As String
        Try
            Dim itemqty As String = String.Empty
            Barcode = Barcode.Replace("(", "*")
            Barcode = Barcode.Replace(")", "~")
            Dim Items As String() = Barcode.Split("*")
            For Each item As String In Items
                If item <> String.Empty Then
                    If item.Split("~")(0) = "30" Then
                        itemqty = item.Split("~")(1).TrimEnd()
                        Exit For
                    End If
                End If
            Next
            Return If(itemqty <> String.Empty, itemqty, "0")
        Catch ex As Exception
            Return "0"
        End Try       
    End Function

    Public Shared Function GetUniqCode(ByVal barcode As String) As String
        Try
            Dim uniq As String = String.Empty
            Barcode = Barcode.Replace("(", "*")
            Barcode = Barcode.Replace(")", "~")
            Dim Items As String() = Barcode.Split("*")
            For Each item As String In Items
                If item <> String.Empty Then
                    If item.Split("~")(0) = "90" Then
                        uniq = item.Split("~")(1).TrimEnd()
                        Exit For
                    End If
                End If
            Next
            Return If(uniq <> String.Empty, uniq, "None")
        Catch ex As Exception
            Return "NONE"
        End Try       
    End Function
End Class
