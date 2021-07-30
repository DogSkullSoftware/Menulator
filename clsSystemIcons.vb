Imports System.Runtime.InteropServices


Public Class Win32SystemIcons
    Public Enum ShellGetFileInfoFlags As Integer
        SHGFI_ADDOVERLAYS = &H20
        SHGFI_ATTR_SPECIFIED = &H20000
        SHGFI_ATTRIBUTES = &H800
        SHGFI_DISPLAYNAME = &H200
        SHGFI_EXETYPE = &H2000
        SHGFI_ICON = &H100
        SHGFI_ICONLOCATION = &H1000
        SHGFI_LARGEICON = &H0
        SHGFI_SMALLICON = &H1
        SHIL_JUMBO = &H4
        SHIL_EXTRALARGE = &H2
        SHGFI_LINKOVERLAY = &H8000
        SHGFI_OPENICON = &H2
        SHGFI_OVERLAYINDEX = &H40
        SHGFI_PIDL = &H8
        SHGFI_SELECTED = &H10000
        SHGFI_SHELLICONSIZE = &H4
        SHGFI_SYSICONINDEX = &H4000
        SHGFI_TYPENAME = &H400
        SHGFI_USEFILEATTRIBUTES = &H10
    End Enum


    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.[Unicode])>
    Public Structure SHFILEINFOW
        Public hIcon As IntPtr
        Public iIcon As Integer
        Public dwAttributes As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> Public szDisplayName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)> Public szTypeName As String
    End Structure
    <DllImport("shell32.dll", EntryPoint:="Shell_GetImageLists")>
    Private Shared Function Shell_GetImageLists(ByRef phimlLarge As IntPtr, ByRef lphimlSmall As IntPtr) As Boolean
    End Function
    <DllImport("shell32.dll", EntryPoint:="SHGetFileInfoW")>
    Private Shared Function SHGetFileInfoW(<InAttribute(), MarshalAs(UnmanagedType.LPTStr)> ByVal pszPath As String, ByVal dwFileAttributes As Integer, ByRef psfi As SHFILEINFOW, ByVal cbFileInfo As Integer, ByVal uFlags As ShellGetFileInfoFlags) As Integer
    End Function
    <DllImport("user32.dll", EntryPoint:="DestroyIcon")>
    Private Shared Function DestroyIcon(ByVal hIcon As System.IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    <DllImport("Kernel32.dll")>
    Public Shared Function CloseHandle(handle As IntPtr) As Boolean

    End Function
    Private Structure IMAGELISTDRAWPARAMS
        Public cbSize As Integer
        Public himl As IntPtr
        Public i As Integer
        Public hdcDst As IntPtr
        Public x As Integer
        Public y As Integer
        Public cx As Integer
        Public cy As Integer
        Public xBitmap As Integer '        // x offest from the upperleft Of bitmap
        Public yBitmap As Integer '        // y offset from the upperleft Of bitmap
        Public rgbBk As Integer
        Public rgbFg As Integer
        Public fStyle As Integer
        Public dwRop As Integer
        Public fState As Integer
        Public Frame As Integer
        Public crEffect As Integer
    End Structure
    <StructLayout(LayoutKind.Sequential)>
    Private Structure IMAGEINFO
        Public hbmImage As IntPtr
        Public hbmMask As IntPtr
        Public Unused1 As Integer
        Public Unused2 As Integer
        Public rcImage As Rectangle
    End Structure
#Region "Private ImageList COM Interop (XP)"
    <ComImport,
        Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Private Interface IImageList
        <PreserveSig>
        Function Add(hbmImage As IntPtr, hbmMask As IntPtr, ByRef pi As Integer) As Integer

        <PreserveSig>
        Function ReplaceIcon(i As Integer, hicon As IntPtr, ByRef pi As Integer) As Integer
        <PreserveSig>
        Function SetOverlayImage(iImage As Integer, iOverlay As Integer) As Integer

        <PreserveSig>
        Function Replace(i As Integer, hbmImage As IntPtr, hbmMask As IntPtr) As Integer

        <PreserveSig>
        Function AddMasked(hbmImage As IntPtr, crMask As Integer, ByRef pi As Integer) As Integer

        <PreserveSig>
        Function Draw(ByRef pimldp As IMAGELISTDRAWPARAMS) As Integer

        <PreserveSig>
        Function Remove(i As Integer) As Integer

        <PreserveSig>
        Function GetIcon(i As Integer, flags As Integer, ByRef picon As IntPtr) As Integer

        <PreserveSig>
        Function GetImageInfo(i As Integer, ByRef pImageInfo As IMAGEINFO) As Integer

        <PreserveSig>
        Function Copy(iDst As Integer, punkSrc As IImageList, iSrc As Integer, uFlags As Integer) As Integer

        <PreserveSig>
        Function Merge(i1 As Integer, punk2 As IImageList, i2 As Integer, dx As Integer, dy As Integer, ByRef riid As Guid, ByRef ppv As IntPtr) As Integer

        <PreserveSig>
        Function Clone(ByRef riid As Guid, ByRef ppv As IntPtr) As Integer
        <PreserveSig>
        Function GetImageRect(i As Integer, ByRef prc As Rectangle) As Integer

        <PreserveSig>
        Function GetIconSize(ByRef cx As Integer, ByRef cy As Integer) As Integer

        <PreserveSig>
        Function SetIconSize(cx As Integer, cy As Integer) As Integer

        <PreserveSig>
        Function GetImageCount(ByRef pi As Integer) As Integer

        <PreserveSig>
        Function SetImageCount(uNewCount As Integer) As Integer

        <PreserveSig>
        Function SetBkColor(clrBk As Integer, ByRef pclr As Integer) As Integer

        <PreserveSig>
        Function GetBkColor(ByRef pclr As Integer) As Integer

        <PreserveSig>
        Function BeginDrag(iTrack As Integer, dxHotspot As Integer, dyHotspot As Integer) As Integer

        <PreserveSig>
        Function EndDrag() As Integer

        <PreserveSig>
        Function DragEnter(hwndLock As IntPtr, x As Integer, y As Integer) As Integer

        <PreserveSig>
        Function DragLeave(hwndLock As IntPtr) As Integer

        <PreserveSig>
        Function DragMove(x As Integer, y As Integer) As Integer

        <PreserveSig>
        Function SetDragCursorImage(ByRef punk As IImageList, iDrag As Integer, dxHotspot As Integer, dyHotspot As Integer) As Integer

        <PreserveSig>
        Function DragShowNolock(fShow As Integer) As Integer

        <PreserveSig>
        Function GetDragImage(ByRef ppt As Point, ByRef pptHotspot As Point, ByRef riid As Guid, ByRef ppv As IntPtr) As Integer

        <PreserveSig>
        Function GetItemFlags(i As Integer, ByRef dwFlags As Integer) As Integer

        <PreserveSig>
        Function GetOverlayImage(iOverlay As Integer, ByRef piIndex As Integer) As Integer
    End Interface
#End Region
    <DllImport("shell32.dll", EntryPoint:="#727")>
    Private Shared Function SHGetImageList(iImageList As Integer, ByRef riid As Guid, ByRef ppv As IImageList) As Integer

    End Function
    <DllImport("Shell32.dll")>
    Public Shared Function SHGetFileInfo(pszPath As String, dwFileAttributes As Integer, ByRef psfi As SHFILEINFOW, cbFileInfo As Integer, uFlags As Integer) As Integer

    End Function

    <DllImport("Shell32.dll")>
    Public Shared Function SHGetFileInfo(pszPath As IntPtr, dwFileAttributes As Integer, ByRef psfi As SHFILEINFOW, cbFileInfo As Integer, uFlags As Integer) As Integer

    End Function

    <DllImport("shell32.dll", EntryPoint:="SHGetFileInfo")>
    Private Shared Function SHGetFileInfoAsImageList(pszPath As String, dwFileAttributes As Integer, ByRef psfi As SHFILEINFOW, cbFileInfo As Integer, uFlags As Integer) As IImageList

    End Function

    Dim _Path As String
    Dim fi As New SHFILEINFOW
    Public Function GetIcon(ByVal LargeIco As Boolean) As Bitmap
        Dim bm As Bitmap
        If LargeIco Then

            'SHGetFileInfoW(_Path, 0, fi, Marshal.SizeOf(fi), ShellGetFileInfoFlags.SHGFI_ICON Or 2 Or ShellGetFileInfoFlags.SHGFI_SYSICONINDEX Or ShellGetFileInfoFlags.SHGFI_ADDOVERLAYS)
            SHGetFileInfo(_Path, &H80, fi, Marshal.SizeOf(fi), ShellGetFileInfoFlags.SHGFI_SYSICONINDEX)
            Dim iconindex = fi.iIcon



            Dim iidImageList = New Guid("46EB5926-582E-4017-9FDF-E8998DAA0950")
            'Dim iidImageList = New Guid("192B9D83-50FC-457B-90A0-2B82A8B5DAE1")
            Dim iml As IImageList = Nothing
            SHGetImageList(ShellGetFileInfoFlags.SHIL_JUMBO, iidImageList, iml)
            Dim hIcon As IntPtr
            iml.GetIcon(iconindex, 1, hIcon)
            bm = System.Drawing.Icon.FromHandle(hIcon).ToBitmap
            DestroyIcon(hIcon)


        Else
            SHGetFileInfoW(_Path, 0, fi, Marshal.SizeOf(fi), ShellGetFileInfoFlags.SHGFI_ICON Or ShellGetFileInfoFlags.SHGFI_SMALLICON Or ShellGetFileInfoFlags.SHGFI_ADDOVERLAYS)

            bm = Icon.FromHandle(fi.hIcon).ToBitmap
            DestroyIcon(fi.hIcon)
        End If
        Return bm
    End Function



    Public Sub New(sPath As String)
        fi = New SHFILEINFOW
        _Path = sPath
    End Sub
    Public ReadOnly Property DisplayName As String
        Get
            fi = New SHFILEINFOW
            SHGetFileInfoW(_Path, 0, fi, Marshal.SizeOf(fi), ShellGetFileInfoFlags.SHGFI_DISPLAYNAME)
            Return fi.szDisplayName
        End Get
    End Property


End Class
