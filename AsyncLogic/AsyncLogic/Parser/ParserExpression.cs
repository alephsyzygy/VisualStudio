using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Parser
{
    public interface IParserExpressionVisitor<out T>
    {
        T Visit(ParserVariable variable);
        T Visit(ParserNumConstant constant);
        T Visit(ParserStringConstant constant);
        T Visit(ParserUnaryOp op);
        T Visit(ParserBinaryOp op);
        T Visit(ParserBinder binder);
        T Visit(ParserRec rec);
        T Visit(ParserPair pair);
        T Visit(ParserApp app);
    }

    public abstract class ParserExpression
    {
        public ParserType Type;
        public abstract T Accept<T>(IParserExpressionVisitor<T> visitor);
    }

    public class ParserVariable : ParserExpression
    {
        public string VariableName;

        public ParserVariable(string VariableName)
        {
            this.VariableName = VariableName;
        }

        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return VariableName;
        }
    }

    public class ParserNumConstant : ParserExpression
    {
        public int Value;

        public ParserNumConstant(int Value)
        {
            this.Value = Value;
        }

        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ParserStringConstant : ParserExpression
    {
        public string Value;

        public ParserStringConstant(string Value)
        {
            this.Value = Value;
        }

        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public enum ParserUnaryOpType
    {
        ProjL,
        ProjR
    }

    public class ParserUnaryOp : ParserExpression
    {
        public ParserUnaryOpType OpType;
        public ParserExpression Expr;

        public ParserUnaryOp(ParserUnaryOpType OpType, ParserExpression Expr)
        {
            this.OpType = OpType;
            this.Expr = Expr;
        }


        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return OpType.ToString() + "(" + Expr.ToString() + ")";
        }
    }

    public enum ParserBinaryOpType
    {
        Add,
        Mul,
        And,
        Or,
        EQ,
        NEQ,
        GT,
        LT,
        GTE,
        LTE
    }

    public class ParserBinaryOp : ParserExpression
    {
        public ParserBinaryOpType OpType;
        public ParserExpression Left;
        public ParserExpression Right;

        public ParserBinaryOp(ParserBinaryOpType OpType, ParserExpression Left, ParserExpression Right)
        {
            this.OpType = OpType;
            this.Left = Left;
            this.Right = Right;
        }


        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return OpType.ToString() + "(" + Left.ToString() +  ", " + Right.ToString() + ")";
        }
    }

    public class ParserApp : ParserExpression
    {
        public ParserExpression Lambda;
        public ParserExpression Expression;

        public ParserApp(ParserExpression Lambda, ParserExpression Expression)
        {
            this.Lambda = Lambda;
            this.Expression = Expression;

        }


        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "App(" + Lambda.ToString() + ", " + Expression.ToString() + ")";
        }
    }

    public enum ParserBinderType
    {
        Exists,
        Lambda,
        The
    }

    public class ParserBinder : ParserExpression
    {
        public ParserBinderType OpType;
        public string VariableName;
        public ParserExpression Expr;
        public ParserType VariableType;

        public ParserBinder(ParserBinderType OpType, string VariableName, ParserType VariableType, ParserExpression Expr)
        {
            this.OpType = OpType;
            this.VariableName = VariableName;
            this.Expr = Expr;
            this.VariableType = VariableType;
        }

        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return OpType.ToString() + " " + VariableName +" : " + VariableType.ToString() + ". " + Expr.ToString();
        }
    }

    public class ParserPair : ParserExpression
    {
        public ParserExpression Left;
        public ParserExpression Right;

        public ParserPair(ParserExpression Left, ParserExpression Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "<" + Left.ToString() + ", " + Right.ToString() + ">";
        }


    }

    public class ParserRec : ParserExpression
    {
        public ParserExpression Input;
        public ParserExpression Start;
        public string NumVariableName;
        public string AccVariableName;
        public ParserType AccType;
        public ParserExpression Step;

        public ParserRec(ParserExpression Input, ParserExpression Start, 
            string NumVariableName, string AccVariableName, ParserType AccType, ParserExpression Step)
        {
            this.Input = Input;
            this.Start = Start;
            this.NumVariableName = NumVariableName;
            this.AccVariableName = AccVariableName;
            this.Step = Step;
            this.AccType = AccType;

        }


        public override T Accept<T>(IParserExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return "Rec(" + Input.ToString() + ", " + Start.ToString() + ", " + NumVariableName + "." + AccVariableName + " : " +
                AccType.ToString() + ". " + Step.ToString() + ")";
        }
    }
}
