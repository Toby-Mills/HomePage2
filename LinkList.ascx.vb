Partial Class LinkList
    Inherits System.Web.UI.UserControl

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private c_intSelected As Integer
    Private c_tblLink As tblLink.tblLinkDataTable
    Private c_strSelectedDisplay As String
    Private c_strSelectedURL As String
    Private c_strSelectedShortCut As String

    Public Event SelectionChanged()
    Public Event SelectionEdited()
    Public Event SelectionPinned()
    Public Event SelectionUnpinned()
    Public Event SelectionDeleted()

    Public Enum Column
        LinkID
        LinkDisplay
        LinkURL
        LinkShortCut
        UsedCount
        LastUsed
        Edit
        Pin
        Unpin
        Delete

    End Enum

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then

        Else
            c_tblLink = Session.Item(Me.ClientID & "c_tblLink")
        End If
    End Sub

    Public WriteOnly Property LinkTable() As tblLink.tblLinkDataTable
        Set(ByVal Value As tblLink.tblLinkDataTable)
            c_tblLink = Value

        End Set
    End Property

    Public ReadOnly Property Selected() As Integer
        Get
            Selected = c_intSelected
        End Get
    End Property

    Public ReadOnly Property SelectedDisplay() As String
        Get
            SelectedDisplay = c_strSelectedDisplay
        End Get
    End Property

    Public ReadOnly Property SelectedURL() As String
        Get
            SelectedURL = c_strSelectedURL
        End Get
    End Property

    Public ReadOnly Property SelectedShortCut() As String
        Get
            SelectedShortCut = c_strSelectedShortCut
        End Get
    End Property

    Private Sub grdLink_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdLink.ItemCommand
        Dim txtTextBox As Web.UI.WebControls.TextBox

        Select Case e.CommandName
            Case "link"
                c_intSelected = e.Item.Cells(ColumnPosition(Column.LinkID)).Text()
                RaiseEvent SelectionChanged()
            Case "edit"
                grdLink.EditItemIndex = e.Item.ItemIndex
                Bind()
            Case "save"
                c_intSelected = e.Item.Cells(ColumnPosition(Column.LinkID)).Text()

                txtTextBox = e.Item.Cells(ColumnPosition(Column.LinkDisplay)).FindControl("txtDisplay")
                c_strSelectedDisplay = txtTextBox.Text
                txtTextBox = e.Item.Cells(ColumnPosition(Column.LinkDisplay)).FindControl("txtURL")
                c_strSelectedURL = txtTextBox.Text
                txtTextBox = e.Item.Cells(ColumnPosition(Column.LinkDisplay)).FindControl("txtShortcut")
                c_strSelectedShortCut = txtTextBox.Text

                grdLink.EditItemIndex = -1
                Bind()

                RaiseEvent SelectionEdited()
            Case "cancel"
                grdLink.EditItemIndex = -1
                Bind()
            Case "pin"
                c_intSelected = e.Item.Cells(ColumnPosition(Column.LinkID)).Text()
                RaiseEvent SelectionPinned()
            Case "unpin"
                c_intSelected = e.Item.Cells(ColumnPosition(Column.LinkID)).Text()
                RaiseEvent SelectionUnpinned()
            Case "delete"
                c_intSelected = e.Item.Cells(ColumnPosition(Column.LinkID)).Text()
                RaiseEvent SelectionDeleted()
        End Select
    End Sub

    Public Sub Bind()

        Me.grdLink.DataSource = c_tblLink
        Me.grdLink.DataBind()

    End Sub
    Private Function ColumnPosition(ByVal intColumn As Column) As Integer
        Dim intReturn As Integer

        Select Case intColumn
            Case Column.LinkID
                intReturn = 0
            Case Column.Delete
                intReturn = 1
            Case Column.Pin
                intReturn = 2
            Case Column.Unpin
                intReturn = 3
            Case Column.Edit
                intReturn = 4
            Case Column.LinkDisplay
                intReturn = 5
            Case Column.LinkShortCut
                intReturn = 6
            Case Column.LinkURL
                intReturn = 7
            Case Column.UsedCount
                intReturn = 8
            Case Column.LastUsed
                intReturn = 9
        End Select

        Return intReturn

    End Function

    Private Sub grdLink_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdLink.ItemDataBound
        'Dim drvItem As DataRowView
        'Dim dteDate As Date
        'Dim strDate As String
        'Dim strURLIcon As String
        'Dim imgIcon As Web.UI.WebControls.Image

        'drvItem = e.Item.DataItem

        'If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.SelectedItem Then
        '    'format date last used
        '    LoadDBValue(drvItem(c_tblLink.dteLastUsedColumn.ColumnName), dteDate)
        '    strDate = DisplayDBValue(dteDate, "dd MMMM yy")
        '    e.Item.Cells(ColumnPosition(Column.LastUsed)).Text = strDate
        'End If

    End Sub
    Public Property DisplayColumn(ByVal intColumn As Column) As Boolean
        Get
            DisplayColumn = grdLink.Columns(ColumnPosition(intColumn)).Visible
        End Get
        Set(ByVal Value As Boolean)
            grdLink.Columns(ColumnPosition(intColumn)).Visible = Value
        End Set
    End Property

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Unload
        Session.Add(Me.ClientID & "c_tblLink", c_tblLink)
    End Sub

End Class
