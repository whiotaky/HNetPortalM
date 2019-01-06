<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HNetPortal.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>	

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<div class="jumbotron text-center">
		<h1>The HNet Portal</h1>	
		<h2 class="text-danger"><span class="glyphicon glyphicon-ban-circle"></span>&nbsp;Private System</h2>
		<p> 
			Access is restricted to authenticated users only.<br />
		</p>		
		<a href="Login.aspx"  class="btn btn-primary btn-lg" > <span class="glyphicon glyphicon-lock"></span> Login</a>
	</div>

</asp:Content>