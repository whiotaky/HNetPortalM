<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="RestServiceTester.aspx.cs" Inherits="HNetPortal.Private.RestServiceTester" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .trash {
            color: rgb(209, 91, 71);
        }

        .edit {
            color: rgb(6, 26, 248);
        }

        .new {
            color: rgb(8, 248, 6);
        }
    </style>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <div class="jumbotron text-center">
        <h1>The HNet Portal</h1>
        <p>
            RESTful Service tester
        </p>
    </div>

    <div class="container">
        <div class="row">
            <div class="col-xs-12">
                <button class="btn btn-primary" id="testGet" name="testGet">GET</button>
                <button class="btn btn-primary" id="testAdd" name="testAdd">ADD</button>
            </div>
        </div>
    </div>

    <div class="container">
        <div class="row">
            <div id="resultMsg" class="col-xs-12 alert alert-warning" style="visibility: hidden">
            </div>
        </div>
    </div>

    <div id='tableDiv' class="container" style="margin-top: 20px; visibility: hidden">
        <div class="row">
            <div class="col-xs-12">
                <div class="panel panel-primary">
                    <div class="panel-heading ">
                        <h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;Sample Data</h3>
                    </div>
                    <div class="panel-body">
                        <div class='table-responsive'>
                            <table id='secTable' class="table table-hover">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Last Name</th>
                                        <th>First Name</th>
                                        <th>Pay Rate</th>
                                        <th>Start Date</th>
                                        <th>End Date</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
    <script>

        var enabledGlyph = "<span class='new glyphicon glyphicon-ok'></span>";
        var disabledGlyph = "<span class='trash glyphicon glyphicon-remove'></span>";
        var spinGlyph = "<span class='glyphicon glyphicon-cog glyphicon-spin' style='margin-right:8px;'></span>";
        var alertGlyph = "<span class='glyphicon glyphicon-alert' style='margin-right:8px;'></span>";

        //These you wont have to change (but in the code-behind you might need to)
        var token = '<%=token%>';
        var apiKey = '<%=APIKey%>';


        //Yeah...choose one appropriately
        // var serverBase = "http://services.hnet.local";
        //var serverBase = "http://servicesdev.hnet.local";
        //var serverBase = "http://localhost:50462";
        var serverBase = '<%=serviceBaseUrl%>';


        /*
            Your test REST service is configurable for 2 types of authentication, APIKey and username/password. Use one or the other
            (though both could be used simultaneously) .  Configure below per however the service is currently configured.  
        */
        reqSecKeyName = "APIKey";
        reqSecKeyValue = apiKey;
        //reqSecKeyName = "Authorization";        
        //reqSecKeyvalue = "Basic "+token;

        $(document).ready(function () {

            $("#testSaveBtn").click(function () { doPersonSave(); });

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

        });


        $("#testGet").click(function () {

            $('#secTable tbody').empty();
            $('#tableDiv').css("visibility", "visible");

            $.ajax({
                type: "GET",
                url: serverBase + "/api/person",

                xhrFields: {
                    withCredentials: true
                },

                beforeSend: function (xhr) {
                    xhr.setRequestHeader(reqSecKeyName, reqSecKeyValue);
                },

                dataType: 'json',
                async: false,

                success: function (data) {

                    $.each(data, function (i, node) {
                        var secId = node.id;
                        var lastName = node.lastName;
                        var firstName = node.firstName;
                        var payRate = node.payRate;
                        var startDate = node.startDate;
                        var endDate = node.endDate;

                        var editLink = "<a href='#' onclick=\"launchTestDlg('EDIT','" + secId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
                        var deleteLink = "<a href='#' onclick=\"deletePrompt('" + secId + "','" + firstName + ' ' + lastName + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

                        var newRow = $("<tr  data-secId='" + secId + "' id='secTable_Row_" + secId + "'>" +
                            "<td id='secTable_cell_secId_" + secId + "'>" + secId + "</td>" +
                            "<td id='secTable_cell_lastName_" + secId + "'>" + lastName + "</td>" +
                            "<td id='secTable_cell_firstName_" + secId + "'>" + firstName + "</td>" +
                            "<td id='secTable_cell_payRate_" + secId + "'>" + payRate + "</td>" +
                            "<td id='secTable_cell_startDate_" + secId + "'>" + startDate + "</td>" +
                            "<td id='secTable_cell_endDate_" + secId + "'>" + endDate + "</td>" +
                            "<td>" + editLink +
                            " &nbsp;&nbsp;&nbsp; " +
                            deleteLink +
                            "</td>" +
                            "</tr>");
                        $('#secTable').append(newRow);
                        updMsg("GET Succeeded");
                    });

                },
                error: function (x, y, z) {
                    console.log(x); console.log(y); console.log(z);
                    updMsg("Error " + x + ' ' + y + ' ' + z);
                }
            });

            return false;
        });


        function updMsg(msg) {
            $('#resultMsg').html(msg);
            $('#resultMsg').css("visibility", "visible");
        }


        function deletePrompt(whichRow, name) {

            var name = $('#secTable_cell_lastName_' + whichRow).text();
            bootbox.confirm({
                title: "Delete Person?",
                message: "Please confirm deletion of Person <b>" + name + "</b>?",

                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> Cancel'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> Confirm'
                    }
                },
                callback: function (result) {
                    doDelete(result, whichRow, name);
                }
            });

        };

        function doDelete(result, whichRow, name) {

            if (!result) {
                return;
            }

            $.ajax
                ({
                    type: "DELETE",
                    url: serverBase + "/api/person/" + whichRow,
                    xhrFields: {
                        withCredentials: true
                    },
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader(reqSecKeyName, reqSecKeyValue);
                    },
                    dataType: 'json',
                    async: false,

                    complete: function (xhr, statusText) {
                        switch (xhr.status) {
                            case 204:
                                updMsg('Status ' + xhr.status + ' Deleted id: ' + whichRow);
                                $('#secTable_Row_' + whichRow).remove();
                                bootbox.alert('Person \'<b>' + name + '\'</b> Deleted');
                                break;
                            case 404:
                                updMsg('Status ' + xhr.status + ' No Record to delete');
                                break;
                            case 502:
                                updMsg('Status ' + xhr.status + ' Connection Failed');
                                break;
                            case 401:
                                updMsg('Status ' + xhr.status + ' Unauthorized');
                                break;
                            default:
                                updMsg('Status ' + xhr.status + ' Record Not deleted');
                        }
                    }

                });

            return false;

        };


        $("#testAdd").click(function () {
            launchTestDlg('ADD', '(NEW)');
            return false;
        });


        function launchTestDlg(mode, whichId) {

            Mode = mode;
            sectionID = whichId;

            $('#testEditDlgMsg').html("Ready");
            $("#id").val(whichId);
            $("#firstName").val('');
            $("#lastName").val('');
            $("#payRate").val('');
            $("#startDate").val('');
            $("#endDate").val('');
            $("#testEditDlg").modal('show');

            if (Mode == "ADD") {
                $("#testEditDlgHeader").text("Add Person");
                $("#testSaveBtn").prop("disabled", false);
                $("#id").prop("disabled", true);
            } else {
                $('#testEditDlgMsg').html(spinGlyph + " Fetching Section from server...");
                $.ajax
                    ({
                        type: "GET",
                        url: serverBase + "/api/person/" + whichId,

                        xhrFields: {
                            withCredentials: true
                        },

                        beforeSend: function (xhr) {
                            xhr.setRequestHeader(reqSecKeyName, reqSecKeyValue);
                        },

                        dataType: 'json',
                        async: false,

                        success: function (data) {
                            $("#firstName").val(data.firstName);
                            $("#lastName").val(data.lastName);
                            $("#payRate").val(data.payRate);
                            $("#startDate").val(data.startDate);
                            $("#endDate").val(data.endDate);
                            $('#testEditDlgMsg').html("Fetching Section from server...Success");
                        },
                        error: function (x, y, z) {
                            console.log(x); console.log(y); console.log(z);
                            updMsg("Error " + x + ' ' + y + ' ' + z);
                        }
                    });
            }

        }


        function doPersonSave() { //the edit dialog's Save button was pressed
            
            which = $('#id').val();
            var eput = JSON.stringify($('#testEditForm').serializeObject());

            action = "PUT"; //Update
            if (which == '(NEW)') {
                action = "POST"; //Create
                $('#testEditDlgMsg').html(spinGlyph + " Executing POST...");
            } else {
                $('#testEditDlgMsg').html(spinGlyph + " Executing PUT...");
            }

            var success = false;
            $.ajax({
                type: action,
                url: serverBase + "/api/person/" + which,

                //data: '{"lastName":"Hiotaky, "firstName":"William", "payRate":"45.00", "startDate":"2017-01-01", "endDate":"2017-12-21" }',
                data: eput,
                contentType: "application/json",

                xhrFields: {
                    withCredentials: true
                },

                beforeSend: function (xhr) {
                    xhr.setRequestHeader(reqSecKeyName, reqSecKeyValue);
                },

                dataType: 'json',
                async: false,
                complete: function (xhr, statusText) {
                    switch (xhr.status) {
                        case 201: // Our service did a successful create                            
                            which = xhr.getResponseHeader('X-HNET-ID'); //The new record's autoID                           
                            updMsg('Status ' + xhr.status + ' Created! New Record ID=' + which);
                            $('#testEditDlgMsg').html("Created Successfully!");
                            success = true;
                            break;
                        case 204:
                            updMsg('Status ' + xhr.status + ' Updated! ');
                            $('#testEditDlgMsg').html("Saved Successfully!");
                            success = true;
                            break;
                        case 404:
                            updMsg('Status ' + xhr.status + ' Record Not Found or Created');
                            $('#testEditDlgMsg').html("NOT SAVED");
                            break;
                        case 502:
                            updMsg('Status ' + xhr.status + ' Connection Failed');
                            $('#testEditDlgMsg').html("NOT SAVED");
                            break;
                        case 401:
                            updMsg('Status ' + xhr.status + ' Unauthorized');
                            $('#testEditDlgMsg').html("NOT SAVED");
                            break;
                        default:
                            updMsg('Status ' + xhr.status + ' Record Not updated');
                            $('#testEditDlgMsg').html("NOT SAVED");
                    }
                }

            });

            if (success) {
                //Get the newly created/updated record from the service 
                //and redisplay it (if updated) or display new record to bottom (if created)

                var theRow = "";

                $.ajax({
                    type: "GET",
                    url: serverBase + "/api/person/" + which,

                    xhrFields: {
                        withCredentials: true
                    },

                    beforeSend: function (xhr) {
                        xhr.setRequestHeader(reqSecKeyName, reqSecKeyValue);
                    },

                    dataType: 'json',
                    async: false,

                    success: function (data) {

                        var secId = data.id;
                        var lastName = data.lastName;
                        var firstName = data.firstName;
                        var payRate = data.payRate;
                        var startDate = data.startDate;
                        var endDate = data.endDate;

                        var editLink = "<a href='#' onclick=\"launchTestDlg('EDIT','" + secId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
                        var deleteLink = "<a href='#' onclick=\"deletePrompt('" + secId + "','" + firstName + ' ' + lastName + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

                        theRow = $("<tr  data-secId='" + secId + "' id='secTable_Row_" + secId + "'>" +
                            "<td id='secTable_cell_secId_" + secId + "'>" + secId + "</td>" +
                            "<td id='secTable_cell_lastName_" + secId + "'>" + lastName + "</td>" +
                            "<td id='secTable_cell_firstName_" + secId + "'>" + firstName + "</td>" +
                            "<td id='secTable_cell_payRate_" + secId + "'>" + payRate + "</td>" +
                            "<td id='secTable_cell_startDate_" + secId + "'>" + startDate + "</td>" +
                            "<td id='secTable_cell_endDate_" + secId + "'>" + endDate + "</td>" +
                            "<td>" + editLink +
                            " &nbsp;&nbsp;&nbsp; " +
                            deleteLink +
                            "</td>" +
                            "</tr>");
                    },
                    error: function (x, y, z) {
                        console.log(x); console.log(y); console.log(z);
                        updMsg("Error " + x + ' ' + y + ' ' + z);
                    }
                });

                //Refresh the main screen with the row we just build
                if (action == "PUT") {
                    $('#secTable_Row_' + which).replaceWith(theRow);
                } else { //POST
                    $('#secTable').append(theRow);
                }

            }

        }

    </script>


    <!-- Hidden Test  Dialog box ----------------------------------------------------->
    <div id="testEditDlg" class="modal fade">

        <div class="modal-dialog modal-md">
            <div class="modal-content">
                <div class="modal-header alert alert-info" style="margin-bottom: 1px;">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="testEditDlgHeader">Person Edit</h4>
                </div>

                <div class="modal-body" style="margin-top: 1px;">

                    <form name="testEditForm" id="testEditForm" method="post" style="margin-top: 1px;">
                        <div class="row">
                            <label class="col-sm-12" for="id">ID</label>
                            <div class="col-sm-12">
                                <input type="text" id='id' name='id' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                        <div class="row">
                            <label class="col-sm-12" for="firstName">First Name</label>
                            <div class="col-sm-12">
                                <input type="text" id='firstName' name='firstName' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                        <div class="row">
                            <label class="col-sm-12" for="lastName">Last Name</label>
                            <div class="col-sm-12">
                                <input type="text" id='lastName' name='lastName' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                        <div class="row">
                            <label class="col-sm-12" for="payRate">Pay Rate</label>
                            <div class="col-sm-12">
                                <input type="text" id='payRate' name='payRate' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                        <div class="row">
                            <label class="col-sm-12" for="startDate">Start Date</label>
                            <div class="col-sm-12">
                                <input type="text" id='startDate' name='startDate' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                        <div class="row">
                            <label class="col-sm-12" for="endDate">End Date</label>
                            <div class="col-sm-12">
                                <input type="text" id='endDate' name='endDate' class="form-control" maxlength="255" placeholder="" required />
                            </div>
                        </div>
                    </form>

                    <div id="testEditDlgMsg" class="text-warning" style="font-size: 20px;">&nbsp;</div>
                </div>

                <div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
                    <button type="button" class="btn btn-primary btn-s" id="testSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
                    <button type="button" class="btn btn-warning btn-s" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
                </div>
                &nbsp;
            </div>
        </div>

    </div>
    <!-- END Hidden linkEdit Dialog box -------------------------------------------------->

</asp:Content>
