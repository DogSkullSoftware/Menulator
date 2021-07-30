Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput
    ''' <summary>
    ''' Specifies the type of the input event. This member can be one of the following values. 
    ''' </summary>
    Public Enum InputType As UInteger '// UInt32
        ''' <summary>
        ''' INPUT_MOUSE = 0x00 (The event Is a mouse event. Use the mi structure of the union.)
        ''' </summary>
        MOUSE = 0

        ''' <summary>
        ''' INPUT_KEYBOARD = 0x01 (The event Is a keyboard event. Use the ki structure of the union.)
        ''' </summary>
        KEYBOARD = 1

        ''' <summary>
        ''' INPUT_HARDWARE = 0x02 (Windows 95/98/Me: The Event is from input hardware other than a keyboard Or mouse. Use the hi Structure Of the union.)
        ''' </summary>
        HARDWARE = 2
    End Enum
End Namespace
