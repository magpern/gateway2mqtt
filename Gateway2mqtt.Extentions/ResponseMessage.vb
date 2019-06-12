Public Class ResponseMessage
    Implements IDeviceMessage

    Public Property device_id As String
    Public Property family As String
    Public Property action As String
    Public Property topic As String
    Public Property param As String
    Public Property payload As String
    Public Property qos As Integer
    Public Property timestamp As Double
End Class
