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
        'Me.FlowLayoutPanel1 = New Menulator_Zero.FlowLayoutPanelEx()
        'Me.pnlGames = New System.Windows.Forms.Panel()
        'Me.Panel4 = New System.Windows.Forms.Panel()
        'Me.Label14 = New System.Windows.Forms.Label()
        'Me.pnlMusic = New System.Windows.Forms.Panel()
        'Me.Panel5 = New System.Windows.Forms.Panel()
        'Me.Label15 = New System.Windows.Forms.Label()
        'Me.pnlNetwork = New System.Windows.Forms.Panel()
        'Me.Panel7 = New System.Windows.Forms.Panel()
        'Me.Label16 = New System.Windows.Forms.Label()
        'Me.pnlApplication = New System.Windows.Forms.Panel()
        'Me.Panel9 = New System.Windows.Forms.Panel()
        'Me.Label7 = New System.Windows.Forms.Label()
        'Me.pnlFiles = New System.Windows.Forms.Panel()
        'Me.Panel11 = New System.Windows.Forms.Panel()
        'Me.Label8 = New System.Windows.Forms.Label()
        'Me.pnlSettings = New System.Windows.Forms.Panel()
        'Me.Panel13 = New System.Windows.Forms.Panel()
        'Me.Label9 = New System.Windows.Forms.Label()
        'Me.pnlHelp = New System.Windows.Forms.Panel()
        'Me.Panel15 = New System.Windows.Forms.Panel()
        'Me.Label10 = New System.Windows.Forms.Label()
        'Me.pnlPower = New System.Windows.Forms.Panel()
        'Me.Panel17 = New System.Windows.Forms.Panel()
        'Me.Label11 = New System.Windows.Forms.Label()
        Me.imgMenulator = New Menulator_Zero.PanelEx()
        Me.lblTime = New System.Windows.Forms.Label()
        Me.pnlRight.SuspendLayout()
        Me.pnlLeft.SuspendLayout()
        'Me.FlowLayoutPanel1.SuspendLayout()
        'Me.pnlGames.SuspendLayout()
        'Me.pnlMusic.SuspendLayout()
        'Me.pnlNetwork.SuspendLayout()
        'Me.pnlApplication.SuspendLayout()
        'Me.pnlFiles.SuspendLayout()
        'Me.pnlSettings.SuspendLayout()
        'Me.pnlHelp.SuspendLayout()
        'Me.pnlPower.SuspendLayout()
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
        Me.chkMature.Text = "Block Mature"
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
        'Me.pnlLeft.Controls.Add(Me.FlowLayoutPanel1)
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
        ''
        ''FlowLayoutPanel1
        ''
        'Me.FlowLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
        '    Or System.Windows.Forms.AnchorStyles.Left) _
        '    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        'Me.FlowLayoutPanel1.AnimateTime = 150
        'Me.FlowLayoutPanel1.AutoScroll = True
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlGames)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlMusic)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlNetwork)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlApplication)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlFiles)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlSettings)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlHelp)
        'Me.FlowLayoutPanel1.Controls.Add(Me.pnlPower)
        'Me.FlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        'Me.FlowLayoutPanel1.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        'Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 100)
        'Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        'Me.FlowLayoutPanel1.Size = New System.Drawing.Size(295, 633)
        'Me.FlowLayoutPanel1.TabIndex = 1
        'Me.FlowLayoutPanel1.Visible = False
        'Me.FlowLayoutPanel1.WrapContents = False
        ''
        ''pnlGames
        ''
        'Me.pnlGames.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlGames.Controls.Add(Me.Panel4)
        'Me.pnlGames.Controls.Add(Me.Label14)
        'Me.pnlGames.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlGames.Location = New System.Drawing.Point(3, 3)
        'Me.pnlGames.Name = "pnlGames"
        'Me.pnlGames.Size = New System.Drawing.Size(286, 48)
        'Me.pnlGames.TabIndex = 8
        ''
        ''Panel4
        ''
        'Me.Panel4.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_games
        'Me.Panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel4.Location = New System.Drawing.Point(6, 3)
        'Me.Panel4.Name = "Panel4"
        'Me.Panel4.Size = New System.Drawing.Size(52, 42)
        'Me.Panel4.TabIndex = 2
        ''
        ''Label14
        ''
        'Me.Label14.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label14.Location = New System.Drawing.Point(64, 0)
        'Me.Label14.Name = "Label14"
        'Me.Label14.Size = New System.Drawing.Size(222, 48)
        'Me.Label14.TabIndex = 8
        'Me.Label14.Text = "Games"
        'Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlMusic
        ''
        'Me.pnlMusic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlMusic.Controls.Add(Me.Panel5)
        'Me.pnlMusic.Controls.Add(Me.Label15)
        'Me.pnlMusic.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlMusic.Location = New System.Drawing.Point(3, 57)
        'Me.pnlMusic.Name = "pnlMusic"
        'Me.pnlMusic.Size = New System.Drawing.Size(286, 48)
        'Me.pnlMusic.TabIndex = 9
        ''
        ''Panel5
        ''
        'Me.Panel5.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_audio
        'Me.Panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel5.Location = New System.Drawing.Point(6, 3)
        'Me.Panel5.Name = "Panel5"
        'Me.Panel5.Size = New System.Drawing.Size(52, 42)
        'Me.Panel5.TabIndex = 2
        ''
        ''Label15
        ''
        'Me.Label15.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label15.Location = New System.Drawing.Point(64, 0)
        'Me.Label15.Name = "Label15"
        'Me.Label15.Size = New System.Drawing.Size(222, 48)
        'Me.Label15.TabIndex = 8
        'Me.Label15.Text = "Media"
        'Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlNetwork
        ''
        'Me.pnlNetwork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlNetwork.Controls.Add(Me.Panel7)
        'Me.pnlNetwork.Controls.Add(Me.Label16)
        'Me.pnlNetwork.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlNetwork.Location = New System.Drawing.Point(3, 111)
        'Me.pnlNetwork.Name = "pnlNetwork"
        'Me.pnlNetwork.Size = New System.Drawing.Size(286, 48)
        'Me.pnlNetwork.TabIndex = 10
        ''
        ''Panel7
        ''
        'Me.Panel7.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_globe
        'Me.Panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel7.Location = New System.Drawing.Point(6, 3)
        'Me.Panel7.Name = "Panel7"
        'Me.Panel7.Size = New System.Drawing.Size(52, 42)
        'Me.Panel7.TabIndex = 2
        ''
        ''Label16
        ''
        'Me.Label16.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label16.Location = New System.Drawing.Point(64, 0)
        'Me.Label16.Name = "Label16"
        'Me.Label16.Size = New System.Drawing.Size(222, 48)
        'Me.Label16.TabIndex = 8
        'Me.Label16.Text = "Internet"
        'Me.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlApplication
        ''
        'Me.pnlApplication.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlApplication.Controls.Add(Me.Panel9)
        'Me.pnlApplication.Controls.Add(Me.Label7)
        'Me.pnlApplication.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlApplication.Location = New System.Drawing.Point(3, 165)
        'Me.pnlApplication.Name = "pnlApplication"
        'Me.pnlApplication.Size = New System.Drawing.Size(286, 48)
        'Me.pnlApplication.TabIndex = 11
        ''
        ''Panel9
        ''
        'Me.Panel9.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_gotostart
        'Me.Panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel9.Location = New System.Drawing.Point(6, 3)
        'Me.Panel9.Name = "Panel9"
        'Me.Panel9.Size = New System.Drawing.Size(52, 42)
        'Me.Panel9.TabIndex = 2
        ''
        ''Label7
        ''
        'Me.Label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label7.Location = New System.Drawing.Point(64, 0)
        'Me.Label7.Name = "Label7"
        'Me.Label7.Size = New System.Drawing.Size(222, 48)
        'Me.Label7.TabIndex = 8
        'Me.Label7.Text = "Application"
        'Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlFiles
        ''
        'Me.pnlFiles.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlFiles.Controls.Add(Me.Panel11)
        'Me.pnlFiles.Controls.Add(Me.Label8)
        'Me.pnlFiles.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlFiles.Location = New System.Drawing.Point(3, 219)
        'Me.pnlFiles.Name = "pnlFiles"
        'Me.pnlFiles.Size = New System.Drawing.Size(286, 48)
        'Me.pnlFiles.TabIndex = 12
        ''
        ''Panel11
        ''
        'Me.Panel11.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_folder
        'Me.Panel11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel11.Location = New System.Drawing.Point(6, 3)
        'Me.Panel11.Name = "Panel11"
        'Me.Panel11.Size = New System.Drawing.Size(52, 42)
        'Me.Panel11.TabIndex = 2
        ''
        ''Label8
        ''
        'Me.Label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label8.Location = New System.Drawing.Point(64, 0)
        'Me.Label8.Name = "Label8"
        'Me.Label8.Size = New System.Drawing.Size(222, 48)
        'Me.Label8.TabIndex = 8
        'Me.Label8.Text = "File Explorer"
        'Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlSettings
        ''
        'Me.pnlSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlSettings.Controls.Add(Me.Panel13)
        'Me.pnlSettings.Controls.Add(Me.Label9)
        'Me.pnlSettings.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlSettings.Location = New System.Drawing.Point(3, 273)
        'Me.pnlSettings.Name = "pnlSettings"
        'Me.pnlSettings.Size = New System.Drawing.Size(286, 48)
        'Me.pnlSettings.TabIndex = 13
        ''
        ''Panel13
        ''
        'Me.Panel13.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_settings2
        'Me.Panel13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel13.Location = New System.Drawing.Point(6, 3)
        'Me.Panel13.Name = "Panel13"
        'Me.Panel13.Size = New System.Drawing.Size(52, 42)
        'Me.Panel13.TabIndex = 2
        ''
        ''Label9
        ''
        'Me.Label9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label9.Location = New System.Drawing.Point(64, 0)
        'Me.Label9.Name = "Label9"
        'Me.Label9.Size = New System.Drawing.Size(222, 48)
        'Me.Label9.TabIndex = 8
        'Me.Label9.Text = "Control Panel"
        'Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlHelp
        ''
        'Me.pnlHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlHelp.Controls.Add(Me.Panel15)
        'Me.pnlHelp.Controls.Add(Me.Label10)
        'Me.pnlHelp.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlHelp.Location = New System.Drawing.Point(3, 327)
        'Me.pnlHelp.Name = "pnlHelp"
        'Me.pnlHelp.Size = New System.Drawing.Size(286, 48)
        'Me.pnlHelp.TabIndex = 14
        ''
        ''Panel15
        ''
        'Me.Panel15.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_help
        'Me.Panel15.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel15.Location = New System.Drawing.Point(6, 3)
        'Me.Panel15.Name = "Panel15"
        'Me.Panel15.Size = New System.Drawing.Size(52, 42)
        'Me.Panel15.TabIndex = 2
        ''
        ''Label10
        ''
        'Me.Label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label10.Location = New System.Drawing.Point(64, 0)
        'Me.Label10.Name = "Label10"
        'Me.Label10.Size = New System.Drawing.Size(222, 48)
        'Me.Label10.TabIndex = 8
        'Me.Label10.Text = "Help"
        'Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        ''
        ''pnlPower
        ''
        'Me.pnlPower.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        'Me.pnlPower.Controls.Add(Me.Panel17)
        'Me.pnlPower.Controls.Add(Me.Label11)
        'Me.pnlPower.Dock = System.Windows.Forms.DockStyle.Left
        'Me.pnlPower.Location = New System.Drawing.Point(3, 381)
        'Me.pnlPower.Name = "pnlPower"
        'Me.pnlPower.Size = New System.Drawing.Size(286, 48)
        'Me.pnlPower.TabIndex = 15
        ''
        ''Panel17
        ''
        'Me.Panel17.BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.notifications_powermenu_icon_standby
        'Me.Panel17.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        'Me.Panel17.Location = New System.Drawing.Point(6, 3)
        'Me.Panel17.Name = "Panel17"
        'Me.Panel17.Size = New System.Drawing.Size(52, 42)
        'Me.Panel17.TabIndex = 2
        ''
        ''Label11
        ''
        'Me.Label11.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        'Me.Label11.Location = New System.Drawing.Point(64, 0)
        'Me.Label11.Name = "Label11"
        'Me.Label11.Size = New System.Drawing.Size(222, 48)
        'Me.Label11.TabIndex = 8
        'Me.Label11.Text = "Power"
        'Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlRight.ResumeLayout(False)
        Me.pnlRight.PerformLayout()
        Me.pnlLeft.ResumeLayout(False)
        'Me.FlowLayoutPanel1.ResumeLayout(False)
        'Me.pnlGames.ResumeLayout(False)
        'Me.pnlMusic.ResumeLayout(False)
        'Me.pnlNetwork.ResumeLayout(False)
        'Me.pnlApplication.ResumeLayout(False)
        'Me.pnlFiles.ResumeLayout(False)
        'Me.pnlSettings.ResumeLayout(False)
        'Me.pnlHelp.ResumeLayout(False)
        'Me.pnlPower.ResumeLayout(False)
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
