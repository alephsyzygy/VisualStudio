using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// Visitor interface to visit an Expression
    /// </summary>
    /// <typeparam name="T">The return type</typeparam>
    public interface IExpressionVisitor<T> 
    {
        T VisitLogicVariable(LogicVariable variable);
        T VisitTrue(LogicTrue constant);
        T VisitFalse(LogicFalse constant);
        T VisitAnd(LogicAnd op);
        T VisitOr(LogicOr op);
        T VisitNumVariable(NumVariable variable);
        T VisitNumConstant(NumConstant constant);
        T VisitNumBinaryOp(NumBinaryOp op);
        T VisitNumRel(NumRelation relation);
        T VisitNumExists(NumExists expression);
        T VisitNumThe(NumThe expression);
    }
}
