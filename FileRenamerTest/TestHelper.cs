using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileRenamerTest
{
    public static class TestHelper
    {
        public static void AssertListsEqual(IEnumerable<string> expected, IEnumerable<string> result)
        {
            if (expected.Count() != result.Count())
            {
                Assert.Fail("Error in test, expected and result lists should have the same length");
            }

            var query = expected.Zip(result, (a, b) => new { first = a, second = b });
            int i = 0;
            foreach (var pair in query)
            {
                Assert.AreEqual(pair.first, pair.second, "Case " + i + " failed");
                i++;
            }

            //for (var i = 0; i < expected.Count(); i++)
            //{
            //    Assert.AreEqual(expected[i], result[i], "Case " + i + " failed");
            //}
        }
    }
}
