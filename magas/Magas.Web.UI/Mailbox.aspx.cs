using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Magas.Core;
using Magas.Services;
using Pop3;
using Magas.Core;
using Magas.Services;
using System.Collections;

namespace Magas.Web.UI
{
    public partial class Mailbox : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {               
                this.Bind();
            }
        }

        protected void lnkRefresh_Click(object sender, EventArgs e)
        {
            this.Bind();
        }

        private void Bind()
        {
            string accountId = Request.QueryString["AccountId"];
            if (accountId == null)
            {
                lblError.Text = "Account Id not specified. Back to mailbox";
                lblError.Visible = true;
                pnlMail.Visible = false;
                return;
            }

            string userId = Request.QueryString["UserId"];
            if (userId == null)
            {
                lblError.Text = "User Id not specified. Back to mailbox";
                lblError.Visible = true;
                pnlMail.Visible = false;
                return;
            }

            UserServices userService = new UserServices();
            User currentUser = userService.GetUserByUsername(Context.User.Identity.Name);
            if (currentUser.Id != userId)
            {
                Response.Redirect("MailboxAccount.aspx");
                return;
            }

            MailMessageServices services = new MailMessageServices();
            IList<MailMessage> messages = services.GetMailMessagesOfAccount(userId, accountId);
            this.grvMails.DataSource = messages;
            this.grvMails.DataBind();
            this.grvMails.Visible = (this.grvMails.DataSource as IList).Count > 0;
            this.lblNoMessage.Visible = (this.grvMails.DataSource as IList).Count == 0;
        }

        protected void grvMails_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            string accountId = Request.QueryString["AccountId"];
            string userId = Request.QueryString["UserId"];

            this.grvMails.SelectedIndex = e.NewSelectedIndex;
            Response.Redirect(string.Format("MessageDetail.aspx?UserId={0}&AccountId={1}&MessageId={2}", userId, accountId, (string)this.grvMails.SelectedDataKey.Value.ToString()));
        }

        protected void grvMails_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string messageId = (string)this.grvMails.DataKeys[e.RowIndex].Value;
            MailMessageServices services = new MailMessageServices();

            string accountId = Request.QueryString["AccountId"];
            string userId = Request.QueryString["UserId"];

            services.DeleteMailMessageOfAccount(userId, accountId, messageId);
            this.Bind();
        }

      
    }

  

    //public class MailMessageGateway
    //{
    //    public string Username { get; private set; }
    //    public string Password { get; private set; }
    //    public string Server { get; private set; }

    //    public MailMessageGateway(string username, string password, string server)
    //    {
    //        this.Username = username;
    //        this.Password = password;
    //        this.Server = server;
    //    }

    //    public IList<MailMessage> GetAll()
    //    {
    //        Pop3Client client = null;
    //        IList<MailMessage> messages = null;

    //        try
    //        {
    //            client = new Pop3Client(this.Username, this.Password, this.Server);
    //            client.OpenInbox();

    //            messages = new List<MailMessage>();

    //            while (client.NextEmail())
    //            {
    //                MailMessage message = new MailMessage() { Id = client.InboxPosition.ToString(), To = client.To, From = client.From, Subject = client.Subject, Body = client.Body };
    //                message.Format = client.ContentType.ToLower().Contains("text") ? MailMessageFormat.Text : MailMessageFormat.Html;
    //                messages.Add(message);
    //            }
    //        }
    //        finally
    //        {
    //            client.CloseConnection();
    //        }
    //        return messages;
    //    }

    //    public MailMessage GetAllById(long id)
    //    {
    //        IList<MailMessage> messages = this.GetAll();
    //        foreach (var item in messages)
    //        {
    //            if (item.Id.Equals(id))
    //            {
    //                return item;
    //            }
    //        }
    //        return null;
    //    }
    //}
}
