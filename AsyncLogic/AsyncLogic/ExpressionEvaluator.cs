using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 1998

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
        /// A CancellationToken so that this evaluator can halt evaluation
        /// </summary>
        public CancellationToken CancelToken;

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
            //return await Loop<Value>();
            // Change: we know that this is false, instead of looping just return false
            //  if we want to loop then the expression exists(n) (n != n) will loop
            return await Task.Run(() => new BoolValue(false)); // get rid of compiler warning
        }

        public async Task<Value> VisitAnd(LogicAnd op)
        {
            Value[] results = await Task.WhenAll<Value>(op.Left.Visit(this), op.Right.Visit(this));
            return new BoolValue(results.All(b => ((BoolValue)b).Value));
        }

        public async Task<Value> VisitOr(LogicOr op)
        {
            Task<Value> left = op.Left.Visit(this);
            Task<Value> right = op.Right.Visit(this);
            Task<Value> resultTask = await Task.WhenAny<Value>(left,right);
            BoolValue result = (BoolValue)await resultTask;
            if (result.Value)
            {
                // The value is true, so return it
                return result;
            }
            else
            {
                // The value is false, so the return value is given by the other task
                if (resultTask == left)
                    return await right;
                else
                    return await left;                
            }
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
                await Task.Delay(500,CancelToken);
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
            // Have a list of cancelTokens so that we can cancel once we have succeeded
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Store a mapping of tasks to their values
            Dictionary<Task<Value>,int> taskDictionary = new Dictionary<Task<Value>,int>();
              

            while (!foundValue)
            {
                // See if we have been canceled.  If so cancel everything
                if (CancelToken != null && CancelToken.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }
                
                // Clone the context
                Dictionary<string,Value> newContext = (from x in Context
                                                       select x).ToDictionary(x => x.Key, x => x.Value);
                newContext[expression.VariableName] = new NumValue(nextNum);
                // Construct a new evaluator with an updated context
                ExpressionEvaluator nextEvaluator = new ExpressionEvaluator(newContext);
                nextEvaluator.CancelToken = tokenSource.Token;

                // Add the new evaluator to our list of tasks
                Task<Value> newTask = expression.Expression.Visit(nextEvaluator);
                runningTasks.Add(newTask);
                taskDictionary[newTask] = nextNum;


                // create a new delay task to add to the list
                Task<Value> delay = Delay<Value>(ExistsTimeout,CancelToken);
                runningTasks.Add(delay);
                

                // Run the tasks until one completes
                Task<Value> resultTask = await Task.WhenAny(runningTasks);

                // If it is not the delay task we have a result, so check to see if it is true
                if (resultTask != delay)
                {
                    Value result = await resultTask;
                    // Remove the tasks from the running
                    runningTasks.Remove(resultTask);
                    if (result is BoolValue)
                    {
                        if (((BoolValue)result).Value)
                        {
                            // We have a true result.
                            tokenSource.Cancel();  // cancel all tasks which were spawned
                            return result;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("In exists the predicate should evaluate to a BoolValue");
                    }
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

        public static async Task<T> Delay<T>(int timeout, CancellationToken token)
        {
            await Task.Delay(timeout, token);
            return default(T);
        }


        public async Task<Value> VisitNumThe(NumThe expression)
        {
            // The idea here is similar to VisitNumExists, except this time we remember
            // which number we found.

            // The number we are currently up to
            int nextNum = 0;
            bool foundValue = false;
            List<Task<Value>> runningTasks = new List<Task<Value>>();
            // Have a list of cancelTokens so that we can cancel once we have succeeded
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            // Store a mapping of tasks to their values
            Dictionary<Task<Value>, int> taskDictionary = new Dictionary<Task<Value>, int>();

            while (!foundValue)
            {
                // See if we have been canceled.  If so cancel everything
                if (CancelToken != null && CancelToken.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }
                // Clone the context
                Dictionary<string, Value> newContext = (from x in Context
                                                        select x).ToDictionary(x => x.Key, x => x.Value);
                newContext[expression.VariableName] = new NumValue(nextNum);
                // Construct a new evaluator with an updated context
                ExpressionEvaluator nextEvaluator = new ExpressionEvaluator(newContext);
                nextEvaluator.CancelToken = tokenSource.Token;

                // Add the new evaluator to our list of tasks
                Task<Value> newTask = expression.Expression.Visit(nextEvaluator);
                runningTasks.Add(newTask);
                taskDictionary[newTask] = nextNum;

                // create a new delay task to add to the list
                Task<Value> delay = Delay<Value>(ExistsTimeout,CancelToken);
                runningTasks.Add(delay);

                // Run the tasks until one completes
                Task<Value> resultTask = await Task.WhenAny(runningTasks);

                // If it is not the delay task we have a true result, so return it
                if (resultTask != delay)
                {
                    Value resultValue = await resultTask;
                    runningTasks.Remove(resultTask);
                    if (resultValue is BoolValue)
                        if (((BoolValue)resultValue).Value)
                        {
                            // cancel all running tasks
                            tokenSource.Cancel();
                            return new NumValue(taskDictionary[resultTask]);
                        }
                        else
                        {
                            // expression is false, so remove from dictionary
                            taskDictionary.Remove(resultTask);
                        }

                    else
                        throw new ArgumentException("For 'the' the predicate should have a logic value");
                }
                else  // delay has fired, so remove it and loop to the next number
                {
                    runningTasks.Remove(delay);
                    nextNum++;
                }
            }


            throw new NotImplementedException("This code should not be reached");
        }


        public async Task<Value> VisitPair(PairExpression expression)
        {
            // Don't actually do these, let someone else do them
            var left = expression.Left.Visit(this);
            var right = expression.Right.Visit(this);

            return new PotentialPairValue<Value,Value>(left, right);
        }


        public async Task<Value> VisitLeft(ProjL expression)
        {
            // idea here is to extract the PairValue then return its left entry.
            var value = await expression.Expression.Visit(this);
            if (value is PotentialPairValue<Value, Value>)
                return await (value as PotentialPairValue<Value, Value>).Left;
            else
                throw new ArgumentException("Not a pair value");
        }

        public async Task<Value> VisitRight(ProjR expression)
        {
            var value = await expression.Expression.Visit(this);
            if (value is PotentialPairValue<Value, Value>)
                return await (value as PotentialPairValue<Value, Value>).Right;
            else
                throw new ArgumentException("Not a pair value");
        }


        public async Task<Value> VisitLambda(LambdaExpression lambda) 
        {
            // can't really do much here
            // do we normalise it, or just leave it as is?
            return new LambdaValue(lambda.VariableName, lambda.Expression);
        }


        public async Task<Value> VisitApply(Apply apply)
        {
            // Find out what the lambda is, then evaluate it with the given expression
            // Do we do a syntactic substitution?

            // evaluate our lambda until we get an LambdaValue.  Then we substitute and continue.
            var lambda = await apply.Lambda.Visit(this);
            if (lambda is LambdaValue)
            {
                LambdaValue val = (LambdaValue)lambda;
                VariableSubstituter subst = new VariableSubstituter(val.VariableName, apply.Expression);
                var newExpr = subst.Substitute(val.Expression);
                return await newExpr.Visit(this);
            }
            else
            {
                throw new ArgumentException("Value cannot be evaluated to a Lambda");
            }
            //VariableSubstituter subst = new VariableSubstituter(apply.va)
            throw new NotImplementedException();
        }


        public async Task<Value> VisitPairVariable(PairVariable variable)
        {
            Value result = Context[variable.VariableName];
            if (result == null)
            {
                await Task.Delay(10);
                throw new ArgumentException("Variable not found in context");
            }
            return result;
        }


        public async Task<Value> VisitLambdaVariable(LambdaVariable variable)
        {
            Value result = Context[variable.VariableName];
            if (result == null)
            {
                await Task.Delay(10);
                throw new ArgumentException("Variable not found in context");
            }
            return result;
        }


        //public async Task<Value> VisitRecNum(RecNumExpression rec)
        //{
        //    // A rec has an input, a start, and a step, which itself has two free vars
        //    // The idea is to find out what value the input is.
        //    // if it is zero we return the start
        //    // otherwise we evaluate the rec with an input one less, then put this through the step

        //    Value input = await Run(rec.Input);  // we need this
        //    if (input is NumValue)
        //    {
        //        NumValue num = input as NumValue;
        //        if (num.Value == 0)
        //            return await Run(rec.Start);
        //        else
        //        {
        //            // this looks bad.  Fix it
        //            NumValue newNum = new NumValue(num.Value - 1);
        //            VariableSubstituter substNum = new VariableSubstituter(rec.NumVariableName, new NumConstant(num.Value));
        //            Expression newRec = new RecNumExpression(new NumConstant(num.Value - 1), rec.Start, rec.NumVariableName,
        //                rec.AccVariableName, rec.Step);
        //            VariableSubstituter substAcc = new VariableSubstituter(rec.AccVariableName, newRec);
        //            Expression temp = substNum.Substitute(rec.Step);
        //            Expression temp2 = substAcc.Substitute(temp);
        //            return await Run(temp2);
        //        }
        //    }
        //    else
        //        throw new ArgumentException("In rec the input should evaluate to a Num");

        //    throw new NotImplementedException();
        //}


        public async Task<Value> VisitRec<A>(IRecExpression<A> rec) where A : Expression
        {
            // A rec has an input, a start, and a step, which itself has two free vars
            // The idea is to find out what value the input is.
            // if it is zero we return the start
            // otherwise we evaluate the rec with an input one less, then put this through the step

            Value input = await Run(rec.Input);  // we need this
            if (input is NumValue)
            {
                NumValue num = input as NumValue;
                if (num.Value == 0)
                    return await Run(rec.Start);
                else
                {
                    // this looks bad.  Fix it
                    NumValue newNum = new NumValue(num.Value - 1);
                    VariableSubstituter substNum = new VariableSubstituter(rec.NumVariableName, new NumConstant(num.Value));
                    Expression newRec = rec.Construct(new NumConstant(num.Value - 1), rec.Start, rec.NumVariableName,
                        rec.AccVariableName, rec.Step);
                    VariableSubstituter substAcc = new VariableSubstituter(rec.AccVariableName, newRec);
                    Expression temp = substNum.Substitute(rec.Step);
                    Expression temp2 = substAcc.Substitute(temp);
                    return await Run(temp2);
                }
            }
            else
                throw new ArgumentException("In rec the input should evaluate to a Num");

            throw new NotImplementedException();
        }
    }
}
