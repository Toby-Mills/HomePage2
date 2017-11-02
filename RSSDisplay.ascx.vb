Public Partial Class RSSDisplay
    Inherits System.Web.UI.UserControl

    Private c_conDB As OleDb.OleDbConnection
    Private c_tblRSSFeed_Manager As tblRSSFeed_Manager

    Private Const XML_NEWS_INPUT As String = "NewsInput.xml"
    Private Const XML_NEWS_OUTPUT As String = "NewsOutput.xml"
    Private Const XSLT_NEWS_SORT As String = "NewsSort.xslt"


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            c_tblRSSFeed_Manager = New tblRSSFeed_Manager
            c_tblRSSFeed_Manager.LoadData(c_conDB, "", 0)
        Else
            c_tblRSSFeed_Manager = Session.Item(Me.ClientID & "c_tblRSSFeed_Manager")
        End If
        DisplayNews()
    End Sub

    Private Sub RefreshNews()

        Dim objRSSFeed As tblRSSFeed.tblRSSFeedRow

        Dim xmlDisplay As System.Xml.XmlDocument

        Dim xsltSortNews As System.Xml.Xsl.XslCompiledTransform
        Dim strPathXMLInput As String
        Dim strPathXMLOutput As String
        Dim strPathXSLNewsSort As String

        strPathXMLInput = Server.MapPath(XML_NEWS_INPUT)
        strPathXMLOutput = Server.MapPath(XML_NEWS_OUTPUT)
        strPathXSLNewsSort = Server.MapPath(XSLT_NEWS_SORT)

        Try
            xmlDisplay = New System.Xml.XmlDocument
            xmlDisplay.LoadXml("<rss></rss>")

            For Each objRSSFeed In c_tblRSSFeed_Manager.tblRSSFeedDataTable.Rows
                If objRSSFeed.blnActive = True Then
                    AddFeed(xmlDisplay, objRSSFeed.strRSSURL)
                End If
            Next

            xmlDisplay.Save(strPathXMLInput)
            Me.xmlSourceRSS.DataFile = Nothing

            xsltSortNews = New System.Xml.Xsl.XslCompiledTransform
            xsltSortNews.Load(strPathXSLNewsSort)
            xsltSortNews.Transform(strPathXMLInput, strPathXMLOutput)

            DisplayNews()

        Catch ex As Exception
            WebMsgBox(Me.Page, "Exception", ex.Message)
        End Try

    End Sub

    'Private Sub RefreshNews()
    '    Dim xmlDisplay As System.Xml.XmlDocument

    '    Dim intSource As Integer
    '    Dim xsltSortNews As System.Xml.Xsl.XslCompiledTransform
    '    Dim strPathXMLInput As String
    '    Dim strPathXMLOutput As String
    '    Dim strPathXSLNewsSort As String

    '    strPathXMLInput = Server.MapPath(XML_NEWS_INPUT)
    '    strPathXMLOutput = Server.MapPath(XML_NEWS_OUTPUT)
    '    strPathXSLNewsSort = Server.MapPath(XSLT_NEWS_SORT)

    '    Try
    '        xmlDisplay = New System.Xml.XmlDocument
    '        xmlDisplay.LoadXml("<rss></rss>")

    '        If Me.cmbFeedSource.SelectedValue = "All" Then
    '            For intSource = 1 To Me.cmbFeedSource.Items.Count - 1
    '                If Me.cmbFeedSource.Items(intSource).Enabled = True Then
    '                    AddFeed(xmlDisplay, Me.cmbFeedSource.Items(intSource).Value)
    '                End If
    '            Next

    '        Else
    '            AddFeed(xmlDisplay, Me.cmbFeedSource.SelectedValue)
    '        End If

    '        xmlDisplay.Save(strPathXMLInput)
    '        Me.xmlSourceRSS.DataFile = Nothing

    '        xsltSortNews = New System.Xml.Xsl.XslCompiledTransform
    '        xsltSortNews.Load(strPathXSLNewsSort)
    '        xsltSortNews.Transform(strPathXMLInput, strPathXMLOutput)

    '        DisplayNews()

    '    Catch ex As Exception
    '        WebMsgBox(Me.Page, "Exception", ex.Message)
    '    End Try

    'End Sub

    Private Sub AddFeed(ByRef xmlAggregatedFeed As System.Xml.XmlDocument, ByVal strURLFeed As String)
        Dim xmlSource As System.Xml.XmlDocument
        Dim ndeChannel As System.Xml.XmlNode
        Dim fragChannel As System.Xml.XmlDocumentFragment

        Me.xmlSourceRSS.DataFile = strURLFeed
        xmlSource = Me.xmlSourceRSS.GetXmlDocument
        ndeChannel = xmlSource.SelectSingleNode("/rss/channel")
        fragChannel = xmlAggregatedFeed.CreateDocumentFragment()
        fragChannel.InnerXml = ndeChannel.OuterXml
        xmlAggregatedFeed.DocumentElement.AppendChild(fragChannel)

    End Sub

    Private Sub DisplayNews()
        Me.xmlSourceRSS.DataFile = Server.MapPath(XML_NEWS_OUTPUT)
        Me.lstRSS.DataSource = Me.xmlSourceRSS
        Me.lstRSS.DataBind()
    End Sub

    Protected Sub btnRefreshNews_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefreshNews.Click
        RefreshNews()
    End Sub

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        Session.Add(Me.ClientID & "c_tblRSSFeed_Manager", c_tblRSSFeed_Manager)
    End Sub

    Protected Sub btnSource_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSource.Click
        Me.TblRSSFeed_Editor1.RSSFeed_Manager = c_tblRSSFeed_Manager.Clone
        Me.TblRSSFeed_Editor1.Bind()

        Me.lstRSS.Visible = False
        Me.TblRSSFeed_Editor1.Visible = True
    End Sub

    Private Sub TblRSSFeed_Editor1_Cancel() Handles TblRSSFeed_Editor1.Cancel
        Me.lstRSS.Visible = True
        Me.TblRSSFeed_Editor1.Visible = False
    End Sub

    Private Sub TblRSSFeed_Editor1_OK() Handles TblRSSFeed_Editor1.OK
        Me.lstRSS.Visible = True
        Me.TblRSSFeed_Editor1.Visible = False

        c_tblRSSFeed_Manager = Me.TblRSSFeed_Editor1.RSSFeed_Manager
        RefreshNews()
    End Sub

    Private Sub TblRSSFeed_Editor1_Save() Handles TblRSSFeed_Editor1.Save
        Me.lstRSS.Visible = True
        Me.TblRSSFeed_Editor1.Visible = False

        c_tblRSSFeed_Manager = Me.TblRSSFeed_Editor1.RSSFeed_Manager
        c_tblRSSFeed_Manager.SaveChanges(c_conDB)
        RefreshNews()

    End Sub
End Class