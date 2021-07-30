Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

Class Program

    Const INPUT_MOUSE As Integer = 0
    Const INPUT_KEYBOARD As Integer = 1
    Const INPUT_HARDWARE As Integer = 2
    Const KEYEVENTF_EXTENDEDKEY As Integer = &H1
    Const KEYEVENTF_KEYUP As Integer = &H2
    Const KEYEVENTF_UNICODE As Integer = &H4
    Const KEYEVENTF_SCANCODE As Integer = &H8

    Structure INPUT
        Public type As Integer
        Public u As InputUnion
    End Structure

    <StructLayout(LayoutKind.Explicit)>
    Structure InputUnion
        <FieldOffset(0)>
        Public mi As MOUSEINPUT
        <FieldOffset(0)>
        Public ki As KEYBDINPUT
        <FieldOffset(0)>
        Public hi As HARDWAREINPUT
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure MOUSEINPUT
        Public dx As Integer
        Public dy As Integer
        Public mouseData As Integer
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure KEYBDINPUT
        Public wVk As Short
        Public wScan As Short
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Structure HARDWAREINPUT
        Public uMsg As Integer
        Public wParamL As Short
        Public wParamH As Short
    End Structure

    <DllImport("user32.dll")>
    Private Shared Function GetMessageExtraInfo() As IntPtr()
    End Function
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SendInput(nInputs As Integer, pInputs As INPUT(), cbSize As Integer) As Integer
    End Function


    Shared Sub SendKey(key As Char)

        '// convert a normal key to the scan code
        '// https://gist.github.com/tracend/912308
        Dim scanKey As Short = 0
        Select Case key

            Case "5"c
                scanKey = &H6
            Case "1"c
                scanKey = &H2
            Case "l"c
                scanKey = &HCB
            Case "r"c
                scanKey = &HCD
            Case "u"c
                scanKey = &HC8
            Case "d"c
                scanKey = &HD0
            Case Else
                Throw New NotImplementedException()
        End Select

        Dim keyPress(0) As INPUT
        keyPress(0) = New INPUT With {.type = INPUT_KEYBOARD}
        keyPress(0).u = New InputUnion With {.ki = New KEYBDINPUT With {.wVk = 0, .wScan = scanKey, .dwFlags = KEYEVENTF_SCANCODE, .dwExtraInfo = IntPtr.Zero}}



        '// Key DOWN
        If (SendInput(keyPress.Length, keyPress, Marshal.SizeOf(GetType(INPUT))) = 0) Then
            Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString())
        End If
        Thread.Sleep(50) '; // MAME Emulator requires this

        '// key UP
        keyPress(0).u.ki.dwFlags = KEYEVENTF_KEYUP Or KEYEVENTF_SCANCODE
        If (SendInput(keyPress.Length, keyPress, Marshal.SizeOf(GetType(INPUT))) = 0) Then
            Console.WriteLine("SendInput failed with code: " + Marshal.GetLastWin32Error().ToString())
        End If
        Thread.Sleep(50) '; // MAME Emulator requires this
    End Sub

    'Static void Main(String[] args)
    '{
    '    Console.WriteLine("TODO: Automatically set focus to MAME Emulator");
    '    Console.WriteLine("Waiting for you to do that...");
    '    Thread.Sleep(5000);
    '    Console.WriteLine("Sending keys...");

    '    // add a credit
    '    SendKey('5');

    '    // player 1
    '    SendKey('1');

    '    // wait for the game to start up...
    '    Thread.Sleep(2000);

    '    // left, then right, then left, ... )
    '    For (int i = 0; i < 50; i++)
    '    {
    '        SendKey('l');
    '        SendKey('r');
    '    }
    '}
End Class