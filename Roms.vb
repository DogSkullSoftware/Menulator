Imports System.IO
Imports System.Runtime.InteropServices
Imports MAME
Imports System.Runtime.CompilerServices
Imports System.Text

Module Module1
    'Private Function Combine(high As Byte, low As Byte) As Short
    '    Return CShort(high) << 8 Or CShort(low)
    'End Function
    'Private Function HiByte(b As Byte) As Byte
    '    Return b >> 4
    'End Function
    'Private Function LoByte(b As Byte) As Byte
    '    Return b And &HF
    'End Function
    'Public Function MakeInt(ByVal LByte As Byte, ByVal hByte As Byte) As Integer
    '    Return ((hByte * &H100) + LByte)
    'End Function
    'Public Function HiWord(i As Integer) As Byte
    '    Return i >> 4
    'End Function
    'Public Function LoWord(i As Integer) As Byte
    '    Return i And &HFF
    'End Function

    <Extension> Public Function HiNibble(b As Byte) As Byte
        Return b >> 4
    End Function
    <Extension> Public Function LoNibble(b As Byte) As Byte
        Return b And &HF
    End Function

    <Extension> Public Function Combine(LByte As Byte, HByte As Byte) As Short
        Return CShort(HByte) << 8 Or CShort(LByte)
    End Function

    <Extension> Public Function HiWord(i As Integer) As Short
        Return i >> 8
    End Function
    <Extension> Public Function LoWord(i As Integer) As Short
        Return i And &HFF
    End Function
    <Extension> Public Function Combine(LWord As Short, HWord As Short) As Integer
        Return HWord << 8 Or LWord
    End Function

    Public Function EnumerateFiles(strFolder As String, extensions As String, Optional searchOption As FileIO.SearchOption = FileIO.SearchOption.SearchAllSubDirectories) As ObjectModel.ReadOnlyCollection(Of String)


        'FileIO.FileSystem.GetDirectories(strFolder)
        Return FileIO.FileSystem.GetFiles(strFolder, searchOption, Split(extensions, ";"))
    End Function
End Module



Public Class SnesROM
    Implements MAME.IRom


    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure SNES_RomHeader
        Public MakerCode As Short
        Public GameCode As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> Public unused As Byte()
        Public ExpansionRomSize As Byte
        Public SpecialVersion As Byte
        Public CartType As Byte
    End Structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure SNES_CartHeader
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=21)> Public RomName As Byte()
        Public MapMode As Byte
        Public RomType As Byte
        Public RomSize As Byte
        Public SRAMSize As Byte
        Public DestCode As Byte
        Public reserved As Byte
        Public Version As Byte
        Public ComplementCheck As Short
        Public Checksum As Short
    End Structure
    Dim RomHeader As SNES_RomHeader
    Dim CartHeader As SNES_CartHeader

    Dim _Name As String
    Dim _Path As String
    Dim _ImagePath As String
    Dim _Description As String
    Public Sub New(strPath As String, strImagePath As String)
        Me._Path = strPath
        RomHeader = New SNES_RomHeader
        CartHeader = New SNES_CartHeader
        Using f = IO.File.OpenRead(Path)
            Dim HaveSMCHeader As Boolean
            If (f.Length And &H7FFF) = 512 Then
                f.Seek(&H200, IO.SeekOrigin.Begin)
                HaveSMCHeader = True
            End If
            f.Seek(&H7FC0 + 21, IO.SeekOrigin.Current)
            Dim HaveRomInfo As Boolean, ReadOffset As Int32
            Dim InRecursive As Boolean
ReadThatByte:
            Dim i = f.ReadByte
            Select Case i
                Case &H20
                    'no rom info; name at 7fc0
                    HaveRomInfo = False
                    ReadOffset = &H7FC0
                Case &H30
                    'no rom info; name at 7fc0
                    HaveRomInfo = False
                    ReadOffset = &H7FC0
                Case &H21
                    '? probably the same as 31
                    HaveRomInfo = True
                    ReadOffset = &HFFB0
                Case &H31
                    'have rom info; starts at ffc0
                    HaveRomInfo = True
                    ReadOffset = &HFFB0
                Case Else
                    'try next offset
                    If InRecursive Then Exit Select
                    'If HaveSMCHeader Then f.Seek(&H200, IO.SeekOrigin.Begin) Else f.Seek(0, IO.SeekOrigin.Begin)
                    f.Seek((If(HaveSMCHeader, &H200, &H0) + &HFFB0 + &H10 + 21), IO.SeekOrigin.Begin)
                    InRecursive = True
                    GoTo ReadThatByte
            End Select
            If HaveSMCHeader Then
                f.Seek(&H200, IO.SeekOrigin.Begin)
            Else
                f.Seek(0, IO.SeekOrigin.Begin)
            End If
            f.Seek(ReadOffset, IO.SeekOrigin.Current)
            If HaveRomInfo Then
                'MsgBox(Marshal.SizeOf(GetType(SNES_RomHeader)) = &H10)
                Dim rawData2(&H10) As Byte
                f.Read(rawData2, 0, UBound(rawData2))

                Dim handle2 As GCHandle = GCHandle.Alloc(rawData2, GCHandleType.Pinned)
                Try

                    Dim rawDataPtr As IntPtr = handle2.AddrOfPinnedObject()
                    RomHeader = Marshal.PtrToStructure(rawDataPtr, GetType(SNES_RomHeader))

                Finally

                    handle2.Free()
                End Try
            End If
            Dim rawData(Marshal.SizeOf(CartHeader)) As Byte

            f.Read(rawData, 0, UBound(rawData))
            f.Close()

            Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
            Try

                Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
                CartHeader = Marshal.PtrToStructure(rawDataPtr, GetType(SNES_CartHeader))

            Finally

                handle.Free()
            End Try
        End Using
        Me._ImagePath = strImagePath
    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            _Name = System.Text.Encoding.ASCII.GetString(CartHeader.RomName).Trim

            If _Name.Contains(vbNullChar) OrElse _Name.Contains(vbCr) OrElse _Name.Contains(vbLf) OrElse _Name.Contains(Chr(24)) Then
                Return IO.Path.GetFileNameWithoutExtension(Path)
            Else
                Return _Name
            End If

        End Get
        Private Set(value As String)
            _Name = value
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _Path
        End Get
        Friend Set(value As String)
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
            Dim s As String = Name
            If s Is Nothing OrElse String.IsNullOrEmpty(s) OrElse s.Any(Function(c) c < Chr(32)) Then
                Return IO.Path.GetFileNameWithoutExtension(_Path)
            Else
                Return s
            End If
        End Get
        Private Set(value As String)
            Name = value
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite




    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.smc;*.sfc;*.fig", Optional filter As Func(Of SnesROM, Boolean) = Nothing) As List(Of SnesROM)
        '    Dim sourceDirectory As New DirectoryInfo(strRomPath)
        '    If filter Is Nothing Then filter = Function(r As SnesROM) True


        '    Dim i As IEnumerable(Of SnesROM) = (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                                               Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                                                           End Function)) Select z = New SnesROM(a.FullName, "") Where filter(z))

        '    If filter IsNot Nothing Then
        '        i = (From a In i Where filter(a))
        '    End If
        '    Return i.ToList
        'End Function
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New SnesROM(a, "")).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New SnesROM(a, "") Where filter(z)).ToList

        End If
    End Function

