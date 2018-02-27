using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the CookiesManager to Set/Delete and Query cookies.
    /// </summary>
    [TestClass]
    public class CookiesSupport : BaseTest
    {
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
        [Description("How to set and query for a cookie on the machine.")]
        public void SetGetDeleteCookies()
        {
            Manager.LaunchNewBrowser();

            // Let's create a new cookie for a url.
            System.Net.Cookie c = new System.Net.Cookie("WebAii", "Rocks", "/", "http://www.webaii.com");
            c.Expires = DateTime.Now.AddHours(1);
            ActiveBrowser.Cookies.SetCookie(c);

            // Query the cookie
            System.Net.CookieCollection siteCookies = ActiveBrowser.Cookies.GetCookies("http://www.webaii.com");

            Assert.AreEqual(1, siteCookies.Count);
            Log.WriteLine(siteCookies[0].Name + ":" + siteCookies[0].Value + ":" + siteCookies[0].Domain);

            siteCookies[0].Domain = @"http://www.webaii.com";
            // Now delete the cookie.
            ActiveBrowser.Cookies.DeleteCookie(siteCookies[0]);

            Assert.AreEqual(0, ActiveBrowser.Cookies.GetCookies("http://www.webaii.com").Count);
        }

        #endregion
    }
}
