Imports System.ComponentModel

Public Class NewUIListItem
    Dim _text As String = "Games"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DoubleBuffered = True
    End Sub
    Public Sub New(strText As String, img As Image, onclick As EventHandler)
        Me.New
        Text = strText
        Image = img
        ClickHandler = onclick
    End Sub
    Public Sub New(strText As String, img As Image, onclick As EventHandler, altonclick As EventHandler)
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
    Public Property ClickHandler As EventHandler
    Protected Overrides Sub OnClick(e As EventArgs)
        PerformClick()
    End Sub
    Public Sub PerformClick()
        If ClickHandler IsNot Nothing Then
            ClickHandler.Invoke(Me, Nothing)
        End If
    End Sub

    Public Property AltClickHandler As EventHandler

    Public Sub PerformAltClick()
        If AltClickHandler IsNot Nothing Then
            AltClickHandler.Invoke(Me, Nothing)
        End If
    End Sub

End Class
