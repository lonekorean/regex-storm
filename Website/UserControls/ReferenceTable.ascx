<%@ Control Language="C#" AutoEventWireup="false" CodeFile="ReferenceTable.ascx.cs" Inherits="UserControls.ReferenceTable" %>

<asp:DataList ID="dlRefTable" runat="server" ExtractTemplateRows="true" CssClass="ref_table">
    <HeaderTemplate>
        <asp:Table runat="server">
            <asp:TableHeaderRow runat="server">
                <asp:TableHeaderCell runat="server" ColumnSpan="3">
                    <asp:Literal id="litHeader" runat="server" />
                </asp:TableHeaderCell>
            </asp:TableHeaderRow>
        </asp:Table>
    </HeaderTemplate>
    <ItemTemplate>
        <asp:Table runat="server">
            <asp:TableRow ID="trRef" runat="server">
                <asp:TableCell runat="server" CssClass="ref_element"><asp:Literal id="litElement" runat="server" /></asp:TableCell>
                <asp:TableCell runat="server" CssClass="ref_definition"><asp:Literal id="litDefinition" runat="server" /></asp:TableCell>
                <asp:TableCell runat="server" CssClass="ref_example"><asp:HyperLink id="hlExample" runat="server">see an example</asp:HyperLink></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </ItemTemplate>
</asp:DataList>
