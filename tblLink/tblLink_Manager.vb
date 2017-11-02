Public Class tblLink_Manager

    Private c_tblLink As tblLink

    Public Sub New()
        c_tblLink = New tblLink
    End Sub

    Public ReadOnly Property tblLink() As tblLink.tblLinkDataTable
        Get
            tblLink = c_tblLink._tblLink
        End Get
    End Property

    Public Sub LoadData(ByRef conDb As OleDb.OleDbConnection, ByVal strWHERE As String, ByVal intMaxRecords As Integer)
        Dim command As OleDb.OleDbCommand
        Dim Adapter As OleDb.OleDbDataAdapter
        Dim strSQL As String

        command = New OleDb.OleDbCommand
        OpenDBConnection(conDb)
        command.Connection = conDb

        strSQL = "SELECT "

        If intMaxRecords > 0 Then
            strSQL &= "TOP " & intMaxRecords & " "
        End If

        strSQL &= "* FROM tblLink INNER JOIN (SELECT idLink, intCountUsed / (CASE WHEN " & _
            " DATEDIFF([day], dteLastUsed, GETDATE()) = 0 THEN 0.5 ELSE DATEDIFF([day], " & _
            " dteLastUsed, GETDATE()) END) AS Rank FROM tblLink) tblLinkRank ON " & _
            " tblLink.idLink = tblLinkRank.idLink"

        If strWHERE > "" Then
            strSQL &= " WHERE " & strWHERE
        End If
        strSQL &= "ORDER BY tblLinkRank.Rank DESC"

        command.CommandText = strSQL

        c_tblLink._tblLink.Clear()

        Adapter = New OleDb.OleDbDataAdapter
        Adapter.SelectCommand = command
        Adapter.Fill(c_tblLink._tblLink)

    End Sub

    Public Function WHERE_Pinned(ByVal blnPinned As Boolean)
        Dim strReturn As String

        strReturn = " (blnPinned = " & SQLFormat(blnPinned) & ") "

        Return strReturn

    End Function

    Public Function WHERE_URL(ByVal strURL As String)
        Dim strReturn As String

        strReturn = " (strURL LIKE " & SQLFormat(strURL) & ") "

        Return strReturn

    End Function

    Public Function WHERE_ID(ByVal intLink As Integer)
        Dim strReturn As String

        strReturn = " (tblLink.idLink = " & SQLFormat(intLink) & ") "

        Return strReturn

    End Function

    Public Function WHERE_DISPLAY(ByVal strDisplay As String)
        Dim strReturn As String

        strReturn = " (strDisplay LIKE " & SQLFormat(strDisplay) & ") "

        Return strReturn
    End Function

    Public Function WHERE_SHORTCUT(ByVal strShortCut As String)
        Dim strReturn As String

        strReturn = " (strShortCut LIKE " & SQLFormat(strShortCut) & ") "

        Return strReturn
    End Function

    Public Function AddLink(ByRef conDb As OleDb.OleDbConnection, ByVal strURL As String, ByVal strDisplay As String, ByVal strShortCut As String, ByVal blnPinned As Boolean, ByVal dteUsed As Date) As Integer
        Dim strSQL As System.Text.StringBuilder
        Dim intLink As Integer

        strSQL = New System.Text.StringBuilder

        intLink = modDBFunctions.GetLastRecord(conDb, "tblLink", "idLink")
        intLink += 1

        strSQL.Append("INSERT INTO tblLink (idLink, strURL, strDisplay, strShortCut, blnPinned, intCountUsed, dteLastUsed)")
        strSQL.Append(" VALUES (")
        strSQL.AppendFormat("{0}", SQLFormat(intLink))
        strSQL.AppendFormat(", {0}", SQLFormat(strURL))
        strSQL.AppendFormat(", {0}", SQLFormat(strDisplay))
        strSQL.AppendFormat(", {0}", SQLFormat(strShortCut))
        strSQL.AppendFormat(", {0}", SQLFormat(blnPinned))
        strSQL.AppendFormat(", {0}", SQLFormat(1))
        strSQL.AppendFormat(", {0}", SQLFormat(dteUsed))
        strSQL.Append(")")

        ExecuteSQL(conDb, strSQL.ToString)

        Return intLink

    End Function

    Public Sub UpdateLink(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer, ByVal strDisplay As String, ByVal strShortCut As String, ByVal strURL As String)
        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("UPDATE tblLink SET ")
        strSQL.AppendFormat(" strDisplay = {0} ", SQLFormat(strDisplay))
        strSQL.AppendFormat(", strShortCut = {0} ", SQLFormat(strShortCut))
        strSQL.AppendFormat(", strURL = {0} ", SQLFormat(strURL))
        strSQL.AppendFormat("WHERE idLink = {0}", SQLFormat(intLink))

        ExecuteSQL(conDB, strSQL.ToString)
    End Sub

    Public Sub LinkUsed(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer)

        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("UPDATE tblLink SET intCountUsed = intCountUsed + 1 ")
        strSQL.AppendFormat(", dteLastUsed = {0} ", SQLFormat(Now))
        strSQL.AppendFormat("WHERE idLink = {0}", SQLFormat(intLink))

        ExecuteSQL(conDB, strSQL.ToString)

    End Sub

    Public Sub PinLink(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer)
        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("UPDATE tblLink SET ")
        strSQL.AppendFormat(" blnPinned = {0} ", SQLFormat(True))
        strSQL.AppendFormat("WHERE idLink = {0}", SQLFormat(intLink))

        ExecuteSQL(conDB, strSQL.ToString)
    End Sub

    Public Sub UnpinLink(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer)
        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("UPDATE tblLink SET ")
        strSQL.AppendFormat(" blnPinned = {0} ", SQLFormat(False))
        strSQL.AppendFormat("WHERE idLink = {0}", SQLFormat(intLink))

        ExecuteSQL(conDB, strSQL.ToString)
    End Sub

    Public Sub DeleteLink(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer)
        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("DELETE FROM tblLink ")
        strSQL.AppendFormat("WHERE idLink = {0}", SQLFormat(intLink))

        ExecuteSQL(conDB, strSQL.ToString)
    End Sub

End Class
