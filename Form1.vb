Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports MAME


<DebuggerStepThrough>
Module submain
    <MTAThread>
    Public Sub Main()
        AddHandler Application.ThreadException, AddressOf MyGlobalExceptionHandler
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
        Application.Run(Form1)
    End Sub
    Sub MyGlobalExceptionHandler(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        frmMsg.Msgbox(e.Exception.Message, MsgBoxStyle.OkOnly, e.Exception.ToString)
        Application.Exit()
    End Sub
    Sub CurrentDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        If e.IsTerminating Then
            frmMsg.Msgbox(DirectCast(e.ExceptionObject, Exception).Message, MsgBoxStyle.OkOnly, e.ExceptionObject.ToString)
        Else
            Select Case frmMsg.Msgbox(DirectCast(e.ExceptionObject, Exception).Message, MsgBoxStyle.AbortRetryIgnore, e.ExceptionObject.ToString)
                Case MsgBoxResult.Abort
                    Application.Exit()
                Case MsgBoxResult.Retry

                Case MsgBoxResult.Ignore
            End Select
        End If
    End Sub
End Module

Public Class Form1

    Public Structure FileSize
        Public Length As Long
        Public Enum [Format] As Integer
            Auto = -1
            Bytes = 0
            KiloBytes = 1
            MegaBytes = 2
            GigaBytes = 3
            TeraBytes = 4
            PetaBytes = 5
            ExaBytes = 6
            ZettaBytes = 7
            YottaBytes = 8
        End Enum
        Public Enum FormatShortHand As Integer
            B = FileSize.[Format].Bytes
            KB = FileSize.[Format].KiloBytes
            MB = FileSize.Format.MegaBytes
            GB = FileSize.Format.GigaBytes
            TB = FileSize.Format.TeraBytes
            PB = FileSize.Format.PetaBytes
            EB = FileSize.Format.ExaBytes
            ZB = FileSize.Format.ZettaBytes
            YB = FileSize.Format.YottaBytes
        End Enum

        Public Overloads Function ToString(Optional DigitsAfterDecimal As Integer = 2, Optional f As [Format] = [Format].Auto, Optional bShorthand As Boolean = False) As String
            If f = Format.Auto Then
                If Length < 1024 Then
                    Return ToString(DigitsAfterDecimal, Format.Bytes)
                ElseIf Length < 1024 ^ 2 Then
                    Return ToString(DigitsAfterDecimal, Format.KiloBytes)
                ElseIf Length < 1024 ^ 3 Then
                    Return ToString(DigitsAfterDecimal, Format.MegaBytes)
                ElseIf Length < 1024 ^ 4 Then
                    Return ToString(DigitsAfterDecimal, Format.GigaBytes)
                ElseIf Length < 1024 ^ 5 Then
                    Return ToString(DigitsAfterDecimal, Format.TeraBytes)
                ElseIf Length < 1024 ^ 6 Then
                    Return ToString(DigitsAfterDecimal, Format.PetaBytes)
                ElseIf Length < 1024 ^ 7 Then
                    Return ToString(DigitsAfterDecimal, Format.ExaBytes)
                ElseIf Length < 1024 ^ 8 Then
                    Return ToString(DigitsAfterDecimal, Format.ZettaBytes)
                Else 'If Length < 1024 ^ 9 Then
                    Return ToString(DigitsAfterDecimal, Format.YottaBytes)
                End If
            Else
                Return FormatNumber(Length \ (1024 ^ f), DigitsAfterDecimal,,, TriState.True) & " " & IIf(bShorthand, [Enum].GetName(GetType(FormatShortHand), f), [Enum].GetName(GetType([Format]), f))
            End If
        End Function
    End Structure
    Private Class Emulator
        Public Property Name As String
        Public Property Path As String
        Public Property Actions As IEnumerable(Of MenulatorAction)
        Public Property RomTags As IEnumerable(Of String)
        Public Property Args As String
    End Class
    Public Class MenulatorAction
        Public Property Name As String
        Public Property KeyboardKey As Keys
        Public Property Joystick As IEnumerable(Of MenulatorJoystickAction)
        Public Property Modifiers As Keys
        Public Property ImgTag As String
        Public Property closeWindow As Boolean = False
        Public Property hideWindow As Boolean = True
        Public Sub New()

        End Sub
        Public Sub New(strName As String, key As Keys, Optional modifier As Keys = Keys.None, Optional imgTag As String = "", Optional closeWindow As Boolean = False, Optional hideWindow As Boolean = True)
            Me.New
            Name = strName
            KeyboardKey = key
            Me.Modifiers = modifier
            Me.ImgTag = imgTag
            Me.closeWindow = closeWindow
            Me.hideWindow = hideWindow

        End Sub
    End Class
    Public Class MenulatorJoystickAction
        Public Property Id As Integer = -1
        Dim iButton As Integer = -1
        Public Property Button As Integer
            Get
                Return iButton
            End Get
            Set(value As Integer)
                If value = Nothing Then iButton = -1
                iButton = value
            End Set
        End Property
        Dim iAxis As Integer = -1
        Public Property Axis As Integer
            Get
                Return iAxis
            End Get
            Set(value As Integer)
                If value = Nothing Then iAxis = -1
                iAxis = value
            End Set
        End Property
        Public Property AxisDirection As Integer

        Public Property KeyboardKey As Keys

        Public Overrides Function ToString() As String
            If iButton > 0 Then
                Return "{Id=" & Id & ";Button=" & iButton & ";Key=" & KeyboardKey.ToString & "}"
            Else
                Return "{Id=" & Id & ";Axis=" & iAxis & ";Direction=" & AxisDirection & ";Key=" & KeyboardKey.ToString & "}"
            End If
        End Function

    End Class

    Dim myMame As MAME.App
    Dim myMame_Ini As MAME.INI
    Friend myMame_XML As MAME.MameXml
    Public InGame As Boolean
    Dim snapdir As String()
    Dim WithEvents myList As MenulatorListView

    Dim MenulatorGameMenu As frmMenulator


    Dim myGamesMenu As New Dictionary(Of String, NewUIListItem)
    Dim myEmulatorList As New EmulatorDictionary
    Dim WithEvents myWMP As New WMPLib.WindowsMediaPlayer

    Private Class EmulatorDictionary
        Inherits Collections.ObjectModel.Collection(Of Emulator)
        Default Public Overloads ReadOnly Property Item(strTag As String) As IEnumerable(Of Emulator)
            Get
                Return (From a As Emulator In Me, b In a.RomTags Where b = strTag Select a)
            End Get
        End Property
        Public Function ContainsKey(strTag As String) As Boolean
            Return (From a In Me, b In a.RomTags Where b = strTag Take 1).Count
        End Function



    End Class
    Public myKeybindings As New Dictionary(Of String, MenulatorAction)
    Public myJoystickBindings As New JoystickActionDictionary
    Public Class JoystickActionDictionary
        Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction))
        Dim l() As List(Of MenulatorJoystickAction)
        Dim indexlookup As New Dictionary(Of Integer, Integer)

        Default Public Property Item(iJoystickID As Integer) As List(Of MenulatorJoystickAction) Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).Item
            Get
                Return l(indexlookup(iJoystickID))
            End Get
            Set(value As List(Of MenulatorJoystickAction))
                l(indexlookup(iJoystickID)) = value
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of Integer) Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).Keys
            Get
                Return indexlookup.Keys
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of List(Of MenulatorJoystickAction)) Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).Values
            Get
                Return l
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).Count
            Get
                Return indexlookup.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Sub Add(key As Integer, value As List(Of MenulatorJoystickAction)) Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).Add
            If Not indexlookup.ContainsKey(key) Then
                If l Is Nothing Then ReDim l(0) Else ReDim Preserve l(UBound(l) + 1)
                indexlookup.Add(key, UBound(l))
            End If
            l(indexlookup(key)) = value
        End Sub

        Public Sub Add(item As KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))) Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).Add
            Throw New NotImplementedException()
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).Clear
            indexlookup.Clear()
            Erase l
            l = Nothing
        End Sub

        Public Sub CopyTo(array() As KeyValuePair(Of Integer, List(Of MenulatorJoystickAction)), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).CopyTo
            Throw New NotImplementedException()
        End Sub

        Public Function ContainsKey(key As Integer) As Boolean Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).ContainsKey
            Return indexlookup.ContainsKey(key)
        End Function

        Public Function Remove(key As Integer) As Boolean Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).Remove
            If ContainsKey(key) Then
                l(indexlookup(key)).Clear()
                indexlookup.Remove(key)
                Return True
            Else
                Return False
            End If
        End Function

        Public Sub Remap(oldIndex As Integer, newIndex As Integer)
            indexlookup(oldIndex) = newIndex
        End Sub

        Public Function Remove(item As KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))) As Boolean Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).Remove
            Throw New NotImplementedException()
        End Function

        Public Function TryGetValue(key As Integer, ByRef value As List(Of MenulatorJoystickAction)) As Boolean Implements IDictionary(Of Integer, List(Of MenulatorJoystickAction)).TryGetValue
            Throw New NotImplementedException()
        End Function

        Public Function Contains(item As KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))) As Boolean Implements ICollection(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).Contains
            Return l.Contains(item.Value)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))) Implements IEnumerable(Of KeyValuePair(Of Integer, List(Of MenulatorJoystickAction))).GetEnumerator
            Return l.GetEnumerator
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return l.GetEnumerator
        End Function
    End Class

    Dim myFavorites As New Favorites("Favorites.xml")
    Dim MyBad As New MarkedAsBad("Favorites.xml")

    Private Class Rom
        Implements IRom

        Dim _Name As String
        Dim _Path As String
        Dim _ImagePath As String
        Dim _Description As String
        Public ClickHandler As EventHandler
        Public Property Name As String Implements IRom.Name
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        Public Property Path As String Implements IRom.Path
            Get
                Return _Path
            End Get
            Set(value As String)
                _Path = value
            End Set
        End Property

        Public Property ImagePath As String Implements IRom.ImagePath
            Get
                Return _ImagePath
            End Get
            Set(value As String)
                _ImagePath = value
            End Set
        End Property

        Public Property Description As String Implements IRom.Description
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property

        Public Property Favorite As Boolean Implements IRom.Favorite

    End Class
    Private Sub InitializeComponents()
        myList = New MenulatorListView
        myList_ScaleImageRoutine(".", My.Resources.MAME)

        myList.ForeColor = Color.White


        'myList.ReInit(Function(x As XElement)
        '                  Return x.<description>.Value.StartsWith("bl", StringComparison.InvariantCultureIgnoreCase)
        '              End Function)
        'myList.ApplyFilter(Function(x As XElement) As Boolean
        '                       Return True ' x.<players> IsNot Nothing AndAlso Val(x.<players>.Value) >= 2

        '                   End Function)




        Me.EnableBlur()
        SetStyle(ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        BackColor = Color.LimeGreen
        TransparencyKey = BackColor


        myList.Dock = DockStyle.Fill
        Me.Controls.Add(myList)
        myList.Font = New Font("Malgun Gothic", 18.0F)
        'myList.Font = New Font("Reggae One Regular", 18.0F)
        myMame_XML = New MameXml()

        'Dim raw As New RAWINPUTDEVICE() With {.hwndTarget = Me.Handle, .usUsage = 4, .usUsagePage = 1, .dwFlags = 0}
        'RegisterRawInputDevices(raw, 1, Marshal.SizeOf(GetType(RAWINPUTDEVICE)))

    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        pnlLeft.Animation = PanelEx.AnimateWindowFlags.AW_HOR_POSITIVE Or PanelEx.AnimateWindowFlags.AW_SLIDE
        'pnlLeft2.Animation = PanelEx.AnimateWindowFlags.AW_HOR_POSITIVE 'Or PanelEx.AnimateWindowFlags.AW_SLIDE
        imgMenulator.Animation = PanelEx.AnimateWindowFlags.AW_HOR_POSITIVE ' Or PanelEx.AnimateWindowFlags.AW_SLIDE
        'imgMenulatorIcon.Animation = PanelEx.AnimateWindowFlags.AW_HOR_POSITIVE Or PanelEx.AnimateWindowFlags.AW_SLIDE
        pnlRight.Animation = PanelEx.AnimateWindowFlags.AW_HOR_NEGATIVE Or PanelEx.AnimateWindowFlags.AW_SLIDE
        'SystemParametersInfo(22, 0, KeyboardDelay, 0)
        'Select Case KeyboardDelay
        '    Case 0
        '        KeyboardDelay = 250
        '    Case 1
        '        KeyboardDelay = 500
        '    Case 2
        '        KeyboardDelay = 750
        '    Case 3
        '        KeyboardDelay = 1000
        'End Select
        'SystemParametersInfo(10, 0, KeyboardRepeatSpeed, 0)
        'Select Case KeyboardRepeatSpeed
        '    Case 0 : KeyboardRepeatSpeed = 400
        '    Case 1 : KeyboardRepeatSpeed = 363
        '    Case 2 : KeyboardRepeatSpeed = 352
        '    Case 3 : KeyboardRepeatSpeed = 341
        '    Case 4 : KeyboardRepeatSpeed = 330
        '    Case 5 : KeyboardRepeatSpeed = 319
        '    Case 6 : KeyboardRepeatSpeed = 308
        '    Case 7 : KeyboardRepeatSpeed = 297
        '    Case 8 : KeyboardRepeatSpeed = 286
        '    Case 9 : KeyboardRepeatSpeed = 275
        '    Case 10 : KeyboardRepeatSpeed = 264
        '    Case 11 : KeyboardRepeatSpeed = 253
        '    Case 12 : KeyboardRepeatSpeed = 242
        '    Case 13 : KeyboardRepeatSpeed = 231
        '    Case 14 : KeyboardRepeatSpeed = 220
        '    Case 15 : KeyboardRepeatSpeed = 209
        '    Case 16 : KeyboardRepeatSpeed = 198
        '    Case 17 : KeyboardRepeatSpeed = 187
        '    Case 18 : KeyboardRepeatSpeed = 176
        '    Case 19 : KeyboardRepeatSpeed = 165
        '    Case 20 : KeyboardRepeatSpeed = 154
        '    Case 21 : KeyboardRepeatSpeed = 143
        '    Case 22 : KeyboardRepeatSpeed = 114
        '    Case 23 : KeyboardRepeatSpeed = 105
        '    Case 24 : KeyboardRepeatSpeed = 96
        '    Case 25 : KeyboardRepeatSpeed = 88
        '    Case 26 : KeyboardRepeatSpeed = 75
        '    Case 27 : KeyboardRepeatSpeed = 69
        '    Case 28 : KeyboardRepeatSpeed = 60
        '    Case 29 : KeyboardRepeatSpeed = 51
        '    Case 30 : KeyboardRepeatSpeed = 42
        '    Case 31 : KeyboardRepeatSpeed = 33

        'End Select
        'XInput.XInputWrapper.XboxController.StartPolling()
        myKeybindings.Add("Action", New MenulatorAction("Action", Keys.Return))
        myKeybindings.Add("Cancel", New MenulatorAction("Cancel", Keys.None))
        myKeybindings.Add("AltAction", New MenulatorAction("AltAction", Keys.None))
        myKeybindings.Add("Menu", New MenulatorAction("Menu", Keys.None))
        myKeybindings.Add("Escape", New MenulatorAction("Escape", Keys.Escape))
        myKeybindings.Add("Pause", New MenulatorAction("Pause", Keys.None))
        myKeybindings.Add("Left", New MenulatorAction("Left", Keys.Left))
        myKeybindings.Add("Right", New MenulatorAction("Right", Keys.Right))
        myKeybindings.Add("Up", New MenulatorAction("Up", Keys.Up))
        myKeybindings.Add("Down", New MenulatorAction("Down", Keys.Down))
        myKeybindings.Add("Search", New MenulatorAction("Search", Keys.None))
        myKeybindings.Add("Refresh", New MenulatorAction("Refresh", Keys.F5))
        myKeybindings.Add("MameVerification", New MenulatorAction("MameVerification", Keys.F12))
        myKeybindings.Add("Start1", New MenulatorAction("Start1", Keys.None))
        myKeybindings.Add("Start2", New MenulatorAction("Start2", Keys.None))
        myKeybindings.Add("Coin1", New MenulatorAction("Coin1", Keys.None))
        myKeybindings.Add("Coin2", New MenulatorAction("Coin2", Keys.None))
        myKeybindings.Add("Save", New MenulatorAction("Save", Keys.None))
        myKeybindings.Add("Load", New MenulatorAction("Load", Keys.None))
        myKeybindings.Add("TestSwitch", New MenulatorAction("TestSwitch", Keys.None))
        myKeybindings.Add("PageUp", New MenulatorAction("PageUp", Keys.PageUp))
        myKeybindings.Add("PageDown", New MenulatorAction("PageDown", Keys.PageDown))
        myKeybindings.Add("Home", New MenulatorAction("Home", Keys.Home))
        myKeybindings.Add("End", New MenulatorAction("End", Keys.End))


        LoadSettings()


        Dim strMamePath As String = Nothing
        If myEmulatorList.ContainsKey("MAME") Then strMamePath = myEmulatorList("MAME")(0).Path

        If IO.File.Exists(strMamePath) Then

            myMame = New MAME.App(strMamePath)

            myMame_Ini = New MAME.INI(myMame.MamePath)
            snapdir = myMame_Ini.ExpandPath(Split(myMame_Ini.RootValues("snapshot_directory").ValueOrDefault, ";"))

            'If Not IO.File.Exists(IO.Path.Combine(My.Application.Info.DirectoryPath, "Systems\MAME.xml")) Then
            '    frmMsg.Msgbox("Menulator needs to complile a verified list of ROMs installed on this machine. This will take some time.", vbOKOnly)
            '    Dim i = MAME.MameXml.VerifyRoms(myMame.MamePath)
            '    frmMsg.Msgbox("Verification complete! " & FormatNumber(i, 0,,, TriState.True) & " ROM(s) verified.", vbOKOnly)
            'End If
        End If
        'MAME.MameXml.VerifyRoms(strMamePath)


        InitializeComponents()

        'FlowLayoutPanel1.Visible = True
        imgMenulator.Visible = True
        imgMenulatorIcon.Visible = False
        ' debugobject.BringToFront()
        imgMenulatorIcon.BringToFront()
        myList.Index = 0
        myList.Focus()

        cboPlayersOp.SelectedIndex = 0
        cboYearOp.SelectedIndex = 0

        'pnlLeft.Controls.Remove(Me.FlowLayoutPanel1)
        ReDim subMenus(0)
        subMenus(0) = CreateMainMenu()
        ReDim _subMenuIndex(0)
        pnlLeft.Controls.Add(subMenus(0))
        'pnlLeft.Visible = True

    End Sub
    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        pnlLeft.Height = Me.Height
        pnlLeft.Visible = True
    End Sub
    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Try
            If myPopup IsNot Nothing Then myPopup.Dispose()
            If MenulatorGameMenu IsNot Nothing Then MenulatorGameMenu.Dispose()
        Catch
        End Try
    End Sub

    Private Sub LoadSettings(Optional strPath As String = "Menulator.xml")
        Dim file As New Xml.XmlDocument()
        file.Load(strPath)
        Dim xmlVersion = file.DocumentElement.GetAttribute("version")
        myGamesMenu.Clear()
        For Each x As Xml.XmlElement In file.SelectNodes("descendant::games")
            For Each y As Xml.XmlNode In x.ChildNodes
                'Debug.Print(y.InnerText)
                If InferBooleanValue(y.Attributes("isMame")) Then
                    myGamesMenu.Add("MAME", New NewUIListItem() With {.Dock = DockStyle.Left, .Height = 48, .Width = pnlLeft.Width - 28,
                                        .Text = y.InnerText, .Tag = "MAME", .Image = My.Resources.MAME, .ClickHandler = Sub()
                                                                                                                            ListMAME(ListMAMEDefaultClause(Function(z As XElement) As Boolean
                                                                                                                                                               Return Not (z.<genre>.Value.Contains("* Mature *") Or z.<genre>.Value.Contains("Pinball"))
                                                                                                                                                           End Function))
                                                                                                                            pnlLeft.Visible = False
                                                                                                                        End Sub,
                                    .AltClickHandler = Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)

                                                           With NewSubMenu(New NewUIListItem() {
                                                    New NewUIListItem("Display Favorites", My.Resources.favorite, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                      Dim favs() As String = Nothing
                                                                                                                      favs = myFavorites("MAME")
                                                                                                                      'If DirectCast(subMenus(SubMenuDepth).Controls(SubMenuIndex(SubMenuDepth)), NewUIListItem).Tag = "MAME" Then
                                                                                                                      ListMAME(ListMAMEDefaultClause(Function(element As XElement) As Boolean
                                                                                                                                                         Return (From a2 In favs Where a2 = element.@<name> Take 1).Count
                                                                                                                                                     End Function))
                                                                                                                      'Else
                                                                                                                      ' myGamesMenu(subMenus(SubMenuDepth).Controls(SubMenuIndex(SubMenuDepth)).Tag).PerformAltClick(New NewUIListItem.NewUIListItemClickedEvent(result2))
                                                                                                                      'End If
                                                                                                                      result2.CloseMenu = True
                                                                                                                      result2.CloseAllMenu = True
                                                                                                                      result2.Handled = True
                                                                                                                  End Sub)
                           })
                                                           End With
                                                           result.Handled = True
                                                           result.CloseMenu = False
                                                       End Sub})

                Else
                    Dim strTag As String
                    If y.Attributes("romTag") Is Nothing Then
                        strTag = y.Attributes("name").Value
                    Else
                        strTag = y.Attributes("romTag").Value
                    End If
                    myGamesMenu.Add(strTag, New NewUIListItem() With {.Dock = DockStyle.Left, .Height = 48, .Width = pnlLeft.Width - 28,
                                 .Text = y.Item("description").InnerText,
                                 .Tag = strTag,
                                 .Image = My.Resources.ResourceManager.GetObject(y.Attributes("image").Value),
                                 .ClickHandler = Sub()

                                                     ListRom(y.Attributes("romType").Value,
                                                             strTag,
                                                             y.Attributes("romPath").Value,
                                                             My.Resources.ResourceManager.GetObject(y.Attributes("image").Value), InferStringValue(y.Attributes("romMethod")))

                                                 End Sub,
                                 .AltClickHandler = Sub()

                                                        ListRom(y.Attributes("romType").Value,
                                                                strTag,
                                                                y.Attributes("romPath").Value,
                                                                My.Resources.ResourceManager.GetObject(y.Attributes("image").Value), y.Attributes("romMethod").Value, Function(r As IRom) As Boolean
                                                                                                                                                                          Return myFavorites.IsFavorite(y.Attributes("romType").Value, r.Name)
                                                                                                                                                                      End Function)

                                                    End Sub
                            })
                End If
            Next
        Next

        myEmulatorList.Clear()
        For Each x As Xml.XmlElement In file.SelectNodes("descendant::emulators")
            For Each y As Xml.XmlNode In x.ChildNodes
                Dim actions As IEnumerable(Of MenulatorAction) = (From z As Xml.XmlNode In y.SelectNodes("descendant::action") Select New MenulatorAction() With {
                                                                                                                                       .Name = z.Attributes("display").Value,
                                                                                                                                       .ImgTag = z.Attributes("imgTag").Value,
                                                                                                                                       .KeyboardKey = (From z2 As Xml.XmlNode In z.SelectNodes("descendant::keyboard") Select [Enum].Parse(GetType(Keys), z2.Attributes("button").Value))(0),
                                                                                                                                       .closeWindow = InferBooleanValue(z.Attributes("closeWindow"), True),
                                                                                                                                           .hideWindow = InferBooleanValue(z.Attributes("hideWindow"))})

                Dim romtags As IEnumerable(Of String) = (From z As Xml.XmlNode In y.SelectNodes("descendant::romtag") Select z.Attributes("id").Value)



                myEmulatorList.Add(New Emulator() With {
                                       .Name = y.Attributes("name").Value,
                                       .Path = y.Attributes("emuPath").Value,
                                       .Actions = actions,
                                       .RomTags = romtags,
                                       .Args = InferStringValue(y.Attributes("args"))
                    })


            Next
        Next

        myKeybindings.Clear()
        myJoystickBindings.Clear()

        For Each x As Xml.XmlElement In file.SelectNodes("descendant::keybinding")
            For Each y As Xml.XmlNode In x.ChildNodes
                If y.Name = "action" Then
                    Dim a = New MenulatorAction() With {.Name = y.Attributes("display").Value,
                                                                                                 .ImgTag = InferStringValue(y.Attributes("imgTag")),
                                                                                                 .KeyboardKey = (From z2 As Xml.XmlNode In y.SelectNodes("descendant::keyboard") Select [Enum].Parse(GetType(Keys), z2.Attributes("button").Value))(0),
                                                                                                                                       .closeWindow = InferBooleanValue(y.Attributes("closeWindow"), True),
                                                                                                                                       .hideWindow = InferBooleanValue(y.Attributes("hideWindow")),
                                                                                                 .Joystick = (From z2 As Xml.XmlNode In y.SelectNodes("descendant::joystick") Select New MenulatorJoystickAction() With {.Id = z2.Attributes("id").Value,
                                                                                                                                       .Button = InferStringValue(z2.Attributes("button")),
                                                                                                                                       .Axis = InferStringValue(z2.Attributes("axis"), "-1"),
                                                                                                                                       .AxisDirection = If(InferStringValue(z2.Attributes("value")) = "-", -1, 1)})
                    }
                    For Each z2 As Xml.XmlNode In y.SelectNodes("descendant::joystick")
                        If Not myJoystickBindings.ContainsKey(z2.Attributes("id").Value) Then
                            myJoystickBindings.Add(z2.Attributes("id").Value, New List(Of MenulatorJoystickAction))
                        End If
                        myJoystickBindings(z2.Attributes("id").Value).Add(New MenulatorJoystickAction() With {.Id = z2.Attributes("id").Value,
                                                                                                                                       .Button = InferStringValue(z2.Attributes("button")),
                                                                                                                                       .Axis = InferStringValue(z2.Attributes("axis"), "-1"),
                                                                                                                                       .AxisDirection = If(InferStringValue(z2.Attributes("value")) = "-", -1, 1),
                                                                                                                                       .KeyboardKey = a.KeyboardKey})
                    Next
                    If myKeybindings.ContainsKey(a.Name) Then
                        myKeybindings(a.Name) = a
                    Else
                        myKeybindings.Add(a.Name, a)
                    End If
                ElseIf y.Name = "remap" Then
                    myJoystickBindings.Remap(y.Attributes("id").Value, y.Attributes("newid").Value)
                End If
            Next
        Next

        file = Nothing


    End Sub
    Private Class Favorites
        'Inherits Hashtable
        Dim keys() As String
        Dim Data()() As String
        Dim strFavXML As String
        'Dim lstFav As List(Of String)
        'Dim file As Xml.XmlDocument
        Public Sub New(strFavXML As String)
            Me.strFavXML = strFavXML
            Load()
        End Sub

        Public Sub Load()

            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectNodes("//menulator/favorites/favorite")
            Erase keys
            Erase Data
            For Each i As Xml.XmlNode In root
                Dim strEmu As String = i.Attributes("emulator").Value
                Dim lookup As Integer = LookupEmulator(strEmu)
                If lookup = -1 Then
                    If keys Is Nothing Then ReDim keys(0) Else ReDim Preserve keys(UBound(keys) + 1)
                    If Data Is Nothing Then ReDim Data(0) : ReDim Data(0)(0) Else ReDim Preserve Data(UBound(Data) + 1) : ReDim Preserve Data(UBound(Data))(0)
                    keys(UBound(keys)) = strEmu
                    lookup = UBound(keys)
                Else
                    ReDim Preserve Data(lookup)(UBound(Data(lookup)) + 1)
                End If
                'me.Add(i.Attributes("emulator").Value, i.Attributes("name").Value)
                Data(lookup)(UBound(Data(lookup))) = i.Attributes("name").Value
            Next
        End Sub
        Private ReadOnly Property LookupEmulator(strEmu As String) As Integer
            Get
                If keys Is Nothing Then Return -1
                For t As Integer = 0 To UBound(keys)
                    If keys(t) = strEmu Then Return t
                Next
                Return -1
            End Get
        End Property
        Public Sub Add(strEmu As String, name As String)
            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectSingleNode("/menulator/favorites[last()]")
            If root Is Nothing Then root = file.SelectSingleNode("/menulator/favorites")
            Dim c = file.CreateElement("favorite")
            c.SetAttribute("emulator", strEmu)
            c.SetAttribute("name", name)
            root.AppendChild(c)
            file.Save(strFavXML)

            'Load(strEmu)
            'MyBase.Add(strEmu, name)

            Dim lookup As Integer = LookupEmulator(strEmu)
            If lookup = -1 Then
                If keys Is Nothing Then ReDim keys(0) Else ReDim Preserve keys(UBound(keys) + 1)
                If Data Is Nothing Then ReDim Data(0) : ReDim Data(0)(0) Else ReDim Preserve Data(UBound(Data) + 1) : ReDim Preserve Data(UBound(Data))(0)
                keys(UBound(keys)) = strEmu
                lookup = UBound(keys)
            Else
                If Data(lookup) Is Nothing Then ReDim Data(lookup)(0) Else ReDim Preserve Data(lookup)(UBound(Data(lookup)) + 1)
            End If
            'me.Add(i.Attributes("emulator").Value, i.Attributes("name").Value)
            Data(lookup)(UBound(Data(lookup))) = name
        End Sub
        Public Function IsFavorite(strEmu As String, name As String) As Boolean
            'Dim file As New Xml.XmlDocument()
            'File.Load(strFavXML)
            'Dim root = file.SelectSingleNode("//menulator/favorites/favorite[@emulator='" & strEmu & "' and @name=" & Chr(34) & name.Replace(Chr(34), "\" & Chr(34)) & Chr(34) & "]")
            'Return root IsNot Nothing
            Dim lookup As Integer = LookupEmulator(strEmu)
            If lookup = -1 Then Return False
            For i As Integer = 0 To UBound(Data(lookup))
                If Data(lookup)(i) = name Then Return True
            Next
            Return False
        End Function
        Public Sub Remove(strEmu As String, name As String)
            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectSingleNode("//menulator/favorites/favorite[@emulator='" & strEmu & "' and @name='" & name & "']")
            If root IsNot Nothing Then
                root.ParentNode.RemoveChild(root)
                file.Save(strFavXML)
            End If

            'Load(strEmu)
            'MyBase.Remove()

            Dim lookup = LookupEmulator(strEmu)
            Dim index As Integer
            For index = 0 To UBound(Data(lookup))
                If Data(lookup)(index) = name Then Exit For
            Next
            For z As Integer = index To UBound(Data(lookup)) - 1
                Data(lookup)(z) = Data(lookup)(z + 1)
            Next
            If UBound(Data(lookup)) - 1 > 0 Then
                ReDim Preserve Data(lookup)(UBound(Data(lookup)) - 1)
            Else
                Erase Data(lookup)
            End If
        End Sub

        Default Public Overloads ReadOnly Property Item(strEmu As String) As String()
            Get
                Dim lookup = LookupEmulator(strEmu)
                If lookup = -1 Then Return Nothing
                Return Data(lookup)
            End Get
        End Property
    End Class

    Private Class MarkedAsBad
        Dim strFavXML As String
        Dim keys As String()
        Dim data()() As String

        Public Sub New(strFavPath As String)
            strFavXML = strFavPath
            Load()
        End Sub
        Sub Load()
            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectSingleNode("//menulator/markedasbad/bad")
            Erase keys
            Erase data
            If root IsNot Nothing Then
                For Each i As Xml.XmlNode In root
                    Dim strEmu As String = i.Attributes("emulator").Value
                    Dim lookup As Integer = LookupEmulator(strEmu)
                    If lookup = -1 Then
                        If keys Is Nothing Then ReDim keys(0) Else ReDim Preserve keys(UBound(keys) + 1)
                        If data Is Nothing Then ReDim data(0) : ReDim data(0)(0) Else ReDim Preserve data(UBound(data) + 1) : ReDim Preserve data(UBound(data))(0)
                        keys(UBound(keys)) = strEmu
                        lookup = UBound(keys)
                    Else
                        ReDim Preserve data(lookup)(UBound(data(lookup)) + 1)
                    End If
                    'me.Add(i.Attributes("emulator").Value, i.Attributes("name").Value)
                    data(lookup)(UBound(data(lookup))) = i.Attributes("name").Value
                Next
            End If
        End Sub
        Private Function LookupEmulator(strEmu As String) As Integer
            If keys Is Nothing Then Return -1
            For t As Integer = 0 To UBound(keys)
                If keys(t) = strEmu Then Return t
            Next
            Return -1
        End Function
        Public Sub Add(strEmu As String, name As String)
            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectSingleNode("/menulator/markedasbad[last()]")
            If root Is Nothing Then root = file.SelectSingleNode("/menulator/markedasbad")
            Dim c = file.CreateElement("bad")
            c.SetAttribute("emulator", strEmu)
            c.SetAttribute("name", name)
            root.AppendChild(c)
            file.Save(strFavXML)

            'Load(strEmu)
            'MyBase.Add(strEmu, name)

            Dim lookup As Integer = LookupEmulator(strEmu)
            If lookup = -1 Then
                If keys Is Nothing Then ReDim keys(0) Else ReDim Preserve keys(UBound(keys) + 1)
                If data Is Nothing Then ReDim data(0) : ReDim data(0)(0) Else ReDim Preserve data(UBound(data) + 1) : ReDim Preserve data(UBound(data))(0)
                keys(UBound(keys)) = strEmu
                lookup = UBound(keys)
            Else
                If data(lookup) Is Nothing Then ReDim data(lookup)(0) Else ReDim Preserve data(lookup)(UBound(data(lookup)) + 1)
            End If
            'me.Add(i.Attributes("emulator").Value, i.Attributes("name").Value)
            data(lookup)(UBound(data(lookup))) = name
        End Sub
        Public Function IsBad(strEmu As String, name As String) As Boolean
            'Dim file As New Xml.XmlDocument()
            'File.Load(strFavXML)
            'Dim root = file.SelectSingleNode("//menulator/favorites/favorite[@emulator='" & strEmu & "' and @name=" & Chr(34) & name.Replace(Chr(34), "\" & Chr(34)) & Chr(34) & "]")
            'Return root IsNot Nothing
            Dim lookup As Integer = LookupEmulator(strEmu)
            If lookup = -1 Then Return False
            For i As Integer = 0 To UBound(data(lookup))
                If data(lookup)(i) = name Then Return True
            Next
            Return False
        End Function
        Public Sub Remove(strEmu As String, name As String)
            Dim file As New Xml.XmlDocument()
            file.Load(strFavXML)
            Dim root = file.SelectSingleNode("//menulator/markedasbad/bad[@emulator='" & strEmu & "' and @name='" & name & "']")
            If root IsNot Nothing Then
                root.ParentNode.RemoveChild(root)
                file.Save(strFavXML)
            End If

            Dim lookup = LookupEmulator(strEmu)
            Dim index As Integer
            For index = 0 To UBound(data(lookup))
                If data(lookup)(index) = name Then Exit For
            Next
            For z As Integer = index To UBound(data(lookup)) - 1
                data(lookup)(z) = data(lookup)(z + 1)
            Next
            If UBound(data(lookup)) - 1 > 0 Then
                ReDim Preserve data(lookup)(UBound(data(lookup)) - 1)
            Else
                Erase data(lookup)
            End If
        End Sub
        Default Public Overloads ReadOnly Property Item(strEmu As String) As String()
            Get
                Dim lookup = LookupEmulator(strEmu)
                If lookup = -1 Then Return Nothing
                Return data(lookup)
            End Get
        End Property
    End Class




    Private Function InferBooleanValue(o As Object, Optional reverse As Boolean = False) As Boolean
        If o Is Nothing Then
            Return If(reverse, Not False, False)

        ElseIf TypeOf o Is String Then

            If o = "0" Then Return (False)
            If o = "1" Then Return (True)
            Return Boolean.Parse(o)
        ElseIf TypeOf o Is Xml.XmlAttribute Then
            Return InferBooleanValue(CStr(o.value), reverse)
        Else

            Return Convert.ToBoolean(o)
        End If
    End Function
    Private Function InferStringValue(o As Object, Optional defaultIfNothing As String = Nothing) As String
        If o Is Nothing Then
            Return defaultIfNothing
        ElseIf TypeOf o Is Xml.XmlAttribute Then
            Return o.value
        Else
            Return CStr(o)
        End If
    End Function


