Imports Moq

Imports com.magpern.gateway2mqtt.Extentions.Interfaces 

Public MustInherit Class TestHelper
    Public Shared Function CreateMockConfig As Mock(Of IRFLinkConfig)
        Dim config = New Mock(Of IRFLinkConfig)()
        config.Setup(function(s) s.MqttHost).Returns("mqtt.home")
        config.Setup(Function(s) s.MqttPort).Returns(1883)
        config.Setup(Function(s) s.MqttUser).Returns("user")
        config.Setup(Function(s) s.MqttPassword).Returns("password")
        config.Setup(Function(s) s.MqttPrefix).Returns("rflink")
        config.Setup(Function(s) s.MqttMessageTimeout).Returns(60)
        config.Setup(Function(s) s.MqttSwitchInclTopic).Returns(True)
        config.Setup(Function(s) s.MqttJson).Returns(True)
        config.Setup(Function(s) s.MqttIncludeMessage).Returns(False)
        config.Setup(Function(s) s.RflinkTtyDevice).Returns("COM6")

        Dim ropp = New Dictionary(Of String, List(Of String))
        ropp.Add("ID", New List(Of String) From {})
        ropp.Add("SWITCH", New List(Of String) From {})
        ropp.Add("CMD", New List(Of String) From {})

        ropp.Add("SET_LEVEL", New List(Of String) From {"str2dec"})
        ropp.Add("TEMP", New List(Of String) From {"shex2dec", "div10"})
        ropp.Add("HUM", New List(Of String) From {"str2dec"})
        ropp.Add("BARO", New List(Of String) From {"hex2dec"})
        ropp.Add("HSTATUS", New List(Of String) From {"str2dec"})
        ropp.Add("BFORECAST", New List(Of String) From {"str2dec"})

        ropp.Add("LUX", New List(Of String) From {"hex2dec"})
        ropp.Add("BAT", New List(Of String) From {})
        ropp.Add("RAIN", New List(Of String) From {"hex2dec", "div10"})
        ropp.Add("RAINRATE", New List(Of String) From {"hex2dec", "div10"})
        ropp.Add("WINSP", New List(Of String) From {"hex2dec", "div10"})
        ropp.Add("AWINSP", New List(Of String) From {"hex2dec", "div10"})
        ropp.Add("WINGS", New List(Of String) From {"hex2dec", "div10"})
        ropp.Add("WINDIR", New List(Of String) From {"mapdir"})
        ropp.Add("WINCHL", New List(Of String) From {"shex2dec", "div10"})
        ropp.Add("WINTMP", New List(Of String) From {"shex2dec", "div10"})
        ropp.Add("CHIME", New List(Of String) From {"str2dec"})
        ropp.Add("SMOKEALERT", New List(Of String) From {})
        ropp.Add("PIR", New List(Of String) From {})
        ropp.Add("CO2", New List(Of String) From {})
        ropp.Add("SOUND", New List(Of String) From {"str2dec"})
        ropp.Add("KWATT", New List(Of String) From {"hex2dec"})
        ropp.Add("WATT", New List(Of String) From {"hex2dec"})
        ropp.Add("CURRENT", New List(Of String) From {"str2dec"})
        ropp.Add("CURRENT2", New List(Of String) From {"str2dec"})
        ropp.Add("CURRENT3", New List(Of String) From {"str2dec"})
        ropp.Add("DIST", New List(Of String) From {"str2dec"})
        ropp.Add("METER", New List(Of String) From {"str2dec"})
        ropp.Add("VOLT", New List(Of String) From {"str2dec"})
        ropp.Add("RGBW", New List(Of String) From {})
        ropp.Add("message", New List(Of String) From {})
        config.Setup(function(s) s.RflinkOutputParamsProcessing).Returns(ropp)
        Return config
    End Function
End Class