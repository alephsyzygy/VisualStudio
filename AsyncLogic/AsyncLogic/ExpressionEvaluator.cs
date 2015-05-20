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
            await Task.Delay(10);
            throw new NotImplementedException();  // we shouldn't have any free variables
            //return null;
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
            await Task.Delay(10);
            throw new NotImplementedException();  // we shouldn't have any free variables
        }

        public async Task<Value> VisitNumConstant(NumConstant constant)
        {
            return await Task.Run(() => new NumValue(constant.Value)); // get rid of compiler warning
        }

        //public async Task<Value> VisitNumUnaryOp(NumUnaryOp op)
        //{
        //    Value result = await op.Expression.Visit(this);
        //    throw new NotImplementedException();
        //}

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
    }
}
