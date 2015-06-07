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
        T Start { get; }
        T Step { get; }
        string NumVariableName { get;  }
        string AccVariableName { get; }

        T Construct(NumExpression Input, T Start, string NumVariableName,
             string AccVariableName, T Step);
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
            return visitor.VisitRec(this);
        }


        public NumExpression Construct(NumExpression Input, NumExpression Start, string NumVariableName, string AccVariableName, NumExpression Step)
        {
            return new RecNumExpression(Input, Start, NumVariableName, AccVariableName, Step);
        }
    }

    public class RecLogicExpression : LogicExpression, IRecExpression<LogicExpression>
    {
        public NumExpression Input { get; private set; }
        public LogicExpression Start { get; private set; }
        public LogicExpression Step { get; private set; }
        public string NumVariableName { get; private set; }
        public string AccVariableName { get; private set; }

        public RecLogicExpression(NumExpression Input, LogicExpression Start, string NumVariableName,
             string AccVariableName, LogicExpression Step)
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


        public LogicExpression Construct(NumExpression Input, LogicExpression Start, string NumVariableName, string AccVariableName, LogicExpression Step)
        {
            return new RecLogicExpression(Input, Start, NumVariableName, AccVariableName, Step);
        }
    }

        public class RecPairExpression : AbstractPairExpression, IRecExpression<AbstractPairExpression>
    {
        public NumExpression Input {get; private set;}
        public AbstractPairExpression Start { get; private set; }
        public AbstractPairExpression Step { get; private set; }
        public string NumVariableName { get; private set; }
        public string AccVariableName { get; private set; }

        public RecPairExpression(NumExpression Input, AbstractPairExpression Start, string NumVariableName, 
             string AccVariableName, AbstractPairExpression Step)
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


        public AbstractPairExpression Construct(NumExpression Input, AbstractPairExpression Start, string NumVariableName, string AccVariableName, AbstractPairExpression Step)
        {
            return new RecPairExpression(Input, Start, NumVariableName, AccVariableName, Step);
        }
        }

        public class RecLambdaExpression : AbstractLambdaExpression, IRecExpression<AbstractLambdaExpression>
    {
        public NumExpression Input {get; private set;}
        public AbstractLambdaExpression Start { get; private set; }
        public AbstractLambdaExpression Step { get; private set; }
        public string NumVariableName { get; private set; }
        public string AccVariableName { get; private set; }

        public RecLambdaExpression(NumExpression Input, AbstractLambdaExpression Start, string NumVariableName, 
             string AccVariableName, AbstractLambdaExpression Step)
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


        public AbstractLambdaExpression Construct(NumExpression Input, AbstractLambdaExpression Start, string NumVariableName, string AccVariableName, AbstractLambdaExpression Step)
        {
            return new RecLambdaExpression(Input, Start, NumVariableName, AccVariableName, Step);
        }
    }
}
