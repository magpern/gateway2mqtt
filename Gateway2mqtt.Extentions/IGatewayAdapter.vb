Imports Microsoft.Extensions.DependencyInjection

Public Interface IGatewayAdapter
    Property ServiceProvider As ServiceProvider
    Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg)
    Event DataRecieved(sender As IGatewayAdapter, e As GatewayDataRecievedArg)

    Function RequestGatewayVerionAsync() As Task(Of IgatewayVersion)
    Function DataSendAsync(dataPayload As DataPayload) As Task
    Function StartAdapter() As Task
    Sub StopAdapter()

End Interface
