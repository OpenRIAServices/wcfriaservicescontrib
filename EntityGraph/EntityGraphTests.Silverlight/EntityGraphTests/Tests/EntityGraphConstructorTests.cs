using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RiaServicesContrib.DomainServices.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiaServicesContrib;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class EntityGraphConstructorTests : EntityGraphTest
    {
        [TestMethod]
        [Description("Tests that passing a null value for the source entity constructor argument generates an exception")]
        public void NullConstructorArgumentThrowsExceptionTest1()
        {
            try
            {
                new EntityGraph(null, new RiaServicesContrib.EntityGraphAttributeShape());
                Assert.Fail();
            }
            catch(ArgumentNullException)
            {
            }            
        }
        [TestMethod]
        [Description("Tests that passing a null value for the graph shape constructor argument generates an exception")]
        public void NullConstructorArgumentThrowsExceptionTest2()
        {
            try
            {
                new EntityGraph(a, default(EntityGraphShape));
                Assert.Fail();
            }
            catch(ArgumentNullException)
            {
            }
        }

    }
}
