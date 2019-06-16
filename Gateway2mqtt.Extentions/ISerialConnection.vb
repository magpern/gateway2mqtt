Imports System.IO.Ports

Public interface ISerialConnection
    Property ComPort As SerialPort
    Property DoContinue As Boolean
    Property Port As String 
    Property BaudRate As Integer
    Property DataBit As Integer 
    Property Parity As SerialConnectionHelper.SerialParity 
    Property StopBit As SerialConnectionHelper.SerialStopBits
    Property ReadTimeout As Integer
    Property WriteTimeout As Integer
end interface