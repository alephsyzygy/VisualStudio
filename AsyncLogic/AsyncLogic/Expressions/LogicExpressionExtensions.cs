using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Syntactic extensions to expressions

namespace AsyncLogic.Expressions
{
    /// <summary>
    /// Represents a bounded forall over the naturals.
    /// </summary>
    public class BoundedForall : LogicExpression
    {
        public string VariableName;
        public NumExpression Bound;
        public LogicExpression Predicate;
        RecLogicExpression DeSugar;

        public BoundedForall(string VariableName, NumExpression Bound, LogicExpression Predicate)
        {
            this.VariableName = VariableName;
            this.Bound = Bound;
            this.Predicate = Predicate;
            string internalVar = "%" + VariableName;
            var step = new LogicVariable(internalVar) & Predicate;
            DeSugar = new RecLogicExpression(Bound, new LogicTrue(), VariableName, internalVar, step);
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(DeSugar);
        }

        public override string ToString()
        {
            return "Forall " + VariableName + " < " + Bound.ToString() + ". " + Predicate.ToString();
        }
    }

    /// <summary>
    /// Represents a bounded exists over the naturals.  Unlike the standard exists
    /// plus an inequality the bounded exists only spawns a bounded number of tasks.
    /// This is controlled by a recursion, which will increase the size of the term.
    /// </summary>
    public class BoundedExists : LogicExpression
    {
        public string VariableName;
        public NumExpression Bound;
        public LogicExpression Predicate;
        RecLogicExpression DeSugar;

        public BoundedExists(string VariableName, NumExpression Bound, LogicExpression Predicate)
        {
            this.VariableName = VariableName;
            this.Bound = Bound;
            this.Predicate = Predicate;
            string internalVar = "%" + VariableName;
            var step = new LogicVariable(internalVar) | Predicate;
            DeSugar = new RecLogicExpression(Bound, new LogicFalse(), VariableName, internalVar, step);
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(DeSugar);
        }

        public override string ToString()
        {
            return "Exists " + VariableName + " < " + Bound.ToString() + ". " + Predicate.ToString();
        }
    }
}
