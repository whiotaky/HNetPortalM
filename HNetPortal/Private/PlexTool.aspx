<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="PlexTool.aspx.cs" Inherits="HNetPortal.Private.PlexTool" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
	<link href="<%=ResolveClientUrl("~/Content/hnet_checkboxes.css") %>" rel="stylesheet" />
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">


	<div class="row ">
		<div class="hidden-xs hidden-sm  hidden-md col-lg-12">
			<h1><span class="glyphicon glyphicon-picture"></span>&nbsp; The HNet Portal - Plex Tool</h1>
		</div>


		<div class="hidden-xs hidden-sm  col-md-12 hidden-lg">
			<h2><span class="glyphicon glyphicon-picture"></span>&nbsp; The HNet Portal - Plex Tool</h2>
		</div>


		<div class="hidden-xs col-sm-12  hidden-md hidden-lg">
			<h3><span class="glyphicon glyphicon-picture"></span>&nbsp; The HNet Portal - Plex Tool</h3>
		</div>

		<div class="col-xs-12 hidden-sm  hidden-md hidden-lg">
			<h5><span class="glyphicon glyphicon-picture"></span>&nbsp; The HNet Portal - Plex Tool</h5>
		</div>
	</div>


	<asp:XmlDataSource ID="plexPathsDS" runat="server" XPath="/HNetPortal/plexPath[isHome='FALSE' and @enabled='TRUE']" DataFile="~/App_Data/PlexPaths.xml" />

	<asp:Repeater ID="Repeater1" runat="server" DataSourceID="plexPathsDS">
		<HeaderTemplate>
			<div class="panel panel-primary">

				<div class="panel-heading ">
					<h3 class="panel-title"><span class="glyphicon glyphicon-th-list"></span>&nbsp;&nbsp;&nbsp;Selections</h3>
				</div>

				<div class="panel-body">
					<div class='table-responsive'>
						<table id='table1' class="table table-hover">
							<thead>
								<tr>
									<th>Num Files</th>
									<th>Folder</th>
								</tr>
							</thead>
							<tbody>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td>
					<input type="number" min="0" max="9999" class="form-control form-control-lg" name="plexID_<%# XPath("@ID")%>" id="plexID_<%# XPath("@ID")%>" data-plexid="<%# XPath("@ID")%>" value="" style="max-width: 75px" />
				</td>
				<td>
					<%# XPath("Name")%>
				</td>

			</tr>
		</ItemTemplate>

		<FooterTemplate>
			<tr>
				<td>
					<button name="submitBtn" id="submitBtn" class="btn btn-primary  btn-block">Copy</button><br />
					<button name="restoreHomeBtn" id="restoreHomeBtn" class="btn btn-primary btn-block">Restore Home</button><br />
					<button name="plexSectionRefreshBtn" id="plexSectionRefreshBtn" class="btn btn-primary btn-block">Plex Refresh</button><br />
				</td>
				<td>
					<div class="checkbox checkbox-primary">
						<asp:CheckBox runat="server" ID="plex_clearDestCB" Checked="true" ClientIDMode="Predictable" />
						<asp:Label ID="Label3" runat="server" AssociatedControlID="plex_clearDestCB">Clear destination first</asp:Label>
					</div>

					<div class="form-check">
						<input class="form-check-input" type="radio" id="plex_newest" name="selOption" value="newest" checked>
						<label class="form-check-label" for="plex_newest">Select Newest</label>
					</div>
					<div class="form-check">
						<input class="form-check-input" type="radio" id="plex_random" name="selOption" value="random">
						<label class="form-check-label" for="plex_random">Select Randomly</label>
					</div>

				</td>
			</tr>
			<tr>
				<td colspan="2">
					<div id="progress" class="progress progress-striped active" style="visibility: hidden">
						<div id="progressMsg" class="progress-bar" style="width: 100%;">Working</div>
					</div>
					<span id="msg"></span>
				</td>
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

		//https://stackoverflow.com/questions/25408666/how-do-i-wait-for-multiple-ajax-calls-to-complete-from-a-each-loop
		//https://stackoverflow.com/questions/38139221/running-ajax-request-inside-each-loop-one-after-another

		
		var taskArray = new Array();

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

		});


		$("#submitBtn").click(function () {

			var clearDestFlag = $('#plex_clearDestCB').is(":checked");
			taskArray = new Array();

			$('#form1 *').find("input[name^='plexID_']").each(function () {
				var plexPathID = $(this).data('plexid');
				var fileCount = this.value;
				var whichRbOption = $('input[name=selOption]:checked').val();
				console.log("SUBMIT WhichRb=" + whichRbOption);


				if (fileCount > 0) {
					var sendDat = '{"clearDestFirst": "' + clearDestFlag +
						'","plexPathID": "' + plexPathID +
						'","fileCount": "' + fileCount +
						'","selOption": "' + whichRbOption +
						'"}';
					taskArray.push(sendDat);
					clearDestFlag = false;
				}
			});

			if (taskArray.length > 0) {
				doAjaxTasks(0);
			} else {
				$('#msg').html("No non-zero tasks to copy");
			}

			return false;
		});


		$("#restoreHomeBtn").click(function () {

			taskArray = new Array();
			taskArray.push('{"clearDestFirst": "true","plexPathID": "HOME","fileCount": "0","selOption":""}');

			doAjaxTasks(0);
			return false;
		});


		function doAjaxTasks(taskIdx) {

			var counter = taskIdx + 1;
			var errors = false;

			$("#submitBtn").prop("disabled", true);
			$("#restoreHomeBtn").prop("disabled", true);
			$("#plexSectionRefreshBtn").prop("disabled", true);
			$("#progress").css("visibility", "visible");
			$('#progressMsg').html("Working on Task #" + counter);
			$('#msg').html("Working on Task #" + counter);

			var taskJSON = taskArray[taskIdx];
			console.log(taskJSON);
			$.ajax({
				type: "POST",
				contentType: "application/json",
				data: taskJSON,
				url: '<%= ResolveClientUrl("~/api/plex") %>',
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("POST api/Plex xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						default:
							$("#progress").css("visibility", "hidden");
							$("#submitBtn").prop("disabled", false);
							$("#restoreHomeBtn").prop("disabled", false);
							$("#plexSectionRefreshBtn").prop("disabled", false);
							$('#msg').html("Error: " + xhr.status);
					}
				},
				success: function (data) {
					console.log("Success caught: Plex POST returned " + data);
					$("#progress").css("visibility", "hidden");
					$('#msg').html('task ' + counter + " is done");
				
					if (data.indexOf("Success") >= 0) {
						$('#msg').html('task ' + counter + "Success: " + data);
					} else {
						$('#msg').html('task ' + counter + "NOT Success: " + data);
						bootbox.alert('task ' + counter + " failure: " + data);
						errors = true;
					}

					taskIdx++;
					if (taskIdx < taskArray.length) {
						doAjaxTasks(taskIdx);
					} else {
						$("#submitBtn").prop("disabled", false);
						$("#restoreHomeBtn").prop("disabled", false);
						$("#plexSectionRefreshBtn").prop("disabled", false);
						if (!errors)
							$('#msg').html("Task list completed");
					}

				}

			});
		}

		$("#plexSectionRefreshBtn").click(function () {

			$("#submitBtn").prop("disabled", true);
			$("#restoreHomeBtn").prop("disabled", true);
			$("#plexSectionRefreshBtn").prop("disabled", true);
			$("#progress").css("visibility", "visible");
			$('#progressMsg').html("Sending Plex Refresh signal...");
			$('#msg').html("");

			$.ajax({
				type: "GET",
				contentType: "application/json",
				//data: '{"sectionNum": "' +  <%=plexSec%>+'"}',
				url: '<%= ResolveClientUrl("~/api/plex/") %>' + <%=plexSec%>,
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("GET api/Plex xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log("Server returned 200 -- Success");
							break;
						default:
							$("#progress").css("visibility", "hidden");
							$("#submitBtn").prop("disabled", false);
							$("#restoreHomeBtn").prop("disabled", false);
							$("#plexSectionRefreshBtn").prop("disabled", false);
							$('#msg').html("Error: " + xhr.status);
					}
				},

				success: function (data) {

					console.log("Refresh success. Data: " + data);

					if (data.indexOf("Success") >= 0) {
						$('#msg').html("Plex Refresh Success");
					} else {
						$('#msg').html("Plex Refresh NOT Success: " + data);
					}

					$("#submitBtn").prop("disabled", false);
					$("#restoreHomeBtn").prop("disabled", false);
					$("#plexSectionRefreshBtn").prop("disabled", false);
					$("#progress").css("visibility", "hidden");

				}

			});

			return false;

		});

	</script>

</asp:Content>
