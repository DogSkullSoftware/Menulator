Imports MAME

Public Class MenulatorListView
    Inherits Control

    Public Class GameRangeArgs
        Inherits EventArgs
        Friend Sub New(startgameindex As Integer, endgameindex As Integer)
            _start = startgameindex
            _end = endgameindex

        End Sub
        Private _start As Integer, _end As Integer
        Public ReadOnly Property StartIndex As Integer
            Get
                Return _start
            End Get
        End Property
        Public ReadOnly Property EndIndex As Integer
            Get
                Return _end
            End Get
        End Property
    End Class
    Public Event IndexRangechanged As EventHandler(Of GameRangeArgs)
    Public Event NeedInvalidate()

    Dim l As IEnumerable(Of IRom)
    Public Enum BitBucketState As Byte
        Unknown = 0
        Searching = 1
        Loading = 2
        Ready = 8
    End Enum
    Public Class BitBucketDictionary
        Inherits Concurrent.ConcurrentDictionary(Of String, Image)
        Private _states As New Concurrent.ConcurrentDictionary(Of String, BitBucketState)
        Public ReadOnly Property GetState(key As String) As BitBucketState
            Get
                If key Is Nothing OrElse Not _states.ContainsKey(key) Then Return BitBucketState.Unknown
                Return _states(key)
            End Get
        End Property
        Public Sub UpdateState(key As String, state As BitBucketState)
            _states.AddOrUpdate(key, state, Function(k, v) As BitBucketState
                                                Return state
                                            End Function)
        End Sub

    End Class
    Public bitBucket As New BitBucketDictionary

    Public BackgroundColor As Color = Color.FromArgb(45, 45, 48)
    Public ItemHilightColor As Color = SystemColors.MenuHighlight
    Public ItemBackground As Color = Color.FromArgb(51, 51, 55)
    Dim alphaBrush As New SolidBrush(Color.FromArgb(128, 0, 0, 0))

    Public Property GameList As IEnumerable(Of IRom)
        Get
            Return l
        End Get
        Set(value As IEnumerable(Of IRom))
            Dump()
            l = value
            Page = 0
        End Set
    End Property
    Public Property Searcher As XDocument

    Dim iconB As New SolidBrush(ItemBackground), iconH As New SolidBrush(ItemHilightColor), _forecolor As SolidBrush
    Public Sub New()
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.ResizeRedraw Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer, True)

        ItemSize = New Size(160, 200)
        ItemPadding = New Point(10, 10)
        'VerticalScroll.Visible = True
        Padding = New Padding(10, 10, 30, 30)
        'm = New MameXml() 
        'l = m.GetNextX(30)
        _forecolor = New SolidBrush(Me.ForeColor)


    End Sub
    Protected Overrides Sub OnForeColorChanged(e As EventArgs)
        MyBase.OnForeColorChanged(e)
        _forecolor = New SolidBrush(Me.ForeColor)
    End Sub
    Public Sub Dump()
        'l.Clear()
        'If m IsNot Nothing Then m.Dispose()
        _Index = -1
        _page = -1
        'm = Nothing
    End Sub
    'Private SuppressEvents As Boolean = False
    'Private ItemLocateRect As Rectangle
    Public Sub RefreshIndex(i As Integer)
        If l Is Nothing Then Return
        If i < 0 OrElse i > l.Count - 1 Then Return
        Dim x = i Mod ItemsPerX
        Dim y = (i - (_page * VisibleItemCount())) \ ItemsPerX
        Dim ItemLocate = New Point(Padding.Left + (x * (ItemSize.Width + ItemPadding.X + ItemSpacingX)),
                                                                                     Padding.Top + (y * (ItemSize.Height + ItemPadding.Y + ItemSpacingY)))
        ItemLocate.Offset(VisualOffset)
        Dim ItemLocateRect = New Rectangle(ItemLocate, ItemSize)

        'SuppressEvents = True
        Invalidate(ItemLocateRect)
        'SuppressEvents = False
    End Sub
    'Public Sub ReInit()
    '    'm = New MameXml()

    '    OnResize(Nothing)
    'End Sub
    'Public Sub ApplyFilter(f As Func(Of XElement, Boolean))
    '    ' m = New MameXml
    '    'l.Clear()
    '    _page = -1
    '    ' Index = -1
    '    'm.Init(f)

    '    OnResize(Nothing)
    '    'Index = 0
    'End Sub
    Dim _Index As Integer = -1
    Public Property Index As Integer
        Get
            Return _Index
        End Get
        Set(value As Integer)
            If value <= 0 Then
                value = 0
            End If
            If l IsNot Nothing AndAlso value >= l.Count - 1 Then
                value = l.Count - 1
            End If
            Dim oldIndex = _Index
            _Index = value
            Page = (_Index \ VisibleItemCount())

            RefreshIndex(oldIndex)
            RefreshIndex(_Index)
        End Set
    End Property
    Protected Overridable Sub OnNeedInvalidate()
        Invalidate()

        RaiseEvent NeedInvalidate()
    End Sub
    Protected Overridable Sub OnIndexRangeChanged(e As GameRangeArgs)
        RaiseEvent IndexRangechanged(Me, e)
    End Sub

    Public ReadOnly Property SelectedItem As MAME.IRom
        Get
            If l IsNot Nothing Then Return l(_Index) Else Return Nothing
        End Get
    End Property


    Dim _enableWait As Boolean
    Dim _spinThread As Threading.Thread
    Dim _Multi As Single = 2
    Dim waiter As New Threading.ManualResetEventSlim(False)
    Dim cancel As New Threading.CancellationTokenSource
    Public Property EnableWait As Boolean
        Get
            Return _enableWait
        End Get
        Set(value As Boolean)
            _enableWait = value
            If value Then
                'StartSpin()
                fAngle = 0
                If _spinThread Is Nothing Then
                    _spinThread = New Threading.Thread(AddressOf DoWork)
                    _spinThread.Start()
                    waiter.Set()
                Else
                    waiter.Set()
                End If
                'waiter.Set()
            Else
                waiter.Reset()
                'If _spinner IsNot Nothing Then _spinner.Dispose()
                '_spinner = Nothing
            End If
            OnResize(Nothing)
        End Set
    End Property
    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        cancel.Cancel(False)
        iconB.Dispose()
        iconH.Dispose()
        _forecolor.Dispose()
        alphaBrush.Dispose()

        'busy_back.Dispose()
        'busy_front.Dispose()
    End Sub
    Protected Sub DoWork(o As Object)
        'If Threading.Monitor.TryEnter(ParentInternal.RenderLock) Then
        Do
            Try
                waiter.Wait(cancel.Token)
            Catch ex As OperationCanceledException
                Exit Do
            End Try
            If cancel.IsCancellationRequested Then Exit Do
            AngleInc(fAngle, _Multi)
            'Threading.Monitor.Exit(ParentInternal.RenderLock)

            'Await Threading.Tasks.Task.Run(Sub()
            Dim imgSize = My.Resources.loading.Size
            Dim frontRect As New RectangleF((Bounds.Width / 2.0F) - (imgSize.Width / 2.0F), (Bounds.Height / 2.0F) - (imgSize.Height / 2.0F), imgSize.Width, imgSize.Height)
            'OnNeedsInvalidate({frontRect})

            'Dim x As New Form1.Rects({frontRect})
            'SendMessage(_frmHwnd, 10002, 1, x)
            Invalidate(Rectangle.Ceiling(frontRect))

            'End Sub)
            'End If
            Threading.Thread.Sleep(15)
        Loop
    End Sub

    Dim fAngle As Single = 0F
    Public Shared Function AngleInc(ByRef zAngle As Single, Optional ByVal multi As Single = 2) As Single

        zAngle += Math.PI * multi
        If zAngle > 360.0F Then zAngle = 0.0F
        Return zAngle
    End Function
    Dim _v As Point
    Public Property VisualOffset As Point
        Get
            Return _v
        End Get
        Set(value As Point)
            _v = value
            Invalidate()
        End Set
    End Property
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Threading.Thread.BeginCriticalRegion()
        'MyBase.OnPaint(e)
        'e.Graphics.Clear(BackgroundColor)
        e.Graphics.TranslateTransform(VisualOffset.X, VisualOffset.Y)
        If EnableWait Then
            Dim busyr As New Rectangle(Point.Empty, My.Resources.loading.Size)
            Dim r As New Rectangle((Size.Width / 2.0F) - (busyr.Width / 2.0F), (Size.Height / 2.0F) - (busyr.Height / 2.0F), busyr.Width, busyr.Height)
            'e.Graphics.DrawImage(My.Resources.loading, r, New Rectangle(0, 0, 89, 89), GraphicsUnit.Pixel)


            Dim m = e.Graphics.Transform
            m.RotateAt(fAngle, New PointF(Size.Width / 2.0F, Size.Height / 2.0F))
            e.Graphics.Transform = m
            e.Graphics.DrawImage(My.Resources.loading, r, New Rectangle(0, 0, busyr.Width, busyr.Height), GraphicsUnit.Pixel)
            m.Reset()
            e.Graphics.Transform = m
            m.Dispose()
            m = Nothing
            Threading.Thread.EndCriticalRegion()
            Exit Sub
        End If

        If l Is Nothing OrElse l.Count = 0 Then Return
        Dim sf As New StringFormat With {
        .Alignment = StringAlignment.Center,
        .LineAlignment = StringAlignment.Near,
        .Trimming = StringTrimming.EllipsisCharacter,
        .FormatFlags = StringFormatFlags.LineLimit Or StringFormatFlags.FitBlackBox
        }
        Dim sf_selected As New StringFormat With {
        .Alignment = StringAlignment.Center,
        .LineAlignment = StringAlignment.Near,
        .Trimming = StringTrimming.None,
        .FormatFlags = StringFormatFlags.NoClip Or StringFormatFlags.LineLimit Or StringFormatFlags.FitBlackBox
        }
        Dim menuParentRect As New RectangleF
        Dim starti As Integer = -1, endi As Integer = -1
        Dim _exit As Boolean
        Dim bIsMame As Boolean = TypeOf l(0) Is MAME.MameGame
        For y As Integer = 0 To IIf(ItemsPerY - 1 <= -1, 0, ItemsPerY - 1)
            For x As Integer = 0 To IIf(ItemsPerX - 1 <= -1, 0, ItemsPerX - 1)
                Dim _ForIndex As Integer = ((y * (ItemsPerX)) + x) + (Page * VisibleItemCount())
                If _ForIndex > l.Count - 1 Then _exit = True : Exit For
                If starti = -1 Then starti = _ForIndex
                endi = _ForIndex
                Dim ItemLocate = New Point(Padding.Left + (x * (ItemSize.Width + ItemPadding.X + ItemSpacingX)),
                                                                                                 Padding.Top + (y * (ItemSize.Height + ItemPadding.Y + ItemSpacingY)))
                Dim itemrect = New RectangleF(ItemLocate, ItemSize)
                If e.ClipRectangle.Contains(itemrect) Then
                    With l(_ForIndex)
                        e.Graphics.FillRectangle(IIf(Index = _ForIndex, iconH, iconB), itemrect)
                        If _ForIndex = _AppsMenuParentIndex Then
                            menuParentRect = itemrect
                        End If
                        'If bIsMame Then
                        '    If Not String.IsNullOrEmpty(DirectCast(l(_ForIndex), MAME.MameGame).CloneOf) Then
                        '    e.Graphics.FillRectangle(alphaBrush, itemrect)
                        '    End If
                        'End If



                        If bitBucket.GetState(.ImagePath) = BitBucketState.Ready Then
                            'bitBucket.TryGetValue(l(_ForIndex).ImagePath, ti)
                            e.Graphics.DrawImage(bitBucket(.ImagePath), New Point(ItemLocate.X + ItemPadding.X, ItemLocate.Y + ItemPadding.Y))
                            'New Rectangle(ItemLocate.X + ItemPadding.X, ItemLocate.Y + ItemPadding.Y,                        ItemSize.Width -(ItemPadding.X + ItemPadding.X), ItemSize.Height - (ItemPadding.Y + ItemPadding.Y) - 32))
                        ElseIf bitBucket.GetState(".") = BitBucketState.Ready Then
                            'bitBucket.TryGetValue(".", ti)
                            e.Graphics.DrawImage(bitBucket("."), New Point(ItemLocate.X + ItemPadding.X, ItemLocate.Y + ItemPadding.Y))
                        End If
                        'If ti IsNot Nothing Then
                        ' e.Graphics.DrawImage(ti, New Point(ItemLocate.X + ItemPadding.X, ItemLocate.Y + ItemPadding.Y))
                        ' End If


                        'e.Graphics.DrawString("#" & _ForIndex, Me.Font, Brushes.Black, ItemLocate)
                        If Index = _ForIndex Then
                            Dim f As New Font(Me.Font.FontFamily, Me.Font.Size)

                            Dim fit = e.Graphics.MeasureString(.Description, f, CSng(ItemSize.Width), sf_selected)
                            Dim TextSize As New SizeF(ItemSize.Width, 64)

                            Do Until fit.Height < TextSize.Height Or f.Size <= 1
                                f = New Font(f.FontFamily, f.Size - 1, FontStyle.Bold)
                                fit = e.Graphics.MeasureString(.Description, f, CSng(ItemSize.Width), sf_selected)
                            Loop

                            e.Graphics.DrawString(.Description, f, _forecolor, New Rectangle(ItemLocate.X, ItemLocate.Y + ItemSize.Height - 64, ItemSize.Width, 64), sf_selected)
                            f.Dispose()
                        Else
                            e.Graphics.DrawString(.Description, Me.Font, _forecolor, New Rectangle(ItemLocate.X, ItemLocate.Y + ItemSize.Height - 64, ItemSize.Width, 64), sf)
                        End If
                        'If l(_ForIndex).ImagePath IsNot Nothing AndAlso bitBucket.ContainsKey(l(_ForIndex).ImagePath) AndAlso bitBucket(l(_ForIndex).ImagePath) IsNot Nothing Then
                        'Dim ti As Image = Nothing

                        If .Favorite Then e.Graphics.DrawImage(My.Resources.favorite, New Rectangle(ItemLocate.X, ItemLocate.Y, 32, 32))
                    End With

                End If
            Next
            If _exit Then Exit For
        Next

        If menuParentRect <> RectangleF.Empty Then
            Dim menu_r = New Rectangle(menuParentRect.Right, menuParentRect.Y, 200, (AppsMenu.Count * 32) + (AppsMenu.Count * 4))
            e.Graphics.FillRectangle(SystemBrushes.ControlLight, menu_r)
            menu_r.Height = 32

            For x2 As Integer = 0 To AppsMenu.Count - 1
                If x2 = _AppsMenuIndex Then
                    e.Graphics.FillRectangle(iconH, New Rectangle(menu_r.X, menu_r.Y, menu_r.Width, menu_r.Height + 2))
                End If
                e.Graphics.DrawImage(AppsMenu(x2).Image, ScaleRect(AppsMenu(x2).Image.Size, New Rectangle(menu_r.X + 5, menu_r.Y + 2, 32, 32)))

                e.Graphics.DrawString(AppsMenu(x2).Text, Me.Font, Brushes.Black, New Rectangle(menu_r.X + 5 + 5 + 32, menu_r.Y + 2, menu_r.Width - 5 - 5 - 32, menu_r.Height))
                menu_r.Y += 32 + 4
            Next
        End If

        'draw thumb
        'Dim maxHeight = ((ItemsPerY * ItemSize.Height) + ((ItemsPerY - 1) * ItemSpacingY) + ItemPadding.Y)
        Dim thumbHeight = VisibleItemCount() / (l.Count + 1) * Height
            Dim maxPages = Math.Ceiling((l.Count + 1) / VisibleItemCount())
            Dim pageProgress = _page / (maxPages - 1)
            'Debug.Print(thumbHeight )
            e.Graphics.FillRectangle(_forecolor, New RectangleF(Width - 5, pageProgress * (Me.Height - thumbHeight), 5, thumbHeight))

        ' e.Graphics.Flush()

        'Using b As New Bitmap(Width, Height, e.Graphics) 'b As New Bitmap("D:\shares\pictures\300953_127284160700277_1222528_n.jpg")
        '    Me.Parent.DrawToBitmap(b, New Rectangle(Point.Empty, Size))
        '    Dim kernel = GaussianBlur(6, 6)
        '    Dim newbit = Convolve(b, kernel)
        '    e.Graphics.DrawImage(newbit, Point.Empty)
        'End Using

        'Dim rnd As New Random
        'Using b As New SolidBrush(Color.FromArgb(128, rnd.Next(128, 255), rnd.Next(128, 255), rnd.Next(128, 255)))
        '    e.Graphics.FillRectangle(b, e.ClipRectangle)
        'End Using
