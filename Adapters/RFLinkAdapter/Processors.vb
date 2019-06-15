
Imports System.Globalization

Public MustInherit Class Processors
    Private Shared ReadOnly CardinalPtsMapping As New Dictionary(Of Integer, String) From {
        {0, "N"},
        {1, "NNE"},
        {2, "NE"},
        {3, "ENE"},
        {4, "E"},
        {5, "ESE"},
        {6, "SE"},
        {7, "SSE"},
        {8, "S"},
        {9, "SSW"},
        {10, "SW"},
        {11, "WSW"},
        {12, "W"},
        {13, "WNW"},
        {14, "NW"},
        {15, "NNW"}
        }

    Public Shared Function UvMapping(value As Integer) As String
        Try
            Return UvMappingRanges.Single(Function(range) range.Key.Contains(value)).Value
        Catch e As Exception
            Return "Undef"
        End Try
    End Function

    Private Shared ReadOnly UvMappingRanges As New Dictionary(Of IEnumerable(Of Integer), String) From {
        {Enumerable.Range(0, 3 - 0), "LOW"},
        {Enumerable.Range(3, 6 - 3), "MED"},
        {Enumerable.Range(6, 8 - 6), "HI"},
        {Enumerable.Range(8, 11 - 8), "V.HI"},
        {Enumerable.Range(11, 10000 - 11), "EX.HI"}
        }

    Public Shared Function WindMapping(value As Integer) As String
        Try
            Return WindMappingRanges.Single(Function(range) range.Key.Contains(value)).Value
        Catch e As Exception
            Return "Undef"
        End Try
    End Function

    Private Shared ReadOnly WindMappingRanges As New Dictionary(Of IEnumerable(Of Integer), String) From {
        {Enumerable.Range(0, 14 - 0), "LIGHT"},
        {Enumerable.Range(14, 42 - 14), "MODERATE"},
        {Enumerable.Range(42, 88 - 42), "STRONG"},
        {Enumerable.Range(88, 1000 - 88), "STORM"}
        }

    Shared Function SignedHex2dec(value As String) As String
        Try
            Dim val As Integer = Convert.ToInt32(value, 16)

            If val >= 32768 Then
                val = - 1*(val - 32768)
            End If

            Return val.ToString("G", CultureInfo.InvariantCulture)
        Catch
            Return value
        End Try
    End Function

    Shared Function Hex2dec(value As String) As String
        Try
            Return Convert.ToInt32(value, 16).ToString("G", CultureInfo.InvariantCulture)
        Catch
            Return value
        End Try
    End Function

    'This function is really useless, but kept for compatibility
    Shared Function Str2dec(value As String) As String
        Try
            Return Convert.ToInt32(value).ToString("G", CultureInfo.InvariantCulture)
        Catch
            Return value
        End Try
    End Function

    Shared Function Div10(value As String) As String
        Try
            Return (Convert.ToDouble(value)/10).ToString("G", CultureInfo.InvariantCulture)
        Catch
            Return value
        End Try
    End Function

    Shared Function Dir2deg(value As String) As String
        Try
            'Definition of direction is
            'Wind direction (integer value from 0-15) reflecting 0-360 degrees in 22.5 degree steps
            'meaning, 0 is 0 and 15 is 337.5 if steps are 22.5 degrees
            If Enumerable.Range(0, 16).Contains(Convert.ToInt32(value)) Then
                Return (Convert.ToInt32(value)*22.5).ToString("G", CultureInfo.InvariantCulture)
            Else
                Throw New IndexOutOfRangeException
            End If

        Catch
            Return "Undef"
        End Try
    End Function

    Shared Function Dir2Car(value As String) As String
        Try
            Return CardinalPtsMapping(Convert.ToInt32(value))
        Catch
            Return "Undef"
        End Try
    End Function

    Shared Function Uv2Level(value As String) As String
        Return UvMapping(Convert.ToInt32(value))
    End Function

    Shared Function Wind2Level(value As String) As String
        Return WindMapping(Convert.ToInt32(value))
    End Function

    Public Shared ReadOnly Processors As New Dictionary(Of String, Func(Of String, String)) From {
        {"shex2dec", AddressOf SignedHex2dec},
        {"hex2dec", AddressOf Hex2dec},
        {"str2dec", AddressOf Str2dec},
        {"dir2deg", AddressOf Dir2deg},
        {"dir2car", AddressOf Dir2Car},
        {"div10", AddressOf Div10},
        {"uv2level", AddressOf Uv2Level},
        {"wind2level", AddressOf Wind2Level}
        }
End Class
