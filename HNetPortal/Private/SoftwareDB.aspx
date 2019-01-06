<%@ Page Title="HNet Software Database" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="SoftwareDB.aspx.cs" Inherits="HNetPortal.Private.knockoutTester" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
	<link href="<%=ResolveClientUrl("~/Content/bootstrap-datepicker.css") %>" rel="stylesheet" />
	<script src="<%=ResolveClientUrl("~/Scripts/knockout-3.4.2.js")%>"> </script>
	<script src="<%=ResolveClientUrl("~/Scripts/bootstrap-datepicker.js")%>"> </script>

	<style>
		tbody tr:hover td, th {
			cursor: pointer;
		}

		.panel.with-nav-tabs .panel-heading {
			padding: 5px 5px 0 5px;
		}

		.panel.with-nav-tabs .nav-tabs {
			border-bottom: none;
		}

		.panel.with-nav-tabs .nav-justified {
			margin-bottom: -1px;
		}

		/* this fixes issues where scroller doesnt appear on
			tables on small screens.
		*/
		.pre-scrollable {
			min-height: 540px;
			max-height: 540px;
			overflow-y: scroll; /*this is the main fix*/
		}

		/********************************************************************/

		/*** PANEL PRIMARY ***/
		.with-nav-tabs.panel-primary .nav-tabs > li > a,
		.with-nav-tabs.panel-primary .nav-tabs > li > a:hover,
		.with-nav-tabs.panel-primary .nav-tabs > li > a:focus {
			color: #fff;
		}

			.with-nav-tabs.panel-primary .nav-tabs > .open > a,
			.with-nav-tabs.panel-primary .nav-tabs > .open > a:hover,
			.with-nav-tabs.panel-primary .nav-tabs > .open > a:focus,
			.with-nav-tabs.panel-primary .nav-tabs > li > a:hover,
			.with-nav-tabs.panel-primary .nav-tabs > li > a:focus {
				color: #fff;
				background-color: #3071a9;
				border-color: transparent;
			}

		.with-nav-tabs.panel-primary .nav-tabs > li.active > a,
		.with-nav-tabs.panel-primary .nav-tabs > li.active > a:hover,
		.with-nav-tabs.panel-primary .nav-tabs > li.active > a:focus {
			color: #428bca;
			background-color: #fff;
			border-color: #428bca;
			border-bottom-color: transparent;
		}

		.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu {
			background-color: #428bca;
			border-color: #3071a9;
		}

			.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > li > a {
				color: #fff;
			}

				.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > li > a:hover,
				.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > li > a:focus {
					background-color: #3071a9;
				}

			.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > .active > a,
			.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > .active > a:hover,
			.with-nav-tabs.panel-primary .nav-tabs > li.dropdown .dropdown-menu > .active > a:focus {
				background-color: #4a9fe9;
			}


		/********************************************************************/
	</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<h1>Software Database</h1>

	<div class="row">

		<!-- https://bootsnipp.com/snippets/featured/panels-with-nav-tabs -->
		<div class="panel with-nav-tabs panel-primary">
			<div class="panel-heading">
				<ul class="nav nav-tabs" id="tabSet">
					<li id="infoTabli" class="active"><a href="#infoTab" data-toggle="tab"><i class="glyphicon glyphicon-info-sign"></i>&nbsp;Info</a></li>
					<li id="searchTabli"><a href="#searchTab" data-toggle="tab" onclick="launchSearchDlg();"><i class="glyphicon glyphicon-search"></i>&nbsp;Search...</a></li>
					<li id="compCdTabli"><a href="#compCdTab" data-toggle="tab"><i class="glyphicon glyphicon-folder-close"></i>&nbsp;Compilation CD's</a></li>
					<li id="isoCdTabli"><a href="#isoCdTab" data-toggle="tab"><i class="glyphicon glyphicon-cd"></i>&nbsp;ISO CD's</a></li>
					<li id="combMediaTabli"><a href="#combMediaTab" data-toggle="tab"><i class="glyphicon glyphicon-list-alt"></i>&nbsp;Combined Media</a></li>
					<li id="appTypesTabli"><a href="#appTypesTab" data-toggle="tab"><i class="glyphicon glyphicon-th-list"></i>&nbsp; App Types</a></li>
					<li>
						<a href="#" id="filterTabli" onclick="ChangeFilter();"><i class="glyphicon glyphicon-filter"></i>&nbsp;Filter: [None]</a>
						<%--
							This effectively creates a dropdown menu
							<ul class="dropdown-menu" role="menu">
								<li><a href="#" onclick="ChangeFilter(); ">Change Filter...</a></li>
								<li><a href="#" onclick="ChangeOil(); ">Change Oil...</a></li>
							</ul>
						--%>
					</li>

				</ul>
			</div>
			<div class="panel-body">
				<div class="tab-content">

					<div class="tab-pane fade in active" id="infoTab">
						<p>
							<b>This is the <i>NEW</i> HNet Software Database</b>. It is now fully integrated into the HNet portal and hence 
							replaces the previous <a href='http://softwaredb.hiotaky.com/' target="_blank">stand-alone version website</a>
							that was originally written in 2004. The previous version will be decommissioned sometime in 2018.
						</p>
						<p>
							<a href='#' onclick='activateTab("#isoCdTab");'>ISO cd's</a> are usually CD's that contain a single application, delivered on Compact Disk from a software 
						company.  Sometimes they are images of downloaded .iso files "burnt" onto the the media.
						</p>
						<p>
							<a href='#' onclick='activateTab("#compCdTab");'>Compilation CD's</a> contain multiple applications, data files, pictures, music files, etc..  
						</p>

						<p>
							The <a href='#' onclick='activateTab("#combMediaTab");'>Combined Tab</a> lists all software from all sources.  
						</p>

						Other Points:
							<ul>
								<li>The  <a href='#' onclick='activateTab("#filterTab");'>Filter Tab</a> will set an application type filter for the ISO, compilation, and combined lists tabs.  The filter is NOT 
									applied to the Search or App Types tabs.
								</li>
								<li>The <a href='#' onclick='activateTab("#searchTab");'>Search Tab</a> provides a "deep" search function.  </li>
								<li>The <a href='#' onclick='activateTab("#appTypesTab");'>App Types Tab</a> is provided for maintaining application types.  </li>
								<li>Any screen that displays an <a href="#" id='demo' class="btn btn-success btn-xs"><i class="fa fa-plus"></i>&nbsp;Add Record</a>
									button allows an <b>HNet Administrator</b> to add records to the associated record type.</li>
								<li>Datastore files (IE, files stored on the HNET VMWare server) are cataloged under <a href='#' onclick='activateTab("#compCdTab");'>Compilation CD's</a>.  
									The CD Name and Volume Name fields	are used to identify the particular datastore location.</li>
							</ul>

						<p style="text-align: center">
							<b>HNet Software Database version 3.0.0, © 2004-<%=DateTime.Now.Year%></b>
						</p>
					</div>

					<div class="tab-pane fade" id="searchTab">
						<div class='col-xs-12 table-responsive pre-scrollable' id="searchDiv">
							<table class="table table-hover" id="searchTbl">
								<thead>
									<tr>
										<th>Application Name</th>
										<th>CD Name</th>
										<th>Volume Name</th>
										<th>App Type</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: appList }">
									<tr class='clickable-row' data-bind="click: $root.clickRow, attr: { id: 'SEARCHRESULT_' + $index() }">
										<td data-bind="text: appName"></td>
										<td data-bind="text: cdName"></td>
										<td data-bind="text: volumeName"></td>
										<td data-bind="text: lookupAppType(appType())"></td>
									</tr>
								</tbody>
							</table>

						</div>
					</div>

					<div class="tab-pane fade" id="compCdTab">
						<div class='col-md-4 table-responsive pre-scrollable' id="compCdDiv" style="margin-bottom: 10px">							
							<table class="table table-hover" id="compCdTbl">
								<thead>
									<tr>
										<th>CD Name</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: cdnames, afterRender: updateCompCdTbl }">
									<tr class='clickable-row' data-bind="click: $root.fetchCompCdApps, attr: { id: 'NAMES_' + $index(), text: cdName }">
										<td data-bind="text: cdName"></td>
									</tr>
								</tbody>
							</table>
						</div>

						<div class='col-md-8 table-responsive pre-scrollable' id="compCdAppsDiv">
							<button onclick="return false;" id='btn_add_CompItem' name='btn_add_CompItem' class="btn btn-success" style="display: none"><i class="fa fa-plus"></i>&nbsp;Add Record</button>
							<table class="table table-hover" style='display: none' id="compCdAppsTbl">
								<thead>
									<tr>
										<th>Volume Name</th>
										<th>Application Name</th>
										<th>App Type</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: appList, afterRender: afterRenderer }">
									<tr class='clickable-row' data-bind="click: $root.clickRow, attr: { id: 'COMPAPPS_' + $index(), text: id() }">
										<td data-bind="text: volumeName"></td>
										<td data-bind="text: appName"></td>
										<td data-bind="text: lookupAppType(appType())"></td>
									</tr>
								</tbody>
							</table>							
						</div> 
					</div>

					<div class="tab-pane fade" id="isoCdTab">
						<div id='isoCdTab_top' style="margin-bottom: 20px;">
							<button id='btn_add_IsoItem' class="btn btn-success" style="display: none" onclick="return false;"><i class="fa fa-plus"></i>&nbsp;Add Record</button>
						</div>

						<div class='col-md-12 table-responsive pre-scrollable' id="isoCdDiv">
							<table class="table table-hover" id="isoCdTbl">
								<thead>
									<tr>
										<th>Application Name</th>
										<th>CD Name</th>
										<th>Volume Name</th>
										<th>App Type</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: appList }">
									<tr class='clickable-row' data-bind="click: $root.clickRow, attr: { id: 'ISO_' + id() }">
										<td data-bind="text: appName"></td>
										<td data-bind="text: cdName"></td>
										<td data-bind="text: volumeName"></td>
										<td data-bind="text: lookupAppType(appType())"></td>
									</tr>
								</tbody>
							</table>							
						</div>

					</div>

					<div class="tab-pane fad" id="combMediaTab">

						<div class='col-xs-12 table-responsive pre-scrollable' id="combMediaDiv">
							<table class="table table-hover" id="combMediaTbl">
								<thead>
									<tr>
										<th>Application Name</th>
										<th>CD Name</th>
										<th>Volume Name</th>
										<th>App Type</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: appList }">
									<tr class='clickable-row' data-bind="click: $root.clickRow, attr: { id: 'COMBINED_' + $index() }">
										<td data-bind="text: appName"></td>
										<td data-bind="text: cdName"></td>
										<td data-bind="text: volumeName"></td>
										<td data-bind="text: lookupAppType(appType())"></td>
									</tr>
								</tbody>
							</table>
						</div>

					</div>

					<div class="tab-pane fad" id="appTypesTab">
						<div id='appTypesTab_top' style="margin-bottom: 20px;">
							<button id='btn_add_AppTypeItem' class="btn btn-success" style="display: none" onclick="return false;"><i class="fa fa-plus"></i>&nbsp;Add Record</button>
						</div>

						<div class='col-xs-12 table-responsive pre-scrollable' id="appTypesDiv">
							<table class="table table-hover" id="appTypesTbl">
								<thead>
									<tr>
										<th>Type Code</th>
										<th>Description</th>
										<th>Long Description</th>
									</tr>
								</thead>
								<tbody data-bind="foreach: { data: appTypesList }">
									<tr class='clickable-row' data-bind="click: $parent.clickRow, attr: { id: 'APPTYPE_'+type_id() }">
										<td data-bind="text: type_id"></td>
										<td data-bind="text: description"></td>
										<td data-bind="text: longDescription"></td>
									</tr>
								</tbody>
							</table>

							<!-- select data-bind="options: AppTypes, optionsValue: 'type_id', optionsText: 'description'"></ -->
						</div>

					</div>

				</div>
			</div>
		</div>
	</div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

	<!-- Hidden FilterSelection  Dialog box ----------------------------------------------------->
	<div id="filterDlg" class="modal fade ">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="FilterDlgHeader"><i class="glyphicon glyphicon-filter"></i>&nbsp;App Type Filter </h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">
					<form name="FilterDlgForm" id="FilterDlgForm" method="post" style="margin-top: 1px;">
						<div class="row">
							<div class="col-md-12">
								<select class='form-control' id='filterSelect' data-bind="options: appTypesList, optionsValue: 'type_id', optionsText: 'longDescription', optionsCaption: '[None]'"></select>
							</div>
						</div>
					</form>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary" id="filterDlgBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Apply</button>
				</div>
				&nbsp;
			</div>
		</div>
	</div>
	<!-- END Hidden FilterSelection Dialog box -------------------------------------------------->

	<!-- Hidden Search  Dialog box -------------------------------------------------------------->
	<div id="searchDlg" class="modal fade">
		<div class="modal-dialog modal-md">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="searchDlgHeader"><i class="glyphicon glyphicon-search"></i>&nbsp;App Search </h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">
					<form name="searchDlgForm" id="searchDlgForm" method="post" style="margin-top: 1px;">
						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="searchTerm">Search Term</label>
								<div class="col-sm-8">
									<input type="text" id='searchTerm' name='searchTerm' class="form-control" style="min-width: 100%" maxlength="50" value="" required />
								</div>
							</div>
						</div>
					</form>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button type="button" class="btn btn-primary" id="searchDlgBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Run Search</button>
				</div>
				&nbsp;
			</div>
		</div>
	</div>
	<!-- END Hidden Search Dialog box -------------------------------------------------------------->

	<!-- AppType Edit Dialog box ------------------------------------------------------------------->
	<div id="AppTypeEditDlg" class="modal fade">
		<div class="modal-dialog modal-md" data-bind="with: selectedAppType">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="AppTypeEditDlgHeader">App Type Edit</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">
					<form name="AppTypeEditDlgForm" id="AppTypeEditDlgForm" method="post" style="margin-top: 1px;">
						<input type="hidden" id="AppTypeEditDlgEditType" name="dbActionType" value="Create" />
						<input type="hidden" id="type_id_orig" name="type_id_orig" value="" />

						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="type_id">Type ID</label>
								<div class="col-sm-8">
									<input type="text" id='type_id' name='type_id' data-bind="value: type_id" class="form-control" style="min-width: 100%" maxlength="2" value="" required />
								</div>
							</div>
						</div>
						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="description">Description</label>
								<div class="col-sm-8">
									<input type="text" id='description' name='description' data-bind="value: description" class="form-control" style="min-width: 100%" maxlength="10" value="" required />
								</div>
							</div>
						</div>
						<div class="row">
							<div class="col-md-12">
								<label class="col-sm-4" for="longDescription">Long Description</label>
								<div class="col-sm-8">
									<input type="text" id='longDescription' name='longDescription' data-bind="value: longDescription" class="form-control" style="min-width: 100%" maxlength="25" value="" required />
								</div>
							</div>
						</div>
					</form>
				</div>

				<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
					<button class="btn btn-primary" id="AppTypeEditDlgBtnSubmit"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Save</button>
					<button class="btn btn-danger" id="AppTypeEditDlgBtnDelete"><span class='glyphicon glyphicon-trash' style='margin-right: 8px;'></span>Delete</button>
					<button class="btn btn-warning" id="AppTypeEditDlgBtnClose"><span class='glyphicon glyphicon-remove'></span>Close</button>
				</div>
				&nbsp;
			</div>
		</div>
	</div>
	<!-- END Hidden Search Dialog box -------------------------------------------------------------------->

	<!-- App Edit Dialog box ----------------------------------------------------------------------------->
	<div id="AppEditDlg" class="modal fade">
		<div class="modal-dialog modal-lg">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="AppEditDlgHeader">App Edit</h4>
				</div>

				<div class="modal-body" style="margin-top: 1px;">
					<form name="AppEditDlgForm" id="AppEditDlgForm" method="post" style="margin-top: 1px;">
						<input type="hidden" id="AppEditDlgEditType" name="dbActionType" value="Create" />
						<input type="hidden" id ="filetype" name="filetype" data-bind="value: appItem.filetype" />
						<input type="hidden" name="id" data-bind="value: appItem.id" />

						<div class="row">
							<div class="col-md-6">

								<div class="row">
									<div class="col-md-12">
										<h2 style="color: green"><i class="glyphicon glyphicon-cd"></i>&nbsp;ISO CD Detail</h2>
									</div>
								</div>

								<div class="row">
									<div class="col-md-12">
										<label class="col-sm-8" for="id">App ID</label>
										<div class="col-sm-4">
											<input type="text" id='displayid' name='displayid' data-bind="value: appItem.id" class="form-control" style="min-width: 100%" maxlength="5" value="" disabled="true" />
										</div>
									</div>
								</div>

								<div class="row">
									<div class="col-md-12">
										<label class="col-sm-5" for="appName">App Name</label>
										<div class="col-sm-7">
											<input type="text" data-bind="value: appItem.appName" id='appName' name='appName' class="form-control" style="min-width: 100%" maxlength="128" value="" required />
										</div>
									</div>
								</div>

								<div class="row">
									<div class="col-md-12">
										<label class="col-sm-5" for="cdName">CD Name</label>
										<div class="col-sm-7">
											<input type="text" data-bind="value: appItem.cdName" id='cdName' name='cdName' class="form-control" style="min-width: 100%" maxlength="64" value="" required />
										</div>
									</div>
								</div>

								<div class="row">
									<div class="col-md-12">
										<label class="col-sm-5" for="volumeName">Volume Name</label>
										<div class="col-sm-7">
											<input type="text" data-bind="value: appItem.volumeName" id='volumeName' name='volumeName' class="form-control" style="min-width: 100%" maxlength="64" value="" required />
										</div>
									</div>
								</div>

								<div class="row">
									<div class="col-md-12">
										<label class="col-sm-5" for="appType">App Type</label>
										<div class="col-sm-7">
											<select class='form-control' id='appType' name='appType' data-bind="value: appItem.appType, options: appTypesVM.appTypesList(), optionsValue: 'type_id', optionsText: 'longDescription'"></select>
										</div>
									</div>
								</div>

							</div>

							<div class="col-md-6">
								<div class="row">
									<div class="col-md-12">
										<h2 style="color: green"><i class="glyphicon glyphicon-folder-close"></i>&nbsp;Compilation CD Detail</h2>
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-7" for="appDate">App Date</label>
									<div class="col-sm-5">
										<input data-bind="value: appItem.appDate" data-date-format="mm/dd/yyyy" data-provide="datepicker" type="text" id='appDate' name='appDate' class="form-control" style="min-width: 100%" maxlength="64" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-7" for="volumeName">Number of Files</label>
									<div class="col-sm-5">
										<input type="text" data-bind="value: appItem.numFiles" id='numFiles' name='numFiles' class="form-control" style="min-width: 100%" maxlength="20" value="" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-7" for="numBytes">Number of Bytes</label>
									<div class="col-sm-5">
										<input type="text" data-bind="value: appItem.numBytes" id='numBytes' name='numBytes' class="form-control" style="min-width: 100%" maxlength="32" value="" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-7" for="numSubDirs">Number of Subdirs</label>
									<div class="col-sm-5">
										<input type="text" data-bind="value: appItem.numSubDirs" id='numSubDirs' name='numSubDirs' class="form-control" style="min-width: 100%" maxlength="10" value="" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-12" for="path">Path on Disk</label>
									<div class="col-sm-12">
										<input type="text" data-bind="value: appItem.path" id='path' name='path' class="form-control" style="min-width: 100%" maxlength="256" value="" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-12" for="fileTypes">File Types</label>
									<div class="col-sm-12">
										<input type="text" data-bind="value: appItem.fileTypes" id='fileTypes' name='fileTypes' class="form-control" style="min-width: 100%" maxlength="30" value="" required />
									</div>
								</div>

								<div class="col-md-12">
									<label class="col-sm-4" for="fileTypes">Private</label>
									<div class="col-sm-8">
										<input type="checkbox" data-bind="checked: appItem.private" id='private' name='private' class="form-check-input" />
									</div>
								</div>

							</div>

						</div>

						<div class="row">
							<label class="col-sm-5" for="notes">Notes</label>
							<div class="col-md-12">
								<textarea id="notes" data-bind="value: appItem.notes" name="notes" rows="5" class="form-control"></textarea>
							</div>
						</div>

					</form>

					<div class="row" style="margin-top: 4px;">
						<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
							<button class="btn btn-primary" id="AppEditDlgBtnSubmit" name="AppEditDlgBtnSubmit"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
							<button class="btn btn-danger" id="AppEditDlgBtnDelete" name="AppEditDlgBtnDelete"><span class='glyphicon glyphicon-trash' style='margin-right: 8px;'></span>Delete</button>
							<button class="btn btn-warning" id="AppEditDlgBtnClose" name="AppEditDlgBtnClose" data-dismiss="modal"><span class='glyphicon glyphicon-remove'></span>Close</button>
						</div>
					</div>

				</div>
			</div>

		</div>
	</div>

	<!-- END Hidden AppEdit Dialog box ------------------------------------------------------------------------------>

	<script>
		//https://opensoul.org/2011/06/23/live-search-with-knockoutjs/

		var getSoftwareDBCombinedListUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBCombinedCdAppList") %>";
		var getSoftwareDBCompCdNameListUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBCompCdNameList") %>";
		var getSoftwareDBCompCdAppListUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBCompCdAppList") %>";
		var getSoftwareDBIsoCdListUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBIsoCdList") %>";
		var getSoftwareDBAppTypeListUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBAppTypeList") %>";
		var getSoftwareDBSearchResultsUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/getSoftwareDBSearchResults") %>";
		var writeSoftwareDBAppTypeUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/writeSoftwareDBAppType") %>";
		var writeSoftwareDBAppRecUrl = "<%= ResolveClientUrl("~/WebServices/PortalServices.svc/writeSoftwareDBAppRec") %>";

		var appTypeFilter = "";
		var searchTextInbound = '<%=searchTxt%>';

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

		//AppTypes array will look like...
		//[{"type_id":"WB","description":"Website","longDescription":"Website Files"},{...}]

		//var AppTypes = ko.observableArray();

		var AppTypeItem = function (data) {
			var self = this;
			self.type_id = ko.observable(data.type_id);
			self.description = ko.observable(data.description);
			self.longDescription = ko.observable(data.longDescription);			
		};

		//Generic data type for both cdIso and cdComp.  filetype differenciates.
		var AppItem = function (data) {
			var self = this;
			self.id = ko.observable(data.id);
			self.appName = ko.observable(data.appName);
			self.cdName = ko.observable(data.cdName);
			self.volumeName = ko.observable(data.volumeName);
			self.appType = ko.observable(data.appType);
			self.notes = ko.observable(data.notes);

			self.appDate = ko.observable(parseJSONDate(data.appDate));
			self.numFiles = ko.observable(data.numFiles);
			self.numBytes = ko.observable(data.numBytes);
			self.numSubDirs = ko.observable(data.numSubDirs);
			self.path = ko.observable(data.path);
			self.fileTypes = ko.observable(data.fileTypes);
			self.filetype = ko.observable(data.filetype);

			self.private = ko.observable(data.private);

		};


		var AppItemObj = function (data) {
			var self = this;
			self.id = data.id();
			self.appName = data.appName();
			self.cdName = data.cdName();
			self.volumeName = data.volumeName();
			self.appType = data.appType();
			self.notes = data.notes();

			self.appDate = data.appDate();
			self.numFiles = data.numFiles();
			self.numBytes = data.numBytes();
			self.numSubDirs = data.numSubDirs();
			self.path = data.path();
			self.fileTypes = data.fileTypes();
			self.filetype = data.filetype();

			self.private = data.private();
		};


		function AppObject(_filetype) {
			var x = new Object();
			x.id = -1;
			x.appName = 'test new';
			x.cdName = '';
			x.volumeName = '';
			x.appType = '00';
			x.notes = '';
			x.appDate = '/Date(0)/'; //our parser will make it today's date
			x.numFiles = 0;
			x.numBytes = 0;
			x.numSubDirs = 0;
			x.path = '';
			x.filetype = _filetype;
			x.fileTypes = '';
			x.private = 0;
			return x;
		}

		var currentVM = "";

		function GenericCdVM(instanceName) {

			var self = this;
			self.instanceName = instanceName;
			self.appList = ko.observableArray();
			self.selected = ko.observable();

			self.replaceSelected = function (editedRow) {
				self.appList.replace(self.selected, editedRow);
			}

			self.addNewRow = function (newItem) {
				self.appList.unshift(newItem);
				self.selected=newItem;
			};

			self.removeSelectedRow = function () {
				if (self.selected != null) {
					self.appList.remove(self.selected);
					self.selected = null;
				}
			};

			self.clickRow = function (which) {

				//This is my sketchy way of preping an app record
				//for edit into the dialog box.  I dont want the underlying 
				//ui table to be changed until the user commits the dialog edits,
				//so a copy of the selected row data is prepped and sent to the 
				//dialog.  Not the best approach Im sure.
				currentVM = self.instanceName;
				self.selected = which;
				var rowForEdit = new AppItemObj(which);
				appsDlgVM.appItem = new AppItem(rowForEdit);
				$('#AppEditDlgEditType').val("Update");

				//appDlg.appItem = which;
				//this is puzzling
				ko.cleanNode(document.getElementById('AppEditDlg'));
				ko.applyBindings(appsDlgVM, document.getElementById('AppEditDlg'));
			
				setCompDetailDlgAttrs(which);
				$('#AppEditDlgHeader').html("App Edit");
				$("#AppEditDlg").modal('show');
			};

			//http://www.knockmeout.net/2012/04/knockoutjs-performance-gotcha.html
			self.loadData = function (newData) {
				var newList = ko.utils.arrayMap(newData, function (item) {
					return new AppItem(item);
				});
				self.appList(newList);
			};

			self.afterRenderer = function () {
				//placeholder
			};

		};
		
		function AppTypesVM() {

			var self = this;		
			self.appTypesList = ko.observableArray();
			self.selectedAppType = ko.observable(null);
			self.addIsDirty = false;

			self.clickRow = function (which) {
				//https://stackoverflow.com/questions/34193238/knockout-js-geting-a-modal-popup-to-edit-items				
				self.addIsDirty = false;
				self.selectedAppType(which);
				launchAppTypeEdit(which.type_id());
			};

			//add a new appType to the top of the array.   It is blank to 
			//allow for editing.  Making it the selectedAppType allows for 
			//immediate edit in the apptypeedit dialog box.
			self.addNewRow = function () {
				var x = new Object();
				x.type_id = "!!";
				x.description = "";
				x.longDescription = "";
				var newItem = new AppTypeItem(x);
				self.appTypesList.unshift(newItem);
				self.selectedAppType(newItem);
				self.addIsDirty = true;
			};
	

			//remove the appType at the selectedRow postion from the VM.  This 
			//function is useful for delete, and cancelling an addNewRow that has
			//not been sent to server yet (addIsDirty should be set to false in your code as soon as
			//possible after successful send to server.)
			self.removeSelectedRow = function () {
				if (self.selectedAppType != null) {
					self.appTypesList.remove(self.selectedAppType());
					self.selectedAppType(null);
				}
			};

			self.loadData = function (newData) {
				var newList = ko.utils.arrayMap(newData, function (item) {					
					return new AppTypeItem(item);
				});			
				self.appTypesList(newList);
			};

		};

		function AppsDlgVM() {

			var self = this;
			self.appItem = ko.observable();
		
			self.prepNew = function (_filetype) {
				var newItem = new AppItem(AppObject(_filetype));
				self.appItem = newItem;
				setCompDetailDlgAttrs(newItem);
				//this is puzzling
				ko.cleanNode(document.getElementById('AppEditDlg'));
				ko.applyBindings(appsDlgVM, document.getElementById('AppEditDlg'));
			};

		};

		function CompCdNamesVM() {

			var self = this;
			self.cdnames = ko.observableArray();
			self.fetchCompCdApps = function (which) {
				
				var arg = JSON.stringify({ "cdName": which.cdName, "appTypeFilter": appTypeFilter });
				$('#btn_add_compItem').attr("style", "display: normal");
				fetchData("getSoftwareDBCompCdAppListUrl", arg);
			};

			//is called on every foreach completion. 
			self.updateCompCdTbl = function () {
				if ($('#compCdTbl tr').length === self.cdnames().length) { //do it after last row				
				}
			};

		};

		var compCdNamesVM = new CompCdNamesVM();		
		var compCdAppsVM  = new GenericCdVM('compCdAppsVM');		
		var isoCdVM = new GenericCdVM('isoCdVM');
		var combMediaVM   = new GenericCdVM('combMediaVM');
		var searchVM      = new GenericCdVM('searchVM');
		var appTypesVM    = new AppTypesVM();
		var appsDlgVM     = new AppsDlgVM();

		ko.applyBindings(appTypesVM, document.getElementById('appTypesDiv'));
		ko.applyBindings(appTypesVM, document.getElementById('AppTypeEditDlg'));
		ko.applyBindings(appTypesVM, document.getElementById('filterDlg'));

		ko.applyBindings(compCdNamesVM, document.getElementById('compCdDiv'));
		ko.applyBindings(compCdAppsVM, document.getElementById('compCdAppsDiv'));
		ko.applyBindings(isoCdVM, document.getElementById('isoCdDiv'));
		ko.applyBindings(combMediaVM, document.getElementById('combMediaDiv'));		
		ko.applyBindings(searchVM, document.getElementById('searchDiv'));		

		ko.applyBindings(appsDlgVM, document.getElementById('AppEditDlg'));

		function fetchData(whichUrl, dat) {

			var pleaseWaitDlg = bootbox.dialog({
				message: '<p class="text-center"><i class="fa fa-spin fa-spinner"></i> Fetching data, please wait...</p>',
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
				url: eval(whichUrl),
				contentType: "application/json",
				data: dat,
				dataType: "json",
				dataFilter: function (data) {
					var msg = eval('(' + data + ')');
					if (msg.hasOwnProperty('d'))
						return msg.d;
					else
						return msg;
				},
				success: function (content, textStatus, jqXHR) {

					var status = "";
					if (content.length > 0 && content[0].hasOwnProperty("status")) {
						status = content[0].status;
						if (status.indexOf("Error:") >= 0) {
							pleaseWaitDlg.modal('hide');
							bootbox.alert("Result: " + status);
						}
					}

					if (whichUrl == "getSoftwareDBCompCdNameListUrl") {
						compCdNamesVM.cdnames(content);
					} else if (whichUrl == "getSoftwareDBCompCdAppListUrl") {
						$('#compCdAppsTbl').attr("style", "display: normal");
						compCdAppsVM.loadData(content);
						$('#btn_add_CompItem').attr("style", "display: normal");
					} else if (whichUrl == "getSoftwareDBIsoCdListUrl") {
						isoCdVM.loadData(content);
						$('#btn_add_IsoItem').attr("style", "display: normal");
					} else if (whichUrl == "getSoftwareDBCombinedListUrl") {
						combMediaVM.loadData(content);
					} else if (whichUrl == "getSoftwareDBSearchResultsUrl") {
						searchVM.loadData(content);
					} else if (whichUrl == "getSoftwareDBAppTypeListUrl") {
						appTypesVM.loadData(content);
						$('#btn_add_AppTypeItem').attr("style", "display: normal");
					} else if (whichUrl == "writeSoftwareDBAppTypeUrl") {
						appTypesVM.addIsDirty = false;  //if write was for an add, this is needed
		
					} else if (whichUrl == "writeSoftwareDBAppRecUrl") {

						//Though the underlying table that listed the record can be
						//undated onscreen via ko, we dont want that since the user can 
						//cancel their edits.  DOnt know how to roll back the edits so.
						var editedRow = appsDlgVM.appItem;
						var addRecNo= -1;
						var delRecNo = -1;
						//an add record action will return the added records id if successful
						if (status.indexOf("Success: AddRec=") >= 0) {
							var n = status.indexOf("=");
							if (n > 0) {
								addRecNo = status.substr(n + 1);
								editedRow.id(addRecNo);								
							}							
						} else if (status.indexOf("Success: DelRec=") >= 0) {
							var n = status.indexOf("=");
							if (n > 0) {
								delRecNo = status.substr(n + 1);
							}
						}

						switch (currentVM) {
							
							case "isoCdVM":
								if (addRecNo >= 0) { //was add rec
									isoCdVM.addNewRow(editedRow);								
								} else if (delRecNo >= 0) { //was delete rec
									isoCdVM.removeSelectedRow();
								} else { //was an edit
									isoCdVM.replaceSelected(editedRow);
								}
								$("#AppEditDlg").modal('hide');
								break;

							case "combMediaVM":
								 if (delRecNo >= 0) { //was delete rec
									combMediaVM.removeSelectedRow();
								} else { //was an edit
									combMediaVM.replaceSelected(editedRow);
								}
								$("#AppEditDlg").modal('hide');
								break;

							case "searchVM":
								if (delRecNo >= 0) { //was delete rec
									searchVM.removeSelectedRow();
								} else { //was an edit
									searchVM.replaceSelected(editedRow);
								}
								$("#AppEditDlg").modal('hide');
								break;

							case "compCdAppsVM":								
								if (addRecNo >= 0) { //was add rec

									//TODO: If the new app is the first one added to the system
									//for a new comilation CD, then no screen refresh will occur.  The
									//names panel will need to be refreshed with the new cd name and 
									//the detail panel as well.  Also, BUG:  If an existing App's cdName changes
									//to something that is now unique, the screen will not be refresh accurately either. 
									//Leaving these as known issues for now since this project has been painful.

									compCdAppsVM.addNewRow(editedRow);
								} else if (delRecNo >= 0) { //was delete rec
									compCdAppsVM.removeSelectedRow();
								} else { //was an edit
									compCdAppsVM.replaceSelected(editedRow);
								}
								$("#AppEditDlg").modal('hide');
								break;

							default:
								break;
						}
						
					} else {
						//shoud never happen
						bootbox.alert("Error.  An unknown service url was called: " + whichUrl);
					}
					pleaseWaitDlg.modal('hide');

				},
				complete: function () {
					//setTimeout(update, 5000);
				},
				error: function (x, y, z) {
					pleaseWaitDlg.modal('hide');
					bootbox.alert("Service Error: " + x + ' ' + whichUrl + ' ' + z);
				}
			});

		}

		function lookupAppType(targetType) {

			var match = ko.utils.arrayFirst(appTypesVM.appTypesList(), function (item) {
				return targetType === item.type_id();
			});
			return match ? match.description : "**BAD TYPE ID**";

		}

		function activateTab(tab) {

			$(tab + "li").click();
			$('.nav-tabs a[href="' + tab + '"]').tab('show');

			if (tab == "#searchTab")
				launchSearchDlg();

		};
		
		//Start up
		$(function () {

			$('#compCdTabli').click(function (e) {

				$('#compCdTbl tr').slice(1).remove();
				$('#compCdAppsTbl tr').slice(1).remove();
				$('#btn_add_CompItem').attr("style", "display: normal");
				fetchData("getSoftwareDBCompCdNameListUrl", "");
			});

			$('#isoCdTabli').click(function (e) {
				$('#isoCdTbl tr').slice(1).remove();
				$('#btn_add_IsoItem').attr("style", "display: none");
				fetchData("getSoftwareDBIsoCdListUrl", JSON.stringify({ "appTypeFilter": appTypeFilter }));
			});

			$('#combMediaTabli').click(function (e) {
				$('#combMediaTbl tr').slice(1).remove();
				fetchData("getSoftwareDBCombinedListUrl", JSON.stringify({ "appTypeFilter": appTypeFilter }));
			});

			$('#appTypesTabli').click(function (e) {
				$('#appTypesTbl tr').slice(1).remove();
				$('#btn_add_AppTypeItem').attr("style", "display: none");
				fetchData("getSoftwareDBAppTypeListUrl", "");
			});

			$('#filterTab').click(function (e) {
				$('#searchTbl tr').slice(1).remove();
			});

			$('#compCdTbl').on('click', '.clickable-row', function (event) {
				currRowID = $(this).closest('tr').attr('id');
				$('#compCdTbl tr').attr("style", "background-color: white;");
				$('#' + currRowID).attr("style", "background-color: #C6E2EE;")
			});

			$('#compCdAppsTbl').on('click', '.clickable-row', function (event) {
				currRowID = $(this).closest('tr').attr('id');
				$('#compCdAppsTbl tr').attr("style", "background-color: white;");
				$('#' + currRowID).attr("style", "background-color: #C6E2EE;")
			});

			$('#isoCdTbl').on('click', '.clickable-row', function (event) {
				currRowID = $(this).closest('tr').attr('id');
				$('#isoCdTbl tr').attr("style", "background-color: white;");
				$('#' + currRowID).attr("style", "background-color: #C6E2EE;")
			});

			$('#combMediaTbl').on('click', '.clickable-row', function (event) {
				currRowID = $(this).closest('tr').attr('id');
				$('#combMediaTbl tr').attr("style", "background-color: white;");
				$('#' + currRowID).attr("style", "background-color: #C6E2EE;")
			});

			//Magical block of functions that sorts tables based on column header click
			$('th').click(function () {
				var table = $(this).parents('table').eq(0)
				var rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()))
				this.asc = !this.asc
				if (!this.asc) { rows = rows.reverse() }
				for (var i = 0; i < rows.length; i++) { table.append(rows[i]) }
			})
			function comparer(index) {
				return function (a, b) {
					var valA = getCellValue(a, index), valB = getCellValue(b, index)
					return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB)
				}
			}
			function getCellValue(row, index) { return $(row).children('td').eq(index).text() }

			//Must have AppTypes loaded before app tabs can be properly displayed
			fetchData("getSoftwareDBAppTypeListUrl", "");

		});


		function ChangeFilter() {
			$("#filterDlg").modal('show');
		}

		$('#filterDlgBtn').click(function () {

			$("#filterDlg").modal('hide');

			//grab the new filter code and update the
			//filterText display
			var icon = "<i class='glyphicon glyphicon-filter'></i>&nbsp;";
			var sel = $("#filterSelect :selected").text();
			appTypeFilter = $("#filterSelect :selected").val();
			$('#filterTabli').html(icon + "Filter: " + sel);

			//refresh the contents of the active tab, whereupon
			//click, the backend will be called with the new filter arg
			var id = $('.nav-tabs .active > a').attr('href');
			$(id + 'li').click();
		});

		function launchSearchDlg() {
			$("#searchDlg").modal('show');
		}

		//Get Search Results
		$('#searchDlgBtn').click(function () {
			$('#searchTbl tr').slice(1).remove();

			$("#searchDlg").modal('hide');
			var searchTerm = $("#searchTerm").val();
			fetchData("getSoftwareDBSearchResultsUrl", JSON.stringify({ "searchTerm": searchTerm }));
		});

		//Launch AppType edit for EDIT
		function launchAppTypeEdit(origType_id) {

			$('#AppTypeEditDlgHeader').html("Edit App Type");
			$("#AppTypeEditDlgBtnDelete").prop("disabled", false);
			$('#type_id_orig').val(origType_id);
			$('#AppTypeEditDlgEditType').val("Update");
			$("#AppTypeEditDlg").modal('show');
		}

		//Launch AppType edit for Add Record
		$('#btn_add_AppTypeItem').click(function () {

			appTypesVM.addNewRow();

			$('#AppTypeEditDlgHeader').html("Add New App Type");
			$("#AppTypeEditDlgBtnDelete").prop("disabled", true);
			$('#type_id_orig').val("");			
			$('#AppTypeEditDlgEditType').val("Create");
			$("#AppTypeEditDlg").modal('show');
		});

		//Save AppType Edit
		$(document).delegate("#AppTypeEditDlgBtnSubmit", "click", function (event) {
			$("#AppTypeEditDlg").modal('hide');
			fetchData("writeSoftwareDBAppTypeUrl", JSON.stringify($('#AppTypeEditDlgForm').serializeObject()));
		});

		//Close AppType Dialog
		$(document).delegate("#AppTypeEditDlgBtnClose", "click", function (event) {
			if (appTypesVM.addIsDirty) {
				appTypesVM.removeSelectedRow();
			}
			$("#AppTypeEditDlg").modal('hide');
		});

		//Send AppType Delete request
		$(document).delegate("#AppTypeEditDlgBtnDelete", "click", function (event) {

			var appTypeName = $('#longDescription').val();
			bootbox.confirm({
				title: "Delete App Type?",
				message: "Please confirm deletion of App Type: <b>" + appTypeName + "</b>",
				buttons: {
					cancel: {
						label: '<i class="fa fa-times"></i> Cancel'
					},
					confirm: {
						label: '<i class="fa fa-check"></i> Confirm'
					}
				},
				callback: function (result) {
					doDeleteAppType(result);
				}
			});

		});

		function doDeleteAppType(result) {

			if (!result) {
				return;
			}

			//serialize the form data before removing row from the VM!
			$('#AppTypeEditDlgEditType').val("Delete");
			var dat = JSON.stringify($('#AppTypeEditDlgForm').serializeObject());

			appTypesVM.removeSelectedRow();			

			//we've removed row from the VM now, lets hope the db update doesn't error,
			//otherwise it will appear to be gone from UI but still exist in db.
			$("#AppTypeEditDlg").modal('hide');
			fetchData("writeSoftwareDBAppTypeUrl", dat);
			
		}

		//App Edit - Save      
		$(document).delegate("#AppEditDlgBtnSubmit", "click", function (event) {

			var dat = JSON.stringify($('#AppEditDlgForm').serializeObject());
			fetchData("writeSoftwareDBAppRecUrl", dat);
		});

		//App Edit - Delete       
		$(document).delegate("#AppEditDlgBtnDelete", "click", function (event) {

			var appName = $('#appName').val();
			var at = $('#filetype').val();
			if (at == "cdComp") {
				at = "Compilation CD";
			} else {
				at = "ISO CD";
			}

			bootbox.confirm({
				title: "Delete "+at+"?",
				message: "Please confirm deletion of "+at+": <b>" + appName + "</b>",
				buttons: {
					cancel: {
						label: '<i class="fa fa-times"></i> Cancel'
					},
					confirm: {
						label: '<i class="fa fa-check"></i> Confirm'
					}
				},
				callback: function (result) {
					doDeleteApp(result);
				}
			});

		});
		function doDeleteApp(result) {

			if (!result) {
				return;
			}

			//serialize the form data before removing row from the VM!
			$('#AppEditDlgEditType').val("Delete");
			var dat = JSON.stringify($('#AppEditDlgForm').serializeObject());
			
			//the fetchData success callback will update the UI if the
			//delete action works successfully.  (with AppTypes, we update
			//the UI here, which is not as accurate since the deletion could fail.)

			$("#AppEditDlg").modal('hide');
			fetchData("writeSoftwareDBAppRecUrl", dat);

		}


		//App Edit - ADD New ISO
		$(document).delegate("#btn_add_IsoItem", "click", function (event) {

			$('#AppEditDlgEditType').val("Create");
			currentVM = "isoCdVM";
			appsDlgVM.prepNew('cdIso');
			$('#AppEditDlgHeader').html("ISO App--ADD");
			$("#AppEditDlg").modal('show');
		});


		//App Edit - ADD New CDComp -from CD Comp
		$('#btn_add_CompItem').click(function (e) {
			
			$('#AppEditDlgEditType').val("Create");
			currentVM = "compCdAppsVM";
			appsDlgVM.prepNew('cdComp');
			$('#AppEditDlgHeader').html("Compilation CD--ADD");
			$("#AppEditDlg").modal('show');
		});


		//Close AppEdit Dialog
		$(document).delegate("#AppEditDlgBtnClose", "click", function (event) {			
			$("#AppEditDlg").modal('hide');
		});


		function parseJSONDate(jsonDate) {

			if ((typeof jsonDate != "undefined") && jsonDate == null) {
				//assume its empty?
				console.log("parsedate bailout #1, returning empty string");
				return "";
			}

			if ((typeof jsonDate != "undefined") &&  jsonDate.indexOf("/Date") == -1) {
				//assume it does not need parsing?
				console.log("parsedate bailout #2");
				return jsonDate;
			}

			var date = new Date();
			try {
				if ((typeof jsonDate != "undefined") && jsonDate != "/Date(0)/") {
					date = new Date(parseInt(jsonDate.substr(6)));
				}
			} catch (e) { }

			function pad(s) { return (s < 10) ? '0' + s : s; }
			var ret = [pad(date.getMonth() + 1), pad(date.getDate()), date.getFullYear()].join('/');

			console.log("parsedate in="+jsonDate+"out="+ret);
			return ret;

		};

		function setCompDetailDlgAttrs(which) {

			var toggle = true;
			
			if (which.filetype() == "cdIso") {
				toggle = false;

				//enable so they can be cleared
				$('#appDate').attr("disabled", false);
				$('#numFiles').attr("disabled", false);
				$('#numBytes').attr("disabled", false);
				$('#numSubDirs').attr("disabled", false);
				$('#path').attr("disabled", false);
				$('#fileTypes').attr("disabled", false);
				$('#private').attr("disabled", false);

				//clear data from Iso type cds that comes in from backend
				//as dummy placeholders from the combined record type, as
				//these fields are meant to satisfy the ko vm objects.
				$('#appDate').val('');
				$('#numFiles').val('');
				$('#numBytes').val('');
				$('#numSubDirs').val('');
				$('#path').val('');
				$('#fileTypes').val('');
				$('#private').val('');
			}

			$('#appDate').attr("disabled", !toggle);
			$('#numFiles').attr("disabled", !toggle);
			$('#numBytes').attr("disabled", !toggle);
			$('#numSubDirs').attr("disabled", !toggle);
			$('#path').attr("disabled", !toggle);
			$('#fileTypes').attr("disabled", !toggle);
			$('#private').attr("disabled", !toggle);

		}

		// Any startup tasks go below this line.

		//Upon page load complete, launch straight into the search screen and 
		//display results if we have an inbound search term via form post.
		if (searchTextInbound != "") {
			$('.nav-tabs a[href="#searchTab"]').tab('show');
			$("#searchTerm").val(searchTextInbound); //pre-populate the dialog
			fetchData("getSoftwareDBSearchResultsUrl", JSON.stringify({ "searchTerm": searchTextInbound }));
		}
		
	</script>

</asp:Content>
