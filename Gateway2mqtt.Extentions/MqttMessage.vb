
Imports com.magpern.gateway2mqtt.Extentions.Interfaces

public Class MqttMessage
    Implements IMQTTMessage

    Public Property Topic As String Implements IMqttMessage.Topic
    Public Property Payload As String Implements IMqttMessage.Payload

    Private Overloads Function ToString() As String Implements IMqttMessage.ToString 
        Return $"Topic: '{Topic}' : Payload: '{Payload}'"
    End Function
End Class