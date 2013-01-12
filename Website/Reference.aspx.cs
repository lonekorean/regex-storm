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

public partial class Reference : PageBase
{

    #region Constructors

    public Reference()
    {
        // wire page events
        this.Init += new EventHandler(Reference_Init);
        this.Load += new EventHandler(Reference_Load);
        this.PreRender += new EventHandler(Reference_PreRender);
    }

    #endregion

    #region Events

    protected void Reference_Init(object sender, EventArgs e)
    {
        // add head stuff
        Title = ".NET Regex Reference - Regex Storm";
        AddStylesheet("~/Stylesheets/Reference.css");
        AddJavascript("~/Javascripts/Reference.js");
    }

    protected void Reference_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            // turn POST into GET
            string redirectUrl = this.RawPath;
            if(!string.IsNullOrEmpty(txtElement.Text))
            {
                // add search term
                redirectUrl += "?s=" + HttpUtility.UrlEncode(txtElement.Text);
            }
            Response.Redirect(redirectUrl);
        }
    }

    protected void Reference_PreRender(object sender, EventArgs e)
    {
        // disable autocomplete
        txtElement.Attributes.Add("autocomplete", "off");

        // character classes
        rtCharacterClasses.AddRow("[<em>characters</em>]", "Matches any character found in <em>characters</em>.", "~/tester?p=%5baeiou%5d&i=everyone+likes+vowels");
        rtCharacterClasses.AddRow("[^<em>characters</em>]", "Matches any character not found in <em>characters</em>.", "~/tester?p=%5b%5eaeiou%5d&i=nobody+likes+vowels");
        rtCharacterClasses.AddRow("[<em>first</em>-<em>last</em>]", "Matches any character in the range of characters from <em>first</em> to <em>last</em>.", "~/tester?p=%5bn-z%5d&i=just+the+second+half+of+the+alphabet");
        rtCharacterClasses.AddRow(".", "Wildcard. Matches any character except <strong>\\n</strong>.", "~/tester?p=.&i=look%2c+everyone's+a+winner!");
        rtCharacterClasses.AddRow("\\p{<em>category</em>}", "Matches any character in a category of unicode characters, specified by <em>category</em>. To see what you can use for <em>category</em>, check out the <a href=\"http://msdn.microsoft.com/en-us/library/20bw873z.aspx#SupportedUnicodeGeneralCategories\" target=\"_blank\">supported unicode general categories</a> and the <a href=\"http://msdn.microsoft.com/en-us/library/20bw873z.aspx#SupportedNamedBlocks\" target=\"_blank\">supported named blocks</a>.", "~/tester?p=%5cp%7bP%7d&i=there+are+many+unicode+categories%2c+including+%22P%22%2c+which+finds+all+punctuation!");
        rtCharacterClasses.AddRow("\\P{<em>category</em>}", "Matches any character not in a category of unicode characters, specified by <em>category</em>. To see what you can use for <em>category</em>, check out the <a href=\"http://msdn.microsoft.com/en-us/library/20bw873z.aspx#SupportedUnicodeGeneralCategories\" target=\"_blank\">supported unicode general categories</a> and the <a href=\"http://msdn.microsoft.com/en-us/library/20bw873z.aspx#SupportedNamedBlocks\" target=\"_blank\">supported named blocks</a>.", "~/tester?p=%5cP%7bL%7d&i=the+%22L%22+unicode+category+contains+all+letters%2c+so+this+pattern+will+find+all+non-letters.");
        rtCharacterClasses.AddRow("\\w", "Matches any letter, decimal digit, or an underscore.", "~/tester?p=%5cw%2b&i=Jenny%2c+don't+change+your+number!+867-5309!");
        rtCharacterClasses.AddRow("\\W", "Matches any character except a letter, decimal digit, or an underscore", "~/tester?p=%5cW%2b&i=Jenny%2c+don't+change+your+number!+867-5309!");
        rtCharacterClasses.AddRow("\\s", "Matches any white-space character.", "~/tester?p=%5cs&i=mind+the+gap");
        rtCharacterClasses.AddRow("\\S", "Matches any character except a white-space character.", "~/tester?p=%5cS&i=mind+the+gap");
        rtCharacterClasses.AddRow("\\d", "Matches any decimal digit.", "~/tester?p=%5cd%2b&i=Jenny%2c+don't+change+your+number!+867-5309!");
        rtCharacterClasses.AddRow("\\D", "Matches any character except a decimal digit.", "~/tester?p=%5cD%2b&i=Jenny%2c+don't+change+your+number!+867-5309!");

        // character escapes
        rtCharacterEscapes.AddRow("\\r", "Matches a carriage return.", "~/tester?p=(%5cr)(%5cn)&i=most+line+breaks+are+actually+two+white-space%0d%0acharacters+together%3a+%5cr%5cn+(carriage+return+%2b+newline)");
        rtCharacterEscapes.AddRow("\\n", "Matches a newline.", "~/tester?p=(%5cr)(%5cn)&i=most+line+breaks+are+actually+two+white-space%0d%0acharacters+together%3a+%5cr%5cn+(carriage+return+%2b+newline)");
        rtCharacterEscapes.AddRow("\\t", "Matches a tab.");
        rtCharacterEscapes.AddRow("[\\b]", "Matches a backspace. Note that it must be enclosed in brackets to have this meaning.");
        rtCharacterEscapes.AddRow("\\f", "Matches a form feed.");
        rtCharacterEscapes.AddRow("\\e", "Matches an escape.");
        rtCharacterEscapes.AddRow("\\v", "Matches a vertical tab.");
        rtCharacterEscapes.AddRow("\\a", "Matches the bell character.");
        rtCharacterEscapes.AddRow("\\<em>octal</em>", "Matches a character, where <em>octal</em> is the octal representation of that character.", "~/tester?p=%5c145&i=the+octal+value+for+%22e%22+is+145");
        rtCharacterEscapes.AddRow("\\x<em>hex</em>", "Matches a character, where <em>hex</em> is the two digit hexadecimal representation of that character.", "~/tester?p=%5cx65&i=the+hex+value+for+%22e%22+is+65");
        rtCharacterEscapes.AddRow("\\u<em>unicode</em>", "Matches a unicode character, where <em>unicode</em> is the four digit hexadecimal representation of that unicode character.", "~/tester?p=%5cu0065&i=the+unicode+value+for+%22e%22+is+0065");
        rtCharacterEscapes.AddRow("\\c<em>character</em>", "Matches an ASCII control character specified by <em>character</em>.");

        // anchors
        rtAnchors.AddRow("^", "Matches the beginning of the input.","~/tester?p=%5e&i=the+beginning...");
        rtAnchors.AddRow("$", "Matches the end of the input, or the point before a final <strong>\\n</strong> at the end of the input.", "~/tester?p=%24&i=...of+the+end");
        rtAnchors.AddRow("\\A", "Matches the beginning of the input. Identical to <strong>^</strong>, except it is unaffected by the multiline option.", "~/tester?p=%5cA&i=even+with+multiline+on%2c%0d%0aonly+the+beginning%0d%0aof+the+first+line+is+matched&o=m");
        rtAnchors.AddRow("\\Z", "Matches the end of the input, or the point before a final <strong>\\n</strong> at the end of the input. Identical to <strong>$</strong>, except it is unaffected by the multiline option.", "~/tester?p=%5cZ&i=even+with+multiline+on%2c%0d%0aonly+the+end%0d%0aof+the+last+line+is+matched&o=m");
        rtAnchors.AddRow("\\z", "Matches the end of the input, without exception.", "~/tester?p=%5cz&i=matches+only+the+end+of+the+input%2c%0d%0aeven+with+multiline+on%2c%0d%0aand+even+with+an+extra+line+break+at+the+end%0d%0a&o=m");
        rtAnchors.AddRow("\\G", "Matches the point that the previous match ended. Used to find contiguous matches.", "~/tester?p=%5cG%5cp%7bL%7d%2b+%3f&i=the+matches+keep+coming%2c+until+the+comma+messes+it+up");
        rtAnchors.AddRow("\\b", "Matches any word boundary. Specifically, any point between a <strong>\\w</strong> and a <strong>\\W</strong>.", "~/tester?p=%5cbp%5cw*&i=this+pattern+picks+out+words+that+start+with+the+letter+p");
        rtAnchors.AddRow("\\B", "Matches any point that is not a word boundary. Specifically, any point not between a <strong>\\w</strong> and a <strong>\\W</strong>.", "~/tester?p=%5cBt&i=this+finds+all+the+t's+that+do+not+appear+at+the+beginning+of+words");

        // grouping constructs
        rtGroupingConstructs.AddRow("(<em>subpattern</em>)", "Captures <em>subpattern</em> as an unnamed group.", "~/tester?p=(%5cw%2b)&i=every+word+is+captured+in+a+group");
        rtGroupingConstructs.AddRow("(?&lt;<em>name</em>&gt;<em>subpattern</em>)", "Captures <em>subpattern</em> as a named group specified by <em>name</em>.", "~/tester?p=(%3f%3cword%3e%5cw%2b)&i=every+word+is+captured+in+a+named+group");
        rtGroupingConstructs.AddRow("(?&lt;<em>name</em>-<em>previous</em>&gt;<em>subpattern</em>)", "Balancing group definition. This allows nested constructs to be matched, such as parentheses or HTML tags. The previously defined group to balance against is specified by <em>previous</em>. Captures <em>subpattern</em> as a named group specified by <em>name</em>, or <em>name</em> can be omitted to capture as an unnamed group. For more information, check out Morten Maate's tutorial on <a href=\"http://www.codeproject.com/KB/recipes/Nested_RegEx_explained.aspx\" target=\"_blank\">matching nested constructs</a>.", "~/tester?p=(((%3f%3copen%3e%3cspan%3e)%5b%5e%3c%5d*)%2b(%5b%5e%3c%5d*(%3f%3cclose-open%3e%3c%2fspan%3e))%2b)%2b(%3f(open)(%3f!))&i=this+%3cspan%3epattern%3c%2fspan%3e+captures+all+%3cspan%3eoutermost+%3cspan%3e(not+nested)%3c%2fspan%3e+balanced+span+tags%3c%2fspan%3e%2c+so+if+we+leave+a+span+tag+open%2c+%3cspan%3eit+won't+be+captured");
        rtGroupingConstructs.AddRow("(?:<em>subpattern</em>)", "Noncapturing group. Allows the use of parentheses without <em>subpattern</em> being captured into a group.", "~/tester?p=(%3f%3aaa%7cbb%7ccc)(dd%7cee%7cff)(%3f%3agg%7chh%7cii)&i=aaddgg+bbeehh+ccffii");
        rtGroupingConstructs.AddRow("(?<em>enabled</em>-<em>disabled</em>:<em>subpattern</em>)", "Allows <em>subpattern</em> to be matched with different options than the rest of the pattern. Any inline option characters in <em>enabled</em> or <em>disabled</em> will enable or disable specific options, respectively. To see what inline option characters are available, check out <a href=\"http://msdn.microsoft.com/en-us/library/yd1hzczs.aspx\" target=\"_blank\">regular expressions options</a>.", "~/tester?p=(%3fi%3aa%7cb%7cc)(d%7ce%7cf)&i=ad+Be+CF");
        rtGroupingConstructs.AddRow("(?=<em>subpattern</em>)", "Zero-width positive lookahead assertion. Continues matching only if <em>subpattern</em> matches on the right.", "~/tester?p=%5cw%2b(%3f%3ding%5cb)&i=this+finds+the+root+part+of+words+that+end+in+%22ing%22%3a+walking%2c+talking%2c+chewing+bubble+gum");
        rtGroupingConstructs.AddRow("(?!<em>subpattern</em>)", "Zero-width negative lookahead assertion. Continues matching only if <em>subpattern</em> does not match on the right.", "~/tester?p=%5cb%5cw%2b%5c.(%3f!exe)%5cw%2b%5cb&i=this+catches+things+that+look+like+file+names%2c+except+for+%22exe%22+files%0d%0aimage.jpg%0d%0apage.html%0d%0avirus.exe");
        rtGroupingConstructs.AddRow("(?&lt;=<em>subpattern</em>)", "Zero-width positive lookbehind assertion. Continues matching only if <em>subpattern</em> matches on the left.", "~/tester?p=(%3f%3c%3d%5c%24)%5cd%2b&i=this+picks+out+dollar+values%2c+like+%2441+and+%24200%2c+without+catching+the+%22%24%22+(numbers+not+preceded+by+a+dollar+sign%2c+like+23+and+678%2c+are+not+matched)");
        rtGroupingConstructs.AddRow("(?&lt;!<em>subpattern</em>)", "Zero-width negative lookbehind assertion. Continues matching only if <em>subpattern</em> does not match on the left.", "~/tester?p=%5cb%5cw(%3f%3c!%5baeiou%5d)%5cw*&i=this+finds+all+words+that+do+not+start+with+a+vowel");
        rtGroupingConstructs.AddRow("(?&gt;<em>subpattern</em>)", "Prevents backtracking over <em>subpattern</em>, which can improve performance.");

        // quantifiers
        rtQuantifiers.AddRow("*", "Matches previous element zero or more times.", "~/tester?p=co*kie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("+", "Matches previous element one or more times.", "~/tester?p=co%2bkie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("?", "Matches previous element zero or one times.", "~/tester?p=co%3fkie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("{<em>n</em>}", "Matches previous element exactly <em>n</em> times.", "~/tester?p=co%7b2%7dkie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("{<em>n</em>,}", "Matches previous element at least <em>n</em> times.", "~/tester?p=co%7b2%2c%7dkie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("{<em>n</em>,<em>m</em>}", "Matches previous element at least <em>n</em> times and at most <em>m</em> times.", "~/tester?p=co%7b1%2c2%7dkie&i=ckie%0d%0acokie%0d%0acookie%0d%0acooooookie");
        rtQuantifiers.AddRow("*?", "Matches previous element zero or more times, but as few times as possible.", "~/tester?p=ba(ka)*%3f&i=bakakaka!");
        rtQuantifiers.AddRow("+?", "Matches previous element one or more times, but as few times as possible.", "~/tester?p=ba(ka)%2b%3f&i=bakakaka!");
        rtQuantifiers.AddRow("??", "Matches previous element zero or one times, but as few times as possible.", "~/tester?p=ba(ka)%3f%3f&i=bakakaka!");
        rtQuantifiers.AddRow("{<em>n</em>,}?", "Matches previous element at least <em>n</em> times, but as few times as possible.", "~/tester?p=ba(ka)%7b2%2c%7d%3f&i=bakakaka!");
        rtQuantifiers.AddRow("{<em>n</em>,<em>m</em>}?", "Matches previous element at least <em>n</em> times and at most <em>m</em> times, but as few times as possible.", "~/tester?p=ba(ka)%7b1%2c3%7d%3f&i=bakakaka!");

        // backreference constructs
        rtBackreferenceConstructs.AddRow("\\<em>number</em>", "Matches the value of a previously captured group, specified by <em>number</em>.", "~/tester?p=%5cb(%5cw)%5cw*%5c1%5cb&i=finds+all+words+(at+least+two+characters+long)+that+start+and+end+with+the+same+letter+(like+grinding+and+trumpet)");
        rtBackreferenceConstructs.AddRow("\\k&lt;<em>name</em>&gt;", "Matches the value of a previously captured named group, specified by <em>name</em>.", "~/tester?p=(%3f%3cpunctuation%3e%5cp%7bP%7d)%5cw%2b%5ck%3cpunctuation%3e&i=find+all+words+wrapped+in+matching+punctuation%3a+%23zip%23+*pow*+!bam!%0d%0anotice+(blah)+doesn't+match+because+%22(%22+is+not+the+same+character+as+%22)%22");

        // alternation constructs
        rtAlternationConstructs.AddRow("|", "Functions as a logical or. Matches any elements that it separates.", "~/tester?p=penguin%7cbear%7clion&i=lion+cat+bear+turtle+dolphin+penguin");
        rtAlternationConstructs.AddRow("(?(<em>subpattern</em>)<em>yes</em>|<em>no</em>)", "Treats <em>subpattern</em> as a zero-width assertion to check if it matches. If so, attempts to match with the <em>yes</em> subpattern. Otherwise, tries the <em>no</em> subpattern. The <strong>|<em>no</em></strong> part is optional.", "~/tester?p=%5cb(%3f(%5cw%2bday%5cb)%5cw%7b3%7d%7c%5cw%2b)&i=matches+the+first+three+letters+of+any+word+that+ends+in+%22day%22%2c+like+sunday+or+saturday%2c+otherwise+matches+the+whole+word");
        rtAlternationConstructs.AddRow("(?(<em>group</em>)<em>yes</em>|<em>no</em>)", "Checks if a previously defined group was succesfully captured, specified by <em>group</em>, which can be a number or a name for a named group. If so, attempts to match with the <em>yes</em> subpattern. Otherwise, tries the <em>no</em> subpattern. The <strong>|<em>no</em></strong> part is optional.", "~/tester?p=%5cb(%5baeiou%5d)%3f(%3f(1)%5cw%7c%5cw%2b)&i=all+words+that+start+with+vowels+will+only+have+their+first+two+characters+matched%2c+while+all+other+words+will+be+matched+in+full");

        // substitutions
        rtSubstitutions.AddRow("$<em>number</em>", "Substitutes the value of a group, specified by <em>number</em>.");
        rtSubstitutions.AddRow("${<em>name</em>}", "Substitutes the value of a named group, specified by <em>name</em>.");
        rtSubstitutions.AddRow("$$", "Substitutes the $ character.");
        rtSubstitutions.AddRow("$&", "Substitutes the entire match.");
        rtSubstitutions.AddRow("$`", "Substitutes all input text found before the match.");
        rtSubstitutions.AddRow("$'", "Substitutes all input text found after the match.");
        rtSubstitutions.AddRow("$+", "Substitutes the last group that was captured.");
        rtSubstitutions.AddRow("$_", "Substitutes the entire input.");

        // miscellaneous
        rtMiscellaneous.AddRow("(?<em>enabled</em>-<em>disabled</em>)", "Changes options in the middle of a pattern. Any inline option characters in <em>enabled</em> or <em>disabled</em> will enable or disable specific options, respectively. To see what inline option characters are available, check out <a href=\"http://msdn.microsoft.com/en-us/library/yd1hzczs.aspx\" target=\"_blank\">regular expressions options</a>.");
        rtMiscellaneous.AddRow("(?# <em>comment</em>)", "Inline comment, not evaluated as part of the pattern.", "~/tester?p=needle(%3f%23+this+will+find+a+needle)&i=hayneedlestack");
        rtMiscellaneous.AddRow("# <em>comment</em>", "End of line comment, not evaluated as part of the pattern. The Ignore Whitespace option must be enabled to use this.", "~/tester?p=nee+%23+this+will+find+a+nee...%0d%0adle+%23+...dle+(the+split+means+nothing+when+white-space+is+ignored)&i=hayneedlestack&o=x");

        // is an elemet search term present?
        string element = Request.QueryString["s"];
        if (!string.IsNullOrEmpty(element))
        {
            // preset the search box to this value
            txtElement.Text = element;

            // only show elements that contain search term and count how many
            int countVisibleRows = 0;
            countVisibleRows += rtCharacterClasses.FilterVisibility(txtElement.Text);
            countVisibleRows += rtCharacterEscapes.FilterVisibility(txtElement.Text);
            countVisibleRows += rtAnchors.FilterVisibility(txtElement.Text);
            countVisibleRows += rtGroupingConstructs.FilterVisibility(txtElement.Text);
            countVisibleRows += rtQuantifiers.FilterVisibility(txtElement.Text);
            countVisibleRows += rtBackreferenceConstructs.FilterVisibility(txtElement.Text);
            countVisibleRows += rtAlternationConstructs.FilterVisibility(txtElement.Text);
            countVisibleRows += rtSubstitutions.FilterVisibility(txtElement.Text);
            countVisibleRows += rtMiscellaneous.FilterVisibility(txtElement.Text);

            // if no rows are visible, show "no results" message (initially, at least)
            if (countVisibleRows == 0)
            {
                pnlNoResults.Style["display"] = "block";
            }
        }
    }

    #endregion
}
