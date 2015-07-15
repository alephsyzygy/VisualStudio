using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Model;

namespace FileRenamerTest.Model
{
    [TestClass]
    public class TestID3Tag
    {
        [TestMethod]
        public void TestRemoveNul()
        {
            // Note: PrivateType in order to invoke private static methods
            PrivateType accessor = new PrivateType(typeof (ID3Tag));
            string result = (string) accessor.InvokeStatic("RemoveNul", "test" + '\0' + "rest");
            Assert.AreEqual("test", result);

            result = (string)accessor.InvokeStatic("RemoveNul", "test");
            Assert.AreEqual("test", result);
        }
    }
}
