using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class EntityChangeSetTests : EntityGraphTest
    {
        [TestMethod]
        public void ChangeSetIsEmptyTest()
        {
            var cs = new EntityGraphChangeSet();
            Assert.IsTrue(cs.IsEmpty);

            cs.RemovedEntities = new ReadOnlyCollection<Entity>(new List<Entity>());
            Assert.IsTrue(cs.IsEmpty);

            cs.AddedEntities = new ReadOnlyCollection<Entity>(new List<Entity> { a });
            Assert.IsFalse(cs.IsEmpty);
        }
        [Asynchronous]
        [TestMethod]
        public void ChangeSetTest() {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
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

                    var changeSet = a.EntityGraph(EntityGraphs.CircularGraphFull).GetChanges();
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
