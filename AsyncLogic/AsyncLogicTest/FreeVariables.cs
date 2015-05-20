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

        [TestMethod]
        public void TestFreeVariables()
        {
            LogicExpression test = x * x;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(1, lister.Variables.Count(), "There should only be one variable");
            Assert.AreEqual("x", lister.Variables.First());

        }
    }
}