#Region "ROM Acquistion"
    Private Sub ListMAME()
        ListMAME(ListMAMEDefaultClause)
    End Sub
    Private Function ListMAMEDefaultClause() As Func(Of XElement, Boolean)
        Dim bads = MyBad("MAME")
        Return (Function(element As XElement) As Boolean
                    Return Not bads.Contains(element.@<name>)
                End Function)
    End Function
    Private Function ListMAMEDefaultClause(additional As Func(Of XElement, Boolean)) As Func(Of XElement, Boolean)
        Dim bads = MyBad("MAME")
        Return (Function(element As XElement) As Boolean
                    If bads IsNot Nothing AndAlso bads.Contains(element.@<name>) Then Return False
                    Return additional(element)
                End Function)
    End Function
    Private Sub ListMAME(whereClause As Func(Of XElement, Boolean))
        If whereClause Is Nothing Then
            myMame_XML.Reset()
        Else
            myMame_XML.Init(whereClause)
        End If
        If myMame_XML.Count = 0 Then
            PerformMameVerification("No MAME roms found. Start MAME Rom Verification? This process can take 30-60 minutes depending on number of roms.")
            Exit Sub
        End If
        myList.Reset()
        myList.GameList = myMame_XML.GetNextX(myMame_XML.Count)
        myList.ClickCallback = Sub(sender As Object, e As NewUIListItem.NewUIListItemClickedEvent)
                                   LaunchRom()
                                   e.Handled = True
                               End Sub


        For Each i In myFavorites("MAME")
            Dim v = (From z In myList.GameList Where z.Name = i)
            If v.Count Then
                For Each a In v : a.Favorite = True : Next
            End If
        Next



        'Me.Invoke(Sub() Me.Text = FormatNumber(myList.GameList.Count, 0) & " Roms")
        myList.Tag = "MAME"
        myList.SetDefaultGameIcon(ScaleImage(My.Resources.MAME, myList.IconSize.ToSize))
        myList.Index = 0


        myList.AppsMenu = New List(Of NewUIListItem)
        Dim emulators = myEmulatorList("MAME")
        If emulators.Count > 1 Then
            For Each emu In emulators
                myList.AppsMenu.Add(New NewUIListItem(emu.Name, myList.bitBucket("."), Sub()
                                                                                           LaunchRom(emu)
                                                                                       End Sub)
                                                      )
            Next
        End If
        myList.AppsMenu.AddRange({New NewUIListItem("Favorite", My.Resources.favorite, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                           If myFavorites.IsFavorite(myList.Tag, myList.SelectedItem.Name) Then
                                                                                               myList.SelectedItem.Favorite = False
                                                                                               myList.RefreshIndex(myList.Index)
                                                                                               myFavorites.Remove(myList.Tag, myList.SelectedItem.Name)
                                                                                           Else
                                                                                               myList.SelectedItem.Favorite = True
                                                                                               myList.RefreshIndex(myList.Index)
                                                                                               myFavorites.Add(myList.Tag, myList.SelectedItem.Name)
                                                                                           End If
                                                                                           result2.CloseMenu = True
                                                                                       End Sub),
                                                                               New NewUIListItem("Mark as Bad", My.Resources.notifications_powermenu_icon_close, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                                                     If MyBad.IsBad(myList.Tag, myList.SelectedItem.Name) Then
                                                                                                                                                                         MyBad.Remove(myList.Tag, myList.SelectedItem.Name)
                                                                                                                                                                     Else
                                                                                                                                                                         MyBad.Add(myList.Tag, myList.SelectedItem.Name)
                                                                                                                                                                     End If
                                                                                                                                                                     result2.CloseMenu = True
                                                                                                                                                                 End Sub),
                                                                            New NewUIListItem("More Info", My.Resources.icon_wm10_help, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                            If TypeOf myList.SelectedItem Is MAME.MameGame Then
                                                                                                                                                With DirectCast(myList.SelectedItem, MAME.MameGame)
                                                                                                                                                    frmMsg.Msgbox(.XML.ToString, MsgBoxStyle.OkOnly, "MAME Info")

                                                                                                                                                End With
                                                                                                                                            Else
                                                                                                                                                With DirectCast(myList.SelectedItem, IRom)
                                                                                                                                                    frmMsg.Msgbox(.Path, MsgBoxStyle.OkOnly, myList.Tag & " Game Info")
                                                                                                                                                End With
                                                                                                                                            End If
                                                                                                                                            result2.CloseMenu = True
                                                                                                                                        End Sub)
                            })

    End Sub

    Private Sub ListRom(RomClass As Type, strTag As String, strPath As String, defaultIcon As Bitmap, Optional MethodOverride As String = "CreateSearcher", Optional AdditionalSearch As Func(Of IRom, Boolean) = Nothing)
        If RomClass Is Nothing Then
            Throw New TypeLoadException
        End If
        If String.IsNullOrEmpty(MethodOverride) Then MethodOverride = "CreateSearcher"

        pnlLeft.Visible = False
        myList.EnableWait = True
        myList.Tag = strTag
        myList.SetDefaultGameIcon(ScaleImage(defaultIcon, myList.IconSize.ToSize))

        myList.ClickCallback = Sub(sender As Object, e As NewUIListItem.NewUIListItemClickedEvent)
                                   LaunchRom()
                                   e.Handled = True
                               End Sub
        myList.AppsMenu = New List(Of NewUIListItem)
        Dim emulators = myEmulatorList(strTag)
        If emulators.Count > 1 Then
            For Each emu In emulators
                myList.AppsMenu.Add(New NewUIListItem(emu.Name, myList.bitBucket("."), Sub()
                                                                                           LaunchRom(emu)
                                                                                       End Sub)
                                                      )
            Next
        End If
        myList.AppsMenu.AddRange({New NewUIListItem("Favorite", My.Resources.favorite, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                           If myFavorites.IsFavorite(myList.Tag, myList.SelectedItem.Name) Then
                                                                                               myList.SelectedItem.Favorite = False
                                                                                               myList.RefreshIndex(myList.Index)
                                                                                               myFavorites.Remove(myList.Tag, myList.SelectedItem.Name)
                                                                                           Else
                                                                                               myList.SelectedItem.Favorite = True
                                                                                               myList.RefreshIndex(myList.Index)
                                                                                               myFavorites.Add(myList.Tag, myList.SelectedItem.Name)
                                                                                           End If
                                                                                           result2.CloseMenu = True
                                                                                       End Sub),
                                                                               New NewUIListItem("Mark as Bad", My.Resources.notifications_powermenu_icon_close, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                                                     If MyBad.IsBad(myList.Tag, myList.SelectedItem.Name) Then
                                                                                                                                                                         MyBad.Remove(myList.Tag, myList.SelectedItem.Name)
                                                                                                                                                                     Else
                                                                                                                                                                         MyBad.Add(myList.Tag, myList.SelectedItem.Name)
                                                                                                                                                                     End If
                                                                                                                                                                     result2.CloseMenu = True
                                                                                                                                                                 End Sub),
                                                                            New NewUIListItem("More Info", My.Resources.icon_wm10_help, Sub(s2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                            If TypeOf myList.SelectedItem Is MAME.MameGame Then
                                                                                                                                                With DirectCast(myList.SelectedItem, MAME.MameGame)
                                                                                                                                                    frmMsg.Msgbox(.XML.ToString, MsgBoxStyle.OkOnly, "MAME Info")

                                                                                                                                                End With
                                                                                                                                            Else
                                                                                                                                                With DirectCast(myList.SelectedItem, IRom)
                                                                                                                                                    frmMsg.Msgbox(.Path, MsgBoxStyle.OkOnly, myList.Tag & " Game Info")
                                                                                                                                                End With
                                                                                                                                            End If
                                                                                                                                            result2.CloseMenu = True
                                                                                                                                        End Sub)
                            })



        Dim thread As Threading.Thread

        thread = New Threading.Thread(New Threading.ThreadStart(Sub()
                                                                    Dim t As Type = RomClass
                                                                    Dim m = t.GetMethod(MethodOverride, Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public)
                                                                    Dim d = (From z In m.GetParameters.Where(Function(a) a.IsOptional) Select z.DefaultValue)(0)

                                                                    Dim iRom As IEnumerable(Of IRom)
                                                                    iRom = m.Invoke(Nothing, Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.InvokeMethod, Nothing, {strPath, d, AdditionalSearch}, Globalization.CultureInfo.CurrentCulture)
                                                                    'For Each i In GetFavorites(strTag)
                                                                    '    Dim z = (From j In iRom Where j.Name = i)
                                                                    '    For Each x In z : x.Favorite = True : Next

                                                                    'Next
                                                                    Dim fav = myFavorites(strTag) ' GetFavorites(strTag)
                                                                    If fav IsNot Nothing Then
                                                                        iRom = iRom.OrderBy(Function(a)
                                                                                                If fav.Contains(a.Name) Then
                                                                                                    a.Favorite = True
                                                                                                End If

                                                                                                Return a.Description
                                                                                            End Function)
                                                                    End If
                                                                    'iRom = iRom.OrderBy(Of String)(Function(a) a.Description).ToList
                                                                    myList.GameList = iRom.ToList
                                                                    ' myList.GameList = SnesROM.CreateSearcher("D:\shares\games\snes\roms")
                                                                    'Me.Invoke(Sub() Me.Text = FormatNumber(myList.GameList.Count, 0) & " Roms")
                                                                    myList.Index = 0
                                                                    myList.EnableWait = False

                                                                End Sub))
        thread.Start()

    End Sub
    Private Sub ListRom(RomClass As String, strTag As String, strPath As String, defaultIcon As Bitmap, Optional MethodOverride As String = "CreateSearcher", Optional AdditionalSearch As Func(Of IRom, Boolean) = Nothing)
        ListRom(Type.GetType("Menulator_Zero" & "." & RomClass), strTag, strPath, defaultIcon, MethodOverride, AdditionalSearch)

    End Sub
    Private Class iArtist
        Implements IRom
        Dim strPath As String
        Dim strName As String
        Dim strImagePath As String
        Dim bFav As Boolean
        Public Property Name As String Implements IRom.Name
            Get
                Return strName
            End Get
            Set(value As String)
                strName = value
            End Set
        End Property

        Public Property Path As String Implements IRom.Path
            Get
                Return strPath
            End Get
            Set(value As String)
                strPath = value
            End Set
        End Property

        Public Property ImagePath As String Implements IRom.ImagePath
            Get
                Return strImagePath
            End Get
            Set(value As String)
                strImagePath = value
            End Set
        End Property

        Public Property Description As String Implements IRom.Description
            Get
                Return Name
            End Get
            Set(value As String)
                Name = value
            End Set
        End Property

        Public Property Favorite As Boolean Implements IRom.Favorite
            Get
                Return bFav
            End Get
            Set(value As Boolean)
                bFav = value
            End Set
        End Property
    End Class
    Private Class iSong
        Implements IRom
        Dim strPath As String
        Dim strName As String
        Dim strImagePath As String
        Dim bFav As Boolean
        Public Property Name As String Implements IRom.Name
            Get
                Return strName
            End Get
            Set(value As String)
                strName = value
            End Set
        End Property

        Public Property Path As String Implements IRom.Path
            Get
                Return strPath
            End Get
            Set(value As String)
                strPath = value
            End Set
        End Property

        Public Property ImagePath As String Implements IRom.ImagePath
            Get
                Return strImagePath
            End Get
            Set(value As String)
                strImagePath = value
            End Set
        End Property

        Public Property Description As String Implements IRom.Description
            Get
                Return Name
            End Get
            Set(value As String)
                Name = value
            End Set
        End Property

        Public Property Favorite As Boolean Implements IRom.Favorite
            Get
                Return bFav
            End Get
            Set(value As Boolean)
                bFav = value
            End Set
        End Property
    End Class

    Private Class iFileSystemObject
        Implements IRom

        Dim strPath As String
        Dim strName As String
        Dim strImagePath As String
        Public Sub New(strPath As String)
            Me.strPath = strPath
            Try
                Me.strName = FileIO.FileSystem.GetName(strPath)
            Catch ex As Exception
                Me.strName = ex.Message
            End Try
            Me.strImagePath = strPath
        End Sub

        Public Property Name As String Implements IRom.Name
            Get
                Return Description
            End Get
            Set(value As String)
                Description = value
            End Set
        End Property

        Public Property Path As String Implements IRom.Path
            Get
                Return strPath
            End Get
            Set(value As String)
                strPath = value
            End Set
        End Property

        Public Property ImagePath As String Implements IRom.ImagePath
            Get
                Return strImagePath
            End Get
            Set(value As String)
                strImagePath = value
            End Set
        End Property

        Public Property Description As String Implements IRom.Description
            Get
                Return strName
            End Get
            Set(value As String)
                strName = value
            End Set
        End Property

        Public Property Favorite As Boolean Implements IRom.Favorite


    End Class

    Private Async Sub ListDrive(strPath As String, Optional selectIndex As Integer = 0)
        pnlLeft.Visible = False
        myList.EnableWait = True
        myList.Tag = strPath
        Dim az As New Win32SystemIcons(strPath)
        myList.SetDefaultGameIcon(New Bitmap(32, 32))

        Await Task.Run(Sub()
                           Dim folders As String() = Nothing
                           Try
                               folders = IO.Directory.GetDirectories(strPath)
                               myList.GameList = (From a In folders Select New iFileSystemObject(a)).Concat(From a In IO.Directory.GetFiles(strPath) Select New iFileSystemObject(a))
                               myList.AppsMenu = New List(Of NewUIListItem)(New NewUIListItem() {New NewUIListItem("Open", Nothing, Sub(sender As Object, e As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                        Process.Start(myList.SelectedItem.Path)
                                                                                                                                    End Sub),
                                                                                                 New NewUIListItem("Delete", Nothing, Sub()

                                                                                                                                      End Sub),
                                                                                                 New NewUIListItem("Properties", Nothing, Sub()

                                                                                                                                          End Sub)
                                                                            })
                           Catch
                           End Try

                       End Sub)

        myList.Index = selectIndex
        myList.EnableWait = False

    End Sub

#End Region

#Region "Menus and Submenus"
    Private Function NewSubMenu(l As IEnumerable(Of NewUIListItem)) As FlowLayoutPanelEx
        'CloseSubMenu()
        If l Is Nothing Then Return Nothing
        SubMenuDepth += 1
        If subMenus Is Nothing Then
            ReDim subMenus(0)
            ReDim _subMenuIndex(0)
        Else
            ReDim Preserve subMenus(UBound(subMenus) + 1)
            ReDim Preserve _subMenuIndex(UBound(_subMenuIndex) + 1)
        End If
        subMenus(UBound(subMenus)) = New FlowLayoutPanelEx
        With DirectCast(subMenus(UBound(subMenus)), FlowLayoutPanelEx)
            .SuspendLayout()
            If SubMenuDepth > 1 Then pnlLeft.Width += 70

            .Left = 70 * SubMenuDepth '  70
            .Top = 0
            .Width = pnlLeft.Width - (70 * SubMenuDepth)
            .Height = pnlLeft.Height - lblTime.Height - 10 '1396
            For Each a In l
                a.Width = .Width
            Next
            .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom Or AnchorStyles.Right
            .Animation = Menulator_Zero.PanelEx.AnimateWindowFlags.AW_HOR_NEGATIVE
            '.AutoScroll = True
            .AnimateTime = 150
            .BackColor = System.Drawing.SystemColors.ControlLight
            .FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            .Padding = New System.Windows.Forms.Padding(0, 0, 20, 0)
            .Visible = False
            .WrapContents = False
            .Controls.AddRange(l)

            .HorizontalScroll.Maximum = 0
            .AutoScroll = False
            .VerticalScroll.Visible = False
            .AutoScroll = True

            '.Controls.AddRange((From a In myGamesMenu Select a.Value).ToArray)

            pnlLeft.Controls.Add(subMenus(UBound(subMenus)))
            .BringToFront()
            '.Visible = True
            SubMenuIndex(UBound(_subMenuIndex)) = 0
            '.Update()
            AddHandler .VisibleChanged, AddressOf pnlSubMenu_VisibleChanged
            'SubMenuIndex(SubMenuDepth) = 0
            .ResumeLayout()
            .Visible = True
        End With
        Return subMenus(UBound(subMenus))
    End Function
    Private Sub CloseSubMenu()
        If subMenus IsNot Nothing AndAlso SubMenuDepth <> 0 Then

            With DirectCast(subMenus(SubMenuDepth), FlowLayoutPanelEx)
                .Visible = False
                pnlLeft.Controls.Remove(subMenus(SubMenuDepth))
                If Not subMenus(SubMenuDepth).IsDisposed Then subMenus(SubMenuDepth).Controls(_subMenuIndex(SubMenuDepth)).BackColor = subMenus(SubMenuDepth).BackColor
                RemoveHandler .VisibleChanged, AddressOf pnlSubMenu_VisibleChanged
                '.Dispose()

                If SubMenuDepth > 1 Then pnlLeft.Width -= 70
            End With
            ReDim Preserve subMenus(UBound(subMenus) - 1)
            ReDim Preserve _subMenuIndex(UBound(_subMenuIndex) - 1)
        End If
        SubMenuDepth -= 1
        If SubMenuDepth < 0 Then
            pnlLeft.Visible = False
        End If
        If SubMenuDepth <= 0 Then
            SubMenuDepth = 0
            imgMenulator.Visible = True
            imgMenulatorIcon.Visible = False
        End If
        'If SubMenuDepth = -1 Then
        '    SubMenuDepth = 0
        '    Erase subMenus
        '    Erase _subMenuIndex

        'End If

    End Sub
    Private Sub CloseAllSubMenu()
        'If subMenus IsNot Nothing Then
        'SubMenuDepth -= 1
        'If SubMenuDepth = -1 Then
        '    SubMenuDepth = 0
        '    Erase subMenus
        '    Erase _subMenuIndex
        'Else
        pnlLeft.SuspendLayout()
        For z = SubMenuDepth To 1 Step -1
            RemoveHandler subMenus(z).VisibleChanged, AddressOf pnlSubMenu_VisibleChanged
            subMenus(z).Animation = FlowLayoutPanelEx.AnimateWindowFlags.AW_HIDE
            subMenus(z).Visible = False
            If Not subMenus(SubMenuDepth).IsDisposed Then subMenus(z).Controls(_subMenuIndex(z)).BackColor = subMenus(z).BackColor
            pnlLeft.Controls.Remove(subMenus(z))
            'subMenus(z).Dispose()
        Next
        'With DirectCast(subMenus(0), FlowLayoutPanelEx)
        '    .Visible = False
        '    pnlLeft.Controls.Remove(subMenus(0))
        '    RemoveHandler .VisibleChanged, AddressOf pnlSubMenu_VisibleChanged
        '    '.Dispose()
        '    'debugobject = Nothing
        'End With
        pnlLeft.Width = 295

        ReDim Preserve subMenus(0)
        ReDim Preserve _subMenuIndex(0)
        imgMenulator.Visible = True
        imgMenulatorIcon.Visible = False
        SubMenuDepth = 0
        pnlLeft.ResumeLayout()
        pnlLeft.Visible = False
        'End If
        'End If
    End Sub

    'Dim _MenuIndex As Integer = -1
    Dim subMenus() As Menulator_Zero.FlowLayoutPanelEx = Nothing
    Dim _subMenuIndex() As Integer
    Dim SubMenuDepth As Integer = 0
    Dim _subScreenIndex() As Integer

    Public Class JsonHelper
        Public Shared Function FromClass(Of T As Class)(data As T, Optional isEmptyToNull As Boolean = False, Optional jsonSettings As Newtonsoft.Json.JsonSerializerSettings = Nothing) As String

            Dim response As String = String.Empty

            If (Not EqualityComparer(Of T).Default.Equals(data, Activator.CreateInstance(Of T))) Then
                response = Newtonsoft.Json.JsonConvert.SerializeObject(data, jsonSettings)
            End If
            Return IIf(response = "{}", "null", response)
        End Function

        Public Shared Function ToClass(Of T As Class)(data As String, Optional jsonSettings As Newtonsoft.Json.JsonSerializerSettings = Nothing) As T

            Dim response = Activator.CreateInstance(Of T)

            If (Not String.IsNullOrEmpty(data)) Then
                response = IIf(jsonSettings Is Nothing, Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(data), Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(data, jsonSettings))
            End If
            Return response
        End Function
    End Class
    Public Class MainMenuClass
        <Newtonsoft.Json.JsonProperty("id")>
        Public Property Id As String

        <Newtonsoft.Json.JsonProperty("display")>
        Public Property Display As String
        <Newtonsoft.Json.JsonProperty("icon")>
        Public Property Icon As String
        <Newtonsoft.Json.JsonProperty("menu")>
        Public Property Menu As MainMenuClass()
        <Newtonsoft.Json.JsonProperty("altmenu")>
        Public Property AltMenu As MainMenuClass()
        Public Function GetMenuParent(target As MainMenuClass) As MainMenuClass
            For Each a In Menu
                If a Is target Then
                    Return Me
                End If
            Next
            Return GetMenuParent(target)
        End Function

    End Class

#Region "MenuClicks"
    Public Sub MainMenu_pnlGamesClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        NewSubMenu(myGamesMenu.Values.ToArray)

        e.CloseMenu = False
    End Sub

    Public Sub MainMenu_wmpArtistClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        myList.Reset()
        myList.SetDefaultGameIcon(ScaleImage(My.Resources.music_2_64, myList.IconSize.ToSize))

        myList.ClickCallback = Sub(sender3 As Object, e3 As NewUIListItem.NewUIListItemClickedEvent)

                                   Dim artist2 = myWMP.mediaCollection.getByAuthor(myList.SelectedItem.Description)
                                   Dim col2 As New List(Of iSong)
                                   If _subScreenIndex Is Nothing Then ReDim _subScreenIndex(0) Else ReDim Preserve _subScreenIndex(UBound(_subScreenIndex) + 1)
                                   _subScreenIndex(UBound(_subScreenIndex)) = myList.Index

                                   For i As Integer = 0 To artist2.count - 1
                                       col2.Add(New iSong With {.Description = artist2.Item(i).name})
                                   Next
                                   'myList.Reset()
                                   'myList.SetDefaultGameIcon(ScaleImage(My.Resources.music_2_64, myList.IconSize.ToSize))
                                   myList.GameList = col2
                                   myList.Index = 0
                                   myList.BackCallback = Sub() MainMenu_wmpArtistClick(r, e) ' .ClickHandler
                                   myList.ClickCallback = Sub()
                                                              myWMP.currentPlaylist = myWMP.mediaCollection.getByName(myList.SelectedItem.Description)
                                                              myWMP.controls.play()
                                                          End Sub
                                   e.CloseAllMenu = True
                               End Sub

        Dim artist = myWMP.mediaCollection.getAttributeStringCollection("Artist", "Audio")
        Dim col As New List(Of iArtist)
        For i As Integer = 0 To artist.count - 1
            col.Add(New iArtist With {.Description = artist.Item(i)})
        Next
        myList.GameList = col
        If _subScreenIndex IsNot Nothing Then
            myList.Index = _subScreenIndex(UBound(_subScreenIndex))
            If UBound(_subScreenIndex) = 0 Then Erase _subScreenIndex Else ReDim Preserve _subScreenIndex(UBound(_subScreenIndex) - 1)
        Else
            myList.Index = 0
        End If
        e.CloseAllMenu = True
    End Sub
    Public Sub MainMenu_wmpAlbumClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        myList.Reset()
        myList.SetDefaultGameIcon(ScaleImage(My.Resources.music_2_64, myList.IconSize.ToSize))

        Dim artist = myWMP.mediaCollection.getAttributeStringCollection("Album", "Audio")
        Dim col As New List(Of iSong)
        For i As Integer = 0 To artist.count - 1
            col.Add(New iSong With {.Description = artist.Item(i)})
        Next
        myList.GameList = col
        e.CloseAllMenu = True
    End Sub
    Public Sub MainMenu_wmpGenreClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        myList.Reset()
        myList.SetDefaultGameIcon(ScaleImage(My.Resources.icons8_concert_day_64_white, myList.IconSize.ToSize))

        Dim artist = myWMP.mediaCollection.getAttributeStringCollection("WM/Genre", "Audio")
        Dim col As New List(Of iSong)
        For i As Integer = 0 To artist.count - 1
            col.Add(New iSong With {.Description = artist.Item(i)})
        Next
        myList.GameList = col
        e.CloseAllMenu = True
    End Sub
    Public Sub MainMenu_wmpPlaylistsClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        myList.Reset()
        myList.SetDefaultGameIcon(ScaleImage(My.Resources.music_2_64, myList.IconSize.ToSize))

        Dim playlists = myWMP.playlistCollection.getAll
        Dim col As New List(Of iSong)
        For i As Integer = 0 To playlists.count - 1
            col.Add(New iSong With {.Description = playlists.Item(i).name})
        Next
        myList.GameList = col
        e.CloseAllMenu = True
    End Sub
    Public Sub MainMenu_setJoystickRemapClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Dim file As New Xml.XmlDocument()
        Dim strFavXml As String = "Favorites.xml"
        file.Load(strFavXml)
        Dim root = file.SelectNodes("/keybinding/remap")
        If root IsNot Nothing Then
            For Each ar As Xml.XmlNode In root
                file.DocumentElement.Item("keybinding").RemoveChild(ar)
            Next
        End If
        For j As Integer = 0 To Controllers.Count - 1
            If frmMsg.Msgbox("Press any joystick function for Joystick Index " & j, Integer.MaxValue) = MsgBoxResult.Ok Then
                Dim z = DirectCast(frmMsg.JoyTestOutput(1), JoyApi.Joystick.JoyStickChangedArgs)
                myJoystickBindings.Remap(j, z.JoyID)



                Dim c = file.CreateElement("remap")
                c.SetAttribute("id", j)
                c.SetAttribute("newid", z.JoyID)
                file.DocumentElement.Item("keybinding").AppendChild(c)

            Else
                Exit For
            End If
        Next
        file.Save(strFavXml)
    End Sub

    Public Sub MainMenu_pnlFilesClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)

        myList.ClickCallback = Sub(sender As Object, e2 As NewUIListItem.NewUIListItemClickedEvent)
                                   'If TypeOf myList.SelectedItem Is Rom Then
                                   '    With DirectCast(myList.SelectedItem, Rom)
                                   '        If .ClickHandler IsNot Nothing Then
                                   '            InGame = True
                                   '            .ClickHandler.Invoke(myList.SelectedItem, Nothing)


                                   '            e2.Handled = True
                                   '        End If
                                   '    End With
                                   'ElseIf TypeOf myList.SelectedItem Is iFileSystemObject Then
                                   If _subScreenIndex Is Nothing Then ReDim _subScreenIndex(0) Else ReDim Preserve _subScreenIndex(UBound(_subScreenIndex) + 1)
                                   _subScreenIndex(UBound(_subScreenIndex)) = myList.Index
                                   With DirectCast(myList.SelectedItem, iFileSystemObject)
                                       ListDrive(.Path)

                                   End With
                                   'End If
                                   e2.Handled = True
                               End Sub
        myList.BackCallback = Sub(sender As Object, e2 As NewUIListItem.NewUIListItemClickedEvent)
                                  Try
                                      Dim si As Integer = 0
                                      If _subScreenIndex IsNot Nothing Then
                                          si = _subScreenIndex(UBound(_subScreenIndex))
                                          If UBound(_subScreenIndex) = 0 Then Erase _subScreenIndex Else ReDim Preserve _subScreenIndex(UBound(_subScreenIndex) - 1)
                                      End If
                                      ListDrive(FileIO.FileSystem.GetParentPath(myList.Tag), si)
                                  Catch
                                  End Try
                                  e2.Handled = True
                              End Sub

        Dim i As New List(Of NewUIListItem)
        For Each drive In System.IO.DriveInfo.GetDrives()
            Dim a As New Win32SystemIcons(drive.Name)
            i.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub(sender2 As Object, e2 As EventArgs)
                                                                                                                                                                        ListDrive(drive.Name)
                                                                                                                                                                    End Sub})
        Next

        Dim reg = My.Computer.Registry.CurrentUser.OpenSubKey("Network")
        For Each subname In reg.GetSubKeyNames
            Dim data = reg.OpenSubKey(subname)
            Dim a As New Win32SystemIcons(subname & ":\")
            i.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
        Next
        NewSubMenu(i.ToArray)
        '.Visible = True
        'SubMenuIndex(SubMenuDepth) = 0
        e.CloseMenu = False
    End Sub
    Public Sub MainMenu_setEmulatorClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        pnlLeft.Visible = False
        myList.EnableWait = True
        myList.Tag = "Emulator"

        myList.GameList = {New Rom() With {.Description = "Multiple Arcade Machine Emulator", .ImagePath = "MAME"},
                New Rom() With {.Description = "Nintendo Entertainment System", .ImagePath = "nes", .ClickHandler = Sub(s1 As Object, e1 As EventArgs)
                                                                                                                        myList.GameList = {New Rom() With {.Description = "Name"}, New Rom() With {.Description = "Path"}}
                                                                                                                        myList.Index = 0
                                                                                                                    End Sub},
                New Rom() With {.Description = "Super Nintendo Entertainment System", .ImagePath = "snes"},
                New Rom() With {.Description = "Nintendo Gameboy", .ImagePath = "gbx"},
                New Rom() With {.Description = "Nintendo Gameboy Advance", .ImagePath = "gba"},
                New Rom() With {.Description = "Nintendo DS", .ImagePath = "DS"},
                New Rom() With {.Description = "Nintendo 64", .ImagePath = "n64"},
                New Rom() With {.Description = "Nintendo GameCube"},
                New Rom() With {.Description = "Nintendo Wii"},
                New Rom() With {.Description = "Sega Master System", .ImagePath = "sms"},
                New Rom() With {.Description = "Sega Genesis", .ImagePath = "gen"},
                New Rom() With {.Description = "Sega 32x", .ImagePath = "gen32"},
                New Rom() With {.Description = "Sega CD", .ImagePath = "gencd"},
                New Rom() With {.Description = "Sega Saturn", .ImagePath = "saturn"},
                New Rom() With {.Description = "Sega Dreamcast", .ImagePath = "dreamcast"},
                New Rom() With {.Description = "Sega GameGear", .ImagePath = "gg"},
                New Rom() With {.Description = "Atari Jaguar", .ImagePath = "jag"},
                New Rom() With {.Description = "Atari Lynx", .ImagePath = "lynx"},
                New Rom() With {.Description = "Sony Playstation", .ImagePath = "psx"},
                New Rom() With {.Description = "Sony Playstation 2", .ImagePath = "ps2"},
                New Rom() With {.Description = "Sony Playstation 3", .ImagePath = "PS3"},
                New Rom() With {.Description = "Sony PSP", .ImagePath = "PSP"}
            }


        myList.Index = 0
        myList.EnableWait = False
    End Sub
    Public Sub MainMenu_helpWebsiteClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Process.Start("http://www.dogskullsoftware.com")
        e.Handled = True
    End Sub
    Public Sub MainMenu_ApplicationRestartClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Application.Restart()
    End Sub
    Public Sub MainMenu_CloseClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Me.Close()
    End Sub
    Public Sub MainMenu_RestartClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Process.Start("shutdown", "-r -f -t 00")
    End Sub
    Public Sub MainMenu_ShutdownClick(r As MainMenuClass, e As NewUIListItem.NewUIListItemClickedEvent)
        Process.Start("shutdown", "-s -f -t 00")
    End Sub

#End Region

    Private Function _CreateSubMenu(r As MainMenuClass) As IEnumerable(Of NewUIListItem)
        If r Is Nothing Then Return Nothing
        If r.Menu Is Nothing Then Return Nothing
        Return (From m In r.Menu Select New NewUIListItem(m.Display, My.Resources.ResourceManager.GetObject(m.Icon), Sub(sender As Object, e As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                         Dim i = Me.GetType().GetMethod("MainMenu_" & sender.tag.id & "Click")
                                                                                                                         If i IsNot Nothing Then
                                                                                                                             e.Handled = True
                                                                                                                             i.Invoke(Me, {m, e})
                                                                                                                         Else
                                                                                                                             e.CloseAllMenu = False
                                                                                                                             e.CloseMenu = False
                                                                                                                             If Not CreateSubMenu(sender.tag) Then
                                                                                                                                 e.Handled = False
                                                                                                                             End If
                                                                                                                         End If
                                                                                                                     End Sub,
                                                                                                          Sub(sender As Object, e As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                              Dim i = Me.GetType().GetMethod("MainMenu_" & sender.tag.id & "AltClick")
                                                                                                              If i IsNot Nothing Then
                                                                                                                  e.Handled = True
                                                                                                                  i.Invoke(Me, {m, e})
                                                                                                              Else
                                                                                                                  e.CloseAllMenu = False
                                                                                                                  e.CloseMenu = False
                                                                                                                  If Not CreateSubMenu(sender.tag) Then
                                                                                                                      e.Handled = False
                                                                                                                  End If
                                                                                                              End If
                                                                                                          End Sub) With {.Tag = m}).ToArray



    End Function
    Private Function CreateSubMenu(r As MainMenuClass) As Boolean
        Return NewSubMenu(_CreateSubMenu(r)) IsNot Nothing
    End Function

    Private Function CreateMainMenu() As Menulator_Zero.FlowLayoutPanelEx

        Dim myMainMenu = JsonHelper.ToClass(Of MainMenuClass)(My.Resources.MainMenu)

        Dim fp1 As New FlowLayoutPanelEx
        With fp1
            .AnimateTime = 150
            .AutoScroll = True


            .Controls.AddRange(_CreateSubMenu(myMainMenu))

            .Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                   Or System.Windows.Forms.AnchorStyles.Left) _
                   Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            .FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            .Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            .Location = New System.Drawing.Point(0, 100)
            .Name = "FlowLayoutPanel1"
            .Size = New System.Drawing.Size(295, 633)
            .TabIndex = 1
            .Visible = True
            .WrapContents = False

        End With
        Return fp1
    End Function
    Private Property MenuIndex As Integer
        Get
            'Return _MenuIndex
            Return SubMenuIndex(0)
        End Get
        Set(value As Integer)
            'If value < 0 Then value = 0
            'If value > FlowLayoutPanel1.Controls.Count - 1 Then value = FlowLayoutPanel1.Controls.Count - 1
            'If _MenuIndex >= 0 Then FlowLayoutPanel1.Controls(_MenuIndex).BackColor = FlowLayoutPanel1.BackColor
            '_MenuIndex = value
            'FlowLayoutPanel1.Controls(_MenuIndex).BackColor = SystemColors.ActiveBorder : FlowLayoutPanel1.ScrollControlIntoView(FlowLayoutPanel1.Controls(_MenuIndex))
            SubMenuIndex(0) = value
        End Set
    End Property
    'Private ReadOnly Property MainMenu_Current As String
    '    Get
    '        Return subMenus(0).Controls(_subMenuIndex(0)).Name
    '    End Get

    'End Property

    Private Property SubMenuIndex(index As Integer) As Integer
        Get
            If index > UBound(subMenus) Then Return -1
            If index < 0 Then Return -1
            Return _subMenuIndex(index)
        End Get
        Set(value As Integer)
            If index > UBound(subMenus) Then Return
            If value < 0 Then value = 0
            If value > subMenus(index).Controls.Count - 1 Then value = subMenus(index).Controls.Count - 1
            If _subMenuIndex(index) >= 0 AndAlso _subMenuIndex(index) <= subMenus(index).Controls.Count - 1 Then subMenus(index).Controls(_subMenuIndex(index)).BackColor = subMenus(index).BackColor
            _subMenuIndex(index) = value
            If _subMenuIndex(index) >= 0 AndAlso _subMenuIndex(index) <= subMenus(index).Controls.Count - 1 Then subMenus(index).Controls(_subMenuIndex(index)).BackColor = SystemColors.ActiveBorder : subMenus(index).ScrollControlIntoView(subMenus(index).Controls(_subMenuIndex(index)))

        End Set
    End Property
    Private Sub pnlSubMenu_VisibleChanged(sender As Object, e As EventArgs) 'Handles debugobject.VisibleChanged
        If SubMenuDepth > 0 Then
            imgMenulatorIcon.Visible = True
            imgMenulator.Visible = False
        Else
            imgMenulator.Visible = True
            imgMenulatorIcon.Visible = False
        End If
    End Sub

    Private Sub pnlLeft_VisibleChanged(sender As Object, e As EventArgs) Handles pnlLeft.VisibleChanged
        If myList Is Nothing Then Return
        If pnlLeft.Visible Then
            MenuIndex = 0
            'myList.VisualOffset = New Point(pnlLeft.Width / 2, 0)
        Else
            myList.VisualOffset = Point.Empty
        End If
    End Sub


    Private Sub ProcessSubMenuKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case myKeybindings("Action").KeyboardKey
                Dim result As New NewUIListItem.NewUIListItemClickedEvent()
                If SubMenuIndex(SubMenuDepth) >= 0 AndAlso subMenus(SubMenuDepth).Controls.Count Then DirectCast(subMenus(SubMenuDepth).Controls(SubMenuIndex(SubMenuDepth)), NewUIListItem).PerformClick(result)
                e.Handled = result.Handled
                'End Select
                'SubMenuDepth += 1
                'debugobject.Visible = False
                If result.CloseMenu Then
                    If result.CloseAllMenu Then
                        CloseAllSubMenu()
                    Else
                        CloseSubMenu()
                    End If
                    'imgMenulator.Visible = True
                    'imgMenulatorIcon.Visible = False
                End If
                'Case myKeybindings("Down").KeyboardKey  'Keys.Down
                '    SubMenuIndex(SubMenuDepth - 1) += 1
                '    e.Handled = True

            'Case myKeybindings("Up").KeyboardKey
            '    SubMenuIndex(SubMenuDepth - 1) -= 1
            '    e.Handled = True
            Case myKeybindings("AltAction").KeyboardKey
                Dim result As New NewUIListItem.NewUIListItemClickedEvent()
                If SubMenuIndex(SubMenuDepth) >= 0 AndAlso subMenus(SubMenuDepth).Controls.Count Then DirectCast(subMenus(SubMenuDepth).Controls(SubMenuIndex(SubMenuDepth)), NewUIListItem).PerformAltClick(result)
                e.Handled = result.Handled
                If result.CloseMenu Then
                    If result.CloseAllMenu Then
                        CloseAllSubMenu()
                    Else
                        CloseSubMenu()
                    End If
                End If
            Case myKeybindings("Down").KeyboardKey  'Keys.Down
                'If pnlRight.Visible Then
                '    e.Handled = False
                'Else
                If pnlLeft.Visible Then
                    If SubMenuDepth > 0 Then
                        SubMenuIndex(SubMenuDepth) += 1
                    Else
                        MenuIndex += 1
                    End If
                    e.Handled = True
                End If
                'End If
            Case myKeybindings("Up").KeyboardKey
                'If pnlRight.Visible Then
                '    e.Handled = False
                'Else
                If pnlLeft.Visible Then
                    If SubMenuDepth > 0 Then
                        SubMenuIndex(SubMenuDepth) -= 1
                    Else
                        MenuIndex -= 1
                    End If
                    e.Handled = True
                End If
                'End If

            Case myKeybindings("Escape").KeyboardKey
                CloseSubMenu()
                e.Handled = True
        End Select

        e.Handled = True
    End Sub

    'Private Sub ProcessMainMenuKeyDown(sender As Object, e As KeyEventArgs)
    '    Select Case e.KeyCode
    '        Case myKeybindings("Action").KeyboardKey

    '            Select Case MainMenu_Current
    '                'games
    '                'Case "pnlGames"
    '                'With NewSubMenu()
    '                '    For Each a In myGamesMenu.Values
    '                '        a.Width = .Width
    '                '    Next

    '                '    .HorizontalScroll.Maximum = 0
    '                '    .AutoScroll = False
    '                '    .VerticalScroll.Visible = False
    '                '    .AutoScroll = True

    '                '    .Controls.AddRange(myGamesMenu.Values.ToArray)
    '                '    SubMenuIndex(SubMenuDepth) = 0

    '                '    myList.AppsMenu = New List(Of NewUIListItem)({New NewUIListItem("Favorite", My.Resources.favorite, Sub()
    '                '                                                                                                           If myFavorites.IsFavorite(myList.Tag, myList.SelectedItem.Name) Then
    '                '                                                                                                               myList.SelectedItem.Favorite = False
    '                '                                                                                                               myList.RefreshIndex(myList.Index)
    '                '                                                                                                               myFavorites.Remove(myList.Tag, myList.SelectedItem.Name)
    '                '                                                                                                           Else
    '                '                                                                                                               myList.SelectedItem.Favorite = True
    '                '                                                                                                               myList.RefreshIndex(myList.Index)
    '                '                                                                                                               myFavorites.Add(myList.Tag, myList.SelectedItem.Name)
    '                '                                                                                                           End If
    '                '                                                                                                       End Sub),
    '                '                                                              New NewUIListItem("Mark as Bad", My.Resources.notifications_powermenu_icon_close, Sub()
    '                '                                                                                                                                                    If MyBad.IsBad(myList.Tag, myList.SelectedItem.Name) Then
    '                '                                                                                                                                                        MyBad.Remove(myList.Tag, myList.SelectedItem.Name)
    '                '                                                                                                                                                    Else
    '                '                                                                                                                                                        MyBad.Add(myList.Tag, myList.SelectedItem.Name)
    '                '                                                                                                                                                    End If
    '                '                                                                                                                                                End Sub),
    '                '                                                              New NewUIListItem("More Info", My.Resources.icon_wm10_help, Sub()
    '                '                                                                                                                              If TypeOf myList.SelectedItem Is MAME.MameGame Then
    '                '                                                                                                                                  With DirectCast(myList.SelectedItem, MAME.MameGame)
    '                '                                                                                                                                      frmMsg.Msgbox(.XML.ToString, MsgBoxStyle.OkOnly, "MAME Info")

    '                '                                                                                                                                  End With
    '                '                                                                                                                              Else
    '                '                                                                                                                                  With DirectCast(myList.SelectedItem, IRom)
    '                '                                                                                                                                      frmMsg.Msgbox(.Path, MsgBoxStyle.OkOnly, myList.Tag & " Game Info")
    '                '                                                                                                                                  End With
    '                '                                                                                                                              End If
    '                '                                                                                                                              Exit Sub
    '                '                                                                                                                          End Sub)
    '                '                })
    '                '    .Visible = True
    '                'End With

    '                'Case "pnlPower"
    '                '    With NewSubMenu()
    '                '        .Controls.Add(New NewUIListItem() With {.Text = "Close Menulator", .Image = My.Resources.notifications_powermenu_icon_close, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                                                Me.Close()
    '                '                                                                                                                                                                                                                'Application.Restart()
    '                '                                                                                                                                                                                                            End Sub})
    '                '        .Controls.Add(New NewUIListItem() With {.Text = "Restart Menulator", .Image = My.Resources.notifications_powermenu_icon_standby, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                                                    'Me.Close()
    '                '                                                                                                                                                                                                                    Application.Restart()
    '                '                                                                                                                                                                                                                End Sub})
    '                '        '.Controls.Add(New NewUIListItem() With {.Text = "Log off", .Image = My.Resources.notifications_powermenu_icon_logoff, .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
    '                '        '.Controls.Add(New NewUIListItem() With {.Text = "Standby", .Image = My.Resources.notifications_powermenu_icon_standby, .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
    '                '        .Controls.Add(New NewUIListItem() With {.Text = "Restart Computer", .Image = My.Resources.notifications_powermenu_icon_restart, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                                                   Process.Start("shutdown", "-r -f -t 00")
    '                '                                                                                                                                                                                                               End Sub})
    '                '        .Controls.Add(New NewUIListItem() With {.Text = "Shutdown Computer", .Image = My.Resources.notifications_powermenu_icon_shutdown, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                                                     Process.Start("shutdown", "-s -f -t 00")
    '                '                                                                                                                                                                                                                 End Sub})
    '                '        SubMenuIndex(SubMenuDepth) = 0
    '                '        .Visible = True
    '                '    End With
    '                'Case "pnlMusic"
    '                'Case "pnlNetwork"
    '                'Case "pnlApplication"
    '                'Case "pnlFiles"
    '                '    With NewSubMenu()
    '                '        For Each drive In System.IO.DriveInfo.GetDrives()
    '                '            Dim a As New Win32SystemIcons(drive.Name)
    '                '            .Controls.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub(sender2 As Object, e2 As EventArgs)
    '                '                                                                                                                                                                                ListDrive(drive.Name)
    '                '                                                                                                                                                                            End Sub})
    '                '        Next

    '                '        Dim reg = My.Computer.Registry.CurrentUser.OpenSubKey("Network")
    '                '        For Each subname In reg.GetSubKeyNames
    '                '            Dim data = reg.OpenSubKey(subname)
    '                '            Dim a As New Win32SystemIcons(subname & ":\")
    '                '            .Controls.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
    '                '        Next
    '                '        .Visible = True
    '                '        SubMenuIndex(SubMenuDepth) = 0
    '                'End With
    '                'Case "pnlSettings"
    '                'With NewSubMenu()
    '                '    .Controls.Add(New NewUIListItem() With {.Text = "Emulator Settings", .Image = My.Resources.application_settings, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                                ShowEmulatorSettings()
    '                '                                                                                                                                                                                                e.Handled = True
    '                '                                                                                                                                                                                            End Sub})

    '                '    .Controls.Add(New NewUIListItem() With {.Text = "Joystick Remap", .Image = My.Resources.joystick, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                 Dim joys(JoyApi.Joystick.JoyManager.EnumerateDevices.Count - 1) As Guid
    '                '                                                                                                                                                                                 For count = 0 To UBound(joys)
    '                '                                                                                                                                                                                     Dim f As New frmMsg("Press any button on Joystick " & count + 1, Integer.MaxValue, "")
    '                '                                                                                                                                                                                     Select Case f.ShowDialog(Me)

    '                '                                                                                                                                                                                         Case DialogResult.OK
    '                '                                                                                                                                                                                             Dim i = DirectCast(f.JoyTestOutput(0), Menulator_Zero.JoyApi.Joystick).joyIndex
    '                '                                                                                                                                                                                             For Each j In JoyApi.Joystick.JoyManager.EnumerateDevices
    '                '                                                                                                                                                                                                 If j.Value.JoyID = i Then joys(count) = j.Value.GUID : Exit For
    '                '                                                                                                                                                                                             Next
    '                '                                                                                                                                                                                     End Select
    '                '                                                                                                                                                                                     f.Dispose()
    '                '                                                                                                                                                                                 Next
    '                '                                                                                                                                                                                 'Computer\HKEY_USERS\S-1-5-21-3065010685-3923032649-1297513653-1001\System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration
    '                '                                                                                                                                                                                 'For Each subkeys In My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration").GetSubKeyNames
    '                '                                                                                                                                                                                 '    Dim reg As Guid = New Guid(CType(My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration\" & subkeys).GetValue("GUID"), Byte()))
    '                '                                                                                                                                                                                 '    For j As Integer = 0 To UBound(joys)
    '                '                                                                                                                                                                                 '        If joys(j) = reg Then
    '                '                                                                                                                                                                                 '            My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration\" & subkeys).SetValue("Joystick ID", j, Microsoft.Win32.RegistryValueKind.Binary)
    '                '                                                                                                                                                                                 '        End If
    '                '                                                                                                                                                                                 '    Next

    '                '                                                                                                                                                                                 'Next
    '                '                                                                                                                                                                             End Sub})
    '                '    .Visible = True
    '                '    SubMenuIndex(SubMenuDepth) = 0
    '                'End With
    '                'Case "pnlHelp"
    '                '    With NewSubMenu()
    '                '        .Controls.Add(New NewUIListItem() With {.Text = "Website", .Image = My.Resources.icon_wm10_folder, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
    '                '                                                                                                                                                                                      Process.Start("http://www.dogskullsoftware.com")
    '                '                                                                                                                                                                                      e.Handled = True
    '                '                                                                                                                                                                                  End Sub})
    '                '        .Visible = True
    '                '        SubMenuIndex(SubMenuDepth) = 0
    '                '    End With
    '            End Select
    '            ' pnlLeft2_Index(SubMenuDepth - 1) = 0
    '            'debugobject.Refresh()
    '            'debugobject.Visible = True
    '            'e.Handled = True
    '        Case myKeybindings("Escape").KeyboardKey
    '            pnlLeft.Visible = False
    '            e.Handled = True

    '    End Select

    'End Sub
#End Region
    Private Sub ProcessFilterKeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.KeyCode
            Case myKeybindings("AltAction").KeyboardKey, myKeybindings("Start1").KeyboardKey, myKeybindings("Start2").KeyboardKey

                e.Handled = True
            Case myKeybindings("Escape").KeyboardKey
                pnlRight.Visible = False
                e.Handled = True
        End Select
        'e.Handled = True
    End Sub

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean

    End Function

    Private Sub LaunchRom()
        LaunchRom(myList.Tag.ToString)
    End Sub
    Private Sub LaunchRom(strTag As String)
        LaunchRom(myEmulatorList(strTag)(0))
    End Sub
    Private Sub LaunchRom(e As Emulator)
        Me.Cursor = Cursors.WaitCursor
        Dim oldState = Me.WindowState
        CloseAllSubMenu()

        InGame = True
        Me.WindowState = FormWindowState.Minimized
        Dim c As frmMenulator.MenuItemCollection
        With e
            If .Actions IsNot Nothing AndAlso .Actions.Count > 0 Then
                c = New frmMenulator.MenuItemCollection(Keys.Pause)
                For Each i In .Actions
                    c.Add(New frmMenulator.MenuItem(i.Name, i.ImgTag, i.KeyboardKey, i.hideWindow, i.closeWindow))
                Next
            Else
                c = frmMenulator.ConsoleDefaultItemCollection
            End If

            If myList.Tag = "MAME" Then
                MenulatorGameMenu = New frmMenulator(.Path, myList.SelectedItem.Name, frmMenulator.MameDefaultItemCollection, False, False)
            Else

                If Not String.IsNullOrEmpty(.Args) Then
                    MenulatorGameMenu = New frmMenulator(.Path, Chr(34) & myList.SelectedItem.Path & Chr(34), .Args, c)
                Else
                    MenulatorGameMenu = New frmMenulator(.Path, Chr(34) & myList.SelectedItem.Path & Chr(34), c)
                End If
            End If

        End With
        AddHandler MenulatorGameMenu.FormClosed, Sub()
                                                     Me.WindowState = oldState
                                                     Me.Cursor = Cursors.Default
                                                     InGame = False
                                                 End Sub

        MenulatorGameMenu.Show()
    End Sub

    Protected Overrides Function ProcessDialogChar(charCode As Char) As Boolean
        'Return MyBase.ProcessDialogChar(charCode)
        Return False
    End Function
    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        'Return MyBase.ProcessDialogKey(keyData)
        Return False
    End Function

    Private Sub me_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown ', myList.KeyDown

        If InGame Then
            If (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
                'we are in a game and the menu is visible
                MenulatorGameMenu.ProcessKeyDown(sender, e)
                If e.Handled Then
                    e.SuppressKeyPress = True
                    Exit Sub
                End If
            Else
                Exit Sub
            End If
        End If

        If pnlRight.Visible Then
            ProcessFilterKeyDown(sender, e)
            If e.Handled Then
                e.SuppressKeyPress = True
                Exit Sub
            End If
        End If

        'toasty
        Select Case e.KeyCode
            Case myKeybindings("Start1").KeyboardKey

                If Controllers.Count >= 0 AndAlso Controllers(0).lastInfo.IsAxisMin(1, Controllers(0).DeviceCaps) Then
                    myList.Toasty(Sub()
                                      If myList.GameList IsNot Nothing AndAlso myList.GameList.Count Then
                                          myList.Index = New Random().Next(0, myList.GameList.Count - 1)
                                      End If
                                  End Sub)
                End If
            Case myKeybindings("Start2").KeyboardKey
                If Controllers.Count >= 1 AndAlso Controllers(1).lastInfo.IsAxisMin(1, Controllers(1).DeviceCaps) Then
                    myList.Toasty(Sub()
                                      If myList.GameList IsNot Nothing AndAlso myList.GameList.Count Then
                                          myList.Index = New Random().Next(0, myList.GameList.Count - 1)
                                      End If
                                  End Sub)
                End If
            Case Keys.M
                frmMsg.Msgbox("this is a test. But it is a very long test, not that the test long, rather, this string is very long. Note that it does not contain carriage returns!" & vbCrLf & vbCrLf & "Actually i lied" & vbCrLf & "line breaks!" & vbCrLf & vbCrLf & "!", MsgBoxStyle.AbortRetryIgnore Or MsgBoxStyle.Critical, "testing a very long string that shuld not try to wrap")
            Case Keys.R
                frmPopup.DoPopUp("Now Playing", "My Band" & vbCrLf & "This album" & vbCrLf & "1990", My.Resources.audio_file_64)
        End Select

        If pnlLeft.Visible Then
            ProcessSubMenuKeyDown(sender, e)
            If e.Handled Then
                e.SuppressKeyPress = True
                Exit Sub
            End If
        End If

        Select Case e.KeyCode


            Case myKeybindings("Escape").KeyboardKey  ' Keys.Escape

                If myList.InAppsMenu Then

                Else
                    pnlLeft.Visible = True
                End If
                        'Me.Close()
                    'End If
            Case myKeybindings("Down").KeyboardKey  'Keys.Down
                'If pnlRight.Visible Then
                '    e.Handled = False
                'Else
                If pnlLeft.Visible Then
                    If SubMenuDepth > 0 Then
                        SubMenuIndex(SubMenuDepth - 1) += 1
                    Else
                        MenuIndex += 1
                    End If
                    e.Handled = True
                End If
                    'End If
            Case myKeybindings("Up").KeyboardKey
                'If pnlRight.Visible Then
                '    e.Handled = False
                'Else
                If pnlLeft.Visible Then
                    If SubMenuDepth > 0 Then
                        SubMenuIndex(SubMenuDepth - 1) -= 1
                    Else
                        MenuIndex -= 1
                    End If
                    e.Handled = True
                End If
                    'End If
            'Case myKeybindings("Left").KeyboardKey, myKeybindings("Right").KeyboardKey
                    'If pnlLeft.Visible Then

                    '    e.Handled = True
                        'ElseIf pnlRight.Visible Then
                        '    e.Handled = True
                        'End If
            Case myKeybindings("TestSwitch").KeyboardKey
                SetForegroundWindow(Me.Handle)
            Case myKeybindings("Menu").KeyboardKey
                pnlLeft.Visible = Not pnlLeft.Visible

                e.Handled = True

            Case myKeybindings("Search").KeyboardKey

                pnlRight.Visible = True
                txtDescription.Focus()
                'Case Keys.F3
                '    myList.EnableWait = Not myList.EnableWait
                '    e.Handled = True
            Case myKeybindings("Refresh").KeyboardKey
                'myList.RefreshIndex(8)
                OnResize(Nothing)
            Case myKeybindings("MameVerification").KeyboardKey
                PerformMameVerification()

                e.Handled = True
            Case Keys.D0
                frmMsg.Msgbox("This is a pretty long string but there are no line breaks here. Hopefully the size calculated will allow all of this text to fit! But we must add more to ensure that a line wrap will occur or not? THis is a very high resolution monitor! Need to add even more characters in order to cause a line wrap since so many charactrers can fit horizontally. You know, that is probably an issue, should increase font size!")
        End Select
        'e.Handled = True
    End Sub

    Public Sub PerformMameVerification(Optional strMessage As String = "Start MAME Rom Verification? This process can take 30-60 minutes depending on number of roms.")
        'Me.Text = MAME.App.Version("D:\games\emulation\mame\mame64.exe")
        If frmMsg.Msgbox(strMessage, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then

            myList.Dump()
            myMame_XML.Dispose()

            myList.EnableWait = True
            Dim worker As New BackgroundWorker

            myList.EnableWait = True
            AddHandler worker.DoWork, Sub(zsender As Object, eargs As DoWorkEventArgs)
                                          Dim i = MameXml.VerifyRoms(myMame.MamePath)

                                          frmMsg.Msgbox("Verified " & FormatNumber(i, 0,,, TriState.True) & " ROMs")
                                      End Sub
            AddHandler worker.RunWorkerCompleted, Sub(zsender As Object, eargs As RunWorkerCompletedEventArgs)
                                                      myMame_XML = New MameXml()
                                                      Dim z As New NewUIListItem.NewUIListItemClickedEvent()
                                                      myList.EnableWait = False
                                                      myGamesMenu("MAME").PerformClick(z)
                                                  End Sub
            worker.RunWorkerAsync()
        End If
    End Sub

#Region "Filter"
    Private Sub Button3_Click(sender As Object, e As EventArgs)
        pnlRight.Visible = False
    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        pnlRight.Visible = False
        myList.EnableWait = True
        Dim worker As New BackgroundWorker
        AddHandler worker.DoWork, Sub(s2 As Object, o As DoWorkEventArgs)
                                      Dim filterAllowMature As Boolean = o.Argument(0)
                                      Dim filterDescription As String = o.Argument(1)
                                      Dim filterPlayers As String = o.Argument(2)
                                      Dim filterPlayersOp As Integer = o.Argument(3)
                                      Dim filterYear As String = o.Argument(4)
                                      Dim filterYearOp As Integer = o.Argument(5)
                                      Dim filterGenre As String = o.Argument(6)
                                      Dim filterRating As String = o.Argument(7)
                                      Dim filterStatus As String = o.Argument(8)

                                      ListMAME(Function(x As XElement) As Boolean

                                                   If filterAllowMature = False AndAlso x.<genre>.Value.Contains("* Mature *") Then Return False

                                                   If String.IsNullOrEmpty(filterDescription) And String.IsNullOrEmpty(filterPlayers) And String.IsNullOrEmpty(filterYear) And String.IsNullOrEmpty(filterGenre) And String.IsNullOrEmpty(filterRating) Then
                                                       Return True
                                                   End If

                                                   If (Not String.IsNullOrEmpty(filterDescription) AndAlso x.<description>.Value.ToLower Like "*" & filterDescription.ToLower & "*") Then Return True
                                                   If (Not String.IsNullOrEmpty(filterPlayers) AndAlso Choose(filterPlayersOp, {x.<players>.Value = filterPlayers, Val(x.<players>.Value) >= Val(filterPlayers), Val(x.<players>.Value) <= Val(filterPlayers)})) Then Return True
                                                   If (Not String.IsNullOrEmpty(filterYear) AndAlso Choose(filterYearOp, {x.<year>.Value = filterYear, Val(x.<year>.Value) >= Val(filterYear), Val(x.<year>.Value) <= Val(filterYear)})) Then Return True
                                                   If (Not String.IsNullOrEmpty(filterGenre) AndAlso x.<genre>.Value.ToLower Like "*" & filterGenre.ToLower & "*") Then Return True
                                                   If (Not String.IsNullOrEmpty(filterRating) AndAlso x.<rating>.Value.ToLower Like "*" & filterRating.ToLower & "*") Then Return True
                                                   If (Not String.IsNullOrEmpty(filterStatus) AndAlso x.<status>.Value = filterStatus) Then Return True
                                                   Return False
                                               End Function)


                                  End Sub

        AddHandler worker.RunWorkerCompleted, Sub()
                                                  myList.EnableWait = False
                                                  myList.Index = 0
                                              End Sub
        worker.RunWorkerAsync(New Object() {chkMature.Checked, txtDescription.Text, txtPlayers.Text, cboPlayersOp.SelectedIndex, txtYear.Text, cboYearOp.SelectedIndex, txtGenre.Text, txtRating.Text, cboStatus.Text})
    End Sub
    Private Sub btnFilterClear_Click(sender As Object, e As EventArgs) Handles btnFilterClear.Click
        txtDescription.Clear()
        cboPlayersOp.SelectedIndex = -1
        txtPlayers.Clear()
        cboYearOp.SelectedIndex = -1
        txtYear.Clear()
        txtGenre.Clear()
        txtRating.Clear()
        chkMature.Checked = False
        txtDescription.Focus()
    End Sub
    'Private Sub txtDescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDescription.KeyDown, txtGenre.KeyDown, txtPlayers.KeyDown, txtRating.KeyDown, txtYear.KeyDown, cboPlayersOp.KeyDown, cboStatus.KeyDown, cboYearOp.KeyDown, chkMature.KeyDown
    '    Select Case e.KeyCode
    '        Case Keys.Up
    '            If TypeOf sender Is ComboBox Then
    '                With DirectCast(sender, ComboBox)
    '                    If .DroppedDown Then
    '                        Return
    '                    End If
    '                End With
    '            End If
    '            SendKeys.Send("+{TAB}")
    '            e.Handled = True

    '        Case Keys.Down
    '            If TypeOf sender Is ComboBox Then
    '                With DirectCast(sender, ComboBox)
    '                    If .DroppedDown Then
    '                        Return
    '                    End If
    '                End With
    '            End If
    '            SendKeys.Send("{TAB}")
    '            e.Handled = True
    '        Case Keys.Return
    '            If TypeOf sender Is ComboBox Then
    '                With DirectCast(sender, ComboBox)
    '                    If .DroppedDown Then
    '                        .DroppedDown = False
    '                    Else
    '                        .DroppedDown = True
    '                    End If
    '                End With
    '                e.Handled = True
    '            ElseIf TypeOf sender Is CheckBox Then
    '                DirectCast(sender, CheckBox).Checked = Not sender.checked
    '            End If
    '    End Select
    'End Sub
    Private Sub pnlMameFilter_VisibilityChanged(sender As Object, e As EventArgs) Handles pnlRight.VisibleChanged
        If pnlRight.Visible Then
            pnlRight.Focus()
        Else
            If myList IsNot Nothing Then myList.Focus()
        End If
    End Sub
#End Region

#Region "myList"
    Private Sub myList_NeedInvalidate() Handles myList.NeedInvalidate
        '   OnResize(Nothing)
    End Sub

    Dim tokensource2 As Threading.CancellationTokenSource
    Dim ct As Threading.CancellationToken '= tokensource2.Token

    Private Sub myList_RangeChanged(sender As Object, e As MenulatorListView.GameRangeArgs) Handles myList.IndexRangechanged

        If e.StartIndex + e.EndIndex <= 0 Then Return

        Try
            'worker.RunWorkerAsync(e.StartIndex & "|" & e.EndIndex)
            'Dim t As New Threading.ParameterizedThreadStart(AddressOf DoWork)
            't.DynamicInvoke(e)

            If tokensource2 IsNot Nothing Then
                tokensource2.Cancel()
            End If
            tokensource2 = New Threading.CancellationTokenSource
            ct = tokensource2.Token
            Task.Factory.StartNew(AddressOf DoImageAcquistion, e, tokensource2.Token)
            'System.Threading.ThreadPool.QueueUserWorkItem(AddressOf DoImageAcquistion, e)

        Catch
        End Try
    End Sub
    'Shared UriQueue As New Concurrent.ConcurrentQueue(Of String)
    'Public Shared Function ResizeKeepAspect(src As Size, maxWidth As Integer, maxHeight As Integer) As SizeF

    '    Dim Rnd As Single = Math.Min(maxWidth / src.Width, maxHeight / src.Height)
    '    Return New SizeF(Math.Round(src.Width * Rnd), Math.Round(src.Height * Rnd))
    'End Function
    Public Shared Function ResizeKeepAspect(CurrentDimensions As Size, maxWidth As Integer, maxHeight As Integer) As SizeF



        Dim newHeight As Single = CurrentDimensions.Height
        Dim newWidth As Single = CurrentDimensions.Width
        If (maxWidth > 0 AndAlso newWidth > maxWidth) Then ' WidthResize
            Dim divider As Single = Math.Abs(newWidth / maxWidth)
            newWidth = maxWidth
            newHeight = (Math.Round((newHeight / divider)))
        End If
        If (maxHeight > 0 AndAlso newHeight > maxHeight) Then 'HeightResize
            Dim divider As Single = Math.Abs(newHeight / maxHeight)
            newHeight = maxHeight
            newWidth = (Math.Round((newWidth / divider)))
        End If
        If newWidth < maxWidth And newHeight < maxHeight Then
            newWidth = maxWidth
            newHeight = maxHeight
        End If
        Return New SizeF(newWidth, newHeight)
    End Function
    Public Shared Function ResizeKeepAspect(CurrentDimensions As Size, maxSize As SizeF) As SizeF
        Return ResizeKeepAspect(CurrentDimensions, maxSize.Width, maxSize.Height)
    End Function


    Private Sub myList_ScaleImageRoutine(key As String, src As Image)
        Dim iconsize = myList.IconSize
        Dim nsize = ResizeKeepAspect(src.Size, iconsize)
        Using b2 As New Bitmap(iconsize.Width, iconsize.Height, Imaging.PixelFormat.Format32bppArgb)
            Using g = Drawing.Graphics.FromImage(b2)
                myList.bitBucket.UpdateState(key, MenulatorListView.BitBucketState.Loading)
                g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                g.InterpolationMode = Drawing2D.InterpolationMode.Bicubic
                g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality



                g.DrawImage(src, New RectangleF(Math.Abs((nsize.Width / 2) - (iconsize.Width / 2)),
                                                  Math.Abs((nsize.Height / 2) - (iconsize.Height / 2)), nsize.Width, nsize.Height))
            End Using
            myList.bitBucket.TryAdd(key, b2.Clone)
            myList.bitBucket.UpdateState(key, MenulatorListView.BitBucketState.Ready)
        End Using

    End Sub

    Private Sub myList_ScaleImageRoutine(filePath As String)
        Try
            Using b = Image.FromFile(filePath)
                Dim iconsize = myList.IconSize
                Dim nsize = ResizeKeepAspect(b.Size, iconsize)
                Using b2 As New Bitmap(iconsize.Width, iconsize.Height, Imaging.PixelFormat.Format32bppArgb)
                    myList.bitBucket.UpdateState(filePath, MenulatorListView.BitBucketState.Loading)
                    Using g = Drawing.Graphics.FromImage(b2)
                        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                        g.CompositingMode = Drawing2D.CompositingMode.SourceOver
                        g.InterpolationMode = Drawing2D.InterpolationMode.Bicubic
                        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        g.DrawImage(b, New RectangleF(Math.Abs((nsize.Width / 2) - (iconsize.Width / 2)),
                                                          Math.Abs((nsize.Height / 2) - (iconsize.Height / 2)), nsize.Width, nsize.Height))
                    End Using
                    myList.bitBucket.TryAdd(filePath, b2.Clone)
                    myList.bitBucket.UpdateState(filePath, MenulatorListView.BitBucketState.Ready)
                End Using

            End Using
        Catch
            IO.File.Delete(filePath)
        End Try
    End Sub

    Private Sub DoImageAcquistion(e As MenulatorListView.GameRangeArgs)
        Try
            ct.ThrowIfCancellationRequested()
            If myList.GameList Is Nothing Then Return
            If myList.GameList.Count = 0 Then Return
            Dim startIndex As Integer, endIndex As Integer

            startIndex = e.StartIndex
            endIndex = e.EndIndex
            For i As Integer = startIndex To endIndex
                ct.ThrowIfCancellationRequested()

                If TypeOf myList.GameList(i) Is MameGame Then
                    With DirectCast(myList.GameList(i), MameGame)
                        If .ImagePath Is Nothing Then

                            Dim v = MAME.Snap.FoundSnaps(myMame.MamePath, snapdir, .Name)
                            If v.Count Then
                                .ImagePath = v(0)

                                myList_ScaleImageRoutine(v(0))
                                myList.RefreshIndex(i)

                            ElseIf My.Computer.Network.IsAvailable Then
                                v = {MAME.Snap.GetAddress(Snap.WebSnaps.Paradise_Title, .Name)}
                                If v IsNot Nothing Then
                                    ' .ImagePath = v(0)
                                    'Debug.Print(v(0))
                                    'UriQueue.Enqueue(v(0) & "|" & .Name & "|" & i)
                                    'If Not Directory.Exists(snapdir(0)) Then Directory.CreateDirectory(snapdir(0))
                                    Dim path = System.IO.Path.Combine(snapdir(0), System.IO.Path.GetFileName(v(0)))
                                    myList.bitBucket.UpdateState(path, MenulatorListView.BitBucketState.Loading)
                                    If DownloadRemoteImageFile(v(0), path) Then
                                        myList_ScaleImageRoutine(path)
                                        .ImagePath = path
                                        myList.RefreshIndex(i)
                                    Else
                                        ' myList.bitBucket.TryAdd(path, Nothing)
                                        myList.bitBucket.UpdateState(path, MenulatorListView.BitBucketState.Searching)
                                        .ImagePath = ""
                                    End If
                                Else
                                    .ImagePath = ""
                                End If
                            End If
                        End If
                    End With
                ElseIf TypeOf myList.GameList(i) Is DSRom Then
                    With DirectCast(myList.GameList(i), DSRom)
                        If .ImagePath Is Nothing Then
                            Try
                                myList_ScaleImageRoutine(.Path, .EmbeddedImage)
                                .ImagePath = .Path
                                myList.RefreshIndex(i)
                            Catch
                                .ImagePath = "."
                            End Try
                        End If

                    End With
                ElseIf TypeOf myList.GameList(i) Is Rom AndAlso myList.GameList(i).ImagePath IsNot Nothing Then
                    If Not myList.bitBucket.ContainsKey(myList.GameList(i).ImagePath) Then
                        Try
                            myList_ScaleImageRoutine(myList.GameList(i).ImagePath, My.Resources.ResourceManager.GetObject(myList.GameList(i).ImagePath))
                            myList.RefreshIndex(i)
                        Catch
                            myList.GameList(i).ImagePath = "."
                        End Try
                    End If
                ElseIf TypeOf myList.GameList(i) Is iFileSystemObject Then
                    ' Try
                    If Not myList.bitBucket.ContainsKey(myList.GameList(i).ImagePath) Then
                        If IO.Directory.Exists(myList.GameList(i).ImagePath) OrElse IO.File.Exists(myList.GameList(i).ImagePath) Then
                            Dim img As New Win32SystemIcons(myList.GameList(i).ImagePath)
                            myList_ScaleImageRoutine(myList.GameList(i).ImagePath, img.GetIcon(True, myList.IconSize.Width, myList.IconSize.Height))
                            myList.RefreshIndex(i)

                        End If
                    End If
                ElseIf TypeOf myList.GameList(i) Is iSong Then
                    If String.IsNullOrEmpty(myList.GameList(i).ImagePath) OrElse Not myList.bitBucket.ContainsKey(myList.GameList(i).ImagePath) Then
                        Try
                            Dim album As String = Nothing
                            If Not String.IsNullOrEmpty(myList.GameList(i).Description) Then
                                Dim playlist = myWMP.mediaCollection.getByName(myList.GameList(i).Description)
                                For plist As Integer = 0 To playlist.count - 1
                                    album = playlist.Item(plist).getItemInfo("Album")
                                    If Not String.IsNullOrEmpty(album) Then
                                        'Dim Media As WMPLib.IWMPMedia = myWMP.Player.mediaCollection.getByAttribute("Title", album).get_Item(0)
                                        Dim imagePath As String = Path.Combine(Path.GetDirectoryName(playlist.Item(plist).sourceURL), String.Format("AlbumArt_{0}_Large.jpg", playlist.Item(plist).getItemInfo("WM/WMCollectionID")))
                                        If (IO.File.Exists(imagePath)) Then
                                            myList.GameList(i).ImagePath = imagePath
                                            If Not myList.bitBucket.ContainsKey(imagePath) Then
                                                myList_ScaleImageRoutine(imagePath, New Bitmap(imagePath))
                                            End If
                                            myList.RefreshIndex(i)
                                            Exit Try
                                        End If
                                    End If
                                Next

                            End If
                            If String.IsNullOrEmpty(album) Then
                                myList.GameList(i).ImagePath = "."
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                ElseIf TypeOf myList.GameList(i) Is iArtist Then
                    If String.IsNullOrEmpty(myList.GameList(i).ImagePath) OrElse Not myList.bitBucket.ContainsKey(myList.GameList(i).ImagePath) Then
                        Try
                            Dim album As String = Nothing
                            If Not String.IsNullOrEmpty(myList.GameList(i).Description) Then
                                Dim playlist = myWMP.mediaCollection.getByAuthor(myList.GameList(i).Description)
                                For plist As Integer = 0 To playlist.count - 1
                                    album = playlist.Item(plist).getItemInfo("Album")
                                    If Not String.IsNullOrEmpty(album) Then
                                        'Dim Media As WMPLib.IWMPMedia = myWMP.Player.mediaCollection.getByAttribute("Title", album).get_Item(0)
                                        Dim imagePath As String = Path.Combine(Path.GetDirectoryName(playlist.Item(plist).sourceURL), String.Format("AlbumArt_{0}_Large.jpg", playlist.Item(plist).getItemInfo("WM/WMCollectionID")))
                                        If (IO.File.Exists(imagePath)) Then
                                            myList.GameList(i).ImagePath = imagePath
                                            myList_ScaleImageRoutine(imagePath, New Bitmap(imagePath))
                                            myList.RefreshIndex(i)
                                            Exit Try
                                        End If
                                    End If
                                Next

                            End If
                            If String.IsNullOrEmpty(album) Then
                                myList.GameList(i).ImagePath = "."
                            End If
                        Catch ex As Exception
                        End Try
                    End If

                ElseIf i < myList.GameList.Count AndAlso myList.GameList(i).ImagePath IsNot Nothing Then
                    Try
                        If Not myList.bitBucket.ContainsKey(myList.GameList(i).ImagePath) Then
                            myList_ScaleImageRoutine(myList.GameList(i).ImagePath, New Bitmap(myList.GameList(i).ImagePath))
                            myList.RefreshIndex(i)
                        End If
                    Catch
                        myList.GameList(i).ImagePath = "."
                    End Try
                End If
            Next
        Catch
        End Try
    End Sub

    Private Shared ReadOnly Property WMPImagePath(media As WMPLib.IWMPMedia) As String
        Get
            Dim s = Path.Combine(Path.GetDirectoryName(media.sourceURL), String.Format("AlbumArt_{0}_Large.jpg", media.getItemInfo("WM/WMCollectionID")))
            If IO.File.Exists(s) Then
                Return s
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private Shared Function DownloadRemoteImageFile(uri As String, fileName As String) As Boolean

        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(uri), HttpWebRequest)
        Dim response As HttpWebResponse
        Try
            response = DirectCast(request.GetResponse(), HttpWebResponse)
        Catch
            Return False
        End Try

        '// Check that the remote file was found. The ContentType
        '// check Is performed since a request for a non-existent
        '// image file might be redirected to a 404-page, which would
        '// yield the StatusCode "OK", even though the image was Not
        '// found.
        If ((response.StatusCode = HttpStatusCode.OK OrElse
    response.StatusCode = HttpStatusCode.Moved OrElse
    response.StatusCode = HttpStatusCode.Redirect) AndAlso
    response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) Then


            '// if the remote file was found, download oit
            Try
                Using inputStream As Stream = response.GetResponseStream()

                    Using outputStream As Stream = File.OpenWrite(fileName)

                        Dim buffer(4096) As Byte
                        Dim bytesRead As Integer
                        Do

                            bytesRead = inputStream.Read(buffer, 0, buffer.Length)
                            outputStream.Write(buffer, 0, bytesRead)
                        Loop While bytesRead <> 0
                    End Using
                End Using
            Catch
                Return False
            End Try
            Return True
        Else
            Return False
        End If
    End Function


#End Region



#Region "Joystick"
    Dim needJoyId As Boolean = False
    Dim NeedJoyIDCallback As EventHandler
    'Dim JoyID = 1
    'Dim WithEvents pad As XInput.XInputWrapper.XboxController = XInput.XInputWrapper.XboxController.RetrieveController(JoyID)

    'Private Declare Function SystemParametersInfo Lib "user32" Alias "SystemParametersInfoA" (ByVal uAction As Integer, ByVal uParam As Integer, ByRef lpvParam As Integer, ByVal fuWinIni As Integer) As Integer

    'Dim KeyboardDelay As Integer, KeyboardRepeatSpeed As Integer

    '<StructLayout(LayoutKind.Sequential)>
    'Public Structure JOYINFOEX
    '    Public dwSize As Integer
    '    Public dwFlags As Integer
    '    Public dwXpos As Integer
    '    Public dwYpos As Integer
    '    Public dwZpos As Integer
    '    Public dwRpos As Integer
    '    Public dwUpos As Integer
    '    Public dwVpos As Integer
    '    Public dwButtons As Integer
    '    Public dwButtonNumber As Integer
    '    Public dwPOV As Integer
    '    Public dwReserved1 As Integer
    '    Public dwReserved2 As Integer


    'End Structure


    '<ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")>
    'Class UIHostNoLaunch

    'End Class

    '<ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b"),
    'InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    'Interface ITipInvocation

    '    Sub Toggle(hwnd As IntPtr)
    'End Interface

    '<DllImport("user32.dll", SetLastError:=False)>
    'Private Shared Function GetDesktopWindow() As IntPtr

    'End Function
    'Private Sub pad_StateChanged(sender As Object, e As XboxControllerStateChangedEventArgs) Handles pad.StateChanged
    '    Static dwYpos(1) As DateTime, dwXpos(1) As DateTime, dwButton(11) As DateTime


    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT) Then
    '        If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
    '                dwXpos(0) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_left) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
    '        dwXpos(0) = Now
    '    End If
    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT) Then
    '        If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
    '                dwXpos(1) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_right) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
    '        dwXpos(1) = Now
    '    End If

    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_UP) Then
    '        If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)
    '                dwYpos(0) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_UP) Then
    '        'SendKeys.SendWait("{UP}")
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)
    '        dwYpos(0) = Now
    '    End If
    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN) Then
    '        If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
    '                dwYpos(1) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_DPAD_down) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
    '        dwYpos(1) = Now
    '    End If


    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_A) Then
    '        If (Now - dwButton(0)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwButton(0)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                'WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.Return)
    '                dwButton(0) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_A) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.Return)
    '        dwButton(0) = Now
    '    End If

    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_B) Then
    '        If (Now - dwButton(1)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwButton(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
    '                dwButton(1) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_b) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
    '        dwButton(1) = Now
    '    End If

    '    If e.PreviousInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_START) Then
    '        If (Now - dwButton(8)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwButton(8)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
    '                dwButton(8) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.Gamepad.IsButtonPressed(XInput.XInputWrapper.ButtonFlags.XINPUT_GAMEPAD_START) Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
    '        dwButton(8) = Now
    '    End If


    '    If e.PreviousInputState.PacketNumber = -255 Then
    '        'home
    '        If (Now - dwButton(8)).TotalMilliseconds >= KeyboardDelay Then
    '            If (Now - dwButton(9)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
    '                dwButton(8) = Now
    '            End If
    '        End If
    '    ElseIf e.CurrentInputState.PacketNumber = -255 Then
    '        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
    '        dwButton(8) = Now
    '    End If
    'End Sub

    'Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
    '    'XInput.XInputWrapper.XboxController.StopPolling()
    'End Sub

    'Dim state As JOYINFOEX
    'Dim stateIsEmpty As Boolean = True
    'Const JOYERR_BASE = 160
    'Const JOYERR_UNPLUGGED = JOYERR_BASE + 7
    'Private Declare Function joyGetPosEx Lib "winmm.dll" (ByVal uJoyID As Integer, ByRef pji As JOYINFOEX) As Integer
    '<DllImport("xinput1_3.dll", EntryPoint:="#100")>
    'Private Shared Function secret_get_gamepad(playerIndex As Integer, <Out> ByRef struc As XINPUT_GAMEPAD_SECRET) As Integer
    'End Function
    'Public Structure XINPUT_GAMEPAD_SECRET
    '    Public eventCount As UInt32
    '    Public wButtons As UShort
    '    Public bLeftTrigger As Byte
    '    Public bRightTrigger As Byte
    '    Public sThumbLX As Short
    '    Public sThumbLY As Short
    '    Public sThumbRX As Short
    '    Public sThumbRY As Short
    'End Structure

    'Dim xgs As XINPUT_GAMEPAD_SECRET

    'Function testHomeButton() As Boolean

    '    Dim stat As Integer
    '    Dim value As Boolean


    '    For i = 4 To 4
    '        stat = secret_get_gamepad(i, xgs)
    '        If (stat <> 0) Then Continue For
    '        value = value Or ((xgs.wButtons And &H400) <> 0)
    '    Next
    '    If (value) Then Return True
    '    Return False
    'End Function


    'Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    '    Static dwYpos(1) As DateTime, dwXpos(1) As DateTime
    '    Dim newState As JOYINFOEX
    '    newState.dwSize = 64
    '    newState.dwFlags = &HFF

    '    'If JOYERR_UNPLUGGED Then

    '    joyGetPosEx(JoyID, newState)
    '    stateIsEmpty = True
    '    'End If
    '    'If testHomeButton() Then
    '    '    pad_StateChanged(sender, New XboxControllerStateChangedEventArgs() With {.CurrentInputState = New XInputState() With {.Gamepad = New XInput.XInputWrapper.XInputGamepad() With {.wButtons = &H400}, .PacketNumber = -255}})
    '    'End If

    '    If Not stateIsEmpty Then
    '        If state.dwYpos <> newState.dwYpos Then
    '            Select Case newState.dwYpos
    '                Case 32255
    '                    dwYpos(0) = Nothing
    '                    dwYpos(1) = Nothing
    '                    'a release, where did we come from?
    '                    If state.dwYpos = 0 Then
    '                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
    '                        Debug.Print("Release UP")
    '                    Else
    '                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
    '                        Debug.Print("Release DOWN")
    '                    End If
    '                    '32255 '65535
    '                Case 0
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
    '                    Debug.Print("Press UP")
    '                    dwYpos(0) = Now
    '                Case 65535
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
    '                    Debug.Print("Press DOWN")
    '                    dwYpos(0) = Now
    '            End Select
    '        ElseIf state.dwYpos = 0 Then
    '            'holding up
    '            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '                If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
    '                    dwYpos(1) = Now
    '                End If
    '            End If
    '        ElseIf state.dwYpos = 65535 Then
    '            'holding down
    '            If (Now - dwYpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '                If (Now - dwYpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
    '                    dwYpos(1) = Now
    '                End If
    '            End If
    '        End If

    '        If state.dwXpos <> newState.dwXpos Then
    '            Select Case newState.dwXpos
    '                Case 32255
    '                    dwXpos(0) = Nothing
    '                    dwXpos(1) = Nothing
    '                    'a release, where did we come from?
    '                    If state.dwXpos = 0 Then
    '                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
    '                    Else
    '                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
    '                    End If
    '                    '32255 '65535
    '                Case 0
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
    '                    dwXpos(0) = Now
    '                Case 65535
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
    '                    dwXpos(0) = Now
    '            End Select
    '        ElseIf state.dwXpos = 0 Then
    '            If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '                If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
    '                    dwXpos(1) = Now
    '                End If
    '            End If
    '        ElseIf state.dwXpos = 65535 Then
    '            If (Now - dwXpos(0)).TotalMilliseconds >= KeyboardDelay Then
    '                If (Now - dwXpos(1)).TotalMilliseconds >= KeyboardRepeatSpeed Then
    '                    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
    '                    dwXpos(1) = Now
    '                End If
    '            End If
    '        End If

    '        If state.dwButtons <> newState.dwButtons Then
    '            If (newState.dwButtons And 1) = 1 Then
    '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.Return)
    '            ElseIf (state.dwButtonNumber And 1) = 1 Then
    '                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.Return)
    '            End If
    '        End If
    '        state = newState
    '        stateIsEmpty = False
    '    End If
    'End Sub
    Private Sub Form1_JoyStickPOVDown(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs) Handles Me.JoyStickPOVDown
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            With e.RawJoyInfo
                Select Case .dwPOV
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD_AND_LEFT
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT_AND_FORWARD
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD_AND_RIGHT
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT_AND_BACKWARD
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
                End Select
            End With
        End If
    End Sub
    Private Sub Form1_JoyStickPOVUp(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs) Handles Me.JoyStickPOVUp
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            With e.RawJoyInfo
                Select Case .dwPOV
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD_AND_LEFT
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT_AND_FORWARD
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD_AND_RIGHT
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT_AND_BACKWARD
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD
                        WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
                End Select
            End With
        End If
    End Sub
    Private Sub Form1_JoyStickPOVPress(sender As Object, e As JoyApi.Joystick.JoyStickButtonPressEventArgs) Handles Me.JoyStickPOVPress
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            With e.RawJoyInfo
                Select Case .dwPOV
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD_AND_LEFT
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVLEFT_AND_FORWARD
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD_AND_RIGHT
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVRIGHT_AND_BACKWARD
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVFORWARD
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)

                    Case JoyApi.Joystick.NativeMethods.JoyPOVDirections.JOY_POVBACKWARD
                        WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
                End Select
            End With
        End If
    End Sub



    Private Sub Form1_JoyStickAxisDown(sender As Object, e As JoyApi.Joystick.JoyStickAxisEventArgs) Handles Me.JoyStickAxisDown

        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub
            'If sender.joyindex = 2 Then
            '    Select Case e.AxisID
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.SPACE)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
            '            End If
            '    End Select
            'Else
            'Select Case e.AxisID
            '        Case 0
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
            '            End If
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
            '            End If

            '    End Select
            If myJoystickBindings.ContainsKey(sender.joyindex) Then
                For Each axis In (From a In myJoystickBindings(sender.joyindex) Where a.Axis = e.AxisID)
                    If (e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) And axis.AxisDirection < 0) Then
                        WindowsInput.InputSimulator.SimulateKeyDown(axis.KeyboardKey)
                    ElseIf (e.RawJoyInfo.IsAxisMax(e.AxisID, Controllers(sender.joyindex).DeviceCaps) And axis.AxisDirection > 0) Then
                        WindowsInput.InputSimulator.SimulateKeyDown(axis.KeyboardKey)

                    End If
                Next
            End If
            'End If
        End If
    End Sub

    Private Sub Form1_JoyStickAxisUp(sender As Object, e As JoyApi.Joystick.JoyStickAxisEventArgs) Handles Me.JoyStickAxisUp
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub

            'If sender.joyindex = 2 Then
            '    Select Case e.AxisID
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                '  WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.SPACE)
            '            Else
            '                ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
            '            End If
            '    End Select
            'Else
            'Select Case e.AxisID
            '        Case 0
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
            '            End If
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
            '            End If

            '    End Select
            ''End If
            If myJoystickBindings.ContainsKey(sender.joyindex) Then
                For Each axis In (From a In myJoystickBindings(sender.joyindex) Where a.Axis = e.AxisID)
                    If (e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F And axis.AxisDirection < 0) OrElse (e.RawJoyInfo.IsAxisMax(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F And axis.AxisDirection > 0) Then
                        WindowsInput.InputSimulator.SimulateKeyUp(axis.KeyboardKey)
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub Form1_JoyStickAxisPress(sender As Object, e As JoyApi.Joystick.JoyStickAxisPressEventArgs) Handles Me.JoyStickAxisPress
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub

            'If sender.joyindex = 2 Then
            '    Select Case e.AxisID
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
            '            Else
            '                ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
            '            End If
            '    End Select
            'Else
            If myJoystickBindings.ContainsKey(sender.joyindex) Then
                For Each axis In (From a In myJoystickBindings(sender.joyindex) Where a.Axis = e.AxisID)
                    If (e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F And axis.AxisDirection < 0) OrElse (e.RawJoyInfo.IsAxisMax(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F And axis.AxisDirection > 0) Then
                        WindowsInput.InputSimulator.SimulateKeyPress(axis.KeyboardKey)
                    End If
                Next
            End If
            'Select Case e.AxisID
            '        Case 0
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
            '            End If
            '        Case 1
            '            If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
            '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)
            '            Else
            '                WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
            '            End If
            '    End Select
            ''End If
        End If
    End Sub

    Private Sub Form1_JoyStickButtonDown(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs) Handles Me.JoyStickButtonDown
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub


            'Dim i = (From a In myKeybindings From b In a.Value.Joystick Where b.Id = sender.joyindex And b.Button = Math.Min(e.buttonID + 1, 512))

            'If i IsNot Nothing Then
            '    For Each a In i
            '        WindowsInput.InputSimulator.SimulateKeyDown(a.a.Value.KeyboardKey)
            '        e.Handled = True
            '    Next
            'End If
            If myJoystickBindings.ContainsKey(sender.joyindex) Then
                For Each button In (From a In myJoystickBindings(sender.joyindex) Where a.Button = e.buttonID + 1)
                    WindowsInput.InputSimulator.SimulateKeyDown(button.KeyboardKey)
                    e.Handled = True
                Next
            End If
            Exit Sub


            Select Case e.buttonID
                Case 0
                    If sender.joyindex = 0 Or sender.joyindex = 1 Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.Return)
                        e.Handled = True
                    End If
                Case 1
                    If sender.joyindex = 0 Or sender.joyindex = 1 Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.BACK)
                        e.Handled = True
                    End If
                Case 2
                    If sender.joyindex = 2 Then
                        'WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.PAUSE)
                        'e.Handled = True

                        Dim data(0) As frmMenulator.INPUT
                        data(0).type = frmMenulator.INPUT_TYPE.INPUT_KEYBOARD
                        data(0).ki.wVk = 0
                        data(0).ki.wScan = WindowsInput.VirtualKeyCode.PAUSE
                        data(0).ki.dwFlags = WindowsInput.KeyboardFlag.SCANCODE

                        'data(1).ki.wVk = WindowsInput.VirtualKeyCode.PAUSE
                        'data(1).ki.dwFlags = 2

                        frmMenulator.SendInput(0, data, Marshal.SizeOf(GetType(frmMenulator.INPUT)))
                        Threading.Thread.Sleep(50)

                        data(0).ki.dwFlags = WindowsInput.KeyboardFlag.SCANCODE Or WindowsInput.KeyboardFlag.KEYUP

                        frmMenulator.SendInput(0, data, Marshal.SizeOf(GetType(frmMenulator.INPUT)))
                        Threading.Thread.Sleep(50)
                    Else
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.F2)
                        e.Handled = True
                    End If
                Case 3
                    If sender.joyindex = 0 Or sender.joyindex = 1 Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.APPS)
                        e.Handled = True
                    End If
                Case 4
                    If sender.joyindex = 0 Or sender.joyindex = 1 Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.F5)
                        e.Handled = True
                    End If
                Case 5
                    If sender.joyindex = 2 Then
                        WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.F12)
                        e.Handled = True
                    End If
                Case 6
                Case 7


                Case 8
                    'If sender.joyindex = 2 Then
                    '    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.SPACE)
                    'e.Handled = True
                    'End If
            End Select
        ElseIf (InGame AndAlso MenulatorGameMenu IsNot Nothing AndAlso Not MenulatorGameMenu.Visible) Then
            'see if special button is pressed
            'If e.RawJoyInfo.IsButtonPressed(JoyApi.Joystick.NativeMethods.JoyButtons.JOY_BUTTON10) Then
            '    WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.SPACE)
            '    'MenulatorGameMenu.KeyHook_KeyDown(New frmMenulator.KeyboardHook.KeyEventArgsEx(Keys.Space))
            'End If


        End If
    End Sub
