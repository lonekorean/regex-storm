<%@ Page Language="C#" AutoEventWireup="false" CodeFile="Reference.aspx.cs" Inherits="Reference" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" %>
<%@ Register TagPrefix="RS" TagName="ReferenceTable" Src="~/UserControls/ReferenceTable.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="description" content="Complete list of all .NET regular expression elements on one searchable reference page with explanations and live examples." />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <asp:Image runat="server" ImageUrl="~/Images/Titles/regex_reference.png" AlternateText="Regex reference" CssClass="title" />
    
    <div class="element_search_section">
        <asp:Label runat="server" AssociatedControlID="txtElement" CssClass="element_search_label">
            Regex element quick search:
        </asp:Label>
        <asp:TextBox ID="txtElement" runat="server" CssClass="element_input" />
        <asp:ImageButton runat="server" CssClass="element_search_button" AlternateText="Element search submit button" ImageUrl="~/Images/rounded_input_right.png" />
    </div>
    <div class="clear"></div>

    <div class="tables_section">
        <RS:ReferenceTable ID="rtCharacterClasses" runat="server" HeaderText="Character Classes" />
        <RS:ReferenceTable ID="rtCharacterEscapes" runat="server" HeaderText="Character Escapes" />
        <RS:ReferenceTable ID="rtAnchors" runat="server" HeaderText="Anchors" />
        <RS:ReferenceTable ID="rtGroupingConstructs" runat="server" HeaderText="Grouping Constructs" />
        <RS:ReferenceTable ID="rtQuantifiers" runat="server" HeaderText="Quantifiers" />
        <RS:ReferenceTable ID="rtBackreferenceConstructs" runat="server" HeaderText="Backreference Constructs" />
        <RS:ReferenceTable ID="rtAlternationConstructs" runat="server" HeaderText="Alternation Constructs" />
        <RS:ReferenceTable ID="rtSubstitutions" runat="server" HeaderText="Substitutions" />
        <RS:ReferenceTable ID="rtMiscellaneous" runat="server" HeaderText="Miscellaneous" />

        <asp:Panel ID="pnlNoResults" runat="server" CssClass="no_results">
            No regex elements found that match your search term
        </asp:Panel>
    </div>
    
</asp:Content>
