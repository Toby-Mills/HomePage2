Public Partial Class tblRSSFeed_Editor
    Inherits System.Web.UI.UserControl

    Public Event OK()
    Public Event Cancel()
    Public Event Save()

    Private c_tblRSSFeed_Manager As tblRSSFeed_Manager


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then

        Else
            c_tblRSSFeed_Manager = Session.Item(Me.ClientID & "c_tblRSSFeed")
        End If
    End Sub

    Public Property RSSFeed_Manager() As tblRSSFeed_Manager
        Get
            Return c_tblRSSFeed_Manager
        End Get
        Set(ByVal value As tblRSSFeed_Manager)
            c_tblRSSFeed_Manager = value
        End Set
    End Property

    Public Sub Bind()

        Me.grdRSSFeed.DataSource = c_tblRSSFeed_Manager.tblRSSFeedDataTable
        Me.grdRSSFeed.DataBind()

    End Sub

    Protected Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        c_tblRSSFeed_Manager.tblRSSFeedDataTable.RejectChanges()
        RaiseEvent Cancel()
    End Sub

    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        ApplyChanges()
        RaiseEvent Save()
    End Sub

    Private Sub grdRSSFeed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdRSSFeed.RowCommand
        Select Case e.CommandName
            Case "Add"
                ApplyChanges()
                AddRowFromFooter()
        End Select
    End Sub

    Private Sub grdRSSFeed_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdRSSFeed.RowDeleting

        c_tblRSSFeed_Manager.RemoveRowAt(e.RowIndex)
        
        Me.Bind()

    End Sub

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        Session.Add(Me.ClientID & "c_tblRSSFeed", c_tblRSSFeed_Manager)
    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click
        ApplyChanges()
        RaiseEvent OK()
    End Sub

    Private Sub ApplyChanges()
        Dim gvrRSSFeed As GridViewRow

        Dim intRSSFeedID As Integer
        Dim chkActive As Web.UI.WebControls.CheckBox
        Dim txtURL As Web.UI.WebControls.TextBox
        Dim txtDescription As Web.UI.WebControls.TextBox

        For Each gvrRSSFeed In Me.grdRSSFeed.Rows
            intRSSFeedID = gvrRSSFeed.Cells(0).Text
            chkActive = gvrRSSFeed.FindControl("chkActive")
            txtURL = gvrRSSFeed.FindControl("txtURL")
            txtDescription = gvrRSSFeed.FindControl("txtDescription")
            c_tblRSSFeed_Manager.UpdateRSSFeed(intRSSFeedID, txtURL.Text, txtDescription.Text, chkActive.Checked)
        Next

    End Sub

    Private Sub AddRowFromFooter()
        Dim chkActive As Web.UI.WebControls.CheckBox
        Dim txtURL As Web.UI.WebControls.TextBox
        Dim txtDescription As Web.UI.WebControls.TextBox

        chkActive = grdRSSFeed.FooterRow.FindControl("chkActiveFooter")
        txtURL = grdRSSFeed.FooterRow.FindControl("txtURLFooter")
        txtDescription = grdRSSFeed.FooterRow.FindControl("txtDescriptionFooter")

        c_tblRSSFeed_Manager.AddRSSFeed(txtURL.Text, txtDescription.Text, chkActive.Checked)
        Me.Bind()

    End Sub

End Class