#End Region

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        lblTime.Text = Now.ToString("h:mm tt  M/d/yyyy")
    End Sub
    Dim myPopup As frmPopup
    Private Sub myWMP_MediaChange(Item As WMPLib.IWMPMedia3) Handles myWMP.MediaChange
        Me.Invoke(Sub()

                      Dim s = WMPImagePath(Item)
                      Dim i As Bitmap
                      If String.IsNullOrEmpty(s) Then
                          i = My.Resources.audio_file_64
                      Else
                          i = New Bitmap(s)
                      End If
                      If Not myPopup.Visible Then
                          myPopup.DoPopUp(Item.name, Item.getItemInfo("WM/AlbumArtist") & vbCrLf & Item.getItemInfo("WM/AlbumTitle") & vbCrLf & Item.getItemInfo("WM/Year"), i)
                      Else
                          myPopup.Push(Item.name, Item.getItemInfo("WM/AlbumArtist") & vbCrLf & Item.getItemInfo("WM/AlbumTitle") & vbCrLf & Item.getItemInfo("WM/Year"), i)
                      End If
                  End Sub)
    End Sub


End Class

Public Class ButtonEX
    Inherits Control
    Implements IButtonControl
    Public Sub New()
        MyBase.New

        SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.EnableNotifyMessage Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)
        Me.DoubleBuffered = True
        foreBrushHot = New SolidBrush(ForeColorHot)
        ForeBrush = New SolidBrush(ForeColor)
        backBrush = New SolidBrush(BackColor)
        backBrushHot = New SolidBrush(BackColorHot)
        backBrushDefault = New SolidBrush(backColorDefault)
        backBrushHotDefault = New SolidBrush(BackColorHotDefault)
        foreBrushDefault = New SolidBrush(ForeColorDefault)
    End Sub
    Dim _backColorHot As Color = Color.Green, _foreColorHot As Color = Color.Green, _backColorHotDefault As Color = Color.Green, _backColorDefault As Color = Color.Green, _foreColorDefault As Color = Color.White

    Dim foreBrushHot, ForeBrush As SolidBrush
    Dim backBrushHot, backBrush As SolidBrush
    Dim backBrushDefault, backBrushHotDefault As SolidBrush
    Dim foreBrushDefault As SolidBrush
    <Category("Appearance")>
    Public Property BackColorHot As Color
        Get
            Return _backColorHot
        End Get
        Set(value As Color)
            _backColorHot = value
            backBrushHot = New SolidBrush(value)
        End Set
    End Property
    <Category("Appearance")>
    Public Property BackColorHotDefault As Color
        Get
            Return _backColorHotDefault
        End Get
        Set(value As Color)
            _backColorHotDefault = value
            backBrushHotDefault = New SolidBrush(value)
        End Set
    End Property
    <Category("Appearance")>
    Public Property ForeColorHot As Color
        Get
            Return _foreColorHot
        End Get
        Set(value As Color)
            _foreColorHot = value
            foreBrushHot = New SolidBrush(value)
        End Set
    End Property
    Public Overrides Property ForeColor As Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(value As Color)
            MyBase.ForeColor = value
            ForeBrush = New SolidBrush(value)
        End Set
    End Property
    <Category("Appearance")>
    Public Property ForeColorDefault As Color
        Get
            Return _foreColorDefault
        End Get
        Set(value As Color)
            _foreColorDefault = value
            foreBrushDefault = New SolidBrush(value)
        End Set
    End Property
    Public Overrides Property BackColor As Color
        Get
            If TurnOffRender Then
                Return Color.Transparent
            Else
                Return MyBase.BackColor
            End If
        End Get
        Set(value As Color)
            MyBase.BackColor = value
            backBrush = New SolidBrush(value)
        End Set
    End Property
    <Category("Appearance")>
    Public Property BackColorDefault As Color
        Get
            If TurnOffRender Then
                Return Color.Transparent
            Else
                Return _backColorDefault
            End If
        End Get
        Set(value As Color)
            _backColorDefault = value
            backBrushDefault = New SolidBrush(value)
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property IsHot As Boolean
        Get
            If Not Me.DesignMode Then Return Parent.FindForm.ActiveControl Is Me
            Return False
        End Get
    End Property
    <Browsable(False)> Public Property IsDefault As Boolean

    Dim _dialogResult As DialogResult
    <Browsable(False)>
    Public Property DialogResult As DialogResult Implements IButtonControl.DialogResult
        Get
            Return _dialogResult
        End Get
        Set(value As DialogResult)
            _dialogResult = value
        End Set
    End Property


    Dim sf As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .FormatFlags = StringFormatFlags.FitBlackBox Or StringFormatFlags.NoWrap}

    Public Property TurnOffRender As Boolean = False

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        'If TurnOffRender Then Return
        MyBase.OnPaintBackground(pevent)
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)
        If TurnOffRender Then Return
        DoPaint(e)
    End Sub
    Protected Friend Sub DoPaint(e As PaintEventArgs)
        Dim r = New RectangleF(Point.Empty, Me.Size)
        If Me.IsHot Then
            'e.Graphics.Clear(Color.Black)
            e.Graphics.FillRectangle(backBrushHot, r)
            e.Graphics.DrawString(Me.Text, Me.Font, foreBrushHot, r, sf)
        Else
            'e.Graphics.Clear(BackColor)
            IsDefault = Parent.FindForm.AcceptButton Is Me
            If IsDefault Then
                e.Graphics.FillRectangle(backBrushdefault, r)
                e.Graphics.DrawString(Me.Text, Me.Font, ForeBrushDefault, r, sf)
            Else
                e.Graphics.FillRectangle(backBrush, r)
                e.Graphics.DrawString(Me.Text, Me.Font, ForeBrush, r, sf)
            End If
        End If
    End Sub

    Public Sub NotifyDefault(value As Boolean) Implements IButtonControl.NotifyDefault
        IsDefault = value
        Invalidate()
    End Sub

    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
    End Sub
    Public Sub PerformClick() Implements IButtonControl.PerformClick
        OnClick(Nothing)
    End Sub




