using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Magas.Core;
using Magas.Services;
using Magas.Core;
using Magas.Services;
using MailboxAccount=Magas.Core.MailboxAccount;
using MailboxAccountType=Magas.Core.MailboxAccountType;

namespace Magas.Web.UI
{
    public partial class MailboxAccounts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Bind();
            }
        }

        private void Bind()
        {
            string username = Context.User.Identity.Name;
            Core.User user = new UserServices().GetUserByUsername(username);
            this.grvAccounts.DataSource = user.MailboxAccounts;
            this.grvAccounts.DataBind();
        }

        protected void grvAccounts_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            this.grvAccounts.SelectedIndex = e.NewSelectedIndex;
            Core.User user = new UserServices().GetUserByUsername(Context.User.Identity.Name);

            Response.Redirect(string.Format("Mailbox.aspx?UserId={0}&AccountId={1}", user.Id, (string)this.grvAccounts.SelectedDataKey.Value));
        }

        protected void grvAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!(e.Row.Cells[5].Controls.Count > 0 && e.Row.Cells[5].Controls[0] is TextBox))
                {
                    e.Row.Cells[5].Text = "*********";
                }

                e.Row.Cells[2].Enabled = this.grvAccounts.EditIndex == -1;

                if (this.grvAccounts.EditIndex == e.Row.RowIndex)
                { 
                    DropDownList ddl = new DropDownList();
                    ddl.Items.Add(new ListItem("Pop3", "Pop3"));
                    ddl.Items.Add(new ListItem("Imap", "Imap"));
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Clear();
                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(ddl);
                }
            }
        }

        protected void grvAccounts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            this.Bind();
        }

        protected void grvAccounts_RowEditing(object sender, GridViewEditEventArgs e)
        {
            this.grvAccounts.EditIndex = e.NewEditIndex;
            this.Bind();
            this.lnkAdd.Enabled = false;
        }

        protected void grvAccounts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string id = (string)this.grvAccounts.DataKeys[e.RowIndex].Value;

            MailboxAccount account = new MailboxAccount();
            account.Id = id;
            account.Name = Request.Form[3];
            account.Username = Request.Form[4];
            account.Password = Request.Form[5];
            account.Server = Request.Form[6];
            account.Port = Convert.ToInt16(Request.Form[7]);
            account.UseSSL = Convert.ToBoolean(Request.Form[8]);
            account.FoldersToBeDownloadAllMessages = Request.Form[9];
            account.FoldersToBeDownloadOnlyNewMessages = Request.Form[10];
            account.AccountType = (MailboxAccountType)Enum.Parse(typeof(MailboxAccountType), Request.Form[11]);

            Core.User user = new UserServices().GetUserByUsername(Context.User.Identity.Name);
            if (string.IsNullOrEmpty(id))
            {
                new MailboxAccountServices().CreateMailboxAccount(user.Id, account);
            }
            else
            {
                new MailboxAccountServices().SaveMailboxAccount(user.Id, account);
            }
            
            this.grvAccounts.EditIndex = -1;
            this.Bind();
            this.lnkAdd.Enabled = true;
        }

        protected void grvAccounts_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            this.grvAccounts.EditIndex = -1;
            this.Bind();
            this.lnkAdd.Enabled = true;
        }

        protected void lnkAdd_Click(object sender, EventArgs e)
        {
            MailboxAccount account = new MailboxAccount();
            account.Id = string.Empty;

            string username = Context.User.Identity.Name;
            User user = new UserServices().GetUserByUsername(username);
            IList<MailboxAccount> accounts = user.MailboxAccounts;
            accounts.Add(account);
            this.grvAccounts.DataSource = accounts;
            this.grvAccounts.EditIndex = accounts.Count - 1;
            this.grvAccounts.DataBind();

            this.lnkAdd.Enabled = false;
        }
    }
}
