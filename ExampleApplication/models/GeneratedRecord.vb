Public Class GeneratedRecord
    Inherits TXTModel(Of GeneratedRecord)

    Public Overrides Function getIdFieldName() As String
        Return "FileName"
    End Function

    Public Shared Function filenameToGeneratedRecord(ByVal filename As String) As String
        Dim tituloGeneratedRecord = filename.Substring(6, 2) & "/" & _
            filename.Substring(4, 2) & "/" & _
            filename.Substring(0, 4) & " - " & _
            filename.Substring(15, filename.Length - 15 - 4)
        Return tituloGeneratedRecord
    End Function

    Public Shared Function listOptions() As String()
        Dim list() As String = listFiles()
        Dim resultList As List(Of String) = New List(Of String)()
        For i As Integer = 0 To list.Length - 1
            Try
                resultList.Add(filenameToGeneratedRecord(list(i).Replace(Config.originalDataBasePath, "")))
            Catch ex As Exception
            End Try
        Next
        Return resultList.ToArray
    End Function

    Public Shared Function listFiles() As String()
        Try
            Dim list() As String = System.IO.Directory.GetFiles(Config.originalDataBasePath)
            Return list
        Catch ex As Exception
            Return New String() {}
        End Try
    End Function

    Public Shared Sub clearDirectory()
        FileHelper.clearDirectory(Config.textDataBasePath)
        clearIndexDirectory()
        clearAllExperimentalListCache()
    End Sub

    Public Shared Sub clearDataBaseFile()
        FileHelper.delete(Config.textDataBasePath & "GeneratedRecords.txt")
        GeneratedRecord.clearExperimentalListCache("GeneratedRecords.txt")
    End Sub

    Public Sub New()
        initialize("GeneratedRecords.txt")
    End Sub

    Public Sub New(ByVal userId As Long)
        Me.New()
        attributes("userId") = userId
        prepareCache()
    End Sub

    Public Sub setIdentification(ByVal IdentificationGeneratedRecord As String)
        If attributes("Identification") Is Nothing Then
            attributes("Identification") = IdentificationGeneratedRecord
            attributes("FileName") = (Now.ToString("yyyyMMddHHmmss") & "_" & IdentificationGeneratedRecord) & ".txt"
        End If
    End Sub

    Public Overrides Sub loadAttributeNames()
        attributeNames = New String() { _
            "FileName", _
            "Identification", _
            "userId"}
    End Sub

    Public Overrides Sub loadAttributesFormat()
        attributesFormat = New String() {"String(31)", "String(12)", "Long"}
    End Sub

    Public Sub prepareCache()
        Dim itemGeneratedRecord As ItemGeneratedRecord = New ItemGeneratedRecord(attributes.Item("FileName"))
        Dim attributeLineIdx As Integer = itemGeneratedRecord.getAttributeLineIdx(itemGeneratedRecord.idFieldName)
        Dim attributeLength As Integer = itemGeneratedRecord.getAttributeColumnLength(itemGeneratedRecord.idFieldName)
    End Sub
End Class
