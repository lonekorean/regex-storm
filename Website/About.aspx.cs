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

public partial class About : PageBase
{
    #region Constructor

    public About()
    {
        // wire page events
        this.Init += new EventHandler(About_Init);
        this.Load += new EventHandler(About_Load);
    }

    #endregion

    #region Events

    protected void About_Init(object sender, EventArgs e)
    {
        // add head stuff
        Title = "Site Info and Credits - Regex Storm";
    }

    protected void About_Load(object sender, EventArgs e)
    {
    }

    #endregion
}
