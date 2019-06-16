
Imports com.magpern.gateway2mqtt.Extentions.EventArgs

Namespace Interfaces
    Public Interface IGatewayAdapter
        'Property ServiceProvider As ServiceProvider
        Event ConnectionState(sender As IGatewayAdapter, e As GatewayConnectionStateArg)
        Event DataReceived(sender As IGatewayAdapter, e As GatewayDataRecievedArg)

        'Function RequestGatewayVerionAsync() As Task(Of IgatewayVersion)
        Function StartAdapter() As Task
        Sub StopAdapter()
    End Interface
End NameSpace