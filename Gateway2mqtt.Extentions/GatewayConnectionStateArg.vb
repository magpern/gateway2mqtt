
Public Class GatewayConnectionStateArg
    Inherits EventArgs

    Sub New(state As ConnectionState)
        Me.State = state
    End Sub

    Public ReadOnly Property State As ConnectionState
End Class
