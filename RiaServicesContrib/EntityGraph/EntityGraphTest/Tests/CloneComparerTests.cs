using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityGraph;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class CloneComparerTests : EntityGraphTest
    {
        [TestMethod]
        public void IsCloneEqualIdentityTest() {
            var gr = a.EntityGraph();
            Assert.IsTrue(gr.IsCloneEqual(gr));
        }
        [TestMethod]
        public void IsCloneEqualSimpleCloneTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneEqual(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneEqualNamedIdentityTest() {
            var gr = a.EntityGraph("MyGraph");
            Assert.IsTrue(gr.IsCloneEqual(gr));
        }
        [TestMethod]
        public void IsCloneEqualNamedCloneTest() {
            var gr = a.EntityGraph("MyGraph");
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneEqual(cloneOfA.EntityGraph("MyGraph")));
        }
        [TestMethod]
        public void IsCloneEqualEqualShallowCloneTest() {
            var gr = a.EntityGraph("MyGraph");
            a.BNotInGraph = new B();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneEqual(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneEqualCheckModifiedPropertyTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            a.B.name = "test";
            Assert.IsFalse(gr.IsCloneEqual(cloneOfA.EntityGraph()));

            cloneOfA.B.name = "test";
            Assert.IsTrue(gr.IsCloneEqual(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneEqualCheckModifiedCollectionTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            a.BSet.Add(new B());
            Assert.IsFalse(gr.IsCloneEqual(cloneOfA.EntityGraph()));

            // Observe that entities are checked for membe-wise equality, not for binair equality.
            cloneOfA.BSet.Add(new B());
            Assert.IsTrue(gr.IsCloneEqual(cloneOfA.EntityGraph()));
        }
    }
}
