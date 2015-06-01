using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// A RecExpression represents Num recursion into any type.
    /// We require an input, of Num, a start value, also of Num
    /// a step expression with two free variables.
    /// </summary>
    public class RecExpression : Expression
    {
        public NumExpression Input;
        public NumExpression Start;
        public Expression Step;
        public string NumVariableName;
        public string AccVariableName;

        public RecExpression(NumExpression Input, NumExpression Start, string NumVariableName, 
             string AccVariableName, Expression Step)
        {
            this.Input = Input;
            this.Start = Start;
            this.Step = Step;
            this.NumVariableName = NumVariableName;
            this.AccVariableName = AccVariableName;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitRec(this);
        }
    }
}
