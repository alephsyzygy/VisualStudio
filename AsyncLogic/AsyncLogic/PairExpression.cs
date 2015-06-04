using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public abstract class AbstractPairExpression : Expression
    { }

    /// <summary>
    /// Represents a pairing expression
    /// </summary>
    /// <typeparam name="A">The left type of a pair</typeparam>
    /// <typeparam name="B">The right type of a pair</typeparam>
    public class PairExpression : AbstractPairExpression
    {
        public Expression Left;
        public Expression Right;

        public PairExpression(Expression Left, Expression Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitPair(this);
        }
    }


    public class ProjL : Expression
    {
        public PairExpression Expression;

        public ProjL(PairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLeft(this);
        }
    }

    public class ProjR : Expression
    {
        public PairExpression Expression;

        public ProjR(PairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitRight(this);
        }
    }

}
