using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic.Expressions;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestToString
    {
        const int defaultTimeout = 500;  // 0.5 seconds
        static NumExpression n = new NumVariable("n");
        static NumExpression m = new NumVariable("m");
        static LogicExpression logicTrue = new LogicTrue();
        static LogicExpression logicFalse = new LogicFalse();
        static LogicExpression logicLoop = new NumExists("n", n < n); // this expression loops forever
        static LogicExpression x = new LogicVariable("x");
        static NumExpression zero = new NumConstant(0);
        static NumExpression one = new NumConstant(1);
        static NumExpression two = new NumConstant(2);


        const string True = "True";
        const string Loop = "False";
        const string False = Loop;

        [TestMethod]
        public void TestString()
        {
            var step2 = x & m <= new NumConstant(5);
            var rec2 = new RecLogicExpression(n, logicTrue, "m", "x", step2);
            var testLog = new NumConstant(6) == new NumThe("n", n > new NumConstant(5) & rec2);

            Assert.AreEqual("(6 == The n. ((n > 5) & Rec(n, True, m.x. (x & (m <= 5)))))", testLog.ToString());

            var test5 = new Apply(new LambdaExpression("x", x | logicTrue), logicLoop);
            Assert.AreEqual("(Lambda x. (x | True) @ Exists n. (n < n))", test5.ToString());

            var loopTest = new PairExpression<LogicExpression,LogicExpression>(logicLoop, logicTrue);
            var pairLoop = new LogicProjR(loopTest);

            Assert.AreEqual("Snd <Exists n. (n < n), True>", pairLoop.ToString());
        }
    }
}
