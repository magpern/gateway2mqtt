Imports com.magpern.gateway2mqtt.Extentions
Imports com.magpern.gateway2mqtt.Extentions.EventArgs
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
    Public Sub ProcessMessage_Button_click_message()
        'Setup
        Dim mqttMessage As IMqttMessage = Nothing
        Dim hitCount as Integer 
        'Arrange
        Dim rflink As New RfLinkAdapter(Mock.Of(Of ILogger(Of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        AddHandler rflink.DataReceived, Sub(sender As IGatewayAdapter, e As GatewayDataRecievedArg)
            mqttMessage = e.Payload.Message 
            hitCount += 1
        End Sub
        'Act
        rflink.ProcessMessage("20;06;Kaku;ID=41;SWITCH=1;CMD=ON;")

        'Assert
        Assert.NotNull(mqttMessage)
        hitCount.Should.Be(1)
        mqttMessage.Payload.Should.Be("{""CMD"": ON}")
        mqttMessage.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/Kaku/41/R/1/message")
    End Sub

    <TestCase>
    Public Async Function ProcessMessageAsync_Button_click_message() As Task
        'Setup
        Dim mqttMessage As IMqttMessage = Nothing
        Dim hitCount as Integer 
        'Arrange
        Dim rflink As New RfLinkAdapter(Mock.Of(Of ILogger(Of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        AddHandler rflink.DataReceived, Sub(sender As IGatewayAdapter, e As GatewayDataRecievedArg)
            mqttMessage = e.Payload.Message 
            hitCount += 1
        End Sub
        'Act
        Await rflink.ProcessMessageAsync("20;06;Kaku;ID=41;SWITCH=1;CMD=ON;")

        'Assert
        Assert.NotNull(mqttMessage)
        hitCount.Should.Be(1)
        mqttMessage.Payload.Should.Be("{""CMD"": ON}")
        mqttMessage.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/Kaku/41/R/1/message")
    End Function

    <TestCase>
    Public Async Function MessageToMqttMessage_Button_click_message() As Task
        'Arrange
        Dim rflinkadapter As New RfLinkAdapter(Mock.Of(Of ILogger(Of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        Dim message As List(Of Dictionary(Of String, String)) = MessageConverter.DecodeRawMessage("20;06;Kaku;ID=41;SWITCH=1;CMD=ON;")

        'Act
        Dim res = Await RfLinkAdapter.MessageToMqttMessage(message(0))

        'Assert
        Assert.IsNotNull(rflinkadapter)
        Assert.IsNotNull(res)
        res.Payload.Should.Be("{""CMD"": ON}")
        res.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/Kaku/41/R/1/message")
    End Function

    <TestCase>
    Public Async Function MessageToMqttMessage_Button_click_message_no_family() As Task
        'Arrange
        Dim rflinkadapter As New RfLinkAdapter(Mock.Of(Of ILogger(Of IGatewayAdapter)), TestHelper.CreateMockConfig.Object)
        Dim message As List(Of Dictionary(Of String, String)) = MessageConverter.DecodeRawMessage("20;06;;ID=41;SWITCH=1;CMD=ON;")

        'Act
        Dim res = Await RfLinkAdapter.MessageToMqttMessage(message(0))

        'Assert
        Assert.IsNotNull(rflinkadapter)
        Assert.IsNotNull(res)
        res.Payload.Should.Be("{""CMD"": ON}")
        res.Topic.Should.Be($"{RfLinkAdapter.Config.MqttPrefix}/_COMMAND/OUT")
    End Function

End Class
