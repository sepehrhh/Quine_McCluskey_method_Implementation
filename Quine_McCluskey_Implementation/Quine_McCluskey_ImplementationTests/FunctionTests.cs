using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quine_McCluskey_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quine_McCluskey_Implementation.Tests
{
    [TestClass()]
    public class FunctionTests
    {
        [TestMethod()]
        public void CombineTest()
        {
            var implicant1 = new PrimeImplicant(0, 4);
            var implicant2 = new PrimeImplicant(8, 4);
            var expected = (true, "-000");
            var actual = Function.Combine(implicant1, implicant2);
            Assert.AreEqual(expected, actual);
        }
    }
}