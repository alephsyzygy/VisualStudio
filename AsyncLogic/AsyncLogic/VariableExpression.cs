using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public interface IVariableExpression<T> where T : Expression
    {
        string VariableName { get; }
        T Construct(string VariableName);
        T Self();
    }

    /// <summary>
    /// A logic variable
    /// </summary>
    public class LogicVariable : LogicExpression, IVariableExpression<LogicExpression>
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The variable name</param>
        public LogicVariable(string VariableName)
        {
            this.VariableName = VariableName;
            this.Type = Type.SigmaType;
        }


        public LogicExpression Construct(string VariableName)
        {
            return new LogicVariable(VariableName);
        }


        public LogicExpression Self()
        {
            return this;
        }
    }

    public class NumVariable : NumExpression, IVariableExpression<NumExpression>
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The variable name</param>
        public NumVariable(string VariableName)
        {
            this.VariableName = VariableName;
            this.Type = Type.NumType;
        }


        public NumExpression Construct(string VariableName)
        {
            return new NumVariable(VariableName);
        }


        public NumExpression Self()
        {
            return this;
        }
    }

    public class PairVariable : AbstractPairExpression, IVariableExpression<AbstractPairExpression>
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The variable name</param>
        public PairVariable(string VariableName)
        {
            this.VariableName = VariableName;
            this.Type = Type.SigmaType;
        }


        public AbstractPairExpression Construct(string VariableName)
        {
            return new PairVariable(VariableName);
        }


        public AbstractPairExpression Self()
        {
            return this;
        }
    }

    public class LambdaVariable : AbstractLambdaExpression, IVariableExpression<AbstractLambdaExpression>
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string VariableName { get; private set; }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The variable name</param>
        public LambdaVariable(string VariableName)
        {
            this.VariableName = VariableName;
            this.Type = Type.SigmaType;
        }


        public AbstractLambdaExpression Construct(string VariableName)
        {
            return new LambdaVariable(VariableName);
        }


        public AbstractLambdaExpression Self()
        {
            return this;
        }
    }

}