End Class
Public Class NesROM
    Implements MAME.IRom

    Dim _name As String
    Dim _path As String
    Dim _imagepath As String
    Dim _description As String
    Public Property Name As String Implements IRom.Name
        Get
            Return _name
        End Get
        Friend Set(value As String)
            _name = value
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return _imagepath
        End Get
        Set(value As String)
            _imagepath = value
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite

    Public Sub New(strName As String, strPath As String, strDescription As String, strImagePath As String)
        Me._description = strDescription
        Me._imagepath = strImagePath
        Me._name = strName
        Me._path = strPath
    End Sub
    Public Sub New(strPath As String)
        Me._path = strPath
        Me._name = IO.Path.GetFileNameWithoutExtension(strPath)
        Me._description = Me._name
    End Sub
    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.nes", Optional filter As Func(Of NesROM, Boolean) = Nothing) As List(Of NesROM)
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New NesROM(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New NesROM(a) Where filter(z)).ToList

        End If
    End Function
End Class
Public Class SmsRom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure Sms_Header
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public SMS() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public Copyright() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=48)> Public DomesticName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=48)> Public OverseasName() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=14)> Public SerialNumber() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> Public RomChecksum() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)> Public IOSupport As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public RomStart As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public RamStart As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> Public ExtraMemory As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> Public ModemSupport As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=40)> Public Unused1 As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> Public RegionSupport As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=13)> Public Unused2 As Byte()
    End Structure
    Dim header As New Sms_Header
    Dim _path As String
    Public Sub New(path As String)
        _path = path
        Dim f = IO.File.OpenRead(path)
        Dim rawData(Marshal.SizeOf(header)) As Byte
        f.Seek(&H100, SeekOrigin.Begin)
        f.Read(rawData, 0, UBound(rawData))
        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(Sms_Header))

        Finally

            handle.Free()
        End Try
    End Sub
    Public ReadOnly Property BadHeader As Boolean
        Get
            Return (Text.Encoding.ASCII.GetChars(header.SMS, 0, 4) <> "SEGA" OrElse header.SMS(4) <> &H20 OrElse Text.Encoding.ASCII.GetChars(header.Copyright, 0, 3) <> "(C)")
        End Get
    End Property
    Public Property Name As String Implements IRom.Name
        Get
            If BadHeader Then
                Return IO.Path.GetFileNameWithoutExtension(_path)
            End If

            Dim enc = System.Text.Encoding.GetEncoding("shift-jis")

            Dim s As String
            If header.DomesticName.All(Function(c) c = 32) Then
                If header.OverseasName.All(Function(c) c = 32) Then
                    s = IO.Path.GetFileNameWithoutExtension(_path)
                Else
                    s = enc.GetString(header.OverseasName)
                End If
            Else
                If header.OverseasName.All(Function(c) c = 32) Then
                    s = enc.GetString(header.DomesticName)
                Else

                    s = enc.GetString(header.OverseasName)
                End If
            End If
            Return s.Trim
            ' If String.IsNullOrEmpty(s) OrElse header.DomesticName.Any(Function(c) c < 32 Or c > 126) Then
            ' Return System.Text.Encoding.ASCII.GetString(header.OverseasName)
            ' Else
            ' Return s
            ' End If

        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite

    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.sms;*.gen;*.32x;*.bin", Optional filter As Func(Of SmsRom, Boolean) = Nothing) As List(Of SmsRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function) Where Not (a.FullName.Contains("(Track") And Not a.FullName.Contains("(Track 1)"))) Select New SmsRom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Where Not (a.FullName.Contains("(Track") And Not a.FullName.Contains("(Track 1)")) Select z = New SmsRom(a.FullName) Where filter(z)).ToList
        '    'Return From a In i Where filter(a) Select a
        'End If

        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Where Not (a.Contains("(Track") And Not a.Contains("(Track 1)")) Select New SmsRom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Where Not (a.Contains("(Track") And Not a.Contains("(Track 1)")) Select z = New SmsRom(a) Where filter(z)).ToList

        End If
    End Function
End Class
Public Class GBRom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GB_Header
        '0134
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=15)> Public Name As Byte()
        Public GBCFlag As Byte
        Public NewLicensee As Short
        Public SGBFeatures As Byte
        Public CartType As Byte
        Public RomSize As Byte
        Public SaveRamSize As Byte
        Public CountryCode As Byte
        Public Licensee As Byte
        Public HeaderCheck As Byte
        Public GlobalCheck As Short

    End Structure
    Dim header As New GB_Header
    Dim _path As String
    Public Sub New(path As String)
        _path = path
        Dim rawData(Marshal.SizeOf(header)) As Byte
        Dim f = IO.File.OpenRead(path)
        f.Seek(&H134, SeekOrigin.Begin)
        f.Read(rawData, 0, UBound(rawData))
        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(GB_Header))

        Finally

            handle.Free()
        End Try

    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Return System.Text.Encoding.ASCII.GetString(header.Name)
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.gb;*.gbc", Optional filter As Func(Of GBRom, Boolean) = Nothing) As List(Of GBRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New GBRom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New GBRom(a.FullName) Where filter(z)).ToList

        'End If

        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New GBRom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New GBRom(a) Where filter(z)).ToList

        End If
    End Function
