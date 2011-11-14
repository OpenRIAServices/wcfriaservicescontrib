using RiaServicesContrib.DomainServices.Client;
using EntityGraphTests.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraphTests.Tests
{
    [TestClass]
    public class CloneComparerTests : EntityGraphTest
    {
        public override void TestSetup()
        {
            base.TestSetup();
            IDFactory.AutoGenerateKeys = true;
        }
        public override void TestCleanup()
        {
            base.TestCleanup();
            IDFactory.AutoGenerateKeys = false;
        }
        /// <summary>
        /// Checks that an entitygraph can never be a clone of it self
        /// </summary>
        [TestMethod]
        public void IsCloneOfIdentityTest() {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.IsCopyOf(gr));
        }
        [TestMethod]
        public void IsCloneOfNamedIdentityTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphShape1);
            Assert.IsFalse(gr.IsCloneOf(gr));
        }
        [TestMethod]
        public void IsCloneOfSimpleCopyTest() {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var cloneOfA = a.Clone(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.IsCloneOf(cloneOfA.EntityGraph(EntityGraphs.CircularGraphFull)));
        }
        // The primary keys of a copy differ, therefore a copy can never be a clone
        [TestMethod]
        public void CopyIsNotACloneTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var copyOfA = a.Copy(EntityGraphs.CircularGraphFull);
            var copyGr = copyOfA.EntityGraph(EntityGraphs.CircularGraphFull);
            Assert.IsFalse(gr.IsCloneOf(copyGr));
        }
        [TestMethod]
        public void CloneIsACopyTest()
        {
            var gr = a.EntityGraph(EntityGraphs.CircularGraphFull);
            var cloneOfA = a.Clone(EntityGraphs.CircularGraphFull);
            Assert.IsTrue(gr.IsCopyOf(cloneOfA.EntityGraph(EntityGraphs.CircularGraphFull)));
        }
    }
}
