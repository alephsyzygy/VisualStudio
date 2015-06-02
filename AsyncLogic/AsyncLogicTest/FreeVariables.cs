using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Linq;
using System.Collections.Generic;

// Hide some of the warnings to do with self comparison
#pragma warning disable 1718

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
        static NumExpression two = new NumConstant(2);

        [TestMethod]
        public void TestVariables()
        {
            LogicExpression test = x & x;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(1, lister.Variables.Count(), "There should only be one variable");
            Assert.AreEqual("x", lister.Variables.First());

        }

        [TestMethod]
        public void TestNumVariables()
        {
            NumExpression numTest = n;
            LogicExpression test = new NumRelation(NumRels.EQ, n, m) | y;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(3, lister.Variables.Count(), "There should be three variables");
            Assert.IsTrue(lister.Variables.Contains("y"));
            Assert.IsTrue(lister.Variables.Contains("n"));
            Assert.IsTrue(lister.Variables.Contains("m"));

            test = (new NumThe("n", n == two)) == two;
            lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(1, lister.Variables.Count);
            Assert.IsTrue(lister.Variables.Contains("n"));
        }

        [TestMethod]
        public void TestFreeVariables()
        {
            SortedSet<string> variables;
            LogicExpression test = new NumExists("n", n == m);
            FreeVariableLister lister = new FreeVariableLister();
            variables = test.Visit(lister);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.Contains("m"));

            test = new NumExists("x", n == n);
            lister = new FreeVariableLister();
            variables = test.Visit(lister);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.Contains("n"));

            test = (new NumThe("n", n == two)) == two;
            lister = new FreeVariableLister();
            variables = test.Visit(lister);
            Assert.AreEqual(0, variables.Count);
            //Assert.IsTrue(lister.Variables.Contains("n"));

            // this test shows that the original method does not work
            test = n == n & new NumExists("n", n == two);
            lister = new FreeVariableLister();
            variables = test.Visit(lister);
            Assert.AreEqual(1, variables.Count);
        }
    }
}
