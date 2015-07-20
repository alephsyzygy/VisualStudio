using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic.Parser;
using Sprache;
using AsyncLogic.Expressions;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestToExpression()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
            {
                var parserExpr = ExpressionParser.Expr.Parse(input);
                var visitor = new ToExpressionVisitor();
                var output = visitor.Run(parserExpr);
                Assert.AreEqual(expected, output.ToString(), testNo.ToString());
                testNo++;
            };

            Test("3 + 4", "(3 + 4)");
            Test("Lambda x:Nat. x == x", "Lambda x. (x == x)");
            Test("T & F | (3 == 2)", "((T & F) | (3 == 2))");
            Test("Exists x:Nat. x == x","Exists x. (x == x)");
            Test("Rec(0,0,n.x:Nat.0)", "Rec(0, 0, n.x. 0)");
            Test("Rec(0,T,n.x:Sigma.x&T)", "Rec(0, T, n.x. (x & T))");
            Test("(Lambda x:Sigma. x) @ T", "(Lambda x. x @ T)");
            Test("<T,3>", "<T, 3>");
            Test("Lambda phi: Nat * [Nat,Sigma]. Rec(Fst phi,F,n.x:Sigma.x & (Snd phi)@(Fst phi))", 
                "Lambda phi. Rec(Fst phi, F, n.x. (x & (Snd phi @ Fst phi)))");


        }

        [TestMethod]
        public void TestTypeChecker()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
                {
                    var visitor = new TypeVisitor();
                    Assert.AreEqual(expected, ExpressionParser.Expr.Parse(input).Accept(visitor).ToString(), testNo.ToString());
                    testNo++;
                };

            Test("3 + 4", "Nat");
            Test("Lambda x:Nat. x == x" ,"[Nat, Sigma]");
            Test("T & F | (3 == 2)" ,"Sigma");
            Test("<T,3>" ,"(Sigma * Nat)");
            Test("Lambda x: Nat * Sigma. (Fst x) == 2" ,"[(Nat * Sigma), Sigma]");
            Test("(Lambda x: Nat. x == 3) @ 4" ,"Sigma");
            Test("Lambda phi: [Nat, Sigma]. Exists x:Nat. phi @ x" ,"[[Nat, Sigma], Sigma]");
            Test("(Lambda phi: [Nat, Sigma]. Exists x:Nat. phi @ x) @ (Lambda x: Nat. x == 2)" ,"Sigma");
            Test("Rec(0,0,n.x:Nat.x+1)" ,"Nat");
            Test("Rec(0,0,n.x:Nat.n+1)" ,"Nat");
            Test("Lambda phi: Nat * [Nat,Sigma]. Rec(Fst phi,F,n.x:Sigma.x & (Snd phi)@(Fst phi))" ,"[(Nat * [Nat, Sigma]), Sigma]");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTypeCheckerFailure()
        {
            
            Action<string> Test = (input) =>
            {
                var visitor = new TypeVisitor();
                ExpressionParser.Expr.Parse(input).Accept(visitor).ToString();
            };
            Test("2 & F");
            Test("T == F");
            Test("T * 2");
            Test("Lambda x:Nat. x == T");
            Test("(Lambda x:Nat. x == 3) @ T");
            Test("Rec(0,0,n.x:Nat.s+1)");

        }

        [TestMethod]
        public void TestSequentParser()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
                {
                    Assert.AreEqual(expected, SequentParser.ParseSequent.Parse(input).ToString(), testNo.ToString());
                    testNo++;
                };

            Test("x:Nat |- x", "x : Nat |- x");
        }

        [TestMethod]
        public void TestSecondParser()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
            {
                Assert.AreEqual(expected, ExpressionParser.Expr.Parse(input).ToString(),testNo.ToString());
                testNo++;
            };
            
            Test("x", "x");
            Test("x+y", "Add(x, y)");
            Test("Lambda x : Nat. x", "Lambda x : Nat. x");
            Test("Lambda x:Nat. Lambda y:Nat. x", "Lambda x : Nat. Lambda y : Nat. x");
            Test("x & y | z", "Or(And(x, y), z)");
            Test("(Lambda z:Nat. z) @ 3 == 4", "EQ(App(Lambda z : Nat. z, 3), 4)");
            Test("Lambda r:Sigma. r + 4 & 2", "Lambda r : Sigma. And(Add(r, 4), 2)");
            Test("<2,3>", "<2, 3>");
            Test("Snd<2,3>", "ProjR(<2, 3>)");
            Test("Rec(1,0,n.x:Sigma. n + x)", "Rec(1, 0, n.x : Sigma. Add(n, x))");
            Test("Lambda x:[Nat*Sigma,Sigma].x", "Lambda x : [(Nat * Sigma), Sigma]. x");
            Test("Lambda x:Nat*Sigma*Sigma.x", "Lambda x : ((Nat * Sigma) * Sigma). x");
        }
    }
}
