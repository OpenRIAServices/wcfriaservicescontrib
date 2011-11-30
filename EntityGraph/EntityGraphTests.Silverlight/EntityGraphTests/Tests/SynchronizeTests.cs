using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class SynchronizeTests : EntityGraphTest
    {
        [TestMethod]
        public void SynchronizePreservesEntityStateUnmodifiedTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone();
            ctxNew.As.Attach(clone.Source);
            clone.Synchronize(gr);
            Assert.IsTrue(a.EntityState == EntityState.Unmodified);
            Assert.IsTrue(clone.Source.EntityState == EntityState.Unmodified);
        }
        [TestMethod]
        public void SynchronizePreservesEntityStateModifiedTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            a.name = "Hello World";

            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone();
            ctxNew.As.Attach(clone.Source);
            clone.Synchronize(gr);
            Assert.IsTrue(a.EntityState == EntityState.Modified);
            Assert.IsTrue(clone.Source.EntityState == EntityState.Modified);
        }
        //[TestMethod]
        //public void SynchronizePreservesEntityStateNewTest()
        //{
        //    EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
        //    EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
        //    ctx.As.Add(a);
        //    var gr = new EntityGraph(a, new FullEntityGraphShape());
        //    var clone = gr.Clone();
        //    ctxNew.As.Attach(clone.Source);
        //    clone.Synchronize(gr);
        //    Assert.IsTrue(a.EntityState == EntityState.New);
        //    Assert.IsTrue(clone.Source.EntityState == EntityState.New);
        //}
        [Asynchronous]
        [TestMethod]
        public void SynchronizePreservesPrimaryKeyTest()
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
                    var clone = gr.Clone();
                    ctxNew.Bs.Attach(clone.Source);
                    clone.Synchronize(gr);
                    Assert.IsTrue(((B)clone.Source).Id == existingB.Id);
                });
            EnqueueTestComplete();
        }
        //[TestMethod]
        //public void SynchronizePreservesAllEntityStates()
        //{
        //    EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
        //    EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
        //    ctx.As.Add(a);
        //    var gr = new EntityGraph(a, new FullEntityGraphShape());
        //    var clone = gr.Clone();
        //    ctxNew.As.Attach(clone.Source);
        //    clone.Synchronize(gr);
        //    var result =
        //        from source in gr
        //        from cloned in clone
        //        where source.GetType() == cloned.GetType()
        //        where source.EntityState ==  cloned.EntityState
        //        select cloned;

        //    Assert.IsTrue(result.Count() == gr.Count());
        //}
        [TestMethod]
        public void SynchronizeResetsEntityNewState()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Add(a);
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone();
            ctxNew.As.Attach(clone.Source);
            Assert.IsTrue(a.EntityState == EntityState.New);
            gr.Synchronize(clone);
            Assert.IsTrue(a.EntityState == EntityState.Unmodified);
        }
        [TestMethod]
        public void SynchronizeResetsEntityModifiedState()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            EntityGraphTestsDomainContext ctxNew = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            a.name = "Some name";
            var gr = new EntityGraph(a, new FullEntityGraphShape());
            var clone = gr.Clone();
            ctxNew.As.Attach(clone.Source);
            
            Assert.IsTrue(a.EntityState == EntityState.Modified);
            gr.Synchronize(clone);
            Assert.IsTrue(a.EntityState == EntityState.Unmodified);
        }
    }
}
