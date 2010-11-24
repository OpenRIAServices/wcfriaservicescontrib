using System.Linq;

using EntityGraph.RIA;
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

            EntityGraph<A> gr = a.EntityGraph();
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
            EntityGraph<A> gr = a.EntityGraph();
            Assert.IsFalse(gr.Contains(newB));

            a.B = newB;
            Assert.IsTrue(gr.Contains(newB));            
        }
        [TestMethod]
        public void RemoveAssociationTest()
        {
            EntityGraph<A> gr = a.EntityGraph();
            Assert.IsTrue(gr.Contains(b));

            a.B = null; ;
            Assert.IsFalse(gr.Contains(b));
        }
    }
}
