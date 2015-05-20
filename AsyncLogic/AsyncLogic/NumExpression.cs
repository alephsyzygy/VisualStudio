using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// A NumExpressions is an abstract syntax tree representing a
    /// natural number.
    /// </summary>
    public abstract class NumExpression : Expression
    {
        public abstract T Visit<T>(INumVisitor<T> visitor);

        public static LogicExpression operator ==(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.EQ, first, second);
        }

        public static LogicExpression operator !=(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.NEQ, first, second);
        }

        public static LogicExpression operator >(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.GT, first, second);
        }

        public static LogicExpression operator <(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.LT, first, second);
        }

        public static LogicExpression operator >=(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.GTE, first, second);
        }

        public static LogicExpression operator <=(NumExpression first, NumExpression second)
        {
            return new NumRelation(NumRels.LTE, first, second);
        }

        public static NumExpression operator +(NumExpression first, NumExpression second)
        {
            return new NumBinaryOp(NumBinOps.Add, first, second);
        }

        public static NumExpression operator *(NumExpression first, NumExpression second)
        {
            return new NumBinaryOp(NumBinOps.Mul, first, second);
        }
    }

    public class NumVariable : NumExpression
    {
        public string VariableName;

        public NumVariable(string VariableName)
        {
            this.VariableName = VariableName;
        }

        public override T Visit<T>(INumVisitor<T> visitor)
        {
            return visitor.VisitNumVariable(this);
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumVariable(this);
        }
    }

    public class NumConstant : NumExpression
    {
        public int Value;

        public NumConstant(int Value)
        {
            this.Value = Value;
        }


        public override T Visit<T>(INumVisitor<T> visitor)
        {
            return visitor.VisitNumConstant(this);
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumConstant(this);
        }
    }

    //public enum NumUnOps
    //{
    //    Neg
    //}

    //public class NumUnaryOp : NumExpression
    //{
    //    public NumUnOps Operation;
    //    public NumExpression Expression;

    //    public NumUnaryOp(NumUnOps Operation, NumExpression Expression)
    //    {
    //        this.Operation = Operation;
    //        this.Expression = Expression;
    //    }

    //    public override T Visit<T>(INumVisitor<T> visitor)
    //    {
    //        return visitor.VisitNumUnaryOp(this);
    //    }
    //}

    public enum NumBinOps
    {
        Add,
        Mul
    }

    public class NumBinaryOp : NumExpression
    {
        public NumBinOps Operation;
        public NumExpression Left;
        public NumExpression Right;

        public NumBinaryOp(NumBinOps Operation, NumExpression Left, NumExpression Right)
        {
            this.Operation = Operation;
            this.Left = Left;
            this.Right = Right;
        }

        public override T Visit<T>(INumVisitor<T> visitor)
        {
            return visitor.VisitNumBinaryOp(this);
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumBinaryOp(this);
        }
    }

    public enum NumRels
    {
        GT,
        LT,
        EQ,
        NEQ,
        GTE,
        LTE
    }

    public class NumRelation : LogicExpression // Note that this is a logic expression!
    {
        public NumRels Relation;
        public NumExpression Left;
        public NumExpression Right;

        public NumRelation(NumRels Relation, NumExpression Left, NumExpression Right)
        {
            this.Relation = Relation;
            this.Left = Left;
            this.Right = Right;
        }

        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    throw new NotImplementedException();
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumRel(this);
        }

    }
}
