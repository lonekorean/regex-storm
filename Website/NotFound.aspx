<%@ Page Language="C#" AutoEventWireup="false" CodeFile="NotFound.aspx.cs" Inherits="NotFound" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="robots" content="noindex,nofollow" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <asp:Image runat="server" ImageUrl="~/Images/Titles/page_not_found.png" AlternateText="Page not found" CssClass="title" />

    <div class="section_text">
        <p>
            <strong>Congratulations! You found the 404 page!</strong>
        </p>
        <p>
            Sadly, there isn't much to do here. Try going to the <asp:HyperLink runat="server" NavigateUrl="~/">homepage</asp:HyperLink>
            to find something more interesting.
        </p>
    </div>

</asp:Content>
