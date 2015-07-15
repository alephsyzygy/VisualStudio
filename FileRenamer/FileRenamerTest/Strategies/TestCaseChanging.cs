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
        List<string> extensions = new List<string> { "txt", "tXt", "TXT" };

        // Use Moq to create an IFileMetaData object
        private IFileMetaData CreateFileMetaData(string name)
        {
            Mock<IFileMetaData> mock = new Mock<IFileMetaData>();
            mock.SetupGet(m => m.Name).Returns(name);
            return mock.Object;
        }



        [TestMethod]
        public void TestLowerCase()
        {
            IEnumerable<string> filenames        = from name in names
                                                   from ext in extensions
                                                   select name + '.' + ext;
            IEnumerable<string> expectedNameOnly = from name in names
                                                   from ext in extensions
                                                   select name.ToLower() + '.' + ext;
            IEnumerable<string> expectedExtOnly  = from name in names
                                                   from ext in extensions
                                                   select name + '.' + ext.ToLower();
            IEnumerable<string> expectedFull     = from name in names
                                                   from ext in extensions
                                                   select name.ToLower() + '.' + ext.ToLower();
            // Test full filename behaviour
            NameExtensionHelper test= NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension);
            strategy = new CaseChangingStrategy(CaseTypes.Lowercase);
            List<string> result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedFull, result);

            // Test name part of the filename
            test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            strategy = new CaseChangingStrategy(CaseTypes.Lowercase);
            result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedNameOnly, result);

            // test extension part of the filename
            test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.ExtensionOnly);
            strategy = new CaseChangingStrategy(CaseTypes.Lowercase);
            result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedExtOnly, result);

        }

        [TestMethod]
        public void TestUpperCase()
        {
            IEnumerable<string> filenames        = from name in names
                                                   from ext in extensions
                                                   select name + '.' + ext;
            IEnumerable<string> expectedNameOnly = from name in names
                                                   from ext in extensions
                                                   select name.ToUpper() + '.' + ext;
            IEnumerable<string> expectedExtOnly  = from name in names
                                                   from ext in extensions
                                                   select name + '.' + ext.ToUpper();
            IEnumerable<string> expectedFull     = from name in names
                                                   from ext in extensions
                                                   select name.ToUpper() + '.' + ext.ToUpper();
            // Test full filename behaviour
            NameExtensionHelper test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.BothNameExtension);
            strategy = new CaseChangingStrategy(CaseTypes.Uppercase);
            List<string> result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedFull, result);

            // Test name part of the filename
            test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.NameOnly);
            strategy = new CaseChangingStrategy(CaseTypes.Uppercase);
            result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedNameOnly, result);

            // test extension part of the filename
            test = NameExtensionHelper.CreateNameExtensionHelper(NameExtensionBehaviour.ExtensionOnly);
            strategy = new CaseChangingStrategy(CaseTypes.Uppercase);
            result = filenames.Select(name => strategy.RenameFile(CreateFileMetaData(name), 0, test)).ToList();
            TestHelper.AssertListsEqual(expectedExtOnly, result);
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
            TestHelper.AssertListsEqual(expected, result);

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
            TestHelper.AssertListsEqual(expected, result);

            //for (int i = 0; i < data.Count; i++)
            //{
            //    string result = (string)accessor.Invoke("toSentenceCase", data[i]);
            //    Assert.AreEqual(expected[i], result, "Case " + i + " failed");
            //}

        }
    }
}
