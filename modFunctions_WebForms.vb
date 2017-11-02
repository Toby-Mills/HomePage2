Imports System.Web.UI.WebControls
Imports System.Data

Public Module modFunctions_WebForms

    Public Function GetIndex(ByVal ctlControl As Web.UI.WebControls.CheckBoxList, ByVal strValue As String) As Integer
        'Takes a combobox and a value. Searches the list of items for the first one with the specified value(rather than text)
        'returns the index of that item
        Dim intcount As Integer

        GetIndex = -1
        If ctlControl.Items.Count > 0 Then
            For intcount = 0 To ctlControl.Items.Count - 1
                If ctlControl.Items(intcount).Value = strValue Then
                    GetIndex = intcount
                    Exit Function
                End If
            Next
        End If

    End Function

    Public Function GetIndex(ByVal ctlControl As Web.UI.WebControls.ListBox, ByVal strValue As String) As Integer
        'Takes a combobox and a value. Searches the list of items for the first one with the specified value(rather than text)
        'returns the index of that item
        Dim intcount As Integer

        GetIndex = -1
        If ctlControl.Items.Count > 0 Then
            For intcount = 0 To ctlControl.Items.Count - 1
                If ctlControl.Items(intcount).Value = strValue Then
                    GetIndex = intcount
                    Exit Function
                End If
            Next
        End If

    End Function

    Public Function GetIndex(ByVal ctlControl As Web.UI.WebControls.RadioButtonList, ByVal strValue As String) As Integer
        'Takes a combobox and a value. Searches the list of items for the first one with the specified value(rather than text)
        'returns the index of that item
        Dim intcount As Integer

        GetIndex = -1
        If ctlControl.Items.Count > 0 Then
            For intcount = 0 To ctlControl.Items.Count - 1
                If ctlControl.Items(intcount).Value = strValue Then
                    GetIndex = intcount
                    Exit Function
                End If
            Next
        End If

    End Function

    Public Function GetIndex(ByVal ctlControl As Web.UI.WebControls.DropDownList, ByVal strValue As String) As Integer
        'Takes a combobox and a value. Searches the list of items for the first one with the specified value(rather than text)
        'returns the index of that item
        Dim intcount As Integer

        GetIndex = -1
        If ctlControl.Items.Count > 0 Then
            For intcount = 0 To ctlControl.Items.Count - 1
                If ctlControl.Items(intcount).Value = strValue Then
                    GetIndex = intcount
                    Exit Function
                End If
            Next
        End If

    End Function

    Public Sub OpenPopUp(ByVal opener As System.Web.UI.WebControls.WebControl, ByVal PagePath As String)
        Dim clientScript As String

        'Building the client script- window.open
        clientScript = "window.open('" & PagePath & "')"
        'regiter the script to the clientside click event of the 'opener' control
        opener.Attributes.Add("onClick", clientScript)
    End Sub

    Public Sub OpenPopUp(ByVal opener As System.Web.UI.WebControls.WebControl, ByVal PagePath As String, ByVal windowName As String, ByVal width As Integer, ByVal height As Integer)
        Dim clientScript As String
        Dim windowAttribs As String

        'Building Client side window attributes with width and height.
        'Also the the window will be positioned to the middle of the screen
        windowAttribs = "width=" & width & "px," & _
                        "height=" & height & "px," & _
                        "left='+((screen.width -" & width & ") / 2)+'," & _
                        "top='+ (screen.height - " & height & ") / 2+'"

        'Building the client script- window.open, with additional parameters
        clientScript = "newwin = window.open('" & PagePath & "','" & windowName & "','" & windowAttribs & "');newwin.focus();return false;"
        'regiter the script to the clientside click event of the 'opener' control
        opener.Attributes.Add("onClick", clientScript)
    End Sub

    Public Sub OpenPopup(ByVal page As Web.UI.Page, ByVal strPagePath As String, ByVal strWindowName As String, Optional ByVal width As Integer = -1, Optional ByVal height As Integer = -1, Optional ByVal strAttributes As String = "")
        Dim strScript As String

        'Building Client side window attributes with width and height.
        'Also the the window will be positioned to the middle of the screen
        If width > -1 Then
            If strAttributes > "" Then
                strAttributes += ", "
            End If
            strAttributes += "width=" & width & ", left='+((screen.width -" & width & ") / 2)+'"
        End If
        If height > -1 Then
            If strAttributes > "" Then
                strAttributes += ", "
            End If
            strAttributes += "height=" & height & ", top='+ (screen.height - " & height & ") / 2+'"
        End If

        strWindowName = strWindowName.Replace(" ", "_")

        strScript = "<script language='javascript'>newwin = window.open('" & strPagePath & "','" & strWindowName & "','" & strAttributes & "');newwin.focus();</script>"
        page.RegisterClientScriptBlock("popup", strScript)

    End Sub

    Public Sub RefreshPopupOpener(ByVal page As Web.UI.Page)
        Dim strScript As String

        strScript = "<script language='javascript'>window.opener.location.href=window.opener.location.href</script>"

        page.RegisterStartupScript("refreshpopupopener", strScript)

    End Sub

    Public Sub RedirectPopupOpener(ByVal page As Web.UI.Page, ByVal strPath As String)
        Dim strScript As String

        strScript = "<script language='javascript'>window.opener.location.href='" & strPath & "'</script>"

        page.RegisterStartupScript("redirectpopupopener", strScript)

    End Sub

    Public Sub CloseWebWindow(ByVal page As Web.UI.Page)
        Dim strScript As String

        strScript = "<script language='javascript'>self.close()</script>"

        page.RegisterClientScriptBlock("closewebwindow", strScript)

    End Sub

	Public Function ExpanderControlScript(ByVal btnExpander As Web.UI.WebControls.Button, ByVal divDisplay As Web.UI.HtmlControls.HtmlGenericControl, ByVal txtDisplay As Web.UI.WebControls.TextBox) As String

		ExpanderControlScript = "DoExpand " & divDisplay.ClientID & ", " & txtDisplay.ClientID
		btnExpander.Attributes("language") = "vbscript"
		btnExpander.Attributes("onclick") = ExpanderControlScript
		btnExpander.CausesValidation = False
		txtDisplay.Style.Add("width", "0")

	End Function

	Public Function ExpanderScript() As String
		ExpanderScript = "<script language='vbscript'>" & vbCrLf & _
		 "<!--" & vbCrLf & _
		 vbTab & "sub DoExpand (divName, txtName)" & vbCrLf & _
		 vbTab & vbTab & "If divname.style.display = ""none"" Then" & vbCrLf & _
		 vbTab & vbTab & vbTab & "divname.style.display = """"" & vbCrLf & _
		 vbTab & vbTab & vbTab & "txtName.value = """"" & vbCrLf & _
		 vbTab & vbTab & "Else" & vbCrLf & _
		 vbTab & vbTab & vbTab & "divname.style.display = ""none""" & vbCrLf & _
		 vbTab & vbTab & vbTab & "txtName.value = ""none""" & vbCrLf & _
		 vbTab & vbTab & "End If" & vbCrLf & _
		 vbTab & vbTab & " window.event.returnValue = false" & vbCrLf & _
		 vbTab & "End Sub" & vbCrLf & _
		 "-->" & vbCrLf & _
		 "</script>"
	End Function

	Public Sub WebMsgBox(ByRef page As Web.UI.Page, ByVal strTitle As String, ByVal strMessage As String)

		strMessage = strMessage.Replace("'", "''")
		page.RegisterClientScriptBlock("message", "<Script language='vbscript'>msgbox """ & strMessage & """,,""" & strTitle & """</script>")

	End Sub

	Public Sub WebFocusControl(ByRef page As Web.UI.Page, ByRef control As Web.UI.WebControls.WebControl)
		Dim strScript As String

		strScript = "<SCRIPT language='vbscript'>Form1." & control.ClientID & ".focus</SCRIPT>"

		page.RegisterStartupScript("Focus", strScript)

	End Sub

	Public Sub WebFocusControl(ByRef page As Web.UI.Page, ByVal strControlID As String)
		Dim strScript As String

		strScript = "<SCRIPT language='vbscript'>" & strControlID & ".focus</SCRIPT>"

		page.RegisterStartupScript("Focus", strScript)

	End Sub

	Public Sub DefaultButton(ByVal objTrigger As Web.UI.WebControls.WebControl, ByVal objTarget As Web.UI.WebControls.WebControl)
		'objTrigger is the control (usually a textbox) which receives the 'Enter' key
		'objTarget is the control who should be clicked as a result

		Dim strScript As String

		strScript = "if(event.keyCode == 13) {document.getElementById('" & objTarget.ClientID & "').click(); return false;} "
		objTrigger.Attributes.Add("onkeypress", strScript)

	End Sub

	Public Sub DefaultButton(ByRef page As Web.UI.Page, ByVal strControlID As String)

		page.RegisterHiddenField("__EVENTTARGET", strControlID)

	End Sub
    Public Sub ShowHourGlass(ByRef page As Web.UI.Page)
        'Changes the mousepointer to a hourglass and disables the input controls
        'The js function should be called from <body onbeforeunload)

        Dim strScript As String
        Dim strControls As String

        'strScript = "<SCRIPT language='javascript'>"
        'strScript &= "function doHourglass(){"
        'strScript &= "document.body.style.cursor = 'wait';"
        'strScript &= "nr = document.forms[0].all.length;"
        'strScript &= "for(i=0;i<nr;i++){"
        'strScript &= "if(document.forms[0].all(i).tagName == ""SELECT"" ||"
        'strScript &= "(document.forms[0].all(i).tagName == ""INPUT"" &&"
        'strScript &= "(document.forms[0].all(i).type == ""radio"" || document.forms[0].all(i).type =="
        'strScript &= """checkbox"" || document.forms[0].all(i).type == ""button"") ))"
        'strScript &= "document.forms[0].all(i).disabled = true;"
        'strScript &= "}"
        'strScript &= "}"
        'strScript &= "</SCRIPT>"

        strControls = DisableControl(page)

        strScript = "<SCRIPT language='javascript'>"
        strScript &= "function doHourglass(){"
        strScript &= "document.body.style.cursor = 'wait';"
        strScript &= strControls
        strScript &= "}"
        strScript &= "</SCRIPT>"

        'Registers the jscript on the page
        page.RegisterStartupScript("ShowHourGlass", strScript)

    End Sub
    Public Function DisableControl(ByVal objControl As Web.UI.Control, Optional ByVal strControlID As String = "") As String
        'This function will return a string of all controls that will 
        'be disabled during a postback. Only controls of type textbox,dropdownlist & button will 
        'be returned. 
        Dim strReturn As String
        Dim objCtl As Web.UI.Control
        Dim objTextBox As TextBox

        strReturn = ""

        For Each objCtl In objControl.Controls
            If objCtl.HasControls Then 'Check to see if the control has controls (i.e. panel)
                If objCtl.Visible = True Then
                    If Right(objCtl.GetType.Name, 4) = "ascx" Then
                        'if the control has child controls and is a user control then pass the name of the user control
                        If strControlID > "" Then
                            strReturn &= DisableControl(objCtl, strControlID & "_" & objCtl.ID)
                        Else
                            strReturn &= DisableControl(objCtl, objCtl.ID)
                        End If
                    Else
                        strReturn &= DisableControl(objCtl, strControlID)
                    End If
                End If
            End If

            'Add the name of the control if it is a textbox, dropdownlist or button
            If TypeOf objCtl Is TextBox Or TypeOf objCtl Is DropDownList Or TypeOf objCtl Is Button Then
                If objCtl.Visible = True Then
                    If objCtl.ID <> "0" Then
                        If strControlID > "" Then
                            strReturn &= "document.forms[0]." & strControlID & "_" & objCtl.ID & ".disabled  = true;"
                        Else
                            strReturn &= "document.forms[0]." & objCtl.ID & ".disabled  = true;"
                        End If
                    End If
                End If
            End If
        Next

        Return strReturn

    End Function
    'Public Sub SortComboBox(ByRef cmb As System.Web.UI.WebControls.DropDownList, Optional ByVal blnAscending As Boolean = True)
    '    'Function sorts the items in the combobox in ascending (blnAscending = True)
    '    'or descending order (blnAscending = false)

    '    Dim intCount As Integer
    '    Dim strValues(cmb.Items.Count) As String
    '    Dim strVal As String

    '    'Loop through all values in the drop down list
    '    For intCount = 0 To cmb.Items.Count - 1
    '        'Write text to string array
    '        strVal = cmb.Items(intCount).Text
    '        strValues.SetValue(strVal, intCount)
    '    Next

    '    'Sort the array
    '    Array.Sort(strValues)

    '    If Not blnAscending Then
    '        Array.Reverse(strValues)
    '    End If

    '    cmb.Items.Clear()

    '    'Loop through all values in the array, add to combo box
    '    For intCount = 0 To strValues.Length - 1
    '        If Not strValues.GetValue(intCount) = "" Then
    '            cmb.Items.Add(strValues.GetValue(intCount))
    '        End If
    '    Next

    'End Sub

    Public Sub SortComboBox(ByRef cmb As System.Web.UI.WebControls.DropDownList, Optional ByVal blnAscending As Boolean = True)
        'Function sorts the items in the combobox in ascending (blnAscending = True)
        'or descending order (blnAscending = false)
        'Saliegh 30 Dec, 2004 - added check for input being an integer, and converting string representation of int to int, and them sorting

        Dim intCount As Integer
        Dim strValues(cmb.Items.Count) As String
        Dim strVal As String
        Dim dblArray(cmb.Items.Count - 1) As Double
        Dim dblParse As Double
        Dim blnIsInteger As Boolean

        'Loop through all values in the drop down list
        For intCount = 0 To cmb.Items.Count - 1
            'Write text to string array
            strVal = cmb.Items(intCount).Text
            strValues.SetValue(strVal, intCount)
        Next

        blnIsInteger = True
        'try to convert value to double by choosing the middle value
        Try
            dblParse.Parse(cmb.Items(cmb.Items.Count / 2).Text)
            dblParse.Parse(cmb.Items(cmb.Items.Count - 1).Text)
            dblParse.Parse(cmb.Items(0).Text)
        Catch ex As Exception
            blnIsInteger = False
        End Try

        ' if the value is an string rep. of an int, parse to double
        If blnIsInteger Then
            For intCount = 0 To cmb.Items.Count - 1
                'convert string to int
                dblArray.SetValue(dblParse.Parse(cmb.Items(intCount).Text), intCount)
            Next
            Array.Sort(dblArray)
            cmb.Items.Clear()
            If Not blnAscending Then
                Array.Reverse(dblArray)
            End If
            'Loop through all values in the array, add to combo box
            For intCount = 0 To dblArray.Length - 1
                If Not dblArray.GetValue(intCount) Is Nothing Then
                    cmb.Items.Add(dblArray.GetValue(intCount))
                End If
            Next
        Else
            Array.Sort(strValues)
            cmb.Items.Clear()
            If Not blnAscending Then
                Array.Reverse(strValues)
            End If
            'Loop through all values in the array, add to combo box
            For intCount = 0 To strValues.Length - 1
                If Not strValues.GetValue(intCount) = "" Then
                    cmb.Items.Add(strValues.GetValue(intCount))
                End If
            Next
        End If
    End Sub
    Public Sub SortListBox(ByRef lst As System.Web.UI.WebControls.ListBox, Optional ByVal blnAscending As Boolean = True)
        'Function sorts the items in the listbox in ascending (blnAscending = True)
        'or descending order (blnAscending = false)

        Dim intCount As Integer
        Dim strValuesArrayKey(lst.Items.Count) As String
        Dim strValuesArrayValue(lst.Items.Count) As String
        Dim strListBoxVal1 As String
        Dim strListBoxVal2 As String

        Dim lstTemp As Web.UI.WebControls.ListBox
        lstTemp = lst

        'Loop through all values in the listbox
        For intCount = 0 To lst.Items.Count - 1
            'Write text to string array
            strListBoxVal1 = lst.Items(intCount).Text
            strListBoxVal2 = lst.Items(intCount).Value
            strValuesArrayKey.SetValue(strListBoxVal1, intCount)
            strValuesArrayValue.SetValue(strListBoxVal2, intCount)
        Next

        'Sort the array
        Array.Sort(strValuesArrayKey, strValuesArrayValue)

        If Not blnAscending Then
            Array.Reverse(strValuesArrayKey)
        End If

        lst.Items.Clear()


        'Loop through all values in the array, add to list box
        For intCount = 0 To strValuesArrayKey.Length - 1
            If Not strValuesArrayKey.GetValue(intCount) = "" Then
                lst.Items.Add(New Web.UI.WebControls.ListItem(strValuesArrayKey.GetValue(intCount), strValuesArrayValue.GetValue(intCount)))
            End If
        Next

    End Sub
    Public Function GetPageUser(ByVal Page As System.Web.UI.Page) As String
        Dim strUserName As String
        Dim strFullUserName As String
        Dim strTokens() As String

        strFullUserName = Page.User.Identity.Name
        strTokens = strFullUserName.Split("\")
        strUserName = strTokens(1)
        Return strUserName
    End Function
    Public Function GetFormattedGridDataset(ByRef grd As Web.UI.WebControls.DataGrid, Optional ByVal strHeader As String = "") As DataSet
        'Returns a DataSet give a datagrid formatted according to the grds current setup
        'Only colnames and fields that the user sees appear in the resultant dataset
        'used in ExportToExcel
        Dim dsFormatedResult As New DataSet

        Dim dt As DataTable
        Dim dr As DataRow
        Dim i As Integer
        Dim rowIndex As Integer
        Dim strColHeader As String


        If Not grd Is Nothing Then

            'create a DataTable
            dt = New DataTable("table")

            'TODO:add a header if one is supplied
            If (strHeader > "") Then
                'dt.Columns.Add(New DataColumn(strHeader, GetType(String)))
                'dr = dt.NewRow()
                'dr.Item(0) = strHeader
                'dt.Rows.Add(dr)
            End If

            ' add  coloumns from the grd
            For i = 0 To grd.Columns.Count - 1
                If grd.Columns(i).Visible = True Then
                    strColHeader = grd.Columns(i).HeaderText
                    'stop the table from adding in a default col header (like 'Colomn1')
                    If Not strColHeader > "" Then
                        strColHeader = " "
                    End If
                    dt.Columns.Add(New DataColumn(strColHeader, GetType(String)))
                End If
            Next

            'add row for each row/item in the datagrid
            For rowIndex = 0 To grd.Items.Count - 1

                dr = dt.NewRow()

                'for each col in the datagrid add the cell to the row
                Dim colIndex As Integer
                colIndex = 0
                For i = 0 To grd.Columns.Count - 1    'grd.Items(i).Cells.Count - 1
                    'only add the data for the visible rows
                    If grd.Columns(i).Visible = True Then
                        If grd.Items(rowIndex).Cells(i).HasControls Then
                            Dim lnkBtn As System.Web.UI.WebControls.LinkButton
                            Dim lbl As System.Web.UI.WebControls.Label
                            Dim hlnk As System.Web.UI.WebControls.HyperLink

                            If grd.Items(rowIndex).Cells(i).Controls(0).GetType().FullName = "System.Web.UI.WebControls.LinkButton" Then
                                lnkBtn = grd.Items(rowIndex).Cells(i).Controls(0)
                                dr.Item(colIndex) = lnkBtn.Text
                            End If

                            If grd.Items(rowIndex).Cells(i).Controls(0).GetType().FullName = "System.Web.UI.WebControls.HyperLink" Then
                                hlnk = grd.Items(rowIndex).Cells(i).Controls(0)
                                dr.Item(colIndex) = hlnk.Text
                            End If

                            If grd.Items(rowIndex).Cells(i).Controls(0).GetType().FullName = "System.Web.UI.WebControls.Label" Then
                                lbl = grd.Items(rowIndex).Cells(i).Controls(0)
                                dr.Item(colIndex) = lbl.Text
                            End If
                        Else
                            dr.Item(colIndex) = grd.Items(rowIndex).Cells(i).Text
                        End If

                        colIndex += 1
                    End If

                Next

                dt.Rows.Add(dr)

            Next
            dsFormatedResult.Tables.Add(dt)
            'return a DataView to the DataTable
            Return dsFormatedResult
        Else
            'no imsRS
            Return Nothing
        End If

    End Function

    Public Function DataGridFormattedDataTable(ByRef grdDataGrid As Web.UI.WebControls.DataGrid, ByVal dtDataGrid As DataTable) As DataTable
        'Function accepts a datagrid and its datatable datasource 
        'and returns a datatable that represents all the 
        'visible items in the datagrid
        'Note this function assumes that the input datagrid's datasource
        'is the datatable passed in as parameter
        'This is done because it seems the datagrid looses its datasource
        'object after postback
        'Note only those fields visible in the datagrid are returned
        'and all datarows are returned, not only those currently visible
        'in the datagrid

        Dim dtReturn As DataTable
        Dim drAdd As DataRow
        Dim intColumnIndex As Integer
        Dim intRowIndex As Integer
        Dim strColumnHeader As String
        Dim strSourceField As String
        Dim strDestinationField As String
        Dim blnTemplateColumn As Boolean
        Dim ButtonColumn As ButtonColumn
        Dim BoundColumn As BoundColumn

        'Error check that datagrid is not nothing
        If Not grdDataGrid Is Nothing Then

            'Create a new return DataTable
            dtReturn = New DataTable

            'Add column names from the datagrid
            For intColumnIndex = 0 To grdDataGrid.Columns.Count - 1
                If grdDataGrid.Columns(intColumnIndex).Visible Then
                    If Not TypeOf (grdDataGrid.Columns(intColumnIndex)) Is TemplateColumn Then
                        strColumnHeader = grdDataGrid.Columns(intColumnIndex).HeaderText
                        'Prevent the table from adding in a default 
                        'col header (like 'Colomn1')
                        If Not strColumnHeader > "" Then
                            strColumnHeader = " "
                        End If
                        dtReturn.Columns.Add(New DataColumn(strColumnHeader, GetType(String)))
                    End If
                End If
            Next

            'Add rows for each item in the input datatable
            For intRowIndex = 0 To dtDataGrid.Rows.Count - 1

                drAdd = dtReturn.NewRow()

                'for each column in the datagrid, add the cell to the row
                intColumnIndex = 0
                For intColumnIndex = 0 To grdDataGrid.Columns.Count - 1

                    'Only add data for the visible rows
                    If grdDataGrid.Columns(intColumnIndex).Visible = True Then

                        strSourceField = ""
                        strDestinationField = ""

                        strDestinationField = grdDataGrid.Columns(intColumnIndex).HeaderText

                        If TypeOf (grdDataGrid.Columns(intColumnIndex)) Is ButtonColumn Then
                            ButtonColumn = grdDataGrid.Columns(intColumnIndex)
                            strSourceField = ButtonColumn.DataTextField
                        ElseIf TypeOf (grdDataGrid.Columns(intColumnIndex)) Is BoundColumn Then
                            BoundColumn = grdDataGrid.Columns(intColumnIndex)
                            strSourceField = BoundColumn.DataField
                        End If

                        If Not blnTemplateColumn Then
                            If strSourceField > "" AndAlso dtDataGrid.Columns.Contains(strSourceField) Then
                                drAdd.Item(strDestinationField) = dtDataGrid.Rows.Item(intRowIndex).Item(strSourceField)
                            End If
                        End If
                    End If
                Next

                dtReturn.Rows.Add(drAdd)

            Next

            'return the DataTable
            Return dtReturn

        Else 'datagrid is nothing
            Return Nothing
        End If

    End Function

    Public Sub DataTableToExcel(ByVal dtDataTable As DataTable, ByVal response As System.Web.HttpResponse, ByVal strFileName As String)
        'Writes the input datatable to excel, prompting the 
        'user to open or save the file
        'Writes to excel using the httpresponse passed as parameter

        Dim stringWrite As System.IO.StringWriter
        Dim htmlWrite As System.Web.UI.HtmlTextWriter
        Dim dgDataGrid As DataGrid

        response.Clear()
        response.Charset = ""
        'set the response mime type for excel
        response.ContentType = "application/vnd.ms-excel"
        response.AddHeader("Content-Disposition", "attachment ; filename=" & strFileName & ".xls")

        'create a string writer
        stringWrite = New System.IO.StringWriter

        'create an htmltextwriter which uses the stringwriter
        htmlWrite = New System.Web.UI.HtmlTextWriter(stringWrite)

        'instantiate a new dummy datagrid
        dgDataGrid = New DataGrid

        dgDataGrid.DataSource = dtDataTable
        dgDataGrid.DataBind()

        'tell the datagrid to render itself to our htmltextwriter
        dgDataGrid.RenderControl(htmlWrite)

        'Output the html using the httpresonse object passed in
        response.Write(stringWrite.ToString)
        response.End()

    End Sub

    Public Sub DataTableToWord(ByVal dtDataTable As DataTable, ByVal response As System.Web.HttpResponse, ByVal strFileName As String)
        'Writes the input datatable to excel, prompting the 
        'user to open or save the file
        'Writes to excel using the httpresponse passed as parameter

        Dim stringWrite As System.IO.StringWriter
        Dim htmlWrite As System.Web.UI.HtmlTextWriter
        Dim dgDataGrid As DataGrid

        response.Clear()
        response.Charset = ""
        'set the response mime type for excel
        response.ContentType = "application/msword"
        response.AddHeader("Content-Disposition", "attachment ; filename=" & strFileName & ".doc")

        'create a string writer
        stringWrite = New System.IO.StringWriter

        'create an htmltextwriter which uses the stringwriter
        htmlWrite = New System.Web.UI.HtmlTextWriter(stringWrite)

        'instantiate a new dummy datagrid
        dgDataGrid = New DataGrid

        dgDataGrid.DataSource = dtDataTable
        dgDataGrid.DataBind()

        'tell the datagrid to render itself to our htmltextwriter
        dgDataGrid.RenderControl(htmlWrite)

        'Output the html using the httpresonse object passed in
        response.Write(stringWrite.ToString)
        response.End()

    End Sub

    Public Sub RedirectFrame(ByVal page As Web.UI.Page, ByVal frame As String, ByVal strPath As String)
        'if the page is in a frame, it will set the location/page of another frame
        Dim strScript As String

        strScript = "<script language='javascript'>top." & frame & ".document.location.href='" & strPath & "'</script>"

        page.RegisterStartupScript("redirectframe", strScript)

    End Sub

    Public Function RemoveHTMLCharacters(ByVal strFieldValue As String) As String

        'function cleans the string to allow it to be inserted into html
        strFieldValue = strFieldValue.Replace(",", " ")
        strFieldValue = strFieldValue.Replace(";", " ")
        strFieldValue = strFieldValue.Replace("'", "")
        strFieldValue = strFieldValue.Replace("""", " ")
        strFieldValue = strFieldValue.Replace("(", "")
        strFieldValue = strFieldValue.Replace(")", "")
        strFieldValue = strFieldValue.Replace("&", "and")
        strFieldValue = strFieldValue.Replace("\", " ")
        strFieldValue = strFieldValue.Replace("|", "")
        strFieldValue = strFieldValue.Replace("/", " ")
        strFieldValue = strFieldValue.Replace(">", "")
        strFieldValue = strFieldValue.Replace("<", "")
        strFieldValue = strFieldValue.Replace("?", "")
        strFieldValue = strFieldValue.Replace("@", "")
        strFieldValue = strFieldValue.Replace("%", "")
        strFieldValue = strFieldValue.Replace("#", "")
        strFieldValue = strFieldValue.Replace("$", "")
        Return strFieldValue

    End Function

    Public Function SessionTimeOut(ByVal Page As System.Web.UI.Page) As Boolean

        If Not Page.Session Is Nothing Then
            If Page.Session.IsNewSession() Then
                Dim strCookieHeader As String

                strCookieHeader = Page.Request.Headers("Cookie")

                If Not strCookieHeader Is Nothing AndAlso strCookieHeader.IndexOf("ASP.NET_SessionId") >= 0 Then
                    SessionTimeOut = True
                End If
            End If
        Else
            SessionTimeOut = False
        End If
    End Function

    Public Function MeetVersionRequirements(ByVal Page As System.Web.UI.Page, ByVal strBrowser As String, ByVal strBrowserMajor As String, ByVal strBrowserMinor As String) As Boolean
        Dim blnMeet As Boolean

        blnMeet = False

        If Page.Request.Browser.Browser = strBrowser Then
            If Page.Request.Browser.MajorVersion = strBrowserMajor Then
                If Page.Request.Browser.MinorVersion = strBrowserMinor Then
                    blnMeet = True
                End If
            End If
        End If

        MeetVersionRequirements = blnMeet

        Return MeetVersionRequirements


    End Function

End Module
