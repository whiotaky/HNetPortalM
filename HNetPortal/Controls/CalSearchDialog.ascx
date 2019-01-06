<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalSearchDialog.ascx.cs" Inherits="HNetPortal.Controls.CalSearchDialog" %>

<!-- Hidden CalSearch  Dialog box ----------------------------------------------------->
<div id="calSearchDlg" class="modal fade">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
				<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
				<h4 class="modal-title" id="calSearchHeader">Search Calendar</h4>
			</div>
			<div class="modal-body" style="margin-top: 1px;">

				<form name="calForm" id="calSearchForm" method="post" style="margin-top: 1px;">
					<input id="calDate" name="calDate" type="hidden" value="" />



					<div class="row" style="margin-bottom: 1px;">
						<div class="form-group text-center  col-md-12 col-sm-12 col-lg-12 col-xs-12 col-xl-12">
							<h4 class="bg-primary" style="margin-bottom: 3px;">Search For:</h4>
							<div class="col-md-12">
								<input class="form-control" type="text" id="calSearchText" name="calSearchText" />
							</div>

						</div>
					</div>

					<div class="row">
						<div class="form-group text-center  col-md-12 col-sm-12 col-lg-12 col-xs-12 col-xl-12">
							<button type="button" class="btn btn-primary btn-xs" id="calSearchSubmitBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Search</button>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">
						<div class="form-group text-center  col-md-12 col-sm-12 col-lg-12 col-xs-12 col-xl-12">
							<h4 class="bg-primary" style="margin-bottom: 3px;">Search results:</h4>
							<div class="col-md-12">
								<select class="form-control" id="calSearchResults" name="calSearchResults" size="8"></select>
							</div>
							
						</div>
					</div>

					<div class="row">
						<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">						
							<button type="button" class="btn btn-primary btn-xs" id="calSearchGoBtn"><span class='glyphicon glyphicon-pencil' style='margin-right: 8px;'></span>Edit Selected</button>
							<button type="button" class="btn btn-warning btn-xs" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
						</div>
					</div>
				</form>
			</div>
		</div>
	</div>
</div>
<!-- END Hidden CalSearch Dialog box ------------------------------------------------->

