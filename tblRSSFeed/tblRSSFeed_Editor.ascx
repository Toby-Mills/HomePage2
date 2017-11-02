<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="tblRSSFeed_Editor.ascx.vb" Inherits="HomePage2.tblRSSFeed_Editor" %>
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
    <tr>
        <td align="left">
            <asp:LinkButton ID="btnOK" runat="server">ok</asp:LinkButton>
            &nbsp; &nbsp;<asp:LinkButton ID="btnCancel" runat="server">cancel</asp:LinkButton>
            &nbsp;
            <asp:LinkButton ID="btnSave" runat="server">save as default</asp:LinkButton></td>
    </tr>
    <tr>
        <td>
<asp:GridView ID="grdRSSFeed" runat="server" AutoGenerateColumns="False" ShowFooter="True">
    <Columns>
        <asp:BoundField DataField="idRSSFeed" />
        <asp:TemplateField HeaderText="Active">
            <ItemTemplate>
                <asp:CheckBox ID="chkActive" runat="server" Checked=<%# DataBinder.Eval(Container.DataItem,"blnActive") %> />
            </ItemTemplate>
            <FooterTemplate>
                <asp:CheckBox ID="chkActiveFooter" runat="server" BackColor="LightYellow" BorderColor="#E0E0E0" />
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Description">
            <ItemTemplate>
                <asp:TextBox ID="txtDescription" runat="server" Width="200px" Text='<%# DataBinder.Eval(Container.DataItem,"strDescription") %> '></asp:TextBox>
            </ItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtDescriptionFooter" runat="server" BackColor="LightYellow" Width="200px"></asp:TextBox>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="URL">
            <ItemTemplate>
                <asp:TextBox ID="txtURL" runat="server" Width="400px" Text='<%# DataBinder.Eval(Container.DataItem,"strRSSURL") %>'></asp:TextBox>
            </ItemTemplate>
            <FooterTemplate>
                <asp:TextBox ID="txtURLFooter" runat="server" BackColor="LightYellow" Width="400px"></asp:TextBox>
            </FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Del">
            <FooterTemplate>
                <asp:LinkButton ID="btnAddFooter" runat="server" CommandName="Add">add</asp:LinkButton>
            </FooterTemplate>
            <ItemTemplate>
                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete">del</asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
        </td>
    </tr>
</table>
