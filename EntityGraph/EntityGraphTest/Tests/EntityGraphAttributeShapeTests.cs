using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntityGraphAttributeShapeTests
    {
        public class A : Entity
        {
            [EntityGraph]
            public B B { get; set; }
            [EntityGraph("SomeEntityGraphName")]
            public C C { get; set; }
        }
        public class B : Entity
        {
        }
        public class C : Entity
        {
        }
        [TestMethod]
        public void AnymousEntityGraphAttributeShapeTest()
        {

            A a = new A();
            B b = new B();
            C c = new C();
            a.B = b;
            a.C = c;

            var gr = a.EntityGraph(new EntityGraphAttributeShape());
            Assert.IsTrue(gr.Count() == 3);
            Assert.IsTrue(gr.Contains(a));
            Assert.IsTrue(gr.Contains(b));
            Assert.IsTrue(gr.Contains(c));
        }
        [TestMethod]
        public void NamedEntityGraphAttributeShapeTest()
        {

            A a = new A();
            B b = new B();
            C c = new C();
            a.B = b;
            a.C = c;

            var gr = a.EntityGraph(new EntityGraphAttributeShape("SomeEntityGraphName"));
            Assert.IsTrue(gr.Count() == 2);
            Assert.IsTrue(gr.Contains(a));
            Assert.IsFalse(gr.Contains(b));
            Assert.IsTrue(gr.Contains(c));
        }
    }
}
