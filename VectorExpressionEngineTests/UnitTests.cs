using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorExpressionEngine;
using MSTestExtensions;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TokenizerTest()
        {
            var testString = "10 + 20 - 30.123 * 1.1e10";
            var t = new Tokenizer(new StringReader(testString));

            // "10"
            Assert.AreEqual(t.Token, Token.Number);
            Assert.AreEqual(t.Number, 10);
            t.NextToken();

            // "+"
            Assert.AreEqual(t.Token, Token.Add);
            t.NextToken();

            // "20"
            Assert.AreEqual(t.Token, Token.Number);
            Assert.AreEqual(t.Number, 20);
            t.NextToken();

            // "-"
            Assert.AreEqual(t.Token, Token.Subtract);
            t.NextToken();

            // "30.123"
            Assert.AreEqual(t.Token, Token.Number);
            Assert.AreEqual(t.Number, 30.123);
            t.NextToken();

            // "*"
            Assert.AreEqual(t.Token, Token.Multiply);
            t.NextToken();

            // "1e10"
            Assert.AreEqual(t.Token, Token.Number);
            Assert.AreEqual(t.Number, 1.1e10);
            t.NextToken();

            testString = "< <= > >= == != || && !1 < =";
            t = new Tokenizer(new StringReader(testString));

            Assert.AreEqual(t.Token, Token.Lesser); t.NextToken();
            Assert.AreEqual(t.Token, Token.LesserOrEqual); t.NextToken();
            Assert.AreEqual(t.Token, Token.Greater); t.NextToken();
            Assert.AreEqual(t.Token, Token.GreaterOrEqual); t.NextToken();
            Assert.AreEqual(t.Token, Token.Equal); t.NextToken();
            Assert.AreEqual(t.Token, Token.NotEqual); t.NextToken();
            Assert.AreEqual(t.Token, Token.LogicalOr); t.NextToken();
            Assert.AreEqual(t.Token, Token.LogicalAnd); t.NextToken();
            Assert.AreEqual(t.Token, Token.LogicalNot); t.NextToken();
            Assert.AreEqual(t.Token, Token.Number); t.NextToken();
            Assert.AreEqual(t.Token, Token.Lesser); t.NextToken();
            Assert.AreEqual(t.Token, Token.Assignment); t.NextToken();
            Assert.AreEqual(t.Token, Token.EndOfFile);


            Assert.AreEqual(Parser.ParseSingle("1").Eval(null), 1.0);
            Assert.AreEqual(Parser.ParseSingle("1.0").Eval(null), 1.0);
            Assert.AreEqual(Parser.ParseSingle(".0").Eval(null), 0.0);
            Assert.AreEqual(Parser.ParseSingle("-.0").Eval(null), 0.0);
            Assert.AreEqual(Parser.ParseSingle("-1.0").Eval(null), -1.0);
            Assert.AreEqual(Parser.ParseSingle("-1.0e10").Eval(null), -1.0e10);
            Assert.AreEqual(Parser.ParseSingle("1.0e10").Eval(null), 1.0e10);
            Assert.AreEqual(Parser.ParseSingle("1.e-10").Eval(null), 1.0e-10);
            Assert.AreEqual(Parser.ParseSingle("1.0e-10").Eval(null), 1.0e-10);
            Assert.AreEqual(Parser.ParseSingle("-1.0e-10").Eval(null), -1.0e-10);
            Assert.AreEqual(Parser.ParseSingle("-.0e-10").Eval(null), 0.0);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("1e").Eval(null), "could not parse number: '1e'");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("1e1.1").Eval(null), "Unexpected characters at end of expression");


            Assert.AreEqual(Parser.ParseSingle("\"abc\"").Eval(null), "abc");
            Assert.AreEqual(Parser.ParseSingle("'abc'").Eval(null), "abc");

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("'abc").Eval(null), "Unterminated string constant");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("\"abc").Eval(null), "Unterminated string constant");

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true & false").Eval(null), "unexpected character after '&' sign");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true | false").Eval(null), "unexpected character after '|' sign");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true @ false").Eval(null), "unexpected character '@'");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("+ +"), "Unexpected token: EndOfFile");
        }

        [TestMethod]
        public void AddSubtractTest()
        {
            Assert.AreEqual(Parser.ParseSingle("10 + 20").Eval(null), 30.0);
            Assert.AreEqual(Parser.ParseSingle("10 - 20").Eval(null), -10.0);
            Assert.AreEqual(Parser.ParseSingle("10 + 20 - 40 + 100").Eval(null), 90.0);

            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] + 1").Eval(null)).SequenceEqual(new double[] { 2, 3 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("1 + [1, 2]").Eval(null)).SequenceEqual(new double[] { 2, 3 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] + [1, 2]").Eval(null)).SequenceEqual(new double[] { 2, 4 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] + [1]").Eval(null)).SequenceEqual(new double[] { 2, 3 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[1] + [1, 2]").Eval(null)).SequenceEqual(new double[] { 2, 3 }));

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, 2] + [1, 2, 3]").Eval(null), "binary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true + [1, 2, 3]").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, 2, 3] + true").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("1 + true").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true + 1").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);

            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] - 1").Eval(null)).SequenceEqual(new double[] { 0, 1 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("1 - [1, 2]").Eval(null)).SequenceEqual(new double[] { 0, -1 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] - [1, 2]").Eval(null)).SequenceEqual(new double[] { 0, 0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2] - [1]").Eval(null)).SequenceEqual(new double[] { 0, 1 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[1] - [1, 2]").Eval(null)).SequenceEqual(new double[] { 0, -1 }));

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, 2] - [1, 2, 3]").Eval(null), "binary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true - [1, 2, 3]").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, 2, 3] - true").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("1 - true").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true - 1").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);

        }

        [TestMethod]
        public void ArrayTest()
        {
            Assert.IsTrue(((double[])Parser.ParseSingle("[1, 2]").Eval(null)).SequenceEqual(new double[] { 1, 2 }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("[true, false]").Eval(null)).SequenceEqual(new [] { true, false }));
            Assert.IsTrue(((string[])Parser.ParseSingle("[\"a\", 'b']").Eval(null)).SequenceEqual(new [] { "a", "b" }));

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[ ]").Eval(null), "invalid array definition - array cannot be empty");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, true]").Eval(null), "invalid array definition - element types not compatible");
        }

        [TestMethod]
        public void UnaryTest()
        {
            Assert.AreEqual(Parser.ParseSingle("-10").Eval(null), -10.0);
            Assert.AreEqual(Parser.ParseSingle("+10").Eval(null), 10.0);
            Assert.AreEqual(Parser.ParseSingle("--10").Eval(null), 10.0);
            Assert.AreEqual(Parser.ParseSingle("--++-+-10").Eval(null), 10.0);
            Assert.AreEqual(Parser.ParseSingle("10 + -20 - +30").Eval(null), -40.0);

            Assert.IsTrue(((double[])Parser.ParseSingle("-[1, 2]").Eval(null)).SequenceEqual(new double[] { -1, -2 }));
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("-true").Eval(null), "unary operation cannot handle type", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
        }

        [TestMethod]
        public void MultiplyDivideTest()
        {
            Assert.AreEqual(Parser.ParseSingle("10 * 20").Eval(null), 200.0);
            Assert.AreEqual(Parser.ParseSingle("10 / 20").Eval(null), 0.5);
            Assert.AreEqual(Parser.ParseSingle("10 * 20 / 50").Eval(null), 4.0);
        }

        [TestMethod]
        public void ExponentiateTest()
        {
            Assert.AreEqual(Parser.ParseSingle("10 ^ 2").Eval(null), 100.0);
            Assert.AreEqual(Parser.ParseSingle("10^2^2").Eval(null), 10000.0);
            Assert.AreEqual(Parser.ParseSingle("10^-2").Eval(null), 0.01);
            Assert.AreEqual(Parser.ParseSingle("-10^---2").Eval(null), -0.01);

            Assert.AreEqual(Parser.ParseSingle("-5^2").Eval(null), -25.0);
            Assert.AreEqual(Parser.ParseSingle("(-5)^2").Eval(null), 25.0);
            Assert.AreEqual(Parser.ParseSingle("(-5.5)^(-2.2)").Eval(null), double.NaN);

            Assert.AreEqual(Parser.ParseSingle("[1,2,3][1]").Eval(null), 2.0);
        }

        [TestMethod]
        public void TernaryTest()
        {
            Assert.AreEqual(Parser.ParseSingle("true ? 10 - 5 : 20").Eval(null), 5.0);
            Assert.AreEqual(Parser.ParseSingle("false ? 10 : 20").Eval(null), 20.0);
            Assert.AreEqual(Parser.ParseSingle("1 < 2 ? 10 : 20").Eval(null), 10.0);

            Assert.IsTrue(((double[])Parser.ParseSingle("true ? 10 : [10, 20]").Eval(null)).SequenceEqual(new [] { 10.0, 10.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("false ? 10 : [10, 20]").Eval(null)).SequenceEqual(new [] { 10.0, 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("true ? [10, 11] : 20").Eval(null)).SequenceEqual(new [] { 10.0, 11.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("false ? [10, 11] : 20").Eval(null)).SequenceEqual(new [] { 20.0, 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("true ? [10, 11] : [10, 20]").Eval(null)).SequenceEqual(new [] { 10.0, 11.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("false ? [10, 11] : [10, 20]").Eval(null)).SequenceEqual(new [] { 10.0, 20.0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[true, false] ? [10, 11] : [12, 20]").Eval(null)).SequenceEqual(new [] { 10.0, 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[false, true] ? [10, 11] : [12, 20]").Eval(null)).SequenceEqual(new [] { 12.0, 11.0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[false, true] ? 10 : [12, 20]").Eval(null)).SequenceEqual(new [] { 12.0, 10.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[false, true] ? [12, 20] : 10").Eval(null)).SequenceEqual(new [] { 10.0, 20.0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[false, true] ? 20 : 10").Eval(null)).SequenceEqual(new [] { 10.0, 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true, false] ? 20 : 10").Eval(null)).SequenceEqual(new [] { 20.0, 10.0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[true, false] ? [20] : 10").Eval(null)).SequenceEqual(new [] { 20.0, 10.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true, false] ? 20 : [10]").Eval(null)).SequenceEqual(new [] { 20.0, 10.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true] ? 20 : [10]").Eval(null)).SequenceEqual(new [] { 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true] ? [20] : 10").Eval(null)).SequenceEqual(new [] { 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true] ? [20] : [10]").Eval(null)).SequenceEqual(new [] { 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("true ? [20, 11] : [10]").Eval(null)).SequenceEqual(new [] { 20.0, 11.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("false ? [20] : [10]").Eval(null)).SequenceEqual(new [] { 10.0 }));

            Assert.IsTrue(((double[])Parser.ParseSingle("[true, true] ? [20] : [10, 11]").Eval(null)).SequenceEqual(new [] { 20.0, 20.0 }));
            Assert.IsTrue(((double[])Parser.ParseSingle("[true, true] ? [20, 21] : [10]").Eval(null)).SequenceEqual(new [] { 20.0, 21.0 }));


            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, false] ? [20, 11, 12] : 10").Eval(null), "ternary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, false] ? 10 : [20, 11, 12]").Eval(null), "ternary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true ? [10, 9] : [20, 11, 12]").Eval(null), "ternary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? [10, 9] : [20, 11, 12]").Eval(null), "ternary operation of different length arrays not defined");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? [10, 9, 8] : [20, 11]").Eval(null), "ternary operation of different length arrays not defined");

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true ? 1 : 'text'").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true ? 'text' : 1").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true ? [20, 11] : 'text'").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("true ? 'text' : [20, 11]").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("'text' ? [20, 11] : 'text'").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? 1 : 'text'").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? 'text' : 1").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? [20, 11] : 'text'").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[true, true] ? 'text' : [20, 11]").Eval(null), "ternary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
        }

        [TestMethod]
        public void LogicalOperationTest()
        {
            Assert.AreEqual(Parser.ParseSingle("true && false").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("true && true").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("false && false").Eval(null), false);

            Assert.AreEqual(Parser.ParseSingle("true || false").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("true || true").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("false || false").Eval(null), false);

            Assert.AreEqual(Parser.ParseSingle("!true").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("!false").Eval(null), true);

            Assert.AreEqual(Parser.ParseSingle("true && !true").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("!false || false").Eval(null), true);

            Assert.AreEqual(Parser.ParseSingle("true == false").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("true == true").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("false == false").Eval(null), true);

            Assert.IsTrue(((bool[])Parser.ParseSingle("[false, true] == false").Eval(null)).SequenceEqual(new [] { true, false }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("[false, false] == false").Eval(null)).SequenceEqual(new [] { true, true }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("false == [false, true]").Eval(null)).SequenceEqual(new [] { true, false }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("false == [false, false]").Eval(null)).SequenceEqual(new [] { true, true }));


            Assert.AreEqual(Parser.ParseSingle("true != false").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("true != true").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("false != false").Eval(null), false);

            Assert.IsTrue(((bool[])Parser.ParseSingle("[false, true] != false").Eval(null)).SequenceEqual(new [] { false, true }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("[false, false] != false").Eval(null)).SequenceEqual(new [] { false, false }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("false != [false, true]").Eval(null)).SequenceEqual(new [] { false, true }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("false != [false, false]").Eval(null)).SequenceEqual(new [] { false, false }));

            Assert.AreEqual(Parser.ParseSingle("1.0 == 0.0").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("1.0 == 1.0").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("0.0 == 0.0").Eval(null), true);

            Assert.AreEqual(Parser.ParseSingle("1.0 != 0.0").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("1.0 != 1.0").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("0.0 != 0.0").Eval(null), false);

            Assert.AreEqual(Parser.ParseSingle("1 < 0").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("1 < 1").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("0 < 1").Eval(null), true);

            Assert.AreEqual(Parser.ParseSingle("1 <= 0").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("1 <= 1").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("0 <= 1").Eval(null), true);

            Assert.AreEqual(Parser.ParseSingle("1 > 0").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("1 > 1").Eval(null), false);
            Assert.AreEqual(Parser.ParseSingle("0 > 1").Eval(null), false);

            Assert.AreEqual(Parser.ParseSingle("1 >= 0").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("1 >= 1").Eval(null), true);
            Assert.AreEqual(Parser.ParseSingle("0 >= 1").Eval(null), false);

            Assert.IsTrue(((bool[])Parser.ParseSingle("[1, 2, 3] >= [1, 1, 4]").Eval(null)).SequenceEqual(new [] { true, true, false }));
            Assert.IsTrue(((bool[])Parser.ParseSingle("[1, 2, 3] >= 2").Eval(null)).SequenceEqual(new [] { false, true, true }));

        }

        [TestMethod]
        public void StringOperations()
        {
            Assert.AreEqual(Parser.ParseSingle("'test ' + 'string'").Eval(null), "test string");
            Assert.AreEqual(Parser.ParseSingle("'test ' + 10").Eval(null), "test 10");
            Assert.AreEqual(Parser.ParseSingle("10 + 'test '").Eval(null), "10test ");

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("'test' + true").Eval(null), "binary operation cannot handle types", ExceptionMessageCompareOptions.Contains, ExceptionInheritanceOptions.Exact);
        }

        [TestMethod]
        public void OrderOfOperation()
        {
            Assert.AreEqual(Parser.ParseSingle("10 + 20 * 30").Eval(null), 610.0);
            Assert.AreEqual(Parser.ParseSingle("(10 + 20) * 30").Eval(null), 900.0);
            Assert.AreEqual(Parser.ParseSingle("-(10 + 20) * 30").Eval(null), -900.0);
            Assert.AreEqual(Parser.ParseSingle("-((10 + 20) * 5) * 30").Eval(null), -4500.0);

            Assert.IsTrue(((double[])Parser.ParseSingle("[1,2,3]").Eval(null)).SequenceEqual(new double[] { 1,2,3 }));
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("[1, 2").Eval(null), "Missing close bracket");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("(1 + 2").Eval(null), "Missing close parenthesis");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("func(1, 2").Eval(null), "Missing close parenthesis");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("1 ? 2 + 1").Eval(null), "missing ternary else operator");
        }

        private class MyContext : Context
        {
            public MyContext(double r)
            {
                _r = r;
            }

            private readonly double _r;

            public override object ResolveVariable(string name)
            {
                switch (name)
                {
                    case "pi": return Math.PI;
                    case "r": return _r;
                }

                throw new SyntaxException($"Unknown variable: '{name}'");
            }

            public override object CallFunction(string name, object[] arguments)
            {
                throw new InvalidOperationException();
            }

            public override void AssignVariable(string name, object value)
            {
                throw new InvalidOperationException();
            }
        }

        [TestMethod]
        public void Variables()
        {
            var ctx = new MyContext(10);

            Assert.AreEqual(Parser.ParseSingle("2 * pi * r").Eval(ctx), 2 * Math.PI * 10);
        }

        private class MyFunctionContext : Context
        {
            public override object ResolveVariable(string name)
            {
                throw new InvalidDataException($"Unknown variable: '{name}'");
            }

            public override object CallFunction(string name, object[] arguments)
            {
                if (name == "rectArea")
                {
                    return (double)arguments[0] * (double)arguments[1];
                }

                if (name == "rectPerimeter")
                {
                    return ((double)arguments[0] + (double)arguments[1]) * 2;
                }

                throw new InvalidDataException($"Unknown function: '{name}'");
            }

            public override void AssignVariable(string name, object value)
            {
                throw new InvalidOperationException();
            }
        }

        [TestMethod]
        public void Functions()
        {
            var ctx = new MyFunctionContext();
            Assert.AreEqual(Parser.ParseSingle("rectArea(10,20)").Eval(ctx), 200.0);
            Assert.AreEqual(Parser.ParseSingle("rectPerimeter(10,20)").Eval(ctx), 60.0);
        }

        private class MyLibrary
        {
            public ReflectionContext Context { get; set; }

            [Expression("pi", isConstant: true)] public static double Pi => Math.PI;

            [Expression("r", isConstant: false)] public static double R => 10;

            [Expression("rectArea")]
            public double RectArea(double width, double height)
            {
                return width * height;
            }

            [Expression("rectArea", isConstant: true)]
            [SuppressMessage("Style", "IDE0060:Remove unused parameter")]
            public static string RectArea(double width, string height)
            {
                return "makes no sense";
            }

            [Expression("rectPerimeter", isConstant: true)]
            public double RectPerimeter(double width, double height)
            {
                return (width + height) * 2;
            }

            [Expression("clear")]
            public void Clear()
            {
                Context?.Reset();
            }
        }

        [TestMethod]
        public void Reflection()
        {
            // Create a library of helper function
            var lib = new MyLibrary();

            // Create a context that uses the library
            var ctx = new ReflectionContext(lib);
            lib.Context = ctx;

            // Test
            Assert.AreEqual(Parser.ParseSingle("rectArea(10,20)").Eval(ctx), 200.0);
            Assert.AreEqual(Parser.ParseSingle("rectPerimeter(10,20)").Eval(ctx), 60.0);
            Assert.AreEqual(Parser.ParseSingle("2 * pi * r").Eval(ctx), 2 * Math.PI * 10);
            Assert.AreEqual(Parser.ParseSingle("rectArea(10,'test')").Eval(ctx), "makes no sense");
            Assert.IsNull(Parser.ParseSingle("clear()").Eval(ctx));
            Assert.IsNull(Parser.ParseSingle("clear").Eval(ctx));
            Assert.AreEqual(Parser.ParseSingle("clear = 1").Eval(ctx), 1.0);
            Assert.AreEqual(Parser.ParseSingle("clear").Eval(ctx), 1.0);
            Assert.IsNull(Parser.ParseSingle("clear()").Eval(ctx));
            Assert.IsNull(Parser.ParseSingle("clear").Eval(ctx));

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("unknown").Eval(ctx), "Unknown variable or function: 'unknown'");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("unknown('x')").Eval(ctx), "No function 'unknown' found for arguments of type 'String'");
            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("rectArea('x', 'x')").Eval(ctx), "No function 'rectArea' found for arguments of type 'String, String'");
        }

        class MyLibraryTyped : BasicOperations
        {
            [Expression]
            public object S(string name)
            {
                if (name == "S1")
                {
                    return new double[] { 1, 2, 3, 4, 5 };
                }
                
                if (name == "S2")
                {
                    return new double[] { 2, 2, 2, 2, 2 };
                }

                return null;
            }

        }

        [TestMethod]
        public void ReflectionTyped()
        {
            // Create a library of helper function
            var lib = new MyLibraryTyped();

            // Create a context that uses the library
            var ctx = new ReflectionContext(lib);

            // Test
            var result = Parser.ParseSingle("S(\"S1\") + S(\"S2\")").Eval(ctx);
            Assert.IsInstanceOfType(result, typeof(double[]));
            Assert.IsTrue(((double[])result).SequenceEqual(new double[] { 3, 4, 5, 6, 7 }));
        }

        [TestMethod]
        public void OptimizerTest()
        {
            var lib = new MyLibrary();
            var ctx = new ReflectionContext(lib);

            var node = TreeOptimizer.Optimize(Parser.ParseSingle("5 + 5"));
            Assert.IsTrue(node is NodeObject nodeObject && Math.Abs((double)nodeObject.Eval(null) - 10.0) < double.Epsilon);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("5 + 5 * 10 + 1 - 2 / 2"));
            Assert.IsTrue(node is NodeObject o && Math.Abs((double)o.Eval(null) - 55.0) < double.Epsilon);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("5 + pi"));
            Assert.IsTrue(node is NodeBinaryOperation);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("[1, 2, 3]"));
            Assert.IsTrue(node is NodeObject nodeObject1 && ((double[])nodeObject1.Eval(null)).SequenceEqual(new [] { 1.0, 2.0, 3.0 }));

            node = TreeOptimizer.Optimize(Parser.ParseSingle("[1, 2, 1 + 2]"));
            Assert.IsTrue(node is NodeObject o1 && ((double[])o1.Eval(null)).SequenceEqual(new [] { 1.0, 2.0, 3.0 }));

            node = TreeOptimizer.Optimize(Parser.ParseSingle("[1, 2, 1 + 2, pi]"));
            Assert.IsTrue(node is NodeObjectArray);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("[1, 2, 3] + [2, 3, 4]"));
            Assert.IsTrue(node is NodeObject nodeObject2 && ((double[])nodeObject2.Eval(null)).SequenceEqual(new [] { 3.0, 5.0, 7.0 }));

            node = TreeOptimizer.Optimize(Parser.ParseSingle("call([1, 2, 3] + [2, 3, 4], 1 + 1)"));
            Assert.IsTrue(node is NodeFunctionCall nodeObject3 && nodeObject3.FunctionName == "call" && nodeObject3.Arguments[0] is NodeObject && nodeObject3.Arguments[1] is NodeObject);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("rectArea(1, 'asd')"), ctx);
            Assert.IsTrue(node is NodeObject nodeObject4 && (string)nodeObject4.Eval(null) == "makes no sense");

            node = TreeOptimizer.Optimize(Parser.ParseSingle("rectArea(1, 2)"), ctx);
            Assert.IsTrue(node is NodeFunctionCall nodeObject5 && nodeObject5.FunctionName == "rectArea");

            node = TreeOptimizer.Optimize(Parser.ParseSingle("rectArea(1, 2)"));
            Assert.IsTrue(node is NodeFunctionCall nodeObject8 && nodeObject8.FunctionName == "rectArea");

            node = TreeOptimizer.Optimize(Parser.ParseSingle("pi"), ctx);
            Assert.IsTrue(node is NodeObject nodeObject6 && Math.Abs((double)nodeObject6.Eval(null) - Math.PI) < double.Epsilon);

            node = TreeOptimizer.Optimize(Parser.ParseSingle("r"), ctx);
            Assert.IsTrue(node is NodeVariable nodeObject7 && nodeObject7.VariableName == "r");

            node = TreeOptimizer.Optimize(Parser.ParseSingle("r"));
            Assert.IsTrue(node is NodeVariable nodeObject9 && nodeObject9.VariableName == "r");
        }
    }
}