End Class
Public Class GBARom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GBA_Header
        'http://nocash.emubase.de/gbatek.htm
        'http://cdn.preterhuman.net/texts/gaming_and_diversion/Gameboy%20Advance%20Programming%20Manual%20v1.1.pdf
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> Public rom_Entry As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=156)> Public NintendoLogo As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> Public GameTitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> Public GameCode As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> Public MakerCode As Byte()
        Public Reserved0000 As Byte
        Public MainUnitCode As Byte
        Public DeviceType As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> Public Reserved0001 As Byte()
        Public SoftwareVersion As Byte
        Public ComplementCheck As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> Public Reserved0002 As Byte()

    End Structure
    Dim header As New GBA_Header
    Dim _path As String
    Public Sub New(path As String)
        _path = path
        Dim f = IO.File.OpenRead(path)

        Dim rawData(Marshal.SizeOf(header)) As Byte

        f.Read(rawData, 0, UBound(rawData))
        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(GBA_Header))

        Finally

            handle.Free()
        End Try

    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Dim s = System.Text.Encoding.ASCII.GetString(header.GameTitle).Trim
            Return System.Text.RegularExpressions.Regex.Replace(s, "\x00+", " ")
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.gba", Optional filter As Func(Of GBARom, Boolean) = Nothing) As List(Of GBARom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New GBARom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New GBARom(a.FullName) Where filter(z))

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New GBARom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New GBARom(a) Where filter(z)).ToList

        End If
    End Function
End Class
Public Class DSRom
    Implements MAME.IRom

    Public Structure DS_Header
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> Public ShortName As Byte()
        Public GameID As Int32
        Public Publisher As Short
        Public UnitCode As Byte
        Public EncryptionSeed As Byte
        Public CartSize As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=9)> Public Unused As Byte()
        Public ROMVersion As Byte
        Public AutostartFlag As Byte
        Public ARM9BinOffset As Int32
        Public ARM9ExeAddr As Int32
        Public ARM9RAMAddr As Int32
        Public ARM9BinLen As Int32
        Public ARM7BinOffset As Int32
        Public ARM7ExeAddr As Int32
        Public ARM7RAMAddr As Int32
        Public ARM7BinLen As Int32
        Public FileTableOffset As Int32
        Public FileTableLen As Int32
        Public FATOffset As Int32
        Public FATLen As Int32
        Public ARM9OverlayOffset As Int32
        Public ARM9OverlayLen As Int32
        Public ARM7OverlayOffset As Int32
        Public ARM7OverlayLen As Int32
        Public BusTiming As Int32
        Public BusTimingDecrypt As Int32
        Public IconOffset As Int32
        Public CRC16 As Short
        Public SecureTimeOut As Short
        Public ARM9Ptr As Int32
        Public ARM7Ptr As Int32
        Public SecureDisable As Int64
        Public RomLength As Int32
        Public RomHeaderLen As Int32
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=56)> Public unused2 As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=156)> Public NintendoLogo As Byte()
        Public NintendoLogoCRC As Int16
        Public HeaderCRC16 As Int16
    End Structure
    Public Structure DSIcon
        Public Version As Int16
        Public CRC16 As Int16
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=28)> Public unused As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)> Public IconData As Byte()
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=32)> Public IconPalette As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)> Public IconPalette As Int16()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public JTitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public ETitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public FrTitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public GTitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public ITitle As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=256)> Public SpTitle As Byte()

        Public ReadOnly Property EnglishTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(ETitle)
            End Get
        End Property
        Public ReadOnly Property JapaneseTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(JTitle)
            End Get
        End Property
        Public ReadOnly Property FrenchTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(FrTitle)
            End Get
        End Property
        Public ReadOnly Property GermanTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(GTitle)
            End Get
        End Property
        Public ReadOnly Property ItalianTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(ITitle)
            End Get
        End Property
        Public ReadOnly Property SpanishTitle() As String
            Get
                Return System.Text.Encoding.Unicode.GetString(SpTitle)
            End Get
        End Property
        'Private Function createMask(a As Integer, b As Integer) As Integer

        '    Dim r As Integer = 0
        '    For i As Integer = a To b
        '        r = r Or (1 << i)
        '    Next
        '    Return r
        'End Function
        Public Function GetPalette() As Color()
            Dim p(15) As Color
            Dim maskR, maskG, maskB As Short
            maskR = &H7C00 'createMask(0, 4)
            maskG = &H3E0 'createMask(5, 9)
            maskB = &H1F ' createMask(10, 14)
            Dim rMax, gMax, bMax As Byte
            For t As Integer = 1 To 15
                Dim r As Byte = ((maskR And IconPalette(t)) >> 10)
                r = (r / 32) * 255
                rMax = Math.Max(rMax, r)
                Dim g As Byte = ((maskG And IconPalette(t)) >> 5)
                g = (g / 32) * 255
                gMax = Math.Max(gMax, g)
                Dim b As Byte = ((maskB And IconPalette(t)))
                b = (b / 32) * 255
                bMax = Math.Max(bMax, b)
                p(t) = Color.FromArgb(b, g, r)
            Next
            Return p
        End Function
        Public Function ShowPalette()
            '4x4 so 
            Const scale = 40
            Dim p = GetPalette()

            Dim b As New Bitmap(4 * scale, 4 * scale)
            Using g As Graphics = Graphics.FromImage(b)
                For y As Integer = 0 To 3
                    For x As Integer = 0 To 3
                        Using br As New SolidBrush(p((y * 4) + x))
                            g.FillRectangle(br, New Rectangle(x * scale, y * scale, scale, scale))
                        End Using
                    Next
                Next
            End Using
            Return b
        End Function
        Public ReadOnly Property GetIcon As Bitmap
            Get
                Dim b As New Bitmap(32, 32)
                Dim pal = GetPalette()
                'Using g As Graphics = Graphics.FromImage(b)
                Dim dumbcounter As Integer

                For tileY As Integer = 0 To 3
                    For tileX As Integer = 0 To 3
                        For y As Integer = 0 To 7
                            For x As Integer = 0 To 7 Step 2
                                Dim add = ((x / 8)) * 8
                                'Dim i2 = (add + (y * 8) + (tileX * 4) + (tileY * 4))
                                Dim i = dumbcounter
                                'MsgBox(i & vbCrLf & i2)
                                '0 1 2 3
                                'If dumbcounter >= 512 Then Exit For
                                Dim l = IconData(i).LoNibble
                                Dim h = IconData(i).HiNibble
                                If l > 0 Then b.SetPixel((tileX * 8) + x, (tileY * 8) + y, pal(l))
                                If h > 0 Then b.SetPixel((tileX * 8) + 1 + x, (tileY * 8) + y, pal(h))
                                dumbcounter += 1

                            Next
                        Next
                    Next
                Next
                On Error Resume Next
                'End Using
                Return b
            End Get
        End Property

    End Structure

    Dim header As DS_Header
    Dim Icon As DSIcon
    Dim _path As String
    Dim _imagePath As String

    Public Sub New(path As String)
        header = New DS_Header
        _path = path
        Dim f = IO.File.OpenRead(path)
        Dim rawData(Marshal.SizeOf(header)) As Byte
        f.Read(rawData, 0, UBound(rawData))

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(DS_Header))

        Finally

            handle.Free()
        End Try

        f.Seek(header.IconOffset, IO.SeekOrigin.Begin)
        ReDim rawData(Marshal.SizeOf(Icon))
        f.Read(rawData, 0, UBound(rawData))

        handle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            Icon = Marshal.PtrToStructure(rawDataPtr, GetType(DSIcon))

        Finally

            handle.Free()
        End Try

        f.Close()
    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Return Icon.EnglishTitle.Replace(vbNullChar, "").Trim
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return _imagePath
        End Get
        Set(value As String)
            _imagePath = value
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public ReadOnly Property EmbeddedImage As Bitmap
        Get
            Return Icon.GetIcon
        End Get
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.nds;*.ds;*.gba;*.fig", Optional filter As Func(Of DSRom, Boolean) = Nothing) As List(Of DSRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New DSRom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New DSRom(a.FullName) Where filter(z)).ToList

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New DSRom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New DSRom(a) Where filter(z)).ToList

        End If
    End Function
