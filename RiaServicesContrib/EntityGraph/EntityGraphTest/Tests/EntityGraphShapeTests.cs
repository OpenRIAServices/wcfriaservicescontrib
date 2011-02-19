using System.ServiceModel.DomainServices.Client;
using EntityGraph;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using EntityGraph.RIA;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntityGraphShapeTests : EntityGraphTest
    {
        [TestMethod]
        public void MyTestMethod()
        {
        }

        [TestMethod]
        public void SimpleGraphShapeTests()
        {
            var newB = new B { name = "NewB" };
            a.BSet.Add(newB);
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<A, B>(A => A.BSet);

            var gr = a.EntityGraph(shape);

            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(newB));
        }
        [TestMethod]
        public void CopyEqualGraphShapeTests()
        {
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<B,C>(B => B.C)
            .Edge<C,D>(C => C.D)
            .Edge<A, B>(A => A.BSet);

            var copy1 = a.Copy(shape);
            var copy2 = a.Copy();

            var gr1 = copy1.EntityGraph();
            var gr2 = copy2.EntityGraph();

            Assert.IsTrue(gr1.IsCopyOf(gr2));
        }

        [TestMethod]
        public void CopyNotEqualGraphShapeTests()
        {
            var shape = new EntityGraphShape()
            .Edge<A,B>(A => A.B)
            .Edge<A, D>(A => A.DSet)
            .Edge<A, B>(A => A.BSet);

            var copy1 = a.Copy(shape);
            var copy2 = a.Copy();

            var gr1 = copy1.EntityGraph();
            var gr2 = copy2.EntityGraph();

            Assert.IsFalse(gr1.IsCopyOf(gr2));
        }
    }
}
