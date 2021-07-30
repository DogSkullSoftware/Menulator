Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput

    ''' <summary>
    ''' XButton definitions for use in the MouseData property of the <see cref="MOUSEINPUT"/> structure. (See: http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx)
    ''' </summary>
    Public Enum XButton As UInteger
        ''' <summary>
        ''' Set if the first X button Is pressed Or released.
        ''' </summary>
        XBUTTON1 = &H1

        ''' <summary>
        ''' Set if the second X button Is pressed Or released.
        ''' </summary>
        XBUTTON2 = &H2
    End Enum
End Namespace
