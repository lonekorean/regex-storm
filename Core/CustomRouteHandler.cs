using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.UI;

public class CustomRouteHandler : IRouteHandler
{
    public string VirtualPath { get; private set; }

    public CustomRouteHandler(string virtualPath)
    {
        this.VirtualPath = virtualPath;
    }

    public IHttpHandler GetHttpHandler(RequestContext requestContext)
    {
        var page = BuildManager.CreateInstanceFromVirtualPath(VirtualPath, typeof(Page)) as IHttpHandler;
        return page;
    }
}