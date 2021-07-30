Imports System.Runtime.InteropServices
Imports Menulator_Zero
'Imports XInput
'Imports XInput.XInputWrapper

Public Class frmMenulator
    'Dim WithEvents pad As XInput.XInputWrapper.XboxController = XInputWrapper.XboxController.RetrieveController(0)
    Dim WithEvents KeyHook As KeyboardHook


    Private Class FormAnimator
        'The types of animation available.
        Public Enum AnimationTypes
            Roll = &H0      'Roll out from edge to show; Roll in to edge to hide.  Requires direction.  Default animation.
            Centre = &H10   'Expand out from centre to show; Collapse in to centre to hide.
            Slide = &H40000 'Slide out from edge to show; Slide in to edge to hide.  Requires direction.
            Blend = &H80000 'Blend from transaprent to opaque to show; Blend from opaque to transparent to hide.
        End Enum

        'The directions in which the Slide animation can be shown.
        'The Flags attribute indicates that directions can be combined.
        <Flags()> Public Enum SlideDirections
            None = 0
            Right = &H1 'Slide from left to right.
            Left = &H2  'Slide from right to left.
            Down = &H4  'Slide from top to bottom.
            Up = &H8    'Slide from bottom to top.
        End Enum

        Private Const AW_HIDE As Integer = &H10000      'Hide the form.
        Private Const AW_ACTIVATE As Integer = &H20000  'Activate the form.

        Private WithEvents m_Form As Form               'The form to be animated.

        Private m_Type As AnimationTypes                'The type of animation used to show and hide the form.
        'Private m_Direction As SlideDirections          'The direction in which to Roll or Slide the form.
        Private m_Duration As Integer = 500             'The number of milliseconds over which the animation is played.
        Private m_ShowDirection As SlideDirections
        Private m_HideDirection As SlideDirections

        'The type of animation used to show and hide the form.
        Public Property AnimationType() As AnimationTypes
            Get
                Return Me.m_Type
            End Get
            Set(ByVal Value As AnimationTypes)
                Me.m_Type = Value
            End Set
        End Property

        'The direction in which to Roll or Slide the form.
        'Public Property SlideDirection() As SlideDirections
        '    Get
        '        Return Me.m_Direction
        '    End Get
        '    Set(ByVal Value As SlideDirections)
        '        Me.m_Direction = Value
        '    End Set
        'End Property
        Public Property ShowDirection As SlideDirections
            Get
                Return m_ShowDirection
            End Get
            Set(value As SlideDirections)
                m_ShowDirection = value
            End Set
        End Property
        Public Property HideDirection As SlideDirections
            Get
                Return m_HideDirection
            End Get
            Set(value As SlideDirections)
                m_HideDirection = value
            End Set
        End Property

        'The number of milliseconds over which the animation is played.
        Public Property AnimationDuration() As Integer
            Get
                Return Me.m_Duration
            End Get
            Set(ByVal Value As Integer)
                Me.m_Duration = Value
            End Set
        End Property

        'Windows API function used to animate the form.
        Private Declare Auto Function AnimateWindow Lib "user32" (ByVal hwnd As IntPtr,
                                                                  ByVal dwtime As Integer,
                                                                  ByVal dwflags As Integer) As Boolean

        'Creates a new FormAnimator object for the specified form.
        Public Sub New(ByVal form As Form)
            Me.m_Form = form
        End Sub

        'Creates a new FormAnimator object for the specified form using the specified animation type over the specified duration.
        Public Sub New(ByVal form As Form,
                       ByVal type As AnimationTypes,
                       ByVal duration As Integer)
            Me.New(form)

            Me.m_Type = type
            Me.m_Duration = duration
        End Sub

        'Creates a new FormAnimator object for the specified form using the specified animation type in the specified
        'direction over the specified duration.  This is intended to be used with Roll and Slide animations.
        Public Sub New(ByVal form As Form,
                       ByVal type As AnimationTypes,
                       ByVal showdirection As SlideDirections,
                       ByVal hidedirection As SlideDirections,
                       ByVal duration As Integer)
            Me.New(form, type, duration)

            Me.m_ShowDirection = showdirection
            m_HideDirection = hidedirection
        End Sub

        'Animates the form automatically when it is shown or hidden.
        Private Sub m_Form_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_Form.VisibleChanged
            Dim flags As Integer = Me.m_Type ' Or Me.m_Direction

            If Me.m_Form.Visible Then
                'Activate the form.
                flags = flags Or m_ShowDirection
                flags = flags Or AW_ACTIVATE
            Else
                'Hide the form.
                flags = flags Or m_HideDirection
                flags = flags Or AW_HIDE
            End If

            AnimateWindow(Me.m_Form.Handle,
                             Me.m_Duration,
                             flags)
        End Sub

        'Animates the form automatically when it closes.
        Private Sub m_Form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles m_Form.Closing
            'Hide the form.
            AnimateWindow(Me.m_Form.Handle,
                             Me.m_Duration,
                             AW_HIDE Or Me.m_Type Or Me.HideDirection)
        End Sub
    End Class

