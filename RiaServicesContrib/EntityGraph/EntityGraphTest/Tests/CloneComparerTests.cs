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
            Assert.IsFalse(gr.IsCopyOf(gr));
        }
        [TestMethod]
        public void IsCloneOfNamedIdentityTest()
        {
            var gr = a.EntityGraph("MyGraph");
            Assert.IsFalse(gr.IsCloneOf(gr));
        }
        [TestMethod]
        public void IsCloneOfSimpleCopyTest() {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph()));
        }
        [TestMethod]
        public void CopyIsNotACloneTest()
        {
            var gr = a.EntityGraph();
            var copyOfA = a.Copy();
            Assert.IsFalse(gr.IsCloneOf(copyOfA.EntityGraph()));
        }
        [TestMethod]
        public void CloneIsACopyTest()
        {
            var gr = a.EntityGraph();
            var cloneOfA = a.Clone();
            Assert.IsTrue(gr.IsCopyOf(cloneOfA.EntityGraph()));
        }
    }
}
