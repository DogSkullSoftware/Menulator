Imports System.ComponentModel

Public Class NewUIListItem
    Dim _text As String = "Games"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DoubleBuffered = True
    End Sub
    Public Sub New(strText As String, img As Image, onclick As EventHandler(Of NewUIListItemClickedEvent))
        Me.New
        Text = strText
        Image = img
        ClickHandler = onclick
    End Sub
    Public Sub New(strText As String, img As Image, onclick As EventHandler(Of NewUIListItemClickedEvent), altonclick As EventHandler(Of NewUIListItemClickedEvent))
        Me.New
        Text = strText
        Image = img
        ClickHandler = onclick
        AltClickHandler = altonclick
    End Sub
    <Browsable(True), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Overrides Property Text As String
        Get
            Return _text
        End Get
        Set(value As String)
            _text = value
            Label14.Text = _text
        End Set
    End Property
    Public Property Image As Image
        Get
            Return Panel4.BackgroundImage
        End Get
        Set(value As Image)
            Panel4.BackgroundImage = value

        End Set
    End Property
    Public Property ClickHandler As EventHandler(Of NewUIListItemClickedEvent)
    Public Class NewUIListItemClickedEvent
        Inherits EventArgs
        Public CloseMenu As Boolean = True
        Public Handled As Boolean = True
        Public CloseAllMenu As Boolean = False

        Public Sub New()

        End Sub
        Public Sub New(a As NewUIListItemClickedEvent)
            Me.New()
            Me.CloseMenu = a.CloseMenu
            Me.Handled = a.Handled
        End Sub
    End Class
    Protected Overrides Sub OnClick(e As EventArgs)
        PerformClick(Nothing)
    End Sub
    Public Sub PerformClick(ByVal e As NewUIListItemClickedEvent)
        If e Is Nothing Then e = New NewUIListItemClickedEvent()
        If ClickHandler IsNot Nothing Then

            ClickHandler.Invoke(Me, e)
        Else
            e.CloseMenu = False
        End If
    End Sub

    Public Property AltClickHandler As EventHandler(Of NewUIListItemClickedEvent)

    Public Sub PerformAltClick(ByVal e As NewUIListItemClickedEvent)
        If AltClickHandler IsNot Nothing Then
            AltClickHandler.Invoke(Me, e)
        Else
            e.CloseMenu = False
        End If
    End Sub

End Class
