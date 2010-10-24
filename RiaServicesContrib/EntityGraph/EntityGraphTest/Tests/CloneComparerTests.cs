using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class CloneComparerTests : EntityGraphTest
    {
        /// <summary>
        /// Checks that an entitygraph can never be a clone of it self
        /// </summary>
        [TestMethod]
        public void IsCloneOfIdentityTest() {
            var gr = a.EntityGraph();
            Assert.IsFalse(gr.IsCloneOf(gr));
        }
        [TestMethod]
        public void IsCloneOfNamedIdentityTest()
        {
            var gr = a.EntityGraph("MyGraph");
            Assert.IsFalse(gr.IsCloneOf(gr));
        }
        [TestMethod]
        public void IsCloneOfSimpleCloneTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneOfNamedCloneTest() {
            var gr = a.EntityGraph("MyGraph");
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph("MyGraph")));
        }
        [TestMethod]
        public void IsCloneOfEqualShallowCloneTest() {
            var gr = a.EntityGraph("MyGraph");
            a.BNotInGraph = new B();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneOfCheckModifiedPropertyTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            a.B.name = "test";
            Assert.IsFalse(gr.IsCloneOf(cloneOfA.EntityGraph()));

            cloneOfA.B.name = "test";
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCloneOfCheckModifiedCollectionTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            a.BSet.Add(new B());
            Assert.IsFalse(gr.IsCloneOf(cloneOfA.EntityGraph()));

            // Observe that entities are checked for membe-wise equality, not for binair equality.
            cloneOfA.BSet.Add(new B());
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph()));
        }
    }
}
