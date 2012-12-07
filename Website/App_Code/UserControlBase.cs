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

public class UserControlBase : System.Web.UI.UserControl
{
    #region Properties

    public PageBase MyPage
    {
        get { return (PageBase)this.Page; }
    }

    #endregion
}
