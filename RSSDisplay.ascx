<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RSSDisplay.ascx.vb" Inherits="HomePage2.RSSDisplay" %>
<%@ Register Src="tblRSSFeed/tblRSSFeed_Editor.ascx" TagName="tblRSSFeed_Editor"
    TagPrefix="uc1" %>
&nbsp;<asp:LinkButton ID="btnRefreshNews" runat="server" Font-Size="Smaller">refresh</asp:LinkButton>
<asp:LinkButton ID="btnSource" runat="server" Font-Size="Smaller">select source</asp:LinkButton><br />
<asp:DataList ID="lstRSS" runat="server" Width="500px">
<ItemTemplate>
<table style="width: 100%">
    <tr>
        <td>
        <a href="<%#XPath("link")%>"><%#XPath("title")%></a><br /></td>
        <td align="right" style="width: 100px;">
            <a href="<%#XPath("link")%>" target=_blank> <font style="font-size: 10pt;">(new window)</font></a></td>
    </tr>
</table>
         
         <font style="font-size: 10pt; color: gray;">(<%#XPath("pubDate")%>)</font><br /> 
         <%#XPath("description")%><hr />

</ItemTemplate>
</asp:DataList>

         <asp:XmlDataSource ID="xmlSourceRSS" runat="server"
    XPath="/rss/channel/item" EnableCaching="False" TransformFile="~/NewsDisplay.xslt"></asp:XmlDataSource>
<asp:Panel ID="pnlRSSFeedEditor" runat="server" Height="50px" Width="125px">
    <uc1:tblRSSFeed_Editor ID="TblRSSFeed_Editor1" runat="server" />
</asp:Panel>
