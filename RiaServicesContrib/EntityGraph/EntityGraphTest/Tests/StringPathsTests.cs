using System.Linq;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class StringEdgesTests : EntityGraphTest
    {
        [TestMethod]
        public void SimpleStringEdgesTests()
        {
            var newB = new B { name = "NewB" };
            a.BSet.Add(newB);
            var gr = a.EntityGraph(new string[] { "A.B", "A.BSet" });

            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(newB));
        }
        [TestMethod]
        public void CloneEqualStringEdgesTests()
        {
            string[] edges = new string[] { "A.B.C.D.A", "A.BSet"};
            var clone1 = a.Clone(edges);
            var clone2 = a.Clone();

            var gr1 = clone1.EntityGraph();
            var gr2 = clone2.EntityGraph();

            Assert.IsTrue(gr1.IsCloneOf(gr2));
        }

        [TestMethod]
        public void CloneNotEqualStringEdgesTests()
        {
            string[] edges = new string[] { "A.B", "A.DSet", "A.BSet" };
            var clone1 = a.Clone(edges);
            var clone2 = a.Clone();

            var gr1 = clone1.EntityGraph();
            var gr2 = clone2.EntityGraph();

            Assert.IsFalse(gr1.IsCloneOf(gr2));
        }

    }
}
