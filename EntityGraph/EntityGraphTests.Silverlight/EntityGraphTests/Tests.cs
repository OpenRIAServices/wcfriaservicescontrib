using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTests
{
    [TestClass]
    public class EntityCollectionTests : SilverlightTest
    {
        
        [Asynchronous]
        [TestMethod]
        public void EntityCollectionWithExistingEntityTest() {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            LoadOperation<B> loadOp = ctx.Load(ctx.GetBSetQuery());
            EnqueueConditional(() => loadOp.IsComplete);
            EnqueueCallback(
                () =>
                {
                    A a = new A();
                    B b = loadOp.Entities.SingleOrDefault();
                    b.ASet.Add(a);
                    Assert.IsTrue(a.B == b);
                    Assert.IsTrue(b.ASet.Count() == 1);
                    Assert.IsTrue(b.ASet.SingleOrDefault() == a);
                    a.BId = null;
                    Assert.IsTrue(a.B == null);
                    Assert.IsTrue(b.ASet.Count() == 0);
                    Assert.IsTrue(b.ASet.SingleOrDefault() == null);
                });
            EnqueueTestComplete();
        }
        [TestMethod]
        public void EntityCollectionWithNewEntityTest() {
            A a = new A();
            B b = new B();
            b.ASet.Add(a);
            Assert.IsTrue(a.B == b);
            Assert.IsTrue(b.ASet.Count() == 1);
            Assert.IsTrue(b.ASet.SingleOrDefault() == a);
            a.BId = null;
            Assert.IsTrue(a.B == null);
            Assert.IsTrue(b.ASet.Count() == 1); // should be 0
            Assert.IsTrue(b.ASet.SingleOrDefault() == a); // should be null
        }
    }
}