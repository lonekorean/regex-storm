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

public class PageBase : System.Web.UI.Page
{

    #region Variables

    private List<Control> viewStateControls = new List<Control>();
    private List<string> stylesheetHrefs = new List<string>();
    private List<string> javascriptSrcs = new List<string>();
    private Dictionary<string, string> javascriptVars = new Dictionary<string, string>();
    
    #endregion

    #region Properties

    public bool IsDevEnvironment
    {
        get { return (Request.Url.Host == "dev.regexstorm.net"); }
    }

    public string RawPath
    {
        get
        {
            string path = Request.RawUrl;
            if (path.Contains('?'))
            {
                // remove query string
                path = path.Remove(path.IndexOf('?'));
            }
            return path;
        }
    }

    #endregion

    #region Constructors

    public PageBase()
    {
        // wire page events
        this.PreInit += new EventHandler(PageBase_PreInit);
        this.PreLoad += new EventHandler(PageBase_PreLoad);
        this.PreRenderComplete += new EventHandler(PageBase_PreRenderComplete);
    }

    #endregion

    #region Events

    protected void PageBase_PreInit(object sender, EventArgs e)
    {
        this.MaintainScrollPositionOnPostBack = true;
        AddStylesheet("~/Stylesheets/Base.css");
        AddJavascript("http://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js");

        // sessoin flag for excluding stats (so I can view the site in prod without google analytics)
        string excludeStats = Request.QueryString["excludestats"] ?? "";
        if (excludeStats == "true" || IsDevEnvironment)
        {
            Session["excludestats"] = true;
        }
        else if (excludeStats == "false")
        {
            Session["excludestats"] = false;
        }
    }

    protected void PageBase_PreLoad(object sender, EventArgs e)
    {
        RecurseDisableViewState(this);
    }

    protected void PageBase_PreRenderComplete(object sender, EventArgs e)
    {
        Form.Action = Request.RawUrl;

        if (viewStateControls.Count == 0)
        {
            // blanket disable view state
            this.EnableViewState = false;
        }
        else
        {
            // first, individually disable every control's view state
            RecurseDisableViewState(this);

            // then, selectively enable specified controls and their ancestors
            Control bubbledControl;
            foreach (Control control in viewStateControls)
            {
                bubbledControl = control;
                while (true)
                {
                    bubbledControl.EnableViewState = true;
                    if (bubbledControl.Parent != null)
                    {
                        // bubble up
                        bubbledControl = bubbledControl.Parent;
                    }
                    else
                    {
                        // done
                        break;
                    }
                }
            }
        }

        if (!ScriptManager.GetCurrent(this).IsInAsyncPostBack)
        {
            // add a placeholder to the beginning of the head, in which all this other stuff will be inserted
            PlaceHolder phHeaderStuff = new PlaceHolder();
            this.Header.Controls.AddAt(0, phHeaderStuff);
            phHeaderStuff.Controls.Add(new LiteralControl("\n"));

            // stylesheet references
            foreach (string href in stylesheetHrefs)
            {
                HtmlLink stylesheetRef = new HtmlLink();
                stylesheetRef.Href = ResolveUrl(href);
                stylesheetRef.Attributes.Add("rel", "stylesheet");
                stylesheetRef.Attributes.Add("type", "text/css");
                phHeaderStuff.Controls.Add(stylesheetRef);
                phHeaderStuff.Controls.Add(new LiteralControl("\n"));
            }

            // javascript references
            foreach (string src in javascriptSrcs)
            {
                HtmlGenericControl javascriptRef = new HtmlGenericControl("script");
                javascriptRef.Attributes.Add("src", ResolveUrl(src));
                javascriptRef.Attributes.Add("type", "text/javascript");
                phHeaderStuff.Controls.Add(javascriptRef);
                phHeaderStuff.Controls.Add(new LiteralControl("\n"));
            }

            if (javascriptVars.Count > 0)
            {
                HtmlGenericControl script = new HtmlGenericControl("script");
                script.Attributes.Add("type", "text/javascript");
                script.Controls.Add(new LiteralControl("\n"));
                foreach (KeyValuePair<string, string> pair in javascriptVars)
                {
                    script.Controls.Add(new LiteralControl("\tvar " + pair.Key + " = '" + pair.Value + "';\n"));
                }
                phHeaderStuff.Controls.Add(script);
                phHeaderStuff.Controls.Add(new LiteralControl("\n"));
            }
        }
    }

    #endregion

    #region Public Methods

    public void AddViewStateControl(Control control)
    {
        viewStateControls.Add(control);
    }

    public void AddStylesheet(string href)
    {
        stylesheetHrefs.Remove(href);
        stylesheetHrefs.Add(href);
    }

    public void AddJavascript(string src)
    {
        javascriptSrcs.Remove(src);
        javascriptSrcs.Add(src);
    }

    public void AddJavascriptVar(string name, string value)
    {
        javascriptVars.Remove(name);
        javascriptVars.Add(name, value);
    }

    #endregion

    #region Private Methods

    private void RecurseDisableViewState(Control control)
    {
        // check for child controls (this is depth first)
        if (control.HasControls())
        {
            foreach (Control childControl in control.Controls)
            {
                // recurse
                this.RecurseDisableViewState(childControl);
            }
        }

        // disable view state
        control.EnableViewState = false;
    }

    #endregion
}
