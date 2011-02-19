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
        public void CopyEqualStringEdgesTests()
        {
            string[] edges = new string[] { "A.B.C.D.A", "A.BSet"};
            var copy1 = a.Copy(edges);
            var copy2 = a.Copy();

            var gr1 = copy1.EntityGraph();
            var gr2 = copy2.EntityGraph();

            Assert.IsTrue(gr1.IsCopyOf(gr2));
        }

        [TestMethod]
        public void CopyNotEqualStringEdgesTests()
        {
            string[] edges = new string[] { "A.B", "A.DSet", "A.BSet" };
            var copy1 = a.Copy(edges);
            var copy2 = a.Copy();

            var gr1 = copy1.EntityGraph();
            var gr2 = copy2.EntityGraph();

            Assert.IsFalse(gr1.IsCopyOf(gr2));
        }

    }
}
