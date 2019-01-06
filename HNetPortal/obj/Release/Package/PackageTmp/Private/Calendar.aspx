<%@ Page Title="HNet Calendar" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Calendar.aspx.cs" Inherits="HNetPortal.Private.Calendar" %>

<%@ Register Src="~/Controls/CalEditDialog.ascx" TagName="CalEditDialog" TagPrefix="ucl" %>
<%@ Register Src="~/Controls/CalSearchDialog.ascx" TagName="CalSearchDialog" TagPrefix="ucl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/scripts/fullcalendar/fullcalendar.css") %>" rel="stylesheet" />
	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
	<style>
		.fc h2 {
			font-size: 20px;
		}
	</style>
	<script src="<%=ResolveClientUrl("~/scripts/fullcalendar/moment.min.js")%>"></script>
	<script src="<%=ResolveClientUrl("~/scripts/fullcalendar/fullcalendar.js")%>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<!-- button onclick="$('#calendar').fullCalendar('render');">Render</!-->
	<div class="container row">

		<div class="row ">
			<div class="hidden-xs hidden-sm  hidden-md col-lg-12">
				<h1><span class="glyphicon glyphicon-calendar"></span>&nbsp; The HNet Portal - Calendar</h1>
			</div>


			<div class="hidden-xs hidden-sm  col-md-12 hidden-lg">
				<h2><span class="glyphicon glyphicon-calendar"></span>&nbsp; The HNet Portal - Calendar</h2>
			</div>


			<div class="hidden-xs col-sm-12  hidden-md hidden-lg">
				<h3><span class="glyphicon glyphicon-calendar"></span>&nbsp; The HNet Portal - Calendar</h3>
			</div>

			<div class="col-xs-12 hidden-sm  hidden-md hidden-lg">
				<h5><span class="glyphicon glyphicon-calendar"></span>&nbsp; The HNet Portal - Calendar</h5>
			</div>

		</div>

	</div>

	<div id='calendar'></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
	<ucl:CalEditDialog runat="server" ID="CalEditDialog" />
	<ucl:CalSearchDialog runat="server" ID="CalSearchDialog" />

	<script>
		$(document).ready(function () {


			var getCalEventsURL = "<%= ResolveClientUrl("~/api/Calendar/GetEvents/FullCalendar") %>";
			var editCalEventURL = "<%= ResolveClientUrl("~/api/Calendar/EditEvent") %>";
			var deleteCalEventURL = "<%= ResolveClientUrl("~/api/Calendar/DeleteEvent") %>";
			var spinGlyph = "<span class='glyphicon glyphicon-cog glyphicon-spin' style='margin-right:8px;'></span>";
			var alertGlyph = "<span class='glyphicon glyphicon-alert' style='margin-right:8px;'></span>";

			var selEvent = null;
			var selDate = null;

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

			$('#calendar').fullCalendar({

				defaultDate: moment('<%=initialDate%>'),

				customButtons: {
					search: {
						icon: 'fa fa fa-search',
						click: function () {
							$("#calSearchDlg").modal('show');
						}
					}
				},

				header: {
					center: 'title, search',
					left: 'prevYear, prev',
					right: 'next, nextYear',
				},

				dayClick: function (date, jsEvent, view) {
					console.log("dayClick: selDate is " + date);
					selDate = date;
					selEvent = null;
					launchDlg();
				},

				eventClick: function (calEvent, jsEvent, view) {
					console.log("eventClick: selDate is " + calEvent.start);
					selDate = calEvent.start;
					selEvent = calEvent;
					launchDlg();
				},

				loading: function (isLoading) {
					//if (!isLoading)
					//	alert("Done?");
				},

				events: {
					// your event source

					url: getCalEventsURL,
					type: 'GET',
					contentType: "application/json",
					dataType: "json",
					cache: false,

					success: function (result) {
						//FIXME, dont know how to handle when server returns our status: Error JSON
						// alert(result);
					},

					error: function (x, y, z) {
						alert("Error " + x + ' ' + y + ' ' + z);
						//debugger;
					},

					//color: 'white',   // a non-ajax option
					//textColor: 'black' // a non-ajax option
				}
			});


			$("#calSearchSubmitBtn").click(function () { doCalSearch(); });

			function doCalSearch() {

				$("#calSearchResults").empty();
				var searchText = $("#calSearchText").val();

				$('#calEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

				$.ajax({
					data: '{"searchText": "' + searchText + '"}',
					url: '<%= ResolveClientUrl("~/api/Calendar/SearchEvents") %>',
					type: 'POST',
					contentType: "application/json",
					dataType: "json",
					cache: false,

					success: function (result) {
						if (result.length == 0) {
							console.log("No events returned");
						} else {
							console.log("Got search " + result.length + " results");
							$.each(result, function (i, node) {
								$('#calSearchResults').append($('<option/>', {
									value: node.start,
									text: node.start + ' ' + node.title
								}));
							});
						}

					},

					error: function (x, y, z) {
						console.log("Error " + x + ' ' + y + ' ' + z);
					}

				});

			}


			//Calendar Search Go button
			$("#calSearchGoBtn").click(function () {
				var e = document.getElementById("calSearchResults");
				if (e.selectedIndex > -1) {
					var targetDate = e.options[e.selectedIndex].value;
					doSearchResultEdit(targetDate);
				}
			});

			//Calendar Search double click
			$("#calSearchResults").bind("dblclick", function (e) {
				console.log("doublclick for ");
				var targetDate = e.target.value;
				console.log("doublclick for " + targetDate);
				doSearchResultEdit(targetDate);
			});



			function doSearchResultEdit(targetDate) {
			
				$.ajax({
					data: '{"start": "' + targetDate + '", "end": "' + targetDate + '"}',
					url: '<%= ResolveClientUrl("~/api/Calendar/GetEvents") %>',
					type: 'POST',
					contentType: "application/json",
					dataType: "json",
					cache: false,

					success: function (result) {
						if (result.length == 0) {
							console.log("No events returned");
						} else {
							console.log("result: " + JSON.stringify(result))
							$("#calSearchDlg").modal('hide');
							var theTitle = result[0].title;
							console.log("selected search Item date: " + targetDate);
							console.log("selected search Item text: " + theTitle);

							//force calender to view of the selDate's month
							console.log("render cal for target date");
							selDate = moment(targetDate);
							$('#calendar').fullCalendar('gotoDate', selDate);

							//launch the editor dialog with the date's content 
							selEvent = { title: theTitle, start: selDate, allDay: true };
							launchDlg();
						}

					},

					error: function (x, y, z) {
						console.log("Error " + x + ' ' + y + ' ' + z);
					}

				});

			};

			function launchDlg() {

				var dt = moment(selDate, "YYYY-MM-DD HH:mm:ss");
				var dayName = dt.format('dddd');
				$('#calDateDisplay').html(dayName + ', ' + moment(selDate).format("MMMM DD, YYYY"));
				$('#calDate').val(selDate.format());
				$('#calEditDlgMsg').html("If you don't Submit, your changes will be lost.");
				if (selEvent == null) {
					$("#calDeleteBtn").prop("disabled", true);
					$("#calDeleteBtn2").prop("disabled", true);
					$("#calContent").val("");
				} else {
					$("#calDeleteBtn").prop("disabled", false);
					$("#calDeleteBtn2").prop("disabled", false);
					$("#calContent").val(selEvent.title);

				}
				//alert("before show");
				$("#calEditDlg").modal('show');
			};

			//fixme dead code, not called?
			function addCalanderEvent(id, title) {

				var eventObject = {
					title: title,
					start: selDate,
					allDay: true,
					id: 'TMP-' + id

				};

				$('#calendar').fullCalendar('renderEvent', eventObject);
				return eventObject;
			}

			$("#calSaveBtn").click(function () { doCalSave(); });
			$("#calSaveBtn2").click(function () { doCalSave(); });

			$("#calDeleteBtn").click(function () { doCalDeletePrompt(); });
			$("#calDeleteBtn2").click(function () { doCalDeletePrompt(); });

			function doCalDeletePrompt() {
				bootbox.confirm({
					title: "Clear The Date?",
					message: 'Confirm deletion for <b>' + selDate.format() + '</b>',
					buttons: {
						cancel: {
							label: '<i class="fa fa-times"></i> Cancel'
						},
						confirm: {
							label: '<i class="fa fa-check"></i> Confirm'
						}
					},
					callback: function (result) {
						doCalDelete(result);
					}
				});
			}

			function doCalDelete(isConfirmed) {

				if (!isConfirmed) {
					return;
				}

				if (selEvent != null) {

					var eput = JSON.stringify($('#calForm').serializeObject());
					$('#calEditDlgMsg').html(spinGlyph + " The server is deleting the event...");

					$.ajax({
						url: deleteCalEventURL,
						type: "DELETE",
						data: eput,
						dataType: 'json',
						contentType: "application/json",
						complete: function (xhr, statusText) {
							console.log("xhr.status=" + xhr.status + ", statusText=" + statusText);
							switch (xhr.status) {
								case 200: // Delete success                            
									if (selEvent != null) {
										$('#calendar').fullCalendar('removeEvents', selEvent._id);
									}
									$("#calDeleteBtn").prop("disabled", true);
									$("#calDeleteBtn2").prop("disabled", true);
									$("#calContent").val("");
									selEvent = null;
									$('#calEditDlgMsg').html("Deleted successfully!");
									$("#calEditDlg").modal('hide');
									break;
								case 204: // Delete failed
									console.log("Server returned 204 -- Delete fail");
									$("#calDeleteBtn").prop("disabled", false);
									$("#calDeleteBtn2").prop("disabled", false);
									$('#calEditDlgMsg').html("There was no record to delete (404 error)");
									break;
								case 404:
									$("#calDeleteBtn").prop("disabled", false);
									$("#calDeleteBtn2").prop("disabled", false);
									$('#calEditDlgMsg').html("Service connection worked, but operation failed(404 error)");
									break;
								default:
									$("#calDeleteBtn").prop("disabled", false);
									$("#calDeleteBtn2").prop("disabled", false);
									$('#calEditDlgMsg').html("Error " + xhr.status);
							}
						},
						error: function (data, textStatus, errorThrown) {
							$('#calEditDlgMsg').html(alertGlyph + "Failed Fetching: " + errorThrown + "\r\n" + textStatus + "\r\n" + data);
						}

					});

				}
			}

			function doCalSave() {

				var eput = JSON.stringify($('#calForm').serializeObject());
				$('#calEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

				$.ajax({
					url: editCalEventURL,
					type: "POST",
					data: eput,
					dataType: 'json',
					contentType: "application/json",
					complete: function (xhr, statusText) {
						console.log("xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // Our service did a successful Edit                            
								console.log("Save returned 200 (success)");
								if (selEvent == null) {
									$('#calendar').fullCalendar('refetchEvents');
								} else {
									//update the calendar and dialog controls
									selEvent.title = $('#calContent').val();
									var unPack = JSON.stringify(selEvent);
									console.log("selEvent update:" + unPack);
									//$('#calendar').fullCalendar('updateEvent', selEvent);
									$('#calendar').fullCalendar('refetchEvents'); //supports search-edited save
									$("#calDeleteBtn").prop("disabled", false);
									$("#calDeleteBtn2").prop("disabled", false);
								}
								$('#calEditDlgMsg').html("Saved successfully!");
								break;
							case 404:
								$("#calDeleteBtn").prop("disabled", false);
								$("#calDeleteBtn2").prop("disabled", false);
								$('#calEditDlgMsg').html("Service connection worked, but operation failed(404 error)");
								break;
							default:
								$("#calDeleteBtn").prop("disabled", false);
								$("#calDeleteBtn2").prop("disabled", false);
								$('#calEditDlgMsg').html("Error " + xhr.status);
						}
					},
					error: function (data, textStatus, errorThrown) {
						$('#calEditDlgMsg').html(alertGlyph + "Failed Fetching: " + errorThrown + "\r\n" + textStatus + "\r\n" + data);
					}

				});
			};

			$("#searchCalBtn").click(function () { alert("search") });
		});

	</script>

</asp:Content>
