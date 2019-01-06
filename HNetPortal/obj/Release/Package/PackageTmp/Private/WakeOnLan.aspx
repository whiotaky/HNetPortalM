<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="WakeOnLan.aspx.cs" Inherits="HNetPortal.Private.WakeOnLan" ClientIDMode="Static" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">


    <div class="jumbotron text-center">
        <h1>The HNet Portal</h1>
        <p>
            <span class="glyphicon glyphicon-lamp"></span>&nbsp;Wake On Lan
        </p>
    </div>
    <p>
        Most modern computers support an option called "Wake On Lan" (WOL).  When this feature is 
        enabled in its BIOS configuration, a computer that is in "sleep" mode can be awakened remotely.  
        This can be accomplished by sending the sleeping PC a "Magic Packet".  Note that a powered-off or 
        hibernating PC is not a sleeping PC, and hence cannot be awakened.  Neither can a sleeping PC that 
        is NOT WOL-enabled be awakend by a magic packet.
    </p>
    <p>
        The HNet computers below are currently configured for WOL.  If they are sleeping, you may wake 
        them up by clicking their buttons.
    </p>


    <div class="row">
        <div id="successMsg" runat="server" class="col-xs-12 alert alert-success" style="display: none">
            Success
        </div>
    </div>
    <div class="row">
        <div id="errorMsg" runat="server" class="col-xs-12 alert alert-danger" style="display: none">
            Error
        </div>
    </div>

    <asp:XmlDataSource ID="wakeDS" runat="server" XPath="HNetPortal/wakeOnLan" DataFile="~/App_Data/WakeOnLanData.xml" />
    <asp:Repeater ID="Repeater1" runat="server" DataSourceID="wakeDS">
        <ItemTemplate>
            <div class="col-xs-12 col-sm-4 col-md-3">
                <button id="<%# XPath("aName")%>" style="padding-left: 0px; padding-right: 0px; margin-bottom: 3px;" data-macaddress='<%# XPath("macAddress")%>' type="submit" class="btn btn-primary btn-block" onclick="setWhich(this);" style="margin: 5px;" />
                <span class='glyphicon glyphicon-lamp'></span>&nbsp;Wake <%# XPath("description")%>
                    </button>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <input id="whichToWake" name="whichToWake" runat="server" type="hidden" value="" />
    <input id="whichAName" name="whichAName" runat="server" type="hidden" value="" />



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
    <script>
        function setWhich(which) {
            $('#whichToWake').val($("#" + which.id).data('macaddress'));
            $('#whichAName').val(which.id);
        }
    </script>
</asp:Content>
