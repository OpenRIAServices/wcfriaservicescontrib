using System.ComponentModel;
using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib.DomainServices.Client;
using Microsoft.Silverlight.Testing;
using System.Linq;
namespace EntityGraphTests.Tests
{
    [TestClass]
    public class PartialSaveTests : EntityGraphTest
    {
        [TestMethod]
        public void PartialSaveTest()
        {
            EntityGraphTestsDomainContext context = new EntityGraphTestsDomainContext();
            var a = new A { Id = 1 };
            var b = new B { Id = 1 };
            context.As.Attach(a);
            context.Bs.Attach(b);
            context.Bs.Attach(new B { Id = 2, AId = 1 });
            context.Bs.Attach(new B { Id = 3, AId = 1 });

            a.name = "Modified Task 1";

            EntityGraphTestsDomainContext tempContext = new EntityGraphTestsDomainContext();

            var shape = new FullEntityGraphShape();

            var graph = new EntityGraph(a, shape);
            Assert.IsTrue(context.HasChanges);
            Assert.IsTrue(graph.HasChanges);
            Assert.IsTrue(graph.IsChanged);

            var clone = graph.Clone(tempContext);
            var cloneGraph = new EntityGraph(clone, shape);

            Assert.IsTrue(cloneGraph.HasChanges);
            Assert.IsTrue(((IChangeTracking)clone).IsChanged);
            Assert.IsTrue(tempContext.HasChanges);
            
            cloneGraph.AcceptChanges();
            graph.Synchronize(cloneGraph);

            Assert.IsFalse(context.HasChanges);
            Assert.IsFalse(graph.HasChanges);
            Assert.IsFalse(graph.IsChanged);

            Assert.IsFalse(cloneGraph.HasChanges);
            Assert.IsFalse(((IChangeTracking)clone).IsChanged);
            Assert.IsFalse(tempContext.HasChanges);
        }
    }
}
