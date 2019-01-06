<%@ Page Title="Page Not Found (Error 404)" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="HNetPortal.ErrorPages._404" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

     <div class="jumbotron text-center ">
        <h1>The HNet Portal</h1>
      
    </div>


      <div class="row">
        <div class="col-lg-12">
            <h1><span class="glyphicon glyphicon-warning-sign alert alert-danger"></span>&nbsp;<%: Title %><br />             
            </h1>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12 alert alert-danger">
          <span class="glyphicon glyphicon-warning-sign"></span>&nbsp;The page you requested was not found. <a href="/">Return to Home page.</a>
        </div>
    </div>
    <!-- /.row -->
    <div class="row">
        <div class="col-md-12">
            &nbsp;
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="belowFormPlaceHolder" runat="server">
</asp:Content>
