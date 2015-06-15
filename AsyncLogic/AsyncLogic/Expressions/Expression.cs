using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Expressions
{
    public abstract class Expression
    {
        public abstract T Accept<T>(IExpressionVisitor<T> visitor);

        public Type Type { get; protected set; }
    }

    /// <summary>
    /// Visitor interface to visit an Expression
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    public interface IExpressionVisitor<out T>
    {
        T Visit(LogicTrue constant);
        T Visit(LogicFalse constant);
        T Visit(LogicAnd op);
        T Visit(LogicOr op);
        T Visit(NumConstant constant);
        T Visit(NumBinaryOp op);
        T Visit(NumRelation relation);
        T Visit(NumExists expression);
        T Visit(NumThe expression);
        T Visit(PairExpression expression);
        T Visit(ProjL pair);
        T Visit(ProjR pair);
        T Visit(LambdaExpression lambda);
        T Visit(Apply apply);
        T Visit<A>(IRecExpression<A> rec) where A : Expression;
        T Visit<A>(IVariableExpression<A> variable) where A : Expression;
    }
}
