using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to handle and extend Win32 dialogs that might pop-up during website testing.
    /// </summary>
    [TestClass]
    public class VisualCapturing : BaseTest
    {
        private const string TESTPAGE = "VisualCapturing.htm";

        #region Page Under Test

        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>VisualCapturing</title>
        //    </head>
        //    <body>
        //        <h1>Visual Capturing</h1>
        //        <div style="background-color:Lime; width:100%; height:10%; text-align:center">
        //            <img src="Images/Logo.gif" alt="ArtOfTest, Inc. Logo" />
        //        </div>
        //        <div style="background-color:Red; width:100%; height:10%; text-align:center">
        //            <img src="Images/Logo.gif" alt="ArtOfTest, Inc. Logo" />
        //        </div>
        //        <div style="background-color:Blue; width:100%; height:10%; text-align:center">
        //            <img src="Images/Logo.gif" alt="ArtOfTest, Inc. Logo" />
        //        </div>
        //        <div style="background-color:Yellow; width:100%; height:10%; text-align:center">
        //            <img src="Images/Logo.gif" alt="ArtOfTest, Inc. Logo" />
        //        </div>
        //        <div style="background-color:Gray; width:100%; height:10%; text-align:center">
        //            <img src="Images/Logo.gif" alt="ArtOfTest, Inc. Logo" />
        //        </div>

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
        [Description("Capturing browser states")]
        [DeploymentItem(@"Pages\VisualCapturing.htm")]
        [DeploymentItem(@"Pages\Images\Logo.gif", "Images")]
        public void VisuallyCapturingBrowserStates()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Capture the browser state visually as a bitmap.

            // This will capture the bitmap as a bmp file and store it in the
            // log location with a link pointing to it added to the log.
            // The bitmap is also accessible using Log.CapturedBitmaps collections
            Log.CaptureBrowser(ActiveBrowser);

            //
            // OR
            //

            // Capture the bitmap to an in memory bitmap.
            // Note:
            //
            // The GetBitmap() method should work with any Window object regardless
            // whether it is the browser window or any other Window object.
            Bitmap myBrowser = ActiveBrowser.Window.GetBitmap();
            Assert.IsTrue(myBrowser.Size == myBrowser.Size);

        }

        [TestMethod]
        [Description("Capturing browser elements as bitmaps")]
        [DeploymentItem(@"Pages\VisualCapturing.htm")]
        [DeploymentItem(@"Pages\Images\Logo.gif", "Images")]
        public void VisuallyCapturingPageElements()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Let's capture the second div visually and store it to our log location.
            Element secondDiv = Find.ByTagIndex("div", 1);

            // Returns the image of the div.
            Bitmap bmp = ActiveBrowser.Window.GetBitmap(secondDiv.GetRectangle());

            string saveLocation = Path.Combine(Log.LogLocation, "secondDiv.bmp");
            bmp.Save(saveLocation);
            Log.WriteLine("Second Div bitmap saved to: " + saveLocation);
        }

        #endregion
    }
}
