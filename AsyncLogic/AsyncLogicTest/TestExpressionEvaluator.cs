using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic;
using System.Threading.Tasks;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestExpressionEvaluator
    {
        const int timeout = 2000;  // 2 seconds
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



        /// <summary>
        /// Tests a LogicExpression through an ExpressionEvaluator.  Returns "True" if the 
        /// expression is true.  If it takes longer than timeout to evaluate it returns "Loop"
        /// </summary>
        /// <param name="Expression">The expression to evaluate</param>
        /// <param name="timeout">How many milliseconds to run for</param>
        /// <returns>The strings "True" or "Loop"</returns>
        private static async Task<string> testAsync(LogicExpression Expression, int timeout)
        {

            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            //var task = Expression.Visit(evaluator);
            var task = evaluator.Run(Expression);

            if (await Task<bool>.WhenAny<bool>(task, Delay<bool>(timeout)) == task)
            {
                return task.Result.ToString();
            }
            return "Loop";
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
