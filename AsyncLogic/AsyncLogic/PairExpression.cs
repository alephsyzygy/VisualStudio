using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// Represents a pairing expression
    /// </summary>
    /// <typeparam name="A">The left type of a pair</typeparam>
    /// <typeparam name="B">The right type of a pair</typeparam>
    public class PairExpression<A,B> : Expression where A: Expression 
                                                  where B: Expression
    {
        public A Left;
        public B Right;

        public PairExpression(A Left, B Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitPair<A,B>(this);
        }
    }

    public class ProjL<A,B> : Expression
        where A : Expression
        where B : Expression
    {
        public PairExpression<A, B> Expression;

        public ProjL(PairExpression<A,B> Expression)
        {
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLeft(this);
        }
    }

    public class ProjR<A, B> : Expression
        where A : Expression
        where B : Expression
    {
        public PairExpression<A, B> Expression;

        public ProjR(PairExpression<A, B> Expression)
        {
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitRight(this);
        }
    }

}
