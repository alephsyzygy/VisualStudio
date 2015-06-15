using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Expressions
{
    public abstract class AbstractLambdaExpression : Expression
    { }

    /// <summary>
    /// A Lambda expression from a type to the type Sigma of logic values
    /// Note that the return type of a lambda is restricted to logical values only.
    /// </summary>
    /// <typeparam name="A">The type of the input</typeparam>
    public class LambdaExpression : AbstractLambdaExpression
    {
        public string VariableName;
        public Expression Expression;

        public LambdaExpression(string VariableName, Expression Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }


}
