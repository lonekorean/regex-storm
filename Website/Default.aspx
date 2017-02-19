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
                    <div class="update_item_title">February 19, 2017</div>
                    <div class="update_item_blurb">
                        Moved to a new host. Regex Storm is still open source. Check out the <a href="https://github.com/lonekorean/regex-storm" target="_blank">GitHub Repo</a>!
                    </div>
                </li>
            </ul>
        </div>

        <asp:Image runat="server" ImageUrl="~/Images/welcome.png" AlternateText="Welcome to Regex Storm" />
        
        <p class="welcome_intro">
            Regex Storm is a free tool for building and testing regular expressions on the .NET regex engine, featuring a comprehensive
            <asp:HyperLink runat="server" NavigateUrl="~/tester">.NET regex tester</asp:HyperLink> and complete
            <asp:HyperLink runat="server" NavigateUrl="~/reference">.NET regex reference</asp:HyperLink>.
        </p>
        <div class="front_jump_block front_jump_tester">
            <asp:HyperLink runat="server" NavigateUrl="~/tester" CssClass="front_jump_link">Regex Tester</asp:HyperLink>
            <div class="front_jump_blurb">Test regular expressions with real-time highlighting.</div>
        </div>
        <div class="front_jump_block front_jump_reference">
            <asp:HyperLink runat="server" NavigateUrl="~/reference" CssClass="front_jump_link">Regex Reference</asp:HyperLink>
            <div class="front_jump_blurb">Complete reference, with examples, for regex elements and constructs.</div>
        </div>
        <div class="front_jump_block front_jump_about">
            <asp:HyperLink runat="server" NavigateUrl="~/about" CssClass="front_jump_link">About</asp:HyperLink>
            <div class="front_jump_blurb">Additional information about Regex Storm.</div>
        </div>
    </div>

</asp:Content>
