using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

// Hide some of the warnings to do with self comparison
#pragma warning disable 1718

namespace AsyncLogicTest
{
    [TestClass]
    public class TestExpressionEvaluator
    {
        const int defaultTimeout = 500;  // 0.5 seconds
        static LogicExpression logicTrue = new LogicTrue();
        static LogicExpression logicFalse = new LogicFalse();
        static LogicExpression x = new LogicVariable("x");
        static NumExpression zero = new NumConstant(0);
        static NumExpression two = new NumConstant(0);
        static NumExpression n = new NumVariable("n");
        static LogicExpression logicLoop = new NumExists("n", n < n); // this expression loops forever
        const string True = "True";
        const string Loop = "False";
        const string False = Loop;

        [TestMethod]
        public void TestLooping()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Elapsed={0}", sw.Elapsed);
            LogicExpression test1 = logicLoop | logicLoop;
            LogicExpression test2 = logicTrue | logicLoop;
            LogicExpression test3 = logicLoop | logicTrue;
            LogicExpression test4 = logicTrue | logicTrue;
            Assert.AreEqual(Loop, testAsync(test1, 200).Result);
            Assert.AreEqual(True, testAsync(test2, defaultTimeout).Result);
            Assert.AreEqual(True, testAsync(test3, defaultTimeout).Result);
            Assert.AreEqual(True, testAsync(test4, defaultTimeout).Result);

            test1 = logicLoop & logicLoop;
            test2 = logicTrue & logicLoop;
            test3 = logicLoop & logicTrue;
            test4 = logicTrue & logicTrue;
            Assert.AreEqual(Loop, testAsync(test1, 200).Result);
            Assert.AreEqual(Loop, testAsync(test2, 200).Result);
            Assert.AreEqual(Loop, testAsync(test3, 200).Result);
            Assert.AreEqual(True, testAsync(test4, defaultTimeout).Result);

            sw.Stop();
            Assert.IsTrue(sw.ElapsedMilliseconds > 800, "This test should take at least 800ms");
        }

        [TestMethod]
        public void TestAnd()
        {
            LogicExpression test1 = logicFalse & logicFalse;
            LogicExpression test2 = logicTrue  & logicFalse;
            LogicExpression test3 = logicFalse & logicTrue;
            LogicExpression test4 = logicTrue  & logicTrue;
            Assert.AreEqual(False, testAsync(test1, defaultTimeout).Result);
            Assert.AreEqual(False, testAsync(test2, defaultTimeout).Result);
            Assert.AreEqual(False, testAsync(test3, defaultTimeout).Result);
            Assert.AreEqual(True,  testAsync(test4, defaultTimeout).Result);
        }

        [TestMethod]
        public void TestOr()
        {
            LogicExpression test1 = logicFalse | logicFalse;
            LogicExpression test2 = logicTrue  | logicFalse;
            LogicExpression test3 = logicFalse | logicTrue;
            LogicExpression test4 = logicTrue  | logicTrue;
            Assert.AreEqual(False, testAsync(test1, defaultTimeout).Result);
            Assert.AreEqual(True,  testAsync(test2, defaultTimeout).Result);
            Assert.AreEqual(True,  testAsync(test3, defaultTimeout).Result);
            Assert.AreEqual(True,  testAsync(test4, defaultTimeout).Result);
        }

        [TestMethod]
        public void TestNum()
        {
            NumExpression test = new NumConstant(500);
            Assert.AreEqual(500, testAsyncNum(test, defaultTimeout).Result);
            NumExpression test2 = test * test;
            Assert.AreEqual(500 * 500, testAsyncNum(test2, defaultTimeout).Result);
            NumExpression test3 = test + test;
            Assert.AreEqual(500 + 500, testAsyncNum(test3, defaultTimeout).Result);
        }

