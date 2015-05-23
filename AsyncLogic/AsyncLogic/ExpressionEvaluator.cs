using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// A visitor to evaluate a LogicExpression.
    /// This expression should have no free variables (i.e. be closed).
    /// The result type is a Task which returns true if the expression is true,
    /// otherwise it loops forever.
    /// </summary>
    public class ExpressionEvaluator : IExpressionVisitor<Task<Value>>
    {
        /// <summary>
        /// The context which maps variables to values
        /// </summary>
        public Dictionary<string, Value> Context;

        /// <summary>
        /// How often to spawn new tasks when using exists
        /// </summary>
        private int ExistsTimeout = 10;

        /// <summary>
        /// Construct an ExpressionEvaluator with the given Context
        /// </summary>
        /// <param name="Context">A mapping from variables to values</param>
        public ExpressionEvaluator(Dictionary<string,Value> Context)
        {
            this.Context = Context;
        }

        public ExpressionEvaluator()
        {
            this.Context = new Dictionary<string, Value>();
        }

        /// <summary>
        /// Run this visitor on the given expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>The task which performs the evaluation</returns>
        public async Task<Value> Run(Expression expr)
        {
            return await expr.Visit(this);
        }
        public async Task<Value> VisitLogicVariable(LogicVariable variable)
        {
            Value result = Context[variable.VariableName];
            if (result == null)
            {
                await Task.Delay(10);
                throw new ArgumentException("Variable not found in context");
            }
            return result;
        }

        public async Task<Value> VisitTrue(LogicTrue constant)
        {
            return await Task.Run(() => new BoolValue(true)); // get rid of compiler warning
        }

        public async Task<Value> VisitFalse(LogicFalse constant)
        {
            return await Loop<Value>();
        }

        public async Task<Value> VisitAnd(LogicAnd op)
        {
            Value[] results = await Task.WhenAll<Value>(op.Left.Visit(this), op.Right.Visit(this));
            return new BoolValue(results.All(b => ((BoolValue)b).Value));
        }

        public async Task<Value> VisitOr(LogicOr op)
        {
            Task<Value> result = await Task.WhenAny<Value>(op.Left.Visit(this), op.Right.Visit(this));
            return await result;
        }

        /// <summary>
        /// Loop forever.  WARNING: this does not terminate.
        /// </summary>
        /// <returns>A Task that runs forever</returns>
        public async Task<T> Loop<T>()
        {     
            while (true) 
            {
                // loop forever
                await Task.Delay(500);
            }  
        }




        public async Task<Value> VisitNumRel(NumRelation relation)
        {
            Value[] results = await Task.WhenAll<Value>(relation.Left.Visit(this), relation.Right.Visit(this));
            switch (relation.Relation)
            {
                case NumRels.GT:
                    return await MakeBool((results[0] as NumValue) > (results[1] as NumValue));
                case NumRels.LT:
                    return await MakeBool((results[0] as NumValue) < (results[1] as NumValue));
                case NumRels.EQ:
                    return await MakeBool((results[0] as NumValue) == (results[1] as NumValue));
                case NumRels.NEQ:
                    return await MakeBool((results[0] as NumValue) != (results[1] as NumValue));
                case NumRels.GTE:
                    return await MakeBool((results[0] as NumValue) >= (results[1] as NumValue));
                case NumRels.LTE:
                    return await MakeBool((results[0] as NumValue) <= (results[1] as NumValue));
                default:
                    throw new ArgumentException();
            }
            
            
        }

        /// <summary>
        /// This takes a bool and turns into a Task which loops if the input is false
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<Value> MakeBool(bool input)
        {
            if (input)
                return new BoolValue(input);
            else
                return await Loop<Value>();
        }

        public async Task<Value> VisitNumVariable(NumVariable variable)
        {
            Value result = Context[variable.VariableName];
            if (result == null)
            {
                await Task.Delay(10);
                throw new ArgumentException("Variable not found in context");
            }
            return result;
        }

        public async Task<Value> VisitNumConstant(NumConstant constant)
        {
            return await Task.Run(() => new NumValue(constant.Value)); // get rid of compiler warning
        }

        public async Task<Value> VisitNumBinaryOp(NumBinaryOp op)
        {
            Value[] results = await Task.WhenAll<Value>(op.Left.Visit(this), op.Right.Visit(this));
            switch (op.Operation)
            {
                case NumBinOps.Add:
                    return (results[0] as NumValue) + (results[1] as NumValue);
                case NumBinOps.Mul:
                    return (results[0] as NumValue) * (results[1] as NumValue);
                default:
                    throw new ArgumentException();
            }
            throw new NotImplementedException();
        }


        public async Task<Value> VisitNumExists(NumExists expression)
        {
            // The idea here is to continually spawn off tasks evaluating the expression
            // in the environment where the variable is bound to first 0, then 1, then 2, ...
            // When one of these completes we have true.  Otherwise we loop forever.

            // The number we are currently up to
            int nextNum = 0;
            bool foundValue = false;
            List<Task<Value>> runningTasks = new List<Task<Value>>();
            // The first task is the delay task.  

            while (!foundValue)
            {
                
                // Clone the context
                Dictionary<string,Value> newContext = (from x in Context
                                                       select x).ToDictionary(x => x.Key, x => x.Value);
                newContext[expression.VariableName] = new NumValue(nextNum);
                // Construct a new evaluator with an updated context
                ExpressionEvaluator nextEvaluator = new ExpressionEvaluator(newContext);

                // Add the new evaluator to our list of tasks
                Task<Value> newTask = expression.Expression.Visit(nextEvaluator);
                runningTasks.Add(newTask);

                // create a new delay task to add to the list
                Task<Value> delay = Delay<Value>(ExistsTimeout);
                runningTasks.Add(delay);

                // Run the tasks until one completes
                Task<Value> resultTask = await Task.WhenAny(runningTasks);

                // If it is not the delay task we have a true result, so return it
                if (resultTask != delay)
                {
                    foundValue = true;
                    return await resultTask;
                }
                else  // delay has fired, so remove it and loop to the next number
                {
                    runningTasks.Remove(delay);
                    nextNum++;
                }              
            }

            throw new NotImplementedException("This code should not be reached");
        }

        public static async Task<T> Delay<T>(int timeout)
        {
            await Task.Delay(timeout);
            return default(T);
        }
    }
}
