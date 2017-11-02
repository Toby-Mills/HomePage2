Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization

Public Class tblRSSFeed_Manager

    Private c_tblRSSFeedDataTable As tblRSSFeed.tblRSSFeedDataTable

    Public Sub New()
        c_tblRSSFeedDataTable = New tblRSSFeed.tblRSSFeedDataTable
    End Sub

    Public Property tblRSSFeedDataTable() As tblRSSFeed.tblRSSFeedDataTable
        Get
            tblRSSFeedDataTable = c_tblRSSFeedDataTable
        End Get
        Set(ByVal value As tblRSSFeed.tblRSSFeedDataTable)
            c_tblRSSFeedDataTable = value
        End Set
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

        strSQL &= "* FROM tblRSSFeed"

        If strWHERE > "" Then
            strSQL &= " WHERE " & strWHERE
        End If
        strSQL &= " ORDER BY tblRSSFeed.strDescription ASC"

        command.CommandText = strSQL

        c_tblRSSFeedDataTable.Clear()

        Adapter = New OleDb.OleDbDataAdapter
        Adapter.SelectCommand = command
        Adapter.Fill(c_tblRSSFeedDataTable)

    End Sub

    Public Function WHERE_ID(ByVal intRSSFeed As Integer) As String
        Dim strReturn As String

        strReturn = " (tblRSSFeed.idRSSFeed = " & SQLFormat(intRSSFeed) & ") "

        Return strReturn

    End Function

    Public Function WHERE_Active(ByVal blnActive As Boolean) As String
        Dim strReturn As String

        strReturn = " (tblRSSFeed.blnActive = " & SQLFormat(blnActive) & ") "

        Return strReturn

    End Function

    Public Sub AddRSSFeed(ByVal strURL As String, ByVal strDescription As String, ByVal blnActive As Boolean)
        Dim objNewRow As tblRSSFeed.tblRSSFeedRow

        objNewRow = c_tblRSSFeedDataTable.NewtblRSSFeedRow

        objNewRow.idRSSFeed = Me.NextID
        objNewRow.strRSSURL = strURL
        objNewRow.strDescription = strDescription
        objNewRow.blnActive = blnActive

        c_tblRSSFeedDataTable.AddtblRSSFeedRow(objNewRow)

    End Sub

    Public Sub UpdateRSSFeed(ByVal idRSSFeed As Integer, ByVal strURL As String, ByVal strDescription As String, ByVal blnActive As Boolean)
        Dim objRow As tblRSSFeed.tblRSSFeedRow

        objRow = c_tblRSSFeedDataTable.FindByidRSSFeed(idRSSFeed)
        objRow.strRSSURL = strURL
        objRow.strDescription = strDescription
        objRow.blnActive = blnActive

    End Sub

    Private Function NextID() As Integer
        Dim objRow As tblRSSFeed.tblRSSFeedRow
        Dim intReturn As Integer

        For Each objRow In c_tblRSSFeedDataTable.Rows
            If objRow.idRSSFeed > intReturn Then
                intReturn = objRow.idRSSFeed
            End If
        Next

        intReturn += 1
        Return intReturn

    End Function

    Private Function DB_AddRSSFeed(ByRef conDb As OleDb.OleDbConnection, ByVal strURL As String, ByVal strDescription As String, ByVal blnActive As Boolean) As Integer
        Dim strSQL As System.Text.StringBuilder
        Dim intRSSFeed As Integer

        strSQL = New System.Text.StringBuilder

        intRSSFeed = modDBFunctions.GetLastRecord(conDb, "tblRSSFeed", "idRSSFeed")
        intRSSFeed += 1

        strSQL.Append("INSERT INTO tblRSSFeed (idRSSFeed, strRSSURL, strDescription, blnActive)")
        strSQL.Append(" VALUES (")
        strSQL.AppendFormat("{0}", SQLFormat(intRSSFeed))
        strSQL.AppendFormat(", {0}", SQLFormat(strURL))
        strSQL.AppendFormat(", {0}", SQLFormat(strDescription))
        strSQL.AppendFormat(", {0}", SQLFormat(blnActive))
        strSQL.Append(")")

        ExecuteSQL(conDb, strSQL.ToString)

        Return intRSSFeed

    End Function

    Private Sub DB_DeleteRSSFeed_ALL(ByVal conDB As OleDb.OleDbConnection)

        DB_DeleteRSSFeed(conDB, "")

    End Sub

    Private Sub DB_DeleteRSSFeed_Single(ByRef conDB As OleDb.OleDbConnection, ByVal intRSSFeed As Integer)
        Dim strWHERE As String

        strWHERE = " WHERE idRSSFeed = " & SQLFormat(intRSSFeed)

        DB_DeleteRSSFeed(conDB, strWHERE)

    End Sub

    Private Sub DB_DeleteRSSFeed(ByRef conDB As OleDb.OleDbConnection, ByVal strWHERE As String)
        Dim strSQL As System.Text.StringBuilder

        strSQL = New System.Text.StringBuilder

        strSQL.Append("DELETE FROM tblRSSFeed ")
        strSQL.AppendFormat(strWHERE)

        ExecuteSQL(conDB, strSQL.ToString)
    End Sub

    Public Sub SaveChanges(ByRef conDb As OleDb.OleDbConnection)
        Dim objRow As tblRSSFeed.tblRSSFeedRow

        DB_DeleteRSSFeed_ALL(conDb)

        For Each objRow In c_tblRSSFeedDataTable.Rows
            DB_AddRSSFeed(conDb, objRow.strRSSURL, objRow.strDescription, objRow.blnActive)
        Next

    End Sub


