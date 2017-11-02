Imports System.Data.OleDb
Imports System.Globalization

''' <summary>
''' This module contains functions for interacting with the database
''' The functions all take a connection object as an argument, but all share a common connectin string
''' This allows for connection pooling
''' </summary>
Public Module modDBFunctions

#Region "Variables"
    ''' <summary>
    ''' A string valriable that stores the Oledb connection string
    ''' </summary>
    Public strConnectionString As String

    ''' <summary>
    ''' A Constant storing a null number value to be -9999
    ''' </summary>
    Public Const NULL_NUMBER As Integer = -9999
#End Region

#Region "Enums"
    ''' <summary>
    ''' An enum storing the Oledb connection providers being catered for.
    ''' </summary>
    Public Enum OleDBConnectionProvider
        MicrosoftJet4 = 1
        SQLServer2000 = 2
    End Enum
#End Region

#Region "DB Connection"
    ''' <summary>
    ''' This function opens a specified DB connection
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection obejct
    ''' </param>
    ''' <returns>
    ''' A true/false value indicating whether the oledb connection has been successfully opened.
    ''' </returns>
    Public Function OpenDBConnection(ByRef conDB As OleDb.OleDbConnection) As Boolean
        'if the passed connection is not yet open,
        'use the common connection string to open it

        'assume success
        OpenDBConnection = True

        'create new object if needed
        If conDB Is Nothing Then
            conDB = New System.Data.OleDb.OleDbConnection
        End If

        Try
            If conDB.ConnectionString = "" Then
                conDB.ConnectionString = strConnectionString
            End If
        Catch ex As Exception
            Dim exNew As New Exception("Error occured setting the connection string in OpenDBConnection Function", ex)
            Throw exNew
        End Try

        Select Case conDB.State
            Case Is = ConnectionState.Closed
                Try
                    'occasionally db is stuck 'connecting' - need to close before reattempting
                    conDB.Close()
                    conDB.Open()
                Catch ex As InvalidOperationException
                    'this occurs when the connection state is 'Connecting'
                    OpenDBConnection = False
                Catch ex As Exception
                    'any other exception
                    OpenDBConnection = False
                    Dim exNew As New Exception("Error occured in OpenDBConnection Function(ConnectionState.Closed)", ex)
                    Throw exNew
                End Try
            Case Is = ConnectionState.Broken
                Try
                    conDB.Open()
                Catch ex As InvalidOperationException
                    'this occurs when the connection state is 'Connecting'
                    OpenDBConnection = False
                Catch ex As Exception
                    'any other exception
                    OpenDBConnection = False
                    Dim exNew As New Exception("Error occured in OpenDbConnection Function (ConnectionState.Broken)", ex)
                    Throw exNew
                End Try
        End Select

    End Function

    ''' <summary>
    ''' This function opens a specified DB connection
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strContextInfo">
    ''' An oledb connection parametre giving the DB context info for the connection
    ''' </param>
    ''' <returns>
    ''' A true/false value indicating whether the oledb connection has been successfully opened.
    ''' </returns>
    Public Function OpenDBConnection(ByRef conDB As OleDb.OleDbConnection, ByVal strContextInfo As String) As Boolean
        'ContextInfo included
        Dim cmdContextInfo As OleDbCommand

        If OpenDBConnection(conDB) Then
            cmdContextInfo = New OleDbCommand
            cmdContextInfo.Connection = conDB
            cmdContextInfo.CommandText = "declare @var varbinary(128);set @var = cast('" & strContextInfo & "' as varbinary(128));SET CONTEXT_INFO @var"
            cmdContextInfo.ExecuteNonQuery()
        End If
    End Function

    ''' <summary>
    ''' This function closes a specified DB connection
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub CloseDBConnection(ByRef conDB As OleDb.OleDbConnection)
        'if the passed connection is open, close it
        If Not conDB Is Nothing Then
            If conDB.State <> ConnectionState.Closed Then
                conDB.Close()
            End If
        End If

    End Sub
#End Region

#Region "Execute SQL"
    ''' <summary>
    ''' This function executes a specified SQL Statement
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <returns>
    ''' Number of rows affected by the query execution
    ''' </returns>
    Public Function ExecuteSQL(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String) As Integer
        'execute the passed SQL command - return the rows affected
        Dim com As New OleDb.OleDbCommand

        Try
            OpenDBConnection(conDB)
            com.Connection = conDB
            com.CommandText = strSQL
            ExecuteSQL = com.ExecuteNonQuery()
        Catch ex As Exception
            'This exception can be passed up the call chain
            Throw ex
        End Try

    End Function

    ''' <summary>
    ''' This function executes a specified SQL Statement
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <param name="strContextInfo">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    ''' <returns>
    ''' Number of rows affected by the query execution
    ''' </returns>
    Public Function ExecuteSQL(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String, ByVal strContextInfo As String) As Integer
        'ContextInfo included
        OpenDBConnection(conDB, strContextInfo)
        ExecuteSQL = ExecuteSQL(conDB, strSQL)
    End Function

    ''' <summary>
    ''' This function executes a specified SQL Statement
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <param name="trans">    
    ''' </param>
    ''' <returns>
    ''' Number of rows affected by the query execution
    ''' </returns>
    Public Function ExecuteSQL(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String, ByVal trans As OleDb.OleDbTransaction) As Integer
        Dim com As OleDb.OleDbCommand

        Try
            com = New OleDbCommand
            com.Connection = conDB
            com.Transaction = trans
            com.CommandText = strSQL
            ExecuteSQL = com.ExecuteNonQuery
        Catch ex As Exception
            Throw ex
        End Try

    End Function
#End Region

