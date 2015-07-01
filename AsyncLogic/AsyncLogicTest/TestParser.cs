using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic.Parser;
using Sprache;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestNumParser()
        {
            string input = "345";
            var output = ExpressionParser.Number.Parse(input);
            Assert.AreEqual(345, output);

            input = "((56 + 3) * (8))";
            var numExp = ExpressionParser.NumExpr.Parse(input);
            var result = ExpressionParser.NumExpr.TryParse(input);
            
            Assert.AreEqual("((56 + 3) * 8)", numExp.ToString());

            input = " (( 5 + x) * (y+6 ))";
            numExp = ExpressionParser.NumExpr.Parse(input);
            Assert.AreEqual("((5 + x) * (y + 6))", numExp.ToString());

            input = " 3 + 4 + 5";
            Assert.AreEqual("((3 + 4) + 5)", ExpressionParser.NumExpr.Parse(input).ToString());

            input = "The n. n == n + 3";
            Assert.AreEqual("The n. (n == (n + 3))", ExpressionParser.NumExpr.Parse(input).ToString());



            input = "The n. Exists m. n == m";
            Assert.AreEqual("The n. Exists m. (n == m)", ExpressionParser.NumExpr.Parse(input).ToString(), "the exist");
        }

        [TestMethod]
        public void TestLogParser()
        {
            string input = "x | T";
            Assert.AreEqual("(x | T)", ExpressionParser.LogExpr.Parse(input).ToString());



            input = "x & T | y";
            Assert.AreEqual("((x & T) | y)", ExpressionParser.LogExpr.Parse(input).ToString());

            input = "Exists n. n == n";
            Assert.AreEqual("Exists n. (n == n)", ExpressionParser.LogExpr.Parse(input).ToString());

            input = "Exists n. (The n . n == 2) == 3";
            Assert.AreEqual("Exists n. (The n. (n == 2) == 3)", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the");

            input = "Exists n. The n . n == 2 == 3";
            Assert.AreEqual("Exists n. (The n. (n == 2) == 3)", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the2");


        }

        [TestMethod]
        public void TestDoubleExists()
        {
            string input = "Exists n. Exists m. T";
            Assert.AreEqual("Exists n. Exists m. T", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the");
        }
    }
}