theend:
        Threading.Thread.EndCriticalRegion()

    End Sub

    'Public Shared Function GaussianBlur(length As Integer, weight As Double) As Double(,)

    '    Dim kernel(length, length) As Double
    '    Dim kernelSum As Double = 0
    '    Dim foff As Integer = (length - 1) / 2
    '    Dim distance As Double
    '    Dim constant As Double = 1D / (2 * Math.PI * weight * weight)
    '    For y = -foff To foff
    '        For x = -foff To foff
    '            distance = ((y * y) + (x * x)) / (2 * weight * weight)
    '            kernel(y + foff, x + foff) = constant * Math.Exp(-distance)
    '            kernelSum += kernel(y + foff, x + foff)
    '        Next
    '    Next
    '    For y = 0 To length
    '        For x = 0 To length
    '            kernel(y, x) = kernel(y, x) * 1D / kernelSum
    '        Next
    '    Next
    '    Return kernel
    'End Function

    'Public Shared Function Convolve(srcImage As Bitmap, kernel As Double(,)) As Bitmap

    '    Dim width = srcImage.Width
    '    Dim height = srcImage.Height
    '    Dim srcData As Imaging.BitmapData = srcImage.LockBits(New Rectangle(0, 0, width, height),
    '    Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
    '    Dim bytes As Integer = srcData.Stride * srcData.Height
    '    Dim buffer(bytes) As Byte
    '    Dim result(bytes) As Byte
    '    Runtime.InteropServices.Marshal.Copy(srcData.Scan0, buffer, 0, bytes)
    '    srcImage.UnlockBits(srcData)
    '    Dim colorChannels As Integer = 3
    '    Dim rgb(colorChannels) As Double
    '    Dim foff As Integer = (kernel.GetLength(0) - 1) / 2
    '    Dim kcenter As Integer
    '    Dim kpixel As Integer
    '    For y = foff To height - foff - 1

    '        For x = foff To width - foff - 1

    '            For c = 0 To colorChannels

    '                rgb(c) = 0.0
    '            Next
    '            kcenter = y * srcData.Stride + x * 4
    '            For fy = -foff To foff

    '                For fx = -foff To foff - 1

    '                    kpixel = kcenter + fy * srcData.Stride + fx * 4
    '                    For c = 0 To colorChannels

    '                        rgb(c) += CDbl((buffer(kpixel + c)) * kernel(fy + foff, fx + foff))
    '                    Next
    '                Next
    '            Next
    '            For c = 0 To colorChannels

    '                If (rgb(c) > 255) Then
    '                    rgb(c) = 255
    '                ElseIf (rgb(c) < 0) Then
    '                    rgb(c) = 0
    '                End If
    '            Next
    '            For c = 0 To colorChannels
    '                result(kcenter + c) = CByte(rgb(c))
    '            Next
    '            result(kcenter + 3) = 255
    '        Next
    '    Next
    '    Dim resultImage As New Bitmap(width, height)
    '    Dim resultData As Imaging.BitmapData = resultImage.LockBits(New Rectangle(0, 0, width, height),
    '    Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format32bppArgb)
    '    Runtime.InteropServices.Marshal.Copy(result, 0, resultData.Scan0, bytes)
    '    resultImage.UnlockBits(resultData)
    '    Return resultImage
    'End Function

    Public Sub SetDefaultGameIcon(image As Bitmap)

        bitBucket(".") = image
        bitBucket.UpdateState(".", BitBucketState.Ready)
    End Sub

    Public ReadOnly Property IconSize As SizeF
        Get
            Return New SizeF(ItemSize.Width - (ItemPadding.X + ItemPadding.X), ItemSize.Height - (ItemPadding.Y + ItemPadding.Y) - 64)
        End Get
    End Property
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)

        ' If m IsNot Nothing Then l.AddRange(m.GetNextX(((Page + 1) * VisibleItemCount()) - l.Count))
        If l IsNot Nothing Then RaiseEvent IndexRangechanged(Me, New GameRangeArgs(VisibleIndexRange(0), Math.Min(VisibleIndexRange(1), l.Count - 1)))
        OnNeedInvalidate()
    End Sub

    Public Property ItemSize As Size

    Public ReadOnly Property ItemsPerX() As Integer
        Get
            Return Math.Floor((ClientSize.Width + ItemPadding.X) / ((ItemSize.Width + ItemPadding.X)))
        End Get
    End Property
    Public ReadOnly Property ItemsPerY() As Integer
        Get
            Return Math.Floor((ClientSize.Height + ItemPadding.Y) / ((ItemSize.Height + ItemPadding.Y)))
        End Get
    End Property
    Public ReadOnly Property ItemPadding As Point

    Public ReadOnly Property ItemSpacingX As Single
        Get
            If ItemsPerX > 1 Then
                Dim OccupiedSpace = ((ItemsPerX) * ItemSize.Width) + (ItemPadding.X * (ItemsPerX - 1))
                Return (ClientSize.Width - OccupiedSpace) / (ItemsPerX - 1)
            Else
                Return 0
            End If
        End Get
    End Property
    Public ReadOnly Property ItemSpacingY As Single
        Get
            If ItemsPerY > 1 Then
                Dim OccupiedSpace = ((ItemsPerY) * ItemSize.Height) + (ItemPadding.Y * (ItemsPerY - 1))
                Return (ClientSize.Height - OccupiedSpace) / (ItemsPerY - 1)
            Else
                Return 0
            End If
        End Get
    End Property

    Public Function VisibleItemCount() As Integer
        Return ItemsPerX * ItemsPerY
    End Function
    Public Function VisibleIndexRange() As Integer()
        Dim x = Math.Floor(_Index / (VisibleItemCount()))
        Return {x * VisibleItemCount(), x * VisibleItemCount() + (VisibleItemCount())}
    End Function
    Dim _page As Integer = -1
    Public Sub NextPage()
        _page += 1
        OnResize(Nothing)
    End Sub
    Public Sub PrevPage()
        _page -= 1
        OnResize(Nothing)
    End Sub
    Public Property Page As Integer
        Get
            Return _page
        End Get
        Set(value As Integer)
            If value = _page Then Return
            If value < 0 Then
                value = 0
            End If
            'If value > MaxPages Then

            'End If
            If value - 1 = _page Then
                NextPage()
            ElseIf value + 1 = _page Then
                PrevPage()
            Else
                _page = value
                OnResize(Nothing)
            End If


            'Dim _exit As Boolean = False
            'Dim starti As Integer = 1, endi As Integer = -1
            'For y As Integer = 0 To IIf(ItemsPerY - 1 <= -1, 0, ItemsPerY - 1)
            '    For x As Integer = 0 To IIf(ItemsPerX - 1 <= -1, 0, ItemsPerX - 1)
            '        Dim _ForIndex As Integer = ((y * (ItemsPerX)) + x) + (Page * VisibleItemCount())
            '        If _ForIndex > l.Count - 1 Then _exit = True : Exit For
            '        If starti = -1 Then starti = _ForIndex
            '        endi = _ForIndex
            '    Next
            '    If _exit Then Exit For
            'Next

            Dim start2 As Integer, end2 As Integer
            start2 = _page * VisibleItemCount()
            end2 = start2 + VisibleItemCount() - 1

            'If starti <> start2 Or endi <> end2 Then
            '    Debug.Print("!")
            'End If

            If l IsNot Nothing Then OnIndexRangeChanged(New GameRangeArgs(start2, IIf(end2 >= l.Count - 1, l.Count - 1, end2)))

        End Set
    End Property

    Shadows Property ClientSize As Size
        Get
            Dim _r = MyBase.ClientSize
            _r.Width -= Padding.Horizontal
            _r.Height -= Padding.Vertical
            Return _r
        End Get
        Set(value As Size)
            MyBase.ClientSize = value
        End Set
    End Property

    Dim _AppsMenuParentIndex As Integer = -1
    Dim _AppsMenuIndex As Integer = 0
    Dim _AppsMenu As List(Of NewUIListItem)
    Public ReadOnly Property InAppsMenu As Boolean
        Get
            Return _AppsMenuParentIndex <> -1
        End Get
    End Property

    Public Property AppsMenu As List(Of NewUIListItem)
        Get
            Return _AppsMenu
        End Get
        Set(value As List(Of NewUIListItem))
            _AppsMenu = value
        End Set
    End Property


    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Right
                If _AppsMenuParentIndex = -1 Then
                    Index += 1
                Else
                End If
                e.Handled = True
            Case Keys.Left
                If _AppsMenuParentIndex = -1 Then
                    Index -= 1
                Else
                End If
                e.Handled = True
            Case Keys.Up
                If _AppsMenuParentIndex <> -1 Then
                    _AppsMenuIndex -= 1
                    If _AppsMenuIndex < 0 Then _AppsMenuIndex = 0
                    Refresh()
                Else
                    Index -= ItemsPerX
                End If
                e.Handled = True
            Case Keys.Down
                If _AppsMenuParentIndex <> -1 Then
                    _AppsMenuIndex += 1
                    If _AppsMenuIndex > AppsMenu.Count - 1 Then _AppsMenuIndex = AppsMenu.Count - 1
                    Refresh()
                Else
                    Index += ItemsPerX
                End If
                e.Handled = True
            Case Keys.Return
                If _AppsMenuParentIndex <> -1 Then
                    Dim result As New NewUIListItem.NewUIListItemClickedEvent
                    AppsMenu(_AppsMenuIndex).PerformClick(result)
                    If result.CloseMenu Then
                        _AppsMenuParentIndex = -1
                        _AppsMenuIndex = 0
                        Refresh()
                    End If
                    e.Handled = True
                    End If
                    Case Keys.Apps
                If _AppsMenuParentIndex = -1 Then
                    _AppsMenuParentIndex = _Index

                Else
                    _AppsMenuParentIndex = -1
                End If


                Refresh()
                e.Handled = True
            Case Keys.PageDown
                If _AppsMenuParentIndex = -1 Then
                    Index += VisibleItemCount()
                    'Page += 1
                    Refresh()
                    e.Handled = True
                End If
            Case Keys.PageUp
                If _AppsMenuParentIndex = -1 Then
                    Index -= VisibleItemCount()
                    'Page -= 1
                    Refresh()
                    e.Handled = True
                End If
            Case Keys.Home
                If _AppsMenuParentIndex = -1 Then
                    Index = 0
                    e.Handled = True
                End If
            Case Keys.End
                If _AppsMenuParentIndex = -1 Then
                    Index = l.Count - 1
                    e.Handled = True
                End If
            Case Keys.Escape
                If _AppsMenuParentIndex <> -1 Then
                    _AppsMenuParentIndex = -1
                    _AppsMenuIndex = 0
                    Refresh()
                    e.Handled = True
                End If
            Case Else
                MyBase.OnKeyDown(e)

        End Select
    End Sub
    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        Return False
    End Function
    Public Overrides Function PreProcessMessage(ByRef msg As Message) As Boolean
        Return False
    End Function
    Protected Overrides Function ProcessDialogKey(keyData As Keys) As Boolean
        Return False
    End Function


End Class

Public Module mduHelper
    <Runtime.CompilerServices.Extension> Public Function Contains(r As Rectangle, p As PointF) As Boolean
        Return r.Contains(p.X, p.Y)
    End Function

    <Runtime.CompilerServices.Extension> Public Function Contains(r As Rectangle, r2 As RectangleF) As Boolean
        Return r.Contains(New Rectangle(r2.X, r2.Y, r2.Width, r2.Height))
    End Function

End Module