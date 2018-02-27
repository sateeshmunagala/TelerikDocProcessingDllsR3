using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.Common.Win32;
using ArtOfTest.WebAii.BrowserSpecialized.InternetExplorer;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// Other advanced topics (i.e. Extensibility events, Native Win32 Windows support)
    /// </summary>
    [TestClass]
    public class Advanced : BaseTest
    {
        private const string TESTPAGE = @"Advanced.htm";
        private const string TESTPAGE2 = @"modal.html";

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>Advanced</title>
        //    </head>
        //    <body>
        //        <h1 id="title1">Advanced Scenarios Sample</h1>
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
        [Description("WebAii events accessible for pluggability and extensibility of the framework")]
        [DeploymentItem(@"Pages\Advanced.htm")]
        public void ExtensibleWebAiiEvents()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            //
            // Events setup
            //
            // Logging events: Triggered each time a Log.WriteLine is fired.
            Log.LogWrite += new EventHandler<ArtOfTest.WebAii.EventsArgs.LogWriteEventArgs>(Log_LogWrite);

            // Browser events: BeforeCommandExecuted/AfterCommandExecuted/DOMRefreshed
            ActiveBrowser.BeforeCommandExecuted += new EventHandler<ArtOfTest.WebAii.EventsArgs.BrowserCommandEventArgs>(ActiveBrowser_BeforeCommandExecuted);
            ActiveBrowser.AfterCommandExecuted += new EventHandler<ArtOfTest.WebAii.EventsArgs.BrowserCommandEventArgs>(ActiveBrowser_AfterCommandExecuted);
            ActiveBrowser.DomRefreshed += new EventHandler(ActiveBrowser_DomRefreshed);

            // Find events: When a failed search returns null or an empty list for Find.Allxx
            Find.ReturnedNullOrEmpty += new EventHandler<ArtOfTest.WebAii.EventsArgs.ReturnedNullOrEmptyEventArgs>(Find_ReturnedNullOrEmpty);

            // Perform a command to simulate the browser events
            ActiveBrowser.Refresh();

            // Perform a find operation to simulate the find events
            Element e = Find.ById("invalidId");

            // Perform a log operation to simulate the logging events
            Log.WriteLine("Logging from Test");
        }

        #region Event Sinks

        void Find_ReturnedNullOrEmpty(object sender, ArtOfTest.WebAii.EventsArgs.ReturnedNullOrEmptyEventArgs e)
        {
            Log.WriteLine("FindReturnedNullOrEmptyEvent:" + e.FindErrorData.ToString());
        }

        void ActiveBrowser_AfterCommandExecuted(object sender, ArtOfTest.WebAii.EventsArgs.BrowserCommandEventArgs e)
        {
            Log.WriteLine("AfterCommandExecuted:" + e.Command.ToString());
        }

        void ActiveBrowser_DomRefreshed(object sender, EventArgs e)
        {
            Log.WriteLine("DomRefreshedEvent!");
        }

        void ActiveBrowser_BeforeCommandExecuted(object sender, ArtOfTest.WebAii.EventsArgs.BrowserCommandEventArgs e)
        {
            Log.WriteLine("BeforeCommandExecutedEvent: " + e.Command.ToString());
        }

        void Log_LogWrite(object sender, ArtOfTest.WebAii.EventsArgs.LogWriteEventArgs e)
        {
            // CAUTION:
            //
            // Make sure NOT TO call Log.WriteLine() in this sink. Otherwise, you will cause
            // a deadlock due to an infinite loop.

            // DO YOUR CUSTOM LOGGING to a different medium here.
            // e.Message should contain the logged message.
        }

        #endregion

        [TestMethod]
        [Description("WebAii's support for native Win32 Windows")]
        public void NativeWin32WindowSupport()
        {
            // NOTE:
            // Make sure you have at least one Visual Studio IDE open when running this
            // test.
            Window vsWindow = null;

            // Get all desktop Windows
            // The empty constructor will get all desktop windows because no
            // hwnd is specified.
            WindowManager winManager = new WindowManager();
            winManager.GetWindows();

            // WinManager.Items is now initialized with all top level windows of the desktop
            foreach (Window win in winManager.Items)
            {
                if (win.Caption.Contains("Microsoft Visual Studio"))
                    vsWindow = win;

                Log.WriteLine(string.Format("Window (hwnd:{0},caption:{1},class:{2})",
                    win.Handle.ToString(), win.Caption, win.ClassName));
            }

            // Find the vsWindow recursively. Passing IntPtr.Zero will start from the desktop.
            Window vsWindowRecur = WindowManager.FindWindowRecursively(
                IntPtr.Zero, "Microsoft Visual Studio", true, 0);

            Assert.IsNotNull(vsWindowRecur);
        }

        [TestMethod]
        [Description("How to access and handle IE's modal dialogs")]
        [DeploymentItem(@"Pages\IEModal\modal.html")]
        [DeploymentItem(@"Pages\IEModal\dialog.html")]
        public void IEModalDialogsSupport()
        {
            Manager.LaunchNewBrowser(BrowserType.InternetExplorer, true);
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE2));

            // Open the popup using mouse click so it doesn't hang execution.
            Find.ByAttributes<HtmlInputButton>("type=button").MouseClick();

            // ** Special IE Code. Given that IE Dialog is an IE specific feature.
            if (ActiveBrowser.BrowserType == BrowserType.InternetExplorer)
            {
                InternetExplorerActions ieActions = (InternetExplorerActions)ActiveBrowser.Actions;

                // Connect the dialog
                ieActions.ConnectIEDialog("THIS IS A MODAL DIALOG", 300);
                Manager.WaitForNewBrowserConnect("dialog.html", true, 10000);

                Assert.IsTrue(ActiveBrowser.IsIEDialog);

                // The ActiveBrowser instance is now the dialog instance. Do what ever you want with the dialog
                ActiveBrowser.Find.ByTagIndex<HtmlContainerControl>("H1", 0).BaseElement.SetValue<string>("style.backgroundColor", "red");


                // Once done, make sure to close the dialog.
                // Even if the dialog is closed due to a button click within the dialog, you still need this line
                // at this point to revert the ActiveBrowser instance to the main instance. We are searching for a
                // good approach to make this automatic.
                ActiveBrowser.Close();
            }

            Assert.IsFalse(ActiveBrowser.IsIEDialog);

            // The ActiveBrowser is back to the main browser window.
            ActiveBrowser.NavigateTo("http://www.google.com");
        }

        [TestMethod]
        [Description("Switching between test deployment servers using globally configured BaseUrl setting")]
        [DeploymentItem(@"Pages\BaseUrls\TestServer", "TestServer")]
        [DeploymentItem(@"Pages\BaseUrls\DeploymentServer", "DeploymentServer")]
        public void UsingBaseUrl()
        {
            string testServerPath = @"TestServer\";
            string deploymentServerPath = @"DeploymentServer\";

            // NOTE: In typical test environments this setting should be set in config. We are setting
            // it here in the code for the sake of illustrating this feature.

            Manager.LaunchNewBrowser();

            // Set the BaseUrl to be test server
            Manager.Settings.Web.BaseUrl = Path.Combine(TestContext.TestDeploymentDir, testServerPath);

            ActiveBrowser.NavigateTo("/default.html");
            Assert.IsTrue(ActiveBrowser.ViewSourceString.Contains("TestServer"));

            // Switch to the deployment server
            Manager.Settings.Web.BaseUrl = Path.Combine(TestContext.TestDeploymentDir, deploymentServerPath);

            ActiveBrowser.NavigateTo("/default.html");
            Assert.IsTrue(ActiveBrowser.ViewSourceString.Contains("deployment"));
        }

        #endregion
    }
}
