Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.Interfaces

Public Class RFLinkConfig
    Inherits Config
    Implements IRFLinkConfig

    Public ReadOnly Property RflinkIgnoredDevices As List(Of String) Implements IRFLinkConfig.RflinkIgnoredDevices

    Public ReadOnly Property RflinkOutputParamsProcessing As Dictionary(Of String, List(Of String)) _
        Implements IRFLinkConfig.RflinkOutputParamsProcessing

    Public ReadOnly Property RflinkTtyDevice As String Implements IRFLinkConfig.RflinkTtyDevice

    Public Sub New()
        RflinkTtyDevice = ConfigData.Element("rflink_tty_device").value

        Dim ignoreDeviceList = ConfigData.Elements("rflink_ignored_devices")
        RflinkIgnoredDevices = New List(Of String)
        If ignoreDeviceList.Count > 0 Then
            For Each e As XElement In ignoreDeviceList
                RflinkIgnoredDevices.Add(e.Value)
            Next
        End If

        Dim rflinkParamsProcessing = ConfigData.Elements("rflink_output_params_processing")
        RflinkOutputParamsProcessing = New Dictionary(Of String, List(Of String))

        If rflinkParamsProcessing.Count > 0 Then
            For Each e As XElement In rflinkParamsProcessing.Elements
                Dim key As String = e.Name.LocalName
                Dim processingList As New List(Of String)
                For Each el As XElement In e.Elements
                    processingList.Add(el.Value)
                Next
                If processingList.Count > 0 Then
                    RflinkOutputParamsProcessing.Add(key, processingList)
                End If
            Next
        End If
    End Sub
End Class
