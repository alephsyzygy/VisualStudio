using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;
using Sprache;

namespace AsyncLogic.Parser
{
    public static class TypeParser
    {
        public static readonly Parser<ParserType> Sigma = Parse.String("Sigma").Token().Return(new ParserLogicType());
        public static readonly Parser<ParserType> Nat   = Parse.String("Nat").Token().Return(new ParserNumType());

        public static readonly Parser<ParserType> Pair
            = (from lparen in Parse.Char('(')
               from left in Parse.Ref(() => ParseType)
               from comma in Parse.Char(',')
               from right in Parse.Ref(() => ParseType)
               from rparen in Parse.Char(')')
               select new ParserProdType(left, right)).Named("Product Type");

        public static readonly Parser<ParserType> Exp
            = (from lparen in Parse.Char('[')
               from expr in Parse.Ref(() => ParseType)
               from comma in Parse.Char(',')
               from sigma in Sigma
               from rparen in Parse.Char(']')
               select new ParserLambdaType(expr)).Named("Lambda Type")
               .XOr(from lparen in Parse.Char('(')
                    from expr in Parse.Ref(() => ParseType)
                    from rparen in Parse.Char(')')
                    select expr)
               .XOr(Sigma)
               .XOr(Nat)
               .Token();

        public static readonly Parser<ParserType> ParseType = Parse.ChainOperator(Parse.String("*").Token().Return(true),
            Exp, (op,a,b) => new ParserProdType(a,b));
    }
    public static class ExpressionParser
    {
        public static readonly Parser<int> Number = Parse.Decimal.Token().Select(s => int.Parse(s));
        public static readonly Parser<string> Identifier = Parse.Identifier(Parse.Lower, Parse.Letter).Token();

