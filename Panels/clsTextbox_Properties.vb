Public Class Textbox_Properties
    Inherits UserControl

    Friend WithEvents txtDescription As TextBox
    Friend WithEvents Label1 As Label

    Public Sub New(txtLabelText As String, Optional txtDefaultValue As String = "")
        InitializeComponent()
        Label1.Text = txtLabelText
        txtDescription.Text = txtDefaultValue
    End Sub

    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 11.25!)
        Me.Label1.Location = New System.Drawing.Point(3, 4)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Label1"
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(3, 27)
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(250, 27)
        Me.txtDescription.TabIndex = 2
        '
        'Textbox_Properties
        '
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 11.25!)
        Me.Name = "Textbox_Properties"
        Me.Size = New System.Drawing.Size(256, 68)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private Sub txtDescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDescription.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If TypeOf sender Is ComboBox Then
                    With DirectCast(sender, ComboBox)
                        If .DroppedDown Then
                            Return
                        End If
                    End With
                End If
                SendKeys.Send("+{TAB}")
                e.Handled = True

            Case Keys.Down
                If TypeOf sender Is ComboBox Then
                    With DirectCast(sender, ComboBox)
                        If .DroppedDown Then
                            Return
                        End If
                    End With
                End If
                SendKeys.Send("{TAB}")
                e.Handled = True
            Case Keys.Return
                If TypeOf sender Is ComboBox Then
                    With DirectCast(sender, ComboBox)
                        If .DroppedDown Then
                            .DroppedDown = False
                        Else
                            .DroppedDown = True
                        End If
                    End With
                    e.Handled = True
                ElseIf TypeOf sender Is CheckBox Then
                    DirectCast(sender, CheckBox).Checked = Not sender.checked
                End If
        End Select
    End Sub

End Class
