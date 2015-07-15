using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Strategies;
using System.Linq;
using FileRenamer.Model;
using Moq;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestNumbering
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
        [DeploymentItem("Data\\NumberingData.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "NumberingData.csv", "NumberingData#csv", DataAccessMethod.Sequential)]
        public void TestNumberingStrategy()
        {
            var row = TestContext.DataRow;
            
            // extract csv data
            string name = row["Name"].ToString();
            string extension = row["Extension"].ToString();
            int number = Int32.Parse(row["Number"].ToString());
            NumberingFormat format = (NumberingFormat) Int32.Parse(row["Format"].ToString());
            NumberingTextFormat textformat = (NumberingTextFormat)Int32.Parse(row["TextFormat"].ToString());
            string text = row["Text"].ToString();
            string expected = row["Expected"].ToString();

            NumberingStrategy strategy = new NumberingStrategy(format, textformat, "0", text);
            NameExtensionHelper helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            IFileMetaData file = CreateFileMetaData(name + "." + extension);

            string result = strategy.RenameFile(file, number, helper);

            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void TestNumberingStrategySequential()
        {
            NumberingStrategy strategy = new NumberingStrategy(NumberingFormat.NoZeros, NumberingTextFormat.TextNumber, "5", "Test");
            var helper = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            var file = CreateFileMetaData("test.txt");
            string result = strategy.RenameFile(file, 4, helper);
            Assert.AreEqual("Test9.txt", result);

        }

        /// <summary>
        /// Test the round trip number to alpha to number
        /// </summary>
        [TestMethod]
        public void TestNumberAlphaNumber()
        {
            for (int i = 1; i < 100000; i++)
            {
                Assert.AreEqual(StringNumberConversions.StringToNumber(StringNumberConversions.NumberToString(i)), i,i.ToString());
            }
        }
        
        /// <summary>
        /// Test the round trip alpha to number to alpha
        /// </summary>
        [TestMethod]
        public void TestAlphaNumberAlpha()
        {
            string randomStr;
            // only test upto length 5
            for (int j = 1; j < 6; j++)
                // run 1000 tests
                for (int i = 0; i < 1000; i++)
                {
                    randomStr = RandomString(j);
                    Assert.AreEqual(StringNumberConversions.NumberToString(StringNumberConversions.StringToNumber(randomStr)), randomStr, randomStr);
                }
        }

        // create some random string
        private string RandomString(int Num)
        {
            int x = Num;
            var chars = "abcdefghijlkmnopqrstuvwxyz";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, Num)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
