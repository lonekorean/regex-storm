using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Tester : PageBase
{
    #region Constants

    private const int MAX_INPUT_LENGTH = 10000;
    private const int MAX_MATCHES = 100;
    private const int MAX_GROUPS = 50;
    private const int MAX_CAPTURES = 20;
    private const int MAX_PERMALINK_LENGTH = 1024;
    private const string MATCH_HIGHLIGHT_LEFT_MARKER = "(--##REGEXSTORM:LEFT##--)";
    private const string MATCH_HIGHLIGHT_RIGHT_MARKER = "(--##REGEXSTORM:RIGHT##--)";
    private const string EMPTY_START_MESSAGE = "Enter your <strong>pattern</strong> and <strong>input</strong> above to see the good stuff here.";

    #endregion

    #region Enums

    private enum DetailsPanels
    {
        None = 0,
        RegexInfo = 1,
        Table = 2,
        Context = 3,
        SplitList = 4,
        All = 5
    }

    #endregion

    #region Variables

    private int startPosition;              // where in input to begin search
    private int matchesLimit;               // limits total matches displayed
    private Regex testRegex;                // the main regex used for testing
    private MatchCollection matches;        // matches found from regex, set from RegexMatchWorker()

    #endregion

    #region Constructors

    public Tester()
    {
        // wire page events
        this.Init += new EventHandler(Tester_Init);
        this.Load += new EventHandler(Tester_Load);
        this.PreRender += new EventHandler(Tester_PreRender);
    }

    #endregion

    #region Events

    protected void Tester_Init(object sender, EventArgs e)
    {
        // wire control events
        btnRun.Click += new ImageClickEventHandler(btnRun_Click);
        rptSplitList.ItemDataBound += new RepeaterItemEventHandler(rptSplitList_ItemDataBound);
        dlDetailsTable.ItemDataBound += new DataListItemEventHandler(dlDetailsTable_ItemDataBound);

        // declare controls that need view state
        AddViewStateControl(upHaystackBackdrop);
        AddViewStateControl(upDetailsInfo);
        AddViewStateControl(upDetailsTable);
        AddViewStateControl(upDetailsContext);
        AddViewStateControl(upDetailsSplit);

        // this stuff doesn't need to be set during async postbacks
        if (!ScriptManager.GetCurrent(this).IsInAsyncPostBack)
        {
            // add head stuff
            Title = ".NET Regex Tester - Regex Storm";
            AddStylesheet("~/Stylesheets/Tester.css");
            AddJavascript("~/Javascripts/Tester.js");

            // ugly browser sniffing to apply stylesheets to fix little issues
            switch(Request.Browser.Browser)
            {
                case "Firefox":
                    // all firefox
                    AddStylesheet("~/Stylesheets/TesterFixes/firefox-all.css");
                    if (Request.Browser.MajorVersion == 3 && Request.Browser.MinorVersion == 0)
                    {
                        // only for firefox3.0
                        AddStylesheet("~/Stylesheets/TesterFixes/firefox-3.0.css");
                    }
                    break;
                case "IE":
                    if (Request.Browser.MajorVersion <= 7)
                    {
                        // only for ie7
                        AddStylesheet("~/Stylesheets/TesterFixes/ie-7.x.css");
                    }
                    break;
                case "AppleMAC-Safari":
                    if (!Request.UserAgent.Contains("Chrome"))
                    {
                        // must check user agent to distinguish safari from chrome
                        AddStylesheet("~/Stylesheets/TesterFixes/safari-all.css");
                    }
                    break;
            }

            // lots of javascript vars
            AddJavascriptVar("hgcHaystackBackdropId", hgcHaystackBackdrop.ClientID);
            AddJavascriptVar("btnRunId", btnRun.ClientID);
            AddJavascriptVar("txtStartPositionId", txtStartPosition.ClientID);
            AddJavascriptVar("txtMatchesLimit", txtMatchesLimit.ClientID);
            AddJavascriptVar("hlDetailsInfoId", hlDetailsInfo.ClientID);
            AddJavascriptVar("hlDetailsTableId", hlDetailsTable.ClientID);
            AddJavascriptVar("hlDetailsContextId", hlDetailsContext.ClientID);
            AddJavascriptVar("hlDetailsSplitId", hlDetailsSplit.ClientID);
            AddJavascriptVar("pnlDetailsInfoId", pnlDetailsInfo.ClientID);
            AddJavascriptVar("pnlDetailsSplitId", pnlDetailsSplit.ClientID);
            AddJavascriptVar("pnlDetailsContextId", pnlDetailsContext.ClientID);
            AddJavascriptVar("pnlDetailsTableId", pnlDetailsTable.ClientID);
            AddJavascriptVar("pnlPermalinkId", pnlPermalink.ClientID);
            AddJavascriptVar("hfUpdateHaystackBackdropId", hfUpdateHaystackBackdrop.ClientID);
            AddJavascriptVar("hfWipeDetailsLinksId", hfWipeDetailsLinks.ClientID);
            AddJavascriptVar("hfDetailsLinkId", hfDetailsLink.ClientID);
            AddJavascriptVar("hfHasMatchesId", hfHasMatches.ClientID);
        }
    }

    protected void Tester_Load(object sender, EventArgs e)
    {
        // set up some validators that depend on server-side constants
        string lengthExpression = "^(.|\\s){0," + MAX_INPUT_LENGTH.ToString() + "}$";
        revPatternLength.ValidationExpression = lengthExpression;
        revPatternLength.ErrorMessage = string.Format(revPatternLength.ErrorMessage, MAX_INPUT_LENGTH);
        revHaystackLength.ValidationExpression = lengthExpression;
        revHaystackLength.ErrorMessage = string.Format(revHaystackLength.ErrorMessage, MAX_INPUT_LENGTH);
        revReplacementLength.ValidationExpression = lengthExpression;
        revReplacementLength.ErrorMessage = string.Format(revReplacementLength.ErrorMessage, MAX_INPUT_LENGTH);
        rvMatchesLimit.MaximumValue = MAX_MATCHES.ToString();
        rvMatchesLimit.ErrorMessage = string.Format(rvMatchesLimit.ErrorMessage, MAX_MATCHES);

        if (!Page.IsPostBack)
        {
            // there may be stuff on the query string to load into the form
            this.LoadFromQueryString();

            if (txtPattern.Text.Length > 0 && txtHaystack.Text.Length > 0)
            {
                // we already have enough stuff from the query string to run the regex
                Page.Validate();
                RunRegex(true, DetailsPanels.All);
            }
            else
            {
                // starting off with empty pattern and haystack inputs
                lblRegexInfoBlurb.Text = EMPTY_START_MESSAGE;
            }
        }
        else if (ScriptManager.GetCurrent(this).IsInAsyncPostBack)
        {
            // get boolean for whether to update haystack backdrop
            bool updateHaystackBackdrop = (hfUpdateHaystackBackdrop.Value == "true");

            // translate detals tab id to details panel enum
            DetailsPanels detailsPanel = DetailsPanels.None;
            if (hfDetailsLink.Value == hlDetailsInfo.ClientID) { detailsPanel = DetailsPanels.RegexInfo; }
            else if (hfDetailsLink.Value == hlDetailsTable.ClientID) { detailsPanel = DetailsPanels.Table; }
            else if (hfDetailsLink.Value == hlDetailsContext.ClientID) { detailsPanel = DetailsPanels.Context; }
            else if (hfDetailsLink.Value == hlDetailsSplit.ClientID) { detailsPanel = DetailsPanels.SplitList; }

            // validate and run
            Page.Validate();
            RunRegex(updateHaystackBackdrop, detailsPanel);
        }
    }

    protected void Tester_PreRender(object sender, EventArgs e)
    {
        if (!ScriptManager.GetCurrent(this).IsInAsyncPostBack)
        {
            // fix issue where the leading line break is eaten in multiline textboxes
            txtPattern.Text = Environment.NewLine + txtPattern.Text;
            txtReplacement.Text = Environment.NewLine + txtReplacement.Text;
            txtHaystack.Text = Environment.NewLine + txtHaystack.Text;
        }

        if (this.IsDevEnvironment)
        {
            // simulate round trip delay (helps with testing async stuff)
            Random rand = new Random();
            Thread.Sleep(rand.Next(200, 400));
        }
    }

    protected void dlDetailsTable_ItemDataBound(Object sender, DataListItemEventArgs e)
    {
        // for limiting groups displayed
        int numDisplayGroups = Math.Min(testRegex.GetGroupNumbers().Count(), MAX_GROUPS + 1);

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                // grab controls
                TableHeaderRow thrHeader = (TableHeaderRow)e.Item.FindControl("thrHeader");

                if (cbReplacement.Checked)
                {
                    // add header cell for replacement value
                    TableHeaderCell thcReplacement = new TableHeaderCell();
                    thcReplacement.Controls.Add(new LiteralControl("Replacement"));
                    thrHeader.Cells.Add(thcReplacement);
                }

                // add a header cell for each group name
                for (int i = 1; i < numDisplayGroups; i++)
                {
                    // decide how to display this group column header
                    string groupDisplay;
                    int groupNumber = testRegex.GetGroupNumbers()[i];
                    string groupName = testRegex.GroupNameFromNumber(groupNumber);
                    if (groupNumber.ToString() == groupName)
                    {
                        // name is same as number, assume group is unnamed
                        groupDisplay = "$" + groupName;
                    }
                    else
                    {
                        // named group
                        groupDisplay = "${" + groupName + "}";
                    }

                    TableHeaderCell thcGroup = new TableHeaderCell();
                    thcGroup.Controls.Add(new LiteralControl(groupDisplay));
                    if (i == 1)
                    {
                        thcGroup.CssClass = "first_group";
                    }
                    thrHeader.Cells.Add(thcGroup);
                }
                break;

            case ListItemType.Item:
            case ListItemType.AlternatingItem:
                TableRow trRow = (TableRow)e.Item.FindControl("trRow");

                if (e.Item.ItemIndex < matchesLimit)
                {
                    // grab match
                    Match match = (Match)e.Item.DataItem;

                    // grab controls
                    Literal litMatchIndex = (Literal)e.Item.FindControl("litMatchIndex");
                    Literal litPosition = (Literal)e.Item.FindControl("litPosition");
                    Literal litValue = (Literal)e.Item.FindControl("litValue");

                    // alternating row style
                    trRow.CssClass += (((e.Item.ItemIndex & 1) == 0) ? "odd" : "even");

                    // set literals
                    litMatchIndex.Text = e.Item.ItemIndex.ToString();
                    litPosition.Text = match.Index.ToString();
                    litValue.Text = GetTextDisplay(match.Value, false);

                    if (cbReplacement.Checked)
                    {
                        // add cell for replacement
                        TableCell tcReplacement = new TableCell();
                        trRow.Cells.Add(tcReplacement);

                        // label with value
                        Literal litReplacementValue = new Literal();
                        litReplacementValue.Text = GetTextDisplay(match.Result(txtReplacement.Text), false);
                        tcReplacement.Controls.Add(litReplacementValue);
                    }

                    // add a cell for each group value
                    for (int i = 1; i < numDisplayGroups; i++)
                    {
                        Group displayGroup = match.Groups[i];

                        // cell
                        TableCell tcGroupValue = new TableCell();
                        if (i == 1)
                        {
                            tcGroupValue.CssClass = "first_group";
                        }

                        // label with value
                        Literal litGroupValue = new Literal();
                        litGroupValue.Text = GetTextDisplay(displayGroup.Value, false);
                        tcGroupValue.Controls.Add(litGroupValue);

                        // if capture collection is not trivial (1) then show them
                        if (displayGroup.Captures.Count > 1)
                        {
                            // line break
                            tcGroupValue.Controls.Add(new LiteralControl("<br />"));

                            // captures count text (changed to toggle link if javascript on)
                            Label lblCapturesCount = new Label();
                            lblCapturesCount.Text = displayGroup.Captures.Count.ToString() + " captures";
                            lblCapturesCount.CssClass = "captures_count";
                            tcGroupValue.Controls.Add(lblCapturesCount);

                            // captures container
                            Panel pnlCapturesContainer = new Panel();
                            pnlCapturesContainer.CssClass = "captures";
                            tcGroupValue.Controls.Add(pnlCapturesContainer);

                            // ordered list
                            HtmlGenericControl hgcCapturesList = new HtmlGenericControl("ol");
                            hgcCapturesList.Attributes.Add("start", "0");
                            pnlCapturesContainer.Controls.Add(hgcCapturesList);

                            // captures within list
                            Repeater rptCaptures = new Repeater();
                            rptCaptures.ItemDataBound += new RepeaterItemEventHandler(rptCaptures_ItemDataBound);
                            rptCaptures.DataSource = displayGroup.Captures;
                            rptCaptures.DataBind();
                            hgcCapturesList.Controls.Add(rptCaptures);

                            if (displayGroup.Captures.Count > MAX_CAPTURES)
                            {
                                Panel pnlCapturesRemaining = new Panel();
                                pnlCapturesRemaining.CssClass = "captures_remaining";
                                pnlCapturesRemaining.Controls.Add(
                                    new LiteralControl((displayGroup.Captures.Count - MAX_CAPTURES).ToString() + " more not shown"));
                                pnlCapturesContainer.Controls.Add(pnlCapturesRemaining);
                            }
                        }
                        trRow.Cells.Add(tcGroupValue);
                    }
                }
                else
                {
                    // this row is beyond the matches limit, do not show
                    trRow.Visible = false;
                }
                break;
        }
    }

    protected void rptCaptures_ItemDataBound(Object sender, RepeaterItemEventArgs e)
    {
        // only render if we haven't exceeded the max number of captures to show
        if (e.Item.ItemIndex < MAX_CAPTURES)
        {
            Capture capture = (Capture)e.Item.DataItem;

            Literal litCapture = new Literal();
            litCapture.Text = GetTextDisplay(capture.Value, true);

            HtmlGenericControl hgcCaptureItem = new HtmlGenericControl("li");
            hgcCaptureItem.Controls.Add(litCapture);

            e.Item.Controls.Add(hgcCaptureItem);
        }
    }

    protected void rptSplitList_ItemDataBound(Object sender, RepeaterItemEventArgs e)
    {
        string text = e.Item.DataItem.ToString();
        Literal litSplitTextItem = (Literal)e.Item.FindControl("litSplitTextItem");

        litSplitTextItem.Text = GetTextDisplay(text, true);
    }

    protected void btnRun_Click(Object sender, EventArgs e)
    {
        if (!ScriptManager.GetCurrent(this).IsInAsyncPostBack)
        {
            // old fashioned submit button click
            RunRegex(true, DetailsPanels.All);
        }
    }

    #endregion

    #region Private Methods

    private void RunRegex(bool updateHaystackBackdrop, DetailsPanels detailsPanel)
    {
        bool isPageValid = false;       // flag that keeps track of validation success
        long elapsedMilliseconds = 0;   // total time it took for the regex to run

        // begin series of setup and validation steps
        if (Page.IsValid)
        {
            // at this point, we know these are numerical values
            startPosition = Convert.ToInt32(txtStartPosition.Text);
            matchesLimit = Convert.ToInt32(txtMatchesLimit.Text);

            // check numerical value of start position
            if (startPosition > txtHaystack.Text.Length)
            {
                cvStartPositionMaxValue.IsValid = false;
            }

            if (Page.IsValid)
            {
                // browsers other than IE do not include \r in inputs during async postback
                // this normalizes inputs to always have \r\n for newlines instead of just \n
                txtPattern.Text = Regex.Replace(txtPattern.Text, "\r?\n", "\r\n");
                txtHaystack.Text = Regex.Replace(txtHaystack.Text, "\r?\n", "\r\n");
                txtReplacement.Text = Regex.Replace(txtReplacement.Text, "\r?\n", "\r\n");

                // collect options
                RegexOptions regexOptions = 0;
                if (cbIgnoreCase.Checked) { regexOptions = regexOptions | RegexOptions.IgnoreCase; }
                if (cbIgnorePatternWhitespace.Checked) { regexOptions = regexOptions | RegexOptions.IgnorePatternWhitespace; }
                if (cbExplicitCapture.Checked) { regexOptions = regexOptions | RegexOptions.ExplicitCapture; }
                if (cbCultureInvariant.Checked) { regexOptions = regexOptions | RegexOptions.CultureInvariant; }
                if (cbSingleline.Checked) { regexOptions = regexOptions | RegexOptions.Singleline; }
                if (cbMultiline.Checked) { regexOptions = regexOptions | RegexOptions.Multiline; }
                if (cbRightToLeft.Checked) { regexOptions = regexOptions | RegexOptions.RightToLeft; }
                if (cbEcmaScript.Checked) { regexOptions = regexOptions | RegexOptions.ECMAScript; }

                // initialize new regex object, careful to catch any parsing exception from
                // the pattern to show to the user as a validation error
                try
                {
                    testRegex = new Regex(txtPattern.Text, regexOptions);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // ECMA script option throws exception if used with certain other options
                    cvInvalidOptionsCombo.IsValid = false;
                }
                catch (ArgumentException ex)
                {
                    // catch parsing exceptions, double check message to make sure it's
                    // the type of exception we're expecting
                    if (ex.Message.StartsWith("parsing") && ex.Message.Contains(" - "))
                    {
                        // display part of the exception message string as a validation error
                        cvPatternParsing.ErrorMessage = ex.Message.Substring(ex.Message.LastIndexOf(" - ") + 3).TrimEnd('.');
                        cvPatternParsing.IsValid = false;
                    }
                }

                if (Page.IsValid)
                {
                    // the actual regex match execution happens within
                    elapsedMilliseconds = TimeLimitedRegexMatch();

                    if (Page.IsValid)
                    {
                        // winner!
                        isPageValid = true;
                    }
                }
            }
        }

        if (!isPageValid)
        {
            // page is invalid, will fall back to regex info details tab to show errors
            hfHasMatches.Value = "false";
            lblRegexInfoBlurb.Visible = false;
            pnlPermalinkWrap.Visible = false;

            hfDetailsLink.Value = hlDetailsInfo.ClientID;
            upDetailsInfo.Update();
        }
        else
        {
            // page is valid
            if (txtPattern.Text.Length == 0 || txtHaystack.Text.Length == 0)
            {
                // not enough input
                hfHasMatches.Value = "false";
                lblRegexInfoBlurb.Text = EMPTY_START_MESSAGE;

                if (updateHaystackBackdrop)
                {
                    // clear haystack backdrop (panel updated later)
                    hgcHaystackBackdrop.InnerHtml = "";
                }

                hfDetailsLink.Value = hlDetailsInfo.ClientID;
                upDetailsInfo.Update();
            }
            else
            {
                // count matches
                int matchesCount = Math.Min(matches.Count, matchesLimit);

                // send this flag back to the page
                hfHasMatches.Value = (matchesCount > 0) ? "true" : "false";

                // regex info details panel doesn't require matches to be found
                // furthermore, if no matches are found, active panel is reverted to regex info on client-side
                if (matchesCount == 0 || detailsPanel == DetailsPanels.All || detailsPanel == DetailsPanels.RegexInfo)
                {
                    // display matches count and timing info in the regex info blurb
                    lblRegexInfoBlurb.Text = "<strong>" + matchesCount.ToString() + "</strong>" +
                        (matchesCount == 1 ? " match" : " matches") +
                        " found in about " +
                        "<strong>" + elapsedMilliseconds.ToString() + "</strong>" +
                        (elapsedMilliseconds == 1 ? " millisecond" : " milliseconds") +
                        ".";

                    SetPermalink();

                    hfDetailsLink.Value = hlDetailsInfo.ClientID;
                    upDetailsInfo.Update();
                }

                // all other details panels (and the haystack backdrop) require that matches were found
                if (matchesCount > 0)
                {
                    if (updateHaystackBackdrop)
                    {
                        string matchesInContext = testRegex.Replace(txtHaystack.Text,
                            new MatchEvaluator(MatchHighlightNoReplacements), matchesLimit, startPosition);
                        matchesInContext = HttpUtility.HtmlEncode(matchesInContext);
                        matchesInContext = matchesInContext.Replace(MATCH_HIGHLIGHT_LEFT_MARKER, "<em>");
                        matchesInContext = matchesInContext.Replace(MATCH_HIGHLIGHT_RIGHT_MARKER, "</em>");

                        // fix newline weirdness
                        matchesInContext = matchesInContext.Replace("\r", "");

                        // update haystack backdrop
                        hgcHaystackBackdrop.InnerHtml = '\n' + matchesInContext + '\n';
                    }

                    if (detailsPanel == DetailsPanels.All || detailsPanel == DetailsPanels.Context)
                    {
                        // both haystack backdrop and the context details panel need the string for matches in context
                        string matchesInContext = testRegex.Replace(txtHaystack.Text,
                            new MatchEvaluator(MatchHighlight), matchesLimit, startPosition);
                        matchesInContext = HttpUtility.HtmlEncode(matchesInContext);
                        matchesInContext = matchesInContext.Replace(MATCH_HIGHLIGHT_LEFT_MARKER, "<em>");
                        matchesInContext = matchesInContext.Replace(MATCH_HIGHLIGHT_RIGHT_MARKER, "</em>");

                        // fix newline weirdness
                        matchesInContext = matchesInContext.Replace("\r", "");

                        // update context details panel, adding zero-width spaces after newlines to
                        // ensure proper highlighting around line breaks
                        hgcDetailsContext.InnerHtml = "\n" + matchesInContext.Replace("\n", "\n&#8203;") + '\n';

                        upDetailsContext.Update();
                    }

                    if (detailsPanel == DetailsPanels.All || detailsPanel == DetailsPanels.Table)
                    {
                        // matches table
                        dlDetailsTable.DataSource = matches;
                        dlDetailsTable.DataBind();

                        upDetailsTable.Update();
                    }

                    if (detailsPanel == DetailsPanels.All || detailsPanel == DetailsPanels.SplitList)
                    {
                        // split list
                        hgcSplitList.Attributes.Add("start", "0");
                        rptSplitList.DataSource = testRegex.Split(txtHaystack.Text, matchesLimit, startPosition);
                        rptSplitList.DataBind();

                        upDetailsSplit.Update();
                    }
                }
            }
        }

        // this is just for users that don't have javascript on
        if (hfHasMatches.Value == "true")
        {
            pnlDetailsTable.Style["display"] = "block";
            pnlDetailsContext.Style["display"] = "block";
            pnlDetailsSplit.Style["display"] = "block";
        }
        else
        {
            pnlDetailsTable.Style["display"] = "none";
            pnlDetailsContext.Style["display"] = "none";
            pnlDetailsSplit.Style["display"] = "none";
        }

        if (updateHaystackBackdrop)
        {
            upHaystackBackdrop.Update();
        }
    }

    private void LoadFromQueryString()
    {
        // remembers if an invalid query string value was encountered
        bool invalidValueFound = false;

        // pattern
        if (!string.IsNullOrEmpty(Request.QueryString["p"]))
        {
            txtPattern.Text = Request.QueryString["p"];
        }

        // haystack
        if (!string.IsNullOrEmpty(Request.QueryString["i"]))
        {
            txtHaystack.Text = Request.QueryString["i"];
        }

        // replacement (this one allows empty inputs)
        if (Request.QueryString["r"] != null)
        {
            cbReplacement.Checked = true;
            txtReplacement.Text = Request.QueryString["r"];
        }

        // options
        string regexOptionsCode = Request.QueryString["o"];
        if (!string.IsNullOrEmpty(regexOptionsCode))
        {
            if (regexOptionsCode.Contains("i")) { cbIgnoreCase.Checked = true; }
            if (regexOptionsCode.Contains("x")) { cbIgnorePatternWhitespace.Checked = true; }
            if (regexOptionsCode.Contains("n")) { cbExplicitCapture.Checked = true; }
            if (regexOptionsCode.Contains("c")) { cbCultureInvariant.Checked = true; }
            if (regexOptionsCode.Contains("s")) { cbSingleline.Checked = true; }
            if (regexOptionsCode.Contains("m")) { cbMultiline.Checked = true; }
            if (regexOptionsCode.Contains("r")) { cbRightToLeft.Checked = true; }
            if (regexOptionsCode.Contains("e")) { cbEcmaScript.Checked = true; }
            if (Regex.IsMatch(regexOptionsCode, "[^ixncsmre]"))
            {
                // something invalid in options, redirect to correct
                invalidValueFound = true;
            }
        }

        // start position
        string startPositionQS = Request.QueryString["s"] ?? "";
        if (startPositionQS.Length == 0)
        {
            // default
            startPosition = 0;
        }
        else
        {
            int startPositionValue;
            if (!int.TryParse(startPositionQS, out startPositionValue) || startPositionValue < 0)
            {
                // converstion failed or value is negative, redirect to correct
                startPosition = 0;
                invalidValueFound = true;
            }
            else
            {
                // incoming value is good
                startPosition = startPositionValue;
            }
        }
        txtStartPosition.Text = startPosition.ToString();

        // matches limit
        string matchesLimitQS = Request.QueryString["l"] ?? "";
        if (matchesLimitQS.Length == 0)
        {
            // default
            matchesLimit = MAX_MATCHES;
        }
        else
        {
            int matchesLimitValue;
            if (!int.TryParse(matchesLimitQS, out matchesLimitValue) || matchesLimitValue < 1 || matchesLimitValue > MAX_MATCHES)
            {
                // converstion failed or value is not positive or value is too high, redirect to correct
                matchesLimit = MAX_MATCHES;
                invalidValueFound = true;
            }
            else
            {
                // incoming value is good
                matchesLimit = matchesLimitValue;
            }
        }
        txtMatchesLimit.Text = matchesLimit.ToString();

        // if anything was invalid, redirect to self, but with a query string that
        // has the invalid values defaulted to something better
        if (invalidValueFound)
        {
            Response.Redirect(this.GetStateUrl());
        }
    }

    private string GetStateUrl()
    {
        StringBuilder queryString = new StringBuilder();

        // pattern
        if (!string.IsNullOrEmpty(txtPattern.Text))
        {
            queryString.Append("&p=" + HttpUtility.UrlEncode(txtPattern.Text));
        }

        // haystack
        if (!string.IsNullOrEmpty(txtHaystack.Text))
        {
            queryString.Append("&i=" + HttpUtility.UrlEncode(txtHaystack.Text));
        }

        // replacement (determined by checkbox)
        if (cbReplacement.Checked)
        {
            queryString.Append("&r=" + HttpUtility.UrlEncode(txtReplacement.Text));
        }

        // regex options
        StringBuilder regexOptionsCode = new StringBuilder();
        if (cbIgnoreCase.Checked) { regexOptionsCode.Append("i"); }
        if (cbIgnorePatternWhitespace.Checked) { regexOptionsCode.Append("x"); }
        if (cbExplicitCapture.Checked) { regexOptionsCode.Append("n"); }
        if (cbCultureInvariant.Checked) { regexOptionsCode.Append("c"); }
        if (cbSingleline.Checked) { regexOptionsCode.Append("s"); }
        if (cbMultiline.Checked) { regexOptionsCode.Append("m"); }
        if (cbRightToLeft.Checked) { regexOptionsCode.Append("r"); }
        if (cbEcmaScript.Checked) { regexOptionsCode.Append("e"); }
        if (regexOptionsCode.Length > 0)
        {
            queryString.Append("&o=" + HttpUtility.UrlEncode(regexOptionsCode.ToString()));
        }

        // start position
        if (startPosition != 0)
        {
            queryString.Append("&s=" + HttpUtility.UrlEncode(txtStartPosition.Text));
        }

        // matches limit
        if (matchesLimit != MAX_MATCHES)
        {
            queryString.Append("&l=" + HttpUtility.UrlEncode(txtMatchesLimit.Text));
        }

        // create a new url with current scheme and host
        UriBuilder url = new UriBuilder(Request.Url.Scheme, Request.Url.Host);

        // set raw path
        url.Path = this.RawPath;

        // set new query string
        url.Query = queryString.ToString().TrimStart('&');

        // return the full absolute url
        return url.Uri.AbsoluteUri;
    }

    private string MatchHighlightNoReplacements(Match m)
    {
        return MATCH_HIGHLIGHT_LEFT_MARKER + m.Value + MATCH_HIGHLIGHT_RIGHT_MARKER;
    }
    
    private string MatchHighlight(Match m)
    {
        if (cbReplacement.Checked)
        {
            // replace, show the replacement string
            return MATCH_HIGHLIGHT_LEFT_MARKER + m.Result(txtReplacement.Text) + MATCH_HIGHLIGHT_RIGHT_MARKER;
        }
        else
        {
            // match and split, show the matched string
            return MATCH_HIGHLIGHT_LEFT_MARKER + m.Value + MATCH_HIGHLIGHT_RIGHT_MARKER;
        }
    }

    private string GetTextDisplay(string text, bool requireWrappingSpan)
    {
        // why return strings with span tags inside instead of just using a Label?
        // to avoid putting long-ass namespaced IDs in the HTML, which is especially
        // unfavorable when returning async payloads
        string returnText;
        if (text.Length == 0)
        {
            returnText = "<span class=\"non-lit\">empty string</span>";
        }
        else if (Regex.IsMatch(text, "^\\s+$"))
        {
            returnText = "<span class=\"non-lit\">whitespace</span>";
        }
        else
        {
            returnText = HttpUtility.HtmlEncode(text).Replace("\n", "<br />\n");
            if (requireWrappingSpan)
            {
                returnText = "<span>" + returnText + "</span>";
            }
        }

        return returnText;
    }

    private long TimeLimitedRegexMatch()
    {
        // run RegexMatchWorker() in a time limited thread
        Thread t = new Thread(new ThreadStart(RegexMatchWorker));

        // start thread and wait while timing it
        Stopwatch matchStopWatch = Stopwatch.StartNew();
        t.Start();
        bool finished = t.Join(3000);
        matchStopWatch.Stop();

        if (!finished)
        {
            // taking too long, kill it and show the timeout error message
            t.Abort();
            cvPatternTimeout.IsValid = false;
        }

        return matchStopWatch.ElapsedMilliseconds;
    }

    private void RegexMatchWorker()
    {
        try
        {
            matches = testRegex.Matches(txtHaystack.Text, startPosition);

            // regex isn't actually executed until the matches collection is referenced
            // this throw-away variable instantiation exists solely for that purpose
            int totalMatches = matches.Count;
        }
        catch (ThreadAbortException)
        {
        }
    }

    private void SetPermalink()
    {
        pnlPermalinkWrap.Visible = true;
        txtPermalink.Text = this.GetStateUrl();
        pnlPermalinkLengthWarning.Visible = (txtPermalink.Text.Length > MAX_PERMALINK_LENGTH);
    }

    #endregion
}
