using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    //public interface IVariableExpression<T> where T : Expression
    //{
    //    string VariableName { get; }
    //    T Construct(string VariableName);
    //}

    ///// <summary>
    ///// A logic variable
    ///// </summary>
    //public class LogicVariable : LogicExpression, IVariableExpression<LogicExpression>
    //{
    //    /// <summary>
    //    /// The name of the variable
    //    /// </summary>
    //    public string VariableName {get; private set;}

    //    public override T Visit<T>(IExpressionVisitor<T> visitor)
    //    {
    //        return visitor.VisitVariable(this);
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
}
