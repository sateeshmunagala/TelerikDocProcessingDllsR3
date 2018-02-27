using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to access HTMLPopups.
    /// </summary>
    [TestClass]
    public class HTMLPopups : BaseTest
    {
        private const string TESTPAGE = "HTMLPopups.htm";

        #region Page Under Test
        //<html>
        //<head>
        //    <title>HTMLPopups</title>
        //    <script type="text/javascript">
        //    function OpenWindow()
        //    {
        //        window.open ('http://www.google.com',
        //        'newwindow', 'height=100,width=400, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, directories=no,status=no')
        //    }
        //    </script>

        //</head>
        //<body>
        //    <a onclick="OpenWindow()">Invoke HTML Popup</a>
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
        [DeploymentItem(@"Pages\HTMLPopups.htm")]
        [Find("mylink", "TagIndex=a:0")]
        public void AccessHTMLPopups()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Set new browser tracking to enabled.
            // This will make all new browser instances connect to
            // the Manager.
            Manager.SetNewBrowserTracking(true);

            // Invoke the popup
            Actions.Click(this.Elements.GetHtml("mylink"));

            // Wait for the new browser instance to connect.
            Manager.WaitForNewBrowserConnect("www.bing.com", true, 3000);

            // disable new browser tracking
            Manager.SetNewBrowserTracking(false);

            // At this point the browsers collection should have two
            // browser instances. ActiveBrowser should be set
            // to the popup instance.
            Assert.AreEqual(2, Manager.Browsers.Count);
            Assert.IsTrue(ActiveBrowser.Url.Contains("www.bing.com"));

            // Close the popup
            ActiveBrowser.Close();

            // At this point, the browsers collection should contain
            // only one instance and the ActiveBrowser is set to
            // the last launched active instance.
            Assert.AreEqual(1, Manager.Browsers.Count);
            Assert.IsTrue(ActiveBrowser.Url.Contains(Path.GetFileName(TESTPAGE)));
        }

        #endregion
    }
}
