using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.Javascript;
using ArtOfTest.WebAii.Messaging.Http;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// Summary description for WebAii2Samples
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\WebAii2", "WebAii2Pages")]
    public class WebAii2Samples : BaseTest
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

            // TODO Fix this so we set up the log directory correctly.
            Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            //Settings settings = GetSettings();

            // Override the settings you want. For example:
            //settings.BaseUrl = System.IO.Path.Combine(this.TestContext.TestDeploymentDir, "WebAii2Pages");

            // Now call Initialize again with your updated settings object
            //Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

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

        [TestMethod]
        [Description("Demonstrates chaining FindExpressions")]
        public void ChainedFindExpressions()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, "WebAii2Pages/Tables.html"));

            // centerTD is the ID of the center cell in the second table
            HtmlTableCell center = Find.ById<HtmlTableCell>("centerTD");
            Assert.AreEqual("2, 2, 2", center.TextContent);

            // Get the second cell of the second row of the second table with a chained FindExpression
            // table:1 -> tr:1 -> td:1 (indices start at 0)
            HtmlTableCell center2 = Find.ByExpression<HtmlTableCell>("TagIndex=table:1", "|", "TagIndex=tr:1", "|", "TagIndex=td:1");
            Assert.AreEqual(center.TextContent, center2.TextContent);
        }

        private void CheckTypeForImage(object sender, HttpResponseEventArgs e)
        {
            Log.WriteLine(String.Format("Request for {0}", e.Response.Request.RequestUri));
            if (e.Response.IsImage)
            {
                Log.WriteLine(String.Format("Image detected; MIME type: {0}", e.Response.Headers["Content-Type"]));
            }
            else
            {
                Log.WriteLine(String.Format("Not an image; MIME type: {0}", e.Response.Headers["Content-Type"]));
            }
        }

        [TestMethod]
        [Description("Detect images as they arrive from the web server")]
        public void ImageDetection()
        {
            //we have to set following property before initialize method
            Manager.Settings.Web.UseHttpProxy = true;

            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);         

            ResponseListenerInfo li = new ResponseListenerInfo(CheckTypeForImage);

            Manager.Http.AddBeforeResponseListener(li);
            ActiveBrowser.NavigateTo("http://news.google.com/");
            Manager.Http.RemoveBeforeResponseListener(li);

            // Check the test results for a log of all responses during the page load
        }

        [TestMethod]
        [Description("Invoke mouse events with a specific button")]
        public void MouseEvents()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, "WebAii2Pages/JavascriptEvents.html"));

            HtmlTextArea area1 = Find.ById<HtmlTextArea>("area1");
            Assert.IsNotNull(area1);

            // Invoke a click event and verify that the correct button was pressed
            MouseEvent me = new MouseEvent("mousedown");
            me.Button = MouseButton.Left;
            area1.InvokeEvent(me);
            Assert.AreEqual("left", area1.Text);

            me.Button = MouseButton.Right;
            area1.InvokeEvent(me);
            Assert.AreEqual("right", area1.Text);
        }

        [TestMethod]
        [Description("Receive notification of click events on an HTML control")]
        public void JavascriptEventNotifications()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, "WebAii2Pages/JavascriptEvents.html"));

            HtmlTextArea area2 = Find.ById<HtmlTextArea>("area2");
            Assert.IsNotNull(area2);

            // Invoke a click event, and wait for notification to be received
            _onClickARE.Reset();
            _onClickOccurred = false;
            area2.AddEventListener("click", area2_OnClick);
            area2.InvokeEvent(ScriptEventType.OnClick);
            _onClickARE.WaitOne(500);
            Assert.IsTrue(_onClickOccurred);
        }

        private System.Threading.AutoResetEvent _onClickARE = new System.Threading.AutoResetEvent(false);
        private volatile bool _onClickOccurred;
        private void area2_OnClick(object sender, JavascriptEventArgs e)
        {
            _onClickOccurred = true;
            _onClickARE.Set();
        }

        [TestMethod]
        [Description("Gets a JSON-encoded object as a return value from a Javascript function")]
        public void JavascriptReturnsJson()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, "WebAii2Pages/JavascriptFunctions.html"));

            // JsonObject offers access to weakly-typed Javascript objects
            JsonObject json = Actions.InvokeScript<JsonObject>("getKeyValuePair();"); // returns {one: 1, fifteen: 15}
            Assert.IsNotNull(json);
            Assert.IsNotNull(json["root"]); // All fields of the JSON object are accessed through the "root" key
            Assert.IsNotNull(json["root"]["one"]);
            Assert.AreEqual(1, (int)json["root"]["one"]);
            Assert.IsNotNull(json["root"]["fifteen"]);
            Assert.AreEqual(15, (int)json["root"]["fifteen"]);
        }

        [TestMethod]
        [Description("Gets a strongly-typed object as a return value from a Javascript function")]
        public void JavascriptReturnsObject()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, "WebAii2Pages/JavascriptFunctions.html"));

            MyObject o = Actions.InvokeScript<MyObject>("getKeyValuePair();");
            Assert.IsNotNull(o);
            Assert.AreEqual(1, o.One);
            Assert.AreEqual(15, o.Fifteen);
        }

        // Define an object that matches the one returned from the Javascript function.  Objects used this way must be adorned with the DataContract
        // attribute
        [DataContract]
        private class MyObject
        {
            [DataMember(Name = "one")]
            public int One { get; set; }
            [DataMember(Name = "fifteen")]
            public int Fifteen { get; set; }
        }
    }
}
