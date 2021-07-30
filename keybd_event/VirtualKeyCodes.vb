Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace WindowsInput

    ''' <summary>
    ''' The list of VirtualKeyCodes (see: http:''msdn.microsoft.com/en-us/library/ms645540(VS.85).aspx)
    ''' </summary> 
    Public Enum VirtualKeyCode As UInt16 ''UShort '' 

        ''' <summary>
        ''' Left mouse button
        ''' </summary>
        LBUTTON = &H1

        ''' <summary>
        ''' Right mouse button
        ''' </summary>
        RBUTTON = &H2

        ''' <summary>
        ''' Control-break processing
        ''' </summary>
        CANCEL = &H3

        ''' <summary>
        ''' Middle mouse button (three-button mouse) - Not contiguous with LBUTTON And RBUTTON
        ''' </summary>
        MBUTTON = &H4

        ''' <summary>
        ''' Windows 2000/XP: X1 mouse button - Not contiguous With LBUTTON And RBUTTON
        ''' </summary>
        XBUTTON1 = &H5

        ''' <summary>
        ''' Windows 2000/XP: X2 mouse button - Not contiguous With LBUTTON And RBUTTON
        ''' </summary>
        XBUTTON2 = &H6

        '' &H07  Undefined

        ''' <summary>
        ''' BACKSPACE key
        ''' </summary>
        BACK = &H8

        ''' <summary>
        ''' TAB key
        ''' </summary>
        TAB = &H9

        '' &H0A - &H0B  Reserved

        ''' <summary>
        ''' CLEAR key
        ''' </summary>
        CLEAR = &HC

        ''' <summary>
        ''' ENTER key
        ''' </summary>
        [Return] = &HD

        '' &H0E - &H0F  Undefined

        ''' <summary>
        ''' SHIFT key
        ''' </summary>
        SHIFT = &H10

        ''' <summary>
        ''' CTRL key
        ''' </summary>
        CONTROL = &H11

        ''' <summary>
        ''' ALT key
        ''' </summary>
        MENU = &H12

        ''' <summary>
        ''' PAUSE key
        ''' </summary>
        PAUSE = &H13

        ''' <summary>
        ''' CAPS LOCK key
        ''' </summary>
        CAPITAL = &H14

        ''' <summary>
        ''' Input Method Editor (IME) Kana mode
        ''' </summary>
        KANA = &H15

        ''' <summary>
        ''' IME Hanguel mode (maintained for compatibility use HANGUL)
        ''' </summary>
        HANGEUL = &H15

        ''' <summary>
        ''' IME Hangul mode
        ''' </summary>
        HANGUL = &H15

        '' &H16  Undefined

        ''' <summary>
        ''' IME Junja mode
        ''' </summary>
        JUNJA = &H17

        ''' <summary>
        ''' IME final mode
        ''' </summary>
        FINAL = &H18

        ''' <summary>
        ''' IME Hanja mode
        ''' </summary>
        HANJA = &H19

        ''' <summary>
        ''' IME Kanji mode
        ''' </summary>
        KANJI = &H19

        '' &H1A  Undefined

        ''' <summary>
        ''' ESC key
        ''' </summary>
        ESCAPE = &H1B

        ''' <summary>
        ''' IME convert
        ''' </summary>
        CONVERT = &H1C

        ''' <summary>
        ''' IME nonconvert
        ''' </summary>
        NONCONVERT = &H1D

        ''' <summary>
        ''' IME accept
        ''' </summary>
        ACCEPT = &H1E

        ''' <summary>
        ''' IME mode change request
        ''' </summary>
        MODECHANGE = &H1F

        ''' <summary>
        ''' SPACEBAR
        ''' </summary>
        SPACE = &H20

        ''' <summary>
        ''' PAGE UP key
        ''' </summary>
        PRIOR = &H21

        ''' <summary>
        ''' PAGE DOWN key
        ''' </summary>
        [Next] = &H22

        ''' <summary>
        ''' END key
        ''' </summary>
        [End] = &H23

        ''' <summary>
        ''' HOME key
        ''' </summary>
        HOME = &H24

        ''' <summary>
        ''' LEFT ARROW key
        ''' </summary>
        LEFT = &H25

        ''' <summary>
        ''' UP ARROW key
        ''' </summary>
        UP = &H26

        ''' <summary>
        ''' RIGHT ARROW key
        ''' </summary>
        RIGHT = &H27

        ''' <summary>
        ''' DOWN ARROW key
        ''' </summary>
        DOWN = &H28

        ''' <summary>
        ''' SELECT key
        ''' </summary>
        [Select] = &H29

        ''' <summary>
        ''' PRINT key
        ''' </summary>
        PRINT = &H2A

        ''' <summary>
        ''' EXECUTE key
        ''' </summary>
        EXECUTE = &H2B

        ''' <summary>
        ''' PRINT SCREEN key
        ''' </summary>
        SNAPSHOT = &H2C

        ''' <summary>
        ''' INS key
        ''' </summary>
        INSERT = &H2D

        ''' <summary>
        ''' DEL key
        ''' </summary>
        DELETE = &H2E

        ''' <summary>
        ''' HELP key
        ''' </summary>
        HELP = &H2F

        ''' <summary>
        ''' 0 key
        ''' </summary>
        VK_0 = &H30

        ''' <summary>
        ''' 1 key
        ''' </summary>
        VK_1 = &H31

        ''' <summary>
        ''' 2 key
        ''' </summary>
        VK_2 = &H32

        ''' <summary>
        ''' 3 key
        ''' </summary>
        VK_3 = &H33

        ''' <summary>
        ''' 4 key
        ''' </summary>
        VK_4 = &H34

        ''' <summary>
        ''' 5 key
        ''' </summary>
        VK_5 = &H35

        ''' <summary>
        ''' 6 key
        ''' </summary>
        VK_6 = &H36

        ''' <summary>
        ''' 7 key
        ''' </summary>
        VK_7 = &H37

        ''' <summary>
        ''' 8 key
        ''' </summary>
        VK_8 = &H38

        ''' <summary>
        ''' 9 key
        ''' </summary>
        VK_9 = &H39

        ''
        '' &H3A - &H40 : Udefined
        ''

        ''' <summary>
        ''' A key
        ''' </summary>
        VK_A = &H41

        ''' <summary>
        ''' B key
        ''' </summary>
        VK_B = &H42

        ''' <summary>
        ''' C key
        ''' </summary>
        VK_C = &H43

        ''' <summary>
        ''' D key
        ''' </summary>
        VK_D = &H44

        ''' <summary>
        ''' E key
        ''' </summary>
        VK_E = &H45

        ''' <summary>
        ''' F key
        ''' </summary>
        VK_F = &H46

        ''' <summary>
        ''' G key
        ''' </summary>
        VK_G = &H47

        ''' <summary>
        ''' H key
        ''' </summary>
        VK_H = &H48

        ''' <summary>
        ''' I key
        ''' </summary>
        VK_I = &H49

        ''' <summary>
        ''' J key
        ''' </summary>
        VK_J = &H4A

        ''' <summary>
        ''' K key
        ''' </summary>
        VK_K = &H4B

        ''' <summary>
        ''' L key
        ''' </summary>
        VK_L = &H4C

        ''' <summary>
        ''' M key
        ''' </summary>
        VK_M = &H4D

        ''' <summary>
        ''' N key
        ''' </summary>
        VK_N = &H4E

        ''' <summary>
        ''' O key
        ''' </summary>
        VK_O = &H4F

        ''' <summary>
        ''' P key
        ''' </summary>
        VK_P = &H50

        ''' <summary>
        ''' Q key
        ''' </summary>
        VK_Q = &H51

        ''' <summary>
        ''' R key
        ''' </summary>
        VK_R = &H52

        ''' <summary>
        ''' S key
        ''' </summary>
        VK_S = &H53

        ''' <summary>
        ''' T key
        ''' </summary>
        VK_T = &H54

        ''' <summary>
        ''' U key
        ''' </summary>
        VK_U = &H55

        ''' <summary>
        ''' V key
        ''' </summary>
        VK_V = &H56

        ''' <summary>
        ''' W key
        ''' </summary>
        VK_W = &H57

        ''' <summary>
        ''' X key
        ''' </summary>
        VK_X = &H58

        ''' <summary>
        ''' Y key
        ''' </summary>
        VK_Y = &H59

        ''' <summary>
        ''' Z key
        ''' </summary>
        VK_Z = &H5A

        ''' <summary>
        ''' Left Windows key (Microsoft Natural keyboard)
        ''' </summary>
        LWIN = &H5B

        ''' <summary>
        ''' Right Windows key (Natural keyboard)
        ''' </summary>
        RWIN = &H5C

        ''' <summary>
        ''' Applications key (Natural keyboard)
        ''' </summary>
        APPS = &H5D

        '' &H5E  reserved

        ''' <summary>
        ''' Computer Sleep key
        ''' </summary>
        SLEEP = &H5F

        ''' <summary>
        ''' Numeric keypad 0 key
        ''' </summary>
        NUMPAD0 = &H60

        ''' <summary>
        ''' Numeric keypad 1 key
        ''' </summary>
        NUMPAD1 = &H61

        ''' <summary>
        ''' Numeric keypad 2 key
        ''' </summary>
        NUMPAD2 = &H62

        ''' <summary>
        ''' Numeric keypad 3 key
        ''' </summary>
        NUMPAD3 = &H63

        ''' <summary>
        ''' Numeric keypad 4 key
        ''' </summary>
        NUMPAD4 = &H64

        ''' <summary>
        ''' Numeric keypad 5 key
        ''' </summary>
        NUMPAD5 = &H65

        ''' <summary>
        ''' Numeric keypad 6 key
        ''' </summary>
        NUMPAD6 = &H66

        ''' <summary>
        ''' Numeric keypad 7 key
        ''' </summary>
        NUMPAD7 = &H67

        ''' <summary>
        ''' Numeric keypad 8 key
        ''' </summary>
        NUMPAD8 = &H68

        ''' <summary>
        ''' Numeric keypad 9 key
        ''' </summary>
        NUMPAD9 = &H69

        ''' <summary>
        ''' Multiply key
        ''' </summary>
        MULTIPLY = &H6A

        ''' <summary>
        ''' Add key
        ''' </summary>
        ADD = &H6B

        ''' <summary>
        ''' Separator key
        ''' </summary>
        SEPARATOR = &H6C

        ''' <summary>
        ''' Subtract key
        ''' </summary>
        SUBTRACT = &H6D

        ''' <summary>
        ''' Decimal key
        ''' </summary>
        [Decimal] = &H6E

        ''' <summary>
        ''' Divide key
        ''' </summary>
        DIVIDE = &H6F

        ''' <summary>
        ''' F1 key
        ''' </summary>
        F1 = &H70

        ''' <summary>
        ''' F2 key
        ''' </summary>
        F2 = &H71

        ''' <summary>
        ''' F3 key
        ''' </summary>
        F3 = &H72

        ''' <summary>
        ''' F4 key
        ''' </summary>
        F4 = &H73

        ''' <summary>
        ''' F5 key
        ''' </summary>
        F5 = &H74

        ''' <summary>
        ''' F6 key
        ''' </summary>
        F6 = &H75

        ''' <summary>
        ''' F7 key
        ''' </summary>
        F7 = &H76

        ''' <summary>
        ''' F8 key
        ''' </summary>
        F8 = &H77

        ''' <summary>
        ''' F9 key
        ''' </summary>
        F9 = &H78

        ''' <summary>
        ''' F10 key
        ''' </summary>
        F10 = &H79

        ''' <summary>
        ''' F11 key
        ''' </summary>
        F11 = &H7A

        ''' <summary>
        ''' F12 key
        ''' </summary>
        F12 = &H7B

        ''' <summary>
        ''' F13 key
        ''' </summary>
        F13 = &H7C

        ''' <summary>
        ''' F14 key
        ''' </summary>
        F14 = &H7D

        ''' <summary>
        ''' F15 key
        ''' </summary>
        F15 = &H7E

        ''' <summary>
        ''' F16 key
        ''' </summary>
        F16 = &H7F

        ''' <summary>
        ''' F17 key
        ''' </summary>
        F17 = &H80

        ''' <summary>
        ''' F18 key
        ''' </summary>
        F18 = &H81

        ''' <summary>
        ''' F19 key
        ''' </summary>
        F19 = &H82

        ''' <summary>
        ''' F20 key
        ''' </summary>
        F20 = &H83

        ''' <summary>
        ''' F21 key
        ''' </summary>
        F21 = &H84

        ''' <summary>
        ''' F22 key
        ''' </summary>
        F22 = &H85

        ''' <summary>
        ''' F23 key
        ''' </summary>
        F23 = &H86

        ''' <summary>
        ''' F24 key
        ''' </summary>
        F24 = &H87

        ''
        '' &H88 - &H8F : Unassigned
        ''

        ''' <summary>
        ''' NUM LOCK key
        ''' </summary>
        NUMLOCK = &H90

        ''' <summary>
        ''' SCROLL LOCK key
        ''' </summary>
        SCROLL = &H91

        '' &H92 - &H96 : OEM Specific

        '' &H97 - &H9F  Unassigned

        ''
        '' L* & R* - left And right Alt Ctrl And Shift virtual keys.
        '' Used only as parameters to GetAsyncKeyState() And GetKeyState().
        '' No other API Or message will distinguish left And right keys in this way.
        ''

        ''' <summary>
        ''' Left SHIFT key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        LSHIFT = &HA0

        ''' <summary>
        ''' Right SHIFT key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        RSHIFT = &HA1

        ''' <summary>
        ''' Left CONTROL key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        LCONTROL = &HA2

        ''' <summary>
        ''' Right CONTROL key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        RCONTROL = &HA3

        ''' <summary>
        ''' Left MENU key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        LMENU = &HA4

        ''' <summary>
        ''' Right MENU key - Used only as parameters to GetAsyncKeyState() And GetKeyState()
        ''' </summary>
        RMENU = &HA5

        ''' <summary>
        ''' Windows 2000/XP: Browser Back key
        ''' </summary>
        BROWSER_BACK = &HA6

        ''' <summary>
        ''' Windows 2000/XP: Browser Forward key
        ''' </summary>
        BROWSER_FORWARD = &HA7

        ''' <summary>
        ''' Windows 2000/XP: Browser Refresh key
        ''' </summary>
        BROWSER_REFRESH = &HA8

        ''' <summary>
        ''' Windows 2000/XP: Browser Stop key
        ''' </summary>
        BROWSER_STOP = &HA9

        ''' <summary>
        ''' Windows 2000/XP: Browser Search key
        ''' </summary>
        BROWSER_SEARCH = &HAA

        ''' <summary>
        ''' Windows 2000/XP: Browser Favorites key
        ''' </summary>
        BROWSER_FAVORITES = &HAB

        ''' <summary>
        ''' Windows 2000/XP: Browser Start And Home key
        ''' </summary>
        BROWSER_HOME = &HAC

        ''' <summary>
        ''' Windows 2000/XP: Volume Mute key
        ''' </summary>
        VOLUME_MUTE = &HAD

        ''' <summary>
        ''' Windows 2000/XP: Volume Down key
        ''' </summary>
        VOLUME_DOWN = &HAE

        ''' <summary>
        ''' Windows 2000/XP: Volume Up key
        ''' </summary>
        VOLUME_UP = &HAF

        ''' <summary>
        ''' Windows 2000/XP: Next Track key
        ''' </summary>
        MEDIA_NEXT_TRACK = &HB0

        ''' <summary>
        ''' Windows 2000/XP: Previous Track key
        ''' </summary>
        MEDIA_PREV_TRACK = &HB1

        ''' <summary>
        ''' Windows 2000/XP: Stop Media key
        ''' </summary>
        MEDIA_STOP = &HB2

        ''' <summary>
        ''' Windows 2000/XP: Play/Pause Media key
        ''' </summary>
        MEDIA_PLAY_PAUSE = &HB3

        ''' <summary>
        ''' Windows 2000/XP: Start Mail key
        ''' </summary>
        LAUNCH_MAIL = &HB4

        ''' <summary>
        ''' Windows 2000/XP: Select Case Media key
        ''' </summary>
        LAUNCH_MEDIA_SELECT = &HB5

        ''' <summary>
        ''' Windows 2000/XP: Start Application 1 key
        ''' </summary>
        LAUNCH_APP1 = &HB6

        ''' <summary>
        ''' Windows 2000/XP: Start Application 2 key
        ''' </summary>
        LAUNCH_APP2 = &HB7

        ''
        '' &HB8 - &HB9 : Reserved
        ''

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the ':' key 
        ''' </summary>
        OEM_1 = &HBA

        ''' <summary>
        ''' Windows 2000/XP: For any country/region the '+' key
        ''' </summary>
        OEM_PLUS = &HBB

        ''' <summary>
        ''' Windows 2000/XP: For any country/region the '' key
        ''' </summary>
        OEM_COMMA = &HBC

        ''' <summary>
        ''' Windows 2000/XP: For any country/region the '-' key
        ''' </summary>
        OEM_MINUS = &HBD

        ''' <summary>
        ''' Windows 2000/XP: For any country/region the '.' key
        ''' </summary>
        OEM_PERIOD = &HBE

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the '/?' key 
        ''' </summary>
        OEM_2 = &HBF

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the '`~' key 
        ''' </summary>
        OEM_3 = &HC0

        ''
        '' &HC1 - &HD7 : Reserved
        ''

        ''
        '' &HD8 - &HDA : Unassigned
        ''

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the '[{' key
        ''' </summary>
        OEM_4 = &HDB

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the '\|' key
        ''' </summary>
        OEM_5 = &HDC

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the ']}' key
        ''' </summary>
        OEM_6 = &HDD

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard. Windows 2000/XP: For the US standard keyboard the 'single-quote/double-quote' key
        ''' </summary>
        OEM_7 = &HDE

        ''' <summary>
        ''' Used for miscellaneous characters it can vary by keyboard.
        ''' </summary>
        OEM_8 = &HDF

        ''
        '' &HE0 : Reserved
        ''

        ''
        '' &HE1 : OEM Specific
        ''

        ''' <summary>
        ''' Windows 2000/XP: Either the angle bracket key Or the backslash key On the RT 102-key keyboard
        ''' </summary>
        OEM_102 = &HE2

        ''
        '' (&HE3-E4) : OEM specific
        ''

        ''' <summary>
        ''' Windows 95/98/Me Windows NT 4.0 Windows 2000/XP: IME PROCESS key
        ''' </summary>
        PROCESSKEY = &HE5

        ''
        '' &HE6 : OEM specific
        ''

        ''' <summary>
        ''' Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The PACKET key Is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information see Remark in KEYBDINPUT SendInput WM_KEYDOWN And WM_KEYUP
        ''' </summary>
        PACKET = &HE7

        ''
        '' &HE8 : Unassigned
        ''

        ''
        '' &HE9-F5 : OEM specific
        ''

        ''' <summary>
        ''' Attn key
        ''' </summary>
        ATTN = &HF6

        ''' <summary>
        ''' CrSel key
        ''' </summary>
        CRSEL = &HF7

        ''' <summary>
        ''' ExSel key
        ''' </summary>
        EXSEL = &HF8

        ''' <summary>
        ''' Erase EOF key
        ''' </summary>
        EREOF = &HF9

        ''' <summary>
        ''' Play key
        ''' </summary>
        PLAY = &HFA

        ''' <summary>
        ''' Zoom key
        ''' </summary>
        ZOOM = &HFB

        ''' <summary>
        ''' Reserved
        ''' </summary>
        NONAME = &HFC

        ''' <summary>
        ''' PA1 key
        ''' </summary>
        PA1 = &HFD

        ''' <summary>
        ''' Clear key
        ''' </summary>
        OEM_CLEAR = &HFE
    End Enum
    Public Class WindowsInputHelper
        Public Shared Function ConvertKeysToVKey(k As Keys) As VirtualKeyCode
            Select Case k
                Case Keys.A To Keys.Z
                    Return [Enum].Parse(GetType(VirtualKeyCode), "VK_" & k.ToString)
                Case Keys.D0 To Keys.D9
                    Return [Enum].Parse(GetType(VirtualKeyCode), "VK_" & k.ToString.Remove(1))
                Case Else
                    Return [Enum].Parse(GetType(VirtualKeyCode), k.ToString)

            End Select
        End Function
    End Class
End Namespace