End Class


Public Class WiiRom
    Implements MAME.IRom

    Dim _Name As String
    Dim _Path As String
    Dim _imagePath As String = Nothing
    Public Sub New(strPath As String)
        _Path = strPath
        Dim f = IO.File.OpenRead(strPath)
        Dim rawData(255) As Byte
        f.Seek(&H20, SeekOrigin.Begin)
        f.Read(rawData, &H0, UBound(rawData))
        _Name = System.Text.Encoding.UTF8.GetString(rawData).Replace(vbNullChar, "").Trim


    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Return _Name
        End Get
        Private Set(value As String)
            _Name = value
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _Path
        End Get
        Private Set(value As String)
            _Path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return _imagePath
        End Get
        Set(value As String)
            _imagePath = value
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return _Name
        End Get
        Private Set(value As String)
            _Name = value
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.iso", Optional filter As Func(Of WiiRom, Boolean) = Nothing) As List(Of WiiRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New WiiRom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New WiiRom(a.FullName) Where filter(z)).ToList

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New WiiRom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New WiiRom(a) Where filter(z)).ToList

        End If
    End Function
End Class

Public Class N64Rom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure N64_Header
        Public Validate As Short
        Public Compression As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> Public reserved As Byte() '400000000Fh
        Public EntryPoint As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> Public reserved2 As Byte() '000014h
        Public unknown As Byte
        Public CRC1 As Integer
        Public CRC2 As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public ununsed As Byte() '0000000000000000h
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=20)> Public ImageName As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> Public ununsed2 As Byte() '0000000000000000h
        Public Manufacturer As Byte
        Public CartID As Short
        Public CountryCode As Byte
    End Structure
    Dim header As New N64_Header
    Dim _path As String
    Public Sub New(path As String)
        _path = path
        Dim f = IO.File.OpenRead(path)
        Dim rawData(Marshal.SizeOf(header)) As Byte
        f.Read(rawData, 0, UBound(rawData))


        Select Case rawData(0)
            Case &H37
                'have to swap bytes
                Dim temp As Byte
                For t As Integer = 0 To UBound(rawData) Step 2
                    temp = rawData(t)
                    rawData(t) = rawData(t + 1)
                    rawData(t + 1) = temp
                Next
            Case Else

        End Select


        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(N64_Header))

        Finally

            handle.Free()
        End Try

    End Sub

    Public Property Name As String Implements IRom.Name
        Get
            Return System.Text.Encoding.ASCII.GetString(header.ImageName)
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.z64;*.rom;*.n64;*.v64", Optional filter As Func(Of N64Rom, Boolean) = Nothing) As List(Of N64Rom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New N64Rom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New N64Rom(a.FullName) Where filter(z)).ToList


        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New N64Rom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New N64Rom(a) Where filter(z)).ToList

        End If
    End Function
