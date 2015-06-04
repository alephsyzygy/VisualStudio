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



    /// <summary>
    /// A constant of type Num
    /// </summary>
    public class NumConstant : NumExpression
    {
        public int Value;

        public NumConstant(int Value)
        {
            this.Value = Value;
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

    /// <summary>
    /// Binary operations of Num's
    /// </summary>
    public enum NumBinOps
    {
        Add,
        Mul
    }

    /// <summary>
    /// A Num binary operation
    /// </summary>
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

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumBinaryOp(this);
        }
    }

    /// <summary>
    /// The 'the' binder for natural numbers.  the x. phi(x)
    /// represents the unique natural number x which satifies phi(x)
    /// If phi does not uniquely determine a number then evaluation is undefined
    /// but we tend to give first found natural, or loop if there is none.
    /// </summary>
    public class NumThe : NumExpression
    {
        public string VariableName;
        public LogicExpression Expression;

        public NumThe(string VariableName, LogicExpression Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumThe(this);
        }
    }

}
