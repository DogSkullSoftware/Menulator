Public Class RangeNumeric_Properties
    Inherits UserControl

    Friend WithEvents Combobox1 As ComboBox
    Friend WithEvents Number1 As NumericUpDown
    Friend WithEvents Label1 As Label

    Public Sub New(txtLabelText As String, txtDefaultValue As String, cboDefaultValue As String, ParamArray Values() As String)
        InitializeComponent()
        Label1.Text = txtLabelText
        Number1.Text = txtDefaultValue
        Combobox1.Items.AddRange(Values)
        Combobox1.Text = cboDefaultValue
    End Sub

    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Combobox1 = New System.Windows.Forms.ComboBox()
        Me.Number1 = New System.Windows.Forms.NumericUpDown()
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
        'Combobox1
        '
        Me.Combobox1.Location = New System.Drawing.Point(3, 27)
        Me.Combobox1.Name = "Combobox1"
        Me.Combobox1.Size = New System.Drawing.Size(80, 28)
        Me.Combobox1.TabIndex = 2
        '
        'TextBox1
        '
        Me.Number1.Location = New System.Drawing.Point(89, 27)
        Me.Number1.Name = "TextBox1"
        Me.Number1.Size = New System.Drawing.Size(164, 27)
        Me.Number1.TabIndex = 3
        '
        'Range_Properties
        '
        Me.Controls.Add(Me.Number1)
        Me.Controls.Add(Me.Combobox1)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 11.25!)
        Me.Name = "Range_Properties"
        Me.Size = New System.Drawing.Size(256, 68)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private Sub txtDescription_KeyDown(sender As Object, e As KeyEventArgs) Handles Combobox1.KeyDown, Number1.KeyDown
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
