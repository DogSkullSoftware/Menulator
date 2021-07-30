Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput
    '#pragma warning disable 649
    ''' <summary>
    ''' The INPUT structure Is used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, And mouse clicks. (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    ''' Declared in Winuser.h, include Windows.h
    ''' </summary>
    ''' <remarks>
    ''' This structure contains information identical to that used in the parameter list of the keybd_event Or mouse_event function.
    ''' Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard input methods, such As handwriting recognition Or voice recognition, As If it were text input by Imports the KEYEVENTF_UNICODE flag. For more information, see the remarks section Of KEYBDINPUT.
    ''' </remarks>
    Structure INPUT
        ''' <summary>
        ''' Specifies the type of the input event. This member can be one of the following values. 
        ''' InputType.MOUSE - The event Is a mouse event. Use the mi structure of the union.
        ''' InputType.KEYBOARD - The event Is a keyboard event. Use the ki structure of the union.
        ''' InputType.HARDWARE - Windows 95/98/Me: The Event is() from input hardware other than a keyboard Or mouse. Use the hi Structure Of the union.
        ''' </summary>
        Public Type As UInt32

        ''' <summary>
        ''' The data structure that contains information about the simulated Mouse, Keyboard Or Hardware event.
        ''' </summary>
        Public Data As MOUSEKEYBDHARDWAREINPUT
    End Structure
    '#pragma warning restore 649
End Namespace
