<%@ Page  Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="Mailbox.aspx.cs" Inherits="Magas.Web.UI.Mailbox" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:LinkButton runat="server" ID="lnkRefresh" Text="Refresh" onclick="lnkRefresh_Click" />
<br />
<br />
<asp:Panel runat="server" ID="pnlMail" Visible="true">
<asp:GridView runat="server" ID="grvMails" AutoGenerateColumns="False" DataKeyNames="Id"
        onselectedindexchanging="grvMails_SelectedIndexChanging" 
        onrowdeleting="grvMails_RowDeleting">
    <Columns>
        <asp:CommandField InsertVisible="False" SelectText="View" 
            ShowCancelButton="False" ShowSelectButton="True"/>

        <asp:TemplateField ShowHeader="False">
            <ItemTemplate>
                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                    CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this message?');"></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" />
        <asp:BoundField DataField="From" HeaderText="From" ReadOnly="True" />
        <asp:BoundField DataField="To" HeaderText="To" ReadOnly="True" />
        <asp:BoundField DataField="Date" HeaderText="Date" ReadOnly="True" />
        <asp:BoundField DataField="FolderPath" HeaderText="Path" ReadOnly="True" />
        <asp:BoundField DataField="Subject" HeaderText="Subject" ReadOnly="True" />
        <asp:BoundField DataField="BodyPreview" HeaderText="Preview " DataFormatString="{0} ..." ReadOnly="True" />
    </Columns>
</asp:GridView>
<asp:Label ID="lblNoMessage" runat="server" Text="There are no messages" visible="false"/>
</asp:Panel>
<asp:Label runat="server" ID="lblError" ForeColor="Red"/>

</asp:Content>
