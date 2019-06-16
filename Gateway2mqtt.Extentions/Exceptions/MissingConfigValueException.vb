﻿Imports System.Diagnostics.CodeAnalysis
Imports System.Runtime.Serialization

Namespace Exceptions
    <ExcludeFromCodeCoverage>
    <Serializable>
    Public Class MissingConfigValueException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End NameSpace