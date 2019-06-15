
Namespace Interfaces
    Public Interface IConfig
        ReadOnly Property MqttHost As String
        ReadOnly Property MqttPort As Integer
        ReadOnly Property MqttUser As String
        ReadOnly Property MqttPassword As String
        ReadOnly Property MqttPrefix As String
        ReadOnly Property MqttMessageTimeout As Integer
        ReadOnly Property MqttSwitchInclTopic As Boolean
        ReadOnly Property MqttJson As Boolean
        ReadOnly Property MqttIncludeMessage As Boolean
    End Interface
End NameSpace