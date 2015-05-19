using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Model;
using Moq;
using FileRenamer.Strategies;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestTextInsert
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

        // Use Moq to create an IFileMetaData object
        private IFileMetaData CreateFileMetaData(string name)
        {
            Mock<IFileMetaData> mock = new Mock<IFileMetaData>();
            mock.SetupGet(m => m.Name).Returns(name);
            return mock.Object;
        }

        /// <summary>
        /// Test the numbering strategy by loading a csv file
        /// </summary>
        [TestMethod]
        [DeploymentItem("Data\\TextInsertData.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "TextInsertData.csv", "TextInsertData#csv", DataAccessMethod.Sequential)]
        public void TestNumberingStrategy()
        {
            var row = TestContext.DataRow;

            // extract csv data
            string name = row["Name"].ToString();
            string extension = row["Extension"].ToString();
            int position = Int32.Parse(row["Position"].ToString());
            bool fromLeft = row["FromLeft"].ToString() == "Y";
            bool insert = row["Insert"].ToString() == "Y";
            string text = row["Text"].ToString();
            string expected = row["Expected"].ToString();

            InsertTextStrategy strategy = new InsertTextStrategy(position, text, fromLeft, insert);
            NameExtensionHelper helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            IFileMetaData file = CreateFileMetaData(name + "." + extension);

            string result = strategy.RenameFile(file, 0, helper);

            Assert.AreEqual(expected, result);

        }
    }
}