End Class
Public Class LabelEX
    Inherits Control
    Public Sub New()
        MyBase.New

        SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.CacheText Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        SetStyle(ControlStyles.Selectable, False)
        Me.DoubleBuffered = True
        _foreBrush = New SolidBrush(Color.Black)
        _backBrush = New SolidBrush(Color.Transparent)

    End Sub
    Public Property TurnOffRender As Boolean = False
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)
        If TurnOffRender Then Return
        DoPaint(e)
    End Sub
    Dim _foreBrush As SolidBrush, _backBrush As SolidBrush
    Public Overrides Property ForeColor As Color
        Get
            Return MyBase.ForeColor
        End Get
        Set(value As Color)
            MyBase.ForeColor = value
            _foreBrush = New SolidBrush(value)
            Invalidate()
        End Set
    End Property
    Public Overrides Property BackColor As Color
        Get
            Return MyBase.BackColor
        End Get
        Set(value As Color)
            MyBase.BackColor = value
            _backBrush = New SolidBrush(value)
            Invalidate()
        End Set
    End Property
    Dim sf As New StringFormat With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Near}
    Public Property Alignment As StringAlignment
        Get
            Return sf.Alignment
        End Get
        Set(value As StringAlignment)
            sf.Alignment = value
            Invalidate()
        End Set
    End Property
    Public Property LineAlignment As StringAlignment
        Get
            Return sf.LineAlignment
        End Get
        Set(value As StringAlignment)
            sf.LineAlignment = value
            Invalidate()
        End Set
    End Property
    Public Property FormatFlags As StringFormatFlags
        Get
            Return sf.FormatFlags
        End Get
        Set(value As StringFormatFlags)
            sf.FormatFlags = value
            Invalidate()
        End Set
    End Property
    Public Property HotkeyPrefix As Text.HotkeyPrefix
        Get
            Return sf.HotkeyPrefix
        End Get
        Set(value As Text.HotkeyPrefix)
            sf.HotkeyPrefix = value
            Invalidate()
        End Set
    End Property
    Public Property Trimming As StringTrimming
        Get
            Return sf.Trimming
        End Get
        Set(value As StringTrimming)
            sf.Trimming = value
            Invalidate()
        End Set
    End Property

    Protected Friend Sub DoPaint(e As PaintEventArgs)
        If Me.BackColor <> Color.Transparent Then
            e.Graphics.FillRectangle(_backBrush, Me.DisplayRectangle)
        End If
        e.Graphics.DrawString(Text, Me.Font, _foreBrush, New Rectangle(Point.Empty, Size), sf)
    End Sub
    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        Invalidate()
    End Sub
    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)
        Invalidate()
    End Sub
    'Protected Overrides Sub OnSizeChanged(e As EventArgs)
    '    MyBase.OnSizeChanged(e)
    '    Invalidate()
    'End Sub


