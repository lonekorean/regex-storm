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

public partial class NotFound : PageBase
{
    #region Constructor

    public NotFound()
    {
        // wire page events
        this.Init += new EventHandler(NotFound_Init);
    }

    #endregion

    #region Events

    protected void NotFound_Init(object sender, EventArgs e)
    {
        Title = "Page Not Found - Regex Storm";
        Response.StatusCode = 404;
    }

    #endregion
}
