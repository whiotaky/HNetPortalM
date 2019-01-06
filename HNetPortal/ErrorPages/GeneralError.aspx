<%@ Page Title="General Error" Language="C#" MasterPageFile="~/MasterPages/FrontEnd.Master" AutoEventWireup="true" CodeBehind="GeneralError.aspx.cs" Inherits="HNetPortal.ErrorPages.GeneralError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

     <div class="jumbotron text-center">
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
          <span class="glyphicon glyphicon-warning-sign"></span>&nbsp;An error has occurred.  <a href="/Private/">Return to Home page.</a>
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
