using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public class MasterPageBase : System.Web.UI.MasterPage
{
    #region Properties

    public PageBase MyPage
    {
        get { return (PageBase)Page; }
    }

    #endregion
}
