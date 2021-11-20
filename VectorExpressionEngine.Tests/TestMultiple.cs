using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using VectorExpressionEngine;

namespace VectorExpressionEngine.Tests
{
    [TestClass]
    public class TestMultiple
    {
        [TestMethod]
        public void TestMultipleExpressions()
        {
            var basicMath = new BasicOperations();
            var ctx = new ScopedContext(new ReflectionContext(basicMath));

            var nodes = Parser.Parse("a=1;b=2;c=a+b");
            Assert.AreEqual(3, nodes.Length);
            Assert.IsNotNull(nodes[0] is NodeAssignment);
            Assert.IsNotNull(nodes[1] is NodeAssignment);
            Assert.IsNotNull(nodes[2] is NodeAssignment);

            var results = nodes.Select(node => node.Eval(ctx)).ToArray();
            Assert.AreEqual(1.0, results[0]);
            Assert.AreEqual(2.0, results[1]);
            Assert.AreEqual(3.0, results[2]);
        }
    }
}