#Region "GetData Functions"
    ''' <summary>
    ''' This function executes a specified SQL Statement and returns a datareader
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <returns>
    ''' A datareader from the executed sql statement
    ''' </returns>
    Public Function GetDataReader(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String) As System.Data.OleDb.OleDbDataReader
        'return a datareader based on the SELECT statement passed
        Dim comDataset As System.Data.OleDb.OleDbCommand

        If OpenDBConnection(conDB) Then
            comDataset = conDB.CreateCommand()
            comDataset.CommandText = strSQL
            GetDataReader = comDataset.ExecuteReader()
            comDataset = Nothing
        Else
            GetDataReader = Nothing
        End If

    End Function

    ''' <summary>
    ''' This function executes a specified SQL Statement and returns a dataset
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <param name="strTableName">
    '''  The name of the source table to use for table mapping. 
    ''' </param>
    ''' <returns>
    ''' A dataset from the executed sql statement
    ''' </returns>
    Public Function GetDataSet(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String, ByVal strTableName As String) As System.Data.DataSet
        'return a dataset, with a datatable filled with rows based on the SELECT statement passed
        Dim adapter As System.Data.OleDb.OleDbDataAdapter
        Dim dsReturn As DataSet

        OpenDBConnection(conDB)
        adapter = New OleDb.OleDbDataAdapter(strSQL, conDB)
        dsReturn = New System.Data.DataSet
        adapter.Fill(dsReturn, strTableName)

        Return dsReturn

    End Function

    ''' <summary>
    ''' This function executes a specified SQL Statement and returns a datatable
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <returns>
    ''' A datatable from the executed sql statement
    ''' </returns>
    Public Function GetDataTable(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String) As DataTable
        'return a datatable filled with rows based on the SQL query passed
        Dim dsDataset As DataSet
        Dim dtReturn As DataTable

        dsDataset = GetDataSet(conDB, strSQL, "return")
        dtReturn = dsDataset.Tables("return")

        Return dtReturn

    End Function

    ''' <summary>
    ''' This function returns the last integer ID in a table from the DB
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTable">
    ''' The table in the DB from which to retrieve the last ID value
    ''' </param>
    ''' <param name="strIDField">
    ''' The column name in the table which contains the ID fields
    ''' </param>
    ''' <returns>
    ''' An integer value which is the last ID value
    ''' </returns>
    Public Function GetLastRecord(ByRef conDB As OleDb.OleDbConnection, ByVal strTable As String, ByVal strIDField As String) As Integer
        'get the highest ID in a table
        'generally used to get the ID of the most recently added record, in a table with an autonumber

        Dim strSQL As String
        Dim drLastRecord As Data.OleDb.OleDbDataReader

        strSQL = "SELECT * FROM " & strTable & " ORDER BY " & strIDField & " DESC"
        drLastRecord = GetDataReader(conDB, strSQL)
        GetLastRecord = -1
        If drLastRecord.Read Then
            GetLastRecord = drLastRecord.Item(strIDField)
        End If
        drLastRecord.Close()
        drLastRecord = Nothing

    End Function

    ''' <summary>
    ''' This function returns the first integer ID in a table from the DB
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTable">
    ''' The table in the DB from which to retrieve the first ID value
    ''' </param>
    ''' <param name="strIDField">
    ''' The column name in the table which contains the ID fields
    ''' </param>
    ''' <returns>
    ''' An integer value which is the first ID value
    ''' </returns>
    Public Function GetFirstRecord(ByRef conDB As OleDb.OleDbConnection, ByVal strTable As String, ByVal strIDField As String) As Integer
        'get the first (lowest) ID in a table

        Dim strSQL As String
        Dim drFirstRecord As Data.OleDb.OleDbDataReader

        strSQL = "SELECT * FROM " & strTable & " ORDER BY " & strIDField
        drFirstRecord = GetDataReader(conDB, strSQL)
        GetFirstRecord = -1
        If drFirstRecord.Read Then
            GetFirstRecord = drFirstRecord.Item(strIDField)
        End If
        drFirstRecord.Close()
        drFirstRecord = Nothing

    End Function
#End Region

#Region "Add & Find Methods"
    ''' <summary>
    ''' This function fills the specified dataset, with a datatable filled with rows based on the SQL statement passed
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="objDataSet">
    ''' The dataset in which to fill the table
    ''' </param>
    ''' <param name="strTableName">
    ''' The name of the source table to use for table mapping. 
    ''' </param>
    ''' <param name="strSQL">
    ''' The SQL Statement to be executed 
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub AddDataSet(ByRef conDB As OleDb.OleDbConnection, ByRef objDataSet As Data.DataSet, ByVal strSQL As String, ByVal strTableName As String)
        'return a dataset, with a datatable filled with rows based on the SELECT statement passed
        Dim adapter As System.Data.OleDb.OleDbDataAdapter

        OpenDBConnection(conDB)
        adapter = New OleDb.OleDbDataAdapter(strSQL, conDB)
        If objDataSet Is Nothing Then
            objDataSet = New System.Data.DataSet
        End If

        adapter.Fill(objDataSet, strTableName)

    End Sub

    ''' <summary>
    ''' The function Searches for a value in the specified column in the datatable
    ''' This is the original version of this function.
    ''' This version is left for backward compatibility
    ''' </summary>
    ''' <param name="conDB">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSearchValue">
    ''' The value to search for the DB
    ''' </param>
    ''' <param name="strSearchField">
    ''' The Column name in which the value is to be searched
    ''' </param>
    ''' <param name="objDataTable">
    ''' The datatable in which to search for the values specified.
    ''' </param>
    ''' <param name="blnTrim">
    ''' A true/false value indicating whether the passes in value needs to be trimmed or not.
    ''' </param>
    ''' <returns>
    ''' A datarow that contains the value being searched.
    ''' </returns>
    Public Function FindDataRow(ByRef conDB As OleDb.OleDbConnection, ByVal strSearchValue As String, ByVal strSearchField As String, ByVal objDataTable As Data.DataTable, Optional ByVal blnTrim As Boolean = False) As DataRow
        'This is the original version of this function.
        'The parameter conDB is passed but never used.
        'Rather use the other overload (below).
        'This version is left for backward compatibility

        'search through the passed datatable for a row with the specifed value
        'return that row
        Dim objDataRow As Data.DataRow
        Dim strValue As String

        FindDataRow = Nothing
        If blnTrim Then
            strSearchValue = Trim(strSearchValue)
        End If


        For Each objDataRow In objDataTable.Rows
            strValue = objDataRow(strSearchField)
            If blnTrim Then
                strValue = Trim(strValue)
            End If
            If strValue = strSearchValue Then
                FindDataRow = objDataRow
                Exit For
            End If
        Next

    End Function

    ''' <summary>
    ''' The function Searches for a value in the specified column in the datatable
    ''' This overload corrects the problem with the original function (above):
    ''' This version is left for backward compatibility
    ''' </summary>
    ''' <param name="strSearchValue">
    ''' The value to search for the DB
    ''' </param>
    ''' <param name="strSearchField">
    ''' The Column name in which the value is to be searched
    ''' </param>
    ''' <param name="objDataTable">
    ''' The datatable in which to search for the values specified.
    ''' </param>
    ''' <param name="blnTrim">
    ''' A true/false value indicating whether the passes in value needs to be trimmed or not.
    ''' </param>
    ''' <returns>
    ''' A datarow that contains the value being searched.
    ''' </returns>
    Public Function FindDataRow(ByVal strSearchValue As String, ByVal strSearchField As String, ByVal objDataTable As Data.DataTable, Optional ByVal blnTrim As Boolean = False) As DataRow
        'This overload corrects the problem with the original function (above):
        'The parameter conDB is passed but not used.
        'The original function is left for backward compatibility

        'search through the passed datatable for a row with the specifed value
        'return that row
        Dim objDataRow As Data.DataRow
        Dim strValue As String

        FindDataRow = Nothing
        If blnTrim Then
            strSearchValue = Trim(strSearchValue)
        End If


        For Each objDataRow In objDataTable.Rows
            strValue = objDataRow(strSearchField)
            If blnTrim Then
                strValue = Trim(strValue)
            End If
            If strValue = strSearchValue Then
                FindDataRow = objDataRow
                Exit For
            End If
        Next
    End Function
#End Region

