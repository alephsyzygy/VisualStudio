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
    public class FreeVariableLister<T> : IExpressionVisitor<T>
    {
        /// <summary>
        /// The SortedSet of the variables
        /// </summary>
        public SortedSet<string> Variables;

        /// <summary>
        /// Constructor
        /// </summary>
        public FreeVariableLister()
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
            // Since this is a quantifier it binds a variable, which is no longer free. 
            // Note: find the free variables before removing them!
            expression.Expression.Visit(this);
            this.Variables.Remove(expression.VariableName);
            return default(T);
        }


        public T VisitNumThe(NumThe expression)
        {
            // 'the' is a binder, so it binds its variable
            expression.Expression.Visit(this);
            this.Variables.Remove(expression.VariableName);
            return default(T);
        }


        public T VisitPair<A, B>(PairExpression<A, B> expression)
            where A : Expression
            where B : Expression
        {
            expression.Left.Visit(this);
            expression.Right.Visit(this);
            return default(T);
        }


        public T VisitLeft<A, B>(ProjL<A, B> expression)
            where A : Expression
            where B : Expression
        {
            expression.Expression.Visit(this);
            return default(T);
        }

        public T VisitRight<A, B>(ProjR<A, B> expression)
            where A : Expression
            where B : Expression
        {
            expression.Expression.Visit(this);
            return default(T);
        }
    }
}
