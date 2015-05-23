using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic
{
    /// <summary>
    /// A class representing the value types of a logic expressions.
    /// So far either true, false, or a number
    /// </summary>
    public abstract class Value
    {
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
            // The semantics states that we should never see false, so we throw an exception if we do
            if (!Value)
                throw new ArgumentException("This really shouldn't be false, you should be looping instead");
        }

        public static bool operator ==(BoolValue first, BoolValue second)
        {
            return first.Value == second.Value;
        }

        public static bool operator !=(BoolValue first, BoolValue second)
        {
            return first.Value != second.Value;
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
    }
}
