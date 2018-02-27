using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.Common;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the built in HtmlControls.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\HtmlControls.htm")]
    [DeploymentItem(@"Pages\Waiting.htm")]
    [Find("mainDiv", "id=mainDiv")]
    public class HtmlControls : BaseTest
    {
        private const string PAGE_NAME = @"HtmlControls.htm";
        private const string W_PAGE_NAME = @"Waiting.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.
        //

        #endregion

        #region Additional test attributes

        private TestContext testContextInstance = null;
        /// <summary>
        ///Gets or sets the VS test context which provides
        ///information about and functionality for the
        ///current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }

        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            #region WebAii Initialization

            // Initializes WebAii manager to be used by the test case.
            // If a WebAii configuration section exists, settings will be
            // loaded from it. Otherwise, will create a default settings
            // object with system defaults.
            //
            // Note: We are passing in a delegate to the VisualStudio
            // testContext.WriteLine() method in addition to the Visual Studio
            // TestLogs directory as our log location. This way any logging
            // done from WebAii (i.e. Manager.Log.WriteLine()) is
            // automatically logged to the VisualStudio test log and
            // the WebAii log file is placed in the same location as VS logs.
            //
            // If you do not care about unifying the log, then you can simply
            // initialize the test by calling Initialize() with no parameters;
            // that will cause the log location to be picked up from the config
            // file if it exists or will use the default system settings (C:\WebAiiLog\)
            // You can also use Initialize(LogLocation) to set a specific log
            // location for this test.
            Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            /*

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.DefaultBrowser = BrowserType.FireFox;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

            */

            // Set the current test method. This is needed for WebAii to discover
            // its custom TestAttributes set on methods and classes.
            // This method should always exist in [TestInitialize()] method.
            SetTestMethod(this, (string)TestContext.Properties["TestName"]);

            #endregion

            Manager.LaunchNewBrowser();

            if (TestContext.TestName.StartsWith("Wait"))
            {
                ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, W_PAGE_NAME));
            }
            else
            {
                ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, PAGE_NAME));
            }
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            #region WebAii CleanUp

            // Shuts down WebAii manager and closes all browsers currently running
            this.CleanUp();

            #endregion
        }

        #endregion

        #region Tests

        [TestMethod]
        [Description("Using the Find.By<> object to find strongly typed HtmlControls")]
        public void FindingControls()
        {
            // Find the first table on the page.
            HtmlTable outertable = Find.ByTagIndex<HtmlTable>("table", 0);
            Assert.IsTrue(outertable.Rows.Count == 3);

            // Find the first table inside the outer table
            //
            // Note: HtmlContainerControls have a Find object
            //       associated with them that scopes all the Find.Byxx searches
            //       to elements contained within them only.
            //       So even if you have multiple controls with similar contained
            //       elements, this will help avoid any conflicts.
            //       Also, note how we are referencing the innertable using index 0
            //       since it is the first table inside our outer table.
            //
            HtmlTable innerTable = outertable.Find.ByTagIndex<HtmlTable>("table", 0);

            // Special Find for tables.
            HtmlTableCell cell = innerTable.Find.TableCell("TD21");
            Assert.IsTrue(cell.TextContent.Equals("TD21"));

            // Find all HtmlInputImage controls with 'src' containing partial value 'logo'
            IList<HtmlInputImage> imgCtrls = Find.AllByAttributes<HtmlInputImage>("src=~logo");
            // There should only be one HtmlInputText control. All controls
            // That match the attribute list but fail validation by the HtmlInputText control
            // will be ignored.
            Assert.AreEqual(1, imgCtrls.Count);

            // Find the <div> section containing the Eastern US Division sales report
            HtmlDiv EasternUSDivision = Find.ByContent<HtmlDiv>("Eastern US Division", FindContentType.TextContent);
            Assert.IsNotNull(EasternUSDivision);

            // Traverse the control tree upwards too. You can easily
            // Find the container control of a certain type from its children.

            // Find the owner form of the submit button
            HtmlInputSubmit submitButton = Find.ById<HtmlInputSubmit>("submit");
            HtmlForm form = submitButton.Parent<HtmlForm>();

            // Get the name of the frame targeted by the form
            string myFrame = form.Target;
            Assert.AreEqual("_self", myFrame);

            // Find the 3rd text input field on the form. Index is 0 based, so we enter 2 to mean the 3rd one.
            HtmlInputText input = form.Find.ByTagIndex<HtmlInputText>("input", 2);
            Assert.IsNotNull(input);

            // Find the table that contains this cell.
            HtmlTable table = cell.Parent<HtmlTable>();
            Assert.IsTrue(table.ID.Equals(innerTable.ID));

            // Find the parent table of this inner table.
            HtmlTable table2 = table.Parent<HtmlTable>();
            Assert.IsTrue(table2.ID.Equals(outertable.ID));

            // Find the form this table is contained in.
            HtmlForm form1 = table2.Parent<HtmlForm>();
            Assert.IsTrue(form1.ID.Equals("form1"));

            // Note: if Find.IgnoreFindAllControlMismatch is set to false, the
            // Find.Allxx<>() method will throw when encounters a control that matches the search
            // criteria but fails to be the desired HTMLControl.

            // TIP: The above example is to show the functionality of the Find.Byxx<> function.
            // It is not recommended to use loose attribute finds like the one above since it will
            // match many controls on the page that will be ignored. The better the find constraints
            // are the more performant the search is.
            // For example, the above Find can be enhanced by doing ("id=~1","type=text")
        }

        [TestMethod]
        [Description("Easily start using HtmlControls within your existing tests")]
        public void MigratingElementObjectToHtmlControls()
        {
            // If you already have tests written using WebAii that you want to gradually move to
            // you can use the HTML wrapper class's constructor passing in the existing element
            // object from you current code.

            // The first line is a line of code in your existing test case.
            Element myButtonElement1 = Find.ById("htmlbutton");
            // Add this line to your code to start using the HTML element wrapper class instead.
            HtmlButton buttonObj1 = new HtmlButton(myButtonElement1);

            // Or the next feature is to use the AssignElement() method. Using this method you
            // can wrap any existing element object into one of the new HTML wrapper classes.

            // The first line is a line of code in your existing test case.
            Element myHtmlButtonElement2 = Find.ById("htmlbutton");
            // Create a new control instance and assign the existing element to it.
            HtmlButton buttonObj2 = new HtmlButton();
            buttonObj2.AssignElement(myHtmlButtonElement2);

            // The last advanced feature is the .As<TControl> construct. The .As<TControl>
            // construct acts like a typecast converting your plain element object into the
            // typecast HTML element wrapper object.

            // The first line is a line of code in your existing test case.
            Element myHtmlButtonElement3 = Find.ById("htmlbutton");
            // Make a comparison using the .As<TControl> construct.
            Assert.IsTrue(myHtmlButtonElement3.As<HtmlButton>().InnerText.Equals("Html Button", StringComparison.OrdinalIgnoreCase));
        }

        [TestMethod]
        [Description("Demonstrates the common methods/properties accessible from all HtmlControls")]
        public void CommonHtmlControlMethodsProperties()
        {
            // All controls have a Click/MouseClick. The .Click invokes a click from the DOM,
            // the MouseClick(), moves the mouse to the controls and clicks it.

            //
            // CLICKING
            //
            Find.ById<HtmlInputButton>("button1").Click();
            ActiveBrowser.Refresh();
            Find.ById<HtmlInputButton>("button1").MouseClick();

            // You can capture any element on the page using the .Capture()
            Find.ById<HtmlInputImage>("image1").Capture("myfile"); // Will be stored to the Log.LogLocation

            //
            // CHECKING
            //
            // Check a checkbox and invoke the onclick event.
            HtmlInputCheckBox ck = Find.ById<HtmlInputCheckBox>("checkbox1");
            if (ActiveBrowser.BrowserType == BrowserType.Safari)
            {
                // Unfortunately the way Safari behaves is different then the other browsers.
                ck.Check(true, false);
            }
            else
            {
                ck.Check(true, true);
            }

            // Query the checked state
            Assert.IsTrue(ck.Checked);
            // You can also simply set the value without invoking the clicked event.
            ck.Checked = false;
            Assert.IsFalse(ck.Checked);

            // Get whether a checkbox is enabled or disabled.
            HtmlInputCheckBox cks = Find.ById<HtmlInputCheckBox>("checkbox1");
            bool disabled = cks.GetValue<bool>("disabled");

            // Disable it
            cks.SetValue<string>("disabled", "true");
            Assert.IsTrue(cks.GetValue<bool>("disabled"));

            // Is it visible
            Assert.IsTrue(cks.IsVisible());

            // When the contents of a div element are larger than the declared
            // width or height of element it automatically adds scroll bars.
            // When that happens we can scroll the contents of the div element.
            // The value represents the offset in pixels to scroll the contents.
            HtmlDiv infoDiv = Find.ById<HtmlDiv>("AutoInfo");
            infoDiv.ScrollTop = 50;
            infoDiv.ScrollLeft = 75;

            //
            // SELECTING
            //
            HtmlSelect select = Find.ById<HtmlSelect>("color_product");
            Assert.IsTrue(select.SelectedOption.Value.Equals("Blue"));
            Assert.IsTrue(select.SelectedOption.Text.Equals("Color : Blue"));

            select.SelectByIndex(1);
            Assert.IsTrue(select.SelectedOption.Text.Equals("Color : Green"));

            //
            // SET TEXT
            //
            Find.ById<HtmlTextArea>("textarea1").Text = "NEW TEXT";

            // Access common methods
            HtmlAnchor link = Find.ByAttributes<HtmlAnchor>("href=~google");
            Assert.IsTrue(link.Attributes.Count == 3);
            Assert.IsTrue(link.BaseElement.TextContent.Trim().Equals("Link"));

            // Invoke any events on the control
            link.InvokeEvent(ScriptEventType.OnFocus);
        }

        [TestMethod]
        [Description("Waiting for control attributes")]
        public void WaitForAttributes()
        {
            exec(2);
            this.Elements.GetHtml("mainDiv").Wait.ForAttributes(5000, "align=center", "title=~foo", "lang=fr");
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").GetAttribute("align").Value.Equals("center"));
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").GetAttribute("title").Value.Equals("foofoo"));
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").GetAttribute("lang").Value.Equals("fr"));

            // Should return immediately but not throw.
            this.Elements.GetHtml("mainDiv").Wait.ForAttributesNot(200, "align=bla");

            // Set the attribute align to be center
            this.Elements.GetHtml("mainDiv").SetValue<string>("align", "center");
            this.Elements.GetHtml("mainDiv").SetValue<int>("style.borderWidth", 20);

            exec(3);

            this.Elements.GetHtml("mainDiv").Wait.ForAttributesNot("align=center");
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").GetAttribute("align").Value.Equals("left"));
        }

        [TestMethod]
        [Description("Waiting for control contents")]
        public void WaitForContent()
        {
            exec(4);
            this.Elements.GetHtml("mainDiv").Wait.ForContent(FindContentType.TextContent, "l:hello", 5000);
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").TextContent.Equals("Hello"));

            ActiveBrowser.Refresh(); // Reset everything.
            this.Elements.GetHtml("mainDiv").Refresh(true);
            exec(4);
            this.Elements.GetHtml("mainDiv").Wait.ForContent(FindContentType.TextContent, "p:hell", 5000);
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").TextContent.Equals("Hello"));


            ActiveBrowser.Refresh(); // Reset everything.
            this.Elements.GetHtml("mainDiv").Refresh(true);
            exec(4);
            this.Elements.GetHtml("mainDiv").Wait.ForContent(FindContentType.InnerText, "l:Hello", 5000);
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").TextContent.Equals("Hello"));


            this.Elements.GetHtml("mainDiv").SetValue<string>("innerHTML", "bal");
            this.Elements.GetHtml("mainDiv").Refresh(true);
            exec(4);
            this.Elements.GetHtml("mainDiv").Wait.ForContentNot(FindContentType.TextContent, "l:bal", 5000);
            Assert.IsTrue(this.Elements.GetHtml("mainDiv").TextContent.Equals("Hello"));
        }

        [TestMethod]
        [Description("Waiting for control styles")]
        public void WaitForStyles()
        {
            HtmlDiv div = this.Elements.GetHtml("mainDiv").As<HtmlDiv>();

            exec(5);
            div.Wait.ForStyles("backgroundColor=blue");

            ActiveBrowser.Refresh();
            div.Refresh();

            exec(5);

            string s = div.GetComputedStyleValue("backgroundColor");

            // Wait until background color is NOT yellow.
            div.Wait.ForStylesNot(true, "backgroundColor=yellow");

        }

        [TestMethod]
        [Description("Waiting for visibility changes")]
        public void WaitForVisible()
        {
            HtmlDiv div = this.Elements.GetHtml("mainDiv").As<HtmlDiv>();

            Assert.IsTrue(div.IsVisible());
            div.SetValue<string>("style.visibility", "hidden");

            Assert.IsFalse(div.IsVisible());
            div.SetValue<string>("style.visibility", "visible");

            exec(8);
            div.Wait.ForVisibleNot();
            Assert.IsFalse(div.IsVisible());

            exec(9);
            div.Wait.ForVisible();
            Assert.IsTrue(div.IsVisible());

        }

        [TestMethod]
        [Description("Demonstrate how to use the HtmlSelect control")]
        public void HtmlSelectExamples()
        {
            /// You can find the HtmlSelect control via a number of methods. Use any of the following
            /// Find the control by assigned name
            HtmlSelect mySelect1 = Find.ByName<HtmlSelect>("ctl00$MainContent$drpMake");
            /// Or find the control by it's assigned ID
            HtmlSelect mySelect2 = Find.ById<HtmlSelect>("ctl00_MainContent_drpMake");
            /// Using a partial match to find the control even if the name changes slightly
            HtmlSelect mySelect3 = Find.ByAttributes<HtmlSelect>("id=~_drpMake");
            /// Find all the controls matching a pattern which can then be iterated through
            IList<HtmlSelect> mySelectList = Find.AllByAttributes<HtmlSelect>("id=~_product");

            /// Verify the select contains the expected options
            Assert.AreEqual(76, mySelect1.Options.Count);
            Assert.IsTrue(mySelect1.Options[0].Text.Equals("- Select -"));
            Assert.IsTrue(mySelect1.Options[1].Text.Equals("Alfa Romeo"));
            Assert.IsTrue(mySelect1.Options[2].Text.Equals("AM General"));
            Assert.IsTrue(mySelect1.Options[3].Text.Equals("AMC"));

            /// Iterate through the list of selects
            foreach (HtmlSelect aSelect in mySelectList)
            {
                if (aSelect.ID.Contains("color"))
                {
                    /// Found the Color drop down
                    /// Validate the options for it
                    /// Iterate through all the options looking for something
                    foreach (HtmlOption anOption in aSelect.Options)
                    {
                        Assert.IsTrue(anOption.Text.StartsWith("Color : "));
                    }
                }
                else if (aSelect.ID.Contains("size"))
                {
                    /// Found the Size drop down
                    /// Validate the options for it
                    /// Iterate through all the options looking for something
                    foreach (HtmlOption anOption in aSelect.Options)
                    {
                        Assert.IsTrue(anOption.Text.StartsWith("Size : "));
                    }
                }
                else if (aSelect.ID.Contains("option"))
                {
                    /// Found the Option drop down
                    /// Validate the options for it
                    /// Iterate through all the options looking for something
                    foreach (HtmlOption anOption in aSelect.Options)
                    {
                        Assert.IsTrue(anOption.Text.StartsWith("Option : "));
                    }
                }
                else
                {
                    Assert.Fail("Unrecognized Select drop down found");
                }
            }
        }

        [TestMethod]
        [Description("Demonstrate how you can use the SetValue/GetValue methods to set/get any property on any html control in the DOM")]
        public void SetGetValuesFromDOM()
        {
            // HtmlControl/Elements now have a generic SetValue/GetValue that allows developers
            // to set/get values directly from the DOM for any property on the control. Whether it is inherited,
            // readonly or an emitted attribute.

            // Get whether a checkbox is enabled or disabled.
            HtmlInputCheckBox cks = Find.ById<HtmlInputCheckBox>("checkbox1");
            bool disabled = cks.GetValue<bool>("disabled");

            // Disable it
            cks.SetValue<string>("disabled", "true");
            Assert.IsTrue(cks.GetValue<bool>("disabled"));

            // Is it visible. IsVisible follows the CSS chain to determine if the
            // element is actually visible or not. An element is visible when
            // the CSS the display style is not 'none' and the visibility style
            // is not 'hidden'.
            Assert.IsTrue(cks.IsVisible());

            // Get the color style
            HtmlSpan mySpan = Find.ById<HtmlSpan>("Warning");
            HtmlStyle styleColor = mySpan.GetStyle("color");
            string strColor = mySpan.GetStyleValue("color");

            // Getting the computed style will follow the CSS chain and return the
            // style.
            HtmlStyle styleMargin = mySpan.GetComputedStyle("margin");
            string strMargin = mySpan.GetComputedStyleValue("margin");
        }

        [TestMethod]
        [Description("Demonstrates how to use the HtmlStyle class.")]
        public void HtmlStyleClass()
        {
            // Verify the color of the warning text is Red
            HtmlSpan warningSpan = Find.ById<HtmlSpan>("Warning");
            HtmlStyle warningColorStyle = warningSpan.GetStyle("color");

            // Verify the style's value is a color value.
            // In reality it's not necessary to test using both IsColor and IsInt.
            // They're both used here for demonstration purposes only.
            Assert.IsTrue(warningColorStyle.IsColor());
            Assert.IsFalse(warningColorStyle.IsInt());
            Color warningColor = warningColorStyle.ToColor();
            Assert.AreEqual<Color>(Color.Red, warningColor);

            // If we don't want to use Assert, we can use IsSameColor instead.
            if (!HtmlStyle.IsSameColor(Color.Red, warningColor))
            {
                Log.WriteLine(LogType.Error, string.Format("Warning color is not red. It is {0}", warningColor.ToString()));
            }

            // We have the option of converting a .NET Color object into an HTML color string
            string htmlColorString = HtmlStyle.ToHtmlColor(Color.SaddleBrown);
            htmlColorString = HtmlStyle.ToHtmlColor(Color.FromArgb(33, 44, 55));

            // Let's log the name and value of all the styles attached to this element
            foreach (string strStyle in warningSpan.Styles)
            {
                // NOTE: Some of the styles have a '-' in the middle of the name in the HTML (e.g. TEXT-ALIGN).
                // But when we go to fetch the styles attributes by that name, it doesn't exist in JavaScript.
                // The equivalent does exist without the '-'. So we will blindly strip out any '-' characters
                // prior to calling GetStyle in order to convert it to be JavaScript compatible.
                string modStyle = strStyle.Replace("-", "");
                HtmlStyle aStyle = warningSpan.GetStyle(modStyle);
                Log.WriteLine(LogType.Information, string.Format("Style name: {0}, Style Value {1}",
                    aStyle.Name, aStyle.Value));
            }

            // Verify the margin is set to 30 units (but note we don't know what the unit of measure is)
            HtmlStyle warningMarginStyle = warningSpan.GetStyle("margin");
            Assert.IsTrue(warningMarginStyle.IsInt());
            int warningMargin = warningMarginStyle.ToInt();
            // Unfortunately the value we get from Firefox is different than all other browsers.
            if (ActiveBrowser.BrowserType == BrowserType.FireFox)
                Assert.AreEqual(30303030, warningMargin);
            else
                Assert.AreEqual(30, warningMargin);

            // GetStyle returns the value of an explicit style applied to the element.
            // If the element does not have an explicit style applied, GetStyle returns an empty value.
            // Since our warning span does not have the backgroundColor style explicitly applied to it,
            // GetStyle returns a style with an empty value.
            HtmlStyle backgroundStyle = warningSpan.GetStyle("backgroundColor");
            Assert.IsTrue(string.IsNullOrEmpty(backgroundStyle.Value), "Actual padding value was: \"{0}\"", backgroundStyle.Value);
            // However GetComputedStyle will follow the CSS until it finds the currently active style value.
            // Our warning span is contained within a form that has a backgroundColor style.
            // Therefore GetComputedStyle on the warning span will return the value set in the parent form tag.
            backgroundStyle = warningSpan.GetComputedStyle("backgroundColor");
            Assert.IsFalse(string.IsNullOrEmpty(backgroundStyle.Value), "Actual padding value was: \"{0}\"", backgroundStyle.Value);
        }

        [TestMethod]
        [Description("Demonstrates some of the HtmlAsserts you can take advantage of")]
        public void HtmlAssertTests()
        {
            // Attribute checks
            HtmlSpan span = Find.ById<HtmlSpan>("Warning");
            span.AssertAttribute().Exists("style");
            span.AssertAttribute().Value("style", ArtOfTest.Common.StringCompareType.Contains, "color");

            // Checkbox checks
            HtmlInputCheckBox cbx = Find.ById<HtmlInputCheckBox>("Checkbox1");
            cbx.AssertCheck().IsTrue();
            cbx.Click();
            cbx.AssertCheck().IsFalse();

            // Content checks
            span.AssertContent().InnerText(StringCompareType.Contains, "Warning");
            span.AssertContent().InnerText(StringCompareType.NotContain, "Error");
            span.AssertContent().StartTagContent(StringCompareType.StartsWith, "<span");

            HtmlDiv topdiv = Find.ById<HtmlDiv>("topmost");
            topdiv.AssertContent().TextContent(StringCompareType.Exact, "Top most text");
            topdiv.AssertContent().InnerText(StringCompareType.Exact,
                "Top most textMiddle level textInnermost text");
            topdiv.AssertContent().OuterMarkup(StringCompareType.EndsWith,
                "Innermost text</DIV></DIV></DIV>");
            topdiv.AssertContent().InnerMarkup(StringCompareType.EndsWith,
                "Innermost text</DIV></DIV>");

            // Select checks
            HtmlSelect select = Find.ById<HtmlSelect>("color_product");
            select.AssertSelect().ItemsCountIs(NumberCompareType.Equals, 5);
            select.AssertSelect().SelectedIndex(NumberCompareType.Equals, 0);
            select.AssertSelect().SelectedText(StringCompareType.Exact, "Color : Blue");
            select.AssertSelect().SelectedValue(StringCompareType.Exact, "Blue");

            select.SelectByIndex(3);
            select.AssertSelect().SelectedIndex(NumberCompareType.Equals, 3);
            select.AssertSelect().SelectedText(StringCompareType.Exact, "Color : Orange");
            select.AssertSelect().SelectedValue(StringCompareType.Exact, "Orange");

            select.AssertSelect().TextExists("Color : Black");
            select.AssertSelect().TextExistsNot("Color : Magenta");
            select.AssertSelect().ValueExists("Black");
            select.AssertSelect().ValueExistsNot("Magenta");

            // Style checks
            NameValueCollection styles = span.Styles;
            span.AssertStyle().Font(ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts.HtmlStyleFont.Style, "italic");
            span.AssertStyle().Text(ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts.HtmlStyleText.TextAlign, "right");
            span.AssertStyle().ColorAndBackground(ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts.HtmlStyleColorAndBackground.Color, "red",
                ArtOfTest.WebAii.Controls.HtmlControls.HtmlAsserts.HtmlStyleType.Computed,
                StringCompareType.Exact);

            // Table checks
            HtmlTable table = Find.ById<HtmlTable>("outertable1");
            table.AssertTable().ColumnCount(NumberCompareType.Equals, 3);
            table.AssertTable().ColumnRange(NumberRangeCompareType.InRange, 2, 5);
            table.AssertTable().RowCount(NumberCompareType.Equals, 3);
            table.AssertTable().RowRange(NumberRangeCompareType.OutsideRange, 1, 2);
            table.AssertTable().Contains(StringCompareType.Contains, "TD5");
            table.AssertTable().Contains(StringCompareType.NotContain, "TD37");
        }

        #endregion

        /// <summary>
        /// Helper routine.
        /// </summary>
        /// <param name="action">int number of the action to perform.</param>
        private void exec(int action)
        {
            ActiveBrowser.Actions.InvokeScript(string.Format(@"doAction(""{0}"")", action.ToString()));
        }

    }

    [TestClass]
    [DeploymentItem(@"Pages\HTMLControls.htm")]
    public class SubmitAdTestClass : BaseTest
    {
        private const string TESTPAGE = "HTMLControls.htm";

        #region Additional test attributes

        private TestContext testContextInstance = null;
        /// <summary>
        ///Gets or sets the VS test context which provides
        ///information about and functionality for the
        ///current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }

        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            #region WebAii Initialization

            // Initializes WebAii manager to be used by the test case.
            // If a WebAii configuration section exists, settings will be
            // loaded from it. Otherwise, will create a default settings
            // object with system defaults.
            //
            // Note: We are passing in a delegate to the VisualStudio
            // testContext.WriteLine() method in addition to the Visual Studio
            // TestLogs directory as our log location. This way any logging
            // done from WebAii (i.e. Manager.Log.WriteLine()) is
            // automatically logged to the VisualStudio test log and
            // the WebAii log file is placed in the same location as VS logs.
            //
            // If you do not care about unifying the log, then you can simply
            // initialize the test by calling Initialize() with no parameters;
            // that will cause the log location to be picked up from the config
            // file if it exists or will use the default system settings (C:\WebAiiLog\)
            // You can also use Initialize(LogLocation) to set a specific log
            // location for this test.
            Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            /*

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.DefaultBrowser = BrowserType.FireFox;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

            */

            // Set the current test method. This is needed for WebAii to discover
            // its custom TestAttributes set on methods and classes.
            // This method should always exist in [TestInitialize()] method.
            SetTestMethod(this, (string)TestContext.Properties["TestName"]);

            #endregion

            //
            // Place any additional initialization here
            //
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

            //
            // Place any additional cleanup here
            //

            #region WebAii CleanUp

            // Shuts down WebAii manager and closes all browsers currently running
            this.CleanUp();

            #endregion
        }

        #endregion

        #region Constants

        // Since we know the order of the input fields
        // we can use an enum to more easily reference them.
        enum TextInputFields
        {
            Model,
            Price,
            Phone,
            Email,
            FirstName,
            LastName,
            Company,
            Address,
            City,
            Zip,
            UserName,
        }
        // Same thing for the select drop downs
        enum Selects
        {
            Year,
            Make,
            AllowEmail,
            State
        }

        #endregion

        [TestMethod]
        [Description("Submits a used car classified ad")]
        public void SubmitAdTest()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Find all the required elements on the page
            HtmlForm form = Find.ById<HtmlForm>("Form2");
            IList<HtmlSelect> selectsList = form.Find.AllByTagName<HtmlSelect>("select");
            IList<HtmlInputText> textInputsList = form.Find.AllByTagName<HtmlInputText>("input");
            HtmlInputPassword passwordField = form.Find.ById<HtmlInputPassword>("txtPassword");
            HtmlTextArea description = form.Find.ById<HtmlTextArea>("txtDescription");
            HtmlInputSubmit submit = form.Find.ById<HtmlInputSubmit>("submit");

            // Enter data into the input fields
            Actions.ScrollToVisible(submit.BaseElement, ScrollToVisibleType.ElementBottomAtWindowBottom);
            selectsList[(int)Selects.Year].SelectByText("1956");
            selectsList[(int)Selects.Make].SelectByText("Ford");
            textInputsList[(int)TextInputFields.Model].Text = "T-Bird";
            textInputsList[(int)TextInputFields.Price].Text = "175000";
            textInputsList[(int)TextInputFields.Phone].Text = "555-122-5544";
            textInputsList[(int)TextInputFields.Email].Text = "test@myemail.com";
            selectsList[(int)Selects.AllowEmail].SelectByText("Yes");
            description.Text = "Beautifully restored two tone red & white classic T-Bird. Just like factory mint condition. Actual mileage is 175,600.";
            textInputsList[(int)TextInputFields.FirstName].Text = "Willard";
            textInputsList[(int)TextInputFields.LastName].Text = "Laird";
            textInputsList[(int)TextInputFields.Company].Text = "Laird Auto Restoration";
            textInputsList[(int)TextInputFields.Address].Text = "147 Industrial Dr.";
            textInputsList[(int)TextInputFields.City].Text = "Sacramento";
            selectsList[(int)Selects.State].SelectByText("California");
            textInputsList[(int)TextInputFields.Zip].Text = "22746";
            textInputsList[(int)TextInputFields.UserName].Text = "wlaird335";
            passwordField.Text = "lairdinc";

            // Submit the ad
            submit.Click();
        }
    }
}
