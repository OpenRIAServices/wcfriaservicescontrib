using System.Linq;
using EntityGraphTest.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RIA.EntityGraph;

namespace EntityGraphTest.Tests
{
    [TestClass]
    public class CloneTests : EntityGraphTest
    {
        [TestMethod]
        public void CloneTest() {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone();
            Assert.AreEqual(a.EntityGraph().Count(), cloneOfA.EntityGraph().Count(), "Clone of a does not have same number of elements");
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.IsTrue(el.ea.GetType() == el.eb.GetType(), "Clone is not equal");
                if(el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if(el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if(el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if(el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }

        [TestMethod]
        public void CloneNamedGraphTest() {
            a.BSet.Add(new B { name = "B1" });
            A cloneOfA = a.Clone("MyGraph");
            Assert.IsTrue(a.EntityGraph().Count() == cloneOfA.EntityGraph().Count() + 1, "Clone of a does the correct number of elements");
            Assert.IsTrue(cloneOfA.BSet.Count() == 0);
            var zip = a.EntityGraph().Zip(cloneOfA.EntityGraph(), (ea, eb) => new { ea, eb });
            foreach(var el in zip)
            {
                Assert.AreEqual(el.ea.GetType(), el.eb.GetType(), "Clone is not equal");
                if(el.ea is A)
                {
                    Assert.IsTrue(((A)el.ea).name == ((A)el.eb).name);
                }
                if(el.ea is B)
                {
                    Assert.IsTrue(((B)el.ea).name == ((B)el.eb).name);
                }
                if(el.ea is C)
                {
                    Assert.IsTrue(((C)el.ea).name == ((C)el.eb).name);
                }
                if(el.ea is D)
                {
                    Assert.IsTrue(((D)el.ea).name == ((D)el.eb).name);
                }
            }
        }
    }
}
