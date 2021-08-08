Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports MAME


<DebuggerStepThrough>
Module submain
    <MTAThread>
    Public Sub main()
        Application.Run(Form1)
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
    Private Class EmulatorDictionary
        Inherits Collections.ObjectModel.Collection(Of Emulator)
        Default Public Overloads ReadOnly Property Item(strTag As String) As IEnumerable(Of Emulator)
            Get
                Return (From a As Emulator In Me, b In a.RomTags Where b = strTag Select a).ToList
            End Get
        End Property
        Public Function ContainsKey(strTag As String) As Boolean
            Return (From a In Me, b In a.RomTags Where b = strTag Take 1).Count
        End Function



    End Class
    Public myKeybindings As New Dictionary(Of String, MenulatorAction)
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
        pnlLeft.Visible = True
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
                                                                                                                                                               Return Not z.<genre>.Value.Contains("* Mature *")
                                                                                                                                                           End Function))
                                                                                                                            pnlLeft.Visible = False
                                                                                                                        End Sub,
                                    .AltClickHandler = Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)

                                                           With NewSubMenu()
                                                               .Controls.AddRange(New NewUIListItem() {
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
                                                                                                                      result2.closeallmenu = True
                                                                                                                      result2.Handled = True
                                                                                                                  End Sub)
                           })
                                                               .Visible = True
                                                               SubMenuIndex(SubMenuDepth) = 0
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

        For Each x As Xml.XmlElement In file.SelectNodes("descendant::keybinding")
            For Each y As Xml.XmlNode In x.ChildNodes
                Dim a = New MenulatorAction() With {.Name = y.Attributes("display").Value,
                                                                                                 .ImgTag = InferStringValue(y.Attributes("imgTag")),
                                                                                                 .KeyboardKey = (From z2 As Xml.XmlNode In y.SelectNodes("descendant::keyboard") Select [Enum].Parse(GetType(Keys), z2.Attributes("button").Value))(0),
                                                                                                                                       .closeWindow = InferBooleanValue(y.Attributes("closeWindow"), True),
                                                                                                                                       .hideWindow = InferBooleanValue(y.Attributes("hideWindow")),
                                                                                                 .Joystick = (From z2 As Xml.XmlNode In y.SelectNodes("descendant::joystick") Select New MenulatorJoystickAction() With {.Id = z2.Attributes("id").Value,
                                                                                                                                       .Button = InferStringValue(z2.Attributes("button")),
                                                                                                                                       .Axis = InferStringValue(z2.Attributes("axis")),
                                                                                                                                       .AxisDirection = If(InferStringValue(z2.Attributes("value")) = "-", -1, 1)})
                    }
                If myKeybindings.ContainsKey(a.Name) Then
                    myKeybindings(a.Name) = a
                Else
                    myKeybindings.Add(a.Name, a)
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
    'Private Function GetFavorites(strEmu As String, Optional strFavXML As String = "Favorites.xml") As List(Of String)
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectNodes("//menulator/favorites/favorite[@emulator='" & strEmu & "']")
    '    Dim l As New List(Of String)
    '    For Each i As Xml.XmlNode In root
    '        l.Add(i.Attributes("name").Value)
    '    Next
    '    Return l
    'End Function
    'Private Sub AddToFavorites(strEmu As String, name As String, Optional strFavXML As String = "Favorites.xml")
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("/menulator/favorites[last()]")
    '    If root Is Nothing Then root = file.SelectSingleNode("/menulator/favorites")
    '    Dim c = file.CreateElement("favorite")
    '    c.SetAttribute("emulator", strEmu)
    '    c.SetAttribute("name", name)
    '    root.AppendChild(c)
    '    file.Save(strFavXML)
    'End Sub
    'Private Function IsFavorite(strEmu As String, name As String, Optional strFavXML As String = "Favorites.xml") As Boolean
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("//menulator/favorites/favorite[@emulator='" & strEmu & "' and @name=" & Chr(34) & name.Replace(Chr(34), "\" & Chr(34)) & Chr(34) & "]")
    '    Return root IsNot Nothing
    'End Function
    'Private Sub RemoveFromFavorites(strEmu As String, name As String, Optional strFavXML As String = "Favorites.xml")
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("//menulator/favorites/favorite[@emulator='" & strEmu & "' and @name='" & name & "']")
    '    If root IsNot Nothing Then
    '        root.ParentNode.RemoveChild(root)
    '        file.Save(strFavXML)
    '    End If

    'End Sub
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
    'Private Sub MarkAsBad(emu As String, name As String, Optional strFavXML As String = "Favorites.xml")
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("/menulator/markedasbad[last()]")
    '    If root Is Nothing Then root = file.SelectSingleNode("/menulator/markedasbad")
    '    Dim c = file.CreateElement("bad")
    '    c.SetAttribute("emulator", emu)
    '    c.SetAttribute("name", name)
    '    root.AppendChild(c)
    '    file.Save(strFavXML)
    'End Sub
    'Private Sub MarkAsNotBad(emu As String, name As String, Optional strFavXML As String = "Favorites.xml")
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("//menulator/markedasbad/bad[@emulator='" & emu & "' and @name='" & name & "']")
    '    If root IsNot Nothing Then
    '        root.ParentNode.RemoveChild(root)
    '        file.Save(strFavXML)
    '    End If
    'End Sub
    'Private Function IsMarkedAsBad(emu As String, name As String, Optional strFavXML As String = "Favorites.xml") As Boolean
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectSingleNode("//menulator/markedasbad/bad[@emulator='" & emu & "' and @name='" & name & "']")
    '    Return root IsNot Nothing
    'End Function
    'Private Function GetMarkedAsBad(emu As String, Optional strFavXML As String = "Favorites.xml") As List(Of String)
    '    Dim file As New Xml.XmlDocument()
    '    file.Load(strFavXML)
    '    Dim root = file.SelectNodes("//menulator/markedasbad/bad[@emulator='" & emu & "']")
    '    Dim l As New List(Of String)
    '    For Each i As Xml.XmlNode In root
    '        l.Add(i.Attributes("name").Value)
    '    Next
    '    Return l
    'End Function
    'ui to the settings of each emulator
    Private Sub ShowEmulatorSettings()



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
    Private Function InferStringValue(o As Object) As String
        If o Is Nothing Then
            Return Nothing
        ElseIf TypeOf o Is Xml.XmlAttribute Then
            Return o.value
        Else
            Return CStr(o)
        End If
    End Function


    'Protected Overrides Sub OnResize(e As EventArgs)
    '    MyBase.OnResize(e)
    '    If myList IsNot Nothing Then Me.Text = "Showing Index: " & myList.VisibleIndexRange(0) & " | " & myList.VisibleIndexRange(1) & " | Count " & myList.m.Count  'Me.Text = myList.m.Count & " | " & myList.ItemsPerX & " | " & myList.ItemSpacingX & " | " & myList.VisibleItemCount
    'End Sub

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

        myList.GameList = myMame_XML.GetNextX(myMame_XML.Count)



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

    Private Async Sub ListDrive(strPath As String)
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
                           Catch
                           End Try

                       End Sub)

        myList.Index = 0
        myList.EnableWait = False

    End Sub

