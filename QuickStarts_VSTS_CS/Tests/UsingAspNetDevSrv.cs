using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the ASP.NET development server to execute tests.
    /// </summary>
    [TestClass]
    public class UsingAspNetDevSrv : BaseTest
    {
        private const string TESTPAGE = "~/UsingAspNetDevSrv.htm";

        #region Notes
        //
        // 1. In the WebAii Initialization section, we overrided the UseAspNetDevSrv to be true
        //    and set the WebAppPhysicalPath to the location of 'Pages' folder.
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
            // Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.Web.LocalWebServer = LocalWebServerType.AspNetDevelopmentServer;
            settings.Web.WebAppPhysicalPath = Path.GetFullPath(TestContext.TestDeploymentDir);
            settings.LogLocation = TestContext.TestLogsDir;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

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
        [Description("Use AspNet Development Server as the web server. See WebAii initialization.")]
        [DeploymentItem(@"Pages\UsingAspNetDevSrv.htm")]
        public void UseAspNetDevServer()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(TESTPAGE);
            Assert.IsTrue(ActiveBrowser.ViewSourceString.Contains("This is a test!"));

            // Note:
            //
            // We only need to include the page name here since we already set
            // the location as part of the Manager settings initialization.
            //
            // If you need to switch between AspNet Dev Server and IIS, you might
            // want to initialize a BaseUri that you append when running under IIS.
        }

        #endregion
    }
}
