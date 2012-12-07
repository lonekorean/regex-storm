using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace UserControls
{
    public partial class ReferenceTable : UserControlBase
    {
        #region Variables

        private string headerText = "";
        private List<RefRow> refRows = new List<RefRow>();      // collection of reference rows to display

        #endregion

        #region Properties

        public string HeaderText
        {
            get { return headerText; }
            set { headerText = value; }
        }

        #endregion

        #region Constructors

        public ReferenceTable()
        {
            // wire page events
            this.Init += new EventHandler(ReferenceTable_Init);
            this.PreRender += new EventHandler(ReferenceTable_PreRender);
        }

        #endregion

        #region Events

        protected void ReferenceTable_Init(object sender, EventArgs e)
        {
            // wire control events
            dlRefTable.ItemDataBound += new DataListItemEventHandler(this.dlRefTable_ItemDataBound);

            // add head stuff
            MyPage.AddStylesheet("~/Stylesheets/Reference.css");
        }

        protected void ReferenceTable_PreRender(object sender, EventArgs e)
        {
            dlRefTable.DataSource = refRows;
            dlRefTable.DataBind();

            // count visible rows in this table
            int countVisibleRows = 0;
            foreach (RefRow refRow in refRows)
            {
                if (refRow.IsVisible)
                {
                    countVisibleRows++;
                }
            }
            if (countVisibleRows == 0)
            {
                // none of the rows are visible, so hide entire table via CSS
                // (but still render HTML so it can be made visible client-side)
                dlRefTable.Style["display"] = "none";
            }
        }

        protected void dlRefTable_ItemDataBound(Object sender, DataListItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    // grab controls
                    Literal litHeader = (Literal)e.Item.FindControl("litHeader");

                    litHeader.Text = headerText;
                    break;

                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    // grab struct
                    RefRow refRow = (RefRow)e.Item.DataItem;

                    // grab controls
                    TableRow trRef = (TableRow)e.Item.FindControl("trRef");
                    Literal litElement = (Literal)e.Item.FindControl("litElement");
                    Literal litDefinition = (Literal)e.Item.FindControl("litDefinition");
                    HyperLink hlExample = (HyperLink)e.Item.FindControl("hlExample");

                    // set stuff
                    litElement.Text = refRow.Element;
                    litDefinition.Text = refRow.Definition;
                    if (!string.IsNullOrEmpty(refRow.ExampleUrl))
                    {
                        hlExample.NavigateUrl = refRow.ExampleUrl;
                    }
                    else
                    {
                        hlExample.Visible = false;
                    }

                    // row visibility
                    if (!refRow.IsVisible)
                    {
                        // hide row via CSS (but still render HTML so it can be made visible client-side)
                        trRef.Style["display"] = "none";
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void AddRow(string element, string definition)
        {
            AddRow(element, definition, null);
        }

        public void AddRow(string element, string definition, string exampleUrl)
        {
            refRows.Add(new RefRow(element, definition, exampleUrl));
        }

        public int FilterVisibility(string searchTerm)
        {
            int countVisibleRows = 0;

            for(int i = 0; i < refRows.Count; i++)
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    // if you're looking for nothing in particular, you won't be dissappointed...
                    refRows[i].IsVisible = true;
                    countVisibleRows++;
                }
                else
                {
                    // clean element string first
                    string cleanedElement = Regex.Replace(refRows[i].Element, "<em>.*?</em>", "", RegexOptions.IgnoreCase);
                    cleanedElement = cleanedElement.Replace("&lt;", "<");
                    cleanedElement = cleanedElement.Replace("&gt;", ">");

                    // now check
                    if (cleanedElement.Contains(searchTerm))
                    {
                        refRows[i].IsVisible = true;
                        countVisibleRows++;
                    }
                    else
                    {
                        refRows[i].IsVisible = false;
                    }
                }
            }

            return countVisibleRows;
        }

        #endregion

        #region Child Classes

        private class RefRow
        {
            private string element;
            private string definition;
            private string exampleUrl;
            private bool isVisible = true;

            public string Element
            {
                get { return element; }
                set { element = value; }
            }

            public string Definition
            {
                get { return definition; }
                set { definition = value; }
            }

            public string ExampleUrl
            {
                get { return exampleUrl; }
                set { exampleUrl = value; }
            }

            public bool IsVisible
            {
                get { return isVisible; }
                set { isVisible = value; }
            }

            public RefRow(string element, string definition, string exampleUrl)
            {
                Element = element;
                Definition = definition;
                ExampleUrl = exampleUrl;
            }
        }

        #endregion

    }
}