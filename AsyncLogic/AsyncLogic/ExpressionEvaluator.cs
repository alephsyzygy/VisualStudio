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
    public class ExpressionEvaluator : ILogicVisitor<Task<bool>>
    {
        /// <summary>
        /// Run this visitor on the given expression
        /// </summary>
        /// <param name="expr">The expression</param>
        /// <returns>The task which performs the evaluation</returns>
        public async Task<bool> Run(LogicExpression expr)
        {
            return await expr.Visit(this);
        }
        public async Task<bool> VisitLogicVariable(LogicVariable variable)
        {
            await Task.Delay(10);
            throw new NotImplementedException();  // we shouldn't have any free variables
            //return null;
        }

        public async Task<bool> VisitTrue(LogicTrue constant)
        {
            return await Task.Run(() => true); // get rid of compiler warning
        }

        public async Task<bool> VisitFalse(LogicFalse constant)
        {
            return await Loop<bool>();
        }

        public async Task<bool> VisitAnd(LogicAnd op)
        {
            bool[] results = await Task.WhenAll<bool>(op.Left.Visit(this), op.Right.Visit(this));
            return results.All(b => b);
        }

        public async Task<bool> VisitOr(LogicOr op)
        {
            Task<bool> result = await Task.WhenAny<bool>(op.Left.Visit(this), op.Right.Visit(this));
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

        
    }
}
