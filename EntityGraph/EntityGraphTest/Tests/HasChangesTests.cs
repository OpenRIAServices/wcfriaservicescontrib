using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class HasChangesTests : EntityGraphTest
    {
        [Asynchronous]
        [TestMethod]
        public void HasChangesTest() {
            EntityGraphTestDomainContext ctx = new EntityGraphTestDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
                    B existingB = loadOp.Entities.SingleOrDefault();
                    a.BSet.Add(existingB);
                    Assert.IsFalse(gr.HasChanges);

                    existingB.name = "Some Name";
                    Assert.IsTrue(gr.HasChanges);
                });
            EnqueueTestComplete();
        }
    }
}
