Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Text

Public Class Server

    Private Shared AllListeners As New List(Of TcpListener)

    Public Shared Sub CloseAllListeners()
        For Each l In AllListeners
            Try
                l.Stop()
            Catch ex As Exception

            End Try
        Next
    End Sub

    Partial Public Class Listener
        Public Shared Sub StartListen()
            Dim t As New Thread(AddressOf ListenThread)
            t.Start()
        End Sub

        Public Shared Sub StartListenFile()
            Dim t As New Thread(AddressOf FileListenThread)
            t.Start()
        End Sub

        Public Shared CloseSocket As Boolean = False

        Private Shared Sub ListenThread()
            Try
                Dim ThisIPAddress As IPAddress
                ThisIPAddress = IPAddress.Parse(GetIPv4Address)
                EventLog.WriteEntry("RTIS Odin", "Listener strated listening on " & ThisIPAddress.ToString() & " on port " & My.Settings.Port)
                Dim Listener As TcpListener = New TcpListener(ThisIPAddress, Convert.ToInt32(My.Settings.Port))
                AllListeners.Add(Listener)
                Listener.Start()

                Do Until CloseSocket = True
                    Dim ListenSoc As Socket
                    ListenSoc = Listener.AcceptSocket
                    Dim t As New Thread(AddressOf StartReceive)
                    t.Start(ListenSoc)
                Loop
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "ListenThread Error: " & ex.Message)
                Thread.Sleep(5000)
                ListenThread()
            End Try
        End Sub
        Private Shared Sub FileListenThread()
            Try
                Dim ThisIPAddress As IPAddress
                ThisIPAddress = IPAddress.Parse(GetIPv4Address)
                EventLog.WriteEntry(NameOf(RTIS_Vulcan_SVC), "Listener strated listening on " & ThisIPAddress.ToString() & " on port " & My.Settings.Port)
                Dim Listener As TcpListener = New TcpListener(ThisIPAddress, Convert.ToInt32(My.Settings.Port))
                AllListeners.Add(Listener)
                Listener.Start()

                Do Until CloseSocket = True
                    Dim ListenSoc As Socket
                    ListenSoc = Listener.AcceptSocket
                    Dim t As New Thread(AddressOf StartReceiveFile)
                    t.Start(ListenSoc)
                Loop
            Catch ex As Exception
                EventLog.WriteEntry(NameOf(RTIS_Vulcan_SVC), "ListenThread Error: " & ex.Message)
                Thread.Sleep(5000)
                ListenThread()
            End Try
        End Sub
        Private Shared Sub StartReceiveFile(ByVal ThisSocket As Socket)
            Try
                Dim bufferSize As Integer = 2048
                Dim buffer(2048 - 1) As Byte
                Dim header(2048 - 1) As Byte
                Dim headerStr As String = String.Empty
                Dim filename As String = String.Empty
                Dim fileSize As Integer = 0

                ThisSocket.Receive(header)
                headerStr = Encoding.ASCII.GetString(header)

                Dim splitter As String() = {"#"}
                Dim splitted As String() = headerStr.Split(splitter, StringSplitOptions.RemoveEmptyEntries)
                Dim headers As New Dictionary(Of String, String)
                For Each s As String In splitted
                    If s.Contains(":") Then
                        headers.Add(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1))
                    End If
                Next

                Dim serverCommand As  String = headers("Command")
                Dim fileType As  String = headers("Filetype")
                Select Case fileType
                    Case "SIGN"
                        If Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\") = False Then
                            Directory.CreateDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\")
                        End If
                        filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\Signatures\" + headers("Filename")
                    Case "PALLETLABEL"
                        If Directory.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\") = False Then
                            Directory.CreateDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\")
                        End If
                        filename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\RSC\PalletLabel\" + headers("Filename")
                End Select  

                fileSize = Convert.ToInt32(headers("Content-length"))
                Dim bufferCount As Integer = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileSize) / Convert.ToDouble(bufferSize)))
                Dim fs As New FileStream(filename, FileMode.OpenOrCreate)
                While fileSize > 0
                    Dim size As Integer = ThisSocket.Receive(buffer, SocketFlags.Partial)
                    fs.Write(buffer, 0, size)
                    fileSize -= size
                End While
                fs.Close()

                ServerResponse.Determine(serverCommand, ThisSocket)
            Catch ex As Exception
                'EventLog.WriteEntry("RTIS_Zeus19_SVC", "StartReceive Error: " & ex.Message)
                ThisSocket.Close()
            End Try
        End Sub
        Private Shared Sub StartReceive(ByVal ThisSocket As Socket)
            Try
                Dim ClientMsg As String = ""
                Dim byteData(131072) As Byte

                ThisSocket.ReceiveTimeout = 5000
                Dim length As Integer = ThisSocket.Receive(byteData)
                'For i As Integer = 0 To length - 1
                '    ClientMsg += Convert.ToChar(byteData(i)) 'Char.ConvertFromUtf32(Convert.ToInt32(byteData(i))) '
                'Next

                ClientMsg = UTF8Encoding.UTF8.GetString(byteData).Replace(vbNullChar, String.Empty)
                ServerResponse.Determine(ClientMsg, ThisSocket)
            Catch ex As Exception
                'EventLog.WriteEntry("RTIS Vulcan SVC", "StartReceive Error: " & ex.Message)
                ThisSocket.Close()
            End Try
        End Sub

        Public Shared Sub SendResponse(ByVal ClientSocket As Socket, ByVal ResponseData As String)
            Try
                Dim SendData(ResponseData.Length - 1) As Byte
                'Dim ascenc As New ASCIIEncoding
                Dim ascenc As New UTF8Encoding
                SendData = ascenc.GetBytes(ResponseData)
                ClientSocket.Send(SendData)
                ClientSocket.Close()
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "SendResponse Error: " & ex.Message)
                ClientSocket.Close()
            End Try
        End Sub

        Public Shared Sub SendResponseFile(ByVal ClientSocket As Socket, ByVal ResponseData As String, ByVal FileName As String)
            Try
                Dim SendData(ResponseData.Length - 1) As Byte
                Dim ascenc As New ASCIIEncoding
                SendData = ascenc.GetBytes(ResponseData)
                ClientSocket.SendFile(FileName)
                Thread.Sleep(5000)
                ClientSocket.Close()
            Catch ex As Exception
                EventLog.WriteEntry("RTIS Vulcan SVC", "SendResponse Error: " & ex.Message)
                ClientSocket.Close()
            End Try
        End Sub
    End Class

    Private Shared Function GetIPv4Address() As String
        Try
            Dim Adaptors As List(Of NetworkInformation.NetworkInterface) = NetworkInformation.NetworkInterface.GetAllNetworkInterfaces.ToList

            'For wired networks
            For Each Ad In Adaptors
                If Ad.NetworkInterfaceType = NetworkInformation.NetworkInterfaceType.Ethernet Then
                    If Ad.OperationalStatus <> NetworkInformation.OperationalStatus.Down Then
                        If Ad.Description.Contains("VPN") = False Then
                            For Each addr In Ad.GetIPProperties.UnicastAddresses
                                If addr.Address.AddressFamily = AddressFamily.InterNetwork Then
                                    Return addr.Address.ToString
                                End If
                            Next
                        End If
                    End If
                End If
            Next

            'For wireless networks
            For Each Ad In Adaptors
                If Ad.NetworkInterfaceType = NetworkInformation.NetworkInterfaceType.Wireless80211 Then
                    If Ad.OperationalStatus <> NetworkInformation.OperationalStatus.Down Then
                        For Each addr In Ad.GetIPProperties.UnicastAddresses
                            If addr.Address.AddressFamily = AddressFamily.InterNetwork Then
                                Return addr.Address.ToString
                            End If
                        Next
                    End If
                End If
            Next
        Catch
        End Try
        Return "127.0.0.1"
    End Function
End Class
