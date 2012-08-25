using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Magas.Core;
using Magas.Services;
using Pop3;
using System.Web.Security;
using Magas.Services;
using Magas.Core;

namespace Magas.Web.UI
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                Response.Redirect("Mailbox.aspx");
            }

            //txtUsername.Text = "giangi"; txtPassword.Text = "giangi";
            //btnLogin_Click(this, EventArgs.Empty);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            UserServices services = new UserServices();
            User user = services.GetUserByUsername(username);

            if (user != null && user.HasAuthenticationCredential(username, password))
            {
                FormsAuthentication.RedirectFromLoginPage(username, false);
            }
            else
            {
                this.lblError.Text = "Username or password is wrong. Please, retry";
                this.lblError.Visible = true;
            }

            //Pop3Client client = new Pop3Client(txtUsername.Text, txtPassword.Text, txtAddress.Text);
            //try
            //{
            //    client.OpenInbox();
            //    client.CloseConnection();

            //    Session["Username"] = txtUsername.Text;
            //    Session["Password"] = txtPassword.Text;
            //    Session["Address"] = txtAddress.Text;
            //    FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, false);
            //}
            //catch(Exception ex)
            //{
            //    this.lblError.Text= "Unable connect to server: could be a server error or a username/password missmatch. <br>Details: " + ex.Message;
            //    this.lblError.Visible = true;
            //}
        }
    }
}
