Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.Loader
Imports System.Threading
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.EventArgs
Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports MQTTnet
Imports MQTTnet.Client
Imports MQTTnet.Client.Options

Public Class Program
    Private Const Loglevel As LogLevel = LogLevel.Debug

    Private Shared _logger As ILogger
    Private Shared ReadOnly IsClosingRequested As New AutoResetEvent(False)
    Private Shared _mqttConnection As IMqttClient

    <ExcludeFromCodeCoverage> _
    Public Shared Sub Main(args As String())
        AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf CurrentDomain_ProcessExit
        AddHandler AssemblyLoadContext.Default.Unloading, AddressOf Default_Unloading
        AddHandler System.Console.CancelKeyPress, AddressOf OnExit
        MainAsync(args).GetAwaiter.GetResult 
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Shared Async Function MainAsync(args As String()) As Task
        
        Dim serviceCollection = New ServiceCollection()
        ConfigureServices(serviceCollection)
        Dim serviceProvider = serviceCollection.BuildServiceProvider()
        _logger = serviceProvider.GetService(Of ILogger(Of Program))()
        _logger.LogInformation("Gateway2mqtt is started")

        MessageConverter.Logger = serviceProvider.GetService(Of ILogger(Of MessageConverter))
        MessageConverter.Config = serviceProvider.GetService(Of IRFLinkConfig)

        'TODO Connect the MQTT
        '_mqttConnection = await MqttConnect()
        '_mqttConnection.
        'TODO Start serial communication 
        Dim rflink = serviceProvider.GetService(Of RfLinkAdapter)
        AddHandler rflink.ConnectionState, AddressOf GatewayConnectionState
        AddHandler rflink.DataReceived, AddressOf DataProcessor 
        Await rflink.StartAdapter 

        'TODO Set Subscribe thread

        'Dim rflink As IGatewayAdapter = serviceProvider.GetService(Of RfLinkAdapter)

        'RunningThreads.Add(rflink)
        'rflink.StartAdapter()
        'AddHandler rflink.DataRecieved, AddressOf DataProcessor
        'AddHandler rflink.ConnectionState, AddressOf GatewayConnectionState

        IsClosingRequested.WaitOne()

        'RunningThreads.ForEach(Sub(x) x.StopAdapter())

        'System.Console.ReadKey()
    End Function

    Private Shared Async Sub GatewayConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg)
        If e.State = ConnectionState.Online Then
            'TODO: signal online state
        Else
            'TODO: signal offline state
            Await sender.StartAdapter()
        End If
    End Sub

    Private Shared Sub DataProcessor(sender As Object, e As GatewayDataRecievedArg)
        _logger.LogTrace($"{sender.ToString} : {e.Payload.ToString}")
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Shared Sub ConfigureServices(services As IServiceCollection)
        services.AddLogging(Function(configure) configure.AddConsole()) _
            .Configure(New Action(Of LoggerFilterOptions)(Sub(x) x.MinLevel = Loglevel)) _
            .AddSingleton(Of RfLinkAdapter).AddSingleton(Of HomeAssistantBinder) _
            .AddSingleton(Of IConfig, Config).AddSingleton(Of IRFLinkConfig, RFLinkConfig)
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Shared Sub Default_Unloading(obj As AssemblyLoadContext)
        System.Console.WriteLine("unload")
        IsClosingRequested.Set()
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Shared Sub CurrentDomain_ProcessExit(sender As Object, e As System.EventArgs)
        System.Console.WriteLine("process exit")
        IsClosingRequested.Set()
    End Sub

    <ExcludeFromCodeCoverage> _
    Private Shared Sub OnExit(sender As Object, args As ConsoleCancelEventArgs)
        _logger.LogInformation("OnExit is triggered. Initiating shutdown")
        args.Cancel = True
        IsClosingRequested.Set()
    End Sub

    Public Shared Async Function MqttConnect() As Task(of IMqttClient)
        ' Create a new MQTT client.
        Dim factory = New MqttFactory()
        Dim mqttClient = factory.CreateMqttClient()
        ' Create TCP based options using the builder.
        Dim options = New MqttClientOptionsBuilder().WithClientId("MagnusDator").WithTcpServer("mqtt.pernemark.home", 1883).WithCredentials("myuser", "secretpassword").WithCleanSession().Build()

        'Connect
        Dim source As New CancellationTokenSource()
        Dim token As CancellationToken = source.Token

        Dim res = Await mqttClient.ConnectAsync(options, token)
        Return mqttClient
    End Function
End Class