End Class

Public Class PanelEx
    Inherits Panel
    Public Sub New()
        MyBase.New
        Me.DoubleBuffered = True

    End Sub
    <Flags()>
    Public Enum AnimateWindowFlags
        AW_HOR_POSITIVE = &H1
        AW_HOR_NEGATIVE = &H2
        AW_VER_POSITIVE = &H4
        AW_VER_NEGATIVE = &H8
        AW_CENTER = &H10
        AW_HIDE = &H10000
        AW_ACTIVATE = &H20000
        AW_SLIDE = &H40000
        AW_BLEND = &H80000
    End Enum
    <DllImport("user32.dll")>
    Private Shared Function AnimateWindow(ByVal hwnd As IntPtr, ByVal time As Integer, ByVal flags As AnimateWindowFlags) As Boolean
    End Function

    Dim _visible As Boolean
    Public Shadows Property Visible As Boolean
        Get
            Return _visible
        End Get
        Set(value As Boolean)
            _visible = value
            Me.Refresh()
            Application.DoEvents()
            Try
                If _visible Then
                    AnimateWindow(Handle, AnimateTime, AnimateWindowFlags.AW_ACTIVATE Or Animation)
                Else
                    AnimateWindow(Handle, AnimateTime, AnimateWindowFlags.AW_HIDE Or ReverseAnimation())

                End If
            Catch
            End Try
            MyBase.Visible = value
        End Set
    End Property

    Public Property Animation As AnimateWindowFlags

    Private Function ReverseAnimation() As AnimateWindowFlags
        Dim ret As AnimateWindowFlags
        If (Animation And AnimateWindowFlags.AW_HOR_NEGATIVE) = AnimateWindowFlags.AW_HOR_NEGATIVE Then
            ret = AnimateWindowFlags.AW_HOR_POSITIVE
        ElseIf (Animation And AnimateWindowFlags.AW_HOR_POSITIVE) = AnimateWindowFlags.AW_HOR_POSITIVE Then
            ret = AnimateWindowFlags.AW_HOR_NEGATIVE
        End If
        If (Animation And AnimateWindowFlags.AW_VER_NEGATIVE) = AnimateWindowFlags.AW_VER_NEGATIVE Then
            ret = ret Or AnimateWindowFlags.AW_VER_POSITIVE
        ElseIf (Animation And AnimateWindowFlags.AW_VER_POSITIVE) = AnimateWindowFlags.AW_VER_POSITIVE Then
            ret = ret Or AnimateWindowFlags.AW_VER_NEGATIVE
        End If
        If (Animation And AnimateWindowFlags.AW_SLIDE) = AnimateWindowFlags.AW_SLIDE Then
            ret = ret Or AnimateWindowFlags.AW_SLIDE
        End If
        Return ret
    End Function
    Public Property AnimateTime As Integer = 150

    'Protected Overrides ReadOnly Property CreateParams() As CreateParams
    '    Get
    '        Dim cp As CreateParams = MyBase.CreateParams
    '        cp.ExStyle = cp.ExStyle Or &H20 ' // WS_EX_TRANSPARENT
    '        Return cp
    '    End Get
    'End Property
    'Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
    '    'MyBase.OnPaintBackground(e)
    'End Sub
