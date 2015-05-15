using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileRenamer.Strategies;
using System.Linq;

namespace FileRenamerTest.Strategies
{
    [TestClass]
    public class TestNumbering
    {
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
