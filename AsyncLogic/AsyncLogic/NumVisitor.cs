using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// Visitor interface for a NumExpression
    /// </summary>
    /// <typeparam name="T">The return type of the visit</typeparam>
    public interface INumVisitor<T>
    {
        T VisitNumVariable(NumVariable variable);
        T VisitNumConstant(NumConstant constant);
        //T VisitNumUnaryOp(NumUnaryOp op);
        T VisitNumBinaryOp(NumBinaryOp op);
        //T VisitNumRelation(NumRelation rel);
    }
}
