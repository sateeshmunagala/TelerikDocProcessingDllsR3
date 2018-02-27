using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.BrowserSpecialized.AspNetHost;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// Using the built-in AspNetInProc Host to perform ASP.NET Testing without the
    /// need for any external browsers or processes.
    /// </summary>
    [TestClass]
    public class AspNetInProcHost : BaseTest
    {
        private const string TESTPAGE = "~/AspNetInProcHost.aspx";

        #region Notes
        //
        // 1. In the WebAii Initialization section, we override the settings.DefaultBrowser to
        //    be BrowserType.AspNetHost and set the WebAppPhysicalPath to the location of 'Pages/AspNetApp' folder.
        //
        // 2. The AspNetApplication classes of the AspNetInProc host is based mostly on the Plasma project on
        //    http://www.codeplex.com.
        //
        // 3. AspNetHost is great for performing basic unit testing of ASP.NET applications where you want a light
        //    execution environment and extremely fast execution performance with no dependency on any browser.
        //    The browser those does not work for scenarios where heavy client side scripting is utilized since
        //    the host does not run with any javascript execution engine.
        //
        // 4. Given that all tests run in proc with no UI, sometimes it is difficult to debug the scenarios.
        //    WebAii provides a way to for you to see how the pages are being rendered using EnableUILessRequestViewing setting.
        //    Setting this to 'true' will launch InternetExplorer and push the markup on each request/action
        //    to the browser to be visually rendered so you can see how the pages are being processed.
        //
        // 5. The AspNetHost implements its own custom browser. You can cast the ActiveBrowser object to AspNetHostBrowser
        //    object to access all internal AspNetHost objects. (i.e. AspNetApplication, AspNetRequest ...etc)
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
            settings.Web.DefaultBrowser = BrowserType.AspNetHost;
            settings.Web.WebAppPhysicalPath = Path.GetFullPath(Path.Combine(TestContext.TestDeploymentDir, @"AspNetApp"));
            settings.LogLocation = TestContext.TestLogsDir;

            // Uncomment this line to view the test running with IE being the viewer.
            //settings.EnableUILessRequestViewing = true;

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
        [Description("Demonstrate performing automation actions against Asp.Net in Proc Host.")]
        [DeploymentItem(@"Pages\AspNetApp", "AspNetApp")]
        public void AspNetInProcTesting()
        {
            // Will initialize a new AspNetApplication object.
            Manager.LaunchNewBrowser();

            // Request the page we want.
            ActiveBrowser.NavigateTo(TESTPAGE);

            // We can access raw http request properties when the
            // browser is AspNetHost.
            if (ActiveBrowser.BrowserType == BrowserType.AspNetHost)
            {
                // we wrap this part with an if..else statement so that
                // this test can still run on other browsers without
                // having to perform any recompilation.
                AspNetHostBrowser hostBrowser = (AspNetHostBrowser)ActiveBrowser;

                // get the last status code for the last response.
                // You can also access the full Request/Response objects using
                // hostBrowser.AspNetAppInstance.LastResponse
                // or
                // hostBrowser.AspNetAppInstance.LastRequest

                Assert.IsTrue(hostBrowser.Status == 200);
            }
            else
            {
                // Do some other type of verification...
            }

            Element label = ActiveBrowser.Find.ById("label1");

            // Click a button
            Actions.Click(Find.ById("button1"));
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains("Button1 Clicked"));

            // Click a linkbutton
            Actions.Click(Find.ById("linkbutton1"));
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains("LinkButton1 Clicked"));

            // Set Text
            Actions.SetText(Find.ById("textbox1"), "Hello!");
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains("Hello!"));

            //Select From DropDown
            Actions.SelectDropDown(Find.ById("dropdownlist1"), "Item2");
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains("Item2"));

            // Select a link on the calendar.
            string dateToSelect = DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Today.Month) + " " +
                string.Format("{0:00}", DateTime.Today.Day);

            Actions.Click(Find.ByAttributes("title=~" + dateToSelect));
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains(DateTime.Today.ToShortDateString()));

            // Select a node in a treeview
            Actions.Click(Find.ById("treeView1t1"));
            label.Refresh();
            Assert.IsTrue(label.InnerText.Contains("Node2"));

        }

        #endregion
    }
}
