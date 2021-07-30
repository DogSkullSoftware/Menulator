Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput
    ''' <summary>
    ''' Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
    ''' </summary>
    Public Enum KeyboardFlag As UInteger '// UInt32
        ''' <summary>
        ''' KEYEVENTF_EXTENDEDKEY = &amp;H0001 (If specified the scan code was preceded by a prefix byte that has the value &amp;HE0 (224).)
        ''' </summary>
        EXTENDEDKEY = &H1

        ''' <summary>
        ''' KEYEVENTF_KEYUP = &amp;H0002 (If specified the key Is being released. If Not specified the key Is being pressed.)
        ''' </summary>
        KEYUP = &H2

        ''' <summary>
        ''' KEYEVENTF_UNICODE = &amp;H0004 (If specified wScan identifies the key And wVk Is ignored.)
        ''' </summary>
        UNICODE = &H4

        ''' <summary>
        ''' KEYEVENTF_SCANCODE = &amp;H0008 (Windows 2000/XP: If specified the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined With the KEYEVENTF_KEYUP flag. For more information see the Remarks section.)
        ''' </summary>
        SCANCODE = &H8
    End Enum
End Namespace
