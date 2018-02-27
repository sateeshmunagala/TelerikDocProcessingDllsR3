using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.Common;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.Silverlight;
using ArtOfTest.WebAii.Silverlight.UI;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// Summary description for WebAiiVSUnitTest1
    /// </summary>
    [TestClass]
    public class SilverlightHealthcareSample : BaseTest
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
            //Initialize(this.TestContext.TestLogsDir, new TestContextWriteLine(this.TestContext.WriteLine));

            // If you need to override any other settings coming from the
            // config section you can comment the 'Initialize' line above and instead
            // use the following:

            // This will get a new Settings object. If a configuration
            // section exists, then settings from that section will be
            // loaded

            Settings settings = GetSettings();

            // Override the settings you want. For example:
            settings.Web.EnableSilverlight = true;

            // Now call Initialize again with your updated settings object
            Initialize(settings, new TestContextWriteLine(this.TestContext.WriteLine));

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
        [Description("Automate a complex Silverlight application")]
        public void HealthCareDemo()
        {
            #region ## Launch the application ##

            Manager.Settings.ExecutionDelay = 10;

            Manager.LaunchNewBrowser(BrowserType.InternetExplorer);
            System.Drawing.Point loc = ActiveBrowser.Window.Location;
            ActiveBrowser.NavigateTo("http://pjd.mscui.net/PrimaryCareAdmin.htm");


            ActiveBrowser.Window.Move(new System.Drawing.Rectangle(loc, new System.Drawing.Size(1024, 768)), true);
            // Get an instance of the running Silverlight Application.
            SilverlightApp app = ActiveBrowser.SilverlightApps()[0];

            #endregion

            #region ## Guide Testing ##

            //
            // Verify Show/Hide of the Guide works
            //

            // Click the ShowGuide button
            app.FindName("guideButton").User.Click();


            // Right click the ShowGuide button
            app.FindName("guideButton").User.Click(MouseClickType.RightClick,
                new System.Drawing.Point(5, 5), OffsetReference.AbsoluteCenter);

            //
            // Wait for the guidance to be fully visible.
            //
            // In this application they use the Opacity of the
            // "GuidanceCanvas" to show/hide the guidance.
            Canvas guidanceCanvas = app.FindName<Canvas>("GuidanceCanvas");
            guidanceCanvas.Wait.For(canvas => canvas.Opacity == 1);

            guidanceCanvas.Refresh();

            // Make sure we got the correct # of guidance popups
            // The application pops 15 overlays on top
            IList<FrameworkElement> guidanceOverlays = guidanceCanvas.Find.AllByType("GuidanceOverlay");
            Assert.IsTrue(guidanceOverlays.Count == 15);

            // Now hide the guidance
            app.FindName("guideButton").User.Click();
            // Wait for the guidance to be hidden
            guidanceCanvas.Wait.For(canvas => canvas.Opacity == 0);

            #endregion

            #region ## Test Search ##

            //
            // Let's test the search functionality
            //

            app.VisualTree.Refresh();

            // Find the Patient Search TextBox
            TextBox searchText = app.FindName<TextBox>("searchText");

            // Move the mouse over the search box until we have an IBeam.
            searchText.User.DetectHotSpot(100, System.Windows.Forms.Cursors.IBeam);

            // Make sure the navigation bar is hidden before trying to type.
            // Currently it hides half the text box.

            // The Grid uses a TranslateTransform to hide/show the top bit
            app.FindName<Grid>("navBarGrid").Wait.For(r => (r.RenderTransform as TranslateTransform).Y == -50);

            // Now we can start typing.
            searchText.User.TypeText("A", 100);

            // Refresh the Visual Tree given the search changes.
            app.VisualTree.Refresh();

            string xml = app.VisualTree.Root.ToXml();

            ///
            /// Validate the search results.
            ///

            // Get the search lists.
            ItemsControl searchList = app.FindName<ItemsControl>("patientSearchList");

            // Get the number of results in the search.
            //
            // OBSERVER: WebAii can search custom types too not available
            // in ArtOfTest.WebAii.Silverlight.UI namespace. All the Find.xx methods have a non-generic
            // overload too that takes in a control name.
            //
            IList<FrameworkElement> foundPatients = searchList.Find.AllByType("patientsearchitem").Where(fx => fx.Visibility == Visibility.Visible).ToList();

            // Validate the search
            Assert.IsTrue(CompareUtils.NumberCompareRange(foundPatients.Count, 93, 105, NumberRangeCompareType.InRange),
                string.Format("Actual patiends found:{0}", foundPatients.Count));

            #endregion

            #region ## Test Zoom Data ##

            ///
            /// Verify the zoom in/zoom out functionality.
            ///
            FrameworkElement zoomBox = app.FindName("zoomBox");

            // Inside of it, find the plus part
            Button plusButton = zoomBox.Find.ByName<Button>("~PlusButton");

            // Let's grab one of the patients to verify the zoom functionality on.
            FrameworkElement patientSearchItem = foundPatients.First();

            // Level 0 - Address & ContactDetails are not visible
            Assert.IsTrue(patientSearchItem.Find.ByName("Address").Visibility == Visibility.Collapsed);
            Assert.IsTrue(patientSearchItem.Find.ByName("ContactDetailsIcons").Visibility == Visibility.Collapsed);
            Assert.IsTrue(patientSearchItem.Find.ByName("AdditionalInfo").Visibility == Visibility.Collapsed);

            // Zoom level #1
            plusButton.User.Click();

            // Level 1 - ContactDetails only are visible
            Assert.IsTrue(patientSearchItem.Find.ByName("Address").Visibility == Visibility.Visible);
            Assert.IsTrue(patientSearchItem.Find.ByName("ContactDetailsIcons").Visibility == Visibility.Visible);
            Assert.IsTrue(patientSearchItem.Find.ByName("AdditionalInfo").Visibility == Visibility.Collapsed);

            // Zoom level #2
            plusButton.User.Click();

            // Level 2 - Address only is visible
            Assert.IsTrue(patientSearchItem.Find.ByName("Address").Visibility == Visibility.Visible);
            Assert.IsTrue(patientSearchItem.Find.ByName("ContactDetailsIcons").Visibility == Visibility.Collapsed);
            Assert.IsTrue(patientSearchItem.Find.ByName("AdditionalInfo").Visibility == Visibility.Visible);

            #endregion

            #region ## Test Scrolling ##

            //
            // Scroll the search results
            //

            // Find the scroll viewer
            ScrollViewer searchScroll = app.FindName("patientSearchScroller").Find.ByType<ScrollViewer>();
            
            
            AutomationMethod scrollVert = new AutomationMethod("ScrollToVerticalOffset", typeof(void));
            searchScroll.InvokeMethod(scrollVert, 2000);

            // Assert scrolling position.
            Assert.IsTrue(searchScroll.VerticalOffset == 2000);

            //
            // Close search results
            //
            app.FindName("clearSearch").User.Click();

            #endregion

            #region ## Test UI Drag/Drop ##

            //
            // Re-arrange admins UI
            //
            Grid adminsPanels = app.FindName<Grid>("adminPanels");

            IList<FrameworkElement> admins = adminsPanels.Find.AllByName("~adminDockPanel");

            FrameworkElement admin1 = admins.Where(adm => adm.Name.Equals("adminDockPanel1")).First();
            FrameworkElement admin2 = admins.Where(adm => adm.Name.Equals("adminDockPanel2")).First();
            FrameworkElement admin3 = admins.Where(adm => adm.Name.Equals("adminDockPanel3")).First();

            // Get locations before the move
            System.Drawing.Rectangle admin1Loc = admin1.GetScreenRectangle();
            System.Drawing.Rectangle admin2Loc = admin2.GetScreenRectangle();
            System.Drawing.Rectangle admin3Loc = admin3.GetScreenRectangle();

            // Now perform the drag/drop
            admin1.Find.ByType("Thumb").User.DragTo(admin2.Find.ByType("Thumb"));
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin1.Name).GetScreenRectangle(), admin2Loc);
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin2.Name).GetScreenRectangle(), admin1Loc);
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin3.Name).GetScreenRectangle(), admin3Loc);

            admin2.Find.ByType("Thumb").User.DragTo(admin3.Find.ByType("Thumb"));
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin1.Name).GetScreenRectangle(), admin1Loc);
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin2.Name).GetScreenRectangle(), admin3Loc);
            Assert.AreEqual<System.Drawing.Rectangle>(adminsPanels.Find.ByName(admin3.Name).GetScreenRectangle(), admin2Loc);

            admin1.User.HoverOver();
            admin1.User.MouseEnter(OffsetReference.LeftCenter);
            admin1.User.MouseLeave(OffsetReference.RightCenter);

            ScrollViewer admin1Scroll = app.FindName("adminDockPanel1").Find.ByType<ScrollViewer>();
            Assert.AreEqual(0, admin1Scroll.VerticalOffset);
            admin1.User.TurnMouseWheel(5, MouseWheelTurnDirection.Backward, false);
            // Scrolling is not instantaneous. Wait for the browser to do its fancy scrolling.
            admin1Scroll.Wait.For(elem => elem.As<ScrollViewer>().VerticalOffset == 50);
            Assert.AreEqual(50, admin1Scroll.VerticalOffset);

            //
            // Expand Admin1 information
            //

            admin1.Find.ByName("maximiseButton").User.Click();
            admin1.Wait.ForNoMotion(100); // Wait until the animation is complete.
            System.Drawing.Rectangle r1 = admin1.GetScreenRectangle();
            Assert.AreEqual<System.Drawing.Rectangle>(new System.Drawing.Rectangle(280, 188, 774, 568), r1);

            #endregion

        }
    }
}
