Imports System.Globalization
Imports FluentAssertions
Imports NUnit.Framework

<TestFixture>
Public Class ProcessorsTest
    <TestCase>
    Public Sub UvMappingTest()
        'Arrange
        'Act
        'assert
        Processors.UvMapping(0).Should.be("LOW")
        Processors.UvMapping(4).Should.be("MED")
        Processors.UvMapping(7).Should.be("HI")
        Processors.UvMapping(9).Should.be("V.HI")
        Processors.UvMapping(2000).Should.be("EX.HI")
        Processors.UvMapping(10000).Should.be("Undef")
        Processors.UvMapping(- 1).Should.be("Undef")
    End Sub

    <TestCase>
    Public Sub WindMappingTest()
        'Arrange
        'Act
        'assert
        Processors.WindMapping(1).Should.be("LIGHT")
        Processors.WindMapping(30).Should.be("MODERATE")
        Processors.WindMapping(60).Should.be("STRONG")
        Processors.WindMapping(120).Should.be("STORM")
        Processors.WindMapping(999).Should.be("STORM")
        Processors.WindMapping(1000).Should.be("Undef")
        Processors.WindMapping(- 1).Should.be("Undef")
    End Sub

    <TestCase>
    Public Sub Hex2DecTest()
        'Arrange
        'Act
        'assert
        Processors.Hex2dec("-10").Should.be("-10")
        Processors.Hex2dec("10").Should.be("16")
        Processors.Hex2dec("A0").Should.be("160")
        Processors.Hex2dec("TEST").Should.be("TEST")
    End Sub

    <TestCase>
    Public Sub Shex2decTest()
        'Arrange
        'Act
        'assert
        Processors.SignedHex2dec("1000").Should.be("4096")
        Processors.SignedHex2dec("FFFF").Should.be("-32767")
        Processors.SignedHex2dec("8000").Should.be("0")
        Processors.SignedHex2dec("KABA").Should.be("KABA")
    End Sub

    <TestCase>
    Public Sub Str2decTest()
        'Arrange
        'Act
        'assert
        Processors.Str2dec("10").Should.be("10")
        Processors.Str2dec("ABAB").Should.be("ABAB")
    End Sub

    <TestCase>
    Public Sub Div10Test()
        'Arrange
        Dim sep = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator
        'Act
        'assert
        Processors.Div10("10").Should.be("1")
        Processors.Div10("120").Should.be("12")
        Processors.Div10("124").Should.Be($"12{sep}4")
        Processors.Div10("TEST").Should.be("TEST")
    End Sub

    <TestCase>
    Public Sub Dir2degTest()
        'Arrange
        Dim sep = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator
        'Act
        'assert
        Processors.Dir2deg("0").Should.be("0")
        Processors.Dir2deg("15").Should.be($"337{sep}5")
        Processors.Dir2deg("2").Should.Be("45")
        Processors.Dir2deg("16").Should.Be("Undef")
        Processors.Dir2deg("ABBA").Should.Be("Undef")
    End Sub

    <TestCase>
    Public Sub Dir2CarTest()
        'Arrange
        'Act
        'assert
        Processors.Dir2Car("0").Should.be("N")
        Processors.Dir2Car("15").Should.be("NNW")
        Processors.Dir2Car("2").Should.Be("NE")
        Processors.Dir2Car("16").Should.Be("Undef")
    End Sub

    <TestCase>
    Public Sub Uv2LevelTest()
        'Arrange
        'Act
        'assert
        Processors.Uv2Level("0").Should.be("LOW")
        Processors.Uv2Level("4").Should.be("MED")
        Processors.Uv2Level("7").Should.be("HI")
        Processors.Uv2Level("9").Should.be("V.HI")
        Processors.Uv2Level("2000").Should.be("EX.HI")
        Processors.Uv2Level("10000").Should.be("Undef")
        Processors.Uv2Level("-1").Should.be("Undef")
    End Sub

    <TestCase>
    Public Sub Wind2LevelTest()
        'Arrange
        'Act
        'assert
        Processors.Wind2Level("1").Should.be("LIGHT")
        Processors.Wind2Level("30").Should.be("MODERATE")
        Processors.Wind2Level("60").Should.be("STRONG")
        Processors.Wind2Level("120").Should.be("STORM")
        Processors.Wind2Level("999").Should.be("STORM")
        Processors.Wind2Level("1000").Should.be("Undef")
        Processors.Wind2Level("-1").Should.be("Undef")
    End Sub

    <TestCase>
    Public Sub ProcessorsTest()
        'Arrange
        Dim div10 = Processors.Processors("div10")
        Dim wind2Level = Processors.Processors("wind2level")
        'Act
        Dim div10Res = div10("30")
        Dim wind2LevelRes = wind2Level("120")
        'assert
        div10Res.Should.Be("3")
        wind2LevelRes.Should.Be("STORM")
    End Sub
End Class


