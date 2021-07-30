Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput

    ''' <summary>
    ''' The set of MouseFlags for use in the Flags property of the <see cref="MOUSEINPUT"/> structure. (See: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
    ''' </summary>
    Public Enum MouseFlag As UInteger

        ''' <summary>
        ''' Specifies that movement occurred.
        ''' </summary>
        MOVE = &H1

        ''' <summary>
        ''' Specifies that the left button was pressed.
        ''' </summary>
        LEFTDOWN = &H2

        ''' <summary>
        ''' Specifies that the left button was released.
        ''' </summary>
        LEFTUP = &H4

        ''' <summary>
        ''' Specifies that the right button was pressed.
        ''' </summary>
        RIGHTDOWN = &H8

        ''' <summary>
        ''' Specifies that the right button was released.
        ''' </summary>
        RIGHTUP = &H10

        ''' <summary>
        ''' Specifies that the middle button was pressed.
        ''' </summary>
        MIDDLEDOWN = &H20

        ''' <summary>
        ''' Specifies that the middle button was released.
        ''' </summary>
        MIDDLEUP = &H40

        ''' <summary>
        ''' Windows 2000/XP: Specifies that an X button was pressed.
        ''' </summary>
        XDOWN = &H80

        ''' <summary>
        ''' Windows 2000/XP: Specifies that an X button was released.
        ''' </summary>
        XUP = &H100

        ''' <summary>
        ''' Windows NT/2000/XP: Specifies that the wheel was moved If the mouse has a wheel. The amount Of movement Is specified In mouseData. 
        ''' </summary>
        WHEEL = &H800

        ''' <summary>
        ''' Windows 2000/XP: Maps coordinates To the entire desktop. Must be used With MOUSEEVENTF_ABSOLUTE.
        ''' </summary>
        VIRTUALDESK = &H4000

        ''' <summary>
        ''' Specifies that the dx And dy members contain normalized absolute coordinates. If the flag Is Not set dxand dy contain relative data (the change in position since the last reported position). This flag can be set Or Not set regardless of what kind of mouse Or other pointing device if any Is connected to the system. For further information about relative mouse motion see the following Remarks section.
        ''' </summary>
        ABSOLUTE = &H8000
    End Enum
End Namespace
