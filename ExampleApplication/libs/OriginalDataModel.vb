Public MustInherit Class OriginalDataModel(Of T As OriginalDataModel(Of T))
    Inherits Model(Of T)

    Public Shared cachedAttributeNames As New Hashtable

    Public Overrides Sub loadAttributeNames()
        If cachedAttributeNames.Item(GetType(T)) IsNot Nothing Then
            attributeNames = cachedAttributeNames.Item(GetType(T))
            Return
        End If
        If dbFileExists() Then
            Dim attributeNamesList As List(Of String) = New List(Of String)
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252))
            If objReader.Peek() <> -1 Then
                Dim lineArray() As String
                lineArray = Split(objReader.ReadLine(), "|")
                For i As Integer = 0 To lineArray.Length - 1
                    If lineArray(i) <> "" Then
                        attributeNamesList.Add(lineArray(i))
                    End If
                Next
            End If
            attributeNames = attributeNamesList.ToArray
            cachedAttributeNames.Add(GetType(T), attributeNames)
            objReader.Close()
        Else
            Logger.errorLog("File " & dataFileName & " not found!")
            Throw New Exception("File " & dataFileName & " not found!")
        End If
    End Sub

    Public Sub clone(ByRef originalObject As T)
        If originalObject IsNot Nothing Then
            dataFileName = originalObject.dataFileName
            idFieldName = originalObject.idFieldName
            dbDirectory = originalObject.dbDirectory
            idxDirectory = originalObject.idxDirectory
            recordLineNumber = originalObject.recordLineNumber
            attributeNames = originalObject.attributeNames.Clone()
            attributes = originalObject.attributes.Clone()
        End If
    End Sub

    Public Overrides Sub initialize(ByVal databaseFilePath As String, ByVal databaseFileName As String)
        dataFileName = databaseFileName
        idFieldName = getIdFieldName()
        dbDirectory = databaseFilePath
        idxDirectory = Config.dataBaseIndexPath
        loadAttributeNames()
    End Sub

    Public Overrides Sub initialize(ByVal databaseFileName As String)
        initialize(Config.originalDataBasePath, databaseFileName)
    End Sub

    Public Overrides Sub initialize(ByVal databaseFileName As String, ByVal attributesToUse As String())
        dataFileName = databaseFileName
        idFieldName = getIdFieldName()
        dbDirectory = Config.originalDataBasePath
        idxDirectory = Config.dataBaseIndexPath
        loadAttributeNames()
        For i As Integer = 0 To attributeNames.Length - 1
            attributes.Item(attributeNames(i)) = attributesToUse(i)
        Next
    End Sub

    Public Overrides Sub generateIndexFile(ByVal attributeName As String)
        If dbFileExists() Then
            Dim splittedLine() As String
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252))
            Dim currentLine, occurency As String
            Dim fieldIdx = Array.IndexOf(attributeNames, attributeName)
            If objReader.Peek() <> -1 Then
                objReader.ReadLine() 'FieldNames
                Dim objWriter As New System.IO.StreamWriter(generateIdxFilePath(attributeName))
                Do While objReader.Peek() <> -1
                    currentLine = objReader.ReadLine()
                    If currentLine.Length > 0 Then
                        splittedLine = Split(currentLine, "|", attributeNames.Length)
                        occurency = splittedLine(fieldIdx)
                        If occurency.IndexOfAny(New Char() {Chr(34), vbLf, vbCr}) = -1 Then
                            objWriter.WriteLine(occurency)
                        Else
                            objWriter.WriteLine("""" & occurency.Replace("""", """""") & """")
                        End If
                    Else
                        objWriter.WriteLine("")
                    End If
                    objWriter.Flush()
                Loop
                objWriter.Close()
            End If
            objReader.Close()
        End If
    End Sub

    Public Overrides Function findOneBy(ByVal attributeName As String, ByVal value As Object) As T
        Dim foundObject As T
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            If System.IO.File.Exists(generateIdxFilePath(attributeName)) = False Then
                generateIndexFile(attributeName)
            End If
            Dim currentLine As String
            Dim objReaderIdx As New System.IO.StreamReader(generateIdxFilePath(attributeName), System.Text.Encoding.GetEncoding(1252))
            Dim currentLineIdx As Long = 0
            Dim valueString As String = value.ToString

            Do While objReaderIdx.Peek() <> -1
                currentLine = objReaderIdx.ReadLine()
                If currentLine.Equals(valueString) Then
                    Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName)
                    For i As Long = 0 To currentLineIdx
                        If objReader.Peek() = -1 Then
                            objReader.Close()
                            objReaderIdx.Close()
                            Return Nothing
                        End If
                        objReader.ReadLine()
                    Next
                    currentLine = objReader.ReadLine()
                    objReader.Close()
                    objReaderIdx.Close()

                    Dim attr() As String = Split(currentLine, "|", attributeNames.Length)
                    foundObject = GetType(T).GetConstructor(New System.Type() {GetType(T)}).Invoke(New Object() {Me})
                    foundObject.recordLineNumber = (currentLineIdx + 1)

                    foundObject.attributes.Clear()
                    For i As Integer = 0 To attributeNames.Length - 1
                        foundObject.attributes.Add(attributeNames(i).ToString, attr(i).ToString)
                    Next
                    Return foundObject
                End If
                currentLineIdx = currentLineIdx + 1
            Loop
            objReaderIdx.Close()
        End If
        Return Nothing
    End Function

    Public Overrides Function findAllBy(ByVal attributeName As String, ByVal value As Object, ByVal limit As Integer, ByRef attributesToLoad() As String) As List(Of T)
        Dim foundObjects As New List(Of T)

        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            If System.IO.File.Exists(generateIdxFilePath(attributeName)) = False Then
                generateIndexFile(attributeName)
            End If
            Dim currentLine As String
            Dim objReaderIdx As New System.IO.StreamReader(generateIdxFilePath(attributeName), System.Text.Encoding.GetEncoding(1252))
            Dim linesToSkip As Long = 1
            Dim currentLineIdx As Long = 0
            Dim valueString As String = value.ToString
            Dim foundObject As T
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252))

            Do While objReaderIdx.Peek() <> -1
                linesToSkip = linesToSkip + 1
                currentLine = objReaderIdx.ReadLine()
                currentLineIdx = currentLineIdx + 1
                If currentLine.Equals(valueString) Then
                    For i As Long = 0 To linesToSkip - 2
                        If objReader.Peek() = -1 Then
                            objReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin)
                            Exit For
                        End If
                        objReader.ReadLine()
                    Next
                    linesToSkip = 0
                    currentLine = objReader.ReadLine()

                    Dim attr() As String = Split(currentLine, "|", attributeNames.Length)
                    foundObject = GetType(T).GetConstructor(New System.Type() {GetType(T)}).Invoke(New Object() {Me})
                    foundObject.recordLineNumber = currentLineIdx

                    If attr.Length > 0 Then
                        If attributesToLoad Is Nothing Then
                            For i As Integer = 0 To attributeNames.Length - 1
                                foundObject.attributes.Add(attributeNames(i), attr(i))
                            Next
                        Else
                            For i As Integer = 0 To attributesToLoad.Length - 1
                                foundObject.attributes.Add(attributesToLoad(i), attr(Array.indexOf(attributeNames, attributesToLoad(i))))
                            Next
                        End If
                        foundObjects.Add(foundObject)
                    End If
                End If
            Loop
            objReader.Close()
            objReaderIdx.Close()
        End If
        Return foundObjects
    End Function

    Public Function listAll() As List(Of String())
        Dim foundRegisters As New List(Of String())
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            Dim currentLine As String
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252), False, 8192)
            objReader.ReadLine()
            Do While objReader.Peek() <> -1
                currentLine = objReader.ReadLine()
                Dim attr() As String = Split(currentLine, "|", attributeNames.Length)

                If attr.Length > 0 Then
                    foundRegisters.Add(attr)
                End If
            Loop
            objReader.Close()
        End If
        Return foundRegisters
    End Function

    Public Function lighterListAll() As ArrayList
        Dim foundRegisters As New ArrayList()
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            Dim currentLine As String
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252), False, 8192)
            objReader.ReadLine()
            Do While objReader.Peek() <> -1
                currentLine = objReader.ReadLine()
                Dim attr() As String = Split(currentLine, "|", attributeNames.Length)

                If attr.Length > 0 Then
                    foundRegisters.Add(attr)
                End If
            Loop
            objReader.Close()
        End If
        Return foundRegisters
    End Function

    Public Overrides Function findAll(ByVal limit As Integer, ByRef attributesToLoad() As String) As List(Of T)
        Dim foundObjects As New List(Of T)
        If System.IO.File.Exists(dbDirectory & dataFileName) Then
            Dim currentLine As String
            Dim currentLineIdx As Integer = 0
            Dim foundObject As T
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName, System.Text.Encoding.GetEncoding(1252))
            objReader.ReadLine()
            Do While objReader.Peek() <> -1
                currentLine = objReader.ReadLine()
                Dim attr() As String = Split(currentLine, "|", attributeNames.Length)
                If attr.Length > 0 Then
                    foundObject = GetType(T).GetConstructor(New System.Type() {GetType(T)}).Invoke(New Object() {Me})
                    foundObject.recordLineNumber = (++currentLineIdx)
                    If attributesToLoad Is Nothing Then
                        For i As Integer = 0 To attributeNames.Length - 1
                            foundObject.attributes.Add(attributeNames(i), attr(i))
                        Next
                    Else
                        For i As Integer = 0 To attributesToLoad.Length - 1
                            foundObject.attributes.Add(attributesToLoad(i), attr(Array.IndexOf(attributeNames, attributesToLoad(i))))
                        Next
                    End If
                    foundObjects.Add(foundObject)
                    If limit <> 0 And limit <= foundObjects.Count Then
                        objReader.Close()
                        Return foundObjects
                    End If
                End If
            Loop
            objReader.Close()
        End If
        Return foundObjects
    End Function

    Public Sub reloadAttributesFromLine(ByVal line As String)
        attributes.Clear()
        Dim attrib() As String = Split(line, "|", attributeNames.Length)
        For i As Integer = 0 To attributeNames.Length - 1
            attributes.Add(attributeNames(i), attrib(i))
        Next
    End Sub

    Public Sub reloadAttributesFromStringArray(ByRef attributes As String())
        Me.attributes.Clear()
        For i As Integer = 0 To attributeNames.Length - 1
            Me.attributes.Add(attributeNames(i), attributes(i))
        Next
    End Sub

    Public Shared Sub clearDirectory()
        FileHelper.clearDirectory(Config.originalDataBasePath)
        clearIndexDirectory()
    End Sub

    Public Overrides Sub save()
        Logger.errorLog("Save on OriginalDataModel: Not implemented")
    End Sub
End Class
