using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// Visitor interface for a LogicExpression
    /// </summary>
    /// <typeparam name="T">The return type of the visit</typeparam>
    public interface ILogicVisitor<T> : INumVisitor<T>
    {
        T VisitLogicVariable(LogicVariable variable);
        T VisitTrue(LogicTrue constant);
        T VisitFalse(LogicFalse constant);
        T VisitAnd(LogicAnd op);
        T VisitOr(LogicOr op);
        //T VisitNumRel(NumRelation relation);
    }
}
