Imports System.Timers

Public Class Form1

    Dim tmrTransfer As New System.Timers.Timer()
    Dim triggerd As Boolean = False

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Server.Listener.StartListen()
        Server.Listener.StartListenFile()
        tmrTransfer.Interval = 10000
        AddHandler tmrTransfer.Elapsed, AddressOf tmrTransfer_Tick
        tmrTransfer.Start()
    End Sub

    Private Sub tmrTransfer_Tick(sender As Object, e As ElapsedEventArgs)
        tmrTransfer.Stop()
        Dim rand As New Random()
        Dim lower As Integer = 180000
        Dim upper As Integer = 360000
        Dim intRandomNumber = rand.Next(lower, upper)
        tmrTransfer.Interval = intRandomNumber
        triggerd = True

        Server.Listener.StartListen()


        proccessWhseTransfers()
    End Sub

    Public Sub proccessWhseTransfers()
        Try
            tmrTransfer.Stop()
            WarehouseTransfers.proccessWhseTransfers()
            tmrTransfer.Start()
        Catch ex As Exception
            EventLog.WriteEntry("RTIS Vulcan SVC", "proccessWhseTransfers: " + Environment.NewLine + ex.ToString())
            triggerd = False
            tmrTransfer.Start()
        End Try
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Server.CloseAllListeners()
        Threading.Thread.Sleep(1000)
        Server.CloseAllListeners()
    End Sub
End Class
