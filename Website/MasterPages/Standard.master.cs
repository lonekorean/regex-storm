using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace MasterPages
{
    public partial class Standard : MasterPageBase
    {
        #region Constructors

        public Standard()
        {
            // since all controls on the page will be namespaced with the ID of this masterpage,
            // setting it to something short keeps all control IDs short and saves bandwidth
            ID = "Z";
        }

        #endregion

        #region Events

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
            {
                // rebase inline images and links so error pages don't look funny
                string baseUrl = Request.Url.AbsoluteUri;
                if (baseUrl.Contains('?'))
                {
                    // remove query string
                    baseUrl = baseUrl.Remove(baseUrl.IndexOf('?'));
                }
                hgcBase.Attributes["href"] = baseUrl;

                // highlight active nav links
                switch (Request.AppRelativeCurrentExecutionFilePath.ToLower())
                {
                    case "~/default.aspx":
                        hlNavHome.CssClass = "nav_current";
                        hlFooterHome.CssClass = "nav_current";
                        break;
                    case "~/tester.aspx":
                        hlNavTester.CssClass = "nav_current";
                        hlFooterTester.CssClass = "nav_current";
                        break;
                    case "~/reference.aspx":
                        hlNavReference.CssClass = "nav_current";
                        hlFooterReference.CssClass = "nav_current";
                        break;
                    case "~/about.aspx":
                        hlNavAbout.CssClass = "nav_current";
                        hlFooterAbout.CssClass = "nav_current";
                        break;
                }

                // only add google analytics if exclude stats session flag is not set to false
                phGoogleAnalytics.Visible = (Session["excludestats"] == null || !Convert.ToBoolean(Session["excludestats"]));

                // copyright date
                litYear.Text = DateTime.Now.Year.ToString();
            }
        }

        #endregion
    }
}