using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the Browser object to get information and manupilate the browser window.		
    /// </summary>
    [TestClass]
    public class BrowserObject : BaseTest
    {
        private const string TESTPAGE = "Browser.htm";

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>Browser</title>
        //        <script type="text/javascript" >
        //        function MouseOverEvent()
        //        {
        //            label1.innerHTML = "LABEL1 IS FULL";
        //        }
        //        </script>
        //    </head>
        //    <body>
        //        <h1 id="label1" onmouseover="MouseOverEvent()">LABEL1 IS EMPTY</h1>
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
        [Description("Managing multiple browser instances using one test.")]
        [DeploymentItem(@"Pages\Browser.htm")]
        public void MultiBrowserInstanceSupport()
        {
            // NOTE: this.ActiveBrowser is Manager.ActiveBrowser and is always
            // set to the last browser instance launched using LaunchNewBrowser()

            // Launches 2 instances of a browser of type=Settings.BrowserType
            Manager.LaunchNewBrowser();
            Manager.LaunchNewBrowser();

            // Explicitly launch another instance different than the default one.
            if (Manager.Settings.Web.DefaultBrowser == BrowserType.FireFox)
                Manager.LaunchNewBrowser(BrowserType.InternetExplorer, true);
            else
                Manager.LaunchNewBrowser(BrowserType.FireFox, true);

            // All these launched instances are accessible through
            // Manager.Browsers[] array. The index of the Browser
            // in the array is inline with the order they were launched in.
            Assert.AreEqual(3, Manager.Browsers.Count);

            Manager.Browsers[0].NavigateTo("http://www.google.com");
            Manager.Browsers[1].NavigateTo("http://www.live.com");
            Manager.Browsers[2].NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Browsers[2] is the same as ActiveBrowser. Last launched instance.
            Assert.IsTrue(Manager.Browsers[2].ClientId.Equals(ActiveBrowser.ClientId));
        }

        [TestMethod]
        [Description("Controlling a browser instance.")]
        [DeploymentItem(@"Pages\Browser.htm")]
        public void ControllingBrowserInstance()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));
            Assert.IsTrue(ActiveBrowser.Url.Contains("Browser.htm"));

            // Now navigate to www.google.com
            ActiveBrowser.NavigateTo("http://www.bing.com");
            Assert.IsTrue(ActiveBrowser.Url.Contains("bing.com"));

            // Now go back.
            ActiveBrowser.GoBack();
            Assert.IsTrue(ActiveBrowser.Url.Contains("Browser.htm"));

            // Now go forward
            ActiveBrowser.GoForward();
            Assert.IsTrue(ActiveBrowser.Url.Contains("bing.com"));

            // Do a Refresh()
            ActiveBrowser.Refresh();
            Assert.IsTrue(ActiveBrowser.Url.Contains("bing.com"));
        }

        [TestMethod]
        [Description("Common properties exposed off the browser object")]
        [DeploymentItem(@"Pages\Browser.htm")]
        public void CommonBrowserProperties()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // The source string of this page. It is the raw html of the page that was
            // used to build the ActiveBrowser.DomTree
            Assert.IsTrue(ActiveBrowser.ViewSourceString.Contains("Browser"));

            // Get the version of the browser running.
            Log.WriteLine(string.Format("ActiveBrowser is:{0}, ver:{1}",
                ActiveBrowser.BrowserType.ToString(), ActiveBrowser.Version));

            // Get browser location.
            // The Browser.Window is the native Win32 Window that represents
            // the browser. You can use it to get window location and perform
            // basic window actions like SetFocus()/Minimize/Maximize
            Log.WriteLine(ActiveBrowser.Window.Rectangle.ToString());
        }

        [TestMethod]
        [Description("Common methods exposed off the browser object")]
        [Find("label", "TagIndex=h1:0")]
        [DeploymentItem(@"Pages\Browser.htm")]
        public void CommonBrowserMethods()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // NOTE:
            //
            // The default settings for Browser.AutoDomRefresh &
            // Browser.AutoWaitUntilReady is True. This means that the Browser object
            // will wait for the browser to be ready after each action (AutoWaitUntilReady)
            // and will ensure that the Browser.DomTree is up to date after
            // each action on the page (AutoDomRefresh) by calling Browser.RefreshDomTree().
            // The only exception to this are action performed using the Desktop object against
            // the browser. In this case, if these actions change the page DOM then you need
            // explicitly call Browser.WaitUntilReady() and Browser.RefreshDomTree() if needed too.

            // Get the element before any changes are made to the DOM.
            Element h1 = this.Elements.GetHtml("label");
            Assert.IsTrue(h1.InnerText.Contains("EMPTY"));

            // Make sure the browser window has focus
            ActiveBrowser.Window.SetFocus();

            // Invoke a hover over action using pure mouse move.
            Desktop.Mouse.HoverOver(h1.GetRectangle());

            // Wait for the change to be complete.
            ActiveBrowser.WaitUntilReady();

            // Refresh the DomTree with the latest changes.
            ActiveBrowser.RefreshDomTree();

            // Update this Element object with the latest DOM changes.
            h1.Refresh();
            Assert.IsTrue(h1.InnerText.Contains("FULL"));

            // Close this browser window.
            ActiveBrowser.Close();
        }

        [TestMethod]
        [Description("How to access the native IE IWebBrowser2 object from Browser")]
        public void AccessingIENativeObject()
        {
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer, true);

            object nativeIE = ActiveBrowser.NativeInstance;

            // You can then cast this instance to IWebBrowser2
            // This can be used for integration with other tools and processes
            // that use that object for automation.

            // This object will return Null for FireFox
            Assert.IsNotNull(nativeIE);
        }

        [TestMethod]
        public void ManageDifferentBrowserTypes()
        {
            // Launch a new instance of IE
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer, true);

            // Set this instance to active browser
            Browser ie = Manager.ActiveBrowser;

            // Launch a new instance of Firefox
            Manager.LaunchNewBrowser(BrowserType.FireFox, true);

            // set this instance to the active browser.
            // Note how ActiveBrowser is always set to the last launched instance
            Browser ff = Manager.ActiveBrowser;

            // Launch another instance of fireofx
            Manager.LaunchNewBrowser(BrowserType.FireFox, true);

            // set this instance. Note how we can also use the Browser[] collection instead of
            // using the ActiveBrowser.  // in this
            Browser ff2 = Manager.Browsers[Manager.Browsers.Count - 1];

            // you can then uses these instances just like you would use the 'ActiveBrowser' instance.
            ie.NavigateTo("http://www.google.com");
            ff.NavigateTo("http://www.google.com");
            ie.NavigateTo("http://www.live.com");
            ff2.NavigateTo("http://www.kayak.com");

            // You can also use the Window property off each browser to manipulate the
            // actual browser window
            ie.Window.SetFocus();

            // close the first firefox instance
            // Note: Although the Window class off each browser object has a .close() method,
            // it is recommended to use the Browser.Close() method when closing browser instances
            // since it will also perform the disconnect and clean-up from the manager properly.
            // You can also choose to set a timeout as shown below to double check that the
            // browser has actually closed and its handle is no longer visible.
            ff.Close(40);
        }

        #endregion
    }
}
