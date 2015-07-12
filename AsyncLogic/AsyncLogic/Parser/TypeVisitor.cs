using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Parser
{
    public class TypeVisitor : IParserExpressionVisitor<ParserType>
    {
        public Dictionary<string, ParserType> Context;

        public TypeVisitor(Dictionary<string, ParserType> Context)
        {
            this.Context = Context;
        }

        public TypeVisitor()
        {
            Context = new Dictionary<string, ParserType>();
        }

        private Dictionary<string, ParserType> CloneContext()
        {
            var newCtx = new Dictionary<string,ParserType>();
            foreach (var elem in Context)
                newCtx[elem.Key] = elem.Value;
            return newCtx;
        }

        private void JoinContext(Dictionary<string, ParserType> NewContext)
        {
            foreach (var elem in NewContext)
                Context[elem.Key] = elem.Value;
        }


        public ParserType Visit(ParserVariable variable)
        {
            ParserType output = Context[variable.VariableName];
            if (output == null)
                throw new ArgumentException("Variable Not Found in Context");

            return output;
        }

        public ParserType Visit(ParserNumConstant constant)
        {
            return new ParserNumType();
        }

        public ParserType Visit(ParserStringConstant constant)
        {
            switch (constant.Value)
            {
                case "T":
                case "F":
                    return new ParserLogicType();
                default:
                    throw new ArgumentException("Unknown Constant");
            }
        }

        public ParserType Visit(ParserUnaryOp op)
        {
            ParserType subExpr = op.Expr.Accept(this);
            if (subExpr.Typ != ParserTypeEnum.Pair || !(subExpr is ParserProdType))
                throw new ArgumentException("Projection must apply to a pair");

            var prod = subExpr as ParserProdType;
            switch (op.OpType)
            {
                case ParserUnaryOpType.ProjL:
                    return prod.Left;
                case ParserUnaryOpType.ProjR:
                    return prod.Right;
                default:
                    throw new ArgumentException("Unknown Unary Operation");
            }
        }

        public ParserType Visit(ParserBinaryOp op)
        {
            ParserType leftType = op.Left.Accept(this);
            ParserType rightType = op.Right.Accept(this);

            if (leftType.Typ != rightType.Typ)
                throw new ArgumentException("Types in a binary operation must be the same");

            switch (op.OpType)
            {
                case ParserBinaryOpType.Add:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Number) && leftType.Flags.HasFlag(ParserTypeFlags.Number))
                        return leftType;
                    else
                        throw new ArgumentException("Cannot Add these expressions");
                case ParserBinaryOpType.Mul:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Number) && leftType.Flags.HasFlag(ParserTypeFlags.Number))
                        return leftType;
                    else
                        throw new ArgumentException("Cannot Multiply these expressions");
                case ParserBinaryOpType.And:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Logic) && leftType.Flags.HasFlag(ParserTypeFlags.Logic))
                        return leftType;
                    else
                        throw new ArgumentException("Cannot & these expressions");
                case ParserBinaryOpType.Or:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Logic) && leftType.Flags.HasFlag(ParserTypeFlags.Logic))
                        return leftType;
                    else
                        throw new ArgumentException("Cannot | these expressions");
                case ParserBinaryOpType.EQ:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Discrete) && leftType.Flags.HasFlag(ParserTypeFlags.Discrete))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot == these expressions");
                case ParserBinaryOpType.NEQ:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Hausdorff) && leftType.Flags.HasFlag(ParserTypeFlags.Hausdorff))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot != these expressions");
                case ParserBinaryOpType.GT:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder) && leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot > these expressions");
                case ParserBinaryOpType.LT:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder) && leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot < these expressions");
                case ParserBinaryOpType.GTE:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder) && leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot >= these expressions");
                case ParserBinaryOpType.LTE:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder) && leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder))
                        return new ParserLogicType();
                    else
                        throw new ArgumentException("Cannot <= these expressions");
                default:
                    throw new ArgumentException("Unknown Binary Operation");
            }
        }

        public ParserType Visit(ParserBinder binder)
        {
            // idea: copy our current context, add the new bound variable, visit, remove bound variable
            //   then merge back into our current context
            var newCtx = CloneContext();
            newCtx[binder.VariableName] = binder.VariableType;
            var visitor = new TypeVisitor(newCtx);
            ParserType subExpr = binder.Expr.Accept(visitor);
            newCtx.Remove(binder.VariableName);
            JoinContext(newCtx);
            
            if (subExpr.Typ != ParserTypeEnum.Logic || !(subExpr is ParserLogicType))
                throw new ArgumentException("Bound subexpression must be a logical expression");

            var logicalExpr = subExpr as ParserLogicType;
            switch (binder.OpType)
            {
                case ParserBinderType.Exists:
                    if ((binder.VariableType.Flags & ParserTypeFlags.Overt) == 0)
                        throw new ArgumentException("Exists can only bind overt variables");
                    return new ParserLogicType();
                case ParserBinderType.Lambda:
                    return new ParserLambdaType(binder.VariableType);
                case ParserBinderType.The:
                    if (binder.VariableType is ParserNumType)
                        return new ParserNumType();
                    else
                        throw new ArgumentException("'The' binder can only bind Nat types");
                default:
                    throw new ArgumentException("Unknown Binder");
            }
        }

        public ParserType Visit(ParserRec rec)
        {
            throw new NotImplementedException();
        }

        public ParserType Visit(ParserPair pair)
        {
            throw new NotImplementedException();
        }

        public ParserType Visit(ParserApp app)
        {
            throw new NotImplementedException();
        }
    }
}
