<%@ Page Language="C#" AutoEventWireup="false" CodeFile="Default.aspx.cs" Inherits="Default" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" %>

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
                    <div class="update_item_title">August 14, 2010</div>
                    <div class="update_item_blurb">
                        Made some minor updates around the site to keep everything running smoothly.
                    </div>
                </li>
                <li>
                    <div class="update_item_title">January 27, 2010</div>
                    <div class="update_item_blurb">
                        Big upgrades to the <a href="http://regexstorm.net/tester">Regex Tester</a>, including real-time highlighting.
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
        <div class="front_jump_block front_jump_blog">
            <asp:HyperLink runat="server" NavigateUrl="http://regexstorm.blogspot.com/" Target="_blank" CssClass="front_jump_link">Dev Blog</asp:HyperLink>
            <div class="front_jump_blurb">Take a look at what's going on behind the scenes at Regex Storm.</div>
        </div>
    </div>

</asp:Content>
