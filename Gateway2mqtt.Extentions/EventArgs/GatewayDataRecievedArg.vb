Imports System.Diagnostics.CodeAnalysis
Imports com.magpern.gateway2mqtt.Extentions.Interfaces

Namespace EventArgs
    <ExcludeFromCodeCoverage>
    Public Class GatewayDataRecievedArg
        Inherits System.EventArgs

        Property Payload As IMqttMessage 
    End Class
End NameSpace