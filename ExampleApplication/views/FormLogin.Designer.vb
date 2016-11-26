<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class FormLogin
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.TextBoxUserLogin = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.ButtonLogin = New System.Windows.Forms.Button
        Try
            Me.DecodeComponent1 = New HandHeldProducts.Embedded.Decoding.DecodeComponent(Me.components)
        Catch ex As Exception
            MsgBox("Other application is using the barcode reader." & vbCrLf & "Please close it or restart the device.")
            Application.Exit()
            Return
        End Try
        Me.ButtonCloseApp = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'TextBoxUserLogin
        '
        Me.TextBoxUserLogin.Location = New System.Drawing.Point(3, 36)
        Me.TextBoxUserLogin.Name = "TextBoxUserLogin"
        Me.TextBoxUserLogin.Size = New System.Drawing.Size(234, 21)
        Me.TextBoxUserLogin.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Tahoma", 10.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.Location = New System.Drawing.Point(4, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(233, 23)
        Me.Label1.Text = "Login"
        '
        'ButtonLogin
        '
        Me.ButtonLogin.Location = New System.Drawing.Point(3, 84)
        Me.ButtonLogin.Name = "ButtonLogin"
        Me.ButtonLogin.Size = New System.Drawing.Size(234, 41)
        Me.ButtonLogin.TabIndex = 2
        Me.ButtonLogin.Text = "Login"
        '
        'DecodeComponent1
        '
        Me.DecodeComponent1.DecodeMode = HandHeldProducts.Embedded.Decoding.DecodeAssembly.DecodeModes.Standard
        Me.DecodeComponent1.ScanKey = HandHeldProducts.Embedded.Decoding.DecodeComponent.ScanKeys.Scan
        Me.DecodeComponent1.ScanKeyOperation = HandHeldProducts.Embedded.Decoding.DecodeComponent.ScanKeyOptions.ScanBarcode
        Me.DecodeComponent1.ScanningLightsMode = HandHeldProducts.Embedded.Decoding.DecodeAssembly.ScanningLightsModes.AimerAndIllumination
		'
        'ButtonCloseApp
        '
        Me.ButtonCloseApp.Location = New System.Drawing.Point(3, 131)
        Me.ButtonCloseApp.Name = "ButtonCloseApp"
        Me.ButtonCloseApp.Size = New System.Drawing.Size(234, 41)
        Me.ButtonCloseApp.TabIndex = 4
        Me.ButtonCloseApp.Text = "Close Application"
        '
        'FormLogin
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(240, 294)
        Me.Controls.Add(Me.ButtonCloseApp)
        Me.Controls.Add(Me.ButtonLogin)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextBoxUserLogin)
        Me.Name = "FormLogin"
        Me.Text = "Loading..."
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TextBoxUserLogin As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ButtonLogin As System.Windows.Forms.Button
    Friend WithEvents DecodeComponent1 As HandHeldProducts.Embedded.Decoding.DecodeComponent
    Friend WithEvents ButtonCloseApp As System.Windows.Forms.Button

End Class
