<%@ Page Language="C#" AutoEventWireup="false" Inherits="About" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" Codebehind="About.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="description" content="The purpose behind Regex Storm, key features, credits, and a donate link. Also, rabbits." />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <asp:Image runat="server" ImageUrl="~/Images/Titles/about.png" AlternateText="About" CssClass="title" />

    <div class="section_text">
        <p>
            <strong>What is Regex Storm?</strong>
        </p>
        <p>
            Regex Storm is a website dedicated to helping you build and test .NET regular expressions. My
            primary goal was to create a .NET regex tester that was powerful and provided lots of useful information,
            while also keeping the interface clean and easy to use.
        </p>
        <p>
            To find out more or just see what's new, check out the <a href="http://regexstorm.blogspot.com/" target="_blank">Regex Storm Dev Blog</a>.
        </p>
        <p>
            Key features:
        </p>
        <ul>
            <li>Ajax-driven <asp:HyperLink runat="server" NavigateUrl="~/tester">.NET regex tester</asp:HyperLink> finds matches in real-time, as you type.</li>
            <li>Regex matches are highlighted in context, right where you typed them in.</li>
            <li>Quick tabs show all the details, such as groups, captures, positions, splits, and more.</li>
            <li>Permalink lets you bookmark your regex or share it with someone else.</li>
            <li>Complete <asp:HyperLink runat="server" NavigateUrl="~/reference">regex reference</asp:HyperLink> page links directly to helpful examples.</li>
            <li>Instantly find unfamiliar regex elements with the regex element quick search.</li>
        </ul>
        <br />
        <p>
            <strong>Tell me stuff!</strong>
        </p>
        <p>
            If you have any comments or suggestions, I want to hear them. Please send me an email at
            <span>will</span><span>@</span><span>coders</span><span class="spam-trick">-spam-misdirect-</span><span>block</span><span>.</span><span>com</span>.
        </p>
        <p>
            For more information about me, or to see my other projects, please visit <a href="http://codersblock.com/" target="_blank">Coder's Block</a>.
        </p>
        <br />
        <p>
            <strong>Credits!</strong>
        </p>
        <p>
            Thanks to the following for assets used on this site:
        </p>
        <ul>
            <li>Ubiquitous mini icons created by <a href="http://www.famfamfam.com/lab/icons/silk/" target="_blank">Fam Fam Fam</a>.</li>
            <li>Larger icons created by <a href="http://pixel-mixer.com/" target="_blank">Pixel Mixer</a>.</li>
            <li>Nifty wind brushes created by <a href="http://rubina119.deviantart.com/art/Beser-Brushes-135682641" target="_blank">Eduardo Rubina Hidalgo</a>.</li>
        </ul>
    </div>

</asp:Content>
