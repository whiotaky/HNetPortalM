<%@ Page Title="Feed Master Edit" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="FeedMasterEdit.aspx.cs" Inherits="HNetPortal.Private.FeedMasterEdit" %>

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

	<h1><span class="glyphicon glyphicon-share-alt"></span>&nbsp; The HNet Portal - Feed Master Edit</h1>

	<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="" ProviderName="<%$ ConnectionStrings:MySqlConnString.ProviderName %>" SelectCommand="SELECT feedid, feedName, feedType, feedURL, cacheFilePrefix, enabled FROM feedsmaster"></asp:SqlDataSource>
	<asp:Repeater ID="Repeater1" runat="server" DataSourceID="SqlDataSource1">

		<HeaderTemplate>
			<div class="panel panel-primary">

				<div class="panel-heading ">
					<h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;HNet Portal Feeds Master Edit</h3>
				</div>

				<div class="panel-body">
					<div class='table-responsive'>
						<table id='FMTable' class="table table-hover">
							<thead>
								<tr>
									<th>Enabled</th>
									<th>Type</th>
									<th>Name</th>
									<th>Cache</th>
									<th>Action</th>
								</tr>
							</thead>
							<tbody>
		</HeaderTemplate>

		<ItemTemplate>
			<tr id="row_<%# Eval("feedid") %>">
				<td id="cell_enabled_<%# Eval("feedid") %>"><%# (string) Eval("enabled")=="Y" ? "<span class='new glyphicon glyphicon-ok'></span>" :"<span class='trash glyphicon glyphicon-remove'></span>" %></td>
				<td id="cell_feedType_<%# Eval("feedid") %>"><%# (byte)Eval("feedType")==1 ? "News" : "Career" %></td>
				<td id="cell_feedName_<%# Eval("feedid") %>"><%# Eval("feedName") %></td>
				<td id="cell_cacheFilePrefix_<%# Eval("feedid") %>"><%# Eval("cacheFilePrefix") %>.cache</td>
				<td>
					<a href='#' onclick="launchEditDlg('POST','<%# Eval("feedid") %>');return false;"><span class="edit glyphicon glyphicon-pencil"></span></a>
					&nbsp;&nbsp;&nbsp;
                                    <a href='#' onclick="deleteFMPrompt('<%# Eval("feedid") %>');return false;"><span class="trash glyphicon glyphicon-trash"></span></a>
				</td>
			</tr>
		</ItemTemplate>

		<FooterTemplate>
			<tr id="row_last">
				<td colspan="3"></td>
				<td><b>Add New Feed</b></td>
				<td><a href='#' onclick="launchEditDlg('PUT','0');return false; "><span class="new glyphicon glyphicon-plus"></span></a></td>
			</tr>

			<tr>
				<td colspan="5"></td>
			</tr>

			</tbody>
                        </table>
                    </div>
                </div>
            </div>
		</FooterTemplate>

	</asp:Repeater>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

	<script>
       
		var apiUrl = "<%= ResolveClientUrl("~/api/RSSMaster") %>";

		var spinGlyph = "<span class='glyphicon glyphicon-cog glyphicon-spin' style='margin-right:8px;'></span>";
		var alertGlyph = "<span class='glyphicon glyphicon-alert' style='margin-right:8px;'></span>";
		var enabledGlyph = "<span class='new glyphicon glyphicon-ok'></span>";
		var disabledGlyph = "<span class='trash glyphicon glyphicon-remove'></span>";

		var Method = "";
		var feedID = 0;

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

			$("#feedSaveBtn").click(function () { doFeedSave(); });

		});

		function launchEditDlg(method, which) {

			Method = method;
			feedID = which;

			$("#feedid").val(feedID);
			$("#enabled_cb").prop("checked", true);
			$("#enabled").val("Y");
			$("#feedName").val("");
			$("#feedType").val("1");
			$("#feedURL").val("");
			$("#cacheFilePrefix").val("");
			$("#feedSaveBtn").prop("disabled", false);
			$('#feedEditDlgMsg').html("");

			if (method == "PUT") {				
				$("#feedEditHeader").text("Add NEW RSS Feed");
			} else {
				$("#feedEditHeader").text("Feed Edit");
				$('#feedEditDlgMsg').html(spinGlyph + " Fetching Section from server...");
				console.log("Getting feedid=" + feedID);

				$.ajax({
					url: apiUrl + "/" + feedID,
					contentType: "application/json",
					dataType: "json",
					complete: function (xhr, statusText) {
						console.log("GetFeed xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // success
								console.log("returned 200 -- Success");
								break;
							case 500:
								console.log("returned 500 Error");
								$('#feedEditDlgMsg').html("Exception in api RSS Controller (error 500)");

								break;
							default:
								console.log("returned " + xhr.status + "text: " + xhr.statusText);
								$('#feedEditDlgMsg').html("Unknown status code returned from server: " + xhr.status);
						}
					},
					success: function (data) {
						console.log("Success getting feed master record");
						$("#feedName").val(data.feedName);
						$("#feedType").val(data.feedType);
						$("#feedURL").val(data.feedURL);
						$("#cacheFilePrefix").val(data.cacheFilePrefix);
						$("#enabled_cb").prop("checked", data.enabled == "Y" ? true : false);
						$("#enabled").prop(data.enabled);
						$('#feedEditDlgMsg').html("&nbsp;");
					},
				});

			}

			$("#feedEditDlg").modal('show');
		};


		function doFeedSave() {
			console.log("dosaveFeed method=" + Method);
			if (!$("#feedEditForm")[0].checkValidity()) {
				$('#feedEditDlgMsg').html("There are errors on the form");
				return;
			}

			var ck = $("#enabled_cb").prop("checked");
			$('#enabled').val(ck ? "Y" : "N");

			var eput = JSON.stringify($('#feedEditForm').serializeObject());
			$('#feedEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

			$.ajax({
				url: apiUrl,
				type: Method,
				data: eput,
				dataType: 'json',
				contentType: "application/json",
				complete: function (xhr, statusText) {
					console.log("GetFeed xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("returned 200 -- Success");
							break;
						case 500:
							console.log("returned 500 Error");
							$('#feedEditDlgMsg').html("Exception in api RSS Controller (error 500)");
							break;
						default:
							console.log("returned " + xhr.status + "text: " + xhr.statusText);
							$('#feedEditDlgMsg').html("Unknown status code returned from server: " + xhr.status);
					}
				},
				success: function (data) {

					if (Method == "POST") {
						$("#cell_enabled_" + feedID).html(data.enabled == "Y" ? enabledGlyph : disabledGlyph);
						$("#cell_feedName_" + feedID).text(data.feedName);
						$("#cell_feedType_" + feedID).text(data.feedType == "1" ? "News" : "Career");
						$("#cell_cacheFilePrefix_" + feedID).text(data.cacheFilePrefix + ".cache");
					} else { //ADD (PUT)
						var newId = data.feedid;
						var newEnabled = (data.enabled == "Y") ? enabledGlyph : disabledGlyph;
						var newFeedType = (data.feedType == "1") ? "News" : "Career";
						var newEditLink = "<a href='#' onclick=\"launchEditDlg('POST','" + newId + "');return false;\"><span class='edit glyphicon glyphicon-pencil'></span></a>";
						var newDeleteLink = "<a href='#' onclick=\"deleteFMPrompt('" + newId + "');return false;\"><span class='trash glyphicon glyphicon-trash'></span></a>";

						var newrow = $("<tr id='row_" + newId + "'>" +
							"<td id='cell_enabled_" + newId + "'>" + newEnabled + "</td>" +
							"<td id='cell_feedType_" + newId + "'>" + newFeedType + "</td>" +
							"<td id='cell_feedName_" + newId + "'>" + data.feedName + "</td>" +
							"<td id='cell_cacheFilePrefix_" + newId + "'>" + data.cacheFilePrefix + ".cache</td>" +
							"<td>" + newEditLink +
							" &nbsp;&nbsp;&nbsp; " +
							newDeleteLink +
							"</td>" +
							"</tr>");
						$('#row_last').before(newrow);
						$("#feedSaveBtn").prop("disabled", true);
					}
					$('#feedEditDlgMsg').html("Saved Successfully!");

				},
			});

		};

		function deleteFMPrompt(whichRow) {

			var name = $('#cell_feedName_' + whichRow).text();

			bootbox.confirm({
				title: "Delete Feeds Master Record?",
				message: "Please confirm deletion of Feed Master: <b>" + name + "</b>",
				buttons: {
					cancel: {
						label: '<i class="fa fa-times"></i> Cancel'
					},
					confirm: {
						label: '<i class="fa fa-check"></i> Confirm'
					}
				},
				callback: function (result) {
					doDeleteFM(result, whichRow);
				}
			});

		};


		function doDeleteFM(result, whichRow) {

			if (!result) {
				return;
			}

			$.ajax({
				type: "DELETE",
				url: apiUrl + "/" + whichRow,
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("DeleteFeed xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("returned 200 -- Success");
							break;
						case 204: // record not found
							console.log("returned 204 -- Not found");
							bootbox.alert('Error:  No record found on server to delete.');
							break;
						case 500: //exception
							console.log("returned 500 Error");
							$('#feedEditDlgMsg').html("Exception in api RSS Controller (error 500)");
							bootbox.alert('Error:  (Exception in api RSS Controller (error 500))');
							break;
						default:
							console.log("returned " + xhr.status + "text: " + xhr.statusText);
							$('#feedEditDlgMsg').html("Unknown status code returned from server: " + xhr.status);
							bootbox.alert('Error?  (Server did not return a status code.)');
					}
				},
				success: function (data) {
					var name = $('#cell_feedName_' + whichRow).text();
					$('#row_' + whichRow).remove();
					bootbox.alert('Feed Master \'<b>' + name + '\'</b> Deleted');
				},			
			});

		}

	</script>


	<!-- Hidden feedEdit  Dialog box ----------------------------------------------------->
	<div id="feedEditDlg" class="modal fade ">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="feedEditHeader">Feed Edit</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">

					<form name="feedEditForm" id="feedEditForm" method="post" style="margin-top: 1px;">
						<input id='feedid' name='feedid' type="hidden" value="0" />
						<input id='enabled' name='enabled' type="hidden" value="Y" />

						<div class="row">
							<div class="col-md-12">
								<label class="form-check-label" for="enabled">Enabled</label>
								<input class="form-check-input" type="checkbox" id='enabled_cb' name='enabled_cb' />
							</div>
						</div>

						<div class="row">
							<label class="col-sm-1" for="feedName">Name</label>
							<div class="col-sm-6">
								<input type="text" id='feedName' name='feedName' class="form-control" style="min-width: 100%" maxlength="128" placeholder="Feed Name" required />
							</div>

							<label class="col-sm-1" for="feedType">Type</label>
							<div class="col-sm-4">
								<select id="feedType" name="feedType" class="form-control">
									<option value="1">News</option>
									<option value="2">Career</option>
								</select>
							</div>
						</div>


						<div class="row">
							<label class="col-sm-1" for="feedURL">URL</label>
							<div class="col-sm-7">
								<input type="text" id='feedURL' name='feedURL' class="form-control" style="min-width: 100%" maxlength="128" placeholder="RSS Feed URL" required />
							</div>

							<label class="col-sm-1" for="cacheFilePrefix">Cache Prefix</label>
							<div class="col-sm-3">
								<input type="text" id='cacheFilePrefix' name='cacheFilePrefix' class="form-control" style="min-width: 100%" maxlength="128" placeholder="CacheFilePrefix" required />
							</div>
						</div>

					</form>
					<span id="feedEditDlgMsg" class="text-warning" style="font-size: 20px;">&nbsp;</span>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary btn-s" id="feedSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
					<button type="button" class="btn btn-warning btn-s" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
				</div>
				&nbsp;
			</div>
		</div>

	</div>
	<!-- END Hidden feedEdit Dialog box -------------------------------------------------->


</asp:Content>
