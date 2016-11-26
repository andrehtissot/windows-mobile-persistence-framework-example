Public Class Thing
    Inherits OriginalDataModel(Of Thing)
    Public Shared idIndex As ArrayList = New ArrayList()

    Public Sub New(ByVal objectToClone As Thing)
        clone(objectToClone)
    End Sub

    Public Overrides Function getIdFieldName() As String
        Return "thingId"
    End Function

    Public Sub New(ByVal attributesToUse As String())
        initialize("thing.csv", attributesToUse)
    End Sub

    Public Sub New()
        initialize("thing.csv")
    End Sub

    Public Sub loadIdIndex()
        Dim idFieldPos As Integer = Array.IndexOf(attributeNames, idFieldName)
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            Dim currentLine As String
            idIndex.Clear()
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252), False, 8192)
            objReader.ReadLine()
            Do While objReader.Peek() <> -1
                currentLine = objReader.ReadLine()
                Dim attr() As String = Split(currentLine, "|", attributeNames.Length)
                If attr.Length > 0 Then
                    idIndex.Add(attr(idFieldPos))
                End If
            Loop
            objReader.Close()
        End If
    End Sub

    Public Sub loadAttributesFromFile(ByVal indexPosition As Integer)
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252))
            For i As Integer = 0 To indexPosition
                objReader.ReadLine()
            Next
            reloadAttributesFromLine(objReader.ReadLine())
            objReader.Close()
        End If
    End Sub
End Class
