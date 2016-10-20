<%@ Page Language="C#" AutoEventWireup="false" Inherits="Default" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" Codebehind="Default.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="description" content="Online tool for building and testing .NET regular expressions. Features a .NET regex tester and complete .NET regex reference." />
    <meta name="google-site-verification" content="p29qmsKPcJfDT4CPpEuiqKYlMyBzIzEkDajp49Qtks0" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <div class="section">
        <div class="updates_box">
            <div class="updates_title">What's New?</div>
            <ul>
                <li>
                    <div class="update_item_title">October 20, 2016</div>
                    <div class="update_item_blurb">
                        Fixed some text and highlight alignment issues. Regex Storm is still open source. Check out the <a href="https://github.com/lonekorean/regex-storm" target="_blank">GitHub Repo</a>!
                    </div>
                </li>
            </ul>
        </div>

        <asp:Image runat="server" ImageUrl="~/Images/welcome.png" AlternateText="Welcome to Regex Storm" />
        
        <p class="welcome_intro">
            Regex Storm is a free online tool for building and testing regular expressions on the .NET regex engine, featuring an ajax-powered
            <asp:HyperLink runat="server" NavigateUrl="~/tester">.NET regex tester</asp:HyperLink> and a complete
            <asp:HyperLink runat="server" NavigateUrl="~/reference">.NET regex reference</asp:HyperLink> with quick search.
        </p>
        <div class="front_jump_block front_jump_tester">
            <asp:HyperLink runat="server" NavigateUrl="~/tester" CssClass="front_jump_link">Regex Tester</asp:HyperLink>
            <div class="front_jump_blurb">Test regular expressions using the full-featured tester that finds matches as you type.</div>
        </div>
        <div class="front_jump_block front_jump_reference">
            <asp:HyperLink runat="server" NavigateUrl="~/reference" CssClass="front_jump_link">Regex Reference</asp:HyperLink>
            <div class="front_jump_blurb">A complete, searchable reference for all regex elements and constructs, with live examples.</div>
        </div>
        <div class="front_jump_block front_jump_about">
            <asp:HyperLink runat="server" NavigateUrl="~/about" CssClass="front_jump_link">About</asp:HyperLink>
            <div class="front_jump_blurb">The purpose behind Regex Storm, key features, credits, all that good stuff.</div>
        </div>
    </div>

</asp:Content>
