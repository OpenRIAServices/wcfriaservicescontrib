using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;


namespace EntityGraphTests.Tests
{
    [TestClass]
    public class FullEntityGraphShapeTests : EntityGraphTest
    {
        public class A
        {
            [Association("ab", null, null)]
            public B B { get; set; }

            public int Foo { get; set; }
        }
        public class B
        {
            public int Bar { get; set; }
            [Association("cs", null, null)]
            public List<C> Cs { get; set; }
            [Association("a", null, null)]
            public A A { get; set; }
        }
        public class C
        {
            [Association("a", null, null)]
            public A A { get; set; }
        }
        [TestMethod]
        public void FullEntityGraphShapeTestSingleAssociations()
        {
            A a = new A { B = new B() };

            var shape = new FullEntityGraphShape();

            var outedges = shape.OutEdges(a);
            Assert.IsTrue(outedges.Count() == 1);
            Assert.IsTrue(outedges.Single() == typeof(A).GetProperty("B"));

            var graph = new EntityGraph<object>(a, shape);
            Assert.IsTrue(graph.Count() == 2);
        }
        [TestMethod]
        public void FullEntityGraphShapeTestMultiAssociations()
        {
            B b = new B();
            var shape = new FullEntityGraphShape();
            var outedges = shape.OutEdges(b);
            Assert.IsTrue(outedges.Count() == 2);
        }
        [TestMethod]
        public void FullEntityGraphShapeTestCollectionAssociations()
        {
            A a = new A { B = new B() };
            a.B.Cs = new List<C>() { new C() };

            var shape = new FullEntityGraphShape();

            var outedges = shape.OutEdges(a.B);
            Assert.IsTrue(outedges.Count() == 2);

            var collection = outedges.Single(x => x.Name == "Cs");

            Assert.IsTrue(collection == typeof(B).GetProperty("Cs"));

            var graph = new EntityGraph<object>(a, shape);
            Assert.IsTrue(graph.Count() == 3);
        }
        [TestMethod]
        public void FullEntityGraphShapeTestCycle()
        {
            A a = new A { B = new B() };
            a.B.Cs = new List<C>() { new C { A = a } };

            var shape = new FullEntityGraphShape();

            var graph = new EntityGraph<object>(a, shape);
            Assert.IsTrue(graph.Count() == 3);
        }
    }
}
