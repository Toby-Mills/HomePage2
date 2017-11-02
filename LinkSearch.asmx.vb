Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Collections.Generic

<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
<System.Web.Script.Services.ScriptService()> _
Public Class LinkSearch
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function Search(ByVal prefixText As String, ByVal count As Integer) As Array

        Dim tblLinkManager As tblLink_Manager
        Dim conDb As OleDb.OleDbConnection
        Dim lstLinks As New List(Of String)
        Dim strLinkDisplay As String
        Dim strWHERE As String
        Dim blnPinnedLinksFound As Boolean
        Dim blnUnpinnedLinksFound As Boolean
        Dim intPinnedFound As Integer

        tblLinkManager = New tblLink_Manager

        strWHERE = "(" & tblLinkManager.WHERE_DISPLAY("%" & prefixText & "%") & _
                        " OR " & tblLinkManager.WHERE_URL("%" & prefixText & "%") & _
                        " OR " & tblLinkManager.WHERE_SHORTCUT("%" & prefixText & "%") & _
                        ") AND " & tblLinkManager.WHERE_Pinned(True)
        tblLinkManager.LoadData(conDb, strWHERE, count)

        blnPinnedLinksFound = (tblLinkManager.tblLink.Rows.Count > 0)
        intPinnedFound = tblLinkManager.tblLink.Rows.Count

        For Each drLink As DataRow In tblLinkManager.tblLink.Rows
            strLinkDisplay = drLink.Item(tblLinkManager.tblLink.strDisplayColumn).ToString
            lstLinks.Add(strLinkDisplay)
        Next

        If intPinnedFound < count Then

            strWHERE = "(" & tblLinkManager.WHERE_DISPLAY("%" & prefixText & "%") & _
                            " OR " & tblLinkManager.WHERE_URL("%" & prefixText & "%") & _
                            " OR " & tblLinkManager.WHERE_SHORTCUT("%" & prefixText & "%") & _
                            ") AND " & tblLinkManager.WHERE_Pinned(False)

            tblLinkManager.LoadData(conDb, strWHERE, count - intPinnedFound)

            blnUnpinnedLinksFound = (tblLinkManager.tblLink.Rows.Count > 0)

            If blnPinnedLinksFound AndAlso blnUnpinnedLinksFound Then
                lstLinks.Add("---------------------------")
            End If

            For Each drLink As DataRow In tblLinkManager.tblLink.Rows
                strLinkDisplay = drLink.Item(tblLinkManager.tblLink.strDisplayColumn).ToString
                lstLinks.Add(strLinkDisplay)
            Next

        End If

        Return lstLinks.ToArray

    End Function

End Class