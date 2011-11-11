using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class HasChangesTests : EntityGraphTest
    {
        [TestMethod]
        public void AddNewEntityMakesHasChangesTrue()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.HasChanges);

            ctx.As.Add(a);

            Assert.IsTrue(gr.HasChanges);
        }
        [Asynchronous]
        [TestMethod]
        public void HasChangesTest() {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
                    Assert.IsFalse(gr.HasChanges);
                    B existingB = loadOp.Entities.SingleOrDefault();
                    a.BSet.Add(existingB);
                    Assert.IsTrue(gr.HasChanges);

                    existingB.name = "Some Name";
                    Assert.IsTrue(gr.HasChanges);
                });
            EnqueueTestComplete();
        }
    }
}
