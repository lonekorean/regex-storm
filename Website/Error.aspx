<%@ Page Language="C#" AutoEventWireup="false" Inherits="Error" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" Codebehind="Error.aspx.cs" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="robots" content="noindex,nofollow" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <asp:Image runat="server" ImageUrl="~/Images/Titles/something_terribly_wrong.png" AlternateText="Something has gone terribly wrong" CssClass="title" />

    <div class="section_text">
        <p>
            <strong>Oh despair on a biscuit, what have you done?!</strong>
        </p>
        <p>
            Actually, it's probably not so bad. But if you could kindly send me any info about what happened, then
            I can do my best to fix it for you. Just send an email to will at regexstorm dot net. Thanks!
        </p>
    </div>

</asp:Content>
