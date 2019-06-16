Imports System.IO.Ports
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace AsyncExtensions
    Public Module SerialPortExtensions
        <Extension>
        Async Function ReadLineAsync(serialPort As SerialPort) As Task(Of String)
            Dim buffer = New Byte(0) {}
            Dim ret As String = String.Empty
            Dim encoder As New ASCIIEncoding()

            While True
                Await serialPort.BaseStream.ReadAsync(buffer, 0, 1)
                ret += encoder.GetString(buffer)
                Debug.Print(ret)
                If ret.EndsWith(serialPort.NewLine) Then Return ret.Substring(0, ret.Length - serialPort.NewLine.Length)
            End While
        End Function

        <Extension>
        Async Function WriteLineAsync(serialPort As SerialPort, str As String) As Task
            Dim encodedStr As Byte() = serialPort.Encoding.GetBytes(str & serialPort.NewLine)
            Await serialPort.BaseStream.WriteAsync(encodedStr, 0, encodedStr.Length)
            Await serialPort.BaseStream.FlushAsync()
        End Function
    End Module
End Namespace
