<%@ Page Language="C#" AutoEventWireup="false" EnableEventValidation="false" CodeFile="Tester.aspx.cs" Inherits="Tester" ValidateRequest="false" MasterPageFile="~/MasterPages/Standard.master" %>

<asp:Content runat="server" ContentPlaceHolderID="H">

    <meta name="description" content="Online .NET regular expression tester with real-time highlighting and detailed results output." />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="M">

    <asp:Image runat="server" ImageUrl="~/Images/Titles/regex_tester.png" AlternateText="Regex tester" CssClass="title" />

    <div class="left_form_column">
        <div class="form_section">
            <div class="label">Pattern</div>
            <asp:TextBox ID="txtPattern" runat="server" TextMode="MultiLine" CssClass="text_area_input pattern_input" spellcheck="false" />
        </div>

        <div class="form_section">
            <div class="label">Input</div>
            <asp:Panel ID="pnlHaystackBackdropWrap" runat="server" CssClass="haystack_backdrop_wrap">
                <asp:UpdatePanel ID="upHaystackBackdrop" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <pre id="hgcHaystackBackdrop" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:TextBox ID="txtHaystack" runat="server" TextMode="MultiLine" CssClass="text_area_input haystack_input" spellcheck="false" />
            </asp:Panel>
        </div>
    </div>
    
    <div class="right_form_column">
        <div class="form_section">
            <div class="label">Options</div>
            <div class="options_wrap">
                <div class="check_options">
                    <div class="check_options_left">
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbIgnoreCase" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbIgnoreCase">Ignore Case</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbIgnorePatternWhitespace" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbIgnorePatternWhitespace">Ignore Whitespace</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbExplicitCapture" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbExplicitCapture">Explicit Capture</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbCultureInvariant" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbCultureInvariant">Culture Invariant</asp:Label>
                        </div>
                    </div>
                    <div class="check_options_right">
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbSingleline" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbSingleline">Singleline</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbMultiline" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbMultiline">Multiline</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbRightToLeft" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbRightToLeft">Right To Left</asp:Label>
                        </div>
                        <div class="check_options_block">
                            <asp:CheckBox ID="cbEcmaScript" runat="server" />
                            <asp:Label runat="server" AssociatedControlID="cbEcmaScript">ECMA Script</asp:Label>
                        </div>
                    </div>
                </div>
                <div class="numerical_options">
                    <div class="numerical_option_block">
                        <asp:Label runat="server" AssociatedControlID="txtStartPosition">Start search at character position:</asp:Label>
                        <asp:TextBox ID="txtStartPosition" runat="server" />
                    </div>
                    <div class="numerical_option_block">
                        <asp:Label runat="server" AssociatedControlID="txtMatchesLimit">Max number of matches to find:</asp:Label>
                        <asp:TextBox ID="txtMatchesLimit" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="form_section">
        <div class="left_form_column">
            <div class="label replacement_label">Replacement</div>
            <div class="replacement_toggle_block">
                <asp:CheckBox ID="cbReplacement" runat="server" />
                <asp:Label ID="lblReplacement" runat="server" AssociatedControlID="cbReplacement">Replace matches with...</asp:Label>
            </div>
        </div>

        <div class="replacement_input_wrap">
            <div class="left_form_column">
                <asp:TextBox ID="txtReplacement" runat="server" TextMode="MultiLine" CssClass="text_area_input replacement_input" spellcheck="false" />
            </div>
            <div class="right_form_column">
                <div class="replacement_tip"><strong>Tip:</strong> Click the <strong>table</strong> or <strong>context</strong> tab to see your replacement results.</div>
            </div>
        </div>
    </div>
        
    <div class="submit_section">
        <asp:ImageButton ID="btnRun" runat="server" PostBackUrl="~/Tester" ImageUrl="~/Images/Buttons/run_regex.png" AlternateText="Run regex" />
    </div>
    <div class="clear"></div>

    <ul class="details_tabs">
        <li><asp:HyperLink ID="hlDetailsInfo" runat="server" NavigateUrl="#" CssClass="details_info_link">Regex Info</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlDetailsTable" runat="server" NavigateUrl="#" CssClass="details_table_link">Table</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlDetailsContext" runat="server" NavigateUrl="#" CssClass="details_context_link">Context</asp:HyperLink></li>
        <li><asp:HyperLink ID="hlDetailsSplit" runat="server" NavigateUrl="#" CssClass="details_split_link">Split List</asp:HyperLink></li>
    </ul>
    
    <div class="details_content details_content_no_script">
        <!-- loading -->
        <div class="details_loading">
            <asp:Image runat="server" ImageUrl="~/Images/loading.gif" />
        </div>

        <asp:RegularExpressionValidator ID="revPatternLength" runat="server" Display="None" ControlToValidate="txtPattern" ForeColor="#e54f3e" ErrorMessage="Pattern cannot be longer than {0} characters" />
        <asp:RegularExpressionValidator ID="revHaystackLength" runat="server" Display="None" ControlToValidate="txtHaystack" ForeColor="#e54f3e" ErrorMessage="Input cannot be longer than {0} characters" />
        <asp:RegularExpressionValidator ID="revReplacementLength" runat="server" Display="None" ControlToValidate="txtReplacement" ForeColor="#e54f3e" ErrorMessage="Replacement cannot be longer than {0} characters" />
        <asp:CustomValidator ID="cvInvalidOptionsCombo" runat="server" Display="None" ForeColor="#e54f3e" ErrorMessage="ECMA Script can only be combined with Ignore Case, Multiline, and Culture Invariant" />
        <asp:CustomValidator ID="cvPatternParsing" runat="server" Display="None" ForeColor="#e54f3e" />
        <asp:CustomValidator ID="cvPatternTimeout" runat="server" Display="None" ForeColor="#e54f3e" ErrorMessage="Execution timeout (this may be a sign that your pattern is inefficient)" />
        <asp:RequiredFieldValidator runat="server" Display="None" ControlToValidate="txtStartPosition" ForeColor="#e54f3e" ErrorMessage="Start search at character position is required" />
        <asp:RangeValidator runat="server" Display="None" ControlToValidate="txtStartPosition" Type="Integer" MinimumValue="0" MaximumValue="2147483647" ForeColor="#e54f3e" ErrorMessage="Start search at character position must be a non-negative number" />
        <asp:CustomValidator ID="cvStartPositionMaxValue" runat="server" Display="None" ForeColor="#e54f3e" ErrorMessage="Start search at character position cannot exceed length of input" />
        <asp:RequiredFieldValidator runat="server" Display="None" ControlToValidate="txtMatchesLimit" ForeColor="#e54f3e" ErrorMessage="Max number of matches to find is required" />
        <asp:RangeValidator ID="rvMatchesLimit" runat="server" Display="None" ControlToValidate="txtMatchesLimit" Type="Integer" MinimumValue="0" ForeColor="#e54f3e" ErrorMessage="Max number of matches to find must be a positive number no greater than {0}" />
    
        <!-- regex info -->
        <asp:Panel ID="pnlDetailsInfo" runat="server" CssClass="details_section details_info_section">
            <asp:UpdatePanel ID="upDetailsInfo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:ValidationSummary runat="server" CssClass="val_sum" ForeColor="#e54f3e" DisplayMode="BulletList" ShowSummary="true" />
                    
                    <asp:Label ID="lblRegexInfoBlurb" runat="server" CssClass="regex_info_blurb" />

                    <asp:Panel ID="pnlPermalinkWrap" runat="server" CssClass="permalink_wrap" Visible="false">
                        <asp:HyperLink ID="hlShowPermalink" runat="server" CssClass="show_permalink" NavigateUrl="#">Show Permalink</asp:HyperLink>
                        <asp:Panel ID="pnlPermalink" runat="server">
                            <div class="label">Permalink</div>
                            <asp:Panel ID="pnlPermalinkLengthWarning" runat="server" CssClass="permalink_length_warning">
                                Note: This permalink is long enough that some browsers may not properly load it. Try shortening the input.
                            </asp:Panel>
                            <asp:TextBox ID="txtPermalink" runat="server" ReadOnly="true" CssClass="permalink_input" />
                        </asp:Panel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>

        <!-- table -->
        <asp:Panel ID="pnlDetailsTable" runat="server" CssClass="details_section">
            <asp:UpdatePanel ID="upDetailsTable" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="details_table_wrap">
                        <asp:DataList ID="dlDetailsTable" runat="server" ExtractTemplateRows="true" CssClass="details_table">
                            <HeaderTemplate>
                                <asp:Table runat="server">
                                    <asp:TableHeaderRow ID="thrHeader" runat="server" CssClass="details_table_header_row">
                                        <asp:TableHeaderCell runat="server">Index</asp:TableHeaderCell>
                                        <asp:TableHeaderCell runat="server">Position</asp:TableHeaderCell>
                                        <asp:TableHeaderCell runat="server">Matched String</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                </asp:Table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:Table runat="server">
                                    <asp:TableRow ID="trRow" runat="server">
                                        <asp:TableCell runat="server">
                                            <asp:Literal ID="litMatchIndex" runat="server" />
                                        </asp:TableCell>
                                        <asp:TableCell runat="server">
                                            <asp:Literal ID="litPosition" runat="server" />
                                        </asp:TableCell>
                                        <asp:TableCell runat="server">
                                            <asp:Literal ID="litValue" runat="server" />
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                            </ItemTemplate>
                        </asp:DataList>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>

        <!-- context -->
        <asp:Panel ID="pnlDetailsContext" runat="server" CssClass="details_section">
            <asp:UpdatePanel ID="upDetailsContext" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="details_context details_context_no_script">
                        <pre id="hgcDetailsContext" runat="server"/>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        
        <!-- split list -->
        <asp:Panel ID="pnlDetailsSplit" runat="server" CssClass="details_section">
            <asp:UpdatePanel ID="upDetailsSplit" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <ol id="hgcSplitList" runat="server" class="split_list">
                        <asp:Repeater ID="rptSplitList" runat="server">
                            <ItemTemplate>
                                <li><asp:Literal ID="litSplitTextItem" runat="server" /></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ol>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
    
    <!-- to pass variables between client and server during async -->
    <asp:UpdatePanel ID="upSauce" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnRun" />
        </Triggers>
        <ContentTemplate>
            <asp:HiddenField ID="hfUpdateHaystackBackdrop" runat="server" />
            <asp:HiddenField ID="hfWipeDetailsLinks" runat="server" />
            <asp:HiddenField ID="hfDetailsLink" runat="server" />
            <asp:HiddenField ID="hfHasMatches" runat="server" Value="false" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
