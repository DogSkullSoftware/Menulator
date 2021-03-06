
Public Class frmMsg

    Dim msg As String
    Dim _foreColor As SolidBrush

    Public Sub New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' Me.AccentState = eAccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND
        Me.EnableBlur()
        Font = New Font("Malgun Gothic", 25.0F)
        SetStyle(ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        'BackColor = Color.LimeGreen
        'TransparencyKey = BackColor
        _foreColor = New SolidBrush(Me.ForeColor)
    End Sub

    Protected Overrides Sub OnForeColorChanged(e As EventArgs)
        MyBase.OnForeColorChanged(e)
        _foreColor = New SolidBrush(Me.ForeColor)
    End Sub
    Public JoyTestOutput As Object
    Dim JoyTestMode As Boolean
    Public Sub New(strMsg As String, Optional buttons As MsgBoxStyle = MsgBoxStyle.ApplicationModal Or MsgBoxStyle.OkOnly, Optional Title As String = "")
        Me.New


        ' Add any initialization after the InitializeComponent() call.
        If buttons = Integer.MaxValue Then
            JoyTestMode = True

        End If

        If (buttons And MsgBoxStyle.ApplicationModal) = MsgBoxStyle.ApplicationModal Then

        End If
        If (buttons And MsgBoxStyle.Critical) = MsgBoxStyle.Critical Then
            lblIcon.BackColor = Color.Red
            lblIcon.Text = "⚠"
        End If

        If (buttons And MsgBoxStyle.Exclamation) = MsgBoxStyle.Exclamation Then
            lblIcon.BackColor = Color.Orange
            lblIcon.Text = "❕"
        End If
        If (buttons And MsgBoxStyle.Information) = MsgBoxStyle.Information Then
            lblIcon.BackColor = Color.Blue
            lblIcon.Text = "ⅈ"
        End If
        If (buttons And MsgBoxStyle.MsgBoxHelp) = MsgBoxStyle.MsgBoxHelp Then
            HelpButton = True
        End If
        If (buttons And MsgBoxStyle.MsgBoxRight) = MsgBoxStyle.MsgBoxRight Then

        End If
        If (buttons And MsgBoxStyle.MsgBoxRtlReading) = MsgBoxStyle.MsgBoxRtlReading Then

        End If
        If (buttons And MsgBoxStyle.MsgBoxSetForeground) = MsgBoxStyle.MsgBoxSetForeground Then

        End If
        If (buttons And MsgBoxStyle.Question) = MsgBoxStyle.Question Then
            lblIcon.BackColor = Color.LightGray
            lblIcon.Text = "❓"
        End If
        If (buttons And MsgBoxStyle.SystemModal) = MsgBoxStyle.SystemModal Then

        End If


        Dim buttonset As Boolean = False
        If (buttons And MsgBoxStyle.DefaultButton2) = MsgBoxStyle.DefaultButton2 Then
            AcceptButton = Button2

            Button2.Select()
            buttonset = True
        End If
        If (buttons And MsgBoxStyle.DefaultButton3) = MsgBoxStyle.DefaultButton3 Then
            AcceptButton = Button3
            Button3.Select()
            buttonset = True
        End If
        If buttonset = False Then '(buttons And MsgBoxStyle.DefaultButton1) = MsgBoxStyle.DefaultButton1 Then
            AcceptButton = Button1
            Button1.Select()
        End If
        buttonset = False
        If (buttons And MsgBoxStyle.AbortRetryIgnore) = MsgBoxStyle.AbortRetryIgnore Then
            Button1.Text = "Abort"
            Button2.Text = "Retry"
            Button3.Text = "Ignore"
            buttonset = True
        End If
        If (buttons And MsgBoxStyle.OkCancel) = MsgBoxStyle.OkCancel Then
            Button1.Text = "OK"
            Button1.Tag = DialogResult.OK
            Button2.Text = "Cancel"
            Button2.Tag = DialogResult.Cancel
            Controls.Remove(Button3)
            buttonset = True
        End If
        If (buttons And MsgBoxStyle.RetryCancel) = MsgBoxStyle.RetryCancel Then
            Button1.Text = "Retry"
            Button1.Tag = DialogResult.Retry
            Button2.Text = "Cancel"
            Button2.Tag = DialogResult.Cancel
            Controls.Remove(Button3)
            buttonset = True
        End If

        If (buttons And MsgBoxStyle.YesNo) = MsgBoxStyle.YesNo Then
            Button1.Text = "Yes"
            Button1.Tag = DialogResult.Yes
            Button2.Text = "No"
            Button2.Tag = DialogResult.No
            Controls.Remove(Button3)
            buttonset = True
        End If
        If (buttons And MsgBoxStyle.YesNoCancel) = MsgBoxStyle.YesNoCancel Then
            Button1.Text = "Yes"
            Button1.Tag = DialogResult.Yes
            Button2.Text = "No"
            Button2.Tag = DialogResult.No
            Button3.Text = "Cancel"
            Button3.Tag = DialogResult.Cancel
            buttonset = True
        End If
        If buttonset = False Then ' (buttons And MsgBoxStyle.OkOnly) = MsgBoxStyle.OkOnly Then
            Button1.Text = "OK"
            Button1.Tag = DialogResult.OK
            Controls.Remove(Button2)
            Controls.Remove(Button3)

        End If

        'msg = strMsg
        Label1.Text = Title.ToUpper
        Label2.Text = strMsg
        Dim testRect As SizeF = Label2.Size
        Using g As Graphics = Graphics.FromHwnd(Me.Handle)
            'testRect = TextRenderer.MeasureText(strMsg, Label2.Font, testRect, TextFormatFlags.)
            testRect = g.MeasureString(strMsg, Label2.Font, Label2.Width)
        End Using
        Label2.Height = Math.Max(testRect.Height, 34)
        Height = Math.Min(Label2.Height + 132 + 5, Screen.FromControl(Me).WorkingArea.Height - 25)
        'Width += 32
        'Height += 54 + 25

        'Button1.TurnOffRender = True
        'Button2.TurnOffRender = True
        'Button3.TurnOffRender = True
        'Label1.TurnOffRender = True
        'Label2.TurnOffRender = True
        'lblIcon.TurnOffRender = True
    End Sub

    Public Shared Function Msgbox(strMsg As String, Optional buttons As MsgBoxStyle = MsgBoxStyle.ApplicationModal Or MsgBoxStyle.OkOnly, Optional Title As String = "") As MsgBoxResult
        Dim f As New frmMsg(strMsg, buttons, Title)
        Return f.ShowDialog(My.Forms.Form1)
    End Function
    Public BackgroundColor As Color = Color.FromArgb(45, 45, 48)
    Dim iconB As New SolidBrush(BackgroundColor)
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        'e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        'e.Graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
        'e.Graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBilinear
        'e.Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        'e.Graphics.TranslateTransform(Button1.Left, Button1.Top)
        'Button1.DoPaint(e)
        'e.Graphics.TranslateTransform(-Button1.Left, -Button1.Top)
        'e.Graphics.TranslateTransform(Button2.Left, Button2.Top)
        'Button2.DoPaint(e)
        'e.Graphics.TranslateTransform(-Button2.Left, -Button2.Top)
        'e.Graphics.TranslateTransform(Button3.Left, Button3.Top)
        'Button3.DoPaint(e)
        'e.Graphics.TranslateTransform(-Button3.Left, -Button3.Top)

        'e.Graphics.TranslateTransform(Label1.Left, Label1.Top)
        'Label1.DoPaint(e)
        'e.Graphics.TranslateTransform(-Label1.Left, -Label1.Top)
        'e.Graphics.TranslateTransform(Label2.Left, Label2.Top)
        'Label2.DoPaint(e)
        'e.Graphics.TranslateTransform(-Label2.Left, -Label2.Top)
        'e.Graphics.TranslateTransform(lblIcon.Left, lblIcon.Top)
        'lblIcon.DoPaint(e)
        'e.Graphics.TranslateTransform(-lblIcon.Left, -lblIcon.Top)

    End Sub
    Private Sub Button1_GotFocus(sender As Object, e As EventArgs) Handles Button1.GotFocus, Button1.LostFocus, Button2.GotFocus, Button2.LostFocus, Button3.GotFocus, Button3.LostFocus
        Refresh()
    End Sub

    Protected Friend Overrides Sub OnJoyStickAxisDown(sender As Object, e As JoyApi.Joystick.JoyStickAxisEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickAxisDown(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickAxisPress(sender As Object, e As JoyApi.Joystick.JoyStickAxisPressEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickAxisPress(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickAxisUp(sender As Object, e As JoyApi.Joystick.JoyStickAxisEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickAxisUp(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickButtonDown(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickButtonDown(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickButtonPress(sender As Object, e As JoyApi.Joystick.JoyStickButtonPressEventArgs)

        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickButtonPress(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickButtonUp(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickButtonUp(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoystickChanged(sender As Object, e As JoyApi.Joystick.JoyStickChangedArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoystickChanged(sender, e)
    End Sub
    'Protected Friend Overrides Sub OnJoyStickPoll(sender As Object, e As JoyApi.Joystick.JoyStickChangedArgs)
    '    If JoyTestMode Then
    '        JoyTestOutput = {sender, e}
    '        Me.DialogResult = DialogResult.OK
    '        Me.Close()
    '    End If
    '    MyBase.OnJoyStickPoll(sender, e)
    'End Sub
    Protected Friend Overrides Sub OnJoyStickPOVDown(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickPOVDown(sender, e)
    End Sub
    Protected Friend Overrides Sub OnJoyStickPOVPress(sender As Object, e As JoyApi.Joystick.JoyStickButtonPressEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickPOVPress(sender, e)

    End Sub
    Protected Friend Overrides Sub OnJoyStickPOVUp(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs)
        If JoyTestMode Then
            JoyTestOutput = {sender, e}
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
        MyBase.OnJoyStickPOVUp(sender, e)
    End Sub
    'Protected Friend Overrides Sub OnJoyStickPrePoll(sender As Object, e As EventArgs)
    '    If JoyTestMode Then
    '        JoyTestOutput = {sender, e}
    '        Me.DialogResult = DialogResult.OK
    '        Me.Close()
    '    End If
    '    MyBase.OnJoyStickPrePoll(sender, e)
    'End Sub
    'Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
    '    Return False
    'End Function
    'Public Overrides Function PreProcessMessage(ByRef msg As Message) As Boolean
    '    Return False
    'End Function
    'Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
    '    Return False
    'End Function
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Return
                If ActiveControl Is Button1 Then
                    Me.DialogResult = Button1.Tag
                ElseIf Activecontrol Is Button2 Then
                    Me.DialogResult = Button2.Tag
                ElseIf ActiveControl Is Button3 Then
                    Me.DialogResult = Button3.Tag
                Else
                    Me.DialogResult = DialogResult.None
                End If
                Me.Close()

            Case Keys.Escape
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            Case Else
                MyBase.OnKeyDown(e)
        End Select
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click, Button2.Click, Button3.Click
        Me.DialogResult = sender.tag
        Me.Close()
    End Sub

End Class