Imports System.IO.Ports
Imports System.Threading
Imports System.Timers
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.AsyncExtensions
Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports Microsoft.Extensions.Logging

Public Class RfLinkAdapter
    Inherits SerialConnection
    Implements IGatewayAdapter

    Private ReadOnly Property Logger As ILogger
    Private ReadOnly _config As IRFLinkConfig
    Private Shared _readThread As Thread

    Public Sub New(logger As ILogger(Of RfLinkAdapter), config As IRFLinkConfig)
        Me.Logger = logger
        _config = config
    End Sub

    Public Event DataReceived(sender As IGatewayAdapter, e As GatewayDataRecievedArg) _
        Implements IGatewayAdapter.DataReceived

    Public Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg) _
        Implements IGatewayAdapter.ConnectionState

    Public Sub StopAdapter() Implements IGatewayAdapter.StopAdapter
        [Continue] = False
        Thread.Sleep(1)
        _readThread.Join()
        comPort.Close()
    End Sub

    Private Async Sub ReadAsync()

        Dim aTimer = New Timers.Timer(60000)
        ' Hook up the Elapsed event for the timer. 
        AddHandler aTimer.Elapsed, AddressOf OnTimedEventAsync
        aTimer.AutoReset = True
        aTimer.Enabled = True

        Try
            While [Continue]
                Try
                    Dim message As String = Await comPort.ReadLineAsync
                    Await ProcessMessageAsync(message)
                Catch unusedTimeoutException As TimeoutException
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

    Private Async Sub ProcessMessage(message As String)

        Dim result = MessageConverter.DecodeRawMessage(message)
        Logger.LogDebug($"Recieved {result.Count} messages to process for queue")
        For Each cmd As Dictionary(Of  String,string) In result
            Dim mqttMessage As ImqttMessage = Await MessageToMQTTmessage(cmd)
            
            RaiseEvent _
                DataReceived(Me,
                             New GatewayDataRecievedArg With {.Payload = New DataPayload With {.message = mqttMessage}})
        Next
    End Sub

    Private Function MessageToMqttMessage(msg As Dictionary(Of String,String)) As ValueTask(Of IMqttMessage)
        Logger.LogInformation(string.Join(";", msg.Select(Function(x) x.Key + "=" + x.Value).ToArray()))
        Dim message As New MQTTMessage
        message.Message = $"{msg("family")}/{msg("device_id")}/R/{msg("param")}"
        
            
        
    End Function


    Private Async Function StartAdapter() As Task Implements IGatewayAdapter.StartAdapter
        OpenComPort()
        [Continue] = True

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
        comPort = New SerialPort(_config.RflinkTtyDevice, BaudRate, MapParity(Parity), DataBit, MapStopBits(StopBit)) _
            With {
                .ReadTimeout = ReadTimeout,
                .WriteTimeout = WriteTimeout
                }

        Do
            Try
                Logger.LogDebug($"Atempting to open COM-port {_config.RflinkTtyDevice}")
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

'    Public Async Function RequestGatewayVerionAsync() As Task(Of IgatewayVersion) Implements IGatewayAdapter.RequestGatewayVerionAsync
'        If _gatewayversion Is Nothing Then
'            If comPort.IsOpen Then
'                Await comPort.WriteLineAsync("10;VERSION;")
'                _gatewayversion = New GatewayVersion With {.Name = "fake version"}
'            End If
'            Do
'                Thread.Sleep(50)
'            Loop While _gatewayversion Is Nothing
'        End If
'        Return _gatewayversion
'    End Function
End Class