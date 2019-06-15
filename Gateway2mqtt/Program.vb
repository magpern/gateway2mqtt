Imports System.Runtime.Loader
Imports System.Threading
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging
Imports MQTTnet
Imports MQTTnet.Client
Imports MQTTnet.Client.Options
Imports MQTTnet.Server

Public Class Program
    Private Const Loglevel As LogLevel = LogLevel.Trace

    Private Shared _logger As ILogger
    Private Shared ReadOnly _closing As New AutoResetEvent(False)
    Private Shared RunningThreads As New List(Of IGatewayAdapter)
    Private Shared _mqttConnection As IMqttClient
    Private Shared ReadOnly cts As New CancellationTokenSource()

    Public Shared Sub Main(ByVal args As String())
        AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf CurrentDomain_ProcessExit
        AddHandler AssemblyLoadContext.Default.Unloading, AddressOf Default_Unloading
        AddHandler System.Console.CancelKeyPress, AddressOf OnExit
        MainAsync(args, cts.Token).GetAwaiter.GetResult 
    End Sub

    Private Shared Async Function MainAsync(ByVal args As String(), ByVal token As CancellationToken) As Task
        
        Dim serviceCollection = New ServiceCollection()
        ConfigureServices(serviceCollection)
        Dim serviceProvider = serviceCollection.BuildServiceProvider()
        _logger = serviceProvider.GetService(Of ILogger(Of Program))()
        _logger.LogInformation("Gateway2mqtt is started")

        'TODO Connect the MQTT
        _mqttConnection = await MqttConnect
        '_mqttConnection.
        'TODO Start serial communication 
        'TODO Set Subscribe thread

        'Dim rflink As IGatewayAdapter = serviceProvider.GetService(Of RfLinkAdapter)

        'RunningThreads.Add(rflink)
        'rflink.StartAdapter()
        'AddHandler rflink.DataRecieved, AddressOf DataProcessor
        'AddHandler rflink.ConnectionState, AddressOf GatewayConnectionState

        _closing.WaitOne()

        'RunningThreads.ForEach(Sub(x) x.StopAdapter())

        System.Console.ReadKey()
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
        _logger.LogTrace(e.Payload.message)
    End Sub

    Private Shared Sub ConfigureServices(ByVal services As IServiceCollection)
        services.AddLogging(Function(configure) configure.AddConsole()) _
            .Configure(New Action(Of LoggerFilterOptions)(Sub(x) x.MinLevel = Loglevel)) _
            .AddSingleton(Of RfLinkAdapter).AddSingleton(Of HomeAssistantBinder).AddTransient(Of MessageConverter) _
            .AddSingleton(Of IConfig, Config).AddSingleton(Of IRFLinkConfig, RFLinkConfig)
    End Sub

    Private Shared Sub Default_Unloading(obj As AssemblyLoadContext)
        System.Console.WriteLine("unload")
        _closing.Set()
        cts.CancelAfter(300)
    End Sub

    Private Shared Sub CurrentDomain_ProcessExit(sender As Object, e As EventArgs)
        System.Console.WriteLine("process exit")
        _closing.Set()
        cts.CancelAfter(300)
    End Sub

    Private Shared Sub OnExit(sender As Object, args As ConsoleCancelEventArgs)
        _logger.LogInformation("OnExit is triggered. Initiating shutdown")
        args.Cancel = True
        _closing.Set()
        cts.CancelAfter(300)
    End Sub

    Public Shared Async Function MqttConnect() As Task(of Client.IMqttClient)
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

