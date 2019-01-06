<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalEditDialog.ascx.cs" Inherits="HNetPortal.Controls.CalEditDialog" %>
<!-- Hidden CalEdit Dialog box ----------------------------------------------------->
<div id="calEditDlg" class="modal fade">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
				<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
				<h4 class="modal-title" id="calEditHeader">Edit Calendar</h4>
			</div>
			<div class="modal-body" style="margin-top: 1px;">

				<form name="calForm" id="calForm" method="post" style="margin-top: 1px;">
					<input id="calDate" name="calDate" type="hidden" value="" />

					<div class="row" style="margin-bottom: 1px;">
						<div class="text-center col-sm-8 col-md-8 col-xs-8 col-xl-8 col-lg-8">
							<h4 class="bg-primary" style="margin-bottom: 1px;">Date</h4>
							<span id='calDateDisplay'></span>
						</div>
						<div class="text-center col-sm-4 col-md-4 col-xs-4 col-xl-4 col-lg-4">
							<h4 class="bg-primary" style="margin-bottom: 1px;">User</h4>
							<%=HttpContext.Current.User.Identity.Name%>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">						
						<div class="form-group text-center  col-md-12 col-sm-12 col-lg-12 col-xs-12 col-xl-12">
							<h4 class="bg-primary" style="margin-bottom: 3px;">Events</h4>
							<div class="col-md-12">
								<textarea id='calContent' name='calContent' rows="8" class="form-control" style="min-width: 100%"></textarea>
							</div>
							<div class="col-md-12">
								<small><span id="calEditDlgMsg" class="text-warning">&nbsp;</span></small>
							</div>
						</div>
					</div>
					
					<div class="row">	
					<div class="text-center col-sm-12  col-md-12 col-lg-12 col-xs-12">
						<button type="button" class="btn btn-primary btn-xs" id="calSaveBtn"><span class='glyphicon glyphicon-ok' style='margin-right: 8px;'></span>Submit</button>
						<button type="button" class="btn btn-danger btn-xs" id="calDeleteBtn"><span class='glyphicon glyphicon-trash' style='margin-right: 8px;'></span>Delete</button>
						<button type="button" class="btn btn-warning btn-xs" data-dismiss="modal"><span class='glyphicon glyphicon-remove' style='margin-right: 8px;'></span>Close</button>
					</div>
						</div>
				</form>
			</div>
		</div>
	</div>
</div>
<!-- END Hidden CalEdit Dialog box ------------------------------------------------->