End Class

Public Class FlowLayoutPanelEx
    Inherits FlowLayoutPanel
    Public Sub New()

        Me.DoubleBuffered = True
        SuspendLayout()

        SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        SetStyle(ControlStyles.UserPaint, True)

        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        SetStyle(ControlStyles.ResizeRedraw, True)
        Me.UpdateStyles()

        ResumeLayout()
    End Sub

    <Flags()>
    Public Enum AnimateWindowFlags
        AW_HOR_POSITIVE = &H1
        AW_HOR_NEGATIVE = &H2
        AW_VER_POSITIVE = &H4
        AW_VER_NEGATIVE = &H8
        AW_CENTER = &H10
        AW_HIDE = &H10000
        AW_ACTIVATE = &H20000
        AW_SLIDE = &H40000
        AW_BLEND = &H80000
    End Enum
    <DllImport("user32.dll")>
    Private Shared Function AnimateWindow(ByVal hwnd As IntPtr, ByVal time As Integer, ByVal flags As AnimateWindowFlags) As Boolean
    End Function

    Dim _visible As Boolean
    Public Shadows Property Visible As Boolean
        Get
            Return _visible
        End Get
        Set(value As Boolean)
            _visible = value
            Me.Refresh()
            Application.DoEvents()
            Try
                If _visible Then
                    AnimateWindow(Handle, AnimateTime, AnimateWindowFlags.AW_ACTIVATE Or Animation)
                Else
                    AnimateWindow(Handle, AnimateTime, AnimateWindowFlags.AW_HIDE Or ReverseAnimation())

                End If

            Catch
            End Try
            MyBase.Visible = value

        End Set
    End Property

    Public Property Animation As AnimateWindowFlags

    Private Function ReverseAnimation() As AnimateWindowFlags
        Dim ret As AnimateWindowFlags
        If (Animation And AnimateWindowFlags.AW_HOR_NEGATIVE) = AnimateWindowFlags.AW_HOR_NEGATIVE Then
            ret = AnimateWindowFlags.AW_HOR_POSITIVE
        ElseIf (Animation And AnimateWindowFlags.AW_HOR_POSITIVE) = AnimateWindowFlags.AW_HOR_POSITIVE Then
            ret = AnimateWindowFlags.AW_HOR_NEGATIVE
        End If
        If (Animation And AnimateWindowFlags.AW_VER_NEGATIVE) = AnimateWindowFlags.AW_VER_NEGATIVE Then
            ret = ret Or AnimateWindowFlags.AW_VER_POSITIVE
        ElseIf (Animation And AnimateWindowFlags.AW_VER_POSITIVE) = AnimateWindowFlags.AW_VER_POSITIVE Then
            ret = ret Or AnimateWindowFlags.AW_VER_NEGATIVE
        End If
        If (Animation And AnimateWindowFlags.AW_SLIDE) = AnimateWindowFlags.AW_SLIDE Then
            ret = ret Or AnimateWindowFlags.AW_SLIDE
        End If
        Return ret
    End Function
    Public Property AnimateTime As Integer = 150

    <DllImport("user32.dll")>
    Private Shared Function ShowScrollBar(hWnd As IntPtr, wBar As Integer, bShow As Boolean) As Boolean
    End Function
    Private Enum ScrollBarDirection

        SB_HORZ = 0
        SB_VERT = 1
        SB_CTL = 2
        SB_BOTH = 3
    End Enum

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)

        If (m.Msg = &H85) Then '// WM_NCPAINT

            ShowScrollBar(Me.Handle, ScrollBarDirection.SB_BOTH, False)
        End If
        MyBase.WndProc(m)
    End Sub

