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

    Public Shared Property Logger As ILogger
    Public Shared Property Config As IRfLinkConfig
    Public Shared Property ReadThread As Thread

    Public Sub New(log As ILogger(Of IGatewayAdapter), conf As IRfLinkConfig)
        Logger = log
        Config = conf
    End Sub

    Public Event DataReceived(sender As IGatewayAdapter, e As GatewayDataRecievedArg) _
        Implements IGatewayAdapter.DataReceived

    Public Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg) _
        Implements IGatewayAdapter.ConnectionState

    Public Sub StopAdapter() Implements IGatewayAdapter.StopAdapter
        [Continue] = False
        Thread.Sleep(1)
        ReadThread.Join()
        ComPort.Close()
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
                    Dim message As String = Await ComPort.ReadLineAsync
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
        For Each cmd As Dictionary(Of String, String) In result
            Dim mqttMessage As IMqttMessage = Await MessageToMqttMessage(cmd)

            RaiseEvent DataReceived(Me,
                             New GatewayDataRecievedArg With {.Payload = New DataPayload With {.Message = mqttMessage}})
        Next
    End Sub

    Public Shared Function MessageToMqttMessage(msg As Dictionary(Of String, String)) As ValueTask(Of IMqttMessage)
        Dim subtopic As String

        If Not String.IsNullOrEmpty(msg("family")) Then
            subtopic = $"{msg("family")}/{msg("device_id")}/R/{msg("param")}"
        Else
            subtopic = "_COMMAND/OUT"
        End If

        Dim topic = $"{Config.MqttPrefix}/{subtopic}"
        Logger.LogInformation($"Sending:{String.Join(";", msg.Select(Function(x) x.Key + "=" + x.Value).ToArray())} to {topic}")

        Return New ValueTask(Of IMqttMessage)(New MqttMessage With {
            .Topic = topic,
            .Payload = msg("payload")
        })
    End Function


    Private Async Function StartAdapter() As Task Implements IGatewayAdapter.StartAdapter
        OpenComPort()
        [Continue] = True

        Await ComPort.BaseStream.FlushAsync
        ComPort.RtsEnable = False
        ComPort.DtrEnable = True
        ReadThread = New Thread(AddressOf ReadAsync)
        ReadThread.IsBackground = True
        ReadThread.Name = Me.GetType.ToString
        ReadThread.Start()
        RaiseEvent ConnectionState(Me, New GatewayConnectionStateArg(Extentions.ConnectionState.Online))
    End Function

    Private Sub OpenComPort()
        ComPort = New SerialPort(Config.RflinkTtyDevice, BaudRate, MapParity(Parity), DataBit, MapStopBits(StopBit)) _
            With {
                .ReadTimeout = ReadTimeout,
                .WriteTimeout = WriteTimeout
                }

        Do
            Try
                Logger.LogDebug($"Atempting to open COM-port {Config.RflinkTtyDevice}")
                ComPort.Open()
                Exit Do
            Catch ex As Exception
                Logger.LogWarning(ex.Message)
                Thread.Sleep(5000)
            End Try
        Loop While Not ComPort.IsOpen
    End Sub

    Private Async Sub OnTimedEventAsync(sender As Object, e As ElapsedEventArgs)
        If ComPort.IsOpen Then
            Logger.LogTrace("Heartbeat" + e.SignalTime.ToString)
            Await ComPort.WriteLineAsync("10;PING;")
        End If
    End Sub

    Public Function DataSendAsync(dataPayload As DataPayload) As Task Implements IGatewayAdapter.DataSendAsync
        Throw New NotImplementedException()
    End Function

End Class