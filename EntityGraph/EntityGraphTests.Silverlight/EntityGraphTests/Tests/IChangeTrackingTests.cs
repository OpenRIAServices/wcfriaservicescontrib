using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class IChangeTrackingTests : EntityGraphTest
    {
        [TestMethod]
        public void IsChangedAndAcceptChangesTest()
        {
            EntityGraphTestsDomainContext ctx = new EntityGraphTestsDomainContext();
            ctx.As.Attach(a);
            var graph = new EntityGraph(a, new FullEntityGraphShape());
            Assert.IsFalse(graph.IsChanged);

            a.B.name = "Changed";
            Assert.IsTrue(graph.IsChanged);

            graph.AcceptChanges();
            Assert.IsFalse(graph.IsChanged);
        }
    }
}
