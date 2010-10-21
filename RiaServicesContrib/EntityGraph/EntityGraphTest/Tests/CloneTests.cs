using System.Linq;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RIA.EntityGraph;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class CloneTests : EntityGraphTest
    {
        [TestMethod]
        public void CloneTest() {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone();
            Assert.AreEqual(a.EntityGraph().Count(), cloneOfA.EntityGraph().Count(), "Clone of a does not have same number of elements");
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.IsTrue(el.ea.GetType() == el.eb.GetType(), "Clone is not equal");
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
        public void CloneNamedGraphTest() {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone("MyGraph");
            Assert.IsTrue(a.EntityGraph().Count() == cloneOfA.EntityGraph().Count() + 1, "Clone of a does the correct number of elements");
            Assert.IsTrue(cloneOfA.BSet.Count() == 0);
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.AreEqual(el.ea.GetType(), el.eb.GetType(), "Clone is not equal");
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
        /// Test that cloning a one-2-many relation clones all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyCloneTest() {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var cloneOfF = f.Clone();
            var g1 = f.EntityGraph();
            var g2 = cloneOfF.EntityGraph();

            Assert.IsTrue(g1.IsCloneEqual(g2));
            Assert.IsTrue(cloneOfF.ESet.Count() == 2);
        }
        /// <summary>
        /// Test that cloning a "named" one-2-many relation clones all members of the entity graph
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCloneTest1() {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var cloneOfF = f.Clone("MyGraph");
            var g1 = f.EntityGraph("MyGraph");
            var g2 = cloneOfF.EntityGraph();

            Assert.IsTrue(g1.IsCloneEqual(g2));
            Assert.IsTrue(cloneOfF.ESet.Count() == 2);
        }
        /// <summary>
        /// Tests that cloning using a non-existing graph name, will yield a graph without the one-2-many associations
        /// </summary>
        [TestMethod]
        public void OneToManyNamedCloneTest2() {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var cloneOfF = f.Clone("MyGraph2");
            var g1 = f.EntityGraph("MyGraph2");
            var g2 = cloneOfF.EntityGraph();

            Assert.IsTrue(g1.IsCloneEqual(g2));
            Assert.IsTrue(cloneOfF.ESet.Count() == 0);
        }
        /// <summary>
        /// Tests that cloning a many-2-one relation will clone the complete entity graph
        /// </summary>
        [TestMethod]
        public void ManyToOneCloneTest1() {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var cloneOfE1 = e1.Clone();
            var g1 = e1.EntityGraph();
            var g2 = cloneOfE1.EntityGraph();

            Assert.IsTrue(g1.IsCloneEqual(g2));
            Assert.IsTrue(cloneOfE1.F.ESet.Count() == 2);
            Assert.IsTrue(cloneOfE1.FId != f.Id);
            Assert.IsTrue(cloneOfE1.F != f);
        }
        /// <summary>
        /// Tests that an association points to the origional entitiy if it is not part of an entity graph
        /// </summary>
        [TestMethod]
        public void ManyToOneNamedCloneTest1() {
            F f = new F();
            E e1 = new E { F = f };
            E e2 = new E { F = f };

            var cloneOfE1 = e1.Clone("MyGraph2");
            var g1 = e1.EntityGraph();
            var g2 = cloneOfE1.EntityGraph();

            Assert.IsFalse(g1.IsCloneEqual(g2));

            // Check that F/FId point to the origional (non-cloned) entity f
            Assert.IsTrue(cloneOfE1.FId == f.Id);
            Assert.IsTrue(cloneOfE1.F == f); 
        }
        /// <summary>
        /// Test that creates a full clone of an m2m relation.
        /// It tests that if the other end in an join table entity is part of the entity graph,
        /// the other end entities will be cloned
        /// </summary>
        [TestMethod]
        public void Many2ManyFullCloneTest() {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var cloneOfg = g.Clone();
            var g1 = g.EntityGraph();
            var g2 = cloneOfg.EntityGraph();

            Assert.IsTrue(g1.IsCloneEqual(g2));

            // Check that the m2m association to h is now an association to a clone of h
            var cloneOfH = cloneOfg.GHSet.First().H;
            Assert.IsTrue(cloneOfH != h);
        }
        /// <summary>
        /// Test that creates a clone of an m2m relation by cloning the join table objects, but not the
        /// target entities.
        /// It tests that if the other end in an join table entity is _not_ part of the graph,
        /// the other end entities are be cloned and the origional values will be used.
        /// </summary>
        [TestMethod]
        public void Many2ManyShallowCloneTest() {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var cloneOfg = g.Clone("MyGraph1");
            var g1 = g.EntityGraph();
            var g2 = cloneOfg.EntityGraph();

            Assert.IsFalse(g1.IsCloneEqual(g2));

            // Check that the m2m association to h still exists, because h is not cloned
            var cloneOfH = cloneOfg.GHSet.First().H;
            Assert.IsTrue(cloneOfH == h);            
        }
        /// <summary>
        /// Test that creates a clone without cloning an m2m relation.
        /// It tests that if the collection property (holding m2m relations) is not part of the entity graph the
        /// m2m relations are not cloned. The resulting collection will be empty.
        /// </summary>
        [TestMethod]
        public void Many2ManyNoCloneTest() {
            G g = new G();
            H h = new H();
            GH gh = new GH { G = g, H = h };

            var cloneOfg = g.Clone("MyGraph2");
            var g1 = g.EntityGraph();
            var g2 = cloneOfg.EntityGraph();

            Assert.IsFalse(g1.IsCloneEqual(g2));
            Assert.IsTrue(cloneOfg.GHSet.Count() == 0);
        }
        /// <summary>
        /// Test to check for a circular entity graph it doesn't matter which element is cloned. The 
        /// result will always be an entity graph with the same entities.
        /// </summary>
        [TestMethod]
        public void CircularGraphCloneTest() {
            var g1 = a.EntityGraph();
            var g2 = b.EntityGraph();
            Assert.IsTrue(g1.All(n => g2.Contains(n)));
            Assert.IsTrue(g2.All(n => g1.Contains(n)));
        }
    }
}
