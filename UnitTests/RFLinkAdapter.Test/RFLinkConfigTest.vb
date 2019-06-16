Imports com.magpern.gateway2mqtt.Extentions.Exceptions
Imports FluentAssertions
Imports NUnit.Framework

<TestFixture>
Public Class RfLinkConfigTest

    <TestCase>
    Public sub ConstructorTest_missing_configfile
        Assert.That(Function() new RfLinkConfig("missingconfig.xml"),
                    Throws.Exception.TypeOf(Of MissingConfigValueException))
    End sub

    <TestCase>
    Public sub ConstructorTest_partial_config_file
        'Act
        Dim obj = new RfLinkConfig("config2.xml")

        'Assert
        Assert.IsNotNull(obj)
        obj.MqttPassword.Should.BeEmpty 
        obj.MqttUser.Should.BeEmpty 
    End sub

    <TestCase>
    Public sub ConstructorTest_full_config_file
        'Act
        Dim obj = new RfLinkConfig

        'Assert
        Assert.IsNotNull(obj)
        obj.MqttPort.Should.Be(1883)
    End sub
End Class
