Public MustInherit Class TXTModel(Of T As TXTModel(Of T))
    Inherits Model(Of T)
    Public attributesFormat() As String
    Public recordLineLength As Long
    Private Shared experimentalListCache As Hashtable = New Hashtable()
    Private Shared experimentalListCacheIndex As Hashtable = New Hashtable()

    Public MustOverride Sub loadAttributesFormat()

    Public Overrides Sub initialize(ByVal databaseFilePath As String, ByVal databaseFileName As String)
        dataFileName = databaseFileName
        idFieldName = getIdFieldName()
        dbDirectory = databaseFilePath
        idxDirectory = Config.dataBaseIndexPath
        loadAttributeNames()
        loadAttributesFormat()
    End Sub

    Public Overrides Sub initialize(ByVal databaseFileName As String)
        initialize(Config.textDataBasePath, databaseFileName)
    End Sub

    Public Overrides Sub initialize(ByVal databaseFileName As String, ByVal attributesToUse As String())
        dataFileName = databaseFileName
        idFieldName = getIdFieldName()
        dbDirectory = Config.textDataBasePath
        idxDirectory = Config.dataBaseIndexPath
        loadAttributeNames()
        loadAttributesFormat()
        For i As Integer = 0 To attributeNames.Length - 1
            attributes.Item(attributeNames(i)) = attributesToUse(i)
        Next
    End Sub

    Public Overrides Sub generateIndexFile(ByVal attributeName As String)
        If dbFileExists() Then
            Dim indexRegs, splittedLine As New ArrayList()
            Dim objReader As New System.IO.StreamReader(dbDirectory & dataFileName)
            Dim currentLine As String
            Dim attributeIdx = Array.indexOf(attributeNames, attributeName)
            Dim attributeLineIdx As Integer = getAttributeLineIdx(attributeName)
            Dim attributeLength As Integer = getAttributeColumnLength(attributeName)

            If objReader.Peek() <> -1 Then
                Dim objWriter As System.IO.StreamWriter = FileHelper.StreamWriter(generateIdxFilePath(attributeName), False)
                Do While objReader.Peek() <> -1
                    currentLine = objReader.ReadLine()
                    If currentLine.Length = 0 Then
                        Continue Do
                    End If
                    objWriter.WriteLine(currentLine.Substring(attributeLineIdx, attributeLength))
                Loop
                objWriter.Close()
            End If
            objReader.Close()
        End If
    End Sub

    Public Function generateIndexList(ByVal attributeName As String) As List(Of Integer)
        Dim indexes As List(Of Integer) = New List(Of Integer)()
        If dbFileExists() Then
            Dim objReader As System.IO.StreamReader = FileHelper.StreamReader(dbDirectory & dataFileName)
            Dim currentLine As String
            Dim attributeLineIdx As Integer = getAttributeLineIdx(attributeName)
            Dim attributeLength As Integer = getAttributeColumnLength(attributeName)

            If objReader.Peek() <> -1 Then
                Do While objReader.Peek() <> -1
                    currentLine = objReader.ReadLine()
                    If currentLine.Length > (attributeLineIdx + attributeLength) Then
                        indexes.Add(currentLine.Substring(attributeLineIdx, attributeLength))
                    Else
                        indexes.Add("")
                    End If
                Loop
            End If
            objReader.Close()
        End If
        Return indexes
    End Function

    Public Function listAll() As List(Of String)
        Dim indexes As List(Of String) = New List(Of String)()
        If dbFileExists() Then
            Dim objReader As System.IO.StreamReader = Nothing
            dataFileName = Trim(dataFileName)
            Try
                objReader = FileHelper.StreamReader(dbDirectory & dataFileName)
                Dim currentLine As String
                If objReader.Peek() <> -1 Then
                    Do While objReader.Peek() <> -1
                        currentLine = objReader.ReadLine()
                        indexes.Add(currentLine)
                    Loop
                End If
            Catch ex As Exception
                Logger.errorLog(ex)
                MsgBox(ex.Message)
            End Try
            If objReader IsNot Nothing Then
                objReader.Close()
            End If
        End If
        Return indexes
    End Function

    Public Overloads Overrides Function findAllBy(ByVal attributeName As String, ByVal value As Object, ByVal limit As Integer, ByRef attributesToLoad() As String) As System.Collections.Generic.List(Of T)
        Logger.errorLog("findAllBy: not implemented!")
        Return Nothing
    End Function

    Public Overrides Function findAll(ByVal limit As Integer, ByRef attributesToLoad() As String) As System.Collections.Generic.List(Of T)
        Dim objList As List(Of T) = New List(Of T)()
        Dim strList As List(Of String) = listAll()
        Dim obj As T
        For i As Integer = 0 To strList.Count - 1
            obj = GetType(T).GetConstructor(New System.Type() {}).Invoke(New Object() {})
            obj.attributes = obj.fromRecordLine(strList(i))
            obj.loadIdFieldName()
            obj.recordLineNumber = i
            objList.Add(obj)
        Next
        Return objList
    End Function

    Public Overrides Function findOneBy(ByVal attributeName As String, ByVal value As Object) As T
        Dim foundObject As T
        If dbFileExists() Then
            If System.IO.File.Exists(generateIdxFilePath(attributeName)) = False Then
                generateIndexFile(attributeName)
            End If
            Dim currentLine As String
            Dim objReaderIdx As System.IO.StreamReader = FileHelper.StreamReader(generateIdxFilePath(attributeName))
            Dim currentLineIdx As Long = 0
            Dim valueString As String = attributeToRecordLineFormat(attributeName, value)

            Do While objReaderIdx.Peek() <> -1
                currentLine = objReaderIdx.ReadLine()
                If currentLine.Equals(valueString) Then
                    Dim objReader As System.IO.StreamReader
                    Try
                        objReader = New System.IO.StreamReader(dbDirectory & dataFileName)
                    Catch ex As Exception
                        Logger.errorLog(ex.ToString)
                        objReaderIdx.Close()
                        Throw New Exception("Unable to create/edit file: " & dbDirectory & dataFileName)
                    End Try

                    For i As Long = 0 To currentLineIdx
                        If objReader.Peek() = -1 Then
                            objReader.Close()
                            objReaderIdx.Close()
                            Return Nothing
                        End If
                        currentLine = objReader.ReadLine()
                    Next
                    objReader.Close()
                    objReaderIdx.Close()

                    foundObject = GetType(T).GetConstructor(New System.Type() {GetType(String)}).Invoke(New Object() {dataFileName})
                    foundObject.recordLineNumber = (currentLineIdx + 1)
                    foundObject.recordLineLength = currentLine.Length
                    foundObject.attributes = fromRecordLine(currentLine)
                    Return foundObject
                End If
                currentLineIdx = currentLineIdx + 1
            Loop
            objReaderIdx.Close()
        End If
        Return Nothing
    End Function

    Public Function newRecord() As Boolean
        loadExperimentalListCacheIfEmpty()
        Dim id = recordLineNumber
        If id > -1 Then
            Return False
        End If
        Dim listIndex As List(Of Long) = DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long))
        For i As Integer = 0 To listIndex.Count() - 1
            If getId() = listIndex.Item(i) Then
                recordLineNumber = i
                Return False
            End If
        Next
        Return True
    End Function

    Public Sub delete()
        If Not newRecord() Then
            For i As Integer = 0 To attributeNames.Length - 1
                attributes.Item(attributeNames(i)) = Nothing
            Next
            update()
        End If
    End Sub

    Public Overrides Sub save()
        If newRecord() Then
            insert()
        Else
            update()
        End If
    End Sub

    Public Function toRecordLine() As String
        Dim line As String = ""
        For i As Integer = 0 To attributesFormat.Length - 1
            Dim attributeName = attributeNames(i)
            line = line & attributeToRecordLineFormat(attributeName)
        Next
        Return line
    End Function

    Private Function attributeToRecordLineFormat(ByVal attributeName As String, ByVal value As Object) As String
        If attributeTypeIs(attributeName, "Long") Then
            If value Is Nothing Then
                Return "".PadLeft(getAttributeColumnLength(attributeName), "0")
            End If
            Dim valueString As String = value.ToString
            Return valueString.PadLeft(getAttributeColumnLength(attributeName), "0")
        ElseIf attributeTypeIs(attributeName, "String") Then
            If value Is Nothing Then
                Return "".PadLeft(getAttributeColumnLength(attributeName), " ")
            End If
            Dim valueString As String = value.ToString
            Return valueString.PadRight(getAttributeColumnLength(attributeName), " ")
        End If
        Return Nothing
    End Function

    Private Function attributeToRecordLineFormat(ByVal attributeName As String) As String
        Return attributeToRecordLineFormat(attributeName, attributes.Item(attributeName))
    End Function

    Public Function fromRecordLine(ByVal line As String) As Hashtable
        Dim attrs As Hashtable = New Hashtable()
        Dim skip = 0
        Dim length = 0
        If attributeNames Is Nothing Then
            loadAttributeNames()
            loadAttributesFormat()
        End If

        For i As Integer = 0 To attributeNames.Length - 1
            length = getAttributeColumnLength(attributeNames(i))
            attrs.Add(attributeNames(i), line.Substring(skip, length))
            skip = skip + length
        Next
        Return attrs
    End Function

    Public Function getAttributeLineIdx(ByVal attributeName As String) As Integer
        Dim attributeIdx As Integer = Array.indexOf(attributeNames, attributeName)
        Dim count As Integer = 0
        For i As Integer = 0 To attributesFormat.Length - 1
            If attributeIdx = i Then
                Exit For
            End If
            count = count + getAttributeColumnLength(attributeName)
        Next
        Return count
    End Function

    Private Function attributeTypeIs(ByVal attributeName As String, ByVal attributeType As String) As Boolean
        Dim attributeIdx As Integer = Array.indexOf(attributeNames, attributeName)
        Return (attributesFormat(attributeIdx).Equals(attributeType) Or attributesFormat(attributeIdx).StartsWith(attributeType & "("))
    End Function

    Public Function getAttributeColumnLength(ByVal attributeName As String) As Integer
        If attributeTypeIs(attributeName, "Long") Then
            Return 8
        ElseIf attributeTypeIs(attributeName, "String") Then
            Try
                Dim attributeIdx As Integer = Array.IndexOf(attributeNames, attributeName)
                Dim strValue As String = attributesFormat(attributeIdx).Substring(6)
                strValue = strValue.Substring(1, strValue.Length - 1)
                Return Val(strValue)
            Catch ex As Exception
                Logger.errorLog(Array.IndexOf(attributeNames, attributeName))
                Logger.errorLog(ex)
                Throw New Exception("Found wrongly formatted file")
            End Try
        End If
    End Function

    Private Sub loadExperimentalListCacheIfEmpty()
        dataFileName = Trim(dataFileName)
        If Not experimentalListCache.ContainsKey(dataFileName) Then
            Dim list As List(Of T) = findAll()
            experimentalListCache.Add(dataFileName, list)
            experimentalListCacheIndex.Add(dataFileName, New List(Of Long))
            Dim obj As T
            For i As Integer = 0 To list.Count - 1
                obj = list.Item(i)
                DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long)).Add(obj.getId())
            Next
        End If
    End Sub

    Private Sub addOnExperimentalListCache(ByRef obj As T)
        DirectCast(experimentalListCache.Item(dataFileName), List(Of T)).Add(obj)
        DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long)).Add(obj.getId())
    End Sub

    Private Sub setOnExperimentalListCache(ByRef obj As T, ByVal position As Long)
        DirectCast(experimentalListCache.Item(dataFileName), List(Of T)).Item(position) = obj
        DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long)).Item(position) = obj.getId()
    End Sub

    Public Sub clearExperimentalListCache()
        If experimentalListCache.ContainsKey(dataFileName) Then
            DirectCast(experimentalListCache.Item(dataFileName), List(Of T)).Clear()
            DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long)).Clear()
        End If
    End Sub

    Public Shared Sub clearExperimentalListCache(ByVal dataFileName)
        If experimentalListCache.ContainsKey(dataFileName) Then
            DirectCast(experimentalListCache.Item(dataFileName), List(Of T)).Clear()
            DirectCast(experimentalListCacheIndex.Item(dataFileName), List(Of Long)).Clear()
        End If
    End Sub

    Public Shared Sub clearAllExperimentalListCache()
        experimentalListCache.Clear()
        experimentalListCacheIndex.Clear()
    End Sub

    Public Sub reloadAttributesFromCache()
        If Not newRecord() Then
            attributes = DirectCast(experimentalListCache.Item(dataFileName), List(Of T)).Item(recordLineNumber).attributes
        End If
    End Sub

    Public Sub update()
        dataFileName = Trim(dataFileName)
        Dim objStream As System.IO.FileStream
        Try
            objStream = FileHelper.FileStream(dbDirectory & dataFileName)
            Try
                Dim recordLine As String = toRecordLine()
                Dim bytes() As Byte = (New System.Text.UTF8Encoding()).GetBytes(recordLine)
                recordLineLength = recordLine.Length
                objStream.Seek(recordLineNumber * (recordLineLength + 2), IO.SeekOrigin.Begin)
                objStream.Write(bytes, 0, recordLineLength)
                setOnExperimentalListCache(Me, recordLineNumber)
            Catch ex As Exception
                Logger.errorLog(ex)
            End Try
            objStream.Close()
        Catch ex As Exception
            Logger.errorLog(ex)
            Throw New Exception("Unable to update the record in " & dataFileName)
        End Try
    End Sub

    Public Sub insert()
        Try
            loadExperimentalListCacheIfEmpty()
            Dim objWriter As System.IO.StreamWriter = FileHelper.StreamWriter(dbDirectory & dataFileName, True)
            Try
                Dim recordLine As String = toRecordLine()
                objWriter.WriteLine(recordLine)
                addOnExperimentalListCache(Me)
            Catch ex As Exception
                Logger.errorLog(ex)
            End Try
            objWriter.Close()
        Catch ex As Exception
            Logger.errorLog(ex)
            Throw New Exception("Unable to insert the record in " & dataFileName)
        End Try
    End Sub
End Class
