using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;
using ArtOfTest.WebAii.TestAttributes;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use TestRegions support in WebAii
    /// </summary>
    [TestClass]
    public class TestRegions : BaseTest
    {
        private const string TESTPAGE = @"TestRegions.htm";

        #region Notes
        //
        // 1. Please make sure to read the out whitepaper on TestRegions published
        //    here: http://www.artoftest.com/Resources/Whitepapers/introtesttags.aspx
        //    to better understand these samples
        //
        #endregion

        #region Page Under Test

        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //<head>
        //    <title>TestRegions</title>
        //</head>
        //<body>
        //    <!--testregion id="MainTable"-->
        //    <table border="1" id="maintable1">
        //        <tr>
        //            <td>
        //                <img src="./Images/FindEl01.png" alt="bannerImage" />
        //            </td>
        //        </tr>
        //        <tr>
        //            <td>
        //                <!--testregion id="ProgramsTable"-->
        //                <table summary="This table lists program available at the university
        //                        based on the discipline and type of degree."
        //                         border="1" rules="all">
        //                    <caption>
        //                        Programs Available</caption>
        //                    <colgroup class="program-discipline"></colgroup>
        //                    <colgroup class="program-type" span="5"></colgroup>

        //                    <!--testregion id="ProgramsTableHead"-->
        //                    <thead>
        //                        <tr>
        //                            <th scope="col">
        //                                Program</th>
        //                            <th scope="col">
        //                                Honors Co-op</th>
        //                            <th scope="col">
        //                                Honors Regular</th>
        //                            <th scope="col">
        //                                General Regular</th>
        //                            <th scope="col">
        //                                *Preprofessional or Professional</th>
        //                        </tr>
        //                    </thead>
        //                    <!--/testregion-->
        //                    <!--testregion id="ProgramsTableFooter"-->
        //                    <tfoot class="footnote">
        //                        <tr>
        //                            <td colspan="5">
        //                                Many disciplines are also available as Minors and Joint Honors programs.
        //                            </td>
        //                        </tr>
        //                        <tr>
        //                            <td colspan="5">
        //                                * Preprofessional programs normally fulfill the academic requirements for registration
        //                                in the related professions.
        //                                <input name="lastUpdate" value="July 20, 2007" type="hidden" />
        //                            </td>
        //                        </tr>
        //                    </tfoot>
        //                    <!--/testregion-->
        //                    <!--testregion id="ProgramsTableBody"-->
        //                    <tbody>
        //                        <tr>
        //                            <td scope="row">
        //                                Computer Science</td>
        //                            <td>
        //                                yes</td>
        //                            <td>
        //                                yes</td>
        //                            <td>
        //                                no</td>
        //                            <td>
        //                                no</td>
        //                        </tr>
        //                        <tr>
        //                            <td scope="row">
        //                                Education</td>
        //                            <td>
        //                                no</td>
        //                            <td>
        //                                no</td>
        //                            <td>
        //                                yes</td>
        //                            <td>
        //                                no</td>
        //                        </tr>
        //                        <tr>
        //                            <td scope="row">
        //                                Physics</td>
        //                            <td>
        //                                yes</td>
        //                            <td>
        //                                no</td>
        //                            <td>
        //                                yes</td>
        //                            <td>
        //                                no</td>
        //                        </tr>
        //                    </tbody>
        //                    <!--/testregion-->
        //                </table>
        //                <!--/testregion-->
        //            </td>
        //        </tr>
        //    </table>
        //    <!--/testregion-->
        //</body>
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
        [Description("How to access testregions defined in markup page.")]
        [DeploymentItem(@"Pages\TestRegions.htm")]
        public void AccessingTestRegions()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // The TestPage has 5 TestRegions defined on it. They all can be accessed
            // using the DomTree.Regions collection which contains a flat list of
            // all regions regardless of their nesting in the markup.
            Assert.IsTrue(ActiveBrowser.Regions.Count == 5);

            // Access the MainTable test region.
            TestRegion mainTable = ActiveBrowser.Regions["MainTable"];
            Assert.IsTrue(mainTable.Id.Equals("MainTable"));

            // Access the programs table header.
            TestRegion programsTableHeader = ActiveBrowser.Regions["ProgramsTableHead"];

            // Or - You can walk down the hierarchy of test regions.
            //
            // Note: The only reason you might want to do that is to validate a certain
            // high-level structure of your page as part of your testing
            // to make sure that all main parts are correctly positioned
            programsTableHeader = mainTable.ChildRegions["ProgramsTable"].ChildRegions["ProgramsTableHead"];

            // TestRegion.ChildRegions contains only the immediate child
            // TestRegions where AllChildRegions contains all sub TestRegions
            // regardless of their nesting even within other sub TestRegions.
            Assert.IsTrue(mainTable.ChildRegions.Count == 1);
            Assert.IsTrue(mainTable.AllChildRegions.Count == 4);
        }

        [TestMethod]
        [Description("How to access elements contained in a TestRegion using Region Based Identification")]
        [DeploymentItem(@"Pages\TestRegions.htm")]
        public void AccessingElementsUsingTestRegions()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // Each TestRegion object has a Find object associated with it that
            // performs the search within the elements contained in that region ONLY.
            // Any element search that requires indexing will be performed with
            // the <testregion> tag of that region as the root element to start
            // the indexing from. Below are examples to illustrate that.

            // MainTable region contains the table id="maintable1" but other
            // regions don't contain it.
            TestRegion MainTableRegion = ActiveBrowser.Regions["MainTable"];
            TestRegion ProgramsTableRegion = ActiveBrowser.Regions["ProgramsTable"];

            //
            // Scoping Search To TestRegion:
            //
            Assert.IsNotNull(MainTableRegion.Find.ById("maintable1"));
            Assert.IsNull(ProgramsTableRegion.Find.ById("maintable1"));

            //
            // Scoping Indexing To TestRegion :
            //
            // Let's find the ProgramsTable using tag index.
            //
            //
            // A) Common search without TestRegions
            // [We usually index the tag name from the root of the page which in this
            //  case is '1' - the second occurrence of the tag 'table']
            Element secondTableWithoutTR = Find.ByTagIndex("table", 1);

            // B) Using TestRegions
            // [We only need to index from the beginning of the TestRegion we are accessing]
            Element secondTableWithTR = ProgramsTableRegion.Find.ByTagIndex("table", 0);

            Assert.IsTrue(secondTableWithoutTR.Equals(secondTableWithTR));

            // Note:
            //
            // With B) we don't care how the page changes outside the testregion
            // this test is targetting. This means that we can literally take
            // that TestRegion and paste it in another page and the test should
            // work. In contrast with A) the test is more vulnerable to changes. For
            // example, if another table is added before ProgramsTable, the test
            // will break where B won't.
            //
            // For highly dynamic sites or sites in early development with
            // lots of churn, it is recommended to use TestRegions to limit
            // the cost of maintaining tests.
        }

        [TestMethod]
        [Description("Illustrate how to use FindExpressionAttributes with TestRegions")]
        [DeploymentItem(@"Pages\TestRegions.htm")]
        [Find("progtbl", "TagIndex=table:1")]
        [Find("progtbltr", "TagIndex=table:0", TestRegionId = "ProgramsTable")]
        public void UsingFindExpressionAttributeWithTestRegions()
        {
            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));

            // FindExpression attributes can be associated to a specific TestRegion.
            // When a TestRegionId is specified, only the Find object off that region
            // object can be used to access that element. The search will be scoped
            // to that region also.

            // Two FindExpression attribues are defined on this method. Both point to the
            // same element on the page.
            // The first is generic can be accessed using the ActiveBrowser.Find object.
            // The second can be accessed only using the ProgramsTable.Find object.
            TestRegion ProgramsTable = ActiveBrowser.Regions["ProgramsTable"];

            // Contains only the element defined and scoped to "ProgramsTable";
            Assert.IsTrue(this.Elements.GetHtml("progtbl").Equals(this.Elements.GetHtml("progtbltr")));

            // Note: You can also use the FindAttribute(fileName) and scope all
            // the elements contained in file to a specific testregion.
        }
        #endregion
    }
}
