<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="HNetPortal.MasterPages.Header" %>
<script runat="server">
	void HeadLoginStatus_LoggedOut(Object sender, System.EventArgs e) {
		WSHLib.Logger.Log("Front End Master (Header.ascx): User logged out");
	}
</script>
<nav class="navbar navbar-default navbar-fixed-top">
	<div class="container">
		<div class="navbar-header">
			<button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
			</button>
			<a runat="server" class="navbar-brand" href="~/Private/">HNet Portal</a>
		</div>
		<div class="navbar-collapse collapse">
			<asp:LoginView ID="LoginView" runat="server" ViewStateMode="Disabled">
				<AnonymousTemplate>
					<ul class="nav navbar-nav navbar-right">
						<li><a runat="server" href="~/Login.aspx"><span class="glyphicon glyphicon-lock"></span>&nbsp;Login</a></li>
					</ul>
				</AnonymousTemplate>
				<LoggedInTemplate>
					<ul class="nav navbar-nav">
						<li><a runat="server" href="~/Private/Calendar.aspx"><span class="glyphicon glyphicon-calendar"></span>&nbsp;Calendar</a></li>
						<asp:PlaceHolder ID="menuPlaceHolder" runat="server"></asp:PlaceHolder>
						<li class="dropdown">
							<a class="dropdown-toggle" href="#" data-toggle="dropdown"><i class='glyphicon glyphicon-cog'></i>&nbsp;Customize<span class="caret"></span></a>
							<ul class="dropdown-menu">
								<li><a runat="server" href="~/Private/UserSectionEdit.aspx"><span class="glyphicon glyphicon-link"></span>&nbsp;Links Edit</a></li>
								<li><a runat="server" href="~/Private/FeedMasterEdit.aspx"><span class="glyphicon glyphicon-share-alt"></span>&nbsp;Feed Master Edit</a></li>
								<li><a runat="server" href="~/Private/UserRSSEdit.aspx"><span class="glyphicon glyphicon-list-alt"></span>&nbsp;RSS Edit</a></li>
								<li><a runat="server" href="~/Private/ICalImport.aspx"><span class="glyphicon glyphicon-calendar"></span>&nbsp;Calendar Import</a></li>
							</ul>
						</li>
					</ul>

					<ul class="nav navbar-nav navbar-right">
						<li>
							<asp:LoginStatus ID="HeadLoginStatus"
								runat="server"
								LogoutAction="Redirect"
								LogoutText=" &lt;span class='glyphicon glyphicon-lock'&gt;&lt;/span&gt; Log off"
								LogoutPageUrl="~/Default.aspx"
								OnLoggedOut="HeadLoginStatus_LoggedOut" />
						</li>
					</ul>

				</LoggedInTemplate>
			</asp:LoginView>
		</div>
	</div>
</nav>
