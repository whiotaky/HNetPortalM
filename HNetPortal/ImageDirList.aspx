<%@ Page Title="HNet Images - Index" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ImageDirList.aspx.cs" Inherits="HNetPortal.ImageDirList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <div class="row">
        <div class="col-xs-12 col-xs-offset-0 col-md-11 col-md-offset-1">

            <asp:LoginView ID="LoginView1" runat="server" ViewStateMode="Disabled">

                <AnonymousTemplate>
                    <h1><span class="glyphicon glyphicon-picture"></span>&nbsp;HNet Imagebase - Guest</h1>
                    <asp:ListView ID="ListView1" runat="server">

                        <EmptyDataTemplate>
                            <table runat="server" style="background-color: #FFFFFF; border-collapse: collapse; border-color: #999999; border-style: none; border-width: 1px;">
                                <tr>
                                    <td>There are no publicly shared images at this time</td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>

                        <ItemTemplate>
                            <a href="/ImageDirDisplay.aspx?dir=<%# Container.DataItem %>"><%# Container.DataItem %></a>
                            <br />
                        </ItemTemplate>
                    </asp:ListView>
                </AnonymousTemplate>

                <LoggedInTemplate>
                    <h1><span class="glyphicon glyphicon-picture"></span>&nbsp;HNet Imagebase</h1>
                    <div class="container">
                        <asp:TreeView ID="TreeView1" runat="server"></asp:TreeView>
                    </div>
                </LoggedInTemplate>

            </asp:LoginView>

        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

    <asp:LoginView ID="LoginView2" runat="server" ViewStateMode="Disabled">
        <AnonymousTemplate>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <script>
                var createImageDirUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/createImageDir") %>";
                var deleteImageDirUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/deleteImageDir") %>";

                $(document).ready(function () {

                    $.fn.serializeObject = function () {
                        var o = {};
                        var a = this.serializeArray();
                        $.each(a, function () {
                            if (o[this.name] !== undefined) {
                                if (!o[this.name].push) {
                                    o[this.name] = [o[this.name]];
                                }
                                o[this.name].push(this.value || '');
                            } else {
                                o[this.name] = this.value || '';
                            }
                        });
                        return o;
                    };

                    $("#newDirBtn").click(function () {
                        doNewDir();
                        return false;
                    });

                });


                function deleteDirClicked(which) {

                    var whichDir = which.getAttribute('data-dirName');

                    bootbox.dialog({
                        message: "Are you sure you want to delete the directory " + whichDir + "?",
                        title: "Delete Directory " + whichDir + "?",
                        buttons: {
                            cancel: {
                                label: "Cancel",
                                className: "btn-primary",
                                callback: function () {
                                }
                            },
                            main: {
                                label: "Confirm",
                                className: "btn-primary btn-danger",
                                callback: function () {

                                    $("#createProgress").css("visibility", "visible");
                                    $("#errorMsg").css("visibility", "hidden");
                                    $("#theMsg").html("");

                                    $.ajax({
                                        type: "POST",
                                        data: '{"dirName":"' + whichDir + '"}',
                                        url: deleteImageDirUrl,
                                        contentType: "application/json",
                                        dataType: "json",
                                        dataFilter: function (data) {
                                            var msg = eval('(' + data + ')');
                                            if (msg.hasOwnProperty('d'))
                                                return msg.d;
                                            else
                                                return msg;
                                        },

                                        success: function (data) {
                                            //HANDLE error msg
                                            $("#deleteProgress").css("visibility", "hidden");
                                            var status = data[0].status;
                                            if (status.indexOf("Error") >= 0) {
                                                var status = data[0].status;
                                                $("#errorMsg").css("visibility", "visible");
                                                $("#theMsg").html('Error Deleting: ' + status);
                                            } else { //successfully created
                                                $("#errorMsg").css("visibility", "hidden");
                                                bootbox.hideAll();
                                                location.reload(true); //cheesy
                                            }
                                        },

                                        error: function (data, textStatus, errorThrown) {
                                            $("#deleteProgress").css("visibility", "hidden");
                                            $("#errorMsg").css("visibility", "visible");
                                            $("#theMsg").html('AJax Error: ' + errorThrown);
                                        }

                                    });

                                    return false;

                                }
                            }
                        }
                    });

                }


                function doNewDir() {

                    bootbox.dialog({
                        message: "Directory Name:<input type='text' name='newDirName' id='newDirName' ><br/>" +
                            ' <div id="createProgress" class="progress progress-striped active" style="visibility:hidden">' +
                                '<div class="progress-bar" style="width: 100%;">Sending create directory request...</div>' +
                            '</div>' +
                            '<div id="errorMsg" class="alert alert-danger fade in" style="visibility:hidden">' +
                            '<strong>Error!</strong> <span id="theMsg"></span>' +
                            '</div>'

                            ,
                        title: "Enter new Directory Name ",
                        buttons: {
                            cancel: {
                                label: "Cancel",
                                className: "btn-primary",
                                callback: function () {
                                }
                            },
                            main: {
                                label: "Create",
                                className: "btn-primary",
                                callback: function () {

                                    $("#createProgress").css("visibility", "visible");
                                    $("#errorMsg").css("visibility", "hidden");
                                    $("#theMsg").html("");

                                    var newDirName = $("#newDirName").val();

                                    $.ajax({
                                        type: "POST",
                                        data: '{"newDirName":"' + newDirName + '"}',
                                        url: createImageDirUrl,
                                        contentType: "application/json",
                                        dataType: "json",
                                        dataFilter: function (data) {
                                            var msg = eval('(' + data + ')');
                                            if (msg.hasOwnProperty('d'))
                                                return msg.d;
                                            else
                                                return msg;
                                        },

                                        success: function (data) {
                                            //HANDLE error msg
                                            $("#createProgress").css("visibility", "hidden");
                                            var status = data[0].status;
                                            if (status.indexOf("Error") >= 0) {
                                                var status = data[0].status;
                                                $("#errorMsg").css("visibility", "visible");
                                                $("#theMsg").html('Error Creating: ' + status);
                                            } else { //successfully created
                                                $("#errorMsg").css("visibility", "hidden");
                                                bootbox.hideAll();
                                                location.reload(true); //cheesy
                                            }
                                        },

                                        error: function (data, textStatus, errorThrown) {
                                            $("#createProgress").css("visibility", "hidden");
                                            $("#errorMsg").css("visibility", "visible");
                                            $("#theMsg").html('AJax Error: ' + errorThrown);
                                        }

                                    });

                                    return false;

                                }
                            }
                        }
                    });

                }

            </script>

        </LoggedInTemplate>
    </asp:LoginView>


</asp:Content>
