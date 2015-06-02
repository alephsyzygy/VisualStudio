using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    // WARNING: this does not work correctly.
    /// <summary>
    /// A visitor object to construct the set of all variables
    /// </summary>
    public class FreeVariableLister : IExpressionVisitor<SortedSet<string>>
    {

        public SortedSet<string> Run (Expression Expr)
        {
            return Expr.Visit(this);
        }

        private SortedSet<T> JoinSets<T>(SortedSet<T> FirstSet, SortedSet<T> SecondSet)
        {
            SortedSet<T> output = new SortedSet<T>();
            foreach (var elem in FirstSet)
                output.Add(elem);
            foreach (var elem in SecondSet)
                output.Add(elem);
            return output;
        }

        public SortedSet<string> VisitLogicVariable(LogicVariable variable)
        {
            return new SortedSet<string> { variable.VariableName };
        }

        public SortedSet<string> VisitTrue(LogicTrue constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> VisitFalse(LogicFalse constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> VisitAnd(LogicAnd op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }

        public SortedSet<string> VisitOr(LogicOr op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }


        public SortedSet<string> VisitNumRel(NumRelation relation)
        {
            var vars = Run(relation.Left);
            vars.UnionWith(Run(relation.Right));
            return vars;
        }

        public SortedSet<string> VisitNumVariable(NumVariable variable)
        {
            return new SortedSet<string> { variable.VariableName };
        }

        public SortedSet<string> VisitNumConstant(NumConstant constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> VisitNumBinaryOp(NumBinaryOp op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }


        public SortedSet<string> VisitNumExists(NumExists expression)
        {
            // Since this is a quantifier it binds a variable, which is no longer free. 
            // Note: find the free variables before removing them!
            var vars = Run(expression.Expression);
            vars.Remove(expression.VariableName);
            return vars;

        }


        public SortedSet<string> VisitNumThe(NumThe expression)
        {
            // 'the' is a binder, so it binds its variable
            var vars = Run(expression.Expression);
            vars.Remove(expression.VariableName);
            return vars;
        }


        public SortedSet<string> VisitPair(PairExpression expression)
        {
            var vars = Run(expression.Left);
            vars.UnionWith(Run(expression.Right));
            return vars;
        }


        public SortedSet<string> VisitLeft(ProjL expression)
        {
            return Run(expression.Expression);
        }

        public SortedSet<string> VisitRight(ProjR expression)
        {
            return Run(expression.Expression);
        }


        public SortedSet<string> VisitLambda(LambdaExpression lambda)
        {
            // Lambda is a binder, so remove it from the free variable list
            var vars = Run(lambda.Expression);
            vars.Remove(lambda.VariableName);
            return vars;
        }


        public SortedSet<string> VisitApply(Apply apply) 
        {
            var vars = Run(apply.Lambda);
            vars.UnionWith(Run(apply.Expression));
            return vars;
        }


        public SortedSet<string> VisitPairVariable(PairVariable variable)
        {
            return new SortedSet<string> { variable.VariableName };
        }


        public SortedSet<string> VisitLambdaVariable(LambdaVariable variable) 
        {
            return new SortedSet<string> { variable.VariableName };
        }


        public SortedSet<string> VisitRecNum(RecNumExpression rec)
        {
            // First find free variables in the step
            var vars = Run(rec.Step);
            // Then remove binders
            vars.Remove(rec.NumVariableName);
            vars.Remove(rec.AccVariableName);

            // Now add free variables from input and step
            vars.UnionWith(Run(rec.Input));
            vars.UnionWith(Run(rec.Start));

            return vars;
        }
    }
}
