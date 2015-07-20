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
                var parserExpr = ExpressionParser2.Expr.Parse(input);
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
            Action<string, string> Test = (input, expected) =>
                {
                    var visitor = new TypeVisitor();
                    Assert.AreEqual(expected, ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString());
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
                ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString();
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
            string input = "x:Nat |- x";
            Assert.AreEqual("x : Nat |- x", SequentParser.ParseSequent.Parse(input).ToString());
        }

        [TestMethod]
        public void TestSecondParser()
        {
            string input = "x";
            var result = ExpressionParser2.Expr.Parse(input);
            Assert.AreEqual("x", result.ToString());

            Assert.AreEqual("Add(x, y)", ExpressionParser2.Expr.Parse("x+y").ToString());
            Assert.AreEqual("Lambda x : Nat. x", ExpressionParser2.Expr.Parse("Lambda x : Nat. x").ToString(), "2");
            Assert.AreEqual("Lambda x : Nat. Lambda y : Nat. x", ExpressionParser2.Expr.Parse("Lambda x:Nat. Lambda y:Nat. x").ToString(), "3");
            Assert.AreEqual("Or(And(x, y), z)", ExpressionParser2.Expr.Parse("x & y | z").ToString());
            Assert.AreEqual("EQ(App(Lambda z : Nat. z, 3), 4)", ExpressionParser2.Expr.Parse("(Lambda z:Nat. z) @ 3 == 4").ToString());
            Assert.AreEqual("Lambda r : Sigma. And(Add(r, 4), 2)", ExpressionParser2.Expr.Parse("Lambda r:Sigma. r + 4 & 2").ToString());
            Assert.AreEqual("<2, 3>", ExpressionParser2.Expr.Parse("<2,3>").ToString());
            Assert.AreEqual("ProjR(<2, 3>)", ExpressionParser2.Expr.Parse("Snd<2,3>").ToString());
            Assert.AreEqual("Rec(1, 0, n.x : Sigma. Add(n, x))", ExpressionParser2.Expr.Parse("Rec(1,0,n.x:Sigma. n + x)").ToString());
            Assert.AreEqual("Lambda x : [(Nat * Sigma), Sigma]. x", ExpressionParser2.Expr.Parse("Lambda x:[Nat*Sigma,Sigma].x").ToString());
            Assert.AreEqual("Lambda x : ((Nat * Sigma) * Sigma). x", ExpressionParser2.Expr.Parse("Lambda x:Nat*Sigma*Sigma.x").ToString());
        }

        [TestMethod]
        public void TestLambdaParser()
        {
            string input = "Lambda x. (x == x)";
            var result = ExpressionParser.LambdaExpr.Parse(input);
            Assert.AreEqual("Lambda x. (x == x)", result.ToString());

            input = "(Lambda x. x == x) @ 3";
            var result2 = ExpressionParser.LogExpr.Parse(input);
            Assert.AreEqual("(Lambda x. (x == x) @ 3)", result2.ToString());

            input = "(Lambda x. Lambda y. x == x @ 4) @ 3";
            result2 = ExpressionParser.LogExpr.Parse(input);
            Assert.AreEqual("(Lambda x. (Lambda y. (x == x) @ 4) @ 3)", result2.ToString());
        }

        [TestMethod]
        public void TestNumParser()
        {
            string input = "345";
            var output = ExpressionParser.Number.Parse(input);
            Assert.AreEqual(345, output);

            input = "((56 + 3) * (8))";
            var numExp = ExpressionParser.NumExpr.Parse(input);
            var result = ExpressionParser.NumExpr.TryParse(input);
            
            Assert.AreEqual("((56 + 3) * 8)", numExp.ToString());

            input = " (( 5 + x) * (y+6 ))";
            numExp = ExpressionParser.NumExpr.Parse(input);
            Assert.AreEqual("((5 + x) * (y + 6))", numExp.ToString());

            input = " 3 + 4 + 5";
            Assert.AreEqual("((3 + 4) + 5)", ExpressionParser.NumExpr.Parse(input).ToString());

            input = "The n. n == n + 3";
            Assert.AreEqual("The n. (n == (n + 3))", ExpressionParser.NumExpr.Parse(input).ToString());



            input = "The n. Exists m. n == m";
            Assert.AreEqual("The n. Exists m. (n == m)", ExpressionParser.NumExpr.Parse(input).ToString(), "the exist");
        }

        [TestMethod]
        public void TestLogParser()
        {
            string input = "x | T";
            Assert.AreEqual("(x | T)", ExpressionParser.LogExpr.Parse(input).ToString());



            input = "x & T | y";
            Assert.AreEqual("((x & T) | y)", ExpressionParser.LogExpr.Parse(input).ToString());

            input = "Exists n. n == n";
            Assert.AreEqual("Exists n. (n == n)", ExpressionParser.LogExpr.Parse(input).ToString());

            input = "Exists n. (The n . n == 2) == 3";
            Assert.AreEqual("Exists n. (The n. (n == 2) == 3)", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the");

            input = "Exists n. The n . n == 2 == 3";
            Assert.AreEqual("Exists n. (The n. (n == 2) == 3)", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the2");


        }

        [TestMethod]
        public void TestDoubleExists()
        {
            string input = "Exists n. Exists m. T";
            Assert.AreEqual("Exists n. Exists m. T", ExpressionParser.LogExpr.Parse(input).ToString(), "exist the");
        }
    }
}
