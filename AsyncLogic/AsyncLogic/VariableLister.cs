using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// A visitor object to construct the set of all variables
    /// </summary>
    /// <typeparam name="T">Phantom type, not used</typeparam>
    public class VariableLister<T> : IExpressionVisitor<T>
    {
        /// <summary>
        /// The SortedSet of the variables
        /// </summary>
        public SortedSet<string> Variables;

        /// <summary>
        /// Constructor
        /// </summary>
        public VariableLister()
        {
            Variables = new SortedSet<string>();
        }

        public T VisitLogicVariable(LogicVariable variable)
        {
            // when we visit a variable we add it to the set
            this.Variables.Add(variable.VariableName);
            return default(T);
        }

        public T VisitTrue(LogicTrue constant)
        {
            return default(T);
        }

        public T VisitFalse(LogicFalse constant)
        {
            return default(T);
        }

        public T VisitAnd(LogicAnd op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }

        public T VisitOr(LogicOr op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }


        public T VisitNumRel(NumRelation relation)
        {
            relation.Left.Visit(this);
            relation.Right.Visit(this);
            return default(T);
        }

        public T VisitNumVariable(NumVariable variable)
        {
            // when we visit a variable we add it to the set
            this.Variables.Add(variable.VariableName);
            return default(T);
        }

        public T VisitNumConstant(NumConstant constant)
        {
            return default(T);
        }

        public T VisitNumBinaryOp(NumBinaryOp op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }


        public T VisitNumExists(NumExists expression)
        {
            expression.Visit(this);
            return default(T);
        }


        public T VisitNumThe(NumThe expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }





        public T VisitPair(PairExpression expression)
        {
            expression.Left.Visit(this);
            expression.Right.Visit(this);
            return default(T);
        }


        public T VisitLeft(ProjL expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }

        public T VisitRight(ProjR expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }


        public T VisitLambda(LambdaExpression lambda) 
        {
            lambda.Expression.Visit(this);
            return default(T);
        }


        public T VisitApply(Apply apply) 
        {
            apply.Lambda.Visit(this);
            apply.Expression.Visit(this);
            return default(T);
        }


        public T VisitPairVariable(PairVariable variable)
        {
            this.Variables.Add(variable.VariableName);
            return default(T);
        }


        public T VisitLambdaVariable(LambdaVariable variable) 
        {
            this.Variables.Add(variable.VariableName);
            return default(T);
        }

    }
}
