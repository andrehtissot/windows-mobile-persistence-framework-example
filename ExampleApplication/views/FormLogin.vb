Public Class FormLogin
    Private Sub FormLogin_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SuspendLayout()
        TextBoxUserLogin.Visible = False
        Session.currentSession().setWindow(Me)
        'Logger.active = False
        Try
            FileHelper.mkdir(Config.root & "../../")
            FileHelper.mkdir(Config.root & "../")
            FileHelper.mkdir(Config.root)
            FileHelper.mkdir(Config.dataBaseIndexPath)
            FileHelper.mkdir(Config.logsPath)
            FileHelper.mkdir(Config.originalDataBasePath)
            FileHelper.mkdir(Config.textDataBasePath)
            FileHelper.mkdir(Config.originalDataBasePath)
            User.clearIndexDirectory()
        Catch ex As Exception
            MsgBox(ex.Message)
            Application.Exit()
        End Try

        If FileHelper.isEmpty(Config.originalDataBasePath) Then
            MsgBox("Data dir is empty!")
            Application.Exit()
            Return
        End If

        Dim errorMessage As String = ""
        Try
            Dim model As Thing = New Thing()
        Catch ex As Exception
            errorMessage = errorMessage & vbTab & vbCrLf & ex.Message
        End Try
        Try
            Dim model As User = New User()
        Catch ex As Exception
            errorMessage = errorMessage & vbTab & vbCrLf & ex.Message
        End Try
        If errorMessage.Length > 0 Then
            MsgBox("While loading: " & errorMessage)
            Application.Exit()
        End If

        Try
            Session.prepareCache()
        Catch ex As Exception
            MsgBox(ex.Message)
            Logger.errorLog(ex.Message)
            Logger.errorLog(ex.ToString)
        End Try

        Text = "Login"
        TextBoxUserLogin.Visible = True
        TextBoxUserLogin.Focus()
        ResumeLayout(False)
    End Sub

    Private Sub TextBoxUserLogin_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxUserLogin.KeyPress
        e.Handled = (Not Char.IsNumber(e.KeyChar) Or TextBoxUserLogin.Text.Length > 8) And Not Asc(e.KeyChar) = System.Windows.Forms.Keys.Back
    End Sub

    Private Sub ButtonLogar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonLogar.Click
        login()
    End Sub

    Private Sub login()
        Try
            Session.currentSession().login(TextBoxUserLogin.Text)
            MsgBox("User authenticated!")
            Session.currentSession().setWindow(FormMenu)
            TextBoxUserLogin.Text = ""
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub ButtonCloseApp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCloseApp.Click
        Application.Exit()
    End Sub
End Class