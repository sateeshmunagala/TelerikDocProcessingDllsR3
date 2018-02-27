using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the Desktop object to invoke pure UI actions against your web pages.
    /// </summary>
    [TestClass]
    public class DesktopActions : BaseTest
    {
        private const string TESTPAGE = @"DesktopActions.htm";

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //    <head>
        //        <title>DesktopActions</title>
        //        <script type="text/javascript">

        //        function MouseOverEvent(sender)
        //        {
        //            MessageLabel.innerHTML = "Mouse is over: " + sender.id;
        //            sender.style.backgroundColor = "red";
        //        }

        //        function TextChanged(sender)
        //        {
        //            MessageLabel.innerHTML = "Text changed to " + sender.value;
        //        }

        //        function MouseOut(sender)
        //        {
        //            sender.style.backgroundColor = "";
        //        }


        //        </script>
        //    </head>
        //    <body>
        //        <table width="100%">
        //            <tr>
        //                <td id="Cell00" onmousemove="MouseOverEvent(this)" onmouseout="MouseOut(this)">
        //                    CELL00
        //                </td>
        //                <td id="Cell01" onmousemove="MouseOverEvent(this)" onmouseout="MouseOut(this)">
        //                    CELL01
        //                </td>
        //            </tr>
        //            <tr>
        //                <td id="Cell10" onmousemove="MouseOverEvent(this)" onmouseout="MouseOut(this)">
        //                    CELL10
        //                </td>
        //                <td id="Cell11" onmousemove="MouseOverEvent(this)" onmouseout="MouseOut(this)">
        //                    CELL11
        //                </td>
        //            </tr>
        //        </table>
        //        <h1 id="MessageLabel">Empty</h1>
        //        <input id="TextBox" type="text" onchange="TextChanged(this)" />
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
        [Description("Use Win32 UI to enter text")]
        [DeploymentItem(@"Pages\DesktopActions.htm")]
        public void SimpleWin32UI()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            Element input1 = Find.ById("TextBox");

            /// First scroll the element into view
            /// Then click on it to set the input focus to the input
            /// Simulate typing on the keyboard which should enter text
            /// into our control
            ActiveBrowser.Actions.ScrollToVisible(input1);
            Manager.Desktop.Mouse.Click(MouseClickType.LeftClick, input1.GetRectangle());
            Manager.Desktop.KeyBoard.TypeText("Hello there", 100);
        }

        [TestMethod]
        [Description("Samples of how to used the Desktop object to interact with a web page")]
        [DeploymentItem(@"Pages\DesktopActions.htm")]
        [Find("Cell00", "id=Cell00")]
        [Find("Cell01", "id=Cell01")]
        [Find("Cell10", "id=Cell10")]
        [Find("Cell11", "id=Cell11")]
        [Find("TextBox", "id=TextBox")]
        [Find("MessageLabel", "id=MessageLabel")]
        public void PureDesktopUIActions()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            Desktop.Mouse.HoverOver(this.Elements.GetHtml("Cell00").GetRectangle());
            Desktop.Mouse.HoverOver(this.Elements.GetHtml("Cell01").GetRectangle());
            Desktop.Mouse.HoverOver(this.Elements.GetHtml("Cell10").GetRectangle());
            Desktop.Mouse.HoverOver(this.Elements.GetHtml("Cell11").GetRectangle());

            // Now click inside the textbox.
            Desktop.Mouse.Click(MouseClickType.LeftClick, this.Elements.GetHtml("TextBox").GetRectangle());
            // Type some text
            Desktop.KeyBoard.TypeText("'Hello from Telerik Testing Framework'", 5);
            Desktop.KeyBoard.KeyPress(System.Windows.Forms.Keys.Tab);    // this to invoke the onchange event

            ActiveBrowser.WaitUntilReady();
            ActiveBrowser.RefreshDomTree();

            // Given that the DOM has changed. You can either Re-Find all elements
            // by forcing a call to Find.All() or simply refreshing the element you need:
            this.Elements.GetHtml("MessageLabel").Refresh();

            Assert.IsTrue(this.Elements.GetHtml("MessageLabel").InnerText.Contains("Telerik Testing Framework"));
        }

        #endregion
    }
}
