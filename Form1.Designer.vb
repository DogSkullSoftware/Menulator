<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits WindowExtension

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.pnlRight = New Menulator_Zero.PanelEx()
        Me.chkMature = New System.Windows.Forms.CheckBox()
        Me.cboYearOp = New Menulator_Zero.ComboBoxEx()
        Me.cboStatus = New Menulator_Zero.ComboBoxEx()
        Me.cboPlayersOp = New Menulator_Zero.ComboBoxEx()
        Me.btnFilterClear = New System.Windows.Forms.Button()
        Me.btnFilter = New System.Windows.Forms.Button()
        Me.txtYear = New Menulator_Zero.TextBoxEx()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtPlayers = New Menulator_Zero.TextBoxEx()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtRating = New Menulator_Zero.TextBoxEx()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtGenre = New Menulator_Zero.TextBoxEx()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtDescription = New Menulator_Zero.TextBoxEx()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlLeft = New Menulator_Zero.PanelEx()
        Me.imgMenulatorIcon = New Menulator_Zero.PanelEx()
        Me.imgMenulator = New Menulator_Zero.PanelEx()
        Me.lblTime = New System.Windows.Forms.Label()
        Me.pnlRight.SuspendLayout()
        Me.pnlLeft.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 64
        '
        'pnlRight
        '
        Me.pnlRight.AnimateTime = 150
        Me.pnlRight.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlRight.Controls.Add(Me.chkMature)
        Me.pnlRight.Controls.Add(Me.cboYearOp)
        Me.pnlRight.Controls.Add(Me.cboStatus)
        Me.pnlRight.Controls.Add(Me.cboPlayersOp)
        Me.pnlRight.Controls.Add(Me.btnFilterClear)
        Me.pnlRight.Controls.Add(Me.btnFilter)
        Me.pnlRight.Controls.Add(Me.txtYear)
        Me.pnlRight.Controls.Add(Me.Label4)
        Me.pnlRight.Controls.Add(Me.Label3)
        Me.pnlRight.Controls.Add(Me.txtPlayers)
        Me.pnlRight.Controls.Add(Me.Label2)
        Me.pnlRight.Controls.Add(Me.txtRating)
        Me.pnlRight.Controls.Add(Me.Label6)
        Me.pnlRight.Controls.Add(Me.txtGenre)
        Me.pnlRight.Controls.Add(Me.Label5)
        Me.pnlRight.Controls.Add(Me.txtDescription)
        Me.pnlRight.Controls.Add(Me.Label1)
        Me.pnlRight.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlRight.Font = New System.Drawing.Font("Segoe UI", 18.0!)
        Me.pnlRight.Location = New System.Drawing.Point(710, 0)
        Me.pnlRight.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnlRight.Name = "pnlRight"
        Me.pnlRight.Size = New System.Drawing.Size(287, 777)
        Me.pnlRight.TabIndex = 0
        Me.pnlRight.Visible = False
        '
        'chkMature
        '
        Me.chkMature.AutoSize = True
        Me.chkMature.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.chkMature.Location = New System.Drawing.Point(18, 581)
        Me.chkMature.Name = "chkMature"
        Me.chkMature.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.chkMature.Size = New System.Drawing.Size(171, 39)
        Me.chkMature.TabIndex = 9

        Me.chkMature.Text = "Show Mature"
        Me.chkMature.UseCompatibleTextRendering = True
        Me.chkMature.UseVisualStyleBackColor = True
        '
        'cboYearOp
        '
        Me.cboYearOp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboYearOp.FormattingEnabled = True
        Me.cboYearOp.Items.AddRange(New Object() {"=", "<=", ">="})
        Me.cboYearOp.Location = New System.Drawing.Point(18, 349)
        Me.cboYearOp.Name = "cboYearOp"
        Me.cboYearOp.Size = New System.Drawing.Size(62, 40)
        Me.cboYearOp.TabIndex = 5
        '
        'cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.FormattingEnabled = True
        Me.cboStatus.Items.AddRange(New Object() {"", "imperfect", "good", "preliminary"})
        Me.cboStatus.Location = New System.Drawing.Point(18, 259)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(250, 40)
        Me.cboStatus.TabIndex = 4
        '
        'cboPlayersOp
        '
        Me.cboPlayersOp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPlayersOp.FormattingEnabled = True
        Me.cboPlayersOp.Items.AddRange(New Object() {"=", "<=", ">="})
        Me.cboPlayersOp.Location = New System.Drawing.Point(18, 178)
        Me.cboPlayersOp.Name = "cboPlayersOp"
        Me.cboPlayersOp.Size = New System.Drawing.Size(62, 40)
        Me.cboPlayersOp.TabIndex = 2
        '
        'btnFilterClear
        '
        Me.btnFilterClear.Location = New System.Drawing.Point(18, 12)
        Me.btnFilterClear.Name = "btnFilterClear"
        Me.btnFilterClear.Size = New System.Drawing.Size(250, 43)
        Me.btnFilterClear.TabIndex = 0
        Me.btnFilterClear.Text = "Clear Filter"
        Me.btnFilterClear.UseVisualStyleBackColor = True
        '
        'btnFilter
        '
        Me.btnFilter.Location = New System.Drawing.Point(9, 636)
        Me.btnFilter.Name = "btnFilter"
        Me.btnFilter.Size = New System.Drawing.Size(250, 43)
        Me.btnFilter.TabIndex = 10
        Me.btnFilter.Text = "Filter"
        Me.btnFilter.UseVisualStyleBackColor = True
        '
        'txtYear
        '
        Me.txtYear.IsNumeric = True
        Me.txtYear.Location = New System.Drawing.Point(86, 350)
        Me.txtYear.MaxNumber = 2021
        Me.txtYear.MinNumber = 1971
        Me.txtYear.Name = "txtYear"
        Me.txtYear.Size = New System.Drawing.Size(182, 39)
        Me.txtYear.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(17, 302)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 32)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Year"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(17, 224)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(79, 32)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Status"
        '
        'txtPlayers
        '
        Me.txtPlayers.IsNumeric = True
        Me.txtPlayers.Location = New System.Drawing.Point(86, 178)
        Me.txtPlayers.MaxNumber = 16
        Me.txtPlayers.MinNumber = 1
        Me.txtPlayers.Name = "txtPlayers"
        Me.txtPlayers.Size = New System.Drawing.Size(182, 39)
        Me.txtPlayers.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(17, 142)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 32)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Players"
        '
        'txtRating
        '
        Me.txtRating.IsNumeric = True
        Me.txtRating.Location = New System.Drawing.Point(18, 525)
        Me.txtRating.MaxNumber = 10
        Me.txtRating.MinNumber = 1
        Me.txtRating.Name = "txtRating"
        Me.txtRating.Size = New System.Drawing.Size(250, 39)
        Me.txtRating.TabIndex = 8
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(17, 481)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(83, 32)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Rating"
        '
        'txtGenre
        '
        Me.txtGenre.IsNumeric = False
        Me.txtGenre.Location = New System.Drawing.Point(18, 427)
        Me.txtGenre.MaxNumber = 2147483646
        Me.txtGenre.MinNumber = 0
        Me.txtGenre.Name = "txtGenre"
        Me.txtGenre.Size = New System.Drawing.Size(250, 39)
        Me.txtGenre.TabIndex = 7
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(17, 392)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(79, 32)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Genre"
        '
        'txtDescription
        '
        Me.txtDescription.IsNumeric = False
        Me.txtDescription.Location = New System.Drawing.Point(18, 100)
        Me.txtDescription.MaxNumber = 2147483646
        Me.txtDescription.MinNumber = 0
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(250, 39)
        Me.txtDescription.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(17, 58)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 32)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Name"
        '
        'pnlLeft
        '
        Me.pnlLeft.AnimateTime = 150
        Me.pnlLeft.Animation = Menulator_Zero.PanelEx.AnimateWindowFlags.AW_VER_POSITIVE
        Me.pnlLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.pnlLeft.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlLeft.Controls.Add(Me.imgMenulatorIcon)
        Me.pnlLeft.Controls.Add(Me.imgMenulator)
        Me.pnlLeft.Controls.Add(Me.lblTime)
        Me.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlLeft.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.pnlLeft.Location = New System.Drawing.Point(0, 0)
        Me.pnlLeft.Name = "pnlLeft"
        Me.pnlLeft.Size = New System.Drawing.Size(295, 777)
        Me.pnlLeft.TabIndex = 2
        Me.pnlLeft.Visible = False
        '
        'imgMenulatorIcon
        '
        Me.imgMenulatorIcon.AnimateTime = 150
        Me.imgMenulatorIcon.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.Menulator_Icon
        Me.imgMenulatorIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.imgMenulatorIcon.Location = New System.Drawing.Point(8, 12)
        Me.imgMenulatorIcon.Name = "imgMenulatorIcon"
        Me.imgMenulatorIcon.Size = New System.Drawing.Size(52, 73)
        Me.imgMenulatorIcon.TabIndex = 10
        Me.imgMenulatorIcon.Visible = False
        '
        'imgMenulator
        '
        Me.imgMenulator.AnimateTime = 150
        Me.imgMenulator.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.Menulator_logo
        Me.imgMenulator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.imgMenulator.Dock = System.Windows.Forms.DockStyle.Top
        Me.imgMenulator.Location = New System.Drawing.Point(0, 0)
        Me.imgMenulator.Name = "imgMenulator"
        Me.imgMenulator.Size = New System.Drawing.Size(295, 100)
        Me.imgMenulator.TabIndex = 0
        Me.imgMenulator.Visible = False
        '
        'lblTime
        '
        Me.lblTime.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTime.BackColor = System.Drawing.Color.Transparent
        Me.lblTime.Font = New System.Drawing.Font("Segoe UI", 18.0!)
        Me.lblTime.ForeColor = System.Drawing.Color.Black
        Me.lblTime.Location = New System.Drawing.Point(3, 736)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.lblTime.Size = New System.Drawing.Size(286, 32)
        Me.lblTime.TabIndex = 3
        Me.lblTime.Text = "Label12"



        '
        'Form1
        '
        Me.AcceptButton = Me.btnFilter
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(997, 777)
        Me.Controls.Add(Me.pnlLeft)
        Me.Controls.Add(Me.pnlRight)
        Me.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "Form1"
        Me.Text = "Menulator"
        Me.TransparencyKey = System.Drawing.Color.Green
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlRight.ResumeLayout(False)
        Me.pnlRight.PerformLayout()
        Me.pnlLeft.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Timer1 As Timer
    Friend WithEvents pnlRight As PanelEx
    Friend WithEvents btnFilter As Button
    Friend WithEvents txtDescription As TextboxEx
    Friend WithEvents Label1 As Label
    Friend WithEvents cboPlayersOp As ComboBoxEx
    Friend WithEvents txtYear As TextboxEx
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtPlayers As TextboxEx
    Friend WithEvents Label2 As Label
    Friend WithEvents cboStatus As ComboBoxEx
    Friend WithEvents cboYearOp As ComboBoxEx
    Friend WithEvents txtRating As TextboxEx
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents txtGenre As TextBoxEx
    Friend WithEvents chkMature As CheckBox
    Friend WithEvents btnFilterClear As Button
    Friend WithEvents pnlLeft As PanelEx
    Friend WithEvents imgMenulatorIcon As PanelEx
    'Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanelEx
    'Friend WithEvents pnlGames As Panel
    'Friend WithEvents Panel4 As Panel
    'Friend WithEvents Label14 As Label
    'Friend WithEvents pnlMusic As Panel
    'Friend WithEvents Panel5 As Panel
    'Friend WithEvents Label15 As Label
    'Friend WithEvents pnlNetwork As Panel
    'Friend WithEvents Panel7 As Panel
    'Friend WithEvents Label16 As Label
    'Friend WithEvents pnlApplication As Panel
    'Friend WithEvents Panel9 As Panel
    'Friend WithEvents Label7 As Label
    'Friend WithEvents pnlFiles As Panel
    'Friend WithEvents Panel11 As Panel
    'Friend WithEvents Label8 As Label
    'Friend WithEvents pnlSettings As Panel
    'Friend WithEvents Panel13 As Panel
    'Friend WithEvents Label9 As Label
    'Friend WithEvents pnlHelp As Panel
    'Friend WithEvents Panel15 As Panel
    'Friend WithEvents Label10 As Label
    'Friend WithEvents pnlPower As Panel
    'Friend WithEvents Panel17 As Panel
    'Friend WithEvents Label11 As Label
    Friend WithEvents imgMenulator As PanelEx

    Friend WithEvents lblTime As Label
End Class
