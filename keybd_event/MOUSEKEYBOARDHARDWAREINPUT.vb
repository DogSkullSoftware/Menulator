Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.InteropServices

Namespace WindowsInput

    '#pragma warning disable 649
    ''' <summary>
    ''' The combined/overlayed structure that includes Mouse, Keyboard And Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    ''' </summary>
    <StructLayout(LayoutKind.Explicit)>
    Structure MOUSEKEYBDHARDWAREINPUT
        <FieldOffset(0)>
        Public Mouse As MOUSEINPUT

        <FieldOffset(0)>
        Public Keyboard As KEYBDINPUT

        <FieldOffset(0)>
        Public Hardware As HARDWAREINPUT
    End Structure
    '#pragma warning restore 649
End Namespace
