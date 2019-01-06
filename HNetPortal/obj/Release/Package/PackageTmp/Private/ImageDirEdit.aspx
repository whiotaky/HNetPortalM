<%@ Page Title="HNet Images - Edit" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ImageDirEdit.aspx.cs" Inherits="HNetPortal.Private.ImageDirEdit" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="<%=ResolveClientUrl("~/Content/bootstrap-fileinput/css/fileinput.css") %>" rel="stylesheet" />
    <script src="<%=ResolveClientUrl("~/Scripts/fileinput.js")%>"> </script>

    <style>
        #carousel-custom {
            margin: 20px auto;
            width: 80%;
        }

            #carousel-custom .carousel-indicators {
                margin: 10px 0 0;
                overflow: auto;
                position: static;
                text-align: left;
                white-space: nowrap;
                width: 100%;
            }

                #carousel-custom .carousel-indicators li {
                    background-color: transparent;
                    -webkit-border-radius: 0;
                    border-radius: 0;
                    display: inline-block;
                    height: auto;
                    margin: 0 !important;
                    width: auto;
                }

                    #carousel-custom .carousel-indicators li img {
                        display: block;
                    }


            #carousel-custom .carousel-outer {
                position: relative;
            }

        .carousel-inner {
            width: 100%;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <div class="row">
        <div class="col-xs-12 col-xs-offset-0 col-md-11 col-md-offset-1">

            <h1><span class="glyphicon glyphicon-picture"></span>&nbsp;HNet Images - Edit</h1>
            <b><a href='/ImageDirList.aspx'>Index</a> |
                <asp:Label ID="fpNameLabel" runat="server" Text="Label"></asp:Label></b><br />
            <b>Rename:</b>
            <asp:TextBox ID="dirNameInput" runat="server"></asp:TextBox>
            <button id='dirNameBtn' name='dirNameBtn' class='btn btn-xs btn-primary'>Save</button>&nbsp;
            <button runat="server" id='secToggleBtn' name='secToggleBtn' class="btn btn-xs btn-success"/>

            <div id="carousel-custom" class="carousel slide lazy-load" data-ride="carousel" data-interval="false">

                <ol class='carousel-indicators' id="carouselIndicators">
                    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                        <ItemTemplate>
                            <li data-target='#carousel-custom' data-slide-to='<%# Eval("idx") %>' class='<%# Eval("CaroClass") %>'>
                                <asp:Image ID="imgThumb" Style="width: 50px; height: 50px" runat="server" OnClick="showImg(this);" />
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ol>
            </div>

            <asp:Label ID="Label2" runat="server" Style="color: red;" Text=""></asp:Label>
            <input id="input-fup" name="input-fup" type="file" multiple="multiple" class="file-loading" />

            <div id="uploadFailed" style="margin-top: 10px; display: none"></div>
            <div id="uploadFailed2" class="alert alert-danger alert-dismissable fade in" role="alert" style="margin-top: 10px; display: none"></div>
            <div id="uploadSuccess" class="alert alert-success fade in" style="margin-top: 10px; visibility: hidden"></div>

        </div>
    </div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

    <script>

        var uploadUrl = "<%= ResolveClientUrl("~/WebServices/UploadHandler.ashx?caller=ib&dir=") %>";
        var dirNameChangeUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/dirNameChange") %>";
        var deleteImageUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/deleteImage") %>";
        var imageDirSetPermUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/imageDirSetPerm") %>";


        var dirParam = "<% =whichDir %>";
        var nextCaroIdx = "<% =nextCaroIdx %>";
        var isPublic = "<% =isPublic %>";

        $.ajaxSetup({
            // Disable caching of AJAX responses
            cache: false
        });


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

            $("#dirNameBtn").click(function () {
                doDirNameSave();
                return false;
            });


            $("#delImageBtn").click(function () {
                doDeleteImage();
            });


            $("#secToggleBtn").click(function () {
                doSecToggle();
                return false;
            });
            
        });


        function doSecToggle() {
            //alert(dirParam+" "+isPublic);
            //imageDirSetPermUrl


            var newStatus = isPublic == 'Y' ? 'N' : 'Y';
            $.ajax({
                type: "POST",
                data: '{"dirName":"' + dirParam + '","hasPublicAccess": "'+newStatus+'"}',
                url: imageDirSetPermUrl,
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
                    var status = data[0].status;
                    if (status.indexOf("Error") >= 0) {
                        var status = data[0].status;
                        bootbox.alert('Error setting access: ' + status);
                    } else {

                        isPublic = newStatus;
                        
                        if (isPublic == 'Y') {
                            $("#secToggleBtn").removeClass("btn-danger").addClass("btn-success");
                            $("#secToggleBtn").html("Make Private&nbsp;<span class=\"glyphicon glyphicon-eye-close\"></span>");
                            bootbox.alert('Directory has been made Public&nbsp;<span class=\"glyphicon glyphicon-eye-open\"></span>');
                        } else {
                            $("#secToggleBtn").removeClass("btn-success").addClass("btn-danger");
                            $("#secToggleBtn").html("Make Public&nbsp;<span class=\"glyphicon glyphicon-eye-open\"></span>");
                            bootbox.alert('Directory has been made  Private&nbsp;<span class=\"glyphicon glyphicon-eye-close\"></span>');
                        }
                        
                    }

                },
                error: function (data, textStatus, errorThrown) {
                    bootbox.alert('Error: ' + errorThrown);
                }
            });

        }


        function doDeleteImage() {
            var imageName = $("#delImageBtn").attr('data-which');

            $.ajax({
                type: "POST",
                data: '{"whichImage":"' + imageName + '"}',
                url: deleteImageUrl,
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
                    var status = data[0].status;
                    if (status.indexOf("Error") >= 0) {
                        var status = data[0].status;
                        bootbox.alert('Error deleting: ' + status);
                    } else {

                        //remove the thumbnail image from the carousel 
                        $('#carouselIndicators li').each(function (i) {
                            var testSrc = $('img', this).attr('data-imagePath');
                            if (testSrc == imageName) {
                                $(this).remove();
                            }
                        });

                        $("#imgDlg").modal('hide');
                        $("#dlgImg").attr("src", "/images/ajax-loader3.gif");

                        bootbox.alert('Image deleted');
                    }

                },
                error: function (data, textStatus, errorThrown) {
                    bootbox.alert('Error: ' + errorThrown);
                }
            });


        }


        function doDirNameSave() {

            var newDirName = "/" + $('#dirNameInput').val();

            $.ajax({
                type: "POST",

                data: '{"whichDir":"' + dirParam + '", "newDirName":"' + newDirName + '"}',
                url: dirNameChangeUrl,
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
                    var status = data[0].status;
                    if (status.indexOf("Error") >= 0) {
                        var status = data[0].status;
                        bootbox.alert('Error renaming: ' + status);
                    } else {

                        //update on-screen dir path display                   
                        dirParam = newDirName;

                        $('#fpNameLabel').html("Full Path: " + dirParam);
                        // alert(uploadUrl + dirParam);

                        //update the fileinput's path to the upload dir.  Note that this will
                        //also clear out the input's preview box; oh well.
                        $("#input-fup").fileinput('refresh', { uploadUrl: uploadUrl + dirParam });

                        //update images paths in the carousel 
                        $('#carouselIndicators li').each(function (i) {
                            var oldSrc = $('img', this).attr('src');
                            var n = oldSrc.lastIndexOf('/');
                            var fn = oldSrc.substring(n + 1);
                            var newThumbSrc = "/imagebase/thumb" + dirParam + "/" + fn;
                            var newFullSrc = "/imagebase/full" + dirParam + "/" + fn;
                            $('img', this).attr('src', newThumbSrc);
                            $('img', this).attr('data-imagePath', newFullSrc);
                        });

                        bootbox.alert('Directory successfully renamed to ' + dirParam);
                    }

                },
                error: function (data, textStatus, errorThrown) {
                    bootbox.alert('Error: ' + errorThrown);
                }
            });

        }


        $("#input-fup").fileinput({
            uploadUrl: uploadUrl + dirParam, // server upload action
            uploadAsync: true,
            showPreview: true,
            allowedFileExtensions: ['jpg', 'gif', 'png', 'bmp'],
            minFileCount: 1,
            maxFileCount: 20,
            slugCallback: function (text) {
                return String(text).replace(/[\[\]\/\{}:;#%=\*\+\?\\\^\$\|<>&\"]/g, '_');
            },

        }).on('fileuploaded', function (event, data, id, idx) {

            //console.log(data.files);     
            //$.each(data.files, function (i) {
            //alert(data.files[i].name);

            newThumbSrc = "/imagebase/thumb" + dirParam + "/" + data.files[idx].name;
            newFullSrc = "/imagebase/full" + dirParam + "/" + data.files[idx].name;
            $("<li data-target='#carousel-custom' data-slide-to='" + nextCaroIdx + "'>" +
                "<img id='imgThumb' Onclick='showImg(this);' src='" + newThumbSrc +
                "' alt='' data-imagePath='" + newFullSrc + "' style='width: 50px; height: 50px' /></li>"
                ).appendTo('#carouselIndicators');

            nextCaroIdx++;
            //});

        });


        function showImg(which) {
            $("#dlgImg").attr("src", "/images/ajax-loader3.gif");
            var s = which.getAttribute('data-imagePath');
            $("#dlgImg").attr("src", s);
            $("#myLargeModalLabel-1").html(s);
            $("#delImageBtn").attr("data-which", s);

            $("#imgDlg").modal('show');
            return false;
        };

    </script>


    <div id="imgDlg" class="modal" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel-1" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 class="modal-title" id="myLargeModalLabel-1"></h4>
                </div>
                <div class="modal-body text-center">
                    <img id="dlgImg" src="/images/ajax-loader3.gif" class="img-responsive img-rounded center-block" alt="" /><br />
                    <button class="btn btn-danger" id="delImageBtn" data-which="">Delete Image</button>

                </div>

            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal mixer image -->


</asp:Content>
