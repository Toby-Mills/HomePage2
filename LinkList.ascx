<%@ Control Language="vb" AutoEventWireup="false" Codebehind="LinkList.ascx.vb" Inherits="HomePage2.LinkList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script runat="server">
  Private results As Integer
  
  <Personalizable()> _
  Property ResultsPerPage() As Integer
    
    Get
      Return results
    End Get
    
    Set(ByVal value As Integer)
      results = value
    End Set
    
  End Property
</script>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<asp:datagrid id="grdLink" runat="server" Width="500px" ShowHeader="False" AutoGenerateColumns="False"
	GridLines="None">
	<AlternatingItemStyle Font-Names="Arial" BackColor="#FFCC99"></AlternatingItemStyle>
	<ItemStyle Font-Names="Arial" BackColor="PeachPuff"></ItemStyle>
	<Columns>
		<asp:BoundColumn Visible="False" DataField="idLink" ReadOnly="True" HeaderText="idLink"></asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Delete">
			<ItemTemplate>
			<cc1:HoverMenuExtender ID="HoverMenuExtender1" runat="server" TargetControlID="btnDelete" PopupControlID="pnlMenu" PopupPosition=Right PopDelay="10"></cc1:HoverMenuExtender>
				<asp:ImageButton id="btnDelete" runat="server" ImageUrl="images/disagree.gif"
					AlternateText="delete link"></asp:ImageButton>&nbsp;
					    <asp:Panel ID="pnlMenu" runat="server" BackColor="#FFE0C0" Width="300px" BorderStyle="Solid" BorderWidth="1pt" CssClass="LinkInfoPanel">
                            <asp:Label ID="Label2" runat="server" Text="Used:" Font-Bold="True"></asp:Label>
                            <asp:Label ID="lblUseCount" runat="server" Font-Bold="False" Text=<%# DataBinder.Eval(Container.DataItem,"intCountUsed") %>></asp:Label>
                            <asp:Label ID="Label3" runat="server"  Font-Bold="False" Text="times"></asp:Label><br />
                            <asp:Label ID="Label1" runat="server" Text="Last Used:" Font-Bold="True"></asp:Label>
                            <asp:Label ID="lblLastUsed" runat="server"  Font-Bold="False" Text=<%# DataBinder.Eval(Container.DataItem,"dteLastUsed") %>></asp:Label><br />
                            <asp:Label ID="Label7" runat="server" Font-Bold="True" Text="URL:"></asp:Label>
                            <asp:Label ID="lblURL" runat="server"  Font-Bold="False" Text='<%# DataBinder.Eval(Container.DataItem,"strURL") %>'></asp:Label></asp:Panel>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Pin">
			<ItemTemplate>
				<asp:ImageButton id="btnPin" runat="server" ImageUrl="images/Unpinned.bmp" CommandName="pin" AlternateText="pin link"></asp:ImageButton>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Unpin">
			<ItemTemplate>
				<asp:ImageButton id="btnUnpin" runat="server" ImageUrl="images/Pinned.bmp" CommandName="unpin" AlternateText="unpin link"></asp:ImageButton>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Edit">
			<ItemStyle Font-Size="8pt"></ItemStyle>
			<ItemTemplate>
				<asp:ImageButton id="btnEdit" runat="server" ImageUrl="images/Edit.bmp" CommandName="edit" AlternateText="edit link details"></asp:ImageButton>
			</ItemTemplate>
			<EditItemTemplate>
				<asp:LinkButton id="btnSave" runat="server" CommandName="save">save</asp:LinkButton>
				<asp:LinkButton id="btnCancel" runat="server" CommandName="cancel">cancel</asp:LinkButton>
			</EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Display">
			<ItemStyle Font-Size="10pt"></ItemStyle>
			<ItemTemplate>
				<asp:Image id="imgIcon" Width="16px" runat="server" Height="16px" Visible="False"></asp:Image>
				<asp:LinkButton id=btnDisplay runat="server" CommandName="link" Text='<%# DataBinder.Eval(Container.DataItem,"strDisplay") %>'>
				</asp:LinkButton>
			</ItemTemplate>
			<EditItemTemplate>
                <asp:Label ID="Label5" runat="server" Text="Display:" Font-Bold="True"></asp:Label>
				<asp:TextBox id=txtDisplay runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"strDisplay") %>'>
				</asp:TextBox><br />
                <asp:Label ID="Label4" runat="server" Text="URL:" Font-Bold="True"></asp:Label><br />
                <asp:TextBox ID="txtURL" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"strURL") %>'
                    Width="400px"></asp:TextBox><br />
                <asp:Label ID="Label6" runat="server" Text="Shortcut:" Font-Bold="True"></asp:Label>
                <asp:TextBox ID="txtShortcut" runat="server" Font-Bold="True" Text='<%# DataBinder.Eval(Container.DataItem,"strShortcut") %>'></asp:TextBox>
			</EditItemTemplate>
		</asp:TemplateColumn>
        <asp:BoundColumn DataField="strShortcut" ReadOnly="True">
            <ItemStyle Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Size="12pt"
                Font-Strikeout="False" Font-Underline="False" />
        </asp:BoundColumn>
	</Columns>
</asp:datagrid>
<cc1:Accordion ID="Accordion1" runat="server">
</cc1:Accordion>
&nbsp;


