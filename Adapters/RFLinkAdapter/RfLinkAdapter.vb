Imports System.Diagnostics.CodeAnalysis
Imports System.IO.Ports
Imports System.Threading
Imports System.Timers
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.AsyncExtensions
Imports com.magpern.gateway2mqtt.Extentions.EventArgs
Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports Microsoft.Extensions.Logging

Public Class RfLinkAdapter
    Inherits SerialConnectionHelper
    Implements IGatewayAdapter, ISerialConnection

    Public Shared Property Logger As ILogger
    Public Shared Property Config As IRfLinkConfig
    Public Shared Property ReadThread As Thread

    Public Property ComPort As SerialPort Implements ISerialConnection.ComPort
    Public Property DoContinue As Boolean Implements ISerialConnection.DoContinue
    Public Property Port As String = "COM6" Implements ISerialConnection.Port
    Public Property BaudRate As Integer = 57600 Implements ISerialConnection.BaudRate
    Public Property DataBit As Integer = 8 Implements ISerialConnection.DataBit
    Public Property Parity As SerialParity = SerialParity.None Implements ISerialConnection.Parity
    Public Property StopBit As SerialStopBits = SerialStopBits.One Implements ISerialConnection.StopBit
    Public Property ReadTimeout As Integer = SerialPort.InfiniteTimeout Implements ISerialConnection.ReadTimeout
    Public Property WriteTimeout As Integer = SerialPort.InfiniteTimeout Implements ISerialConnection.WriteTimeout

    Public Sub New(log As ILogger(Of RfLinkAdapter), conf As IRfLinkConfig)
        Logger = log
        Config = conf
    End Sub

    Public Event DataReceived(sender As IGatewayAdapter, e As GatewayDataRecievedArg) _
        Implements IGatewayAdapter.DataReceived

    Public Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg) _
        Implements IGatewayAdapter.ConnectionState

    <ExcludeFromCodeCoverage> _
    Public Sub StopAdapter() Implements IGatewayAdapter.StopAdapter
        DoContinue = False
        Thread.Sleep(1)
        ReadThread.Join()
        ComPort.Close()
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Async Sub ReadAsync()

        Dim aTimer As New Timers.Timer(60000)
        If Config.Rflinkheartbeat Then
            aTimer.Interval = Config.RflinkHeartbeatInterval
            ' Hook up the Elapsed event for the timer. 
            AddHandler aTimer.Elapsed, AddressOf OnTimedEventAsync
            aTimer.AutoReset = True
            aTimer.Enabled = True
        End If
        
        Try
            While DoContinue
                Try
                    Dim message As String = Await ComPort.ReadLineAsync
                    Await ProcessMessageAsync(message)
                Catch unusedTimeoutException As TimeoutException
                Catch e As OperationCanceledException
                    Logger.LogError(e, "Error while accessing serial port")
                    RaiseEvent ConnectionState(Me, New GatewayConnectionStateArg(Extentions.ConnectionState.Offline))
                    Exit While
                End Try
            End While
        Finally
            aTimer.Dispose()
            ComPort.Dispose 
        End Try
    End Sub

    Public Async Function ProcessMessageAsync(message As String) As Task
        Await Task.Factory.StartNew(Sub() ProcessMessage(message))
    End Function

    Public Async Sub ProcessMessage(msg As String)
        Dim message = msg.Substring(0, msg.LastIndexOf(";", StringComparison.Ordinal) + 1)
        Dim result = MessageConverter.DecodeRawMessage(message)
        If result IsNot Nothing Then
            Logger.LogDebug($"Recieved {result.Count} messages to process for queue")
            For Each cmd As Dictionary(Of String, String) In result
                Dim mqttMessage As IMqttMessage = Await MessageToMqttMessage(cmd)

                RaiseEvent DataReceived(Me,
                                 New GatewayDataRecievedArg With {.Payload = mqttMessage})
            Next
        Else
            Logger.LogDebug($"Message not releyed to MQTT ({message})")
        End If
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

    <ExcludeFromCodeCoverage> _
    Public Async Function StartAdapter() As Task Implements IGatewayAdapter.StartAdapter
        OpenComPort()
        DoContinue = True
        Await ComPort.BaseStream.FlushAsync
        ComPort.RtsEnable = true
        ComPort.DtrEnable = True
        ComPort.DiscardInBuffer 
        ComPort.DiscardOutBuffer
        'ComPort.WriteLineAsync("10;RESET;").Wait 

        ReadThread = New Thread(AddressOf ReadAsync) With {
            .IsBackground = True,
            .Name = Me.GetType.ToString
                        }
        ReadThread.Start()
        RaiseEvent ConnectionState(Me, New GatewayConnectionStateArg(Extentions.ConnectionState.Online))
    End Function

    <ExcludeFromCodeCoverage> _
    Private Sub OpenComPort()
        ComPort = New SerialPort(Config.RflinkTtyDevice, BaudRate, MapParity(Parity), DataBit, MapStopBits(StopBit)) _
            With {
                .ReadTimeout = ReadTimeout,
                .WriteTimeout = WriteTimeout
                }

        Do
            Try
                Logger.LogDebug($"Atempting to open serial port {Config.RflinkTtyDevice}")
                ComPort.Open()
                Logger.LogInformation("Serial port open")
                Exit Do
            Catch ex As Exception
                Logger.LogWarning(ex.Message)
                Thread.Sleep(5000)
            End Try
        Loop While Not ComPort.IsOpen
    End Sub

    <ExcludeFromCodeCoverage> _
    Public Async Sub OnTimedEventAsync(sender As Object, e As ElapsedEventArgs)
        If ComPort.IsOpen Then
            Logger.LogTrace("Heartbeat" + e.SignalTime.ToString)
            Await ComPort.WriteLineAsync("10;PING;")
        End If
    End Sub

End Class