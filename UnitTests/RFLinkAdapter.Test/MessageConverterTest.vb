﻿Imports com.magpern.gateway2mqtt
Imports com.magpern.gateway2mqtt.Exceptions
Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports FluentAssertions
Imports Microsoft.Extensions.Logging
Imports Moq
Imports NUnit.Framework

<TestFixture>
Public Class MessageConverterTest
    <SetUp>
    Public Sub SetUp_MessageConverter()
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

        MessageConverter.Config = config.Object

        Dim logger = Mock.Of (Of ILogger)
        MessageConverter.Logger = logger
    End Sub

    <TestCase>
    Public Sub Temp_and_Humidity_message_from_RFLink_to_mqtt2gateway_json()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Const message = "20;3D;Alecto V1;ID=2000;TEMP=0011;HUM=61;"
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        MessageConverter.Config.MqttJson.Should.BeTrue
        result.Count.Should.Be(1)
        result(0)("device_id").should.Be("2000")
        result(0)("payload").Should.Contain("1.7").And.Contain("61")
    End Sub

    <Test>
    <TestCase("20;00;Nodo RadioFrequencyLink - RFLink Gateway V1.1 - R46;")>
    <TestCase("Junk data input")>
    Public sub Presentation_message_from_RFLink_to_mqtt2gateway(message As string)
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        Assert.IsNull(result)
    End sub

    <TestCase>
    Public Sub Ping_message_from_RFLink_to_mqtt2gateway()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Const message = "20;99;PONG;"
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        result.Count.Should.Be(1)
        result(0)("action").should.Be("SCC")
    End Sub


    <TestCase>
    Public Sub Temp_and_Humidity_message_from_RFLink_to_mqtt2gateway_no_json()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Dim cnf = Mock.Get(MessageConverter.Config)
        cnf.Setup(Function(s) s.MqttJson).Returns(False)

        Const message = "20;3D;Alecto V1;ID=2000;TEMP=0011;HUM=61;"
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        MessageConverter.Config.MqttJson.Should.BeFalse
        result.Count.Should.Be(2)
        result(0)("device_id").should.Be("2000")
        result(0)("payload").Should.Contain("1.7")
        result(1)("payload").Should.Contain("61")
        result(0)("payload").Should.NotContain("message")
    End Sub

    <TestCase>
    Public Sub Switch_message_from_RFLink_to_mqtt2gateway_json()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Const message = "20;06;Kaku;ID=41;SWITCH=1;CMD=ON;"
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        result.Count.Should.Be(1)
        result(0)("device_id").should.Be("41")
        result(0)("payload").Should.Contain("CMD").And.Contain("ON")
    End Sub

    <TestCase>
    Public Sub Switch_message_from_RFLink_to_mqtt2gateway_no_json()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Dim cnf = Mock.Get(MessageConverter.Config)
        cnf.Setup(Function(s) s.MqttJson).Returns(False)

        Const message = "20;06;Kaku;ID=41;SWITCH=1;CMD=ON;"
        'Act
        dim result = MessageConverter.DecodeRawMessage(message)

        'Assess
        result.Count.Should.Be(1)
        result(0)("device_id").should.Be("41")
        result(0)("payload").Should.Contain("ON")
    End Sub

    <TestCase>
    Public Sub Message_from_RFLink_with_ignore_devices()
        'Arrange
        'Setup of Shared/Static objects in MessageConverter is made in <SetUp>
        Dim cnf = Mock.Get(MessageConverter.Config)
        dim ignoreDevice = New list(Of String) From { "Alecto V1",
                "Oregon Temp/FD10",
                "BL999",
                "RTS"}
        cnf.Setup(function(s) s.RflinkIgnoredDevices).Returns(ignoreDevice)

        Const message = "20;3D;Alecto V1;ID=2000;TEMP=0011;HUM=61;"
        Const message2 = "20;3D;RTS;ID=2000;TEMP=0011;HUM=61;"
        'Act

        'Assess
        Assert.That(Function() MessageConverter.DecodeRawMessage(message),
                    Throws.Exception.TypeOf (Of DeviceIgnoredException))
        Assert.That(Function() MessageConverter.DecodeRawMessage(message2),
                    Throws.Exception.TypeOf (Of DeviceIgnoredException))
    End Sub
End Class