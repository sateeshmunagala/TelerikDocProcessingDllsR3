using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to find elements by ID on your web pages.
    /// </summary>
    [TestClass]
    public class ElementID : BaseTest
    {
        private const string TESTPAGE = "ElementID.htm";

        #region Page Under Test
        //<html xmlns="http://www.w3.org/1999/xhtml">
        //<head>
        //<title></title>
        //</head>
        //<body>
        //<!--testregion id="Region1"-->
        //    <div>
        //        <input type="text" id="input0"/>
        //            <!--testregion id="Region11"-->
        //                <div id="div0" style="background-color:Red">Data Display</div>
        //                <input id="input1" type="button" value="Click Me" onclick="clicked();"/>
        //                   <!--testregion id="Region111"-->
        //                       <a id="a0" href="http://www.google.com">Go Google</a>
        //                           <!--testregion id="Region1111"-->
        //                               <a id="a1" href="http://www.kayak.com">Go Kayak</a>
        //                           <!--/testregion-->
        //                   <!--/testregion-->
        //                   <!--testregion id="Region112"-->
        //                       <div id="div1">Some Data</div>
        //                   <!--/testregion-->
        //                <div id="div2">Some Data
        //                   <a id="a2" href="http://www.kayak.com">Go Kayak</a>
        //                   <a id="a3" href="http://www.kayak.com">Go Kayak</a>
        //                   <a name="a4" href="http://www.kayak.com">Go Kayak</a>
        //                   <a name="a5" href="http://www.kayak.com">Go Kayak</a>
        //                </div>
        //                <div id="div3">Some Data</div>
        //                <div id="div4">Some Data
        //                   <input attr1="Button1"type="button" value="Click Me" onclick="clicked();"/>
        //                   <div id="div5">Some Data</div>
        //                </div>
        //                <div id="div6">Some Data</div>
        //                       <input id="Button2" type="button" value="Click Me" onclick="clicked();"/>
        //                       <input id="Button3" type="button" value="Click Me" onclick="clicked();"/>
        //                </div>
        //                <div id="div7" bla="foo:$$__fdd">Some Data</div>
        //                <div id="div8">Some Data</div>
        //            <!--/testregion-->
        //    </div>
        //<!--/testregion-->
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

        #region Tests

        [TestMethod]
        [DeploymentItem(@"Pages\ElementID.htm")]
        public void FindElementsByID()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Set the short-cuts to the main automation objects.
            Browser brwser = Manager.ActiveBrowser;
            Find rootFind = brwser.Find;

            // All the testregion are initialized here.
            TestRegion r1 = brwser.Regions["Region1"];
            TestRegion r11 = brwser.Regions["Region11"];
            TestRegion r111 = brwser.Regions["Region111"];
            TestRegion r1111 = brwser.Regions["Region1111"];
            TestRegion r112 = brwser.Regions["Region112"];

            //*** Using identification by id.
            Element div0 = r1.Find.ById("div0");

            //*** Using tag name occurrence index.
            Element div = r1.Find.ByTagIndex("div", 0);
            Element div1 = r112.Find.ByTagIndex("div", 0);

            // Some verification to illustrate how the same element that was found
            // using TestRegion Find objects above, can be also found
            // using the main Browser Find object.
            Assert.IsTrue(div.Equals(rootFind.ByTagIndex("div", 0)));
            Assert.IsTrue(div0.Equals(rootFind.ByTagIndex("div", 1)));

            //*** Using attribute identification.
            Assert.IsTrue(div1.Equals(rootFind.ByAttributes("id=div1")));
            Assert.IsNull(rootFind.ByAttributes("id=bla"));
            Assert.IsNotNull(rootFind.ByAttributes("href=http://www.kayak.com"));

            //*** Using partial attribute identification.
            Assert.IsTrue(rootFind.ByAttributes("bla=~__").Equals(rootFind.ById("div7")));
            Assert.IsNull(rootFind.ByAttributes("id=~div7", "bla=~wow"));
            Assert.IsNotNull(rootFind.ByAttributes("onclick=~clicked();", "id=~button2"));

            //*** Using 'All' elements identification.

            // Note here that the first 'div' does not have any id that contains 'div' hence the '- 1'.
            Assert.AreEqual(rootFind.AllByTagName("div").Count - 1,
                rootFind.AllByXPath("/descendant::node()[starts-with(@id,'div')]").Count);

            Assert.AreEqual(5, rootFind.AllByAttributes("href=http://www.kayak.com").Count);
            Assert.AreEqual(2, rootFind.AllByAttributes("id=~button").Count);
            Assert.AreEqual(10, r1.Find.AllByTagName("div").Count);
            Assert.AreEqual(0, r1111.Find.AllByTagName("div").Count);
            Assert.AreEqual(2, r111.Find.AllByTagName("a").Count);
            Assert.AreEqual(9, r11.Find.AllByAttributes("id=~div").Count);

            //*** Using NodeIndexPath identification.
            Assert.IsTrue(r1.Find.ByNodeIndexPath("0/1/1").IdAttributeValue.Equals("input1"));
            Assert.IsTrue(rootFind.ByNodeIndexPath("1/0/0").TagName.Equals("div", StringComparison.OrdinalIgnoreCase));

            //*** Using name
            Assert.IsNull(r1.Find.ByName("bla"));
        }

        [TestMethod]
        [DeploymentItem(@"Pages\ElementID.htm")]
        public void FindElementByExpression()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // The find object will only return an element if all conditions
            // exist. Else NULL.
            Element e = Find.ByExpression(new HtmlFindExpression("TagIndex=div:1", "class=myclass"));
            Assert.IsNotNull(e);
        }

        [TestMethod]
        [DeploymentItem(@"Pages\ElementID.htm")]
        public void FindElementOtherExamples()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            Element e;

            // Find.ById() is implemented as
            e = Find.ById("Button2");
            Assert.IsNotNull(e);

            // Find an element with partial TextContent="Some Data" and has the 'src' attribute
            // containing a partial value = "foo.gif" and class attribute = "myClass"
            e = Find.ByExpression(new HtmlFindExpression("TextContent=~Some Data", "title=~Div", "class=myClass"));
            Assert.IsNotNull(e);

            // Same as above but use InnerMarkup instead
            e = Find.ByExpression(new HtmlFindExpression("InnerMarkup=~Some Data", "title=~Div", "class=myClass"));
            Assert.IsNotNull(e);
        }

        #endregion
    }
}
