<%@ Page Title="" Language="C#"  MasterPageFile="~/MasterPages/FrontEnd.Master" Inherits="HNetPortal.RazorView"  ClientIDMode="Predictable" %>
      
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
          <%RenderPartial((string) ViewBag._ViewName); %>		
    </asp:Content>
