using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntitySetTests : EntityGraphTest
    {
        [TestMethod]
        public void DetachTest() {
            EntityGraph gr = a.EntityGraph();
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            a.BSet.Add(new B());

            ctx.As.Add(a);

            gr.DetachEntityGraph(ctx.As);

            Assert.IsTrue(ctx.EntityContainer.GetChanges().AddedEntities.Count() == 0);
        }

        [TestMethod]
        public void DetachWithNonIncludedNewEntityTest()
        {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            EntityGraph gr = a.EntityGraph();
            ctx.As.Add(a);
            var newB = new B();
            a.BNotInGraph = newB;

            Assert.IsTrue(a.EntityState == EntityState.New);
            Assert.IsTrue(a.BNotInGraph.EntityState == EntityState.New);
            Assert.IsTrue(a.BNotInGraphId == newB.Id);

            gr.DetachEntityGraph(ctx.As);

            Assert.IsTrue(a.EntityState == EntityState.Detached);
            Assert.IsTrue(a.BNotInGraph.EntityState == EntityState.New);
            Assert.IsTrue(a.BNotInGraphId == newB.Id);

        }
        [Asynchronous]
        [TestMethod]
        public void DetachWithNonIncludedExistingEntityTest()
        {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    EntityGraph gr = a.EntityGraph();
                    B existingB = ctx.Bs.Single();
                    ctx.As.Add(a);
                    a.BNotInGraph = existingB;
                    Assert.IsTrue(a.EntityState == EntityState.New);
                    Assert.IsTrue(a.BNotInGraph.EntityState == EntityState.Unmodified);
                    Assert.IsTrue(a.BNotInGraphId == existingB.Id);

                    gr.DetachEntityGraph(ctx.As);

                    Assert.IsTrue(a.EntityState == EntityState.Detached);
                    Assert.IsTrue(a.BNotInGraph.EntityState == EntityState.Unmodified);
                    Assert.IsTrue(a.BNotInGraphId == existingB.Id);
                });
            EnqueueTestComplete();
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
                    EntityGraph gr = a.EntityGraph();
                    B existingB = ctx.Bs.Single();
                    ctx.As.Add(a);
                    a.BSet.Add(new B());
                    a.BSet.Add(existingB);
                    gr.RemoveEntityGraph(ctx.As);

                    var changeSet = ctx.EntityContainer.GetChanges();
                    Assert.IsTrue(changeSet.AddedEntities.Count() == 0, "Added entities should be zero");
                    Assert.IsTrue(changeSet.RemovedEntities.Count() == 2, "Removed entities should be 2");
                });
            EnqueueTestComplete();
        }
    }
}