#Region "Clone Functionality"

    Public Function Clone() As tblRSSFeed_Manager
        Dim tblRSSFeed_ManagerReturn As tblRSSFeed_Manager
        Dim tblRSSFeedDataTableClone As tblRSSFeed.tblRSSFeedDataTable

        tblRSSFeed_ManagerReturn = New tblRSSFeed_Manager

        tblRSSFeedDataTableClone = c_tblRSSFeedDataTable.Clone()
        tblRSSFeed_ManagerReturn.tblRSSFeedDataTable = tblRSSFeedDataTable.Clone

        For Each drRow As DataRow In c_tblRSSFeedDataTable.Rows
            tblRSSFeed_ManagerReturn.AddRowFrom(drRow)
        Next

        Return tblRSSFeed_ManagerReturn

    End Function

    Public Function AddRowFrom(ByRef drOriginal As DataRow) As Boolean
        ' Adds a new row to the datatableviewer based on the given row.
        Dim drNew As DataRow
        Dim blnReturn As Boolean
        Dim objItem As Object

        Dim objClone As Object
        Dim objMemStream As MemoryStream
        Dim objBinaryFormatter As BinaryFormatter

        blnReturn = False

        drNew = Me.NewRow

        For Each dcOldColumn As DataColumn In drOriginal.Table.Columns
            If c_tblRSSFeedDataTable.Columns.Contains(dcOldColumn.ColumnName) Then
                If Not drOriginal(dcOldColumn.ColumnName).GetType.Equals(GetType(DBNull)) Then
                    objItem = drOriginal(dcOldColumn.ColumnName)

                    ' Clone the original item and add it to the new datarow.
                    objClone = Nothing
                    objMemStream = New MemoryStream
                    objBinaryFormatter = New BinaryFormatter(Nothing, New StreamingContext(StreamingContextStates.Clone))
                    objBinaryFormatter.Serialize(objMemStream, objItem)
                    objMemStream.Seek(0, SeekOrigin.Begin)
                    objClone = objBinaryFormatter.Deserialize(objMemStream)
                    objMemStream.Close()

                    drNew(dcOldColumn.ColumnName) = objClone
                    blnReturn = True
                End If
            End If
        Next

        If Not blnReturn Then
            Return False
        End If

        Me.AddRow(drNew)

        Return blnReturn
    End Function

    Private Overloads Sub AddRow(ByRef drNewRow As DataRow)
        ' Adds a new DataRow.
        c_tblRSSFeedDataTable.Rows.Add(drNewRow)
    End Sub

    Private Overloads Sub AddRow(ByRef drNewRow As DataRow, ByVal intPosition As Integer)
        ' Inserts a new DataRow at the given position.
        c_tblRSSFeedDataTable.Rows.InsertAt(drNewRow, intPosition)
    End Sub

    Private Function NewRow() As DataRow
        ' Adds a new row to the datatableviewer
        Return c_tblRSSFeedDataTable.NewRow
    End Function

    Public Sub RemoveRowAt(ByVal intRowIndex As Integer)

        c_tblRSSFeedDataTable.Rows.RemoveAt(intRowIndex)

    End Sub
#End Region

End Class
