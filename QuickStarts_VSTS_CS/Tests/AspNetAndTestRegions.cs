using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// TestRegions are a great fit to be used with dynamic languages like ASP.NET.
    /// We'll explore using them in some common development scenarios to easily produce
    /// robust test automation that is 1) highly abstracted and targetted at the
    /// portion of the application being tests 2) has low maintenance costs. Enjoy!
    /// </summary>
    [TestClass]
    public class AspNetAndTestRegions : BaseTest
    {
        private const string PAGE_NAME_1 = "~/Default.aspx";
        private const string PAGE_NAME_2 = "~/DataPage.aspx";

        /// <summary>
        /// Declare the regions defined in our app here so that if we change a name
        /// or a region later on in the source code, we simply need to change it here
        /// instead of going through all tests.
        ///
        /// Note: Tests contained in this class do not use all these regions. We defined
        /// them to illustrate one way of how you might want to lay out your test regions
        /// within an application. Please refer to AppMaster.master,DataPage.aspx & Default.aspx
        /// to see how these regions are defined.
        /// </summary>
        public static class DefinedRegions
        {
            public const string WebApp = "WebApp";
            public const string LoginContent = "LoginContent";
            public const string DataPageContent = "DataPageContent";
            public const string CalendarControl = "CalendarControl";
            public const string GridControl = "GridControl";
            public const string GridEditId = "GridEditId";
            public const string GridEditBalance = "GridEditBalance";
        }

        #region Notes
        //
        // 1. In the WebAii Initialization section, we overrided the UseAspNetDevSrv to be true
        //    and set the WebAppPhysicalPath to the location of 'Pages/AspNetApp' folder.
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
            settings.Web.WebAppPhysicalPath = Path.GetFullPath(Path.Combine(TestContext.TestDeploymentDir, @"AspNetApp"));
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
        [Description("Logging in to the data page")]
        [DeploymentItem(@"Pages\AspNetApp", "AspNetApp")]
        [Find("UserName", "id=~username", TestRegionId = DefinedRegions.LoginContent)]
        [Find("Password", "id=~password", TestRegionId = DefinedRegions.LoginContent)]
        [Find("LogInBtn", "id=~LoginButton", TestRegionId = DefinedRegions.LoginContent)]
        public void LogInTest()
        {

            Manager.LaunchNewBrowser();
            // Navigate to the LoginPage
            ActiveBrowser.NavigateTo(PAGE_NAME_1);

            // Login content region
            TestRegion loginRegion = ActiveBrowser.Regions[DefinedRegions.LoginContent];

            Actions.SetText(Manager.Elements.GetHtml("UserName"), "artoftest");
            Actions.SetText(Manager.Elements.GetHtml("Password"), "rocks_01");
            Actions.Click(Manager.Elements.GetHtml("LogInBtn"));

            // Verify we logged in are redirected.
            Assert.IsTrue(ActiveBrowser.Url.Contains("DataPage.aspx"));

            // OBSERVE:
            //
            // 1. The TestRegion LoginContent, can be moves and located on a differnent page and this test
            //    will continue to run.
            // 2. If there was another element on the page that contains an id that has a partial value=username
            //    this won't affect this test since the search is scoped to on the this region. This will avoid
            //    an undesired breaks in your automation for future updates.
            //

            // Note: In some website, you need to perform this login for every test in your unit testing suite.
            // In that case, move the FindExpression attribute to the class level and move this to be a routine instead
            // of testmethod so you can call it from your tests.
        }

        [TestMethod]
        [Description("Select a date from the calendar, edit the balance in the grid.")]
        [DeploymentItem(@"Pages\AspNetApp", "AspNetApp")]
        [Find("BalanceEditTextBox", "TagIndex=input:0", TestRegionId = DefinedRegions.GridEditBalance)]
        public void DataPageTest()
        {
            Manager.LaunchNewBrowser();

            // Navigate to the DataPage. In some websites,
            ActiveBrowser.NavigateTo(PAGE_NAME_2);

            // Pick a date we want selected. For this sample we will pick Today
            string dateToSelect = DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Today.Month) + " " +
                string.Format("{0:00}", DateTime.Today.Day);


            TestRegion calendarRegion = ActiveBrowser.Regions[DefinedRegions.CalendarControl];

            // The Asp.Net renders the day as a title on the href
            Element calendarCellToClick = calendarRegion.Find.ByAttributes("title=~" + dateToSelect);
            Actions.Click(calendarCellToClick);

            // Get the index selected.
            //
            // OBSERVE:
            //
            // 1. The GridEditId testregion is part of the EditTemplate for the ID column.
            //    This means that we can simply access that region since it will only render
            //    in the column of the selected row.
            // 2. Also given that the GridEditId contains a text value of the index, we can simply
            //    access it using the InnerText of the region Element object.
            //    Please refer to DataPage.aspx
            // 3. Such approach can save lots of time crafting the tests and can save lots of
            //    in maintaining these tests. No longer do you need to spend time counting TD/TR
            //    or dealing with the auto-generated ids of the Grid
            //
            string selectedIndex = ActiveBrowser.Regions[DefinedRegions.GridEditId].Element.InnerText;

            Assert.IsTrue(Convert.ToInt16(selectedIndex) == 0); // given that we selected today's date
            // and the table starts from today's date.


            // Let's update the balance of selected row.
            //
            // OBSERVE:
            //
            // 1. First we need to access the textbox inside that row. Given that we have a testregion
            //    in the EditTemplate of the balance column we can automatically access that cell.
            // 2. The testregion contains only the TextBox which renders as an input. This means
            //    can we can simply find the first input tag inside that region and hence why
            //    the FindExpression defined on the method is input:0.
            //

            // Access the element and set the text to 5500.
            Actions.SetText(Manager.Elements.GetHtml("BalanceEditTextBox"), "5500");

            // We done now with this sample.

        }

        #endregion
    }
}
