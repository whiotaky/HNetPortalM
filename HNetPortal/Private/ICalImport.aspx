<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="ICalImport.aspx.cs" Inherits="HNetPortal.Private.ICalImport" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/Content/bootstrap-fileinput/css/fileinput.css") %>" rel="stylesheet" />
	<script src="<%=ResolveClientUrl("~/Scripts/fileinput.js")%>"> </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<h1><span class="glyphicon glyphicon-calendar"></span>&nbsp; The HNet Portal - Calendar Import For <%=User.Identity.Name %></h1>

	<div class="container row" style="margin-bottom: 20px;">
		<div class="row">
			<div class="hidden-sm hidden-md col-lg-2">
				&nbsp;
			</div>
			<div class="col-sm-12 col-md-12 col-lg-8">
				<asp:Label ID="Label1" runat="server" Style="color: red;" Text=""></asp:Label>
				<input id="input-710" name="input-710" type="file" class="file-loading" />
				<div id="uploadFailed" style="margin-top: 10px; display: none"></div>
				<div id="uploadFailed2" class="alert alert-danger alert-dismissable fade in" role="alert" style="margin-top: 10px; display: none"></div>
				<div id="uploadSuccess" class="alert alert-success fade in" style="margin-top: 10px; visibility: hidden"></div>
			</div>
		</div>
	</div>

	<img id='spinner' class="img-responsive center-block" src="<%= ResolveClientUrl("~/images/ajax-loader-feed2.gif")%>" style="display: none;" />




	<div id='optionsDiv' class="container row" style="margin-bottom: 40px; display: none">
		<div class="row panel">

			<div class="text-center col-xs-12" style='background-color: #C6E2EE; margin-bottom: 5px; border: 1px solid #003cff;'>
				<h4>Import Options</h4>
				<input type='checkbox' name='IMPORT_inclEnd' checked />
				Import End Date 
                <input type='checkbox' name='IMPORT_inclSummary' checked />
				Import Summary
                <input type='checkbox' name='IMPORT_inclDescr' checked />
				Import Description
                <input type='checkbox' name='IMPORT_inclLocation' checked />
				Import Location?<br />
				<br />
				<p>
					<b>When calendar date has existing event(s):</b><br />
					<input type="radio" name="IMPORT_whereToPut" value="top" checked />
					Add New Event to Top of Day
                <input type="radio" name="IMPORT_whereToPut" value="bottom" />
					Append New Event to Bottom of Day 
                <input type="radio" name="IMPORT_whereToPut" value="replace" />
					Replace with imported event
                <p />
					<button id='doImportBtn' type="submit" class="btn btn-primary btn-lg" onclick="this.disable">Import Now</button>
					<input type="hidden" name="IMPORT_iCalFileName" id="IMPORT_iCalFileName" value="" />
			</div>
		</div>
	</div>

	<div class="container row" id='dataDiv' style="display: none"></div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

	<script>

		$.ajaxSetup({
			// Disable caching of AJAX responses
			cache: false
		});

		var uploadUrl = "<%= ResolveClientUrl("~/WebServices/UploadHandler.ashx?caller=iCal") %>";
		var parseICalUrl = "<%= ResolveClientUrl("~/api/ICal/ParseFile")%>";

		$("#input-710").fileinput({
			uploadUrl: uploadUrl, // server upload action
			uploadAsync: true,
			showPreview: false,
			allowedFileExtensions: ['ics'],
			maxFileCount: 1,
			elErrorContainer: '#uploadFailed',
			slugCallback: function (text) {
				return String(text).replace(/[\[\]\/\{}:;#%=\*\+\?\\\^\$\|<>&\"]/g, '_');
			}

		}).on('filebatchpreupload', function (event, data, id, index) {
			$('#uploadFailed2').attr('style', 'display: none');
			$('#optionsDiv').attr("style", "display:none");
			$('dataDiv').attr("style", "display:none");
			$('#uploadSuccess').html('<h4>Upload Status</h4><ul></ul>').hide();

		}).on('fileuploaded', function (event, data, id, index) {
			//var fname = data.files[index].name,
			//        out = '<li>' + 'Uploaded file # ' + (index + 1) + ' - ' +
			//           fname + ' successfully.' + '</li>';

			//$('#uploadSuccess').attr('style', 'visibility: visible');
			//$('#uploadSuccess ul').append(out);
			//$('#uploadSuccess').fadeIn('slow');
			//$('#optionsDiv').fadeIn('slow');
			$('#IMPORT_iCalFileName').val(data.files[index].name);
			doICalParse(data.files[index].name);
		});


		function doICalParse(fileName) {

			console.log("DoICalParse url=" + fileName);

			$('#spinner').attr('style', 'display: normal');

			$.ajax({
				type: "POST",
				data: '{"sourceFileName":"' + fileName + '"}',
				url: parseICalUrl,
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {

					console.log("GET api/ICal/ParseFile " + fileName + " xhr.status=" + xhr.status + ", statusText=" + statusText);
					$('#spinner').attr('style', 'display: none');
					
					var err = "<span class='glyphicon glyphicon-warning-sign'></span>&nbsp;";

					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							err = "";
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							err = err + "Server returned 204 -- Delete fail";
							break;
						case 404:
							console.log("Server returned 404 -- Service Success but operation fail");
							break;
						case 500:
							console.log("Server returned 500 Error");
							console.log(xhr);
							err = err + "Exception in apiController (error 500): " + xhr.responseText;
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							err = err + "Unknown status code returned from server: " + xhr.status;
					}
					if (err != "") {
						//$('#optionsDiv').empty();
						err = err + "<a href='#' class='close' data-dismiss='alert' aria-label='close'>&times;</a>";
						$('#uploadFailed2').html(err);
						$('#uploadFailed2').fadeIn('slow');
						$('#dataDiv').empty();
						console.log("Emptied datadiv in complete");
					}
				},
				success: function (data) {

					console.log(data);
					$('#dataDiv').empty();					
					console.log("Emptied datadiv in success");
					$('#optionsDiv').fadeIn('slow');

					$.each(data, function (i, node) {

						var s = "<div class='row panel' style='background-color: #C6E2EE; margin-bottom: 5px; border: 1px solid #003cff; '>" +
							"<div class='col-xs-2'>" +
							"<input name='IMPORT_UID_" + node.uid + "' id='IMPORT_UID_" + node.uid + "'  type='checkbox' checked />   Import <br /> " +
							"</div>" +
							"<div class='col-xs-8'>" +
							"<b>Uid: " + node.uid + "</b><br/>" +
							"<b>Starts: " + node.startDate + "</b><br/>" +
							"<b>Ends:</b> " + node.endDate + "<br/>" +
							"<b>Summary:</b> " + node.summary + "<br/>" +
							"<b>Description:</b> " + node.description + "<br/>" +
							"<b>Location:</b> " + node.location + "<br/>" +
							"</div>" +
							"</div>";

						$('#dataDiv').append(s);
					});
					
					$('#dataDiv').fadeIn('slow');
				}
				
			});

		}

	</script>

</asp:Content>