        [TestMethod]
        public void TestNumRel()
        {
            NumExpression test500 = new NumConstant(500);
            NumExpression test1000 = new NumConstant(1000);
            NumExpression zero = new NumConstant(0);
            NumExpression test2 = test500 + test500;
            LogicExpression testEq = test2 == test1000;
            Assert.AreEqual("True", testAsync(testEq, defaultTimeout).Result);
            LogicExpression testNeq = test2 != test1000;
            Assert.AreEqual("False", testAsync(testNeq, defaultTimeout).Result);
            LogicExpression testLT = test500 < test1000;
            Assert.AreEqual("True", testAsync(testLT, defaultTimeout).Result);
            LogicExpression testLTE = test500 <= test1000;
            Assert.AreEqual("True", testAsync(testLTE, defaultTimeout).Result);
            LogicExpression testGT = test1000 > test500;
            Assert.AreEqual("True", testAsync(testLT, defaultTimeout).Result);
            LogicExpression testGTE = test1000 >= test500;
            Assert.AreEqual("True", testAsync(testLTE, defaultTimeout).Result);
            LogicExpression testZero = zero > zero;
            Assert.AreEqual("False", testAsync(testZero, defaultTimeout).Result, "TestZero");
        }

        [TestMethod]
        public void TestContext()
        {
            Dictionary<string,Value> context = new Dictionary<String,Value> { {"n", new NumValue(2)}};
            NumExpression n = new NumVariable("n");
            Expression test = n == new NumConstant(2);

            Assert.AreEqual(True, testAsync(test, 1000, context).Result);


        }

        [TestMethod]
        public void TestExists()
        {
            NumExpression n = new NumVariable("n");
            var two =  new NumConstant(2);
            var hundred = new NumConstant(100);

            Expression test = new NumExists("n", n == two);
            Assert.AreEqual(True, testAsync(test, 1000).Result);

            Expression test2 = new NumExists("n", n == n + n);
            Assert.AreEqual(True, testAsync(test2, 1000).Result);


            Expression test3 = new NumExists("n", n == n + two);
            Assert.AreEqual(False, testAsync(test3, 1000).Result);

            Expression test4 = new NumExists("n", n == hundred);  // should take 1 sec to find (100 * 10ms)
            Assert.AreEqual(True, testAsync(test4, 2000).Result);

            Expression test5 = new NumExists("n", two < n);
            Assert.AreEqual(True, testAsync(test5, 2000).Result);

            Expression test6 = new NumExists("n", n < n);
            Assert.AreEqual(False, testAsync(test6, 1000).Result, "test6");
        }

        [TestMethod]
        public void TestThe()
        {
            NumExpression n = new NumVariable("n");
            var two =  new NumConstant(2);
            NumExpression test = new NumThe("n", n == two);
            Assert.AreEqual("2", testAsync(test, 1000).Result);

            test = new NumThe("n", n == two + two);
            Assert.AreEqual("4", testAsync(test, 1000).Result);

            test = new NumThe("n", n + n == n * n);  // not valid since it has two results, but we ignore this
            Assert.AreEqual("0", testAsync(test, 1000).Result);

            // Find the positive natural such that 2n=n^2
            test = new NumThe("n", (n + n == n * n) & (n > zero));  
            Assert.AreEqual("2", testAsync(test, 1000).Result);

            // Should fail
            test = new NumThe("n", (n == zero) & (n > zero));
            Assert.AreEqual(False, testAsync(test, 1000).Result);
        }



        /// <summary>
        /// Tests a LogicExpression through an ExpressionEvaluator.  Returns "True" if the 
        /// expression is true.  If it takes longer than timeout to evaluate it returns "False"
        /// </summary>
        /// <param name="Expression">The expression to evaluate</param>
        /// <param name="timeout">How many milliseconds to run for</param>
        /// <returns>The strings "True" or "False"</returns>
        private static async Task<string> testAsync(Expression Expression, int timeout)
        {

            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            //var task = Expression.Visit(evaluator);
            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                if (task.Result is BoolValue)
                    return (task.Result as BoolValue).Value.ToString();
                else
                    return (task.Result as NumValue).Value.ToString();
            }
            return Loop;
        }

        private static async Task<string> testAsync(Expression Expression, int timeout, Dictionary<string,Value> Context)
        {

            ExpressionEvaluator evaluator = new ExpressionEvaluator(Context);
            //var task = Expression.Visit(evaluator);
            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                if (task.Result is BoolValue)
                    return (task.Result as BoolValue).Value.ToString();
                else
                    return (task.Result as NumValue).Value.ToString();
            }
            return "False";
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