#End Region

#Region "Menus and Submenus"
    Private Function NewSubMenu() As FlowLayoutPanelEx
        'CloseSubMenu()
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
            If SubMenuDepth > 1 Then pnlLeft.Width += 70

            .Left = 70 * SubMenuDepth '  70
            .Top = 0
            .Width = pnlLeft.Width - (70 * SubMenuDepth)
            .Height = pnlLeft.Height - lblTime.Height  '1396
            .Anchor = AnchorStyles.Left Or AnchorStyles.Bottom Or AnchorStyles.Right
            .Animation = Menulator_Zero.PanelEx.AnimateWindowFlags.AW_HOR_NEGATIVE
            .AutoScroll = True
            .AnimateTime = 150
            .BackColor = System.Drawing.SystemColors.ControlLight
            .FlowDirection = System.Windows.Forms.FlowDirection.TopDown
            .Padding = New System.Windows.Forms.Padding(0, 0, 20, 0)
            AddHandler .VisibleChanged, AddressOf pnlSubMenu_VisibleChanged
            .Visible = False
            .WrapContents = False

            '.Controls.AddRange((From a In myGamesMenu Select a.Value).ToArray)

            pnlLeft.Controls.Add(subMenus(UBound(subMenus)))
            .BringToFront()
            '.Visible = True
            SubMenuIndex(UBound(_subMenuIndex)) = 0
            '.Update()

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

    Private Function CreateMainMenu() As Menulator_Zero.FlowLayoutPanelEx


        'Dim _Panel4 As New Panel
        'With _Panel4
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_games
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel4"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label14
        ''
        'Dim _label14 As New Label
        'With _label14
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label14"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Games"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        'Dim _pnlGames As New Panel
        'With _pnlGames
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel4)
        '    .Controls.Add(_label14)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 3)
        '    .Name = "pnlGames"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 8
        'End With
        ''
        ''Panel5
        ''
        'Dim _Panel5 As New Panel
        'With _Panel5
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_audio
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel5"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label15
        ''
        'Dim _Label15 As New Label
        'With _Label15
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label15"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Media"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlMusic
        ''
        'Dim _pnlMusic As New Panel
        'With _pnlMusic
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel5)
        '    .Controls.Add(_Label15)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 57)
        '    .Name = "pnlMusic"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 9
        'End With
        ''
        ''Panel7
        ''
        'Dim _Panel7 As New Panel
        'With _Panel7
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_globe
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel7"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label16
        ''
        'Dim _Label16 As New Label
        'With _Label16
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label16"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Internet"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlNetwork
        ''
        'Dim _pnlNetwork As New Panel
        'With _pnlNetwork
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel7)
        '    .Controls.Add(_Label16)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 111)
        '    .Name = "pnlNetwork"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 10
        'End With
        ''
        ''Panel9
        ''
        'Dim _Panel9 As New Panel
        'With _Panel9
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_gotostart
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel9"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label7
        ''
        'Dim _Label7 As New Label
        'With _Label7
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label7"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Application"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlApplication
        ''
        'Dim _pnlApplication As New Panel
        'With _pnlApplication
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel9)
        '    .Controls.Add(_Label7)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 165)
        '    .Name = "pnlApplication"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 11
        'End With
        ''
        ''Panel11
        ''
        'Dim _Panel11 As New Panel
        'With _Panel11
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_folder
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel11"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label8
        ''
        'Dim _Label8 As New Label
        'With _Label8
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label8"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "File Explorer"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlFiles
        ''
        'Dim _pnlFiles As New Panel
        'With _pnlFiles
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel11)
        '    .Controls.Add(_Label8)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 219)
        '    .Name = "pnlFiles"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 12
        'End With
        ''
        ''Panel13
        ''
        'Dim _Panel13 As New Panel
        'With _Panel13
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_settings2
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel13"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label9
        ''
        'Dim _Label9 As New Label
        'With _Label9
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label9"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Control Panel"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlSettings
        ''
        'Dim _pnlSettings As New Panel
        'With _pnlSettings
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel13)
        '    .Controls.Add(_Label9)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 273)
        '    .Name = "pnlSettings"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 13
        'End With
        ''
        ''Panel15
        'Dim _Panel15 As New Panel
        'With _Panel15
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.icon_wm10_help
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel15"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label10
        ''
        'Dim _Label10 As New Label
        'With _Label10
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label10"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Help"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With
        ''
        ''pnlHelp
        ''
        'Dim _pnlHelp As New Panel
        'With _pnlHelp
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel15)
        '    .Controls.Add(_Label10)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 327)
        '    .Name = "pnlHelp"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 14
        'End With
        ''
        ''Panel17
        ''
        'Dim _Panel17 As New Panel
        'With _Panel17
        '    .BackgroundImage = Global.Menulator_Zero.My.Resources.Resources.notifications_powermenu_icon_standby
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '    .Location = New System.Drawing.Point(6, 3)
        '    .Name = "Panel17"
        '    .Size = New System.Drawing.Size(52, 42)
        '    .TabIndex = 2
        'End With
        ''
        ''Label11
        ''
        'Dim _Label11 As New Label
        'With _Label11
        '    .ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        '    .Location = New System.Drawing.Point(64, 0)
        '    .Name = "Label11"
        '    .Size = New System.Drawing.Size(222, 48)
        '    .TabIndex = 8
        '    .Text = "Power"
        '    .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        'End With

        ''
        ''pnlPower
        ''
        'Dim _pnlPower As New Panel
        'With _pnlPower
        '    .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        '    .Controls.Add(_Panel17)
        '    .Controls.Add(_Label11)
        '    .Dock = System.Windows.Forms.DockStyle.Left
        '    .Location = New System.Drawing.Point(3, 381)
        '    .Name = "pnlPower"
        '    .Size = New System.Drawing.Size(286, 48)
        '    .TabIndex = 15
        'End With

        Dim fp1 As New FlowLayoutPanelEx
        With fp1
            .Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
               Or System.Windows.Forms.AnchorStyles.Left) _
               Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            .AnimateTime = 150
            .AutoScroll = True
            '.Controls.Add(_pnlGames)
            '.Controls.Add(_pnlMusic)
            '.Controls.Add(_pnlNetwork)
            '.Controls.Add(_pnlApplication)
            '.Controls.Add(_pnlFiles)
            '.Controls.Add(_pnlSettings)
            '.Controls.Add(_pnlHelp)
            '.Controls.Add(_pnlPower)
            .Controls.Add(New NewUIListItem("Games", My.Resources.icon_wm10_games, Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)
                                                                                       With NewSubMenu()
                                                                                           For Each a In myGamesMenu.Values
                                                                                               a.Width = .Width
                                                                                           Next

                                                                                           .HorizontalScroll.Maximum = 0
                                                                                           .AutoScroll = False
                                                                                           .VerticalScroll.Visible = False
                                                                                           .AutoScroll = True

                                                                                           .Controls.AddRange(myGamesMenu.Values.ToArray)
                                                                                           SubMenuIndex(SubMenuDepth) = 0


                                                                                           .Visible = True
                                                                                           result.CloseMenu = False
                                                                                       End With
                                                                                   End Sub) With {.Name = "pnlGames"})
            .Controls.Add(New NewUIListItem("Music", My.Resources.icon_wm10_audio, Nothing) With {.Name = "pnlMusic"})
            .Controls.Add(New NewUIListItem("Network", My.Resources.icon_wm10_globe, Nothing) With {.Name = "pnlNetwork"})
            .Controls.Add(New NewUIListItem("Application", My.Resources.icon_wm10_gotostart, Nothing) With {.Name = "pnlApplications"})
            .Controls.Add(New NewUIListItem("File Explorer", My.Resources.icon_wm10_folder, Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                With NewSubMenu()
                                                                                                    For Each drive In System.IO.DriveInfo.GetDrives()
                                                                                                        Dim a As New Win32SystemIcons(drive.Name)
                                                                                                        .Controls.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub(sender2 As Object, e2 As EventArgs)
                                                                                                                                                                                                                                                                            ListDrive(drive.Name)
                                                                                                                                                                                                                                                                        End Sub})
                                                                                                    Next

                                                                                                    Dim reg = My.Computer.Registry.CurrentUser.OpenSubKey("Network")
                                                                                                    For Each subname In reg.GetSubKeyNames
                                                                                                        Dim data = reg.OpenSubKey(subname)
                                                                                                        Dim a As New Win32SystemIcons(subname & ":\")
                                                                                                        .Controls.Add(New NewUIListItem() With {.Text = a.DisplayName, .Image = a.GetIcon(True), .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
                                                                                                    Next
                                                                                                    .Visible = True
                                                                                                    SubMenuIndex(SubMenuDepth) = 0
                                                                                                    result.CloseMenu = False
                                                                                                End With
                                                                                            End Sub) With {.Name = "pnlFiles"})
            .Controls.Add(New NewUIListItem("Settings", My.Resources.icon_wm10_settings2, Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)
                                                                                              With NewSubMenu()
                                                                                                  .Controls.Add(New NewUIListItem() With {.Text = "Emulator Settings", .Image = My.Resources.application_settings, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub(sender2 As Object, e2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                                                                                                                                                                              ShowEmulatorSettings()
                                                                                                                                                                                                                                                                                              e2.Handled = True
                                                                                                                                                                                                                                                                                          End Sub})

                                                                                                  .Controls.Add(New NewUIListItem() With {.Text = "Joystick Remap", .Image = My.Resources.joystick, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
                                                                                                                                                                                                                                                                               Dim joys(JoyApi.Joystick.JoyManager.EnumerateDevices.Count - 1) As Guid
                                                                                                                                                                                                                                                                               For count = 0 To UBound(joys)
                                                                                                                                                                                                                                                                                   Dim f As New frmMsg("Press any button on Joystick " & count + 1, Integer.MaxValue, "")
                                                                                                                                                                                                                                                                                   Select Case f.ShowDialog(Me)

                                                                                                                                                                                                                                                                                       Case DialogResult.OK
                                                                                                                                                                                                                                                                                           Dim i = DirectCast(f.JoyTestOutput(0), Menulator_Zero.JoyApi.Joystick).joyIndex
                                                                                                                                                                                                                                                                                           For Each j In JoyApi.Joystick.JoyManager.EnumerateDevices
                                                                                                                                                                                                                                                                                               If j.Value.JoyID = i Then joys(count) = j.Value.GUID : Exit For
                                                                                                                                                                                                                                                                                           Next
                                                                                                                                                                                                                                                                                   End Select
                                                                                                                                                                                                                                                                                   f.Dispose()
                                                                                                                                                                                                                                                                               Next
                                                                                                                                                                                                                                                                               'Computer\HKEY_USERS\S-1-5-21-3065010685-3923032649-1297513653-1001\System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration
                                                                                                                                                                                                                                                                               'For Each subkeys In My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration").GetSubKeyNames
                                                                                                                                                                                                                                                                               '    Dim reg As Guid = New Guid(CType(My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration\" & subkeys).GetValue("GUID"), Byte()))
                                                                                                                                                                                                                                                                               '    For j As Integer = 0 To UBound(joys)
                                                                                                                                                                                                                                                                               '        If joys(j) = reg Then
                                                                                                                                                                                                                                                                               '            My.Computer.Registry.CurrentUser.OpenSubKey("System\CurrentControlSet\Control\MediaProperties\PrivateProperties\DirectInput\VID_045E&PID_0007\Calibration\" & subkeys).SetValue("Joystick ID", j, Microsoft.Win32.RegistryValueKind.Binary)
                                                                                                                                                                                                                                                                               '        End If
                                                                                                                                                                                                                                                                               '    Next

                                                                                                                                                                                                                                                                               'Next
                                                                                                                                                                                                                                                                           End Sub})
                                                                                                  .Visible = True
                                                                                                  SubMenuIndex(SubMenuDepth) = 0
                                                                                                  result.CloseMenu = False
                                                                                              End With
                                                                                          End Sub) With {.Name = "pnlSettings"})
            .Controls.Add(New NewUIListItem("Help", My.Resources.icon_wm10_help, Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)
                                                                                     With NewSubMenu()
                                                                                         .Controls.Add(New NewUIListItem() With {.Text = "Website", .Image = My.Resources.icon_wm10_folder, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub(sender2 As Object, result2 As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                                                                                                                                                                                       Process.Start("http://www.dogskullsoftware.com")
                                                                                                                                                                                                                                                                       result2.Handled = True
                                                                                                                                                                                                                                                                   End Sub})
                                                                                         .Visible = True
                                                                                         SubMenuIndex(SubMenuDepth) = 0
                                                                                         result.CloseMenu = False
                                                                                     End With
                                                                                 End Sub) With {.Name = "pnlHelp"})
            .Controls.Add(New NewUIListItem("Power", My.Resources.notifications_powermenu_icon_standby, Sub(s As Object, result As NewUIListItem.NewUIListItemClickedEvent)
                                                                                                            With NewSubMenu()
                                                                                                                .Controls.Add(New NewUIListItem() With {.Text = "Close Menulator", .Image = My.Resources.notifications_powermenu_icon_close, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
                                                                                                                                                                                                                                                                                                                        Me.Close()
                                                                                                                                                                                                                                                                                                                        'Application.Restart()
                                                                                                                                                                                                                                                                                                                    End Sub})
                                                                                                                .Controls.Add(New NewUIListItem() With {.Text = "Restart Menulator", .Image = My.Resources.notifications_powermenu_icon_standby, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
                                                                                                                                                                                                                                                                                                                            'Me.Close()
                                                                                                                                                                                                                                                                                                                            Application.Restart()
                                                                                                                                                                                                                                                                                                                        End Sub})
                                                                                                                '.Controls.Add(New NewUIListItem() With {.Text = "Log off", .Image = My.Resources.notifications_powermenu_icon_logoff, .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
                                                                                                                '.Controls.Add(New NewUIListItem() With {.Text = "Standby", .Image = My.Resources.notifications_powermenu_icon_standby, .Dock = DockStyle.Left, .Height = 48, .Width = .Width})
                                                                                                                .Controls.Add(New NewUIListItem() With {.Text = "Restart Computer", .Image = My.Resources.notifications_powermenu_icon_restart, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
                                                                                                                                                                                                                                                                                                                           Process.Start("shutdown", "-r -f -t 00")
                                                                                                                                                                                                                                                                                                                       End Sub})
                                                                                                                .Controls.Add(New NewUIListItem() With {.Text = "Shutdown Computer", .Image = My.Resources.notifications_powermenu_icon_shutdown, .Dock = DockStyle.Left, .Height = 48, .Width = .Width, .ClickHandler = Sub()
                                                                                                                                                                                                                                                                                                                             Process.Start("shutdown", "-s -f -t 00")
                                                                                                                                                                                                                                                                                                                         End Sub})
                                                                                                                SubMenuIndex(SubMenuDepth) = 0
                                                                                                                .Visible = True
                                                                                                                result.CloseMenu = False
                                                                                                            End With
                                                                                                        End Sub) With {.Name = "pnlPower"})

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
    Private ReadOnly Property MainMenu_Current As String
        Get
            Return subMenus(0).Controls(_subMenuIndex(0)).Name
        End Get

    End Property

    Private Property SubMenuIndex(index As Integer) As Integer
        Get
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
                Dim result As New NewUIListItem.NewUIListItemClickedEvent
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
                Dim result As New NewUIListItem.NewUIListItemClickedEvent
                If SubMenuIndex(SubMenuDepth) >= 0 AndAlso subMenus(SubMenuDepth).Controls.Count Then DirectCast(subMenus(SubMenuDepth).Controls(SubMenuIndex(SubMenuDepth)), NewUIListItem).PerformAltClick (result)
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
        e.Handled = True
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


    Private Sub me_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown ', myList.KeyDown
        'Debug.Print(e.KeyCode.ToString)

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
        'ElseIf SubMenuDepth > 0 Then
        If pnlLeft.Visible Then
            ProcessSubMenuKeyDown(sender, e)
            If e.Handled Then
                e.SuppressKeyPress = True
                Exit Sub
            End If
        End If
        'ElseIf pnlLeft.Visible Then
        '    ProcessMainMenuKeyDown(sender, e)
        '    If e.Handled Then
        '        e.SuppressKeyPress = True
        '        Exit Sub
        '    End If
        'End If

        Select Case e.KeyCode
                'mylist handling
            Case myKeybindings("Action").KeyboardKey ' Keys.Return

                If myList.GameList IsNot Nothing Then
                    Select Case MainMenu_Current
                        Case "pnlGames"
                            If myList.InAppsMenu Then
                                Exit Select
                            End If

                            LaunchRom()

                            e.Handled = True

                        Case "pnlFiles"
                            'Case Else
                            If TypeOf myList.SelectedItem Is Rom Then
                                With DirectCast(myList.SelectedItem, Rom)
                                    If .ClickHandler IsNot Nothing Then
                                        InGame = True
                                        .ClickHandler.Invoke(myList.SelectedItem, Nothing)
                                        e.SuppressKeyPress = True

                                        e.Handled = True
                                    End If
                                End With
                            ElseIf TypeOf myList.SelectedItem Is iFileSystemObject Then
                                With DirectCast(myList.SelectedItem, iFileSystemObject)
                                    ListDrive(.Path)
                                    e.SuppressKeyPress = True
                                    e.Handled = True
                                End With
                            End If
                    End Select
                End If


            Case myKeybindings("Cancel").KeyboardKey
                If MainMenu_Current = "pnlFiles" Then
                    Try
                        ListDrive(FileIO.FileSystem.GetParentPath(myList.Tag))
                    Catch
                    End Try
                    e.Handled = True
                End If
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
            Case myKeybindings("Left").KeyboardKey, myKeybindings("Right").KeyboardKey
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
                'Me.Text = MAME.App.Version("D:\games\emulation\mame\mame64.exe")

                If frmMsg.Msgbox("Start MAME Rom Verification?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    myList.Dump()
                    Dim worker As New BackgroundWorker

                    myList.EnableWait = True
                    AddHandler worker.DoWork, Sub(zsender As Object, eargs As DoWorkEventArgs)
                                                  Dim i = MameXml.VerifyRoms(myMame.MamePath)

                                                  frmMsg.Msgbox("Verified " & FormatNumber(i, 0,,, TriState.True) & " ROMs")
                                              End Sub
                    AddHandler worker.RunWorkerCompleted, Sub(zsender As Object, eargs As RunWorkerCompletedEventArgs)
                                                              'myList.EnableWait = False
                                                              '' myList.ReInit()
                                                              'myMame_XML.Init(Function() True)
                                                              'myList.Index = 0
                                                              Me.ListMAME()
                                                          End Sub
                    worker.RunWorkerAsync()
                End If
                e.Handled = True
            Case Keys.D0
                frmMsg.Msgbox("This is a pretty long string but there are no line breaks here. Hopefully the size calculated will allow all of this text to fit! But we must add more to ensure that a line wrap will occur or not? THis is a very high resolution monitor! Need to add even more characters in order to cause a line wrap since so many charactrers can fit horizontally. You know, that is probably an issue, should increase font size!")
        End Select
        'e.Handled = True
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
                                      Dim filterMature As Boolean = o.Argument(0)
                                      Dim filterDescription As String = o.Argument(1)
                                      Dim filterPlayers As String = o.Argument(2)
                                      Dim filterPlayersOp As Integer = o.Argument(3)
                                      Dim filterYear As String = o.Argument(4)
                                      Dim filterYearOp As Integer = o.Argument(5)
                                      Dim filterGenre As String = o.Argument(6)
                                      Dim filterRating As String = o.Argument(7)
                                      Dim filterStatus As String = o.Argument(8)

                                      ListMAME(Function(x As XElement) As Boolean

                                                   If filterMature AndAlso x.<genre>.Value.Contains("* Mature *") Then Return False

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
                            myList_ScaleImageRoutine(myList.GameList(i).ImagePath, img.GetIcon(True))
                            myList.RefreshIndex(i)

                        End If
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

    'Private Sub worker_DoWork(sender As Object, e As DoWorkEventArgs) Handles worker.DoWork
    '    Dim startIndex As Integer, endIndex As Integer
    '    Dim f = Split(e.Argument, "|")
    '    startIndex = f(0)
    '    endIndex = f(1)
    '    For i As Integer = startIndex To endIndex
    '        With DirectCast(myList.GameList(i), MameGame)
    '            If .ImagePath Is Nothing Then
    '                Dim v = MAME.Snap.FoundSnaps(myMame.MamePath, snapdir, .Name)
    '                If v.Count Then
    '                    .ImagePath = v(0)

    '                    ScaleImageRoutine(v(0))
    '                    myList.RefreshIndex(i)

    '                Else
    '                    v = MAME.Snap.FoundSnapsWithWebImages(myMame.MamePath, snapdir, .Name)
    '                    If v IsNot Nothing Then
    '                        ' .ImagePath = v(0)
    '                        Debug.Print(v(0))
    '                        'UriQueue.Enqueue(v(0) & "|" & .Name & "|" & i)

    '                        Dim path = System.IO.Path.Combine(snapdir(0), System.IO.Path.GetFileName(v(0)))
    '                        If DownloadRemoteImageFile(v(0), path) Then
    '                            ScaleImageRoutine(v(0))
    '                            .ImagePath = path
    '                            myList.RefreshIndex(i)
    '                        Else
    '                            myList.bitBucket.TryAdd(path, Nothing)
    '                            .ImagePath = ""
    '                        End If
    '                    Else
    '                        .ImagePath = ""
    '                    End If
    '                End If
    '            End If
    '        End With
    '    Next


    '    'Do
    '    '    Dim t As String = Nothing
    '    '    If Not UriQueue.TryDequeue(t) Then Exit Do

    '    '    Dim s As String() = Split(t, "|")
    '    '    Dim path = System.IO.Path.Combine(snapdir(0), System.IO.Path.GetFileName(s(0)))
    '    '    If DownloadRemoteImageFile(s(0), path) Then
    '    '        myList.bitBucket.TryAdd(path, Image.FromFile(path))
    '    '    Else
    '    '        myList.bitBucket.TryAdd(path, Nothing)
    '    '    End If
    '    '    myList.RefreshIndex(s(2))
    '    'Loop
    'End Sub
    ' Dim WithEvents worker As New System.ComponentModel.BackgroundWorker

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
            If sender.joyindex = 2 Then
                Select Case e.AxisID
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.SPACE)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
                        End If
                End Select
            Else
                Select Case e.AxisID
                    Case 0
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.LEFT)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.RIGHT)
                        End If
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.UP)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyDown(WindowsInput.VirtualKeyCode.DOWN)
                        End If

                End Select
            End If
        End If
    End Sub

    Private Sub Form1_JoyStickAxisUp(sender As Object, e As JoyApi.Joystick.JoyStickAxisEventArgs) Handles Me.JoyStickAxisUp
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub

            If sender.joyindex = 2 Then
                Select Case e.AxisID
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            '  WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.SPACE)
                        Else
                            ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
                        End If
                End Select
            Else
                Select Case e.AxisID
                    Case 0
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.LEFT)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.RIGHT)
                        End If
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.UP)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyUp(WindowsInput.VirtualKeyCode.DOWN)
                        End If

                End Select
            End If
        End If
    End Sub

    Private Sub Form1_JoyStickAxisPress(sender As Object, e As JoyApi.Joystick.JoyStickAxisPressEventArgs) Handles Me.JoyStickAxisPress
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub

            If sender.joyindex = 2 Then
                Select Case e.AxisID
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.SPACE)
                        Else
                            ' WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.ESCAPE)
                        End If
                End Select
            Else

                Select Case e.AxisID
                    Case 0
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.LEFT)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.RIGHT)
                        End If
                    Case 1
                        If e.RawJoyInfo.IsAxisMin(e.AxisID, Controllers(sender.joyindex).DeviceCaps) < 0F Then
                            WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.UP)
                        Else
                            WindowsInput.InputSimulator.SimulateKeyPress(WindowsInput.VirtualKeyCode.DOWN)
                        End If
                End Select
            End If
        End If
    End Sub

    Private Sub Form1_JoyStickButtonDown(sender As Object, e As JoyApi.Joystick.JoyStickButtonEventArgs) Handles Me.JoyStickButtonDown
        If InGame = False OrElse (MenulatorGameMenu IsNot Nothing AndAlso MenulatorGameMenu.Visible) Then
            If needJoyId Then NeedJoyIDCallback.Invoke(sender, e) : needJoyId = False : Exit Sub


            Dim i = (From a In myKeybindings From b In a.Value.Joystick Where b.Id = sender.joyindex And b.Button = Math.Min(e.buttonID + 1, 512))

            If i IsNot Nothing Then
                For Each a In i
                    WindowsInput.InputSimulator.SimulateKeyDown(a.a.Value.KeyboardKey)
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
        lblTime.Text = Now.ToString("h:mm tt  M/dd/yyyy")
    End Sub





    '<DllImport("User32.dll")>
    'Private Shared Function RegisterRawInputDevices(ByRef pRawInputDevices As RAWINPUTDEVICE, uiNumDevices As Integer, cbSize As Integer) As Boolean


    'End Function
    'Private Structure RAWINPUTDEVICE
    '    Dim usUsagePage As UShort
    '    Dim usUsage As UShort
    '    Dim dwFlags As Integer
    '    Dim hwndTarget As IntPtr
    'End Structure
    'Protected Overrides Sub DefWndProc(ByRef m As Message)
    '        Const WM_INPUT As Integer = &HF
    '        Select Case m.Msg
    '            Case WM_INPUT
    '                Debug.Print(m.WParam)
    '        End Select
    '        MyBase.DefWndProc(m)
    '    End Sub
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
    Public Shared Sub EnableBlur(this As Form)
        Dim accent As New AccentPolicy
        accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
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


    Private Enum AccentState
        ACCENT_DISABLED = 0
        ACCENT_ENABLE_GRADIENT = 1
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2
        ACCENT_ENABLE_BLURBEHIND = 3
        ACCENT_INVALID_STATE = 4
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4
    End Enum

    Private Structure AccentPolicy
        Public AccentState As AccentState
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
End Class

