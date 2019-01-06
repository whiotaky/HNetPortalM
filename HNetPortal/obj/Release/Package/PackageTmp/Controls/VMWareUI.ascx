<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VMWareUI.ascx.cs" Inherits="HNetPortal.Controls.VMWareUI" %>

<!--
    Used by HNetPortal-M private mainpage, and VMTool aspx page.  
    Assumes that bootstrap css and jquery scripts are loaded.
-->

<!-- Hidden VMTool Dialog box ----------------------------------------------------->
<div id="vmToolDlg" class="modal fade">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header alert alert-info" style="margin-bottom: 1px;">				
				<h4 class="modal-title" id="vmToolDlgHeader">VM State Changer</h4>
			</div>
			<div class="modal-body" style="margin-top: 1px;">

				<div class="row" style="margin-bottom: 1px;">
					<div class="col-xs-4 text-right">
						<h4>Name</h4>
					</div>
					<div class="col-xs-7 col-sm-4 text-center" style="background-color: black; color: white" id="vmToolVMNameContainer">
						<h4 id='vmToolVMName'></h4>
					</div>
				</div>
				<div class="row" style="margin-bottom: 1px;">
					<h4 class="text-center" id="vmToolMsg" style="min-height: 20px"></h4>
				</div>
				<div class="row" style="margin-bottom: 1px;">
					<h4 class="text-center">
						<button class="btn btn-primary" name="vmToolBtn" id="vmToolBtn"></button>
						&nbsp;
                            <button type="button" data-dismiss="modal" aria-hidden="true" class="btn btn-primary" name="vmToolCloseBtn" id="vmToolCloseBtn">Close</button>
					</h4>
				</div>
			</div>
		</div>
	</div>
</div>
<!-- END Hidden VMTool Dialog box ----------------------------------------------------->

