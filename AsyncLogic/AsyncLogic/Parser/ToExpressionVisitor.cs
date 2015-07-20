using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;

namespace AsyncLogic.Parser
{
    public class ToExpressionVisitor : IParserExpressionVisitor<Expression>
    {
        public Expression Run(ParserExpression Expression)
        {
            // First run the type visitor to make the type info available.
            var typeVisitor = new TypeVisitor();
            var type = Expression.Accept(typeVisitor);

            return Expression.Accept(this);
        }
        public Expression Visit(ParserVariable variable)
        {
            if (variable.Type is ParserNumType) 
                return new NumVariable(variable.VariableName);
            if (variable.Type is ParserLogicType)
                return new LogicVariable(variable.VariableName);
            if (variable.Type is ParserLambdaType)
                return new LambdaVariable(variable.VariableName);
            if (variable.Type is ParserProdType)
                return new PairVariable(variable.VariableName);

            throw new ArgumentException("Unknown variable type");
        }

        public Expression Visit(ParserNumConstant constant)
        {
            return new NumConstant(constant.Value);
        }

        public Expression Visit(ParserStringConstant constant)
        {
            switch (constant.Value)
            {
                case "T":
                    return new LogicTrue();
                case "F":
                    return new LogicFalse();
                default:
                    break;
            }

            throw new ArgumentException("Unknown constant");
        }

        public Expression Visit(ParserUnaryOp op)
        {
            Expression subExpr = op.Expr.Accept(this);
            ParserProdType type = op.Expr.Type as ParserProdType;

            switch (op.OpType)
            {
                case ParserUnaryOpType.ProjL:
                    if (type.Left is ParserNumType)
                        return new NumProjL(subExpr as AbstractPairExpression);
                    if (type.Left is ParserLogicType)
                        return new LogicProjL(subExpr as AbstractPairExpression);
                    if (type.Left is ParserProdType)
                        return new PairProjL(subExpr as AbstractPairExpression);
                    if (type.Left is ParserLambdaType)
                        return new LambdaProjL(subExpr as AbstractPairExpression);
                    break;
                case ParserUnaryOpType.ProjR:
                    if (type.Right is ParserNumType)
                        return new NumProjR(subExpr as AbstractPairExpression);
                    if (type.Right is ParserLogicType)
                        return new LogicProjR(subExpr as AbstractPairExpression);
                    if (type.Right is ParserProdType)
                        return new PairProjR(subExpr as AbstractPairExpression);
                    if (type.Right is ParserLambdaType)
                        return new LambdaProjR(subExpr as AbstractPairExpression);
                    break;
                default:
                    break;
            }

            throw new ArgumentException("Unknown unary operation");
        }

        public Expression Visit(ParserBinaryOp op)
        {
            Expression leftExpr = op.Left.Accept(this);
            Expression rightExpr = op.Right.Accept(this);

            switch (op.OpType)
            {
                case ParserBinaryOpType.Add:
                    return new NumBinaryOp(NumBinOps.Add, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.Mul:
                    return new NumBinaryOp(NumBinOps.Mul, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.And:
                    return new LogicAnd(leftExpr as LogicExpression, rightExpr as LogicExpression);
                case ParserBinaryOpType.Or:
                    return new LogicOr(leftExpr as LogicExpression, rightExpr as LogicExpression);
                case ParserBinaryOpType.EQ:
                    return new NumRelation(NumRels.EQ, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.NEQ:
                    return new NumRelation(NumRels.NEQ, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.GT:
                    return new NumRelation(NumRels.GT, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.LT:
                    return new NumRelation(NumRels.LT, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.GTE:
                    return new NumRelation(NumRels.GTE, leftExpr as NumExpression, rightExpr as NumExpression);
                case ParserBinaryOpType.LTE:
                    return new NumRelation(NumRels.LTE, leftExpr as NumExpression, rightExpr as NumExpression);
                default:
                    break;
            }


            throw new ArgumentException("Unknown binary operation");
        }

        public Expression Visit(ParserBinder binder)
        {
            Expression subExpr = binder.Expr.Accept(this);

            switch (binder.OpType)
            {
                case ParserBinderType.Exists:
                    return new NumExists(binder.VariableName, subExpr as LogicExpression);
                case ParserBinderType.Lambda:
                    return new LambdaExpression(binder.VariableName, subExpr as LogicExpression);
                case ParserBinderType.The:
                    return new NumThe(binder.VariableName, subExpr as LogicExpression);
                default:
                    break;
            }

            throw new ArgumentException("Unknown binder");
        }

        public Expression Visit(ParserRec rec)
        {
            Expression input = rec.Input.Accept(this);
            Expression start = rec.Start.Accept(this);
            Expression step = rec.Step.Accept(this);

            if (rec.Start.Type is ParserNumType)
                return new RecNumExpression(input as NumExpression, start as NumExpression, rec.NumVariableName, rec.AccVariableName,
                    step as NumExpression);
            if (rec.Start.Type is ParserLogicType)
                return new RecLogicExpression(input as NumExpression, start as LogicExpression, rec.NumVariableName, rec.AccVariableName,
                    step as LogicExpression);
            if (rec.Start.Type is ParserLambdaType)
                return new RecLambdaExpression(input as NumExpression, start as AbstractLambdaExpression, rec.NumVariableName, rec.AccVariableName,
                    step as AbstractLambdaExpression);
            if (rec.Start.Type is ParserProdType)
                return new RecPairExpression(input as NumExpression, start as AbstractPairExpression, rec.NumVariableName, rec.AccVariableName,
                    step as AbstractPairExpression);



            throw new ArgumentException("Unknown Rec type");
        }

        public Expression Visit(ParserPair pair)
        {
            Expression left = pair.Left.Accept(this);
            Expression right = pair.Right.Accept(this);

            // this might cause problems later
            return new PairExpression<Expression,Expression>(left, right);
        }

        public Expression Visit(ParserApp app)
        {
            Expression lambda = app.Lambda.Accept(this);
            Expression expr = app.Expression.Accept(this);

            return new Apply(lambda as AbstractLambdaExpression, expr);
        }
    }
}
