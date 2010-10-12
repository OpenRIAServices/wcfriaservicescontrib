using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIA.EntityGraph;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntitySetTests : EntityGraphTest
    {
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
    }
}
