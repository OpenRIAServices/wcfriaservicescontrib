using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityGraph;

namespace EntityGraphTest
{
    [TestClass]
    public class Tests : SilverlightTest
    {
        private A a;
        private B b;
        private C c;
        private D d;
        [TestInitialize]
        public void TestSetup()
        {
            a = new A { name = "A" };
            b = new B { name = "B" };
            c = new C { name = "C" };
            d = new D { name = "D" };

            a.B = b;
            b.C = c;
            c.D = d;
            d.A = a;

        }
        [TestMethod]
        public void CreateEntityGraphTest()
        {
            EntityGraph<A> gr = a.EntityGraph();
            Assert.AreEqual(4, gr.Count(), "Graph contains unexpected number of elements");
        }
        [TestMethod]
        public void INotifyPropertyChangedTest()
        {
            bool propertyChangedHandlerVisited = false;

            EntityGraph<A> gr = a.EntityGraph();
            gr.PropertyChanged += (sender, args) =>
                {
                    propertyChangedHandlerVisited = true;
                };
            d.name = "Hello";
            Assert.IsTrue(propertyChangedHandlerVisited, "PropertyChanged handler not called");
        }

        [TestMethod]
        public void ICollectionChangedTest()
        {
            bool collectionChangedHandlerVisited = false;
            EntityGraph<A> gr = a.EntityGraph();
            gr.CollectionChanged += (sender, args) =>
            {
                collectionChangedHandlerVisited = true;
            };
            a.BSet.Add(new B());
            Assert.IsTrue(collectionChangedHandlerVisited, "CollectionChanged handler not called");
        }
        [TestMethod]
        public void CloneTest()
        {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone();
            Assert.AreEqual(a.EntityGraph().Count(), cloneOfA.EntityGraph().Count(), "Clone of a does not have same number of elements");
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach (var el in zip)
            {
                Assert.IsTrue(el.ea.GetType() == el.eb.GetType(), "Clone is not equal");
                if (el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if (el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if (el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if (el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }

        [TestMethod]
        public void CloneNamedGraphTest()
        {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone("MyGraph");
            Assert.IsTrue(a.EntityGraph().Count() == cloneOfA.EntityGraph().Count() + 1, "Clone of a does the correct number of elements");
            Assert.IsTrue(cloneOfA.BSet.Count() == 0);
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach (var el in zip)
            {
                Assert.AreEqual(el.ea.GetType(), el.eb.GetType(), "Clone is not equal");
                if (el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if (el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if (el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if (el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }

        [Asynchronous]
        [TestMethod]
        public void ChangeSetTest()
        {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B existingB = ctx.Bs.Single();
                    a.BSet.Add(existingB);
                    existingB.name = "Hello";

                    B newB = new B();
                    a.BSet.Add(newB);
                    ctx.As.Add(a);

                    var changeSet = a.EntityGraph().GetChanges();
                    Assert.IsTrue(changeSet.ModifiedEntities.Contains(existingB), "ChangeSet.ModifiedEntities should contain b");
                    Assert.AreEqual(5, changeSet.AddedEntities.Count(), "Incorrect number of added entities");
                    Assert.IsTrue(changeSet.AddedEntities.Contains(b), "ChangeSet.AddedEntities shoudl contain b");
                    Assert.IsTrue(changeSet.AddedEntities.Contains(newB), "ChangeSet.AddedEntities shoudl contain newB");
                    Assert.IsTrue(changeSet.RemovedEntities.Count() == 0, "ChangeSet.RemovedEntities should be 0");
                }
            );
            EnqueueTestComplete();
        }
        [TestMethod]
        public void DetachTest()
        {
            EntityGraph<A> gr = a.EntityGraph();
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            a.BSet.Add(new B());

            ctx.As.Add(a);

            gr.DetachEntityGraph(ctx.As);

            Assert.IsTrue(ctx.EntityContainer.GetChanges().AddedEntities.Count() == 0);
        }
        [Asynchronous]
        [TestMethod]
        public void RemoveTest()
        {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    EntityGraph<A> gr = a.EntityGraph();
                    B existingB = ctx.Bs.Single();
                    a.BSet.Add(new B());
                    a.BSet.Add(existingB);
                    ctx.As.Add(a);

                    gr.RemoveEntityGraph(ctx.As);

                    var changeSet = ctx.EntityContainer.GetChanges();
                    Assert.IsTrue(changeSet.AddedEntities.Count() == 0);
                    Assert.IsTrue(changeSet.RemovedEntities.Count() == 1);
                });
            EnqueueTestComplete();
        }
        [TestMethod]
        public void IEnumerableTest()
        {
            B newB = new B();
            a.BSet.Add(newB);
            EntityGraph<A> eg = a.EntityGraph();
            Assert.AreEqual(a, eg.OfType<A>().Single());
            Assert.AreEqual(c, eg.OfType<C>().Single());
            Assert.AreEqual(d, eg.OfType<D>().Single());

            Assert.IsTrue(eg.OfType<B>().Contains(b));
            Assert.IsTrue(eg.OfType<B>().Contains(newB));

            Assert.IsTrue(eg.Count() == 5);
        }

        [TestMethod]
        public void IEnumerableNamedGraphTest()
        {
            B newB = new B();
            a.BSet.Add(newB);
            EntityGraph<A> eg = a.EntityGraph("MyGraph");
            Assert.AreEqual(a, eg.OfType<A>().Single());
            Assert.AreEqual(b, eg.OfType<B>().Single());
            Assert.AreEqual(c, eg.OfType<C>().Single());
            Assert.AreEqual(d, eg.OfType<D>().Single());

            Assert.IsTrue(eg.Count() == 4);
        }
    }
}