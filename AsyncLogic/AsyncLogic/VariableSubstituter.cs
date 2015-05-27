using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
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
            return Expression.Visit(this);
        }

        public Expression VisitLogicVariable(LogicVariable variable)
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }

        public Expression VisitTrue(LogicTrue constant)
        {
            return constant;
        }

        public Expression VisitFalse(LogicFalse constant)
        {
            return constant;
        }

        public Expression VisitAnd(LogicAnd op)
        {
            var left = (LogicExpression)op.Left.Visit(this);
            var right = (LogicExpression)op.Right.Visit(this);
            return new LogicAnd(left, right);
        }

        public Expression VisitOr(LogicOr op)
        {
            var left = (LogicExpression)op.Left.Visit(this);
            var right = (LogicExpression)op.Right.Visit(this);
            return new LogicOr(left, right);
        }

        public Expression VisitNumVariable(NumVariable variable)
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }

        public Expression VisitNumConstant(NumConstant constant)
        {
            return constant;
        }

        public Expression VisitNumBinaryOp(NumBinaryOp op)
        {
            var left = (NumExpression)op.Left.Visit(this);
            var right = (NumExpression)op.Right.Visit(this);
            return new NumBinaryOp(op.Operation,left, right);
        }

        public Expression VisitNumRel(NumRelation relation)
        {
            var left = (NumExpression)relation.Left.Visit(this);
            var right = (NumExpression)relation.Right.Visit(this);
            return new NumRelation(relation.Relation, left, right);
        }

        public Expression VisitNumExists(NumExists expression)
        {
            if (expression.VariableName == VariableName)
                return expression; // variable is shadowed
            else
            {
                var expr = (LogicExpression)expression.Expression.Visit(this);
                return new NumExists(expression.VariableName, expr);
            }
        }

        public Expression VisitNumThe(NumThe expression)
        {
            if (expression.VariableName == VariableName)
                return expression; // variable is shadowed
            else
            {
                var expr = (LogicExpression)expression.Expression.Visit(this);
                return new NumThe(expression.VariableName, expr);
            }
        }

        public Expression VisitPair<A, B>(PairExpression<A, B> expression)
            where A : Expression
            where B : Expression
        {
            var left = (A)expression.Left.Visit(this);
            var right = (B)expression.Right.Visit(this);
            return new PairExpression<A,B>(left, right);
        }

        public Expression VisitLeft<A, B>(ProjL<A, B> pair)
            where A : Expression
            where B : Expression
        {
            var expr = (PairExpression<A,B>)pair.Expression.Visit(this);
            return new ProjL<A,B>(expr);
        }

        public Expression VisitRight<A, B>(ProjR<A, B> pair)
            where A : Expression
            where B : Expression
        {
            var expr = (PairExpression<A, B>)pair.Expression.Visit(this);
            return new ProjR<A, B>(expr);
        }

        public Expression VisitLambda<A>(LambdaExpression<A> lambda) where A : Expression
        {
            if (lambda.VariableName == VariableName)
                return lambda; // variable is shadowed
            else
            {
                var expr = (A)lambda.Expression.Visit(this);
                return new LambdaExpression<A>(lambda.VariableName, expr);
            }
        }

        public Expression VisitApply<A>(Apply<A> apply) where A : Expression
        {
            var lambda = (LambdaExpression<A>)apply.Lambda.Visit(this);
            var expr = (A)apply.Expression.Visit(this);
            return new Apply<A>(lambda, expr);
        }


        public Expression VisitPairVariable<A, B>(PairVariable<A, B> variable)
            where A : Expression
            where B : Expression
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }


        public Expression VisitLambdaVariable<A>(LambdaVariable<A> variable) where A : Expression
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }
    }
}
