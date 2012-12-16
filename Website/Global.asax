<%@ Application Language="C#" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<%@ Import Namespace="System.Web.Routing" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        RegisterRoutes(RouteTable.Routes);
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
        CleanUpDomain();
    }
    
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.Add(new Route("tester", new CustomRouteHandler("~/Tester.aspx")));
        routes.Add(new Route("reference", new CustomRouteHandler("~/Reference.aspx")));
        routes.Add(new Route("about", new CustomRouteHandler("~/About.aspx")));
    }

    private void CleanUpDomain()
    {
        // start with current url
        UriBuilder redirectUrl = new UriBuilder(Request.Url);
        
        // remove www and change .com to .net (if necessary)
        redirectUrl.Host = Regex.Replace(redirectUrl.Host, "^www.", "");
        redirectUrl.Host = Regex.Replace(redirectUrl.Host, "\\.com$", ".net");

        // if anything on the redirect url was changed, then a redirect is due
        if (redirectUrl.Uri.ToString() != Request.Url.ToString())
        {
            // remove default.aspx from path before redirecting
            redirectUrl.Path = Regex.Replace(redirectUrl.Path, "/default.aspx$", "/");

            // permanently redirect to new url
            Response.AddHeader("Location", redirectUrl.Uri.ToString());
            Response.StatusCode = 301;
            Response.End();
        }

    }
       
</script>
