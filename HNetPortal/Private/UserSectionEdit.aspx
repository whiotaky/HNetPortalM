<%@ Page Title="User Links Edit" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="UserSectionEdit.aspx.cs" Inherits="HNetPortal.Private.UserSectionEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link rel="stylesheet" href="/Content/font-awesome.css" type="text/css" />
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

		tbody tr:hover td {
			/*background-color: #C6E2EE; */
			cursor: pointer;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">


	<h1><span class="glyphicon glyphicon-link"></span>&nbsp; The HNet Portal - Links For <%=User.Identity.Name %></h1>

	<div class="container">
		<div class="row">
			<div class="panel panel-primary">

				<div class="panel-heading ">
					<h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;Links Sections</h3>
				</div>

				<div class="panel-body">
					<div class="pre-scrollable" style="height: 180px; max-height: 180px;">
						<div class='table-responsive'>
							<table id="secTable" class="table table-hover">
								<thead>
									<tr>
										<th>Enabled</th>
										<th>Links Section</th>
										<th>Order</th>
										<th>Action</th>
									</tr>
								</thead>
								<tbody>
									<tr id="secTable_row_Spinner">
										<td colspan='4'>
											<div id="secTable_row_Progress" class="progress progress-striped active">
												<div class="progress-bar" style="width: 100%; font-size: 20px">Fetching Section Definitions...</div>
											</div>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>


	<div class="container" id="linksContainer" style="display: none;">
		<div class="row">
			<div class="panel panel-success">

				<div class="panel-heading">
					<h3 id="linksPanelHeading" class="panel-title"><span class="glyphicon glyphicon-bookmark"></span>&nbsp;&nbsp;&nbsp;Links</h3>
				</div>

				<div class="panel-body">
					<div class="pre-scrollable" style="height: 250px; max-height: 250px;">
						<div class='table-responsive'>
							<table id="linksTable" class="table table-hover">
								<thead>
									<tr>
										<th>Enabled</th>
										<th>Link Text</th>
										<th>Sub-Section</th>
										<th>Order</th>
										<th>Action</th>
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

		//Deprecated 3/10/2017
		//var getUserLinksSectionsUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getUserLinksSections") %>";
		//var delUserLinkSectionRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/delUserLinkSectionRec") %>";
		//var editUserLinkSectionRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/editUserLinkSectionRec") %>";
		//var addUserLinkSectionRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/addUserLinkSectionRec") %>";
		//var getUserLinksUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getUserLinks") %>";
		//var editUserLinkRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/editUserLinkRec") %>";
		//var addUserLinkRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/addUserLinkRec") %>";
		//var delUserLinkRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/delUserLinkRec") %>";
		//var resequenceLinksSectionsUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/resequenceLinksSections") %>";
		//var resequenceLinkSectionDetailsUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/resequenceLinkSectionDetails") %>";

		var enabledGlyph = "<span class='new glyphicon glyphicon-ok'></span>";
		var disabledGlyph = "<span class='trash glyphicon glyphicon-remove'></span>";
		var spinGlyph = "<span class='glyphicon glyphicon-cog glyphicon-spin' style='margin-right:8px;'></span>";
		var alertGlyph = "<span class='glyphicon glyphicon-alert' style='margin-right:8px;'></span>";
		var linksGlyph = "<span class='glyphicon glyphicon-bookmark'></span>&nbsp;&nbsp;&nbsp;";

		var Method;
		var sectionID;
		var linkID;
		var currSectionRowID;
		var currSectionText;

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

			$('#secTable').on('click', '.clickable-cell', function (event) {

				currSectionRowID = $(this).closest('tr').attr('id');
				sectionID = $('#' + currSectionRowID).attr("data-sectionid");
				currSectionText = $('#' + currSectionRowID).attr("data-sectionText");

				$('#secTable tr').attr("style", "background-color: white;");
				$('#' + currSectionRowID).attr("style", "background-color: #C6E2EE;")

				$('#linksTable tr').slice(1).remove();
				$('#linksContainer').attr("style", "display:normal");
				loadLinks(sectionID);

			});

			$("#sectionSaveBtn").click(function () { doSectionSave(); });
			$("#linkSaveBtn").click(function () { doLinkSave(); });
			$("#seqSaveBtn").click(function () { doSeqSave(); });

			loadSections();

		});


		function hideLinks() {
			$('#secTable tr').attr("style", "background-color: white;");
			$('#linksTable tr').slice(1).remove();
			$('#linksContainer').attr("style", "display:none ");
		}


		function loadLinks(secID) {

			$('#linksPanelHeading').html(linksGlyph + "Links For " + currSectionText);

			var spinRow = $("<tr id='linksTable_Row_Spinner'>" +
				"<td colspan='5'>" +
				"<div id='linksTable_row_Progress' class='progress progress-striped active'>" +
				"<div class='progress-bar  progress-bar-success' style='width: 100%; font-size: 20px'>Fetching Links For " + currSectionText + "...</div>" +
				"</div>" +
				"</td>" +
				"</tr>");
			var errorRow = $("<tr>" +
				"<td colspan='5'>" +
				"<div class='alert alert-danger' style='font-size:20px;'><span class='glyphicon glyphicon-warning-sign'></span>&nbsp;&nbsp;An AJAX error has occurred. </div>" +
				"</td>" +
				"</tr>");

			$('#linksTable').append(spinRow);

			var theUrl = '<%= ResolveClientUrl("~/api/UserLinks/User/List/") %>' + secID;
			console.log("TheUrl: " + theUrl);

			$.ajax({
				type: "GET",
				url: theUrl,
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("LoadLinks section=" + secID + " xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 204 -- Delete fail");
							break;
						case 404:
							console.log("Server returned 404 -- Service Success but operation fail");
							$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 404 -- Service Success but operation fail");
							break;
						case 500:
							console.log("Server returned 500 Error");
							$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 500 Error");
							bootbox.alert("Exception in apiController (error 500)");
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {
					$('#linksTable_Row_Spinner').remove();
					buildLinksTable(data);
				}

			});

		}


		function buildLinksTable(data) {

			$('#linksTable tr').slice(1).remove();
			$('#linksContainer').attr("style", "display:normal");

			var addRecRow = $("<tr id='linksTable_Row_AddRec'>" +
				"<td>&nbsp;</td>" +
				"<td>&nbsp;</td>" +
				"<td>&nbsp;</td>" +
				"<td><button class='btn btn-success' onclick=\"launchResequenceLinks('" + sectionID + "'); return false;\"><i class=\"fa fa-list-ol \" ></i>&nbsp;Resequence</button></td>" +
				"<td><button class='btn btn-success' onclick=\"launchEditLinkDlg('PUT','0','" + sectionID + "');return false;\"><i class=\"fa fa-plus \"></i>&nbsp;Add</button></td>" +
				"</td>" +
				"</tr>");

			$.each(data, function (i, node) {
				console.log(node);
				var linkId = node.linkid;
				var secId = node.sectionid;
				var orderBy = node.orderby;
				var linkText = node.linkText;
				var subSectionText = node.subSectionText;
				var enabled = (node.enabled == "Y") ? enabledGlyph : disabledGlyph;
				var editLink = "<a href='#' onclick=\"launchEditLinkDlg('POST','" + linkId + "','" + secId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
				var deleteLink = "<a href='#' onclick=\"deleteLinkPrompt('" + linkId + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

				var newRow = $("<tr data-linkid='" + linkId + "' id='linksTable_Row_" + linkId + "' class='clickable-row'>" +
					"<td id='linksTable_cell_enabled_" + linkId + "' class='clickable-cell'>" + enabled + "</td>" +
					"<td id='linksTable_cell_linkText_" + linkId + "' class='clickable-cell'>" + linkText + "</td>" +
					"<td id='linksTable_cell_subSectionText_" + linkId + "' class='clickable-cell'>" + subSectionText + "</td>" +
					"<td id='linksTable_cell_orderby_" + linkId + "' class='clickable-cell'>" + orderBy + "</td>" +
					"<td>" + editLink +
					" &nbsp;&nbsp;&nbsp; " +
					deleteLink +
					"</td>" +
					"</tr>");
				$('#linksTable').append(newRow);
			});

			$('#linksTable').append(addRecRow);
		}

		function launchEditLinkDlg(method, whichLink, whichSec) {

			console.log("launchEditDlg method=" + method + " whichLink=" + whichLink + " whichsec=" + whichSec);

			Method = method;
			linkID = whichLink;
			sectionID = whichSec;

			$('#linkEditDlgMsg').html("&nbsp;");
			$("#sectionid_l").val(sectionID);
			$('#linkid').val(linkID);
			$("#enabled_cb_l").prop("checked", true);
			$("#newwindow_cb_l").prop("checked", true);
			$("#ismenuitem_cb_l").prop("checked", false);
			$("#enabled_l").val("Y");
			$("#newwindow_l").val("Y");
			$("#ismenuitem_l").val("N");
			$("#orderby_l").val("");
			$("#linkText").val("");
			$("#linkURL").val("");
			$("#subSectionText").val("");
			$("#hoverText").val("");
			$("#linkSaveBtn").prop("disabled", true);
			$("#enabled_cb_l").prop("disabled", true);
			$("#newwindow_cb_l").prop("disabled", true);
			$("#ismenuitem_cb_l").prop("disabled", true);
			$("#orderby_l").prop("disabled", true);
			$("#linkText").prop("disabled", true);
			$("#linkURL").prop("disabled", true);
			$("#subSectionText").prop("disabled", true);
			$("#hoverText").prop("disabled", true);
			$('#timestamp_l').html("Updated: N/A");

			$("#linkEditDlg").modal('show');

			if (Method == "PUT") {
				$("#linkEditHeader").text("Add Link");
				$("#linkSaveBtn").prop("disabled", false);
				$("#enabled_cb_l").prop("disabled", false);
				$("#newwindow_cb_l").prop("disabled", false);
				$("#ismenuitem_cb_l").prop("disabled", false);
				$("#orderby_l").prop("disabled", false);
				$("#linkText").prop("disabled", false);
				$("#linkURL").prop("disabled", false);
				$("#subSectionText").prop("disabled", false);
				$("#hoverText").prop("disabled", false);
			} else {

				var theUrl = '<%= ResolveClientUrl("~/api/UserLinks/User/Link/") %>' + whichSec + '/' + whichLink;
				console.log("TheUrl: " + theUrl);

				$('#linkEditDlgMsg').html(spinGlyph + " Fetching Section from server...");
				$.ajax({
					type: "GET",
					//data: '{"linkid":"' + linkID + '", "sectionid":"0"}',
					url: theUrl,
					contentType: "application/json",
					dataType: "json",
					complete: function (xhr, statusText) {
						console.log("launchEditLinkDlg section=" + whichSec + " xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // success
								console.log("Server returned 200 -- Success!");
								break;
							case 204: // Delete failed
								console.log("Server returned 204 -- Delete fail");
								$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 204 -- Delete fail");
								break;
							case 404:
								console.log("Server returned 404 -- Service Success but operation fail");
								$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 404 -- Service Success but operation fail");
								break;
							case 500:
								console.log("Server returned 500 Error");
								$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Server returned 500 Error");
								bootbox.alert("Exception in apiController (error 500)");
								break;
							default:
								console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
								$('#linksTable_Row_Spinner').replaceWith(alertGlyph + "Unknown status code returned from server: " + xhr.status);
								bootbox.alert("Unknown status code returned from server: " + xhr.status);
						}
					},


					success: function (data) {
						$("#linkSaveBtn").prop("disabled", false);
						$("#enabled_cb_l").prop("disabled", false);
						$("#newwindow_cb_l").prop("disabled", false);
						$("#ismenuitem_cb_l").prop("disabled", false);
						$("#orderby_l").prop("disabled", false);
						$("#linkText").prop("disabled", false);
						$("#linkURL").prop("disabled", false);
						$("#subSectionText").prop("disabled", false);
						$("#hoverText").prop("disabled", false);
						$("#enabled_cb_l").prop("checked", data.enabled == "Y" ? true : false);
						$("#newwindow_cb_l").prop("checked", data.newwindow == "Y" ? true : false);
						$("#ismenuitem_cb_l").prop("checked", data.ismenuitem == "Y" ? true : false);
						$("#enabled_l").val("Y");
						$("#newwindow_l").val("N");
						$("#ismenuitem_l").val("N");
						$("#orderby_l").val(data.orderby);
						$("#linkText").val(data.linkText);
						$("#linkURL").val(data.linkURL);
						$("#subSectionText").val(data.subSectionText);
						$("#hoverText").val(data.hoverText);
						$('#linkEditDlgMsg').html("&nbsp;");
						$('#timestamp_l').html("Updated: " + data.timestamp);
					},

				});
			}

		}

		function deleteLinkPrompt(whichRow) {

			var name = $('#linksTable_cell_linkText_' + whichRow).text();
			bootbox.confirm({
				title: "Delete Link?",
				message: "Please confirm deletion of Link  <b>\"" + name + "\"</b>, ",
				buttons: {
					cancel: {
						label: '<i class="fa fa-times"></i> Cancel'
					},
					confirm: {
						label: '<i class="fa fa-check"></i> Confirm'
					}
				},
				callback: function (result) {
					doDeleteLink(result, whichRow, name);
				}
			});

		}


		function doDeleteLink(result, whichRow, name) {

			if (!result) {
				return;
			}

			console.log("doDeleteLink whichRow=" + whichRow + " name=" + name);

			$.ajax({
				type: "DELETE",
				url: '<%= ResolveClientUrl("~/api/UserLinks/User/Link/") %>' + whichRow,
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("doDeleteLink xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // Delete success 
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							bootbox.alert('Error Deleting -- Nothing was there to delete (204 error)');
							break;
						case 500:
							console.log("Server returned 500 Error");
							bootbox.alert('Error Deleting (500 error)');
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {
					console.log("Delete link");
					$('#linksTable_Row_' + whichRow).remove();
				}

			});

		}

		function loadSections() {

			console.log("loadSections (all)");

			var errorRow = $("<tr>" +
				"<td colspan='4'>" +
				"<div class='alert alert-danger' style='font-size:20px;'><span class='glyphicon glyphicon-warning-sign'></span>&nbsp;&nbsp;An AJAX error has occurred. </div>" +
				"</td>" +
				"</tr>");

			$.ajax({
				type: "GET",
				url: '<%= ResolveClientUrl("~/api/UserLinks/Master/List") %>',
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("loadSections xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						case 500:
							console.log("Server returned 500 Error");
							$('#secTable_row_Spinner').replaceWith(errorRow);
							bootbox.alert("Exception in apiController (error 500)");
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#secTable_row_Spinner').replaceWith(errorRow);
							$('#sectionEditDlgMsg').html("Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {
					$('#secTable_row_Spinner').remove();
					buildSectionsTable(data);
				}

			});

		};


		function buildSectionsTable(data) {
			console.log("buildSectionsTable()");
			$('#secTable tr').slice(1).remove();

			$.each(data, function (i, node) {
				var secId = node.sectionid;
				var orderBy = node.orderby;
				var sectionText = node.sectionText;
				var enabled = (node.enabled == "Y") ? enabledGlyph : disabledGlyph;
				var editLink = "<a href='#' onclick=\"launchEditSecDlg('POST','" + secId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
				var deleteLink = "<a href='#' onclick=\"deleteSecPrompt('" + secId + "','" + sectionText + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

				var newRow = $("<tr data-sectionText='" + sectionText + "' data-sectionid='" + secId + "' id='secTable_Row_" + secId + "' class='clickable-row'>" +
					"<td id='secTable_cell_enabled_" + secId + "' class='clickable-cell'>" + enabled + "</td>" +
					"<td id='secTable_cell_linkName_" + secId + "' class='clickable-cell'>" + sectionText + "</td>" +
					"<td id='secTable_cell_orderby_" + secId + "' class='clickable-cell'>" + orderBy + "</td>" +
					"<td>" + editLink +
					" &nbsp;&nbsp;&nbsp; " +
					deleteLink +
					"</td>" +
					"</tr>");
				$('#secTable').append(newRow);

			});

			var addRecRow = $("<tr id='secTable_Row_AddRec'>" +
				"<td>&nbsp;</td>" +
				"<td>&nbsp;</td>" +
				"<td><button class='btn btn-success' onclick=\"launchResequenceSections(); return false;\"><i class=\"fa fa-list-ol \" ></i>&nbsp;Resequence</button></td>" +
				"<td><button class='btn btn-success' onclick=\"launchEditSecDlg('PUT','0');return false; \"><i class=\"fa fa-plus \"></i>&nbsp;Add</button></td>" +
				"</tr>");
			$('#secTable').append(addRecRow);

		}


		function launchEditSecDlg(method, which) {

			hideLinks();

			Method = method;
			sectionID = which;

			$('#sectionEditDlgMsg').html("&nbsp;");

			$("#sectionid").val(sectionID);
			$("#enabled_cb").prop("checked", true);
			$("#enabled").val("Y");
			$("#orderby").val("");
			$("#sectionText").val("");
			$("#sectionSaveBtn").prop("disabled", true);
			$("#enabled_cb").prop("disabled", true);
			$("#orderby").prop("disabled", true);
			$("#sectionText").prop("disabled", true);
			$('#timestamp').html("Updated: N/A");

			if (Method == "PUT") {
				$("#sectionEditHeader").text("Add Section");
				$("#sectionSaveBtn").prop("disabled", false);
				$("#enabled_cb").prop("disabled", false);
				$("#orderby").prop("disabled", false);
				$("#sectionText").prop("disabled", false);
			} else {
				console.log("launchEditSecDlg method=" + Method + ", which=" + which, " SectionID=" + sectionID);
				$('#sectionEditDlgMsg').html(spinGlyph + " Fetching Section from server...");
				$.ajax({
					type: "GET",
					url: '<%= ResolveClientUrl("~/api/UserLinks/Master/") %>' + sectionID,
					contentType: "application/json",
					dataType: "json",
					complete: function (xhr, statusText) {
						console.log("loadEditSecDlg xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // success
								console.log("Server returned 200 -- Success");
								break;
							case 500:
								console.log("Server returned 500 Error");
								bootbox.alert("Exception fetching in apiController (error 500)");
								break;
							default:
								console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
								$('#sectionEditDlgMsg').htm("Unknown status code returned from server: " + xhr.status);
								bootbox.alert("Unknown status code returned from server: " + xhr.status);
						}
					},
					success: function (data) {
						$("#sectionSaveBtn").prop("disabled", false);
						$("#enabled_cb").prop("disabled", false);
						$("#orderby").prop("disabled", false);
						$("#sectionText").prop("disabled", false);
						$("#enabled_cb").prop("checked", data.enabled == "Y" ? true : false);
						$("#enabled").val("Y");
						$("#orderby").val(data.orderby);
						$("#sectionText").val(data.sectionText);
						$('#sectionEditDlgMsg').html("&nbsp;");
						$('#timestamp').html("Updated: " + data.timestamp);
					},

				});

			}

			$("#sectionEditDlg").modal('show');
		}


		function deleteSecPrompt(whichRow, name) {

			var name = $('#secTable_cell_linkName_' + whichRow).text();
			bootbox.confirm({
				title: "Delete Link Section And Related Links?",
				message: "Please confirm deletion of Link Section <b>\"" + name + "\"</b>, " +
				"AND BE AWARE THIS WILL DELETE ALL LINKS ASSOCIATED WITH IT\!",
				buttons: {
					cancel: {
						label: '<i class="fa fa-times"></i> Cancel'
					},
					confirm: {
						label: '<i class="fa fa-check"></i> Confirm'
					}
				},
				callback: function (result) {
					doDeleteSec(result, whichRow, name);
				}
			});

		};


		function doDeleteSec(result, whichRow, name) {

			if (!result) {
				return;
			}

			hideLinks();

			$.ajax({
				type: "DELETE",
				url: '<%= ResolveClientUrl("~/api/UserLinks/Master/") %>' + whichRow,
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("doDeleteSec xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // Delete success 
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							bootbox.alert('Error Deleting -- Nothing was there to delete (204 error)');
							break;
						case 500:
							console.log("Server returned 500 Error");
							bootbox.alert('Error Deleting (500 error)');
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#sectionEditDlgMsg').htm("Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {
					var name = $('#secTable_cell_linkName_' + whichRow).text();
					$('#secTable_Row_' + whichRow).remove();
					bootbox.alert('Link Section \'<b>' + name + '\'</b> Deleted');
				}

			});

		}


		function doSectionSave() {

			if (!$("#sectionEditForm")[0].checkValidity()) {
				$('#sectionEditDlgMsg').html("There are errors on the form");
				return;
			}

			var ck = $("#enabled_cb").prop("checked");
			$('#enabled').val(ck ? "Y" : "N");

			var formData = JSON.stringify($('#sectionEditForm').serializeObject());
			$('#sectionEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

			console.log("doSectionSave Method=" + Method);

			$.ajax({
				url: '<%= ResolveClientUrl("~/api/UserLinks/Master") %>',
				type: Method,
				data: formData,
				dataType: 'json',
				contentType: "application/json",
				complete: function (xhr, statusText) {
					console.log("doSectionSave xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							$('#sectionEditDlgMsg').html(alertGlyph + "Server returned 204 -- Delete fail");
							break;
						case 404:
							console.log("Server returned 404 -- Service Success but operation fail");
							$('#sectionEditDlgMsg').html(alertGlyph + "Server returned 404 -- Service Success but operation fail");
							break;
						case 500:
							console.log("Server returned 500 Error");
							$('#sectionEditDlgMsg').html(alertGlyph + "Server returned 500 Error");
							bootbox.alert("Exception in apiController (error 500)");
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#sectionEditDlgMsg').htm(alertGlyph + "Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},

				success: function (data) {

					if (Method == "POST") {
						console.log("Success for post");
						$("#secTable_cell_enabled_" + sectionID).html(data.enabled == "Y" ? enabledGlyph : disabledGlyph);
						$("#secTable_cell_linkName_" + sectionID).text(data.sectionText);
						$("#secTable_cell_orderby_" + sectionID).html(data.orderby);

					} else { //ADD
						console.log("Success for PUT");
						var newId = data.sectionid;
						var newEnabled = (data.enabled == "Y") ? enabledGlyph : disabledGlyph;
						var newSectionText = data.sectionText;
						var newOrderBy = data.orderby;
						console.log("New section was added, its id=" + newId + " its orderby is" + newOrderBy);
						var newEditLink = "<a href='#' onclick=\"launchEditSecDlg('POST','" + newId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
						var newDeleteLink = "<a href='#' onclick=\"deleteSecPrompt('" + newId + "','" + newSectionText + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

						var newRow = $("<tr data-sectionText='" + newSectionText + "'data-sectionid='" + newId + "' id='secTable_Row_" + newId + "' class='clickable-row'>" +
							"<td id='secTable_cell_enabled_" + newId + "' class='clickable-cell'>" + newEnabled + "</td>" +
							"<td id='secTable_cell_linkName_" + newId + "' class='clickable-cell'>" + newSectionText + "</td>" +
							"<td id='secTable_cell_orderby_" + newId + "' class='clickable-cell'>" + newOrderBy + "</td>" +
							"<td>" + newEditLink +
							" &nbsp;&nbsp;&nbsp; " +
							newDeleteLink +
							"</td>" +
							"</tr>");

						$('#secTable_Row_AddRec').before(newRow);
						$("#sectionSaveBtn").prop("disabled", true);

					}
					$('#sectionEditDlgMsg').html("Saved Successfully!");
					$("#timestamp").html("Updated: " + data.timestamp);

				}

			});
		}


		function doLinkSave() {

			if (!$("#linkEditForm")[0].checkValidity()) {
				$('#linkEditDlgMsg').html("There are errors on the form");
				return;
			}

			var ck = $("#enabled_cb_l").prop("checked");
			$('#enabled_l').val(ck ? "Y" : "N");

			ck = $("#newwindow_cb_l").prop("checked");
			$('#newwindow_l').val(ck ? "Y" : "N");

			ck = $("#ismenuitem_cb_l").prop("checked");
			$('#ismenuitem_l').val(ck ? "Y" : "N");

			var eput = JSON.stringify($('#linkEditForm').serializeObject());
			$('#linkEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

			var theUrl = '<%= ResolveClientUrl("~/api/UserLinks/User/Link/") %>' + sectionID + "/" + linkID;
			console.log(theUrl);

			$.ajax({
				url: theUrl,
				type: Method,
				data: eput,
				dataType: 'json',
				contentType: "application/json",
				complete: function (xhr, statusText) {
					console.log("doLinkSave section=" + secID + " xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							$('#linkEditDlgMsg').html(alertGlyph + "Server returned 204 -- Delete fail");
							break;
						case 404:
							console.log("Server returned 404 -- Service Success but operation fail");
							$('#linkEditDlgMsg').html(alertGlyph + "Server returned 404 -- Service Success but operation fail");
							break;
						case 500:
							console.log("Server returned 500 Error");
							$('#linkEditDlgMsg').html(alertGlyph + "Server returned 500 Error");
							bootbox.alert("Exception in apiController (error 500)");
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#linkEditDlgMsg').html(alertGlyph + "Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {
					console.log("Save link success: ");
					if (Method == "POST") {
						$("#linksTable_cell_enabled_" + linkID).html(data.enabled == "Y" ? enabledGlyph : disabledGlyph);
						$("#linksTable_cell_linkText_" + linkID).text(data.linkText);
						$("#linksTable_cell_subSectionText_" + linkID).text(data.subSectionText);
						$("#linksTable_cell_orderby_" + linkID).html(data.orderby);

					} else { //ADD
						var newLinkId = data.linkid;
						var newEnabled = (data.enabled == "Y") ? enabledGlyph : disabledGlyph;
						var newLinkText = data.linkText;
						var newLinkURL = data.linkURL;
						var newOrderBy = data.orderby;
						var newSubSectionText = data.subSectionText;
						var newHoverText = data.hoverText;
						var newEditLink = "<a href='#' onclick=\"launchEditLinkDlg('POST','" + newLinkId + "','" + sectionID + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
						var newDeleteLink = "<a href='#' onclick=\"deleteLinkPrompt('" + newLinkId + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";
						var newRow = $("<tr data-linkid='" + newLinkId + "' id='linksTable_Row_" + newLinkId + "' class='clickable-row'>" +
							"<td id='linksTable_cell_enabled_" + newLinkId + "' class='clickable-cell'>" + newEnabled + "</td>" +
							"<td id='linksTable_cell_linkText_" + newLinkId + "' class='clickable-cell'>" + newLinkText + "</td>" +
							"<td id='linksTable_cell_subSectionText_" + newLinkId + "' class='clickable-cell'>" + newSubSectionText + "</td>" +
							"<td id='linksTable_cell_orderby_" + newLinkId + "' class='clickable-cell'>" + newOrderBy + "</td>" +
							"<td>" + newEditLink +
							" &nbsp;&nbsp;&nbsp; " +
							newDeleteLink +
							"</td>" +
							"</tr>");

						$('#linksTable_Row_AddRec').before(newRow);
						$("#linkSaveBtn").prop("disabled", true);
					}

					$("#timestamp_l").html("Updated: " + data.timestamp);
					$('#linkEditDlgMsg').html("Saved Successfully!");
				}

			});
		}


		function launchResequenceSections() {

			$('#seqEditDlgMsg').html("&nbsp;");
			$('#seqEditHeader').html("Re-sequence Sections");
			$("#seq_sectionId").val(-1);

			$("#seqEditDlg").modal('show');

		}

		function launchResequenceLinks(whichId) {

			sectionID = whichId;
			$('#seqEditDlgMsg').html("&nbsp;");
			$('#seqEditHeader').html("Re-seq. Section: " + currSectionText);
			$("#seq_sectionId").val(sectionID);

			$("#seqEditDlg").modal('show');

		}

		function doSeqSave() {

			var eput = JSON.stringify($('#seqEditForm').serializeObject());

			$('#seqEditDlgMsg').html(spinGlyph + " Sending resequence request...");
			$.ajax({
				type: "POST",
				data: eput,
				url: '<%= ResolveClientUrl("~/api/UserLinks/Resequence") %>',
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("doSeqSave section=" + secID + " xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						case 204: // Delete failed
							console.log("Server returned 204 -- Delete fail");
							$('#seqEditDlgMsg').html(alertGlyph + "Server returned 204 -- Delete fail");
							break;
						case 404:
							console.log("Server returned 404 -- Service Success but operation fail");
							$('#seqEditDlgMsg').html(alertGlyph + "Server returned 404 -- Service Success but operation fail");
							break;
						case 500:
							console.log("Server returned 500 Error");
							$('#seqEditDlgMsg').html(alertGlyph + "Server returned 500 Error");
							bootbox.alert("Exception in apiController (error 500)");
							break;
						default:
							console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
							$('#seqEditDlgMsg').html(alertGlyph + "Unknown status code returned from server: " + xhr.status);
							bootbox.alert("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {

					$('#seqEditDlgMsg').html("Resequencing completed successfully.");
					if ($("#seq_sectionId").val() == -1) {
						buildSectionsTable(data);
					} else {
						buildLinksTable(data);
					}
				}

			});

		}

	</script>

	<!-- Hidden sectionEdit  Dialog box ----------------------------------------------------->
	<div id="sectionEditDlg" class="modal fade ">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="sectionEditHeader">Section Edit</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">

					<form name="sectionEditForm" id="sectionEditForm" method="post" style="margin-top: 1px;">
						<input id='sectionid' name='sectionid' type="hidden" value="0" />
						<input id='enabled' name='enabled' type="hidden" value="Y" />

						<div class="row">
							<div class="col-md-12">
								<label class="form-check-label" for="enabled">Enabled</label>
								<input class="form-check-input" type="checkbox" id='enabled_cb' name='enabled_cb' />
							</div>
						</div>

						<div class="row">
							<label class="col-sm-1" for="orderby">Order</label>
							<div class="col-sm-2">
								<input type="text" id='orderby' name='orderby' class="form-control" style="min-width: 100%" maxlength="5" required />
							</div>

							<label class="col-sm-1" for="sectionText">Name</label>
							<div class="col-sm-8">
								<input type="text" id='sectionText' name='sectionText' class="form-control" style="min-width: 100%" maxlength="128" placeholder="Section Heading" required />
							</div>
						</div>

					</form>
					<span id="timestamp"></span>
					<div id="sectionEditDlgMsg" class="text-warning" style="font-size: 20px;">&nbsp;</div>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary btn-s" id="sectionSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
					<button type="button" class="btn btn-warning btn-s" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
				</div>
				&nbsp;
			</div>
		</div>

	</div>
	<!-- END Hidden sectionEdit Dialog box -------------------------------------------------->

	<!-- Hidden LinksEdit  Dialog box ----------------------------------------------------->
	<div id="linkEditDlg" class="modal fade">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="linkEditHeader">Link Edit</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">

					<form name="linkEditForm" id="linkEditForm" method="post" style="margin-top: 1px;">
						<input id='linkid' name='linkid' type="hidden" value="0" />
						<input id='sectionid_l' name='sectionid' type="hidden" value="0" />
						<input id='enabled_l' name='enabled' type="hidden" value="Y" />
						<input id='newwindow_l' name='newwindow' type="hidden" value="N" />
						<input id='ismenuitem_l' name='ismenuitem' type="hidden" value="N" />

						<div class="row">
							<div class="col-md-12">

								<label class="form-check-label" for="enabled_cb_l">Enabled</label>
								<input class="form-check-input" type="checkbox" id='enabled_cb_l' name='enabled_cb_l' />

								<label class="form-check-label" for="newwindow_cb_l">New Window</label>
								<input class="form-check-input" type="checkbox" id='newwindow_cb_l' name='newwindow_cb_l' />

								<label class="form-check-label" for="ismenuitem_cb_l">Menu Item</label>
								<input class="form-check-input" type="checkbox" id='ismenuitem_cb_l' name='ismenuitem_cb_l' />

							</div>
						</div>

						<div class="row">
							<label class="col-sm-1" for="orderby">Order</label>
							<div class="col-sm-2">
								<input type="text" id='orderby_l' name='orderby' class="form-control" style="min-width: 100%" maxlength="5" required />
							</div>

							<label class="col-sm-1" for="sectionText">Link Text</label>
							<div class="col-sm-8">
								<input type="text" id='linkText' name='linkText' class="form-control" style="min-width: 100%" maxlength="255" placeholder="Link Text" required />
							</div>
						</div>

						<div class="row">
							<label class="col-sm-12" for="linkURL">Link URL</label>
							<div class="col-sm-12">
								<input type="text" id='linkURL' name='linkURL' class="form-control" style="min-width: 100%" maxlength="255" placeholder="http(s)://..." />
							</div>
						</div>

						<div class="row">
							<label class="col-sm-4" for="linkURL">Subsection Text</label>
							<div class="col-sm-8">
								<input type="text" id='subSectionText' name='subSectionText' class="form-control" style="min-width: 100%" maxlength="255" placeholder="List under Subsection" />
							</div>
						</div>

						<div class="row">
							<label class="col-sm-4" for="linkURL">Hover Text</label>
							<div class="col-sm-8">
								<input type="text" id='hoverText' name='hoverText' class="form-control" style="min-width: 100%" maxlength="255" placeholder="tool-tip text here" />
							</div>
						</div>

					</form>
					<span id="timestamp_l"></span>
					<div id="linkEditDlgMsg" class="text-warning" style="font-size: 20px;">&nbsp;</div>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary btn-s" id="linkSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
					<button type="button" class="btn btn-warning btn-s" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
				</div>
				&nbsp;
			</div>
		</div>

	</div>
	<!-- END Hidden linkEdit Dialog box -------------------------------------------------->

	<!-- Hidden Sequence Edit  Dialog box ----------------------------------------------------->
	<div id="seqEditDlg" class="modal fade">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="seqEditHeader">Re-Sequence</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">

					<form name="seqEditForm" id="seqEditForm" method="post" style="margin-top: 1px;">

						<input id='seq_sectionId' name='seq_sectionId' type="hidden" value="0" />

						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="seq_initialNum">Initial</label>
								<div class="col-sm-8">
									<input type="text" id='seq_initialNum' name='seq_initialNum' class="form-control" style="min-width: 100%" maxlength="4" value="10" required />
								</div>
							</div>
						</div>

						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="seq_incrementBy">Increment</label>
								<div class="col-sm-8">
									<input type="text" id='seq_incrementBy' name='seq_incrementBy' class="form-control" style="min-width: 100%" maxlength="4" value="10" required />
								</div>
							</div>
						</div>

					</form>

					<div id="seqEditDlgMsg" class="text-warning" style="font-size: 20px;">&nbsp;</div>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary btn-s" id="seqSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
					<button type="button" class="btn btn-warning btn-s" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
				</div>
				&nbsp;
			</div>
		</div>

	</div>
	<!-- END Hidden Sequence Edit  Dialog box ----------------------------------------------------->


</asp:Content>
