using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Model;
using FileRenamer.Strategies;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestNameExtensionHelperClass
    {
        // This holds the testcontext
        public TestContext TestContext { get; set; }

        private static TestContext _testContext;

        // Store the testcontext when the tests are started
        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        /// <summary>
        /// Test the remove characters strategy by loading a csv file
        /// </summary>
        [TestMethod]
        [DeploymentItem("Data\\NameExtensionData.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "NameExtensionData.csv", "NameExtensionData#csv", DataAccessMethod.Sequential)]
        public void TestNameExtensionHelper()
        {
            var row = TestContext.DataRow;

            // extract csv data
            string name = row["Name"].ToString();
            string extension = row["Extension"].ToString();
            int expectedPos = Int32.Parse(row["ExpectedPos"].ToString());
            bool hasExt = row["HasExt"].ToString() == "Y";
                        string behaviour = row["Behaviour"].ToString();

            string filename = hasExt ? name + "." + extension : name;

            NameExtensionHelper helper;
            switch (behaviour)
            {
                case "N":
                    helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
                    break;
                case "X":
                    helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.ExtensionOnly);
                    break;
                default: // Normally use "B" in csv file
                    helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension);
                    break;
            }

            helper.Text = filename;
            Assert.AreEqual(expectedPos, helper.ExtensionPos);

        }

    }
}
