using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use the Find object to locate elements on the page.
    /// </summary>
    [TestClass]
    // Please refer to UsingFindExpressionTestAttribute test for more details.
    [Find("MainTable", "id=mainTable1")]
    public class FindingElements : BaseTest
    {
        private const string TESTPAGE = "FindingElements.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.

        #endregion

        #region Page Under Test

        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //<head>
        //    <title>Finding Elements</title>
        //</head>
        //<body>
        //    <table border="1" id="maintable1">
        //        <tr>
        //            <td>
        //                <img src="./Images/FindEl01.png" alt="bannerImage" />
        //            </td>
        //        </tr>
        //        <tr>
        //            <td>
        //                <table summary="This table lists program available at the university
        //                        based on the discipline and type of degree."
        //                         border="1" rules="all">
        //                    <caption>
        //                        Programs Available</caption>
        //                    <colgroup class="program-discipline" />
        //                    <colgroup class="program-type" span="5" />
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
        //                </table>
        //            </td>
        //        </tr>
        //    </table>
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

            Manager.LaunchNewBrowser();
            ActiveBrowser.NavigateTo(Path.Combine(TestContext.TestDeploymentDir, TESTPAGE));
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

        #region Tests

        [TestMethod]
        [Description("Finding an element using its id attribute")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindById()
        {
            Element mainTable = Find.ById("mainTable1");
            Assert.IsTrue(mainTable.ElementType == ElementType.Table);
        }

        [TestMethod]
        [Description("Finding an element using its name attribute")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByName()
        {
            Element lastUpdate = Find.ByName("lastUpdate");
            Assert.IsTrue(lastUpdate.ElementType == ElementType.Input);
        }

        [TestMethod]
        [Description("Finding an element using its tag index")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByTagIndex()
        {
            // Finding the "ProgramsTable"
            //
            // Given that the table does not have an id/name we will use it's index.
            // In this case, the 'ProgramsTable' is the second occurring <table> on the page, so index=1
            Element programsTable = Find.ByTagIndex("table", 1);
            Assert.IsTrue(programsTable.ElementType == ElementType.Table);
        }

        [TestMethod]
        [Description("Finding an element using its content. (i.e. TextContent, Tag ... etc")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByContent()
        {
            // Get the row that contains the Education programs.

            // OPTION I
            //
            // First find the table cell that contains the literal text :'Education' (l: means literal)
            Element educationCell = Find.ByContent("l:Education", FindContentType.TextContent);
            // The first ByContent("") overload has default literal text and FindContentType = TextContent.
            // So we could also use:
            // Element educationRow = ActiveBrowser.Find.ByContent("Education");

            // The parent element is the table row desired
            Element educationRow = educationCell.Parent;

            // OPTION II - More complex
            //
            // Use RegEx with InnerMarkup. (x: means a RegEx)
            educationRow = Find.ByContent(@"x:^(<tr>\s*<td\s*scope=.*>\s*Education)", FindContentType.OuterMarkup);
            Assert.IsTrue(educationRow.TagName.Equals("tr"));
            Assert.IsTrue(educationRow.InnerText.Contains("Education"));

            // NOTE:
            //
            // There is a difference between FindContentType.InnerText & FindContentType.TextContent that is worth noting:
            //
            // Example: <div id="div1">Text1<div id="div2">Text2</div></div>
            //
            // InnerText for div1 : Text1Text2  {recursive}
            // TextContent of div1 : Text1      {non-recursive}
            //
            // Default for ByContent is TextContent which is the most common usage.
        }

        [TestMethod]
        [Description("Find an attribute using its markup attributes including partial matching")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByTagAttributes()
        {
            // Find this table:
            //          <table summary="This table lists program available at the university
            //                          based on the discipline and type of degree."
            //                          border="1" rules="all">

            // OPTION I
            //
            // ~ : signifies a partial value.
            // ByAttributes takes a params list, so you can include N number of name=value pairs to check.
            Element table = Find.ByAttributes("summary=~This table lists", "border=1", "rules=all");

            Assert.IsTrue(table.ElementType == ElementType.Table);
        }

        [TestMethod]
        [Description("Finding an element using its XPath")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByXPath()
        {
            // Find the banner img element
            Element img = Find.ByXPath("//body[1]/table[1]/tbody[1]/tr[1]/td[1]/img[1]");

            Assert.IsTrue(img.ElementType == ElementType.Image);

            // Notes:
            //
            // 1. Although the page does not contain a tbody for this table, IE/Firefox automatically
            //    add a tbody to the tables markup.
            // 2. The root element here is <HTML> so we can start the XPath with the body tag.
            // 3. You can use any XPath expression supported by System.Xml .NET 2.0
        }

        [TestMethod]
        [Description("Finding an element using its NodeIndexPath")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByNodeIndexPath()
        {
            // NodeIndexPath is similar to XPath identification. This type of identification
            // is introduced by WebAii.
            //
            // NodeIndexPath is the path to the element from the root node without a need to
            // specify a tag name as part of the path. NodeIndexPath is a list of integers
            // separated by a '/'. Each number is a zero based index of the tree node to follow
            // from the root to the desired element. The '/' represents a level in the tree path.
            //
            // To find the image above starting from the <HTML> node
            Element img = Find.ByNodeIndexPath("1/0/0/0/0/0");

            //<html> {root}
            //<head> {NodeIndex=0, Level 0}
            //    <title>Finding Elements</title>
            //</head>
            //<body> {NodeIndex=1, Level 0}
            //    <table border="1" id="maintable1"> {NodexIndex=0, Level 1}
            //      <tbody> {NodexIndex=0, Level 2}
            //        <tr> {NodexIndex=0, Level 3}
            //            <td> {NodexIndex=0, Level4}
            //                <img src="./Images/FindEl01.png" alt="bannerImage" /> {NodexIndex=0, Level5}

            Assert.IsTrue(img.ElementType == ElementType.Image);
        }

        [TestMethod]
        [Description("Find an element using a generic FindExpression object")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByExpression()
        {
            // Find.ByExpression is the core Element identification method that all the Find.Byxx methods
            // call into. ByExpression() takes in a FindExpression() class that can describe all the element
            // identifications supported by WebAii. The Find.Byxx methods simply construct a
            // FindExpression class under the covers to define their desired identification and pass it
            // on to this generic method Find.ByExpression() to perform the identification. Find.Byxx are simply short-cut
            // wrappers to this method.
            //
            // We can simply perform all the identifications described above
            // using this method with a FindExpression object.

            // Example 1: Find using an id (Identical to Find.ById)
            //
            Element e1 = Find.ByExpression(new HtmlFindExpression("id=mainTable1"));
            Assert.IsNotNull(e1);

            // Example 2: Find by attributes (Identical to Find.ByAttributes)
            //
            e1 = Find.ByExpression(new HtmlFindExpression("summary=~This table lists", "border=1", "rules=all"));
            Assert.IsNotNull(e1);

            // Example 3: Find by tag index (Identical to Find.ByTagIndex)
            //
            e1 = Find.ByExpression(new HtmlFindExpression("TagIndex=table:1"));
            Assert.IsNotNull(e1);

            // Other more complex find scenarios only supported through Find.ByExpression()
            // [These complex find scenarios will be revisited in the AjaxSupport scenarios and WaitForElement() scenarios]
            //
            // Example 4: Find the element with TextContent = Education and has an attribute scope=row
            e1 = Find.ByExpression(new HtmlFindExpression("TextContent=Education", "scope=row"));
            Assert.IsNotNull(e1);
        }

        [TestMethod]
        [Description("How to use [FindExpression()] attribute on test class/method to define elements to use.")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        [Find("UpdateDate", "name=lastUpdate")]
        public void UsingFindExpressionTestAttribute()
        {
            /// FindAttribute ([FindAttribute()] or [Find()], .NET allows both syntax)
            /// simply allows you to define a Find on a TestMethod or TestClass
            /// with a key so that you can refer to it later from within a test method or a test class. By using
            /// Find attributes you no longer need to identify each element on its on in your test and allows
            /// you to separate element identification from your test logic.
            ///
            /// If you are sharing specific elements across tests, you can place the common elements
            /// on the TestClass object and it will be accessible from all TestMethods() in that class.
            ///
            /// Refer to Find.ByExpression to see Find samples.

            // Example 1:
            //
            // Find the mainTable1 as defined on this TestClass (see class declaration)
            // in addition to the lastUpdate input element defined on this method

            // Access the elements directly since the this.Elements will do the Find.All() call before
            // returning the list of dictionary elements.
            Assert.IsTrue(this.Elements.GetHtml("MainTable").ElementType == ElementType.Table);
            Assert.IsTrue(this.Elements.GetHtml("UpdateDate").ElementType == ElementType.Input);

            // Example 2:
            //
            // Only identify the element defined on the test method but NOT on the TestClass

            Assert.IsTrue(this.Elements.GetHtml("UpdateDate").ElementType == ElementType.Input);
        }

        [TestMethod]
        [Description("Finding a collection of elements using different methods and criterion")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindAllByxx()
        {
            //
            // The Find.AllByxx methods allow you to find multiple elements on the page using certain criteria
            //

            // Example 1: Get all table elements on the page.
            //
            IList<Element> tables = Find.AllByTagName("table");
            Assert.IsTrue(tables.Count == 2);

            // Example 2: Get all table cells that contain a 'yes'
            //
            IList<Element> cells = Find.AllByContent("yes");
            Assert.IsTrue(cells.Count == 5);

            // Example 3: Find all table rows that contain a scope=row attribute
            IList<Element> rows = Find.AllByAttributes("scope=row"); // partial compare (~) is also supported
            Assert.IsTrue(rows.Count == 3);

            // Example 4: Same as #1 above but using an XPath
            tables = Find.AllByXPath("/descendant::table");
            Assert.IsTrue(tables.Count == 2);
        }

        [TestMethod]
        [Description("Advanced element identification using 'Chained Identification'")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void FindByChainedFindExpression()
        {
            // Example: Find the cell that contain 'scope=row' ONLY within the 'Education' row.

            // First: Find the 'Education' row. [See FindByContent example]
            // Second: Starting at that row, search its content for the first cell that contains a yes.
            //
            // Use chained identification to first find the row and then scope the second search
            // within the first element identified.
            //
            // With chained identification you can use N search expressions. Each one scopes the search
            // to the element previously identified in the chain
            Element cell = Find.ByExpression(@"outermarkup=#^(?i:<tr>\s*<td\s*scope=.*>\s*Education)", "|", "scope=row");
            Assert.IsTrue(cell.GetAttribute("scope").Value.Equals("row"));
            Assert.IsTrue(cell.Parent.Children[0].InnerText.Equals("Education"));
        }

        [TestMethod]
        [Description("Generating an external XML element identification file to be used by next test.")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        public void GenerateElemIDFile()
        {
            // First we build the list of FindParam objects we want serialized
            // and add them to a FindParamCollection object.

            HtmlFindExpression param1 = new HtmlFindExpression("TagIndex=table:0");
            HtmlFindExpression param2 = new HtmlFindExpression("TextContent=~test1", "class=mystyle");
            HtmlFindExpression param3 = new HtmlFindExpression("id=input1");

            // Now add each FindParam defined above to the FindParamCollection
            // and give it a key that you can use to access it with later on.
            FindExpressionCollection<HtmlFindExpression> paramCol = new FindExpressionCollection<HtmlFindExpression>();
            paramCol.Add("DataTable", param1);
            paramCol.Add("TargetDataCell", param2);
            paramCol.Add("InputTextBox", param3);

            // Now you can serialize this list to a string or a file
            // you can store with the test code.
            paramCol.Save("C:\\AppParams.xml");

            // Or you can serialize to a string to be stored
            // in your choice of storage medium.
            //
            //Dim serializedParams As String = paramCol.ToXml()
        }

        [TestMethod]
        [Description("De-coupling of element identification from test code by using external XML files.")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void UsingLoadFromFile()
        {
            // Given we already have a datasource of FindExpression's stored on a different medium,
            // we can deserialize the data back into a FindExpressionCollection object and then
            // select one of these FindExpression's to locate a specific element on the page
            // like this:
            FindExpressionCollection<HtmlFindExpression> exprColl = FindExpressionCollection<HtmlFindExpression>.LoadFromFile(Path.Combine(TestContext.TestDir, @"..\..\SupportFiles\FindElementsFromFile.xml"));

            Assert.IsTrue(exprColl.Count == 2);
            Assert.IsTrue(this.Find.ByExpression(exprColl["MainTable1"]).ElementType == ElementType.Table);
            Assert.IsTrue(this.Find.ByExpression(exprColl["ProgramsTable"]).ElementType == ElementType.Table);
        }

        [TestMethod]
        [Description("De-coupling of element identification from test code by using external XML files.")]
        [DeploymentItem(@"Pages\FindingElements.htm")]
        [DeploymentItem(@"SupportFiles\FindElementsFromFile.xml")]
        [DeploymentItem(@"Pages\Images\FindEl01.png", "Images")]
        public void UsingFindFromFile()
        {
            // FindExpression's can be completely extracted out of test code and stored in an external xml file.
            //
            // This allows you to:
            // 1. Clean out your test code so that it purely contains the testing logic without it being
            //    cluttered with identification of elements.
            // 2. Update your automated tests due to website changes by simply updating the xml file
            //    without a need to recompile your tests.
            // 3. Flexibility in segmenting and organizing your FindExpression's for your project. (i.e. an XML file
            //    that contains FindExpression's for certain versions of the website. Or XML file for each page of your site...etc)
            // 4. Store the FindExpression's in SQL or use DataSources to retrieve these files.

            // The SupportFiles\FindElementsFromFile.xml shows a sample xml file with two FindExpression's defined; one for
            // the MainTable and the other for ProgramsTable.
            IDictionary<string, Element> elements = Find.FromExpressionsFile(Path.Combine(TestContext.TestDeploymentDir, "FindElementsFromFile.xml"));

            Assert.IsTrue(elements["MainTable1"].ElementType == ElementType.Table);
            Assert.IsTrue(elements["ProgramsTable"].ElementType == ElementType.Table);

            // Note:
            // 1. If you have the XML string stored in a DataBase, you can use the Find.FromXml()
            //    to find these elements directly from the string.
        }

        #endregion
    }
}
