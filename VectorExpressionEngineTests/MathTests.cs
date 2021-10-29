using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleCalculator;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class MathTests
    {
        [TestMethod]
        public void TestFilter()
        {
            var filterResult = MathLibrary.filter(new [] { 1, 0.1 }, new [] { 1.0, 2, 3, 4 });
            var filterTarget = new [] { 1, 2.1, 3.2, 4.3 };
            Assert.IsTrue(filterResult.SequenceEqual(filterTarget));
        }

        [TestMethod]
        public void TestFiltFilt()
        {
            var filterResult = MathLibrary.filtfilt(new [] { 1, 0.1 }, new [] { 1.0, 2, 3, 4 });
            var filterTarget = new [] { 1.21, 2.42, 3.63, 4.84 };
            Assert.IsTrue(filterResult.Zip(filterTarget, (r, t) => Math.Abs(r - t)).All(a => a < 1e-8));
        }
    }
}
