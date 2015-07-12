using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncLogic.Parser;
using Sprache;

namespace AsyncLogicTest
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void TestTypeChecker()
        {
            string input = "3 + 4";
            var visitor = new TypeVisitor();
            Assert.AreEqual("Nat", ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString());

            input = "Lambda x:Nat. x == x";
            visitor = new TypeVisitor();
            Assert.AreEqual("[Nat, Sigma]", ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString());

            input = "T & F | (3 == 2)";
            visitor = new TypeVisitor();
            Assert.AreEqual("Sigma", ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString());

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTypeCheckerFailure()
        {
            var input = "2 & F";
            var visitor = new TypeVisitor();
            ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString();

            input = "T == F";
            visitor = new TypeVisitor();
            ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString();

            input = "T * 2";
            visitor = new TypeVisitor();
            ExpressionParser2.Expr.Parse(input).Accept(visitor).ToString();
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
