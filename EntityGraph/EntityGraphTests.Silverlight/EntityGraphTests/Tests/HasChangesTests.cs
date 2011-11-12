using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;
using System.ComponentModel;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class HasChangesTests : EntityGraphTest
    {
        [TestMethod]
        public void AddNewEntityMakesHasChangesTrueTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.HasChanges);

            ctx.As.Add(a);

            Assert.IsTrue(gr.HasChanges);
        }
        [TestMethod]
        public void EntityGraphInitializesHasChangesTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            ctx.As.Add(a);

            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.HasChanges);
        }
        [Asynchronous]
        [TestMethod]
        public void INotifyCollectionChangedUpdatesHasChangesTest()
        {
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
                });
            EnqueueTestComplete();

        }
        [Asynchronous]
        [TestMethod]
        public void INotifyPropertyChangeUpdatesHasChangesTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    B existingB = loadOp.Entities.SingleOrDefault();
                    var gr = existingB.EntityGraph(EntityGraphs.CircularGraphFull);
                    Assert.IsFalse(gr.HasChanges);
                    ((IEditableObject)existingB).BeginEdit();
                    existingB.name = "Some Name";
                    Assert.IsTrue(gr.HasChanges);

                    ((IEditableObject)existingB).CancelEdit();
                    Assert.IsFalse(gr.HasChanges);
                });
            EnqueueTestComplete();
        }
    }
}
