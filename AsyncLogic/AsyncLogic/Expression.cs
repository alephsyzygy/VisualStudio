using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    public abstract class Expression
    {
        public abstract T Visit<T>(IExpressionVisitor<T> visitor);
    }
}
