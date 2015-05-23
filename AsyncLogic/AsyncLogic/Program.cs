using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    class Program
    {
        const int timeout = 2000;  // 2 seconds
        static LogicExpression logicTrue = new LogicTrue();
        static LogicExpression logicFalse = new LogicFalse();
        static LogicExpression x = new LogicVariable("x");

        static void Main(string[] args)
        {
            LogicExpression test = x & x;
            VariableLister<bool> lister = new VariableLister<bool>();
            test.Visit(lister);
            foreach (var variable in lister.Variables)
            {
                Console.WriteLine("Variable: {0}", variable);
            }

            LogicExpression test2;
            test2 = logicTrue | logicFalse;
            Console.WriteLine("Result: {0}", testAsync(test2, timeout).Result);

            test2 = logicTrue & logicFalse;
            Console.WriteLine("Result: {0}", testAsync(test2, timeout).Result);
            
            

            Console.Read();
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

            if (await Task<Value>.WhenAny<Value>(task, Delay<Value>(timeout)) == task)
            {
                return (task.Result as BoolValue).Value.ToString();
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
