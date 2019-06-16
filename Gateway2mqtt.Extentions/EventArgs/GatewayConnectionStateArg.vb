Imports System.Diagnostics.CodeAnalysis

Namespace EventArgs

    <ExcludeFromCodeCoverage>
    Public Class GatewayConnectionStateArg
        Inherits System.EventArgs

        Sub New(state As ConnectionState)
            Me.State = state
        End Sub

        Public ReadOnly Property State As ConnectionState
    End Class
End NameSpace