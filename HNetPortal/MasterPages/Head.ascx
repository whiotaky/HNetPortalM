<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Head.ascx.cs" Inherits="HNetPortal.MasterPages.Head" %>
<meta http-equiv="X-UA-Compatible" content="IE=Edge" />
<meta http-equiv="Refresh" content="1800" />
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />

<link rel="stylesheet" href="/Content/bootstrap.css" type="text/css" />
<link href="<%=ResolveClientUrl("~/Content/portal.css") %>" rel="stylesheet" />
<link href="<%=ResolveClientUrl("~/Content/HNetNavBar.css") %>" rel="stylesheet" />
<script src="<%=ResolveClientUrl("~/Scripts/jquery-3.3.1.min.js")%>"> </script>
<script src="<%=ResolveClientUrl("~/Scripts/bootstrap.js")%>"> </script>
<script src="<%=ResolveClientUrl("~/Scripts/bootbox.js")%>"> </script>
<style>
	body.modal-open, .modal-open .navbar-fixed-top, .modal-open .navbar-fixed-bottom {
		padding-right: 0px !important;
		overflow-y: auto;
	}
</style>
