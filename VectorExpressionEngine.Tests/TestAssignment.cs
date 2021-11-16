using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;
using VectorExpressionEngine;

namespace UnitTests
{
    [TestClass]
    public class TestAssignment
    {
        public class EmptyScope { }

        [TestMethod]
        public void TestSimpleAssignment()
        {
            var scope = new EmptyScope();
            var context = new ScopedContext(new ReflectionContext(scope));

            var tree = Parser.ParseSingle("a = 1");
            Assert.IsTrue(tree is NodeAssignment);

            var result = tree.Eval(context);
            Assert.AreEqual(1.0, result);

            var tree2 = Parser.ParseSingle("1 + a");

            var result2 = tree2.Eval(context);
            Assert.AreEqual(2.0, result2);

            ThrowsAssert.Throws<SyntaxException>(() => Parser.ParseSingle("'a' = 1"), "Cannot assign value to non variable node");

        }

        [TestMethod]
        public void TestAssignmentOptimization()
        {
            var scope = new EmptyScope();
            var context = new ReflectionContext(scope);

            var tree = Parser.ParseSingle("a = 1 + 1");
            tree = TreeOptimizer.Optimize(tree, context);

            Assert.IsTrue(tree is NodeAssignment);
            var assignment = (tree as NodeAssignment).Assignment as NodeObject;
            Assert.IsNotNull(assignment);
            Assert.AreEqual(2.0, assignment.Value);
        }
    }
}
