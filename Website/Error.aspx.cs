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

public partial class Error : PageBase
{
    #region Constructor

    public Error()
    {
        // wire page events
        this.Init += new EventHandler(Error_Init);
    }

    #endregion

    #region Events

    protected void Error_Init(object sender, EventArgs e)
    {
        Title = "Error - Regex Storm";
    }

    #endregion
}
