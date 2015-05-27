using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace AsyncLogic
{
    /// <summary>
    /// A class representing the value types of a logic expressions.
    /// So far either true, false, or a number, or a pair.
    /// WARNING: may contain unevaluated values.  Use Normalize to clear these.
    /// </summary>
    public abstract class Value
    {
        public virtual async Task<string> ToStringAsync()
        {
            return this.ToString();
        }

        public virtual async Task<Value> Normalize()
        {
            return this;
        }
    }

    /// <summary>
    /// A Boolean value, either true or false
    /// </summary>
    public class BoolValue : Value
    {
        public bool Value;

        public BoolValue(bool Value)
        {
            this.Value = Value;

            // We are now easier on this requirement in order to increase performance.  If we obtain a false
            // then we can avoid looping.
            // OLD: The semantics states that we should never see false, so we throw an exception if we do
            //if (!Value)
            //    throw new ArgumentException("This really shouldn't be false, you should be looping instead");
        }

        public static bool operator ==(BoolValue first, BoolValue second)
        {
            return first.Value == second.Value;
        }

        public static bool operator !=(BoolValue first, BoolValue second)
        {
            return first.Value != second.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A number, such as 0,1,2,3,4,...
    /// </summary>
    public class NumValue : Value
    {
        public int Value;

        public NumValue(int Value)
        {
            if (Value < 0)
                throw new ArgumentException("NumValue must not be negative");
            this.Value = Value;
        }

        public static bool operator ==(NumValue first, NumValue second)
        {
            return first.Value == second.Value;
        }

        public static bool operator !=(NumValue first, NumValue second)
        {
            return first.Value != second.Value;
        }

        public static bool operator >(NumValue first, NumValue second)
        {
            return first.Value > second.Value;
        }

        public static bool operator <(NumValue first, NumValue second)
        {
            return first.Value < second.Value;
        }

        public static bool operator >=(NumValue first, NumValue second)
        {
            return first.Value >= second.Value;
        }

        public static bool operator <=(NumValue first, NumValue second)
        {
            return first.Value <= second.Value;
        }

        public static NumValue operator +(NumValue first, NumValue second)
        {
            return new NumValue(first.Value + second.Value);
        }

        public static NumValue operator *(NumValue first, NumValue second)
        {
            return new NumValue(first.Value * second.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A pair of values
    /// </summary>
    /// <typeparam name="A">The type of the first value</typeparam>
    /// <typeparam name="B">The type of the second value</typeparam>
    public class PairValue<A,B> : Value
        where A : Value
        where B : Value
    {
        public A Left;
        public B Right;

        public PairValue(A Left, B Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override string ToString()
        {
            return "< " + Left.ToString() + " , " + Right.ToString() + " >"; 
        }
    }

    /// <summary>
    /// A potential pair of values.  Instead of storing these values it stores Tasks
    /// </summary>
    /// <typeparam name="A">The type of the first value</typeparam>
    /// <typeparam name="B">The type of the second value</typeparam>
    public class PotentialPairValue<A, B> : Value
        where A : Value
        where B : Value
    {
        public Task<A> Left;
        public Task<B> Right;

        public PotentialPairValue(Task<A> Left, Task<B> Right)
        {
            this.Left = Left;
            this.Right = Right;
        }

        public override string ToString()
        {
            return "< " + Left.ToString() + " , " + Right.ToString() + " >";
        }

        public async override Task<string> ToStringAsync()
        {
            var l = await Left;
            var r = await Right;
            return "< " + l.ToString() + " , " + r.ToString() + " >";
        }

        public override async Task<Value> Normalize()
        {
            var l = await Left;
            var r = await Right;
            return new PairValue<A, B>(l, r);
        }
    }

    /// <summary>
    /// A Lambda value.  Not much we can do with it.
    /// </summary>
    public class LambdaValue : Value
    {
        public string VariableName;
        public Expression Expression;

        public LambdaValue(string VariableName, Expression Expression)
        {
            this.VariableName = VariableName;
            this.Expression = Expression;
        }

        public override string ToString()
        {
            return "{Lambda " + VariableName + " }";
        }
    }
}
