Imports System.Runtime.InteropServices

Public Class frmPopup
    'Inherits Form
    Inherits WindowExtension

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 3000
        '
        'frmPopup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 30.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(420, 120)
        Me.ControlBox = False
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 16.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.Name = "frmPopup"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.ResumeLayout(False)

    End Sub

    Public Sub New()
        InitializeComponent()

        SuspendLayout()

        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        SetStyle(ControlStyles.UserPaint, True)

        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        SetStyle(ControlStyles.ResizeRedraw, True)
        Me.UpdateStyles()
        Me.CaptionFont = New System.Drawing.Font("Segoe UI", 20.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))

        Me.Left = Screen.PrimaryScreen.WorkingArea.Width - Me.Width - 20
        Me.Top = Screen.PrimaryScreen.WorkingArea.Height - Me.Height
        Me.EnableBlur()
        ResumeLayout()

        Me.AnimateTime = 150
        Me.Animation = AnimateWindowFlags.AW_BLEND
    End Sub

    Friend WithEvents Timer1 As Timer
    Private components As System.ComponentModel.IContainer

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim p As CreateParams
            p = MyBase.CreateParams
            p.ExStyle = p.ExStyle Or &H8
            Return p
        End Get
    End Property


    Public Property Caption As String
    Private Class PopupInfo
        Public Text As String
        Public Caption As String
        Public Image As Bitmap
        Public Time As Integer
        Public Sub New(strCaption As String, strText As String, Image As Bitmap, Optional dwellTime As Integer = 3000)
            Me.Caption = strCaption
            Me.Text = strText
            Me.Image = Image
            Me.Time = dwellTime
        End Sub
    End Class

    Dim pushStack As New Collections.Generic.Stack(Of PopupInfo)


    Public Sub Push(strTitle As String, strMessage As String, Optional image As Bitmap = Nothing, Optional dwelltime As Integer = 3000)
        pushStack.Push(New PopupInfo(strTitle, strMessage, image, dwelltime))
    End Sub

    Public Async Sub DoPopUp(strTitle As String, strMessage As String, Optional image As Bitmap = Nothing, Optional dwelltime As Integer = 3000)
        'Using _f As New frmPopup
        With Me
            .Text = strMessage
            .Caption = strTitle
            .BackgroundImage = image


            .Visible = True

            Await Task.Delay(dwelltime)
            .Visible = False
        End With
        'End Using
        If pushStack.Count Then
            Dim p = pushStack.Pop
            If p IsNot Nothing Then
                DoPopUp(p.Caption, p.Text, p.Image, p.Time)
            End If
        End If
    End Sub
    Dim CaptionFont As Font

    <Flags()>
    Public Enum AnimateWindowFlags
        AW_HOR_POSITIVE = &H1
        AW_HOR_NEGATIVE = &H2
        AW_VER_POSITIVE = &H4
        AW_VER_NEGATIVE = &H8
        AW_CENTER = &H10
        AW_HIDE = &H10000
        AW_ACTIVATE = &H20000
        AW_SLIDE = &H40000
        AW_BLEND = &H80000
    End Enum
    <DllImport("user32.dll")>
    Private Shared Function AnimateWindow(ByVal hwnd As IntPtr, ByVal time As Integer, ByVal flags As AnimateWindowFlags) As Boolean
    End Function
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        'MyBase.OnPaintBackground(e)
    End Sub
    Private Shared im As New StringFormat() With {.FormatFlags = StringFormatFlags.NoClip Or StringFormatFlags.NoWrap, .Trimming = StringTrimming.EllipsisCharacter}
    Private Shared iconSize As New Size(96, 96)
    Private Shared iconPad As New Padding(4, 14, 4, 4)
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)

        If Me.BackgroundImage IsNot Nothing Then
            e.Graphics.DrawImage(Me.BackgroundImage, New Rectangle(iconPad.Left, iconPad.Top, iconSize.Width, iconSize.Height))
        End If
        e.Graphics.DrawString(Me.Caption, Me.CaptionFont, SystemBrushes.ControlText, New Rectangle(iconSize.Width + iconPad.Horizontal, 0, Me.Width - (iconSize.Width + iconPad.Horizontal), 0), im)
        e.Graphics.DrawString(Me.Text, Me.Font, SystemBrushes.ControlText, New Rectangle(iconSize.Width + iconPad.Horizontal, 32, Width - (iconSize.Width + iconPad.Horizontal), Height - (32)), im)

    End Sub
    Dim _visible As Boolean
    Public Shadows Property Visible As Boolean
        Get
            Return _visible
        End Get
        Set(value As Boolean)
            _visible = value
            Me.Invoke(Sub() Refresh())
            Application.DoEvents()
            Try
                If _visible Then
                    AnimateWindow(Handle, AnimateTime, Animation)
                Else
                    AnimateWindow(Handle, AnimateTime, AnimateWindowFlags.AW_HIDE Or ReverseAnimation())

                End If

            Catch
            End Try
            Me.Invoke(Sub() MyBase.Visible = value)

        End Set
    End Property

    Public Property Animation As AnimateWindowFlags

    Private Function ReverseAnimation() As AnimateWindowFlags
        'Dim ret As AnimateWindowFlags
        'If (Animation And AnimateWindowFlags.AW_HOR_NEGATIVE) = AnimateWindowFlags.AW_HOR_NEGATIVE Then
        '    ret = AnimateWindowFlags.AW_HOR_POSITIVE
        'ElseIf (Animation And AnimateWindowFlags.AW_HOR_POSITIVE) = AnimateWindowFlags.AW_HOR_POSITIVE Then
        '    ret = AnimateWindowFlags.AW_HOR_NEGATIVE
        'End If
        'If (Animation And AnimateWindowFlags.AW_VER_NEGATIVE) = AnimateWindowFlags.AW_VER_NEGATIVE Then
        '    ret = ret Or AnimateWindowFlags.AW_VER_POSITIVE
        'ElseIf (Animation And AnimateWindowFlags.AW_VER_POSITIVE) = AnimateWindowFlags.AW_VER_POSITIVE Then
        '    ret = ret Or AnimateWindowFlags.AW_VER_NEGATIVE
        'End If
        'If (Animation And AnimateWindowFlags.AW_SLIDE) = AnimateWindowFlags.AW_SLIDE Then
        '    ret = ret Or AnimateWindowFlags.AW_SLIDE
        'End If
        'Return ret
        Return AnimateWindowFlags.AW_BLEND Or AnimateWindowFlags.AW_HIDE
    End Function
    Public Property AnimateTime As Integer = 150

    <DllImport("user32.dll")>
    Private Shared Function ShowScrollBar(hWnd As IntPtr, wBar As Integer, bShow As Boolean) As Boolean
    End Function
    Private Enum ScrollBarDirection

        SB_HORZ = 0
        SB_VERT = 1
        SB_CTL = 2
        SB_BOTH = 3
    End Enum

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)

        If (m.Msg = &H85) Then '// WM_NCPAINT

            ShowScrollBar(Me.Handle, ScrollBarDirection.SB_BOTH, False)
        End If
        MyBase.WndProc(m)
    End Sub


End Class
