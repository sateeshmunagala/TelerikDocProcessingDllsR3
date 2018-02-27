using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.Common;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.Win32;
using ArtOfTest.WebAii.Win32.Dialogs;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to handle and extend Win32 dialogs that might pop-up during website testing.
    /// </summary>
    [TestClass]
    [DeploymentItem(@"Pages\Win32Dialogs.htm")]
    // We are going to use this element through out this class so we define it here to be shared.
    [Find("uploadfile", "TagIndex=input:0")]
    public class Win32Dialogs : BaseTest
    {
        private const string TESTPAGE = "Win32Dialogs.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.
        // 2. Note, the built in dialog support in WebAii for Alerts, FileUpload and Logon dialogs in meant
        //    to be used with the native dialogs that come with FireFox and IE only.
        //

        #endregion

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>Win32 Dialogs</title>
        //        <script type="text/javascript">
        //        function InvokeAlert()
        //        {
        //            alert("Hi there");
        //        }
        //        </script>
        //    </head>
        //    <body>
        //        <input type="file" id="InputFile" />	
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

            Manager.Settings.UnexpectedDialogAction = UnexpectedDialogAction.DoNotHandle;
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

        #region ALERT DIALOGS
        //
        [TestMethod]
        [Description("Handling alert dialogs")]
        public void AlertDialogs()
        {
            // Add an alert dialog to monitor
            Manager.DialogMonitor.AddDialog(AlertDialog.CreateAlertDialog(ActiveBrowser, DialogButton.OK));

            // Given that there were not dialog attribute set, the manager will not start the monitoring.
            // You need to invoke the monitoring
            Manager.DialogMonitor.Start();

            // Invoke an alert dialog
            Actions.InvokeScript("InvokeAlert()");

            // The dialog will now be automatically handled transparently in the background without any help from the test code.
        }
        //
        // OR
        //
        [TestMethod]
        [Dialog(DialogButton.OK)] // handle any alert dialogs in this test method
        public void AlertDialogsUsingAttributes()
        {
            // Invoke an alert dialog
            Actions.InvokeScript("InvokeAlert()");
        }

        #endregion

        #region DOWNLOAD DIALOGS

        [TestMethod]
        [Description("Handling Download Dialogs")]
        [DeploymentItem(@"SupportFiles\json2.js")]
        public void DownloadDialogs()
        {
            // Note:
            // Given that the download is usually a sequence of dialogs,
            // WebAii wraps the download sequence using the DownloadDialogHandler.
            // The DownloadDialogHandler supports Cancel or Save operations to a location on disk.
            // If you want more control over each dialog, the dialogs are available
            // under the Win32.Dialogs namespace
            //
            // Also the DialogHandler managers its own instance of the DialogMonitor so it won't affect your own
            // Manager.DialogMonitor instance of your class.

            //
            // Option I - Using the generic handler.
            //
            Element e = Find.ByTagIndex("a", 0);

            string saveLocation = Path.Combine(Path.GetTempPath(), "json2.txt");
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }

            DownloadDialogsHandler handler = new DownloadDialogsHandler(ActiveBrowser, DialogButton.SAVE, saveLocation, Desktop);
            
            Actions.Click(e);
            handler.WaitUntilHandled(30000);
            //
            // Option II - Using the built in support in HtmlControls
            //

            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }

            Find.ByTagIndex<HtmlAnchor>("a", 0).Download(false, DownloadOption.Save, saveLocation, 20000);
        }

        #endregion

        #region FILEUPLOAD DIALOGS

        [TestMethod]
        [DeploymentItem(@"SupportFiles\EmptyTextFile.txt")]
        public void FileUpLoadDialog()
        {
            // Add a FileUpload dialog to be monitored.
            Manager.DialogMonitor.AddDialog(new FileUploadDialog(ActiveBrowser,
                Path.Combine(TestContext.TestDeploymentDir, @"EmptyTextFile.txt"), DialogButton.OPEN));

            // Given that there were no dialog attribute set, the manager will not start the monitoring.
            // You need to invoke the monitoring
            Manager.DialogMonitor.Start();

            // Cause the upload Dialog to pop-up

            // With Firefox, it is not allowed to pop the dialog using script due to security restrictions.
            if (ActiveBrowser.BrowserType == BrowserType.FireFox)
            {
                // invoke the dialog using a direct mouse click (Click 10 pixels to the left of the right edge of the control)
                Desktop.Mouse.Click(MouseClickType.LeftClick, this.Elements.GetHtml("uploadfile").GetRectangle(),
                    new Point(-10, 0), OffsetReference.RightCenter);

                // Given that we are using pure UI outside the browser, let's make sure we wait for the browser to
                // be ready
                ActiveBrowser.WaitUntilReady();
            }
            else
            {
                // just click it
                Actions.Click(this.Elements.GetHtml("uploadfile"));
            }
        }

        //
        // OR
        [TestMethod]
        [DeploymentItem(@"SupportFiles\EmptyTextFile.txt")]
        [Dialog("EmptyTextFile.txt", DialogButton.OPEN)]
        public void FileUpLoadDialogUsingAttributes()
        {
            // Cause the upload Dialog to pop-up
            // FileUploadDialog dialog = new FileUploadDialog(ActiveBrowser, "custom file path", DialogButton.OPEN);
            FileUploadDialog dialog = new FileUploadDialog(ActiveBrowser, string.Empty, DialogButton.CANCEL);
            Manager.DialogMonitor.AddDialog(dialog);
            Manager.DialogMonitor.Start();


            // With Firefox, it is not allowed to pop the dialog using script due to security restrictions.
            if (ActiveBrowser.BrowserType == BrowserType.FireFox)
            {
                // invoke the dialog using a direct mouse click (Click 10 pixels to the left of the right edge of the control)
                Desktop.Mouse.Click(MouseClickType.LeftClick, this.Elements.GetHtml("uploadfile").GetRectangle(),
                    new Point(-10, 0), OffsetReference.RightCenter);

                // Given that we are using pure UI outside the browser, let's make sure we wait for the browser to
                // be ready
                ActiveBrowser.WaitUntilReady();
            }
            else
            {
                // just click it
                Actions.Click(this.Elements.GetHtml("uploadfile"));
            }

            dialog.WaitUntilHandled();
        }
        #endregion

        #region LOGON DIALOGS

        [TestMethod]
        public void LogonDialogTest()
        {
            // TODO: Remove this line once you have updated this test with Url, UserName and Password
            throw new NotImplementedException(
                "LogonDialog() test method needs to be updated with a Url/UserName & Password before you can execute it!");

            // Add a logon dialog support with username/password
            Manager.DialogMonitor.AddDialog(LogonDialog.CreateLogonDialog(ActiveBrowser, "<username>", "<password>", DialogButton.OK));
            Manager.DialogMonitor.Start();

            // Navigate to a page that need a logon
            ActiveBrowser.NavigateTo("<Place a Url to LogOn to here>");
        }
        //
        // OR
        //
        [TestMethod]
        [Dialog("<username>", "<password>", DialogButton.OK)]
        public void LogonDialogWithAttributes()
        {
            // TODO: Remove this line once you have updated this test with Url, UserName and Password
            throw new NotImplementedException(
                "LogonDialog() test method needs to be updated with a Url/UserName & Password before you can execute it!");

            // Navigate to a page that needs a logon
            ActiveBrowser.NavigateTo("<Place a Url to LogOn to here>");
        }
        //
        #endregion

        #region Custom Dialog Handling

        [TestMethod]
        public void DoCustomDialogHandlingForBuiltInDialogs()
        {
            AlertDialog myAlertDialog = AlertDialog.CreateAlertDialog(ActiveBrowser, DialogButton.OK);
            myAlertDialog.HandlerDelegate = new DialogHandlerDelegate(MyCustomAlertHandler);

            // Add dialog to monitor and start monitoring
            Manager.DialogMonitor.AddDialog(myAlertDialog); Manager.DialogMonitor.Start();

            Actions.InvokeScript("InvokeAlert()");

        }

        /// <summary>
        /// Custom dialog handler delegate
        /// </summary>
        /// <param name="dialog">The dialog to handle</param>
        public void MyCustomAlertHandler(IDialog dialog)
        {
            // Simply close the dialog
            dialog.Window.Close();

            try
            {
                // Wait until it is closed
                dialog.Window.WaitForVisibility(false, 50);
                Log.WriteLine("Alert Handled!");
                return;
            }
            catch
            {
                Log.WriteLine("Failed to handle alert!");
                // return false if the dialog did not close as expected
                return;
            }
        }

        #endregion

        #endregion

        #region Sample Custom Dialog Handler Implementation

        /// <summary>
        /// Implements IE's Security Dialog handling
        /// </summary>
        public class SecurityAlertDialog : BaseDialog
        {

            #region Private Constants

            /// <summary>
            /// The title of the dialog we want handled.
            /// </summary>
            private const string SECURITY_ALERT_TITLE = "Security Alert";

            #endregion

            #region Constructor


            /// <summary>
            /// Create the dialog passing it the parent browser and the button to use
            /// to dismiss the instance of this dialog.
            /// </summary>
            /// <param name="parentBrowser">The parent browser.</param>
            /// <param name="dismissButton">The button to use to dismiss the dialog.</param>
            public SecurityAlertDialog(Browser parentBrowser, DialogButton dismissButton)
                : base(parentBrowser, dismissButton)
            {

                if (dismissButton != DialogButton.YES && dismissButton != DialogButton.NO
                    && dismissButton != DialogButton.CLOSE && dismissButton != DialogButton.OK)
                {

                    throw new ArgumentException("Security dialog can only have dismiss button of types : YES, NO or CLOSE.");

                }

            }


            #endregion

            #region Base Dialog Implementation

            /// <summary>
            /// Check whether the dialog is present or not. This function is
            /// called by the dialogmonitor object
            /// </summary>

            /// <param name="dialogs">This is a list of dialog passes in
            /// the by the DialogMonitor object.</param>
            /// <returns>True/False whether this dialog is present.</returns>
            public override bool IsDialogActive(ArtOfTest.Common.Win32.WindowCollection dialogs)
            {
                // TODO: if the dialog exists in both firefox & IE and you want this dialog
                // handler to handle it in both browsers, you need to first check the
                // this.ParentBrowser.BrowserType property and depending on the type
                // of the browser, check the correct title of the dialog
                return this.IsDialogActiveByTitle(dialogs, SECURITY_ALERT_TITLE);
            }

            /// <summary>
            /// This is called by the DialogMonitor whenever IsDialogActive returns true.
            /// </summary>
            public override void Handle()
            {
                // If you are sharing this implementation with other
                // developers, this allows them to override this method
                // by setting the handler delegate. So if the
                // delegate is not null, perform the built in handling logic
                // otherwise call the custom handling logic.
                if (this.HandlerDelegate != null)
                {
                    this.HandlerDelegate(this);
                }
                else
                {

                    try
                    {

                        Button yesButton = new Button(this.Window.Handle, "&Yes", false);
                        Button noButton = new Button(this.Window.Handle, "&No", false);
                        Button okButton = new Button(this.Window.Handle, "OK", false);

                        switch (this.DismissButton)
                        {
                            case DialogButton.CLOSE:
                                this.Window.Close();
                                break;

                            case DialogButton.OK:
                                okButton.Click();
                                break;

                            case DialogButton.YES:
                                yesButton.Click();
                                break;

                            case DialogButton.NO:
                                noButton.Click();
                                break;
                        }

                        return;
                    }
                    catch
                    {

                        // Do any custom handling and return error.
                        return;
                    }

                }
            }


            #endregion

        }

        #endregion
    }
}
