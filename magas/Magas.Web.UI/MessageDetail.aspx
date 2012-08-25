<%@ Page  Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="MessageDetail.aspx.cs" Inherits="Magas.Web.UI.MessageDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<a href="Mailbox.aspx" runat="server" id="aBackToMailbox">Back to mailbox</a>
<br /><br />
<asp:Panel runat="server" ID="pnlMailDetail" Visible="true">
<label><i>ID</i>: </label><asp:Label runat="server" ID="lblId"/>
<br />
<label><i>From</i>: </label><asp:Label runat="server" ID="lblFrom"/>
<br />
<label><i>To</i>: </label><asp:Label runat="server" ID="lblTo"/>
<br />
<label><i>Date</i>: </label><asp:Label runat="server" ID="lblDate"/>
<br />
<label><i>Subject</i>: </label><asp:Label runat="server" ID="lblSubject"/>
<br />
<br />
<asp:Label runat="server" ID="lblBody"/>
</asp:Panel>
<asp:Label runat="server" ID="lblError" ForeColor="Red"/>
</asp:Content>
