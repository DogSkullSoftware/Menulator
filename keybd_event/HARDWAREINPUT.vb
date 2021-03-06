Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput
    '#pragma warning disable 649
    ''' <summary>
    ''' The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard Or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    ''' Declared in Winuser.h, include Windows.h
    ''' </summary>
    Structure HARDWAREINPUT
        ''' <summary>
        ''' Value specifying the message generated by the input hardware. 
        ''' </summary>
        Public Msg As UInt32

        ''' <summary>
        ''' Specifies the low-order word of the lParam parameter for uMsg. 
        ''' </summary>
        Public ParamL As UInt16

        ''' <summary>
        ''' Specifies the high-order word of the lParam parameter for uMsg. 
        ''' </summary>
        Public ParamH As UInt16
    End Structure
    '#pragma warning restore 649
End Namespace