#Region "DeleteDataRows"
    ''' <summary>
    '''     Deletes rows from the database table. Constructs the SQL from parameters. Id field can be string or integer.
    '''     Will delete all rows with specified criteria so needs to be used with caution.
    ''' </summary>
    ''' <param name="strTableName" type="String">
    '''     <para>
    '''         A valid table name for the data source
    '''     </para>
    ''' </param>
    ''' <param name="strIdField" type="String">
    '''     <para>
    '''         A fieldname for specifying the criteria
    '''     </para>
    ''' </param>
    ''' <param name="strIdValue" type="String">
    '''     <para>
    '''         the value of the specified field
    '''     </para>
    ''' </param>
    ''' <param name="blAsInteger" type="Boolean">
    '''     <para>
    '''         indicates if the specified field is numeric or integer for building the sql string
    '''     </para>
    ''' </param>
    ''' <returns>
    '''     An integer value specifying the number of records deleted
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, ByVal strIdValue As String, ByVal blAsInteger As Boolean) As Integer
        'Delete all records in a table meeting the criteria
        Dim strSQL As String
        Dim intCount As Integer

        If blAsInteger = True Then
            strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & "=" & strIdValue
        ElseIf blAsInteger = False Then
            strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & "='" & strIdValue & "'"
        End If

        DeleteDataRows = ExecuteSQL(conDB, strSQL)

    End Function

    ''' <summary>
    ''' Uses the function above to create a sql statement from the passed values and executes the SQL Statement.
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="blAsInteger" type="Boolean">
    ''' True/false value indicating whether the ID is an integer or not
    ''' </param>
    ''' <param name="strIdValue" type="String">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <param name="strContextInfo" type="String">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, _
    ByVal strIdValue As String, ByVal blAsInteger As Boolean, ByVal strContextInfo As String) As Integer
        'context info included
        OpenDBConnection(conDB, strContextInfo)
        DeleteDataRows(conDB, strTableName, strIdField, strIdValue, blAsInteger)
    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="intIDValue" type="Integer">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, ByVal intIDValue As Integer) As Integer
        'Delete all records in a table meeting the criteria
        Dim strSQL As String

        strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & "=" & intIDValue

        DeleteDataRows = ExecuteSQL(conDB, strSQL)

    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="intIDValue" type="Integer">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <param name="strContextInfo" type="String">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, ByVal intIDValue As Integer, ByVal strContextInfo As String) As Integer
        'include context info
        OpenDBConnection(conDB, strContextInfo)
        DeleteDataRows(conDB, strTableName, strIdField, intIDValue)
    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="guidValue" type="Guid">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, ByVal guidValue As Guid) As Integer
        'Delete all records in a table meeting the criteria
        Dim strSQL As String

        strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & "= " & SQLFormat(guidValue)

        DeleteDataRows = ExecuteSQL(conDB, strSQL)

    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="guidValue" type="Guid">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <param name="strContextInfo" type="String">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, ByVal guidValue As Guid, ByVal strContextInfo As String) As Integer
        'includes context info
        OpenDBConnection(conDB, strContextInfo)
        DeleteDataRows(conDB, strTableName, strIdField, guidValue)
    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String) As Integer

        DeleteDataRows = ExecuteSQL(conDB, "DELETE FROM [" & strTableName & "]")

    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strContextInfo" type="String">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strContextInfo As String) As Integer
        'Including context info
        OpenDBConnection(conDB, strContextInfo)
        DeleteDataRows(conDB, strTableName)
    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="guidValue" type="Guid">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <param name="strAdditionalCheck" type="String">
    ''' An additional value to be checked for 
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, _
    ByVal guidValue As Guid, ByVal strAdditionalCheck As String, ByVal objAdditionalValue As Object) As Integer
        'Delete all records in a table meeting the criteria
        Dim strSQL As String

        strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & "= " & SQLFormat(guidValue) & " AND " & strAdditionalCheck & "= " & SQLFormat(objAdditionalValue)

        DeleteDataRows = ExecuteSQL(conDB, strSQL)

    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="guidValue" type="Guid">
    ''' The value of the Id of which the row to delete.
    ''' </param>
    ''' <param name="strAdditionalCheck" type="String">
    ''' An additional value to be checked for 
    ''' </param>
    ''' <param name="strContextInfo" type="String">
    ''' Used to get the context in which a specific user carried out certain actions
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, _
    ByVal guidValue As Guid, ByVal strAdditionalCheck As String, ByVal objAdditionalValue As Object, ByVal strContextInfo As String) As Integer
        'Including context info
        OpenDBConnection(conDB, strContextInfo)
        DeleteDataRows(conDB, strTableName, strIdField, guidValue, strAdditionalCheck, objAdditionalValue)
    End Function

    ''' <summary>
    ''' Creates a SQL Statement from parametres passed and executes the statement
    ''' Delete all records in a table meeting the criteria
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strIdField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="dteValue" type="DateTime">
    ''' The value of the date of which the row to delete.
    ''' </param>
    ''' <returns>
    ''' An integer value of the number of rows affected by the execution.
    ''' </returns>
    Public Function DeleteDataRows(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strIdField As String, _
    ByVal dteValue As DateTime) As Integer
        'Delete all records in a table meeting the criteria
        Dim strSQL As String

        strSQL = "DELETE FROM " & strTableName & " WHERE " & strIdField & " = " & DBFormat(conDB, dteValue)

        DeleteDataRows = ExecuteSQL(conDB, strSQL)

    End Function
#End Region

#Region "Lookup Methods"
    Public Overloads Function LookupValue(ByRef conDB As OleDb.OleDbConnection, ByVal strValueField As String, ByVal strTextField As String, ByVal strTableName As String, ByVal intValue As Integer) As String
        'look up a value from a lookup table in the database
        Dim drLookup As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        LookupValue = Nothing


        strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & "=" & intValue

        LookupValue = LookupValue(conDB, strSQL)

    End Function
    ''' <summary>
    ''' This function looks up a value from a lookup table in the database
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strValueField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="strTextField" type="String">
    ''' The name of the text column in the table
    ''' </param>
    ''' <param name="strSearchText" type="String">
    ''' The value to search the strTextField for
    ''' </param>
    ''' <param name="blAsInteger" type="Boolean">
    ''' True/false on whether the value is an integer 
    ''' </param>
    ''' <returns>
    ''' A string value returned from the execution of the SQL Statement
    ''' </returns>
    Public Overloads Function LookupValue(ByRef conDB As OleDb.OleDbConnection, ByVal strValueField As String, ByVal strTextField As String, _
    ByVal strTableName As String, ByVal strSearchText As String, ByVal blAsInteger As Boolean) As String
        'look up a value from a lookup table in the database
        Dim drLookup As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        LookupValue = Nothing

        If blAsInteger = True Then
            strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & "=" & strSearchText
        ElseIf blAsInteger = False Then
            strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & "='" & strSearchText & "'"
        End If

        LookupValue = LookupValue(conDB, strSQL)

    End Function

    ''' <summary>
    ''' This function looks up an ID from a lookup table in the database
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strValueField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="strTextField" type="String">
    ''' The name of the text column in the table
    ''' </param>
    ''' <param name="strSearchText" type="String">
    ''' The value to search the strTextField for
    ''' <returns>
    ''' A string value returned from the execution of the SQL Statement
    ''' </returns>
    Public Overloads Function LookupID(ByRef conDB As OleDb.OleDbConnection, ByVal strValueField As String, ByVal strTextField As String, _
    ByVal strTableName As String, ByVal strSearchText As String) As Guid
        'look up a value from a lookup table in the database
        Dim drLookup As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        LookupID = Nothing


        strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & " = " & DBFormat(conDB, strSearchText)


        LookupID = LookupValue(conDB, strSQL)

    End Function

    ''' <summary>
    ''' This function looks up an ID from a lookup table in the database
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strValueField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="strTextField" type="String">
    ''' The name of the text column in the table
    ''' </param>
    ''' <param name="objSearchText" type="Object">
    ''' The object value to search in the strTextField column for
    ''' </param>
    ''' <returns>
    ''' A string value returned from the execution of the SQL Statement
    ''' </returns>
    Public Overloads Function LookupID(ByRef conDB As OleDb.OleDbConnection, ByVal strValueField As String, ByVal strTextField As String, _
    ByVal strTableName As String, ByVal objSearchText As Object) As Object
        'look up a value from a lookup table in the database
        Dim drLookup As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        LookupID = Nothing


        strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & "=" & DBFormat(conDB, objSearchText) & ""


        LookupID = LookupValue(conDB, strSQL)

    End Function

    ''' <summary>
    ''' This function looks up a value from a lookup table in the database
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strSQL" type="String">
    ''' The SQL Statement to be executed
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Overloads Function LookupValue(ByRef conDB As OleDb.OleDbConnection, ByVal strSQL As String) As Object
        'look up a value from the database using the provided SQL String
        Dim drLookup As Data.OleDb.OleDbDataReader

        LookupValue = Nothing

        Try
            drLookup = GetDataReader(conDB, strSQL)
            If drLookup.Read() Then
                LookupValue = drLookup.Item(0)
            End If
        Catch ex As Exception
            LookupValue = Nothing
        Finally
            If Not drLookup Is Nothing Then
                drLookup.Close()
            End If
        End Try

    End Function

    ''' <summary>
    ''' This function looks up a value from a lookup table in the database
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strValueField" type="String">
    ''' The name of the ID column in the table
    ''' </param>
    ''' <param name="strTextField" type="String">
    ''' The name of the text column in the table
    ''' </param>
    ''' <param name="guidSearchValue" type="Guid">
    ''' The value to search the strTextField for
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Overloads Function LookupValue(ByRef conDB As OleDb.OleDbConnection, ByVal strValueField As String, ByVal strTextField As String, _
    ByVal strTableName As String, ByVal guidSearchValue As Guid) As Object
        'look up a value from a lookup table in the database
        Dim strSQL As String

        strSQL = "SELECT " & strTextField & " FROM " & strTableName & " WHERE " & strValueField & "=" & DBFormat(conDB, guidSearchValue)

        LookupValue = LookupValue(conDB, strSQL)

    End Function
#End Region

#Region "Exists in table"
    ''' <summary>
    ''' The function checks if a specified value exists in a specific column of the table.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strFieldName" type="String">
    ''' The name of the column in the table to search the value for
    ''' </param>
    ''' <param name="strFieldValue" type="String">
    ''' The value to search for the in the specified DB table 
    ''' </param>
    ''' <returns>
    ''' A true/false indicating if the value specified exists in the DB table.
    ''' </returns>
    Public Overloads Function ExistsInTable(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, _
    ByVal strFieldName As String, ByVal strFieldValue As String) As Boolean

        Dim dtTable As DataTable
        Dim drTable As DataRow
        Dim strSQL As String
        Dim intRecordCount As Integer

        strSQL = "SELECT COUNT(*) as intRecordCount FROM " & strTableName & " WHERE " & strFieldName & " = " & DBFormat(conDB, strFieldValue)
        dtTable = GetDataTable(conDB, strSQL)
        drTable = dtTable.Rows(0)
        intRecordCount = drTable.Item("intRecordCount")

        If intRecordCount > 0 Then
            ExistsInTable = True
        Else
            ExistsInTable = False
        End If

    End Function

    ''' <summary>
    ''' The function checks if a specified value exists in a specific column of the table.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strFieldName" type="String">
    ''' The name of the column in the table to search the value for
    ''' </param>
    ''' <param name="dteFieldValue" type="Date">
    ''' The date value to search for the in the specified DB table 
    ''' </param>
    ''' <returns>
    ''' A true/false indicating if the value specified exists in the DB table.
    ''' </returns>
    Public Overloads Function ExistsInTable(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strFieldName As String, ByVal dteFieldValue As DateTime) As Boolean

        Dim drTable As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        strSQL = "SELECT " & strFieldName & " FROM " & strTableName & " WHERE " & strFieldName & " = " & DBFormat(conDB, dteFieldValue)
        drTable = GetDataReader(conDB, strSQL)
        ExistsInTable = drTable.Read()
        drTable.Close()

    End Function

    ''' <summary>
    ''' The function checks if a specified value exists in a specific column of the table.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strFieldName" type="String">
    ''' The name of the column in the table to search the value for
    ''' </param>
    ''' <param name="intFieldValue" type="Integer">
    ''' The ID value to search for the in the specified DB table 
    ''' </param>
    ''' <returns>
    ''' A true/false indicating if the value specified exists in the DB table.
    ''' </returns>
    Public Overloads Function ExistsInTable(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strFieldName As String, ByVal intFieldValue As Integer) As Boolean

        Dim drTable As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        strSQL = "SELECT " & strFieldName & " FROM " & strTableName & " WHERE " & strFieldName & "=" & DBFormat(conDB, intFieldValue)
        drTable = GetDataReader(conDB, strSQL)
        ExistsInTable = drTable.Read()
        drTable.Close()

    End Function

    ''' <summary>
    ''' The function checks if a specified value exists in a specific column of the table.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' An oledb connection object
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name from which to delete the row.
    ''' </param>
    ''' <param name="strFieldName" type="String">
    ''' The name of the column in the table to search the value for
    ''' </param>
    ''' <param name="strGuidFieldValue" type="Guid">
    ''' The ID value to search for the in the specified DB table 
    ''' </param>
    ''' <returns>
    ''' A true/false indicating if the value specified exists in the DB table.
    ''' </returns>
    Public Overloads Function ExistsInTable(ByRef conDB As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strFieldName As String, ByVal strGuidFieldValue As Guid) As Boolean

        Dim drTable As Data.OleDb.OleDbDataReader
        Dim strSQL As String

        strSQL = "SELECT " & strFieldName & " FROM " & strTableName & " WHERE " & strFieldName & "= " & DBFormat(conDB, strGuidFieldValue)
        Try
            drTable = GetDataReader(conDB, strSQL)
            ExistsInTable = drTable.Read()
        Catch ex As Exception
        Finally
            If Not drTable Is Nothing Then
                drTable.Close()
            End If
        End Try

    End Function
#End Region

#Region "AccessFormat"
    ''' <summary>
    ''' The function formats a passed date value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="dteDate" type="Date">
    ''' The date value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal dteDate As Date) As String
        If dteDate = Date.MinValue Then
            AccessFormat = "NULL"
        Else
            AccessFormat = "#" & dteDate.ToString("MM/dd/yyyy HH:mm:ss") & "#"
            AccessFormat = "#" & dteDate.ToString(CultureInfo.InvariantCulture.DateTimeFormat) & "#"
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed Integer value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="intInteger" type="Integer">
    ''' The Integer value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal intInteger As Integer) As String
        If intInteger = NULL_NUMBER Then
            AccessFormat = "NULL"
        Else
            AccessFormat = intInteger.ToString
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed Decimal value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="decDecimal" type="Decimal">
    ''' The Decimal value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal decDecimal As Decimal) As String
        If decDecimal = NULL_NUMBER Then
            AccessFormat = "NULL"
        Else
            AccessFormat = decDecimal.ToString(CultureInfo.InvariantCulture.NumberFormat)
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed Double value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="dblDouble" type="Double">
    ''' The Double value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal dblDouble As Double) As String
        If dblDouble = NULL_NUMBER Then
            AccessFormat = "NULL"
        Else
            AccessFormat = dblDouble.ToString(CultureInfo.InvariantCulture.NumberFormat)
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed Guid value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="guidGuid" type="Guid">
    ''' The Guid value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal guidGuid As Guid) As String
        AccessFormat = "{guid {" & guidGuid.ToString & "}}"
    End Function

    ''' <summary>
    ''' The function formats a passed String value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="strString" type="String">
    ''' The String value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal strString As String) As String
        If strString Is Nothing Then
            AccessFormat = "NULL"
        Else
            strString = strString.Replace("'", "''")
            AccessFormat = "'" & strString & "'"
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed Boolean value to an MSAccess-query-friendly value
    ''' </summary>
    ''' <param name="blnBoolean" type="Boolean">
    ''' The Boolean value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (MSAccess-query-friendly string) of the passed in parameter
    ''' </returns>
    Public Function AccessFormat(ByVal blnBoolean As Boolean) As String
        AccessFormat = blnBoolean.ToString
    End Function

#End Region

#Region "SQLFormat"
    ''' <summary>
    ''' The function formats a passed String value to a SQL-friendly value
    ''' The Function accepts a string that is to be trimmed, formats the statement to be accepted by SQL, using the LTRIM and RTRIM functions
    ''' </summary>
    ''' <param name="strSQL" type="String">
    ''' The String value to be trimmed.
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed in parameter
    ''' </returns>
    Public Function SQLFormatTrim(ByVal strSQL As String) As String
        'Function accepts a string that is to be trimmed, formats
        'the "trim" statement to be accepted by SQL, using the LTRIM 
        'and RTRIM functions

        If strSQL = "" Then
            SQLFormatTrim = ""
        Else
            SQLFormatTrim = "LTRIM(RTRIM(" & strSQL & "))"
        End If

    End Function

    ''' <summary>
    ''' The function formats a passed-in Date value to a SQL-friendly value
    ''' </summary>
    ''' <param name="dteDate" type="Date">
    ''' The Date value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal dteDate As Date) As String
        If dteDate = Date.MinValue Then
            SQLFormat = "NULL"
        Else
            SQLFormat = "CONVERT(DATETIME, '" & dteDate.ToString("yyyy-MM-dd HH:mm:ss") & "', 102)"
            'AccessFormat = "#" & dteDate.ToString(CultureInfo.InvariantCulture.DateTimeFormat) & "#"
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed-in Integer value to a SQL-friendly value
    ''' </summary>
    ''' <param name="intInteger" type="Integer">
    ''' The Integer value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed-in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal intInteger As Integer) As String
        If intInteger = NULL_NUMBER Then
            SQLFormat = "NULL"
        Else
            SQLFormat = intInteger.ToString
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed-in Decimal value to a SQL-friendly value
    ''' </summary>
    ''' <param name="decDecimal" type="Decimal">
    ''' The Decimal value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed-in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal decDecimal As Decimal) As String
        If decDecimal = NULL_NUMBER Then
            SQLFormat = "NULL"
        Else
            SQLFormat = decDecimal.ToString(CultureInfo.InvariantCulture.NumberFormat)
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed-in Guid value to a SQL-friendly value
    ''' </summary>
    ''' <param name="guidGuid" type="Guid">
    ''' The Guid value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed-in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal guidGuid As Guid) As String
        If guidGuid.Equals(Guid.Empty) Then
            SQLFormat = "NULL"
        Else
            SQLFormat = "'{" & guidGuid.ToString & "}'"
        End If

    End Function

    ''' <summary>
    ''' The function formats a passed-in String value to a SQL-friendly value
    ''' </summary>
    ''' <param name="strString" type="String">
    ''' The String value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed-in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal strString As String) As String
        If strString Is Nothing Then
            SQLFormat = "NULL"
        Else
            strString = strString.Replace("'", "''")
            SQLFormat = "'" & strString & "'"
        End If
    End Function

    ''' <summary>
    ''' The function formats a passed-in Boolean value to a SQL-friendly value
    ''' </summary>
    ''' <param name="blnBoolean" type="Boolean">
    ''' The Boolean value to be formatted
    ''' </param>
    ''' <returns>
    ''' A formatted version (SQL-friendly string) of the passed-in parameter
    ''' </returns>
    Public Function SQLFormat(ByVal blnBoolean As Boolean) As String
        If blnBoolean Then
            SQLFormat = "1"
        Else
            SQLFormat = "0"
        End If
    End Function

