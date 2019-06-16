Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.Exceptions
Imports com.magpern.gateway2mqtt.Extentions.Interfaces

Public Class RfLinkConfig
    Inherits Config
    Implements IRFLinkConfig

    Public ReadOnly Property RflinkIgnoredDevices As List(Of String) Implements IRFLinkConfig.RflinkIgnoredDevices

    Public ReadOnly Property RflinkOutputParamsProcessing As Dictionary(Of String, List(Of String)) _
        Implements IRFLinkConfig.RflinkOutputParamsProcessing

    Public ReadOnly Property RflinkTtyDevice As String Implements IRFLinkConfig.RflinkTtyDevice
    Public ReadOnly Property Rflinkheartbeat As Boolean Implements IRfLinkConfig.Rflinkheartbeat
    Public ReadOnly Property RflinkHeartbeatInterval As Integer Implements IRfLinkConfig.RflinkHeartbeatInterval

    Public Sub New
        MyClass.New(string.Empty)
    End Sub

    Public Sub New(configfile As string)
        MyBase.New(configfile)
        Try
            RflinkTtyDevice = ConfigData.Element("rflink_tty_device").Value
        Catch e As Exception
            Throw New MissingConfigValueException("rflink_tty_device", e)
        End Try

        Dim ignoreDeviceList As IEnumerable(Of XElement)
        Try
            ignoreDeviceList = ConfigData.Elements("rflink_ignored_devices")
        Catch
            ignoreDeviceList = New List(Of XElement)
        End Try

        RflinkIgnoredDevices = New List(Of String)
        If ignoreDeviceList.Count > 0 Then
            For Each e As XElement In ignoreDeviceList
                RflinkIgnoredDevices.Add(e.Value)
            Next
        End If

        Dim rflinkParamsProcessing As IEnumerable(Of XElement)
        Try
            rflinkParamsProcessing = ConfigData.Elements("rflink_output_params_processing")
        Catch
            rflinkParamsProcessing = New List(Of XElement)
        End Try

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

        Rflinkheartbeat = (ConfigData.Element("rflink_heartbeat") Is Nothing) OrElse CType(ConfigData.Element("rflink_heartbeat").Value, Boolean)
        Try
            RflinkHeartbeatInterval = If(ConfigData.Element("rflink_heartbeat_interval") IsNot Nothing, CType(ConfigData.Element("rflink_heartbeat_interval").Value, Integer) * 1000, 60000)
        Catch e As Exception
            Throw new MissingConfigValueException($"rflink_heartbeat_interval {RflinkHeartbeatInterval}", e)
        End Try

    End Sub
End Class
