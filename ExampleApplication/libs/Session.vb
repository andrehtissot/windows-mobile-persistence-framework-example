Public Class Session
    Private Shared currentSessionObj As Session
    Public systemBlocked = False
    Public user As User
    Private currentWindow As Form

    Public Shared Sub prepareCache()
        Dim user As User = New User()
        user.loadCachedListAndIndex();
        Dim thing As Thing = New Thing()
        thing.loadCachedListAndIndex();
    End Function

    Public Shared Function currentSession() As Session
        If currentSessionObj Is Nothing Then
            currentSessionObj = New Session()
        End If
        Return currentSessionObj
    End Function

    Public Shared Sub killSession()
        currentSessionObj = Nothing
    End Sub

    Private Sub New()
        currentSessionObj = Me
    End Sub

    Public Sub setWindow(ByRef window As Form)
        window.Show()
        If currentWindow IsNot Nothing Then
            currentWindow.Hide()
        End If
        currentWindow = window
    End Sub

    Public Sub login(ByVal userLogin As String)
        If userId.Length = 0 Then
            Throw New Exception("Unable to login." & vbCrLf & "Fill login.")
        End If

        Dim user As User = Nothing
        For i As Integer = 0 To user.cachedListIndex.Count - 1
            If user.cachedListIndex.Item(i) = userLogin Then
                user = New User()
                user.reloadAttributesFromStringArray(user.cachedList.Item(i))
                Exit For
            End If
        Next
        If user Is Nothing Then
            Throw New Exception("User not found!")
        End If

        Session.currentSession().user = user
    End Sub

    Public Sub logout()
        Session.currentSession().user = Nothing
        setWindow(FormLogin)
    End Sub
End Class
