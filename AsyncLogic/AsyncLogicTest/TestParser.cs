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
        // This holds the testcontext
        public TestContext TestContext { get; set; }

        private static TestContext _testContext;

        // Store the testcontext when the tests are started
        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        public void ToExpression()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
            {
                var result = ExpressionParser.Expr.TryParse(input);
                if (!result.Remainder.AtEnd && result.WasSuccessful)
                    Assert.Fail("Parse not successful: " + testNo.ToString()  +  "; Output: " + result.Value.ToString() );
                if (!result.Remainder.AtEnd)
                    Assert.Fail("Parse not successful: " + testNo.ToString() /* +  "; Output: " + result.Value.ToString() */);
                var parserExpr = result.Value;
                var visitor = new ToExpressionVisitor();
                var output = visitor.Run(parserExpr);
                Assert.AreEqual(expected, output.ToString(), testNo.ToString());
                testNo++;
            };

            Test("3 + 4", "(3 + 4)");
            Test("Lambda x:Nat. x == x", "Lambda x. (x == x)");
            Test("True & False | (3 == 2)", "((True & False) | (3 == 2))");
            Test("Exists x:Nat. x == x","Exists x. (x == x)");
            Test("Rec(0,0,n.x:Nat.0)", "Rec(0, 0, n.x. 0)");
            Test("Rec(0,True,n.x:Sigma.x&True)", "Rec(0, True, n.x. (x & True))");
            Test("(Lambda x:Sigma. x) @ True", "(Lambda x. x @ True)");
            Test("<True,3>", "<True, 3>");
            Test("Lambda phi: Nat * [Nat,Sigma]. Rec(Fst phi,False,n.x:Sigma.x & (Snd phi)@(Fst phi))", 
                "Lambda phi. Rec(Fst phi, False, n.x. (x & (Snd phi @ Fst phi)))");
            Test("6 == 3", "(6 == 3)");
            Test("6 == (The n:Nat. (n == n))", "(6 == The n. (n == n))");
            Test("6 == (The n:Nat. ((n > 5) & Rec(n, True, m.x:Sigma. (x & (m <= 5)))))", "(6 == The n. ((n > 5) & Rec(n, True, m.x. (x & (m <= 5)))))");

        }

        [TestMethod]
        public void TypeChecker()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
                {
                    var visitor = new TypeVisitor();
                    var temp = ExpressionParser.Expr.TryParse(input);
                    Assert.AreEqual(expected, temp.Value.Accept(visitor).ToString(), testNo.ToString());
                    testNo++;
                };

            Test("3 + 4", "Nat");
            Test("Lambda x:Nat. x == x" ,"[Nat, Sigma]");
            Test("True & False | (3 == 2)", "Sigma");
            Test("<True,3>", "(Sigma * Nat)");
            Test("Lambda x: Nat * Sigma. (Fst x) == 2" ,"[(Nat * Sigma), Sigma]");
            Test("(Lambda x: Nat. x == 3) @ 4" ,"Sigma");
            Test("Lambda phi: [Nat, Sigma]. Exists x:Nat. phi @ x" ,"[[Nat, Sigma], Sigma]");
            Test("(Lambda phi: [Nat, Sigma]. Exists x:Nat. phi @ x) @ (Lambda x: Nat. x == 2)" ,"Sigma");
            Test("Rec(0,0,n.x:Nat.x+1)" ,"Nat");
            Test("Rec(0,0,n.x:Nat.n+1)" ,"Nat");
            Test("Lambda phi: Nat * [Nat,Sigma]. Rec(Fst phi,False,n.x:Sigma.x & (Snd phi)@(Fst phi))" ,"[(Nat * [Nat, Sigma]), Sigma]");
        }

        [TestMethod]
        [DeploymentItem("Data\\ParserFailure.csv")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "ParserFailure.csv", 
            "ParserFailure#csv", DataAccessMethod.Sequential)]
        [ExpectedException(typeof(ArgumentException))]
        public void TypeCheckerFailure()
        {
            var row = TestContext.DataRow;
            string expr = row["Expression"].ToString();
            
            var visitor = new TypeVisitor();
            ExpressionParser.Expr.Parse(expr).Accept(visitor).ToString();

        }

        [TestMethod]
        public void SequentParser()
        {
            int testNo = 0;
            Action<string, string> Test = (input, expected) =>
                {
                    Assert.AreEqual(expected, AsyncLogic.Parser.SequentParser.ParseSequent.Parse(input).ToString(), testNo.ToString());
                    testNo++;
                };

            Test("x:Nat |- x", "x : Nat |- x");
        }

        [TestMethod]
        public void SecondParser()
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
