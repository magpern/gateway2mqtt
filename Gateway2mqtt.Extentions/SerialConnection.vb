Imports System.IO.Ports

Public Class SerialConnection

    Protected comPort As SerialPort

    Protected _continue As Boolean

    Property Port As String = "COM6"
    Property BaudRate As Integer = 57600
    Property DataBit As Integer = 8
    Property Parity As SerialParity = SerialParity.None
    Property StopBit As SerialStopBits = SerialStopBits.One
    Property ReadTimeout As Integer = 500
    Property WriteTimeout As Integer = 500

    Public Enum SerialParity
        '
        ' Summary:
        '     No parity check occurs.
        None = 0
        '
        ' Summary:
        '     Sets the parity bit so that the count of bits set is an odd number.
        Odd = 1
        '
        ' Summary:
        '     Sets the parity bit so that the count of bits set is an even number.
        Even = 2
        '
        ' Summary:
        '     Leaves the parity bit set to 1.
        Mark = 3
        '
        ' Summary:
        '     Leaves the parity bit set to 0.
        Space = 4
    End Enum

    Public Enum SerialStopBits
        ' <summary>No stop bits are used. This value Is Not supported by the <see cref="P:System.IO.Ports.SerialPort.StopBits"></see> property.</summary>
        ' <returns></returns>
        None
        ' <summary>One stop bit Is used.</summary>
        ' <returns></returns>
        One
        ' <summary>Two stop bits are used.</summary>
        ' <returns></returns>
        Two
        ' <summary>1.5 stop bits are used.</summary>
        ' <returns></returns>
        OnePointFive
    End Enum

    Protected Function MapStopBits(stopBit As SerialStopBits) As StopBits
        Select Case stopBit
            Case SerialStopBits.None
                Return StopBits.None
            Case SerialStopBits.One
                Return StopBits.One
            Case SerialStopBits.OnePointFive
                Return StopBits.OnePointFive
            Case SerialStopBits.Two
                Return StopBits.Two
            Case Else
                Return StopBits.One
        End Select
    End Function

    Protected Function MapParity(parity As SerialParity) As Parity
        Select Case parity
            Case SerialParity.Even
                Return System.IO.Ports.Parity.Even
            Case SerialParity.Mark
                Return System.IO.Ports.Parity.Mark
            Case SerialParity.None
                Return System.IO.Ports.Parity.None
            Case SerialParity.Odd
                Return System.IO.Ports.Parity.Odd
            Case SerialParity.Space
                Return System.IO.Ports.Parity.Space
            Case Else
                Return System.IO.Ports.Parity.None
        End Select
    End Function


End Class

