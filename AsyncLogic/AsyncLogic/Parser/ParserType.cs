using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncLogic.Parser
{
    public interface IParserTypeVisitor<out T>
    {
        T Visit(ParserLogicType type);
        T Visit(ParserNumType type);
        T Visit(ParserProdType type);
        T Visit(ParserLambdaType type);
        T Visit(ParserUnkownType type);
    }

    public enum ParserTypeEnum
    {
        Logic,
        Number,
        Pair,
        Lambda,
        Unknown
    }

    public abstract class ParserType
    {
        public abstract T Accept<T>(IParserTypeVisitor<T> visitor);

        public ParserTypeFlags Flags;

        public abstract bool Equivalent(ParserType Other);
        public virtual bool Compatible(ParserTypeFlags Flag)
        {
            return true;
        }

        public ParserTypeEnum Typ;
    }

    [Flags]
    public enum ParserTypeFlags
    {
        Discrete    = 1 << 0,
        Hausdorff   = 1 << 1,
        Overt       = 1 << 2,
        Compact     = 1 << 3,
        StrictOrder = 1 << 4,
        LooseOrder  = 1 << 5,
        Logic       = 1 << 6,
        Number      = 1 << 7,
        Pair        = 1 << 8,
        Lambda      = 1 << 9, // left of app
        Arithmetic  = 1 << 10,
        Division    = 1 << 11,

        Num = Discrete | Hausdorff | Overt | StrictOrder | LooseOrder | Number | Arithmetic,
        Rat = Discrete | Hausdorff | Overt | StrictOrder | LooseOrder | Arithmetic,
        Real = Hausdorff | Overt | StrictOrder | Arithmetic | Division,

        PairInheritable = Discrete | Hausdorff | Overt | Compact
    }

    public class ParserUnkownType : ParserType
    {

        public ParserUnkownType(ParserTypeFlags Flags)
        {
            this.Flags = Flags;
            this.Typ = ParserTypeEnum.Unknown;
        }

        public override T Accept<T>(IParserTypeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equivalent(ParserType Other)
        {
            return true;
        }
    }

    public class ParserLogicType : ParserType
    {
        public ParserLogicType()
        {
            this.Flags = ParserTypeFlags.Logic;
            this.Typ = ParserTypeEnum.Logic;
        }

        public override T Accept<T>(IParserTypeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equivalent(ParserType Other)
        {
            if (Other.Typ == ParserTypeEnum.Logic)
                return true;
            else if (Other.Typ == ParserTypeEnum.Unknown)
            {
                if (Other.Flags == 0 || Other.Flags == ParserTypeFlags.Logic)
                    return true;
                else
                    return false;
            }
            else return false;
        }

        public override bool Compatible(ParserTypeFlags Flag)
        {
            return ((Flag & (~ ParserTypeFlags.Logic)) == 0);
        }

        public override string ToString()
        {

            return "Sigma";
        }
    }

    public class ParserNumType : ParserType
    {
        public ParserNumType()
        {
            this.Flags = ParserTypeFlags.Num;
            this.Typ = ParserTypeEnum.Number;
        }

        public override T Accept<T>(IParserTypeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equivalent(ParserType Other)
        {
            if (Other.Typ == ParserTypeEnum.Number)
                return true;
            else if (Other.Typ == ParserTypeEnum.Unknown)
            {
                if ((Other.Flags & (ParserTypeFlags.Logic | ParserTypeFlags.Pair | ParserTypeFlags.Lambda 
                    | ParserTypeFlags.Compact)) == 0)
                    return true;
                else
                    return false;
            }
            else return false;
        }

        public override bool Compatible(ParserTypeFlags Flag)
        {
            return ((Flag & (ParserTypeFlags.Logic | ParserTypeFlags.Pair | ParserTypeFlags.Lambda)) == 0);
        }

        public override string ToString()
        {

            return "Nat";
        }
    }

    public class ParserProdType : ParserType
    {
        public ParserType Left;
        public ParserType Right;

        public ParserProdType(ParserType Left, ParserType Right)
        {
            this.Flags = ParserTypeFlags.Pair;
            // inherit some of the flags from the children
            this.Flags = this.Flags | (Left.Flags & Right.Flags & ParserTypeFlags.PairInheritable);
            this.Left = Left;
            this.Right = Right;
            this.Typ = ParserTypeEnum.Pair;
        }

        public override T Accept<T>(IParserTypeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equivalent(ParserType Other)
        {
            if (Other.Typ == ParserTypeEnum.Pair)
            {
                var OtherPair = (ParserProdType)Other;
                return Left.Equivalent(OtherPair.Left) && Right.Equals(OtherPair.Right);
            }
            else if (Other.Typ == ParserTypeEnum.Unknown)
            {
                if ((Other.Flags & (~ParserTypeFlags.Pair)) == 0)
                    return true;
                else
                    return false;
            }
            else return false;
        }

        public override string ToString()
        {

            return "(" + Left.ToString() + " * " + Right.ToString() + ")";
        }
    }

    public class ParserLambdaType : ParserType
    {
        public ParserType InputType;

        public ParserLambdaType(ParserType InputType)
        {
            this.Flags = ParserTypeFlags.Lambda;
            this.InputType = InputType;
            this.Typ = ParserTypeEnum.Lambda | ParserTypeEnum.Logic;
        }

        public override T Accept<T>(IParserTypeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override bool Equivalent(ParserType Other)
        {
            if (Other.Typ == ParserTypeEnum.Lambda)
            {
                var OtherLambda = (ParserLambdaType)Other;
                return InputType.Equivalent(OtherLambda.InputType);
            }
            else if (Other.Typ == ParserTypeEnum.Unknown)
            {
                if ((Other.Flags & (~ParserTypeFlags.Lambda)) == 0)
                    return true;
                else
                    return false;
            }
            else return false;
        }

        public override string ToString()
        {

            return "[" + InputType.ToString() + ", Sigma]";
        }
    }
}
