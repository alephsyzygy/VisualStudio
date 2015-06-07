using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Threading;
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
        static NumExpression n = new NumVariable("n");
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
        public void TestRec()
        {
            var step = n + one;
            var rec = new RecNumExpression(two, zero, "x", "n", step);
            Assert.AreEqual("2", testAsync(rec, 200).Result);

            rec = new RecNumExpression(two, two, "x", "n", step);
            Assert.AreEqual("4", testAsync(rec, 200).Result);

            var logStep = x & logicTrue;
            var logRec = new RecLogicExpression(two, logicTrue, "n", "x", logStep);
            Assert.AreEqual(True, testAsync(logRec, 200).Result);

            logStep = x | logicTrue;
            logRec = new RecLogicExpression(two, logicFalse, "n", "x", logStep);
            Assert.AreEqual(True, testAsync(logRec, 200).Result);

            logStep = x | logicTrue;
            logRec = new RecLogicExpression(zero, logicFalse, "n", "x", logStep);
            Assert.AreEqual(False, testAsync(logRec, 200).Result);
        }

        [TestMethod]
        public void TestSubstitution()
        {

            var subst = new VariableSubstituter("n", two);
            var result = n.Visit(subst);
            Assert.AreEqual("2", testAsync(result, 200).Result);

            result = zero.Visit(subst);
            Assert.AreEqual("0", testAsync(result, 200).Result);

            var exists = new NumExists("n", n == n);
            result = exists.Visit(subst);
            Assert.AreEqual(True, testAsync(result, 200).Result);

            var test = new Apply(new LambdaExpression("n", n), two);
            Assert.AreEqual("2", testAsync(test, 200).Result);

            // test that substitution avoids explosions
            var test2 = new Apply(new LambdaExpression("x", logicTrue), logicLoop);
            Assert.AreEqual(True, testAsync(test2, 200).Result);

            var test3 = new Apply(new LambdaExpression("x", logicLoop), logicTrue);
            Assert.AreEqual(Loop, testAsync(test3, 500).Result);

            var test4 = new Apply(new LambdaExpression("x", x & logicTrue), logicLoop);
            Assert.AreEqual(Loop, testAsync(test4, 500).Result);

            var test5 = new Apply(new LambdaExpression("x", x | logicTrue), logicLoop);
            Assert.AreEqual(True, testAsync(test5, 500).Result);

        }

        [TestMethod]
        public void TestLambdaEval()
        {
            var test1 = new LambdaExpression("x", x);
            Assert.AreEqual("{Lambda x }", testAsync(test1, 200).Result);
        }

        [TestMethod]
        public void TestLambdas()
        {
            SortedSet<string> variables;
            var test = new LambdaExpression("x", x);
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            Assert.AreEqual(1, lister.Variables.Count);
            Assert.IsTrue(lister.Variables.Contains("x"));

            test = new LambdaExpression("x", x);
            FreeVariableLister freeLister = new FreeVariableLister();
            variables = test.Visit(freeLister);
            Assert.AreEqual(0, variables.Count);

            test = new LambdaExpression("x", logicLoop);
            freeLister = new FreeVariableLister();
            variables = test.Visit(freeLister);
            Assert.AreEqual(0, variables.Count);
        }

        [TestMethod]
        public void TestPairs()
        {
            NumValue valueTwo = new NumValue(2);
            Value pair = new PairValue<NumValue, NumValue>(valueTwo, valueTwo);
            Assert.AreEqual("< 2 , 2 >", pair.ToString());

            Expression test = new PairExpression(two, two);
            Assert.AreEqual("< 2 , 2 >", testAsync(test, 500).Result);

            var trueTest = new PairExpression(logicTrue, logicTrue);
            Assert.AreEqual("< True , True >", testAsync(trueTest, 500).Result);

            var loopTest = new PairExpression(logicLoop, logicTrue);
            Assert.AreEqual(False, testAsync(loopTest, 500).Result);

            var pairTest = new ProjL(trueTest);
            Assert.AreEqual(True, testAsync(pairTest, 500).Result);

            var pairLoop = new ProjR(loopTest);
            Assert.AreEqual(True, testAsync(pairLoop, 500).Result);

            var pairLoop2 = new ProjL(loopTest);
            Assert.AreEqual(Loop, testAsync(pairLoop2, 500).Result);
        }

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
            Assert.IsTrue(sw.ElapsedMilliseconds > 700, "This test should take at least 800ms");
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
            test = new NumThe("n", (logicLoop | (n + n == n * n) & (n > zero)) | logicLoop);  
            Assert.AreEqual("2", testAsync(test, 1000).Result);

            // Should fail
            test = new NumThe("n", (n == zero) & (n > zero));
            Assert.AreEqual(False, testAsync(test, 1000).Result);

            test = new NumThe("n", (logicLoop | n == new NumConstant(50) | logicLoop));
            Assert.AreEqual("50", testAsync(test, 1000).Result);

            // Find the positive natural such that 2n=n^2 this time with logicFalse
            test = new NumThe("n", ((n + n == n * n) & (n > zero)) | logicFalse);
            Assert.AreEqual("2", testAsync(test, 1000).Result);

            test = new NumThe("n", (n == new NumConstant(50) | logicFalse));
            Assert.AreEqual("50", testAsync(test, 1000).Result);
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
            CancellationTokenSource source = new CancellationTokenSource();
            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            evaluator.CancelToken = source.Token;
            
            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                //var result = task.Result.ToStringAsync();
                
                var result = task.Result.Normalize();
                if (await Task<Value>.WhenAny<Value>(result, Delay<Value>(timeout)) == result)
                {
                    source.Cancel();
                    return result.Result.ToString();
                }
                else
                {
                    source.Cancel();
                    return Loop;
                }

            }
            source.Cancel();
            return Loop;
        }

        private static async Task<string> testAsync(Expression Expression, int timeout, Dictionary<string,Value> Context)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            ExpressionEvaluator evaluator = new ExpressionEvaluator(Context);
            evaluator.CancelToken = source.Token;

            var task = evaluator.Run(Expression);

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                source.Cancel();
                return task.Result.ToString();
            }

            source.Cancel();
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
