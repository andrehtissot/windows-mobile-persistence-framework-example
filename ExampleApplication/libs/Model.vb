Public MustInherit Class Model(Of T As Model(Of T))
    Public dataFileName,idFieldName,dbDirectory,idxDirectory As String
    Public recordLineNumber As Integer = -1
    Public attributeNames() As String
    Public attributes As New Hashtable

    MustOverride Function getIdFieldName() As String
    MustOverride Sub loadAttributeNames()
    MustOverride Sub initialize(ByVal databaseFileName As String)
    MustOverride Sub initialize(ByVal databaseFilePath As String, ByVal databaseFileName As String)
    MustOverride Sub initialize(ByVal databaseFileName As String, ByVal attributesToUse As String())
    MustOverride Sub generateIndexFile(ByVal attributeName As String)
    MustOverride Function findOneBy(ByVal attributeName As String, ByVal value As Object) As T
    MustOverride Function findAllBy(ByVal attributeName As String, ByVal value As Object, ByVal limit As Integer, ByRef attributesToLoad() As String) As List(Of T)
    MustOverride Function findAll(ByVal limit As Integer, ByRef attributesToLoad() As String) As List(Of T)
    MustOverride Sub save()

    Public Sub loadIdFieldName()
        idFieldName = getIdFieldName()
    End Sub

    Public Function dbFileExists() As Boolean
        Return System.IO.File.Exists(dbDirectory & dataFileName)
    End Function

    Public Function getString(ByVal attributeName As String) As String
        Return attributes.Item(attributeName).ToString
    End Function

    Public Function getId() As Long
        Return Val(attributes.Item(idFieldName))
    End Function

    Public Function generateIdxFilePath(ByVal attributeName As String) As String
        Dim path As String = idxDirectory & dataFileName & "/"
        If Not System.IO.Directory.Exists(path) Then
            FileHelper.delete(idxDirectory & dataFileName)
            Try
                FileHelper.mkdir(idxDirectory)
                FileHelper.mkdir(path)
            Catch ex As Exception
                Logger.errorLog(ex)
                Application.Exit()
            End Try
        End If
        Return path & "/" & attributeName & ".txt"
    End Function

    Public Function find(ByVal id As Long) As T
        Return findOneBy(idFieldName, id)
    End Function

    Public Function findAllBy(ByVal attributeName As String, ByVal value As Object, ByVal limit As Integer) As List(Of T)
        Return findAllBy(attributeName, value, limit, Nothing)
    End Function

    Public Function findAllBy(ByVal attributeName As String, ByVal value As Object) As List(Of T)
        Return findAllBy(attributeName, value, 0, Nothing)
    End Function

    Public Function findAll(ByVal limit As Integer) As List(Of T)
        Return findAll(limit, Nothing)
    End Function

    Public Function findAll() As List(Of T)
        Return findAll(0, Nothing)
    End Function

    Public Shared Sub clearIndexDirectory()
        FileHelper.clearDirectory(Config.dataBaseIndexPath)
    End Sub
End Class
