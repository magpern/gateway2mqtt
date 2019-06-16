Imports com.magpern.gateway2mqtt.Extentions
Imports NUnit.Framework

<TestFixture>
Public Class SerialConnectionHelperTest

    <TestCase(SerialConnectionHelper.SerialStopBits.None, ExpectedResult:=IO.Ports.StopBits.None)>
    <TestCase(SerialConnectionHelper.SerialStopBits.One, ExpectedResult:=IO.Ports.StopBits.One)>
    <TestCase(SerialConnectionHelper.SerialStopBits.OnePointFive, ExpectedResult:=IO.Ports.StopBits.OnePointFive)>
    <TestCase(SerialConnectionHelper.SerialStopBits.Two, ExpectedResult:=IO.Ports.StopBits.Two)>
    Public Function MapStopBitsTest(bit As SerialConnectionHelper.SerialStopBits) As IO.Ports.StopBits
        Return SerialConnectionHelper.MapStopBits(bit)
    End Function

    <TestCase(SerialConnectionHelper.SerialParity.None, ExpectedResult:=IO.Ports.Parity.None)>
    <TestCase(SerialConnectionHelper.SerialParity.Even, ExpectedResult:=IO.Ports.Parity.Even)>
    <TestCase(SerialConnectionHelper.SerialParity.Mark, ExpectedResult:=IO.Ports.Parity.Mark)>
    <TestCase(SerialConnectionHelper.SerialParity.Odd, ExpectedResult:=IO.Ports.Parity.Odd)>
    <TestCase(SerialConnectionHelper.SerialParity.Space, ExpectedResult:=IO.Ports.Parity.Space)>
    Public Function MapParityTest(par As SerialConnectionHelper.SerialParity) As IO.Ports.Parity
        Return SerialConnectionHelper.MapParity(par)
    End Function

End Class
