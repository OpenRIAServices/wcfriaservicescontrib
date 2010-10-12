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
    }
}
