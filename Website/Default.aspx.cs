using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Default : PageBase
{
    #region Constructor

    public Default()
    {
        // wire page events
        this.Init += new EventHandler(Default_Init);
    }

    #endregion

    #region Events

    protected void Default_Init(object sender, EventArgs e)
    {
        // add head stuff
        Title = "Regex Storm - Online Resource for .NET Regular Expressions";
        AddStylesheet("~/Stylesheets/Default.css");
    }

    #endregion
}
