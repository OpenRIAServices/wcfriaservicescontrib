using RiaServicesContrib.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class IDisposableTests : EntityGraphTest
    {
        [TestMethod]
        public void IDisposableTest() {
            bool PropertyChangedCalled = false;
            bool CollectionChangedCalled = false;
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.PropertyChanged += (sender, args) =>
            {
                PropertyChangedCalled = true;
            };
            gr.CollectionChanged += (sender, args) =>
            {
                CollectionChangedCalled = true;
            };
            gr.Dispose();
            a.name = "Some Name";
            a.BSet.Add(new B());
            Assert.IsFalse(PropertyChangedCalled, "PropertyChanged event handler is called after Dispose()");
            Assert.IsFalse(CollectionChangedCalled, "CollectionChanged event handler is called after Dispose()");
        }
    }
}