#Region "AppBar"

    Public Enum AppBarPos
        abpLeft = 0&
        abpTop = 1&
        abpRight = 2&
        abpBottom = 3&
    End Enum

    'A rect(angle)
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    'AppBarData struct
    Private Structure APPBARDATA
        Public cbSize As Integer
        Public hwnd As Integer
        Public uCallbackMessage As Integer
        Public uEdge As Integer
        Public rc As RECT
        Public lParam As Integer '  message specific
    End Structure

    'This function makes it happen. Nothing can be done without it
    Private Declare Function SHAppBarMessage Lib "shell32" (ByVal dwMessage As Integer, ByRef pData As APPBARDATA) As Integer
    'Used for adding the WS_EX_TOOLWINDOW style
    Private Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer) As Integer
    'We dont *have* to subclass, but we do want to do things right, dont we?
    Delegate Function WindowProc(ByVal hWnd As IntPtr, ByVal uMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer)
    Private Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    'Used to forward window messages to the next window proc in the queue
    Private Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As WindowProc) As IntPtr
    Private Declare Function CallWindowProc Lib "user32" Alias "CallWindowProcA" (ByVal lpPrevWndFunc As Long, ByVal hwnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    'Move the window
    Private Declare Function SetWindowPos Lib "user32" (ByVal hwnd As IntPtr, ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
    'Get the window dimensions
    Private Declare Function GetWindowRect Lib "user32" (ByVal hwnd As IntPtr, ByRef lpRect As RECT) As Integer
    'Get desktop window
    Private Declare Function GetDesktopWindow Lib "user32" () As IntPtr

    Const ABM_NEW = &H0
    Const ABM_REMOVE = &H1
    Const ABM_QUERYPOS = &H2
    Const ABM_SETPOS = &H3
    Const ABM_GETSTATE = &H4
    Const ABM_GETTASKBARPOS = &H5
    Const ABM_ACTIVATE = &H6               '  lParam == TRUE/FALSE means activate/deactivate
    Const ABM_GETAUTOHIDEBAR = &H7
    Const ABM_SETAUTOHIDEBAR = &H8          '  this can fail at any time.  MUST check the result
    Const ABM_WINDOWPOSCHANGED = &H9

    Const ABN_STATECHANGE = &H0
    Const ABN_POSCHANGED = &H1
    Const ABN_FULLSCREENAPP = &H2
    Const ABN_WINDOWARRANGE = &H3 '  lParam == TRUE means hide

    Const ABS_AUTOHIDE = &H1
    Const ABS_ALWAYSONTOP = &H2

    Const WM_USER = &H400
    Const WM_ACTIVATE = &H6
    Const WM_SIZE = &H5
    Const WM_MOVE = &H3

    Const GWL_WNDPROC = (-4)
    Const GWL_EXSTYLE = (-20)

    Const WS_EX_TOOLWINDOW = &H80&

    Const HWND_TOP = 0&
    Const HWND_BOTTOM = 1&

    Const SWP_NOSIZE = &H1&
    Const SWP_NOMOVE = &H2&
    Const SWP_NOZORDER = &H4

    'The old windowproc
    'Dim lOldProc As IntPtr
    'The hWnd
    Dim lhWnd As IntPtr
    'Since we need this so much, just keep a copy permanently
    Dim abdAppBar As APPBARDATA

    Public Sub StartAppBar(frm As Form, position As AppBarPos)
        'Dont want to subclass twice
        'If lOldProc = 0 Then
        Dim rScreen As RECT
        Dim rFrm As RECT

        lhWnd = frm.Handle
        SetWindowLong(lhWnd, GWL_EXSTYLE, GetWindowLong(lhWnd, GWL_EXSTYLE) Or WS_EX_TOOLWINDOW)

        'GetWindowRect(GetDesktopWindow, rScreen)
        rScreen = New RECT() With {.Left = 0, .Top = 0, .Right = Screen.PrimaryScreen.Bounds.Width, .Bottom = Screen.PrimaryScreen.Bounds.Height}
        GetWindowRect(lhWnd, rFrm)

        rFrm.Bottom = rScreen.Bottom  ' rFrm.Bottom '- rFrm.Top
        rFrm.Right -= rFrm.Left
        rFrm.Top = 0
        rFrm.Left = 0

        'Subclass!
        '    lOldProc = SetWindowLong(lhWnd, GWL_WNDPROC, AddressOf AppBarProc)

        abdAppBar.cbSize = Len(abdAppBar)
        abdAppBar.hwnd = lhWnd
        abdAppBar.uCallbackMessage = WM_USER

        If SHAppBarMessage(ABM_NEW, abdAppBar) = 0 Then
            'Uh-oh, something went wrong!
            StopAppBar()
            Exit Sub
        End If

        'Where is the taskbar?
        SHAppBarMessage(ABM_GETTASKBARPOS, abdAppBar)

        'Size our window so its in the right place
        With abdAppBar.rc

            If .Top > rScreen.Top Then
                'Taskbar is at the bottom
                ' rScreen.Bottom = .Top
            ElseIf .Bottom < rScreen.Bottom Then
                'Taskbar is at the top
                'rScreen.Top = .Bottom
            ElseIf .Right < rScreen.Right Then
                'Taskbar is at the left
                'rScreen.Left = .Right
            Else
                'Taskbar is at the right
                'rScreen.Right = .Left
            End If

            abdAppBar.rc = rScreen

            Select Case position
                Case AppBarPos.abpLeft
                    .Right = rFrm.Right

                Case AppBarPos.abpTop
                    .Bottom = rFrm.Bottom

                Case AppBarPos.abpRight
                    .Left = .Right - rFrm.Right

                Case AppBarPos.abpBottom
                    .Top = .Bottom - rFrm.Bottom

            End Select
        End With

        'Which edge are we using?
        abdAppBar.uEdge = position

        'Ask the OS to find us a space to put the AppBar
        SHAppBarMessage(ABM_QUERYPOS, abdAppBar)
        'Tell the OS we're putting our AppBar there (OS reduces desktop space to fit)
        SHAppBarMessage(ABM_SETPOS, abdAppBar)
        'Move our window
        SetWindowPos(lhWnd, 0, abdAppBar.rc.Left, abdAppBar.rc.Top, abdAppBar.rc.Right - abdAppBar.rc.Left, abdAppBar.rc.Bottom - abdAppBar.rc.Top, SWP_NOZORDER) ' Or &H80)


        'End If
    End Sub
    Public Sub StopAppBar()
        'Dont want to unsubclass a non-subclassed window
        'If lOldProc Then
        'Tell the OS we're done with the AppBar
        SHAppBarMessage(ABM_REMOVE, abdAppBar)
        'Unsubclass
        'SetWindowLong(lhWnd, GWL_WNDPROC, lOldProc)
        'Reset so we can do it all again
        'lOldProc = 0
        'End If
    End Sub

    'Public Function AppBarProc(ByVal hwnd As Long, ByVal uMsg As Long, ByVal wParam As Long, ByVal lParam As Long) As Long

    '    Select Case uMsg
    '        Case WM_ACTIVATE
    '            'Window got activated
    '            SHAppBarMessage(ABM_ACTIVATE, abdAppBar)

    '        Case WM_USER
    '            'Special AppBar message

    '            Select Case wParam
    '                Case ABN_STATECHANGE
    '        'Notifies an appbar that the taskbar's autohide or always-on-top state has changed—that is,
    '        'the user has selected or cleared the "Always on top" or "Auto hide" check box on the taskbar's property sheet.

    '                Case ABN_POSCHANGED
    '                    'Notifies an appbar when an event has occurred that may affect the appbar's size and position.
    '                    'Events include changes in the taskbar's size, position, and visibility state, as well as the
    '                    'addition, removal, or resizing of another appbar on the same side of the screen.

    '                    GetWindowRect(lhWnd, abdAppBar.rc)
    '                    SHAppBarMessage(ABM_QUERYPOS, abdAppBar)
    '                    SHAppBarMessage(ABM_SETPOS, abdAppBar)
    '                    SetWindowPos(lhWnd, 0, abdAppBar.rc.Left, abdAppBar.rc.Top, abdAppBar.rc.Right, abdAppBar.rc.Bottom, SWP_NOZORDER)

    '                Case ABN_FULLSCREENAPP
    '                    'Notifies an appbar when a full-screen application is opening or closing.
    '                    'This notification is sent in the form of an application-defined message that is set by the ABM_NEW message.

    '                    If CBool(lParam) Then
    '                        'Fullscreen app is loading!
    '                        'Pop AppBar to the back
    '                        SetWindowPos(lhWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
    '                    Else
    '                        'Fullscreen app finished
    '                        'Pop AppBar to the front
    '                        SetWindowPos(lhWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
    '                    End If

    '                Case ABN_WINDOWARRANGE
    '                    'Notifies an appbar that the user has selected the Cascade,
    '                    'Tile Horizontally, or Tile Vertically command from the taskbar's context menu.

    '            End Select
    '    End Select

    '    'Forward message to next windowproc
    '    AppBarProc = CallWindowProc(lOldProc, hwnd, uMsg, wParam, lParam)
    'End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_ACTIVATE
                'Window got activated
                SHAppBarMessage(ABM_ACTIVATE, abdAppBar)

            Case WM_USER
                'Special AppBar message

                Select Case m.WParam
                    Case ABN_STATECHANGE
            'Notifies an appbar that the taskbar's autohide or always-on-top state has changed—that is,
            'the user has selected or cleared the "Always on top" or "Auto hide" check box on the taskbar's property sheet.

                    Case ABN_POSCHANGED
                        'Notifies an appbar when an event has occurred that may affect the appbar's size and position.
                        'Events include changes in the taskbar's size, position, and visibility state, as well as the
                        'addition, removal, or resizing of another appbar on the same side of the screen.

                        GetWindowRect(lhWnd, abdAppBar.rc)
                        SHAppBarMessage(ABM_QUERYPOS, abdAppBar)
                        SHAppBarMessage(ABM_SETPOS, abdAppBar)
                        SetWindowPos(lhWnd, 0, abdAppBar.rc.Left, abdAppBar.rc.Top, abdAppBar.rc.Right, abdAppBar.rc.Bottom, SWP_NOZORDER)

                    Case ABN_FULLSCREENAPP
                        'Notifies an appbar when a full-screen application is opening or closing.
                        'This notification is sent in the form of an application-defined message that is set by the ABM_NEW message.

                        If CBool(m.LParam) Then
                            'Fullscreen app is loading!
                            'Pop AppBar to the back
                            SetWindowPos(lhWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
                        Else
                            'Fullscreen app finished
                            'Pop AppBar to the front
                            SetWindowPos(lhWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE)
                        End If

                    Case ABN_WINDOWARRANGE
                        'Notifies an appbar that the user has selected the Cascade,
                        'Tile Horizontally, or Tile Vertically command from the taskbar's context menu.

                End Select
        End Select

        MyBase.WndProc(m)
    End Sub

#End Region



    '<DllImport("user32.dll", SetLastError:=True)>
    'Shared Function FindWindow(lpClassName As String, lpWindowName As String) As IntPtr
    'End Function

    'Private Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As IntPtr, ByVal lpString As String,
    'ByVal cch As Integer) As Integer
    'Private Declare Function GetWindowTextLength Lib "user32" Alias "GetWindowTextLengthA" (ByVal hwnd As IntPtr) As Integer
    'Private Declare Function GetWindow Lib "user32" (ByVal hwnd As IntPtr, ByVal wCmd As Integer) As Integer

    'Private Const GW_HWNDNEXT = 2

    'Private Function GetHandleFromPartialCaption(ByRef lWnd As Long, ByVal sCaption As String) As Boolean
    '    Dim lhWndP As Long
    '    Dim sStr As String
    '    GetHandleFromPartialCaption = False
    '    lhWndP = FindWindow(vbNullString, vbNullString) 'PARENT WINDOW
    '    Do While lhWndP <> 0
    '        sStr = Strings.StrDup(GetWindowTextLength(lhWndP) + 1, ChrW(0))
    '        GetWindowText(lhWndP, sStr, Len(sStr))
    '        sStr = Strings.Left(sStr, Len(sStr) - 1)
    '        If InStr(1, sStr, sCaption) > 0 Then
    '            GetHandleFromPartialCaption = True
    '            lWnd = lhWndP
    '            Exit Do
    '        End If
    '        lhWndP = GetWindow(lhWndP, GW_HWNDNEXT)
    '    Loop
    'End Function
    'Dim _isActivated As Boolean = False
    'Private Property IsActivated As Boolean
    '    Get
    '        Return _isActivated
    '    End Get
    '    Set(value As Boolean)
    '        _isActivated = value
    '        If _isActivated Then
    '            Me.Visible = True
    '        Else
    '            Me.Visible = False
    '        End If
    '    End Set
    'End Property


    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Index = 0
    End Sub
    Dim emuThread As Threading.Thread
    Dim emuProcess As Process
    Dim animator As New FormAnimator(Me, FormAnimator.AnimationTypes.Slide, FormAnimator.SlideDirections.Right, FormAnimator.SlideDirections.Left, 100)
    'for testing
    Public Sub New()
        '-nowindow         -keyboardprovider dinput
        MyClass.New("d:\shares\games\mame\mamepp.exe", "mk", "-noswitchres", New MenuItemCollection(Keys.Pause, New MenuItem() {
                                                                                                                                               New MenuItem("Load State", "appbar.disk.download", Keys.F7 Or Keys.Shift),
                                                                                                                                               New MenuItem("Save State", "appbar.disk.upload", Keys.F7),
                                                                                                                                               New MenuItem("Reset", "appbar.refresh", Keys.F3),
                                                                                                                                               New MenuItem("Mame Config", "appbar.refresh", Keys.Tab),
                                                                                                                                               New MenuItem("Service Menu", "appbar.refresh", Keys.F2),
                                                                                                                                               New MenuItem("Close", "appbar.close", Keys.None,, True)}
                                                                                                                                               ))
        ' This call is required by the designer.


        'D:\Games\emulation\Sega\ROMs\Game Titles - #-Z\Alien 3.bin
        'Dim s = MAME.MameProcess.Redirect("d:\games\emulation\mame\mame.exe", "mk")
    End Sub
    Public Shared Function MameDefaultItemCollection() As MenuItemCollection
        Return New MenuItemCollection(Keys.Pause, New MenuItem() {
                                                                New MenuItem("Toggle Pause", "controls_pause_32", Keys.Pause, False),
                                                                New MenuItem("Load State", "appbar.disk.download", Keys.F7 Or Keys.Shift),
                                                                New MenuItem("Save State", "appbar.disk.upload", Keys.F7),
                                                                New MenuItem("Reset", "appbar.refresh", Keys.F3),
                                                                New MenuItem("Mame Config", "appbar.refresh", Keys.Tab),
                                                                New MenuItem("Service Menu", "appbar.refresh", Keys.F2),
                                                                New MenuItem("Close", "appbar.close", Keys.None,, True)}
                                                                )

    End Function
    Public Shared Function ConsoleDefaultItemCollection() As MenuItemCollection
        Return New MenuItemCollection(Keys.Pause, New MenuItem() {
                                                                New MenuItem("Toggle Pause", "controls_pause_32", Keys.Pause, False),
                                                                New MenuItem("Load State", "appbar.disk.download", Keys.D1),
                                                                New MenuItem("Save State", "appbar.disk.upload", Keys.F1),
                                                                New MenuItem("Reset", "appbar.refresh", Keys.R),
                                                                New MenuItem("Service Menu", "appbar.refresh", Keys.F2),
                                                                New MenuItem("Close", "appbar.close", Keys.None,, True)}
                                                                )

    End Function
    Public Class MenuItem
        Public Text As String
        Public Icon As Image
        Public SendKey As Keys
        Public HideWindow As Boolean = True
        Public CloseWindow As Boolean = False
        Public Sub New(Text As String, Icon As Image, Key As Keys, Optional HideWindow As Boolean = True, Optional CloseWindow As Boolean = False)
            Me.Text = Text
            Me.Icon = Icon
            Me.SendKey = Key
            Me.HideWindow = HideWindow
            Me.CloseWindow = CloseWindow
        End Sub
        Public Sub New(Text As String, Icon As String, Key As Keys, Optional HideWindow As Boolean = True, Optional CloseWindow As Boolean = False)
            Me.Text = Text
            Me.Icon = My.Resources.ResourceManager.GetObject(Icon)
            Me.SendKey = Key
            Me.HideWindow = HideWindow
            Me.CloseWindow = CloseWindow
        End Sub
    End Class
    Public Class MenuItemCollection
        Inherits Collections.Generic.List(Of MenuItem)
        Public SendKey_Pause As Keys

        Public Sub New(PauseKey As Keys, ParamArray Items As MenuItem())
            SendKey_Pause = PauseKey
            MyBase.AddRange(Items)
        End Sub
    End Class
    Dim myMenuCollection As MenuItemCollection
    Public Sub New(c As MenuItemCollection)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        myMenuCollection = c
        ' Add any initialization after the InitializeComponent() call.
        'Me.TopLevel = True
        'Me.TopMost = True

        For Each item In c
            ' UI.Name = "NewUIListItem2"
            'Me.NewUIListItem2.TabIndex = 2

            Dim UI As New NewUIListItem With {
                .Image = item.Icon,
                .Location = New System.Drawing.Point(3, 56),
                .Size = New System.Drawing.Size(282, 47),
                .Text = item.Text
            }
            Me.FlowLayoutPanel1.Controls.Add(UI)

        Next

        FlowLayoutPanel1.Visible = True
        Panel1.Visible = True
    End Sub
    Public Sub New(MamePath As String, MameRom As String, c As MenuItemCollection, switchres As Boolean, window As Boolean)

        MyClass.New(MamePath, MameRom, "-" & IIf(switchres, "", "no") & "switchres -" & IIf(window, "", "no") & "window", c)
    End Sub
    Public Sub New(EmuPath As String, RomPath As String, c As MenuItemCollection)
        Me.New(c)
        emuThread = New Threading.Thread(Sub()
                                             emuProcess = New Process With {
                                                 .StartInfo = New ProcessStartInfo() With {.FileName = EmuPath, .Arguments = RomPath} ', .WorkingDirectory = Chr(34) & IO.Path.GetDirectoryName(EmuPath) & Chr(34)}
                                                 }
                                             emuProcess.Start()
                                             emuProcess.WaitForExit()
                                             If emuProcess IsNot Nothing Then emuProcess.Dispose()
                                             If Me.IsDisposed = False Then
                                                 Me.DialogResult = DialogResult.OK
                                                 Me.BeginInvoke(Sub() Me.Close())
                                             End If
                                         End Sub)
        emuThread.Start()
        ' Me.Visible = False
    End Sub
    Public Sub New(strMamePath As String, strRom As String, strArgs As String, c As MenuItemCollection)
        Me.New(c)

        emuThread = New Threading.Thread(Sub(o As Object)
                                             Threading.Thread.BeginCriticalRegion()
                                             If Not FileIO.FileSystem.FileExists(o(0)) Then GoTo TheEnd
                                             emuProcess = New Process With {
                                                 .StartInfo = New ProcessStartInfo() With {.FileName = o(0), .Arguments = o(1) & " " & o(2), .UseShellExecute = False, .CreateNoWindow = True, .WorkingDirectory = IO.Path.GetDirectoryName(o(0))}
                                             }
                                             emuProcess.Start()
                                             emuProcess.WaitForExit()

                                             'Dim g As New MAME.MameProcess(myMame.MamePath)
                                             'g.PlayRom(myList.SelectedItem.Name)


                                             If emuProcess IsNot Nothing Then
                                                 Try
                                                     emuProcess.Dispose()
                                                 Catch
                                                 End Try
                                             End If
TheEnd:
                                             If Me.IsDisposed = False AndAlso Me.IsHandleCreated Then
                                                 Me.DialogResult = DialogResult.OK
                                                 Me.BeginInvoke(Sub() Me.Close())
                                             End If
                                             Threading.Thread.EndCriticalRegion()

                                         End Sub)
        emuThread.Start({strMamePath, strRom, strArgs})

    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        StartAppBar(Me, AppBarPos.abpLeft)
        Timer1.Interval = 1000
        Timer1.Start()
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        Try
            If Not Me.Visible AndAlso emuProcess.MainWindowHandle <> IntPtr.Zero Then SwitchToThisWindow(emuProcess.MainWindowHandle, True)
        Catch
        End Try
    End Sub
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim m = MyBase.CreateParams
            m.ExStyle = m.ExStyle Or &H40000 Or &H8000000 Or &H8
            Return m
        End Get
    End Property





    Private Sub frmMenulator_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'Try
        '    If emuProcess IsNot Nothing AndAlso emuProcess.HasExited = False Then
        '        Threading.Thread.BeginCriticalRegion()
        '        emuProcess.CloseMainWindow()
        '        emuProcess.WaitForExit()
        '        emuProcess = Nothing
        '        Threading.Thread.EndCriticalRegion()
        '    End If

        'Catch

        'End Try
        If KeyHook IsNot Nothing Then KeyHook.StopHook()
        KeyHook = Nothing
        StopAppBar()

        For Each item As Control In FlowLayoutPanel1.Controls
            item.Dispose()
        Next
        ' XInput.XInputWrapper.XboxController.StopPolling()
    End Sub

    Dim _index As Integer = -1
    Public Property Index As Integer
        Get
            Return _index
        End Get
        Set(value As Integer)
            Dim numItems = FlowLayoutPanel1.Controls.Count - 1
            If value < 0 Then value = numItems
            If value > numItems Then value = 0

            If _index >= 0 Then FlowLayoutPanel1.Controls(_index).BackColor = FlowLayoutPanel1.BackColor
            _index = value
            FlowLayoutPanel1.Controls(_index).BackColor = SystemColors.ActiveBorder

        End Set
    End Property





    Public Enum INPUT_TYPE As Integer
        INPUT_MOUSE = 0
        INPUT_KEYBOARD = 1
        INPUT_HARDWARE = 2
    End Enum


    <StructLayout(LayoutKind.Sequential)>
    Public Structure MOUSEINPUT
        Public dx As Integer
        Public dy As Integer
        Public mouseData As Integer
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure


    <StructLayout(LayoutKind.Sequential)>
    Public Structure KEYBDINPUT
        Public wVk As Short
        Public wScan As Short
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure HARDWAREINPUT
        Public uMsg As UInteger
        Public wParamL As UShort
        Public wParamH As UShort
    End Structure
    <StructLayout(LayoutKind.Explicit)>
    Public Structure INPUT
        ' this field starts at byte o
        <FieldOffset(0)>
        Public type As INPUT_TYPE


        ' the rest of the fields begin at byte 4, immediately
        ' following the type field.
        <FieldOffset(4)>
        Public mi As MOUSEINPUT
        <FieldOffset(4)>
        Public ki As KEYBDINPUT
        <FieldOffset(4)>
        Public hi As HARDWAREINPUT
    End Structure

    Public Declare Function SendInput Lib "user32" (ByVal nInputs As Integer, <MarshalAs(UnmanagedType.LPArray)> ByVal pInputs() As INPUT, ByVal cbSize As Integer) As Integer

    Public Declare Function AttachThreadInput Lib "user32" (ByVal idAttach As IntPtr, ByVal idAttachTo As IntPtr, ByVal fAttach As Boolean) As Boolean
    Public Declare Function GetCurrentThreadId Lib "kernel32" () As IntPtr
    Public Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As IntPtr, ByVal lpwdProcessId As IntPtr) As IntPtr


    Public Sub ProcessKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Up
                If Me.Visible Then
                    Index -= 1
                    e.Handled = True
                    e.SuppressKeyPress = True
                End If
            Case Keys.Down
                If Me.Visible Then
                    Index += 1
                    e.Handled = True
                    e.SuppressKeyPress = True
                End If
            'Case Keys.P
            '    'Dim hMame = GetWindowThreadProcessId(emuProcess.MainWindowHandle, IntPtr.Zero)
            '    'AttachThreadInput(GetCurrentThreadId, hMame, True)
            '    'keybd_event(myMenuCollection.SendKey_Pause, 0, 0, 0)

            '    WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.PAUSE)
            '    e.Handled = True
            '    e.SuppressKeyPress = True
            '    Debug.Print("P")
            Case Keys.Return
                If Me.Visible Then
                    If _index >= 0 Then

                        With myMenuCollection(_index)
                            AppActivate(emuProcess.Id)

                            If .SendKey <> Keys.None Then


                                Dim hMame = GetWindowThreadProcessId(emuProcess.MainWindowHandle, IntPtr.Zero)
                                Dim kvalue As Keys = .SendKey
                                AttachThreadInput(GetCurrentThreadId, hMame, True)

                                If (.SendKey And Keys.Shift) = Keys.Shift Then
                                    keybd_event(Keys.LShiftKey, 0, 0, 0)
                                    kvalue = kvalue Xor Keys.Shift
                                End If
                                If (.SendKey And Keys.Control) = Keys.Control Then
                                    keybd_event(Keys.LControlKey, 0, 0, 0)
                                    kvalue = kvalue Xor Keys.Control
                                End If
                                Debug.Print("Sending " & kvalue.ToString)

                                Dim Data(1) As INPUT
                                Data(0).type = INPUT_TYPE.INPUT_KEYBOARD
                                Data(0).ki.wVk = kvalue

                                'Data(0).ki.wScan = &H19
                                'Data(0).ki.dwFlags = KEYEVENTF_SCANCODE

                                Data(1).ki.wVk = kvalue
                                Data(1).ki.dwFlags = 2

                                SendInput(1, Data, Marshal.SizeOf(GetType(INPUT)))
                                'SendInput(0, Data, Marshal.SizeOf(GetType(INPUT)))
                                'Threading.Thread.Sleep(50)
                                'Data(0).ki.dwFlags = KEYEVENTF_KEYUP Or KEYEVENTF_SCANCODE
                                'SendInput(0, Data, Marshal.SizeOf(GetType(INPUT)))
                                'Threading.Thread.Sleep(50)

                                If (.SendKey And Keys.Shift) = Keys.Shift Then
                                    keybd_event(Keys.LShiftKey, 0, 2, 0)
                                End If
                                If (.SendKey And Keys.Control) = Keys.Control Then
                                    keybd_event(Keys.LControlKey, 0, 2, 0)
                                End If
                                '

                            End If
                            If .HideWindow Then

                                'keybd_event(myMenuCollection.SendKey_Pause, 0, 0, 0)
                                'WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.PAUSE)
                                e.Handled = True
                                e.SuppressKeyPress = True
                                Me.Visible = False

                            End If
                            If .CloseWindow Then

                                Me.DialogResult = DialogResult.Retry
                                emuProcess.CloseMainWindow()
                                emuProcess.WaitForExit()
                                emuProcess = Nothing

                                StopAppBar()
                                Me.Close()
                                e.Handled = True
                                e.SuppressKeyPress = True
                                Return
                            ElseIf .HideWindow Then
                                SwitchToThisWindow(emuProcess.MainWindowHandle, True)
                            End If
                        End With
                        'Select Case FlowLayoutPanel1.Controls(_index).Text
                        '    Case "Close"
                        '        e.Handled = True
                        '        e.SuppressKeyPress = True
                        '        Me.DialogResult = DialogResult.Cancel
                        '        emuProcess.CloseMainWindow()
                        '        emuProcess.WaitForExit()
                        '        emuProcess = Nothing
                        '        'Me.Close()
                        '    Case "Load State"
                        '        'f8
                        '        keybd_event(Keys.F8, 0, 0, 0)
                        '        Me.Visible = False
                        '        keybd_event(Keys.Pause, 0, 0, 0)
                        '    Case "Save State"
                        '        'f5
                        '        keybd_event(Keys.F5, 0, 0, 0)
                        '        Me.Visible = False
                        '        keybd_event(Keys.Pause, 0, 0, 0)
                        '    Case "Reset"
                        '        ' tab
                        '        keybd_event(Keys.Tab, 0, 0, 0)
                        '        Me.Visible = False
                        '        keybd_event(Keys.Pause, 0, 0, 0)

                        'End Select
                    End If
                    e.Handled = True
                    e.SuppressKeyPress = True
                End If

            Case Keys.Escape

                If Me.Visible Then
                    Me.Visible = False
                    'keybd_event(myMenuCollection.SendKey_Pause, 0, 0, 0)
                    'WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.PAUSE)

                    e.Handled = True
                    e.SuppressKeyPress = True

                    'AppActivate(emuProcess.Id)
                    'SetForegroundWindow(emuProcess.MainWindowHandle)
                    SwitchToThisWindow(emuProcess.MainWindowHandle, True)
                End If
                'Case Keys.F12
                '    Me.Visible = Not Me.Visible
                '    e.Handled = True
                '    e.SuppressKeyPress = True
        End Select
    End Sub
    Private Sub frmMenulator_KeyDown(sender As Object, e As KeyEventArgs) 'Handles Me.KeyDown, FlowLayoutPanel1.KeyDown
        ProcessKeyDown(sender, e)
    End Sub
    <DllImport("user32")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean

    End Function

    <DllImport("user32")>
    Private Shared Sub SwitchToThisWindow(hwnd As IntPtr, fUnknown As Boolean)

    End Sub


    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Return False
    End Function
    Protected Overrides Function ProcessDialogChar(charCode As Char) As Boolean
        Return False
    End Function
    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        Return False
    End Function

    Private Declare Function keybd_event Lib "user32" Alias "keybd_event" _
              (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer,
               ByVal dwExtraInfo As Integer) As Integer
    Const KEYEVENTF_KEYDOWN As Integer = &H0 ' Press key        
    Const KEYEVENTF_KEYUP As Integer = &H2 ' Release key 
    Const KEYEVENTF_SCANCODE As Integer = &H8

    'Const VK_PAUSE As Integer = &H13
    'Const VK_F8 As Integer = &H77
    'Const VK_F5 As Integer = &H74
    'Const VK_TAB As Integer = &H9
    'Const VK_CTRL As Integer = &H11

    Private Sub KeyHook_KeyDown(e As KeyboardHook.KeyEventArgsEx) Handles KeyHook.KeyDown
        Select Case e.KeyCode

            Case Keys.Space
                e.Handled = True
                'If e.Repeat > 0 Then
                '    If e.Repeat >= 20 Then Me.BeginInvoke(Sub() Me.Close()) : e.CreateNoMoreEvents = True
                '    e.SuppressKeyPress = True
                '    Return
                'End If
                Try
                    AppActivate(emuProcess.Id)
                Catch
                End Try
                'Debug.Print(e.Time & " " & e.Repeat)

                'keybd_event(myMenuCollection.SendKey_Pause, 0, 0, 0)


                'WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.PAUSE)
                If Me.Visible Then
                    Me.Hide()
                Else
                    Index = 0
                    Me.Show()
                End If

            Case Else
                If Me.Visible Then
                    frmMenulator_KeyDown(Me, e)
                End If
        End Select
    End Sub

    Private Sub KeyHook_KeyUp(e As KeyboardHook.KeyEventArgsEx) Handles KeyHook.KeyUp

        Me.OnKeyUp(e)
    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblTime.Text = Now.ToString("h:mm tt  M/dd/yyyy")

        KeyHook = New KeyboardHook

        Me.Hide()
        Timer1.Enabled = False
    End Sub

    'Private Sub pad_StateChanged(sender As Object, e As XboxControllerStateChangedEventArgs) Handles pad.StateChanged
    '    If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_A) Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Return))
    '    End If
    '    If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_UP) Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Up))
    '    End If
    '    If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN) Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Down))
    '    End If
    '    If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT) Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Left))
    '    End If
    '    If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT) Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Right))
    '    End If
    '    If e.CurrentInputState.Gamepad.wButtons And &H400 Then
    '        KeyHook_KeyDown(New KeyboardHook.KeyEventArgsEx(Keys.Space))
    '    End If
    'End Sub

    '<StructLayout(LayoutKind.Sequential)>
    'Public Structure JOYINFOEX
    '    Public dwSize As Integer
    '    Public dwFlags As Integer
    '    Public dwXpos As Integer
    '    Public dwYpos As Integer
    '    Public dwZpos As Integer
    '    Public dwRpos As Integer
    '    Public dwUpos As Integer
    '    Public dwVpos As Integer
    '    Public dwButtons As Integer
    '    Public dwButtonNumber As Integer
    '    Public dwPOV As Integer
    '    Public dwReserved1 As Integer
    '    Public dwReserved2 As Integer


    'End Structure
    'Dim state As JOYINFOEX
    'Dim stateIsEmpty As Boolean = True
    'Const JOYERR_BASE = 160
    'Const JOYERR_UNPLUGGED = JOYERR_BASE + 7
    'Private Declare Function joyGetPosEx Lib "winmm.dll" (ByVal uJoyID As Integer, ByRef pji As JOYINFOEX) As Integer
    'Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer, ByRef lpvParam As Integer, ByVal fuWinIni As Integer) As Integer

    'Dim KeyboardDelay As Integer, KeyboardRepeatSpeed As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        'SystemParametersInfo(22, 0, KeyboardDelay, 0)
        'Select Case KeyboardDelay
        '    Case 0
        '        KeyboardDelay = 250
        '    Case 1
        '        KeyboardDelay = 500
        '    Case 2
        '        KeyboardDelay = 750
        '    Case 3
        '        KeyboardDelay = 1000
        'End Select
        'SystemParametersInfo(10, 0, KeyboardRepeatSpeed, 0)
        'Select Case KeyboardRepeatSpeed
        '    Case 0 : KeyboardRepeatSpeed = 400
        '    Case 1 : KeyboardRepeatSpeed = 363
        '    Case 2 : KeyboardRepeatSpeed = 352
        '    Case 3 : KeyboardRepeatSpeed = 341
        '    Case 4 : KeyboardRepeatSpeed = 330
        '    Case 5 : KeyboardRepeatSpeed = 319
        '    Case 6 : KeyboardRepeatSpeed = 308
        '    Case 7 : KeyboardRepeatSpeed = 297
        '    Case 8 : KeyboardRepeatSpeed = 286
        '    Case 9 : KeyboardRepeatSpeed = 275
        '    Case 10 : KeyboardRepeatSpeed = 264
        '    Case 11 : KeyboardRepeatSpeed = 253
        '    Case 12 : KeyboardRepeatSpeed = 242
        '    Case 13 : KeyboardRepeatSpeed = 231
        '    Case 14 : KeyboardRepeatSpeed = 220
        '    Case 15 : KeyboardRepeatSpeed = 209
        '    Case 16 : KeyboardRepeatSpeed = 198
        '    Case 17 : KeyboardRepeatSpeed = 187
        '    Case 18 : KeyboardRepeatSpeed = 176
        '    Case 19 : KeyboardRepeatSpeed = 165
        '    Case 20 : KeyboardRepeatSpeed = 154
        '    Case 21 : KeyboardRepeatSpeed = 143
        '    Case 22 : KeyboardRepeatSpeed = 114
        '    Case 23 : KeyboardRepeatSpeed = 105
        '    Case 24 : KeyboardRepeatSpeed = 96
        '    Case 25 : KeyboardRepeatSpeed = 88
        '    Case 26 : KeyboardRepeatSpeed = 75
        '    Case 27 : KeyboardRepeatSpeed = 69
        '    Case 28 : KeyboardRepeatSpeed = 60
        '    Case 29 : KeyboardRepeatSpeed = 51
        '    Case 30 : KeyboardRepeatSpeed = 42
        '    Case 31 : KeyboardRepeatSpeed = 33

        'End Select
        'Dim i = XInput.XInputWrapper.XboxController.RetrieveController(XboxController.FIRST_CONTROLLER_INDEX)
        'If i.IsConnected Then
        ' XInput.XInputWrapper.XboxController.StartPolling()
        ' tmrJoy.Enabled = True
        ' End If

    End Sub
    'Private Sub tmrJoy_Tick(sender As Object, e As EventArgs) 'Handles tmrJoy.Tick
    '    Static dwYpos(1) As DateTime, dwXpos(1) As DateTime, button512 As DateTime
    '    Dim newState As JOYINFOEX
    '    newState.dwSize = 64
    '    newState.dwFlags = &HFF

    '    'If JOYERR_UNPLUGGED Then
    '    joyGetPosEx(0, newState)
    '    '    stateIsEmpty = True
    '    'End If
    '    ''

    '    'If Not stateIsEmpty Then
    '    If state.dwYpos <> newState.dwYpos Then
    '        Select Case newState.dwYpos
    '            Case 32255
    '                dwYpos(0) = Nothing
    '                dwYpos(1) = Nothing
    '                'a release, where did we come from?
    '                If state.dwYpos = 0 Then
    '                    WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
    '                    Debug.Print("Release UP")
    '                Else
    '                    WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
    '                    Debug.Print("Release DOWN")
    '                End If
    '                    '32255 '65535
    '            Case 0
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
    '                Debug.Print("Press UP")
    '                dwYpos(0) = Now
    '            Case 65535
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
    '                Debug.Print("Press DOWN")
    '                dwYpos(0) = Now
    '        End Select
    '    ElseIf state.dwYpos = 0 Then
    '        'holding up
    '        If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
    '                dwYpos(1) = Now
    '            End If
    '        End If
    '    ElseIf state.dwYpos = 65535 Then
    '        'holding down
    '        If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
    '                dwYpos(1) = Now
    '            End If
    '        End If
    '    End If

    '    If state.dwXpos <> newState.dwXpos Then
    '        Select Case newState.dwXpos
    '            Case 32255
    '                dwXpos(0) = Nothing
    '                dwXpos(1) = Nothing
    '                'a release, where did we come from?
    '                If state.dwXpos = 0 Then
    '                    WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
    '                Else
    '                    WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
    '                End If
    '                    '32255 '65535
    '            Case 0
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
    '                dwXpos(0) = Now
    '            Case 65535
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
    '                dwXpos(0) = Now
    '        End Select
    '    ElseIf state.dwXpos = 0 Then
    '        If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
    '                dwXpos(1) = Now
    '            End If
    '        End If
    '    ElseIf state.dwXpos = 65535 Then
    '        If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
    '                dwXpos(1) = Now
    '            End If
    '        End If
    '    End If

    '    If state.dwButtons <> newState.dwButtons Then
    '        'Debug.Print(state.dwButtons)
    '        If (newState.dwButtons And 1) = 1 Then
    '            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.Return)
    '        ElseIf (state.dwButtonNumber And 1) = 1 Then
    '            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.Return)
    '        End If

    '        If (newState.dwButtons And 512) Then
    '            'guide buttong
    '            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.F12)
    '            button512 = Now

    '            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '                If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
    '                    dwYpos(1) = Now
    '                End If
    '            End If
    '        ElseIf (state.dwButtonNumber And 512) Then
    '            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.F12)
    '        End If
    '    End If
    '    state = newState
    '    stateIsEmpty = False
    '    'End If
    'End Sub

    Public Class KeyboardHook

        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal HookProc As KBDLLHookProc, ByVal hInstance As IntPtr, ByVal wParam As Integer) As Integer
        End Function
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function CallNextHookEx(ByVal idHook As Integer, ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
        End Function
        <DllImport("User32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
        Private Overloads Shared Function UnhookWindowsHookEx(ByVal idHook As Integer) As Boolean
        End Function

        <StructLayout(LayoutKind.Sequential)>
        Private Structure KBDLLHOOKSTRUCT
            Public vkCode As UInt32
            Public scanCode As UInt32
            Public flags As KBDLLHOOKSTRUCTFlags
            Public time As UInt32
            Public dwExtraInfo As UIntPtr
        End Structure

        <Flags()>
        Private Enum KBDLLHOOKSTRUCTFlags As UInt32
            LLKHF_EXTENDED = &H1
            LLKHF_INJECTED = &H10
            LLKHF_ALTDOWN = &H20
            LLKHF_UP = &H80
        End Enum
        Public Class KeyEventArgsEx
            Inherits KeyEventArgs
            Public Sub New(keyData As Keys)
                MyBase.New(keyData)
            End Sub
            Public Time As Integer
            Public Repeat As Integer
            Public CreateNoMoreEvents As Boolean
        End Class
        Public Event KeyDown(ByVal e As KeyEventArgsEx)
        Public Event KeyUp(ByVal e As KeyEventArgsEx)

        Private Const WH_KEYBOARD_LL As Integer = 13
        Private Const HC_ACTION As Integer = 0
        Private Const WM_KEYDOWN = &H100
        Private Const WM_KEYUP = &H101
        Private Const WM_SYSKEYDOWN = &H104
        Private Const WM_SYSKEYUP = &H105

        Private Delegate Function KBDLLHookProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer

        Private KBDLLHookProcDelegate As KBDLLHookProc = New KBDLLHookProc(AddressOf KeyboardProc)
        Private HHookID As IntPtr = IntPtr.Zero
        Private keyArray([Enum].GetValues(GetType(Keys)).Length) As Integer
        <DllImport("user32")>
        Private Shared Function SetKeyboardState(lpKeyState As Byte()) As Boolean

        End Function
        <DllImport("user32")>
        Private Shared Function GetKeyboardState(ByVal lpKeyState As Byte()) As Boolean
        End Function

        Private Function KeyboardProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
            If (nCode = HC_ACTION) Then
                Dim struct As KBDLLHOOKSTRUCT
                struct = CType(Marshal.PtrToStructure(lParam, struct.GetType()), KBDLLHOOKSTRUCT)
                'Dim repeat = struct.time
                'Debug.Print(repeat & " " & wParam.ToInt64)
                Select Case wParam
                    Case WM_KEYDOWN, WM_SYSKEYDOWN
                        Dim v = [Enum].GetValues(GetType(Keys))
                        Dim i = Array.IndexOf(v, CType(struct.vkCode, Keys)) ' Array.IndexOf([Enum].GetValues(GetType(Keys)), struct.vkCode)
                        If keyArray(i) >= 0 Then
                            Dim e As New KeyEventArgsEx(CType(struct.vkCode, Keys)) With {.Time = struct.time, .Repeat = keyArray(i)}
                            RaiseEvent KeyDown(e)

                            If e.CreateNoMoreEvents Then
                                e.SuppressKeyPress = True
                                keyArray(i) = -1
                                Dim b(255) As Byte
                                GetKeyboardState(b)
                                b(i) = 0
                                SetKeyboardState(b)
                            ElseIf keyArray(i) >= 0 Then
                                keyArray(i) += 1
                            End If

                            If e.SuppressKeyPress Then Return 1

                        Else
                            Return 1
                        End If
                    Case WM_KEYUP, WM_SYSKEYUP
                        Dim v = [Enum].GetValues(GetType(Keys))
                        Dim i = Array.IndexOf(v, CType(struct.vkCode, Keys)) ' Array.IndexOf([Enum].GetValues(GetType(Keys)), struct.vkCode)
                        If i >= 0 Then
                            Dim e As New KeyEventArgsEx(CType(struct.vkCode, Keys)) With {.Time = struct.time, .Repeat = keyArray(i)}
                        RaiseEvent KeyUp(e)


                        keyArray(i) = 0
                            If e.SuppressKeyPress Then Return 1
                        End If
                End Select
            End If

            Return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam)
        End Function


        Public Sub New()
            HHookID = SetWindowsHookEx(WH_KEYBOARD_LL, KBDLLHookProcDelegate, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly.GetModules()(0)).ToInt32, 0)
            If HHookID = IntPtr.Zero Then
                Throw New Exception("Could not set keyboard hook")
            End If
        End Sub
        Public Sub StopHook()
            If Not HHookID = IntPtr.Zero Then
                Dim b(255) As Byte
                SetKeyboardState(b)
                UnhookWindowsHookEx(HHookID)
                HHookID = IntPtr.Zero
            End If
        End Sub

        Protected Overrides Sub Finalize()
            If Not HHookID = IntPtr.Zero Then
                Dim b(255) As Byte
                SetKeyboardState(b)
                UnhookWindowsHookEx(HHookID)
            End If
            MyBase.Finalize()
        End Sub



    End Class
End Class