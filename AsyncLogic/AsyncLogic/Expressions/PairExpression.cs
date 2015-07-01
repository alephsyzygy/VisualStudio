using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Expressions
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

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "<" + Left.ToString() + ", " + Right.ToString() + ">";
        }
    }

    public interface IProjL<T> where T : Expression
    {
        AbstractPairExpression Expression { get; }
        T Construct(AbstractPairExpression Expression);
    }

    public interface IProjR<T> where T : Expression
    {
        AbstractPairExpression Expression { get; }
        T Construct(AbstractPairExpression Expression);
    }

    public class NumProjL : NumExpression, IProjL<NumExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public NumProjL(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Fst " + Expression.ToString();
        }

        public NumExpression Construct(AbstractPairExpression Expression)
        {
            return new NumProjL(Expression);
        }
    }

    public class NumProjR : NumExpression, IProjR<NumExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public NumProjR(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Snd " + Expression.ToString();
        }

        public NumExpression Construct(AbstractPairExpression Expression)
        {
            return new NumProjR(Expression);
        }
    }

    // Logic

    public class LogicProjL : LogicExpression, IProjL<LogicExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public LogicProjL(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Fst " + Expression.ToString();
        }

        public LogicExpression Construct(AbstractPairExpression Expression)
        {
            return new LogicProjL(Expression);
        }
    }

    public class LogicProjR : LogicExpression, IProjR<LogicExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public LogicProjR(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Snd " + Expression.ToString();
        }

        public LogicExpression Construct(AbstractPairExpression Expression)
        {
            return new LogicProjR(Expression);
        }
    }

    // Pair

    public class PairProjL : AbstractPairExpression, IProjL<AbstractPairExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public PairProjL(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Fst " + Expression.ToString();
        }

        public AbstractPairExpression Construct(AbstractPairExpression Expression)
        {
            return new PairProjL(Expression);
        }
    }

    public class PairProjR : AbstractPairExpression, IProjR<AbstractPairExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public PairProjR(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Snd " + Expression.ToString();
        }

        public AbstractPairExpression Construct(AbstractPairExpression Expression)
        {
            return new PairProjR(Expression);
        }
    }

    // Lambda

    public class LambdaProjL : AbstractLambdaExpression, IProjL<AbstractLambdaExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public LambdaProjL(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Fst " + Expression.ToString();
        }

        public AbstractLambdaExpression Construct(AbstractPairExpression Expression)
        {
            return new LambdaProjL(Expression);
        }
    }

    public class LambdaProjR : AbstractLambdaExpression, IProjR<AbstractLambdaExpression>
    {
        public AbstractPairExpression Expression { get; private set; }

        public LambdaProjR(AbstractPairExpression Expression)
        {
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Snd " + Expression.ToString();
        }

        public AbstractLambdaExpression Construct(AbstractPairExpression Expression)
        {
            return new LambdaProjR(Expression);
        }
    }
}
