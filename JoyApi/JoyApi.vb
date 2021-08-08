Imports System.ComponentModel
Imports System.Runtime.InteropServices

Partial Public Class JoyApi


    '
    ' Summary:
    '     Represents a range that has start and end indexes.
    Public Structure Range

        '
        ' Summary:
        '     Instantiates a new System.Range instance with the specified starting and ending
        '     indexes.
        '
        ' Parameters:
        '   start:
        '     The inclusive start index of the range.
        '
        '   end:
        '     The exclusive end index of the range.
        Public Sub New(start As Integer, [end] As Integer)
            _Start = start
            _End = [end]
        End Sub

        '
        ' Summary:
        '     Gets a System.Range object that starts from the first element to the end.
        '
        ' Returns:
        '     A range from the start to the end.
        Public Shared ReadOnly Property All As Range


        '
        ' Summary:
        '     Gets an System.Index that represents the exclusive end index of the range.
        '
        ' Returns:
        '     The end index of the range.
        Public ReadOnly Property [End] As Integer


        '
        ' Summary:
        '     Gets the inclusive start index of the System.Range.
        '
        ' Returns:
        '     The inclusive start index of the range.
        Public ReadOnly Property Start As Integer



    End Structure
    Public Class Joystick



        'If you use windows messagaing, only get 4 buttons
        'Constant poll gives all info except Guide button

        'Dim MaxDevices As Integer
        Public lastInfo As NativeMethods.JOYINFOEX
        Public ReadOnly DeviceCaps As NativeMethods.JOYCAPS
        Public ReadOnly joyIndex As Integer


        Public Class JoyStickChangedArgs
            Inherits EventArgs

            Public ReadOnly TimeStamp As Long
            Public RawJoyInfo As NativeMethods.JOYINFOEX
            Public PrevJoyInfo As NativeMethods.JOYINFOEX
            Friend Sub New(current As NativeMethods.JOYINFOEX, prev As NativeMethods.JOYINFOEX)
                TimeStamp = NativeMethods.GetTickCount64
                RawJoyInfo = current
                PrevJoyInfo = prev
            End Sub
        End Class

        Public Class JoyStickAxisEventArgs
            Inherits EventArgs

            Public ReadOnly TimeStamp As Long
            Public RawJoyInfo As NativeMethods.JOYINFOEX
            Public prevJoyInfo As NativeMethods.JOYINFOEX
            Public ReadOnly Property AxisID As Integer
            Public ReadOnly Property Value As Integer

            Public ReadOnly Property Elapsed As Long
            Public Property Handled As Boolean

            Friend Sub New(current As NativeMethods.JOYINFOEX, prev As NativeMethods.JOYINFOEX, axisID As Integer, value As Integer, elapsed As Long)
                Me.prevJoyInfo = prev
                TimeStamp = NativeMethods.GetTickCount64
                RawJoyInfo = current
                Me.AxisID = axisID
                Me.Value = value

                Me.Elapsed = elapsed
            End Sub


        End Class
        Public Class JoyStickAxisPressEventArgs
            Inherits EventArgs

            Public ReadOnly TimeStamp As Long
            Public RawJoyInfo As NativeMethods.JOYINFOEX
            Public prevJoyInfo As NativeMethods.JOYINFOEX
            Public ReadOnly Property AxisID As Integer
            Public ReadOnly Property Value As Integer

            Public ReadOnly Property Elapsed As Long
            Public ReadOnly Property Counter As Integer
            Public Property Handled As Boolean
            Friend Sub New(current As NativeMethods.JOYINFOEX, prev As NativeMethods.JOYINFOEX, axisID As Integer, value As Integer, elapsed As Long, accumulator As Integer)
                TimeStamp = NativeMethods.GetTickCount64
                Me.prevJoyInfo = prev
                RawJoyInfo = current
                Me.AxisID = axisID
                Me.Value = value

                Me.Elapsed = elapsed
                Me.Counter = accumulator
            End Sub
        End Class

        Public Class JoyStickButtonEventArgs
            Inherits EventArgs

            Public ReadOnly TimeStamp As Long
            Public RawJoyInfo As NativeMethods.JOYINFOEX
            Public prevJoyInfo As NativeMethods.JOYINFOEX
            Public ReadOnly Property buttonID As Integer
            Public ReadOnly Property Value As Integer

            Public ReadOnly Property Elapsed As Long
            Public Property Handled As Boolean
            Friend Sub New(current As NativeMethods.JOYINFOEX, prev As NativeMethods.JOYINFOEX, buttonID As Integer, value As Integer, elapsed As Long)
                TimeStamp = NativeMethods.GetTickCount64
                Me.prevJoyInfo = prev
                RawJoyInfo = current
                Me.buttonID = buttonID
                Me.Value = value

                Me.Elapsed = elapsed
            End Sub


        End Class
        Public Class JoyStickButtonPressEventArgs
            Inherits EventArgs

            Public ReadOnly TimeStamp As Long
            Public RawJoyInfo As NativeMethods.JOYINFOEX
            Public prevJoyIndo As NativeMethods.JOYINFOEX
            Public ReadOnly Property ButtonID As Integer
            Public ReadOnly Property Value As Integer
            Public ReadOnly Property Elapsed As Long
            Public ReadOnly Property Counter As Integer
            Public Property Handled As Boolean

            Friend Sub New(current As NativeMethods.JOYINFOEX, prev As NativeMethods.JOYINFOEX, buttonID As Integer, value As Integer, elapsed As Long, accumulator As Integer)
                TimeStamp = NativeMethods.GetTickCount64
                Me.prevJoyIndo = prev
                RawJoyInfo = current
                Me.ButtonID = buttonID
                Me.Value = value

                Me.Elapsed = elapsed
                Me.Counter = accumulator
            End Sub
        End Class
        Public Event JoystickChanged As EventHandler(Of JoyStickChangedArgs)



        Public NotInheritable Class NativeMethods
            <DllImport("kernel32.dll")> Public Shared Function GetTickCount64() As Int64

            End Function
            Public Class JoystickNotFoundException
                Inherits Exception
                Friend Sub New(e As MMSYSERR)
                    MyBase.New(e.ToString)
                End Sub
            End Class
            Public Class JoystickNoDevCapsException
                Inherits Exception

                Friend Sub New(e As MMSYSERR)
                    MyBase.New(e.ToString)

                End Sub
            End Class

            Const MAXPNAMELEN = 32
            Const MAXERRORLENGTH = 256
            Const MAX_JOYSTICKOEMVXDNAME = 260

            <StructLayout(LayoutKind.Sequential)>
            Public Structure JOYCAPS
                Public wMid As Short
                Public wPid As Short
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)> Public szPname As String
                Public wXmin As Int32
                Public wXmax As Int32
                Public wYmin As Int32
                Public wYmax As Int32
                Public wZmin As Int32
                Public wZmax As Int32
                Public wNumButtons As Int32
                Public wPeriodMin As Int32
                Public wPeriodMax As Int32
                Public wRmin As Int32
                Public wRmax As Int32
                Public wUmin As Int32
                Public wUmax As Int32
                Public wVmin As Int32
                Public wVmax As Int32
                Public wCaps As JOYCAPFLAGS
                Public wMaxAxes As Int32
                Public wNumAxes As Int32
                Public wMaxButtons As Int32
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAXPNAMELEN)> Public szRegKey As String
                <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MAX_JOYSTICKOEMVXDNAME)> Public szOEMVxD As String
            End Structure

            Public Class JoyCapsEX
                Dim _base As JOYCAPS

                Friend Sub New(j As JOYCAPS, jID As Integer, OEMName As String, GUID As Guid)
                    _base = j
                    _Name = OEMName
                    _GUID = GUID
                    _JoyID = jID
                    Dim r As New List(Of Range)
                    With _base
                        If .wNumAxes > 0 Then
                            If .wNumAxes >= 1 Then
                                r.Add(New Range(.wXmin, .wXmax))
                            End If
                            If .wNumAxes >= 2 Then
                                r.Add(New Range(.wYmin, .wYmax))
                            End If
                            If HasAxis(JOYCAPFLAGS.JOYCAPS_HASZ) Then
                                r.Add(New Range(.wZmin, .wZmax))
                            End If
                            If HasAxis(JOYCAPFLAGS.JOYCAPS_HASR) Then
                                r.Add(New Range(.wRmin, .wRmax))
                            End If
                            If HasAxis(JOYCAPFLAGS.JOYCAPS_HASU) Then
                                r.Add(New Range(.wUmin, .wUmax))
                            End If
                            If HasAxis(JOYCAPFLAGS.JOYCAPS_HASV) Then
                                r.Add(New Range(.wVmin, .wVmax))
                            End If
                        End If
                        _AxesRange = r.ToArray
                    End With
                End Sub

                Public ReadOnly Property Name As String
                Public ReadOnly Property JoyID As Integer
                <Obsolete>
                Public ReadOnly Property ProductName As String
                    Get
                        Return _base.szPname
                    End Get
                End Property
                Public ReadOnly Property ManufacturerID As Short
                    Get
                        Return _base.wMid
                    End Get
                End Property
                Public ReadOnly Property ProductID As Short
                    Get
                        Return _base.wPid
                    End Get
                End Property
                Public ReadOnly Property AxesRange As Range()
                Public ReadOnly Property GUID As Guid
                Public ReadOnly Property PeriodRange As Range
                    Get
                        Return New Range(_base.wPeriodMin, _base.wPeriodMax)
                    End Get
                End Property

                Public ReadOnly Property HasAxis(i As JOYCAPFLAGS) As Boolean
                    Get
                        Return (_base.wCaps And i) = i
                    End Get
                End Property


            End Class

            <Flags>
            Public Enum JOYCAPFLAGS As Int32
                JOYCAPS_HASZ = 1    'Joystick has z-coordinate information.
                JOYCAPS_HASR = 2   'Joystick has rudder (fourth axis) information.
                JOYCAPS_HASU = 4 'Joystick has u-coordinate (fifth axis) information.
                JOYCAPS_HASV = 8  'Joystick has v-coordinate (sixth axis) information.
                JOYCAPS_HASPOV = 16 'Joystick has point-Of-view information.
                JOYCAPS_POV4DIR = 32 'Joystick point-Of-view supports discrete values (centered, forward, backward, left, And right).
                JOYCAPS_POVCTS = 64 'Joystick point-Of-view supports continuous degree bearings.
            End Enum

            <StructLayout(LayoutKind.Sequential)>
            Public Structure JOYINFOEX
                Implements IEquatable(Of JOYINFOEX)

                Public dwSize As Integer
                Public dwFlags As Integer
                Public dwXpos As Integer
                Public dwYpos As Integer
                Public dwZpos As Integer
                Public dwRpos As Integer
                Public dwUpos As Integer
                Public dwVpos As Integer
                Public dwButtons As JoyButtons
                Public dwButtonNumber As Integer
                Public dwPOV As JoyPOVDirections
                Public dwReserved1 As Int64
                'Public dwReserved2 As Integer

                Public Shared Operator =(a As JOYINFOEX, b As JOYINFOEX) As Boolean
                    Return a.Equals(b)
                End Operator
                Public Shared Operator <>(a As JOYINFOEX, b As JOYINFOEX) As Boolean
                    Return Not a.Equals(b)
                End Operator

                Public Overloads Function Equals(other As JOYINFOEX) As Boolean Implements IEquatable(Of JOYINFOEX).Equals
                    With other
                        Return .dwButtonNumber = dwButtonNumber AndAlso .dwButtons = dwButtons AndAlso .dwPOV = dwPOV AndAlso .dwRpos = dwRpos AndAlso .dwUpos = dwUpos AndAlso .dwVpos = dwVpos AndAlso .dwXpos = dwXpos AndAlso .dwYpos = dwYpos AndAlso .dwZpos = dwZpos
                    End With
                End Function

                Public ReadOnly Property IsButtonPressed(b As JoyButtons) As Boolean
                    Get
                        Return (dwButtons And b) = b
                    End Get
                End Property
                Public ReadOnly Property IsAxisMin(index As Integer, devicecaps As JOYCAPS) As Boolean
                    Get
                        Dim x As Integer = AxisValue(index)
                        Dim min As Integer = 0
                        Select Case index
                            Case 0 : min = devicecaps.wXmin
                            Case 1 : min = devicecaps.wYmin
                            Case 2 : min = devicecaps.wZmin
                            Case 3 : min = devicecaps.wRmin
                            Case 4 : min = devicecaps.wUmin
                            Case 5 : min = devicecaps.wVmin
                        End Select

                        Dim max As Integer = 0
                        Select Case index
                            Case 0 : max = devicecaps.wXmax
                            Case 1 : max = devicecaps.wYmax
                            Case 2 : max = devicecaps.wZmax
                            Case 3 : max = devicecaps.wRmax
                            Case 4 : max = devicecaps.wUmax
                            Case 5 : max = devicecaps.wVmax
                        End Select
                        Return (Math.Round(CSng((x - min) / CSng(max - min) * 2.0F - 1.0F)) < 0F)
                    End Get
                End Property
                Public ReadOnly Property IsAxisMax(index As Integer, devicecaps As JOYCAPS) As Boolean
                    Get
                        Dim x As Integer = AxisValue(index)
                        Dim min As Integer = 0
                        Select Case index
                            Case 0 : min = devicecaps.wXmin
                            Case 1 : min = devicecaps.wYmin
                            Case 2 : min = devicecaps.wZmin
                            Case 3 : min = devicecaps.wRmin
                            Case 4 : min = devicecaps.wUmin
                            Case 5 : min = devicecaps.wVmin
                        End Select

                        Dim max As Integer = 0
                        Select Case index
                            Case 0 : max = devicecaps.wXmax
                            Case 1 : max = devicecaps.wYmax
                            Case 2 : max = devicecaps.wZmax
                            Case 3 : max = devicecaps.wRmax
                            Case 4 : max = devicecaps.wUmax
                            Case 5 : max = devicecaps.wVmax
                        End Select
                        Return (Math.Round(CSng((x - min) / CSng(max - min) * 2.0F - 1.0F)) > 0F)
                    End Get
                End Property

                'Public ReadOnly Property IsLeft(deviceCaps As NativeMethods.JOYCAPS) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwXpos - deviceCaps.wXmin) / CSng(deviceCaps.wXmax - deviceCaps.wXmin) * 2.0F - 1.0F)) < 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsLeft(deviceCaps As JoyCapsEX) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwXpos - deviceCaps.AxesRange(0).Start.Value) / CSng(deviceCaps.AxesRange(0).End.Value - deviceCaps.AxesRange(0).Start.Value) * 2.0F - 1.0F)) < 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsRight(deviceCaps As NativeMethods.JOYCAPS) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwXpos - deviceCaps.wXmin) / CSng(deviceCaps.wXmax - deviceCaps.wXmin) * 2.0F - 1.0F)) > 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsRight(deviceCaps As JoyCapsEX) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwXpos - deviceCaps.AxesRange(0).Start.Value) / CSng(deviceCaps.AxesRange(0).End.Value - deviceCaps.AxesRange(0).Start.Value) * 2.0F - 1.0F)) > 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsUp(deviceCaps As NativeMethods.JOYCAPS) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwYpos - deviceCaps.wYmin) / CSng(deviceCaps.wYmax - deviceCaps.wYmin) * 2.0F - 1.0F)) < 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsUp(deviceCaps As JoyCapsEX) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwYpos - deviceCaps.AxesRange(1).Start.Value) / CSng(deviceCaps.AxesRange(1).End.Value - deviceCaps.AxesRange(1).Start.Value) * 2.0F - 1.0F)) < 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsDown(deviceCaps As NativeMethods.JOYCAPS) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwYpos - deviceCaps.wYmin) / CSng(deviceCaps.wYmax - deviceCaps.wYmin) * 2.0F - 1.0F)) > 0F)
                '    End Get
                'End Property
                'Public ReadOnly Property IsDown(deviceCaps As JoyCapsEX) As Boolean
                '    Get
                '        Return (Math.Round(CSng((dwYpos - deviceCaps.AxesRange(1).Start.Value) / CSng(deviceCaps.AxesRange(1).End.Value - deviceCaps.AxesRange(1).Start.Value) * 2.0F - 1.0F)) > 0F)
                '    End Get
                'End Property
                Public ReadOnly Property AxisValue(index As Integer) As Integer
                    Get
                        Select Case index
                            Case 0
                                Return dwXpos
                            Case 1
                                Return dwYpos
                            Case 2
                                Return dwZpos
                            Case 3
                                Return dwRpos
                            Case 4
                                Return dwUpos
                            Case 5
                                Return dwVpos
                            Case Else
                                Return -1
                        End Select
                    End Get
                End Property
                Friend Sub StampIt()
                    dwReserved1 = GetTickCount64() 'Now.ToBinary
                End Sub

            End Structure


            Public Const JOYSTICKID1 = 0
            Public Const JOYSTICKID2 = 1
            Public Const JOYERR_NOERROR = (0)
            Public Const JOY_RETURNX = &H1L
            Public Const JOY_RETURNY = &H2L
            Public Const JOY_RETURNZ = &H4L
            Public Const JOY_RETURNR = &H8L
            Public Const JOY_RETURNU = &H10L
            Public Const JOY_RETURNV = &H20L
            Public Const JOY_RETURNPOV = &H40L
            Public Const JOY_RETURNBUTTONS = &H80L
            Public Const JOY_RETURNCENTERED = &H400L
            Public Const JOY_RETURNALL = (JOY_RETURNX Or JOY_RETURNY Or JOY_RETURNZ Or JOY_RETURNR Or JOY_RETURNU Or JOY_RETURNV Or JOY_RETURNPOV Or JOY_RETURNBUTTONS)
            Public Const JOYERR_BASE = 160
            Public Const JOYERR_PARMS = (JOYERR_BASE + 5)
            Public Const JOYERR_NOCANDO = (JOYERR_BASE + 6)
            Public Const JOYERR_UNPLUGGED = (JOYERR_BASE + 7)

            Public Enum JoyPOVDirections As Integer
                JOY_POVCENTERED = 65535
                JOY_POVFORWARD = 0
                JOY_POVFORWARD_AND_RIGHT = 4500
                JOY_POVRIGHT = 9000
                JOY_POVRIGHT_AND_BACKWARD = 13500
                JOY_POVBACKWARD = 18000
                JOY_POVBACKWARD_AND_LEFT = 22500
                JOY_POVLEFT = 27000
                JOY_POVLEFT_AND_FORWARD = 31500
            End Enum
            <Flags>
            Public Enum JoyButtons
                JOY_BUTTON1 = 1
                JOY_BUTTON2 = 2
                JOY_BUTTON3 = 4
                JOY_BUTTON4 = 8
                JOY_BUTTON1CHG = 256
                JOY_BUTTON2CHG = 512
                JOY_BUTTON3CHG = 1024
                JOY_BUTTON4CHG = 2048
                JOY_BUTTON5 = 16
                JOY_BUTTON6 = 32
                JOY_BUTTON7 = 64
                JOY_BUTTON8 = 128
                JOY_BUTTON9 = 256
                JOY_BUTTON10 = 512
                JOY_BUTTON11 = 1024
                JOY_BUTTON12 = 2048
                JOY_BUTTON13 = 4096
                JOY_BUTTON14 = 8192
                JOY_BUTTON15 = 16384
                JOY_BUTTON16 = 32768
                JOY_BUTTON17 = 65536
                JOY_BUTTON18 = &H20000
                JOY_BUTTON19 = &H40000
                JOY_BUTTON20 = &H80000
                JOY_BUTTON21 = &H100000
                JOY_BUTTON22 = &H200000
                JOY_BUTTON23 = &H400000
                JOY_BUTTON24 = &H800000
                JOY_BUTTON25 = &H1000000
                JOY_BUTTON26 = &H2000000
                JOY_BUTTON27 = &H4000000
                JOY_BUTTON28 = &H8000000
                JOY_BUTTON29 = &H10000000
                JOY_BUTTON30 = &H20000000
                JOY_BUTTON31 = &H40000000
                JOY_BUTTON32 = &H80000000
            End Enum

            Public Enum MMSYSERR As Integer
                MMSYSERR_NOERROR = 0
                MMSYSERR_BASE = 0
                MMSYSERR_ERROR = (MMSYSERR_BASE + 1)
                MMSYSERR_BADDEVICEID = (MMSYSERR_BASE + 2)
                MMSYSERR_NOTENABLED = (MMSYSERR_BASE + 3)
                MMSYSERR_ALLOCATED = (MMSYSERR_BASE + 4)
                MMSYSERR_INVALHANDLE = (MMSYSERR_BASE + 5)
                MMSYSERR_NODRIVER = (MMSYSERR_BASE + 6)
                MMSYSERR_NOMEM = (MMSYSERR_BASE + 7)
                MMSYSERR_NOTSUPPORTED = (MMSYSERR_BASE + 8)
                MMSYSERR_BADERRNUM = (MMSYSERR_BASE + 9)
                MMSYSERR_INVALFLAG = (MMSYSERR_BASE + 10)
                MMSYSERR_INVALPARAM = (MMSYSERR_BASE + 11)
                MMSYSERR_HANDLEBUSY = (MMSYSERR_BASE + 12)
                MMSYSERR_INVALIDALIAS = (MMSYSERR_BASE + 13)
                MMSYSERR_BADDB = (MMSYSERR_BASE + 14)
                MMSYSERR_KEYNOTFOUND = (MMSYSERR_BASE + 15)
                MMSYSERR_READERROR = (MMSYSERR_BASE + 16)
                MMSYSERR_WRITEERROR = (MMSYSERR_BASE + 17)
                MMSYSERR_DELETEERROR = (MMSYSERR_BASE + 18)
                MMSYSERR_VALNOTFOUND = (MMSYSERR_BASE + 19)
                MMSYSERR_NODRIVERCB = (MMSYSERR_BASE + 20)
                MMSYSERR_LASTERROR = (MMSYSERR_BASE + 20)
                JOYERR_UNPLUGGED = (JOYERR_BASE + 7)
                JOYERR_PARMS = (JOYERR_BASE + 5)
            End Enum
            ' This is a "Stub" function - it has no code in its body.
            ' There is a similarly named function inside a dll that comes with windows called
            ' winmm.dll.
            ' The .Net framework will route calls to this function, through to the dll file.
            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joySetCapture", SetLastError:=True)>
            Public Shared Function joySetCapture(ByVal hwnd As IntPtr, ByVal uJoyID As Integer, ByVal uPeriod As Integer, ByVal changed As Integer) As MMSYSERR
            End Function
            ' This is a "Stub" function - it has no code in its body.
            ' There is a similarly named function inside a dll that comes with windows called
            ' winmm.dll.
            ' The .Net framework will route calls to this function, through to the dll file.
            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joyGetNumDevs", SetLastError:=True)>
            Public Shared Function joyGetNumDevs() As MMSYSERR
            End Function
            ' This is a "Stub" function - it has no code in its body.
            ' There is a similarly named function inside a dll that comes with windows called
            ' winmm.dll.
            ' The .Net framework will route calls to this function, through to the dll file.
            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joyGetPosEx", SetLastError:=True)>
            Public Shared Function joyGetPosEx(uJoyId As Integer, ByRef pji As JOYINFOEX) As MMSYSERR
            End Function
            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joyReleaseCapture", SetLastError:=True)>
            Public Shared Function joyReleaseCapture(uJoyId As Integer) As MMSYSERR
            End Function

            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joyGetDevCapsA", SetLastError:=True, CharSet:=CharSet.Ansi)>
            Public Shared Function joyGetDevCaps(uJoyId As Integer, ByRef pjc As JOYCAPS, cbjc As Integer) As MMSYSERR
            End Function

            <DllImport("winmm", CallingConvention:=CallingConvention.Winapi, EntryPoint:="joyConfigChanged", SetLastError:=True)>
            Public Shared Function joyConfigChanged(dwFlags As Integer) As MMSYSERR
            End Function

        End Class

        Public Class JoyInfoWithTimestamp
            Private _base As NativeMethods.JOYINFOEX
            Private _stampButtons() As Long
            Private _stampAxes() As Long
            Private _stampPOV As Long
            Private _counterAxes() As Integer
            Private _counterButtons() As Integer
            Private _counterPOV As Integer
            Public Sub New(jDevCaps As NativeMethods.JOYCAPS)
                ReDim _stampButtons(jDevCaps.wNumButtons - 1)
                ReDim _counterButtons(jDevCaps.wNumButtons - 1)
                ReDim _stampAxes(jDevCaps.wNumAxes - 1)
                ReDim _counterAxes(jDevCaps.wNumAxes - 1)
            End Sub
            Public Property ButtonTimestamp(i As Integer) As Long
                Get
                    Return _stampButtons(i)
                End Get
                Set(value As Long)
                    _stampButtons(i) = value
                End Set
            End Property
            Public Property AxesTimestamp(i As Integer) As Long
                Get
                    Return _stampAxes(i)
                End Get
                Set(value As Long)
                    _stampAxes(i) = value
                End Set
            End Property
            Public Property ButtonCounter(i As Integer) As Integer
                Get
                    Return _counterButtons(i)
                End Get
                Set(value As Integer)
                    _counterButtons(1) = value
                End Set
            End Property
            Public Property AxesCounter(i As Integer) As Integer
                Get
                    Return _counterAxes(i)
                End Get
                Set(value As Integer)
                    _counterAxes(i) = value
                End Set
            End Property
            Public Property POVTimestamp As Long
                Get
                    Return _stampPOV
                End Get
                Set(value As Long)
                    _stampPOV = value
                End Set
            End Property
            Public Property POVCounter As Long
                Get
                    Return _counterPOV
                End Get
                Set(value As Long)
                    _counterPOV = value
                End Set
            End Property


        End Class
        Public joyHistory As JoyInfoWithTimestamp


        Public Sub New(joyId As Integer)
            'AddHandler parent.HandleCreated, AddressOf Me.OnHandleCreated
            'AddHandler parent.HandleDestroyed, AddressOf Me.OnHandleDestroyed
            'AssignHandle(parent.Handle)
            'Me.parent = parent
            'Dim result = NativeMethods.JoySetCapture(Me.Handle, joyId, 50, True)

            Dim JOYINFO As New NativeMethods.JOYINFOEX
            JOYINFO.dwSize = Marshal.SizeOf(GetType(NativeMethods.JOYINFOEX))
            'MaxDevices = NativeMethods.joyGetNumDevs()
            Me.joyIndex = joyId

            Dim ret = NativeMethods.joyGetPosEx(joyId, JOYINFO)
            If ret <> NativeMethods.MMSYSERR.MMSYSERR_NOERROR Then
                Throw New NativeMethods.JoystickNotFoundException(ret)
                Exit Sub
            End If

            DeviceCaps = New NativeMethods.JOYCAPS

            ret = NativeMethods.joyGetDevCaps(joyId, DeviceCaps, Marshal.SizeOf(GetType(NativeMethods.JOYCAPS)))
            If ret <> NativeMethods.MMSYSERR.MMSYSERR_NOERROR Then
                Throw New NativeMethods.JoystickNoDevCapsException(ret)
                Exit Sub
            End If

            joyHistory = New JoyInfoWithTimestamp(DeviceCaps)
        End Sub


        Public NotInheritable Class JoyManager
            Public Shared ReadOnly Property MaxJoyCount
            Shared Sub New()
                MaxJoyCount = NativeMethods.joyGetNumDevs()
            End Sub

            Public Shared Function EnumerateDevices() As Dictionary(Of Integer, NativeMethods.JoyCapsEX)
                Dim ret As New Dictionary(Of Integer, NativeMethods.JoyCapsEX)



                For t As Integer = 0 To MaxJoyCount
                    Dim j = New NativeMethods.JOYCAPS
                    Dim OEMName As String = ""
                    Dim JoyGUID As Guid = Guid.Empty


                    If NativeMethods.joyGetDevCaps(t, j, Marshal.SizeOf(GetType(NativeMethods.JOYCAPS))) = NativeMethods.MMSYSERR.MMSYSERR_NOERROR Then
                        Dim reg As Microsoft.Win32.RegistryKey = Nothing
                        Try
                            reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaResources\Joystick\" & j.szRegKey & "\CurrentJoyStickSettings")
                            Dim joyRegName As String = reg.GetValue("Joystick" & t + 1 & "OEMName")
                            reg.Dispose()
                            reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\Joystick\OEM\" & joyRegName)
                            OEMName = reg.GetValue("OEMName")


                            Dim subkeynames = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\" & joyRegName & "\Calibration").GetSubKeyNames
                            If UBound(subkeynames) = 0 Then
                                reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\" & joyRegName & "\Calibration\0")
                            Else
                                reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\" & joyRegName & "\Calibration\" & t)

                            End If
                            'Dim z = reg.GetValue("GUID")
                            JoyGUID = New Guid(CType(reg.GetValue("GUID"), Byte()))
                        Catch
                        Finally
                            If reg IsNot Nothing Then reg.Dispose()
                        End Try

                        ret.Add(t, New NativeMethods.JoyCapsEX(j, t, OEMName, JoyGUID))
                    End If
                Next
                Return ret
            End Function
        End Class
        Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer, ByRef lpvParam As Integer, ByVal fuWinIni As Integer) As Integer

        Private Shared KeyboardDelay As Integer, KeyboardRepeatSpeed As Integer
        Shared Sub New()
            SystemParametersInfo(22, 0, KeyboardDelay, 0)
            Select Case KeyboardDelay
                Case 0
                    KeyboardDelay = 250
                Case 1
                    KeyboardDelay = 500
                Case 2
                    KeyboardDelay = 750
                Case 3
                    KeyboardDelay = 1000
            End Select
            SystemParametersInfo(10, 0, KeyboardRepeatSpeed, 0)
            Select Case KeyboardRepeatSpeed
                Case 0 : KeyboardRepeatSpeed = 400
                Case 1 : KeyboardRepeatSpeed = 363
                Case 2 : KeyboardRepeatSpeed = 352
                Case 3 : KeyboardRepeatSpeed = 341
                Case 4 : KeyboardRepeatSpeed = 330
                Case 5 : KeyboardRepeatSpeed = 319
                Case 6 : KeyboardRepeatSpeed = 308
                Case 7 : KeyboardRepeatSpeed = 297
                Case 8 : KeyboardRepeatSpeed = 286
                Case 9 : KeyboardRepeatSpeed = 275
                Case 10 : KeyboardRepeatSpeed = 264
                Case 11 : KeyboardRepeatSpeed = 253
                Case 12 : KeyboardRepeatSpeed = 242
                Case 13 : KeyboardRepeatSpeed = 231
                Case 14 : KeyboardRepeatSpeed = 220
                Case 15 : KeyboardRepeatSpeed = 209
                Case 16 : KeyboardRepeatSpeed = 198
                Case 17 : KeyboardRepeatSpeed = 187
                Case 18 : KeyboardRepeatSpeed = 176
                Case 19 : KeyboardRepeatSpeed = 165
                Case 20 : KeyboardRepeatSpeed = 154
                Case 21 : KeyboardRepeatSpeed = 143
                Case 22 : KeyboardRepeatSpeed = 114
                Case 23 : KeyboardRepeatSpeed = 105
                Case 24 : KeyboardRepeatSpeed = 96
                Case 25 : KeyboardRepeatSpeed = 88
                Case 26 : KeyboardRepeatSpeed = 75
                Case 27 : KeyboardRepeatSpeed = 69
                Case 28 : KeyboardRepeatSpeed = 60
                Case 29 : KeyboardRepeatSpeed = 51
                Case 30 : KeyboardRepeatSpeed = 42
                Case 31 : KeyboardRepeatSpeed = 33

            End Select
        End Sub
        Public Property RawDataOnly As Boolean = False
        Public Event JoyStickAxisDown As EventHandler(Of JoyStickAxisEventArgs)
        Public Event JoyStickAxisUp As EventHandler(Of JoyStickAxisEventArgs)
        Public Event JoyStickAxisPress As EventHandler(Of JoyStickAxisPressEventArgs)
        Public Event JoyStickButtonDown As EventHandler(Of JoyStickButtonEventArgs)
        Public Event JoyStickButtonUp As EventHandler(Of JoyStickButtonEventArgs)
        Public Event JoyStickButtonPress As EventHandler(Of JoyStickButtonPressEventArgs)
        Public Event JoyStickPOVDown As EventHandler(Of JoyStickButtonEventArgs)
        Public Event JoyStickPOVUp As EventHandler(Of JoyStickButtonEventArgs)
        Public Event JoyStickPOVPress As EventHandler(Of JoyStickButtonPressEventArgs)

        Public Sub PollEx(ByRef joyinfo As NativeMethods.JOYINFOEX)
            'Dim joyinfo As New NativeMethods.JOYINFOEX
            'joyinfo.dwSize = Marshal.SizeOf(GetType(NativeMethods.JOYINFOEX))
            'joyinfo.dwFlags = NativeMethods.JOY_RETURNALL

            'Dim ret = NativeMethods.joyGetPosEx(joyIndex, joyinfo)
            'If ret = NativeMethods.MMSYSERR.MMSYSERR_NOERROR Then
            joyinfo.StampIt()
            'If lastInfo <> joyinfo Then
            RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, lastInfo))
            ' End If

            Dim elapsed As Double

            For axis As Integer = 0 To DeviceCaps.wNumAxes - 1

                Dim isLeft = joyinfo.IsAxisMin(axis, DeviceCaps)
                Dim wasLeft = lastInfo.IsAxisMin(axis, DeviceCaps)
                For x As Integer = 0 To 1
                    If isLeft And Not wasLeft Then
                        joyHistory.AxesTimestamp(axis) = joyinfo.dwReserved1
                        Dim h = New JoyStickAxisEventArgs(joyinfo, lastInfo, axis, joyinfo.AxisValue(axis), joyHistory.AxesTimestamp(axis))
                        RaiseEvent JoyStickAxisDown(Me, h)
                        If h.Handled Then
                            joyHistory.AxesTimestamp(axis) = 0
                        End If
                    ElseIf Not isLeft AndAlso wasLeft Then
                        RaiseEvent JoyStickAxisUp(Me, New JoyStickAxisEventArgs(joyinfo, lastInfo, axis, joyinfo.AxisValue(axis), joyHistory.AxesTimestamp(axis)))
                        joyHistory.AxesCounter(axis) = 0
                        joyHistory.AxesTimestamp(axis) = 0


                    ElseIf joyHistory.AxesTimestamp(axis) <> 0 AndAlso isLeft Then
                        elapsed = joyinfo.dwReserved1 - joyHistory.AxesTimestamp(axis)
                        If elapsed >= KeyboardDelay Then
                            If elapsed >= KeyboardRepeatSpeed * joyHistory.AxesCounter(axis) Then
                                Dim h = New JoyStickAxisPressEventArgs(joyinfo, lastInfo, axis, joyinfo.AxisValue(axis), elapsed, joyHistory.AxesCounter(axis))
                                RaiseEvent JoyStickAxisPress(Me, h)
                                joyHistory.AxesCounter(axis) += 1
                                If h.Handled Then
                                    joyHistory.AxesTimestamp(axis) = 0
                                    joyHistory.AxesCounter(axis) = 0
                                End If
                            End If
                        End If
                    End If

                    isLeft = joyinfo.IsAxisMax(axis, DeviceCaps)
                    wasLeft = lastInfo.IsAxisMax(axis, DeviceCaps)
                Next

            Next

            If (DeviceCaps.wCaps And NativeMethods.JOYCAPFLAGS.JOYCAPS_HASPOV) = NativeMethods.JOYCAPFLAGS.JOYCAPS_HASPOV Then
                If lastInfo.dwPOV <> joyinfo.dwPOV Then
                    'change occured
                    If joyinfo.dwPOV <> NativeMethods.JoyPOVDirections.JOY_POVCENTERED Then
                        joyHistory.POVTimestamp = joyinfo.dwReserved1
                        joyHistory.POVCounter = 0
                        Dim h = New JoyStickButtonEventArgs(joyinfo, lastInfo, NativeMethods.JOY_RETURNPOV, joyinfo.dwPOV, joyHistory.POVTimestamp)
                        RaiseEvent JoyStickPOVDown(Me, h)
                        If h.Handled Then
                            joyHistory.POVTimestamp = 0
                        End If
                    Else
                        RaiseEvent JoyStickPOVUp(Me, New JoyStickButtonEventArgs(joyinfo, lastInfo, NativeMethods.JOY_RETURNPOV, joyinfo.dwPOV, joyHistory.POVTimestamp))
                        joyHistory.POVTimestamp = 0
                        joyHistory.POVCounter = 0
                    End If
                ElseIf joyHistory.POVTimestamp <> 0 AndAlso lastInfo.dwPOV <> NativeMethods.JoyPOVDirections.JOY_POVCENTERED Then
                    'pressed
                    elapsed = joyinfo.dwReserved1 - joyHistory.POVTimestamp
                    If elapsed >= KeyboardDelay Then
                        If elapsed >= KeyboardRepeatSpeed * joyHistory.POVCounter Then
                            Dim h = New JoyStickButtonPressEventArgs(joyinfo, lastInfo, NativeMethods.JOY_RETURNPOV, joyinfo.dwPOV, elapsed, joyHistory.POVCounter)
                            RaiseEvent JoyStickPOVPress(Me, h)
                            joyHistory.POVCounter += 1
                            If h.Handled Then
                                joyHistory.POVTimestamp = 0
                                joyHistory.POVCounter = 0
                            End If
                        End If
                    End If
                End If
            End If

            For t As Integer = 0 To DeviceCaps.wNumButtons - 1
                Dim eB = [Enum].Parse(GetType(JoyApi.Joystick.NativeMethods.JoyButtons), "JOY_BUTTON" & t + 1)
                If joyinfo.IsButtonPressed(eB) AndAlso Not lastInfo.IsButtonPressed(eB) Then
                    joyHistory.ButtonTimestamp(t) = joyinfo.dwReserved1
                    Dim h = New JoyStickButtonEventArgs(joyinfo, lastInfo, t, joyinfo.IsButtonPressed(t), elapsed)
                    RaiseEvent JoyStickButtonDown(Me, h)
                    If h.Handled Then
                        joyHistory.ButtonTimestamp(t) = 0
                    End If
                ElseIf Not joyinfo.IsButtonPressed(eB) AndAlso lastInfo.IsButtonPressed(eB) Then
                    RaiseEvent JoyStickButtonUp(Me, New JoyStickButtonEventArgs(joyinfo, lastInfo, t, joyinfo.dwYpos, elapsed))

                    joyHistory.ButtonTimestamp(t) = 0
                    joyHistory.ButtonCounter(t) = 0
                ElseIf joyHistory.ButtonTimestamp(t) <> 0 AndAlso joyinfo.IsButtonPressed(eB) Then
                    elapsed = joyinfo.dwReserved1 - joyHistory.ButtonTimestamp(t)
                    If elapsed >= KeyboardDelay Then
                        If elapsed >= KeyboardRepeatSpeed * joyHistory.ButtonCounter(t) Then
                            Dim h = New JoyStickButtonPressEventArgs(joyinfo, lastInfo, t, joyinfo.IsButtonPressed(t), elapsed, joyHistory.ButtonCounter(t))
                            RaiseEvent JoyStickButtonPress(Me, h)
                            joyHistory.ButtonCounter(t) += 1
                            If h.Handled Then
                                joyHistory.ButtonTimestamp(t) = 0
                                joyHistory.ButtonCounter(t) = 0
                            End If
                        End If
                    End If
                End If
            Next



            ' lastInfo = joyinfo
            'End If

        End Sub



        Public Sub Poll()


            Dim joyinfo As New NativeMethods.JOYINFOEX
            joyinfo.dwSize = Marshal.SizeOf(GetType(NativeMethods.JOYINFOEX))
            joyinfo.dwFlags = NativeMethods.JOY_RETURNALL

            Dim ret = NativeMethods.joyGetPosEx(joyIndex, joyinfo)
            If ret = NativeMethods.MMSYSERR.MMSYSERR_NOERROR Then

                If RawDataOnly Then
                    'If lastInfo <> joyinfo Then
                    ' End If
                    joyinfo.StampIt()
                    RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, lastInfo))
                Else
                    PollEx(joyinfo)
                End If
                lastInfo = joyinfo
            End If
            'If joyinfo.dwXpos > 16384 AndAlso joyinfo.dwXpos < 49152 Then
            '    ' X is near the centre line.
            '    If joyinfo.dwYpos < 6000 Then
            '        ' Y is near the top.
            '        'RaiseEvent Up()
            '        RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, "Up"))
            '    ElseIf joyinfo.dwYpos > 59536 Then
            '        ' Y is near the bottom.
            '        'RaiseEvent Down()
            '        RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, "Down"))
            '    End If
            'Else
            '    If joyinfo.dwYpos > 16384 AndAlso joyinfo.dwYpos < 49152 Then
            '        ' Y is near the centre line
            '        If joyinfo.dwXpos < 6000 Then
            '            ' X is near the left.
            '            'RaiseEvent Left()
            '            RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, "Left"))
            '        ElseIf joyinfo.dwXpos > 59536 Then
            '            ' X is near the right
            '            'RaiseEvent Right()
            '            RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, "Right"))
            '        End If
            '    End If
            'End If
            'If joyinfo.dwButtons <> btnValue Then
            '    RaiseEvent JoystickChanged(Me, New JoyStickChangedArgs(joyinfo, "Button"))
            '    btnValue = joyinfo.dwButtons
            'End If

        End Sub



    End Class


End Class