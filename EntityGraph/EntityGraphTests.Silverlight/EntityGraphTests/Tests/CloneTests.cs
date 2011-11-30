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
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.Unmodified);
            Assert.IsTrue(clone.Source.EntityState == EntityState.Unmodified);
        }
        [TestMethod]
        public void CloneIntoPreservesEntityStateModifiedTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            a.name = "Hello World";

            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.Modified);
            Assert.IsTrue(clone.Source.EntityState == EntityState.Modified);
        }
        [TestMethod]
        public void CloneIntoPreservesEntityStateNewTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Add(a);
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone(ctxNew);
            Assert.IsTrue(a.EntityState == EntityState.New);
            Assert.IsTrue(clone.Source.EntityState == EntityState.New);
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
                    var gr = existingB.EntityGraph(new FullEntityGraphShape());
                    var clone = gr.Clone(ctxNew);
                    Assert.IsTrue(((B)clone.Source).Id == existingB.Id);
                });
            EnqueueTestComplete();
        }
        [TestMethod]
        public void CloneIntoPreservesAllEntityStates()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Add(a);
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone(ctxNew);
            var result =
                from source in gr
                from cloned in clone
                where source.GetType() == cloned.GetType()
                where source.EntityState == cloned.EntityState
                select cloned;

            Assert.IsTrue(result.Count() == gr.Count());
        }
        [TestMethod]
        public void CloneIntoDoesNotYieldKeyAlreadyExistsException()
        {
            EntityGraphTestsDomainContext context = new EntityGraphTestsDomainContext();
            var a = new A { Id = 1 };
            context.As.Attach(a);
            context.Bs.Attach(new B { Id = 1, AId = 1 });
            context.Bs.Attach(new B { Id = 2, AId = 1 });

            a.name = "Modified Task 1";

            EntityGraphTestsDomainContext tempContext = new EntityGraphTestsDomainContext();

            var shape = new FullEntityGraphShape();
            // Clone should not raise an exception when instances of B are attached to tmpContext
            a.Clone(tempContext, shape);
        }
    }
}
