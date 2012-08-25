using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Magas.Core;
using Magas.Services;
using Magas.Core;
using Magas.Services;

namespace Magas.Web.UI
{
    public partial class MessageDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string accountId = Request.QueryString["AccountId"];
                if (accountId == null)
                {
                    lblError.Text = "Account Id not specified. Back to mailbox";
                    lblError.Visible = true;
                    pnlMailDetail.Visible = false;
                    return;
                }

                string userId = Request.QueryString["UserId"];
                if (userId == null)
                {
                    lblError.Text = "User Id not specified. Back to mailbox";
                    lblError.Visible = true;
                    pnlMailDetail.Visible = false;
                    return;
                }

                string messageId = Request.QueryString["MessageId"];
                if (userId == null)
                {
                    lblError.Text = "Message Id not specified. Back to mailbox";
                    lblError.Visible = true;
                    pnlMailDetail.Visible = false;
                    return;
                }

                UserServices userService = new UserServices();
                User currentUser = userService.GetUserByUsername(Context.User.Identity.Name);
                if (currentUser.Id != userId)
                {
                    Response.Redirect("MailboxAccount.aspx");
                    return;
                }

                aBackToMailbox.HRef = string.Format("Mailbox.aspx?UserId={0}&AccountId={1}", userId, accountId);

                MailMessage message = new MailMessageServices().GetMailMessageOfAccountById(userId, accountId, messageId);
                lblId.Text = message.Id.ToString();
                lblFrom.Text = message.From;
                lblTo.Text = message.To;
                lblDate.Text = message.Date.ToString();
                lblSubject.Text = message.Subject;

                if (message.Format == MailMessageFormat.Text)
                {
                    lblBody.Text = message.Body.Replace("=F9", "ù").Replace("\n","<br/>").Replace("=20", string.Empty);
                }
                else
                {
                    lblBody.Text = message.Body;
                }
            }
        }
    }
}
