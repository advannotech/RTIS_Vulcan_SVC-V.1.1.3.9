<System.ComponentModel.RunInstaller(True)> Partial Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Local_Advannotech_SVC = New System.ServiceProcess.ServiceProcessInstaller()
        Me.Advannotech_SVC = New System.ServiceProcess.ServiceInstaller()
        '
        'Local_Advannotech_SVC
        '
        Me.Local_Advannotech_SVC.Account = System.ServiceProcess.ServiceAccount.LocalService
        Me.Local_Advannotech_SVC.Password = Nothing
        Me.Local_Advannotech_SVC.Username = Nothing
        '
        'Advannotech_SVC
        '
        Me.Advannotech_SVC.ServiceName = "RTIS Vulcan SVC"
        Me.Advannotech_SVC.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.Local_Advannotech_SVC, Me.Advannotech_SVC})

    End Sub

    Friend WithEvents Local_Advannotech_SVC As ServiceProcess.ServiceProcessInstaller
    Friend WithEvents Advannotech_SVC As ServiceProcess.ServiceInstaller
End Class
