<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Home_WebParts.aspx.vb" Inherits="HomePage2.Home_WebParts" %>
<%@ Register Src="LinkList.ascx" TagName="LinkList" TagPrefix="uc1" %>
<%@ Register Src="RSSDisplay.ascx" TagName="RSSDisplay" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:WebPartManager ID="WebPartManager1" runat="server">
            <Personalization Enabled="False" />
        </asp:WebPartManager>
        <br />
        &nbsp;
        <table>
            <tr>
                <td style="width: 100px">
                    <asp:WebPartZone ID="SidebarZone" runat="server">
                        <ZoneTemplate>
                        <asp:Label ID="Label2" runat="server"  title="SideBar">Welcome to my Home Page</asp:Label>
                            <uc2:RSSDisplay ID="RSSDisplay1" runat="server" />
                        </ZoneTemplate>
                    </asp:WebPartZone>
                </td>
                <td style="width: 100px">
                    <asp:WebPartZone ID="MainZone" runat="server">
                        <ZoneTemplate>
                            <asp:Label ID="Label1" runat="server"  title="Content">Welcome to my Home Page</asp:Label>
                        </ZoneTemplate>
                    </asp:WebPartZone>
                </td>
                <td style="width: 100px">
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
