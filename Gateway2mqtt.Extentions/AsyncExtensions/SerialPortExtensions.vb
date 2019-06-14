Imports System.IO.Ports
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace AsyncExtensions
    Public Module SerialPortExtensions
        <Extension()>
        Async Function ReadLineAsync(ByVal serialPort As SerialPort) As Task(Of String)
            Dim buffer As Byte() = New Byte(0) {}
            Dim ret As String = String.Empty
            Dim encoder As New ASCIIEncoding()

            While True
                Await serialPort.BaseStream.ReadAsync(buffer, 0, 1)
                ret += encoder.GetString(buffer)
                If ret.EndsWith(serialPort.NewLine) Then Return ret.Substring(0, ret.Length - serialPort.NewLine.Length)
            End While
            Return String.Empty
        End Function

        <Extension()>
        Async Function WriteLineAsync(ByVal serialPort As SerialPort, ByVal str As String) As Task
            Dim encodedStr As Byte() = serialPort.Encoding.GetBytes(str & serialPort.NewLine)
            Await serialPort.BaseStream.WriteAsync(encodedStr, 0, encodedStr.Length)
            Await serialPort.BaseStream.FlushAsync()
        End Function

        '<Extension()>
        'Async Function SendCommandAsync(ByVal serialPort As SerialPort, ByVal command As String) As Task(Of String)
        '    Await serialPort.WriteLineAsync(command)
        '    Return "hello world" ' Await serialPort.ReadLineAsync()
        'End Function
    End Module
End Namespace
