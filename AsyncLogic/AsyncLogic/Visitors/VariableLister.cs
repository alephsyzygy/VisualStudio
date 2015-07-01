using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;

namespace AsyncLogic.Visitors
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
            op.Left.Accept(this);
            op.Right.Accept(this);
            return default(T);
        }

        public T Visit(LogicOr op)
        {
            op.Left.Accept(this);
            op.Right.Accept(this);
            return default(T);
        }


        public T Visit(NumRelation relation)
        {
            relation.Left.Accept(this);
            relation.Right.Accept(this);
            return default(T);
        }

        public T Visit(NumConstant constant)
        {
            return default(T);
        }

        public T Visit(NumBinaryOp op)
        {
            op.Left.Accept(this);
            op.Right.Accept(this);
            return default(T);
        }


        public T Visit(NumExists expression)
        {
            expression.Accept(this);
            return default(T);
        }


        public T Visit(NumThe expression)
        {
            expression.Expression.Accept(this);
            return default(T);
        }





        public T Visit(PairExpression expression)
        {
            expression.Left.Accept(this);
            expression.Right.Accept(this);
            return default(T);
        }


        public T Visit<A>(IProjL<A> expression) where A : Expression
        {
            expression.Expression.Accept(this);
            return default(T);
        }

        public T Visit<A>(IProjR<A> expression) where A : Expression
        {
            expression.Expression.Accept(this);
            return default(T);
        }


        public T Visit(LambdaExpression lambda) 
        {
            lambda.Expression.Accept(this);
            return default(T);
        }


        public T Visit(Apply apply) 
        {
            apply.Lambda.Accept(this);
            apply.Expression.Accept(this);
            return default(T);
        }


        public T Visit<A>(IRecExpression<A> rec) where A : Expression
        {
            rec.Input.Accept(this);
            rec.Start.Accept(this);
            rec.Step.Accept(this);
            return default(T);
        }


        public T Visit<A>(IVariableExpression<A> variable) where A : Expression
        {
            // when we visit a variable we add it to the set
            this.Variables.Add(variable.VariableName);
            return default(T);
        }
    }
}
