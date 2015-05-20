using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Linq;

namespace AsyncLogicTest
{
    [TestClass]
    public class FreeVariables
    {
        static LogicExpression x = new LogicVariable("x");
        static LogicExpression y = new LogicVariable("y");
        static LogicExpression z = new LogicVariable("z");
        static NumExpression n = new NumVariable("n");
        static NumExpression m = new NumVariable("m");

        [TestMethod]
        public void TestVariables()
        {
            LogicExpression test = x * x;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(1, lister.Variables.Count(), "There should only be one variable");
            Assert.AreEqual("x", lister.Variables.First());

        }

        [TestMethod]
        public void TestNumVariables()
        {
            NumExpression numTest = n;
            LogicExpression test = new NumRelation(NumRels.EQ, n, m) + y;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit((IExpressionVisitor<bool>) lister);
            Assert.AreEqual(3, lister.Variables.Count(), "There should be three variables");
            Assert.IsTrue(lister.Variables.Contains("y"));
            Assert.IsTrue(lister.Variables.Contains("n"));
            Assert.IsTrue(lister.Variables.Contains("m"));
        }
    }
}
