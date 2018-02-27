using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to access elements within frames.
    /// </summary>
    [TestClass]
    public class Frames : BaseTest
    {
        private const string TESTPAGE = @"Frames\Frames.htm";

        #region Notes

        //
        // Some of these tests may run too fast for you to notice the elements change. Setup
        // break point and step through these tests to easily see the actions occurring on the page.
        //

        #endregion

        #region Page Under Test
        //<HTML>
        //<HEAD><TITLE>THE I HATE FRAMES PAGE</TITLE></HEAD>
        //<FRAMESET COLS="33%,33%">
        //    <FRAME SRC="t1.html" name="T1_Frame">
        //    <FRAME SRC="..\BrowserActions.htm" name="T2_Frame">
        //</FRAMESET>
        //</HTML>
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
        [Description("Performing actions inside a frame.")]
        [DeploymentItem(@"Pages\Frames\Frames.htm", "Frames")]
        [DeploymentItem(@"Pages\Frames\t1.html", "Frames")]
        [DeploymentItem(@"Pages\Frames\t2.html", "Frames")]
        [DeploymentItem(@"Pages\BrowserActions.htm")]
        public void FramesAction()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer, true);

            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Access a frame by name
            //
            // NOTE: The Frames collection contains a flat list of all
            // frames on the page regardless of their nesting.
            Browser t11_frame = ActiveBrowser.Frames["T11_Frame"];
            t11_frame.RefreshDomTree();

            // Find the elements on the page using the Find.ByXxx methods.
            Element toggleOn = t11_frame.Find.ById("btn1");
            Element toggleOff = t11_frame.Find.ById("btn2");

            // Now click the toggleOn button.
            t11_frame.Actions.Click(toggleOn);

            // Click the toggleoff button
            t11_frame.Actions.Click(toggleOff);
        }

        #endregion
    }
}
