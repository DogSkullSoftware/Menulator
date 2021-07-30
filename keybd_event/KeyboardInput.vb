Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput

    '#pragma warning disable 649
    ''' <summary>
    ''' The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http:''msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
    ''' Declared in Winuser.h, include Windows.h
    ''' </summary>
    ''' <remarks>
    ''' Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch As handwriting recognition Or voice recognitionAs If it were text input by Imports the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE Is specified, SendInput sends a WM_KEYDOWN Or WM_KEYUP message To the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
    ''' Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag To define keyboard input In terms Of the scan code. This Is useful To simulate a physical keystroke regardless Of which keyboard Is currently being used. The virtual key value Of a key may alter depending On the current keyboard layout Or what other keys were pressed, but the scan code will always be the same.
    ''' </remarks>
    Structure KEYBDINPUT

        ''' <summary>
        ''' Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
        ''' </summary>
        Public Vk As UInt16

        ''' <summary>
        ''' Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which Is to be sent to the foreground application. 
        ''' </summary>
        Public Scan As UInt16

        ''' <summary>
        ''' Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
        ''' KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value &amp;HE0 (224).
        ''' KEYEVENTF_KEYUP - If specified, the key Is being released. If Not specified, the key Is being pressed.
        ''' KEYEVENTF_SCANCODE - If specified, wScan identifies the key And wVk Is ignored. 
        ''' KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined With the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
        ''' </summary>
        Public Flags As UInt32

        ''' <summary>
        ''' Time stamp for the event, in milliseconds. If this parameter Is zero, the system will provide its own time stamp. 
        ''' </summary>
        Public Time As UInt32

        ''' <summary>
        ''' Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
        ''' </summary>
        Public ExtraInfo As IntPtr
    End Structure
    '#pragma warning restore 649
End Namespace
