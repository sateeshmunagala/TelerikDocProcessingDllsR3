using System.IO;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.Common;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.Win32;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the built in browser annotator to visually follow your test execution.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\BrowserActions.htm")]
    public class VisualAnnotatedExecution : BaseTest
    {
        private const string TESTPAGE = @"BrowserActions.htm";

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
        [Description("How to query for an existing cookie on the machine.")]
        public void EnableAnnotation()
        {
            // Launch an instance of the browser
            Manager.LaunchNewBrowser();

            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // [You can set these globally in the config file too]

            // Enable Annotated Execution.
            Manager.Settings.AnnotateExecution = true;
            // Slow down test execution so we can visually track the actions on the page.
            Manager.Settings.ExecutionDelay = 500; // half a second wait between actions.

            // Perform the actions
            DoSomeAction(Actions);

            // Customize the Annotation Visually and perform the actions again.
            ActiveBrowser.Annotator.Settings.BackColor = Color.Gray;
            ActiveBrowser.Annotator.Settings.FontemSize = 15;
            ActiveBrowser.Annotator.Settings.BackColorAlpha = 200;
            ActiveBrowser.Annotator.Settings.Color = Color.HotPink;
            ActiveBrowser.Annotator.Settings.FontStyle = FontStyle.Italic;
            ActiveBrowser.Annotator.Settings.BorderWidth = 3;

            DoSomeAction(Actions);

            // Disable Annotated Execution.
            Manager.Settings.AnnotateExecution = false;

            DoSomeAction(Actions);
        }

        [TestMethod]
        [Description("Perform manual annotation programatically.")]
        public void ManualAnnotation()
        {
            // [You can set these globally in the config file too]

            // Enable Annotated Execution.
            Manager.Settings.AnnotateExecution = true;
            // Slow down test execution so we can visually track the actions on the page.
            Manager.Settings.ExecutionDelay = 1000; // half a second wait between actions.

            // Launch an instance of the browser
            Manager.LaunchNewBrowser();

            // Navigate to the test page
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            Annotator myAnnotator = new Annotator(ActiveBrowser);
            myAnnotator.Annotate("This annotation message appears at the top of the browser document window");
            myAnnotator.Annotate("This annotation message appears at the top left corner of the browser document window",
                350, OffsetReference.TopLeftCorner);
            myAnnotator.Annotate(new Point(100, 300), "This annotation message appears under the point at 100,300 in the browser document window");
            myAnnotator.Annotate(Find.ById("btn1").GetRectangle(), "This annotation highlights the Color On button");
        }

        #endregion

        private void DoSomeAction(Actions actions)
        {
            // Check the checkbox
            actions.Check(Find.ById("check1"), true);

            // Uncheck the radio button
            actions.Check(Find.ById("radio1"), false);

            // Select using Value
            actions.SelectDropDown(Find.ById("selection1"), 2);

            // Now set the text for the text box
            actions.SetText(Find.ById("textbox1"), "This is a TEST for TextBox!");

            // Now set the text for the text area.
            actions.SetText(Find.ById("textarea1"), "This is a TEST for TextArea!");
        }
    }
}
