using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Model;
using Moq;
using FileRenamer.Strategies;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestRemoveCharacters
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
        /// Test the remove characters strategy by loading a csv file
        /// </summary>
        [TestMethod]
        [DeploymentItem("Data\\RemoveCharactersData.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "RemoveCharactersData.csv", "RemoveCharactersData#csv", DataAccessMethod.Sequential)]
        public void TestRemoveCharactersStrategy()
        {
            var row = TestContext.DataRow;

            // extract csv data
            string name = row["Name"].ToString();
            string extension = row["Extension"].ToString();
            int from = Int32.Parse(row["From"].ToString());
            int to = Int32.Parse(row["To"].ToString());
            bool fromLeft = row["FromLeft"].ToString() == "Y";
            bool toLeft = row["ToLeft"].ToString() == "Y";
            string expected = row["Expected"].ToString();
            string behaviour = row["Behaviour"].ToString();

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

            RemoveCharactersStrategy strategy = new RemoveCharactersStrategy(from, fromLeft, to, toLeft);
            IFileMetaData file = CreateFileMetaData(name + "." + extension);

            string result = strategy.RenameFile(file, 0, helper);

            Assert.AreEqual(expected, result);

        }

    }
}
