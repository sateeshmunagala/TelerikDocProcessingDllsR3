using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the Actions object to invoke actions against web pages.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\BrowserActions.htm")]
    public class BrowserActions : BaseTest
    {
        private const string TESTPAGE = @"BrowserActions.htm";

        #region Notes

        //
        // 1. We are alternating between using [FindExpression()] and Find.Byxxx to locate elements
        //    on the page. You get to choose the approach you are more comfortable with.
        //    Please refer to the 'FindingElements' class for more details and samples
        // 2. Some of these tests will run fast for you to notice the elements change. Setup
        //    break point and step through these tests to easily see the actions occurring on the page.
        //

        #endregion

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>BrowserActions</title>
        //        <script type="text/javascript">
        //            function toggleBg(toggle)
        //            {
        //                if (toggle)
        //                {
        //                    document.body.bgColor = "Yellow";
        //                    lbl1.innerText = "TOGGLE ON";
        //                }
        //                else
        //                {
        //                    document.body.bgColor = "";
        //                    lbl1.innerText = "TOGGLE OFF";
        //                }
        //            }
        //        </script>
        //    </head>
        //    <body>
        //        <input id="btn1" type="button" onclick="toggleBg(true)" value="Color On" />
        //        <input id="btn2" type="button" onclick="toggleBg(false)" value="Color Off" />
        //        <input id="check1" type="checkbox" />
        //        <input id="radio1" type="radio"  checked="checked" />
        //        <input id="textbox1" type="text"/>
        //        <textarea id="textarea1" rows="10" cols="10"></textarea>
        //        <select id="selection1" >
        //            <option value="1" >One</option>
        //            <option value="2" >Two</option>
        //            <option value="3" >Three</option>
        //        </select>
        //        <a href="http://www.google.com">Search The Web</a>
        //        <h1 id="lbl1">TOGGLE OFF</h1>
        //    </body>
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
        [Description("Clicking an element on the page.")]
        public void ClickAnElement()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser();
            // Some test cases may require that all cookies be cleared ahead of time.
            // WebAii does not care but depending on your environment and your specific
            // test case you may care.
            ActiveBrowser.ClearCache(ArtOfTest.WebAii.Core.BrowserCacheType.Cookies);
            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Find the elements on the page using the Find.BYxxx methods.
            Element toggleOn = Find.ById("btn1");
            Element toggleOff = Find.ById("btn2");

            // Now click the toggleOn button.
            Actions.Click(toggleOn);

            // Now click the toggleOff button.
            Actions.Click(toggleOff);

            // Now click the link
            Actions.Click(Find.ByTagIndex("a", 0));
            Assert.IsTrue(ActiveBrowser.Url.Contains("google"));

            // DO ANY TEST VERIFICATION HERE
        }

        [TestMethod]
        [Description("Setting the text for a textbox or a textarea")]
        [Find("MyTextBox","id=textbox1")]
        [Find("MyTextArea","id=textarea1")]
        public void SetTextForAnElement()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser();
            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Now set the text for the text box
            Actions.SetText(this.Elements.GetHtml("MyTextBox"), "This is a TEST for TextBox!");

            // Now set the text for the text area.
            Actions.SetText(this.Elements.GetHtml("MyTextArea"), "This is a TEST for TextArea!");

            // DO ANY TEST VERIFICATION HERE

        }

        [TestMethod]
        [Description("Selecting an item from a dropdown options list")]
        [Find("optionslist", "id=selection1")]
        public void SelectFromDropDownOrList()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser();

            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Select using Value
            Actions.SelectDropDown(this.Elements.GetHtml("optionslist"), "2", true);

            // Select using the item text.
            Actions.SelectDropDown(this.Elements.GetHtml("optionslist"), "One");

            // Select using index
            Actions.SelectDropDown(this.Elements.GetHtml("optionslist"), 2);

            // DO ANY TEST VERIFICATION HERE
        }

        [TestMethod]
        [Description("Check a checkbox or a radio button")]
        public void CheckAnElement()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser();

            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Locate the checkbox and the radio button.
            Element checkBox = Find.ById("check1");
            Element radio = Find.ById("radio1");

            // Check the checkbox
            Actions.Check(checkBox, true);

            // Uncheck the radio button
            Actions.Check(radio, false);

            // DO ANY TEST VERIFICATION HERE
        }

        #endregion
    }
}
