Imports com.magpern.gateway2mqtt.Extentions.Interfaces
Imports FluentAssertions
Imports Microsoft.Extensions.Logging
Imports Moq
Imports NUnit.Framework

<TestFixture>
Public Class RfLinkAdapterTest

    <SetUp>
    Public Sub SetUp_MessageConverter()
        MessageConverter.Config = TestHelper.CreateMockConfig.Object
        Dim logger = Mock.Of(Of ILogger)
        MessageConverter.Logger = logger
    End Sub

    <TestCase>
    Public Async Function MessageToMqttMessage_Button_click_message() As Task
        'Arrange
        Dim rflinkadapter As New RfLinkAdapter(Mock.Of(Of ILogger(of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        Dim message as List(of Dictionary(Of String, String)) = MessageConverter.DecodeRawMessage("20;06;Kaku;ID=41;SWITCH=1;CMD=ON;")

        'Act
        Dim res = Await RfLinkAdapter.MessageToMqttMessage(message(0)) 

        'Assert
        Assert.IsNotNull(rflinkadapter)
        assert.IsNotNull(res)
        res.Payload.Should.Be("{""CMD"": ON}")
        res.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/Kaku/41/R/1/message")
    End Function

    <TestCase>
    Public Async Function MessageToMqttMessage_Button_click_message_no_family() As Task
        'Arrange
        Dim rflinkadapter As New RfLinkAdapter(Mock.Of(Of ILogger(of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        Dim message as List(of Dictionary(Of String, String)) = MessageConverter.DecodeRawMessage("20;06;;ID=41;SWITCH=1;CMD=ON;")

        'Act
        Dim res = Await RfLinkAdapter.MessageToMqttMessage(message(0)) 

        'Assert
        Assert.IsNotNull(rflinkadapter)
        assert.IsNotNull(res)
        res.Payload.Should.Be("{""CMD"": ON}")
        res.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/_COMMAND/OUT")
    End Function
End Class