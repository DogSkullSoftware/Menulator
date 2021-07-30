Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput

    '#pragma warning disable 649
    ''' <summary>
    ''' The MOUSEINPUT structure contains information about a simulated mouse event. (see: http:''msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
    ''' Declared in Winuser.h, include Windows.h
    ''' </summary>
    ''' <remarks>
    ''' If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dxand dy specify information about that movement. The information Is specified as absolute Or relative integer values. 
    ''' If MOUSEEVENTF_ABSOLUTE value Is specified, dx And dy contain normalized absolute coordinates between 0 And 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 
    ''' Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK Is specified, the coordinates map To the entire virtual desktop.
    ''' If the MOUSEEVENTF_ABSOLUTE value Is Not specified, dxand dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (Or down) negative values mean the mouse moved left (Or up). 
    ''' Relative mouse motion Is subject to the effects of the mouse speed And the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values imports the SystemParametersInfo function. 
    ''' The system applies two tests to the specified relative mouse movement. If the specified distance along either the x Or y axis Is greater than the first mouse threshold value, And the mouse speed Is Not zero, the system doubles the distance. If the specified distance along either the x Or y axis Is greater than the second mouse threshold value, And the mouse speed Is equal to two, the system doubles the distance that resulted from applying the first threshold test. It Is thus possible for the system to multiply specified relative mouse movement along the x Or y axis by up to four times.
    ''' </remarks>
    Structure MOUSEINPUT

        ''' <summary>
        ''' Specifies the absolute position of the mouse, Or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data Is specified as the x coordinate of the mouse relative data Is specified as the number of pixels moved. 
        ''' </summary>
        Public X As Int32

        ''' <summary>
        ''' Specifies the absolute position of the mouse, Or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data Is specified as the y coordinate of the mouse relative data Is specified as the number of pixels moved. 
        ''' </summary>
        Public Y As Int32

        ''' <summary>
        ''' If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user a negative value indicates that the wheel was rotated backward, toward the user. One wheel click Is defined as WHEEL_DELTA, which Is 120. 
        ''' Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, Then dwData specifies the amount Of wheel movement. A positive value indicates that the wheel was rotated To the right a negative value indicates that the wheel was rotated To the left. One wheel click Is defined As WHEEL_DELTA, which Is 120.
        ''' Windows 2000/XP: IfdwFlags does Not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, Or MOUSEEVENTF_XUP, Then mouseData should be zero. 
        ''' If dwFlags contains MOUSEEVENTF_XDOWN Or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed Or released. This value may be any combination of the following flags. 
        ''' </summary>
        Public MouseData As UInt32

        ''' <summary>
        ''' A set of bit flags that specify various aspects of mouse motion And button clicks. The bits in this member can be any reasonable combination of the following values. 
        ''' The bit flags that specify mouse button status are set to indicate changes in status, Not ongoing conditions. For example, if the left mouse button Is pressed And held down, MOUSEEVENTF_LEFTDOWN Is set when the left button Is first pressed, but Not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP Is set only when the button Is first released. 
        ''' You cannot specify both the MOUSEEVENTF_WHEEL flag And either MOUSEEVENTF_XDOWN Or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
        ''' </summary>
        Public Flags As UInt32

        ''' <summary>
        ''' Time stamp for the event, in milliseconds. If this parameter Is 0, the system will provide its own time stamp. 
        ''' </summary>
        Public Time As UInt32

        ''' <summary>
        ''' Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
        ''' </summary>
        Public ExtraInfo As IntPtr
    End Structure
    '#pragma warning restore 649
End Namespace