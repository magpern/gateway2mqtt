Imports Microsoft.Extensions.Logging

Public Interface IConfig

    ReadOnly Property mqtt_host As String
    ReadOnly Property mqtt_port As Integer
    ReadOnly Property mqtt_user As String
    ReadOnly Property mqtt_password As String
    ReadOnly Property mqtt_prefix As String
    ReadOnly Property mqtt_message_timeout As Integer
    ReadOnly Property mqtt_switch_incl_topic As Boolean
    ReadOnly Property mqtt_json As Boolean
    ReadOnly Property mqtt_include_message As Boolean

End Interface