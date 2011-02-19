using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class CopyComparerTests : EntityGraphTest
    {
        /// <summary>
        /// Checks that an entitygraph can never be a copy of it self
        /// </summary>
        [TestMethod]
        public void IsCopyOfIdentityTest() {
            var gr = a.EntityGraph();
            Assert.IsFalse(gr.IsCopyOf(gr));
        }
        [TestMethod]
        public void IsCopyOfNamedIdentityTest()
        {
            var gr = a.EntityGraph("MyGraph");
            Assert.IsFalse(gr.IsCopyOf(gr));
        }
        [TestMethod]
        public void IsCopyOfSimpleCopyTest() {
            var gr = a.EntityGraph();
            var copyOfA = a.Copy();
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCopyOfNamedCopyTest() {
            var gr = a.EntityGraph("MyGraph");
            var copyOfA = a.Copy();
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph("MyGraph")));
        }
        [TestMethod]
        public void IsCopyOfEqualShallowCopyTest() {
            var gr = a.EntityGraph("MyGraph");
            a.BNotInGraph = new B();
            var copyOfA = a.Copy();
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCopyOfCheckModifiedPropertyTest() {
            var gr = a.EntityGraph();
            var copyOfA = a.Copy();
            a.B.name = "test";
            Assert.IsFalse(gr.IsCopyOf(copyOfA.EntityGraph()));

            copyOfA.B.name = "test";
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph()));
        }
        [TestMethod]
        public void IsCopyOfCheckModifiedCollectionTest() {
            var gr = a.EntityGraph();
            var copyOfA = a.Copy();
            a.BSet.Add(new B());
            Assert.IsFalse(gr.IsCopyOf(copyOfA.EntityGraph()));

            // Observe that entities are checked for membe-wise equality, not for binair equality.
            copyOfA.BSet.Add(new B());
            Assert.IsTrue(gr.IsCopyOf(copyOfA.EntityGraph()));
        }
    }
}
