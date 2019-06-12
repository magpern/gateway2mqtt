Imports System.IO.Ports
Imports System.Threading
Imports System.Timers
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.AsyncExtensions.SerialPortExtensions
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging

Public Class RfLinkAdapter
    Inherits SerialConnection
    Implements IGatewayAdapter

    Private ReadOnly Property Logger() As ILogger
    Private ReadOnly Config As IRFLinkConfig
    Public Property ServiceProvider As ServiceProvider Implements IGatewayAdapter.ServiceProvider
    Private Shared _readThread As Thread
    Private Shared _gatewayversion As IgatewayVersion

    Public Sub New(ByVal logger As ILogger(Of RfLinkAdapter), config As IRFLinkConfig)
        Me.Logger = logger
        Me.config = config
    End Sub

    Public Event DataRecieved(sender As IGatewayAdapter, e As GatewayDataRecievedArg) Implements IGatewayAdapter.DataRecieved
    Public Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg) Implements IGatewayAdapter.ConnectionState

    Public Sub StopAdapter() Implements IGatewayAdapter.StopAdapter
        _continue = False
        Thread.Sleep(1)
        _readThread.Join()
        comPort.Close()
    End Sub

    Private Async Sub ReadAsync()

        Dim aTimer = New System.Timers.Timer(60000)
        ' Hook up the Elapsed event for the timer. 
        AddHandler aTimer.Elapsed, AddressOf OnTimedEventAsync
        aTimer.AutoReset = True
        aTimer.Enabled = True

        Try
            While _continue
                Try
                    Dim message As String = Await comPort.ReadLineAsync
                    Await ProcessMessageAsync(message)
                Catch __unusedTimeoutException1__ As TimeoutException
                Catch e As OperationCanceledException
                    Logger.LogError(e, "Error while accessing COM port")
                    RaiseEvent ConnectionState(Me, New GatewayConnectionStateArg(Extentions.ConnectionState.Offline))
                    Exit While
                End Try
            End While
        Finally
            aTimer.Dispose()
        End Try

    End Sub

    Private Async Function ProcessMessageAsync(message As String) As Task
        Await Task.Factory.StartNew(Sub() ProcessMessage(message))
    End Function

    Private Sub ProcessMessage(message As String)
        Dim mess = ServiceProvider.GetService(Of MessageConverter)
        mess.MessageToObject(message)

        RaiseEvent DataRecieved(Me, New GatewayDataRecievedArg With
                                       {.Payload = New DataPayload With
                                       {.message = message}})
    End Sub



    Private Async Function StartAdapter() As Task Implements IGatewayAdapter.StartAdapter
        OpenComPort()
        _continue = True

        Await comPort.BaseStream.FlushAsync
        comPort.RtsEnable = False
        comPort.DtrEnable = True
        _readThread = New Thread(AddressOf ReadAsync)
        _readThread.IsBackground = True
        _readThread.Name = Me.GetType.ToString
        _readThread.Start()
        RaiseEvent ConnectionState(Me, New GatewayConnectionStateArg(Extentions.ConnectionState.Online))
    End Function

    Private Sub OpenComPort()
        comPort = New SerialPort(Config.RflinkTtyDevice, BaudRate, MapParity(Parity), DataBit, MapStopBits(StopBit)) With {
            .ReadTimeout = ReadTimeout,
            .WriteTimeout = WriteTimeout
            }

        Do
            Try
                Logger.LogDebug($"Atempting to open COM-port {Config.RflinkTtyDevice}")
                comPort.Open()
                Exit Do
            Catch ex As Exception
                Logger.LogWarning(ex.Message)
                Thread.Sleep(5000)
            End Try
        Loop While Not comPort.IsOpen

    End Sub

    Private Async Sub OnTimedEventAsync(sender As Object, e As ElapsedEventArgs)
        If comPort.IsOpen Then
            Logger.LogTrace("Heartbeat" + e.SignalTime.ToString)
            Await comPort.WriteLineAsync("10;PING;")
        End If
    End Sub

    Public Function DataSendAsync(dataPayload As DataPayload) As Task Implements IGatewayAdapter.DataSendAsync
        Throw New NotImplementedException()
    End Function

    Public Async Function RequestGatewayVerionAsync() As Task(Of IgatewayVersion) Implements IGatewayAdapter.RequestGatewayVerionAsync
        If _gatewayversion Is Nothing Then
            If comPort.IsOpen Then
                Await comPort.WriteLineAsync("10;VERSION;")
                _gatewayversion = New GatewayVersion With {.Name = "fake version"}
            End If
            Do
                Thread.Sleep(50)
            Loop While _gatewayversion Is Nothing
        End If
        Return _gatewayversion
    End Function

End Class
