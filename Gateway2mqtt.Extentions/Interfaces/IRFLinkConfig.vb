Namespace Interfaces
    Public Interface IRfLinkConfig
        Inherits IConfig
        ReadOnly Property RflinkIgnoredDevices As List(Of String)
        ReadOnly Property RflinkOutputParamsProcessing As Dictionary(Of String, List(Of String))
        ReadOnly Property RflinkTtyDevice As String
    End Interface
End NameSpace