Imports System.Timers

Public Class Service

    Dim tmrTransfer As New System.Timers.Timer()
    Dim triggerd As Boolean = False

    Protected Overrides Sub OnStart(ByVal args() As String)
        Admin.AddFirewallException(My.Application.Info.ProductName, System.Windows.Forms.Application.ExecutablePath)
        Server.Listener.StartListen()
         Server.Listener.StartListenFile()
        tmrTransfer.Interval = 180000
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

    Protected Overrides Sub OnStop()
        Server.CloseAllListeners()
        Threading.Thread.Sleep(1000)
        'tmrTransfer.Stop()
        Server.CloseAllListeners()
    End Sub

End Class