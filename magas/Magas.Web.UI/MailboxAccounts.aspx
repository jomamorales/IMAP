<%@ Page  Title="" Language="C#" MasterPageFile="~/Default.Master" AutoEventWireup="true" CodeBehind="MailboxAccounts.aspx.cs" Inherits="Magas.Web.UI.MailboxAccounts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:GridView runat="server" ID="grvAccounts" AutoGenerateColumns="False" 
        onselectedindexchanging="grvAccounts_SelectedIndexChanging" 
        DataKeyNames="Id" 
        onrowdatabound="grvAccounts_RowDataBound" ShowFooter="true"
        onrowcancelingedit="grvAccounts_RowCancelingEdit" 
        onrowdeleting="grvAccounts_RowDeleting" onrowediting="grvAccounts_RowEditing" 
        onrowupdating="grvAccounts_RowUpdating">
    <Columns>
        <asp:CommandField InsertVisible="True" SelectText="Open" ShowSelectButton="True" />
        <asp:CommandField InsertVisible="True" ShowCancelButton="true" ShowEditButton="true" />
        
         <asp:TemplateField ShowHeader="False">
            <ItemTemplate>
                <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" 
                    CommandName="Delete" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this account?');"></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="Username" HeaderText="Username" />
        <asp:BoundField DataField="Password" HeaderText="Password" />
        <asp:BoundField DataField="Server" HeaderText="Server" />
        <asp:BoundField DataField="Port" HeaderText="Port" />
        <asp:BoundField DataField="UseSSL" HeaderText="Use SSL" />
        <asp:BoundField DataField="FoldersToBeDownloadAllMessages" HeaderText="Download all message folders" />
        <asp:BoundField DataField="FoldersToBeDownloadOnlyNewMessages" HeaderText="Download new message folders" />
        <asp:BoundField DataField="AccountType" HeaderText="Type" />
    </Columns>
</asp:GridView>
<asp:LinkButton ID="lnkAdd" runat="server" Text="Add account" 
        onclick="lnkAdd_Click" />
</asp:Content>
