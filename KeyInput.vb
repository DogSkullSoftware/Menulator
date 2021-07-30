Imports System.Runtime.InteropServices



Public Class KeyInput
    Friend Enum INPUT_TYPE As Integer
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
    Private Structure INPUT
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

    Private Declare Function SendInput Lib "user32" (ByVal nInputs As Integer, <MarshalAs(UnmanagedType.LPArray)> ByVal pInputs() As INPUT, ByVal cbSize As Integer) As Integer

    Private Declare Function AttachThreadInput Lib "user32" (ByVal idAttach As IntPtr, ByVal idAttachTo As IntPtr, ByVal fAttach As Boolean) As Boolean
    Private Declare Function GetCurrentThreadId Lib "kernel32" () As IntPtr
    Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As IntPtr, ByVal lpwdProcessId As IntPtr) As IntPtr

    Declare Function keybd_event Lib "user32" Alias "keybd_event" _
          (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer,
           ByVal dwExtraInfo As Integer) As Integer
    Const KEYEVENTF_KEYDOWN As Integer = &H0 ' Press key        
    Const KEYEVENTF_KEYUP As Integer = &H2 ' Release key 

    Public Enum SendMethod
        SendKeys
        KeyboardEvent
        SendInput
    End Enum

    Public Shared Sub SendKey(method As SendMethod, k As Keys, handle As IntPtr)
        Dim pid = GetWindowThreadProcessId(handle, IntPtr.Zero)
        AttachThreadInput(GetCurrentThreadId, pid, True)
        AppActivate(pid.ToInt32)
        Select Case method
            Case SendMethod.SendKeys

                SendKeys.Send([Enum].GetName(GetType(Keys), k))

            Case SendMethod.KeyboardEvent
                keybd_event(k, 0, 0, 0)
            Case SendMethod.SendInput
                Dim i(1) As INPUT
                i(0).ki.wVk = k
                i(1).ki.wVk = k
                i(1).ki.dwFlags = 2
                SendInput(2, i, Marshal.SizeOf(i))
        End Select
        AttachThreadInput(GetCurrentThreadId, pid, False)
    End Sub

End Class
