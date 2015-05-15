using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Strategies;
using System.Collections.Generic;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestCaseChanging
    {
        private CaseChangingStrategy strategy;



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

            for (int i = 0; i < data.Count; i++)
            {
                string result = (string)accessor.Invoke("toCamelCase", data[i]);
                Assert.AreEqual(expected[i], result, "Case " + i + " failed");
            }

        }
    }
}
