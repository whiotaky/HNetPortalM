<%@ Page Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HNetPortal.Login" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="<%=ResolveClientUrl("~/Scripts/CryptoJS v3.1.2/rollups/aes.js")%>"> </script>
	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css") %>" rel="stylesheet" />
	<link href="<%=ResolveClientUrl("~/Content/hnet_checkboxes.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<div class="container">
		<div class="Row">

			<div class="col-md-3"></div>
			<div class="col-md-6">
				<table class="table table-hover" cellspacing="0" cellpadding="0" style="border-collapse: collapse;">
					<tr>
						<td>
							<section id="loginForm">
								<div class="form-horizontal">
									<h4 style="text-align: center"><span class="glyphicon glyphicon-user"></span>Use your HNet credentials to log in.</h4>
									<hr />
									<asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="true">
										<p class="text-danger">
											<asp:Literal runat="server" ID="FailureText" />
										</p>
									</asp:PlaceHolder>
									<div class="form-group">
										<asp:Label ID="Label1" runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">User name</asp:Label>
										<div class="col-md-10">
											<asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
											<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UserName"
												CssClass="text-danger" ErrorMessage="The user name field is required." />
										</div>
									</div>
									<div class="form-group">
										<asp:Label ID="Label2" runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
										<div class="col-md-10">
											<asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
											<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
										</div>
									</div>
									<div class="form-group">
										<div class="col-md-offset-2 col-md-10">
											<div class="checkbox checkbox-primary">
												<asp:CheckBox runat="server" ID="RememberMe" Checked="true" />
												<asp:Label ID="Label3" runat="server" AssociatedControlID="RememberMe">Remember me?</asp:Label>
											</div>
										</div>
									</div>
									<div class="form-group">
										<div class="col-md-offset-2 col-md-10">
											<asp:Button ID="Button1" runat="server" CssClass="btn btn-primary" CommandName="Login" Text="Log In" ValidationGroup="Login1" OnClick="Button1_Click" />
										</div>
									</div>
									<%--<div class="form-group">										
										<div class="col-md-10" id="debugText">
										
										</div>
									</div>--%>
								</div>
							</section>
						</td>
					</tr>
				</table>
			
			</div>
		</div>

	</div>
	
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

	<script type="text/javascript">

		var k = '';

		$(document).ready(function () {

			$.ajax({
				type: "get",
				datatype: "text",
				url: '<%= ResolveClientUrl("~/api/Authentication") %>',
				success: function (ec) {
					console.log("ajax returned >" + ec + "< len=" + ec.length);
					k = ec;
					$('#debugText').html(k);
				},
				error: function (x, y, z) {
					console.log("Error " + x + ' ' + y + ' ' + z);
					k = "error";
					$('#debugText').val("***Ajax Error getting sessionKey **");
				}
			});

		});

		$(document).on('click', "#Button1", function () {

			console.log("login button click handler");
			var p1 = $('#Password').val();
			//console.log("password=" + p1 + " k=" + k + " " + k.length);

			var p2 = encrypt(p1, k, k);
			console.log("p2=" + p2 + " k=" + k);

			$('#Password').val(p2);
			//return false;
		});


		function encrypt(plainString, k, i) {

			try {
				var key = CryptoJS.enc.Utf8.parse(k);
				var iv = CryptoJS.enc.Utf8.parse(i);

				if (k.length < 16 || i.length < 16) {
					console.log("Key or iv is too short, returning null");
					return null;
				}

				if (k.length > 16 || i.length > 16) {
					console.log("Key or iv is too long, returning null");
					return null;
				}

				var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(plainString), key,
					{
						keySize: 128 / 8,
						iv: iv,
						mode: CryptoJS.mode.CBC,
						padding: CryptoJS.pad.Pkcs7
					});

				console.log("encrypt() received:'" + plainString + "', returning:" + encrypted);
				return encrypted;

			} catch (err) {
				console.log("encrypt try/catch fail");
				return null;
			}

		}

	</script>

</asp:Content>

