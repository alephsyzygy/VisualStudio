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

        public Expression VisitPair(PairExpression expression)
        {
            var left = expression.Left.Visit(this);
            var right = expression.Right.Visit(this);
            return new PairExpression(left, right);
        }

        public Expression VisitLeft(ProjL pair)
        {
            var expr = (PairExpression)pair.Expression.Visit(this);
            return new ProjL(expr);
        }

        public Expression VisitRight(ProjR pair)
        {
            var expr = (PairExpression)pair.Expression.Visit(this);
            return new ProjR(expr);
        }

        public Expression VisitLambda(LambdaExpression lambda) 
        {
            if (lambda.VariableName == VariableName)
                return lambda; // variable is shadowed
            else
            {
                var expr = lambda.Expression.Visit(this);
                return new LambdaExpression(lambda.VariableName, expr);
            }
        }

        public Expression VisitApply(Apply apply)
        {
            var lambda = (LambdaExpression)apply.Lambda.Visit(this);
            var expr = apply.Expression.Visit(this);
            return new Apply(lambda, expr);
        }


        public Expression VisitPairVariable(PairVariable variable)
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }


        public Expression VisitLambdaVariable(LambdaVariable variable)
        {
            if (variable.VariableName == VariableName)
                return Expression;
            else
                return variable;
        }


        public Expression VisitRecNum(RecNumExpression rec)
        {
            var input = (NumExpression)rec.Input.Visit(this);
            var start = (NumExpression)rec.Start.Visit(this);
            if (rec.NumVariableName != VariableName && rec.AccVariableName != VariableName)
            {
                var step = (NumExpression)rec.Step.Visit(this);
                return new RecNumExpression(input, start, rec.NumVariableName, rec.AccVariableName, step);
            }
            else  // our variable is shadowed by one of the two binders in rec
                return new RecNumExpression(input, start, rec.NumVariableName, rec.AccVariableName, rec.Step);
        }


        public Expression VisitRec<A>(IRecExpression<A> rec) where A : Expression
        {
            var input = (NumExpression)rec.Input.Visit(this);
            var start = (NumExpression)rec.Start.Visit(this);
            if (rec.NumVariableName != VariableName && rec.AccVariableName != VariableName)
            {
                var step = (A)rec.Step.Visit(this);
                return rec.Construct(input, start, rec.NumVariableName, rec.AccVariableName, step);
            }
            else  // our variable is shadowed by one of the two binders in rec
                return rec.Construct(input, start, rec.NumVariableName, rec.AccVariableName, rec.Step);
        }
    }
}