#End Region

#Region "LoadDBValue"
    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="dteDate" type="Date">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef dteDate As Date)
        If TypeOf objObject Is DBNull Then
            dteDate = Date.MinValue
        Else
            dteDate = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="intInteger" type="Integer">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef intInteger As Integer)
        If TypeOf objObject Is DBNull Then
            intInteger = NULL_NUMBER
        Else
            intInteger = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="lngLong" type="Long">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef lngLong As Long)
        If TypeOf objObject Is DBNull Then
            lngLong = NULL_NUMBER
        Else
            lngLong = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="decDecimal" type="Decimal">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef decDecimal As Decimal)
        If TypeOf objObject Is DBNull Then
            decDecimal = NULL_NUMBER
        Else
            decDecimal = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="strString" type="String">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef strString As String)
        If TypeOf objObject Is DBNull Then
            strString = ""
        Else
            strString = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="guidGUID" type="Guid">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef guidGUID As Guid)
        If TypeOf objObject Is DBNull Then
            guidGUID = Guid.Empty
        Else
            guidGUID = objObject
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="blnBoolean" type="Boolean">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef blnBoolean As Boolean)
        If TypeOf objObject Is DBNull Then
            blnBoolean = False
        Else
            blnBoolean = CBool(objObject)
        End If
    End Sub

    ''' <summary>
    ''' The function loads the Value of the 1st parametre into the second. 
    ''' If the fist parameter is Null, then a Library default Null value is assigned - (like NULL_NUMBER, Date.MinValue)
    ''' </summary>
    ''' <param name="objObject" type="Object">
    ''' The value of the object (usually coming from a cell in the Datatable or datarow) to be loaded into the 2nd Parameter
    ''' </param>
    ''' <param name="objVariable" type="Object">
    ''' The Value in which the DB value (usually from a cell in the Datatable or datarow) is to be loaded
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub LoadDBValue(ByVal objObject As Object, ByRef objVariable As Object)
        If TypeOf objObject Is DBNull Then
            objVariable = Nothing
        Else
            objVariable = objObject
        End If
    End Sub

#End Region

#Region "DisplayDBValue"
    ''' <summary>
    ''' The function formats the passed-in parameter as a readable string and returns that.
    ''' </summary>
    ''' <param name="intInteger" type="Integer">
    ''' The value in the DB to be displayed in a well formatted way.
    ''' </param>
    ''' <returns>
    ''' The formatted readable string value
    ''' </returns>
    Public Function DisplayDBValue(ByVal intInteger As Integer) As String
        If intInteger = NULL_NUMBER Then
            DisplayDBValue = ""
        Else
            DisplayDBValue = intInteger.ToString
        End If
    End Function

    ''' <summary>
    ''' The function formats the passed-in parameter as a readable string and returns that.
    ''' </summary>
    ''' <param name="decDecimal" type="Decimal">
    ''' The value in the DB to be displayed in a well formatted way.
    ''' </param>
    ''' <returns>
    ''' The formatted readable string value
    ''' </returns>
    Public Function DisplayDBValue(ByVal decDecimal As Decimal) As String
        If decDecimal = NULL_NUMBER Then
            DisplayDBValue = ""
        Else
            DisplayDBValue = decDecimal.ToString(CultureInfo.InvariantCulture.NumberFormat)
        End If
    End Function

    ''' <summary>
    ''' The function formats the passed-in parameter as a readable string and returns that.
    ''' </summary>
    ''' <param name="dteDate" type="Date">
    ''' The value in the DB to be displayed in a well formatted way.
    ''' </param>
    ''' <param name="strFormat" type="String">
    ''' Optional - The format in which the Date value should be displayed (eg. "dd MM yyyy")
    ''' </param>
    ''' <returns>
    ''' The formatted readable string value
    ''' </returns>
    Public Function DisplayDBValue(ByVal dteDate As Date, Optional ByVal strFormat As String = "") As String
        If dteDate = Date.MinValue Then
            DisplayDBValue = ""
        Else
            If strFormat > "" Then
                DisplayDBValue = Format(dteDate, strFormat)
            Else
                DisplayDBValue = Format(dteDate, "dd MMM yyyy")
            End If
        End If
    End Function
#End Region

#Region "Others"
    ''' <summary>
    ''' The function determines the OleDBConnectionProvider from the Oledb object.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' The oledb coneection of which the provider must be determined.
    ''' </param>
    ''' <returns>
    ''' An integer value indicating what provider is attached to the oledb connection 
    ''' </returns>
    Public Function DBConnectionProvider(ByVal conDB As OleDb.OleDbConnection) As OleDBConnectionProvider
        Dim intReturn As Integer

        If conDB Is Nothing Then
            conDB = New OleDb.OleDbConnection(strConnectionString)
        End If

        Select Case UCase(conDB.Provider)
            Case "SQLOLEDB.1"
                intReturn = OleDBConnectionProvider.SQLServer2000
            Case "MICROSOFT.JET.OLEDB.4.0"
                intReturn = OleDBConnectionProvider.MicrosoftJet4
        End Select

        Return intReturn

    End Function

    ''' <summary>
    ''' The function determines the OleDBConnectionProvider from the connection string supplied
    ''' Gets the Oledb connection object from the connection string and uses the function above to determine the provider
    ''' </summary>
    ''' <param name="strConnectionString" type="String">
    ''' The connection string of which the provider must be determined.
    ''' </param>
    ''' <returns>
    ''' An integer value indicating what provider is attached to the connection string
    ''' </returns>
    Public Function DBConnectionProvider(ByVal strConnectionString As String) As OleDBConnectionProvider
        Dim intReturn As Integer
        Dim conDB As OleDb.OleDbConnection

        conDB = New OleDb.OleDbConnection(strConnectionString)

        intReturn = DBConnectionProvider(conDB)

        Return intReturn

    End Function

    ''' <summary>
    ''' The function Opens the connection if it is not.
    ''' Determines what connection provider is attached to the connection object
    ''' Formats the object into a format that is either SQL-friendly or access-query-friendly depending on the connection provider
    ''' if SQL provider - uses sqlformat else uses AccessFormat
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' The connection object of which to get the connection provider
    ''' </param>
    ''' <param name="objObject" type="Object">
    ''' The object value to be formatted in a DB friendly string
    ''' </param>
    ''' <returns>
    ''' A string value that can be used in either Access Query or a SQL statement
    ''' </returns>
    Public Function DBFormat(ByVal conDB As OleDb.OleDbConnection, ByVal objObject As Object) As String
        Dim strReturn As String

        'Error check that the connection is not nothing
        If conDB Is Nothing Then
            OpenDBConnection(conDB)
        End If

        Select Case DBConnectionProvider(conDB)
            Case OleDBConnectionProvider.SQLServer2000
                strReturn = SQLFormat(objObject)
            Case OleDBConnectionProvider.MicrosoftJet4
                strReturn = AccessFormat(objObject)
            Case Else
                If strConnectionString > "" Then
                    strReturn = DBFormat(strConnectionString, objObject)
                Else
                    strReturn = objObject.ToString
                End If
        End Select

        Return strReturn
    End Function

    ''' <summary>
    ''' The function gets the connection object from the connection string supplied
    ''' Then uses the function above to format the object supplied
    ''' </summary>
    ''' <param name="strConnectionString" type="String">
    ''' The connection string of which to get the connection string
    ''' </param>
    ''' <param name="objObject" type="Object">
    ''' The object value to be formatted in a DB friendly string
    ''' </param>
    ''' <returns>
    ''' A string value that can be used in either Access Query or a SQL statement
    ''' </returns>
    Public Function DBFormat(ByVal strConnectionString As String, ByVal objObject As Object) As String
        Dim conDB As OleDb.OleDbConnection
        Dim strReturn As String

        conDB = New OleDb.OleDbConnection(strConnectionString)

        strReturn = DBFormat(conDB, objObject)

        Return strReturn

    End Function

    ''' <summary>
    ''' The function gets a datatable from the supplied datatable with distinct value of the strFieldNames
    ''' </summary>
    ''' <param name="dtSourceTable" type="DataTable">
    ''' The datatble from which to get a datatable with distinct values
    ''' </param>
    ''' <param name="strFieldNames" type="String">
    ''' The field names of which to get the distinct values in the datatable.
    ''' </param>
    ''' <returns>
    ''' A datatable with distinct values of the fields; strFieldnames
    ''' </returns>
    Public Function SelectDistinctFromDataTable(ByVal dtSourceTable As DataTable, ByVal ParamArray strFieldNames() As String) As DataTable
        Dim strFieldName As String
        Dim objLastValues() As Object
        Dim dtReturn As DataTable
        Dim drSource As DataRow
        Dim dcSourceColumn As New DataColumn
        Dim drColumnSelection() As DataRow

        If strFieldNames Is Nothing OrElse strFieldNames.Length = 0 Then
            Throw New ArgumentNullException("strFieldNames")
        End If

        objLastValues = New Object(strFieldNames.Length - 1) {}
        dtReturn = New DataTable

        For Each dcSourceColumn In dtSourceTable.Columns
            dtReturn.Columns.Add(dcSourceColumn.ColumnName, dcSourceColumn.DataType)
        Next

        If strFieldNames.Length > 1 Then
            drColumnSelection = dtSourceTable.Select("", String.Join(", ", strFieldNames))
        Else
            drColumnSelection = dtSourceTable.Select("", strFieldNames(0))
        End If

        For Each drSource In drColumnSelection
            If Not DataRowValuesAreEqual(objLastValues, drSource, strFieldNames) Then
                dtReturn.Rows.Add(CopyDataRowValues(drSource, dtReturn.NewRow(), strFieldNames))
                ExtractDataRowValues(objLastValues, drSource, strFieldNames)
            End If
        Next

        Return dtReturn

    End Function

    Public Function SumColumnValues(ByVal dtDataTable As DataTable, ByVal strNumericFieldName As String, ByVal strWhere As String) As Double
        'Sums the values in the source table's specified column
        'The computation is additionally filtered by the where clause

        Dim dblReturn As Double

        If Not dtDataTable Is Nothing Then
            dblReturn = dtDataTable.Compute("SUM(" & strNumericFieldName & ")", strWhere)
        End If

        Return dblReturn

    End Function

    ''' <summary>
    ''' The function checks if values in columns(specified) of a datarow are the same.
    ''' </summary>
    ''' <param name="objLastValues" type="Object">
    ''' </param>
    ''' <param name="drCurrentRow" type="DataRow">
    ''' The datarow in which to check for duplicate values
    ''' </param>
    ''' <param name="strFieldNames" type="String">
    ''' The names of columns in which to check for duplicates.
    ''' </param>
    ''' <returns>
    ''' A true/false indicating if values in 2 columns(specified) of the datarow are the same.
    ''' </returns>
    Private Function DataRowValuesAreEqual(ByVal objLastValues() As Object, ByVal drCurrentRow As DataRow, ByVal strFieldNames() As String) As Boolean
        Dim intCount As Integer
        Dim blnReturn As Boolean

        blnReturn = True

        For intCount = 0 To strFieldNames.Length - 1
            Select Case True
                Case objLastValues(intCount) Is Nothing
                    blnReturn = False
                Case objLastValues(intCount).Equals(drCurrentRow(strFieldNames(intCount)))
                Case Else
                    blnReturn = False
            End Select
            If Not blnReturn Then
                Exit For
            End If
        Next

        Return blnReturn

    End Function
    ''' <summary>
    ''' The function Copies datarow values from the source datarow to the destination
    ''' Note, the datarows need to have the same structure or this function will fail
    ''' </summary>
    ''' <param name="drSourceRow" type="DataRow">
    ''' The datarow from which the values are to be copied to the destination
    ''' </param>
    ''' <param name="drDestinationRow" type="DataRow">
    ''' The final datarow with the values of the source datarow values
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Sub CopyDataRowValues(ByVal drSourceRow As DataRow, ByRef drDestinationRow As DataRow)
        'Copies datarow values from the source datarow to the destination
        'datarow
        'Note, the datarows need to have the same structure or this 
        'function will fail

        Dim intField As Integer

        For intField = 0 To drDestinationRow.ItemArray.Length - 1
            drDestinationRow(intField) = drSourceRow(intField)
        Next

    End Sub

    ''' <summary>
    ''' The function Copies datarow values from the source datarow to the destination (for specified column names only)
    ''' Note, the datarows need to have the same structure or this function will fail
    ''' </summary>
    ''' <param name="drSourceRow" type="DataRow">
    ''' The datarow from which the values are to be copied to the destination (for specified column names only)
    ''' </param>
    ''' <param name="drDestinationRow" type="DataRow">
    ''' The final datarow with the values (for specified column names only) of the source datarow values 
    ''' </param>
    ''' <param name="strFieldNames" type="String">
    ''' The names of the columns for which the values are to be copied to the destination row.
    ''' </param>
    ''' <returns>
    ''' The destination row with the copied values 
    ''' </returns>
    Public Function CopyDataRowValues(ByVal drSourceRow As DataRow, ByVal drDestinationRow As DataRow, ByVal strFieldNames() As String) As DataRow
        'Copies datarow values from the source datarow to the destination
        'datarow
        'Note, the datarows need to have the same structure or this 
        'function will fail

        Dim strFieldName As String

        For Each strFieldName In strFieldNames
            drDestinationRow(strFieldName) = drSourceRow(strFieldName)
        Next

        Return drDestinationRow

    End Function

    ''' <summary>
    ''' The function creates an array of objects with the values in the source datarow in the specified column names.
    ''' </summary>
    ''' <param name="drSourceRow" type="DataRow">
    ''' The datarow from which the values are to be copied to the new object
    ''' </param>
    ''' <param name="objLastValues" type="Object">
    ''' The array of objects to which values in the datarow will be copied for the specified column names.
    ''' </param>
    ''' <param name="strFieldNames" type="String">
    ''' The names of the columns for which the values are to be copied to the new object array
    ''' </param>
    ''' <returns>
    ''' The object array with the values from the datarow for the specified column names.
    ''' </returns>
    Private Sub ExtractDataRowValues(ByVal objLastValues() As Object, ByVal drSourceRow As DataRow, ByVal strFieldNames() As String)
        Dim intCount As Integer

        For intCount = 0 To strFieldNames.Length - 1
            objLastValues(intCount) = drSourceRow(strFieldNames(intCount))
        Next
    End Sub

    ''' <summary>
    ''' The function Gets a datatable of all the tables in the db connection
    ''' </summary>
    ''' <param name="conn" type="OleDbConnection">
    ''' The Oledb connection object
    ''' </param>
    ''' <returns>
    ''' A datatable from the DB connection 
    ''' </returns>
    Public Function GetTableNames(ByVal conn As OleDbConnection) As DataTable
        'Gets a datatable of all the tables in the db connection

        Dim dtReturn As DataTable

        If OpenDBConnection(conn) Then

            dtReturn = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New Object() {Nothing, Nothing, Nothing, "TABLE"})

        End If

        Return dtReturn

    End Function

    ''' <summary>
    ''' The function Creates as connection string for a access database
    ''' </summary>
    ''' <param name="strDataSource" type="String">
    ''' The datasource of which to get the connection string.
    ''' </param>
    ''' <returns>
    ''' A Jet OLEDB Connection string - For MSAccess DB connection
    ''' </returns>
    Public Function CreateConnectionString_Access(ByVal strDataSource As String) As String
        'Creates as connectionstring for a access database
        Dim strConnectString As String

        strConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strDataSource & ";Persist Security Info=False"

        Return strConnectString
    End Function

    ''' <summary>
    ''' The function Creates as connection string for an excel database
    ''' </summary>
    ''' <param name="strDataSource" type="String">
    ''' The datasource of which to get the connection string.
    ''' </param>
    ''' <returns>
    ''' A Jet OLEDB Connection string - For MSExcel DB connection
    ''' </returns>
    Public Function CreateConnectionString_Excel(ByVal strDataSource As String) As String
        'Creates as connectionstring for a access database
        Dim strConnectString As String

        strConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strDataSource & ";Extended Properties=Excel 8.0;"

        Return strConnectString
    End Function

    ''' <summary>
    ''' The function starts up the specified transaction
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' The datasource of which to get the connection string.
    ''' </param>
    ''' <returns>
    ''' The transaction initiated.
    ''' </returns>
    Public Function BeginTransaction(ByRef conDB As OleDb.OleDbConnection) As OleDb.OleDbTransaction
        Dim transReturn As OleDb.OleDbTransaction

        If OpenDBConnection(conDB) Then
            transReturn = conDB.BeginTransaction
        End If

        Return transReturn

    End Function

    ''' <summary>
    ''' The Function creates a where clause from the arraylist of guids, combining them with the logic in parameterblnLogic
    ''' </summary>
    ''' <param name="ArrayListOfGUIDS" type="ArrayList">
    ''' The arraylist that contains the guid values used to formulate the where clause.
    ''' </param>
    ''' <param name="strIDColumn" type="String">
    ''' The column column name from which are equaled to the arraylist values
    ''' </param>
    ''' <param name="blnLogic" type="String">
    ''' Either to use OR or AND in the whereclause
    ''' </param>
    ''' <returns>
    ''' A string value of the where clause
    ''' </returns>
    Public Function WhereClauseFromArrayListOfGuids(ByRef ArrayListOfGUIDS As ArrayList, ByVal strIDColumn As String, Optional ByVal blnLogic As String = "or") As String
        'Function creates a where clause from the arraylist of guids, combining
        'them with the logic in parameterblnLogic

        Dim strWhere As String
        Dim intGUI As Integer
        Dim guidCurrent As Guid

        For intGUI = 0 To ArrayListOfGUIDS.Count - 1
            guidCurrent = ArrayListOfGUIDS.Item(intGUI)
            strWhere += strIDColumn & " = " & DBFormat(strConnectionString, guidCurrent)

            If Not intGUI = (ArrayListOfGUIDS.Count - 1) Then    'not last one
                strWhere += " " & blnLogic & " "
            End If
        Next

        Return strWhere
    End Function

    ''' <summary>
    ''' The Function gets a datareader from the execution of the SQL statement
    ''' Populates the hashtable supplied with the values in the datareader.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' The oledb connection object
    ''' </param>
    ''' <param name="hashTable" type="Hashtable">
    ''' The hashtable to populate the values from the datareader.
    ''' </param>
    ''' <param name="strSQL" type="String">
    ''' The sql statement to execute to obtain the datareader.
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Overloads Sub PopulateHashTable(ByRef conDB As OleDb.OleDbConnection, ByRef hashTable As Hashtable, ByVal strSQL As String)
        Dim drResult As OleDb.OleDbDataReader

        If hashTable Is Nothing Then
            hashTable = New Hashtable
        End If

        Try
            drResult = GetDataReader(conDB, strSQL)
            Do While drResult.Read
                hashTable.Add(drResult.Item(0), drResult.Item(1))
            Loop
        Catch ex As Exception
        Finally
            If Not drResult Is Nothing Then
                drResult.Close()
                drResult = Nothing
            End If
        End Try

    End Sub

    ''' <summary>
    ''' The Function creates a sql statement from the supplied parameters, and then uses the function above to populate the supplied hashtable.
    ''' </summary>
    ''' <param name="conDB" type="OleDbConnection">
    ''' The oledb connection object
    ''' </param>
    ''' <param name="hashTable" type="Hashtable">
    ''' The hashtable to populate 
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name in the DB from which to obtain a datareader
    ''' </param>
    ''' <param name="strTableName" type="String">
    ''' The table name in the DB from which to obtain a datareader
    ''' </param>
    ''' <param name="strKeyField" type="String">
    ''' The name of the column to obtain from the DB (usually the ID column)
    ''' </param>
    ''' <param name="strValueField" type="String">
    ''' The name of the other column to obtain from the DB (usually the text-value column)
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Overloads Sub PopulateHashTable(ByRef conDB As OleDb.OleDbConnection, ByRef hashTable As Hashtable, ByVal strTableName As String, ByVal strKeyField As String, ByVal strValueField As String)
        Dim strSQL As String

        strSQL = "SELECT " & strKeyField & ", " & strValueField & " FROM " & strTableName
        PopulateHashTable(conDB, hashTable, strSQL)

    End Sub

    Public Function UpdateTableValue(ByRef conDb As OleDb.OleDbConnection, ByVal strTable As String, ByVal strIdColumn As String, ByRef objIdValue As Object, ByVal strUpdateColumn As String, ByRef objUpdateValue As Object) As Boolean
        Dim strSQL As String

        strSQL = "UPDATE " & strTable & " SET " & strUpdateColumn & " = " & DBFormat(conDb, objUpdateValue) & " WHERE " & _
                    " " & strIdColumn & " = " & DBFormat(conDb, objIdValue)

        ExecuteSQL(conDb, strSQL)
    End Function

#End Region

#Region "Get Column Details"

    Public Function GetTable_ColumnDetails(ByRef conDb As OleDb.OleDbConnection, ByVal strTableName As String, ByVal strColumnName As String) As DataTable
        Dim strSQL As String
        Dim dtReturn As DataTable

        strSQL = "SELECT syscolumns.name AS ColumnName, syscolumns.prec, syscolumns.scale, " & _
         " syscolumns.xtype AS DataTypeCode, syscolumns.isnullable FROM syscolumns " & _
         " INNER JOIN sysobjects ON sysobjects.id = syscolumns.id WHERE " & _
         " (sysobjects.name = " & SQLFormat(strTableName) & ") AND (syscolumns.name = " & SQLFormat(strColumnName) & ")"

        dtReturn = GetDataTable(conDb, strSQL)

        Return dtReturn
    End Function
#End Region

End Module
