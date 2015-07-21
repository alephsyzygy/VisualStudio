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

        // Clone the context so another visitor may use it
        private Dictionary<string, ParserType> CloneContext()
        {
            var newCtx = new Dictionary<string,ParserType>();
            foreach (var elem in Context)
                newCtx[elem.Key] = elem.Value;
            return newCtx;
        }

        // Merge another context into this one
        private void JoinContext(Dictionary<string, ParserType> NewContext)
        {
            foreach (var elem in NewContext)
            {
                var currentval = Context[elem.Key];
                if (currentval != null && !currentval.Equivalent(elem.Value))
                    throw new ArgumentException(String.Format(
                        "Contexts cannot be joined since they disagree. Key: {0} Old Value: {1} New Value: {2}",
                        elem.Key, currentval, elem.Value));
                Context[elem.Key] = elem.Value;
            }
        }


        public ParserType Visit(ParserVariable variable)
        {
            ParserType output;
            try
            {
                output = Context[variable.VariableName];
            }
            catch (KeyNotFoundException)
            {
                // If the variable is not found in the context we will throw our own exception
                throw new ArgumentException("Variable not found in context. Name: " + variable.VariableName);
            }

            if (output == null)
                throw new ArgumentException("Variable Not Found in Context: " + variable.VariableName);

            variable.Type = output;
            return output;
        }

        public ParserType Visit(ParserNumConstant constant)
        {
            ParserType numType = new ParserNumType();
            constant.Type = numType;
            return numType;
        }

        public ParserType Visit(ParserStringConstant constant)
        {
            switch (constant.Value)
            {
                case "T":
                case "F":
                    ParserType logicType = new ParserLogicType();
                    constant.Type = logicType;
                    return logicType;
                default:
                    throw new ArgumentException("Unknown Constant: " + constant.Value);
            }
        }

        public ParserType Visit(ParserUnaryOp op)
        {
            ParserType subExpr = op.Expr.Accept(this);
            if (subExpr.Typ != ParserTypeEnum.Pair || !(subExpr is ParserProdType))
                throw new ArgumentException("Projection must apply to a pair.  Current type: " + subExpr.ToString());

            var prod = subExpr as ParserProdType;
            switch (op.OpType)
            {
                case ParserUnaryOpType.ProjL:
                    op.Type = prod.Left;
                    return prod.Left;
                case ParserUnaryOpType.ProjR:
                    op.Type = prod.Right;
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
                throw new ArgumentException(String.Format("Types in a binary operation must be the same. Left: {0} Right: {0}",
                    leftType.ToString(), rightType.ToString()));

            switch (op.OpType)
            {
                case ParserBinaryOpType.Add:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Number) && leftType.Flags.HasFlag(ParserTypeFlags.Number))
                    {
                        op.Type = leftType;
                        return leftType;
                    }
                    else
                        throw new ArgumentException("Cannot Add these expressions");
                case ParserBinaryOpType.Mul:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Number) && leftType.Flags.HasFlag(ParserTypeFlags.Number))
                    {
                        op.Type = leftType;
                        return leftType;
                    }
                    else
                        throw new ArgumentException("Cannot Multiply these expressions");
                case ParserBinaryOpType.And:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Logic) && leftType.Flags.HasFlag(ParserTypeFlags.Logic))
                    {
                        op.Type = leftType;
                        return leftType;
                    }
                    else
                        throw new ArgumentException("Cannot & these expressions");
                case ParserBinaryOpType.Or:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Logic) && leftType.Flags.HasFlag(ParserTypeFlags.Logic))
                    {
                        op.Type = leftType;
                        return leftType;
                    }
                    else
                        throw new ArgumentException("Cannot | these expressions");
                case ParserBinaryOpType.EQ:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Discrete) && leftType.Flags.HasFlag(ParserTypeFlags.Discrete))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
                    else
                        throw new ArgumentException("Cannot == these expressions");
                case ParserBinaryOpType.NEQ:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.Hausdorff) && leftType.Flags.HasFlag(ParserTypeFlags.Hausdorff))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
                    else
                        throw new ArgumentException("Cannot != these expressions");
                case ParserBinaryOpType.GT:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder) && leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
                    else
                        throw new ArgumentException("Cannot > these expressions");
                case ParserBinaryOpType.LT:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder) && leftType.Flags.HasFlag(ParserTypeFlags.StrictOrder))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
                    else
                        throw new ArgumentException("Cannot < these expressions");
                case ParserBinaryOpType.GTE:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder) && leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
                    else
                        throw new ArgumentException("Cannot >= these expressions");
                case ParserBinaryOpType.LTE:
                    if (leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder) && leftType.Flags.HasFlag(ParserTypeFlags.LooseOrder))
                    {
                        ParserType logicType = new ParserLogicType();
                        op.Type = logicType;
                        return logicType;
                    }
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
                throw new ArgumentException("Bound subexpression must be a logical expression. Type: " + subExpr.ToString());

            var logicalExpr = subExpr as ParserLogicType;
            switch (binder.OpType)
            {
                case ParserBinderType.Exists:
                    if ((binder.VariableType.Flags & ParserTypeFlags.Overt) == 0)
                        throw new ArgumentException("Exists can only bind overt variables");
                    ParserType logicType = new ParserLogicType();
                    binder.Type = logicType;
                    return logicType;
                case ParserBinderType.Lambda:
                    ParserType newType = new ParserLambdaType(binder.VariableType);
                    binder.Type = newType;
                    return newType;
                case ParserBinderType.The:
                    if (binder.VariableType is ParserNumType)
                    {
                        ParserType numType = new ParserNumType();
                        binder.Type = numType;
                        return numType;
                    }
                    else
                        throw new ArgumentException("'The' binder can only bind Nat types");
                default:
                    throw new ArgumentException("Unknown Binder");
            }
        }

        public ParserType Visit(ParserRec rec)
        {
            var inputType = rec.Input.Accept(this);
            if (!(inputType is ParserNumType))
                throw new ArgumentException("The input to a Rec expression must be numeric. Type: " + inputType.ToString());
            var startType = rec.Start.Accept(this);

            // In order to type check the step clone our current context, add two new variables, run
            // the type check, remove the bound variables, then join the new context with the old
            var newCtx = CloneContext();
            newCtx[rec.NumVariableName] = new ParserNumType();
            newCtx[rec.AccVariableName] = rec.AccType;
            var visitor = new TypeVisitor(newCtx);
            ParserType stepType = rec.Step.Accept(visitor);
            newCtx.Remove(rec.AccVariableName);
            newCtx.Remove(rec.NumVariableName);
            JoinContext(newCtx);

            if (!startType.Equivalent(stepType))
                throw new ArgumentException(String.Format(
                    "Start type and step type in Rec must be equivalent. Start Type: {0} Step Type: {1}",
                    startType.ToString(), stepType.ToString()));

            rec.Type = startType;
            return startType;
        }

        public ParserType Visit(ParserPair pair)
        {
            var leftType = pair.Left.Accept(this);
            var rightType = pair.Right.Accept(this);
            ParserType prodType = new ParserProdType(leftType, rightType);
            pair.Type = prodType;
            return prodType;
        }

        public ParserType Visit(ParserApp app)
        {
            var lambdaType = app.Lambda.Accept(this);
            var expr = app.Expression.Accept(this);

            if (!(lambdaType is ParserLambdaType))
                throw new ArgumentException("In an application left expression must be a lambda. Type: " + lambdaType.ToString());

            ParserLambdaType lambda = lambdaType as ParserLambdaType;
            if (!lambda.InputType.Equivalent(expr))
                throw new ArgumentException(String.Format(
                    "In an application the type of the right expression must match the type of the bound variable "  +
                    "Expr type: {0} Variable type: {1}",
                    expr.ToString(), lambda.InputType.ToString()));

            ParserType logicType = new ParserLogicType();
            app.Type = logicType;
            return logicType;
        }
    }
}
