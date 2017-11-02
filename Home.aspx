<%@ Register TagPrefix="uc1" TagName="LinkList" Src="LinkList.ascx" %>
<%@ Page Language="vb" AutoEventWireup="false" Codebehind="Home.aspx.vb" Inherits="HomePage2.WebForm1" %>

<%@ Register Src="RSSDisplay.ascx" TagName="RSSDisplay" TagPrefix="uc2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link type="text/css" rel=stylesheet href=Styles.css  />
	</HEAD>
	<body >
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManagerHome" runat="server">
           <Services>
                <asp:ServiceReference Path="LinkSearch.asmx" />
           </Services> 
            </asp:ScriptManager>
	<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="0">
        <tr>
            <td align="center" colspan="2" height="10" style="height: 10px" valign="top">
                <asp:label id="lblHome" runat="server" Font-Bold="True" Font-Size="X-Large" Font-Names="Arial">There's no place like 127.0.0.1</asp:label></td>
        </tr>
        <tr>
            <td height="10" style="height: 10px" valign="top">
                <P align="center">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                               <center> 
                                    <asp:label id="Label2" runat="server" Font-Bold="True" Font-Names="Arial">Cape Town:</asp:label><asp:label id="lblCapeTownTime" runat="server" Font-Bold="True" Font-Names="Arial"></asp:label>
                               </center> 
                                </ContentTemplate>
                            </asp:UpdatePanel>
                </p>
                            <center>
                            <br />
                            <table>
                                <tr>
                                    <td style="width: 100px; height: 33px;" nowrap>
                                    
                                    
							            <asp:textbox id="txtInput" runat="server" Width=500 Font-Names="Verdana" CssClass="InputBox"></asp:textbox></td>
							            <td style="height: 33px"><asp:ImageButton ID="btnGo" runat="server" ImageUrl="~/images/Go.bmp" ToolTip="Navigate to URL" /></td>
                                            <td style="height: 33px"><asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/search.jpg" ToolTip="Search with Google" /></td>
                                </tr>
                            </table>                                    
							<asp:linkbutton id="lnkSearch" runat="server" Font-Size="Smaller" Font-Names="Arial" Font-Bold="True">Search</asp:linkbutton>&nbsp;
							<asp:linkbutton id="lnkDefine" runat="server" Font-Size="Smaller" Font-Names="Arial" Font-Bold="True">Define</asp:linkbutton>&nbsp;
							<asp:linkbutton id="lnkGroups" runat="server" Font-Size="Smaller" Font-Names="Arial" DESIGNTIMEDRAGDROP="205" Font-Bold="True">Groups</asp:linkbutton>&nbsp;
							<asp:linkbutton id="lnkDictionary" runat="server" Font-Size="Smaller" Font-Names="Arial" Font-Bold="True">Dictionary</asp:linkbutton>&nbsp;
							<asp:linkbutton id="btnImages" runat="server" Font-Size="Smaller" Font-Names="Arial" Font-Bold="True">Images</asp:linkbutton>&nbsp;
                            <asp:LinkButton ID="btnWikipedia" runat="server" Font-Names="Arial" Font-Size="Smaller" Font-Bold="True">Wikipedia</asp:LinkButton>
                                <asp:LinkButton ID="btnFogBugz" runat="server" Font-Bold="True" Font-Names="Arial"
                                    Font-Size="Smaller">FogBugz</asp:LinkButton></center>
                        <center>
                            &nbsp;</center>
                        <center>
                        <asp:UpdatePanel ID="udpPinnedLinks" runat="server">
                            <ContentTemplate>
                        <cc1:Accordion ID="Accordion1" runat="server"     SelectedIndex="0"
                            HeaderCssClass="accordionHeader"
                            HeaderSelectedCssClass="accordionHeaderSelected"
                            ContentCssClass="accordionContent"
                            AutoSize="None"
                            FadeTransitions="true"
                            TransitionDuration="100"
                            FramesPerSecond="20"
                            RequireOpenedPane="false"
                            SuppressHeaderPostbacks="true"
                            width=500px>
                        <Panes>
                        <cc1:AccordionPane ID="AccordionPanePinned" runat="server">
                        <Header>
                                    Pinned Links
                        </Header>
                        <Content>
                            <uc1:linklist id="LinkListPinned" runat="server" ></uc1:linklist>
                        </Content>
                        </cc1:AccordionPane>
                        <cc1:AccordionPane ID="AccordionPaneUnpinned" runat="server">
                        <Header>Unpinned Links</Header>
                        <Content>
                                <uc1:linklist id="LinkListUnpinned" runat="server"></uc1:linklist>
                                <asp:LinkButton ID="btnAllUnpinned" runat="server">All Unpinned</asp:LinkButton>
                        </Content>
                        </cc1:AccordionPane>
                        </Panes>
                        </cc1:Accordion>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                            &nbsp;</center><P align="center">
                                &nbsp;</p>
            </td>
            <td align="center" rowspan="1" valign="top">
                        &nbsp;<asp:Label ID="Label1" runat="server" Font-Bold="True" Text="News:"></asp:Label>
                        <br />
                        <asp:UpdateProgress ID="udprogRSS" runat="server" AssociatedUpdatePanelID="udpNews" DisplayAfter="100">
                            <ProgressTemplate>
                                &nbsp;<img  src="images/loading.GIF" /><br />
                                <asp:Label ID="Label3" runat="server" Font-Bold="True" Text="Loading News..."></asp:Label>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        &nbsp;
                        <asp:UpdatePanel ID="udpNews" runat="server">
                            <ContentTemplate>
                                <uc2:RSSDisplay ID="RSSDisplay1" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
            </td>
        </tr>
				<TR>
					<TD height="10" style="height: 10px" valign="top">
					</TD>
					<TD vAlign="top" align="center" rowSpan="3">
                    </TD>
				</TR>
				<TR>
					<TD colSpan="1" height="5" valign="top">
                                        </TD>
				</TR>
				<TR>
					<TD>
                                        &nbsp;
                        </TD>
				</TR>
				<TR>
					<TD valign="top">
					<center>
                        &nbsp;</center>
                    </TD>
				</TR>
                <tr>
                    <td valign="top">
                                        <asp:button id="btnEnter" runat="server"></asp:button></td>
                </tr>
			</TABLE>
			<P></P>
			<P>
                <cc1:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" TargetControlID="txtInput" ServicePath="LinkSearch.asmx" ServiceMethod="Search" CompletionSetCount=30 EnableCaching=true MinimumPrefixLength=3 CompletionListCssClass="AutoCompleteList"  CompletionListItemCssClass="AutoCompleteListItem" CompletionListHighlightedItemCssClass="AutoCompleteListHighlightedItem">
               <Animations>
              <OnShow>
              <Sequence>
              <OpacityAction Opacity="0" />
              <HideAction Visible="true" />
              <FadeIn duration=".1" />
              </Sequence></OnShow>
              <OnHide>
              <Sequence>
              <OpacityAction Opacity="1" />
              <HideAction Visible="true" />
              <FadeOut duration=".1" />
              </Sequence>
              </OnHide>
               </Animations> 
                </cc1:AutoCompleteExtender>
                <cc1:AnimationExtender ID="AnimationExtender1" runat="server" TargetControlID="lblHome">
               <Animations>
              <OnHoverOver><Color startValue="#246ACF" endValue="#016ACF" property="forecolor"/></OnHoverOver>

               </Animations> 
                </cc1:AnimationExtender>
            </P>
                                       
 
		</form>
	</body>
</HTML>