<script>

	var getVMStateUrl = "<%= ResolveClientUrl("~/api/VMWare/GetState") %>";
	var setVMStateUrl = "<%= ResolveClientUrl("~/api/VMWare/SetState") %>";
	var vmToolSpinner = '<i class="fa fa-spin fa-spinner"></i>&nbsp;';
	var currentVM = "";
	var vmState = "";

	function launchVMDlg(whichVM) {

		currentVM = whichVM;
		$('#vmToolVMNameContainer').attr("style", "background-color: black; color: white");
		$('#vmToolMsg').html(vmToolSpinner + "Checking with the ESXI server ...");
		$('#vmToolVMName').html(whichVM);

		jQuery("#vmToolBtn").show();
		$('#vmToolBtn').text("Wait...");
		$("#vmToolBtn").prop("class", "btn btn-primary");
		$("#vmToolBtn").prop("disabled", true);
		$("#vmToolCloseBtn").prop("disabled", true);

		$('#vmToolDlg').modal({ backdrop: 'static', keyboard: false });
		$("#vmToolDlg").modal('show');

		$.ajax({
			url: encodeURI(getVMStateUrl + '/' + whichVM +'/'), //trailing slash helps with unconventionally named VM's  
			type: "GET",	
			dataType: 'json',
			contentType: "application/json",
			complete: function (xhr, statusText) {
				console.log("GetVMState xhr.status=" + xhr.status + ", statusText=" + statusText);
				switch (xhr.status) {
					case 200: // success
						console.log("returned 200 -- Success");
						break;
					case 204:
						console.log("Failed to connect to ESXI server (204 error)");
						$('#vmToolMsg').html("Failed to connect to ESXI server");
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
						break;
					case 500:
						console.log("returned 500 Error");
						$('#vmToolMsg').html("Exception in api VMWare Controller (error 500)");
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
						break;
					default:
						console.log("returned " + xhr.status + "text: " + xhr.statusText);
						$('#vmToolMsg').html("Unknown status code returned from server: " + xhr.status);
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
				}
			},

			success: function (data) {

				console.log("Success getting vmState");

				vmState = data;
				console.log("VMState is  " + vmState);

				$('#vmToolMsg').html("State: " + vmState);
				$("#vmToolBtn").prop("disabled", false);
				$("#vmToolCloseBtn").prop("disabled", false);

				if (vmState == 'PoweredOn') {
					$('#vmToolMsg').html("Powered ON. Okay to turn off.");
					$('#vmToolVMNameContainer').attr("style", "background-color: Green; color: white");
					$('#vmToolBtn').text("Power Off");
					$("#vmToolBtn").prop("class", "btn btn-danger");
					$("#vmToolCloseBtn").prop("disabled", false);

				} else if (vmState == 'PoweredOff') {
					$('#vmToolMsg').html("Powered OFF. Okay to turn on.");
					$('#vmToolVMNameContainer').attr("style", "background-color: Red; color: white");
					$('#vmToolBtn').text("Power On");
					$("#vmToolBtn").prop("class", "btn btn-success");
					$("#vmToolCloseBtn").prop("disabled", false);

				} else if (vmState == 'NotFound') {
					$('#vmToolMsg').html("No VM found by that Name");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);
				} else {
					$('#vmToolMsg').html("No actions supported, VM state is '" + vmState + "'");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);
				}
			},

		});

	}


	$("#vmToolBtn").click(function () {

		$('#vmToolVMNameContainer').attr("style", "background-color: DarkBlue; color: white");
		$('#vmToolMsg').html(vmToolSpinner + "Communicating with the ESXI server ...");

		$("#vmToolBtn").prop("disabled", true);
		$("#vmToolCloseBtn").prop("disabled", true);

		var setToState = "Unknown";
		console.log("before: " + setToState);
		if (vmState == 'PoweredOn') {
			setToState = "Shutdown";
		} else if (vmState == 'PoweredOff') {
			setToState = "PowerOn";
		}
		console.log("setToState: " + setToState);
		
		var url = encodeURI(setVMStateUrl + '/' + currentVM + "/" + setToState);
		console.log("URL: "+url)
		$.ajax({
			url: url, 
			type: "GET",
			dataType: 'json',
			contentType: "application/json",
			complete: function (xhr, statusText) {
				console.log("SetVMState xhr.status=" + xhr.status + ", statusText=" + statusText);
				switch (xhr.status) {
					case 200: // success
						console.log("returned 200 -- Success");
						break;
					case 204:
						console.log("Failed to connect to ESXI server (204 error)");
						$('#vmToolMsg').html("Failed to connect to ESXI server");
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
						break;
					case 500:
						console.log("returned 500 Error");
						$('#vmToolMsg').html("Exception in api VMWare Controller (error 500)");
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
						break;
					default:
						console.log("returned " + xhr.status + "text: " + xhr.statusText);
						$('#vmToolMsg').html("Unknown status code returned from server: " + xhr.status);
						jQuery("#vmToolBtn").hide();
						$("#vmToolCloseBtn").prop("disabled", false);
				}
			},
			success: function (data) {

				console.log("Success calling SetState");
				vmState = data;
				console.log("VMState is" + vmState);

				$('#vmToolMsg').html("State: " + vmState);
				$("#vmToolBtn").prop("disabled", false);
				$("#vmToolCloseBtn").prop("disabled", false);

				if (vmState == 'PoweringOn') {
					$('#vmToolMsg').html("Its Powering ON now.  Wait a few minutes.");
					$('#vmToolVMNameContainer').attr("style", "background-color: Green; color: white");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);

					if ($('#vmwareTool_' + currentVM).length) {
						$('#vmwareTool_' + currentVM).attr("style", "background-color: green; color: white;padding-left: 0px; padding-right: 0px; margin-bottom: 3px;");
					}

				} else if (vmState == 'PoweringOff') {
					$('#vmToolMsg').html("It's powering OFF now. Wait a few minutes.");
					$('#vmToolVMNameContainer').attr("style", "background-color: Red; color: white");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);

					if ($('#vmwareTool_' + currentVM).length) {
						$('#vmwareTool_' + currentVM).attr("style", "background-color: red;  color: white;padding-left: 0px; padding-right: 0px; margin-bottom: 3px;");
					}

				} else if (vmState == 'NotFound') {
					$('#vmToolVMNameContainer').attr("style", "background-color: black; color: white");
					$('#vmToolMsg').html("No VM found by that Name");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);

				} else {
					$('#vmToolVMNameContainer').attr("style", "background-color: black; color: white");
					$('#vmToolMsg').html("Action not supported. VM state is '" + vmState + "'");
					jQuery("#vmToolBtn").hide();
					$("#vmToolCloseBtn").prop("disabled", false);
				}				
			}
		});

	});

</script>

