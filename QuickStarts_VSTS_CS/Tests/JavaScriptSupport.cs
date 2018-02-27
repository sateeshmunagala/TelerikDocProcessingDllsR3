using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.TestAttributes;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to invoke javascript method, event and perform logging/tracing from javascript.
    /// </summary>
    [TestClass]
    // The <h1> label we will use through out this class for verification.
    [Find("MessageLabel", "TagIndex=h1:0")]
    public class JavaScriptSupport : BaseTest
    {
        private const string TESTPAGE = "JavaScriptSupport.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.
        // 2. In the WebAii Initialization section, we overrided the EnableScriptLogging to be true.
        //

        #endregion

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>JavaScript Support</title>
        //        <!-- V Enable Script Logging for WebAii on this Page V -->
        //        <script type="text/javascript" src="WebAii_Logging.js"></script>
        //        <!-- ^ Enable Script Logging for WebAii on this Page ^ -->

        //        <script type="text/javascript" >
        //        function Test1()
        //        {
        //            label.innerHTML = "Test1 Called";		
        //        }

        //        function Test2(message,number)
        //        {
        //           label.innerHTML = "Test2 Called: [Message:" + message + ", Number:" + number + "]";
        //        }

        //        function Test3()
        //        {
        //            label.innerHTML = "Test3 Called";
        //            return "Hi from JavaScript";
        //        }

        //        function Test4()
        //        {
        //            label.innerHTML = "Test4 Called: onmouseout Called";
        //        }

        //        function Test5()
        //        {
        //            label.innerHTML = "Test5 Called";
        //            WebAiiLog("Test","Test5: Log this message!");
        //        }
        //        </script>
        //    </head>
        //    <body>
        //        <h1 id="label" onmouseout="Test4()">Empty</h1>
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
            // Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.Web.EnableScriptLogging = true;
            settings.LogLocation = TestContext.TestLogsDir;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

            // Set the current test method. This is needed for WebAii to discover
            // its custom TestAttributes set on methods and classes.
            // This method should always exist in [TestInitialize()] method.
            SetTestMethod(this, (string)TestContext.Properties["TestName"]);

            #endregion

            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));
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
        [Description("How to invoke a javascript method")]
        [DeploymentItem(@"Pages\JavaScriptSupport.htm")]
        public void JsMethodCall()
        {
            // Example - 1:
            //
            // Call a javascript function.
            Actions.InvokeScript("Test1()");

            // Verify that Test1() was called
            Assert.IsTrue(this.Elements.GetHtml("MessageLabel").InnerText.Contains("Test1"));

            // Example - 2:
            //
            // Call a javascript function with parameters
            // NOTE: It is important to enclose literal string parameter with double quote and not single
            // quotes.
            Actions.InvokeScript(@"Test2(""WebAii"",4)");

            // Update the properties of this DOM Element.
            this.Elements.GetHtml("MessageLabel").Refresh(); // Update the object with its latest markup.
            // Verify
            Assert.IsTrue(this.Elements.GetHtml("MessageLabel").InnerText.Contains("Test2"));
        }

        [TestMethod]
        [Description("How to invoke a javascript method and return a value")]
        [DeploymentItem(@"Pages\JavaScriptSupport.htm")]
        public void JsMethodCallWithReturnValue()
        {
            string jsRetValue = Actions.InvokeScript("Test3();");
            Assert.IsTrue(jsRetValue.Equals("Hi from JavaScript"));

            // NOTE: WebAii currently supports string return values ONLY.
        }

        [TestMethod]
        [Description("How to invoke a javascript event on an element")]
        [DeploymentItem(@"Pages\JavaScriptSupport.htm")]
        public void JsEventInvocation()
        {
            // Invoke the event on the label.
            Actions.InvokeEvent(this.Elements.GetHtml("MessageLabel"), ScriptEventType.OnMouseOut);

            // Sync the DOM Element with the latest markup from the browser.
            this.Elements.GetHtml("MessageLabel").Refresh();

            // Verify
            Assert.IsTrue(this.Elements.GetHtml("MessageLabel").InnerText.Contains("onmouseout"));
        }

        [TestMethod]
        [Description("How to perform logging into WebAii log from javascript")]
        [DeploymentItem(@"Pages\JavaScriptSupport.htm")]
        [DeploymentItem(@"Pages\WebAii_Logging.js")]
        public void LoggingFromJs()
        {
            // Settings.Current.EnableScriptLogging must be set to true before
            // LaunchNewBrowser. This was done in the Initialize section of
            // this test.
            // Make sure the web page references the WebAii_Logging.js file.

            // Invoke a function that will perform logging from JavaScript.
            Actions.InvokeScript("Test5()");

            // Verify that we have a log from JavaScript.
            Assert.IsTrue(Log.Text.Contains("Test5"));

            // Disable logging. This won't have any effect until the browser
            // is closed and reopened.
            Settings.Current.Web.EnableScriptLogging = false;

            // NOTE: Once script logging is disabled, any logging calls from
            // the script will be ignored and won't cause a script error.
        }

    #endregion
    }
}
