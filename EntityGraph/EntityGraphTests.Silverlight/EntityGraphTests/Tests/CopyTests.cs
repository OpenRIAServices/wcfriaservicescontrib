using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class CopyTests : EntityGraphTest
    {
        [TestMethod]
        public void CopyTest()
        {
            a.BSet.Add(new B { name = "B1" });
            A copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            Assert.AreEqual(a.EntityGraph(EntityGraphs.CircularGraphFull).Count(), copyOfA.EntityGraph(EntityGraphs.CircularGraphFull).Count(), "Copy of a does not have same number of elements");
            var zip = a.EntityGraph(EntityGraphs.CircularGraphFull).Zip(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.IsTrue(el.ea.GetType() == el.eb.GetType(), "Copy is not equal");
                if(el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if(el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if(el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if(el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }

        [TestMethod]
        public void CopyNamedGraphTest()
        {
            a.BSet.Add(new B { name = "B1" });
            A copyOfA = a.Copy(EntityGraphs.CircularGraphShape1);
            var gr1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var gr2 = copyOfA.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr1.Count() == gr2.Count() + 1, "Copy of a does the correct number of elements");
            Assert.IsTrue(copyOfA.BSet.Count() == 0);
            var zip = a.EntityGraph(EntityGraphs.CircularGraphFull).Zip(copyOfA.EntityGraph(EntityGraphs.CircularGraphFull), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.AreEqual(el.ea.GetType(), el.eb.GetType(), "Copy is not equal");
                if(el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if(el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if(el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if(el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }

        /// <summary>
        /// Test that copying a one-2-many relation copies all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyCopyTest()
        {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShapeFull);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfF.ESet.Count() == 2);
        }
        /// <summary>
        /// Test that copying a "named" one-2-many relation copies all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCopyTest1()
        {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShape1);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShape1);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfF.ESet.Count() == 2);
        }
        /// <summary>
        /// Tests that copying using a non-existing graph name, will yield a graph without the one-2-many associations
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCopyTest2()
        {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var copyOfF = f.Copy(EntityGraphs.SimpleGraphShape3);
            var g1 = f.EntityGraph(EntityGraphs.SimpleGraphShape3);
            var g2 = copyOfF.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfF.ESet.Count() == 0);
        }
        /// <summary>
        /// Tests that copying a many-2-one relation will copy the complete entity graph
        /// </summary>
        [TestMethod]
        public void ManyToOneCopyTest1()
        {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var copyOfE1 = e1.Copy(EntityGraphs.SimpleGraphShapeFull);
            var g1 = e1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfE1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfE1.F.ESet.Count() == 2);
            Assert.IsTrue(copyOfE1.FId != f.Id);
            Assert.IsTrue(copyOfE1.F != f);
        }
        /// <summary>
        /// Tests that an association points to the origional entitiy if it is not part of an entity graph
        /// </summary>
        [TestMethod]
        public void ManyToOneNamedCopyTest1()
        {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var copyOfE1 = e1.Copy(EntityGraphs.SimpleGraphShape3);
            var g1 = e1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfE1.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsFalse(g1.IsCopyOf(g2));

            // Check that F/FId point to the origional (non-copied) entity f
            Assert.IsTrue(copyOfE1.FId == f.Id);
            Assert.IsTrue(copyOfE1.F == f);
        }
        /// <summary>
        /// Test that creates a copy of an m2m relation.
        /// It tests that if the other end in an join table entity is part of the entity graph,
        /// the other end entities will be copied
        /// </summary>
        [TestMethod]
        public void Many2ManyFullCopyTest()
        {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var copyOfg = g.Copy(EntityGraphs.SimpleGraphShapeFull);
            var g1 = g.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfg.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsTrue(g1.IsCopyOf(g2));

            // Check that the m2m association to h is now an association to a copy of h
            var copyOfH = copyOfg.GHSet.First().H;
            Assert.IsTrue(copyOfH != h);
        }
        /// <summary>
        /// Test that creates a copy of an m2m relation by copying the join table objects, but not the
        /// target entities.
        /// It tests that if the other end in an join table entity is _not_ part of the graph,
        /// the other end entities are not copied and the origional values will be used.
        /// </summary>
        [TestMethod]
        public void Many2ManyShallowCopyTest()
        {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var copyOfg = g.Copy(EntityGraphs.SimpleGraphShape2);
            var g1 = g.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfg.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsFalse(g1.IsCopyOf(g2));

            // Check that the m2m association to h still exists, because h is not copied
            var copyOfH = copyOfg.GHSet.First().H;
            Assert.IsTrue(copyOfH == h);
        }
        /// <summary>
        /// Test that creates a copy without copying an m2m relation.
        /// It tests that if the collection property (holding m2m relations) is not part of the entity graph the
        /// m2m relations are not copied . The resulting collection will be empty.
        /// </summary>
        [TestMethod]
        public void Many2ManyNoCopyTest()
        {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var copyOfg = g.Copy(EntityGraphs.SimpleGraphShape3);
            var g1 = g.EntityGraph(EntityGraphs.SimpleGraphShapeFull);
            var g2 = copyOfg.EntityGraph(EntityGraphs.SimpleGraphShapeFull);

            Assert.IsFalse(g1.IsCopyOf(g2));
            Assert.IsTrue(copyOfg.GHSet.Count() == 0);
        }
        /// <summary>
        /// Test to check for a circular entity graph it doesn't matter which element is copied. The 
        /// result will always be an entity graph with the same entities.
        /// </summary>
        [TestMethod]
        public void CircularGraphCopyTest()
        {
            var g1 = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var g2 = b.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(g1.All(n => g2.Contains(n)));
            Assert.IsTrue(g2.All(n => g1.Contains(n)));
        }
        [Asynchronous]
        [TestMethod]
        public void CopyIsDetachedFromContextTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B b = loadOp.Entities.SingleOrDefault();
                    Assert.IsFalse(ctx.HasChanges);
                    var copy = b.Copy(new RiaServicesContrib.EntityGraphShape().Edge<B, A>(B => B.A).Edge<B, C>(B => B.C));
                    Assert.IsFalse(ctx.HasChanges);
                });
            EnqueueTestComplete();
        }
        /// <summary>
        /// The method BuildEntityGraph (in Entitygraph.shared.cs) contains some Silverlight specific code to deal with a bug in 
        /// DomainServices.Client. This test checks that the workaround in RiaServicesContrib.EntityGraph is correct.
        /// </summary>
        [TestMethod]
        public void CopyWithNewEntityTest()
        {
            a.B = new B(); // The DomainServices.Client bug would make copy.B null (only for newly created entities).
            var copy = a.Copy(new EntityGraphShape().Edge<A, D>(A => A.DSet).Edge<A, B>(A => A.BSet));
            Assert.IsNotNull(copy.B);
        }

        [TestMethod]
        public void CopyWithSelfReferenceTest()
        {
            var testClass = new SelfReferenceTestClass();
            testClass.SelfReference = testClass;
            Assert.IsTrue(testClass == testClass.SelfReference);

            var shape = new EntityGraphShape().Edge<SelfReferenceTestClass, SelfReferenceTestClass>(c => c.SelfReference);
            var copy = testClass.Copy(shape);
            Assert.IsTrue(copy == copy.SelfReference);
            Assert.IsTrue(copy.EntityGraph(shape).IsCopyOf(testClass.EntityGraph(shape)));
        }
    }
    public class SelfReferenceTestClass : Entity
    {
        [DataMember]
        public string Name { get; set; }
        public SelfReferenceTestClass SelfReference { get; set; }
    }
}
