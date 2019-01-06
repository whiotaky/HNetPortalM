<%@ Page Title="HNet Password Vault" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="PwdVault.aspx.cs" Inherits="HNetPortal.Private.PwdVault" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="<%=ResolveClientUrl("~/Scripts/bootstrap-treeview.min.js")%>"> </script>
	<script src="<%=ResolveClientUrl("~/Scripts/CryptoJS v3.1.2/rollups/aes.js")%>"> </script>
	<link href="<%=ResolveClientUrl("~/Content/font-awesome.min.css")%>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

	<div class="row ">
		<div class="hidden-xs hidden-sm  hidden-md col-lg-12">
			<h1><span class="fa fa-key"></span>&nbsp; The HNet Portal - Password Vault</h1>
		</div>

		<div class="hidden-xs hidden-sm  col-md-12 hidden-lg">
			<h3><span class="fa fa-key"></span>&nbsp; The HNet Portal - Password Vault</h3>
		</div>

		<div class="hidden-xs col-sm-12  hidden-md hidden-lg">
			<h3><span class="fa fa-key"></span>&nbsp; The HNet Portal - Password Vault</h3>
		</div>

		<div class="col-xs-12 hidden-sm  hidden-md hidden-lg">
			<h5><span class="fa fa-key"></span>&nbsp; The HNet Portal - Password Vault</h5>
		</div>
	</div>

	<h3><a href="#" id="searchBtn"><i class="glyphicon glyphicon-search"></i></a></h3>

	<div class="col-md-12 col-sm-12 col-md-12 col-lg-12">
		<div id="tree"></div>
	</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">

	<!-- Hidden Dialog box ----------------------------------------------------->
	<div id="accountDisplayDlg" class="modal fade">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header alert alert-info" style="margin-bottom: 1px;">
					<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
					<h4 class="modal-title" id="accountDisplayHeader">Account Details</h4>
				</div>
				<div class="modal-body" style="margin-top: 1px;">

					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-4 text-left">
							<h4>User Name</h4>
						</div>
						<div class="col-md-7 text-left">
							<h4 id='accountUserName'>un</h4>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">

						<div class="col-md-12 text-left">
							<h4 id='accountURL'>http://</h4>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-4 text-left">
							<h4>Encrypted Password</h4>
						</div>
						<div class="col-md-7  text-left">
							<input class="form-control" name='encPassword' id="encPassword" type="text" readonly="true" />
						</div>
					</div>
					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-4 text-left">
							<h4>Clear Password</h4>
						</div>
						<div class="col-md-7 text-left">

							<div class="input-group">
								<input type="password" class="form-control pwdclear" name='clearPassword' id="clearPassword" placeholder="Enter encryption key below" />
								<span class="input-group-btn">
									<button class="btn btn-default revealclear" type="button"><i class="glyphicon glyphicon-eye-open"></i></button>
								</span>
							</div>

						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-3 text-left">
							<h4>Decryption Key</h4>
						</div>
						<div class="col-md-6 text-center">

							<div class="input-group">
								<input type="password" class="form-control pwdenc" id="encKey" value="" placeholder="Enter encryption key here" />
								<span class="input-group-btn">
									<button class="btn btn-default revealenc" type="button"><i class="glyphicon glyphicon-eye-open"></i></button>
								</span>
							</div>

						</div>
						<div class="col-md-2 text-center">
							<button type="button" aria-hidden="true" class="btn btn-primary" name="decryptBtn" id="decryptBtn">Decrypt</button>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-12 text-center" style="color: red">
							<h4 id="errorMsg"></h4>
						</div>
					</div>

					<div class="row" style="margin-bottom: 1px;">
						<div class="col-md-12 text-center">
							<button type="button" data-dismiss="modal" aria-hidden="true" class="btn btn-primary" name="closeBtn" id="closeBtn">Close</button>
						</div>
					</div>

				</div>
			</div>
		</div>
	</div>
	<!-- END Hidden Dialog box ----------------------------------------------------->

	<script>

		var treeData = <asp:Literal ID="Literal1" runat="server"></asp:Literal>;

		$('#tree').treeview({

			data: treeData,
			showTags: true,
			searchResultBackColor: 'lightskyblue',
			searchResultColor: 'black',
			onNodeSelected: function (event, data) {
				console.log(data);
				if (!data.hasOwnProperty("uuid")) {
					console.log("No uuid property found");
					return;
				}
				console.log(data.uuid);				
				$.ajax({
					url: '<%= ResolveClientUrl("~/api/Vault/") %>' + data.uuid,
					type: "GET",			
					dataType: 'json',
					contentType: "application/json",
					complete: function (xhr, statusText) {
						console.log("GET api/Vault/" + data.uuid + " xhr.status=" + xhr.status + ", statusText=" + statusText);
						switch (xhr.status) {
							case 200: // success
								console.log("Server returned 200 -- Success");
								break;
							case 204: // Delete failed
								console.log("Server returned 204 -- Delete fail");
								bootbox.alert("Server returned 204 -- Delete fail");
								break;
							case 404:
								console.log("Server returned 404 -- Service Success but operation fail");
								bootbox.alert("Server returned 404 -- Service Success but operation fail");
								break;
							case 500:
								console.log("Server returned 500 Error");
								$('#seqEditDlgMsg').html(alertGlyph + "Server returned 500 Error");
								bootbox.alert("Exception in apiController (error 500)");
								break;
							default:
								console.log("Server returned " + xhr.status + " text: " + xhr.statusText);
								bootbox.alert("Unknown status code returned from server: " + xhr.status);
						}
					},
					success: function (dat) {
						console.log(dat);
						var url = "";
						if (dat.URL != "") {
							url = "<a href='" + dat.URL + "' target='_new'>" + dat.URL + "</a>";
						}
						$(".pwdclear").attr('type', 'password');
						$(".pwdenc").attr('type', 'password');

						$("#accountDisplayHeader").html(dat.Title);
						$("#accountUserName").html(dat.UserName);
						$("#accountURL").html(url);
						$("#encPassword").val(dat.PasswordEncryptedBase64);
						$("#clearPassword").val("");
						$("#errorMsg").html("&nbsp;");

						$('#accountDisplayDlg').modal({ backdrop: 'static', keyboard: false });
						$("#accountDisplayDlg").modal('show');
					}					

				});
			}

		});


		$("#searchBtn").click(function () {
			bootbox.confirm("<H2>Search </H2>Search for: <input type='text' id='treeSearchText' />",
				function (result) {
					var searchTerm = $('#treeSearchText').val();
					if (result && searchTerm != '') {
						doSearch(searchTerm);
					} else {
						$('#tree').treeview('clearSearch');
						$('#tree').treeview('collapseAll', { silent: true });
					}
				});
		});


		function doSearch(theText) {
			$('#tree').treeview('collapseAll', { silent: true });
			$('#tree').treeview('search', [theText, {
				ignoreCase: true,
				exactMatch: false,    // like or equals
				revealResults: true,  // reveal matching nodes
			}]);
		}


		$("#decryptBtn").click(function () {

			$("#clearPassword").val("");
			var decroded = decrypt($("#encPassword").val(), $("#encKey").val(), $("#encKey").val());
			if (decroded != null) {
				$("#errorMsg").html("&nbsp;");
				$("#clearPassword").val(decroded);
			} else {
				$("#errorMsg").html("Error decroding");
				$("#dmsg").attr('style', 'visibility: norma;');
			}

		});


		function decrypt(encryptedString, k, i) {

			try {
				if (k.length < 16 || i.length < 16) {
					console.log("Key or iv is too short, returning null");
					return null;
				}
				if (k.length > 16 || i.length > 16) {
					console.log("Key or iv is too long, returning null");
					return null;
				}

				var key = CryptoJS.enc.Utf8.parse(k);
				var iv = CryptoJS.enc.Utf8.parse(i);

				var decrypted = CryptoJS.AES.decrypt(encryptedString, key, {
					keySize: 128 / 8,
					iv: iv,
					mode: CryptoJS.mode.CBC,
					padding: CryptoJS.pad.Pkcs7
				});

				decrypted = decrypted.toString(CryptoJS.enc.Utf8);

				if (decrypted.length < 1) {
					console.log("decrypt() received:'" + encryptedString + "', decrypted string is empty so returning null");
					return null;
				} else {
					console.log("decrypt() received:'" + encryptedString + "', returning: '" + decrypted + "'");
					return decrypted;
				}

			} catch (err) {
				console.log("decrypt try/catch fail");
				return null;
			}
		}

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

		//password mask/reveal code
		$(".revealenc").on('click', function () {
			var $pwd = $(".pwdenc");
			if ($pwd.attr('type') === 'password') {
				$pwd.attr('type', 'text');
			} else {
				$pwd.attr('type', 'password');
			}
		});

		$(".revealclear").on('click', function () {
			var $pwd = $(".pwdclear");
			if ($pwd.attr('type') === 'password') {
				$pwd.attr('type', 'text');
			} else {
				$pwd.attr('type', 'password');
			}
		});


	</script>
</asp:Content>
