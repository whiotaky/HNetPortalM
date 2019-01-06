<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HNetPortal.Private.Default" %>

<%@ Register Src="~/Controls/VMWareUI.ascx" TagName="VMWareUI" TagPrefix="ucl" %>
<%@ Register Src="~/Controls/CalEditDialog.ascx" TagName="CalEditDialog" TagPrefix="ucl" %>
<%@ Register Src="~/Controls/CalSearchDialog.ascx" TagName="CalSearchDialog" TagPrefix="ucl" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
	<link href="<%=ResolveClientUrl("~/Content/hnet_checkboxes.css") %>" rel="stylesheet" />

	<style>
		.calCell {
			cursor: pointer;
			width: 12px;
			height: 12px;
			text-decoration: none !important;
		}

		.calCellHighlight {
			content: "";
			display: inline-block;
			width: 22px;
			height: 22px;
			line-height: 22px;
			border-radius: 50%;
			color: white;
			text-align: center;
			background: #05ac1d;
			padding: 0px;
		}

		.calCellToday {
			background: #dd1515;
		}
	</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<!-- Image Background Page Header -->
	<!-- Note: The background image is set within the business-casual.css file. -->

	<div class="container row text-center" style="margin-bottom: 20px;">
		<header class="pageBanner">

			<div class="row ">
				<div class="col-xs-12">
					<h2>The HNet Portal</h2>
				</div>
			</div>

			<div class="row">
				<div class="col-lg-8 col-md-push-4 col-md-8 col-md-push-4 col-sm-7 col-sm-push-5" style="margin-bottom: 5px;">
					<div class="row">
						<div class="col-xs-4">
							<a href='http://www.detroitlions.com' target="_blank">
								<img src='<%= ResolveClientUrl("~/images/lions.png") %>' width='110' height='110' class="xcenter-block img-responsive" />
							</a>
						</div>
						<div class="col-xs-4">
							<a href='http://mobile.weather.gov/index.php?lat=42.5&lon=-83.19&unit=0&lg=english#radar' target="_blank">
								<img src='http://radar.weather.gov/ridge/lite/N0R/DTX_5.png?513' width='150' height='112' class="img-responsive" />
								<%--<img src='http://radar.weather.gov/ridge/lite/N0R/GRR_0.png' width='150' height='112' class="img-responsive" />--%>
								
							</a>
						</div>
						<div class="col-xs-4">
							<!-- <iframe width="150" height="112" src="//fncd.net/embed?id=mRzuCey" style="margin-left:100px"></iframe>  -->
							<a href='http://redwings.nhl.com/club/schedule.htm?season=20152016&gameType=2' target="_blank">
								<img src='<%= ResolveClientUrl("~/images/redwings.png") %>' width='114' height='94' class="xcenter-block img-responsive" />
							</a>
						</div>
					</div>
				</div>

				<div class="col-lg-4 col-lg-pull-8 col-md-4 col-md-pull-8 col-sm-5 col-sm-pull-7" style="margin-bottom: 5px;">
					<div id="smallCalendar" class="center-block img-responsive">
						<img src="/images/ajax-loader3.gif" />
					</div>
				</div>

			</div>
		</header>
	</div>

	<div class="container  row ">
		<div class="row">

			<div class="col-md-4  col-sm-4 ">

				<div class="panel panel-primary">
					<div class="panel-heading ">
						<h3 class="panel-title"><span class="glyphicon glyphicon-stats"></span>&nbsp;&nbsp;&nbsp;Stock Quotes</h3>
					</div>

					<div class="panel-body" id="quotesAreaPanel" style="min-height: 330px">
						<div id="quotesArea" class='table-responsive'></div>

						<div id="quotesProgress" class="progress progress-striped active">
							<div class="progress-bar" style="width: 100%;">Fetching Realtime Quotes..</div>
						</div>

						<div class="form-group">
							<div class="input-group">
								<input id="querySymbols" class="form-control" type="text" placeholder="Enter symbol(s) here" />
								<span class="input-group-btn">
									<button class="btn btn-primary " type="button" id="quoteBtn">Get Quote</button>
								</span>
							</div>
						</div>
					</div>

				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-road"></span>&nbsp;&nbsp;&nbsp;Delivery Service Tracking</h3>
					</div>
					<div class="panel-body">
						<div class="form-group">
							<div class="input-group">
								<span class="input-group-btn">
									<button id="trackUpsBtn" class="btn btn-info " style='width: 120px' type="button">Track UPS</button>
								</span>
								<input class="form-control" id="trackUpsText" type="text" placeholder="Enter UPS Tracking Number" list="dl_Ups" />
							</div>
						</div>
						<div class="form-group">
							<div class="input-group">
								<span class="input-group-btn">
									<button id="trackFedExBtn" class="btn btn-info" style='width: 120px' type="button">Track FEDEX</button>
								</span>
								<input id="trackFedExText" class="form-control" type="text" placeholder="Enter FEDEX Tracking Number" list="dl_FedEx" />
							</div>
						</div>
						<div class="form-group">
							<div class="input-group">
								<span class="input-group-btn">
									<button id="trackUspsBtn" class="btn btn-info " style='width: 120px' type="button">Track USPS</button>
								</span>
								<input class="form-control" id="trackUspsText" type="text" placeholder="Enter USPS Tracking Number" list="dl_Usps" />
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;&nbsp;&nbsp;Change Manager Search</h3>
					</div>
					<div class="panel-body">
						<div class="form-group">
							<div class="checkbox checkbox-info checkbox-inline">
								<input type="checkbox" id="descrSearch" checked="" />
								<label for="descrSearch">Description</label>
							</div>
							<div class="checkbox checkbox-info checkbox-inline">
								<input type="checkbox" id="runningStatusSearch" checked="" />
								<label for="runningStatusSearch">Run. Status</label>
							</div>
							<div class="checkbox checkbox-info checkbox-inline">
								<input type="checkbox" id="resolutionSearch" checked="" />
								<label for="resolutionSearch">Resolution</label>
							</div>
							<div class="input-group" style="margin-top: 10px;">
								<input id="cmSearchText" type="text" class="form-control" placeholder="Search Term(s)" />
								<span class="input-group-btn">
									<button id="cmSearchBtn" class="btn btn-info " type="button">Search</button>
								</span>
							</div>
						</div>
						<div class="col-md-12  col-sm-12 hidden-xs">
							<small>
								<b><span class="glyphicon glyphicon-hand-right"></span></b>
								Check at least one field to search. If entering more than one search term, delimit with one space character.
							</small>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-cd"></span>&nbsp;&nbsp;&nbsp;Software DB Search</h3>
					</div>
					<div class="panel-body">
						<div class="form-group">
							<div class="input-group" style="margin-bottom: 10px">
								<input id="swSearchText" type="text" class="form-control" placeholder="Search Text" />
								<span class="input-group-btn">
									<button id="swSearchBtn" class="btn btn-info " type="button">Search</button>
								</span>
							</div>

							<div class="radio radio-info radio-inline">
								<input type="radio" id="r1" name="swSearchType" value="Normal" checked="" />
								<label for="r1">Normal</label>
							</div>
							<div class="radio radio-info radio-inline">
								<input type="radio" id="r2" name="swSearchType" value="Deep" />
								<label for="r2">Deep</label>
							</div>

						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-cloud netNodeRefetch" id='refreshNodeList' title="Click to re-execute the probes"></span>&nbsp;&nbsp;&nbsp;Network Nodes</h3>
					</div>
					<div class="panel-body" id="netNodesArea">

						<div class="progress progress-striped active">
							<div class="progress-bar" style="width: 100%;">Executing Network Probes...</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-tasks"></span>&nbsp;&nbsp;&nbsp;troll Uptime</h3>
					</div>
					<div id="trollUptime" data-nlines='50'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">Fetching Troll Uptime...</div>
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-equalizer"></span>&nbsp;&nbsp;&nbsp;TFS Latest Build Results</h3>
					</div>
					<div id="tfsBuild">
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">Fetching TFS Build Results...</div>
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-info-sign"></span>&nbsp;&nbsp;&nbsp;WEB2: TFS Nightly Backup</h3>
					</div>
					<div id="postload_2001" data-log='TFSBACKUP' data-nlines='120'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">WEB2: TFS Backup Log...</div>
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-info-sign"></span>&nbsp;&nbsp;&nbsp;ESXi: Nightly VM Backup</h3>
					</div>
					<div id="postload_1001" data-log='ESXI' data-nlines='50'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">ESXi: Nightly VM Backup...</div>
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-info-sign"></span>&nbsp;&nbsp;&nbsp;DBS: PFTrack Quotes Update</h3>
					</div>
					<div id="postload_2002" data-log='DBSPFTRACK' data-nlines='20'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">DBS: PFTrack Quotes Update...</div>
							</div>
						</div>
					</div>
				</div>


				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-info-sign"></span>&nbsp;&nbsp;&nbsp;DBS: PFTrack2 Quotes Update</h3>
					</div>
					<div id="postload_2003" data-log='DBSPF2TRACK' data-nlines='60'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">DBS: PFTrack2 Quotes Update...</div>
							</div>
						</div>
					</div>
				</div>

				<div class="panel panel-primary">
					<div class="panel-heading">
						<h3 class="panel-title"><span class="glyphicon glyphicon-info-sign"></span>&nbsp;&nbsp;&nbsp;Troll: Nightly Backup</h3>
					</div>
					<div id="postload_2004" data-log='TROLLNIGHTLY' data-nlines='20'>
						<div class="panel-body">
							<div class="progress progress-striped active">
								<div class="progress-bar" style="width: 100%;">Troll: Nightly Backup...</div>
							</div>
						</div>
					</div>
				</div>

			</div>

			<div runat="server" id="column2" class="col-md-4 col-sm-4">
			</div>

			<div runat="server" id="column3" class="col-md-4 col-sm-4">
			</div>

		</div>
	</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
	<ucl:CalEditDialog runat="server" ID="CalEditDialog" />
	<ucl:CalSearchDialog runat="server" ID="CalSearchDialog" />

	<!-- These lists are lazy loaded as needed if and when their inputs receive focus-->
	<datalist id="dl_Ups"></datalist>
	<datalist id="dl_FedEx"></datalist>
	<datalist id="dl_Usps"></datalist>

	<script type="text/javascript">

		$.ajaxSetup({
			// Disable caching of AJAX responses
			cache: false
		});

		var symbols = ".DJI,.IXIC,.SPX,PM-G-1OZ,PM-S-1OZ,WDC";
		var weekdays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
		var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

		var date = new Date();
		var month = date.getMonth() + 1;
		var year = date.getFullYear();

		var dangerDiv = '<div class="alert alert-danger fade in" role="alert" style="margin-bottom: 1px;">';
		var successDiv = '<div class="alert alert-success  fade in" role="alert" style="margin-bottom: 1px;">';
		var warningDiv = '<div class="alert alert-warning  fade in" role="alert" style="margin-bottom: 1px;">';

		var spinGlyph = "<span class='glyphicon glyphicon-cog glyphicon-spin' style='margin-right:8px;'></span>";
		var alertGlyph = "<span class='glyphicon glyphicon-alert' style='margin-right:8px;'></span>";

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

		loadCalendar(month, year);

		if ($("#netNodesArea").length != 0) {
			loadNetNodes($('#netNodesArea'), 'true');
		};

		if ($("#quotesArea").length != 0) {
			quotes($('#quotesArea'), symbols, "true");
		};

		if ($("#trollUptime").length != 0) {
			trollUptime($('#trollUptime'));
		};

		if ($("#tfsBuild").length != 0) {
			tfsBuild($('#tfsBuild'));
		};

		chainLog($('#postload_1001'));
		chainLog($('#postload_2001'));
		chainFeed($('#postload_2501'));
		chainFeed($('#postload_3001'));

		//This handles click event to refresh the network node list forcing non-cached (fresh) results
		$(document).on('click', "#refreshNodeList", function () {
			$('#netNodesArea').html('<div class="progress progress-striped active"> <div class="progress-bar" style= "width: 100%;"> Re-executing Network Probes...</div></div>');
			loadNetNodes($('#netNodesArea'), 'false');
			return false;
		});


		function tfsBuild(container) {

			console.log("tfsBuild starting");
			var url = '<%=ResolveClientUrl("~/api/TFS/BuildResult") %>';
			$.ajax({
				type: "get",
				url: encodeURI(url + '/WSH/Archive All'),
				contentType: "application/json",
				dataType: "json",
				complete: function (xhr, statusText) {
					console.log("BuildStatus xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // success
							console.log(url + " returned 200 -- Success");
							break;
						case 500:
							console.log(url + " returned 500 Error");
							container.html(dangerDiv + "TeamFoundation.GetBuildStatus failed (500 error)" + "</div>");
							break;
						default:
							container.html(dangerDiv + "Error " + xhr.status + "</div>");
					}
				},

				success: function (data) {
					console.log("Running Build Result Success handler");
					var status = data.status;
					var result = data.result;

					if ($.isArray(data)) {
						//indicates error returned by service
						//3.7/2018: dead code?
						console.log("data is array");
						status = data[0].status;
						result = data[0].result;
					}
					console.log("data status=" + status);
					console.log("data result=" + result);

					var s = "";
					var whichDiv = successDiv;

					var sdate = new Date(data.started).toLocaleString();
					var fdate = "";
					if (status == "InProgress") {
						result = "";
						whichDiv = warningDiv;
					} else {  //It's probably completed
						fdate = new Date(data.finished).toLocaleString();
						if (result != 'Succeeded') {
							whichDiv = dangerDiv;
						}
					}

					s = whichDiv;
					s = s + "<b>Status:</b> " + status + "<br/>"
					s = s + "<b>Result:</b> " + result + "<br/>"
					s = s + "<b>Project/Build Name:</b> " + data.projectName + " / " + data.buildName + "<br/>"
					s = s + "<b>Build Id: </b>" + data.id + "<br/>";
					s = s + "<b>Started: </b>" + sdate + "<br/>";
					s = s + "<b>Finished: </b>" + fdate + "<br/>";
					s = s + "</div>";

					container.html(s + "<div class='logViewer scrollable'>" + data.log + "</div>");

				},

				error: function (x, y, z) {
					console.log("TFS Build Result Error function ran");
					container.html(dangerDiv + "Error " + x + ' ' + y + ' ' + z + "</div>");
				}
			});
		}


		function trollUptime(container) {
			console.log("trollUptime starting");
			$.ajax({
				type: "get",
				datatype: "text",
				url: '<%= ResolveClientUrl("~/api/TrollUptime") %>',
				success: function (result) {
					container.html(result);
				},

				error: function (x, y, z) {
					container.html("Error " + x + ' ' + y + ' ' + z);
				}
			});
		}


		function chainFeed(container) {

			//alert(container.attr("id"));
			var feedId = container.data('feedid');
			//container.html("The feed No is " + feedId);

			var url = '<%= ResolveClientUrl("~/api/NewsFeed/") %>' + feedId;
			console.log("Chainfeed: >" + url + "<");
			$.ajax({
				type: "GET",
				datatype: "text",
				url: url,
				success: function (result) {
					console.log("Chainfeed: success");
					container.html(result);
					var id = container.attr('id');
					var parts = id.split('_');
					var nx = parts[1];
					var n = parseInt(nx) + 1;
					var nextId = "#postload_" + n;
					$('a').tooltip();  //apply/reapply for bootstrap tool tip

					if ($(nextId).length) { //exists
						chainFeed($(nextId));
					};

				},

				error: function (x, y, z) {
					console.log("Chainfeed: Error " + x + ' ' + y + ' ' + z);
					container.html("Error " + x + ' ' + y + ' ' + z);
				}
			});

		}

		function chainLog(container) {
			var wl = container.data('log');
			var nl = container.data('nlines');
			console.log("chainLog: wl=" + wl);

			$.ajax({

				type: "POST",
				data: '{"whichLog":"' + wl + '", "numLines":' + nl + '}',
				url: '<%= ResolveClientUrl("~/api/LogTail") %>',
				contentType: "application/json",
				dataType: "text",

				success: function (result) {
					console.log("chainLog: Success");
					//container.html('<textarea readonly style="font-family: courier; width:100%;height: 150px">' + result + '</textarea>');
					container.html("<div class='logViewer scrollable'>" + result + "</div>");
					var id = container.attr('id');
					var parts = id.split('_');
					var nx = parts[1];
					var n = parseInt(nx) + 1;
					var nextId = "#postload_" + n;

					if ($(nextId).length) { //exists
						chainLog($(nextId));
					};

				},

				error: function (x, y, z) {
					console.log("chainLog: Error " + z);
					container.html("Error " + x + ' ' + y + ' ' + z);
				}
			});
		}


		function unchainedContent(container) {
			//alert("chain test");
			$.ajax({
				url: "~/testAJax.aspx",// container.data('url'),
				success: function (html) {
					container.html(html);
				},
				error: function () {
					container.html("ERROR FETCHING");
				}
			});
		};

		function formatQuote(value, qType) {

			if (qType === "I") {
				var nf = new Intl.NumberFormat('en-US', {
					minimumFractionDigits: 3,
					maximumFractionDigits: 3
				});
				return nf.format(value);

			} else {
				var nf = new Intl.NumberFormat('en-US', {
					style: 'currency',
					currency: 'USD',
					minimumFractionDigits: 2,
					maximumFractionDigits: 2
				});
				return nf.format(value);
			}

		};

		function quotes(container, syms, allowCached) {

			console.log("quotes: syms=" + syms);

			$("#quotesProgress").css("visibility", "visible");
			$.ajax({
				type: "POST",
				data: '{"symbols":"' + syms + '","allowCached": "' + allowCached + '"}',
				url: '<%= ResolveClientUrl("~/api/Quotes") %>',
				contentType: "application/json",
				dataType: "json",

				success: function (data) {
					console.log("quotes: success");
					var s = '<table class="table table-condensed">\n';
					s = s + '<thead><tr><th>Symbol</th><th style="text-align:right">Index/Price</th><th style="text-align:right">Change</th></tr></thead><tbody>\n';
					$.each(data, function (i, node) {
						var color = node.changetrend == "Up" ? "green" : "red";
						var icon = node.changetrend == "Up" ? "top" : "bottom";
						s = s + "<tr><td>" + node.symbol + "</td><td style='text-align:right'>" + formatQuote(node.price, node.type) + "</td><td style='text-align:right;color: " + color + "'>" + formatQuote(node.change, node.type) + " <span class='glyphicon glyphicon-triangle-" + icon + "'></span></td></tr>";
					});
					s = s + "</tbody></table>\n";
					container.html(s);
					$("#quotesProgress").css("visibility", "hidden");
				},

				error: function (x, y, z) {
					console.log("quotes: Error: " + x);
					container.html("Error " + x + ' ' + y + ' ' + z);
					$("#quotesProgress").css("visibility", "hidden");
				}

			});
		}


		function loadNetNodes(container, allowCached) {

			console.log("loadNetNodes: allowCached=" + allowCached);
			$.ajax({
				type: "POST",
				contentType: "application/json",
				data: '{"allowCached":"' + allowCached + '"}',
				url: '<%= ResolveClientUrl("~/api/NodeStatus") %>',
				dataType: "json",

				success: function (data) {
					console.log("loadNetNodes: success");
					var s = "";
					$.each(data, function (i, node) {
						s = s + "<div id='vmwareTool_" + node.nodeName + "' data-vmname='" + node.nodeName + "' class='netNode netNode" + node.nodeStatus + "' title='" + node.nodeIP + "' OnClick=launchVMDlg('" + node.nodeName + "'); style='cursor: pointer;'>" + node.nodeName + "</div>\n";
					});
					container.html(s);

				},

				error: function (data, status, jqXHR) {
					console.log("loadNetNodes: Error: " + jqXHR);
					container.html("<p class='text-danger'>Error: " + jqXHR + "</p>\n");
				}

			})
		}


		$("#cmSearchBtn").click(function () {
			var url = "http://changeman.hiotaky.com/Request/Search?searchText=" + $("#cmSearchText").val();

			//descrSearch=on&runningStatusSearch=on&resolutionSearch=on&searchText=djh";

			if ($('#descrSearch').is(":checked")) {
				url = url + "&descrSearch=on";
			}
			if ($('#runningStatusSearch').is(":checked")) {
				url = url + "&runningStatusSearch=on";
			}
			if ($('#resolutionSearch').is(":checked")) {
				url = url + "&resolutionSearch=on";
			}

			window.open(url);
			return false;
		});


		//http://$sw_searchHost/softwareDB/Public/search.aspx
		$("#swSearchBtn").click(function () {

			var searchType = $('input:radio[name=swSearchType]:checked').val();

			var form = document.createElement("form");
			form.setAttribute("method", "post");

			//TODO: remove deep/normal buttons
			form.setAttribute("action", "/Private/SoftwareDB.aspx");

			var st = document.createElement("input");
			st.setAttribute("type", "hidden");
			st.setAttribute("name", "searchTxt");
			st.setAttribute("value", $("#swSearchText").val());
			form.appendChild(st);

			var sty = document.createElement("input");
			sty.setAttribute("type", "hidden");
			sty.setAttribute("name", "searchType");
			sty.setAttribute("value", searchType);
			form.appendChild(sty);

			document.body.appendChild(form);
			form.submit();

		});

		$("#quoteBtn").click(function () {
			var qs = $("#querySymbols").val();
			quotes($('#quotesArea'), symbols + "," + qs, "false");
		});


		/* Shipper Tracking stuff  */

		//http://www.binaryintellect.net/articles/aaecb7bc-18d2-44c5-8d81-07b8cb634a0b.aspx
		$("#trackUpsText").on("focus", function () { updateShippingDl("Ups"); });
		$("#trackFedExText").on("focus", function () { updateShippingDl("FedEx"); });
		$("#trackUspsText").on("focus", function () { updateShippingDl("Usps"); });

		$("#trackUpsBtn").click(function () {

			if ($("#trackUpsText").val() === "") {
				console.log("window.open supressed due to empty input field--Ups");
				return false;
			}

			var url = "http://wwwapps.ups.com/WebTracking/track?loc=en_US&HTMLVersion=5.0&saveNumbers=null&trackNums=" + $("#trackUpsText").val() + '&track.x=Track';
			window.open(url, 'upsWindow', 'resizable=1,scrollbars=1');
			updateShippingDB("Ups", $("#trackUpsText").val());
			return false;
		});

		$("#trackFedExBtn").click(function () {

			if ($("#trackFedExText").val() === "") {
				console.log("window.open supressed due to empty input field--FedEx");
				return false;
			}

			var url = "http://www.fedex.com/Tracking?cntry_code=US&language=EN&tracknumbers=" + $("#trackFedExText").val() + "&action=track";
			window.open(url, 'fedExWindow', 'resizable=1,scrollbars=1');
			updateShippingDB("FedEx", $("#trackFedExText").val());
			return false;
		});

		$("#trackUspsBtn").click(function () {

			if ($("#tracUspsText").val() === "") {
				console.log("window.open supressed due to empty input field--usps");
				return false;
			}

			var url = "https://tools.usps.com/go/TrackConfirmAction?qtc_tLabels1=" + $("#trackUspsText").val();
			window.open(url, 'uspsWindow', 'resizable=1,scrollbars=1');
			updateShippingDB("Usps", $("#trackUspsText").val());
			return false;
		});

		//updates the datalist with user's recent tracking numbers
		function updateShippingDl(whichList) {
			var options = {};
			options.url = "/api/ShippingTrack/" + whichList;
			options.type = "GET";
			options.dataType = "json";
			options.success = function (data) {
				var dlId = "#dl_" + whichList;
				$(dlId).empty();
				for (var i = 0; i < data.length; i++) {
					$(dlId).append("<option>" + data[i].trackingNo + "</option>");
				}
			};

			$.ajax(options);
			console.log("tracking " + whichList);
		}

		//Update shipping db table with this looked up package inquiry
		function updateShippingDB(ShipperCode, trackingNo) {
			var options = {};
			options.url = "/api/ShippingTrack";
			options.type = "POST";
			options.data = '{"shipperCode":"' + ShipperCode + '","trackingNo": "' + trackingNo + '"}';
			options.dataType = "json";
			options.contentType = "application/json";
			options.success = function (data) {
				console.log("Shipping DB Post worked");
			};
			options.error = function (x, y, z) {
				console.log("Shipping DB Post Error " + x + ' ' + y + ' ' + z);
			};
			$.ajax(options);
			console.log("Shipping DB Update tracking=" + trackingNo + " Shipper=" + ShipperCode);
		}


		/* Calendar stuff start */

		function loadCalendar(mon, year) {

			var getUserCalendarHtmlUrl = '<%= ResolveClientUrl("~/api/Calendar/UserCalendarHtml") %>';
			var url = getUserCalendarHtmlUrl + "?_monthNo=" + mon + "&_yearNo=" + year;
			//This ""jumps" the screen around $('#smallCalendar').html('<img src="/images/ajax-loader3.gif" />');

			console.log("loading calendar w/url=" + url);
			$.ajax({
				type: "get",
				datatype: "text/plain",
				url: url,
				success: function (result) {
					console.log("loading calendar worked");
					$('#smallCalendar').html(result);
					$('a').tooltip();
				},

				error: function (x, y, z) {
					console.log("loading calendar failed");
					$('#smallCalendar').html("Error loading calendar" + x + ' ' + y + ' ' + z);
				}
			});

		}

		function launchCalSearch() {
			$("#calSearchDlg").modal('show');
		}

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
						var tmpDate = new Date(targetDate + "T12:00:00Z");
						console.log("render cal for target date month=" + tmpDate.getMonth());
						loadCalendar(tmpDate.getMonth() + 1, tmpDate.getFullYear());

						//launch edit dialog
						launchCalEditDlg(targetDate);

						//setEventAfterRender = targetDate;
						//selDate = moment(targetDate);
						//$('#calendar').fullCalendar('gotoDate', selDate);

						////launch the editor dialog with the date's content 
						//selEvent = { title: theTitle, start: selDate, allDay: true };							
						//launchDlg();
					}

				},

				error: function (x, y, z) {
					console.log("Error " + x + ' ' + y + ' ' + z);
				}

			});
		}

		function launchCalEditDlg(theDate) { //coming in as yyyy-mm-dd

			var selDate = new Date(theDate + "T12:00:00Z");
			console.log(theDate + " | " + selDate);

			$('#calDateDisplay').html(weekdays[selDate.getDay()] + ', ' + monthNames[selDate.getMonth()] + " " + selDate.getDate() + ', ' + selDate.getFullYear());
			$('#calDate').val(theDate);
			$('#calEditDlgMsg').html("If you don't Submit, your changes will be lost.");
			$("#calContent").val("");
			$("#calDeleteBtn").prop("disabled", true);

			$.ajax({
				data: '{"start": "' + theDate + '", "end": "' + theDate + '"}',
				url: '<%= ResolveClientUrl("~/api/Calendar/GetEvents") %>',
				type: 'POST',
				contentType: "application/json",
				dataType: "json",
				cache: false,

				success: function (result) {
					if (result.length == 0) {
						console.log("No events returned");
					} else {
						console.log("Got data (title): " + result[0].title);
						$("#calDeleteBtn").prop("disabled", false);
						$("#calContent").val(result[0].title);
					}

				},
				error: function (x, y, z) {
					console.log("Error " + x + ' ' + y + ' ' + z);
				}
			});

			$("#calEditDlg").modal('show');
		}

		$("#calSaveBtn").click(function () { doCalSave(); });
		$("#calDeleteBtn").click(function () { doCalDeletePrompt(); });

		function doCalSave() {

			var eput = JSON.stringify($('#calForm').serializeObject());
			$('#calEditDlgMsg').html(spinGlyph + " The server is processing your save request...");

			$.ajax({
				url: '<%= ResolveClientUrl("~/api/Calendar/EditEvent") %>',
				type: "POST",
				data: eput,
				dataType: 'json',
				contentType: "application/json",

				complete: function (xhr, statusText) {
					console.log("xhr.status=" + xhr.status + ", statusText=" + statusText);
					switch (xhr.status) {
						case 200: // Our service did a successful Edit                            
							$("#calDeleteBtn").prop("disabled", false);
							$('#calEditDlgMsg').html("Saved successfully!");
							var selCalDate = $('#calDate').val();
							var selDate = new Date(selCalDate + "T12:00:00Z");
							console.log("reload calendar " + selCalDate + "|" + selDate + " | m=" + parseInt(selDate.getMonth() + 1) + "y=" + selDate.getFullYear());
							loadCalendar(selDate.getMonth() + 1, selDate.getFullYear());
							break;
						case 404:
							$("#calDeleteBtn").prop("disabled", false);
							$('#calEditDlgMsg').html($('#calEditDlgMsg').html("Service connection worked, but operation failed(404 error)"));
							break;
						default:
							$("#calDeleteBtn").prop("disabled", false);
							$('#calEditDlgMsg').html("Error " + xhr.status);
					}
				},
				error: function (data, textStatus, errorThrown) {
					$('#calEditDlgMsg').html(alertGlyph + "Failed Fetching: " + errorThrown + "\r\n" + textStatus + "\r\n" + data);
				}

			});
		};


		function doCalDeletePrompt() {

			var selCalDate = $('#calDate').val();
			console.log("doCalDeletePrompt(), date = " + selCalDate);

			bootbox.confirm({
				title: "Clear The Date?",
				message: 'Confirm deletion for <b>' + selCalDate + '</b>',
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

			var selCalDate = $('#calDate').val();
			if (selCalDate != '') {

				var eput = JSON.stringify($('#calForm').serializeObject());
				$('#calEditDlgMsg').html(spinGlyph + " The server is deleting the event...");

				$.ajax({
					url: '<%= ResolveClientUrl("~/api/Calendar/DeleteEvent") %>',
					type: "DELETE",
					data: eput,
					dataType: 'json',
					contentType: "application/json",
					complete: function (xhr, statusText) {
						console.log("xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // Delete success                            
								var selCalDate = $('#calDate').val();
								var selDate = new Date(selCalDate + "T12:00:00Z");
								console.log("reload calendar " + selCalDate + "|" + selDate + " | m=" + parseInt(selDate.getMonth() + 1) + "y=" + selDate.getFullYear());
								loadCalendar(selDate.getMonth() + 1, selDate.getFullYear())
								$('#calEditDlgMsg').html("Deleted successfully!");
								$("#calEditDlg").modal('hide');
								break;
							case 204: // Delete failed
								console.log("Server returned 204 -- Delete fail");
								$("#calDeleteBtn").prop("disabled", false);
								$('#calEditDlgMsg').html("There was no record to delete (404 error)");
								break;
							case 404:
								$("#calDeleteBtn").prop("disabled", false);
								$('#calEditDlgMsg').html("Service connection worked, but operation failed(404 error)");
								break;
							default:
								$("#calDeleteBtn").prop("disabled", false);
								$('#calEditDlgMsg').html("Error: " + xhr.status);
						}
					},
					error: function (data, textStatus, errorThrown) {
						$('#calEditDlgMsg').html(alertGlyph + "Failed Fetching: " + errorThrown + "\r\n" + textStatus + "\r\n" + data);
					}

				});

			}

		}

		/* Calendar stuff END */

		//enable bootstrap tool tips on links
		$('a').tooltip();

	</script>

	<ucl:VMWareUI runat="server" ID="VMWareUI" />

</asp:Content>
