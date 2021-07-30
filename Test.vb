Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports XInput
Imports XInput.XInputWrapper

Public Class Test
    Dim p As Process
    Dim WithEvents pad As XInput.XInputWrapper.XboxController = XInputWrapper.XboxController.RetrieveController(0)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        p = Process.Start("d:\games\emulation\mame\mame64.exe", "-window")
    End Sub

    Private Sub pad_StateChanged(sender As Object, e As XboxControllerStateChangedEventArgs) Handles pad.StateChanged
        If e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_A) Then
            KeyInput.SendKey(KeyInput.SendMethod.KeyboardEvent, Keys.A, p.Handle)
        End If

    End Sub
    Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer, ByRef lpvParam As Integer, ByVal fuWinIni As Integer) As Integer

    Shared KeyboardDelay As Integer, KeyboardRepeatSpeed As Integer

    Shared Sub New()

        SystemParametersInfo(22, 0, KeyboardDelay, 0)
        Select Case KeyboardDelay
            Case 0
                KeyboardDelay = 250
            Case 1
                KeyboardDelay = 500
            Case 2
                KeyboardDelay = 750
            Case 3
                KeyboardDelay = 1000
        End Select
        SystemParametersInfo(10, 0, KeyboardRepeatSpeed, 0)
        Select Case KeyboardRepeatSpeed
            Case 0 : KeyboardRepeatSpeed = 400
            Case 1 : KeyboardRepeatSpeed = 363
            Case 2 : KeyboardRepeatSpeed = 352
            Case 3 : KeyboardRepeatSpeed = 341
            Case 4 : KeyboardRepeatSpeed = 330
            Case 5 : KeyboardRepeatSpeed = 319
            Case 6 : KeyboardRepeatSpeed = 308
            Case 7 : KeyboardRepeatSpeed = 297
            Case 8 : KeyboardRepeatSpeed = 286
            Case 9 : KeyboardRepeatSpeed = 275
            Case 10 : KeyboardRepeatSpeed = 264
            Case 11 : KeyboardRepeatSpeed = 253
            Case 12 : KeyboardRepeatSpeed = 242
            Case 13 : KeyboardRepeatSpeed = 231
            Case 14 : KeyboardRepeatSpeed = 220
            Case 15 : KeyboardRepeatSpeed = 209
            Case 16 : KeyboardRepeatSpeed = 198
            Case 17 : KeyboardRepeatSpeed = 187
            Case 18 : KeyboardRepeatSpeed = 176
            Case 19 : KeyboardRepeatSpeed = 165
            Case 20 : KeyboardRepeatSpeed = 154
            Case 21 : KeyboardRepeatSpeed = 143
            Case 22 : KeyboardRepeatSpeed = 114
            Case 23 : KeyboardRepeatSpeed = 105
            Case 24 : KeyboardRepeatSpeed = 96
            Case 25 : KeyboardRepeatSpeed = 88
            Case 26 : KeyboardRepeatSpeed = 75
            Case 27 : KeyboardRepeatSpeed = 69
            Case 28 : KeyboardRepeatSpeed = 60
            Case 29 : KeyboardRepeatSpeed = 51
            Case 30 : KeyboardRepeatSpeed = 42
            Case 31 : KeyboardRepeatSpeed = 33

        End Select
        XInput.XInputWrapper.XboxController.StartPolling()

    End Sub

    <StructLayout(LayoutKind.Sequential)>
    Public Structure JOYINFOEX
        Public dwSize As Integer
        Public dwFlags As Integer
        Public dwXpos As Integer
        Public dwYpos As Integer
        Public dwZpos As Integer
        Public dwRpos As Integer
        Public dwUpos As Integer
        Public dwVpos As Integer
        Public dwButtons As Integer
        Public dwButtonNumber As Integer
        Public dwPOV As Integer
        Public dwReserved1 As Integer
        Public dwReserved2 As Integer


    End Structure
    Dim state As JOYINFOEX
    Dim stateIsEmpty As Boolean = True
    Const JOYERR_BASE = 160
    Const JOYERR_UNPLUGGED = JOYERR_BASE + 7
    Declare Function joyGetPosEx Lib "winmm.dll" (ByVal uJoyID As Integer, ByRef pji As JOYINFOEX) As Integer

    Private Sub tmrJoy_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Static dwYpos(1) As DateTime, dwXpos(1) As DateTime, button512 As DateTime
        Dim newState As JOYINFOEX
        newState.dwSize = 64
        newState.dwFlags = &HFF

        'If JOYERR_UNPLUGGED Then
        joyGetPosEx(0, newState)
        '    stateIsEmpty = True
        'End If
        ''

        'If Not stateIsEmpty Then
        If state.dwYpos <> newState.dwYpos Then
            Select Case newState.dwYpos
                Case 32255
                    dwYpos(0) = Nothing
                    dwYpos(1) = Nothing
                    'a release, where did we come from?
                    If state.dwYpos = 0 Then
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
                        Debug.Print("Release UP")
                    Else
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
                        Debug.Print("Release DOWN")
                    End If
                        '32255 '65535
                Case 0
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
                    Debug.Print("Press UP")
                    dwYpos(0) = Now
                Case 65535
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
                    Debug.Print("Press DOWN")
                    dwYpos(0) = Now
            End Select
        ElseIf state.dwYpos = 0 Then
            'holding up
            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
                If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
                    dwYpos(1) = Now
                End If
            End If
        ElseIf state.dwYpos = 65535 Then
            'holding down
            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
                If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
                    dwYpos(1) = Now
                End If
            End If
        End If

        If state.dwXpos <> newState.dwXpos Then
            Select Case newState.dwXpos
                Case 32255
                    dwXpos(0) = Nothing
                    dwXpos(1) = Nothing
                    'a release, where did we come from?
                    If state.dwXpos = 0 Then
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
                    Else
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
                    End If
                        '32255 '65535
                Case 0
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                    dwXpos(0) = Now
                Case 65535
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                    dwXpos(0) = Now
            End Select
        ElseIf state.dwXpos = 0 Then
            If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
                If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                    dwXpos(1) = Now
                End If
            End If
        ElseIf state.dwXpos = 65535 Then
            If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
                If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                    dwXpos(1) = Now
                End If
            End If
        End If

        If state.dwButtons <> newState.dwButtons Then
            'Debug.Print(state.dwButtons)
            If (newState.dwButtons And 1) = 1 Then
                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.TAB)
            ElseIf (state.dwButtonNumber And 1) = 1 Then
                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.TAB)
            End If

            If (newState.dwButtons And 512) Then
                'guide buttong
                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.TAB)
                button512 = Now

                If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
                    If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
                        dwYpos(1) = Now
                    End If
                End If
            ElseIf (state.dwButtonNumber And 512) Then
                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.TAB)
            End If
        End If
        state = newState
        stateIsEmpty = False
        'End If
    End Sub



    Private Sub Test_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        XInput.XInputWrapper.XboxController.StopPolling()
        Timer1.Enabled = False
    End Sub
End Class