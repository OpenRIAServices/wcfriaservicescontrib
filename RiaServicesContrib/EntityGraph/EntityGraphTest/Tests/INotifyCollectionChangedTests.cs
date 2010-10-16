using System.Linq;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RIA.EntityGraph;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class INotifyCollectionChangedTests : EntityGraphTest
    {
        [TestMethod]
        public void ICollectionChangedTest() {
            bool collectionChangedHandlerVisited = false;
            EntityGraph<A> gr = a.EntityGraph();
            gr.CollectionChanged += (sender, args) =>
            {
                collectionChangedHandlerVisited = true;
            };
            a.BSet.Add(new B());
            Assert.IsTrue(collectionChangedHandlerVisited, "CollectionChanged handler not called");
        }
        [TestMethod]
        public void AddEntityToGraphTest() {
            EntityGraph<A> gr = a.EntityGraph();
            var b = new B();
            a.BSet.Add(b);
            Assert.IsTrue(gr.Contains(b), "Entity graph does not contain entity b");
        }
        [TestMethod]
        public void RemoveEntityFromGraphTest() {
            var b = new B();
            a.BSet.Add(b);
            EntityGraph<A> gr = a.EntityGraph();
            a.BSet.Remove(b);
            Assert.IsFalse(gr.Contains(b), "Entity graph still contains entity b");
        }
    }
}
