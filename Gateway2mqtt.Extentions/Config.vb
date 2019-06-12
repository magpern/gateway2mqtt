Public Class Config
    Implements IConfig

    Public ReadOnly Property mqtt_host As String Implements IConfig.mqtt_host
    Public ReadOnly Property mqtt_port As Integer Implements IConfig.mqtt_port
    Public ReadOnly Property mqtt_user As String Implements IConfig.mqtt_user
    Public ReadOnly Property mqtt_password As String Implements IConfig.mqtt_password
    Public ReadOnly Property mqtt_prefix As String Implements IConfig.mqtt_prefix
    Public ReadOnly Property mqtt_message_timeout As Integer Implements IConfig.mqtt_message_timeout
    Public ReadOnly Property mqtt_switch_incl_topic As Boolean Implements IConfig.mqtt_switch_incl_topic
    Public ReadOnly Property mqtt_json As Boolean Implements IConfig.mqtt_json
    Public ReadOnly Property mqtt_include_message As Boolean Implements IConfig.mqtt_include_message

    Protected ConfigData As XElement

    Public Sub New()
        ConfigData = XElement.Load("config.xml")

        mqtt_host = ConfigData.Element("mqtt_host").Value
        mqtt_port = CType(ConfigData.Element("mqtt_port").Value, Integer)
        mqtt_user = ConfigData.Element("mqtt_user").Value
        mqtt_password = ConfigData.Element("mqtt_password").Value
        mqtt_prefix = ConfigData.Element("mqtt_prefix").Value
        mqtt_message_timeout = CType(ConfigData.Element("mqtt_message_timeout").Value, Integer)
        mqtt_switch_incl_topic = CType(ConfigData.Element("mqtt_switch_incl_topic").Value, Boolean)
        mqtt_json = CType(ConfigData.Element("mqtt_json").Value, Boolean)
        mqtt_include_message = CType(ConfigData.Element("mqtt_include_message").Value, Boolean)

    End Sub


End Class