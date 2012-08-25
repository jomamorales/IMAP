<%@ Page  Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Magas.Web.UI.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label runat="server" ID="lblUsername">Username</asp:Label><asp:TextBox ID="txtUsername" runat="server" />
<br /><asp:Label runat="server" ID="lblPassword">Password</asp:Label><asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
<br /><asp:Label runat="server" ID="lblPop3Address" Visible="false">Pop3 address</asp:Label><asp:TextBox ID="txtAddress"  runat="server" Visible="false"/>
<br /><asp:Button ID="btnLogin" runat="server" onclick="btnLogin_Click" Text="Login"/>
<asp:Label runat="server" ID="lblError" ForeColor="Red" Text="Unable connect to server: could be a server error or a username/password missmatch" visible="false"/>
</asp:Content>
