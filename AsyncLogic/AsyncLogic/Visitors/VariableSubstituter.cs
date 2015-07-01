using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;

namespace AsyncLogic.Visitors
{
    public class VariableSubstituter : IExpressionVisitor<Expression>
    {
        public string VariableName { get; private set; }
        public Expression Expression { get; private set; }

        public VariableSubstituter(string VariableName, Expression Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public Expression Substitute(Expression Expression)
        {
            return Expression.Accept(this);
        }

        public Expression Visit(LogicTrue constant)
        {
            return constant;
        }

        public Expression Visit(LogicFalse constant)
        {
            return constant;
        }

        public Expression Visit(LogicAnd op)
        {
            var left = (LogicExpression)op.Left.Accept(this);
            var right = (LogicExpression)op.Right.Accept(this);
            return new LogicAnd(left, right);
        }

        public Expression Visit(LogicOr op)
        {
            var left = (LogicExpression)op.Left.Accept(this);
            var right = (LogicExpression)op.Right.Accept(this);
            return new LogicOr(left, right);
        }

        public Expression Visit(NumConstant constant)
        {
            return constant;
        }

        public Expression Visit(NumBinaryOp op)
        {
            var left = (NumExpression)op.Left.Accept(this);
            var right = (NumExpression)op.Right.Accept(this);
            return new NumBinaryOp(op.Operation,left, right);
        }

        public Expression Visit(NumRelation relation)
        {
            var left = (NumExpression)relation.Left.Accept(this);
            var right = (NumExpression)relation.Right.Accept(this);
            return new NumRelation(relation.Relation, left, right);
        }

        public Expression Visit(NumExists expression)
        {
            if (expression.VariableName == VariableName)
                return expression; // variable is shadowed
            else
            {
                var expr = (LogicExpression)expression.Expression.Accept(this);
                return new NumExists(expression.VariableName, expr);
            }
        }

        public Expression Visit(NumThe expression)
        {
            if (expression.VariableName == VariableName)
                return expression; // variable is shadowed
            else
            {
                var expr = (LogicExpression)expression.Expression.Accept(this);
                return new NumThe(expression.VariableName, expr);
            }
        }

        public Expression Visit(PairExpression expression)
        {
            var left = expression.Left.Accept(this);
            var right = expression.Right.Accept(this);
            return new PairExpression(left, right);
        }

        public Expression Visit<A>(IProjL<A> pair) where A : Expression
        {
            var expr = (AbstractPairExpression)pair.Expression.Accept(this);
            return pair.Construct(expr);
        }

        public Expression Visit<A>(IProjR<A> pair) where A : Expression
        {
            var expr = (AbstractPairExpression)pair.Expression.Accept(this);
            return pair.Construct(expr);
        }

        public Expression Visit(LambdaExpression lambda) 
        {
            if (lambda.VariableName == VariableName)
                return lambda; // variable is shadowed
            else
            {
                var expr = (LogicExpression)lambda.Expression.Accept(this);
                return new LambdaExpression(lambda.VariableName, expr);
            }
        }

        public Expression Visit(Apply apply)
        {
            var lambda = (LambdaExpression)apply.Lambda.Accept(this);
            var expr = apply.Expression.Accept(this);
            return new Apply(lambda, expr);
        }


        public Expression Visit<A>(IRecExpression<A> rec) where A : Expression
        {
            var input = (NumExpression)rec.Input.Accept(this);
            var start = (A)rec.Start.Accept(this);
            if (rec.NumVariableName != VariableName && rec.AccVariableName != VariableName)
            {
                var step = (A)rec.Step.Accept(this);
                return rec.Construct(input, start, rec.NumVariableName, rec.AccVariableName, step);
            }
            else  // our variable is shadowed by one of the two binders in rec
                return rec.Construct(input, start, rec.NumVariableName, rec.AccVariableName, rec.Step);
        }


        public Expression Visit<A>(IVariableExpression<A> variable) where A : Expression
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable.Self();
        }
    }
}
