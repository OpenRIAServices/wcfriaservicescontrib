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
    }
}
