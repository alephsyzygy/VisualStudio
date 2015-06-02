using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public interface IRecExpression<T> where T : Expression
    {
        NumExpression Input {get; }
        NumExpression Start { get; }
        T Step { get; }
        string NumVariableName { get;  }
        string AccVariableName { get; } 
    }
    /// <summary>
    /// A RecExpression represents Num recursion into any type.
    /// We require an input, of Num, a start value, also of Num
    /// a step expression with two free variables.
    /// </summary>
    public class RecNumExpression : NumExpression, IRecExpression<NumExpression>
    {
        public NumExpression Input {get; private set;}
        public NumExpression Start { get; private set; }
        public NumExpression Step { get; private set; }
        public string NumVariableName { get; private set; }
        public string AccVariableName { get; private set; }

        public RecNumExpression(NumExpression Input, NumExpression Start, string NumVariableName, 
             string AccVariableName, NumExpression Step)
        {
            this.Input = Input;
            this.Start = Start;
            this.Step = Step;
            this.NumVariableName = NumVariableName;
            this.AccVariableName = AccVariableName;
        }

        public override T Visit<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitRecNum(this);
        }
    }
}
