using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Expressions
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

    ///// <summary>
    ///// A logic variable
    ///// </summary>
    //public class LogicVariable : LogicExpression
    //{
    //    /// <summary>
    //    /// The name of the variable
    //    /// </summary>
    //    public string VariableName;

    //    public override T Visit<T>(IExpressionVisitor<T> visitor)
    //    {
    //        return visitor.VisitLogicVariable(this);
    //    }

    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    /// <param name="name">The variable name</param>
    //    public LogicVariable(string name)
    //    {
    //        VariableName = name;
    //        this.Type = Type.SigmaType;
    //    }
    //}

    /// <summary>
    /// The constant true
    /// </summary>
    public class LogicTrue : LogicExpression
    {
        public LogicTrue()
        {
            this.Type = Type.SigmaType;
        }
        
        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "T";
        }

    }

    /// <summary>
    /// The constant false
    /// </summary>
    public class LogicFalse : LogicExpression
    {
        public LogicFalse()
        {
            this.Type = Type.SigmaType;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "F";
        }
    }

    /// <summary>
    /// The conjunction of two LogicExpressions
    /// </summary>
    public class LogicAnd : LogicExpression
    {
        public LogicExpression Left;
        public LogicExpression Right;

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
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
            //if (LeftExpression.Type != Type.SigmaType || RightExpression.Type != Type.SigmaType)
            //    throw new ArgumentException();
            this.Type = Type.SigmaType;
        }

        public override string ToString()
        {
            return "(" + Left.ToString() + " & " + Right.ToString() + ")";
        }
    }

    /// <summary>
    /// The disjunction of two LogicExpressions
    /// </summary>
    public class LogicOr : LogicExpression
    {
        public LogicExpression Left;
        public LogicExpression Right;

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
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
            //if (LeftExpression.Type != Type.SigmaType || RightExpression.Type != Type.SigmaType)
            //    throw new ArgumentException();
            this.Type = Type.SigmaType;
        }

        public override string ToString()
        {
            return "(" + Left.ToString() + " | " + Right.ToString() + ")";
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
            //if (Left.Type != Type.NumType || Right.Type != Type.NumType)
            //    throw new ArgumentException();
            this.Type = Type.SigmaType;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            string rel;
            switch (Relation)
            {
                case NumRels.GT:
                    rel = " > ";
                    break;
                case NumRels.LT:
                    rel = " < ";
                    break;
                case NumRels.EQ:
                    rel = " == ";
                    break;
                case NumRels.NEQ:
                    rel = " != ";
                    break;
                case NumRels.GTE:
                    rel = " >= ";
                    break;
                case NumRels.LTE:
                    rel = " <= ";
                    break;
                default:
                    rel = "";
                    break;
            }
            return "(" + Left.ToString() + rel + Right.ToString() + ")";
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
            this.Type = Type.SigmaType;
            //if (Expression.Type != Type.SigmaType)
            //    throw new ArgumentException();
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Exists " + VariableName + ". " + Expression.ToString();
        }
    }

    public class Apply : LogicExpression
    {
        public AbstractLambdaExpression Lambda;
        public Expression Expression;

        public Apply(AbstractLambdaExpression Lambda, Expression Expression)
        {
            this.Lambda = Lambda;
            this.Expression = Expression;
            //if (Expression.Type != Lambda.)
            //    throw new ArgumentException();
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "(" + Lambda.ToString() + " @ " + Expression.ToString() + ")";
        }
    }
   
}
