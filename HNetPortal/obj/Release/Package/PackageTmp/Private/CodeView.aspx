<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="CodeView.aspx.cs" Inherits="HNetPortal.Private.CodeView" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
    <link href="<%=ResolveClientUrl("~/Content/Highlight/ir_black.css") %>" rel="stylesheet" />
    <script src="<%=ResolveClientUrl("~/Scripts/Highlight/highlight.pack.js")%>"></script>
    <script>hljs.initHighlightingOnLoad();</script>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    
  <h1><span class="fa fa-code"></span>&nbsp; The HNet Portal - Code View</h1>
  
    <h3><span class="fa fa-folder-o"></span>&nbsp; Projects:</h3>
    <asp:ListBox ID="projectsLB" class="form-control" runat="server" Rows="10" Font-Size="Larger"></asp:ListBox>

    <h3><span class="fa fa-folder-open-o"></span>&nbsp; Source Files:</h3>
    <asp:ListBox ID="fileNamesLB" class="form-control" runat="server" Rows="10" Font-Size="Larger"></asp:ListBox>

    <h3><span class="fa fa-file-code-o"></span>&nbsp; Source Code: </h3>
    <pre >
        <code id="codeArea" class="css" style="overflow:scroll; height:375px">
[Select a source file]
         </code>
    </pre>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

    <script>

        $.ajaxSetup({
            // Disable caching of AJAX responses
            cache: false
        });

        $(document).ready(function () {

            $('#projectsLB').change(function () {

                var targetProj = $('#projectsLB').val();
                $('#fileNamesLB').attr('disabled', true);
                $('#projectsLB').attr('disabled', true);

                var pleaseWaitDlg = bootbox.dialog({
                    message: '<p class="text-center"><i class="fa fa-spin fa-spinner"></i> Fetching Project ' + targetProj+', Please wait...</p>',
                    animate: false,
                    backdrop: false,
                    closeButton: false

                });
                pleaseWaitDlg.css({
                    'top': '50%',
                    'margin-top': function () {
                        return -(pleaseWaitDlg.height() / 2);
                    }
                });


                $.ajax({
                    type: 'POST',
					url: '<%= ResolveClientUrl("~/api/TFS/ListFiles") %>',
                    data: '{"sourcePathName":"' + targetProj + '"}',
                    contentType: "application/json",
                    dataType: "json",
                   
                    success: function (data) {

                        $('#fileNamesLB').attr('disabled', false);
                        $('#projectsLB').attr('disabled', false);
                        $("[id*=fileNamesLB] option").remove();

                        $("#codeArea").html("[Select a source file]");
                        $.each(data, function (i, node) {
                            var itemName = node.itemName;
                            $("#fileNamesLB").append('<option>' + itemName + '</option>');
                        });

                        pleaseWaitDlg.modal('hide');

                    },
                    error: function (data, textStatus, errorThrown) {
                        pleaseWaitDlg.modal('hide');
                        $('#fileNamesLB').attr('disabled', false);
                        $('#projectsLB').attr('disabled', false);
                        
                        bootbox.alert('Error Fetching source files list: ' + errorThrown);
                    }

                });

            });


            $('#fileNamesLB').change(function () {
               
                var sourceFile = $('#fileNamesLB').val();            
                $('#fileNamesLB').attr('disabled', true);
                $('#projectsLB').attr('disabled', true);

                var pleaseWaitDlg = bootbox.dialog({
                    message: '<p class="text-center"><i class="fa fa-spin fa-spinner"></i> Fetching source ' +sourceFile + ', Please wait...</p>',
                    animate: false,
                    backdrop: false,
                    closeButton: false
                });
                pleaseWaitDlg.css({
                    'top': '50%',
                    'margin-top': function () {
                        return -(pleaseWaitDlg.height() / 2);
                    }
                });

                $.ajax({
                    type: 'POST',
					url: '<%= ResolveClientUrl("~/api/TFS/FileContent") %>',
                    data: '{"sourceFileName":"' + sourceFile + '"}',
                    contentType: "application/json",
                    dataType: "json",
                   
                    success: function (data) {
                        //alert(data.sourceContents);
                        $('#fileNamesLB').attr('disabled', false);
                        $('#projectsLB').attr('disabled', false);

                        var status = "ok";
                        if(Object.prototype.toString.call(data) === '[object Array]') {
                            status = data[0].status;
                        }

                        if (status.indexOf("Error") >= 0) {                            
                            bootbox.alert('Error Fetching Source: ' + status);
                        } else {
                            var ext = sourceFile.substr((sourceFile.lastIndexOf('.') + 1));
                            $("#codeArea").removeClass();
                            $("#codeArea").html(data.sourceContents);
                            $("#codeArea").removeClass().addClass(ext); //generally highlightjs will have a class for the extension
                            $('pre code').each(function (i, block) {
                                hljs.highlightBlock(block);
                            });
                        }
                        pleaseWaitDlg.modal('hide');
                    },
                    error: function (data, textStatus, errorThrown) {
                        pleaseWaitDlg.modal('hide');
                        $('#fileNamesLB').attr('disabled', false);
                        $('#projectsLB').attr('disabled', false);
                        bootbox.alert('Error Fetching source file contents: ' + errorThrown);
                    }

                });

            });

        });

    </script>


</asp:Content>

