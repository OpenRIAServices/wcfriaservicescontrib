using System.Linq;

using RiaServicesContrib.DomainServices.Client;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class INotifyPropertyChangedTests : EntityGraphTest
    {
        [TestMethod]
        public void INotifyPropertyChangedTest() {
            bool propertyChangedHandlerVisited = false;

            EntityGraph gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            gr.PropertyChanged += (sender, args) =>
            {
                propertyChangedHandlerVisited = true;
            };
            d.name = "Hello";
            Assert.IsTrue(propertyChangedHandlerVisited, "PropertyChanged handler not called");
        }
        [TestMethod]
        public void AddAssociationTest()
        {
            B newB = new B();
            EntityGraph gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.Contains(newB));

            a.B = newB;
            Assert.IsTrue(gr.Contains(newB));            
        }
        [TestMethod]
        public void RemoveAssociationTest()
        {
            EntityGraph gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.Contains(b));

            a.B = null; ;
            Assert.IsFalse(gr.Contains(b));
        }
    }
}
