using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class CloneTests : EntityGraphTest
    {
        [TestMethod]
        public void CloneIntoPreservesEntityStateUnmodifiedTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            var gr = new EntityGraph(a, EntityGraphs.CircularGraphFull);
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.Unmodified);
            Assert.IsTrue(clone.EntityState == EntityState.Unmodified);
        }
        [TestMethod]
        public void CloneIntoPreservesEntityStateModifiedTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext(); 
            ctx.As.Attach(a);
            a.name = "Hello World";

            var gr = new EntityGraph(a, EntityGraphs.CircularGraphFull);
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.Modified);
            Assert.IsTrue(clone.EntityState == EntityState.Modified);            
        }
        [TestMethod]
        public void CloneIntoPreservesEntityStateNewTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext(); 
            ctx.As.Add(a);
            var gr = new EntityGraph(a, EntityGraphs.CircularGraphFull);
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.New);
            Assert.IsTrue(clone.EntityState == EntityState.New);
        }
        [Asynchronous]
        [TestMethod]
        public void CloneIntoPreservesPrimaryKeyTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
                    B existingB = loadOp.Entities.SingleOrDefault();
                    var gr = existingB.EntityGraph(EntityGraphs.CircularGraphFull);
                    var clone = gr.Clone(ctxNew);
                    Assert.IsTrue(((B)clone).Id == existingB.Id);
                });
            EnqueueTestComplete();
        }
        [TestMethod]
        public void CloneIntoPreservesAllEntityStates()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Add(a);
            var gr = new EntityGraph(a, EntityGraphs.CircularGraphFull);
            var clone = gr.Clone(ctxNew);
            var result =
                from source in gr
                from cloned in new EntityGraph(clone, EntityGraphs.CircularGraphFull)
                where source.GetType() == cloned.GetType()
                where source.EntityState == cloned.EntityState
                select cloned;

            Assert.IsTrue(result.Count() == gr.Count());
        }
    }
}
