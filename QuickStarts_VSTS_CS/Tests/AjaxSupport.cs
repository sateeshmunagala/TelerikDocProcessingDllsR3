using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestAttributes;
using ArtOfTest.WebAii.TestTemplates;

namespace QuickStarts_VSTS_CS
{
    /// <summary>
    /// How to use WebAii's Ajax support to test Ajax Applications.
    /// </summary>
    [TestClass]
    [Find("activationDiv", "id=data")]
    public class AjaxSupport : BaseTest
    {
        private const string TESTPAGE = @"AjaxSupport.htm";

        #region Notes

        //
        // 1. Given that all tests in this class navigate to the same page, we placed that code directly
        //    in the [TestInitialize()] method.
        //

        #endregion

        #region Page Under Test
        //<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
        //<html>
        //<head>
        //    <title>WebAii - WaitForElement Sample2</title>

        //    <script type="text/javascript">  var newDiv;
        //    // Simulates an ajax call for
        //    // purpose of this demo
        //    function OnAjaxCall()
        //    {
        //        // Will wait 4 seconds and then call the AjaxCallReturned
        //        // function.
        //        window.setTimeout("AjaxCallReturned()",1000);
        //    }

        //    // Function responding to an ajax call
        //    // and invokes changes on the page.
        //    function AjaxCallReturned()
        //    {
        //        newDiv = document.createElement("div");
        //        newDiv.innerHTML = "<h3>We are back!</h3>";
        //        // add the newly created element as child to the data div
        //        var myDataDiv = document.getElementById("data");
        //        myDataDiv.appendChild(newDiv);

        //        // simulate another Ajax call.
        //        window.setTimeout("AjaxCallPart2();",1000);
        //    }

        //    // A function called in response to the returned
        //    // ajax call that also affects the page.
        //    function AjaxCallPart2()
        //    {
        //        // in here we will set the color of the added text.
        //        newDiv.style.backgroundColor = "red";
        //    }
        //    </script>
        //</head>
        //<body>
        //    <div id="data" onclick="OnAjaxCall()" style="width: 200px; height: 100px; border: solid 1px black;
        //        text-align: center">
        //    </div>
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
            #region WebAii CleanUp

            // Shuts down WebAii manager and closes all browsers currently running
            this.CleanUp();

            #endregion
        }

        #endregion

        #region Tests

        [TestMethod]
        [Description("How to wait for a single change on the page")]
        [DeploymentItem(@"Pages\AjaxSupport.htm")]
        public void WaitingForSinglePageChange()
        {
            // The test page performs two operations async; each with 1 second delay
            // after the box is clicked:
            // 1.Creates a new Div with a child H3 element and adds it to the page.
            // 2.Changes the back color of the new div to red

            // Let's invoke the actions and wait for the H3 element to appear.

            // To do so, we will simply use the FindExpression object as described in FindingElements.cs
            // to define how an element should be identified.
            HtmlFindExpression h3 = new HtmlFindExpression("TagIndex=h3:0"); // Basically the first occurrence of an h3 element.

            // Initiate the action.
            Actions.Click(this.Elements.GetHtml("activationDiv"));

            // Wait for the element to appear on the page. Timeout: 2 seconds.
            ActiveBrowser.WaitForElement(h3, 3000, false);

            // Element should be found at this point. To verify.
            ActiveBrowser.RefreshDomTree(); // refresh the current DOM to reflect the javascript changes
            Element h3Element = Find.ByExpression(h3); // find the element
            Assert.IsTrue(h3Element.InnerText.Equals("We are back!"));
        }

        [TestMethod]
        [Description("How to wait for multiple changes on the page")]
        [DeploymentItem(@"Pages\AjaxSupport.htm")]
        public void WaitingForMultiplePageChanges()
        {
            // Using the same example above but instead of only waiting for the new div by checking the h3
            // element and then we want to also wait for the backcolor of the div to change to red.

            // Wait for the h3 to be added.
            HtmlFindExpression h3 = new HtmlFindExpression("TagIndex=h3:0"); // Basically the first occurrence of an h3 element.

            // Wait for the div with index=1 to appear in addition to the style having partial value:
            // background-color: red
            HtmlFindExpression newDiv = new HtmlFindExpression("TagIndex=div:1", "style=~background-color: red");

            // Invoke the action
            Actions.Click(this.Elements.GetHtml("activationDiv"));

            // Wait for all changes using a chained Find Expression
            HtmlFindExpression expr = new HtmlFindExpression();
            expr.AppendClauses(false, newDiv.Clauses);
            expr.AppendChain(h3);
            ActiveBrowser.WaitForElement(expr, 4000, false);

            // All elements should be found at this point. To verify.
            ActiveBrowser.RefreshDomTree();

            Element h3Element = Find.ByExpression(h3); // find the element
            Assert.IsTrue(h3Element.InnerText.Equals("We are back!"));

            Element newDivElement = Find.ByExpression(newDiv);
            Assert.IsTrue(newDivElement.GetAttribute("style").Value.ToLower().
                Contains("background-color: red"));
        }

        #endregion
    }
}