End Class
Public Class PSxRom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure PS_Header
        Public VolumeDescriptorType As Byte '&h1
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=5)> Public StandardID As Byte() 'CD001
        Public VolumeDescriptorVersion As Byte '&h1
        Public reserved As Byte '0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> Public SystemID As Byte() 'PLAYSTATION
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> Public VolumeID As Byte() 'max 8 char for psx?
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public reserved2 As Byte() '0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public VolumeSpaceSize As Byte() '2x32bit, number of logical blocks
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=32)> Public reserved3 As Byte() '0
        Public VolumeSetSize As Integer '(2x16bit) 0001
        Public VolumeSequenceNumber As Integer '(2x16bit) 0001
        Public LogicalBlockSizeInBytes As Integer '(2x16bit) 0800 (1 sector)
        Public PathTableSizeInBytes As Int64 '(2x32bit) max 800 for psx)
        Public PathTable1BlockNumber As Integer '32bit little-endian
        Public PathTable2BlockNumber As Integer '32bit little-endian or 0=none
        Public PathTable3BlockNumber As Integer '32bit big-endian
        Public PathTable4BlockNumber As Integer '32bit big-endian or 0=none
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=34)> Public RootDirectoryRecord As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=128)> Public VolumeSetID As Byte() '(d-characters) (usually empty)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=128)> Public PublisherID As Byte() '(a-characters) (company name)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=128)> Public DataPreparerID As Byte() '(a-characters) (empty or other)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=128)> Public ApplicationID As Byte() 'PLAYSTATION
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=37)> Public CopyrightFilename As Byte() 'FILENAME.EXT;VER
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=37)> Public AbstractFilename As Byte() 'FILENAME.EXT;VER
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=37)> Public BibliographicFilename As Byte() 'FILENAME.EXT;VER
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Public VolumeCreationTimestamp As Byte() 'YYYYMMDDHHMMSSFF, timezone
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Public VolumeModificationTimestamp As Byte() '000000000000, 00
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Public VolumeExpirationTimestamp As Byte() '000000000000, 00
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=17)> Public VolumeEffectiveTimestamp As Byte() '000000000000, 00
        Public FileStructureVersion As Byte '1=standard
        'Public reserved4 As Byte
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=141)> Public ApplicationUseArea As Byte()
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=8)> Public ApplicationUseArea2 As Byte() 'CD-XA001
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=363)> Public ApplicationUseArea3 As Byte()
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=653)> Public reserved5 As Byte()

    End Structure


    Dim header As New PS_Header
    Dim _path As String
    Public Sub New(path As String, Optional byteOffset As Integer = &H9318)
        _path = path
        Dim rawData(Marshal.SizeOf(header)) As Byte
        Dim f = IO.File.OpenRead(path)
        'Select Case IO.Path.GetExtension(path).ToLower
        '    Case ".bin"
        '        '&h1318
        '        'f.Seek(&H9318, IO.SeekOrigin.Begin)
        '        f.Seek(byteOffset + &H1318, IO.SeekOrigin.Begin)
        '    Case Else
        'f.Seek(&H8000, IO.SeekOrigin.Begin)
        f.Seek(byteOffset, IO.SeekOrigin.Begin)
                'End Select
                'f.Seek(byteOffset, IO.SeekOrigin.Begin)
                f.Read(rawData, 0, UBound(rawData))
        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(PS_Header))

        Finally

            handle.Free()
        End Try

    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Dim s = System.Text.Encoding.ASCII.GetString(header.VolumeID).Replace(vbNullChar, "").Trim  'MyBase.Name
            If String.IsNullOrEmpty(s) Then Return IO.Path.GetFileNameWithoutExtension(_path)
            Return s
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcherPSx(strRomPath As String, Optional extensions As String = "*.iso;*.bin;*.img;*.mds", Optional filter As Func(Of PSxRom, Boolean) = Nothing) As List(Of PSxRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New PSxRom(a.FullName, &H9318)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New PSxRom(a.FullName, &H9318) Where filter(z)).ToList

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New PSxRom(a, &H9318)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New PSxRom(a, &H9318) Where filter(z)).ToList

        End If
    End Function

    Public Shared Function CreateSearcherPS2(strRomPath As String, Optional extensions As String = "*.iso;*.bin;*.img;*.mds", Optional filter As Func(Of PSxRom, Boolean) = Nothing) As List(Of PSxRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New PSxRom(a.FullName, &H8000)).ToList

        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New PSxRom(a.FullName, &H8000) Where filter(z)).ToList

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New PSxRom(a, &H8000)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New PSxRom(a, &H8000) Where filter(z)).ToList

        End If
    End Function
    Public Shared Function CreateSearcherPSP(strRomPath As String, Optional extensions As String = "*.iso;*.bin;*.img;*.mds", Optional filter As Func(Of PSxRom, Boolean) = Nothing) As List(Of PSxRom)
        Return CreateSearcherPS2(strRomPath, extensions, filter)
    End Function

