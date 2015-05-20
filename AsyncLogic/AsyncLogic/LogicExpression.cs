using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{

    /// <summary>
    /// A LogicExpression is the abstract syntax tree for an expression of type Sigma (the type
    /// of logic values - classically either true or false)
    /// </summary>
    public abstract class LogicExpression : Expression
    {
        /// <summary>
        /// Visit this method
        /// </summary>
        /// <typeparam name="T">The return type of the visitor</typeparam>
        /// <param name="visitor">The visitor interface</param>
        /// <returns>The result of the visit</returns>
        //public abstract T Visit<T>(ILogicVisitor<T> visitor);
        //public abstract T Visit<T>(IExpressionVisitor<T> visitor);

        /// <summary>
        /// The disjunction (or) of two LogicExpressions
        /// </summary>
        /// <param name="first">First expression</param>
        /// <param name="second">Second expression</param>
        /// <returns>Their disjunction</returns>
        public static LogicExpression operator +(LogicExpression first, LogicExpression second)
        {
            return new LogicOr(first, second);
        }

        /// <summary>
        /// The conjunction (and) of two LogicExpressions
        /// </summary>
        /// <param name="first">First expression</param>
        /// <param name="second">Second expression</param>
        /// <returns>Their conjunction</returns>
        public static LogicExpression operator *(LogicExpression first, LogicExpression second)
        {
            return new LogicAnd(first, second);
        }
    }

    /// <summary>
    /// A logic variable
    /// </summary>
    public class LogicVariable : LogicExpression
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName;

        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    return visitor.VisitLogicVariable(this);
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLogicVariable(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The variable name</param>
        public LogicVariable(string name)
        {
            VariableName = name;
        }
    }

    /// <summary>
    /// The constant true
    /// </summary>
    public class LogicTrue : LogicExpression
    {
        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    return visitor.VisitTrue(this);
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitTrue(this);
        }

    }

    /// <summary>
    /// The constant false
    /// </summary>
    public class LogicFalse : LogicExpression
    {
        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    return visitor.VisitFalse(this);
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitFalse(this);
        }
    }

    //public enum UnaryOps
    //{
    //    Not
    //}

    //public class UnaryOp : LogicExpression
    //{
    //    public UnaryOps Operation;
    //    public LogicExpression Expression;

    //    public override T Visit<T>(ILogicVisitor<T> visitor)
    //    {
    //        return visitor.VisitUnaryOp(this);
    //    }

    //    public UnaryOp(UnaryOps Operation, LogicExpression Expression)
    //    {
    //        this.Operation = Operation;
    //        this.Expression = Expression;
    //    }
    //}


    /// <summary>
    /// The conjunction of two LogicExpressions
    /// </summary>
    public class LogicAnd : LogicExpression
    {
        public LogicExpression Left;
        public LogicExpression Right;

        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    return visitor.VisitAnd(this);
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitAnd(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LeftExpression">First expression</param>
        /// <param name="RightExpression">Second expression</param>
        public LogicAnd(LogicExpression LeftExpression, LogicExpression RightExpression)
        {
            this.Left = LeftExpression;
            this.Right = RightExpression;
        }
    }

    /// <summary>
    /// The disjunction of two LogicExpressions
    /// </summary>
    public class LogicOr : LogicExpression
    {
        public LogicExpression Left;
        public LogicExpression Right;

        //public override T Visit<T>(ILogicVisitor<T> visitor)
        //{
        //    return visitor.VisitOr(this);
        //}

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitOr(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="LeftExpression">First expression</param>
        /// <param name="RightExpression">Second expression</param>
        public LogicOr(LogicExpression LeftExpression, LogicExpression RightExpression)
        {
            this.Left = LeftExpression;
            this.Right = RightExpression;
        }
    }

   
}
