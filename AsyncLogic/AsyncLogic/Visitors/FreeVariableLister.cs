﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;

namespace AsyncLogic.Visitors
{
    // WARNING: this does not work correctly.
    /// <summary>
    /// A visitor object to construct the set of all variables
    /// </summary>
    public class FreeVariableLister : IExpressionVisitor<SortedSet<string>>
    {

        public SortedSet<string> Run (Expression Expr)
        {
            return Expr.Accept(this);
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

        public SortedSet<string> Visit(LogicTrue constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> Visit(LogicFalse constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> Visit(LogicAnd op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }

        public SortedSet<string> Visit(LogicOr op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }


        public SortedSet<string> Visit(NumRelation relation)
        {
            var vars = Run(relation.Left);
            vars.UnionWith(Run(relation.Right));
            return vars;
        }

        public SortedSet<string> Visit(NumConstant constant)
        {
            return new SortedSet<string> { };
        }

        public SortedSet<string> Visit(NumBinaryOp op)
        {
            var vars = Run(op.Left);
            vars.UnionWith(Run(op.Right));
            return vars;
        }


        public SortedSet<string> Visit(NumExists expression)
        {
            // Since this is a quantifier it binds a variable, which is no longer free. 
            // Note: find the free variables before removing them!
            var vars = Run(expression.Expression);
            vars.Remove(expression.VariableName);
            return vars;

        }


        public SortedSet<string> Visit(NumThe expression)
        {
            // 'the' is a binder, so it binds its variable
            var vars = Run(expression.Expression);
            vars.Remove(expression.VariableName);
            return vars;
        }


        public SortedSet<string> Visit<A, B>(PairExpression<A, B> expression)
            where A : Expression
            where B : Expression
        {
            var vars = Run(expression.Left);
            vars.UnionWith(Run(expression.Right));
            return vars;
        }


        public SortedSet<string> Visit<A>(IProjL<A> expression) where A : Expression
        {
            return Run(expression.Expression);
        }

        public SortedSet<string> Visit<A>(IProjR<A> expression) where A : Expression
        {
            return Run(expression.Expression);
        }


        public SortedSet<string> Visit(LambdaExpression lambda)
        {
            // Lambda is a binder, so remove it from the free variable list
            var vars = Run(lambda.Expression);
            vars.Remove(lambda.VariableName);
            return vars;
        }


        public SortedSet<string> Visit(Apply apply) 
        {
            var vars = Run(apply.Lambda);
            vars.UnionWith(Run(apply.Expression));
            return vars;
        }


        public SortedSet<string> Visit<A>(IRecExpression<A> rec) where A : Expression
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


        public SortedSet<string> Visit<A>(IVariableExpression<A> variable) where A : Expression
        {
            return new SortedSet<string> { variable.VariableName };
        }
    }
}
