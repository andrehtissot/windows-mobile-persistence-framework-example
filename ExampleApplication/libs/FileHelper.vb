Public Class FileHelper
    Public Shared Function fixPath(ByVal path As String) As String
        Return path.Replace("\", "/")
    End Function

    Public Shared Sub clearDirectory(ByVal path As String)
        path = fixPath(path)
        Dim tries As Short = 4S
        Do While System.IO.Directory.Exists(path) And Not (isEmpty(path))
            Try
                Dim files() = System.IO.Directory.GetFiles(path)
                For i As Integer = 0 To files.Length - 1
                    Try
                        System.IO.File.Delete(files(i))
                    Catch ex As Exception
                        If tries = 0 Then
                            Logger.errorLog(ex)
                            Throw New Exception("Unable to clean file " & files(i))
                        End If
                    End Try
                Next
            Catch ex As Exception
            End Try
            If System.IO.Directory.Exists(path) Then
                Try
                    Dim dirs() = System.IO.Directory.GetDirectories(path)
                    Try

                        For i As Integer = 0 To dirs.Length - 1
                            delete(dirs(i), (tries = 0))
                        Next
                    Catch ex As Exception
                        If tries = 0 Then
                            Logger.errorLog(ex)
                            Throw New Exception("Unable to clean directory " & path)
                        End If
                    End Try
                Catch ex As Exception
                End Try
            End If
            tries = tries - 1
        Loop
    End Sub

    Public Shared Sub delete(ByVal path As String)
        path = fixPath(path)
        If System.IO.Directory.Exists(path) Then
            Try
                Dim files() = System.IO.Directory.GetFiles(path)
                For i As Integer = 0 To files.Length - 1
                    Try
                        System.IO.File.Delete(files(i))
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception
            End Try
            Try
                Dim dirs() = System.IO.Directory.GetDirectories(path)
                For i As Integer = 0 To dirs.Length - 1
                    delete(dirs(i))
                Next
            Catch ex As Exception
            End Try
            Try
                System.IO.Directory.Delete(path, True)
            Catch ex As Exception

            End Try
        ElseIf System.IO.File.Exists(path) Then
            Try
                System.IO.File.Delete(path)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Shared Sub delete(ByVal path As String, ByVal raiseExceptions As Boolean)
        path = fixPath(path)
        If System.IO.Directory.Exists(path) Then
            Try
                Dim files() = System.IO.Directory.GetFiles(path)
                For i As Integer = 0 To files.Length - 1
                    Try
                        System.IO.File.Delete(files(i))
                    Catch ex As Exception
                        If raiseExceptions Then
                            Logger.errorLog(ex)
                            Throw ex
                        End If
                    End Try
                Next
            Catch ex As Exception
            End Try
            Try
                Dim dirs() = System.IO.Directory.GetDirectories(path)
                For i As Integer = 0 To dirs.Length - 1
                    delete(dirs(i), raiseExceptions)
                Next
            Catch ex As Exception
            End Try
            Try
                System.IO.Directory.Delete(path, True)
            Catch ex As Exception

            End Try
        ElseIf System.IO.File.Exists(path) Then
            Try
                System.IO.File.Delete(path)
            Catch ex As Exception
                If raiseExceptions Then
                    Logger.errorLog(ex)
                    Throw ex
                End If
            End Try
        End If
    End Sub

    Public Shared Sub mkdir(ByVal path As String)
        path = fixPath(path)
        If Not System.IO.Directory.Exists(path) Then
            Try
                System.IO.Directory.CreateDirectory(path)
            Catch ex1 As Exception
                Try
                    System.IO.Directory.CreateDirectory(path)
                Catch ex2 As Exception
                    Try
                        System.IO.Directory.CreateDirectory(path)
                    Catch ex3 As Exception
                        Try
                            System.IO.Directory.CreateDirectory(path)
                        Catch ex4 As Exception
                            Try
                                System.IO.Directory.CreateDirectory(path)
                            Catch ex As Exception
                                Logger.errorLog(ex)
                                Throw New Exception(path & " cannot be created!")
                            End Try
                        End Try
                    End Try
                End Try
            End Try
        End If
    End Sub

    Public Shared Sub touch(ByVal path As String)
        path = fixPath(path)
        If Not System.IO.File.Exists(path) Then
            Try
                System.IO.File.Create(path).Close()
            Catch ex1 As Exception
                Try
                    System.IO.File.Create(path).Close()
                Catch ex2 As Exception
                    Try
                        System.IO.File.Create(path).Close()
                    Catch ex As Exception
                        Logger.errorLog(ex.ToString)
                        Throw New Exception(path & " cannot be created!")
                    End Try
                End Try
            End Try
        End If
    End Sub

    Public Shared Function isEmpty(ByVal path) As Boolean
        path = fixPath(path)
        If System.IO.File.Exists(path) Then
            Return True
        ElseIf System.IO.Directory.Exists(path) Then
            Try
                Dim entries() As String = System.IO.Directory.GetFileSystemEntries(path)
                Return (entries.Length = 0)
            Catch ex As Exception
                Return True
            End Try
        End If
        Return False
    End Function

    Public Shared Function StreamWriter(ByVal path As String, ByVal append As Boolean) As System.IO.StreamWriter
        Dim objWriter As System.IO.StreamWriter
        Try
            objWriter = New System.IO.StreamWriter(path, append)
        Catch ex2 As Exception
            Try
                objWriter = New System.IO.StreamWriter(path, append)
            Catch ex1 As Exception
                objWriter = New System.IO.StreamWriter(path, append)
            End Try
        End Try
        Return objWriter
    End Function

    Public Shared Function FileInfo(ByVal path As String) As System.IO.FileInfo
        Dim fileInfoObj As System.IO.FileInfo = New System.IO.FileInfo(path)
        Try
            fileInfoObj = New System.IO.FileInfo(path)
        Catch ex1 As Exception
            Try
                fileInfoObj = New System.IO.FileInfo(path)
            Catch ex As Exception
                fileInfoObj = New System.IO.FileInfo(path)
            End Try
        End Try
        Return fileInfoObj
    End Function

    Public Shared Function FileStream(ByVal path As String) As System.IO.FileStream
        Dim fileInfoObj As System.IO.FileStream
        Try
            fileInfoObj = FileInfo(path).Open(System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None)
        Catch ex1 As Exception
            Try
                fileInfoObj = FileInfo(path).Open(System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None)
            Catch ex As Exception
                fileInfoObj = FileInfo(path).Open(System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None)
            End Try
        End Try
        Return fileInfoObj
    End Function

    Public Shared Function StreamReader(ByVal path As String) As System.IO.StreamReader
        Dim objReader As System.IO.StreamReader
        Try
            objReader = New System.IO.StreamReader(path)
        Catch ex1 As Exception
            Try
                objReader = New System.IO.StreamReader(path)
            Catch ex2 As Exception
                Try
                    objReader = New System.IO.StreamReader(path)
                Catch ex3 As Exception
                    Try
                        objReader = New System.IO.StreamReader(path)
                    Catch ex4 As Exception
                        Try
                            objReader = New System.IO.StreamReader(path)
                        Catch ex5 As Exception
                            Try
                                objReader = New System.IO.StreamReader(path)
                            Catch ex As Exception
                                Throw New Exception("Unable to access file " & path)
                            End Try
                        End Try
                    End Try
                End Try
            End Try
        End Try
        Return objReader
    End Function
End Class
