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
    public abstract class NumExpression
    {
        public abstract T Visit<T>(INumVisitor<T> visitor);
    }

    public class NumVariable : NumExpression
    {
        public override T Visit<T>(INumVisitor<T> visitor)
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
    }

    public enum NumUnOps
    {
        Neg
    }

    public class NumUnaryOp : NumExpression
    {
        public NumUnOps Operation;

        public NumUnaryOp(NumUnOps Operation)
        {
            this.Operation = Operation;
        }

        public override T Visit<T>(INumVisitor<T> visitor)
        {
            return visitor.VisitNumUnaryOp(this);
        }
    }

    public enum NumBinOps
    {
        Add,
        Mul
    }

    public class NumBinaryOp : NumExpression
    {
        public NumBinOps Operation;

        public NumBinaryOp(NumBinOps Operation)
        {
            this.Operation = Operation;
        }

        public override T Visit<T>(INumVisitor<T> visitor)
        {
            return visitor.VisitNumBinaryOp(this);
        }
    }

    public enum NumRelations
    {
        GT,
        LT,
        EQ,
        NEQ,
        GTE,
        LTE
    }

    //public class NumRelation : LogicExpression // Note the logic expression!
    //{

    //    public override T Visit<T>(ILogicNumVisitor<T> visitor)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
