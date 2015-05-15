using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Strategies;
using System.Collections.Generic;
using System.Linq;
using FileRenamer.Model;
using Moq;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestCaseChanging
    {
        private CaseChangingStrategy strategy;

        List<string> names = new List<string> { "hello", "HELLO", "HELLOworld" };

        // Use Moq to create an IFileMetaData object
        private IFileMetaData CreateFileMetaData(string name)
        {
            Mock<IFileMetaData> mock = new Mock<IFileMetaData>();
            mock.SetupGet(m => m.Name).Returns(name);
            return mock.Object;
        }

        private void AssertListsEqual(List<string> expected, List<string> result)
        {
            if (expected.Count != result.Count)
            {
                Assert.Fail("Error in test, expected and result lists should have the same length");
            }
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], result[i], "Case " + i + " failed");
            }
        }

        [TestMethod]
        public void TestLowerCase()
        {
            List<string> expected = names.Select(n => n.ToLower()).ToList();
            NameExtensionHelper test= NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension);

            strategy = new CaseChangingStrategy(CaseTypes.Lowercase);
            List<string> result = expected.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();

            AssertListsEqual(expected, result);        
        }

        [TestMethod]
        public void TestUpperCase()
        {
            List<string> expected = names.Select(n => n.ToUpper()).ToList();
            NameExtensionHelper test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension);

            strategy = new CaseChangingStrategy(CaseTypes.Uppercase);
            List<string> result = expected.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();

            AssertListsEqual(expected, result);
        }

        [TestMethod]
        public void TestCamelCase()
        {
            // arrange
            strategy = new CaseChangingStrategy(CaseTypes.Camelcase);

            // we are accessing a private method
            PrivateObject accessor = new PrivateObject(strategy);

            List<string> data = new List<string>     { "hello", "hello world", "123hello", "hello123 23world", "hello123 world123",
            "123Hello"};
            List<string> expected = new List<string> { "Hello", "Hello World", "123hello", "Hello123 23world", "Hello123 World123",
            "123hello"};

            // act
            List<string> result = data.Select(str => (string)accessor.Invoke("toCamelCase", str)).ToList();
            AssertListsEqual(expected, result);

            //for (int i = 0; i < data.Count; i++)
            //{
            //    string result = (string)accessor.Invoke("toCamelCase", data[i]);
            //    Assert.AreEqual(expected[i], result, "Case " + i + " failed");
            //}
        }

        [TestMethod]
        public void TestSentenceCase()
        {
            // arrange
            strategy = new CaseChangingStrategy(CaseTypes.Camelcase);

            // we are accessing a private method
            PrivateObject accessor = new PrivateObject(strategy);

            List<string> data = new List<string>     { "hello", "hello world", "123hello", "hello123 23world", "hello123 world123",
            "123Hello", "Hello. world"};
            List<string> expected = new List<string> { "Hello", "Hello world", "123hello", "Hello123 23world", "Hello123 world123",
            "123hello", "Hello. World"};

            // act
            List<string> result = data.Select(str => (string)accessor.Invoke("toSentenceCase", str)).ToList();
            AssertListsEqual(expected, result);

            //for (int i = 0; i < data.Count; i++)
            //{
            //    string result = (string)accessor.Invoke("toSentenceCase", data[i]);
            //    Assert.AreEqual(expected[i], result, "Case " + i + " failed");
            //}

        }
    }
}
