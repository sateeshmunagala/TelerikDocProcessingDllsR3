using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the DOM Element object to inspect a specific element (check attributes ...etc)
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\DOMElement.htm")]
    public class DOMElement : BaseTest
    {
        private const string TESTPAGE = "DOMElement.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.

        #endregion

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //<head>
        //<title>DOMElement</title>
        //<style type="text/css">
        //.style-span-1 {
        //    font-family: "Courier New", Courier, monospace;
        //    color: #0000FF;
        //}
        //.style-span-2 {
        //    font-family: "Arial Narrow";
        //    color: #800080;
        //}
        //.style-span-3 {
        //    font-family: Elephant;
        //    color: #FF00FF;
        //}
        //</style>
        //</head>
        //<body>
        //<div id="div1">
        //    DIV1 TEXT
        //    <div id="div2">
        //        DIV2 TEXT
        //        <span id="span1" class="style-span-1" lang="en-us">SPAN1 TEXT</span>
        //        <span id="span2" class="style-span-2">SPAN2 TEXT</span>
        //        <span id="span3" class="style-span-3">SPAN3 TEXT</span>
        //    </div>
        //</div>
        //</body>
        //</html>
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
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

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
        [Description("Examples of how to inspect an element's attributes")]
        public void InspectingAttributes()
        {
            // Let's start with the span1 element.
            Element span1 = Find.ById("span1");
            // we will assert different properties of this element using the DOM.
            Assert.IsTrue(span1.IdAttributeValue.Equals("span1"));
            Assert.IsTrue(span1.NameAttributeValue.Equals(string.Empty));
            Assert.IsTrue(span1.CssClassAttributeValue.Equals("style-span-1"));

            Assert.IsTrue(span1.GetAttribute("lang").Value.Equals("en-us"));
            Assert.IsTrue(span1.GetAttribute("AttrNotExist") == null);
            Assert.IsTrue(span1.ContainsAttribute(new iAttribute("lang", "en-us"), false, StringComparison.OrdinalIgnoreCase, false));

            // Children are basically only markup children. (Does not include text or comments);
            Assert.IsTrue(span1.Children.Count == 0);
            // ChildNodes include all including text and comments. Children is a subset of ChildNodes.
            Assert.IsTrue(span1.ChildNodes.Count == 1);

            Assert.IsTrue(span1.TagNameIndex == 0); // the first span that occurs on the page.
        }

        [TestMethod]
        [Description("Examples of how to navigate the DOM")]
        public void NavigatingDOM()
        {
            // Let's start with the span1 element.
            Element span1 = Find.ById("span1");

            // Go UP the DOM tree.
            Element parentDiv = span1.Parent;
            Assert.IsTrue(parentDiv.IdAttributeValue.Equals("div2"));

            // Loop DOWN through children.
            Log.WriteLine("Looping through ChildNodes");
            foreach (Element e in parentDiv.ChildNodes)
            {
                Log.WriteLine(e.ToString());
            }

            Log.WriteLine("Looping through Children");
            foreach (Element e in parentDiv.Children)
            {
                Log.WriteLine(e.ToString());
            }

            // check out the log
        }

        [TestMethod]
        [Description("Examples of different levels of inspection of the content of a tag")]
        public void InspectingElementContent()
        {
            // Let's start with the first div on the page.
            Element div1 = Find.ById("div1");

            // The innerText of this element including all it's child elements.
            Log.WriteLine(div1.InnerText);

            // The outerMarkup of this element
            Log.WriteLine(div1.OuterMarkup);

            // The innerMarkup of this element.
            Log.WriteLine(div1.InnerMarkup);

            // The innerText of only this element.
            Log.WriteLine(div1.TextContent);

            // The start tag content.
            Log.WriteLine(div1.Content);

            // Check out the log to see the differences.
        }

        [TestMethod]
        [Description("Examples of navigating siblings")]
        public void SiblingNavigation()
        {
            // Let's start with the span1 element.
            Element span1 = Find.ById("span1");
            Element span2 = span1.GetNextSibling();
            Element span3 = span2.GetNextSibling();

            // Let's verify we got the right elements
            Assert.IsTrue(span1.IdAttributeValue.Equals("span1"), "Actual ID: {0}", span1.IdAttributeValue);
            Assert.IsTrue(span2.IdAttributeValue.Equals("span2"), "Actual ID: {0}", span2.IdAttributeValue);
            Assert.IsTrue(span3.IdAttributeValue.Equals("span3"), "Actual ID: {0}", span3.IdAttributeValue);

            // Now let's navigate backwards
            span2 = span3.GetPreviousSibling();
            span1 = span2.GetPreviousSibling();
            Assert.IsTrue(span1.IdAttributeValue.Equals("span1"), "Actual ID: {0}", span1.IdAttributeValue);
            Assert.IsTrue(span2.IdAttributeValue.Equals("span2"), "Actual ID: {0}", span2.IdAttributeValue);
            Assert.IsTrue(span3.IdAttributeValue.Equals("span3"), "Actual ID: {0}", span3.IdAttributeValue);

            // Now let's look at parent and children
            Element parentDiv = span1.Parent;
            // ChildNodes includes all nodes including text and comments. Children is a subset of ChildNodes.
            Assert.AreEqual(parentDiv.ChildNodes.Count, 4, "ChildNodes = {0}", parentDiv.ChildNodes);
            // Children are basically only markup children. (Does not include text or comments);
            Assert.AreEqual(parentDiv.Children.Count, 3, "Children = {0}", parentDiv.ChildNodes);

            span1 = parentDiv.Children[0];
            span2 = parentDiv.Children[1];
            span3 = parentDiv.Children[2];
            Assert.IsTrue(span1.IdAttributeValue.Equals("span1"), "Actual ID: {0}", span1.IdAttributeValue);
            Assert.IsTrue(span2.IdAttributeValue.Equals("span2"), "Actual ID: {0}", span2.IdAttributeValue);
            Assert.IsTrue(span3.IdAttributeValue.Equals("span3"), "Actual ID: {0}", span3.IdAttributeValue);
        }

        [TestMethod]
        [Description("Examples of interacting directly with the DOM")]
        public void DomTests()
        {
            // Let's check the title
            string pageTitle = ActiveBrowser.PageTitle;
            Assert.AreEqual("DOMElement", pageTitle, "Actual page title = \"{0}\"", pageTitle);

            // Let's start with the span1 element.
            Element span1 = Find.ById("span1");

            // we will assert different properties of this element using the DOM.
            Assert.IsTrue(span1.CssClassAttributeValue.Equals("style-span-1"), "Actual style = \"{0}\"", span1.CssClassAttributeValue);
            Assert.IsTrue(span1.GetAttribute("lang").Value.Equals("en-us"));
            Assert.IsTrue(span1.ContainsAttribute(
                new iAttribute("lang", "en-us"), false, StringComparison.OrdinalIgnoreCase, false));

            // navigate the DOM upward.
            Element spanParent = span1.Parent;

            Log.WriteLine(spanParent.InnerText);
            Log.WriteLine(spanParent.OuterMarkup);
            Log.WriteLine(spanParent.InnerMarkup);
            Log.WriteLine(spanParent.TextContent);
        }

        #endregion
    }
}
