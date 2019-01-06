<%@ Page Title="User RSS Edit" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="UserRSSEdit.aspx.cs" Inherits="HNetPortal.Private.UserRSSEdit" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <h1><span class="glyphicon glyphicon-list-alt"></span>&nbsp; The HNet Portal - RSS Feeds For <%=User.Identity.Name %></h1>

    <div class="container" style="margin-bottom: 20px;">

        <div class="Row">           
            <div class="col-sm-6 col-md-6 col-lg-6">

                <asp:Repeater ID="Repeater2" runat="server" OnItemDataBound="Repeater2_ItemDataBound">
                    <HeaderTemplate>
                        <div class="panel panel-primary">

                            <div class="panel-heading ">
                                <h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;Career Section</h3>
                            </div>

                            <div class="panel-body">
                                <div class='table-responsive'>
                                    <table id='table2' class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th>Enabled</th>
                                                <th>Name</th>
                                                <th>Order</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="c_en" type="checkbox" class="form-check-input" runat="server" onclick="cb_check(this);" ClientIDMode="Predictable" />
                                <input type="hidden" id="c_feedid" runat="server" value="0" />
                            </td>
                            <td>
                                <asp:Label ID="c_fn" class="form-label" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="c_ob" CssClass="form-control" runat="server" MaxLength="2" Style='max-width: 50px' ClientIDMode="Predictable"></asp:TextBox>
                            </td>
                        </tr>
                    </ItemTemplate>

                    <FooterTemplate>
                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </FooterTemplate>

                </asp:Repeater>

            </div>
             <div class="col-sm-6 col-md-6 col-lg-6">
                <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <div class="panel panel-primary">

                            <div class="panel-heading ">
                                <h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;News Section</h3>
                            </div>

                            <div class="panel-body">
                                <div class='table-responsive'>
                                    <table id='table1' class="table table-hover">
                                        <thead>
                                            <tr>
                                                <th>Enabled</th>
                                                <th>Name</th>
                                                <th>Order</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="n_en" type="checkbox" class="form-check-input" runat="server" ClientIDMode="Predictable" onclick="cb_check(this);" />
                                <input type="hidden" id="n_feedid" runat="server" value="0" />
                            </td>
                            <td>
                                <asp:Label ID="n_fn" class="form-label" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="n_ob" CssClass="form-control" runat="server" MaxLength="2" Style='max-width: 50px' ClientIDMode="Predictable"></asp:TextBox>
                            </td>
                        </tr>
                    </ItemTemplate>

                    <FooterTemplate>
                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </FooterTemplate>

                </asp:Repeater>
            </div>
        </div>
    </div>

    <div class="container text-center" style="margin-bottom: 20px;">
        <asp:Button ID="Button1" CssClass="btn btn-primary btn-lg" runat="server" type="submit" Text="Save" OnClick="Button1_Click" />
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

    <script id="postSave">

    </script>

    <script>
        function cb_check(which) {
            var t = which.id;
            var pattern = /n_en/i;
            var pattern2 = /c_en/i;
            t = t.replace(pattern, "n_ob");
            t = t.replace(pattern2, "c_ob");
         
            $("#" + t).prop("disabled", !which.checked);
        }
    </script>
</asp:Content>
