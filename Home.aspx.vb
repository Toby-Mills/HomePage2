Partial Class WebForm1
    Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents LinkListPinned As LinkList
    Protected WithEvents LinkListUnpinned As LinkList

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
        c_tblLink_Pinned = New tblLink_Manager
        c_tblLink_Unpinned = New tblLink_Manager
    End Sub

#End Region

    Private c_conDB As OleDb.OleDbConnection
    Private c_tblLink_Pinned As tblLink_Manager
    Private c_tblLink_Unpinned As tblLink_Manager

    Private Sub lnkDictionary_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkDictionary.Click
        Response.Redirect("http://www.m-w.com/cgi-bin/dictionary?book=Dictionary&va=" & txtInput.Text)
    End Sub

    Private Sub lnkSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkSearch.Click
        Search()
    End Sub

    Private Sub lnkGroups_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkGroups.Click
        Me.txtInput.Text = Replace(Me.txtInput.Text, " ", "+")
        Response.Redirect("http://groups.google.com/groups?q=" & txtInput.Text & "&ie=UTF-8&oe=UTF-8&hl=en")
    End Sub

    Private Sub lnkDefine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkDefine.Click
        Me.txtInput.Text = "define:" & Replace(Me.txtInput.Text, " ", "+")
        Response.Redirect("http://www.google.com/search?hl=en&lr=&ie=UTF-8&oe=UTF-8&q=" & txtInput.Text & "&sa=N&tab=gw")
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        DefaultButton(Me.txtInput, Me.btnEnter)
        WebFocusControl(Me, Me.txtInput)

        If Not Page.IsPostBack Then
            PopulateLinkLists()
            LoadExtras()
            Me.btnEnter.Width = New Web.UI.WebControls.Unit(0)
        End If

    End Sub



    Private Sub GoToURL()
        Dim tblLinkMgr As tblLink_Manager
        Dim tblLinkRow As tblLink.tblLinkRow

        If Not Left(txtInput.Text, 4) = "http" Then
            txtInput.Text = "http://" & txtInput.Text
        End If

        tblLinkMgr = New tblLink_Manager
        tblLinkMgr.LoadData(c_conDB, tblLinkMgr.WHERE_URL(txtInput.Text), 0)
        If tblLinkMgr.tblLink.Rows.Count > 0 Then
            tblLinkRow = tblLinkMgr.tblLink.Rows(0)
            tblLinkMgr.LinkUsed(c_conDB, tblLinkRow.idLink)
        Else
            c_tblLink_Unpinned.AddLink(c_conDB, txtInput.Text, txtInput.Text, "", False, Now())
        End If
        tblLinkMgr = Nothing

        Response.Clear()
        Response.Redirect(txtInput.Text)
    End Sub

    Private Sub Search()

        Me.txtInput.Text = Replace(Me.txtInput.Text, " ", "+")
        Response.Redirect("http://www.google.com/search?hl=en&lr=&ie=UTF-8&oe=UTF-8&q=" & txtInput.Text & "&sa=N&tab=gw")

    End Sub

    Private Sub LinkListPinned_SelectionChanged() Handles LinkListPinned.SelectionChanged

        c_tblLink_Pinned.LinkUsed(c_conDB, LinkListPinned.Selected)
        Response.Redirect(LinkURL(c_conDB, LinkListPinned.Selected))

    End Sub

    Private Sub LinkListUnpinned_SelectionChanged() Handles LinkListUnpinned.SelectionChanged
        c_tblLink_Pinned.LinkUsed(c_conDB, LinkListUnpinned.Selected)
        Response.Redirect(LinkURL(c_conDB, LinkListUnpinned.Selected))
    End Sub

    Private Sub LinkListUnpinned_SelectionEdited() Handles LinkListUnpinned.SelectionEdited
        Dim intLink As Integer
        Dim strLinkDisplay As String
        Dim strLinkURL As String
        Dim strLinkShortCut As String

        intLink = LinkListUnpinned.Selected
        strLinkDisplay = LinkListUnpinned.SelectedDisplay
        strLinkURL = LinkListUnpinned.SelectedURL
        strLinkShortCut = LinkListUnpinned.SelectedShortCut

        c_tblLink_Unpinned.UpdateLink(c_conDB, intLink, strLinkDisplay, strLinkShortCut, strLinkURL)
        PopulateLinkLists()

    End Sub

    Private Sub LinkListPinned_SelectionUnpinned() Handles LinkListPinned.SelectionUnpinned
        c_tblLink_Pinned.UnpinLink(c_conDB, LinkListPinned.Selected)

        PopulateLinkLists()
    End Sub

    Private Sub PopulateLinkLists()

        c_tblLink_Pinned.LoadData(c_conDB, c_tblLink_Pinned.WHERE_Pinned(True), 30)
        Me.LinkListPinned.DisplayColumn(LinkList.Column.Pin) = False
        Me.LinkListPinned.LinkTable = c_tblLink_Pinned.tblLink
        Me.LinkListPinned.Bind()

        c_tblLink_Unpinned.LoadData(c_conDB, c_tblLink_Unpinned.WHERE_Pinned(False), 10)
        Me.LinkListUnpinned.DisplayColumn(LinkList.Column.Unpin) = False
        Me.LinkListUnpinned.LinkTable = c_tblLink_Unpinned.tblLink
        Me.LinkListUnpinned.Bind()
    End Sub

    Private Sub LinkListUnpinned_SelectionPinned() Handles LinkListUnpinned.SelectionPinned
        c_tblLink_Unpinned.PinLink(c_conDB, LinkListUnpinned.Selected)
        PopulateLinkLists()
    End Sub

    Private Sub LinkListPinned_SelectionEdited() Handles LinkListPinned.SelectionEdited
        Dim intLink As Integer
        Dim strLinkDisplay As String
        Dim strLinkURL As String
        Dim strLinkShortCut As String

        intLink = LinkListPinned.Selected
        strLinkDisplay = LinkListPinned.SelectedDisplay
        strLinkURL = LinkListPinned.SelectedURL
        strLinkShortCut = LinkListPinned.SelectedShortCut

        c_tblLink_Pinned.UpdateLink(c_conDB, intLink, strLinkDisplay, strLinkShortCut, strLinkURL)
        PopulateLinkLists()

    End Sub

    Private Sub LinkListPinned_SelectionDeleted() Handles LinkListPinned.SelectionDeleted

        c_tblLink_Pinned.DeleteLink(c_conDB, LinkListPinned.Selected)
        PopulateLinkLists()

    End Sub

    Private Sub LinkListUnpinned_SelectionDeleted() Handles LinkListUnpinned.SelectionDeleted

        c_tblLink_Unpinned.DeleteLink(c_conDB, LinkListUnpinned.Selected)
        PopulateLinkLists()

    End Sub

    Private Sub btnImages_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImages.Click

        Me.txtInput.Text = Replace(Me.txtInput.Text, " ", "+")
        Response.Redirect("http://images.google.com/images?hl=en&lr=&ie=UTF-8&oe=UTF-8&sa=N&um=1&tab=wi&q=" & txtInput.Text)

    End Sub

    Private Sub btnEnter_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnter.Click
        Dim tblLinkMgr As tblLink_Manager
        Dim tblLinkRow As tblLink.tblLinkRow

        tblLinkMgr = New tblLink_Manager
        tblLinkMgr.LoadData(c_conDB, tblLinkMgr.WHERE_DISPLAY(txtInput.Text) & " OR " & tblLinkMgr.WHERE_SHORTCUT(txtInput.Text), 0)
        If tblLinkMgr.tblLink.Rows.Count > 0 Then
            tblLinkRow = tblLinkMgr.tblLink.Rows(0)
            txtInput.Text = tblLinkRow.strURL
        End If
        tblLinkMgr = Nothing

        If Left(txtInput.Text, 4) = "http" Or Left(txtInput.Text, 3) = "www" Then
            GoToURL()
        Else
            Search()
        End If

    End Sub

    Private Sub LoadExtras()
        Dim threadDilbert As Threading.Thread
        Dim threadCapeTownTime As Threading.Thread

        'Create the threads and start them
        threadDilbert = New Threading.Thread(AddressOf GetTodaysDilbert)
        threadDilbert.Priority = Threading.ThreadPriority.Normal
        threadDilbert.Start()

        threadCapeTownTime = New Threading.Thread(AddressOf GetCapeTownTime)
        threadCapeTownTime.Priority = Threading.ThreadPriority.Normal
        threadCapeTownTime.Start()

    End Sub
    Public Sub GetTodaysDilbert()
        Dim DilbertService As Dilbert2.Dilbert

        Try
            DilbertService = New Dilbert2.Dilbert
            Session.Item("DilbertURL") = DilbertService.DailyDilbert(Today)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub GetCapeTownTime()
        Dim serviceTimeService As WorldTimeWebService.WorldTimeWebService
        Dim TimeZoneInfo As WorldTimeWebService.TimeZoneInfo
        Dim CurrentDateTime As DateTime

        Try
            serviceTimeService = New WorldTimeWebService.WorldTimeWebService

            TimeZoneInfo = serviceTimeService.GetTimeZoneInfo("(GMT+02:00) Harare, Pretoria")
            CurrentDateTime = New DateTime(TimeZoneInfo.CurrentTimeTicks)
            Session.Item("CapeTownTime") = CurrentDateTime.ToString("hh:mm:ss tt  (dd MMM yyyy)")
        Catch ex As Exception
            Session.Item("CapeTownTime") = "... no connection..."
        End Try
    End Sub

    Protected Sub btnWikipedia_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWikipedia.Click
        Dim wikipediasearch As New org.wikipedia_lab.Service
        Dim intTopCandidate As Integer
        Dim ds As DataSet

        Me.txtInput.Text = Replace(Me.txtInput.Text, " ", "+")
        wikipediasearch = New org.wikipedia_lab.Service
        intTopCandidate = wikipediasearch.GetTopCandidateIDFromKeyword(Me.txtInput.Text)
        If intTopCandidate > 0 Then
            ds = wikipediasearch.GetCandidateFromID(intTopCandidate)
            Response.Redirect("http://en.wikipedia.org/wiki/" & ds.Tables(0).Rows(0).Item(1).ToString)
        Else
            Response.Redirect("http://en.wikipedia.org/wiki/Special:Search?search=" & Me.txtInput.Text & "&fulltext=Search")
        End If

    End Sub

    Protected Sub btnAllUnpinned_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAllUnpinned.Click
        c_tblLink_Unpinned.LoadData(c_conDB, c_tblLink_Unpinned.WHERE_Pinned(False), 0)
        Me.LinkListUnpinned.DisplayColumn(LinkList.Column.Unpin) = False
        Me.LinkListUnpinned.LinkTable = c_tblLink_Unpinned.tblLink
        Me.LinkListUnpinned.Bind()
    End Sub

    Protected Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnGo.Click
        GoToURL()
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Search()
    End Sub

    Protected Sub btnFogBugz_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFogBugz.Click

        Me.txtInput.Text = Replace(Me.txtInput.Text, " ", "+")
        Response.Redirect("http://www.spatialdimension.com/fogbugz/default.asp?pre=preMultiSearch&pg=pgList&pgBack=pgSearch&search=2&searchFor=" & Me.txtInput.Text & "+Type%3A%22Cases%22+Status%3A%22Active%22&sLastSearchString=&sLastSearchStringJSArgs=%27%27%2C%27%27%2C%27%27")
    End Sub
End Class