End Class

Public Class TextBoxEx
    Inherits TextBox

    Public Property IsNumeric As Boolean
    Public Property MinNumber As Integer
    Public Property MaxNumber As Integer = Integer.MaxValue - 1

    Private Sub TextBoxEx_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Select Case e.KeyCode
            Case Form1.myKeybindings("Up").KeyboardKey
                If Me.Text.Length = 0 Then
                    If IsNumeric = False Then
                        Me.Text = "A"
                        Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    Else
                        Me.Text = MinNumber
                        Me.SelectionStart = 0
                        Me.SelectionLength = Me.Text.Length
                    End If
                Else
                    If Not IsNumeric Then
                        If Not String.IsNullOrEmpty(Me.SelectedText) Then
                            Dim i = Me.SelectionStart
                            Dim c As Char = Me.SelectedText
                            If Asc(c) < 97 Then
                                c = Chr(Asc(c) + 1)
                            Else
                                c = "A"
                            End If
                            Dim cs = Me.Text.ToCharArray
                            cs(i) = c
                            Me.Text = New String(cs)

                            Me.SelectionStart = i
                            Me.SelectionLength = 1
                        End If
                    Else
                        Dim int = Val(Me.Text)
                        int += 1
                        If int > MaxNumber Then int = MaxNumber
                        Me.Text = int
                        Me.SelectionStart = 0
                        Me.SelectionLength = Me.Text.Length
                    End If
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("Down").KeyboardKey
                If Me.Text.Length = 0 Then
                    If Not IsNumeric Then
                        Me.Text = "A"
                        Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    Else
                        Me.Text = Me.MinNumber
                        Me.SelectionStart = 0
                        Me.SelectionLength = Me.Text.Length
                    End If
                Else
                    If Not IsNumeric Then
                        If Not String.IsNullOrEmpty(Me.SelectedText) Then
                            Dim i = Me.SelectionStart
                            Dim c As Char = Me.SelectedText
                            If Asc(c) > 32 Then
                                c = Chr(Asc(c) - 1)
                            Else
                                c = "`"
                            End If
                            Dim cs = Me.Text.ToCharArray
                            cs(i) = c
                            Me.Text = New String(cs)
                            Me.SelectionStart = i
                            Me.SelectionLength = 1
                        End If
                    Else
                        Dim int = Val(Me.Text)
                        int -= 1
                        If int < MinNumber Then int = MinNumber
                        Me.Text = int
                        Me.SelectionStart = 0
                        Me.SelectionLength = Me.Text.Length
                    End If
                End If
                e.Handled = True
                e.SuppressKeyPress = True

            Case Form1.myKeybindings("Cancel").KeyboardKey
                Dim i = Me.SelectionStart
                If i > 0 AndAlso i = Me.Text.Length Then
                    Me.Text = Me.Text.Remove(Me.Text.Length - 1, 1)
                    Me.SelectionStart = Me.Text.Length - 1
                    Me.SelectionLength = 1
                ElseIf i > 0 Then
                    Me.Text = Me.Text.Remove(i, 1)
                    Me.SelectionStart = i - 1
                    Me.SelectionLength = 1
                ElseIf String.IsNullOrEmpty(Me.Text) Then
                    Me.SelectionStart = 0
                    Me.SelectionLength = 0
                Else
                    Me.Text = Me.Text.Remove(0, 1)
                    Me.SelectionStart = 0
                    Me.SelectionLength = 1

                End If
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("Right").KeyboardKey
                If Not IsNumeric Then
                    If Me.Text.Length = 0 Then
                        Me.Text = "A"
                        Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    ElseIf Me.SelectionStart + 1 = Me.Text.Length Then
                        AppendText("A")
                        Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    Else
                        Me.SelectionStart += 1
                        'Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    End If
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("Left").KeyboardKey
                If Not IsNumeric Then
                    If Me.Text.Length = 0 Then
                        Me.Text = "A"
                        Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    ElseIf Me.SelectionStart <= 0 Then
                        'AppendText("A")
                        'Me.SelectionStart += 1
                        'Me.SelectionStart -= 1
                        'Me.SelectionLength = 1
                    Else
                        'Me.SelectionStart += 1
                        Me.SelectionStart -= 1
                        Me.SelectionLength = 1
                    End If
                End If
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("Action").KeyboardKey
                Parent.SelectNextControl(Me, True, True, False, True)
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("AltAction").KeyboardKey, Form1.myKeybindings("Start1").KeyboardKey, Form1.myKeybindings("Start2").KeyboardKey
                If Windows.Forms.Form.ActiveForm IsNot Nothing AndAlso Windows.Forms.Form.ActiveForm.AcceptButton IsNot Nothing Then Windows.Forms.Form.ActiveForm.AcceptButton.PerformClick()
                e.Handled = True
                e.SuppressKeyPress = True
        End Select
    End Sub
    'Private Sub TextboxEx_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
    '    e.Handled = True
    '    e.SuppressKeyPress = True
    'End Sub

    'Private Sub TextboxEx_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
    '    e.Handled = True
    'End Sub


    'Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
    '    e.Handled = True
    '    e.SuppressKeyPress = True
    '    MyBase.OnKeyDown(e)
    'End Sub
    'Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
    '    e.Handled = True
    '    e.SuppressKeyPress = True
    '    MyBase.OnKeyUp(e)

    'End Sub
    Protected Overrides Function ProcessCmdKey(ByRef m As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Back, Keys.Return, Keys.Apps

                Dim a = New KeyEventArgs(keyData)
                TextBoxEx_KeyDown(Me, a)
                Return a.Handled
            Case Else
                Return MyBase.ProcessCmdKey(m, keyData)
        End Select
    End Function
End Class

Public Class ComboBoxEx
    Inherits ComboBox
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        Select Case e.KeyData
            Case Form1.myKeybindings("Action").KeyboardKey
                Parent.SelectNextControl(Me, True, True, False, True)
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("Cancel").KeyboardKey
                Me.Text = ""
                e.Handled = True
                e.SuppressKeyPress = True
            Case Form1.myKeybindings("AltAction").KeyboardKey, Form1.myKeybindings("Start1").KeyboardKey, Form1.myKeybindings("Start2").KeyboardKey
                If Windows.Forms.Form.ActiveForm IsNot Nothing AndAlso Windows.Forms.Form.ActiveForm.AcceptButton IsNot Nothing Then Windows.Forms.Form.ActiveForm.AcceptButton.PerformClick()
                e.Handled = True
                e.SuppressKeyPress = True
        End Select


    End Sub
    'Private Sub TextboxEx_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
    '    e.Handled = True
    'End Sub
    'Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
    '    e.Handled = True
    '    e.SuppressKeyPress = True
    '    MyBase.OnKeyUp(e)
    'End Sub
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Form1.myKeybindings("Action").KeyboardKey, Form1.myKeybindings("Cancel").KeyboardKey, Form1.myKeybindings("AltAction").KeyboardKey, Form1.myKeybindings("Start1").KeyboardKey, Form1.myKeybindings("Start2").KeyboardKey
                OnKeyDown(New KeyEventArgs(keyData))
                Return True

            Case Else
                Return MyBase.ProcessCmdKey(msg, keyData)
        End Select
    End Function
End Class

Public Class WindowExtension
    Inherits JoyStickWindow
    '//import dlls
    <DllImport("uxtheme.dll", EntryPoint:="#95")>
    Private Shared Function GetImmersiveColorFromColorSetEx(dwImmersiveColorSet As Integer, dwImmersiveColorType As Integer, bIgnoreHighContrast As Boolean, dwHighContrastCacheMode As Integer) As Integer
    End Function
    <DllImport("uxtheme.dll", EntryPoint:="#96")>
    Private Shared Function GetImmersiveColorTypeFromName(pName As IntPtr) As Integer

    End Function
    <DllImport("uxtheme.dll", EntryPoint:="#98")>
    Private Shared Function GetImmersiveUserColorSetPreference(bForceCheckRegistry As Boolean, bSkipCheckOnFail As Boolean) As Integer

    End Function
    '// Get theme color
    Public Function GetThemeColor() As Color

        Dim colorSetEx = GetImmersiveColorFromColorSetEx(
             GetImmersiveUserColorSetPreference(False, False),
             GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
             False, 0)

        Dim colour = Color.FromArgb(CByte((&HFF000000 And colorSetEx) >> 24), CByte(&HFF And colorSetEx),
             CByte((&HFF00 And colorSetEx) >> 8), CByte((&HFF0000 And colorSetEx) >> 16))

        Return colour
    End Function



    'Class to use user32.dll API

    <DllImport("user32.dll")>
    Private Shared Function SetWindowCompositionAttribute(hwnd As IntPtr, ByRef data As WindowCompositionAttributeData) As Integer

    End Function
    Public Sub EnableBlur()
        EnableBlur(Me)
    End Sub
    Protected Shared Property AccentState As eAccentState = eAccentState.ACCENT_ENABLE_BLURBEHIND
    Public Shared Sub EnableBlur(this As Form)
        Dim accent As New AccentPolicy
        accent.AccentState = AccentState
        Dim accentStructSize = Marshal.SizeOf(accent)
        Dim accentPtr = Marshal.AllocHGlobal(accentStructSize)
        Marshal.StructureToPtr(accent, accentPtr, False)
        Dim Data = New WindowCompositionAttributeData
        Data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY
        Data.SizeOfData = accentStructSize
        Data.Data = accentPtr
        SetWindowCompositionAttribute(this.Handle, Data)
        Marshal.FreeHGlobal(accentPtr)
    End Sub


    Public Enum eAccentState
        ACCENT_DISABLED = 0
        ACCENT_ENABLE_GRADIENT = 1
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2
        ACCENT_ENABLE_BLURBEHIND = 3
        ACCENT_INVALID_STATE = 4
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4
    End Enum

    Private Structure AccentPolicy
        Public AccentState As eAccentState
        Public AccentFlags As Integer
        Public GradientColor As Integer
        Public AnimationId As Integer
    End Structure

    Private Structure WindowCompositionAttributeData
        Public Attribute As WindowCompositionAttribute
        Public Data As IntPtr
        Public SizeOfData As Integer
    End Structure

    Private Enum WindowCompositionAttribute
        WCA_ACCENT_POLICY = 19
    End Enum



    '  Public Class AcrylicCompositor
    '      Const DWM_TNP_FREEZE = &H100000
    '      Const DWM_TNP_ENABLE3D = &H4000000
    '      Const DWM_TNP_DISABLE3D = &H8000000
    '      Const DWM_TNP_FORCECVI = &H40000000
    '      Const DWM_TNP_DISABLEFORCECVI = &H80000000

    '      Public Enum BackdropSource
    '          BACKDROP_SOURCE_DESKTOP = 0
    '          BACKDROP_SOURCE_HOSTBACKDROP = 1
    '      End Enum

    '      Public Structure AcrylicEffectParameter
    '          Dim blurAmount As Single
    '          Dim saturationAmount As Single
    '          Dim tintColor As D2D1_COLOR_F
    '          Dim fallbackColor As D2D1_COLOR_F
    '      End Structure



    '      Private Enum WINDOWCOMPOSITIONATTRIB

    '          WCA_UNDEFINED = &H0
    '          WCA_NCRENDERING_ENABLED = &H1
    '          WCA_NCRENDERING_POLICY = &H2
    '          WCA_TRANSITIONS_FORCEDISABLED = &H3
    '          WCA_ALLOW_NCPAINT = &H4
    '          WCA_CAPTION_BUTTON_BOUNDS = &H5
    '          WCA_NONCLIENT_RTL_LAYOUT = &H6
    '          WCA_FORCE_ICONIC_REPRESENTATION = &H7
    '          WCA_EXTENDED_FRAME_BOUNDS = &H8
    '          WCA_HAS_ICONIC_BITMAP = &H9
    '          WCA_THEME_ATTRIBUTES = &HA
    '          WCA_NCRENDERING_EXILED = &HB
    '          WCA_NCADORNMENTINFO = &HC
    '          WCA_EXCLUDED_FROM_LIVEPREVIEW = &HD
    '          WCA_VIDEO_OVERLAY_ACTIVE = &HE
    '          WCA_FORCE_ACTIVEWINDOW_APPEARANCE = &HF
    '          WCA_DISALLOW_PEEK = &H10
    '          WCA_CLOAK = &H11
    '          WCA_CLOAKED = &H12
    '          WCA_ACCENT_POLICY = &H13
    '          WCA_FREEZE_REPRESENTATION = &H14
    '          WCA_EVER_UNCLOAKED = &H15
    '          WCA_VISUAL_OWNER = &H16
    '          WCA_HOLOGRAPHIC = &H17
    '          WCA_EXCLUDED_FROM_DDA = &H18
    '          WCA_PASSIVEUPDATEMODE = &H19
    '          WCA_LAST = &H1A
    '      End Enum

    '      Private Structure WINDOWCOMPOSITIONATTRIBDATA
    '          Dim Attrib As WINDOWCOMPOSITIONATTRIB
    '          Dim pvData As Object
    '          Dim cbData As Integer
    '      End Structure

    '      Dim d2Device As ComPtr<ID2D1Device1> 
    'Dim d3d11Device As ComPtr<ID3D11Device> 
    'Dim dxgiDevice As ComPtr<IDXGIDevice2> 
    'Dim dxgiFactory As ComPtr<IDXGIFactory2> 
    'Dim d2dFactory2 As ComPtr<ID2D1Factory2> 
    'Dim deviceContext As ComPtr<ID2D1DeviceContext>
    'Dim dcompDevice As ComPtr<IDCompositionDesktopDevice> 
    'Dim dcompDevice3 As ComPtr<IDCompositionDevice3> 
    'Dim dcompTarget As ComPtr<IDCompositionTarget> 

    'Dim rootVisual As sComPtr<IDCompositionVisual2> 
    'Dim fallbackVisual As ComPtr<IDCompositionVisual2> 
    'Dim desktopWindowVisual As ComPtr<IDCompositionVisual2>
    'Dim topLevelWindowVisual As ComPtr<IDCompositionVisual2> 


    'Dim blurEffect As ComPtr<IDCompositionGaussianBlurEffect> 
    'Dim saturationEffect As ComPtr<IDCompositionSaturationEffect> 
    'Dim translateTransform As ComPtr<IDCompositionTranslateTransform> 
    'Dim clip As ComPtr<IDCompositionRectangleClip> 


    'Dim description As New DXGI_SWAP_CHAIN_DESC1
    '      Dim properties As New D2D1_BITMAP_PROPERTIES1
    '      Dim swapChain As ComPtr<IDXGISwapChain1> 
    'Dim fallbackSurface As ComPtr<IDXGISurface2>
    '      Dim fallbackBitmap As ComPtr<ID2D1Bitmap1>
    'Dim fallbackBrush As ComPtr<ID2D1SolidColorBrush> 
    'Dim tintColor As D2D1_COLOR_F = D2D1 :       ColorF(0.0f, 0.0f, 0.0f, .70f)
    'Dim fallbackColor As D2D1_COLOR_F = D2D1 :      ColorF(1.0f,1.0f,1.0f,1.0f)
    'Dim fallbackRect As RectangleF = New RectangleF(0, 0, CSng(GetSystemMetrics(SM_CXSCREEN)), CSng(GetSystemMetrics(SM_CYSCREEN)))


    '      Dim desktopWindow As IntPtr
    '      Dim desktopWindowRect As Rectangle
    '      Dim thumbnailSize As New Size
    '      Dim thumbnail As DWM_THUMBNAIL_PROPERTIES
    '      Dim desktopThumbnail As IntPtr = Nothing



    '      Private ReadOnly Property sourceRect As Rectangle
    '          Get
    '              Return New Rectangle(0, 0, GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN))
    '          End Get
    '      End Property
    '      Private ReadOnly Property destinationSize As Size
    '          Get
    '              Return New Drawing.Size(GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN))
    '          End Get
    '      End Property
    '      Dim topLevelWindowThumbnail As IntPtr = Nothing
    '      Dim hwndExclusionList As IntPtr


    '      'typedef NTSTATUS(WINAPI * RtlGetVersionPtr)(PRTL_OSVERSIONINFOW);
    '      'typedef BOOL(WINAPI * SetWindowCompositionAttribute())(In HWND hwnd,In WINDOWCOMPOSITIONATTRIBDATA* pwcad);
    '      'typedef HRESULT(WINAPI * DwmpCreateSharedThumbnailVisual)(IN HWND hwndDestination,IN HWND hwndSource,IN DWORD dwThumbnailFlags,IN DWM_THUMBNAIL_PROPERTIES* pThumbnailProperties,IN VOID* pDCompDevice,OUT VOID** ppVisual, OUT PHTHUMBNAIL phThumbnailId);
    '      'typedef HRESULT(WINAPI * DwmpCreateSharedMultiWindowVisual)(IN HWND hwndDestination,IN VOID* pDCompDevice,OUT VOID** ppVisual, OUT PHTHUMBNAIL phThumbnailId);
    '      'typedef HRESULT(WINAPI * DwmpUpdateSharedMultiWindowVisual)(IN HTHUMBNAIL hThumbnailId,IN HWND* phwndsInclude,IN DWORD chwndsInclude,IN HWND* phwndsExclude,IN DWORD chwndsExclude,OUT RECT* prcSource, OUT SIZE* pDestinationSize,In DWORD dwFlags);
    '      'typedef HRESULT(WINAPI * DwmpCreateSharedVirtualDesktopVisual)(IN HWND hwndDestination,IN VOID* pDCompDevice,OUT VOID** ppVisual, OUT PHTHUMBNAIL phThumbnailId);
    '      'typedef HRESULT(WINAPI * DwmpUpdateSharedVirtualDesktopVisual)(IN HTHUMBNAIL hThumbnailId,IN HWND* phwndsInclude,IN DWORD chwndsInclude,IN HWND* phwndsExclude,IN DWORD chwndsExclude,OUT RECT* prcSource, OUT SIZE* pDestinationSize);

    '      Dim DwmpCreateSharedThumbnailVisual As DwmCreateSharedThumbnailVisual
    '      Dim DwmpCreateSharedMultiWindowVisual As DwmCreateSharedMultiWindowVisual
    '      Dim DwmpUpdateSharedMultiWindowVisual As DwmUpdateSharedMultiWindowVisual
    '      Dim DwmpCreateSharedVirtualDesktopVisual As DwmCreateSharedVirtualDesktopVisual
    '      Dim DwmpUpdateSharedVirtualDesktopVisual As DwmUpdateSharedVirtualDesktopVisual
    '      Dim SetWindowCompositionAttribute As DwmSetWindowCompositionAttribute
    '      Dim GetVersionInfo As RtlGetVersionPtr


    '      Dim hr As IntPtr
    '      Dim hostWindowRect As New Rectangle
    '      Sub New(hwnd As IntPtr)
    '          InitLibs()
    '          CreateCompositionDevice()
    '          CreateEffectGraph(dcompDevice3)
    '      End Sub

    '      Function SetAcrylicEffect(hwnd As IntPtr, source As BackdropSource, params As AcrylicEffectParameter) As Boolean

    '          fallbackColor = params.fallbackColor
    '          tintColor = params.tintColor
    '          If (source = BACKDROP_SOURCE_HOSTBACKDROP) Then
    '              Dim enable As Boolean = True
    '              Dim CompositionAttribute As WINDOWCOMPOSITIONATTRIBDATA
    '              CompositionAttribute.Attrib = WCA_EXCLUDED_FROM_LIVEPREVIEW
    '              CompositionAttribute.pvData = enable
    '              CompositionAttribute.cbData = Marshal.SizeOf(Of Boolean)
    '              DwmSetWindowCompositionAttribute(hwnd, CompositionAttribute)
    '          End If

    '          CreateBackdrop(hwnd, source)
    '          CreateCompositionVisual(hwnd)
    '          CreateFallbackVisual()
    '          fallbackVisual.SetContent(swapChain.Get())
    '          rootVisual.RemoveAllVisuals()
    '          Select Case source

    '              Case BACKDROP_SOURCE_DESKTOP
    '                  rootVisual.AddVisual(desktopWindowVisual.Get(), False, Nothing)
    '                  rootVisual.AddVisual(fallbackVisual.Get(), True, desktopWindowVisual.Get())

    '              Case BACKDROP_SOURCE_HOSTBACKDROP
    '                  rootVisual.AddVisual(desktopWindowVisual.Get(), False, Nothing)
    '                  rootVisual.AddVisual(topLevelWindowVisual.Get(), True, desktopWindowVisual.Get())
    '                  rootVisual.AddVisual(fallbackVisual.Get(), True, topLevelWindowVisual.Get())

    '              Case Else
    '                  rootVisual.RemoveAllVisuals()
    '          End Select


    '          rootVisual.SetClip(clip.Get())
    '          rootVisual.SetTransform(translateTransform.Get())

    '          saturationEffect.SetSaturation(params.saturationAmount)

    '          blurEffect.SetBorderMode(D2D1_BORDER_MODE_HARD)
    '          blurEffect.SetInput(0, saturationEffect.Get(), 0)
    '          blurEffect.SetStandardDeviation(params.blurAmount)

    '          rootVisual.SetEffect(blurEffect.Get())
    '          Commit()

    '          SyncCoordinates(hwnd)

    '          Return True
    '      End Function

    '      Function GetBuildVersion() As Long

    '          If (GetVersionInfo <> Nothing) Then

    '              Dim versionInfo As New RTL_OSVERSIONINFOW
    '              versionInfo.dwOSVersionInfoSize = Marshal.SizeOf(versionInfo)
    '              If (GetVersionInfo(versionInfo) = &H0) Then

    '                  Return versionInfo.dwBuildNumber
    '              End If
    '          End If
    '          Return 0
    '      End Function

    '      Function InitLibs() As Boolean

    '          Dim dwmapi = LoadLibrary("dwmapi.dll")
    '          Dim user32 = LoadLibrary("user32.dll")
    '          Dim ntdll = GetModuleHandleW("ntdll.dll")

    '          If (Not dwmapi OrElse Not user32 OrElse Not ntdll) Then
    '              Return False
    '          End If

    '          GetVersionInfo = DirectCast(GetProcAddress(ntdll, "RtlGetVersion"), RtlGetVersionPtr)
    '          DwmSetWindowCompositionAttribute = DirectCast(GetProcAddress(user32, "SetWindowCompositionAttribute"), SetWindowCompositionAttribute)
    '          DwmCreateSharedThumbnailVisual = DirectCast(GetProcAddress(dwmapi, MAKEINTRESOURCEA(147)), DwmpCreateSharedThumbnailVisual)
    '          DwmCreateSharedMultiWindowVisual = DirectCast(GetProcAddress(dwmapi, MAKEINTRESOURCEA(163)), DwmpCreateSharedMultiWindowVisual)
    '          DwmUpdateSharedMultiWindowVisual = DirectCast(GetProcAddress(dwmapi, MAKEINTRESOURCEA(164)), DwmpUpdateSharedMultiWindowVisual)
    '          DwmCreateSharedVirtualDesktopVisual = DirectCast(GetProcAddress(dwmapi, MAKEINTRESOURCEA(163)), DwmpCreateSharedVirtualDesktopVisual)
    '          DwmUpdateSharedVirtualDesktopVisual = DirectCast(GetProcAddress(dwmapi, MAKEINTRESOURCEA(164)), DwmpUpdateSharedVirtualDesktopVisual)

    '          Return True
    '      End Function

    '      Function CreateCompositionDevice() As Boolean

    '          If (D3D11CreateDevice(0, D3D_DRIVER_TYPE_HARDWARE, Nothing, D3D11_CREATE_DEVICE_BGRA_SUPPORT, Nothing, 0, D3D11_SDK_VERSION, d3d11Device.GetAddressOf(), IntPtr.Zero, IntPtr.Zero) <> S_OK) Then
    '              Return False
    '          End If

    '          If (d3d11Device.QueryInterface(dxgiDevice.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          If (D2D1CreateFactory(D2D1_FACTORY_TYPE: D2D1_FACTORY_TYPE_SINGLE_THREADED, __uuidof(ID2D1Factory2), (Void **)d2dFactory2.GetAddressOf()) <> S_OK)
    'Return False
    '          End If

    '          If (d2dFactory2.CreateDevice(dxgiDevice.Get(), d2Device.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          If (DCompositionCreateDevice3(dxgiDevice.Get(), __uuidof(dcompDevice), (Void **)dcompDevice.GetAddressOf()) <> S_OKThen Then)
    '	Return False
    '          End If

    '          If (dcompDevice.QueryInterface(__uuidof(IDCompositionDevice3), (LPVOID *) & dcompDevice3) <> S_OK) Then
    '              Return False
    '          End If

    '          Return True
    '      End Function

    '      Function CreateFallbackVisual() As Boolean

    '          description.Format = DXGI_FORMAT_B8G8R8A8_UNORM
    '          description.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT
    '          description.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL
    '          description.BufferCount = 2
    '          description.SampleDesc.Count = 1
    '          description.AlphaMode = DXGI_ALPHA_MODE_PREMULTIPLIED

    '          description.Width = GetSystemMetrics(SM_CXSCREEN)
    '          description.Height = GetSystemMetrics(SM_CYSCREEN)

    '          d3d11Device.As(dxgiDevice)

    '          If (CreateDXGIFactory2(0, __uuidof(dxgiFactory), reinterpret_cast < Void **> (dxgiFactory.GetAddressOf())) <> S_OK) Then
    '              Return False
    '          End If

    '          If (dxgiFactory.CreateSwapChainForComposition(dxgiDevice.Get(), description, IntPtr.Zero, swapChain.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          If (d2Device.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS_NONE, deviceContext.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          If (swapChain.GetBuffer(0, __uuidof(fallbackSurface), reinterpret_cast < Void **> (fallbackSurface.GetAddressOf())) <> S_OK) Then
    '              Return False
    '          End If

    '          properties.pixelFormat.alphaMode = D2D1_ALPHA_MODE_PREMULTIPLIED
    '          properties.pixelFormat.format = DXGI_FORMAT_B8G8R8A8_UNORM
    '          properties.bitmapOptions = D2D1_BITMAP_OPTIONS_TARGET Or D2D1_BITMAP_OPTIONS_CANNOT_DRAW
    '          If (deviceContext.CreateBitmapFromDxgiSurface(fallbackSurface.Get(), properties, fallbackBitmap.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          deviceContext.SetTarget(fallbackBitmap.Get())
    '          deviceContext.BeginDraw()
    '          deviceContext.Clear()
    '          deviceContext.CreateSolidColorBrush(tintColor, fallbackBrush.GetAddressOf())
    '          deviceContext.FillRectangle(fallbackRect, fallbackBrush.Get())
    '          deviceContext.EndDraw()

    '          If (swapChain.Present(1, 0) <> S_OK) Then
    '              Return False
    '          End If

    '          Return True
    '      End Function

    '      Function CreateCompositionVisual(hwnd As IntPtr)
    '          dcompDevice3.CreateVisual(rootVisual)
    '          dcompDevice3.CreateVisual(fallbackVisual)

    '          If (Not CreateCompositionTarget(hwnd)) Then
    '              Return False
    '          End If

    '          If (dcompTarget.SetRoot(rootVisual.Get()) <> S_OK) Then
    '              Return False
    '          End If

    '          Return True
    '      End Function

    '      Function CreateCompositionTarget(hwnd As IntPtr) As Boolean
    '          If (dcompDevice.CreateTargetForHwnd(hwnd, False, dcompTarget.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If

    '          Return True
    '      End Function

    '      Function CreateBackdrop(hwnd As IntPtr, source As BackdropSource) As Boolean
    '          Select Case source

    '              Case BACKDROP_SOURCE_DESKTOP
    '                  desktopWindow = FindWindow("Progman", Nothing)

    '                  GetWindowRect(desktopWindow, desktopWindowRect)
    '                  thumbnailSize.cx = (desktopWindowRect.Right - desktopWindowRect.Left)
    '                  thumbnailSize.cy = (desktopWindowRect.Bottom - desktopWindowRect.Top)

    '                  thumbnail.dwFlags = DWM_TNP_SOURCECLIENTAREAONLY Or DWM_TNP_VISIBLE Or DWM_TNP_RECTDESTINATION Or DWM_TNP_RECTSOURCE Or DWM_TNP_OPACITY Or DWM_TNP_ENABLE3D
    '                  thumbnail.opacity = 255
    '                  thumbnail.fVisible = True
    '                  thumbnail.fSourceClientAreaOnly = False
    '                  thumbnail.rcDestination = New Rectangle(0, 0, thumbnailSize.cx, thumbnailSize.cy)
    '                  thumbnail.rcSource = New Rectangle(0, 0, thumbnailSize.cx, thumbnailSize.cy)
    '                  If (DwmCreateSharedThumbnailVisual(hwnd, desktopWindow, 2, thumbnail, dcompDevice.Get(), (Void **)desktopWindowVisual.GetAddressOf(), desktopThumbnail) <> S_OK) Then
    '                      Return False
    '                  End If

    '              Case BACKDROP_SOURCE_HOSTBACKDROP
    '                  If (GetBuildVersion() >= 20000) Then
    '                      hr = DwmCreateSharedMultiWindowVisual(hwnd, dcompDevice.Get(), (Void **)topLevelWindowVisual.GetAddressOf(), &topLevelWindowThumbnail)
    '				Else
    '                      hr = DwmCreateSharedVirtualDesktopVisual(hwnd, dcompDevice.Get(), (Void **)topLevelWindowVisual.GetAddressOf(), &topLevelWindowThumbnail)
    '	End If

    '                  If (Not CreateBackdrop(hwnd, BACKDROP_SOURCE_DESKTOP) OrElse hr <> S_OK) Then
    '                      Return False
    '                  End If
    '                  Dim hwndExclusionList(1) As IntPtr
    '                  hwndExclusionList(0) = IntPtr(0)

    '                  If (GetBuildVersion() >= 20000) Then
    '                      hr = DwmUpdateSharedMultiWindowVisual(topLevelWindowThumbnail, Nothing, 0, hwndExclusionList, 1, sourceRect, destinationSize, 1)
    '                  Else
    '                      hr = DwmUpdateSharedVirtualDesktopVisual(topLevelWindowThumbnail, Nothing, 0, hwndExclusionList, 1, sourceRect, destinationSize)
    '                  End If

    '                  If (hr <> S_OK) Then
    '                      Return False
    '                  End If

    '          End Select
    '          Return True
    '      End Function

    '      Function CreateEffectGraph(dcompDevice3 As ComPtr<IDCompositionDevice3> ) As Boolean
    '          If (dcompDevice3.CreateGaussianBlurEffect(blurEffect.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If
    '          If (dcompDevice3.CreateSaturationEffect(saturationEffect.GetAddressOf()) <> S_OK) Then
    '              Return False
    '          End If
    '          If (dcompDevice3.CreateTranslateTransform(translateTransform) <> S_OK) Then
    '              Return False
    '          End If
    '          If (dcompDevice3.CreateRectangleClip(clip) <> S_OK) Then
    '              Return False
    '          End If
    '          Return True
    '      End Function

    '      Sub SyncCoordinates(hwnd As IntPtr)
    '          GetWindowRect(hwnd, hostWindowRect)
    '          clip.SetLeft(CSng(hostWindowRect.Left))
    '          clip.SetRight(CSng(hostWindowRect.Right))
    '          clip.SetTop(CSng(hostWindowRect.Top))
    '          clip.SetBottom(CSng(hostWindowRect.Bottom))
    '          rootVisual.SetClip(clip.Get())
    '          translateTransform.SetOffsetX(-1 * CSng(hostWindowRect.Left) - (GetSystemMetrics(SM_CYFRAME) + GetSystemMetrics(SM_CXPADDEDBORDER)))
    '          translateTransform.SetOffsetY(-1 * CSng(hostWindowRect.Top) - (GetSystemMetrics(SM_CYFRAME) + GetSystemMetrics(SM_CYCAPTION) + GetSystemMetrics(SM_CXPADDEDBORDER)))
    '          rootVisual.SetTransform(translateTransform.Get())
    '          Commit()
    '          DwmFlush()
    '      End Sub

    '      Function Sync(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, active As Boolean) As Boolean

    '          Select Case msg

    '              Case WM_ACTIVATE
    '                  SyncFallbackVisual(active)
    '                  Flush()
    '                  Return True
    '              Case WM_WINDOWPOSCHANGED
    '                  SyncCoordinates(hwnd)
    '                  Return True
    '              Case WM_CLOSE
    '                  Erase hwndExclusionList
    '                  Return True
    '          End Select
    '          Return False
    '      End Function

    '      Function Flush() As Boolean
    '          If (topLevelWindowThumbnail IsNot Nothing) Then
    '              If (GetBuildVersion() >= 20000) Then
    '                  DwmUpdateSharedMultiWindowVisual(topLevelWindowThumbnail, Nothing, 0, hwndExclusionList, 1, sourceRect, destinationSize, 1)
    '              Else
    '                  DwmUpdateSharedVirtualDesktopVisual(topLevelWindowThumbnail, Nothing, 0, hwndExclusionList, 1, sourceRect, destinationSize)
    '              End If
    '              DwmFlush()
    '          End If
    '          Return True
    '      End Function

    '      Function Commit() As Boolean
    '          If (dcompDevice.Commit() <> S_OK) Then
    '              Return False
    '          End If
    '          Return True
    '      End Function

    '      Sub SyncFallbackVisual(active As Boolean)
    '          If (Not active) Then
    '              fallbackBrush.SetColor(fallbackColor)
    '          Else
    '              fallbackBrush.SetColor(tintColor)
    '          End If

    '          deviceContext.BeginDraw()
    '          deviceContext.Clear()
    '          deviceContext.FillRectangle(fallbackRect, fallbackBrush.Get())
    '          deviceContext.EndDraw()
    '          swapChain.Present(1, 0)
    '      End Sub
    '  End Class

End Class
