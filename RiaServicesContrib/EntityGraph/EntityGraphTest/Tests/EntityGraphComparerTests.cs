using EntityGraph.RIA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntityGraphComparerTests : EntityGraphTest
    {
        [TestMethod]
        public void EntityGraphComparerIdentityTest() {
            var gr = a.EntityGraph();
            Assert.IsTrue(gr.EntityGraphEqual(gr));
        }
        [TestMethod]
        public void EntityGraphComparerCloneTest() {
            var gr1 = a.EntityGraph();
            var gr2 = a.Clone().EntityGraph();
            Assert.IsFalse(gr1.EntityGraphEqual(gr2));
        }
        [TestMethod]
        public void EntityGraphComparerCustomComparerTest() {
            var gr1 = a.EntityGraph();
            var gr2 = a.Clone().EntityGraph();
            Assert.IsTrue(gr1.EntityGraphEqual(gr2, (e1, e2) => e1.GetType() == e2.GetType()));
        }
    }
}