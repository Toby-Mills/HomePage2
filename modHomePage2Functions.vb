Module modHomePage2Functions

    Public Function LinkURL(ByRef conDB As OleDb.OleDbConnection, ByVal intLink As Integer) As String
        Dim test As tblLink_Manager
        Dim strReturn As String

        test = New tblLink_Manager

        test.LoadData(conDB, test.WHERE_ID(intLink), 0)
        strReturn = test.tblLink.Rows(0).Item(test.tblLink.strURLColumn)

        Return strReturn

    End Function

    Public Function URLRoot(ByVal strURL As String) As String
        Dim strToken() As String
        Dim strReturn As String

        strReturn = strURL
        strToken = Split(strURL, "/")
        If UBound(strToken) > 1 Then
            strReturn = strToken(0) & "/" & strToken(1) & "/" & strToken(2)
        End If

        Return strReturn

    End Function
End Module
