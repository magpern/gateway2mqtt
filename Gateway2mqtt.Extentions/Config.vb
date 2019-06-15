Imports com.magpern.gateway2mqtt.Extentions.Interfaces

Public Class Config
    Implements IConfig

    Public ReadOnly Property MqttHost As String Implements IConfig.MqttHost
    Public ReadOnly Property MqttPort As Integer Implements IConfig.MqttPort
    Public ReadOnly Property MqttUser As String Implements IConfig.MqttUser
    Public ReadOnly Property MqttPassword As String Implements IConfig.MqttPassword
    Public ReadOnly Property MqttPrefix As String Implements IConfig.MqttPrefix
    Public ReadOnly Property MqttMessageTimeout As Integer Implements IConfig.MqttMessageTimeout
    Public ReadOnly Property MqttSwitchInclTopic As Boolean Implements IConfig.MqttSwitchInclTopic
    Public ReadOnly Property MqttJson As Boolean Implements IConfig.MqttJson
    Public ReadOnly Property MqttIncludeMessage As Boolean Implements IConfig.MqttIncludeMessage

    Protected ConfigData As XElement

    Public Sub New()
        ConfigData = XElement.Load("config.xml")

        MqttHost = ConfigData.Element("mqtt_host").Value
        MqttPort = CType(ConfigData.Element("mqtt_port").Value, Integer)
        MqttUser = ConfigData.Element("mqtt_user").Value
        MqttPassword = ConfigData.Element("mqtt_password").Value
        MqttPrefix = ConfigData.Element("mqtt_prefix").Value
        MqttMessageTimeout = CType(ConfigData.Element("mqtt_message_timeout").Value, Integer)
        MqttSwitchInclTopic = CType(ConfigData.Element("mqtt_switch_incl_topic").Value, Boolean)
        MqttJson = CType(ConfigData.Element("mqtt_json").Value, Boolean)
        MqttIncludeMessage = CType(ConfigData.Element("mqtt_include_message").Value, Boolean)
    End Sub
End Class