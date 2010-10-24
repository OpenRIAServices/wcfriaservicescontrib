using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraph.RIA;
using EntityGraphTest.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class EntityChangeSetTests : EntityGraphTest
    {
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
    }
}
