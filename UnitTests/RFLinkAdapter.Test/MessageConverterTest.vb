
Imports com.magpern.gateway2mqtt.Exceptions
Imports FluentAssertions
Imports Microsoft.Extensions.Logging
Imports Moq
Imports NUnit.Framework

<TestFixture>
Public Class MessageConverterTest
    <SetUp>
    Public Sub SetUp_MessageConverter()
        MessageConverter.Config = TestHelper.CreateMockConfig.Object
        Dim logger = Mock.Of(Of ILogger)
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