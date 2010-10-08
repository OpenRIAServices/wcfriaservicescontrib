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
        public void TestSetup() {
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
        public void CreateEntityGraphTest() {
            EntityGraph<A> gr = a.EntityGraph();
            Assert.AreEqual(4, gr.Count(), "Graph contains unexpected number of elements");
        }
        [TestMethod]
        public void INotifyPropertyChangedTest() {
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
        public void ICollectionChangedTest() {
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

        [Asynchronous]
        [TestMethod]
        public void ChangeSetTest() {
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
        public void DetachTest() {
            EntityGraph<A> gr = a.EntityGraph();
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            a.BSet.Add(new B());

            ctx.As.Add(a);

            gr.DetachEntityGraph(ctx.As);

            Assert.IsTrue(ctx.EntityContainer.GetChanges().AddedEntities.Count() == 0);
        }
        [Asynchronous]
        [TestMethod]
        public void RemoveTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    EntityGraph<A> gr = a.EntityGraph();
                    B existingB = ctx.Bs.Single();
                    ctx.As.Add(a);
                    a.BSet.Add(new B());
                    a.BSet.Add(existingB);
                    gr.RemoveEntityGraph(ctx.As);

                    var changeSet = ctx.EntityContainer.GetChanges();
                    Assert.IsTrue(changeSet.AddedEntities.Count() == 0, "Added entities should be zero");
                    Assert.IsTrue(changeSet.RemovedEntities.Count() == 1, "Removed entities should be 1");
                });
            EnqueueTestComplete();
        }

        [TestMethod]
        public void IEnumerableTest() {
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
        public void IEnumerableNamedGraphTest() {
            B newB = new B();
            a.BSet.Add(newB);
            EntityGraph<A> eg = a.EntityGraph("MyGraph");
            Assert.AreEqual(a, eg.OfType<A>().Single());
            Assert.AreEqual(b, eg.OfType<B>().Single());
            Assert.AreEqual(c, eg.OfType<C>().Single());
            Assert.AreEqual(d, eg.OfType<D>().Single());

            Assert.IsTrue(eg.Count() == 4);
        }
        [TestMethod]
        public void IEditableObjectScalarPropertyTest() {
            string oldNameForA = a.name;
            string oldNameForB = b.name;
            string oldNameForC = c.name;
            string oldNameForD = d.name;

            a.EntityGraph().BeginEdit();
            a.name = "NewNameForA";
            b.name = "NewNameForB";
            c.name = "NewNameForC";
            d.name = "NewNameForD";
            a.EntityGraph().CancelEdit();

            Assert.IsTrue(a.name == oldNameForA, "Change of A's name is not correctly canceled");
            Assert.IsTrue(b.name == oldNameForB, "Change of B's name is not correctly canceled");
            Assert.IsTrue(c.name == oldNameForC, "Change of C's name is not correctly canceled");
            Assert.IsTrue(d.name == oldNameForD, "Change of D's name is not correctly canceled");
        }
        [Asynchronous]
        [TestMethod]
        public void IEditableObjectAssociationSetTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B existingB = loadOp.Entities.SingleOrDefault();
                    a.B = existingB;
                    var gr = a.EntityGraph();

                    gr.BeginEdit();
                    a.BSet.Add(existingB);
                    // Adding a new entity will fail, because this will not correctly raise the proper collection changed events
                    // I consider this a RIA bug.
                    Assert.IsTrue(a.BSet.Count() == 1, "a.BSet has incorrect number of elements");
                    gr.CancelEdit();
                    Assert.IsTrue(a.BSet.Count() == 0, "a.BSet has incorrect number of elements");
                });
            EnqueueTestComplete();
        }
        [Asynchronous]
        [TestMethod]
        public void IEditableObjectAssociationTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    ctx.As.Add(a);
                    B existingB = loadOp.Entities.SingleOrDefault();
                    B newB = new B { Id = -1 };
                    var gr = a.EntityGraph();

                    a.B = existingB;
                    gr.BeginEdit();
                    a.B = newB;
                    gr.CancelEdit();
                    Assert.IsTrue(a.BId == existingB.Id); // Succeeds 
                    Assert.IsTrue(a.B == existingB); // Succeeds

                    a.B = newB;
                    gr.BeginEdit();
                    a.B = existingB;
                    gr.CancelEdit();

                    Assert.IsTrue(a.BId == newB.Id); // Succeeds 
                    Assert.IsTrue(a.B != newB); // Potential RIA bug, (should be equal to newB)
                    Assert.IsTrue(a.B == null); // Potential RIA bug, (should be equal to newB)
                });
            EnqueueTestComplete();
        }
    }
}