using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Parser
{
    public class Sequent
    {
        public Dictionary<string, ParserType> Context;
        public ParserExpression Expression;

        public Sequent(Dictionary<string, ParserType> Context, ParserExpression Expression)
        {
            this.Context = Context;
            this.Expression = Expression;
        }

        public override string ToString()
        {
            var ctx = String.Join(", ", from elem in Context select elem.Key + " : " + elem.Value.ToString());
            return ctx + " |- " + Expression.ToString();
        }

    }
}
