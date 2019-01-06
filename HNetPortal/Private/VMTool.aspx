<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="VMTool.aspx.cs" Inherits="HNetPortal.Private.VMTool" %>
<%@ Register Src="~/Controls/VMWareUI.ascx" TagName="VMWareUI" TagPrefix="ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
		<link rel="stylesheet" href="/Content/font-awesome.css" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <div class="jumbotron text-center ">
        <h1>The HNet Portal</h1>
        <p>
            <i class="fa fa-server"></i>&nbsp;VM Tool
			
        </p>
    </div>
    <p>
        This is a basic tool for starting and stopping virtual machines (VM's) on the HNET network.  
    </p>

    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <div class="col-xs-6 col-sm-4 col-md-3 col-lg-2">
                <button type="button" class="btn btn-block"
                    style="padding-left: 0px; padding-right: 0px; margin-bottom: 3px; color: white; background: <%# (WSHLib.Network.VMState)Eval("state")==WSHLib.Network.VMState.PoweredOn ? "green": "red"%>"
                    id='vmwareTool_<%#Eval("vmName")%>'
                    data-vmname='<%#Eval("vmName")%>'
                    onclick="launchVMDlg('<%#Eval("vmName")%>');">
                    <%#Eval("vmName")%>
                </button>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
    <ucl:VMWareUI runat="server" ID="VMWareUI" />
</asp:Content>
