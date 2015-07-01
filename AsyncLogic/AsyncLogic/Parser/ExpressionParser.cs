using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncLogic.Expressions;
using Sprache;

namespace AsyncLogic.Parser
{
    public static class ExpressionParser
    {
        public static readonly Parser<int> Number = Parse.Decimal.Token().Select(s => int.Parse(s));
        public static readonly Parser<string> Identifier = Parse.Identifier(Parse.Lower, Parse.Letter).Token();

        // operators and relations
        static Parser<T> Operator<T>(string op, T opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        public static readonly Parser<NumBinOps> Add = Operator("+", NumBinOps.Add);
        public static readonly Parser<NumBinOps> Mul = Operator("*", NumBinOps.Mul);
        public static readonly Parser<NumRels> NumEQ = Operator("==", NumRels.EQ);
        public static readonly Parser<NumRels> NumNEQ = Operator("!=", NumRels.NEQ);
        public static readonly Parser<NumRels> NumGT = Operator(">", NumRels.GT);
        public static readonly Parser<NumRels> NumLT = Operator("<", NumRels.LT);
        public static readonly Parser<NumRels> NumGTE = Operator(">=", NumRels.GTE);
        public static readonly Parser<NumRels> NumLTE = Operator("<=", NumRels.LTE);

        // Numeric constant
        public static readonly Parser<NumExpression> NumConstant = from number in Number 
                                                            select new NumConstant(number);

        // Logic constants
        public static readonly Parser<LogicExpression> True = Parse.String("T").Token().Return(new LogicTrue());
        public static readonly Parser<LogicExpression> False = Parse.String("F").Token().Return(new LogicFalse());

        // variables
        public static readonly Parser<NumExpression> NumVar = from ident in Identifier select new NumVariable(ident);
        public static readonly Parser<LogicExpression> LogicVar = from ident in Identifier select new LogicVariable(ident);
        public static readonly Parser<AbstractPairExpression> PairVar = from ident in Identifier select new PairVariable(ident);
        public static readonly Parser<AbstractLambdaExpression> LambdaVar = from ident in Identifier select new LambdaVariable(ident);

        //static readonly Parser<LogicExpression> NumRelation
        //    = Parse.ChainOperator(NumEQ.Or(NumNEQ).Or(NumGT).Or(NumLT).Or(NumGTE).Or(NumLTE), 
        //    NumTerm, (op, a, b) => new NumRelation(op, a, b));

        public static readonly Parser<NumExpression> NumThe
            = (from theString in Parse.String("The ")
               from var in Identifier
               from dot in Parse.Char('.')
               from expr in Parse.Ref(() => LogExpr)
               select new NumThe(var, expr)).Named("Num The");

        public static readonly Parser<NumExpression> NumOperand
            = (from lparen in Parse.Char('(')
               from expr in Parse.Ref(() => NumExpr)  // Parse.ref delays evaluation of NumExpr in case it is null
               from rparen in Parse.Char(')')
               select expr).Named("Num Expression")
             .XOr(NumThe)
             .XOr(NumConstant)
             .XOr(NumVar);

        public static readonly Parser<NumExpression> NumTerm
            = Parse.ChainOperator(Mul, NumOperand, (op, a, b) => new NumBinaryOp(op, a, b));

        public static readonly Parser<NumExpression> NumExpr
            = Parse.ChainOperator(Add, NumTerm, (op, a, b) => new NumBinaryOp(op, a, b)).Token();

        public static readonly Parser<NumRels> NumRel = NumEQ.Or(NumNEQ).Or(NumGT).Or(NumLT).Or(NumLTE).Or(NumGTE);

        public static readonly Parser<bool> LogAnd = Operator("&", true);
        public static readonly Parser<bool> LogOr = Operator("|", false);

        public static readonly Parser<LogicExpression> LogConstant = True.Or(False);

        public static readonly Parser<LogicExpression> LogNumRel
           = //(from lparen in Parse.Char('(')
              (from expr1 in Parse.Ref(() => NumExpr)
              from rel in NumRel
              from expr2 in Parse.Ref(() => NumExpr)
              //from rparen in Parse.Char(')')
              select new NumRelation(rel, expr1, expr2)).Named("Num Relation");

        public static readonly Parser<LogicExpression> LogExists
            = (from existString in Parse.String("Exists ").Token()
               from var in Identifier
               from dot in Parse.Char('.')
               from expr in Parse.Ref(() => LogExpr)
               select new NumExists(var, expr)).Named("Num Exists");
               //.Token();

        public static readonly Parser<LogicExpression> LogOperand
            = (from lparen in Parse.Char('(')
               from expr in Parse.Ref(() => LogExpr)  // Parse.ref delays evaluation of NumExpr in case it is null
               from rparen in Parse.Char(')')
               select expr).Named("Logic Expression")
              //.XOr(LogNumRel)
              .XOr(LogExists)
              .XOr(LogConstant)
              .XOr(LogicVar)
              .Token();

        public static readonly Parser<LogicExpression> LogTerm
            = Parse.ChainOperator(LogAnd, LogOperand, (ignore, a, b) => new LogicAnd(a, b));

        // 'or' has lower precedence than 'and'
        public static readonly Parser<LogicExpression> LogExpr2
            = Parse.ChainOperator(LogOr, LogTerm, (ignore, a, b) => new LogicOr(a, b));

        public static readonly Parser<LogicExpression> LogExpr = LogNumRel.Or(LogExpr2);
    }


    // TODO: 
    //  Logic: Apply
    //  Lambda: Lambda
    //  Pair: Pair, ProjL, ProjR
    //  Rec: All
}
