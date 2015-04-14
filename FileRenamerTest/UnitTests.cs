using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer;
using System.Linq;

namespace FileRenamerTest
{
    [TestClass]
    public class UnitTestNumbering
    {
        [TestMethod]
        public void TestAlphaNumbering()
        {
            for (int i = 1; i < 100000; i++)
            {
                Assert.AreEqual(StringNumberConversions.StringToNumber(StringNumberConversions.NumberToString(i)), i,i.ToString());
            }
        }
        
        [TestMethod]
        public void TestAlphaNumbering2()
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
