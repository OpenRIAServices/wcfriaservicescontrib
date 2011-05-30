using RiaServicesContrib.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class EntityGraphComparerTests : EntityGraphTest
    {
        [TestMethod]
        public void EntityGraphComparerIdentityTest() {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.EntityGraphEqual(gr));
        }
        [TestMethod]
        public void EntityGraphComparerCopyTest() {
            var gr1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = a.Copy(EntityGraphs.CircularGraphFull).EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr1.EntityGraphEqual(gr2));
        }
        [TestMethod]
        public void EntityGraphComparerCustomComparerTest() {
            var gr1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = a.Copy(EntityGraphs.CircularGraphFull).EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr1.EntityGraphEqual(gr2, (e1, e2) => e1.GetType() == e2.GetType()));
        }
    }
}