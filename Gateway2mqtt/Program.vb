Imports System
Imports System.Runtime.Loader
Imports System.Text.Json
Imports System.Threading
Imports com.magpern.gateway2mqtt.Extentions
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Logging

Public Class Program

    Private Const _Loglevel As LogLevel = LogLevel.Trace

    Private Shared _logger As ILogger
    Private Shared ReadOnly _closing As New AutoResetEvent(False)
    Private Shared RunningThreads As New List(Of IGatewayAdapter)


    Public Shared Sub Main(ByVal args As String())

        AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf CurrentDomain_ProcessExit
        AddHandler AssemblyLoadContext.Default.Unloading, AddressOf Default_Unloading
        AddHandler Console.CancelKeyPress, AddressOf OnExit

        Dim serviceCollection = New ServiceCollection()
        ConfigureServices(serviceCollection)
        Dim serviceProvider = serviceCollection.BuildServiceProvider()
        _logger = serviceProvider.GetService(Of ILogger(Of Program))()
        _logger.LogInformation("Gateway2mqtt is started")

        Dim rflink As IGatewayAdapter = serviceProvider.GetService(Of RfLinkAdapter)
        rflink.ServiceProvider = serviceProvider

        'RunningThreads.Add(rflink)
        'rflink.StartAdapter()
        'AddHandler rflink.DataRecieved, AddressOf DataProcessor
        'AddHandler rflink.ConnectionState, AddressOf GatewayConnectionState

        '_closing.WaitOne()
        'RunningThreads.ForEach(Sub(x) x.StopAdapter())

        Console.ReadKey()

    End Sub

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
                .Configure(New Action(Of LoggerFilterOptions)(Sub(x) x.MinLevel = _Loglevel)) _
                .AddSingleton(Of RfLinkAdapter).AddSingleton(Of HomeAssistantBinder).AddTransient(Of MessageConverter) _
        .AddSingleton(Of IConfig, Config).AddSingleton(Of IRFLinkConfig, RFLinkConfig)
    End Sub

    Private Shared Sub Default_Unloading(ByVal obj As AssemblyLoadContext)
        Console.WriteLine("unload")
        _closing.Set()
    End Sub

    Private Shared Sub CurrentDomain_ProcessExit(ByVal sender As Object, ByVal e As EventArgs)
        Console.WriteLine("process exit")
        _closing.Set()
    End Sub

    Private Shared Sub OnExit(ByVal sender As Object, ByVal args As ConsoleCancelEventArgs)
        _logger.LogInformation("OnExit is triggered. Initiating shutdown")
        args.Cancel = True
        _closing.Set()
    End Sub

End Class

