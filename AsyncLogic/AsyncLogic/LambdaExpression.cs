using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public abstract class AbstractLambdaExpression<A> : Expression
        where A : Expression
    { }

    /// <summary>
    /// A Lambda expression from a type to the type Sigma of logic values
    /// Note that the return type of a lambda is restricted to logical values only.
    /// </summary>
    /// <typeparam name="A">The type of the input</typeparam>
    public class LambdaExpression<A> : AbstractLambdaExpression<A>
        where A : Expression
    {
        public string VariableName;
        public A Expression;

        public LambdaExpression(string VariableName, A Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLambda(this);
        }
    }

    public class LambdaVariable<A> : AbstractLambdaExpression<A>
        where A : Expression
    {
        public string VariableName;

        public LambdaVariable(string VariableName)
        {
            this.VariableName = VariableName;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLambdaVariable(this);
        }
    }
}
