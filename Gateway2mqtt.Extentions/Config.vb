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

    Public Sub New(configFileName As String)
        If String.IsNullOrEmpty(configFileName) Then configFileName = "config.xml"
        Try
            ConfigData = XElement.Load(configFileName)
        Catch
            ConfigData = nothing
        End Try

        If ConfigData IsNot Nothing Then
            MqttHost = If(ConfigData.Element("mqtt_host") IsNot Nothing, ConfigData.Element("mqtt_host").Value, "mqtt.home")
            MqttPort = If(ConfigData.Element("mqtt_port") IsNot Nothing, CType(ConfigData.Element("mqtt_port").Value, Integer), 1883)
            MqttUser = If(ConfigData.Element("mqtt_user") IsNot Nothing, ConfigData.Element("mqtt_user").Value, String.Empty)
            MqttPassword = If(ConfigData.Element("mqtt_password") IsNot Nothing, ConfigData.Element("mqtt_password").Value, String.Empty)
            MqttPrefix = If(ConfigData.Element("mqtt_prefix") IsNot Nothing, ConfigData.Element("mqtt_prefix").Value, "myprefix")
            MqttMessageTimeout = If(ConfigData.Element("mqtt_message_timeout") IsNot Nothing, CType(ConfigData.Element("mqtt_message_timeout").Value, Integer), 60)
            MqttSwitchInclTopic = (ConfigData.Element("mqtt_switch_incl_topic") Is Nothing) OrElse CType(ConfigData.Element("mqtt_switch_incl_topic").Value, Boolean)
            MqttJson = (ConfigData.Element("mqtt_json") Is Nothing) OrElse CType(ConfigData.Element("mqtt_json").Value, Boolean)
            MqttIncludeMessage = (ConfigData.Element("mqtt_include_message") IsNot Nothing) AndAlso CType(ConfigData.Element("mqtt_include_message").Value, Boolean)
        Else
            MqttHost = "mqtt.home"
            MqttPort = 1883
            MqttUser = String.Empty
            MqttPassword = String.Empty
            MqttPrefix = "myprefix"
            MqttMessageTimeout = 60
            MqttSwitchInclTopic = True
            MqttJson = True
            MqttIncludeMessage = False
        End If
    End Sub
End Class