End Class
Public Class PS3Rom
    Implements IRom

    'https://www.psdevwiki.com/ps3/PARAM.SFO
    '20 bytes
    <StructLayout(LayoutKind.Sequential, Pack:=1, Size:=&H14)>
    Private Structure SFO_Header
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> Dim Magic As Byte()
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)> Dim version As Byte()
        Dim keyTableStart As Int32
        Dim dataTableStart As Int32
        Dim numberOfEntries As Int32
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1, Size:=&H10)>
    Private Structure SFO_IndexEntry
        Dim keyOffset As Int16
        Dim dataFmt As SFO_INDEX_TABLE_ENTRY_FMT
        Dim dataLen As Int32
        Dim dataMaxLen As Int32
        Dim dataOffset As Int32
    End Structure
    Private Enum SFO_INDEX_TABLE_ENTRY_FMT As Int16
        UTF8_S = &H4
        UTF8 = &H204
        INT32 = &H404
    End Enum


    Private Structure SFOParserResult
        Dim Category As String
        Dim Parental_Level As String
        Dim SAVEDATA_DETAIL As String
        Dim SAVEDATA_DIRECTORY As String
        Dim SAVEDATA_FILE_LIST As String
        Dim SAVEDATA_PARAMS As String
        Dim SAVEDATA_TITLE As String
        Dim TITLE As String
    End Structure

    Dim SFO As New Dictionary(Of String, Object)
    Dim _Path As String
    Dim _imagePath As String
    Dim Header As New SFO_Header
    Dim Indicies() As SFO_IndexEntry
    Dim KEYS() As String
    Dim DATA() As String

    Public Sub New(sPath As String)
        _Path = sPath
        '        Dim d = New IO.DirectoryInfo(sPath)
        'SFO.Add("TITLE", d.Name)
        _imagePath = IO.Path.Combine(sPath, "PS3_GAME\ICON0.PNG")

        If Not IO.File.Exists(IO.Path.Combine(Path, "PS3_GAME\PARAM.SFO")) Then Return
        'Dim f = IO.File.OpenRead(IO.Path.Combine(Path, "PS3_GAME\PARAM.SFO"))
        'Dim rawData(f.Length) As Byte
        'f.Read(rawData, 0, UBound(rawData))
        'f.Close()
        Using ms As New MemoryStream(IO.File.ReadAllBytes(IO.Path.Combine(Path, "PS3_GAME\PARAM.SFO")))
            SFO = SFOParser.ReadSfo(ms)
        End Using


        'Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        'Try


        '    Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
        '    Header = Marshal.PtrToStructure(rawDataPtr, GetType(SFO_Header))
        '    rawDataPtr = IntPtr.Add(rawDataPtr, &H14)

        '    ReDim Indicies(Header.numberOfEntries - 1)
        '    For t As Integer = 0 To Header.numberOfEntries - 1
        '        Indicies(t) = New SFO_IndexEntry
        '        Indicies(t) = Marshal.PtrToStructure(rawDataPtr, GetType(SFO_IndexEntry))
        '        rawDataPtr = IntPtr.Add(rawDataPtr, 16)
        '    Next
        '    Dim bytePosition = 0
        '    'KEY_TABLE
        '    ReDim KEYS(Header.numberOfEntries - 1)
        '    ReDim DATA(Header.numberOfEntries - 1)
        '    For t As Integer = 0 To Header.numberOfEntries - 1
        '        Select Case Indicies(t).dataFmt
        '            Case SFO_INDEX_TABLE_ENTRY_FMT.UTF8_S
        '                'not null terminated

        '                KEYS(t) = Marshal.PtrToStringAnsi(rawDataPtr, Indicies(t + 1).dataOffset)
        '                SFO.Add(KEYS(t), "")
        '                rawDataPtr = IntPtr.Add(rawDataPtr, Len(KEYS(t)))

        '            Case SFO_INDEX_TABLE_ENTRY_FMT.UTF8
        '                'is null terminated
        '                If t < Header.numberOfEntries - 1 Then
        '                    ' KEYS(t) = Marshal.PtrToStringAnsi(rawDataPtr, Indicies(t + 1).keyOffset - bytePosition - 1)
        '                    KEYS(t) = Marshal.PtrToStringAnsi(rawDataPtr, Indicies(t + 1).keyOffset - bytePosition - 0)
        '                Else
        '                    KEYS(t) = Marshal.PtrToStringAnsi(rawDataPtr, Header.dataTableStart - (bytePosition + Header.keyTableStart) - 1)
        '                End If
        '                SFO.Add(KEYS(t), "")
        '                bytePosition += Len(KEYS(t)) + 1
        '                rawDataPtr = IntPtr.Add(rawDataPtr, Len(KEYS(t)) + 1)

        '            Case SFO_INDEX_TABLE_ENTRY_FMT.INT32
        '                'fixed 4 bytes
        '                KEYS(t) = Marshal.PtrToStructure(rawDataPtr, GetType(Int32))
        '                SFO.Add(KEYS(t), "")
        '                rawDataPtr = IntPtr.Add(rawDataPtr, 4)
        '        End Select
        '    Next
        '    'padding
        '    rawDataPtr = IntPtr.Add(rawDataPtr, bytePosition Mod 4)
        '    'data_table
        '    For t As Integer = 0 To Header.numberOfEntries - 1
        '        DATA(t) = Marshal.PtrToStringAnsi(rawDataPtr, Indicies(t).dataLen - 1)
        '        SFO(KEYS(t)) = DATA(t)
        '        rawDataPtr = IntPtr.Add(rawDataPtr, Indicies(t).dataMaxLen)
        '    Next
        'Finally

        '    handle.Free()
        'End Try


    End Sub

    Private Class SFOParser
        Const PSF_TYPE_BIN As Integer = 0
        Const PSF_TYPE_STR As Integer = 2
        Const PSF_TYPE_VAL As Integer = 4
        Public Shared Function ReadSfo(sfo As Stream) As Dictionary(Of String, Object)
            Dim sfoValues As New Dictionary(Of String, Object)

            Dim Magic As UInt32 = ReadUInt32(sfo)
            Dim Version As UInt32 = ReadUInt32(sfo)
            Dim KeyOffset As UInt32 = ReadUInt32(sfo)
            Dim ValueOffset As UInt32 = ReadUInt32(sfo)
            Dim Count As UInt32 = ReadUInt32(sfo)

            If Magic = &H46535000 Then
                For i As Integer = 0 To Count - 1
                    Dim NameOffset As UInt16 = ReadUInt16(sfo)
                    Dim Aligment As Byte = CByte(sfo.ReadByte)
                    Dim [Type] As Byte = CByte(sfo.ReadByte)
                    Dim ValueSize As UInt32 = ReadUInt32(sfo)
                    Dim TotalSize As UInt32 = ReadUInt32(sfo)
                    Dim DataOffset As UInt32 = ReadUInt32(sfo)

                    Dim keyLocation As Integer = Convert.ToInt32(KeyOffset + NameOffset)
                    Dim KeyName As String = ReadStringAt(sfo, keyLocation)
                    Dim ValueLocation As Integer = Convert.ToInt32(ValueOffset + DataOffset)
                    Dim Value As Object = "Unknown Type"

                    Select Case [Type]
                        Case PSF_TYPE_STR
                            Value = (ReadStringAt(sfo, ValueLocation).Replace(vbNullChar, ""))
                        Case PSF_TYPE_VAL
                            Value = ReadUint32At(sfo, ValueLocation + i)
                        Case PSF_TYPE_BIN
                            Value = ReadBytesAt(sfo, ValueLocation + i, Convert.ToInt32(ValueSize))
                    End Select
                    sfoValues(KeyName) = Value
                Next
            Else
                Throw New InvalidDataException("SFO Magic is invalid")
            End If
            Return sfoValues
        End Function

        Private Shared Sub CopyString(ByRef str As Byte(), Text As String, Index As Integer)
            Dim TextBytes = Encoding.UTF8.GetBytes(Text)
            Array.ConstrainedCopy(TextBytes, 0, str, Index, TextBytes.Length)
        End Sub
        Private Shared Sub CopyInt32(ByRef str As Byte(), Value As Int32, Index As Integer)
            Dim ValueBytes = BitConverter.GetBytes(Value)
            Array.ConstrainedCopy(ValueBytes, 0, str, Index, ValueBytes.Length)
        End Sub
        Private Shared Sub CopyInt32BE(ByRef str As Byte(), Value As Int32, index As Integer)
            Dim ValueBytes As Byte() = BitConverter.GetBytes(Value)
            Dim ValueBytesBE As Byte() = ValueBytes.Reverse().ToArray()
            Array.ConstrainedCopy(ValueBytesBE, 0, str, index, ValueBytesBE.Length)
        End Sub

        '// Read From Streams
        Private Shared Function ReadUInt32(ByRef Str As Stream) As UInt32
            Dim IntBytes(&H4 - 1) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToUInt32(IntBytes, &H0)
        End Function
        Private Shared Function ReadInt32(ByRef Str As Stream) As UInt32
            Dim IntBytes(&H4) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToUInt32(IntBytes, &H0)
        End Function
        Private Shared Function ReadUInt64(ByRef Str As Stream) As UInt64
            Dim IntBytes(&H8) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToUInt64(IntBytes, &H0)
        End Function
        Private Shared Function ReadInt64(ByRef Str As Stream) As Int64
            Dim IntBytes(&H8) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToInt64(IntBytes, &H0)
        End Function
        Private Shared Function ReadUInt16(ByRef Str As Stream) As UInt16
            Dim IntBytes(&H2 - 1) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToUInt16(IntBytes, &H0)
        End Function
        Private Shared Function ReadInt16(ByRef Str As Stream) As Int16
            Dim IntBytes(&H2) As Byte
            Str.Read(IntBytes, &H0, IntBytes.Length)
            Return BitConverter.ToInt16(IntBytes, &H0)
        End Function

        Private Shared Function ReadUint32At(ByRef Str As Stream, location As Integer) As UInt32
            Dim oldPos As Long = Str.Position
            Str.Seek(location, SeekOrigin.Begin)
            Dim outp As UInt32 = ReadUInt32(Str)
            Str.Seek(oldPos, SeekOrigin.Begin)
            Return outp
        End Function

        Private Shared Function ReadBytesAt(ByRef Str As Stream, location As Integer, length As Integer) As Byte()
            Dim oldPos As Long = Str.Position
            Str.Seek(location, SeekOrigin.Begin)
            Dim work_buf(length) As Byte
            Str.Read(work_buf, &H0, work_buf.Length)
            Str.Seek(oldPos, SeekOrigin.Begin)
            Return work_buf
        End Function

        Private Shared Function ReadStringAt(ByRef Str As Stream, location As Integer) As String
            Dim oldPos As Long = Str.Position
            Str.Seek(location, SeekOrigin.Begin)
            Dim outp As String = ReadString(Str)
            Str.Seek(oldPos, SeekOrigin.Begin)
            Return outp
        End Function
        Private Shared Function ReadString(ByRef Str As Stream) As String



            Dim MS As New MemoryStream()

            While (True)

                Dim c As Byte = CByte(Str.ReadByte())
                If (c = 0) Then Exit While

                MS.WriteByte(c)
            End While
            MS.Seek(&H0, SeekOrigin.Begin)
            Dim outp As String = Encoding.UTF8.GetString(MS.ToArray())
            MS.Dispose()
            Return outp
        End Function

        '// Write To Streams

        Private Shared Sub WriteUInt32(ByRef Str As Stream, Numb As UInt32)
            Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
            Str.Write(IntBytes, &H0, IntBytes.Length)
        End Sub
        Private Shared Sub WriteInt32(ByRef Str As Stream, Numb As Int32)
            Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
            Str.Write(IntBytes, &H0, IntBytes.Length)
        End Sub
        Private Shared Sub WriteUInt64(ByRef dst As Stream, value As UInt64)
            Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
            dst.Write(ValueBytes, &H0, &H8)
        End Sub
        Private Shared Sub WriteInt64(ByRef dst As Stream, value As Int64)
            Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
            dst.Write(ValueBytes, &H0, &H8)
        End Sub
        Private Shared Sub WriteUInt16(ByRef dst As Stream, value As UInt16)
            Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
            dst.Write(ValueBytes, &H0, &H2)
        End Sub
        Private Shared Sub WriteInt16(ByRef dst As Stream, value As Int16)
            Dim ValueBytes As Byte() = BitConverter.GetBytes(value)
            dst.Write(ValueBytes, &H0, &H2)
        End Sub

        Private Shared Sub WriteInt32BE(ByRef Str As Stream, Numb As Int32)
            Dim IntBytes As Byte() = BitConverter.GetBytes(Numb)
            Dim IntBytesBE As Byte() = IntBytes.Reverse().ToArray()
            Str.Write(IntBytesBE, &H0, IntBytesBE.Length)
        End Sub
        Private Shared Sub WriteString(ByRef Str As Stream, Text As String, Optional ByRef len As Integer = -1)
            If (len < 0) Then
                len = Text.Length
            End If

            Dim TextBytes As Byte() = Encoding.UTF8.GetBytes(Text)
            Str.Write(TextBytes, &H0, TextBytes.Length)
        End Sub


    End Class



    Public Property Name As String Implements IRom.Name
        Get
            If SFO.ContainsKey("TITLE") Then
                Return SFO("TITLE")
            Else
                Dim d = New IO.DirectoryInfo(_Path)
                Return d.Name
            End If
        End Get
        Private Set(value As String)
            SFO("TITLE") = value
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _Path
        End Get
        Friend Set(value As String)
            _Path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return _imagePath
        End Get
        Set(value As String)
            _imagePath = value
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite



    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "", Optional filter As Func(Of PS3Rom, Boolean) = Nothing) As List(Of PS3Rom)
        'This is assuming RPCS3
        If filter Is Nothing Then filter = Function(x As PS3Rom) True

        Dim l As New List(Of PS3Rom)
        Dim sourceDirectory = New DirectoryInfo(strRomPath)
        For Each subfolder In sourceDirectory.EnumerateDirectories()
            If subfolder.Name.StartsWith(".") Then Continue For
            Dim rom = New PS3Rom(subfolder.FullName)
            If filter(rom) Then l.Add(rom)
        Next
        Return l
    End Function
