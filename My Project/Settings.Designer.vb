﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("A GF 100")>  _
        Public Property Project() As String
            Get
                Return CType(Me("Project"),String)
            End Get
            Set
                Me("Project") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("32017")>  _
        Public Property Port() As String
            Get
                Return CType(Me("Port"),String)
            End Get
            Set
                Me("Port") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property RTUser() As String
            Get
                Return CType(Me("RTUser"),String)
            End Get
            Set
                Me("RTUser") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property EvoUser() As String
            Get
                Return CType(Me("EvoUser"),String)
            End Get
            Set
                Me("EvoUser") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property EvoComUser() As String
            Get
                Return CType(Me("EvoComUser"),String)
            End Get
            Set
                Me("EvoComUser") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("CATALER2")>  _
        Public Property EvoComServer() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property RTUser() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("EvoComServer"),String)
            End Get
            Set
                Me("EvoComServer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("CATALER2")>  _
        Public Property RTServer() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property EvoUser() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("RTServer"),String)
            End Get
            Set
                Me("RTServer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("CATALER2")>  _
        Public Property EvoServer() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("Reltech")>  _
        Public Property EvoComUser() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("EvoServer"),String)
            End Get
            Set
                Me("EvoServer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("R3lt3ch!4")>  _
        Public Property EvoComPassword() As String
            Get
                Return CType(Me("EvoComPassword"),String)
            End Get
            Set
                Me("EvoComPassword") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("R3lt3ch!4")>  _
        Public Property RTPassword() As String
            Get
                Return CType(Me("RTPassword"),String)
            End Get
            Set
                Me("RTPassword") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("R3lt3ch!4")>  _
        Public Property EvoPassword() As String
            Get
                Return CType(Me("EvoPassword"),String)
            End Get
            Set
                Me("EvoPassword") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("EvolutionCommon")>  _
        Public Property EvoComDB() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("localhost")>  _
        Public Property RTServer() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("EvoComDB"),String)
            End Get
            Set
                Me("EvoComDB") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("CAT_RTIS_Test")>  _
        Public Property RTDB() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("localhost")>  _
        Public Property EvoServer() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("RTDB"),String)
            End Get
            Set
                Me("RTDB") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
<<<<<<< HEAD
         Global.System.Configuration.DefaultSettingValueAttribute("Cataler_SCN_Test")>  _
        Public Property EvoDB() As String
=======
         Global.System.Configuration.DefaultSettingValueAttribute("localhost")>  _
        Public Property EvoComServer() As String
>>>>>>> 05526b5f42d743be644cbbc39c3d55a9dc1547d0
            Get
                Return CType(Me("EvoDB"),String)
            End Get
            Set
                Me("EvoDB") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.RTIS_Vulcan_SVC.My.MySettings
            Get
                Return Global.RTIS_Vulcan_SVC.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
