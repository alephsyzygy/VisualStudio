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

        public T Visit(LogicTrue constant)
        {
            return default(T);
        }

        public T Visit(LogicFalse constant)
        {
            return default(T);
        }

        public T Visit(LogicAnd op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }

        public T Visit(LogicOr op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }


        public T Visit(NumRelation relation)
        {
            relation.Left.Visit(this);
            relation.Right.Visit(this);
            return default(T);
        }

        public T Visit(NumConstant constant)
        {
            return default(T);
        }

        public T Visit(NumBinaryOp op)
        {
            op.Left.Visit(this);
            op.Right.Visit(this);
            return default(T);
        }


        public T Visit(NumExists expression)
        {
            expression.Visit(this);
            return default(T);
        }


        public T Visit(NumThe expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }





        public T Visit(PairExpression expression)
        {
            expression.Left.Visit(this);
            expression.Right.Visit(this);
            return default(T);
        }


        public T Visit(ProjL expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }

        public T Visit(ProjR expression)
        {
            expression.Expression.Visit(this);
            return default(T);
        }


        public T Visit(LambdaExpression lambda) 
        {
            lambda.Expression.Visit(this);
            return default(T);
        }


        public T Visit(Apply apply) 
        {
            apply.Lambda.Visit(this);
            apply.Expression.Visit(this);
            return default(T);
        }


        public T VisitRec<A>(IRecExpression<A> rec) where A : Expression
        {
            rec.Input.Visit(this);
            rec.Start.Visit(this);
            rec.Step.Visit(this);
            return default(T);
        }


        public T VisitVariable<A>(IVariableExpression<A> variable) where A : Expression
        {
            // when we visit a variable we add it to the set
            this.Variables.Add(variable.VariableName);
            return default(T);
        }
    }
}
