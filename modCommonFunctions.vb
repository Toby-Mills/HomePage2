
Public Module modCommonFunctions

    Public Function GetIndexByText(ByVal ctlControl As Object, ByVal strText As String) As Integer
        'Takes a combobox and a value. Searches the list of items for the first one with the specified text (rather than value)
        'returns the index of that item
        Dim intcount As Integer

        GetIndexByText = -1
        For intcount = 0 To ctlControl.Items.Count - 1
            If ctlControl.Items(intcount).text = strText Then
                GetIndexByText = intcount
                Exit Function
            End If
        Next
    End Function

    Public Function RightOf(ByVal theString As String, ByVal theKey As String) As String
        'returns the sub string to the right of the first occurance of theKey
        'check starts from the right side - if not found returns the same string
        'example: rightof("xxx\yyyy\zzzz","\") returns "zzzz"

        Dim l As Integer, lk As Integer, a As Integer

        RightOf = theString 'default return
        Try
            l = Len(theString)
            lk = Len(theKey)
            a = InStrRev(theString, theKey, -1, vbTextCompare) 'InStr(1, str, tabstr, 1)
            If (a > 0) Then
                RightOf = Right(theString, l - (a + lk - 1))
            Else
                RightOf = theString
            End If
        Catch ex As Exception
        End Try
    End Function
	Public Function LeftOf(ByVal strString As String, ByVal strKey As String) As String
		'returns the sub string left of strKey
		'search for theKey starts from the left side of the string
		'example: leftof("xxx\yyyy\zzzz","\") returns "xxx"

		Dim lngInString As Long
		Dim strReturn As String

		Try
			strReturn = strString		 'default return
			lngInString = InStr(1, strString, strKey, 1)
			If (lngInString > 0) Then
				strReturn = Left(strString, lngInString - 1)
			Else
				strReturn = strString
			End If
		Catch ex As Exception

		End Try

		Return strReturn

	End Function

	Public Sub Info(ByVal Message As String)
		'displays an information message
		MsgBox(Message, MsgBoxStyle.Information, "Information")
	End Sub

	Public Sub Warning(ByVal Message As String)
		'displays an warning message
		MsgBox(Message, MsgBoxStyle.Exclamation, "Warning")
	End Sub

	Public Sub ErrorMsg(ByVal Message As String, Optional ByVal RoutineName As String = "")
		'displays a error message information message
		If Len(Message) > 200 Then Message = Left(Message, 200) & "..."
		If RoutineName = "" Then
			MsgBox(Message, MsgBoxStyle.Critical, "Error ")
		Else
			MsgBox(Message, MsgBoxStyle.Critical, "Error (" & RoutineName & ")")
		End If
	End Sub

	Public Function NewGUID() As String

		NewGUID = Guid.NewGuid().ToString()

	End Function

	Public Function StringToGUID(ByVal InString As String) As Guid
		Dim objConverter As New System.ComponentModel.GuidConverter
		Dim guidOut As Guid

		Try
			guidOut = objConverter.ConvertFromString(InString)
		Catch ex As Exception
			Return Nothing
		End Try
		Return guidOut

	End Function


	Public Function FileName(ByVal strPath As String) As String
		Dim strToken() As String

		strToken = strPath.Split(CChar("/"), CChar("\"))
		FileName = strToken(UBound(strToken))
	End Function

	Public Function FilePath(ByVal strPath As String) As String
		Dim strFileName As String

		strFileName = FileName(strPath)
		If Len(strPath) > Len(strFileName) + 1 Then
			FilePath = Left(strPath, Len(strPath) - (Len(strFileName) + 1))
		Else
			FilePath = ""
		End If

    End Function

    Public Function URIPathToFilePath(ByVal strURIPath As String) As String

        Dim strReturn As String

        strReturn = strURIPath.Replace("file:///", "")
        strReturn = strReturn.Replace("/", "\")

        Return strReturn

    End Function

    Public Function PathExists(ByVal strPath As String, ByVal blnCreateIfMissing As Boolean) As Boolean
        Dim dir As System.IO.DirectoryInfo

        PathExists = False

        dir = New System.IO.DirectoryInfo(strPath)
        If dir.Exists Then
            PathExists = True
        Else
            If blnCreateIfMissing = True Then
                Try
                    dir.Create()
                    dir = New System.IO.DirectoryInfo(strPath)
                    If dir.Exists Then
                        PathExists = True
                    End If
                Catch ex As Exception
                    PathExists = False
                End Try
            End If
        End If

    End Function

    Public Function FilePathToURL(ByVal strPath As String) As String
        Dim strURL As String

        strPath = strPath.Replace("\", "/")
        strURL = "File:///" & strPath

        Return strURL

    End Function

    Public Function IsStringAnDouble(ByVal strInput As String) As Boolean
        Dim dblParse As Double
        Dim blnIsInteger As Boolean
        'try to convert value to int by choosing the middle value
        Try
            dblParse.Parse(strInput)
            blnIsInteger = True
        Catch ex As Exception
            blnIsInteger = False
        End Try

        Return blnIsInteger
    End Function

    Public Function Extension(ByVal strFileName As String) As String

        Dim strName() As String

        strName = Split(strFileName, ".")
        Extension = strName(UBound(strName))

    End Function

    Public Function CollectionContainsItem(ByRef colCollection As Collection, ByVal strKey As String) As Boolean
        'Function checks if the collection contains the item with key "strKey"

        Dim objObject As Object

        Try
            objObject = colCollection.Item(strKey)
        Catch ex As Exception
            CollectionContainsItem = False
        End Try

        If Not objObject Is Nothing Then
            CollectionContainsItem = True
        End If

    End Function

    Public Function DataTableContainsRow(ByVal dtTable As DataTable, ByVal strPrimaryKeyColumnName As String, ByVal objKeyValue As Object) As Boolean
        'Functions checks if the row (specified by column value) is
        'in the datatable

        Dim colArr(1) As DataColumn
        Dim blnReturn As Boolean

        'Set the table primary key
        colArr(0) = dtTable.Columns(strPrimaryKeyColumnName)
        dtTable.PrimaryKey = colArr

        'Check if row is in the list
        blnReturn = Not dtTable.Rows.Find(objKeyValue) Is Nothing

        Return blnReturn

    End Function

    Public Function RoundOff(ByVal decDecimal As Decimal, ByVal intDecimalPlaces As Integer) As Decimal
        'Round a decimal number Upward or Downward to the specified no. of decimal places
        Dim intScale As Integer
        Dim decBig As Decimal
        Dim decSmall As Decimal
        Dim intRemainder As Integer
        Dim decReturn As Decimal

        intScale = 10 ^ intDecimalPlaces

        decBig = decDecimal * (intScale * 10)
        decBig = Decimal.Floor(decBig)

        decSmall = decDecimal * intScale
        decSmall = Decimal.Floor(decSmall)
        decSmall *= 10

        intRemainder = decBig - decSmall

        If intRemainder >= 5 Then
            decSmall += 10
        End If

        decReturn = decSmall / (intScale * 10)

        Return decReturn
    End Function

    Public Function RoundDown(ByVal decDecimal As Decimal, ByVal intDecimalPlaces As Integer) As Decimal
        'Round a decimal number Downward to the specified no. of decimal places
        Dim intScale As Integer
        Dim decReturn As Decimal

        intScale = 10 ^ intDecimalPlaces

        decReturn = decDecimal * intScale
        decReturn = Decimal.Floor(decReturn)
        decReturn /= intScale

        Return decReturn

    End Function

    Public Function RoundUp(ByVal decDecimal As Decimal, ByVal intDecimalPlaces As Integer) As Decimal
        'Round a decimal number Upward to the specified no. of decimal places
        Dim intScale As Integer
        Dim decSmall As Decimal
        Dim intRemainder As Integer
        Dim decReturn As Decimal

        intScale = 10 ^ intDecimalPlaces

        decReturn = decDecimal * intScale
        decSmall = Decimal.Floor(decReturn)

        If decSmall < decReturn Then
            decReturn = decSmall + 1
        End If

        decReturn /= intScale

        Return decReturn

    End Function

    Public Function Ordinal(ByVal intInteger As Integer) As String
        'takes an integer and returns the appropriate Ordinal
        'e.g. 1 = 1st, 2 = 2nd, 3 = 3rd etc.
        Dim strReturn As String
        Dim strLastTwoDigits As String
        Dim charLastDigit As Char

        strReturn = intInteger.ToString
        If Len(strReturn) > 1 Then
            strLastTwoDigits = Right(strReturn, 2)
        End If
        charLastDigit = Right(strReturn, 1)

        Select Case charLastDigit
            Case Is = "0"
            Case Is = "1"
                If strLastTwoDigits = "11" Then
                    strReturn &= "th"
                Else
                    strReturn &= "st"
                End If
            Case Is = "2"
                If strLastTwoDigits = "12" Then
                    strReturn &= "th"
                Else
                    strReturn &= "nd"
                End If
            Case Is = "3"
                If strLastTwoDigits = "13" Then
                    strReturn &= "th"
                Else
                    strReturn &= "rd"
                End If
            Case Else
                strReturn &= "th"
        End Select

        Return strReturn

    End Function

    Public Function GetFileAsByteArray(ByVal strFilePath As String) As Byte()
        Dim filestream As IO.FileStream
        Dim binaryreader As IO.BinaryReader
        Dim byteFile() As Byte

        filestream = New IO.FileStream(strFilePath, IO.FileMode.Open, IO.FileAccess.Read)
        binaryreader = New IO.BinaryReader(filestream)
        byteFile = binaryreader.ReadBytes(filestream.Length)

        binaryreader.Close()
        filestream.Close()

        Return byteFile
    End Function

    'checks whether a date in a string format ia a date
    'converts string date to an actual date and validates
    Public Function ValidateDate(ByVal strDate As String, ByVal strDateFormat As String) As Boolean
        Dim blnReturn As Boolean
        Dim dteValue As Date

        Try
            dteValue = Date.ParseExact(strDate, strDateFormat, Nothing)
            blnReturn = True
        Catch ex As Exception
            blnReturn = False
        End Try

        Return blnReturn
    End Function

    Public Function ValidateEmail(ByVal strEmail As String) As String
        Dim strTemp As String
        Dim counter As Long
        Dim strEXT As String

        'set to blnak string if validation fails then
        'a appropriate error message will be provided, else
        'string will remain blank
        ValidateEmail = ""

        If InStr(1, strEmail, "@") = 0 Then
            'email address does not contain an @
            ValidateEmail = "* Your email address does not contain an @ sign."
        ElseIf InStr(1, strEmail, "@") = 1 Then
            '@ character cannot be the first character
            ValidateEmail = "* Your @ sign can not be the first character in your email address!"
        ElseIf InStr(1, strEmail, "@") = Len(strEmail) Then
            '@ can not be  the last character
            ValidateEmail = "* Your @ sign can not be the last character in your email address!"
        ElseIf Len(strEmail) < 6 Then
            'email address is shorter than 6 characters
            ValidateEmail = "* Your email address is shorter than 6 characters which is impossible."
        End If

        strTemp = strEmail
        Do While InStr(1, strTemp, "@") <> 0
            counter = counter + 1
            strTemp = Right(strTemp, Len(strTemp) - InStr(1, strTemp, "@"))
        Loop

        If counter > 1 Then
            'more than one @
            ValidateEmail = "* You have more than 1 @ sign in your email address"
        End If
    End Function

    Public Function GuidIsEmpty(ByVal guid As Guid) As Boolean
        Return guid.Equals(guid.Empty)
    End Function

    Public Function CalculateGridReference(ByVal decDecimalDegreesY As Decimal, ByVal decDecimalDegreesX As Decimal) As String

        Dim intXX, intYY As Integer
        Dim decXX, decYY As Decimal
        Dim strXX, strYY, strGridRef1, strGridRef2, strGridReference As String

        intYY = Math.Floor(Math.Abs(decDecimalDegreesY))
        intXX = Math.Floor(Math.Abs(decDecimalDegreesX))

        'decYY = Math.Abs(Math.Abs(Math.Floor(decDecimalDegreesY)) - Math.Abs(decDecimalDegreesY))
        'decXX = Math.Abs(Math.Abs(Math.Floor(decDecimalDegreesX)) - Math.Abs(decDecimalDegreesX))

        decYY = Math.Abs(Math.Floor(Math.Abs(decDecimalDegreesY)) - Math.Abs(decDecimalDegreesY))
        decXX = Math.Abs(Math.Floor(Math.Abs(decDecimalDegreesX)) - Math.Abs(decDecimalDegreesX))


        If decYY >= 0 And decYY < 0.5 Then

            If decXX >= 0 And decXX < 0.5 Then
                strGridRef1 = "A"
                If decYY >= 0 And decYY < 0.25 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.25 And decYY < 0.5 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.5 And decYY < 0.75 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.75 And decYY < 1 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                Else
                    strGridRef2 = ""
                End If
            ElseIf decXX >= 0.5 And decXX < 1 Then
                strGridRef1 = "B"
                If decYY >= 0 And decYY < 0.25 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.25 And decYY < 0.5 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.5 And decYY < 0.75 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.75 And decYY < 1 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                Else
                    strGridRef2 = ""
                End If
            End If
        ElseIf decYY >= 0.5 And decYY < 1 Then
            If decXX >= 0 And decXX < 0.5 Then
                strGridRef1 = "C"
                If decYY >= 0 And decYY < 0.25 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.25 And decYY < 0.5 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.5 And decYY < 0.75 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.75 And decYY < 1 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                Else
                    strGridRef2 = ""
                End If
            ElseIf decXX >= 0.5 And decXX < 1 Then
                strGridRef1 = "D"
                If decYY >= 0 And decYY < 0.25 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.25 And decYY < 0.5 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.5 And decYY < 0.75 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "B"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "A"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "B"
                    Else
                        strGridRef2 = ""
                    End If
                ElseIf decYY >= 0.75 And decYY < 1 Then
                    If decXX >= 0 And decXX < 0.25 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.25 And decXX < 0.5 Then
                        strGridRef2 = "D"
                    ElseIf decXX >= 0.5 And decXX < 0.75 Then
                        strGridRef2 = "C"
                    ElseIf decXX >= 0.75 And decXX < 1 Then
                        strGridRef2 = "D"
                    Else
                        strGridRef2 = ""
                    End If
                Else
                    strGridRef2 = ""
                End If
            End If
        Else
            strGridRef1 = ""
        End If

        strYY = intYY.ToString
        strXX = intXX.ToString

        If strYY = "0" Then
            strYY = "00"
        End If

        If strXX = "0" Then
            strXX = "00"
        End If
        strGridReference = strYY & strXX & strGridRef1 & strGridRef2

        Return strGridReference
    End Function
End Module
