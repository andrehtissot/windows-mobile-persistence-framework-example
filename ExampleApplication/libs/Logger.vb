Public Class Logger
    Private outputStream As System.IO.StreamWriter
    Public Shared active As Boolean = True
    Private Shared ticks As Hashtable = New Hashtable()
    Private Shared memoryMarks As Hashtable = New Hashtable()

    Public Sub New(ByVal filePath As String)
        If Not System.IO.Directory.Exists(Config.logsPath) Then
            System.IO.Directory.CreateDirectory(Config.logsPath)
        End If
        Try
            outputStream = New System.IO.StreamWriter(filePath, True)
            Console.SetOut(outputStream)
            Console.Write(Year(Now) & "-" & Month(Now) & "-" & Microsoft.VisualBasic.DateAndTime.Day(Now) & " " & Hour(Now) & ":" & Minute(Now) & ":" & Second(Now) & ":  ")
        Catch ex As Exception
            Throw New Exception("Unable to generate log file!")
        End Try
    End Sub

    Public Sub WriteLine(ByVal text As String)
        Console.WriteLine(text)
    End Sub

    Public Sub Write(ByVal text As String)
        Console.Write(text)
    End Sub

    Public Sub Close()
        outputStream.Close()
    End Sub

    Public Shared Sub log(ByVal text As String)
        If active Then
            Dim logger As New Logger(Config.logsPath & "app_log.txt")
            logger.WriteLine(text)
            logger.Close()
        End If
    End Sub

    Public Shared Sub errorLog(ByVal text As String)
        Dim logger As New Logger(Config.logsPath & "app_errors.txt")
        logger.WriteLine(text)
        logger.Close()
    End Sub

    Public Shared Sub errorLog(ByVal exception As Exception)
        Dim logger As New Logger(Config.logsPath & "app_errors.txt")
        logger.WriteLine(exception.Message & ":" & vbCrLf & exception.ToString)
        logger.Close()
    End Sub

    Public Shared Sub startTick(ByVal number As Integer)
        ticks.Item(number) = Environment.TickCount
    End Sub

    Public Shared Sub startMemoryMark(ByVal number As Integer)
        memoryMarks.Item(number) = GC.GetTotalMemory(True)
    End Sub

    Public Shared Sub logUsedMemory()
        Dim logger As New Logger(Config.logsPath & "app_mem.txt")
        logger.WriteLine("Total: " & GC.GetTotalMemory(True) & " Bytes")
        logger.Close()
    End Sub

    Public Shared Sub stopTick(ByVal number As Integer)
        If active Then
            Dim tickDiff As Long = Environment.TickCount - ticks.Item(number)
            Dim logger As New Logger(Config.logsPath & "app_ticks.txt")
            logger.WriteLine(number & ": " & tickDiff)
            logger.Close()
        End If
    End Sub

    Public Shared Sub stopMemoryMark(ByVal number As Integer)
        If active Then
            Dim memoryMarkDiff As Long = GC.GetTotalMemory(True) - memoryMarks.Item(number)
            Dim logger As New Logger(Config.logsPath & "app_mem.txt")
            logger.WriteLine(number & ": " & memoryMarkDiff & vbTab & vbTab & " Total: " & GC.GetTotalMemory(True))
            logger.Close()
        End If
    End Sub
End Class
