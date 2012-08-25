using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Magas.Web.UI
{
    public partial class Default : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    pnlLogged.Visible = lnkLogout.Visible = true;
                    lblUser.Text = Context.User.Identity.Name;
                }
                else
                {
                    pnlLogged.Visible = lnkLogout.Visible = false;
                }
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Redirect(FormsAuthentication.DefaultUrl);
        }
    }
}
