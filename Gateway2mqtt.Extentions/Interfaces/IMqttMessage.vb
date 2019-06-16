Namespace Interfaces
    Public Interface IMqttMessage
        Property Topic As String
        Property Payload As String
        Function ToString As String
    End Interface
End NameSpace