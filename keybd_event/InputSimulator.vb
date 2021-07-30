Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.InteropServices

Namespace WindowsInput
    ''' <summary>
    ''' Provides a useful wrapper around the User32 SendInput And related native Windows functions.
    ''' </summary>
    Public NotInheritable Class InputSimulator
#Region "DllImports"

        ''' <summary>
        ''' The SendInput function synthesizes keystrokes, mouse motions, And button clicks.
        ''' </summary>
        ''' <param name="numberOfInputs">Number of structures in the Inputs array.</param>
        ''' <param name="inputs">Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard Or mouse input stream.</param>
        ''' <param name="sizeOfInputStructure">Specifies the size, in bytes, of an INPUT structure. If cbSize Is Not the size of an INPUT structure, the function fails.</param>
        ''' <returns>The function returns the number of events that it successfully inserted into the keyboard Or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.Microsoft Windows Vista. This function fails when it Is blocked by User Interface Privilege Isolation (UIPI). Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.</returns>
        ''' <remarks>
        ''' Microsoft Windows Vista. This function Is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal Or lesser integrity level.
        ''' The SendInput function inserts the events in the INPUT structures serially into the keyboard Or mouse input stream. These events are Not interspersed with other keyboard Or mouse input events inserted either by the user (with the keyboard Or mouse) Or by calls to keybd_event, mouse_event, Or other calls to SendInput.
        ''' This function does Not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
        ''' </remarks>
        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function SendInput(numberOfInputs As UInt32, inputs As INPUT(), sizeOfInputStructure As Int32) As UInt32
        End Function
        ''' <summary>
        ''' The GetAsyncKeyState function determines whether a key Is up Or down at the time the function Is called, And whether the key was pressed after a previous call to GetAsyncKeyState. (See: http:''msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        ''' </summary>
        ''' <param name="virtualKeyCode">Specifies one of 256 possible virtual-key codes. For more information, see Virtual Key Codes. Windows NT/2000/XP: You can use left- And right-distinguishing constants To specify certain keys. See the Remarks section For further information.</param>
        ''' <returns>
        ''' If the function succeeds, the return value specifies whether the key was pressed since the last call to GetAsyncKeyState, And whether the key Is currently up Or down. If the most significant bit Is set, the key Is down, And if the least significant bit Is set, the key was pressed after the previous call to GetAsyncKeyState. However, you should Not rely on this last behavior for more information, see the Remarks. 
        ''' 
        ''' Windows NT/2000/XP: The return value Is zero for the following cases: 
        ''' - The current desktop Is Not the active desktop
        ''' - The foreground thread belongs to another process And the desktop does Not allow the hook Or the journal record.
        ''' 
        ''' Windows 95/98/Me: The return value Is the global asynchronous key state for each virtual key. The system does Not check which thread has the keyboard focus.
        ''' 
        ''' Windows 95/98/Me: Windows 95 does Not support the left- And right-distinguishing constants. If you call GetAsyncKeyState with these constants, the return value Is zero. 
        ''' </returns>
        ''' <remarks>
        ''' The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, Not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it Is mapped to the left Or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        ''' Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        ''' 
        ''' Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState And receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value Is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) And should Not be relied upon.
        ''' 
        ''' You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, And VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, Or ALT keys without distinguishing between left And right. 
        ''' 
        ''' Windows NT/2000/XP: You can use the following virtual-key code constants As values For vKey To distinguish between the left And right instances Of those keys. 
        ''' 
        ''' Code Meaning 
        ''' VK_LSHIFT Left-shift key. 
        ''' VK_RSHIFT Right-shift key. 
        ''' VK_LCONTROL Left-control key. 
        ''' VK_RCONTROL Right-control key. 
        ''' VK_LMENU Left-menu key. 
        ''' VK_RMENU Right-menu key. 
        ''' 
        ''' These left- And right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, And MapVirtualKey functions. 
        ''' </remarks>
        <DllImport("user32.dll", SetLastError:=True)>
        Private Shared Function GetAsyncKeyState(virtualKeyCode As UInt16) As Int16
        End Function
        ''' <summary>
        ''' The GetKeyState function retrieves the status of the specified virtual key. The status specifies whether the key Is up, down, Or toggled (on, off alternating each time the key Is pressed). (See: http:''msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        ''' </summary>
        ''' <param name="virtualKeyCode">
        ''' Specifies a virtual key. If the desired virtual key Is a letter Or digit (A through Z, a through z, Or 0 through 9), nVirtKey must be set to the ASCII value of that character. For other keys, it must be a virtual-key code. 
        ''' If a non-English keyboard layout Is used, virtual keys with values in the range ASCII A through Z And 0 through 9 are used to specify most of the character keys. For example, for the German keyboard layout, the virtual key of value ASCII O (0x4F) refers to the "o" key, whereas VK_OEM_1 refers to the "o with umlaut" key.
        ''' </param>
        ''' <returns>
        ''' The return value specifies the status of the specified virtual key, as follows: 
        ''' If the high-order bit Is 1, the key Is down otherwise, it Is up.
        ''' If the low-order bit Is 1, the key Is toggled. A key, such as the CAPS LOCK key, Is toggled if it Is turned on. The key Is off And untoggled if the low-order bit Is 0. A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled, and off when the key is untoggled.
        ''' </returns>
        ''' <remarks>
        ''' The key status returned from this function changes as a thread reads key messages from its message queue. The status does Not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        ''' An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        ''' To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        ''' An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, And VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, Or ALT keys without distinguishing between left And right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left And right instances of those keys. 
        ''' VK_LSHIFT
        ''' VK_RSHIFT
        ''' VK_LCONTROL
        ''' VK_RCONTROL
        ''' VK_LMENU
        ''' VK_RMENU
        ''' 
        ''' These left- And right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, And MapVirtualKey functions. 
        ''' </remarks>
        <DllImport("user32.dll", SetLastError:=True)>
        Private Shared Function GetKeyState(virtualKeyCode As UInt16) As Int16
        End Function
        ''' <summary>
        ''' The GetMessageExtraInfo function retrieves the extra message information for the current thread. Extra message information Is an application- Or driver-defined value associated with the current thread's message queue. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>To set a thread's extra message information, use the SetMessageExtraInfo function. </remarks>
        <DllImport("user32.dll")>
        Private Shared Function GetMessageExtraInfo() As IntPtr
        End Function
#End Region

#Region "Methods"

        ''' <summary>
        ''' Determines whether a key Is up Or down at the time the function Is called by calling the GetAsyncKeyState function. (See: http:''msdn.microsoft.com/en-us/library/ms646293(VS.85).aspx)
        ''' </summary>
        ''' <param name="keyCode">The key code.</param>
        ''' <returns>
        ''' 	<c>true</c> if the key Is down otherwise, <c>false</c>.
        ''' </returns>
        ''' <remarks>
        ''' The GetAsyncKeyState function works with mouse buttons. However, it checks on the state of the physical mouse buttons, Not on the logical mouse buttons that the physical buttons are mapped to. For example, the call GetAsyncKeyState(VK_LBUTTON) always returns the state of the left physical mouse button, regardless of whether it Is mapped to the left Or right logical mouse button. You can determine the system's current mapping of physical mouse buttons to logical mouse buttons by calling 
        ''' Copy CodeGetSystemMetrics(SM_SWAPBUTTON) which returns TRUE if the mouse buttons have been swapped.
        ''' 
        ''' Although the least significant bit of the return value indicates whether the key has been pressed since the last query, due to the pre-emptive multitasking nature of Windows, another application can call GetAsyncKeyState And receive the "recently pressed" bit instead of your application. The behavior of the least significant bit of the return value Is retained strictly for compatibility with 16-bit Windows applications (which are non-preemptive) And should Not be relied upon.
        ''' 
        ''' You can use the virtual-key code constants VK_SHIFT, VK_CONTROL, And VK_MENU as values for the vKey parameter. This gives the state of the SHIFT, CTRL, Or ALT keys without distinguishing between left And right. 
        ''' 
        ''' Windows NT/2000/XP: You can use the following virtual-key code constants As values For vKey To distinguish between the left And right instances Of those keys. 
        ''' 
        ''' Code Meaning 
        ''' VK_LSHIFT Left-shift key. 
        ''' VK_RSHIFT Right-shift key. 
        ''' VK_LCONTROL Left-control key. 
        ''' VK_RCONTROL Right-control key. 
        ''' VK_LMENU Left-menu key. 
        ''' VK_RMENU Right-menu key. 
        ''' 
        ''' These left- And right-distinguishing constants are only available when you call the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, And MapVirtualKey functions. 
        ''' </remarks>
        Public Shared Function IsKeyDownAsync(keyCode As VirtualKeyCode) As Boolean
            Dim result As Int16 = GetAsyncKeyState(CType(keyCode, UInt16))
            Return (result < 0)
        End Function

        ''' <summary>
        ''' Determines whether the specified key Is up Or down by calling the GetKeyState function. (See: http:''msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        ''' </summary>
        ''' <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        ''' <returns>
        ''' 	<c>true</c> if the key Is down otherwise, <c>false</c>.
        ''' </returns>
        ''' <remarks>
        ''' The key status returned from this function changes as a thread reads key messages from its message queue. The status does Not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        ''' An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        ''' To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        ''' An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, And VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, Or ALT keys without distinguishing between left And right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left And right instances of those keys. 
        ''' VK_LSHIFT
        ''' VK_RSHIFT
        ''' VK_LCONTROL
        ''' VK_RCONTROL
        ''' VK_LMENU
        ''' VK_RMENU
        ''' 
        ''' These left- And right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, And MapVirtualKey functions. 
        ''' </remarks>
        Public Shared Function IsKeyDown(keyCode As VirtualKeyCode) As Boolean
            Dim result As Int16 = GetKeyState(CType(keyCode, UInt16))
            Return (result < 0)
        End Function

        ''' <summary>
        ''' Determines whether the toggling key Is toggled on (in-effect) Or Not by calling the GetKeyState function.  (See: http:''msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx)
        ''' </summary>
        ''' <param name="keyCode">The <see cref="VirtualKeyCode"/> for the key.</param>
        ''' <returns>
        ''' 	<c>true</c> if the toggling key Is toggled on (in-effect) otherwise, <c>false</c>.
        ''' </returns>
        ''' <remarks>
        ''' The key status returned from this function changes as a thread reads key messages from its message queue. The status does Not reflect the interrupt-level state associated with the hardware. Use the GetAsyncKeyState function to retrieve that information. 
        ''' An application calls GetKeyState in response to a keyboard-input message. This function retrieves the state of the key when the input message was generated. 
        ''' To retrieve state information for all the virtual keys, use the GetKeyboardState function. 
        ''' An application can use the virtual-key code constants VK_SHIFT, VK_CONTROL, And VK_MENU as values for the nVirtKey parameter. This gives the status of the SHIFT, CTRL, Or ALT keys without distinguishing between left And right. An application can also use the following virtual-key code constants as values for nVirtKey to distinguish between the left And right instances of those keys. 
        ''' VK_LSHIFT
        ''' VK_RSHIFT
        ''' VK_LCONTROL
        ''' VK_RCONTROL
        ''' VK_LMENU
        ''' VK_RMENU
        ''' 
        ''' These left- And right-distinguishing constants are available to an application only through the GetKeyboardState, SetKeyboardState, GetAsyncKeyState, GetKeyState, And MapVirtualKey functions. 
        ''' </remarks>
        Public Shared Function IsTogglingKeyInEffect(keyCode As VirtualKeyCode) As Boolean
            Dim result As Int16 = GetKeyState(CType(keyCode, UInt16))
            Return (result And &H1) = &H1
        End Function

        ''' <summary>
        ''' Calls the Win32 SendInput method to simulate a Key DOWN.
        ''' </summary>
        ''' <param name="keyCode">The VirtualKeyCode to press</param>
        Public Shared Sub SimulateKeyDown(keyCode As VirtualKeyCode)
            Dim down As New INPUT
            down.Type = CType(InputType.KEYBOARD, UInt32)
            down.Data.Keyboard = New KEYBDINPUT()
            down.Data.Keyboard.Vk = CType(keyCode, UInt16)
            down.Data.Keyboard.Scan = 0
            down.Data.Keyboard.Flags = 0
            down.Data.Keyboard.Time = 0
            down.Data.Keyboard.ExtraInfo = IntPtr.Zero

            Dim inputList(0) As Input
            inputList(0) = down

            Dim numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(GetType(INPUT)))
            If (numberOfSuccessfulSimulatedInputs = 0) Then Throw New Exception(String.Format("The key down simulation for {0} was not successful.", keyCode))
        End Sub

        ''' <summary>
        ''' Calls the Win32 SendInput method to simulate a Key UP.
        ''' </summary>
        ''' <param name="keyCode">The VirtualKeyCode to lift up</param>
        Public Shared Sub SimulateKeyUp(keyCode As VirtualKeyCode)
            Dim up As New INPUT()
            up.Type = CType(InputType.KEYBOARD, UInt32)
            up.Data.Keyboard = New KEYBDINPUT()
            up.Data.Keyboard.Vk = CType(keyCode, UInt16)
            up.Data.Keyboard.Scan = 0
            up.Data.Keyboard.Flags = CType(KeyboardFlag.KEYUP, UInt32)
            up.Data.Keyboard.Time = 0
            up.Data.Keyboard.ExtraInfo = IntPtr.Zero

            Dim inputList(0) As INPUT
            inputList(0) = up

            Dim numberOfSuccessfulSimulatedInputs = SendInput(1, inputList, Marshal.SizeOf(GetType(INPUT)))
            If (numberOfSuccessfulSimulatedInputs = 0) Then Throw New Exception(String.Format("The key up simulation for {0} was not successful.", keyCode))
        End Sub

        ''' <summary>
        ''' Calls the Win32 SendInput method with a KeyDown And KeyUp message in the same input sequence in order to simulate a Key PRESS.
        ''' </summary>
        ''' <param name="keyCode">The VirtualKeyCode to press</param>
        Public Shared Sub SimulateKeyPress(keyCode As VirtualKeyCode)
            Dim down As New INPUT
            down.Type = CType(InputType.KEYBOARD, UInt32)
            down.Data.Keyboard = New KEYBDINPUT()
            down.Data.Keyboard.Vk = CType(keyCode, UInt16)
            down.Data.Keyboard.Scan = 0
            down.Data.Keyboard.Flags = 0
            down.Data.Keyboard.Time = 0
            down.Data.Keyboard.ExtraInfo = IntPtr.Zero

            Dim up As New INPUT
            up.Type = CType(InputType.KEYBOARD, UInt32)
            up.Data.Keyboard = New KEYBDINPUT()
            up.Data.Keyboard.Vk = CType(keyCode, UInt16)
            up.Data.Keyboard.Scan = 0
            up.Data.Keyboard.Flags = CType(KeyboardFlag.KEYUP, UInt32)
            up.Data.Keyboard.Time = 0
            up.Data.Keyboard.ExtraInfo = IntPtr.Zero

            Dim inputList(1) As INPUT
            inputList(0) = down
            inputList(1) = up

            Dim numberOfSuccessfulSimulatedInputs = SendInput(2, inputList, Marshal.SizeOf(GetType(INPUT)))
            If (numberOfSuccessfulSimulatedInputs = 0) Then Throw New Exception(String.Format("The key press simulation for {0} was not successful.", keyCode))
        End Sub

        ''' <summary>
        ''' Calls the Win32 SendInput method with a stream of KeyDown And KeyUp messages in order to simulate uninterrupted text entry via the keyboard.
        ''' </summary>
        ''' <param name="text">The text to be simulated.</param>
        Public Shared Sub SimulateTextEntry(text As String)
            If (Text.Length > UInt32.MaxValue / 2) Then Throw New ArgumentException(String.Format("The text parameter is too long. It must be less than {0} characters.", UInt32.MaxValue / 2), "text")

            Dim chars = UTF8Encoding.ASCII.GetBytes(text)
            Dim Len = chars.Length
            Dim inputList(Len * 2) As INPUT
            For x As Integer = 0 To Len

                Dim scanCode As UInt16 = chars(x)

                Dim down As New INPUT
                down.Type = CType(InputType.KEYBOARD, UInt32)
                down.Data.Keyboard = New KEYBDINPUT()
                down.Data.Keyboard.Vk = 0
                down.Data.Keyboard.Scan = scanCode
                down.Data.Keyboard.Flags = CType(KeyboardFlag.UNICODE, UInt32)
                down.Data.Keyboard.Time = 0
                down.Data.Keyboard.ExtraInfo = IntPtr.Zero

                Dim up As New INPUT()
                up.Type = CType(InputType.KEYBOARD, UInt32)
                up.Data.Keyboard = New KEYBDINPUT()
                up.Data.Keyboard.Vk = 0
                up.Data.Keyboard.Scan = scanCode
                up.Data.Keyboard.Flags = CType((KeyboardFlag.KEYUP Or KeyboardFlag.UNICODE), UInt32)
                up.Data.Keyboard.Time = 0
                up.Data.Keyboard.ExtraInfo = IntPtr.Zero

                '' Handle extended keys
                '' If the scan code Is preceded by a prefix byte that has the value 0xE0 (224),
                '' we need to include the KEYEVENTF_EXTENDEDKEY flag in the Flags property. 
                If ((scanCode And &HFF00) = &HE000) Then
                    down.Data.Keyboard.Flags = down.data.keyboard.flags Or CType(KeyboardFlag.EXTENDEDKEY, UInt32)
                    up.Data.Keyboard.Flags = up.data.keyboard.flags Or CType(KeyboardFlag.EXTENDEDKEY, UInt32)
                End If

                inputList(2 * x) = down
                inputList(2 * x + 1) = up

            Next

            Dim numberOfSuccessfulSimulatedInputs = SendInput(CType(Len * 2, UInt32), inputList, Marshal.SizeOf(GetType(INPUT)))
        End Sub

        ''' <summary>
        ''' Performs a simple modified keystroke Like CTRL-C where CTRL Is the modifierKey And C Is the key.
        ''' The flow Is Modifier KEYDOWN, Key PRESS, Modifier KEYUP.
        ''' </summary>
        ''' <param name="modifierKeyCode">The modifier key</param>
        ''' <param name="keyCode">The key to simulate</param>
        Public Shared Sub SimulateModifiedKeyStroke(modifierKeyCode As VirtualKeyCode, keyCode As VirtualKeyCode)
            SimulateKeyDown(modifierKeyCode)
            SimulateKeyPress(keyCode)
            SimulateKeyUp(modifierKeyCode)
        End Sub

        ''' <summary>
        ''' Performs a modified keystroke where there are multiple modifiers And one key Like CTRL-ALT-C where CTRL And ALT are the modifierKeys And C Is the key.
        ''' The flow Is Modifiers KEYDOWN in order, Key PRESS, Modifiers KEYUP in reverse order.
        ''' </summary>
        ''' <param name="modifierKeyCodes">The list of modifier keys</param>
        ''' <param name="keyCode">The key to simulate</param>
        Public Shared Sub SimulateModifiedKeyStroke(modifierKeyCodes As IEnumerable(Of VirtualKeyCode), keyCode As VirtualKeyCode)
            If (modifierKeyCodes IsNot Nothing) Then modifierKeyCodes.ToList().ForEach(Sub(x) SimulateKeyDown(x))
            SimulateKeyPress(keyCode)
            If (modifierKeyCodes IsNot Nothing) Then modifierKeyCodes.Reverse().ToList().ForEach(Sub(x) SimulateKeyUp(x))
        End Sub

        ''' <summary>
        ''' Performs a modified keystroke where there Is one modifier And multiple keys Like CTRL-K-C where CTRL Is the modifierKey And K And C are the keys.
        ''' The flow Is Modifier KEYDOWN, Keys PRESS in order, Modifier KEYUP.
        ''' </summary>
        ''' <param name="modifierKey">The modifier key</param>
        ''' <param name="keyCodes">The list of keys to simulate</param>
        Public Shared Sub SimulateModifiedKeyStroke(modifierKey As VirtualKeyCode, keyCodes As IEnumerable(Of VirtualKeyCode))
            SimulateKeyDown(modifierKey)
            If (keyCodes IsNot Nothing) Then keyCodes.ToList().ForEach(Sub(x) SimulateKeyPress(x))
            SimulateKeyUp(modifierKey)
        End Sub

        ''' <summary>
        ''' Performs a modified keystroke where there are multiple modifiers And multiple keys Like CTRL-ALT-K-C where CTRL And ALT are the modifierKeys And K And C are the keys.
        ''' The flow Is Modifiers KEYDOWN in order, Keys PRESS in order, Modifiers KEYUP in reverse order.
        ''' </summary>
        ''' <param name="modifierKeyCodes">The list of modifier keys</param>
        ''' <param name="keyCodes">The list of keys to simulate</param>
        Public Shared Sub SimulateModifiedKeyStroke(modifierKeyCodes As IEnumerable(Of VirtualKeyCode), keyCodes As IEnumerable(Of VirtualKeyCode))
            If (modifierKeyCodes IsNot Nothing) Then modifierKeyCodes.ToList().ForEach(Sub(x) SimulateKeyDown(x))
            If (keyCodes IsNot Nothing) Then keyCodes.ToList().ForEach(Sub(x) SimulateKeyPress(x))
            If (modifierKeyCodes IsNot Nothing) Then modifierKeyCodes.Reverse().ToList().ForEach(Sub(x) SimulateKeyUp(x))
        End Sub

#End Region
    End Class
End Namespace