using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public abstract class Type 
    { 
        public static Type SigmaType = new SigmaType();
        public static Type NumType = new NumType();
    }

    public class NumType : Type 
    {
        public override string ToString()
        {
            return "N";
        }
    }

    public class SigmaType : Type
    {
        public override string ToString()
        {
            return "Σ";
        }
    }

    public class PairType : Type
    {
        public Type Left;
        public Type Right;

        public PairType(Type Left, Type Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override string ToString()
        {
            return "(" + Left.ToString() + "×" + Right.ToString() + ")";
        }
    }

    public class LambdaType : Type
    {
        public Type Inner;

        public LambdaType(Type Inner)
        {
            this.Inner = Inner;
        }
        public override string ToString()
        {
            return "Σ^" + Inner.ToString();
        }
    }


    //public class ExprToType : IExpressionVisitor<Type>
    //{

    //    public Type VisitLogicVariable(LogicVariable variable)
    //    {
    //        return new SigmaType();
    //    }

    //    public Type VisitTrue(LogicTrue constant)
    //    {
    //        return new SigmaType();
    //    }

    //    public Type VisitFalse(LogicFalse constant)
    //    {
    //        return new SigmaType();
    //    }

    //    public Type VisitAnd(LogicAnd op)
    //    {
    //        var left = op.Left.Visit(this);
    //        var right = op.Right.Visit(this);
    //        if (left is SigmaType && right is SigmaType)
    //            return new SigmaType();
    //        else
    //            throw new ArgumentException("And must join Sigma types");
    //    }

    //    public Type VisitOr(LogicOr op)
    //    {
    //        var left = op.Left.Visit(this);
    //        var right = op.Right.Visit(this);
    //        if (left is SigmaType && right is SigmaType)
    //            return new SigmaType();
    //        else
    //            throw new ArgumentException("Or must join Sigma types");
    //    }

    //    public Type VisitNumVariable(NumVariable variable)
    //    {
    //        return new NumType();
    //    }

    //    public Type VisitNumConstant(NumConstant constant)
    //    {
    //        return new NumType();
    //    }

    //    public Type VisitNumBinaryOp(NumBinaryOp op)
    //    {
    //        var left = op.Left.Visit(this);
    //        var right = op.Right.Visit(this);
    //        if (left is NumType && right is NumType)
    //            return new SigmaType();
    //        else
    //            throw new ArgumentException("NumBinaryOP must join Num types");
    //    }

    //    public Type VisitNumRel(NumRelation relation)
    //    {
    //        var left = relation.Left.Visit(this);
    //        var right = relation.Right.Visit(this);
    //        if (left is NumType && right is NumType)
    //            return new SigmaType();
    //        else
    //            throw new ArgumentException("NumRelation must join Num types");
    //    }

    //    public Type VisitNumExists(NumExists expression)
    //    {
    //        var expr = expression.Expression.Visit(this);
    //        if (expr is SigmaType)
    //            return new SigmaType();
    //        else
    //            throw new ArgumentException("Exists must contain a Sigma Type");
    //    }

    //    public Type VisitNumThe(NumThe expression)
    //    {
    //        var expr = expression.Expression.Visit(this);
    //        if (expr is SigmaType)
    //            return new NumType();
    //        else
    //            throw new ArgumentException("The must contain a Sigma Type");
    //    }

    //    public Type VisitPair<A, B>(PairExpression<A, B> expression)
    //        where A : Expression
    //        where B : Expression
    //    {
    //        var left = expression.Left.Visit(this);
    //        var right = expression.Right.Visit(this);
    //        return new PairType<Type,Type>(left, right);
    //    }

    //    public Type VisitLeft<A, B>(ProjL<A, B> pair)
    //        where A : Expression
    //        where B : Expression
    //    {
    //        var expr = pair.Expression.Visit(this);
    //        var type = expr.GetType();
    //        try
    //        {
    //            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PairType<,>))
    //                return type.GetField("Left").GetValue(expr) as Type;
    //        }
    //        finally
    //        {
    //            throw new ArgumentException("ProjL exception");
    //        }
    //    }

    //    public Type VisitRight<A, B>(ProjR<A, B> pair)
    //        where A : Expression
    //        where B : Expression
    //    {
    //        var expr = pair.Expression.Visit(this);
    //        var type = expr.GetType();
    //        try
    //        {
    //            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PairType<,>))
    //                return type.GetField("Right").GetValue(expr) as Type;
    //        }
    //        finally
    //        {
    //            throw new ArgumentException("ProjR exception");
    //        }
    //    }

    //    public Type VisitLambda<A>(LambdaExpression<A> lambda) where A : Expression
    //    {
    //        var expr = lambda.Expression.Visit(this);

    //        return new LambdaType<Type>(expr);
    //    }

    //    public Type VisitApply<A>(Apply<A> apply) where A : Expression
    //    {
    //        var lambda = apply.Lambda.Visit(this);
    //        var expr = apply.Expression.Visit(this);

    //    }

    //    public Type VisitPairVariable<A, B>(PairVariable<A, B> variable)
    //        where A : Expression
    //        where B : Expression
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Type VisitLambdaVariable<A>(LambdaVariable<A> variable) where A : Expression
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