End Class


Public Class DCRom
    Implements MAME.IRom

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Private Structure DC_Header
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public HardWareID() As Byte 'SEGA SEGAKATANA "
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public MakerID() As Byte 'SEGA ENTERPRISES"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public DeviceInfo() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public AreaSymbols() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public Peripherals() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public ProductNumber() As Byte 'HDR-nnnn"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public VersionNumber() As Byte '   V1.00"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> Public ReleaseDate() As Byte 'yyyymmdd"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H14)> Public BootFilename() As Byte '1st_read.bin"
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> Public DiscProducer() As Byte 'SEGA LC-T-12    "
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H80)> Public Name() As Byte 'STREET FIGHTER 3 3RD STRIKE"
        'TOC immediately after




        'Public ProductType As Short
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=10)> Public ProductCode As Byte()
        'Public Checksum As Short
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=16)> Public IOSupport As Byte()
        'Public RomStart As Integer
        'Public RomEnd As Integer
        'Public RamStart As Integer
        'Public RamEnd As Integer
        'Public EnableSRAM As Short
        'Public unused As Byte
        'Public SRAMStart As Integer
        'Public SRAMEnd As Integer
        '<MarshalAs(UnmanagedType.ByValArray, sizeconst:=68)> Public Notes As Byte()

    End Structure
    Dim header As New DC_Header
    Dim _path As String
    Public Sub New(path As String)
        _path = path
        Dim rawData(Marshal.SizeOf(header)) As Byte
        Dim f = IO.File.OpenRead(path)
        f.Read(rawData, 0, UBound(rawData))
        f.Close()

        Dim handle As GCHandle = GCHandle.Alloc(rawData, GCHandleType.Pinned)
        Try

            Dim rawDataPtr As IntPtr = handle.AddrOfPinnedObject()
            header = Marshal.PtrToStructure(rawDataPtr, GetType(DC_Header))

        Finally

            handle.Free()
        End Try
    End Sub
    Public Property Name As String Implements IRom.Name
        Get
            Return System.Text.Encoding.ASCII.GetString(header.Name).Replace(vbNullChar, "").Trim
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Path As String Implements IRom.Path
        Get
            Return _path
        End Get
        Friend Set(value As String)
            _path = value
        End Set
    End Property

    Public Property ImagePath As String Implements IRom.ImagePath
        Get
            Return Nothing
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Description As String Implements IRom.Description
        Get
            Return Name
        End Get
        Private Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public ReadOnly Property EmbeddedImage As Bitmap
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Property Favorite As Boolean Implements IRom.Favorite


    Public Shared Function CreateSearcher(strRomPath As String, Optional extensions As String = "*.cdi;*.iso;*.bin", Optional filter As Func(Of DCRom, Boolean) = Nothing) As List(Of DCRom)
        'Dim sourceDirectory = New DirectoryInfo(strRomPath)
        'If filter Is Nothing Then
        '    Return (From a In (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                                   Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                               End Function)) Select New DCRom(a.FullName)).ToList
        'Else
        '    Return (From a In extensions.Split(";").SelectMany(Of FileInfo)(Function(b As String) As IEnumerable(Of FileInfo)
        '                                                                        Return sourceDirectory.EnumerateFiles(b, SearchOption.AllDirectories)
        '                                                                    End Function) Select z = New DCRom(a.FullName) Where filter(z)).ToList

        'End If
        If filter Is Nothing Then
            Return (From a In EnumerateFiles(strRomPath, extensions) Select New DCRom(a)).ToList
        Else
            Return (From a In EnumerateFiles(strRomPath, extensions) Select z = New DCRom(a) Where filter(z)).ToList

        End If
    End Function
End Class