        // operators and relations
        static Parser<T> Operator<T>(string op, T opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        // Binary operation types
        public static readonly Parser<ParserBinaryOpType> Add = Operator("+",  ParserBinaryOpType.Add);
        public static readonly Parser<ParserBinaryOpType> Mul = Operator("*",  ParserBinaryOpType.Mul);
        public static readonly Parser<ParserBinaryOpType> EQ  = Operator("==", ParserBinaryOpType.EQ);
        public static readonly Parser<ParserBinaryOpType> NEQ = Operator("!=", ParserBinaryOpType.NEQ);
        public static readonly Parser<ParserBinaryOpType> GT  = Operator(">",  ParserBinaryOpType.GT);
        public static readonly Parser<ParserBinaryOpType> LT  = Operator("<",  ParserBinaryOpType.LT);
        public static readonly Parser<ParserBinaryOpType> GTE = Operator(">=", ParserBinaryOpType.GTE);
        public static readonly Parser<ParserBinaryOpType> LTE = Operator("<=", ParserBinaryOpType.LTE);
        public static readonly Parser<bool> App               = Operator("@",  true);
        public static readonly Parser<ParserBinaryOpType> And = Operator("&",  ParserBinaryOpType.And);
        public static readonly Parser<ParserBinaryOpType> Or  = Operator("|",  ParserBinaryOpType.Or);

        // Numeric constant
        public static readonly Parser<ParserExpression> NumConstant = from number in Number
                                                                   select new ParserNumConstant(number);

        // Logic constants
        public static readonly Parser<ParserExpression> True = Parse.String("T").Token().Return(new ParserStringConstant("T"));
        public static readonly Parser<ParserExpression> False = Parse.String("F").Token().Return(new ParserStringConstant("F"));

        // variables
        public static readonly Parser<ParserExpression> Var = from ident in Identifier select new ParserVariable(ident);

        public static readonly Parser<ParserBinderType> BinderType
            = Parse.String("The ").Return(ParserBinderType.The)
            .XOr(Parse.String("Exists ").Return(ParserBinderType.Exists))
            .XOr(Parse.String("Lambda ").Return(ParserBinderType.Lambda));

        public static readonly Parser<ParserExpression> Binder
            = (from binderType in BinderType
               from var in Identifier
               from colon in Parse.Char(':')
               from type in Parse.Ref(() => TypeParser.ParseType)
               from dot in Parse.Char('.')
               from expr in Parse.Ref(() => Expr)
               select new ParserBinder(binderType,var, type, expr)).Named("Binder");

        public static readonly Parser<ParserUnaryOpType> UnaryType
            = Parse.String("Fst").Return(ParserUnaryOpType.ProjL)
            .XOr(Parse.String("Snd").Return(ParserUnaryOpType.ProjR));

        public static readonly Parser<ParserExpression> ParseUnary
            = (from type in UnaryType
               from expr in Parse.Ref(() => Expr)
               select new ParserUnaryOp(type, expr)).Named("Unary");

        public static readonly Parser<ParserExpression> Pair
            = (from lparen in Parse.Char('<')
               from left in Parse.Ref(() => Expr)
               from comma in Parse.Char(',')
               from right in Parse.Ref(() => Expr)
               from rparen in Parse.Char('>')
               select new ParserPair(left, right)).Named("Pair");

        public static readonly Parser<ParserExpression> Rec
            = (from rec in Parse.String("Rec").Token()
               from lparen in Parse.Char('(')
               from input in Parse.Ref(() => Expr)
               from comma1 in Parse.Char(',')
               from start in Parse.Ref(() => Expr)
               from comma2 in Parse.Char(',')
               from numvar in Identifier
               from dot1 in Parse.Char('.')
               from accvar in Identifier
               from colon in Parse.Char(':')
               from type in Parse.Ref(() => TypeParser.ParseType)
               from dot2 in Parse.Char('.')
               from step in Parse.Ref(() => Expr)
               from rparen in Parse.Char(')')
               select new ParserRec(input, start, numvar, accvar, type, step)).Named("Rec");


        public static readonly Parser<ParserExpression> Operand
            = (from lparen in Parse.Char('(')
               from expr in Parse.Ref(() => Expr)  // Parse.ref delays evaluation of NumExpr in case it is null
               from rparen in Parse.Char(')')
               select expr).Named("Expression")
             .XOr(True)
             .XOr(Pair)
             .XOr(Rec)
             .XOr(ParseUnary.Or(False)) // Both involve the letter F
             .XOr(NumConstant)
             .XOr(Var)
             .Token();

        public static readonly Parser<ParserExpression> ParseApp
            = Parse.ChainOperator(App, Operand, (op, a, b) => new ParserApp(a, b));
        public static readonly Parser<ParserExpression> ParseMul
            = Parse.ChainOperator(Mul, ParseApp, (op, a, b) => new ParserBinaryOp(op, a, b));
        public static readonly Parser<ParserExpression> ParseAdd
            = Parse.ChainOperator(Add, ParseMul, (op, a, b) => new ParserBinaryOp(op, a, b));

        public static readonly Parser<ParserExpression> ParseOrder
            = Parse.ChainOperator(LT.Or(GT).Or(LTE).Or(GTE), ParseAdd, (op, a, b) => new ParserBinaryOp(op, a, b));
        public static readonly Parser<ParserExpression> ParseEQ
            = Parse.ChainOperator(EQ.Or(NEQ), ParseOrder, (op, a, b) => new ParserBinaryOp(op, a, b));

        public static readonly Parser<ParserExpression> ParseAnd
            = Parse.ChainOperator(And, ParseEQ, (op, a, b) => new ParserBinaryOp(op, a, b));
        public static readonly Parser<ParserExpression> ParseOr
            = Parse.ChainOperator(Or, ParseAnd, (op, a, b) => new ParserBinaryOp(op, a, b));

        public static readonly Parser<ParserExpression> Expr
            = Binder
            .Or(ParseOr)
            .Token();

    }

    public static class SequentParser
    {
        public static readonly Parser<Tuple<string, ParserType>> ParsePair
            = (from ident in ExpressionParser.Identifier
               from colon in Parse.Char(':')
               from type in TypeParser.ParseType
               select new Tuple<string, ParserType>(ident, type)).Named("Variable declaration").Token();

        public static readonly Parser<Dictionary<string, ParserType>> ParseContext
            = (from pair in ParsePair
               from comma in Parse.Char(',')
               from ctx in Parse.Ref(() => ParseContext)
               select new Func<Dictionary<string, ParserType>>(() => { ctx.Add(pair.Item1, pair.Item2); return ctx; })())
               .Or(from pair in ParsePair
                   select new Func<Dictionary<string, ParserType>>
                   (() => { Dictionary<string,ParserType> x = new Dictionary<string, ParserType>();
                       x.Add(pair.Item1, pair.Item2);
                       return x; }) ());


        public static readonly Parser<Sequent> ParseSequent
            = (from ctx in ParseContext
               from symb in Parse.String("|-")
               from expr in ExpressionParser.Expr
               select new Sequent(ctx, expr)).Named("Sequent");
    }
}
