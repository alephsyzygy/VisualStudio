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
        /// The disjunction (or) of two LogicExpressions
        /// </summary>
        /// <param name="first">First expression</param>
        /// <param name="second">Second expression</param>
        /// <returns>Their disjunction</returns>
        public static LogicExpression operator |(LogicExpression first, LogicExpression second)
        {
            return new LogicOr(first, second);
        }

        /// <summary>
        /// The conjunction (and) of two LogicExpressions
        /// </summary>
        /// <param name="first">First expression</param>
        /// <param name="second">Second expression</param>
        /// <returns>Their conjunction</returns>
        public static LogicExpression operator &(LogicExpression first, LogicExpression second)
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
        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitFalse(this);
        }
    }

    /// <summary>
    /// The conjunction of two LogicExpressions
    /// </summary>
    public class LogicAnd : LogicExpression
    {
        public LogicExpression Left;
        public LogicExpression Right;

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


    /// <summary>
    /// Relation types between two Num's
    /// </summary>
    public enum NumRels
    {
        GT,
        LT,
        EQ,
        NEQ,
        GTE,
        LTE
    }

    /// <summary>
    /// A relation between two Num's
    /// </summary>
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

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumRel(this);
        }

    }

    public class NumExists : LogicExpression
    {
        public string VariableName;
        public LogicExpression Expression;

        public NumExists(string VariableName, LogicExpression Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitNumExists(this);
        }
    }

   
}
