using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Threading.Tasks;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestExpressionEvaluator
    {
        const int timeout = 500;  // 0.5 seconds
        static LogicExpression logicTrue = new LogicTrue();
        static LogicExpression logicFalse = new LogicFalse();
        static LogicExpression x = new LogicVariable("x");

        [TestMethod]
        public void TestAnd()
        {
            LogicExpression test = logicTrue * logicFalse;
            Assert.AreEqual("Loop", testAsync(test, timeout).Result);
        }

        [TestMethod]
        public void TestOr()
        {
            LogicExpression test = logicTrue + logicFalse;
            Assert.AreEqual("True", testAsync(test, timeout).Result);
        }

        [TestMethod]
        public void TestNum()
        {
            NumExpression test = new NumConstant(500);
            Assert.AreEqual(500, testAsyncNum(test, timeout).Result);
            NumExpression test2 = test * test;
            Assert.AreEqual(500*500, testAsyncNum(test2, timeout).Result);
            NumExpression test3 = test + test;
            Assert.AreEqual(500 + 500, testAsyncNum(test3, timeout).Result);
        }

        [TestMethod]
        public void TestNumRel()
        {
            NumExpression test500 = new NumConstant(500);
            NumExpression test1000 = new NumConstant(1000);
            NumExpression test2 = test500 + test500;
            LogicExpression testEq = test2 == test1000;
            Assert.AreEqual("True", testAsync(testEq, timeout).Result);
            LogicExpression testNeq = test2 != test1000;
            Assert.AreEqual("Loop", testAsync(testNeq, timeout).Result);
            LogicExpression testLT = test500 < test1000;
            Assert.AreEqual("True", testAsync(testLT, timeout).Result);
            LogicExpression testLTE = test500 <= test1000;
            Assert.AreEqual("True", testAsync(testLTE, timeout).Result);
            LogicExpression testGT = test1000 > test500;
            Assert.AreEqual("True", testAsync(testLT, timeout).Result);
            LogicExpression testGTE = test1000 >= test500;
            Assert.AreEqual("True", testAsync(testLTE, timeout).Result);
        }



        /// <summary>
        /// Tests a LogicExpression through an ExpressionEvaluator.  Returns "True" if the 
        /// expression is true.  If it takes longer than timeout to evaluate it returns "Loop"
        /// </summary>
        /// <param name="Expression">The expression to evaluate</param>
        /// <param name="timeout">How many milliseconds to run for</param>
        /// <returns>The strings "True" or "Loop"</returns>
        private static async Task<string> testAsync(Expression Expression, int timeout)
        {

            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            //var task = Expression.Visit(evaluator);
            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                return (task.Result as BoolValue).Value.ToString();
            }
            return "Loop";
        }

        private static async Task<int> testAsyncNum(Expression Expression, int timeout)
        {

            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            //var task = Expression.Visit(evaluator);
            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                return (task.Result as NumValue).Value;
            }
            return -1;
        }

        /// <summary>
        /// Delay for a given number of milliseconds
        /// </summary>
        /// <typeparam name="T">Phantom type</typeparam>
        /// <param name="timeout">How long to delay in milliseconds</param>
        /// <returns>Task which returns a default value</returns>
        public static async Task<T> Delay<T>(int timeout)
        {
            await Task.Delay(timeout);
            return default(T);
        }
    }
}
