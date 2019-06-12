Imports com.magpern.gateway2mqtt.Exceptions
Imports com.magpern.gateway2mqtt.Extentions
Imports Microsoft.Extensions.Logging

Public Class MessageConverter

    Public Shared Property Logger() As ILogger
    Public Shared Config As IRFLinkConfig

    Public Sub New(ByVal log As ILogger(Of MessageConverter), conf As IConfig)
        Logger = log
        Config = CType(conf, IRFLinkConfig)
    End Sub

    Public Shared Function MessageToObject(msg As String) As List(Of Dictionary(Of String, String))
        '20;02;Name;ID=9999;LABEL=data;
        Dim switchIndex As Integer
        Dim switchNum As String = "0"

        Dim family As String
        Dim deviceId As String
        Dim resultOut As List(Of Dictionary(Of String, String)) = Nothing

        Dim data As List(Of String) = msg.TrimEnd(";"c).Split(";").ToList

        If data.Count > 0 AndAlso data(1) = "00" Then
            'This is the presentation message from the Gateway. Nice to look at, not useful for me
            Logger.LogDebug(data(2))
        Else
            Logger.LogDebug($"Recieved message: {String.Join("; ", data.ToArray)}")

            If data.Count > 3 AndAlso data(0) = "20" AndAlso data(2).Split("=")(0) <> "VER" Then ': # Special Control Command 'VERSION' returns a len=5 data object. This trick is necessary... but not very clean
                family = data(2)
                deviceId = If(data(3).Contains("="), data(3).Split("=")(1), data(3)) '# TODO: For some debug messages there Is no =
                Dim ignoreDevices = Config.RflinkIgnoredDevices

                If ignoreDevices Is Nothing OrElse (Not ignoreDevices.Contains(deviceId) AndAlso Not ignoreDevices.Contains(family) AndAlso Not ignoreDevices.Contains($"{family}/{deviceId}")) Then
                    Dim tokens = New List(Of String) From {"dummy", "dummy", "dummy", "dummy"}

                    If Config.mqtt_switch_incl_topic Then
                        For i As Integer = 4 To data.Count - 1
                            tokens.Add(data(i).Split("=")(0))
                        Next

                        If tokens.Contains("SWITCH") Then
                            Logger.LogDebug("Switch recognized in the data, including it in CMD if present")
                            switchIndex = tokens.IndexOf("SWITCH")
                            Logger.LogDebug($"Switch index in data : {switchIndex.ToString};{tokens(switchIndex)};{data(switchIndex)}")
                            switchNum = data(switchIndex).Split("=")(1)
                            data.RemoveAt(switchIndex)
                        End If
                    End If
                    Dim d As New Dictionary(Of String, String) From {{"message", msg}}

                    For i As Integer = 4 To data.Count - 1
                        Dim token() As String = data(i).Split("=")
                        d.Add(token(0), Process_data(token(0), token(1)))
                    Next

                    If Not Config.mqtt_include_message Then
                        d.Remove("message")
                    End If

                    If Config.mqtt_json Then
                        Dim keymod As String
                        If Config.mqtt_switch_incl_topic Then
                            keymod = $"{switchNum}/message"
                        Else
                            keymod = "message"
                        End If

                        Dim dataOut As New Dictionary(Of String, String) From {
                            {"action", "NCC"},
                            {"topic", ""},
                            {"family", family},
                            {"device_id", deviceId},
                            {"param", keymod},
                            {"payload", DictionaryToJson(d)},
                            {"qos", 1},
                            {"timestamp", (Date.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds}
                            }
                        resultOut = New List(Of Dictionary(Of String, String)) From {dataOut}
                    Else
                        For Each key As KeyValuePair(Of String, String) In d
                            Dim val = key.Value
                            Dim keymod As String
                            If key.Key = "CMD" AndAlso Config.mqtt_switch_incl_topic AndAlso Convert.ToInt32(switchNum) >= 0 Then
                                keymod = $"{switchNum}/CMD"
                            Else
                                keymod = key.Key
                            End If

                            Dim dataOut As New Dictionary(Of String, String) From {
                                {"action", "NCC"},
                                {"topic", ""},
                                {"family", family},
                                {"device_id", deviceId},
                                {"param", keymod},
                                {"payload", val},
                                {"qos", 1},
                                {"timestamp", (Date.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds}
                                }
                            If resultOut Is Nothing Then
                                resultOut = New List(Of Dictionary(Of String, String)) From {dataOut}
                            Else
                                resultOut.Append(dataOut)
                            End If
                        Next
                    End If
                Else
                    Logger.LogDebug("Device is ignored")
                    Throw new DeviceIgnoredException(msg)
                End If
            ElseIf (data.Count = 3 AndAlso data(0) = "20") OrElse (data.Count > 3 AndAlso data(0) = "20" AndAlso data(2).split("=")(0) = "VER") Then
                Dim payload = String.Join(";", data.GetRange(2, data.Count - 3))
                Dim dataOut As New Dictionary(Of String, String) From {
                    {"action", "SCC"},
                    {"topic", ""},
                    {"family", ""},
                    {"device_id", ""},
                    {"param", ""},
                    {"payload", payload},
                    {"qos", 1},
                    {"timestamp", (Date.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds}
                    }
                resultOut = New List(Of Dictionary(Of String, String)) From {dataOut}
            End If
        End If

        Return resultOut
    End Function


    Private Shared Function DictionaryToJson(dict As Dictionary(Of String, String)) As String
        Dim entries = dict.Select(Function(d) $"""{d.Key}"": {d.Value}")
        Return "{" & String.Join(",", entries) & "}"
    End Function

    Private Shared Function Process_data(field As String, value As String) As String

        If Config.RflinkOutputParamsProcessing.ContainsKey(field) AndAlso Config.RflinkOutputParamsProcessing(field).Count > 0 Then
            Dim procs = Config.RflinkOutputParamsProcessing(field)
            Dim vv = value
            For Each processor In procs
                If Processors.Processors.ContainsKey(processor) Then
                    vv = Processors.Processors(processor)(vv)
                End If
            Next
            Return vv
        Else
            Return value
        End If

    End Function

End Class
