Imports System.Runtime.InteropServices
Imports Menulator_Zero.JoyApi

Public Class JoyStickWindow
    Inherits Windows.Forms.Form


    <StructLayout(LayoutKind.Explicit)>
    Public Structure JoyPosition
        <FieldOffset(0)>
        Public Raw As IntPtr
        <FieldOffset(0)>
        Public XPos As UShort
        <FieldOffset(2)>
        Public YPos As UShort
    End Structure
    <DllImport("User32")> Private Shared Function SetTimer(hWnd As IntPtr, nIDEvent As Integer, uElapse As Integer, lpTimerFunc As TIMERPROC) As IntPtr

    End Function
    <DllImport("User32")> Private Shared Function KillTimer(hWnd As IntPtr, uIDEvent As Integer) As Integer
    End Function

    Private Delegate Sub TIMERPROC(HWND As IntPtr, message As Integer, idTimer As IntPtr, dwTime As Integer)

    Private WithEvents JoyTimer As Timer

    Public Shared Controllers As Dictionary(Of Integer, Joystick)
    Public Event JoyStickAxisDown As EventHandler(Of Joystick.JoyStickAxisEventArgs)
    Public Event JoyStickAxisUp As EventHandler(Of Joystick.JoyStickAxisEventArgs)
    Public Event JoyStickAxisPress As EventHandler(Of Joystick.JoyStickAxisPressEventArgs)
    Public Event JoyStickButtonDown As EventHandler(Of Joystick.JoyStickButtonEventArgs)
    Public Event JoyStickButtonUp As EventHandler(Of Joystick.JoyStickButtonEventArgs)
    Public Event JoyStickButtonPress As EventHandler(Of Joystick.JoyStickButtonPressEventArgs)
    Public Event JoyStickPOVDown As EventHandler(Of Joystick.JoyStickButtonEventArgs)
    Public Event JoyStickPOVUp As EventHandler(Of Joystick.JoyStickButtonEventArgs)
    Public Event JoyStickPOVPress As EventHandler(Of Joystick.JoyStickButtonPressEventArgs)
    Public Event JoyStickPoll As EventHandler(Of Joystick.JoyStickChangedArgs)

    Public Property StopPolling As Boolean = False
    Shared Sub New()
        Dim e = Joystick.JoyManager.EnumerateDevices()

        Controllers = New Dictionary(Of Integer, Joystick)
        For Each joy In e
            Controllers.Add(joy.Value.JoyID, New Joystick(joy.Value.JoyID))
        Next
    End Sub


    Public Sub New()
        MyBase.New
        If Me.DesignMode Then Return
        Try
            'Dim e = Joystick.JoyManager.EnumerateDevices()

            'Controllers = New Dictionary(Of Integer, Joystick)
            For Each joy In Controllers
                ' Controllers.Add(joy.Value.JoyID, New Joystick(joy.Value.JoyID))
                With Controllers(joy.Value.joyIndex)
                    AddHandler .JoyStickAxisDown, AddressOf OnJoyStickAxisDown
                    AddHandler .JoyStickAxisPress, AddressOf OnJoyStickAxisPress
                    AddHandler .JoyStickAxisUp, AddressOf OnJoyStickAxisUp
                    AddHandler .JoyStickButtonDown, AddressOf OnJoyStickButtonDown
                    AddHandler .JoyStickButtonPress, AddressOf OnJoyStickButtonPress
                    AddHandler .JoyStickButtonUp, AddressOf OnJoyStickButtonUp
                    AddHandler .JoystickChanged, AddressOf OnJoystickChanged
                    AddHandler .JoyStickPOVDown, AddressOf OnJoyStickPOVDown
                    AddHandler .JoyStickPOVPress, AddressOf OnJoyStickPOVPress
                    AddHandler .JoyStickPOVUp, AddressOf OnJoyStickPOVUp
                    AddHandler .JoystickChanged, AddressOf OnJoyStickPoll
                End With
            Next
        Catch
        End Try
        JoyTimer = New Timer()
    End Sub

    Private Sub JoyTimer_Tick(sender As Object, e As EventArgs) Handles JoyTimer.Tick
        If StopPolling Then Return
        If Controllers IsNot Nothing Then
            OnJoyStickPrePoll(Me, Nothing)
            For Each joy In Controllers
                joy.Value.Poll()
            Next
        End If
    End Sub
    'Private Sub MyTimer(HWND As IntPtr, message As Integer, idTimer As IntPtr, dwTime As Integer)
    '    'Static lastTimestamp As Long = WinmmApi.Joystick.NativeMethods.GetTickCount64
    '    'Static Counter As Integer = 0
    '    'Static PPS(100) As Double
    '    'PPS(Counter) = e.TimeStamp - lastTimestamp
    '    'Me.Text = Math.Round(PPS.Average, 2)
    '    'PollsPerSecondAvg = Math.Round(PPS.Average, 0)
    '    'Counter += 1
    '    'If Counter > 100 Then Counter = 0
    '    'lastTimestamp = e.TimeStamp
    '    If Controllers IsNot Nothing Then
    '        OnJoyStickPrePoll(Me, Nothing)
    '        For Each joy In Controllers
    '            joy.Value.Poll()
    '        Next
    '    End If

    'End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        If Me.DesignMode = False Then
            'SetTimer(Me.Handle, 5, 10, AddressOf MyTimer)
            JoyTimer.Interval = 10
            JoyTimer.Enabled = True
        End If
    End Sub
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        'KillTimer(Handle, 5)
        JoyTimer.Enabled = False
        MyBase.OnFormClosing(e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickAxisDown(sender As Object, e As Joystick.JoyStickAxisEventArgs)
        RaiseEvent JoyStickAxisDown(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickAxisPress(sender As Object, e As Joystick.JoyStickAxisPressEventArgs)
        RaiseEvent JoyStickAxisPress(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickAxisUp(sender As Object, e As Joystick.JoyStickAxisEventArgs)
        RaiseEvent JoyStickAxisUp(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickButtonDown(sender As Object, e As Joystick.JoyStickButtonEventArgs)
        RaiseEvent JoyStickButtonDown(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickButtonPress(sender As Object, e As Joystick.JoyStickButtonPressEventArgs)
        RaiseEvent JoyStickButtonPress(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickButtonUp(sender As Object, e As Joystick.JoyStickButtonEventArgs)
        RaiseEvent JoyStickButtonUp(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoystickChanged(sender As Object, e As Joystick.JoyStickChangedArgs)

    End Sub

    Protected Friend Overridable Sub OnJoyStickPOVDown(sender As Object, e As Joystick.JoyStickButtonEventArgs)
        RaiseEvent JoyStickPOVDown(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickPOVPress(sender As Object, e As Joystick.JoyStickButtonPressEventArgs)
        RaiseEvent JoyStickPOVPress(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickPOVUp(sender As Object, e As Joystick.JoyStickButtonEventArgs)
        RaiseEvent JoyStickPOVUp(sender, e)
    End Sub
    Protected Friend Overridable Sub OnJoyStickPoll(sender As Object, e As Joystick.JoyStickChangedArgs)
        RaiseEvent JoyStickPoll(sender, e)
    End Sub

    Protected Friend Overridable Sub OnJoyStickPrePoll(sender As Object, e As EventArgs)

    End Sub



    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Dim ret = JoyApi.Joystick.NativeMethods.joySetCapture(Me.Handle, 0, 10, -1)
    End Sub
    Protected Overrides Sub OnHandleDestroyed(e As EventArgs)
        For Each joy In Controllers
            ' Controllers.Add(joy.Value.JoyID, New Joystick(joy.Value.JoyID))
            With Controllers(joy.Value.joyIndex)
                RemoveHandler .JoyStickAxisDown, AddressOf OnJoyStickAxisDown
                RemoveHandler .JoyStickAxisPress, AddressOf OnJoyStickAxisPress
                RemoveHandler .JoyStickAxisUp, AddressOf OnJoyStickAxisUp
                RemoveHandler .JoyStickButtonDown, AddressOf OnJoyStickButtonDown
                RemoveHandler .JoyStickButtonPress, AddressOf OnJoyStickButtonPress
                RemoveHandler .JoyStickButtonUp, AddressOf OnJoyStickButtonUp
                RemoveHandler .JoystickChanged, AddressOf OnJoystickChanged
                RemoveHandler .JoyStickPOVDown, AddressOf OnJoyStickPOVDown
                RemoveHandler .JoyStickPOVPress, AddressOf OnJoyStickPOVPress
                RemoveHandler .JoyStickPOVUp, AddressOf OnJoyStickPOVUp
                RemoveHandler .JoystickChanged, AddressOf OnJoyStickPoll
            End With
        Next
        MyBase.OnHandleDestroyed(e)
        JoyApi.Joystick.NativeMethods.joyReleaseCapture(0)
    End Sub
    Private Const MM_JOY1MOVE As Integer = &H3A0 '//	Joystick JOYSTICKID1 changed position In the x- Or y-direction.
    Private Const MM_JOY2MOVE As Integer = &H3A1 '//  Joystick JOYSTICKID2 changed position In the x- Or y-direction
    Private Const MM_JOY1ZMOVE As Integer = &H3A2 '//	Joystick JOYSTICKID1 changed position In the z-direction.
    Private Const MM_JOY2ZMOVE As Integer = &H3A3 '//	Joystick JOYSTICKID2 changed position In the z-direction.
    Private Const MM_JOY1BUTTONDOWN As Integer = &H3B5 '//	A button On joystick JOYSTICKID1 has been pressed.
    Private Const MM_JOY2BUTTONDOWN As Integer = &H3B6 '//  A button On joystick JOYSTICKID2 has been pressed.
    Private Const MM_JOY1BUTTONUP As Integer = &H3B7 '//	A button On joystick JOYSTICKID1 has been released.
    Private Const MM_JOY2BUTTONUP As Integer = &H3B8

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Select Case m.Msg
            Case MM_JOY1MOVE, MM_JOY1BUTTONDOWN, MM_JOY1BUTTONUP, MM_JOY1ZMOVE, MM_JOY2BUTTONDOWN, MM_JOY2BUTTONUP, MM_JOY2MOVE, MM_JOY2ZMOVE, MM_JOY2MOVE + 1, MM_JOY2MOVE + 2, MM_JOY2MOVE + 3
                ' Joystick co-ords.
                ' (0,0) (32768,0) (65535, 0)
                '
                '
                '
                ' (0, 32768) (32768, 32768) (65535, 32768)
                '
                '
                '
                '
                ' (0, 65535) (32768, 65535) (65535, 65535)
                '
                Dim p As JoyPosition
                p.Raw = m.LParam
                ' RaiseEvent Move(New Point(p.XPos, p.YPos))
                'If p.XPos > 16384 AndAlso p.XPos < 49152 Then
                '    ' X is near the centre line.
                '    If p.YPos < 6000 Then
                '        ' Y is near the top.
                '        'RaiseEvent Up()
                '        RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(p, m.WParam, "Up"))
                '    ElseIf p.YPos > 59536 Then
                '        ' Y is near the bottom.
                '        'RaiseEvent Down()
                '        RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(p, m.WParam, "Down"))

                '    End If
                'Else
                '    If p.YPos > 16384 AndAlso p.YPos < 49152 Then
                '        ' Y is near the centre line
                '        If p.XPos < 6000 Then
                '            ' X is near the left.
                '            'RaiseEvent Left()
                '            RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(p, m.WParam, "Left"))

                '        ElseIf p.XPos > 59536 Then
                '            ' X is near the right
                '            'RaiseEvent Right()
                '            RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(p, m.WParam, "Right"))

                '        End If
                '    End If
                'End If
                'If btnValue <> m.WParam.ToString Then
                '    RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(p, m.WParam, "Button"))
                '    btnValue = m.WParam.ToString
                'End If
                'If m.WParam.ToString = 512 Then
                '    RaiseEvent JoyStickButtonDown(Controllers(m.WParam), New Joystick.JoyStickButtonEventArgs(New Joystick.NativeMethods.JOYINFOEX With {.dwButtonNumber = 255}, Nothing, m.WParam, 1, 0))
                'End If
                'Debug.Print(m.WParam.ToString)
        End Select
        MyBase.WndProc(m)
    End Sub
End Class

